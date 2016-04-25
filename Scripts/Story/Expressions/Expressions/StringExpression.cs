
using System;
using System.Collections.Generic;

namespace Voltage.Story.Expressions
{


	public class StringExpression : IExpression, IComparable
	{
		public string Value { get; private set; }

		public StringExpression (string value)
		{
            if (value.Length >= 2)
            {
                if ((value[0] == '"') && (value[value.Length - 1] == '"'))
                {
                    value = value.Substring(1, value.Length - 2);
                }
                
            }
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
		
		public int CompareTo(StringExpression other)
		{
			if(other != null && !string.IsNullOrEmpty(other.Value) && !string.IsNullOrEmpty(Value))
			{
				return Value.CompareTo(other.Value);
			}
			else
			{
				throw new ArgumentNullException("StringExpression::CompareTo(StringExpression)");
			}
		}
		
		public int CompareTo(NumeralExpression other)
		{
			if (other != null)
			{
				int num = 0;
				if(Int32.TryParse(Value, out num))
				{
					return num.CompareTo(other.Value);
				}
				else
				{
					throw new ArgumentException("NumeralExpression::CompareTo(NumeralExpression)");
				}
			}
			else
			{
				throw new ArgumentNullException("StringExpression::CompareTo(NumeralExpression)");
			}
		}

		public int CompareTo(IVariableExpression other)
		{
			if(other != null)
			{
				string value = string.Empty;
				if(other.TryGetValue<string>(out value))
				{
					return Value.CompareTo(value);
				}
				else
				{
					throw new ArgumentException("StringExpression::CompareTo(IVariableExpression)");
				}
			}
			else
			{
				throw new ArgumentNullException("StringExpression::CompareTo(IVariableExpression)");
			}
		}

		
		public virtual int CompareTo(object other) 
		{
			if(other == null)
			{
				return 1;
			}
			else if(other is StringExpression)
			{
				return this.CompareTo(other as StringExpression);
			}
			else if(other is NumeralExpression)
			{
				return this.CompareTo(other as NumeralExpression);
			}
			else if (other is IVariableExpression)
			{
				return this.CompareTo(other as IVariableExpression);
			}
			else
			{
				throw new ArgumentException("StringExpression::CompareTo");
			}
		}
		
	}
    
}
