using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Voltage.Witches.Models.Avatar
{
    public class UIScreenshotCreator
    {
        private readonly Camera _renderTextureCamera;
        private readonly Canvas _targetCanvas;
        private readonly ImageBoundCalculator _calculator;

        #if UNITY_IPHONE
        private readonly Shader _shader;
        #endif

        /**
         * NOTES:
         * Canvas must be in:
         *  - Scale with Screen Size
         *  - Screen Space Camera, with the camera set
         */
        public UIScreenshotCreator(Camera renderTextureCamera, Canvas targetCanvas, ImageBoundCalculator calculator, Shader shader=null)
        {
            _renderTextureCamera = renderTextureCamera;
            _targetCanvas = targetCanvas;
            _calculator = calculator;

            #if UNITY_IPHONE
            _shader = (shader != null) ? shader : GetDefaultShader();
            #endif
        }

        public Texture2D GetScreenTexture(ref Rect canvasBounds, Canvas canvas, Vector2 requestedResolution)
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary((int)requestedResolution.x, (int)requestedResolution.y, 0);
            // activate the render texture to capture drawing
            _renderTextureCamera.gameObject.SetActive(true);
            _renderTextureCamera.targetTexture = renderTexture; // causes the previous camera to become disabled
            RenderTexture.active = renderTexture;

            Camera prevCam = _targetCanvas.worldCamera;
            _targetCanvas.worldCamera = _renderTextureCamera;

            // render the entire screen to the render texture
            #if UNITY_IPHONE
            _renderTextureCamera.RenderWithShader(_shader, string.Empty);
            #else
            _renderTextureCamera.Render();
            #endif

            Rect screenBounds = _calculator.convertToScreenBounds(canvas, canvasBounds, requestedResolution);

            // Adjust those coordinates to align with pixel boundaries
            screenBounds = _calculator.alignScreenCoordsToPixels(screenBounds);

            // Change the original canvas coordinates to reflect these changes
            canvasBounds = _calculator.convertScreenCoordsToCanvasCoords(screenBounds, canvas, requestedResolution);

            Texture2D croppedTexture = GenerateTextureFromRenderTexture(screenBounds);

            _renderTextureCamera.gameObject.SetActive(false);
            _renderTextureCamera.targetTexture = null;
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(renderTexture);
            renderTexture = null;

            _targetCanvas.worldCamera = prevCam;
            prevCam.gameObject.SetActive(true);
            _renderTextureCamera.gameObject.SetActive(false);

            return croppedTexture;
        }

        private Texture2D GenerateTextureFromRenderTexture(Rect boundingRect)
        {
            Texture2D texture = new Texture2D((int)boundingRect.width, (int)boundingRect.height, TextureFormat.ARGB32, false);
            texture.ReadPixels(boundingRect, 0, 0);
            texture.Apply(false);

            return texture;
        }

        #if UNITY_IPHONE
        private Shader GetDefaultShader()
        {
            // the default canvas shader doesn't composite images correctly (transparency issues).
            // This is a built-in shader that will work for iOS.
            // Android still needs to use the built-in canvas one due to ETC issues.
            return Shader.Find("Sprites/Default");
        }
        #endif
    }
}

