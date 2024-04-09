using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EncryptionSteganography.Encryption
{
    public class Encryptions
    {
        public static byte[] EncryptMessage(string plainText, string password, string algorithm)
        {
            // Проверка на алгоритъма и задаване на подходящия криптографски провайдър
            using (Aes aesAlg = Aes.Create())
            {
                // Подготовка на ключ и IV (initialization vector)
                var key = new Rfc2898DeriveBytes(password, new byte[] { 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80 }, 10000);
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                // Криптиране
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    MessageBox.Show("Succes");

                    return msEncrypt.ToArray();
                }
            }
        }
        public static string DecryptMessage(byte[] cipherText, string password, string algorithm)
        {
            // Проверка на алгоритъма и задаване на подходящия криптографски провайдър
            using (Aes aesAlg = Aes.Create())
            {
                // Подготовка на ключ и IV
                var key = new Rfc2898DeriveBytes(password, new byte[] { 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80 }, 10000);
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                // Декриптиране
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
