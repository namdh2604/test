using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Models
{
	public class MailData : BaseData 
	{
		public string message_body;
		public bool read_flag;
		public string EI;
		public string title;
		public string sender_id;
		public bool sender_flag;
		public List<MailGiftData> gifts;
		public bool multiply_bonus_flag;
		public string user_id;
		public string login_bonus_id;

		public int? premium_currency;
		public int? free_currency;
		public int? stamina_potion;
		public bool free_currency_received_flag;
		public bool premium_received_flag;
		public bool stamina_potion_received_flag;

		public string sender_type_for_metrics;
	}
}