using System.Collections;
using Newtonsoft.Json.Linq;

namespace Voltage.Witches.AssetManagement
{
	public interface ICharacterBundleManager
	{
		void SetConfiguration(JObject config);
		IEnumerator DownloadBundle(string charName);
        T GetAsset<T>(string charName, string path) where T : UnityEngine.Object;
        void Cleanup();
	}

}

