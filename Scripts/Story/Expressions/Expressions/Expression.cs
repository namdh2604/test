
using System;
using System.Collections.Generic;


namespace Voltage.Story.Expressions
{

	public class Expression : IExpression	// TODO: Enforce operator rules (e.g., AND can only work on expressions or single expressions (evaluate vs operate) 
	{
		public IExpression Left { get; private set; }
		public IExpression Right { get; private set; }
		public IOperator Operator { get; private set; }

		public Expression (IExpression left, IExpression right, IOperator op)
		{
			Left = left;
			Right = right;
			Operator = op;
		}	

		public bool Evaluate()
		{
			if(Left != null && Right != null && Operator != null)
			{
				return Operator.Operate(Left, Right);
			}
			else if(Left != null)
			{
				return Left.Evaluate();
			}
			else
			{
				return true;
			}
		}

        public List<string> GetDependencies()
        {
            List<string> allDepends = new List<string>();
            allDepends.AddRange(Left.GetDependencies());
            allDepends.AddRange(Right.GetDependencies());

            return allDepends;
        }
	}



    
}




