
using System;


namespace Voltage.Witches
{
	using Ninject;

	using Voltage.Common.Logging;

	using Voltage.Witches.Models;
	using Voltage.Witches.Screens;
	using Voltage.Witches.Controllers;

    public class ControllerData
    {

		[Inject]
		public IScreenFactory ScreenFactory { get; set; }
		[Inject]
		public Player Player { get; set; }
		[Inject]
		public ScreenNavigationManager NavManager { get; set; }
		[Inject]
		public ControllerRepo ControllerRepo { get; set; }
		[Inject]
		public ILogger Logger { get; set; }


    }
    
}




