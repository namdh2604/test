using System.Collections.Generic;

namespace Voltage.Story.StoryDivisions
{
    using Voltage.Story.Expressions;

    public interface ILockClassifier
    {
        LockType Classify(List<IExpression> requirements);
    }

    public class LockClassifier : ILockClassifier
    {
        ExpressionClassifier _classifier;

        public LockClassifier(ExpressionClassifier classifier)
        {
            _classifier = classifier;
        }

        public LockType Classify(List<IExpression> requirements)
        {
            LockType lockedReason = LockType.None;

            foreach (var req in requirements)
            {
                if (_classifier.Classify(req) == VariableCategory.Favorability)
                {
                    return LockType.Favorability;
                }
                else
                {
                    lockedReason = LockType.Progress;
                }
            }

            return lockedReason;
        }
    }
}

