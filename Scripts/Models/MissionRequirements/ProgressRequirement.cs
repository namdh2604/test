namespace Voltage.Witches.Models.MissionRequirements
{
    using Voltage.Story.Variables;

    public class ProgressRequirement : IMissionRequirement
    {
        public string SceneName { get; private set; }

        public ProgressRequirement(string sceneName)
        {
            SceneName = sceneName;
        }

        public bool Evaluate(VariableMapper context)
        {
            return true;
        }
    }
}

