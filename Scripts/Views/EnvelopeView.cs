using UnityEngine;
using iGUI;
using System.Collections.Generic;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;

	public class EnvelopeView : MonoBehaviour
	{
		[HideInInspector]
		public iGUIButton open_envelope_button; 

		[HideInInspector]
		public iGUIImage envelope_sealed,stamp_werbury,stamp_veronika,stamp_ty,stamp_trinity,stamp_rhys,stamp_niklas,stamp_mrs_collins,stamp_melanie,stamp_joanna,stamp_ingeborg,
						 stamp_holly,stamp_dad,stamp_catherine,stamp_casey,stamp_aunt_cheryl,stamp_anastasia,stamp_amelia,stamp_alix;

		[HideInInspector]
		public iGUILabel name_label,date_label;
	

		public event GUIEventHandler OpenMailRequest;

		private bool _isFront = false;
		public Mail Mail { get; protected set; }
		public bool IsOpened { get; protected set; }
		public int Count { get; protected set; }

		private Dictionary<string,iGUIElement> _stampReference;

		//HACK For testing
		private static Dictionary<string,string> _nameSwap = new Dictionary<string,string>()
		{
			{"Ellen Ripley","Trinity"},{"Carol Danvers","Veronika"},{"Kamala Khan","Melanie"},
			{"Dario Argento","Niklas"},{"Norinn Radd","Ty"},{"Benedict Cumberbatch","Rhys"},
			{"Thor Odinson","Dad"}
		};

		protected virtual void Start()
		{
			SetUpStampMap();
			RefreshView();
		}

		void SetUpStampMap()
		{
			_stampReference = new Dictionary<string,iGUIElement>()
			{
				{"System",stamp_werbury},{"Veronika",stamp_veronika},
				{"Tyrone",stamp_ty},{"Trinity",stamp_trinity},
				{"Rhys",stamp_rhys},{"Niklas",stamp_rhys},
				{"Mrs.",stamp_mrs_collins},{"Melanie",stamp_melanie},
				{"Joanna",stamp_joanna},{"Ingeborg",stamp_ingeborg},
				{"Holly",stamp_holly},{"Ken",stamp_dad},
				{"Catherine",stamp_catherine},{"Casey",stamp_casey},
				{"Aunt",stamp_aunt_cheryl},{"Anastasia",stamp_anastasia},
				{"Amelia",stamp_amelia},{"Alix",stamp_alix},
				{"K&C",stamp_werbury}
			};
		}

		public void SetEnvelope(Mail mail, int count, bool isFront)
		{
			Mail = mail;
			Count = count;
			_isFront = isFront;
		}

		public void RefreshView()
		{
			if(Mail != null)
			{
				if(Mail.isRead())
				{
					envelope_sealed.setEnabled(false);
				}
				else
				{
					envelope_sealed.setEnabled(true);
				}
				name_label.label.text = Mail.From;
				UpdateDateLabel();
				open_envelope_button.setEnabled(_isFront);
				LoadStamp();
			}
		}

		private void UpdateDateLabel()
		{
			date_label.label.text = "DD MMM";
			var month = Mail.Month_String;
			var date = Mail.Day.ToString("D2");

			var text = date_label.label.text;
			text = text.Replace("DD", date);
			text = text.Replace("MMM", month);
			date_label.label.text = text;
		}

		private void LoadStamp()
		{
			ResetStamps();
			iGUIElement stamp = null;
			string keyValue = string.Empty;
			if(_nameSwap.ContainsKey(Mail.From))
			{
				keyValue = _nameSwap[Mail.From];
			}
			else
			{
				keyValue = Mail.From;
			}

			if(_stampReference.TryGetValue(keyValue, out stamp))
			{
				stamp = _stampReference[keyValue];
			}
			else
			{
				stamp = _stampReference["System"];
			}
			stamp.setEnabled (true);
		}

		void ResetStamps()
		{
			foreach(var pair in _stampReference)
			{
				pair.Value.setEnabled(false);
			}
		}

		public void Open_Envelope()
		{
			Debug.Log("Open this envelope");
			if(OpenMailRequest != null)
			{
				OpenMailRequest(this, new OpenMailRequestEventArgs(Mail));
			}
		}
	}
}
