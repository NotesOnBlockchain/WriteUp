using NBitcoin;
using NBitcoin.DataEncoders;
using System;
using System.Linq;
using System.Text;
using WriteUpProject.Models;

namespace WriteUpProject.Crypto
{
    public static class Helper
    {
        public static Network[] SupportedNetworks =
        {
            Network.Main,
            Network.TestNet,
            Network.TestNet4,
            Network.RegTest
        };

        public static bool ValidateMessageBytesLength(byte[] messageBytes)
        {
            int maximumAllowedBytesForMessage = 80;

            if (messageBytes.Length > maximumAllowedBytesForMessage) 
            {
                return false;
            }

            return true;
        }

        public static BitcoinAddress GetAddressFromString(string address, Network network) 
        {
            return BitcoinAddress.Create(address, network);
        }

        public static PSBT BuildTx(FundingTxInfo fundingTxInfo, OutputSideTxInfo outputSideTxInfo)
        {
            Network network = fundingTxInfo.Network;
            byte[] messageBytes = Encoding.UTF8.GetBytes(outputSideTxInfo.Message);
            BitcoinAddress changeAddress = GetAddressFromString(outputSideTxInfo.ChangeAddress, network);
            Transaction prevTx = Transaction.Parse(fundingTxInfo.FundingTxHex, network);
            uint vout = uint.Parse(fundingTxInfo.Vout);
            FeeRate feeRate = new FeeRate(decimal.Parse(outputSideTxInfo.FeeRate));

            // Legacy wallet
            if (fundingTxInfo.Xpub is null || fundingTxInfo.DerivationPath is null || fundingTxInfo.Fingerprint is null)
            {
                return BuildTx(network, messageBytes, prevTx, vout, changeAddress, feeRate);
            }

            ExtPubKey xpub = ExtPubKey.Parse(fundingTxInfo.Xpub, network);
            KeyPath derivationPath = new KeyPath(fundingTxInfo.DerivationPath);
            HDFingerprint fp = new HDFingerprint(Encoders.Hex.DecodeData(fundingTxInfo.Fingerprint));


            return BuildTx(network, messageBytes, prevTx, vout, xpub, derivationPath, fp,changeAddress, feeRate);
        }

        public static PSBT BuildTx(Network network, byte[] messageBytes, Transaction prevTx, uint vout, ExtPubKey xpub, KeyPath derivationPath, HDFingerprint fp, BitcoinAddress changeAddress, FeeRate fee)
        {
            Script opReturnScript = TxNullDataTemplate.Instance.GenerateScriptPubKey(messageBytes);
            TxOut opReturnOutput = new(Money.Zero, opReturnScript);

            Money inputAmount = prevTx.Outputs[vout].Value;
            OutPoint outpointOfFund = new OutPoint(prevTx.GetHash(), vout);

            TxIn txIn = new TxIn(outpointOfFund);

            Money change = CalcChangeForSelfSpend(network, outpointOfFund, inputAmount, changeAddress.ScriptPubKey, opReturnScript, fee);
            TxOut changeOutput = new TxOut(change, changeAddress);

            var tx = network.CreateTransaction();
            tx.Inputs.Add(txIn);
            tx.Outputs.Add(changeOutput);
            tx.Outputs.Add(opReturnOutput);

            PSBT psbt = PSBT.FromTransaction(tx, network);

            psbt.Inputs[0].NonWitnessUtxo = prevTx;
            psbt.Inputs[0].HDKeyPaths.Add(xpub.PubKey, new RootedKeyPath(fp, derivationPath));

            return psbt;
        }

        // Legacy wallet support
        public static PSBT BuildTx(Network network, byte[] messageBytes, Transaction prevTx, uint vout, BitcoinAddress changeAddress, FeeRate fee)
        {
            Script opReturnScript = TxNullDataTemplate.Instance.GenerateScriptPubKey(messageBytes);
            TxOut opReturnOutput = new(Money.Zero, opReturnScript);

            Money inputAmount = prevTx.Outputs[vout].Value;
            OutPoint outpointOfFund = new OutPoint(prevTx.GetHash(), vout);

            TxIn txIn = new TxIn(outpointOfFund);

            Money change = CalcChangeForSelfSpend(network, outpointOfFund, inputAmount, changeAddress.ScriptPubKey, opReturnScript, fee);
            TxOut changeOutput = new TxOut(change, changeAddress);

            var tx = network.CreateTransaction();
            tx.Inputs.Add(txIn);
            tx.Outputs.Add(changeOutput);
            tx.Outputs.Add(opReturnOutput);

            PSBT psbt = PSBT.FromTransaction(tx, network);
            return psbt;
        }

        public static bool TryParseAddress(string address, Network network)
        {
            // Too long URIs/Bitcoin address are unsupported.
            if (address.Length > 1000)
            {
                return false;
            }

            // Parse a Bitcoin address (not BIP21 URI string)
            if (!address.StartsWith("bitcoin:", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    Network.Parse<BitcoinAddress>(address, network);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public static Money CalcChangeForSelfSpend(Network network, OutPoint input, Money inputAmount, Script changeScriptP2WPKH, Script opReturnScript, FeeRate feeRate)
        {
            // Build a skeleton tx
            var tx = network.CreateTransaction();
            tx.Version = 2;
            tx.Inputs.Add(new TxIn(input));

            // Outputs: placeholder change + OP_RETURN
            tx.Outputs.Add(new TxOut(Money.Zero, changeScriptP2WPKH)); // will set value after fee calc
            tx.Outputs.Add(new TxOut(Money.Zero, opReturnScript));     // OP_RETURN is always zero

            // Add a P2WPKH dummy witness (sig ~72B, pubkey 33B) so vsize is realistic pre-signing
            tx.Inputs[0].WitScript = new WitScript(new byte[][] { new byte[72], new byte[33] });

            int vsize = tx.GetVirtualSize();
            Money fee = feeRate.GetFee(vsize);

            Money change = inputAmount - fee;
            return change;
        }

        public static Script? GetChangeAddress(Transaction transaction)
        {
            return transaction.Outputs.FirstOrDefault(output => output.Value > Money.Zero)?.ScriptPubKey;
        }

        public static string ExtractMessageFromOutput(TxOut txOut)
        {
            if (txOut.Value > Money.Zero)
            {
                throw new Exception("OP_RETURN output not supposed to have any value.");
            }

            var script = txOut.ScriptPubKey.ExtractScriptCode(-1);
            var parts = script.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string hex = parts[1];

            // Convert hex -> byte[]
            byte[] bytes = Enumerable.Range(0, hex.Length / 2)
                .Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16))
                .ToArray();

            // Decode to string
            string message = Encoding.UTF8.GetString(bytes);
            return message;
        }

        public static int ExtractFee(Transaction createdTx, FundingTxInfo fundingTxInfo)
        {
            Transaction fundingTx = Transaction.Parse(fundingTxInfo.FundingTxHex, fundingTxInfo.Network);
            Money fundingOutputValue = fundingTx.Outputs[int.Parse(fundingTxInfo.Vout)].Value;

            Money customMessageTxOutput = createdTx.Outputs.FirstOrDefault(output => output.Value > Money.Zero)?.Value ?? throw new Exception("Didn't create change output. Aborting.");

            return (int)fundingOutputValue.Satoshi - (int)customMessageTxOutput.Satoshi;
        }
    }
}
