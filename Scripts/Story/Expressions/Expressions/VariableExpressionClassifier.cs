using System.Text.RegularExpressions;

namespace Voltage.Story.Expressions
{
    public interface IVariableExpressionClassifier
    {
        VariableCategory Classify(string name);
    }

    public class VariableExpressionClassifier : IVariableExpressionClassifier
	{
		static readonly Regex _affinityPattern = new Regex("Characters/([^/]+)/Affinity");

		public VariableExpressionClassifier()
		{
		}

		public VariableCategory Classify(string name)
		{
			Match match = _affinityPattern.Match(name);
			if (match.Success)
			{
				return VariableCategory.Favorability;
			}
				
			return VariableCategory.Other;
		}
	}
}

