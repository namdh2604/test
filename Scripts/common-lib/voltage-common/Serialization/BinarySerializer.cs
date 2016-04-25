using System;
using System.IO;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Voltage.Common.Serialization
{
	public class BinarySerializer<T> : ISerializer<T>, IDeserializer<T> where T: class
	{
		public bool Serialize(T data, string filepath)	
		{
			if (!string.IsNullOrEmpty(filepath) && (data != null))
			{
				using (FileStream file = File.Create (filepath))
				{
					try
					{
						Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
						new BinaryFormatter().Serialize(file, data);

						return true;
					}
					catch (SerializationException e)
					{
						Console.WriteLine(e);
					}
				}
			}

			return false;
		}

		public T Deserialize(string filepath)
		{
			if (!string.IsNullOrEmpty(filepath) && File.Exists(filepath))
			{
				using (FileStream file = File.Open (filepath, FileMode.Open))
				{
					try
					{
						T data = new BinaryFormatter().Deserialize(file) as T;
						return data;
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