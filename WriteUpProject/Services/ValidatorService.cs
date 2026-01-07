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
        public static bool ValidateTxHex(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out int _);
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
