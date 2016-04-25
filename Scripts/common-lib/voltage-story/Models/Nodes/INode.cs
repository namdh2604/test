using Voltage.Story.General;

namespace Voltage.Story.Models.Nodes
{
	public interface INode : IIdentifiable<string>
	{
		INode Next { get; set; }
		INode Previous { get; set; }
	}
}
