using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Voltage.Witches.Components
{
	public class DataInteraction
	{
		private const string TRACING_FOLDER = "Tracing/";
		private string _dataPath;

		public DataInteraction()
		{
			#if UNITY_EDITOR
//			_dataPath = Application.streamingAssetsPath + "/" + TRACING_FOLDER;
			_dataPath = Application.dataPath + "/Resources/" + TRACING_FOLDER; 
			#else
			_dataPath = "Resources/" + TRACING_FOLDER;
			#endif
//			Debug.Log(_dataPath);

		}

		public void WriteBytesToPng(byte[] bytes,string fileName)
		{
			//FOR USE WHEN WE OPTIMIZE TO WRITE THE IMAGES TO BYTES FILES
//			var name = (_dataPath + fileName + "_0" + ".bytes");
			var name = (_dataPath + fileName + "_0" + ".png");
			if(File.Exists(name))
			{
				name = ReturnNewName(name);
			}

			var file = File.Open(name, FileMode.CreateNew);
			var binary = new System.IO.BinaryWriter(file);
			binary.Write(bytes);
			binary.Close();
		}

		public byte[] ReadBytesFromName(string fileName)
		{
			fileName = ReturnValidatedName(fileName);

			if(!File.Exists(fileName))
			{
				throw new FileNotFoundException(fileName + " does not exist");
			}

			byte[] bytes = File.ReadAllBytes(fileName);
			return bytes;
		}

		public List<byte[]> ReadAllBytesFromAllAssociatedNames(string fileName)
		{
			List<byte[]> allAssociatedBytes = new List<byte[]>();
			fileName = ReturnValidatedName(fileName);

			if(!File.Exists(fileName))
			{
				throw new FileNotFoundException(fileName + " does not exist");
			}

			var allNames = ReturnAllAssociatedNames(fileName);

			for(int i = 0; i < allNames.Length; ++i)
			{
				Debug.LogWarning("FILE :: " + allNames[i]);
				byte[] bytes = File.ReadAllBytes(allNames[i]);
				allAssociatedBytes.Add(bytes);
			}

			return allAssociatedBytes;
		}

		public List<Texture2D> GetAllImagesFromName(string strokeName)
		{
			List<Texture2D> strokeTextures = new List<Texture2D>();
			var textures = Resources.LoadAll<Texture2D>("Tracing");

			for(int i = 0; i < textures.Length; ++i)
			{
				var currentTexture = textures[i];
				if(currentTexture.name.Contains(strokeName))
				{
					var id = GetStrokeIDFromName(currentTexture.name);
					strokeTextures.Insert(id,currentTexture);
				}
			}

			return strokeTextures;
		}

		public List<byte[]> GetAllImagesAsBytesFromName(string runeName)
		{
			List<byte[]> runeStrokes = new List<byte[]>();
			var baseBytes = Resources.LoadAll<TextAsset>("Tracing");

			for(int i = 0; i < baseBytes.Length; ++i)
			{
				var currentText = baseBytes[i];
				if(currentText.name.Contains(runeName))
				{
					var id = GetStrokeIDFromName(currentText.name);
					runeStrokes.Insert(id,currentText.bytes);
				}
			}

			return runeStrokes;
		}

		int GetStrokeIDFromName(string strokeName)
		{
			var parts = strokeName.Split(('_'));
			var id = (int)Convert.ToSingle(parts[parts.Length - 1]);

			return id;
		}

		bool HasAValidId(string fileName)
		{
			var parts = fileName.Split(('_'));
			int id = 0;
			return (int.TryParse((parts[parts.Length - 1]), out id));
		}

		string ReturnValidatedName(string fileName)
		{
			if(!fileName.Contains(_dataPath))
			{
				fileName = _dataPath + fileName;
			}

			if(!HasAValidId(fileName))
			{
				fileName = fileName + "_0";
			}

			return fileName;
		}

		string[] ReturnAllAssociatedNames(string textureName)
		{
			List<string> associatedNames = new List<string>();
			if(!textureName.Contains("_"))
			{
				textureName = textureName + "_0";
			}
			else
			{
				int lastIndex = textureName.LastIndexOf(('_'));
				var toReplace = textureName.Substring(lastIndex);
				textureName = textureName.Replace(toReplace,"_0");
			}

			var lookupName = textureName;
			associatedNames.Add(lookupName);
			var parts = textureName.Split(('_'));
			var id = (int)Convert.ToSingle(parts[parts.Length - 1]);

			while(File.Exists(lookupName))
			{
				if(!associatedNames.Contains(lookupName))
				{
					associatedNames.Add(lookupName);
				}

				++id;
				var toReplace = lookupName.Substring(lookupName.LastIndexOf(('_')));
				var replacement = "_" + id.ToString();
				lookupName = lookupName.Replace(toReplace,replacement);
			}

			return associatedNames.ToArray();
		}

		string ReturnNewName(string textureName)
		{
			if(!textureName.Contains(_dataPath))
			{
				textureName = _dataPath + textureName;
			}

			while(File.Exists(textureName))
			{
				textureName = textureName.Replace(".png","");
				var parts = textureName.Split(('_'));
				var id = (int)Convert.ToSingle(parts[parts.Length - 1]);
				++id;
				
				var toReplace = "_" + parts[parts.Length - 1];
				var replacement = "_" + id.ToString();
				
				textureName = textureName.Replace(toReplace,replacement);
				textureName += ".png";
			}

			return textureName;
		}
	}
}