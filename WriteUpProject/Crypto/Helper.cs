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

        public static PSBT BuildTx(Network network, byte[] messageBytes, string fundingTxID, uint vout, int amountSats, BitcoinAddress changeAddress, int fee)
        {
            Script opReturnScript = TxNullDataTemplate.Instance.GenerateScriptPubKey(messageBytes);
            TxOut opReturnOutput = new(Money.Zero, opReturnScript);

            Money inputAmount = new Money(amountSats, MoneyUnit.Satoshi);
            OutPoint outpointOfFund = new OutPoint(uint256.Parse(fundingTxID), vout);

            TxIn txIn = new TxIn(outpointOfFund);

            Money txFee = new Money(fee, MoneyUnit.Satoshi);
            Money changeAmount = inputAmount - txFee;
            TxOut changeOutput = new TxOut(changeAmount, changeAddress);

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
    }
}
