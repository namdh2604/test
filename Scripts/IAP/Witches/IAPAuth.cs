
using System;
using System.Collections.Generic;

namespace Voltage.Witches.IAP
{
	using Voltage.Common.IAP.Tools;

	public interface IIAPAuth
	{
		string GetKey();
	}


    public class IAPAuth : IIAPAuth
    {
		private readonly string _key;

		public IAPAuth (string key)
		{
			if(string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException();
			}

			_key = key;
		}


		public string GetKey()
		{
			return _key;
		}

    }


	public class IAPXORAuth : IIAPAuth
	{
		private readonly int _salt;
		
		public IAPXORAuth (int salt)
		{
			_salt = salt;
		}
		
		private string _part = "/w57NX2hT8IZMpO+5QtnvwEyuv1L0PGy9BSLkAVib91sVWajjdLlGuAP79ZLrEaaxxu8uZOh7OnAQIDAQAB";
		
		public string GetKey()
		{
			string part = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAlig/"; 

			return string.Format (part + GetPart (_salt) + _part); 
		}
		
		private string GetPart(int salt)
		{
			string part = "dFJKLH@rkFSdK@Rw_}ls_HkfC@c@POUpNaFV@BUknlocRq]^~]tMohJABul`JKUf_aVQoaKCPHsb]kjLA~CbFFdWerJq]VPe^ea}^malAWwlptcsB]jfCNfJ_FJTdnnWdps`FNb^]MFwrcM_ufQNSirBTQKfIcWjh]]DpbpEunQAPsLb`C`HIo_FkNSLD@_SNcvFSCtoulA__wsvjPrcsu";
			return IAPTools.StringTransformXOR (part, salt);
		}
	}
    
}









