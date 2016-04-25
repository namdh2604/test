
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Build
{
	using UnityEngine;

	using System.IO;
	using Newtonsoft.Json;
//	using Newtonsoft.Json.Linq;

	using Voltage.Witches.Components;
	using Voltage.Witches.Exceptions;
//	using Voltage.Common.JsonNet.Utils;

    public class RuneTask
    {
		private readonly ColorArrayConverter _colourConverter;
		private readonly JsonSerializer _serializer;

		public RuneTask(ColorArrayConverter colourConverter)
		{
			if(colourConverter == null)
			{
				throw new ArgumentNullException();
			}

			_colourConverter = colourConverter;
			_serializer = new JsonSerializer ();
		}


		public void PrecompileStrokeMap()
		{
			BuildRuneMap(GetStrokeTextures ());
		}


		private List<Texture2D> GetStrokeTextures()
		{
			List<Texture2D> textures = new List<Texture2D> (Resources.LoadAll<Texture2D> ("Tracing"));

			if(textures != null)
			{
				textures.RemoveAll (t => !IsValidTexture (t));
				return textures;
			}
			else
			{
				throw new WitchesException("No Stroke Textures");
			}
		}

		private bool IsValidTexture(Texture2D texture)
		{
			return !texture.name.Contains ("Test_Image");
		}

		private void BuildRuneMap(IEnumerable<Texture2D> strokes)
		{
			foreach(Texture2D stroke in strokes)
			{
				var pixels = stroke.GetPixels32();				// can throw an exception if texture not readable						
				bool[] accuracyMap = _colourConverter.ConvertColorArrayToBoolArray(pixels);		

				SerializeMap(stroke.name, accuracyMap);
			}
		}

		private void SerializeMap(string filename, bool[] map)
		{
			string fullpath = GetFullPath (filename);
			Debug.Log ("Serializing File: " + fullpath);

			using (StreamWriter streamWriter = new StreamWriter(fullpath))
			{
				using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
				{
//					_serializer.Serialize(jsonWriter, map);
					_serializer.Serialize(jsonWriter, ConvertToRuneData(map));
				}
			}
		}


		private RuneData ConvertToRuneData(bool[] map)
		{
			RuneData data = new RuneData (map.Length);

			for(int i=0; i < map.Length; i++)
			{
				if(map[i])
				{
					data.TrueIndices.Add(i);
				}
			}

			return data;
		}

//		private IList<int> ConvertToIntMap(bool[] map)
//		{
//			IList<int> intList = new List<int> ();
//			foreach(bool value in map)
//			{
//				intList.Add (value ? 1 : 0);
//			}
//
//			return intList;
//		}



		private string GetFullPath(string filename)
		{
			string dir = "Assets/Resources/Tracing/Mapping";

			if(!Directory.Exists(dir))
			{
				Debug.LogWarning ("Creating Directory: " + dir);
				Directory.CreateDirectory(dir);
			}

			return string.Format ("{0}/{1}.json", dir, filename);
		}

    }


    
}

public class RuneData
{
	public int Size { get; set; }
	public List<int> TrueIndices { get; set; }
	
	public RuneData(int size)
	{
		Size = size;
		TrueIndices = new List<int>();
	}
}


