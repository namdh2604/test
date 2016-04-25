using iGUI;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Voltage.Witches.Events;
using Voltage.Witches.Models;
using Voltage.Witches.Exceptions;
using Voltage.Witches.Tutorial;


namespace Voltage.Witches.Screens
{
	using SceneHeader = Voltage.Story.StoryDivisions.SceneHeader;
	using StoryScene = Voltage.Story.StoryDivisions.Scene;

	public class ChapterClearDialog: AbstractDialog
	{
		[HideInInspector]
		public iGUIButton btn_galaxy_long,btn_popup_close;

		[HideInInspector]
		public iGUILabel game_effect_counter_1, game_effect_counter_2,
						 main_header_chapter_name;

		[HideInInspector]
		public iGUIButton Check_Mail_Button;


		[HideInInspector]
		public iGUIImage polaroid_bgs_MA;
		public iGUIImage pointerImage;

		[HideInInspector]
		public iGUIImage favorability_icon_1, favorability_icon_2, favorability_icon_3, 
						favorability_icon_4, favorability_icon_5;

		[HideInInspector]
		public iGUIContainer chapter_complete, pointerRoot;
		
		public GUIEventHandler CloseChapterClear;	// likely DEPRECATED! 
		public GUIEventHandler ContinueToNext;		// likely DEPRECATED! 

		Player _player;
		SceneHeader _header;
		StoryScene _storyScene;

		private const float RATIO_FOR_SECOND_ICON = 0.28f;
		private const float RATIO_FOR_ICON_Y = 0.14f;
		private const float RATIO_FOR_TEXT2_X = 0.26f;

		private Dictionary<string,iGUIImage> _iconMap;

		Dictionary<iGUIButton,iGUIElement> _buttonArtMap;

		IGUIHandler _buttonHandler;

		public void SetParameters(Player player)
		{
			_player = player;
		}

		public void SetHeader(SceneHeader header)
		{
			_header = header;
		}


		protected void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
		}

		protected void Start()
		{
			btn_galaxy_long.clickDownCallback += ClickInit;
			btn_popup_close.clickDownCallback += ClickInit;
			Check_Mail_Button.clickDownCallback += ClickInit;
			_buttonArtMap = new Dictionary<iGUIButton, iGUIElement> ()
			{
				{btn_galaxy_long,btn_galaxy_long.getTargetContainer()},
				{btn_popup_close,btn_popup_close},
				{Check_Mail_Button,Check_Mail_Button}
			};

			SetUpIconMaps();

			DisplayAffinityChanges();
			DisplaySceneImage();
			DisplayChapterName();
		}

		private void SetUpIconMaps()
		{
			_iconMap = new Dictionary<string,iGUIImage> ()
			{
				{"N",favorability_icon_1},
				{"M",favorability_icon_2},
				{"R",favorability_icon_3},
				{"A",favorability_icon_4},
				{"T",favorability_icon_5}
			};
		}

		private void DisplayAffinityChanges()
		{
			Dictionary<string,int> characters = _player.GetCurrentAffectedCharacters();
			var characterInitials = characters.Keys.ToList ();
			bool isFirstIconSet = false;
			for (int i = 0; i < characters.Count(); i++) 
			{
				if (_iconMap.ContainsKey(characterInitials[i]))
				{
					if (!isFirstIconSet)
					{
						_iconMap[characterInitials[i]].setEnabled(true);
						isFirstIconSet = true;
						game_effect_counter_1.setEnabled(true);
						game_effect_counter_1.label.text = "+" + characters[characterInitials[i]].ToString();
					}
					else
					{
						_iconMap[characterInitials[i]].setEnabled(true);
						Rect secondIconOrgRect = _iconMap[characterInitials[i]].positionAndSize;
						Rect parentRect = chapter_complete.rect;

						Rect secondIconRect = new Rect((parentRect.width*RATIO_FOR_SECOND_ICON),  secondIconOrgRect.y, secondIconOrgRect.width, secondIconOrgRect.height);
						_iconMap[characterInitials[i]].setPositionAndSize(secondIconRect);
						game_effect_counter_2.setEnabled(true);
						game_effect_counter_2.label.text = "+" + characters[characterInitials[i]].ToString();
						AdjustTextPositions(game_effect_counter_2);
					}
					AdjustIconsPositions(_iconMap[characterInitials[i]]);

				}

			}
		}

		private void AdjustIconsPositions(iGUIImage icon)
		{
			Rect iconsOrgRect = icon.positionAndSize;
			Rect parentRect = chapter_complete.rect;
			
			Rect newRect = new Rect (iconsOrgRect.x, parentRect.height*RATIO_FOR_ICON_Y, iconsOrgRect.width, iconsOrgRect.height);
			icon.setPositionAndSize (newRect);
		}

		private void AdjustTextPositions(iGUILabel text)
		{
			Rect textOrgRect = text.positionAndSize;
			Rect parentRect = chapter_complete.rect;
			
			Rect newRect = new Rect (parentRect.width*RATIO_FOR_TEXT2_X, textOrgRect.height, textOrgRect.width, textOrgRect.height);
			text.setPositionAndSize (newRect);
		}


		private void DisplaySceneImage()
		{
			if(!string.IsNullOrEmpty(_header.PolaroidPath))
			{
				polaroid_bgs_MA.image = Resources.Load<Texture>(_header.PolaroidPath);
			}
		}

		private void DisplayChapterName()
		{
			var header = main_header_chapter_name;
			header.style.alignment = TextAnchor.MiddleCenter;
			header.setDynamicFontSize(0.6f);

			var label = header.label.text;
			label = label.Replace("CHAPTER NAME GOES HERE",_header.Scene.ToUpper());
			header.label.text = label;
		}

		void ClickInit(iGUIElement element)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
			{
				var button = (iGUIButton)element;
				_buttonHandler.SelectButton(button);
				_buttonArtMap[button].colorTo(Color.grey,0f);
			}
		}
		
		void HandleMovedAway(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}
		
		void HandleMovedBack(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.grey,0f);
		}
		
		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if(isOverButton)
			{
				if(button == btn_popup_close)
				{
					if(CloseChapterClear != null)
					{
						CloseChapterClear(this,new GUIEventArgs());
					}
					SubmitResponse((int)DialogResponse.Cancel);
				}
				else if(button == btn_galaxy_long)
				{
					// likely DEPRECATED! 
					if(ContinueToNext != null)
					{
						ContinueToNext(this, new GUIEventArgs());
					}

					SubmitResponse((int)DialogResponse.OK);
				}
				else if(button == Check_Mail_Button)
				{
					SubmitResponse((int)DialogResponse.Mail);
				}
			}

            if (this != null)
            {
    			_buttonArtMap[button].colorTo(Color.white,0.3f);
            }
		}

		public void DisableInputs()
		{
			_buttonHandler.Deactivate ();
		}

		public void EnableCheckMailButton(bool value)
		{
			Check_Mail_Button.setEnabled (value);
		}

		// HACK: mail button doesn't tie into the button handler, so creating separate state to handle it
		public static bool MailButtonPassive = false;

		public void ActivateCheckMailButton()
		{
			iGUIElement checkMailButtonElement = Check_Mail_Button.GetComponent<iGUIElement> ();
			if (Check_Mail_Button != null) 
			{
				_buttonArtMap = new Dictionary<iGUIButton, iGUIElement> ()
				{
					{Check_Mail_Button, checkMailButtonElement}
				};
				Check_Mail_Button.clickDownCallback += HandleMailClickMovedBacck;
				Check_Mail_Button.mouseOverCallback += HandleMailClickMovedBacck;
				Check_Mail_Button.mouseOutCallback += HandleMailClickMovedAway;
				Check_Mail_Button.clickUpCallback += HandleMailClickUpButtonEvent;
			} else 
			{
				throw new WitchesException("failed to activate " + Check_Mail_Button.variableName);
			}
		}
		private void HandleMailClickMovedBacck(iGUIElement btn)
		{
			if (!MailButtonPassive) 
			{
				HandleMovedBack (btn as iGUIButton);
			}
		}
		
		private void HandleMailClickMovedAway(iGUIElement btn)
		{
			if (!MailButtonPassive) 
			{
				HandleMovedAway (btn as iGUIButton);
			}
		}
		
		private void HandleMailClickUpButtonEvent(iGUIElement btn)
		{
			if (!MailButtonPassive) 
			{
				SubmitResponse ((int)DialogResponse.Mail);

                if (this != null)
                {
    				_buttonArtMap [btn as iGUIButton].colorTo (Color.white, 0.3f);
                }
			}
		}

		
		[Range(0f, 360f)]
		public float degrees = 22f;
		
		[Range(0.00f, 1.00f)]
		public float amplitude = 0.05f;
		public float frequency = 0.66f;	// cycle per second
		
		private IEnumerator _rotatePointerRoutine;	
		private IEnumerator _translatePointerRoutine;


		public void ShowPointer (Vector2 position, bool animate=false)
		{
			StopAnimationRoutine ();
			
			pointerRoot.setPosition (position);
			pointerRoot.setEnabled (true);
			
			if(animate)
			{
				_translatePointerRoutine = TranslatePointerRoutine();
				StartCoroutine(_translatePointerRoutine);					
				
				_rotatePointerRoutine = RotatePointerRoutine();
				StartCoroutine(_rotatePointerRoutine);
			}
		}
		
		private IEnumerator TranslatePointerRoutine()
		{
			float originalY = pointerRoot.positionAndSize.y;
			
			while(true)
			{
				Vector2 newPos = pointerRoot.positionAndSize.position;
				newPos.y = originalY + (Mathf.Sin(2f * Mathf.PI * Time.time * frequency) * amplitude);
				
				pointerRoot.setPosition(newPos);
				
				yield return null;
			}
		}
		
		private IEnumerator RotatePointerRoutine()
		{
			while(true)
			{
				float angle = Mathf.Sin(2f * Mathf.PI * Time.time * frequency) * (degrees/2f);
				
				pointerImage.setRotation(angle);
				
				yield return null;
			}
		}

		private void StopAnimationRoutine()
		{
			if(_rotatePointerRoutine != null)
			{
				StopCoroutine(_rotatePointerRoutine);
				_rotatePointerRoutine = null;
			}
			
			if(_translatePointerRoutine != null)
			{
				StopCoroutine(_translatePointerRoutine);
				_translatePointerRoutine = null;
			}
		}

        // FIXME -- Move this into Abstract Dialog when we are comfortable incorporating this change (and the null check it necessitates)
        // into all iGUI dialogs
        protected override void SubmitResponse(int response, bool autoClose=true)
        {
            base.SubmitResponse(response, false);

            if (autoClose)
            {
                (screenFrame.container as iGUIContainer).removeElement(screenFrame);
            }
        }
	}
}