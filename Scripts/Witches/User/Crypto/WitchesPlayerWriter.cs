using System;
using System.IO;

namespace Voltage.Witches.User
{
	using Voltage.Witches.Crypto;

    public class WitchesPlayerWriter : IPlayerWriter
    {
        private readonly CryptoPlayerWriter _cryptoWriter;
        private readonly PlayerWriter _plainWriter;
        private bool _encrypt;


        private readonly string _jsonPath;
        private readonly string _encryptedPath;


		// WitchesPlayerWriter wraps the CryptoPlayerWriter and PlayerWriter to support the reading/writing of both JSON and encrypted files
		// prioritizes the reading of encrypted files
		// further ensures that on write, the other format is removed (to prevent confusion)
        public WitchesPlayerWriter(CryptoPlayerWriter cryptoWriter, PlayerWriter plainWriter, string encryptedPath, string jsonPath, bool encrypt=true)
        {
            if (cryptoWriter == null || plainWriter == null || string.IsNullOrEmpty(encryptedPath) || string.IsNullOrEmpty(jsonPath))
            {
                throw new ArgumentNullException();
            }

            _encrypt = encrypt;
            _plainWriter = plainWriter;
            _cryptoWriter = cryptoWriter;

            _jsonPath = jsonPath;
            _encryptedPath = encryptedPath;
        }


        public void Save(PlayerDataStore playerData)
        {
            if (playerData == null) 
            {
                throw new ArgumentNullException();
            }

            if (_encrypt)
            {
                _cryptoWriter.Save(playerData);

                // removes any existing json file, or could replace with encrypted...
                // performed after encrypted data is saved out to disk
                if (HasJson())  
                {
                    DeleteFile(_jsonPath);
                }
            }
            else
            {
                _plainWriter.Save(playerData);

                if (HasEncryptedData())
                {
                    DeleteFile(_encryptedPath);
                }
            }
        }

        private void DeleteFile(string path)
        {
            File.Delete(path);
        }


        public PlayerDataStore Load()
        {
            // tries to get encrypted data first
            if (HasEncryptedData()) // && _encrypt)         
            {
                return _cryptoWriter.Load();
            }
            else
            {
                return _plainWriter.Load();
            }
        }


        private bool HasJson()
        {
            return File.Exists(_jsonPath);
        }

        private bool HasEncryptedData()
        {
            return File.Exists(_encryptedPath);
        }
            
        // handle both json and encrypted files????
        public bool HasExistingData
        {
            get 
            { 
                return HasJson() || HasEncryptedData(); 
            }
        }


    }
}











