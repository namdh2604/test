using UnityEngine;
using System.Collections;

public class CloneSpriteAndInterpolate : MonoBehaviour
{
	[SerializeField]
	private Sprite mySprite;
	[SerializeField]
	private Material myMaterial;

	private GameObject newGradient = null;
	private bool _resizeAndChangeColor = false;
	private Color _startColor;
	private Color _targetColor;
	private float _colorChangeStep = 0.0f;
	private float _colorChangeDuration = 5.0f;

	void Awake()
	{
		mySprite = gameObject.GetComponent<SpriteRenderer> ().sprite;
		myMaterial = gameObject.GetComponent<Renderer>().material;
	}

	void Start()
	{
		newGradient = new GameObject("NEW_GRADIENT", typeof(SpriteRenderer));
		newGradient.transform.position = transform.position;
		newGradient.transform.rotation = transform.rotation;
		newGradient.GetComponent<Renderer>().material = myMaterial;
		_startColor = new Color32 (255, 0, 0, 0);
		newGradient.GetComponent<Renderer>().material.color = _startColor;
		_targetColor = new Color32 (255, 0, 0, 255);
		newGradient.GetComponent<SpriteRenderer>().sprite = mySprite;
		newGradient.GetComponent<Renderer>().sortingOrder = 1;
		newGradient.transform.localScale = new Vector3(0.1f, 0.1f, 1.0f);
		_resizeAndChangeColor = true;
	}

	void SwapColorsAndDeleteObject ()
	{
		gameObject.GetComponent<Renderer>().material.color = _targetColor;
		Destroy(newGradient);
		newGradient = null;
	}

	void FixedUpdate()
	{
		if(_resizeAndChangeColor)
		{
			float step = Time.deltaTime * 1.0f;
			newGradient.transform.localScale = Vector3.Slerp(newGradient.transform.localScale,transform.localScale,step);

			newGradient.GetComponent<Renderer>().material.color = Color.Lerp(_startColor,_targetColor,_colorChangeStep);

			if(_colorChangeStep < _colorChangeDuration)
			{
				_colorChangeStep += Time.deltaTime/_colorChangeDuration;
			}

			if(newGradient.transform.localScale == transform.localScale)
			{
				_resizeAndChangeColor = false;
				SwapColorsAndDeleteObject();
			}
		}
	}
}
