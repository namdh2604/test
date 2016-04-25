using System;

namespace Voltage.Witches.Resetter
{
	using Ninject;
	using Ninject.Modules;

	using Voltage.Common.Logging;

	using UnityEngine;
	using UnityEngine.SceneManagement;

    using Voltage.Witches.Exceptions;

    public interface IResetter
    {
		void Reset();
    }

	public class WitchesGameResetter : IResetter
	{

		private readonly IKernel _kernel;

		public WitchesGameResetter (IKernel kernel)
		{
			if(kernel == null)
			{
				throw new ArgumentNullException("WitchesGameResetter::Ctor");
			}

			_kernel = kernel;
		}

		public void Reset()
		{
            GameErrorHandler.DeregisterHandler();
			Voltage.Common.Unity.UnitySingleton.Instance.StopAllCoroutines ();

			IOCDispose ();
//			UnloadAssetBundles ();
			ClearScene ();
			GC.Collect ();
			Restart ();
		}

		private void IOCDispose()	// NOTE: TRY to free up resources, doesn't handle all Kernels in build
		{
			if(_kernel != null)
			{
				foreach (INinjectModule module in _kernel.GetModules())
				{
					AmbientLogger.Current.Log ("\tUnloading Module: " + module.Name, LogLevel.INFO);
					_kernel.Unload(module.Name);
				}
			}
		}

//		private void UnloadAssetBundles()
//		{
//
//		}

		private void ClearScene()
		{
			foreach(GameObject obj in MonoBehaviour.FindObjectsOfType<GameObject>())
			{
//				AmbientLogger.Current.Log ("\tDestroying: " + obj.name, LogLevel.INFO);
				MonoBehaviour.Destroy (obj);
			}

		}

		private void Restart()
		{
//			AmbientLogger.Current.Log ("\tRestarting", LogLevel.INFO);
			SceneManager.LoadScene(0);
		}

	}
    
}


//kernel.GetModules()
//	.Where(m => m is MyNinjectModule) // only for modules I created
//		.ForEach(m => kernel.Unload(m.Name));
//kernel.Components.Get<ICache>().Clear();



