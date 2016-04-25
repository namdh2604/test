using UnityEngine;
using System.Collections;
namespace Voltage.Witches.Components
{
	public interface ICamera
	{
		Camera Main { get; }
		Camera MiniGame { get; }
		void Dispose();
	}

	public class CameraEnabler : MonoBehaviour, ICamera
	{
		public Camera Main { get; protected set; }
		public Camera MiniGame { get; protected set; }

		private float _ortho;

		void Awake()
		{
			Main = Camera.main;

			_ortho = Main.orthographicSize;
			if(Main != null)
			{
				Main.orthographicSize = gameObject.GetComponent<Camera>().orthographicSize;
//				this.GetComponent<Camera>().enabled = false;
				MiniGame = this.GetComponent<Camera>();
			}
			else
			{
				MiniGame = this.GetComponent<Camera>();
				MiniGame.gameObject.tag = "MainCamera";
			}
		}

		public void Dispose()
		{
			if(Main != null)
			{
				Main.orthographicSize = _ortho;
				MiniGame = null;
			}

			else
			{
				MiniGame.enabled = false;
			}
		}
	}
}