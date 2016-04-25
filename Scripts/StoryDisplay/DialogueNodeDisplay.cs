using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Voltage.Witches.Models.Avatar;

using Voltage.Story.Models.Nodes;
using Voltage.Witches.AssetManagement;

using Voltage.Witches.Bundles;

namespace Voltage.Witches.Layout
{
	using Voltage.Story.Models.Data;
	
	[AddComponentMenu("StoryTool/DialogueNodeDisplay")]
	public class DialogueNodeDisplay : MonoBehaviour
	{
        public RawImage _background;
		public CharacterDisplayController _charDisplay;
		public ICharacterBundleManager _assetManager;
        public IAvatarResourceManager _avatarResourceManager;
		
		private DialogueBoxDisplay _boxDisplay;
		private Func<int,bool> _responseHandler;
		private int _timesFired = 0;

#pragma warning disable 414						// apparently used for fastforward, but that can be compiled out
		private bool _buttonPressed = false;
#pragma warning restore 414

		private JObject _config;
		public void SetConfig(JObject config)
		{
			_config = config;
		}
		
        public void SetTextAnimate(bool state)
        {
            if (_boxDisplay != null)
            {
                _boxDisplay.SetAnimateText(state);
            }
        }
        
        public void SetArrowActive(bool state)
        {
            if (_boxDisplay != null)
            {
                _boxDisplay._arrow.SetActive(state);
            }
        }

		public void HideSpeechBox()
		{
			if (_boxDisplay != null) {
				_boxDisplay.gameObject.SetActive(false);
			}
		}

		public IEnumerator Display(DialogueNodeViewData node, Action<List<string>> readyCallback = null)
		{
			yield return StartCoroutine(LoadCharacterBundle(node.Data.Left));
			yield return StartCoroutine(LoadCharacterBundle(node.Data.Right));

			if (!string.IsNullOrEmpty(node.Speaker) || node.SpeakerIsAvatar)
            {
				string speakerBundleName = node.SpeakerIsAvatar ? AvatarNameUtility.GetBundleName () : node.Speaker;
				yield return StartCoroutine(LoadBundle(speakerBundleName));
            }
			
			List<string> warnings = new List<string>();
			warnings.Add(UpdateBackground(node.Data.Background));
            if ((node.Text != null) && (node.Text.Length > 0))
            {
                warnings.Add(LoadSpeechBox(node.Data.SpeechBox, node.Speaker, node.Text, node.SpeakerIsAvatar));
            }
			
			warnings.Add(LoadCharacter(CharacterDisplayController.CharacterPosition.Left, node.Data.Left));
			warnings.Add(LoadCharacter(CharacterDisplayController.CharacterPosition.Right, node.Data.Right));
			
			if (readyCallback != null)
			{
				readyCallback(warnings);
			}
		}

        public IEnumerator DisplayEI(EventIllustrationNodeViewData node, Action<List<string>> readyCallback = null)
        {
			if (!string.IsNullOrEmpty(node.speaker) || node.speakerIsAvatar)
            {
				string speakerBundleName = node.speakerIsAvatar ? AvatarNameUtility.GetBundleName() : node.speaker;
				yield return StartCoroutine(LoadBundle(speakerBundleName));
            }

            List<string> warnings = new List<string>();
            warnings.Add(UpdateEIBackground(node.image));
            if ((node.text != null) && (node.text.Length > 0))
            {
                warnings.Add(LoadSpeechBox(node.speechBox, node.speaker, node.text, node.speakerIsAvatar));
            }

            if (readyCallback != null)
            {
                readyCallback(warnings);
            }

            yield break;
        }
		
		public void ButtonPress()
		{
            if ((_boxDisplay != null) && (_boxDisplay.IsAnimating()))
            {
                // Text was animating so we will complete the animation, and turn on the triangle.
                _boxDisplay.StopAnimating();
            }
			else if(_responseHandler != null)
			{
//				if(transform.parent.GetComponent<LayoutDisplay>()._composite != null)
//				{
//					transform.parent.GetComponent<LayoutDisplay>()._composite.processUGUIResponse(0);
//				}

				_responseHandler(0);
				++_timesFired;
				_buttonPressed = true;
			}
		}

		// HACK to allow quit to disable input on Quit dialogue, though MakePassive on screen controller will handle this
		public static bool InputEnabled = true;
		
		private void Update()
		{
			if (InputEnabled) 
			{
				if(UnityEngine.Input.GetKeyUp(KeyCode.Space))		// could compile this out as well if not editor
				{
					ButtonPress();
				}
				else if((UnityEngine.Input.touchCount > 0) && (UnityEngine.Input.touches[0].phase == TouchPhase.Began))
				{
					if(UnityEngine.Input.touches[0].phase == TouchPhase.Ended)
					{
						ButtonPress();
					}
#if DEVELOPMENT_BUILD || UNITY_EDITOR
					else if((UnityEngine.Input.touches[0].phase == TouchPhase.Stationary) && (UnityEngine.Input.touchCount > 1))	// && UnityEngine.Debug.isDebugBuild)
					{
						if(UnityEngine.Input.touches[1].phase == TouchPhase.Stationary)
						{
							if(!_buttonPressed)
							{
								ButtonPress();		// FAST-FORWARD (Debug ONLY!)
							}
						}
					}
#endif
				}
				else if((UnityEngine.Input.GetMouseButtonUp(0)))	// could compile this out as well if not editor
				{
					ButtonPress();
				}
#if DEVELOPMENT_BUILD || UNITY_EDITOR
				else if((UnityEngine.Input.GetMouseButton(1)))	// && UnityEngine.Debug.isDebugBuild)
				{
					if(!_buttonPressed)
					{
						ButtonPress();		// FAST-FORWARD (Debug ONLY!)
					}
				}
#endif
			}

		}
		
		public void SetInputHandler(Func<int,bool> responseCallback)
		{
			_responseHandler = responseCallback;
		}
		
		private IEnumerator LoadCharacterBundle(CharOptions options)
		{
			if (options.Enabled)
			{
				return LoadBundle(options.Name);
			}
			
			return GetEmptyEnumerator();
		}
		
		private IEnumerator LoadBundle(string name)
		{
			if ((name == null) || (name == string.Empty))
			{
				return GetEmptyEnumerator();
			}
			
			return _assetManager.DownloadBundle(name);
		}
		
		private string UpdateBackground(string name)
		{
			const string WEB_PREFIX = "images/backgrounds/";
			const string BG_PREFIX = "Images/Backgrounds/";
			const string EXT = ".png";
			string path;
			try
			{
				path = _config["Background"][name].ToString();
			}
			catch (Exception)
			{
				Debug.Log("something happened with path");
				return "error with background " + name + " configuration";
			}
			
			path = path.Substring(WEB_PREFIX.Length);
			path = BG_PREFIX + path;
			path = path.Remove(path.Length - EXT.Length);
            Texture2D texture = Resources.Load<Texture2D>(path);
            _background.texture = texture;
			
            if (texture == null)
			{
				return "Background path for " + name + " does not exist";
			}

			return String.Empty;
		}

        private string UpdateEIBackground(string name)
        {
            const string WEB_PREFIX = "images/EIs/";
            const string IMAGE_PREFIX = "Images/EventIllustrations/";
            string path;
            try
            {
                path = _config["EventIllustration"][name].ToString();
            }
            catch (Exception)
            {
                return "error locating event illustration";
            }

            path = path.Substring(WEB_PREFIX.Length);
            path = IMAGE_PREFIX + path;
            if (Path.HasExtension(path))
            {
                int extIndex = path.LastIndexOf('.');
                path = path.Substring(0, extIndex);
            }

            Texture2D texture = Resources.Load<Texture2D>(path);
            _background.texture = texture;

            if (texture == null)
            {
                return "Event Illustration: " + path + " does not exist in the build";
            }

            return string.Empty;
        }
		
        public string LoadSpeechBox(string boxType, string speaker, string text, bool isAvatar)
		{
			string path;
			try
			{
				path = _config["TextBoxFrame"][boxType].ToString();
			}
			catch (Exception)
			{
				//                Debug.Log("Could not locate speech box: " + boxType);
				//                return "Could not locate speech box: " + boxType;
				// TEMP HACK - REMOVE
				path = "images/speechboxes/thought_left.prefab";
			}

			string WEB_PREFIX = "images/";
			string EXT = ".prefab";
			path = path.Substring(WEB_PREFIX.Length);
			path = path.Remove(path.Length - EXT.Length);
			
			GameObject dialoguePrefab = Resources.Load<GameObject>(path);
			GameObject newDialogue = Instantiate(dialoguePrefab) as GameObject;
			newDialogue.name = "dialogue";
			_boxDisplay = newDialogue.GetComponent<DialogueBoxDisplay>();
			try
			{
                _boxDisplay.Setup(_assetManager, _avatarResourceManager, _config["characters"] as JObject);
			}
			catch(System.Exception e)
			{
				Debug.Log(e.Message);
			}
			try
			{
                _boxDisplay.UpdateDisplay(text, speaker, isAvatar); 
			}
			catch(System.Exception e)
			{
				Debug.Log(e.Message);
			}
			
			try
			{
				newDialogue.transform.SetParent(gameObject.transform, false);
			}
			catch(System.Exception e)
			{
				Debug.Log(e.Message);
			}
			
			return String.Empty;
		}
		
		private string LoadCharacter(CharacterDisplayController.CharacterPosition pos, CharOptions opts)
		{
            if (!opts.Enabled)
			{
				_charDisplay.HideCharacter(pos);
				return string.Empty;
			}
			
			JObject characterConfig;
            characterConfig = _config["characters"][opts.Name] as JObject;
            if (characterConfig == null)
            {
                return "Invalid character: " + opts.Name;
            }
			
            string pose = characterConfig["poses"].Value<string>(opts.Pose);
            if (pose == null)
            {
                return "Invalid pose: " + opts.Pose;
			}
			
			string outfit = characterConfig["outfits"].Value<string>(opts.Outfit);
            if (outfit == null)
            {
                return "Invalid outfit: " + opts.Outfit;
			}
			
			string expression = characterConfig["expressions"].Value<string>(opts.Expression);
            if (expression == null)
            {
                return "Invalid expression: " + opts.Expression;
			}
			
			_charDisplay.DisplayCharacter(pos, opts.Name, pose, outfit, expression);
			
			return String.Empty;
		}
		
		private IEnumerator GetEmptyEnumerator()
		{
			yield break;
		}
		
	}
}
