using System;
using System.Collections.Generic;
using System.Collections;

namespace Voltage.Witches.EditorTools
{
    using UnityEditor;
    using UnityEngine;

    using System.IO;
    using System.Security.Cryptography;

	using Voltage.Witches.Crypto;
    using Voltage.Witches.User;

	
    // PlayerDataStoreUtil provides editor acccessible (via Menu Items) cryptography functions for encrypting/decrypting files
    // Uses RijndaelCryptoService
	public class PlayerDataStoreUtil : Editor
	{
        private const string FILE_TYPE = Voltage.Witches.DI.ConfigurationDependencies.FILE_TYPE;

        // NOTE: uses key set by CryptoKeyStore::CURRENT_KEY_INDEX
        [MenuItem("Curses/Crypto/Encrypt File")]
		private static void EncryptFile()
		{
			string filePath = EditorUtility.OpenFilePanel ("Select File To Encrypt", "", "json");

            if(!string.IsNullOrEmpty(filePath))
			{
				CryptoKeyStore keyStore = new CryptoKeyStore();
	            ICryptoService cryptoService = new RijndaelCryptoService(keyStore);

                string encryptedPath = Path.ChangeExtension(filePath, FILE_TYPE);
				CryptoPlayerWriter playerWriter = new CryptoPlayerWriter(cryptoService, keyStore, new JSONPlayerDataSerializer (), encryptedPath);

                string rawData = File.ReadAllText(filePath);
                PlayerDataStore dataStore = new JSONHashedPlayerDataSerializer(10, false).Deserialize(rawData);

                playerWriter.Save(dataStore);

                UnityEngine.Debug.Log(string.Format("[ENCRYPT] Saved out file: {0}", encryptedPath));
			}

		}


        [MenuItem("Curses/Crypto/Decrypt File")]
		private static void DecryptFile()
		{
            string filePath = EditorUtility.OpenFilePanel ("Select File To Decrypt", "", FILE_TYPE);

            if(!string.IsNullOrEmpty(filePath))
            {
                CryptoKeyStore keyStore = new CryptoKeyStore();
                ICryptoService cryptoService = new RijndaelCryptoService(keyStore);
                JSONPlayerDataSerializer serializer = new JSONPlayerDataSerializer ();

                CryptoPlayerWriter cryptoWriter = new CryptoPlayerWriter(cryptoService, keyStore, serializer, filePath);
                PlayerDataStore dataStore = cryptoWriter.Load();

                string jsonPath = Path.ChangeExtension(filePath, "json");
                PlayerWriter plainWriter = new PlayerWriter(serializer, jsonPath);

                plainWriter.Save(dataStore);

                UnityEngine.Debug.Log(string.Format("[DECRYPT] Saved out file: {0}", jsonPath));
            }
		}

	}


    // KeyGenWindow is an editor window, accessible via the menu to generate Rijndael cryptographic keys
	public class KeyGenWindow : EditorWindow
	{
        [MenuItem("Curses/Crypto/Generate Key [Rijndael]")]
        private static void GenerateKey()
        {
            EditorWindow.GetWindow<KeyGenWindow>();
        }

        private const int SALT_LENGTH = 8;              // minimum 8-bytes
        private const int MIN_ITERATION = 1000;         // minimum recommended is 1000
        private const int MAX_ITERATION = 10000;
        private const int BYTE_LENGTH = 32;

        private string _password;
        private byte[] _salt;
        private int _iterations;

        private byte[] _key;

        private Vector2 _scroll;
        private string _result;


		private void OnGUI()
		{
            EditorGUILayout.BeginVertical();
                
                _password = EditorGUILayout.TextField("Password: ", _password);

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("Generate") && !string.IsNullOrEmpty(_password))
                    {
                        _salt = GetSalt(SALT_LENGTH);
                        _iterations = new System.Random().Next(MIN_ITERATION, MAX_ITERATION);

                        _key = GenerateKey(_password, _salt, _iterations, BYTE_LENGTH);

                        _result = GetResultText(_key, _salt, _iterations, BYTE_LENGTH);
                        UnityEngine.Debug.Log("Generated...\n" + _result);
                    }
                    
                    if (GUILayout.Button("Auto-Generate"))
                    {
                        _key = AutoGenerateKey();
                        _result = GetAutoResultText(_key);
                        UnityEngine.Debug.Log("Generated...\n" + _result);
                    }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                _scroll = EditorGUILayout.BeginScrollView(_scroll);
                _result = EditorGUILayout.TextArea(_result, GUILayout.Height(150f));
                EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
		}


        private string GetResultText(byte[] key, byte[] salt, int iterations=0, int byteLength=0)
        {
            string strKey = ByteArrayToString(key);
            string strSalt = ByteArrayToString(salt);

            return string.Format("Key: {{ {0} }}\nSalt: {1}\nIterations: {2}\nByte Length: {3} [{4} bits]", strKey, strSalt, iterations, byteLength, byteLength*8);
        }
       

        // 8-bytes or larger
        private static byte[] GetSalt(int length)
        {
            if (length < 8)
            {
                throw new ArgumentException("salt must be 8-bytes or larger!");
            }

            byte[] salt = new byte[length];

            // using(RNGCryptoServiceProvider cryptoProvider = new RNGCryptoServiceProvider())                               
            RNGCryptoServiceProvider cryptoProvider = new RNGCryptoServiceProvider();                                 // System.Security.Cryptography.RNGCryptoServiceProvider
            cryptoProvider.GetNonZeroBytes(salt);
           
            return salt;
        }


        // Rijndael restricted to key length of: 128, 192, or 256 bits
        // https://msdn.microsoft.com/en-us/library/system.security.cryptography.rijndaelmanaged.aspx
        private byte[] GenerateKey(string password, byte[] salt, int iterations, int byteLength)
        {
            if (string.IsNullOrEmpty(password) || salt == null || salt.Length == 0 || iterations == 0)
            {
                throw new ArgumentException("argument is wrong");
            }

            List<int> validByteLengths = new List<int>(){ 16, 24, 32 };
            if (!validByteLengths.Contains(byteLength))
            {
                throw new ArgumentException("Byte length must be: 16, 24, or 32");
            }

            // using?
            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, iterations);       // System.Security.Cryptography.Rfc2898DeriveBytes
            return rfc2898DeriveBytes.GetBytes(byteLength);
        }


        private string ByteArrayToString(byte[] bytes)
        {
            System.Text.StringBuilder hex = new System.Text.StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes) 
            {
                hex.AppendFormat("0x{0:x2}, ", b);
            }

            hex.Length -= 2;   // removes last comma and space
            return hex.ToString();
        }

        private byte[] AutoGenerateKey()
        {
            var service = new System.Security.Cryptography.RijndaelManaged();
            service.GenerateKey();
            return service.Key;
        }

        private string GetAutoResultText(byte[] key)
        {
            string strKey = ByteArrayToString(key);
            return string.Format("Key: {{ {0} }}", strKey);
        }
	}
}







