
using System;
using System.Collections.Generic;

namespace Voltage.Common.Net
{
	using UnityEngine;

	public sealed class WWWGetRequest : WWWNetworkTransport
	{
		public WWWGetRequest (INetworkRequest request, Action<WWW> onComplete, int timeoutInSeconds=DEFAULT_TIMEOUT) : base (request, onComplete, timeoutInSeconds) {}
		
		protected override WWW MakeWWW (INetworkRequest request)
		{
			return new WWW (CreateGetURL(request));
		}
		
		private string CreateGetURL(INetworkRequest request)
		{
			return string.Format ("{0}{1}", request.URL, InlineParameters(request.Parameters));
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
                    string dataString = Uri.EscapeDataString(parms[keyList[i]]);
                    string parameter = string.Format("{0}={1}", keyList[i], dataString);
					parameter += (i < parms.Count-1 ? "&" : string.Empty);
					
					inline += parameter;
				}
			}
			
			return inline;
		}
		
	}
	
	public sealed class WWWPostRequest : WWWNetworkTransport
	{
		public WWWPostRequest (INetworkRequest request, Action<WWW> onComplete, int timeoutInSeconds=DEFAULT_TIMEOUT) : base (request, onComplete, timeoutInSeconds) {}
		
		protected override WWW MakeWWW (INetworkRequest request)
		{
			WWWForm form = new WWWForm();
			
			if(request.Parameters != null && request.Parameters.Count > 0)
			{
				foreach (KeyValuePair<string,string> kvp in request.Parameters)
				{
					string value = (!string.IsNullOrEmpty(kvp.Value) ? kvp.Value : string.Empty);
					
					form.AddField (kvp.Key, value);
				}
			}
			else
			{
				form.AddField(string.Empty, string.Empty);
			}
			
			return new WWW (request.URL, form);
		}
	}

    
}




