using NBitcoin;
using NBitcoin.DataEncoders;
using System.Net;
using System.Text;
using WriteUpProject.Crypto;
using WriteUpProject.Models;

namespace WriteUpProject.Tests
{
    public class CryptoHelperTest
    {
        public Network Network = Network.TestNet4;
        public const string CustomMessage = "Test Message!";
        [Fact]
        public void CanBuildAndSignTX()
        {
            Mnemonic mnemonic = new Mnemonic("abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon about", Wordlist.English);
            ExtKey masterKey = mnemonic.DeriveExtKey();

            HDFingerprint fingerprint = masterKey.Neuter().PubKey.GetHDFingerPrint();
            KeyPath path = new KeyPath("84'/1'/0'/0/0");

            ExtPubKey xpub = masterKey.Derive(new KeyPath("84'/1'/0'")).Neuter();
            string tpub = xpub.ToString(Network);

            BitcoinAddress changeAddress = xpub.Derive(new KeyPath("0/1")).PubKey.GetAddress(ScriptPubKeyType.Segwit, Network);
            double feeRate = 2.0;
            var fundingTXHex = "02000000000101639956e90940f67c2ba35907f3455fa5ed897bb43acc2c5f0f9386edddf4b7d80000000000fdffffff02b2118594000000001600148aa0a82d2f20d82256145b0a2e771828d7d5b9b3b325050000000000160014d0c4a3ef09e997b6e99e397e518fe3e41a118ca10140849007326d673eaf5ae263312ce79dfcbfef72a1be92be500ce4387bc48e9de224e31d1cb5790160c3f36dbaed74b970741c49d2c142f1c4a5244530a18ec34d10cb0100";
            int vout = 1;
            var fundingPart = new FundingTxInfo(Network, fundingTXHex, vout.ToString(), tpub, path.ToString(), fingerprint.ToString());
            var outputPart = new OutputSideTxInfo(changeAddress.ToString(), feeRate.ToString(), CustomMessage);

            // Build PSBT
            PSBT psbt = Helper.BuildTx(fundingPart, outputPart);

            // Sign and finalize
            ExtKey childExtKey = masterKey.Derive(path);
            Key signingKey = childExtKey.PrivateKey;
            psbt.SignWithKeys(signingKey);
            psbt.Finalize();
        }

        [Fact]
        public void ReadMessageOutOfTx()
        {
            var tx = Transaction.Parse("0100000001bc49d1eac23063996a981926f4c22b0a18aed8a458fa7f8ff54de8f20483f74d0000000000ffffffff02389d070000000000160014b47d3748ba615d0e3d579a7b54153fa51a5eb7d000000000000000000f6a0d54657374204d6573736167652100000000", Network);
            var opReturnOutput = tx.Outputs[1];

            string message = Crypto.Helper.ExtractMessageFromOutput(opReturnOutput);

            Assert.Equal(CustomMessage, message);
        }
    }
}