using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Voltage.Witches.Models.Avatar
{
    using Voltage.Witches.Bundles;

    public class BundledAsset
    {
        public string BundleName { get; set; }
        public string AssetPath { get; set; }
        public string LayerName { get; set; }

        public BundledAsset(string bundleName, string assetPath, string layerName)
        {
            BundleName = bundleName;
            AssetPath = assetPath;
            LayerName = layerName;
        }
    }

    public class AvatarLayerDisplay
    {
        private readonly IAvatarResourceManager _resourceManager;
        private readonly MonoBehaviour _routineLauncher;
        private readonly RectTransform _container;

        private HashSet<string> _loadedBundles;


        public AvatarLayerDisplay(IAvatarResourceManager resourceManager, MonoBehaviour routineLauncher, RectTransform container)
        {
            _resourceManager = resourceManager;
            _routineLauncher = routineLauncher;
            _container = container;

            _loadedBundles = new HashSet<string>();
        }

        public void InitializeWithStructure(List<string> layerOrder)
        {
            Clear();

            foreach (var layer in layerOrder)
            {
                GameObject layerGO = new GameObject(layer);
                layerGO.transform.SetParent(_container, false);
            }
        }

        public void ReleaseBundles()
        {
            foreach (var bundleName in _loadedBundles)
            {
                _resourceManager.UnloadBundle(bundleName);
            }
        }

        private void Clear()
        {
            int numChildren = _container.childCount;
            for (int i = numChildren - 1; i >= 0; --i)
            {
                GameObject.Destroy(_container.GetChild(i).gameObject);
            }
        }

        public IEnumerator DisplayAvatarLayers(List<BundledAsset> layers)
        {
            var bundleLoadingRoutine = LoadAllAssetBundles(layers);
            if (bundleLoadingRoutine != null)
            {
                yield return _routineLauncher.StartCoroutine(bundleLoadingRoutine);
            }

            int currentLayerIndex = 0;
            BundledAsset currentLayer = (layers.Count > 0) ? layers[currentLayerIndex] : null;

            foreach (Transform existingLayer in _container)
            {
                GameObject existingObject = existingLayer.gameObject;

                if (ObjectMatchesLayer(existingObject, currentLayer))
                {
                    if (ObjectIsUsingAsset(existingObject, currentLayer.AssetPath))
                    {
                        UseExistingObject(existingObject);
                    }
                    else
                    {
                        ReplaceLayer(existingObject, currentLayer);
                    }

                    currentLayerIndex++;
                    currentLayer = (currentLayerIndex < layers.Count) ? layers[currentLayerIndex] : null;
                }
                else
                {
                    HideLayer(existingObject);
                }
            }
        }

        private bool ObjectMatchesLayer(GameObject go, BundledAsset asset)
        {
            return ((asset != null) && (go.name == asset.LayerName));
        }

        private bool ObjectIsUsingAsset(GameObject go, string asset)
        {
            return (GetAssetNameFromObject(go) == GetAssetName(asset));
        }

        private void UseExistingObject(GameObject go)
        {
            go.SetActive(true);
        }

        private void HideLayer(GameObject go)
        {
            go.SetActive(false);
        }

        private void ReplaceLayer(GameObject existingLayer, BundledAsset newAsset)
        {
            int existingIndex = existingLayer.transform.GetSiblingIndex();
            GameObject.Destroy(existingLayer);

            GameObject freshLayer = InstantiateLayer(newAsset);
            freshLayer.transform.SetParent(_container, false);
            freshLayer.transform.SetSiblingIndex(existingIndex);
        }

        private IEnumerator LoadAllAssetBundles(List<BundledAsset> bundles)
        {
            HashSet<string> uniqueBundles = new HashSet<string>();
            foreach (var bundle in bundles)
            {
                uniqueBundles.Add(bundle.BundleName);
            }

            List<IEnumerator> routines = new List<IEnumerator>();

            foreach (string bundleName in uniqueBundles)
            {
                _loadedBundles.Add(bundleName);

                IEnumerator routine = _resourceManager.LoadBundle(bundleName);
                if (routine != null)
                {
                    routines.Add(routine);
                }
            }

            if (routines.Count > 0)
            {
                return LoadAllBundlesRoutine(routines);
            }

            return null;
        }

        private IEnumerator LoadAllBundlesRoutine(List<IEnumerator> routines)
        {
            foreach (var routine in routines)
            {
                yield return _routineLauncher.StartCoroutine(routine);
            }
        }


        private const string PREFAB_SUFFIX = ".prefab";

        private string GetAssetName(string assetPath)
        {
            // advances past the last separator (/), or sets it to 0
            int startIndex = assetPath.LastIndexOf('/') + 1;
            int endIndex = assetPath.Length - PREFAB_SUFFIX.Length;

            return assetPath.Substring(startIndex, endIndex - startIndex);
        }

        private string GetAssetNameFromObject(GameObject go)
        {
            Image image = go.GetComponent<Image>();
            return (image == null) ? string.Empty : image.sprite.name;
        }

        private GameObject InstantiateLayer(BundledAsset layer)
        {
            GameObject gameAsset = _resourceManager.GetAsset<GameObject>(layer.BundleName, layer.AssetPath);
            GameObject instantiatedObject = GameObject.Instantiate(gameAsset) as GameObject;

            instantiatedObject.name = layer.LayerName;

            return instantiatedObject;
        }
    }
}

