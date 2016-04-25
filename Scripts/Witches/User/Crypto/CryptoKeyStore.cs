using System;
using System.Collections.Generic;
using System.Collections;

namespace Voltage.Witches.Crypto
{
    using System.Collections.ObjectModel;
	using System.Linq;

	// ICryptoKeyStore is an interface for retrieving keys/IV for symmetric cryptographic algorithms
	// returns a valid key as a byte array
	// supports behaviour when key may differ between that used to encrypt and decrypt (does not explicitly provide the same differentiation for initialization vectors (IV)...tho it still may be possible as an implementation)
	// further allows for mocking (which requires either an interface or virtual method)
	public interface ICryptoKeyStore        
	{
        byte[] GetEncryptKey();    
        byte[] GetDecryptKey();
        byte[] GetIV();             
	}

	// CryptoKeyStore satifies ICryptoKeyStore, further contains configurations related to cryptography of game (list of keys, IV and current key index)
	// returns a key as a byte array for encrypt and decrypt context
	// adds behaviour to take an integer input to specify which key to return (by index) for decryption Note: this (private) field is made static across all instances (see below)
	public class CryptoKeyStore : ICryptoKeyStore
	{

        private const int CURRENT_KEY_INDEX = 0;

		// Rijndael restricted to key length of: 128, 192, or 256 bits
        // NOTE: only 256 keys supported by our file structure
		private static readonly ReadOnlyCollection<ReadOnlyCollection<byte>> _keyList = new List<ReadOnlyCollection<byte>>() 
        {
			// REMOVING/MODIFYING key WILL break any existing encrypted files that use that key
			new List<byte>(){ 0x56, 0x1e, 0x10, 0xf6, 0x23, 0xdb, 0x77, 0x28, 0x1b, 0x55, 0x57, 0x44, 0xa9, 0x3f, 0xef, 0xc7, 0x7c, 0x43, 0x9a, 0xc3, 0x27, 0x17, 0x05, 0xf6, 0xb2, 0x61, 0xc7, 0x70, 0x69, 0xcd, 0x65, 0x17 }.AsReadOnly()
		}.AsReadOnly();

        // initialization vector (IV) needs to be exactly 16bytes long
		// REMOVING/MODIFYING IV WILL break all existing encrypted files
		private static readonly ReadOnlyCollection<byte> _iv = new List<byte>(){0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16}.AsReadOnly();




        public CryptoKeyStore() 
        {
            if (_keyList == null || _keyList.Count == 0) 
            {
                throw new ArgumentNullException();
            }

            if (_iv == null || _iv.Count == 0)
            {
                throw new ArgumentNullException();
            }

            if (!ValidIndex(CURRENT_KEY_INDEX))
            {
                throw new ArgumentNullException();
            }

            _decryptKeyIndex = CURRENT_KEY_INDEX;
        }


        public int CurrentKeyIndex
        {
            get { return CURRENT_KEY_INDEX; }
        }

		private bool ValidIndex(int index)
		{
			return (index < _keyList.Count && index >= 0);
		}


		public byte[] GetEncryptKey()
		{
            return GetByteArrayCopy(_keyList[CURRENT_KEY_INDEX].ToArray());
		}

        private static int _decryptKeyIndex;		// HACK: multiple keystores as well as decrypting multiple files are not required, so making static to avoid chance of different instances being created by the DI
		public void SetDecryptKey(int index)
		{
			if (!ValidIndex (index)) 
			{
                throw new ArgumentNullException();  // or default to _encryptKeyIndex?
			}

            _decryptKeyIndex = index;
		}

        // call SetDecryptKey(int) to modify index
		public byte[] GetDecryptKey()
		{
            return GetByteArrayCopy(_keyList[_decryptKeyIndex].ToArray());
		}


		// get pseudo-random initialization vector (IV) for build
		public byte[] GetIV()
		{
            return GetByteArrayCopy(_iv.ToArray());
		}


		private byte[] GetByteArrayCopy(byte[] original)
		{
			byte[] copy = new byte[original.Length];
			original.CopyTo(copy, 0);

			return copy;
		}
	}
}
