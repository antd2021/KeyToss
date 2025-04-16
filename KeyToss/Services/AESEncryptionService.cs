using System.Security.Cryptography;

namespace KeyToss.Services
{
    internal class AESEncryptionService
    {
        public static string EncryptStringAES(string plainText, byte[] key, byte[] initializationVector)
        {
            //Creating a new aes encryption object and assigning the key and iv
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = initializationVector;

            //Creates an encryptor object that takes the key and iv from the aes object.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            //Creates a MemoryStream to temporarily hold the encrypted data.
            using MemoryStream msEncrypt = new();
            //Wraps the memory stream in a container so any data written to it gets encrypted.
            using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
            //Writes the plainText into the stream, which is encrypted and stored in memory.
            using (StreamWriter swEncrypt = new(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            //Converts the encrypted bytes into a Base64 string(makes it safe to store or transmit as plain text).
            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public static string DecryptStringAES(string cipherTextBase64, byte[] key, byte[] initializationVector)
        {
            byte[] buffer = Convert.FromBase64String(cipherTextBase64);

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = initializationVector;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new(buffer);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
    }
}
