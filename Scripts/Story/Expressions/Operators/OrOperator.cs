

namespace Voltage.Story.Expressions
{


	public class OrOperator : IOperator		// TODO: Enforce that this can only be used with Expressions or Single Expressions
	{
		public bool Operate (IExpression left, IExpression right)
		{
			if (left != null && right != null)	
			{
				return left.Evaluate() || right.Evaluate ();
			}
			else
			{
				return false;
			}
		}
	}
    
}




