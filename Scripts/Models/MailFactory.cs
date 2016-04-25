using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Voltage.Story.Variables;
using Voltage.Witches.Configuration;
using Voltage.Witches.Configuration.JSON;

using UnityEngine;

namespace Voltage.Witches.Models
{
	public interface IMailFactory
	{
		List<Mail> Create(string json);
	}

	public class MailFactory : IMailFactory
	{
		MasterConfiguration _master;
		ItemRawParser _itemParser;
		VariableMapper _mapper;
		private static string _tokenCapture =  @"(\[+.+?\])";

		public MailFactory(MasterConfiguration gameConfig,VariableMapper mapper)
		{
			_master = gameConfig;
			_itemParser = new ItemRawParser(_master);
			_mapper = mapper;
		}

		string AdjustJSONForReadability(string json)
		{
			string fixedJson = json;
			fixedJson = fixedJson.Replace("mail box","mail_box");

			return fixedJson;
		}

		public List<Mail> Create(string json)
		{
			//Invalid model coming in
			var fixedJson = AdjustJSONForReadability(json);

			JObject jsonObject = JObject.Parse(fixedJson);
			MailBoxData testMail = null;
			try
			{
				testMail = JsonConvert.DeserializeObject<MailBoxData>(jsonObject.ToString());
				UnityEngine.Debug.Log("Mail box deserialized...");
			}
			catch(Exception)
			{
				UnityEngine.Debug.LogError("Data was malformed");
				throw;
			}

			List<Mail> mailBox = new List<Mail>();

			for(int i = 0; i < testMail.mail_box.Count; ++i)
			{
				var data = testMail.mail_box[i];
				Mail mail = CreateMailFromData(data);
				if(mail == null)
				{
					throw new Exception("Something was incorrect in the data");
				}
				mailBox.Add(mail);
			}

			return mailBox;
		}

		MailCategory GetMailCategory(bool isRead,bool isCharacter)
		{
			MailCategory category;
			if((isRead) && (isCharacter))
			{
				category = MailCategory.READ | MailCategory.CHARACTER;
				return category;
			}
			else if((!isRead) && (isCharacter))
			{
				category = MailCategory.CHARACTER;
				return category;
			}
			else if((isRead) && (!isCharacter))
			{
				category = MailCategory.READ | MailCategory.SYSTEM;
				return category;
			}
			else
			{
				category = MailCategory.SYSTEM;
				return category;
			}
		}

		Mail CreateMailFromData(MailData data)
		{
			var isRead = data.read_flag;
			//HACK Right now, the server has the wrong flag set for system mails
			string sender = "K&C";
			if(_master.Character_Info.ContainsKey(data.sender_id))
			{
				sender = _master.Character_Info[data.sender_id].first_name;
			}
			else
			{
				var error = string.Format("SENDER {0} does not exist in the master data",data.sender_id);
				UnityEngine.Debug.LogWarning(error);
			}
			var isCharacter = (sender != "K&C") ? data.sender_flag : false;

			MailCategory category = GetMailCategory(isRead,isCharacter);
	
			Mail mail = null;

			string title = data.title;
			var name = ReplaceUserTokens(title);
			var messageBody = data.message_body;
			var message = ReplaceUserTokens(messageBody);

			try
			{
				string type = data.sender_type_for_metrics;
				mail = new Mail(data.id,name,category,sender,message, type);
			}
			catch(Exception)
			{
				throw new Exception("There was a problem generating the Mail");
			}

			List<Item> attachments = new List<Item>();
			List<bool> receivedAttachments = new List<bool>();

			try
			{
				for(int i = 0; i < data.gifts.Count; ++i)
				{
					var current = data.gifts[i];
					if(current == null)
					{
						continue;
					}
					ItemConfiguration itemConfig = _master.Items_Master[current.id];
					Item attachment = null;
					switch(itemConfig.ItemCategory)
					{
						case ItemCategory.CLOTHING:
							AvatarItemData avatarData = itemConfig.Item as AvatarItemData;
							attachment = _itemParser.CreateAvatarItem(avatarData);
							break;
						case ItemCategory.ILLUSTRATION:
							//TODO add ei data
							break;
						case ItemCategory.COINS:
							//TODO add coins data
							break;
						case ItemCategory.INGREDIENT:
							IngredientData ingredientData = itemConfig.Item as IngredientData;
							attachment = _itemParser.CreateIngredient(ingredientData);
							break;
						case ItemCategory.OUTFIT:
							//TODO handle Outftis stuff
							break;
						case ItemCategory.POTION:
							PotionData potionData = itemConfig.Item as PotionData;
							attachment = _itemParser.CreatePotion(potionData);
							break;
						case ItemCategory.STARSTONES:
							//TODO handle fucking starstones
							break;
					}

					if(attachment != null)
					{
						attachments.Add(attachment);
						receivedAttachments.Add(current.received_flag);
					}
				}

				if(data.premium_currency.HasValue)
				{
					StarStoneItem starstones = new StarStoneItem("starstones");
					starstones.Count = data.premium_currency.Value;
					starstones.Description = "A satchel containing " + data.premium_currency.Value.ToString() + " starstones";
					starstones.Category = ItemCategory.STARSTONES;
					
					attachments.Add(starstones as Item);
					receivedAttachments.Add(data.premium_received_flag);
					mail.Premium_Count = data.premium_currency;
				}

				if(data.free_currency.HasValue)
				{
					CoinItem coins = new CoinItem("coins");
					coins.Count = data.free_currency.Value;
					coins.Description = "A satchel containing " + data.free_currency.Value.ToString() + " coins";
					coins.Category = ItemCategory.COINS;

					attachments.Add(coins as Item);
					receivedAttachments.Add(data.free_currency_received_flag);
					mail.Free_Count = data.free_currency;
				}

				if(data.stamina_potion.HasValue)
				{
					string description = string.Format("It's a collection of {0} Stamina Potions",data.stamina_potion.Value);
					if(data.stamina_potion.Value < 2)
					{
						description = description.Replace("Potions","Potion");
					}

					Potion staminaPotion = new Potion("Stamina Potion", "Stamina Potion", description, string.Empty, new Dictionary<string,int>());

					attachments.Add(staminaPotion as Item);
					receivedAttachments.Add (data.stamina_potion_received_flag);
					mail.Stamina_Count = data.stamina_potion;
				}
			}
			catch(Exception)
			{
				#if DEBUG
				throw new Exception("There was a problem creating the attachments for the mail with this message : " + mail.Message);
				#else
				// TODO: Send Error to our servers so we can identify the bad mail.
				// NOOP
				#endif
			}

			mail.Attachments = attachments;
			mail.ReceivedAttachements = receivedAttachments;
            mail.Timestamp = data.install_date.ToLocalTime();

			return mail;
		}

		string ReplaceUserTokens(string text)
		{
			var matches = Regex.Matches(text, _tokenCapture);
			foreach(Match group in matches)
			{
				foreach(Capture capture in group.Captures)
				{
					var pattern = capture.ToString();
					var match = pattern.Substring(1,capture.Length - 2);

					string replacement = string.Empty;
					if(_mapper.TryGetValue(match, out replacement))
					{
						Debug.LogWarning("Made a replacement");
						text = text.Replace(pattern,replacement);
					}
				}
			}

			return text;
		}
	}
}