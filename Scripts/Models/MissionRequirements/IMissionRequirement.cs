namespace Voltage.Witches.Models.MissionRequirements
{
    using Voltage.Story.Variables;

    public interface IMissionRequirement
    {
        bool Evaluate(VariableMapper context);
    }
}

