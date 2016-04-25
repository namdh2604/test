using System;
using System.IO;

using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Voltage.Common.Serialization
{
	public class JsonTextSerializer<T> : ISerializer<T>, IDeserializer<T> //where T: class	// TODO: BSON
	{
		public bool Serialize (T data, string filepath)
		{
			if(!string.IsNullOrEmpty(filepath) && data != null)
			{
				using (StreamWriter file = File.CreateText(filepath))
				{
					try
					{
						new JsonSerializer().Serialize(file, data);		// JsonConvert.SerializeObject(data));
						return true;
					}
					catch (JsonSerializationException e)
					{
						Console.WriteLine(e);
					}
				}
			}

			return false;
		}

		public T Deserialize (string filepath)
		{
			if(!string.IsNullOrEmpty(filepath) && File.Exists(filepath))
			{
//				try
//				{
//					return (T)JsonConvert.DeserializeObject<T>(filepath);
//				}
//				catch (Exception e)
//				{
//					Console.WriteLine (e.ToString());
//				}

				using (StreamReader file = File.OpenText(filepath))
				{
					try
					{
						return (T)new JsonSerializer().Deserialize(file, typeof(T));	// JsonConvert.DeserializeObject<T>(File.ReadAllText(filepath))
					}
					catch (InvalidCastException e)
					{
						Console.WriteLine(e);
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
					}
				}
			}

			return default(T);
		}
	}
}

