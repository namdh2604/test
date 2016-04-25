
using System;
//using Voltage.Witches.User;

namespace Voltage.Witches.Models
{
//	using Voltage.Witches.User;

	public class NPCModel //: IRomanceable	
	{
		public string ID { get; set; }

		public string FirstName { get; private set; }
		public virtual string LastName { get; private set; }
        public string Shorthand { get; private set; }

		public string Initial 
		{ 
			get
			{
				return FirstName[0].ToString().ToUpper();		// NOTE: FirstName can be null/empty !!!!
			}
		}

		public string FullName { get { return string.Format ("{0}{1}", !string.IsNullOrEmpty (FirstName) ? FirstName : string.Empty, !string.IsNullOrEmpty (LastName) ? " " + LastName : string.Empty); } }

		public bool Romanceable { get; private set; }

		public NPCModel (string id, string firstName, string lastName="", string shorthand="", bool romanceable=false)
		{
			ID = id;

			FirstName = firstName;
			LastName = lastName;
            if (string.IsNullOrEmpty(shorthand))
            {
                Shorthand = FirstName;
            }
            else
            {
                Shorthand = shorthand;
            }

			Romanceable = romanceable;
		}
	}


	public class NPCFamily : NPCModel	
	{
		public Player Player { get; private set; }

		public override string LastName { get { return Player.LastName; } }

		public NPCFamily (string id, string firstName, Player player) : base (id, firstName)	// player.LastName
		{
			Player = player;
		}
	}
}



