
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Quitter
{
	using Voltage.Story.General;
	using UnityEngine;

	public interface IQuitter
	{
		void Quit();
	}

//    public class QuitterFactory : IFactory<Application.RuntimePlatform, IQuitter>
//    {
//		public IQuitter Create (RuntimePlatform platform)
//		{
//			switch(platform)
//			{
//			case RuntimePlatform.Android:
//			case RuntimePlatform.IPhonePlayer:
//			default:
////				return new DefaultQuitter();
//			}
//		}
//
//    }

//	public class DefaultQuitter : IQuitter
//	{
//		public void Quit()
//		{
//			Application.Quit ();
//		}
//	}
    
}




