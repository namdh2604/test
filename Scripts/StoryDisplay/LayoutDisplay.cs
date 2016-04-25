using UnityEngine;
using UnityEngine.UI;
//using Voltage.Story.Models.StructNodes;
using Voltage.Story.Models.Nodes;
using Voltage.Witches.AssetManagement;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TMPro;
using System;
using System.Linq;

using System.IO;
using Voltage.Witches.Screens;
using Voltage.Witches.Utilities;
using Voltage.Common.Utilities;

using Voltage.Witches.Bundles;


namespace Voltage.Witches.Layout
{
	using Voltage.Witches.Controllers;
	using Voltage.Story.Models.Data;
	
	using Voltage.Story.Views;

	public enum CharacterPosition
	{
		Left = 0,
		Right = 1
	}
	
	[AddComponentMenu("StoryTool/LayoutDisplay")]
	public class LayoutDisplay : MonoBehaviour, ILayoutDisplay
	{
		[SerializeField]
		private Transform _root;
		public Transform Root { get { return _root; } }
		
		public WitchesStoryPlayerScreenController _controller { get; protected set; }

		public CharacterBundleManager _runtimeManager;
		public ICharacterBundleManager AssetManager { get; protected set; }

        public AssetBundleManager AssetBundleManager;
        public IAvatarResourceManager AvatarResourceManager { get; protected set; }
		
		public GameObject _dialogueDisplay;
		public GameObject _selectionDisplay;
		public ErrorDisplay _errorDisplay;

		private DialogueNodeDisplay _nodeDisplay;

//		private bool _isMakingADialogue = false;

		/* this might need to be abstracted out to a better class, protecting us from the JSON interface */
		private JObject _config;

        void Awake()
        {
            if (AssetBundleManager == null)
            {
                AssetBundleManager = UnityEngine.Object.FindObjectOfType<AssetBundleManager>();
            }
            #if UNITY_EDITOR
            AssetManager = new EditorCharacterBundleManager();
            AvatarResourceManager = new EditorAvatarResourceManager();
            #else
            AssetManager = _runtimeManager;
            AvatarResourceManager = new AvatarResourceManager(AssetBundleManager);
            #endif
        }

		void OnDestroy()
		{
			Debug.Log ("Layout Display is destroyed!!!!");
			AssetManager.Cleanup();
			Resources.UnloadUnusedAssets ();
		}

		public void HideSpeechBox()
		{
            if (_nodeDisplay != null)
            {
    			_nodeDisplay.HideSpeechBox ();
            }
		}

		public void SetController(WitchesStoryPlayerScreenController controller)
		{
			_controller = controller;
		}


		public void SetConfiguration(JObject config)
		{
			_config = config;
			AssetManager.SetConfiguration(config["characters"] as JObject);
			_dialogueDisplay.GetComponent<DialogueNodeDisplay>().SetConfig(_config);
		}

		public void DisplayEmpty()
		{
			ResetDisplay();
		}
		

		
		public void ToggleText()
		{
			DialogueBoxDisplay textDisplay = gameObject.GetComponentInChildren<DialogueBoxDisplay>();
			textDisplay._text.enabled = !textDisplay._text.enabled;
		}
		
		private void SetupSampleConfig()
		{
			string jsonInput = @"{
""Background"": {""Germany/Brocken_Mountains"": ""images/backgrounds/germany/Brocken_Mountains.png"" },
""TextBoxFrame"": {""Thoughts_Left"": ""images/speechboxes/thought_left.prefab"" },
""characters"": {
    ""Anastasia"": { ""bundle"": ""Anastasia"", ""outfits"": {""Nekkid"": ""default""}, ""poses"": {""Guarded"": ""pose_02""}, ""expressions"": {""Smiling"": ""basic""}, ""speaker_info"": { ""pose"": ""Guarded"", ""outfit"": ""Nekkid"", ""expression"": ""Smiling"" } },
    ""Niklas"": { ""bundle"": ""Niklas"", ""outfits"": {""Classy"": ""outfit2""}, ""poses"": {""Guarded"": ""pose_01""}, ""expressions"": {""Angry"": ""angry""}, ""speaker_info"": { ""pose"": ""Guarded"", ""outfit"": ""Classy"", ""expression"": ""Angry"" } },
	""Phantom"": { ""bundle"": ""Niklas"", ""outfits"": {""Casual"": ""outfit1""}, ""poses"": {""Guarded"": ""pose_01""}, ""expressions"": {""Angry"": ""angry""}, ""speaker_info"": { ""pose"": ""Guarded"", ""outfit"": ""Casual"", ""expression"": ""Angry"" } }
}
}";
			SetConfiguration(JObject.Parse(jsonInput));
		}
		


        public void DisplayEI(EventIllustrationNodeViewData node, Action<List<string>> readyCallback = null, Func<int,bool> actionCallback = null)
        {
            StartCoroutine(DisplayEIRoutine(node, readyCallback, actionCallback));
        }

        private IEnumerator DisplayEIRoutine(EventIllustrationNodeViewData node, Action<List<string>> readyCallback, Func<int,bool> actionCallback)
        {
            GameObject go = PrefabHelper.Instantiate(_dialogueDisplay) as GameObject;
            DialogueNodeDisplay display = go.GetComponentInChildren<DialogueNodeDisplay>();
            display._assetManager = AssetManager;
            display._avatarResourceManager = AvatarResourceManager;
            display.SetConfig(_config);

            List<string> warnings = null;
            yield return StartCoroutine(display.DisplayEI(node, (x => warnings = x)));

            ClearScreen();
            go.transform.SetParent(gameObject.transform, false);
            DisplayWarnings(warnings);

            display.gameObject.SetActive(false);
            display.gameObject.SetActive(true);   

            var processHandler = CreateProcessHandler(actionCallback, display);
			display.SetInputHandler(processHandler);

			if (readyCallback != null)
			{
				readyCallback(warnings);
			}
        }

        public IEnumerator PreloadDependencies(IEnumerable<string> resources)
        {
            foreach (var charName in resources)
            {
                yield return StartCoroutine(AssetManager.DownloadBundle(charName));
            }
        }
		
		public void DisplayDialogue(DialogueNodeViewData node, Action<List<string>> readyCallback = null, Func<int,bool> actionCallback = null)
		{
			Action<GameObject, List<string>> wrappedReady = delegate(GameObject _, List<string> warnings)
			{
				if (readyCallback != null)
				{
					readyCallback(warnings);
				}
			};
//			if(!_isMakingADialogue)
//			{
				StartCoroutine(DisplayDialogueRoutine(node, wrappedReady, actionCallback));
//			}
//			else
//			{
//				Debug.Log("Is Already making a dialogue");
//			}
		}

		
		public IEnumerator DisplayDialogueRoutine(DialogueNodeViewData node, Action<GameObject, List<string>> readyCallback, Func<int,bool> actionCallback, bool isOwner = true)
		{
			GameObject go = PrefabHelper.Instantiate(_dialogueDisplay) as GameObject;

			DialogueNodeDisplay display = go.GetComponentInChildren<DialogueNodeDisplay>();
			_nodeDisplay = display;

			display._assetManager = AssetManager;
            display._avatarResourceManager = AvatarResourceManager;
			display.SetConfig(_config);
			
			List<string> warnings = null;
			yield return StartCoroutine(display.Display(node, (x => warnings = x)));
			
			if (isOwner)
			{
				ClearScreen();
				go.transform.SetParent(gameObject.transform, false);
				DisplayWarnings(warnings);
			}
			else
			{
                // Not the owner so we are not a new screen, so don't animate the text and also turn off the arrow.
                display.SetTextAnimate(false);
			}
			
			// HACK -- needed to make the TMPro text display properly
			display.gameObject.SetActive(false);
			display.gameObject.SetActive(true);

			var processHandler = CreateProcessHandler(actionCallback, display);
			display.SetInputHandler(processHandler);


            if (readyCallback != null)
            {
                readyCallback(go, warnings);
            }
        }


		private Func<int,bool> CreateProcessHandler(Func<int,bool> processHandler, DialogueNodeDisplay display)
		{
			if(processHandler == null)
			{
				return null;
			}

			return delegate(int i)
			{
//				display.SetInputHandler(null);				// is this necessary outside the story player context?
				var processed = processHandler(i);
				if(!processed)
				{
					display.SetInputHandler(CreateProcessHandler(processHandler,display));
				}

				return processed;
			};
		}

		private Func<int,bool> CreateProcessHandler(Func<int,bool> processHandler, SelectionDisplay display)
		{
			return delegate(int i)
			{
				var processed = processHandler(i);
				if(processed)
				{
					display.SetInputHandler(null);
				}
				
				return processed;
			};
		}

		public void DisplaySelection(SelectionNodeViewData node, Action<List<string>> readyCallback = null, Func<int,bool> actionCallback = null)
		{
			Action<GameObject, List<string>> wrappedReady = delegate(GameObject ignored, List<string> warnings)
			{
				if (readyCallback != null)
				{
					readyCallback(warnings);
				}
			};
			StartCoroutine(DisplaySelectionRoutine(node, wrappedReady, actionCallback));
		}
		
		public IEnumerator DisplaySelectionRoutine(SelectionNodeViewData node, Action<GameObject, List<string>> readyCallback, Func<int,bool> actionCallback)
		{
			List<string> warnings = new List<string>();

			GameObject dialogueGO = null;
			yield return StartCoroutine(DisplayDialogueRoutine(node.DialogueNode, ((go, x) => { dialogueGO = go; warnings.AddRange(x); }), null, false));
			
			GameObject selectionGO = PrefabHelper.Instantiate(_selectionDisplay) as GameObject;
			SelectionDisplay display = selectionGO.GetComponentInChildren<SelectionDisplay>();
			yield return StartCoroutine(display.Display(node, (y => warnings.AddRange(y))));

			ClearScreen();
			dialogueGO.transform.SetParent(selectionGO.transform, false);
			dialogueGO.transform.SetSiblingIndex(0);
			selectionGO.transform.SetParent(gameObject.transform, false);
			DisplayWarnings(warnings);
			
			// HACK -- needed to make the TMPRO text display properly
			gameObject.SetActive(false);
			gameObject.SetActive(true);


			display.SetInputHandler(actionCallback);

			if (readyCallback != null)
			{
				readyCallback(selectionGO, warnings);
			}
		}
		
		private void ClearScreen()
		{
			GameObjectUtils.RemoveChildren(gameObject);
		}
		
		private void DisplayWarnings(List<string> warnings)
		{
			if (_errorDisplay != null)
			{
				warnings.RemoveAll(x => String.IsNullOrEmpty(x));
				
				if (warnings.Count > 0)
				{
					string errorText = String.Join(" ", warnings.ToArray());
					System.Console.WriteLine(errorText);
					Debug.Log(errorText);
					_errorDisplay.SetText(errorText);
					_errorDisplay.gameObject.SetActive(true);
				}
				else
				{
					_errorDisplay.gameObject.SetActive(false);
				}
			}
		}
		
		

		
		public void ResetDisplay()
		{
			Transform dialogue = this.transform.Find("dialogue");
			if (dialogue)
			{
				GameObject.Destroy(dialogue.gameObject);
			}
		}
		


		public void MakePassive(bool value)
		{
			// HACK: intended to make StoryPlayer passive (to account for selection buttons), but currently will find ANY button
			var buttons = gameObject.GetComponentsInChildren<Button> (true);
			foreach (Button button in buttons) 
			{
				button.enabled = !value;
			}
		}
		
	}
	
}
