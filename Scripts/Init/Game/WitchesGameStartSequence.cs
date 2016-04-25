
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
	using Voltage.Common.Logging;
	using Voltage.Witches.Models;


	public class WitchesGameStartSequence
	{
		public ILogger Logger { get; private set; }
		
		public Player Player { get; private set; }
		
		//		public IControllerRepo ControllerRepo { get; private set; }
		public GameResumer GameResumer { get; private set; }
		
		
		public WitchesGameStartSequence (Player player, GameResumer gameResumer, ILogger logger)
		{
			if (player == null || gameResumer == null || logger == null)
			{
				throw new ArgumentNullException("WitchesGameStartSequence::Ctor >>> ");
			}
			
			Player = player;
			Logger = logger;
//			ControllerRepo = controllerRepo;
			GameResumer = gameResumer;
		}
		
		
		public void Start ()
		{
			string progress = Player.CurrentScene;
			GameResumer.Resume (progress); //this will tell the last location user finished
		}
	}
    
}




