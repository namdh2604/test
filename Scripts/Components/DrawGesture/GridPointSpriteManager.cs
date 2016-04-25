using iGUI;
using UnityEngine;
using System;
using System.Collections;

public class GridPointSpriteManager : MonoBehaviour 
{
	public Sprite[] stateSprites = new Sprite[5];

	enum SPRITE_ART_STATE
	{
		BLUE,
		RED,
		YELLOW,
		GREEN,
		DEFAULT
	}

	private SPRITE_ART_STATE _currentState = SPRITE_ART_STATE.DEFAULT;

	private SpriteRenderer _myRenderer = null;

	public int GetCurrentArtState()
	{
		int state = (int)_currentState;
		return state;
	}

	void Awake()
	{
		_myRenderer = gameObject.GetComponent<SpriteRenderer>();
		_myRenderer.sprite = stateSprites[(int)_currentState];
	}

	void Start()
	{

	}

	void UpdateState()
	{
		_myRenderer.sprite = stateSprites [(int)_currentState];
	}

	public void ChangeState(string activeStrokeObjectName,bool isTargetPoint)
	{
		Debug.Log(activeStrokeObjectName);
		string[] idParts = activeStrokeObjectName.Split(('_'));
		int id = Convert.ToInt32(idParts[1]);
		if(!isTargetPoint)
		{
			switch(id)
			{
				case 0:
					_currentState = SPRITE_ART_STATE.YELLOW;
					break;
				case 1:
					_currentState = SPRITE_ART_STATE.GREEN;
					break;
				case 2:
					_currentState = SPRITE_ART_STATE.BLUE;
					break;
				case 3:
					_currentState = SPRITE_ART_STATE.RED;
					break;
			}
		}
		else
		{
			switch(id)
			{
			case 0:
				_currentState = SPRITE_ART_STATE.BLUE;
				break;
			case 1:
				_currentState = SPRITE_ART_STATE.RED;
				break;
			case 2:
				_currentState = SPRITE_ART_STATE.YELLOW;
				break;
			case 3:
				_currentState = SPRITE_ART_STATE.GREEN;
				break;
			}
		}
		UpdateState();
	}
}
