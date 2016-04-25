using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Screens.Dialogues
{
	using UnityEngine.UI;
	using TMPro;

	using Voltage.Common.Logging;

	using Voltage.Witches.Screens;
	using Voltage.Witches.Models;

	public class AffinityChangeDisplayDialogue : BaseUGUIScreen
    {
		[SerializeField]
		private List<Sprite> _favorabilityIcons;

		[SerializeField]
		private Image _image;

		[SerializeField]
		private TextMeshProUGUI _label;

		[SerializeField]
		private RectTransform _root;		
		[SerializeField]
		private CanvasGroup _canvasGroup;


		private IDictionary<string,Sprite> _iconMap;

		private void Awake()
		{
			if(_favorabilityIcons == null || _favorabilityIcons.Count == 0 || _image == null || _label == null || _root == null || _canvasGroup == null)
			{
				throw new NullReferenceException();
			}

			// FIXME! sync against MasterStoryData.NPCs or PlayerDataStore...better to use some naming convention to derive image!
			IList<string> npcList = new List<string>{"N", "M", "R", "A", "T"};
			_iconMap = CreateSpriteMap (npcList, _favorabilityIcons);
		}

		private Dictionary<string,Sprite> CreateSpriteMap(IList<string> npc, IList<Sprite> icons)	// FIXME! sync against MasterStoryData.NPCs or PlayerDataStore...better to use some naming convention to derive image!
		{
			Dictionary<string,Sprite> map = new Dictionary<string,Sprite> ();

			if(npc.Count == icons.Count)
			{
				for(int i=0; i < npc.Count; i++)
				{
					map.Add (npc[i], icons[i]);		
				}
			}
			else
			{
				throw new ArgumentException("NPC and Icon count do not match");
			}

			return map;
		}




		public void ShowEffect(KeyValuePair<string,int> affinityChange)
		{
			if(ValidChange(affinityChange))
			{
				StartCoroutine(EffectRoutine (affinityChange));			// _effectRoutine = EffectRoutine (affinityChange);
			}
			else
			{
				AmbientLogger.Current.Log ("Invalid affinity change [{0}:{1}] for effect", LogLevel.WARNING); 		// (string.Format ("Invalid affinity change [{0}:{1}] for effect", affinityChange.Key, affinityChange.Value), LogLevel.WARNING); 
				Dispose ();
			}
		}

		private bool ValidChange(KeyValuePair<string,int> affinityChange)
		{
			return _iconMap.ContainsKey(affinityChange.Key); // && affinityChange.Value > 0; can affinityChange be null?
		}

		

		private IEnumerator EffectRoutine(KeyValuePair<string,int> affinityChange)
		{
			// Initialize effect
			SetEffectAlpha (0f);
			SetEffectScale (1f);
			SetEffectPosition (Vector2.zero);

			_image.sprite = _iconMap [affinityChange.Key];
			_label.text = string.Format ("+{0}", affinityChange.Value);


			// Animate effect
			iTween.EaseType scaleUpEase = iTween.EaseType.easeInExpo;
			iTween.EaseType normalEase = iTween.EaseType.easeInOutExpo;
			iTween.EaseType translationEase = iTween.EaseType.easeInOutQuad;

			float delay = 0.3f;
			float fadeInDuration = 0.5f;
			float scaleUpFactor = 3.5f;
			float scaleUpDuration = 0.5f; 
			iTween.ValueTo(gameObject, new Hashtable{ {"from",0f}, {"to",1f}, {"time",fadeInDuration}, {"easetype",normalEase}, {"onupdate", (Action<object>)(value => SetEffectAlpha((float)value))} });
			iTween.ScaleTo(gameObject, new Hashtable{{"scale", new Vector3(scaleUpFactor,scaleUpFactor,scaleUpFactor)}, {"time", scaleUpDuration}, {"easetype",scaleUpEase}}); 

			yield return new WaitForSeconds(scaleUpDuration + delay);

			Vector2 startPos = _root.anchoredPosition;
			Vector2 endPos = new Vector2 (0f, 500f);			// TODO: get relative Y-pos
			float translateDuration = 1.5f;
			float scaleDownFactor = 1f;
			float scaleDownDuration = 1.5f;
			iTween.ValueTo(gameObject, new Hashtable{ {"from",startPos}, {"to",endPos}, {"time",translateDuration}, {"easetype",translationEase}, {"onupdate",(Action<object>)(value => SetEffectPosition((Vector2)value))} });
			iTween.ScaleTo (gameObject, new Hashtable{ {"scale",new Vector3(scaleDownFactor,scaleDownFactor,scaleDownFactor)}, {"time",scaleDownDuration}, {"easetype",normalEase} });

			float delayFactor = 0.5f;
			yield return new WaitForSeconds(translateDuration * delayFactor);

			float fadeOutDuration = 0.75f;
			iTween.ValueTo(gameObject, new Hashtable{ {"from",1f}, {"to",0f}, {"time",fadeOutDuration}, {"easetype",normalEase}, {"onupdate", (Action<object>)(value => SetEffectAlpha((float)value))} });

			yield return new WaitForSeconds (fadeOutDuration);

			Dispose ();			// calling dispose on itself?
		}

		private void SetEffectAlpha(float value)
		{
			_canvasGroup.alpha = value;
		}

		private void SetEffectScale(float value)
		{
			_root.localScale = new Vector3 (value, value, value);
		}

		private void SetEffectPosition (Vector2 pos)
		{
			_root.anchoredPosition = pos;
		}

		public override void Dispose()
		{	
			iTween.Stop (gameObject);
			base.Dispose();
		}




		private Voltage.Witches.Controllers.IScreenController _controller;
		
		public void Init(Voltage.Witches.Controllers.IScreenController controller)
		{
			_controller = controller;
		}
		
		protected override Voltage.Witches.Controllers.IScreenController GetController()
		{
			return _controller;
		}





    }

}




