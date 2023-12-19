using System.Security.Cryptography;
using System.Text;

namespace Alpha.Utilities.Helpers

{
    public static class AesService
    {
        private const string SecretKey = "a5s8d2e9c1721234";

        public static string Decrypt(string val)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(SecretKey);
                var iv = key;

                var encrypted = Convert.FromBase64String(val.Replace(" ", "+"));
                var decrypted = DecryptStringFromBytes(encrypted, key, iv);

                return decrypted;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
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
        public static string Encrypt(string val)
        {
            try
            {
                var key = System.Text.Encoding.UTF8.GetBytes(SecretKey);
                var iv = key;
                var decrypted = EncryptStringToBytes(val, key, iv);

                return Convert.ToBase64String(decrypted);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            if (string.IsNullOrWhiteSpace(plainText))
            {
                throw new ArgumentNullException(nameof(plainText));
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException(nameof(iv));
            }

            byte[] encrypted;
            using (var rijAlg = new AesManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                using (var msEncrypt = new MemoryStream())
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }

            return encrypted;
        }
    }
}