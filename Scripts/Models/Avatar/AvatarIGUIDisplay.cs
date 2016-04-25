using System;
using iGUI;
using UnityEngine;

namespace Voltage.Witches.Models.Avatar
{
    using Voltage.Witches.Unity;

    public class AvatarIGUIDisplay
    {
        private iGUIImage _image;
        private WrappedTexture _texture;
        private Vector2 _resolution;

        private readonly AvatarOffsets _offsetReader;
        private readonly AvatarTextureReader _textureReader;

        public AvatarIGUIDisplay(iGUIImage image, Vector2 resolution)
        {
            _image = image;
            _resolution = resolution;

            _offsetReader = new AvatarOffsets();
            _textureReader = new AvatarTextureReader();
        }

        public void Update(AvatarType avatarType)
        {
            _offsetReader.Load();
            Rect positionInfo = _offsetReader.GetInfo(AvatarTexturePathInfo.GetAvatarSaveName(avatarType));
            ReleaseTexture();
            _texture = _textureReader.GetTexture(avatarType);
            UpdateImage(_texture.Texture, positionInfo);
        }

        public void Destroy()
        {
            ReleaseTexture();
        }

        private void UpdateImage(Texture2D texture, Rect positionInfo)
        {
            float ratio = GetScalingRatio();
            Vector2 dims = GetDimensions(positionInfo, ratio);
            Vector2 translatedOffsets = TranslateOffsets(dims, new Vector2(positionInfo.x, positionInfo.y), ratio);

            Rect newPosition = new Rect(translatedOffsets.x, translatedOffsets.y, dims.x, dims.y);

            _image.setPositionAndSize(newPosition);
            _image.image = texture;
        }

        private float GetScalingRatio()
        {
            Rect screenDims = _image.root.getAbsoluteRectNonScaled();
            return screenDims.height / _resolution.y;
        }

        private Vector2 GetDimensions(Rect dimensions, float ratio)
        {
            float width = dimensions.width * ratio;
            float height = dimensions.height * ratio;

            return new Vector2(width, height);
        }

        private Vector2 TranslateOffsets(Vector2 dimensions, Vector2 offsets, float ratio)
        {
            Rect containerDims = _image.container.getAbsoluteRect();
            float xOffset = (containerDims.width - dimensions.x) / 2.0f + offsets.x * ratio;
            float yOffset = (containerDims.height - dimensions.y) / 2.0f - offsets.y * ratio;

            // Deal with iGUI's limitations -- values between -2 and 2 will be interpreted as screen percents rather than absolute values
            // As these values are so close to 0 anyway, the lesser evil is to just normalize them to 0
            if (Math.Abs(xOffset) <= 2.0f)
            {
                xOffset = 0.0f;
            }

            if (Math.Abs(yOffset) <= 2.0f)
            {
                yOffset = 0.0f;
            }

            return new Vector2(xOffset, yOffset);
        }

        private void ReleaseTexture()
        {
            if (_texture != null)
            {
                _texture.Dispose();
                _texture = null;
            }
        }
    }
}