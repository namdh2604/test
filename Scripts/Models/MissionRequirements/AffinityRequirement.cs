namespace Voltage.Witches.Models.MissionRequirements
{
    using Voltage.Story.Variables;

    public class AffinityRequirement : IMissionRequirement
    {
        public string CharName { get; private set; }
        public int Amount { get; private set; }

        private const string VARIABLE_FORMAT = "Characters/{0}/Affinity";
        private string _varPath;
        private int _cachedEval;

        public AffinityRequirement(string charName, int amount)
        {
	    	CharName = charName;
	    	Amount = amount;

            _varPath = string.Format(VARIABLE_FORMAT, charName);
        }

        public bool Evaluate(VariableMapper context)
        {
            int currentValue = 0;
            context.TryGetValue(_varPath, out currentValue);
            _cachedEval = currentValue;

            return (currentValue >= Amount);
        }

        public int GetCurrentAffinity()
        {
            return _cachedEval;
        }
    }
}

