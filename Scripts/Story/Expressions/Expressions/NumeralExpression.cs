
using System;
using System.Collections.Generic;

namespace Voltage.Story.Expressions
{

	public class NumeralExpression : IExpression, IComparable
	{
		public int Value { get; private set; }
		
		public NumeralExpression (int value)
		{
			Value = value;
		}
		
		public bool Evaluate()
		{
			return true;
		}

        public List<string> GetDependencies()
        {
            return new List<string>();
        }
		
		public int CompareTo(NumeralExpression other)
		{
			if(other != null)
			{
				return Value.CompareTo(other.Value);
			}
			else
			{
				throw new ArgumentNullException("NumeralExpression::CompareTo(NumeralExpression)");
			}
		}
		
		public int CompareTo(StringExpression other)
		{
			if(other != null && !string.IsNullOrEmpty(other.Value))
			{
				int num = 0;
				if(Int32.TryParse(other.Value, out num))
				{
					return Value.CompareTo(num);
				}
				else
				{
					throw new ArgumentException("NumeralExpression::CompareTo(StringExpression)");
				}
			}
			else
			{
				throw new ArgumentNullException("NumeralExpression::CompareTo(StringExpression)");
			}
		}

		public int CompareTo(IVariableExpression other)
		{
			if(other != null)
			{
				int num = 0;
				if(other.TryGetValue<int>(out num))
				{
					return Value.CompareTo(num);
				}
				else
				{
					throw new ArgumentException("NumeralExpression::CompareTo(IVariableExpression)");
				}
			}
			else
			{
				throw new ArgumentNullException("NumeralExpression::CompareTo(IVariableExpression)");
			}
		}

		
		public virtual int CompareTo (object other)
		{
			if(other == null)
			{
				return 1;
			}
			else if (other is NumeralExpression)
			{
				return this.CompareTo(other as NumeralExpression);
			}
			else if (other is StringExpression)
			{
				return this.CompareTo(other as StringExpression);
			}
			else if (other is IVariableExpression)
			{
				return this.CompareTo(other as IVariableExpression);
			}
			else
			{
				throw new ArgumentException("NumeralExpression::CompareTo");
			}
		}
	}

}




