using UnityEngine;
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Screens
{
    public class UGUIScreenFactory : MonoBehaviour, IScreenFactory
    {
        public List<GameObject> _screens;

        public GameObject _screenParent;
        public GameObject _dialogParent;
        public GameObject _overlayParent;

        private Dictionary<Type, GameObject> _screenMap;

        private void Awake()
        {
            _screenMap = new Dictionary<Type, GameObject>();
            foreach (var screenPrefab in _screens)
            {
                if (screenPrefab == null)
                {
                    Debug.LogWarning("Null object found in UGUI listing -- remove or update");
                    continue;
                }

                BaseUGUIScreen validScreen = screenPrefab.GetComponent<BaseUGUIScreen>();
                if (validScreen == null)
                {
                    Debug.LogWarning("Found invalid screen: [" + screenPrefab.name + "] in " + this.GetType().Name);
                }

                Type screenType = validScreen.GetType();
                if (_screenMap.ContainsKey(screenType))
                {
                    var existingPrefab = _screenMap[screenType];
                    Debug.LogWarning("The screen type: " + screenType.Name + " in " + screenPrefab.name 
                        + " already exists in the screen factory map under: " + existingPrefab.name + ". Skipping");
                    continue;
                }

                _screenMap[validScreen.GetType()] = screenPrefab;
            }
        }

        public T GetScreen<T>() where T: UnityEngine.Component
        {
            return GetScreenInternal<T>(_screenParent);
        }

        public T GetDialog<T>() where T: UnityEngine.Component
        {
            return GetScreenInternal<T>(_dialogParent);
        }

        public T GetOverlay<T>() where T: UnityEngine.Component
        {
            return GetScreenInternal<T>(_overlayParent);
        }

        private T GetScreenInternal<T>(GameObject parent) where T : UnityEngine.Component
        {
            GameObject screenPrefab = GetScreenPrefab(typeof(T));
            GameObject screen = Instantiate(screenPrefab) as GameObject;
            screen.name = typeof(T).Name;

            screen.transform.SetParent(parent.transform, false);

            return screen.GetComponent<T>();
        }

        private GameObject GetScreenPrefab(Type screenType)
        {
            return _screenMap[screenType];
        }
    }
}

