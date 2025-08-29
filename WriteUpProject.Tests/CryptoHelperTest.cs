using NBitcoin;
using System.Text;
using WriteUpProject.Crypto;

namespace WriteUpProject.Tests
{
    public class CryptoHelperTest
    {
        public Network Network = Network.TestNet4;
        public const string CustomMessage = "Test Message!";
        [Fact]
        public void CanBuildAndSignTX()
        {
            var messageBytes = Encoding.UTF8.GetBytes(CustomMessage);

            var secret = new BitcoinSecret("cQdZ54D6fwYbr8GvTRwzamaAR1TPZwWXDhaKRHW9zAZL2UBGvDpr", Network);
            var extPubKey = new BitcoinExtPubKey("tpubD9CP652nLmoybLkXNvqtbzdfr53NR7mRoTEJC5YDrseottTiPhA8d4TERCxsJW1A5ugNZ4CtJkyz6biJioRBDvbwGYzCsu7pfiuJcsW8erJ", Network);

            var fundScript = secret.PubKey.WitHash.ScriptPubKey;
            var changeAddress = extPubKey.Derive(1).ExtPubKey.PubKey.GetAddress(ScriptPubKeyType.Segwit, Network);

            var txIdOfFunding = "4df78304f2e84df58f7ffa58a4d8ae180a2bc2f42619986a996330c2ead149bc";
            uint vout = 0;
            var amountOfFundsInSats = 500000;
            int txFeeForCustomTX = 1000;

            // Test BuildTx
            var psbt = Helper.BuildTx(Network, messageBytes, txIdOfFunding, vout, amountOfFundsInSats, changeAddress, txFeeForCustomTX);

            // Add coin so we can sign the psbt
            var coin = new Coin(new OutPoint(uint256.Parse(txIdOfFunding), vout), new TxOut(new Money(amountOfFundsInSats, MoneyUnit.Satoshi), fundScript));
            psbt.AddCoins(coin);
            psbt.SignWithKeys(secret);

            Assert.Single(psbt.Inputs[0].PartialSigs);
            Assert.Equal(2, psbt.GetGlobalTransaction().Outputs.Count);

            psbt.Finalize();
        }

        [Fact]
        public void ReadMessageOutOfTx()
        {
            var tx = Transaction.Parse("0100000001bc49d1eac23063996a981926f4c22b0a18aed8a458fa7f8ff54de8f20483f74d0000000000ffffffff02389d070000000000160014b47d3748ba615d0e3d579a7b54153fa51a5eb7d000000000000000000f6a0d54657374204d6573736167652100000000", Network);
            var opReturnOutput = tx.Outputs[1];

            var script = opReturnOutput.ScriptPubKey.ExtractScriptCode(-1);
            var parts = script.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string hex = parts[1];

            // Convert hex -> byte[]
            byte[] bytes = Enumerable.Range(0, hex.Length / 2)
                .Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16))
                .ToArray();

            // Decode to string
            string message = Encoding.UTF8.GetString(bytes);

            Assert.Equal(CustomMessage, message);
        }
    }
}