using Voltage.Witches.Models;

namespace Voltage.Witches.Events
{
	public class OpenMailRequestEventArgs : GUIEventArgs
	{
		public Mail RequestedMail { get; protected set; }
		
		public OpenMailRequestEventArgs(Mail mail)
		{
			RequestedMail = mail;
		}
	}
}