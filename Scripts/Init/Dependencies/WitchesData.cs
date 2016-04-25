
using System;
using System.Collections.Generic;

namespace Voltage.Witches
{
	using Voltage.Witches.Configuration;
	using Voltage.Witches.Models;
	using Voltage.Story.Configurations;
	using Voltage.Common.IAP;

//	using Voltage.Witches.Net;
	
	public interface IWitchesData		
	{
		Player Player { get; }

		MasterConfiguration MasterConfig { get; }
		MasterStoryData MasterStoryData { get; }

		ITransactionProcessor TransactionProcessor { get; }		// should this be here?

//		WitchesNetworkController NetworkController { get; }
	}

    public class WitchesData : IWitchesData
    {
		public Player Player { get; private set; }

		public MasterConfiguration MasterConfig { get; private set; }
		public MasterStoryData MasterStoryData { get; private set; }

		public ITransactionProcessor TransactionProcessor { get; private set; }

		public WitchesData (Player player, MasterConfiguration masterConfig, MasterStoryData storyData, ITransactionProcessor transactionProcessor)
		{
			if(player == null || masterConfig == null || storyData == null || transactionProcessor == null)
			{
				throw new ArgumentNullException();
			}

			Player = player;
			MasterConfig = masterConfig;
			MasterStoryData = storyData;

			TransactionProcessor = transactionProcessor;	// should this be here?

		}


    }
    
}




