using UnityEngine;
using System.Collections;

public class GradientTestScript : MonoBehaviour 
{
//	Material _myMaterial;

	public int resolution = 256;
	private Texture2D _texture;

	void Awake()
	{
//		_myMaterial = gameObject.renderer.material;

		_texture = new Texture2D (resolution, resolution, TextureFormat.RGB24, true);
		_texture.name = "Procedural Texture";
		_texture.wrapMode = TextureWrapMode.Clamp;
		_texture.filterMode = FilterMode.Bilinear;
		GetComponent<MeshRenderer>().material.mainTexture = _texture;
		FillTexture();
	}

	void FillTexture ()
	{
		if(_texture.width != resolution)
		{
			_texture.Resize(resolution,resolution);
		}

		float stepSize = 1.0f / resolution;
		for(int y = 0; y < resolution; ++y)
		{
			for(int x = 0; x < resolution; ++x)
			{
//				_texture.SetPixel(x,y, new Color((x + 0.5f)* stepSize, (y + 0.5f) * stepSize, 0.0f));
				_texture.SetPixel(x,y, new Color((x + 0.5f)* stepSize % 0.1f, (y + 0.5f) * stepSize % 0.1f, 0.0f) * 10.0f);
			}
		}
		_texture.Apply();
	}

	void Start()
	{
		GradientAlphaKey[] alphaKeys;
		GradientColorKey[] colorKeys;
		Gradient gradient = new Gradient();

		colorKeys = new GradientColorKey[2];
		colorKeys[0].color = Color.black;
		colorKeys[0].time = 0.0f;
		colorKeys[1].color = Color.white;
		colorKeys[1].time = 1.0f;

		alphaKeys = new GradientAlphaKey[2];
		alphaKeys[0].alpha = 1.0f;
		alphaKeys[0].time = 0.0f;
		alphaKeys[1].alpha = 0.0f;
		alphaKeys[1].time = 1.0f;

		gradient.SetKeys(colorKeys, alphaKeys);
	}
}
