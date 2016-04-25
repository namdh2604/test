using System;
namespace Voltage.Witches.Models
{
	[Flags]
	public enum MailCategory:short 
	{
		READ = 1,
		CHARACTER = 2,
		SYSTEM = 4
	}
}