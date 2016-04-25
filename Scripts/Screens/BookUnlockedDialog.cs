using iGUI;

namespace Voltage.Witches.Screens
{
	public class BookUnlockedDialog : AbstractDialog
	{
		public void cancel_button_Click(iGUIButton sender)
		{
			SubmitResponse((int)BookUnlockedResults.Cancel);
		}

		public void confirm_button_Click(iGUIButton sender)
		{
			SubmitResponse((int)BookUnlockedResults.Confirm);
		}
	}
}

public enum BookUnlockedResults
{
	Cancel,
	Confirm
}