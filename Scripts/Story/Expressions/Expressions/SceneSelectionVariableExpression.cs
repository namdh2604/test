using System;

namespace Voltage.Story.Expressions
{
    using Voltage.Story.Mapper;
    using Voltage.Witches.Exceptions;

    public class SceneSelectionVariableExpression : VariableExpression
    {
        public SceneSelectionVariableExpression(string variable, IMapping<string> variableMap)
			: base(SanitizePath(variable), variableMap)
        {
        }

		private const string SEPARATOR = "/";

		private static string SanitizePath(string path)
		{
			string[] tokens = path.Split(new string[] { SEPARATOR }, StringSplitOptions.None);

			for (int i = 0; i < tokens.Length; ++i)
			{
				tokens[i] = tokens[i].Trim();
			}

			return string.Join(SEPARATOR, tokens);
		}

        public static bool HandlesPath(string path)
        {
            return path.StartsWith("Selections/");
        }

        public string Value
        {
            get {
                string value = string.Empty;
                if (!TryGetValue<string>(out value))
                {
					throw new WitchesException("Error retrieving value: " + Variable);
                }

                return value;
            }
        }

        public override bool Evaluate()
        {
            return !string.IsNullOrEmpty(Value);
        }

        protected override int CompareTo(StringExpression other)
        {
            return Value.CompareTo(other.Value);
        }
    }
}

