using System;
using System.Collections;

public class RecognizedResult 
{
	public string GestureClass
	{
		get { return _gestureClass; }
	}
	private string _gestureClass = string.Empty;

	public float Precision
	{
		get { return _precision; }
	}
	private float _precision = 0.0f;

	public RecognizedResult CreateResult(string gestureClass, float precision)
	{
		this._gestureClass = gestureClass;
		this._precision = precision;

		return this;
	}
}
