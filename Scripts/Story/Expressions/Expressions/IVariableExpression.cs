
using System;
using System.Collections.Generic;

namespace Voltage.Story.Expressions
{

	public interface IVariableExpression : IExpression, IComparable
	{
		bool TryGetValue(out int value);
		bool TryGetValue<T> (out T value);
	}

    
}




