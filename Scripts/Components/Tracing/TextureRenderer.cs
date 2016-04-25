using UnityEngine;
using System.Collections;

namespace Voltage.Witches.Components
{
	public class TextureRenderer
	{
		public Camera RenderCamera { get; protected set; }

		public TextureRenderer(Camera camera)
		{
			RenderCamera = camera;
		}

		public Texture2D RenderFromBaseCanvas(GameObject canvas)
		{
			var originalCulling = RenderCamera.cullingMask;
			RenderCamera.cullingMask = ~(1 << 10);
			var original = RenderCamera.orthographicSize;
			var spriteVar = canvas.GetComponent<SpriteRenderer>().sprite.texture;
			
			var width = spriteVar.width / 2;
			var height = spriteVar.height / 2;
			RenderCamera.orthographicSize = (spriteVar.height * 0.5f) / 100;
			
			RenderTexture newRenderTexture = new RenderTexture((int)width,(int)height, 1);
			RenderCamera.targetTexture = newRenderTexture;
			Texture2D newImage = new Texture2D((int)width,(int)height,TextureFormat.RGB24,false);
			RenderCamera.Render();
			RenderTexture.active = newRenderTexture;
			newImage.ReadPixels(new Rect(0,0,width,height),0,0);
			RenderCamera.targetTexture = null;
			RenderTexture.active = null;
			UnityEngine.GameObject.Destroy(newRenderTexture);

			RenderCamera.orthographicSize = original;
			RenderCamera.cullingMask = originalCulling;
			return newImage;
		}

		public Texture2D RenderFromTargetCanvas(GameObject baseCanvas,GameObject targetCanvas)
		{
			var original = RenderCamera.orthographicSize;
			var originalSprite = baseCanvas.GetComponent<SpriteRenderer>().sprite.texture.height;
			var spriteVar = targetCanvas.GetComponent<SpriteRenderer>().sprite.texture;
			
			targetCanvas.GetComponent<Renderer>().enabled = false;
			
			var height = originalSprite;
			RenderCamera.orthographicSize = (height * 0.5f) / 100;
			
			RenderTexture newRenderTexture = new RenderTexture((int)(spriteVar.width),(int)(spriteVar.height), 1);
			RenderCamera.targetTexture = newRenderTexture;
			Texture2D newImage = new Texture2D((int)spriteVar.width,(int)spriteVar.height,TextureFormat.RGB24,false);
			RenderCamera.Render();
			RenderTexture.active = newRenderTexture;
			newImage.ReadPixels(new Rect(0,0,spriteVar.width,spriteVar.height),0,0);
			RenderCamera.targetTexture = null;
			RenderTexture.active = null;
			UnityEngine.GameObject.Destroy(newRenderTexture);

			targetCanvas.GetComponent<Renderer>().enabled = true;
			RenderCamera.orthographicSize = original;

			return newImage;
		}
	}
}