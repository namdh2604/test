namespace Voltage.Common.Serialization
{
	public interface IReferenceAFile
	{
		string Path { get; }
		string Filename { get; }
		string Type { get; }
	}
}
