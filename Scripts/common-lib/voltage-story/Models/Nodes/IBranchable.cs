using System.Collections.Generic;

namespace Voltage.Story.Models.Nodes
{
	public interface IBranchable<T>
	{
		IList<T> Branches { get; }
	}
}

