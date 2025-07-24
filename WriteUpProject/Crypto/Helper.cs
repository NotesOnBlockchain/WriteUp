using NBitcoin;
using System;
using System.Net;

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

        public static string BuildTx(Network network, byte[] messageBytes, string fundingTxID, uint vout, int amountSats,BitcoinAddress fundAddress, BitcoinAddress changeAddress, int fee)
        {
            Script opReturnScript = TxNullDataTemplate.Instance.GenerateScriptPubKey(messageBytes);
            TxOut opReturnOutput = new(Money.Zero, opReturnScript);

            Money inputAmount = new Money(amountSats, MoneyUnit.Satoshi);
            OutPoint outpointOfFund = new OutPoint(uint256.Parse(fundingTxID), vout);
            TxOut utxo = new TxOut(inputAmount, fundAddress);
            TxIn txIn = new TxIn(outpointOfFund);
            Coin coin = new Coin(outpointOfFund, utxo);

            Money txFee = new Money(fee, MoneyUnit.Satoshi);
            Money changeAmount = inputAmount - txFee;
            TxOut changeOutput = new TxOut(changeAmount, changeAddress);

            // Build TX
            Transaction tx = Transaction.Create(network);
            tx.Inputs.Add(txIn);
            tx.Outputs.Add(opReturnOutput);
            tx.Outputs.Add(changeOutput);
            
            return tx.ToHex();
        }
    }
}
