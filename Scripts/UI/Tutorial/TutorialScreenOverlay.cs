using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Tutorial.uGUI
{
	using Voltage.Common.Logging;

	using UnityEngine.UI;
//	using TMPro;

	using Voltage.Witches.Screens;

	using Voltage.Witches.UI;
	using Voltage.Common.UI;

	// eventually replace
	using Voltage.Witches.StoryMap;		
	using Voltage.Witches.Screens.Closet;
	using Voltage.Witches.Screens.AvatarShop;



	public class TutorialScreenOverlay : MonoBehaviour //BaseUGUIScreen  // StoryMapUGUIScreen 
    {
		[SerializeField]
		private Button _clickTrigger;
		public event Action OnClickedTrigger;

		[SerializeField]
		private CharacterNarrator _narrator;

		[SerializeField]
		private CharacterNarrator _narratorB;		// HACK/TODO: build up CharacterNarrator to customize dialogue position (left/right) and different character images


		[SerializeField]
		private HandPointer _pointer;


		[SerializeField]						// TODO: maybe use a list of marks ?
		private RectTransform _rightMark;		// TODO: maybe add mark for outside camera ?
		[SerializeField]
		private RectTransform _leftMark;
		[SerializeField]
		private RectTransform _leftBottomMark;

		[SerializeField]
		private Camera _tutorialCam;


		private Camera _mainCam;
		private SpotlightEffect _spotLightController;


		public void MakePassive(bool value)
		{
			EnableClickTrigger (!value);
		}

		public bool InputEnabled
		{
			get 
			{
				return _clickTrigger.isActiveAndEnabled;
			}
		}

		public void EnableClickTrigger (bool enable)
		{
			_clickTrigger.gameObject.SetActive (enable);
		}

		public void OnTrigger()
		{
			if(OnClickedTrigger != null)
			{
				OnClickedTrigger();
			}
		}



		private void Awake()
		{
			if(_narrator == null || _pointer == null || _clickTrigger == null || _rightMark == null || _leftMark == null || _leftBottomMark == null || _narratorB == null)
			{
				throw new NullReferenceException();
			}

			// FIXME: risk of other similarly named objects or if name changes...or if object is disabled...also not performant!
			_mainCam = GameObject.Find ("Main Camera").GetComponent<Camera> ();			// _cam = this.GetComponentInParent<Canvas> ().worldCamera;		
			_spotLightController = _mainCam.gameObject.AddComponent<SpotlightEffect> ();

			if(_mainCam == null || _spotLightController == null || _tutorialCam == null)
			{
				throw new NullReferenceException();
			}

			AlignCameras ();		

			DisableSpotlight ();		// maybe put all this in Init
			EnableClickTrigger (false);
			HidePointer ();
			HideDialogue ();
			HideNarrator ();
		}
			
		private void AlignCameras()		// FIXME: currently relies on main and tutorial cameras being the same size and position, until I add contextual world-to-viewport translation
		{
			_tutorialCam.orthographicSize = _mainCam.orthographicSize;		
		}


		private void OnDestroy()
		{
			if(_spotLightController != null)
			{
				DisableSpotlight();
				Destroy(_spotLightController);
			}
		}




	
		public void PointAtRibbon()
		{
//			GameObject ribbonArrow = GameObject.Find ("arrow_down");	// FIXME: risk of other similarly named objects or if name changes...or if object is disabled...also not performant!
			Vector2 normalizedPos = new Vector2 (0.1f, 0.9f);
			_pointer.ShowPointer (normalizedPos, true);
		}









       






		public SceneCardView DuplicateSceneCard (SceneCardView sceneCard)
		{
			GameObject duplicate = Duplicate(sceneCard.gameObject);

			// NOTE: the scrollview is scaled down to account for the larger scenecard, so need to apply the same scaling
			duplicate.transform.localScale = sceneCard.GetComponentInParent<ScrollRect> ().transform.localScale;			// duplicate.transform.localScale = Vector3.one;

			return duplicate.GetComponent<SceneCardView> ();
		}



        public void PointAtReadButton(SceneCardView sceneCard, float offsetX=0f, float offsetY=0f)	
		{
            Button button = sceneCard.GetComponentInChildren<Button>(true);

            PointAtTarget(button.gameObject, offsetX, offsetY);
		}


		public void HidePointer()
		{
			_pointer.HidePointer ();
		}




		private Rect GetCanvasDimensions()	// could cache
		{
			return this.GetComponentInParent<Canvas> ().GetComponent<RectTransform> ().rect;	// can throw exception
		}



		public enum NarratorSlideOrigin
		{
			SCREEN_RIGHT,
			SCREEN_LEFT
		}


		[SerializeField]
		private float _narratorSlideDurationInSec = 1.5f;
		[SerializeField]
		private iTween.EaseType _slideEaseInType = iTween.EaseType.easeOutCubic;
		[SerializeField]
		private iTween.EaseType _slideEaseOutType = iTween.EaseType.easeOutCubic;

		// FIXME: cleanup this implementation, also doesn't scale with potential "marks"
		public IEnumerator SlideInNarrator(NarratorSlideOrigin origin=NarratorSlideOrigin.SCREEN_RIGHT)
		{
			Vector2 outsidePos = GetOutsideAnchoredPosition(origin);
			_narrator.SetNarratorPosition (outsidePos);

			_narrator.ShowNarrator (true);
			_narrator.ShowDialogueBox (false);

			Vector3 targetPos = _rightMark.transform.position;
			if (origin == NarratorSlideOrigin.SCREEN_LEFT) 
			{
				targetPos = _leftMark.transform.position;
			}

			object[] args = new object[]
			{
				"from", _narrator.transform.position,
				"to", targetPos,
				"time", _narratorSlideDurationInSec,
				"easetype", _slideEaseInType,
				"onupdate", (Action<object>)(value => _narrator.transform.position = (Vector3)value),
			};

			iTween.ValueTo (_narrator.gameObject, iTween.Hash(args));			// iTween.MoveTo (_narrator.gameObject, targetPos, _narratorSlideDurationInSec);
			yield return new WaitForSeconds (_narratorSlideDurationInSec);
		}

		// currently only handles sliding out to to screen right
		public IEnumerator SlideOutNarrator()
		{
			Vector2 outsidePos = GetOutsideAnchoredPosition(NarratorSlideOrigin.SCREEN_RIGHT); // GetOutsideRightAnchoredPosition ();

			object[] args = new object[]
			{
				"from", _narrator.CurrentAnchoredPosition,
				"to", outsidePos,
				"time", _narratorSlideDurationInSec,
				"easetype", _slideEaseOutType,
				"onupdate", (Action<object>)(value => _narrator.SetNarratorPosition((Vector2)value)),
			};
			
			iTween.ValueTo (_narrator.gameObject, iTween.Hash(args));
			yield return new WaitForSeconds (_narratorSlideDurationInSec);
		}
			

		// maybe should just use a "mark" for this in the scene
		private Vector2 GetOutsideAnchoredPosition(NarratorSlideOrigin origin)
		{
			float narratorWidth = _narrator.NarratorWidth;

			Rect canvasDimensions = GetCanvasDimensions ();
			float canvasWidth = canvasDimensions.width;
			float canvasHeight = canvasDimensions.height;

			switch (origin) 
			{
				case NarratorSlideOrigin.SCREEN_LEFT:
					Vector2 normalizedLeftMark = TranslateWorldPosToViewPos (_leftMark.transform.position, _mainCam);
					return new Vector2(-narratorWidth / 2f, normalizedLeftMark.y * canvasHeight);

				case NarratorSlideOrigin.SCREEN_RIGHT:
				default:
					Vector2 normalizedRightMark = TranslateWorldPosToViewPos (_rightMark.transform.position, _mainCam);	// _mainCam.WorldToViewportPoint (_rightMark.transform.position);
					return new Vector2 (canvasWidth + (narratorWidth / 2f), normalizedRightMark.y * canvasHeight);
			}
		}




		// i imagine this implementation will change if further images are needed or other characters are used
		[SerializeField]
		private Sprite _rhysBustShot;
		[SerializeField]
		private Sprite _rhysFullBodyPocket;
		[SerializeField]
		private Sprite _rhysFullBodyPonder;

		public enum NarratorImage
		{
			BUST,
			FULL_POCKET,
			FULL_PONDER
		}

		// i imagine this implementation will change if further images are needed or other characters are used
		public void SetNarratorImage(NarratorImage image)
		{	
			// TODO: need to handle resizing
			switch(image) 
			{
				case NarratorImage.BUST:
					_narrator.SetImage(_rhysBustShot);
					break;
				case NarratorImage.FULL_PONDER:
					_narrator.SetImage(_rhysFullBodyPonder);
					break;
				case NarratorImage.FULL_POCKET:
					_narrator.SetImage(_rhysFullBodyPocket);
					break;
				default:
					throw new ArgumentNullException ();
			}
		}


		public void SetNarratorDirection(CharacterNarrator.FacingDirection dir)
		{
			_narrator.SetDirection(dir);
		}



		// HACK: create call/setup to support any narrator or mark
		public IEnumerator FadeInNarratorB()		
		{
			return FadeInNarrator (_narratorB, _leftBottomMark.transform.position);
		}

		// HACK: create call/setup to support any narrator or mark
		public IEnumerator FadeOutNarratorB()		
		{
			return FadeOutNarrator (_narratorB, _narratorFadeOutDurationInSec);
		}

		[SerializeField]
		private float _narratorFadeInDurationInSec = 0.5f;
		[SerializeField]
		private float _narratorFadeOutDurationInSec = 0.5f;

		public IEnumerator FadeInNarrator()			// FIXME: support any narrators and marks
		{
			return FadeInNarrator (_narrator, _rightMark.transform.position);
		}
	
		public IEnumerator FadeOutNarrator()	// FIXME: support any narrators and marks
		{
			return FadeOutNarrator(_narrator, _narratorFadeOutDurationInSec);
		}

		// show immediatly, maybe not necessary
		public void ShowNarrator()				// FIXME: support any narrators and marks
		{
			_narrator.ShowNarrator (true);
		}
		// hide immediatly, maybe not necessary
		public void HideNarrator()				// FIXME: support any narrators and marks
		{
			_narrator.ShowNarrator (false);
			_narratorB.ShowNarrator (false);
		}



		private IEnumerator FadeInNarrator(CharacterNarrator narrator, Vector3 worldPos)
		{
			Rect canvasDimension = GetCanvasDimensions ();
			Vector2 normalizedPosition = TranslateWorldPosToViewPos (worldPos, _mainCam);	// _mainCam.WorldToViewportPoint (worldPos);	
			Vector2 markPosition = new Vector2 (normalizedPosition.x * canvasDimension.width, normalizedPosition.y * canvasDimension.height);
			
			narrator.SetNarratorPosition (markPosition);
			narrator.ShowNarrator (true, _narratorFadeInDurationInSec);
			yield return new WaitForSeconds (_narratorFadeInDurationInSec);
		}

		private IEnumerator FadeOutNarrator(CharacterNarrator narrator, float overDurationInSec)
		{
			narrator.ShowNarrator (false, overDurationInSec);
			yield return new WaitForSeconds (overDurationInSec);
		}






		[SerializeField]
		public float _dialogueFadeDurationInSec = 0.5f;

		public IEnumerator ShowDialogueWithFade(string text)		// FIXME: support all narrators (tho technically should only be a SINGLE narrator)
		{
			return ShowDialogueWithFade (text, _narrator);
		}
			
		public IEnumerator FadeOutDialogue()						// FIXME: support all narrators (tho technically should only be a SINGLE narrator)
		{
			return FadeOutDialogue (_narrator);
		}

		public void ShowDialogue(string text)						// FIXME: support all narrators (tho technically should only be a SINGLE narrator)
		{
			_narrator.SetText (text);
			_narrator.ShowDialogueBox (true);
		}

		public void HideDialogue()									// FIXME: support all narrators (tho technically should only be a SINGLE narrator)
		{
			_narrator.ShowDialogueBox (false);
			_narratorB.ShowDialogueBox (false);						
		}

		public IEnumerator FadeOutNarratorAndDialogue()				// FIXME: support all narrators (tho technically should only be a SINGLE narrator)
		{
			return FadeOutNarratorAndDialogue (_narrator);
		}


		// HACK: create call/setup to support all narrators
		public IEnumerator ShowNarratorBDialogueWithFade(string text)
		{
			return ShowDialogueWithFade (text, _narratorB);
		}
		// HACK: create call/setup to support all narrators
		public IEnumerator FadeOutNarratorBAndDialogue()				// FIXME: support all narrators (tho technically should only be a SINGLE narrator)
		{
			return FadeOutNarratorAndDialogue (_narratorB);
		}



		private IEnumerator ShowDialogueWithFade(string text, CharacterNarrator narrator)
		{
			if(!narrator.DialogueVisible)
			{
				narrator.ShowDialogueBox(true, _dialogueFadeDurationInSec);
				yield return new WaitForSeconds(_dialogueFadeDurationInSec / 2f);
			}
			
			float textFadeDuration = _dialogueFadeDurationInSec / 2f;
			
			narrator.SetText(text, textFadeDuration);
			yield return new WaitForSeconds (textFadeDuration);
		}

		public IEnumerator FadeOutDialogue(CharacterNarrator narrator)						
		{
			narrator.ShowDialogueBox (false, _dialogueFadeDurationInSec);
			yield return new WaitForSeconds (_dialogueFadeDurationInSec);
		}

		private IEnumerator FadeOutNarratorAndDialogue (CharacterNarrator narrator)
		{
			float duration = (_narratorFadeOutDurationInSec > _dialogueFadeDurationInSec ? _narratorFadeOutDurationInSec : _dialogueFadeDurationInSec);
			
			narrator.ShowNarrator (false, _narratorFadeOutDurationInSec);
			narrator.ShowDialogueBox (false, _dialogueFadeDurationInSec);
			
			yield return new WaitForSeconds (duration);
		}









		[SerializeField]
		private float _spotlightRadius = 0.075f;
		[SerializeField]
		private float _spotlightAnimationDurationInSec = 0.75f;
		[SerializeField]
		private iTween.EaseType _spotlightEaseType = iTween.EaseType.easeInCubic;
		public IEnumerator ShowStaminaSpotlight()
		{
			GameObject stamina = GetTargetObject("rib_coffee_cup_empty");		// FIXME: risk of other similarly named objects or if name changes...or if object is disabled...also not performant!

			return SpotlightAt (stamina.transform.position, _spotlightRadius, _spotlightAnimationDurationInSec, _spotlightEaseType);
		}


		[SerializeField]
		private float _infoGrpSpotlightRadius = 0.22f;
		private float _infoGrpSpotlightAnimationDurationInSec = 0.7f;
		public IEnumerator ShowInfoStaminaSpotlight()
		{
			GameObject staminaGrp = GetTargetObject("staminaitem_group");		// FIXME: risk of other similarly named objects or if name changes...or if object is disabled...also not performant!

			if(staminaGrp != null)
			{
				return SpotlightAt (staminaGrp.transform.position, _infoGrpSpotlightRadius, _infoGrpSpotlightAnimationDurationInSec, _spotlightEaseType);
			}
			else
			{
				return null;
			}
		}


		public IEnumerator ShowInfoBookSpotlight ()
		{
			return SpotlightAt (_infoBookPos, _spotlightRadius, _spotlightAnimationDurationInSec, _spotlightEaseType);
		}

	
		private IEnumerator SpotlightAt(Vector3 worldPos, float radius, float durationInSec, iTween.EaseType easeType)
		{
			Vector2 normalizedPos = TranslateWorldPosToViewPos (worldPos, _mainCam);	// _mainCam.WorldToViewportPoint (worldPos);	
			return SpotlightAt (normalizedPos, radius, durationInSec, easeType);
		}

		private Color _maskColour = new Color (0f, 0f, 0f, 0.5f);		// values are from 0.0f - 1.0f
		// eventually breakout to a simple utility class to encapsulate this feature
		private IEnumerator SpotlightAt(Vector2 viewPos, float radius, float durationInSec, iTween.EaseType easeType)
		{
			_spotLightController._center = viewPos;
			_spotLightController._radius = 1f;
			_spotLightController._maskColor = _maskColour;		

			return AnimateSpotlight (_spotLightController._radius, radius, durationInSec, easeType);
		}


		public void EnableFullScreenMask(bool value)
		{
			_spotLightController._radius = 0f;
			_spotLightController._center = new Vector2(0.5f, 0.5f);
			_spotLightController._maskColor = _maskColour;

			_spotLightController.enabled = value;
		}



		[SerializeField]
		private float _spotlightCloseDuration = 0.4f;
		[SerializeField]
		private iTween.EaseType _spotlightCloseEaseType = iTween.EaseType.easeInBack;

		public IEnumerator SpotlightToFullScreenMask()
		{	
			_spotLightController._maskColor = _maskColour;
			return AnimateSpotlight (_spotLightController._radius, 0f, _spotlightCloseDuration, _spotlightCloseEaseType);
		}


		[SerializeField]
		private float _spotlightRevealDuration = 0.75f;
		[SerializeField]
		private iTween.EaseType _spotlightRevealEaseType = iTween.EaseType.easeInBack;
		public IEnumerator SpotlightReveal(bool fromBlack=false)
		{
			if(fromBlack)
			{
				_spotLightController._radius = 0f;
			}

			return AnimateSpotlight (_spotLightController._radius, 1f, _spotlightRevealDuration, _spotlightRevealEaseType);
		}

		private IEnumerator AnimateSpotlight(float startRadius, float endRadius, float durationInSec, iTween.EaseType easeType)
		{
			_spotLightController.enabled = true;
			
			object[] closeArgs = new object[]
			{
				"from", startRadius,		
				"to", endRadius,
				"time", durationInSec,						
				"easetype", easeType,
				"onupdate", (Action<object>)(value => _spotLightController._radius = (float)value),
			};
			
			iTween.ValueTo (gameObject, iTween.Hash(closeArgs));
			yield return new WaitForSeconds (durationInSec);
		}




		public void DisableSpotlight()
		{
			_spotLightController.enabled = false;
		}








			


		public void PointAt(float x, float y)	
		{
			PointAt(new Vector2(x,y));
		}

		public void PointAt(Vector2 pos)
		{
			_pointer.ShowPointer (pos, true);
		}



		public ButtonProxy GetInfoButtonProxy()
		{
			GameObject infoButton = GetTargetObject("InfoButton");			// FIXME: risk of other similarly named objects or if name changes...or if object is disabled...also not performant! if anything screen should pass this name
			Button button = infoButton.GetComponentInChildren<Button>();
			
			ButtonProxy proxy = DuplicateButton (button);

			return proxy;
		}
	
		private readonly Vector2 _infoBookPos = new Vector2 (0.94f, 0.92f);

		public void PointAtInfoButton() // (Button button)
		{
//			Vector2 normalizedPos = TranslateWorldPosToViewPos (button.transform.position);
			Vector2 offset = new Vector2 (0.01f, -0.05f);

			PointAt (_infoBookPos + offset);
		}

		public void PointAtInfoDialogueCloseButton()
		{
			GameObject closeButton = GetTargetObject("btn_popup_close");				// FIXME: risk of other similarly named objects or if name changes...or if object is disabled...also not performant! if anything screen should pass this name
			Vector2 normalizedPos = TranslateWorldPosToViewPos (closeButton.transform.position, _mainCam);	// _tutorialCam.WorldToViewportPoint (closeButton.transform.position);			

			Vector2 offset = new Vector2 (0.01f, -0.05f);

			PointAt (normalizedPos + offset);
		}





        public GameObject DuplicateSceneObject(string name)
        {
            GameObject go = GetTargetObject(name);
            return DuplicateSceneObject(go);
        }


        public GameObject DuplicateSceneObject(GameObject go)
        {
            return Duplicate(go);
        }






		// stop-gap refactor, if we're going to use a single tutorial overlay for every screen need to push more out to the
		// tutorial screen controllers/facades
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////


		// radius units are...
		public IEnumerator HighlightTarget(string name, float radius)
		{
			GameObject target = GetTargetObject (name);
//			if(target != null)
			Vector2 normalizedPos = TranslateWorldPosToViewPos (target.transform.position, _tutorialCam);

			// refactor to use a single call to AnimateSpotlight with start/end radius
			if (FullScreenMaskVisible) 
			{
				return UpdateSpotlightTo (normalizedPos, radius);
			} 
			else 
			{
				return SpotlightAt (normalizedPos, radius, _spotlightAnimationDurationInSec, _spotlightEaseType);
			}
		}

		private bool FullScreenMaskVisible
		{
			get 
			{
				return _spotLightController.isActiveAndEnabled && _spotLightController._radius <= 0f;	// issue with precision?
			}
		}


		public void PointAtTarget(string name, float offsetX=0f, float offsetY=0f)
		{
			GameObject target = GetTargetObject(name);

            PointAtTarget(target, offsetX, offsetY);
		}

        public void PointAtTarget(GameObject target, float offsetX=0f, float offsetY=0f)
        {
            Vector2 normalizedPos = TranslateWorldPosToViewPos (target.transform.position, _tutorialCam);
            Vector2 offset = new Vector2 (offsetX, offsetY);
            PointAt(normalizedPos + offset);
        }

		public ButtonProxy GetButtonProxy(string name)
		{
			GameObject target = GetTargetObject(name);			// FIXME: risk of other similarly named objects or if name changes...or if object is disabled...also not performant! if anything screen should pass this name
			Button button = target.GetComponentInChildren<Button>();

			ButtonProxy proxy = DuplicateButton (button);

			// TODO: center proxy's pivot, instead of using original corner
//			RectTransform rectTransform = proxy.Button.GetComponent<RectTransform> ();
//			rectTransform.anchorMin = new Vector2 (0.5f, 0.5f);
//			rectTransform.anchorMax = new Vector2 (0.5f, 0.5f);
//			rectTransform.pivot = new Vector2 (0.5f, 0.5f);

			return proxy;
		}

		public IEnumerator WaitForClickAnywhere()
		{
			bool clicked = false;

			Action onClick = null;
			onClick = () => { 
				EnableClickTrigger (false);
				OnClickedTrigger -= onClick;
				clicked = true;
			};
			OnClickedTrigger += onClick;

			EnableClickTrigger (true);

			while(!clicked)
			{
				yield return null;
			}
		}

		private IDictionary<string,GameObject> _goCache = new Dictionary<string,GameObject> ();

		private GameObject GetTargetObject(string name)
		{
			GameObject target;
			if (_goCache.TryGetValue (name, out target)) 
			{
				if (target != null)	// the key can exist, but the reference could be null right?
				{
					return target;
				} 
				else 
				{
					_goCache.Remove(name);
				}
			} 
			
			// HACK: need reference to view object for any screen, not performant
			return GameObject.Find (name);	// active objects only
		}

		private T GetTargetObject<T>(Predicate<T> predicate) where T : UnityEngine.Object
		{
			// HACK: need reference to view object for any screen
			T[] foundObjects = GameObject.FindObjectsOfType<T> ();	// active objects only
			return new List<T>(foundObjects).Find(predicate);
		}



		public IEnumerator Pause(float durationInSec)
		{
			yield return new WaitForSeconds(durationInSec);
		}

		public IEnumerator WaitForFrameEnd()
		{
			yield return new WaitForEndOfFrame ();
		}


		private ButtonProxy DuplicateButton(Button button)
		{
			GameObject duplicate = Duplicate (button.gameObject);
			Button duplicateButton = duplicate.GetComponent<Button> ();

			// clearing listener must happen before ButtonProxy is added since it adds its own listeners to the duplicate button
			ClearButtonOnClick (duplicateButton);

			ButtonProxy proxy = duplicate.AddComponent<ButtonProxy> ();

			return proxy;
		}

		private void ClearButtonOnClick(Button button)
		{
			// handle non-persistent listeners
			button.onClick.RemoveAllListeners ();	

			// handle persistent listeners
			int persistentCount = button.onClick.GetPersistentEventCount();
			for (int i = 0; i < persistentCount; i++) 
			{
				button.onClick.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
			}

			//			return button;
		}


		private GameObject Duplicate(GameObject original)
		{
			GameObject duplicate = Instantiate (original, original.transform.position, original.transform.rotation) as GameObject;
			duplicate.transform.SetParent (this.transform);
			duplicate.transform.SetAsLastSibling ();
			_pointer.transform.SetAsLastSibling ();
			_narrator.transform.SetAsLastSibling ();

			duplicate.transform.localScale = Vector3.one;

			UpdateLayer (duplicate.transform);

			RectTransform rectTransform = duplicate.GetComponent<RectTransform> ();
			rectTransform.sizeDelta = original.GetComponent<RectTransform> ().sizeDelta;	

			Vector2 normalizedPosition = TranslateWorldPosToViewPos (original.transform.position, _mainCam);	 // _mainCam.WorldToViewportPoint (original.transform.position);				
			Rect screen = transform.GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect;

//			rectTransform.pivot = new Vector2 (0.5f, 0.5f);
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.zero;
//			rectTransform.anchoredPosition = new Vector2 (normalizedPosition.x * screen.width, normalizedPosition.y * screen.height);
			rectTransform.anchoredPosition3D = new Vector3 (normalizedPosition.x * screen.width, normalizedPosition.y * screen.height, 0f);

			return duplicate;
		}

		private void UpdateLayer(Transform root)
		{
			foreach(RectTransform transform in root.GetComponentsInChildren<RectTransform>(true))
			{
				transform.gameObject.layer = LayerMask.NameToLayer("Tutorial");
			}
		}


		private Vector2 TranslateWorldPosToViewPos (Vector3 worldPos, Camera cam)	// maybe not necessary as its a one liner, BUT when multiple camera support is added this could be more readable
		{
			return cam.WorldToViewportPoint (worldPos);
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////








		private AvatarShopItem _shopItem;

		public void InitShop(string id)
		{
			_shopItem = GetTargetObject<AvatarShopItem> ((shopItem) => shopItem.ID == id);
		}

		// FIXME: complete duplication of closet functionality
		public IEnumerator ShowShopItemSpotlight()
		{
			if (_shopItem != null) 
			{
				Button button = _shopItem.GetComponentInChildren<Button> ();	// pivot of closetItem isn't center, so basing it on the button

				Vector2 normalizedPos = TranslateWorldPosToViewPos (button.transform.position, _tutorialCam); 
				return UpdateSpotlightTo (normalizedPos, _spotlightRadius);
			} 
			else 
			{
				AmbientLogger.Current.Log ("TutorialScreenOverlay >>> No shop item!", LogLevel.ERROR);
				return null;
			}
		}
		public void PointAtShopItem()
		{
			if (_shopItem != null) 
			{
				Button button = _shopItem.GetComponentInChildren<Button> ();	// pivot of closetItem isn't center, so basing it on the button

				Vector2 normalizedPos = TranslateWorldPosToViewPos (button.transform.position, _tutorialCam); 
				Vector2 offset = new Vector2 (0.05f, 0f);
				PointAt (normalizedPos + offset);
			} 
			else 
			{
				AmbientLogger.Current.Log ("TutorialScreenOverlay >>> No shop item!", LogLevel.ERROR);
			}
		}

		public void MakeShopItemPassive(bool value)
		{
			if (_shopItem != null) 
			{
				_shopItem.MakePassive (value);
			} 
			else 
			{
				AmbientLogger.Current.Log ("TutorialScreenOverlay >>> No shop item!", LogLevel.ERROR);
			}
		}


		public void HijackShopItemOnClick(Action customHandler)
		{
			if (_shopItem != null) 
			{
				Button button = _shopItem.GetComponentInChildren<Button>();
				ClearButtonOnClick(button);

				button.onClick.AddListener(() => customHandler());
			}
			else 
			{
				AmbientLogger.Current.Log ("TutorialScreenOverlay >>> No shop item!", LogLevel.ERROR);
			}
		}




		private ClosetItem _closetItem;

		public void InitCloset(string id)
		{
			_closetItem = GetTargetObject<ClosetItem> ((closetItem) => closetItem.ID == id);
		}

		public IEnumerator ShowClosetItemSpotlight()
		{
			if (_closetItem != null) 
			{
				Button button = _closetItem.GetComponentInChildren<Button> ();	// pivot of closetItem isn't center, so basing it on the button

				Vector2 normalizedPos = TranslateWorldPosToViewPos (button.transform.position, _tutorialCam); 
				return UpdateSpotlightTo (normalizedPos, _spotlightRadius);
			} 
			else 
			{
				AmbientLogger.Current.Log ("TutorialScreenOverlay::ShowClosetItemSpotlight >>> No closet item!", LogLevel.ERROR);
				return null;
			}
		}

		private IEnumerator UpdateSpotlightTo(Vector2 pos, float radius)
		{
			_spotLightController._center = pos;
			return AnimateSpotlight (_spotLightController._radius, radius, _spotlightCloseDuration, iTween.EaseType.easeOutBack);
		}


		public void PointAtClosetItem()
		{
			if (_closetItem != null) 
			{
				Button button = _closetItem.GetComponentInChildren<Button> ();	// pivot of closetItem isn't center, so basing it on the button

				Vector2 normalizedPos = TranslateWorldPosToViewPos (button.transform.position, _tutorialCam); 
				Vector2 offset = new Vector2 (0.05f, 0f);
				PointAt (normalizedPos + offset);
			} 
			else 
			{
				AmbientLogger.Current.Log ("TutorialScreenOverlay >>> No closet item!", LogLevel.ERROR);
			}
		}

		public void MakeClosetItemPassive(bool value)
		{
			if (_closetItem != null) 
			{
				_closetItem.MakePassive (value);
			} 
			else 
			{
				AmbientLogger.Current.Log ("TutorialScreenOverlay >>> No closet item!", LogLevel.ERROR);
			}
		}

	





    }

}





//		private enum OutsideScreen
//		{
//			LEFT,
//			RIGHT,
//			TOP,
//			BOTTOM,
//			// support? BOTTOMLEFT, BOTTOMRIGHT, TOPLEFT, TOPRIGHT, RIGHTTOP, RIGHTBOTTOM, LEFTTOP, LEFTBOTTOM
//		}





