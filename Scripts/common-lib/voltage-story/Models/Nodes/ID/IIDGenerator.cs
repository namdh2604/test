namespace Voltage.Story.Models.Nodes.ID
{
	public interface IIDGenerator<T,U>
	{
		T GenerateID();
		T GenerateID(U extra);
	}
}

