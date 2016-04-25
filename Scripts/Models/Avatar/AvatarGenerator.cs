using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace Voltage.Witches.Models.Avatar
{
    using Voltage.Witches.Bundles;

    public class AvatarGenerator : MonoBehaviour
    {
        public RectTransform _avatarContainer;
        public Camera _renderCamera;

        private IAvatarPositioner _positioner;
        private UIScreenshotCreator _screenshotCreator;
        private ImageBoundCalculator _boundCalculator;
        private IAvatarResourceManager _resourceManager;

        private AvatarLayerDisplay _layerDisplay;
        private AvatarFileWriter _fileWriter;

        private static readonly Vector2 STORY_RESOLUTION = new Vector2(640, 400);
        private static readonly Vector2 HEADSHOT_RESOLUTION = new Vector2(1280, 800);

        private const string DEFAULT_EXPRESSION = "smile";

        private bool _isInit = false;

        private void Start()
        {
            if (_renderCamera == null)
            {
                _renderCamera = CreateCamera();
            }

            Canvas canvas = _avatarContainer.gameObject.GetComponentInParent<Canvas>();

            _positioner = new AvatarPositioner();
            _boundCalculator = new ImageBoundCalculator();
            _screenshotCreator = new UIScreenshotCreator(_renderCamera, canvas, _boundCalculator);

            _fileWriter = new AvatarFileWriter(_boundCalculator, _screenshotCreator);

        }

        public void Init(IAvatarResourceManager resourceManager)
        {
            if (_isInit)
            {
                return;
            }

            _resourceManager = resourceManager;

            _layerDisplay = new AvatarLayerDisplay(_resourceManager, this, _avatarContainer);
            List<string> layerOrder = new List<string>(AvatarLayerUtility.LAYER_ORDER);
            layerOrder.Reverse();

            _layerDisplay.InitializeWithStructure(layerOrder);

            _isInit = true;
        }

        private void OnDestroy()
        {
            _layerDisplay.ReleaseBundles();
        }

        private Camera CreateCamera()
        {
            GameObject cameraGO = new GameObject("RenderCamera");
            Camera camera = cameraGO.AddComponent<Camera>();
            camera.orthographic = true;
            camera.depth = -1;
            camera.useOcclusionCulling = false;
            camera.transform.position = transform.position;
            camera.backgroundColor = new Color(0, 0, 0, 0);
            camera.enabled = false;

            return camera;
        }

        public void RegenerateImages(Outfit outfit, System.Action onComplete)
        {
            StartCoroutine(RegenerateImageRoutine(outfit, onComplete));
        }

        private IEnumerator RegenerateImageRoutine(Outfit outfit, System.Action onComplete)
        {
            yield return StartCoroutine(SaveAll(outfit));
            if (onComplete != null)
            {
                onComplete();
            }
        }

        public IEnumerator DisplayTop(Outfit outfit, OutfitType outfitType, AvatarType avatarType, string expression=DEFAULT_EXPRESSION)
        {
            var layerUtility = new AvatarLayerUtility(outfit, outfitType, expression);
            yield return StartCoroutine(_layerDisplay.DisplayAvatarLayers(layerUtility.GetTopLayers()));

			_positioner.Reposition(_avatarContainer, avatarType);
        }

        public IEnumerator DisplayBottom(Outfit outfit, OutfitType outfitType, AvatarType avatarType, string expression=DEFAULT_EXPRESSION)
        {
            var layerUtility = new AvatarLayerUtility(outfit, outfitType, expression);
            yield return StartCoroutine(_layerDisplay.DisplayAvatarLayers(layerUtility.GetBotLayers()));

			_positioner.Reposition(_avatarContainer, avatarType);
        }

		public IEnumerator DisplayAll(Outfit outfit, OutfitType outfitType, AvatarType avatarType, string expression=DEFAULT_EXPRESSION)
        {
            var layerUtility = new AvatarLayerUtility(outfit, outfitType, expression);
            yield return StartCoroutine(_layerDisplay.DisplayAvatarLayers(layerUtility.GetAllLayers()));

			_positioner.Reposition(_avatarContainer, avatarType);
        }

		public void SaveImage(AvatarType avatarType)
		{
			string filepath = AvatarTexturePathInfo.GetAvatarSaveName(avatarType);
			Vector2 targetResolution = (avatarType == AvatarType.Story) ? STORY_RESOLUTION : HEADSHOT_RESOLUTION;
			_fileWriter.SaveScreen(filepath, _avatarContainer, targetResolution, Vector2.zero);
		}

        public IEnumerator SaveAll(Outfit outfit)
        {
            OutfitType[] outfitTypes = new OutfitType[] { OutfitType.Default, OutfitType.Naked };
            foreach (var outfitType in outfitTypes)
            {
                var layerUtility = new AvatarLayerUtility(outfit, outfitType, DEFAULT_EXPRESSION);

                var allLayers = layerUtility.GetAllLayers();
                yield return StartCoroutine(_layerDisplay.DisplayAvatarLayers(allLayers));

                if (outfitType == OutfitType.Default)
                {
                    _positioner.Reposition(_avatarContainer, AvatarType.Fullbody);
					SaveImage(AvatarType.Fullbody);

                    _positioner.Reposition(_avatarContainer, AvatarType.Headshot);
					SaveImage(AvatarType.Headshot);
                }

                _positioner.Reposition(_avatarContainer, AvatarType.Story);

                var topLayers = layerUtility.GetTopLayers();
                yield return StartCoroutine(_layerDisplay.DisplayAvatarLayers(topLayers));
                string topSaveName = AvatarTexturePathInfo.GetStorySaveName("Top", outfitType);
                _fileWriter.SaveScreen(topSaveName, _avatarContainer, STORY_RESOLUTION, _avatarContainer.localPosition);

                var bottomLayers = layerUtility.GetBotLayers();
                yield return StartCoroutine(_layerDisplay.DisplayAvatarLayers(bottomLayers));
                string botSaveName = AvatarTexturePathInfo.GetStorySaveName("Bot", outfitType);
                _fileWriter.SaveScreen(botSaveName, _avatarContainer, STORY_RESOLUTION, _avatarContainer.localPosition);
            }

            _fileWriter.WriteCoordinates();
        }
    }
}
