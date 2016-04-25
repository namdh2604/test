using System;
using System.IO;

namespace Voltage.Witches.User
{
	using Voltage.Witches.Crypto;

	// CryptoPlayerWriter handles the reading and writing of encrypted files
	// enforces structure: first byte is an index to the key mapping (up to 256) and the remainder is the encrypted data
	// takes PlayerDataStore as input, encrypts it and writes the bytes out to a file
	// encrypting/decrypting is delegated to the ICryptoService
	// sets CryptoKeyStore's decrypt key from file
	public class CryptoPlayerWriter : IPlayerWriter
	{
        private readonly ICryptoService _cryptoService;
        private readonly CryptoKeyStore _keyStore;
		private readonly IPlayerDataSerializer _serializer;
		private readonly string _path;


        public CryptoPlayerWriter(ICryptoService cryptoService, CryptoKeyStore keyStore, IPlayerDataSerializer serializer, string path)
		{
            if (cryptoService == null || keyStore == null || serializer == null || string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException();
            }

            _cryptoService = cryptoService;
            _keyStore = keyStore;
			_serializer = serializer;
            _path = path;
		}


		public void Save(PlayerDataStore playerData)
		{
			if (playerData == null) 
			{
                throw new ArgumentNullException();
			}

            string data = _serializer.Serialize(playerData, true);
            byte[] encrypted = _cryptoService.Encrypt(data);
            encrypted = AddKeyIndex(encrypted, _keyStore.CurrentKeyIndex);

            File.WriteAllBytes(_path, encrypted);
		}

		public PlayerDataStore Load()
		{
            if (!File.Exists(_path))
            {
                return null;
            }

            byte[] bytes = File.ReadAllBytes(_path);
            FileData fileData = ParseData(bytes);

            _keyStore.SetDecryptKey(fileData.KeyIndex);             // setting decrypt key to that of the file
            string data = _cryptoService.Decrypt(fileData.Data);

            return _serializer.Deserialize(data);
		}

		public bool HasExistingData
		{
			get { return File.Exists(_path); }
		}





        private byte[] AddKeyIndex(byte[] byteData, int index)
        {
            byte[] byteDataWithKeyIndex = new byte[byteData.Length + 1];
            byteData.CopyTo(byteDataWithKeyIndex, 1);

            byteDataWithKeyIndex[0] = Convert.ToByte(index);

            return byteDataWithKeyIndex;
        }

        private FileData ParseData(byte[] byteData)
        {
            int index = Convert.ToInt32(byteData[0]);
            byte[] data = StripKeyIndex(byteData);

            return new FileData() 
            {
                KeyIndex = index,
                Data = data
            };
        }

        private byte[] StripKeyIndex(byte[] byteDataWithKeyIndex)
        {
            byte[] byteData = new byte[byteDataWithKeyIndex.Length - 1];
            Buffer.BlockCopy(byteDataWithKeyIndex, 1, byteData, 0, byteData.Length);

            return byteData;
        }

        private sealed class FileData
        {
            public int KeyIndex { get; set; }
            public byte[] Data { get; set; }
        }
	}
        



}
