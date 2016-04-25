using UnityEngine;
using System.Collections;

public interface IInput
{
	void ActiveDown(Vector3 startPoint);
	void ActiveUpdate(Vector3 currentPoint);
	void ActiveUp(Vector3 endPoint);
	void ClearLinesFromCamera();
	void ResetTracking();
}
