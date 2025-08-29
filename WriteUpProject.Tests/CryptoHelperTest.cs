using NBitcoin;
using System.Text;
using WriteUpProject.Crypto;

namespace WriteUpProject.Tests
{
    public class CryptoHelperTest
    {
        public const string CustomMessage = "Test Message!";
        [Fact]
        public void CanBuildAndSignTX()
        {
            Network network = Network.TestNet4;
            var messageBytes = Encoding.UTF8.GetBytes(CustomMessage);

            var secret = new BitcoinSecret("cQdZ54D6fwYbr8GvTRwzamaAR1TPZwWXDhaKRHW9zAZL2UBGvDpr", network);
            var extPubKey = new BitcoinExtPubKey("tpubD9CP652nLmoybLkXNvqtbzdfr53NR7mRoTEJC5YDrseottTiPhA8d4TERCxsJW1A5ugNZ4CtJkyz6biJioRBDvbwGYzCsu7pfiuJcsW8erJ", network);

            var fundScript = secret.PubKey.WitHash.ScriptPubKey;
            var changeAddress = extPubKey.Derive(1).ExtPubKey.PubKey.GetAddress(ScriptPubKeyType.Segwit, network);

            var txIdOfFunding = "4df78304f2e84df58f7ffa58a4d8ae180a2bc2f42619986a996330c2ead149bc";
            uint vout = 0;
            var amountOfFundsInSats = 500000;
            int txFeeForCustomTX = 1000;

            // Test BuildTx
            var psbt = Helper.BuildTx(network, messageBytes, txIdOfFunding, vout, amountOfFundsInSats, changeAddress, txFeeForCustomTX);

            // Add coin so we can sign the psbt
            var coin = new Coin(new OutPoint(uint256.Parse(txIdOfFunding), vout), new TxOut(new Money(amountOfFundsInSats, MoneyUnit.Satoshi), fundScript));
            psbt.AddCoins(coin);
            psbt.SignWithKeys(secret);

            Assert.Single(psbt.Inputs[0].PartialSigs);
            Assert.Equal(2, psbt.GetGlobalTransaction().Outputs.Count);

            psbt.Finalize();  
        }
    }
}