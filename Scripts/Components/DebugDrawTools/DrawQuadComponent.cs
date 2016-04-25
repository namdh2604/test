using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// NOTE: Possibly used for minigame stuff
public class DrawQuadComponent : MonoBehaviour 
{
	public Material mat;
	private Vector3 startVertexTop;
	private Vector3 startVertexBottom;
	private Vector3 endVertexTop;
	private Vector3 endVertexBottom;
	private bool _initiatingDraw = false;
	private Dictionary<int,List<Vector3>> _linesAndVertexList = new Dictionary<int, List<Vector3>>();
	private bool _useDictionary = false;

	public void DrawLine(Vector2 hotSpot)
	{
		float adjustmentForHeight = Screen.height * 0.01f;
		float adjustmentForWidth = Screen.width * 0.02f;

		startVertexTop = new Vector3((hotSpot.x + adjustmentForWidth),(hotSpot.y + adjustmentForHeight));
		startVertexBottom = new Vector3((hotSpot.x + adjustmentForWidth),(hotSpot.y - adjustmentForHeight));
		endVertexTop = new Vector3((hotSpot.x + (adjustmentForWidth * 3)),(hotSpot.y + adjustmentForHeight));
		endVertexBottom = new Vector3((hotSpot.x + (adjustmentForWidth * 3)),(hotSpot.y - adjustmentForHeight));
		
		CreateLine ();
	}

	public void DrawLines(List<Vector3> drawPoints)
	{
		_useDictionary = true;

		for(int i = 0; i < drawPoints.Count; ++i)
		{
			List<Vector3> vertexList = CreateListOfVertexes(drawPoints[i]);
			_linesAndVertexList[i] = vertexList;
		}

//		_linesAndVertexList[0] = ceilingList;
//		_linesAndVertexList[1] = floorList;

		CreateLine();
	}

	private List<Vector3> CreateListOfVertexes(Vector3 target)
	{
		List<Vector3> newVertexes = new List<Vector3>();

		float adjustmentForHeight = Screen.height * 0.01f;
		float adjustmentForWidth = Screen.width * 0.3f; 

		startVertexTop = new Vector3((target.x - adjustmentForWidth),(target.y + adjustmentForHeight));
		startVertexBottom = new Vector3((target.x - adjustmentForWidth),(target.y - adjustmentForHeight));
		endVertexTop = new Vector3((target.x + adjustmentForWidth),(target.y + adjustmentForHeight));
		endVertexBottom = new Vector3((target.x + adjustmentForWidth),(target.y - adjustmentForHeight));

		newVertexes.Add(startVertexTop);
		newVertexes.Add(endVertexTop);
		newVertexes.Add(endVertexBottom);
		newVertexes.Add(startVertexBottom);

		return newVertexes;
	}

	public void DrawLineBetweenPoints(Vector3 pointA, Vector3 pointB)
	{
		if(_initiatingDraw)
		{
			ClearLines();
		}
		float adjustmentForHeight = Screen.height * 0.01f;

		startVertexTop = new Vector3 (pointA.x, (pointA.y + adjustmentForHeight));
		startVertexBottom = new Vector3(pointA.x, (pointA.y - adjustmentForHeight));
		endVertexTop = new Vector3(pointB.x,(pointB.y + adjustmentForHeight));
		endVertexBottom = new Vector3(pointB.x,(pointB.y - adjustmentForHeight));

		CreateLine();
	}

	public void ClearLines()
	{
		_initiatingDraw = false;
		if(_useDictionary)
		{
			_useDictionary = false;
			_linesAndVertexList.Clear();
		}
	}
	
	private void CreateLine()
	{
		if(!mat)
		{
			mat = new Material(Shader.Find("Sprites/Default"));
			mat.color = Color.white;
		}
		_initiatingDraw = true;
	}
	
	void OnPostRender() {
		if(_initiatingDraw)
		{
			if (!mat) {
				Debug.LogError("Please Assign a material on the inspector");
				return;
			}

			if(!_useDictionary)
			{
				GL.PushMatrix();
				mat.SetPass(0);
				GL.LoadPixelMatrix();
				GL.Begin(GL.QUADS);
				GL.Color(Color.red);
				GL.Vertex(startVertexTop);
				GL.Vertex(endVertexTop);
				GL.Vertex(endVertexBottom);
				GL.Vertex(startVertexBottom);
				GL.End();
				GL.PopMatrix();
			}
			else
			{
				GL.PushMatrix();
				mat.SetPass(0);
				GL.LoadPixelMatrix();
				GL.Begin(GL.QUADS);
				GL.Color(Color.white);
				for(int i = 0; i < _linesAndVertexList.Count; ++i)
				{
					List<Vector3> current = _linesAndVertexList[i];
					for(int l = 0; l < current.Count; ++l)
					{
						GL.Vertex(current[l]);
					}
				}
				GL.End();
				GL.PopMatrix();
			}
		}
	}
}
