

namespace Voltage.Common.Net
{
	using System.Net;
	using UnityEngine;

    public interface INetworkPayload	// need to redo this and rename!
    {
		INetworkRequest Request { get; }
//		HttpStatusCode Status { get; }		// need to change!
		string Text { get; }
    }

//	public interface IUnityPayload : INetworkPayload
//	{
//		WWW WWW { get; }
////		AudioClip AudioClip { get; }
////		AssetBundle AssetBundle { get; }
//	}
    

	public struct NetworkPayload : INetworkPayload	// need to redo this and rename!
	{
		public INetworkRequest Request { get; set; }
//		public HttpStatusCode Status { get; set; }
		public string Text { get; set; }
	}


	public class WWWNetworkPayload : INetworkPayload	// need to redo this and rename!
	{
		public INetworkRequest Request { get; set; }
//		public HttpStatusCode Status { get; set; }		// need to change!
		public string Text { get; set; }

		public WWW WWW { get; set; }

		public virtual AudioClip AudioClip { get { return WWW.audioClip; } }
		public virtual AssetBundle AssetBundle { get { return WWW.assetBundle; } }

		public WWWNetworkPayload () : this (null, HttpStatusCode.NotFound, null) {}
		public WWWNetworkPayload (INetworkRequest request, HttpStatusCode status, WWW www)
		{
			Request = request;
			WWW = www;
			Text = www != null  && www.isDone ? www.text : string.Empty;

//			AudioClip = www.audioClip;
//			AssetBundle = www.assetBundle;
		}
	}



}



