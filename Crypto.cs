using System;
using System.Security.Cryptography;
using System.Text;

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
        public static string Decrypt(string resp)
        {
            byte[] src = Convert.FromBase64String(resp.Substring(0, resp.Length - 15));
            RijndaelManaged aes = new RijndaelManaged();
            byte[] key = Encoding.UTF8.GetBytes(getKey(resp.Split(resp.Substring(0, resp.Length - 15))[1]));
            aes.KeySize = 128;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode =  CipherMode.ECB;
            using (ICryptoTransform decrypt = aes.CreateDecryptor(key, null))
            {
                byte[] decr = decrypt.TransformFinalBlock(src, 0, src.Length);
                decrypt.Dispose();
                return Encoding.UTF8.GetString(decr);
            }
        }

        public static string Encrypt(string req)
        {
            byte[] src = Encoding.UTF8.GetBytes(req);
            byte[] key = Encoding.UTF8.GetBytes("09e32ji68RDaae6H");
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 128;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.ECB;
            using (ICryptoTransform encrypt = aes.CreateEncryptor(key, null))
            {
                byte[] encr = encrypt.TransformFinalBlock(src, 0, src.Length);
                encrypt.Dispose();
                return Convert.ToBase64String(encr);
            }
        }
    }
}