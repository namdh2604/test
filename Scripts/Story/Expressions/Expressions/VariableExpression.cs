
using System;
using System.Collections.Generic;

namespace Voltage.Story.Expressions
{
	using Voltage.Story.Mapper;
    using Voltage.Witches.Exceptions;

    public abstract class VariableExpression : IVariableExpression
    {
        public string Variable { get; private set; }
        protected IMapping<string> VariableMap { get; private set; }

        protected VariableExpression(string variable, IMapping<string> variableMap)
        {
            Variable = variable;
            VariableMap = variableMap;
        }

        public virtual bool Evaluate()
        {
            return false;
        }

        public virtual List<string> GetDependencies()
        {
            return new List<string>() { Variable };
        }

		public bool TryGetValue(out int value)
		{
			return VariableMap.TryGetValue(Variable, out value);
		}

        public bool TryGetValue<T>(out T value)
        {
            return VariableMap.TryGetValue<T>(Variable, out value);
        }

        public int CompareTo (object other)
        {
            if (!(other is IExpression))
            {
                throw new ArgumentException("Unsupported type: " + other.GetType().Name);
            }

            return this.CompareTo(other as IExpression);
        }

        public int CompareTo(IExpression other)
        {
            if (other == null)
            {
                return 1;
            }

            if (other is StringExpression)
            {
                return this.CompareTo(other as StringExpression);
            }
            else if (other is NumeralExpression)
            {
                return this.CompareTo(other as NumeralExpression);
            }
            else if (other is IVariableExpression)
            {
                return this.CompareTo(other as IVariableExpression);
            }

            throw new ArgumentException("Unsupported expression type: " + other.GetType().Name);
        }

        protected virtual int CompareTo(StringExpression other)
        {
            throw new WitchesException("Unsupported operation");
        }

        protected virtual int CompareTo(NumeralExpression other)
        {
            throw new WitchesException("Unsupported operation");
        }

        protected virtual int CompareTo(IVariableExpression other)
        {
            object valueThis;
            object valueOther;

            if (TryGetValue<object>(out valueThis) && other.TryGetValue<object>(out valueOther))
            {
                int thisInt;
                int otherInt;
                if (Int32.TryParse(valueThis.ToString(), out thisInt) && Int32.TryParse(valueOther.ToString(), out otherInt))
                {
                    return new NumeralExpression(thisInt).CompareTo(new NumeralExpression(otherInt));
                }
                else
                {
                    return new StringExpression(valueThis.ToString()).CompareTo(new StringExpression(valueOther.ToString()));
                }
            }
            else
            {
                throw new WitchesException("Retrieving variable values failed");
            }
        }
    }
}




