using System;

namespace Voltage.Story.Expressions
{
    using Voltage.Story.Mapper;
    using Voltage.Witches.Exceptions;

    public class DefaultVariableExpression : VariableExpression
    {
        public DefaultVariableExpression(string variable, IMapping<string> variableMap)
            : base(variable, variableMap)
        {
        }

        protected override int CompareTo(StringExpression other)
        {
            string value = string.Empty;
            if (TryGetValue(out value))
            {
                return value.CompareTo(other.Value);
            }

            throw new WitchesException("Retrieving variable values failed");
        }

        protected override int CompareTo(NumeralExpression other)
        {
            int num = 0;
            if (TryGetValue(out num))
            {
                return num.CompareTo(other.Value);
            }

            throw new WitchesException("Retrieving variable values failed");
        }
    }
}

