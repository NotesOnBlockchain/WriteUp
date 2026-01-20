using NBitcoin;
using System;
using System.Buffers.Text;
using System.Net;
using System.Text;
using WriteUpProject.Crypto;

namespace WriteUpProject.Services
{
    public static class ValidatorService
    {
        public static bool ValidateTxHex(string hex)
        {
            foreach (Network network in Crypto.Helper.SupportedNetworks)
            {
                if (Transaction.TryParse(hex, network, out _))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ValidateXpub(string xpub, Network network)
        {
            try
            {
                _ = ExtPubKey.Parse(xpub, network);
                return true;
            }
            catch (Exception) 
            {
                return false;
            }   
        }

        public static bool ValidateDerivationPath(string path)
        {
            try
            {
                _ = KeyPath.Parse(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ValidateFingerprint(string fingerprint)
        {
            return HDFingerprint.TryParse(fingerprint, out _);
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
