using UnityEngine;
using System.Collections.Generic;
using System.Collections;

// NOTE: Possibly used for minigame stuff
public class LineDrawing : MonoBehaviour, IInteraction {

	public Material mat;
	private bool _initiatingDraw = false;

	private List<List<Vector3>> _lineVertexes = new List<List<Vector3>> ();

	private void StoreVectors(List<Vector3> vectors)
	{
		_lineVertexes.Add(vectors);
	}

	public void DrawLine(List<Vector3> drawPoints)
	{
		StoreVectors(drawPoints);

		CreateLine();
	}

	public void DrawLine(Vector3 startPoint,Vector3 endPoint)
	{
		List<Vector3> current = new List<Vector3> ();
		current.Add(startPoint);
		current.Add(endPoint);
		StoreVectors(current);

		CreateLine();
	}

	public void ClearLines()
	{
		_lineVertexes.Clear();
//		_initiatingDraw = false;
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

	void OnPostRender()
	{
		if(_initiatingDraw)
		{
			if (!mat) 
			{
				Debug.LogError("Please Assign a material on the inspector");
				return;
			}

			foreach(List<Vector3> list in _lineVertexes)
			{
				GL.PushMatrix();
				mat.SetPass(0);
				GL.LoadOrtho();
				GL.Begin(GL.LINES);
				GL.Color(Color.red);
				foreach(Vector3 vector in list)
				{
					GL.Vertex(new Vector3((vector.x / Screen.width), (vector.y / Screen.height), 0.0f));
				}
				GL.End();
				GL.PopMatrix();
			}
		}
	}
}
