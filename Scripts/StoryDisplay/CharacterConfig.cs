namespace Voltage.Witches.Configuration
{
	public class CharacterConfig
	{
		public string Name;
		public string DefaultPose;
		public string DefaultOutfit;
		public string DefaultExpression;

		public CharacterConfig(string charName, string poseName, string outfitName, string expressionName)
		{
			Name = charName;
			DefaultPose = poseName;
			DefaultOutfit = outfitName;
			DefaultExpression = expressionName;
		}
	}
}

