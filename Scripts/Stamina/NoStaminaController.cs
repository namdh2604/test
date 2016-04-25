
using System;
using System.Collections.Generic;

namespace Voltage.Witches.User
{
	using Voltage.Common.Logging;
	using Voltage.Witches.Exceptions;

	using Voltage.Witches.Shop;

	using Voltage.Witches.Models;
	using Voltage.Witches.Screens;
//	using Voltage.Witches.Controllers;


	public interface INoStaminaController
	{
		void Show (Action onComplete=null);
		void Dispose();

		event Action OnNoStaminaOpen;
		event Action OnNoStaminaClose;
	}


	public class NoStaminaController : INoStaminaController
    {
		private readonly Player _player;
		private readonly ShopDialogueController _shopDialogueController;
		private readonly IScreenFactory _screenFactory;

		private Action _onClose;

		public event Action OnNoStaminaOpen;
		public event Action OnNoStaminaClose;


		public NoStaminaController (Player player, ShopDialogueController shopDialogueController, IScreenFactory screenFactory)
		{
			if(player == null || shopDialogueController ==  null || screenFactory == null)
			{
				throw new ArgumentNullException();
			}

			_player = player;
			_shopDialogueController = shopDialogueController;
			_screenFactory = screenFactory;
		}

		public void Show(Action onComplete=null)		// maybe could drop the callback, in lieu of the event
		{
            _onClose = onComplete;

			if(OnNoStaminaOpen != null)
			{
				OnNoStaminaOpen();	
			}

            bool starterPackTriggered = HandleStarterPackTrigger();
            if (starterPackTriggered) 
            {
                ShowStarterPackDialog();
            }
            else
            {
                ShowNoStaminaDialogue();
            }

		}

		public void Dispose()
		{
			if (_shopDialogueController != null) 
			{
				_shopDialogueController.Dispose ();
			}

			// FIXME: need to implement dispose of any dialogues created in this controller (MC)

		}

        private bool HandleStarterPackTrigger()
        {
            if (StarterPackTriggerable())
            {
                AmbientLogger.Current.Log("NoStaminaController: Starter Pack Triggered!", LogLevel.INFO);

                // FIXME: need to finalize how STARTER_PACK_DURATION_IN_DAYS will be passed to player
                _player.MakeStarterPackAvailable(Voltage.Witches.DI.WitchesGameDependencies.STARTER_PACK_DURATION_IN_DAYS);
                return true;
            }

            return false;
        }

        private bool StarterPackTriggerable()
        {
            return !_player.StarterPackTriggered && _player.StaminaPotions == 0;
        }

		private void ShowStarterPackDialog()
		{
            _shopDialogueController.ShowStarterPackDialogue((success) =>
            {
                _shopDialogueController.Show(ShopDisplayType.POTION, OnClose);
            });
		}


		private void ShowNoStaminaDialogue() //(Action onClose)
		{
			NoStaminaDialog dialogue = _screenFactory.GetDialog<iGUISmartPrefab_NoStaminaDialog>();
			dialogue.Display ((choice) => {
				DialogResponse response = (DialogResponse)choice;

				if(response == DialogResponse.OK)
				{
					TryUseStaminaPotion();
				}
				else
				{
					OnClose();
				}
			});
		}



		private void TryUseStaminaPotion()
		{
			if(HasStaminaPotion)
			{
				ShowSystemDialog ("Refilled Stamina", OnClose);
				_player.ExchangePotionForStamina();
			}
			else
			{
				ShowNoStaminaPotionDialogue();
			}
		}

		private bool HasStaminaPotion
		{
			get { return _player.StaminaPotions > 0; }
		}

		private void ShowSystemDialog(string message, Action onClose=null)
		{
			var dialogue = _screenFactory.GetDialog<iGUISmartPrefab_SystemPopupDialog>();
			dialogue.SetText (message);
			
			dialogue.Display ((response) => {
				AmbientLogger.Current.Log (string.Format("System Dialogue \"{0}\" Closed", message), LogLevel.INFO);
				if(onClose != null)
				{
					onClose();
				}
			});
		}

		private void ShowNoStaminaPotionDialogue()
		{
			var dialogue = _screenFactory.GetDialog<iGUISmartPrefab_NoStaminaPotionDialog>();
			dialogue.Display ((choice) => {
				DialogResponse response = (DialogResponse)choice;

				if(response == DialogResponse.OK)
				{
					_shopDialogueController.Show(ShopDisplayType.POTION, ShowNoStaminaDialogue);
				}
				else
				{
					OnClose();
				}
			});
		}

		private void OnClose()
		{
			if(_onClose != null)
			{
				AmbientLogger.Current.Log ("NoStaminaController::OnClose >>> closing...", LogLevel.INFO);
				_onClose();
			}
			else
			{
				AmbientLogger.Current.Log ("NoStaminaController::OnClose >>> nothing to be done...", LogLevel.INFO);
			}

			if(OnNoStaminaClose != null)
			{
				OnNoStaminaClose();
			}
		}



    }
    

}




