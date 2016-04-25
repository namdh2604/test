using System;
using System.Collections.Generic;
using System.Collections;

// maybe move to Voltage.Common namespace
namespace Voltage.Witches.Crypto
{
    using System.Security.Cryptography;
    using System.IO;

    // RijndaelCryptoService performs symmetric encryption/decryption using the Rijndael algorithm
	// takes data as a string and returns an encrypted byte array
	// restricted to key length of: 128, 192, or 256 bits
    public class RijndaelCryptoService : ICryptoService
    {
        private readonly ICryptoKeyStore _cryptoKeyStore;

        public RijndaelCryptoService(ICryptoKeyStore cryptoKeyStore) 
        {
            if (cryptoKeyStore == null)
            {
                throw new ArgumentNullException();
            }

            _cryptoKeyStore = cryptoKeyStore;
        }


        public byte[] Encrypt(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException();
            }

            byte[] encrypted = new byte[0];

            using (RijndaelManaged rijndaelService = new RijndaelManaged())
            {
                byte[] key = _cryptoKeyStore.GetEncryptKey();
                byte[] iv = _cryptoKeyStore.GetIV();

                ICryptoTransform encryptor = rijndaelService.CreateEncryptor(key, iv);

                using(MemoryStream memStream = new MemoryStream())
                {
                    using(CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                    {
                        using(StreamWriter strStream = new StreamWriter(cryptoStream))
                        {
                            strStream.Write(str);
                        }

                        encrypted = memStream.ToArray();
                    }
                }
            }

            return encrypted;
        }


        public string Decrypt(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 0)
            {
                throw new ArgumentNullException();
            }

            string decrypted = string.Empty;

            using (RijndaelManaged rijndaelService = new RijndaelManaged())
            {
                byte[] key = _cryptoKeyStore.GetDecryptKey();
                byte[] iv = _cryptoKeyStore.GetIV();

                ICryptoTransform decryptor = rijndaelService.CreateDecryptor(key, iv);

                using(MemoryStream memStream = new MemoryStream(bytes))
                {
                    using(CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                    {
                        using(StreamReader strStream = new StreamReader(cryptoStream))
                        {
                            decrypted = strStream.ReadToEnd();
                        }
                    }
                }
            }

            return decrypted;
        }
    }
}
