using NBitcoin;
using System.Text;
using WriteUpProject.Crypto;

namespace WriteUpProject.Services
{
    public static class ValidatorService
    {
        public static bool ValidateTxID(string txid)
        {
            if (uint256.TryParse(txid, out _))
            {
                return true;
            }
            return false;
        }

        public static bool ValidateChangeAddress(string changeAddress, Network network) 
        {
            if (Helper.TryParseAddress(changeAddress, network))
            {
                return true;
            }
            return false;
        }

        public static (bool, int) ValidateMessage(string message) 
        {
            int byteLength = Encoding.UTF8.GetBytes(message).Length;

            if (byteLength > 80)
            {
                return (false, byteLength);
            }
            return (true, byteLength);
        }
    }
}
