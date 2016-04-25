using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

using System;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Voltage.Story.Import.LexImport
{
    public class UGUIImporter
    {
        private string _baseName;
        private string _resourcePath;

        public UGUIImporter()
        {
            
        }

        // destination path should be relative to Application.dataPath
        public void doImport(string lexPath, string destination, float scaleFactor=1f)
        {
            _baseName = Path.GetFileNameWithoutExtension(lexPath);
            _resourcePath = destination + "/" + _baseName;

            string path = LexUtils.ExtractLexToTempPath(lexPath);
            ImportImages(path, _resourcePath);

            string lexDataPath = GetLexDataPath(path);
            ConstructLayout(lexDataPath, scaleFactor);
        }

        private string GetLexDataPath(string containingFolderPath)
        {
            var files = Directory.GetFiles(containingFolderPath, "*.json");
            if (files.Length != 1)
            {
                throw new Exception("No Lex Data found!");
            }

            return files[0];
        }
        
        public void ImportImages(string importFromPath, string importToPath)
        {
            string fullToPath = Path.Combine(Application.dataPath, importToPath);
            Directory.CreateDirectory(fullToPath);
            var files = Directory.GetFiles(importFromPath, "*.png");
            foreach (var file in files)
            {
                var dest = Path.Combine(importToPath, Path.GetFileName(file));
                var fulldest = Path.Combine(Application.dataPath, dest);
                File.Copy(file, fulldest, overwrite: true);

                // Unity now wants all asset imports to be relative to the project
                AssetDatabase.ImportAsset("Assets/" + dest);
                AssetDatabase.Refresh();
            }
        }
        
        public void ConstructLayout(string path, float scaleFactor)
        {
            JObject obj = JObject.Parse(File.ReadAllText(path));

            float width = obj["container"].Value<float>("width") * scaleFactor;
            float height = obj["container"].Value<float>("height") * scaleFactor;

            GameObject go = new GameObject("Scene");
            RectTransform rt = go.AddComponent<RectTransform>();
            go.transform.SetParent(UnityEditor.Selection.activeGameObject.transform);
            rt.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.offsetMin = new Vector2(0.0f, 0.0f);
            rt.offsetMax = new Vector2(0.0f, 0.0f);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            
            foreach (var jobj in obj["images"].Children().Reverse())
            {
                ParseLayer(jobj, rt, scaleFactor);
            }
        }
        
        public void ParseLayer(JToken jobj, RectTransform parent, float scaleFactor)
        {
            GameObject go = new GameObject(jobj["name"].ToString());
            RectTransform rt = go.AddComponent<RectTransform>();
            go.transform.SetParent(parent.gameObject.transform);
            rt.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            
            float x = jobj["absolute"].Value<float>("x") * scaleFactor;
            float y = jobj["absolute"].Value<float>("y") * scaleFactor;
			float width = jobj["absolute"].Value<float>("width") * scaleFactor;
			float height = jobj["absolute"].Value<float>("height") * scaleFactor;
            
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            
            float adjustedX = (x + width / 2.0f) - parent.rect.width / 2.0f;
            float adjustedY = parent.rect.height / 2.0f - (y + height / 2.0f);
//            float adjustedY = - (y + height / 2.0f);

            rt.anchoredPosition = new Vector2(adjustedX, adjustedY);
            
            if (jobj.Value<bool>("isGroup"))
            {
                var tokens = jobj["images"].AsJEnumerable().Reverse();
                foreach (var layer in tokens)
                {
                    ParseLayer(layer, rt, scaleFactor);
                }
            }
            else
            {
                // layer is an image -- find the image associated with the name
                Sprite sprite = Resources.Load<Sprite>(_baseName + "/" + jobj["name"].ToString());
                Image image = go.AddComponent<Image>();
                image.sprite = sprite;
            }
        }
    }
}
