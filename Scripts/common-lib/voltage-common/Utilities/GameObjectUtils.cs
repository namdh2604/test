using UnityEngine;

namespace Voltage.Common.Utilities
{
    public static class GameObjectUtils
    {
        public static void Destroy(GameObject go)
        {
            if (Application.isEditor)
            {
                GameObject.DestroyImmediate(go);
            }
            else
            {
                GameObject.Destroy(go);
            }
        }

		public static void RemoveChildren(GameObject go)
		{
			int childCount = go.transform.childCount;
			for (int i = childCount - 1; i >= 0; --i)
			{
				Destroy(go.transform.GetChild(i).gameObject);
			}
		}
    }
}
