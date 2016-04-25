using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquid_Animation : MonoBehaviour 
{
	[SerializeField]
	private Sprite[] _liquidSprites = new Sprite[3];
	[SerializeField]
	private SpriteRenderer _myRenderer = null;
	private bool _isAnimating = false;
	private float _timePassed = 0.0f;

	void Awake()
	{
		_myRenderer = gameObject.GetComponent<SpriteRenderer>();
	}

	void Start()
	{
		StartAnimation();
	}

	void StartAnimation ()
	{
		_isAnimating = true;
	}

	int GetShuffledNextIndex()
	{
		List<int> shuffleList = new List<int> (){ 0, 1, 2 };
		string[] currentSpriteNameParts = _myRenderer.sprite.name.Split(('_'));
		int currentIndex = Convert.ToInt32(currentSpriteNameParts[currentSpriteNameParts.Length - 1]);
		shuffleList.Remove(currentIndex);
		int indexToreturn = UnityEngine.Random.Range(0, 2);
		return shuffleList[indexToreturn];
	}

	Sprite GetNewSprite()
	{
		return _liquidSprites[GetShuffledNextIndex()];
	}

	void ChangeSprite ()
	{
		_myRenderer.sprite = GetNewSprite();
		_timePassed = 0.0f;
		_isAnimating = true;
	}

	void FixedUpdate()
	{
		if(_isAnimating)
		{
			_timePassed += Time.deltaTime;

			if(_timePassed >= 0.35f)
			{
				_isAnimating = false;
				ChangeSprite();
			}
		}
	}
}
