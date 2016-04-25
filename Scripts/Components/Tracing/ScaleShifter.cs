using UnityEngine;
using System.Collections;

namespace Voltage.Witches.Components
{
	public class ScaleShifter : MonoBehaviour 
	{
		private Vector3 _smallScale;
		private Vector3 _largeScale;
		private Vector3 _targetScale;

		private float _baseSteps = 10.0f;
		private float _maxSteps = 25.0f;
		private float _steps = 10.0f;
		private bool _isScaling;

		public void BeginScaling()
		{
			_smallScale = transform.localScale;
			_largeScale = new Vector3((_smallScale.x + 0.1f),(_smallScale.y + 0.1f),1f);
			_targetScale = _largeScale;

			_isScaling = true;
		}

		public void BeginScaling(float? steps)
		{
			_steps = (steps.HasValue)? steps.Value : 10f;
			_baseSteps = _steps;
			_maxSteps = _baseSteps * 2.5f;
			_smallScale = transform.localScale;
			_largeScale = new Vector3((_smallScale.x + 0.1f),(_smallScale.y + 0.1f),1f);
			_targetScale = _largeScale;
			
			_isScaling = true;
		}

		void Update()
		{
			if(_isScaling)
			{
				var step = Time.deltaTime * _steps;
				transform.localScale = Vector3.Lerp(transform.localScale,_targetScale,step);
//				Debug.LogWarning("Changing Scale");
				_steps = Mathf.Clamp(_steps,_baseSteps,_maxSteps);
				var diff = Vector3.Distance(transform.localScale,_targetScale);
//				Debug.LogWarning(diff.ToString());
				if((diff >= -0.01f) && (diff <= 0.01f))
				{
//					Debug.LogWarning("Resetting");
					ResetScaling();
				}
			}
		}

		void ResetScaling()
		{
			_isScaling = false;
			if(_targetScale == _largeScale)
			{
				_targetScale = _smallScale;
			}
			else
			{
				_targetScale = _largeScale;
			}
			_isScaling = true;
		}
	}
}