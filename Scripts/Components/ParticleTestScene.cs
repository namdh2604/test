using iGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTestScene : MonoBehaviour 
{
	private InputHandler _input = null;
	private MouseHandler _mouse = null;
	private TouchHandler _touch = null;
	private Rect _bounds;

	[SerializeField]
	private bool _touchInitiated = false;

	private GameObject _trailRenderer = null;

	public GameObject stirGuide;
	private GameObject _stirObject = null;

	void Awake()
	{

	}

	void Update()
	{
		if(Input.GetKeyUp(KeyCode.W))
		{
			ShootOffGuide();
		}
		if(Input.GetKeyUp(KeyCode.S))
		{
			if(_stirObject != null)
			{
				_stirObject.GetComponent<BezierGuideObject>().KillGuide();
				_stirObject = null;
			}
		}
	}

	void Start()
	{
		AddHandlers();
		SetBounds();
		EnableCallBacks();
//		ShootOffGuide();
	}

	void ShootOffGuide ()
	{
		Vector3[] pathPoints = new Vector3[4];
		pathPoints[0] = new Vector3(0.0f,2.0f,0.0f);
		pathPoints[1] = new Vector3(-7.5f,-5.0f,0.0f);
		pathPoints[2] = new Vector3(7.5f,-5.0f,0.0f);
		pathPoints[3] = new Vector3(0.0f,2.0f,0.0f);

		_stirObject = Instantiate(stirGuide,new Vector3(0.0f,2.0f,0.0f),Quaternion.identity) as GameObject;
		BezierGuideObject bezierObject = _stirObject.AddComponent<BezierGuideObject>();
		bezierObject.StartGuideCurve(pathPoints);
	}

	void AddHandlers ()
	{
		_input = gameObject.AddComponent<InputHandler>();
		_mouse = gameObject.AddComponent<MouseHandler>();
		_touch = gameObject.AddComponent<TouchHandler>();
		
		_mouse.AcceptInterface(_input);
		_touch.AcceptInterface(_input);
	}
	
	void SetBounds ()
	{
		_bounds = gameObject.GetComponent<iGUIContainer>().getAbsoluteRect();
		_mouse.SetBoundaries(_bounds);
		_touch.SetBoundaries(_bounds);
	}
	
	void EnableCallBacks()
	{
		_input.OnSwipeStart += HandleOnSwipeStart;
		_input.OnSwipeDrag += HandleOnSwipeDrag;
		_input.OnSwipeEnd += HandleOnSwipeEnd;
		_input.OnLinesClear += HandleOnLinesClear;
	}

	void CreateTrailRenderer(Vector3 worldPosition)
	{
		GameObject trailRendererObject = new GameObject("TRAIL_RENDERER_0", typeof(TrailRenderer));
		trailRendererObject.transform.position = worldPosition;
		trailRendererObject.GetComponent<Renderer>().sortingOrder = 5;
		trailRendererObject.GetComponent<Renderer>().sortingLayerName = "Default";
		trailRendererObject.GetComponent<Renderer>().sortingLayerID = 5;
		TrailRenderer trailer = trailRendererObject.GetComponent<TrailRenderer>();
//		Material lineMat = new Material(Resources.Load<Material>("Materials/Sprite_Changer"));
		Material lineMat = new Material(Resources.Load<Material>("Textures/Test/Test_Trail_2"));
//		lineMat.color = _trailColors[_currentStroke];
		trailer.material = lineMat;
		//		trailer.
		//		trailer.material.SetColor("_TintColor", Color.red);
		trailer.enabled = true;
		trailer.startWidth = 0.9f;
		trailer.endWidth = 0.5f;
		AddTestParticleObject (trailRendererObject);
//		AddParticleToObject(trailRendererObject);
//		ParticleSystem partSys = trailRendererObject.GetComponentInChildren<ParticleSystem>();
//		partSys.Play();
		if(_trailRenderer == null)
		{
			_trailRenderer = trailRendererObject;
		}
//		if(!_renderedLineObjects.Contains(trailRendererObject))
//		{
//			_renderedLineObjects.Add(trailRendererObject);
//		}
	}

	void AddTestParticleObject(GameObject myObject)
	{
		GameObject particleObject = Instantiate (Resources.Load<GameObject> ("Prefabs/TestParticleEmitter"), myObject.transform.position, myObject.transform.rotation) as GameObject;
		particleObject.transform.parent = myObject.transform;
	}

	void AddParticleToObject(GameObject myObject)
	{
		GameObject particleObject = new GameObject ("PARTICLES", typeof(ParticleSystem));
		particleObject.transform.position = myObject.transform.position;
		particleObject.transform.parent = myObject.transform;
		particleObject.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerID = myObject.GetComponent<Renderer>().sortingLayerID;
		particleObject.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = myObject.GetComponent<Renderer>().sortingOrder;
		particleObject.GetComponent<ParticleSystem>().Pause();
		particleObject.GetComponent<ParticleSystem>().loop = false;
		particleObject.GetComponent<ParticleSystem>().startSize = 0.5f;
		particleObject.GetComponent<ParticleSystem>().startColor = Color.white;
		particleObject.GetComponent<ParticleSystem>().startSpeed = 5.0f;
		particleObject.GetComponent<ParticleSystem>().maxParticles = 200;
		particleObject.GetComponent<ParticleSystem>().startLifetime = 0.25f;
		particleObject.GetComponent<ParticleSystem>().simulationSpace = ParticleSystemSimulationSpace.World;
	}

	void MoveTrailRenderer (Vector3 worldPosition)
	{
		_trailRenderer.transform.position = worldPosition;
	}

	void HandleOnSwipeStart (Vector3 screenPoint)
	{
		if(!_touchInitiated)
		{
			_touchInitiated = true;
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPoint);
			worldPosition.z = 0.0f;
			if(_trailRenderer == null)
			{
				CreateTrailRenderer(worldPosition);
			}
			else
			{
				Destroy(_trailRenderer);
				_trailRenderer = null;
				CreateTrailRenderer(worldPosition);
			}
			StartCoroutine(TouchStartBuffer());
		}
	}

	void HandleOnSwipeDrag (Vector3 screenPoint)
	{
		if(_trailRenderer != null)
		{
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPoint);
			worldPosition.z = 0.0f;
			MoveTrailRenderer(worldPosition);
		}
	}

	void HandleOnSwipeEnd (Vector3 screenPoint)
	{
		StartCoroutine(TerminalTouchEndBuffer());
	}

	void HandleOnLinesClear ()
	{
		//
	}

	IEnumerator TouchStartBuffer()
	{
		yield return new WaitForSeconds(0.25f);
		_touchInitiated = false;
	}
	
	IEnumerator TerminalTouchEndBuffer()
	{
		yield return new WaitForSeconds(0.25f);
		_input.ClearLinesFromCamera();
	}
}
