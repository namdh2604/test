using System.Collections.Generic;

namespace Voltage.Common.Serialization
{
	public class DictionarySavableValueTranslationSerializer<T,U,V> : BaseTranslationSerializer<IDictionary<T,U>,IDictionary<T,V>> where U: ISavable<V> where V: ISavableState<U>
	{
		public DictionarySavableValueTranslationSerializer (ISerializer<IDictionary<T,V>> serializer, IDeserializer<IDictionary<T,V>> deserializer) : base (serializer, deserializer) {}
		
		protected override IDictionary<T,V> PreProcessWriteOutData (IDictionary<T,U> data)
		{
			IDictionary<T,V> serializable = new Dictionary<T,V>();
			
//			if(data != null)
			{
				foreach(KeyValuePair<T,U> kvp in data)
				{
					serializable.Add (kvp.Key, kvp.Value.SavableState());
				}
				
			}
			
			return serializable;
		}
		
		
		protected override IDictionary<T,U> PostProcessReadInData (IDictionary<T,V> saveData)
		{
			IDictionary<T,U> transformed = new Dictionary<T,U>();
			
			if(saveData != null)
			{
				foreach (KeyValuePair<T,V> kvp in saveData)
				{
					transformed.Add (kvp.Key, kvp.Value.CreateInstance());
				}
			}
			
			return transformed;
		}
		
	}
}

