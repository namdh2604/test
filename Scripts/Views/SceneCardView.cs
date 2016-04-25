using UnityEngine;
using iGUI;
using System.Collections;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
    using Voltage.Story.StoryDivisions;
	using Debug = UnityEngine.Debug;

	using Voltage.Common.DebugTool.Timer;

	public class SceneCardView : MonoBehaviour 
	{
		[HideInInspector]
		public iGUIContainer heart_lock_grp;

		[HideInInspector]
		public iGUILabel text,scenetitle,button_label,bits_label;

		[HideInInspector]
		public iGUIImage different_direction_description,polaroid_MA,polaroid_bgs_MA,big_lock,cleared_badge;

		[HideInInspector]
		public iGUIButton btn_scene, different_direction_icon;

		public event GUIEventHandler OnSceneSelect;
		public event GUIEventHandler OnSceneButtonPress;
		public event GUIEventHandler OnCondition2ButtonPress;

        private SceneViewModel _myScene;

        private const string CLEARED_TEXT = "CLEARED";
        private const string LOCKED_TEXT = "LOCKED";
        private const string PLAY_TEXT = "READ";

		private static Color32 ENABLED_COLOUR = Color.white;	// new Color32 (255, 255, 255, 255)
		private static Color32 DISABLED_COLOUR = new Color32 (64, 64, 64, 210);

        public void SetUpSceneCard(SceneViewModel myScene)
		{
			HideAll();
			_myScene = myScene;

            text.label.text = _myScene.Description;
            scenetitle.label.text = _myScene.Name;

			SetUpLocks();
			ShowSceneImage();
			UpdateBitsLabel();
            InitStatus();
		}

		void HideAll()
		{
			heart_lock_grp.setEnabled(false);
		}

		protected void Start()
		{
			btn_scene.clickDownCallback += ClickInit;
			different_direction_icon.clickDownCallback += ClickInit;
		}

		void SetUpLocks()
		{
            if((_myScene.LockStatus & LockType.Favorability) == LockType.Favorability)
            {
                big_lock.setEnabled(true);
            }

            if((_myScene.LockStatus & LockType.Progress) == LockType.Progress)
            {
				big_lock.setEnabled(true);
            }

			if((_myScene.LockStatus & LockType.Clothing) == LockType.Clothing)
			{
				big_lock.setEnabled(true);
			}

			//TODO Display branch lock group
		}

		void ShowSceneImage()
		{
			if(!string.IsNullOrEmpty(_myScene.PolaroidPath))
			{
				polaroid_bgs_MA.image = Resources.Load<Texture>(_myScene.PolaroidPath);
			}
		}

		void UpdateBitsLabel()
		{
			var completed = _myScene.BitProgress;
			var bitstext = bits_label.label.text;

			bitstext = string.Format ("{0}/5 Bits", completed);
			bits_label.label.text = bitstext;
		}

		Color HexToColor(string hex)
		{
			byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
			return new Color32(r,g,b, 255);
		}

		public void EnableButtons()
		{
			btn_scene.clickDownCallback += ClickInit;
			different_direction_icon.clickDownCallback += ClickInit;
		}

		public void DisableButtons()
		{
			btn_scene.clickDownCallback -= ClickInit;
			different_direction_icon.clickDownCallback -= ClickInit;
		}

        private void InitStatus()
        {
			string statusText;
            if (_myScene.Completed)
            {
                statusText = CLEARED_TEXT;
                EnableButton(false);
            }
            else if (_myScene.LockStatus != LockType.None)
            {
                statusText = LOCKED_TEXT;
                EnableButton(true);
                btn_scene.passive = false;
            }
            else
            {
                statusText = PLAY_TEXT;
                EnableButton(true);
            }

            button_label.label.text = statusText;
            
			if(statusText == PLAY_TEXT)
			{
				btn_scene.setColor(ENABLED_COLOUR);
				button_label.setColor(ENABLED_COLOUR);
           		button_label.outlineColor = HexToColor("BF5FFF");
			}
			else if(statusText == CLEARED_TEXT)
			{
//				var image = AddMultiplyLayer(btn_scene,btn_scene.style.normal.background);
//				button_label.setLayer(image.layer + 1);
				btn_scene.setColor(DISABLED_COLOUR);			// setting button colour directly instead relying on an overlay (especially since use of MultiplyLayers is buggy, see below)
				button_label.setColor(DISABLED_COLOUR);

				button_label.setOutlineThickness(0.03f);
				var outlineColor = new Color32((byte)175,(byte)175,(byte)175,(byte)255);
				button_label.outlineColor = (Color)outlineColor;
			}

            cleared_badge.setEnabled(_myScene.Completed);
        }




		// deprecated: scene card generation/cloning not properly setup to account for prior MultiplyLayers  
		iGUIImage AddMultiplyLayer(iGUIElement element,Texture2D texture)
		{
			var parent = element.getTargetContainer () as iGUIContainer;
			var image = parent.addElement<iGUIImage> ("Multiply_Layer", element.positionAndSize);
			image.setVariableName ("Multiply_Layer");
			image.name = "Multiply_Layer";
			image.elementAspectRatio = element.elementAspectRatio;
			image.image = texture;
			image.passive = true;
			var newColor = new Color32 ((byte)64, (byte)64, (byte)64, (byte)255);
			image.setColor ((Color)newColor);
			image.setOpacity (0.45f);
			image.setLayer (element.layer + 1);

			return image;
		}

        private void EnableButton(bool enabled)
        {
            btn_scene.passive = !enabled;
            btn_scene.opacity = (enabled) ? 1.0f : 2.0f; // HACK -- iGUI claims this is the way to get passive without the gray
        }

		void ClickInit(iGUIElement caller)
		{
			if(caller == btn_scene)
			{
				if(OnSceneButtonPress != null)
				{
					OnSceneButtonPress(this, new GUIEventArgs());
				}
			}
			else if(caller == different_direction_icon)
			{
				if(OnCondition2ButtonPress != null)
				{
					OnCondition2ButtonPress(this, new GUIEventArgs());
				}
			}
		}

		public void ExecuteSceneButtonClick()
		{
			if(OnSceneSelect != null)
			{
				OnSceneSelect(this, new SceneSelectedEventArgs(_myScene));
			}
		}

		public void ExecuteConditionButtonPress(int condition)
		{
			if(condition == 2)
			{
				StartCoroutine(DisplayDialogAndClose(different_direction_description));
			}
			else
			{
				//
			}
		}

		IEnumerator DisplayDialogAndClose(iGUIElement element)
		{
			element.setEnabled(true);
			yield return new WaitForSeconds(2.0f);
			element.setEnabled(false);
		}
	}
}