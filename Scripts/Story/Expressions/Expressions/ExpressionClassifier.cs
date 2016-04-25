using System.Collections.Generic;

namespace Voltage.Story.Expressions
{
    public interface IExpressionClassifier
    {
        VariableCategory Classify(IExpression expression);
    }

    public class ExpressionClassifier : IExpressionClassifier
    {
        IVariableExpressionClassifier _classifier;

        public ExpressionClassifier(IVariableExpressionClassifier classifier)
        {
            _classifier = classifier;
        }

        public VariableCategory Classify(IExpression expression)
        {
            List<string> depends = expression.GetDependencies();
            foreach (var dependency in depends)
            {
                if (_classifier.Classify(dependency) == VariableCategory.Favorability)
                {
                    return VariableCategory.Favorability;
                }
            }

            return VariableCategory.Other;
        }
    }
}

