
using System;

namespace Voltage.Story.Expressions
{
	// TODO: maybe don't need all of these type comparisons

	public class GreaterThanOperator : IOperator
	{
		public bool Operate (IExpression left, IExpression right)	
		{
			if(left is NumeralExpression && right is NumeralExpression)								// FIXME: without all the casts
			{
				return (left as NumeralExpression).CompareTo(right as NumeralExpression) > 0;
			}
			if(left is StringExpression && right is StringExpression)
			{
				return (left as StringExpression).CompareTo (right as StringExpression) > 0;
			}
			if(left is NumeralExpression && right is StringExpression)
			{
				return (left as NumeralExpression).CompareTo (right as StringExpression) > 0;
			}
			if(left is StringExpression && right is NumeralExpression)
			{
				return (left as StringExpression).CompareTo (right as NumeralExpression) > 0;
			}

			if(left is IVariableExpression && right is IVariableExpression)
			{
				return (left as IVariableExpression).CompareTo (right as IVariableExpression) > 0;
			}
//			if(left is IVariableExpression && right is StringExpression)
//			{
//				return (left as IVariableExpression).CompareTo (right as StringExpression) > 0;
//			}
			if(left is IVariableExpression && right is NumeralExpression)
			{
				return (left as IVariableExpression).CompareTo (right as NumeralExpression) > 0;
			}
//			if(left is StringExpression && right is IVariableExpression)
//			{
//				return (left as StringExpression).CompareTo (right as IVariableExpression) > 0;
//			}
			if(left is NumeralExpression && right is IVariableExpression)
			{
				return (left as NumeralExpression).CompareTo (right as IVariableExpression) > 0;
			}
			
			throw new ArgumentException ("GreaterThanOperator::Operate");
			
		}
	}
    
}




