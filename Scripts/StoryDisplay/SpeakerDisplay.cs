using UnityEngine;
using UnityEngine.UI;

using Voltage.Witches.Utilities;
using Voltage.Common.Utilities;
using Voltage.Witches.AssetManagement;
//using TMPro;

using Voltage.Story.Layout;

namespace Voltage.Witches.Layout
{
	[AddComponentMenu("StoryTool/SpeakerDisplay")]
	public class SpeakerDisplay : MonoBehaviour
	{
		[SerializeField, HideInInspector]
		private string _character, _pose, _outfit, _expression;

		public string Character { get { return _character; } }
		public string Pose { get { return _pose; } }
		public string Outfit { get { return _outfit; } }
		public string Expression { get { return _expression; } }

		public RectTransform _speakerBounds;

		public CharacterLayoutData _data;
//		public TextMeshProUGUI _text;

//		public float FontSize { get { return _text.fontSize; } }

		public void SetSpeakerInfo(string character, string pose, string outfit, string expression, ICharacterBundleManager manager)
		{
			_character = character;
			_pose = pose;
			_outfit = outfit;
			_expression = expression;

			DisplaySpeaker(_character, _pose, _outfit, _expression, manager);
		}

		public void DisplaySpeaker(string character, string pose, string outfit, string expression, ICharacterBundleManager manager)
		{
			GameObjectUtils.RemoveChildren(_speakerBounds.gameObject);
			// fetch the scriptable object containing the speaker bounds
			// create a child game object with those bounds
			GameObject display = new GameObject("speakerDisplay");
			RectTransform rt = display.AddComponent<RectTransform>();

//			rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _data.position.width);
//			rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _data.position.height);

			// find a random pose, outfit, and expression

//			CharacterPoses poseDisplay = display.AddComponent<CharacterPoses>(); commented out
//			poseDisplay.DisplayPoseEditor(character, pose, outfit, expression);
//			poseDisplay.DisplayPose(character, pose, outfit, expression, manager); commented out
//			Transform poseTransform = poseDisplay.gameObject.transform.GetChild(0);
//            poseTransform.position = new Vector3(poseTransform.position.x - _data.position.x, poseTransform.position.y - _data.position.y, poseTransform.position.z);

			// parent those to the newly created child object

			float scaleX = _speakerBounds.rect.width / _data.position.width;
			float scaleY = _speakerBounds.rect.height / _data.position.height;
			rt.anchoredPosition = new Vector2(-_data.position.x * scaleX, -_data.position.y * scaleY);
			rt.localScale = new Vector3(scaleX, scaleY, 1.0f);

			display.transform.SetParent(_speakerBounds, false);

			Debug.Log ("scale is: " + scaleX + ", " + scaleY);
			// find the width and height of the _speakerBounds transform
			// compare it to the width and height of the new child object -- compute the x & y scale to shrink or scale up this child to fit into the parent window
		}

		public void UpdateTextSize(int size)
		{
//			_text.fontSize = size;
		}
	}
}
