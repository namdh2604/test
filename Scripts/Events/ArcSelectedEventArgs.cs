using Voltage.Witches.Models;

namespace Voltage.Witches.Events
{
	public class ArcSelectedEventArgs : GUIEventArgs
	{
		public CountryArc SelectedCountryArc { get; protected set; }

		public ArcSelectedEventArgs(CountryArc arc)
		{
			SelectedCountryArc = arc;
		}
	}
}