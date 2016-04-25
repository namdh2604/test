using System;
using System.Collections.Generic;

namespace Voltage.Witches.Controllers.Favorability
{
    public class FavorabilityMissionDialogViewModel
    {
        const int MAX_CHARACTERS = 3;
        const string ERR_TOO_MANY_CHARACTERS = "Too many characters requested: {0}, the favorability display only supports {1} characters";
        public List<CharFavorabilityDifferential> Differentials { get; protected set; }
		public string PolaroidPath { get; protected set; }

        public int CostToPurchase { get; protected set; }

        public FavorabilityMissionDialogViewModel(List<CharFavorabilityData> allCharacterStatuses, string polaroidPath, int costToPurchase)
        {
            VerifyCharacters(allCharacterStatuses);
            Differentials = CreateDifferentials(allCharacterStatuses);
			PolaroidPath = polaroidPath;
            CostToPurchase = costToPurchase;
        }

        public bool IsClear()
        {
            bool success = true;
            for (int i = 0; i < Differentials.Count; ++i)
            {
                if (Differentials[i].AmountLacking != 0)
                {
                    success = false;
                    break;
                }
            }

            return success;
        }

        private void VerifyCharacters(List<CharFavorabilityData> favorabilities)
        {
            if (favorabilities.Count > MAX_CHARACTERS)
            {
                string errMsg = string.Format(ERR_TOO_MANY_CHARACTERS, favorabilities.Count, MAX_CHARACTERS);
                throw new Exception(errMsg);
            }
        }

        private List<CharFavorabilityDifferential> CreateDifferentials(List<CharFavorabilityData> favorabilities)
        {
            List<CharFavorabilityDifferential> differentials = new List<CharFavorabilityDifferential>();
            foreach (var favorability in favorabilities)
            {
                int lacking = favorability.Required - favorability.CurrentValue;
                differentials.Add(new CharFavorabilityDifferential(favorability.CharName, favorability.Required, lacking));
            }

            return differentials;
        }

		public MileStoneDialogType Type
		{
			get
			{
				if(IsClear())
				{
					return MileStoneDialogType.RESUME;
				}
				else
				{
					return MileStoneDialogType.USE_POTION; // BREW_POTION
				}
			}
		}
    }

	public enum MileStoneDialogType
	{
		NONE = 0,
		USE_POTION,		// have (any) potion
		RESUME,			// have enough affinity
		BREW_POTION,
	}

    public enum MileStoneDialogResponse
    {
        CLOSE = 0,
        BUY_POTION = 1,
        RESUME = 2,
		BREW_POTION = 3
    }
}

