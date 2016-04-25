using System;
using System.IO;
using UnityEngine;

namespace Voltage.Witches.Models.Avatar
{
    public class AvatarFileWriter
    {
        private readonly ImageBoundCalculator _boundCalculator;
        private readonly UIScreenshotCreator _screenshotCreator;
        private AvatarOffsets _avatarOffsets;

        private static readonly string _dataPath;

        static AvatarFileWriter()
        {
            _dataPath = AvatarTexturePathInfo.GetAvatarRoot();
        }

        public AvatarFileWriter(ImageBoundCalculator boundCalculator, UIScreenshotCreator screenshotCreator)
        {
            _screenshotCreator = screenshotCreator;
            _boundCalculator = boundCalculator;
            _avatarOffsets = new AvatarOffsets();
        }

        public void SaveScreen(string name, RectTransform container, Vector2 requestedResolution, Vector2 offset)
        {
            // Get bounding canvas coordinates
            Rect bounds = _boundCalculator.computeBounds(container);
            Canvas canvas = container.GetComponentInParent<Canvas>();

            // take a picture of that specific boundary area, scaled to the requested resolution
            Texture2D screenTexture = _screenshotCreator.GetScreenTexture(ref bounds, canvas, requestedResolution);
            byte[] bytes = screenTexture.EncodeToPNG();
            string path = _dataPath + "/" + name + ".png";
            Directory.CreateDirectory(_dataPath);

            #if UNITY_IOS
            // do not back up avatar images
            UnityEngine.iOS.Device.SetNoBackupFlag(_dataPath);
            #endif

            File.WriteAllBytes(path, bytes);
            GameObject.Destroy(screenTexture);

            // Account for offset information -- the boundaries used above are relative to the parent anchor,
            // but the bounds we output are meant to be used without that anchor.  This embeds that information
            // in the final output
            Rect finalBounds = new Rect(bounds.x - offset.x, bounds.y - offset.y, bounds.width, bounds.height);
            _avatarOffsets.SetInfo(name, finalBounds);
        }

        public void WriteCoordinates()
        {
            _avatarOffsets.Save();
        }
    }
}

