using UnityEngine;
using System.Collections;

namespace Voltage.Witches.Components
{
	using Sprite = UnityEngine.Sprite;

	public class SpriteGenerator 
	{
		public Material SpriteMasker { get; protected set; }

		public SpriteGenerator()
		{
			try
			{
				SpriteMasker = Resources.Load<Material>("Textures/TestMask");
			}
			catch(UnityException)
			{
				throw new UnityException("Could load the test material with the test shader");
			}
		}

		public Sprite CreateSpriteFromTexture(Texture2D texture)
		{
			Sprite sprite = Sprite.Create(texture,new Rect(0f,0f,texture.width,texture.height),new Vector2(0.5f,0.5f),100f);

			return sprite;
		}

		public GameObject CreateCanvasObject(GameObject baseObject,float scaling)
		{
			GameObject newCanvas = GameObject.Instantiate(baseObject) as GameObject;
			newCanvas.name = "Target_Canvas";
			newCanvas.transform.SetParent(baseObject.transform.parent.transform);
			newCanvas.GetComponent<Renderer>().sortingOrder = baseObject.GetComponent<Renderer>().sortingOrder + 2;
			newCanvas.transform.localScale = new Vector3 (scaling, scaling, 1f);
			newCanvas.GetComponent<Collider2D>().enabled = false;
			newCanvas.GetComponent<Renderer>().material = SpriteMasker;

			return newCanvas;
		}

		public GameObject CreateSpriteAndObject(Texture2D texture,GameObject baseObject,float scaling)
		{
			var sprite = CreateSpriteFromTexture(texture);
			var canvas = CreateCanvasObject(baseObject,scaling);

			canvas.GetComponent<SpriteRenderer>().sprite = sprite;
			return canvas;
		}
	}
}