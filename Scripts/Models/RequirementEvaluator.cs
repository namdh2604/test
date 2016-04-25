using System.Linq;
using Voltage.Story.StoryDivisions;
using Voltage.Story.Variables;
using Voltage.Witches.Models.MissionRequirements;

namespace Voltage.Witches.Models
{
    public class RequirementEvaluator
    {
        private readonly VariableMapper _context;

        public RequirementEvaluator(VariableMapper context)
        {
            _context = context;
        }

        public LockType GetLockType(SceneHeader header)
        {
            LockType lockType = LockType.None;

            var failingReqs = header.Requirements.Where(x => !x.Evaluate(_context));

            foreach (var req in failingReqs)
            {
                if (req.GetType() == typeof(AffinityRequirement))
                {
                    lockType |= LockType.Favorability;
                }
                else if (req.GetType() == typeof(ClothingRequirement))
                {
                    lockType |= LockType.Clothing;
                }
                else if (req.GetType() == typeof(ProgressRequirement))
                {
                    lockType |= LockType.Progress;
                }
            }

            return lockType;
        }
    }
}

