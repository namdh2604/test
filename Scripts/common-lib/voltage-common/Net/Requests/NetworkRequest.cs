using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Common.Net
{


	public class NetworkRequest : INetworkRequest
	{
		public string URL { get; protected set; }
		public Dictionary<string,string> Parameters { get; protected set; }

		public NetworkRequest(string url, IDictionary<string,string> parms=null)
		{
			URL = !string.IsNullOrEmpty (url) ? url : string.Empty;
			Parameters = (parms != null ? new Dictionary<string,string> (parms) : new Dictionary<string,string>());
		}
		
		public override string ToString ()
		{
			return string.Format ("{0}{1}", URL, InlineParameters(Parameters));
		}
		
		// /test/demo_form.asp?name1=value1&name2=value2
		private string InlineParameters(IDictionary<string,string> parms)
		{
			string inline = string.Empty;
			
			if(parms != null && parms.Count > 0)
			{
				inline += "?";
				
				List<string> keyList = new List<string>(parms.Keys);
				for(int i=0; i < keyList.Count; ++i)
				{
					string parameter = string.Format("{0}={1}", keyList[i], parms[keyList[i]]);
					parameter += (i < parms.Count-1 ? "&" : string.Empty);
					
					inline += parameter;
				}
			}
			
			return inline;
		}
	}


//	public class NetworkRequest : INetworkRequest
//	{
////		public Method Method{ get { return Method.GET; } }	// HACK
//
//		public string URL { get; protected set; }
//		public Dictionary<string,string> Parameters { get; protected set; }
//
//		public NetworkRequest (string url, Dictionary<string,string> parms=null)
//		{
//			URL = !string.IsNullOrEmpty(url) ? url : string.Empty;
//			Parameters = parms;		// NOTE: UnityNetworkRequest relies on Parameters being null to determine Post or Get...should probably make this explicit!
//		}
//
//		public override string ToString ()
//		{
//			return string.Format ("{0}{1}", URL, InlineParameters (Parameters));
//		}
//
//		private string InlineParameters(IDictionary<string,string> parms)
//		{
//			string inline = string.Empty;
//			
//			if(parms != null && parms.Count > 0)
//			{
//				inline += "?";
//				
//				List<string> keyList = new List<string>(parms.Keys);
//				for(int i=0; i < keyList.Count; ++i)
//				{
//					string parameter = string.Format("{0}={1}", keyList[i], parms[keyList[i]]);
//					parameter += (i < parms.Count-1 ? "&" : string.Empty);
//					
//					inline += parameter;
//				}
//			}
//			
//			return inline;
//		}
//	}



//
//	public abstract class BaseNetworkRequest : INetworkRequest
//	{
////		public abstract Method Method { get; }
//		
//		public string URL { get; protected set; }
//		public Dictionary<string,string> Parameters { get; protected set; }
//
//		public BaseNetworkRequest (string url, IDictionary<string,string> parms)
//		{
//			URL = url;
//			Parameters = (parms != null ? new Dictionary<string,string> (parms) : new Dictionary<string,string>());
//		}
//	}
//
//
//	public class GetNetworkRequest : BaseNetworkRequest
//	{
////		public override Method Method{ get { return Method.GET; } }
//
//		public GetNetworkRequest(string url, IDictionary<string,string> parms=null) : base (url, parms) {}	
//
//		public override string ToString ()
//		{
//			return string.Format ("{0}{1}", URL, InlineParameters(Parameters));
//		}
//
//		// /test/demo_form.asp?name1=value1&name2=value2
//		private string InlineParameters(IDictionary<string,string> parms)
//		{
//			string inline = string.Empty;
//
//			if(parms != null && parms.Count > 0)
//			{
//				inline += "?";
//
//				List<string> keyList = new List<string>(parms.Keys);
//				for(int i=0; i < keyList.Count; ++i)
//				{
//					string parameter = string.Format("{0}={1}", keyList[i], parms[keyList[i]]);
//					parameter += (i < parms.Count-1 ? "&" : string.Empty);
//
//					inline += parameter;
//				}
//			}
//
//			return inline;
//		}
//	}
//
//	public class PostNetworkRequest : BaseNetworkRequest
//	{
////		public override Method Method{ get { return Method.POST; } }
//
//		public PostNetworkRequest(string url, IDictionary<string,string> parms=null) : base (url, parms) {}
//		
//		public override string ToString ()
//		{
//			return string.Format ("{0}{1}", URL, InlineParameters(Parameters));
//		}
//		
//		private string InlineParameters(IDictionary<string,string> parms)
//		{
//			string inline = string.Empty;
//			
//			if(parms != null && parms.Count > 0)
//			{
//				inline += "\t[ ";
//
//				List<string> keyList = new List<string>(parms.Keys);
//				for(int i=0; i < keyList.Count; ++i)
//				{
//					string parameter = string.Format("{0}={1}", keyList[i], parms[keyList[i]]);
//					parameter += (i < parms.Count-1 ? "&" : string.Empty);
//					
//					inline += parameter;
//				}
//
//				inline += " ]";
//			}
//			
//			return inline;
//		}
//	}



	

}





//public class NetworkRequest : INetworkRequest
//{
//	
//	public string URL { get; protected set; }
//	public Dictionary<string,string> Parameters { get; protected set; }
//	
//	public NetworkRequest (string url, Dictionary<string,string> parms=null)
//	{
//		URL = !string.IsNullOrEmpty(url) ? url : string.Empty;
//		Parameters = parms;		// NOTE: UnityNetworkRequest relies on Parameters being null to determine Post or Get...should probably make this explicit!
//	}
//	
//	public override string ToString ()
//	{
//		return string.Format ("{0} :: {1}", URL, Parameters != null && Parameters.Count > 0 ? GetParametersInlineString (Parameters) : string.Empty);
//	}
//	
//	private string GetParametersInlineString (Dictionary<string,string> parms)
//	{
//		string output = string.Empty;
//		
//		if(parms != null && parms.Count > 0)
//		{
//			if(!URL.EndsWith("/"))
//			{
//				URL += "/";
//			}
//			
//			foreach(KeyValuePair<string,string> kvp in parms)
//			{
//				output += string.Format("{0}={1}, ", kvp.Key, kvp.Value);
//			}
//		}
//		
//		return output;
//	}
//}








