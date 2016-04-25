using UnityEngine;
using System.Collections;

namespace Voltage.Witches.Components
{
	public class DisplayHighlighter
	{
		public GameObject Target { get; protected set; }
		public GameObject Cloned { get; protected set; }

		private Material _defaultSpriteMat;
		private float _defaultLowRange = 0.3f;//original 0.15f
		private float _defaultHighRange = 0.7f;//original 0.45f

		private float _defaultAlphaDuration = 1f;
		private float _defaultScaleSteps = 1f;

		private Color32 _mainColor = new Color32((byte)0,(byte)255,(byte)0,(byte)255);
		private Color32 _subColor = new Color32((byte)218,(byte)5,(byte)15,(byte)255);
		private Color32 _highlightColor = new Color32((byte)255,(byte)103,(byte)194,(byte)255);

		private float _lowRange;
		private float _highRange;

		public DisplayHighlighter(GameObject target)
		{
			Target = target;
			_defaultSpriteMat = UnityEngine.Resources.Load<Material>("Textures/Sprite_Display");
			_lowRange = _defaultLowRange;
			_highRange = _defaultHighRange;
			CloneTarget(null,_defaultAlphaDuration,_defaultScaleSteps);
		}

		public DisplayHighlighter(GameObject target,int layer,float alphaDuration, float scaleDuration, float lowAlpha, float highAlpha)
		{
			Target = target;
			_lowRange = lowAlpha;
			_highRange = highAlpha;
			CloneTarget(layer,alphaDuration,scaleDuration);
		}

		void CloneTarget(int? layer,float? alphaDuration, float? scaleDuration)
		{
			Cloned = new GameObject("Highlight",typeof(SpriteRenderer),typeof(AlphaShifter),typeof(ScaleShifter));

			Cloned.transform.position = Target.transform.position;
			Cloned.transform.rotation = Target.transform.rotation;
			Cloned.transform.SetParent(Target.transform);
			Cloned.transform.localScale = new Vector3(1.1f,1.1f,1f);

			Cloned.GetComponent<Renderer>().sortingOrder = (layer.HasValue) ? (Target.GetComponent<Renderer>().sortingOrder + layer.Value) : Target.GetComponent<Renderer>().sortingOrder;
			Cloned.GetComponent<Renderer>().sortingLayerID = Target.GetComponent<Renderer>().sortingLayerID;
			Cloned.GetComponent<Renderer>().sortingLayerName = Target.GetComponent<Renderer>().sortingLayerName;

			var render = Cloned.GetComponent<SpriteRenderer>();

			render.material = _defaultSpriteMat;
			render.sprite = Target.GetComponent<SpriteRenderer>().sprite;

			ReplaceTexture();

			var color = render.color;
			color.a = color.a * 0.15f;

			render.color = color;

			var shifter = Cloned.GetComponent<AlphaShifter>();
			if(alphaDuration.HasValue) 
			{
				shifter.StartPulse(_lowRange, _highRange,alphaDuration);
			}
			else
			{
				shifter.StartPulse(_lowRange, _highRange);
			}

			var scaler = Cloned.GetComponent<ScaleShifter>();
			if(scaleDuration.HasValue)
			{
				scaler.BeginScaling(scaleDuration);
			}
			else
			{
				scaler.BeginScaling();
			}
		}

		void ReplaceTexture()
		{
			var rect = Cloned.GetComponent<SpriteRenderer>().sprite.rect;
			var texture = Cloned.GetComponent<SpriteRenderer>().sprite.texture;
			var colors = texture.GetPixels32();
			var newColors = new Color32[colors.Length];
			
			for(int i = 0; i < colors.Length; ++i)
			{
				var current = colors[i];
				if(current.Equals(_mainColor))
				{
					current = _highlightColor;
				}
				else if(current.Equals(_subColor))
				{
					current = new Color32((byte)0,(byte)0,(byte)0,(byte)0);
				}
				else
				{
					current = new Color32((byte)0,(byte)0,(byte)0,(byte)0);
				}
				newColors.SetValue(current,i);
			}
			
			var newTexture = new Texture2D((int)rect.width,(int)rect.height,TextureFormat.ARGB32,false,true);
			newTexture.SetPixels32(newColors);
			newTexture.Apply();
			var newSprite = Sprite.Create(newTexture,rect,new Vector2(0.5f,0.5f));
			Cloned.GetComponent<SpriteRenderer>().sprite = newSprite;
		}

		public void DisposeHighlight()
		{
			UnityEngine.GameObject.Destroy(Cloned);
			Cloned = null;
			Target = null;
		}
	}
}