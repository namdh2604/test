using Ninject;
using Ninject.Activation;
using UnityEngine;

using Voltage.Witches.Scheduling;

namespace Voltage.Witches.DI
{
    public class MonoBehaviourProvider<T> : Provider<T> where T : MonoBehaviour
    {
        private readonly string _goName;
        private readonly bool _useExistingWhenAvailable;
        private const string DEFAULT_NAME = "AutoGameObject";

        public MonoBehaviourProvider(string gameObjectName = DEFAULT_NAME, bool useExistingWhenAvailable=false)
        {
            _goName = gameObjectName;
            _useExistingWhenAvailable = useExistingWhenAvailable;
        }

        // NOTE: separating out functionality to these functions causes Unity to give compilation errors
//        private T CreateNewObject()
//        {
//            GameObject go = new GameObject(_goName);
//            T behaviour = go.AddComponent<T>();
//            return behaviour;
//        }
//
//        private T GetOrCreateBehaviour(GameObject go)
//        {
//            T behaviour = go.GetComponent<T>();
//            if (behaviour == null)
//            {
//                behaviour = go.AddComponent<T>();
//            }
//
//            return behaviour;
//        }

        protected override T CreateInstance(IContext context)
        {
            GameObject go = null;

            if (_useExistingWhenAvailable)
            {
                go = GameObject.Find(_goName);
                if (go == null)
                {
                    go = new GameObject(_goName);
                    return go.AddComponent<T>();
                }
                else
                {
                    T behaviour = go.GetComponent<T>();
                    if (behaviour != null)
                    {
                        return behaviour;
                    }
                    else
                    {
                        return go.AddComponent<T>();
                    }
                }
            }
            else
            {
                go = new GameObject(_goName);
                return go.AddComponent<T>();
            }
        }

    }
}
