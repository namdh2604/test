using System.Collections;

namespace Voltage.Witches.Controllers
{
    using Voltage.Witches.Screens;
    using Voltage.Witches.Models;
    using Voltage.Common.Metrics;

	public class NameRegisterDialogController
	{
		private readonly IScreenFactory _factory;
		private readonly StoryPlayerTutorialAPI _storyPlayerTutorialAPI;
        private NameRegistrationDialog _dialog;
		private Player _player;

        private bool _isNameEntered;

		public NameRegisterDialogController (IScreenFactory factory, Player player, StoryPlayerTutorialAPI storyPlayerTutorialAPI)
		{
			_factory = factory;
			_storyPlayerTutorialAPI = storyPlayerTutorialAPI;
			_player = player;

            _isNameEntered = false;
		}

        public IEnumerator WaitForNameEntry()
        {
            if (!IsUnnamed())
            {
                return null;
            }
            else
            {
                return WaitForNameEntryRoutine();
            }
        }

        private IEnumerator WaitForNameEntryRoutine()
        {
            Show();

            while (!_isNameEntered)
            {
                yield return null;
            }

            AmbientMetricManager.Current.LogEvent("name_register");


			MoveToNextNode ();
        }

		private void MoveToNextNode()	// meant to skips establishing node
		{
			_storyPlayerTutorialAPI.MoveToNextNode ();
		}
		
		private void Show()
		{
            _isNameEntered = false;

            _dialog = _factory.GetDialog<NameRegistrationDialog> (); // this dialog needs to be switched with NameRegisterDialogUGUI
			_dialog.Display(HandleNameInputs);
		}

		private void HandleNameInputs(int value)
		{
			RegisterPlayerName();
            _isNameEntered = true;
		}

		private void RegisterPlayerName()
		{
            _player.SetPlayerName(_dialog.FirstName, _dialog.LastName);
		}

        private bool IsUnnamed()
        {
            return (string.IsNullOrEmpty(_player.FirstName) && string.IsNullOrEmpty(_player.LastName));
        }
	}
}