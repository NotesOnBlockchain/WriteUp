using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteUpProject.Models
{
    public record FundingTxInfo(Network Network, string FundingTxID, string Vout, string AmountInSats);
}
