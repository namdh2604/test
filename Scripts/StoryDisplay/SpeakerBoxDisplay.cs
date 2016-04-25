using System;
using UnityEngine;
using UnityEngine.UI;

using Voltage.Common.Utilities;
using Voltage.Witches.Configuration;
using Voltage.Witches.AssetManagement;
using Voltage.Witches.Models.Avatar;
using Voltage.Witches.Bundles;

using Newtonsoft.Json.Linq;

namespace Voltage.Witches.Layout
{
	public class SpeakerBoxDisplay : DialogueBoxDisplay
	{
		public GameObject _speakerGO;
		public RectTransform _speakerBounds;

		private ICharacterBundleManager _assetManager;
        private IAvatarResourceManager _avatarResourceManager;
		private JObject _config;

		public override void Setup(ICharacterBundleManager manager, IAvatarResourceManager avatarResourceManager, JObject config)
		{
			_assetManager = manager;
            _avatarResourceManager = avatarResourceManager;
			_config = config;
		}

		private CharacterConfig GetSpeakerPoseInfo(string speaker)
		{
            string defaultPose = _config[speaker]["speaker_info"].Value<string>("pose") ?? string.Empty;
            string defaultOutfit = _config[speaker]["speaker_info"].Value<string>("outfit") ?? string.Empty;
            string defaultExpression = _config[speaker]["speaker_info"].Value<string>("expression") ?? string.Empty;

			string poseName = _config[speaker]["poses"].Value<string>(defaultPose);
			string outfitName = _config[speaker]["outfits"].Value<string>(defaultOutfit);
			string expressionName = _config[speaker]["expressions"].Value<string>(defaultExpression);
			return new CharacterConfig(speaker, poseName, outfitName, expressionName);
		}

        public override void UpdateDisplay(string text, string speaker, bool isAvatar)
		{
			base.UpdateDisplay(text, speaker, isAvatar);
			GameObjectUtils.RemoveChildren(_speakerBounds.gameObject);

			if (string.IsNullOrEmpty(speaker) && !isAvatar)
			{
				HideSpeaker();
			}
			else
			{
                ShowSpeaker(speaker, isAvatar);
			}
		}

        private void ShowSpeaker(string speaker, bool isAvatar)
		{
			_speakerGO.SetActive(true);

			GameObject display = new GameObject("speakerDisplay");
			RectTransform rt = display.AddComponent<RectTransform>();

			CharacterPoses poseDisplay = display.AddComponent<CharacterPoses>();

            string characterId = (isAvatar) ? AvatarNameUtility.GetBundleName() : speaker;

            CharacterConfig config = GetSpeakerPoseInfo(characterId);
            poseDisplay.DisplayPose(characterId, config.DefaultPose, config.DefaultOutfit, config.DefaultExpression, _assetManager, _avatarResourceManager);

			CharacterLayoutData data = _assetManager.GetAsset<CharacterLayoutData>(characterId, "layoutData.asset");

            if (data != null)
            {
    			float scaleX = _speakerBounds.rect.width / data.position.width;
    			float scaleY = _speakerBounds.rect.height / data.position.height;
    			rt.anchoredPosition = new Vector2(-data.position.x * scaleX, -data.position.y * scaleY);
    			rt.localScale = new Vector3(scaleX, scaleY, 1.0f);
            }
            else
            {
                GameObject obj = Resources.Load<GameObject>("PlaceholderCharacter/background") as GameObject;
                RectTransform myRT = obj.GetComponent<RectTransform>();
                float scaleX = _speakerBounds.rect.width / myRT.rect.width;
                float scaleY = _speakerBounds.rect.height / myRT.rect.height;
                rt.anchoredPosition = new Vector2(-myRT.position.x * scaleX, -myRT.position.y * scaleY);
                rt.localScale = new Vector3(scaleX, scaleY, 1.0f);
            }

			display.transform.SetParent(_speakerBounds, false);
		}

		private void HideSpeaker()
		{
			_speakerGO.SetActive(false);
		}
	}
}

