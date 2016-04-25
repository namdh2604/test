using UnityEngine;

using System;
using System.Collections.Generic;

using Ninject.Modules;

namespace Voltage.Witches
{
	using Voltage.Witches;
	using Voltage.Witches.DI;
    using Voltage.Witches.Scheduling;

	using iGUI;

	using Voltage.Common.Logging;
	using Voltage.Common.Android.ExpansionFile;
	using System.IO;

	public class Main : MonoBehaviour
	{
		public iGUIContainer contentPane;
		public iGUIContainer dialoguePane;
        public iGUIContainer overlayPane;

		public List<UnityEngine.Object> FirstSceneAssets;

		public TextAsset BuildVersion;


		private void Awake()
		{
			if(contentPane == null || dialoguePane == null)
			{
				throw new ArgumentNullException("Main::Awake >>> ");
			}

			//Uncomment if we want to just ignore all multi touch input
//			Input.multiTouchEnabled = false;
		}

		private void Start()
		{
            new InitCompositionRoot(contentPane, dialoguePane, overlayPane).Execute(StartGame);	// new InitCompositionRoot ().Execute (StartGame);
		}


        private void StartGame(IWitchesData data, INinjectModule parentModule)
		{
            new WitchesGameCompositionRoot(contentPane, dialoguePane, data).Execute(parentModule);
		}

	}






    
}




