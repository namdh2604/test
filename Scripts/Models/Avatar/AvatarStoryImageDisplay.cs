using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Voltage.Witches.Models.Avatar
{
    using Voltage.Witches.Unity;
    using Voltage.Witches.Bundles;

//    [ExecuteInEditMode]
    public class AvatarStoryImageDisplay : MonoBehaviour
    {
        public IAvatarResourceManager _resourceManager;

        public OutfitType _outfit;

        public string _expression;
        private string _activeExpression;

        public Image _topImage;
        public GameObject _expressionParent;
        public Image _botImage;

        private Sprite _topSprite;
        private Sprite _botSprite;

        private WrappedTexture _textureBot;
        private WrappedTexture _textureTop;

        private const string LAYER_TOP = "Top";
        private const string LAYER_BOTTOM = "Bot";

        private static readonly Vector2 CENTER_PIVOT = new Vector2(0.5f, 0.5f);

        private const string AVATAR_MODEL_BUNDLE = "standardAvatarModel";

        private bool isInit = false;

        private void Start()
        {
            if (_resourceManager == null)
            {
                GameObject resourceManagerGO = new GameObject("Asset Bundle Manager");
                IAssetBundleManager resourceManager = resourceManagerGO.AddComponent<AssetBundleManager>();
                _resourceManager = new AvatarResourceManager(resourceManager);
            }

            IEnumerator refreshRoutine = Refresh();
            if (refreshRoutine != null)
            {
                StartCoroutine(refreshRoutine);
            }

            isInit = true;
        }

        private void InitDepedencies()
        {
            if (_avatarReader == null)
            {
                _avatarReader = new AvatarTextureReader();
            }

            if (_offsetReader == null)
            {
                _offsetReader = new AvatarOffsets();
                _offsetReader.Load();
            }
        }

        private void OnEnable()
        {
            InitDepedencies();
        }

        public static GameObject CreateAvatar(OutfitType outfitType, string expression, IAvatarResourceManager resourceManager)
        {
            GameObject go = new GameObject("Avatar", typeof(RectTransform));
            go.SetActive(false);

            GameObject bottom = new GameObject("Bottom");
            Image botImage = bottom.AddComponent<Image>();
            bottom.transform.SetParent(go.transform, false);

            GameObject expressionParent = new GameObject("Expression", typeof(RectTransform));
            expressionParent.transform.SetParent(go.transform, false);


            GameObject top = new GameObject("Top");
            Image topImage = top.AddComponent<Image>();
            top.transform.SetParent(go.transform, false);

            AvatarStoryImageDisplay display = go.AddComponent<AvatarStoryImageDisplay>();
            display._botImage = botImage;
            display._expressionParent = expressionParent;
            display._topImage = topImage;
            display._outfit = outfitType;
            display._expression = expression;
            display._resourceManager = resourceManager;

            go.SetActive(true);

            return go;
        }

        private AvatarTextureReader _avatarReader;
        private AvatarOffsets _offsetReader;

        private void Awake()
        {
            InitDepedencies();
        }

        private void OnDestroy()
        {
            DestroyTextures();
        }

        private void OnValidate()
        {
            if (!Application.isPlaying || !isInit)
            {
                // do not support in-editor modifications, because this class depends upon asset bundles
                return;
            }

            IEnumerator refreshRoutine = Refresh();
            if (refreshRoutine != null)
            {
                StartCoroutine(refreshRoutine);
            }
        }

        private IEnumerator Refresh()
        {
            UpdateTextures();
            return UpdateExpression();
        }

        private void UpdateTextures()
        {
            DestroyTextures();

            if (!Enum.IsDefined(typeof(OutfitType), _outfit))
            {
                return;
            }

            string imageNameBot = AvatarTexturePathInfo.GetStorySaveName(LAYER_BOTTOM, _outfit);
            string imageNameTop = AvatarTexturePathInfo.GetStorySaveName(LAYER_TOP, _outfit);

            _textureBot = _avatarReader.GetStoryTexture(LAYER_BOTTOM, _outfit);
            _textureTop = _avatarReader.GetStoryTexture(LAYER_TOP, _outfit);

            _topSprite = Sprite.Create(_textureTop.Texture, new Rect(0.0f, 0.0f, _textureTop.Texture.width, _textureTop.Texture.height), CENTER_PIVOT);
            _topImage.sprite = _topSprite;

            _botSprite = Sprite.Create(_textureBot.Texture, new Rect(0.0f, 0.0f, _textureBot.Texture.width, _textureBot.Texture.height), CENTER_PIVOT);
            _botImage.sprite = _botSprite;

            Rect botPositionData = _offsetReader.GetInfo(imageNameBot);
            RectTransform botRT = _botImage.transform as RectTransform;
            botRT.sizeDelta = new Vector2(botPositionData.width, botPositionData.height);
            botRT.localPosition = new Vector3(botPositionData.x, botPositionData.y, 0);

            Rect topPositionData = _offsetReader.GetInfo(imageNameTop);
            RectTransform topRT = _topImage.transform as RectTransform;
            topRT.sizeDelta = new Vector2(topPositionData.width, topPositionData.height);
            topRT.localPosition = new Vector3(topPositionData.x, topPositionData.y);
        }

        private IEnumerator UpdateExpression()
        {
            if (_expression == _activeExpression)
            {
                return null;
            }

            IEnumerator loadBundleRoutine = _resourceManager.LoadBundle(AVATAR_MODEL_BUNDLE);
            if (loadBundleRoutine == null)
            {
                ChangeExpression();
                return null;
            }

            return UpdateExpressionRoutine(loadBundleRoutine);
        }

        private IEnumerator UpdateExpressionRoutine(IEnumerator loadBundleRoutine)
        {
            yield return StartCoroutine(loadBundleRoutine);

            ChangeExpression();
        }

        private void ChangeExpression()
        {
            foreach (Transform expression in _expressionParent.transform)
            {
                Destroy(expression.gameObject);
            }

            if (string.IsNullOrEmpty(_expression))
            {
                return;
            }

            GameObject expressionPrefab = _resourceManager.GetAsset<GameObject>(AVATAR_MODEL_BUNDLE, GetBundledExpressionPath(_expression));
            if (expressionPrefab == null)
            {
                return;
            }

            GameObject expressionGO = Instantiate(expressionPrefab) as GameObject;
            expressionGO.transform.SetParent(_expressionParent.transform, false);

            _activeExpression = _expression;
        }

        private static string GetBundledExpressionPath(string expression)
        {
            return "expressions/" + expression + ".prefab";
        }

        private void DestroyTextures()
        {
            if (_botSprite != null)
            {
                SafeDestroy(_botSprite);
                _botSprite = null;
            }
            if (_textureBot != null)
            {
                _textureBot.Dispose();
                _textureBot = null;
            }

            if (_topSprite != null)
            {
                SafeDestroy(_topSprite);
                _topSprite = null;
            }
            if (_textureTop != null)
            {
                _textureTop.Dispose();
                _textureTop = null;
            }
        }

        private void SafeDestroy(UnityEngine.Object obj)
        {
            if (Application.isPlaying)
            {
                Destroy(obj);
            }
            else
            {
                DestroyImmediate(obj);
            }
        }
    }
}
