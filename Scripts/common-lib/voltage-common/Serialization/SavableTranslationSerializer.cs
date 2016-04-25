namespace Voltage.Common.Serialization
{
	public class SavableTranslationSerializer<T,U> : BaseTranslationSerializer<T,U> where T: ISavable<U> where U: ISavableState<T>	// TODO: conform UniqueStoryNodeSerializer<T,U> to use this
	{
		public SavableTranslationSerializer (ISerializer<U> serializer, IDeserializer<U> deserializer) : base (serializer, deserializer) {}

		protected override U PreProcessWriteOutData (T data)
		{
			if(data != null)
			{
				return data.SavableState();
			}
			
			return default(U);
		}

		protected override T PostProcessReadInData (U saveData)
		{
			if(saveData != null)
			{
				return saveData.CreateInstance();
			}
			
			return default(T);
		}
	}
}

