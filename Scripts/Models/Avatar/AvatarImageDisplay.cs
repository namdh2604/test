using UnityEngine;
using UnityEngine.UI;

namespace Voltage.Witches.Models.Avatar
{
    using Voltage.Witches.Unity;

    public class AvatarImageDisplay : MonoBehaviour
    {
        public AvatarType _avatarType;

        public Image _image;

        private Sprite _avatarSprite;
        private WrappedTexture _texture;

        private AvatarTextureReader _avatarReader;
        private AvatarOffsets _offsetReader;

        private static readonly Vector2 CENTER_PIVOT = new Vector2(0.5f, 0.5f);

        public static GameObject CreateAvatar(AvatarType avatarType)
        {
            GameObject go = new GameObject("Avatar", typeof(RectTransform));

            Image image = go.AddComponent<Image>();
            image.enabled = false;
            AvatarImageDisplay display = go.AddComponent<AvatarImageDisplay>();
            display._image = image;
            display._avatarType = avatarType;

            return go;
        }

        private void Awake()
        {
            _avatarReader = new AvatarTextureReader();
            _offsetReader = new AvatarOffsets();
            _offsetReader.Load();
        }

        private void Start()
        {
            Refresh();
            _image.enabled = true;
        }

        private void OnValidate()
        {
            Refresh();
        }

        private void Refresh()
        {
            UpdateTexture();
        }

        public void UpdateTexture(bool rereadDimensions=false)
        {
            if (rereadDimensions)
            {
                _offsetReader.Load();
            }

            DestroyTexture();

            string imageName = AvatarTexturePathInfo.GetAvatarSaveName(_avatarType);

            _texture = _avatarReader.GetTexture(_avatarType);
            _avatarSprite = Sprite.Create(_texture.Texture, new Rect(0.0f, 0.0f, _texture.Texture.width, _texture.Texture.height), CENTER_PIVOT);

            _image.sprite = _avatarSprite;

            Rect positionData = _offsetReader.GetInfo(imageName);
            RectTransform rt = _image.transform as RectTransform;
            rt.sizeDelta = new Vector2(positionData.width, positionData.height);
            rt.localPosition = new Vector2(positionData.x, positionData.y);
        }

        private void OnDestroy()
        {
            DestroyTexture();
        }

        private void DestroyTexture()
        {
            if (_avatarSprite != null)
            {
                Destroy(_avatarSprite);
                _avatarSprite = null;
            }

            if (_texture != null)
            {
                _texture.Dispose();
                _texture = null;
            }
        }
    }
}

