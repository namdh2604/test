
using System;


namespace Voltage.Witches.Screens
{

    public class ScreenFactoryData
    {
		public ScreenNavigationManager ScreenNavManager { get; private set; }
		public IScreenFactory ScreenFactory { get; private set; }

		public ScreenFactoryData (IScreenFactory screenFactory, ScreenNavigationManager screenNavManager)
		{
			if(screenFactory == null || screenNavManager == null)
			{
				throw new ArgumentNullException ("ScreenFactoryData::Ctor >>> " );		
			}

			ScreenNavManager = screenNavManager;
			ScreenFactory = screenFactory;
		}

    }
    
}




