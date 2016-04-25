using UnityEngine;
using System.Collections;

namespace Voltage.Witches.Components
{
	public class AlphaShifter : MonoBehaviour 
	{
		private float _lowOpacity;
		private float _highOpacity;
		private bool _isPulsating = false;

		private Color _currentColor;
		private Color _targetColor;
		private float _step = 0f;
		private float _duration = 0.5f;

		private SpriteRenderer _myRenderer;

		public void StartPulse(float low,float high,float? duration)
		{
			_duration = (duration.HasValue) ? duration.Value : 0.5f;
			_lowOpacity = low;
			_highOpacity = high;
			_myRenderer = gameObject.GetComponent<SpriteRenderer>();
			var color = _myRenderer.color;
			_currentColor = color;
			_targetColor = new Color(color.r, color.g, color.b, _highOpacity);
			
			_isPulsating = true;
		}

		public void StartPulse(float low,float high)
		{
			_lowOpacity = low;
			_highOpacity = high;
			_myRenderer = gameObject.GetComponent<SpriteRenderer>();
			var color = _myRenderer.color;
			_currentColor = color;
			_targetColor = new Color(color.r, color.g, color.b, _highOpacity);

			_isPulsating = true;
		}

		void Update()
		{
			if(_isPulsating)
			{
				if(_currentColor != _targetColor)
				{
					_myRenderer.color = Color.Lerp(_currentColor,_targetColor,_step);
					if(_step < _duration)
					{
						_step += (Time.deltaTime / _duration);
					}
					else
					{
						ResetColors();
					}
				}
			}
		}

		void ResetColors()
		{
			_isPulsating = false;
			_step = 0f;
			_currentColor = _myRenderer.color;
			if(_targetColor.a >= _highOpacity)
			{
				_targetColor.a = _lowOpacity;
			}
			else
			{
				_targetColor.a = _highOpacity;
			}
			_isPulsating = true;
		}
	}
}