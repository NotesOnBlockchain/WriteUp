using NBitcoin;

namespace WriteUpProject.Models
{
    public record FundingTxInfo(Network Network, string FundingTxHex, string Vout, string Xpub, string DerivationPath, string Fingerprint);
}
