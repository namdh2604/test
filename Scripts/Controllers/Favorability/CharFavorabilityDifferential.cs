namespace Voltage.Witches.Controllers.Favorability
{
    public struct CharFavorabilityDifferential
    {
        public string Character;
        public int AmountRequired;
        public int AmountLacking;
		public int CurrentAmount;

        public CharFavorabilityDifferential(string character, int amountRequired, int amountLacking)
        {
            Character = character;
            AmountRequired = amountRequired;
            AmountLacking = amountLacking;
			//HACK the updated dialog doesn't need the Amount Lacking
			CurrentAmount = AmountRequired - AmountLacking;
        }
    }
}

