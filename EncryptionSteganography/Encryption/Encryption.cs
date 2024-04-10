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
        public static byte[] EncryptAES(string plainText, string password)
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
        public static string DecryptAES(byte[] cipherText, string password)
        {
            try
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
            catch (Exception ex)
            {
                return " Wrong Password or Wrong encryption method choosen";
            }
        }
        public static byte[] EncryptTDES(string TextToEncrypt,string mysecurityKey)


        {
            byte[] MyEncryptedArray = UTF8Encoding.UTF8
               .GetBytes(TextToEncrypt);

            MD5CryptoServiceProvider MyMD5CryptoService = new
               MD5CryptoServiceProvider();

            byte[] MysecurityKeyArray = MyMD5CryptoService.ComputeHash
               (UTF8Encoding.UTF8.GetBytes(mysecurityKey));

            MyMD5CryptoService.Clear();

            var MyTripleDESCryptoService = new
               TripleDESCryptoServiceProvider();

            MyTripleDESCryptoService.Key = MysecurityKeyArray;

            MyTripleDESCryptoService.Mode = CipherMode.ECB;

            MyTripleDESCryptoService.Padding = PaddingMode.PKCS7;

            var MyCrytpoTransform = MyTripleDESCryptoService
               .CreateEncryptor();

            byte[] MyresultArray = MyCrytpoTransform
               .TransformFinalBlock(MyEncryptedArray, 0,
               MyEncryptedArray.Length);

            MyTripleDESCryptoService.Clear();

            return MyresultArray;
        }
        public static string DecryptTDES(byte[] MyDecryptArray, string mysecurityKey)
        {
            try
            {

                MD5CryptoServiceProvider MyMD5CryptoService = new
                   MD5CryptoServiceProvider();

                byte[] MysecurityKeyArray = MyMD5CryptoService.ComputeHash
                   (UTF8Encoding.UTF8.GetBytes(mysecurityKey));

                MyMD5CryptoService.Clear();

                var MyTripleDESCryptoService = new
                   TripleDESCryptoServiceProvider();

                MyTripleDESCryptoService.Key = MysecurityKeyArray;

                MyTripleDESCryptoService.Mode = CipherMode.ECB;

                MyTripleDESCryptoService.Padding = PaddingMode.PKCS7;

                var MyCrytpoTransform = MyTripleDESCryptoService
                   .CreateDecryptor();

                byte[] MyresultArray = MyCrytpoTransform
                   .TransformFinalBlock(MyDecryptArray, 0,
                   MyDecryptArray.Length);

                MyTripleDESCryptoService.Clear();

                return UTF8Encoding.UTF8.GetString(MyresultArray);
            }
            catch (Exception ex)
            {
                return "Wrong Password or Wrong encryption method choosen";
                
                
            }
        }
    }
}
 