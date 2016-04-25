using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class IngredientSlicerComponent : MonoBehaviour 
{
	private List<GameObject> _myParts = new List<GameObject>();
	private Dictionary<GameObject, List<bool>> _attachedParts = new Dictionary<GameObject, List<bool>>();
	private List<GameObject> _myJoints = new List<GameObject>();
	private bool _isMoving = false;
	private List<GameObject> _movingPieces = new List<GameObject>();
	private GameObject _movingPiece = null;
	private Vector2 _movement = Vector2.zero; 
	private int _totalJoints = 0;
	private int _mostJoints = 0;
	private int _lessThanHalfJoints = 0;

	public delegate void  IngredientStateChangeCallback(INGREDIENT_STATE state);
	public event IngredientStateChangeCallback IngredientStateChanged;

	public enum INGREDIENT_STATE
	{
		WHOLE,
		MOSTLY_WHOLE,
		HALF,
		NOT_SO_WHOLE,
		BARA_BARA
	}

	private INGREDIENT_STATE _myState = INGREDIENT_STATE.WHOLE;

	public enum ATTACHMENT_TYPE
	{
		BOTH,
		TOP,
		BOTTOM,
		NONE
	}

	public INGREDIENT_STATE GetMyState()
	{
		return _myState;
	}

	void Awake()
	{
		_myParts.Capacity = transform.childCount;

		foreach(Transform child in transform)
		{
			_myParts.Add(child.gameObject);
		}
		Debug.Log("Done");

		GetJoints();
	}

	void Start()
	{
		foreach(GameObject child in _myParts)
		{
			string[] nameParts = child.name.Split(('_'));
			int index = Convert.ToInt32(nameParts[1]);
			bool isConnectedOnTop = true;
			bool isConnectedOnBottom = true;
			//HACK Fix this later with better structure
			if(index == 0)
			{
				isConnectedOnTop = false;
			}
			else if(index == 5)
			{
				isConnectedOnBottom = false;
			}
			List<bool> myAttachements = new List<bool>(2);
			myAttachements.Add(isConnectedOnTop);
			myAttachements.Add(isConnectedOnBottom);

			_attachedParts[child.gameObject] = myAttachements;
		}

		Debug.Log("Attachements should be set up");
	}

	private void GetJoints()
	{
		foreach(GameObject piece in _myParts)
		{
			if(piece.transform.childCount > 0)
			{
				GameObject joint = piece.transform.GetChild(0).gameObject;
//				Debug.Log(joint.name);
				_myJoints.Add(joint);
			}
		}

		_totalJoints = _myJoints.Count;
		_mostJoints = Convert.ToInt32(_totalJoints * 0.75f);
		_lessThanHalfJoints = Convert.ToInt32(_totalJoints * 0.25f);
	}

	void FixedUpdate()
	{
		if((_isMoving) && (_movingPiece != null) && (_movingPiece.GetComponent<Rigidbody2D>() != null))
		{
//			_decay -= (Time.deltaTime * 0.95f);
//			_movingPiece.rigidbody2D.AddForceAtPosition(_movement * _decay,_movingPiece.rigidbody2D.centerOfMass);
			_movingPiece.GetComponent<Rigidbody2D>().AddRelativeForce((_movement * 0.5f) ,ForceMode2D.Impulse);
//			_movingPiece.rigidbody2D.AddForce((_movement * 10f),ForceMode2D.Impulse);
			foreach(GameObject piece in _movingPieces)
			{
				if(piece != _movingPiece)
				{
					piece.GetComponent<Rigidbody2D>().AddRelativeForce((_movement * 0.5f) ,ForceMode2D.Impulse);
				}
			}
		}
	}

	IEnumerator WaitToKillJoint(GameObject joint)
	{
		yield return new WaitForSeconds (0.75f);
		_isMoving = false;
		_movingPiece = null;
		_movingPieces.Clear();
		_myJoints.Remove(joint);
		UpdateAttachments(joint);
		Destroy(joint);
		CheckJoints();
	}

	public void KillObject()
	{
		Destroy(gameObject);
	}

	void CheckJoints ()
	{
		if(_myJoints.Count <= 0)
		{
			_myState = INGREDIENT_STATE.BARA_BARA;
		}
		else if((_myJoints.Count > 0) && (_myJoints.Count <= _lessThanHalfJoints))
		{
			_myState = INGREDIENT_STATE.NOT_SO_WHOLE;
		}
		else if((_myJoints.Count > _lessThanHalfJoints) && (_myJoints.Count < _mostJoints))
		{
			_myState = INGREDIENT_STATE.HALF;
		}
		else if((_myJoints.Count >= _mostJoints) && (_myJoints.Count < _totalJoints))
		{
			_myState = INGREDIENT_STATE.MOSTLY_WHOLE;
		}
		else
		{
			_myState = INGREDIENT_STATE.WHOLE;
		}

		if (IngredientStateChanged != null) 
		{
			IngredientStateChanged (_myState);
		}

	}

	public void UpdateAttachments(GameObject joint)
	{
		if(_myJoints.Count <= 0)
		{
			return;
		}
		
		string[] nameParts = joint.name.Split(('_'));
		int id = Convert.ToInt32(nameParts[1]);
		switch(id)
		{
			case 1:
				DetachPieces(0,1);
				break;
			case 2:
				DetachPieces(1,2);
				break;
			case 3:
				DetachPieces(2,3);
				break;
			case 4:
				DetachPieces(3,4);
				break;
			case 5:
				DetachPieces(4,5);
				break;
		}
	}

	void DetachPieces (int indexA, int indexB)
	{
		GameObject pieceA = _myParts[indexA];
		List<bool> attachmentsA = _attachedParts[pieceA];
		attachmentsA[1] = false;

		GameObject pieceB = _myParts[indexB];
		List<bool> attachmentsB = _attachedParts[pieceB];
		attachmentsB[0] = false;
	}

	public void DestroyJoint(GameObject joint, Vector2 movement, List<GameObject> movingParts)
	{
		_movement = movement;
		GameObject piece = joint.transform.parent.gameObject;

		if(piece.GetComponent<Rigidbody2D>().IsSleeping())
		{
			piece.GetComponent<Rigidbody2D>().WakeUp();
		}
		foreach(GameObject part in movingParts)
		{
			if(part.GetComponent<Rigidbody2D>().IsSleeping())
			{
				part.GetComponent<Rigidbody2D>().WakeUp();
			}
		}

		_isMoving = true;
		_movingPiece = piece;
		_movingPieces = movingParts;
		StopCoroutine("WaitToKillJoint");
		StartCoroutine(WaitToKillJoint(joint));
	}

	public void EnableDistanceJoints(List<GameObject> movingPieces)
	{
		if(movingPieces.Count >= 1)
		{
			foreach(GameObject piece in movingPieces)
			{
				ATTACHMENT_TYPE myType = GetMyAttachMentType(piece);
				if((myType == ATTACHMENT_TYPE.BOTH) || (myType == ATTACHMENT_TYPE.BOTTOM))
				{
					ToggleDistanceJoint(piece);
				}
			}
		}
	}

	private List<GameObject> GetAboveAttachedPieces(List<GameObject> abovePieces)
	{
		List<GameObject> returnList = new List<GameObject>();

		for(int i = abovePieces.Count - 1; i > -1; --i)
		{
			ATTACHMENT_TYPE myType = GetMyAttachMentType(abovePieces[i]);

			if(((myType == ATTACHMENT_TYPE.BOTH) || (myType == ATTACHMENT_TYPE.BOTTOM)))
			{
				returnList.Add(abovePieces[i]);

			}
		}

		return returnList;
	}

	private List<GameObject> GetBelowAttachedPieces(List<GameObject> belowPieces)
	{
		List<GameObject> returnList = new List<GameObject>();

		for(int i = belowPieces.Count - 1; i > -1; --i)
		{
			ATTACHMENT_TYPE myType = GetMyAttachMentType(belowPieces[i]);
			
			if(((myType == ATTACHMENT_TYPE.BOTH) || (myType == ATTACHMENT_TYPE.TOP)))
			{
				returnList.Add(belowPieces[i]);
				
			}
		}
		
		return returnList;
	}

	public ATTACHMENT_TYPE GetMyAttachMentType(GameObject piece)
	{
		ATTACHMENT_TYPE myType = ATTACHMENT_TYPE.NONE;
		if(!_attachedParts.ContainsKey(piece))
		{
			return myType;
		}
		List<bool> attachments =_attachedParts[piece];

		bool top = attachments[0];
		bool bottom = attachments[1];

		if((top) && (bottom))
		{
			myType = ATTACHMENT_TYPE.BOTH;
		}
		else if((top) && (!bottom))
		{
			myType = ATTACHMENT_TYPE.TOP;
		}
		else if((!top) && (bottom))
		{
			myType = ATTACHMENT_TYPE.BOTTOM;
		}

		return myType;
	}

	public List<GameObject> GetPiecesAbovePiece(GameObject piece)
	{
		List<GameObject> returnPieces = new List<GameObject>();
		for(int i = 0; i < _myParts.Count; ++i)
		{
			if(_myParts[i] != piece)
			{
				returnPieces.Add(_myParts[i]);
			}
			else if(_myParts[i] == piece)
			{
				break;
			}
		}

		returnPieces = GetAboveAttachedPieces(returnPieces);

		return returnPieces;
	}

	public List<GameObject> GetPiecesBelowPiece(GameObject piece)
	{
		List<GameObject> returnPieces = new List<GameObject>();
		for(int i = _myParts.Count - 1; i > -1; --i)
		{
			if(_myParts[i] != piece)
			{
				returnPieces.Add(_myParts[i]);
			}
			else if(_myParts[i] == piece)
			{
				break;
			}
		}

		returnPieces = GetBelowAttachedPieces(returnPieces);

		return returnPieces;
	}

	public int GetTotalPieces()
	{
		return _myParts.Count;
	}

	public GameObject GetChild(int index)
	{
		return _myParts[index];
	}

	public void ToggleDistanceJoint(GameObject piece)
	{
		DistanceJoint2D distance = null;

		if(piece.GetComponent<DistanceJoint2D>() == null)
		{
			return;
		}
		
		distance = piece.GetComponent<DistanceJoint2D>();
		if(distance.enabled)
		{
			distance.enabled = false;
		}
		else
		{
			distance.enabled = true;
		}
	}

	public void ToggleDistanceJoint(int index)
	{
		DistanceJoint2D distance = null;
		GameObject piece = _myParts[index];
		if(piece.GetComponent<DistanceJoint2D>() == null)
		{
			return;
		}
		
		distance = piece.GetComponent<DistanceJoint2D>();
		if(distance.enabled)
		{
			distance.enabled = false;
		}
		else
		{
			distance.enabled = true;
		}
	}

	public void ToggleSliderJoint(int index)
	{
		SliderJoint2D slider = null;
		GameObject piece = _myParts[index];
		if(piece.GetComponent<SliderJoint2D>() == null)
		{
			return;
		}

		slider = piece.GetComponent<SliderJoint2D>();
		if(slider.enabled)
		{
			slider.enabled = false;
		}
		else
		{
			slider.enabled = true;
		}		
	}
}
