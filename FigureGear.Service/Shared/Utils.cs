using System.Security.Cryptography;
using System.Text;

namespace FigureGear.Service.Shared
{
    public class Utils
    {
        private const int Keysize = 128;
        private const int DerivationIterations = 1000;
        public static string Encrypt(string plainText)
        {
            string passPhrase = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var saltStringBytes = Generate128BitsOfRandomEntropy();
            var ivStringBytes = Generate128BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations, HashAlgorithmName.SHA256))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var aes = Aes.Create())
                {
                    aes.BlockSize = 128;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var encryptor = aes.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();

                                var cipherTextBytes = saltStringBytes
                                    .Concat(ivStringBytes)
                                    .Concat(memoryStream.ToArray())
                                    .ToArray();

                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            string passPhrase = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);

            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(16).ToArray();
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(16).Take(16).ToArray();
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(32).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations, HashAlgorithmName.SHA256))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var aes = Aes.Create())
                {
                    aes.BlockSize = 128;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var decryptor = aes.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                using (var reader = new StreamReader(cryptoStream, Encoding.UTF8))
                                {
                                    return reader.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate128BitsOfRandomEntropy()
        {
            var randomBytes = new byte[16];
            RandomNumberGenerator.Fill(randomBytes);
            return randomBytes;
        }
    }
}
