using System.Collections.Generic;

namespace Voltage.Common.Serialization
{
	public class SavableDictionaryTranslationSerializer<T,U,V,W> : BaseTranslationSerializer<IDictionary<T,U>,IDictionary<V,W>> where T: ISavable<V> where U: ISavable<W> where V: ISavableState<T> where W: ISavableState<U>
	{
		public SavableDictionaryTranslationSerializer (ISerializer<IDictionary<V,W>> serializer, IDeserializer<IDictionary<V,W>> deserializer) : base (serializer, deserializer) {}
		
		protected override IDictionary<V,W> PreProcessWriteOutData (IDictionary<T,U> data)
		{
			IDictionary<V,W> serializable = new Dictionary<V,W>();
			
			foreach(KeyValuePair<T,U> kvp in data)
			{
				serializable.Add (kvp.Key.SavableState(), kvp.Value.SavableState());
			}
			
			return serializable;
		}
		
		
		protected override IDictionary<T,U> PostProcessReadInData (IDictionary<V,W> saveData)
		{
			IDictionary<T,U> transformed = new Dictionary<T,U>();
			
			if(saveData != null)
			{
				foreach (KeyValuePair<V,W> kvp in saveData)
				{
					transformed.Add (kvp.Key.CreateInstance(), kvp.Value.CreateInstance());
				}
			}
			
			return transformed;
		}
		
	}
}

