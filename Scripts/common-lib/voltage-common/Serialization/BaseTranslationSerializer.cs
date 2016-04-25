using System.IO;

namespace Voltage.Common.Serialization
{
	public abstract class BaseTranslationSerializer<T,U> : ISerializer<T>, IDeserializer<T>
	{
		protected ISerializer<U> Serializer { get; set; }
		protected IDeserializer<U> Deserializer { get; set; }
		
		public BaseTranslationSerializer (ISerializer<U> serializer, IDeserializer<U> deserializer)
		{
			Serializer = serializer;
			Deserializer = deserializer;
		}

		public bool Serialize (T data, string filepath)
		{
			if(Serializer != null)
			{
				U transformed = PreProcessWriteOutData(data);
				if(transformed != null)
				{
					return Serializer.Serialize(transformed, filepath);
				}	
			}
			
			return false;
		}
		
		protected abstract U PreProcessWriteOutData (T data);

		
		public T Deserialize (string filepath)
		{
			if(Deserializer != null && !string.IsNullOrEmpty(filepath) && File.Exists(filepath))
			{
				U saveState = Deserializer.Deserialize(filepath);
				
				if(saveState != null)
				{
					return PostProcessReadInData (saveState);
				}
			}
			
			return default(T);
		}
		
		protected abstract T PostProcessReadInData (U saveData);
	}
}

