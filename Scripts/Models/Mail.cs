using System;
using System.Collections.Generic;

namespace Voltage.Witches.Models
{
	public class Mail : Item 
	{
		public MailCategory MailCategory { get; protected set; }
		public string From { get; protected set; }
		public string Message { get; protected set; }
		public List<Item> Attachments { get; set; }
		public List<bool> ReceivedAttachements { get; set; }
		public int ItemCount { get { return Attachments.Count; } }
        public int Day { get { return Timestamp.Day; } }
        public int Month { get { return Timestamp.Month; } }
        public int Year { get { return Timestamp.Year; } }
		public string Month_String { get { return _monthMapping[Month]; } }
		public int? Premium_Count { get; set; }
		public int? Free_Count { get; set; }
		public int? Stamina_Count { get; set; }

        public DateTime Timestamp { get; set; }

		public string Type { get; private set; }

		private static Dictionary<int,string> _monthMapping = new Dictionary<int,string>()
		{
			{1,"JAN"},{2,"FEB"},{3,"MAR"},{4,"APR"},{5,"MAY"},{6,"JUN"},{7,"JUL"},{8,"AUG"},{9,"SEP"},{10,"OCT"},{11,"NOV"},{12,"DEC"}
		};

		public Mail(string id):base(id)
		{
			Attachments = new List<Item>();
			ReceivedAttachements = new List<bool>();
		}

		public Mail(string id, string name, MailCategory mailCategory ,string fromName, string message, string type=""):base(id)
		{
			MailCategory = mailCategory;
			Name = name;
			From = fromName;
			Message = message;
			Attachments = new List<Item>();
			ReceivedAttachements = new List<bool>();

			Type = type;
		}

		public void hasBeenRead()
		{
			if(isCharacterMail())
			{
				MailCategory = MailCategory.READ | MailCategory.CHARACTER;
			}
			else
			{
				MailCategory = MailCategory.READ | MailCategory.SYSTEM;
			}
		}

		public bool isRead()
		{
			return ((MailCategory & MailCategory.READ) != 0);
		}

		public bool isCharacterMail()
		{
			return ((MailCategory & MailCategory.CHARACTER) != 0);
		}

		public bool isSystemMail()
		{
			return ((MailCategory & MailCategory.SYSTEM) != 0);
		}
	}
}