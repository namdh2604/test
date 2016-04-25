using Ninject;
using Ninject.Activation;
using UnityEngine;

namespace Voltage.Witches.DI
{
    using Witches.Exceptions;

    public class SceneObjectProvider<T> : Provider<T> where T : MonoBehaviour
    {
        private readonly string _goName;
        private readonly bool _isTag;

        public SceneObjectProvider(string existingObjectName, bool isTag=false)
        {
            _goName = existingObjectName;
            _isTag = isTag;
        }

        protected override T CreateInstance(IContext context)
        {
            GameObject go = (_isTag) ? GameObject.FindGameObjectWithTag(_goName) : GameObject.Find(_goName);
            if (go == null)
            {
                string objType = (_isTag) ? "tag" : "name";
                throw new WitchesException("No scene object found with " + objType + ": " + _goName);
            }

            T script = go.GetComponent<T>();
            if (script == null)
            {
                throw new WitchesException("No script found of type: " + typeof(T).Name);
            }

            return script;
        }
    }
}

