using NBitcoin;
using System;

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

        public static PSBT BuildTx(Network network, byte[] messageBytes, string fundingTxID, uint vout, int amountSats, BitcoinAddress changeAddress, FeeRate fee)
        {
            Script opReturnScript = TxNullDataTemplate.Instance.GenerateScriptPubKey(messageBytes);
            TxOut opReturnOutput = new(Money.Zero, opReturnScript);

            Money inputAmount = new Money(amountSats, MoneyUnit.Satoshi);
            OutPoint outpointOfFund = new OutPoint(uint256.Parse(fundingTxID), vout);

            TxIn txIn = new TxIn(outpointOfFund);

            Money change = CalcChangeForSelfSpend(network, outpointOfFund, inputAmount, changeAddress.ScriptPubKey, opReturnScript, fee);
            TxOut changeOutput = new TxOut(change, changeAddress);

            var tx = network.CreateTransaction();
            tx.Inputs.Add(txIn);
            tx.Outputs.Add(changeOutput);
            tx.Outputs.Add(opReturnOutput);          

            return PSBT.FromTransaction(tx, network);
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

            // First pass: fee with change output present
            int vsize = tx.GetVirtualSize();
            Money fee = feeRate.GetFee(vsize);

            Money change = inputAmount - fee; // only change bears value in this layout
            var changeOut = new TxOut(change, changeScriptP2WPKH);

            // If change would be dust at this feerate, drop it and recompute fee without change
            if (changeOut.IsDust())
            {
                tx.Outputs.RemoveAt(0); // drop change
                vsize = tx.GetVirtualSize();
                fee = feeRate.GetFee(vsize);
                change = Money.Zero;
            }

            return change;
        }
    }
}
