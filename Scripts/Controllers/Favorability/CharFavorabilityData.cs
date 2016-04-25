namespace Voltage.Witches.Controllers.Favorability
{
    public struct CharFavorabilityData
    {
        public string CharName;
        public int CurrentValue;
        public int Required;

        public CharFavorabilityData(string charName, int current, int required)
        {
            CharName = charName;
            CurrentValue = current;
            Required = required;
        }
    }
}

