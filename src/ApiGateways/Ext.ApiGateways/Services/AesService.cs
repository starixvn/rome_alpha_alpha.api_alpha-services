using System.Security.Cryptography;
using System.Text;

namespace Ext.ApiGateways.Services
{
    public interface IAesService
    {
        string Decrypt(string val);
    }

    public class AesService : IAesService
    {
        private const string SecretKey = "a5s8d2e9c1721234";

        public string Decrypt(string val)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(SecretKey);
                var iv = key;

                var encrypted = Convert.FromBase64String(val.Replace(" ", "+"));
                var decrypted = DecryptStringFromBytes(encrypted, key, iv);

                return decrypted;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        private string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException(nameof(cipherText));
            }

            string plaintext;
            using (var rijAlg = new AesManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                try
                {
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
                catch
                {
                    return string.Empty;
                }
            }

            return plaintext;
        }
    }
}