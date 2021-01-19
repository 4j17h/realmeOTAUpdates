using System;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace realmeOTAUpdates
{
    class Crypto
    {
        public static string getKey(string str)
        {
            string[] keys = { "oppo1997", "baed2017", "java7865", "231uiedn", "09e32ji6", "0oiu3jdy", "0pej387l", "2dkliuyt", "20odiuye", "87j3id7w" };
            int i = int.Parse(str[0].ToString());
            return keys[i] + str.Substring(4, 8);
        }

        public static string Decrypt(String resp)
        {
            byte[] data = Convert.FromBase64String(resp.Substring(0, resp.Length - 15));
            byte[] key = Encoding.UTF8.GetBytes(getKey(resp.Split(resp.Substring(0, resp.Length - 15))[1]));
            IBufferedCipher Cipher = CipherUtilities.GetCipher("AES/ECB/PKCS5Padding");
            Cipher.Init(false, new KeyParameter(key));
            return Encoding.UTF8.GetString(Cipher.DoFinal(data));
        }

        public static string Encrypt(String req)
        {
            byte[] data = Encoding.UTF8.GetBytes(req);
            byte[] key = Encoding.UTF8.GetBytes("09e32ji68RDaae6H");
            IBufferedCipher Cipher = CipherUtilities.GetCipher("AES/ECB/PKCS5Padding");
            Cipher.Init(true, new KeyParameter(key));
            return Convert.ToBase64String(Cipher.DoFinal(data));
        }
    }
}