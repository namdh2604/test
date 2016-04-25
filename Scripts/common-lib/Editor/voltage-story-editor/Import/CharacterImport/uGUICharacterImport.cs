using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Voltage.Common;
using Voltage.Common.Filesystem;
using Voltage.Common.Utilities;
using Voltage.Story.Import.CharacterImport.Model;
using Voltage.Story.Layout;

namespace Voltage.Story.Import.CharacterImport
{
    public class UGUICharacterImport
    {
        private const string ARCHIVE_PATTERN = "*.lex";
        private const float CHAR_IMPORT_SCALE = 2.0f; // current art pipeline is exporting at half size

        private const string POSITION_LAYER_NAME = "positionReference";
        private const string SPEAKER_LAYER_REGEX = @"speaker_image_icon_MA_\d+";
        private const string CHARACTER_ROOT = "Assets/Characters";
//        private const string PREFAB_ROOT = "Assets/Resources/CharacterPrefabs";

        private string _prefabPath;
        private string _assetPath;
        private PoseParser _parser;

        public UGUICharacterImport()
        {
            _parser = new PoseParser();
        }

        public static void ImportAll(string fromPath, string toPath)
        {
            UGUICharacterImport importer = new UGUICharacterImport();

            string[] archives = Directory.GetFiles(fromPath, ARCHIVE_PATTERN);

            foreach (var archive in archives)
            {
                string charName = Path.GetFileNameWithoutExtension(archive);
                importer.doImport(archive, toPath, charName, CHAR_IMPORT_SCALE);
            }
        }

        public void doImport(string lexPath, string destination, string charName, float scaleFactor)
        {
            destination = destination + "/" + charName;
			if (Directory.Exists(destination))
			{
				FileUtil.DeleteFileOrDirectory(destination);
			}
            Directory.CreateDirectory(destination);

            if (charName == string.Empty)
            {
				throw new Exception("Character Name Required");
            }

            string lexDir = ExtractLex(lexPath);

			_prefabPath = CHARACTER_ROOT + "/" + charName;
			if (Directory.Exists(_prefabPath))
			{
				FileUtil.DeleteFileOrDirectory(_prefabPath);
			}

			Directory.CreateDirectory(_prefabPath);
            _assetPath = CHARACTER_ROOT + "/" + charName + "/Images";
			Directory.CreateDirectory(_assetPath);

            ImportImages(lexDir, _assetPath);
//            var poses = GetPoses(lexDir, scaleFactor);
            ParseLex(lexDir, scaleFactor);

            var poses = _parser.Poses;
            GeneratePrefabs(poses);

            Rect speakerFrame = _parser.SpeakerFrame;
            CharacterLayoutData layoutData = CharacterLayoutData.CreateInstance<CharacterLayoutData>();
            layoutData.position = speakerFrame;

            AssetDatabase.CreateAsset(layoutData, _prefabPath + "/layoutData.asset");
            AssetDatabase.SaveAssets();

            GeneratePreviews(charName, poses, destination);
//            CreateAssetBundle(destination + "/" + charName + ".unity3d");
//            }
        }

		// TODO #5.0# -- This will be taken care of by the new 5.0 asset bundle system
//        private void CreateAssetBundle(string destination)
//        {
//            string[] guids = AssetDatabase.FindAssets("t:GameObject t:CharacterLayoutData", new string[] {_prefabPath});
//            var paths = guids.Select(x => AssetDatabase.GUIDToAssetPath(x));
//            var objects = paths.Select(x => AssetDatabase.LoadAssetAtPath(x, typeof(UnityEngine.Object))).ToArray();
//            var names = paths.Select(x => NormalizePath(x, _prefabPath)).ToArray();
//
//            BuildPipeline.BuildAssetBundleExplicitAssetNames(objects, names, destination, BuildAssetBundleOptions.CollectDependencies);
//        }

        private static string NormalizePath(string path, string prefix)
        {
            return path.Substring(prefix.Length + 1);
        }

        private void GeneratePrefabs(Dictionary<string, PoseInfo> poses)
        {
            foreach (var pair in poses)
            {
                CreatePosePrefab(pair.Key, pair.Value);
            }
        }

        private void GeneratePreviews(string charName, Dictionary<string, PoseInfo> poses, string destination)
        {
            HashSet<string> processedPoses = new HashSet<string>();
            HashSet<string> processedOutfits = new HashSet<string>();
            HashSet<string> processedExpressions = new HashSet<string>();

            Canvas canvas = GameObject.Find("StoryCanvas").GetComponent<Canvas>();
			Camera camera = GameObject.Find("CaptureCamera").GetComponent<Camera>();
            GameObject anchor = canvas.transform.Find("LeftCharacter").gameObject;
            ScreenCapture captureUtil = new ScreenCapture(canvas, camera, anchor);

            foreach (var posePair in poses)
            {
                PoseInfo pose = posePair.Value;
                string poseName = pose.name;

                string previewRoot = destination + "/previews";
                Directory.CreateDirectory(previewRoot);

                string poseRoot = destination + "/poses";
                Directory.CreateDirectory(poseRoot);

                string outfitRoot = destination + "/outfits";
                Directory.CreateDirectory(outfitRoot);

                string expressionRoot = destination + "/expressions";
                Directory.CreateDirectory(expressionRoot);

                foreach (var outfitName in pose.outfits.Keys)
                {
                    foreach (var rawExpressionName in pose.expressions.Keys)
                    {
                        string expressionName = CharacterPoses.TruncateExpression(rawExpressionName);
                        string[] tokens = new string[] { poseName, outfitName, expressionName };
                        string fullName = string.Join("_", tokens);
                        string path = constructPath(fullName, previewRoot);
                        captureUtil.CreatePNG(charName, poseName, outfitName, expressionName, path);

                        if (!processedPoses.Contains(poseName))
                        {
                            File.Copy(path, constructPath(poseName, poseRoot));
                            processedPoses.Add(poseName);
                        }

                        if (!processedOutfits.Contains(outfitName))
                        {
                            File.Copy(path, constructPath(outfitName, outfitRoot));
                            processedOutfits.Add(outfitName);
                        }

                        if (!processedExpressions.Contains(expressionName))
                        {
                            File.Copy(path, constructPath(expressionName, expressionRoot));
                            processedExpressions.Add(expressionName);
                        }
                    }
                }
            }
        }

        private string constructPath(string name, string root)
        {
            return root + "/" + name + ".png";
        }

        private void CreatePosePrefab(string poseName, PoseInfo pose)
        {
            GameObject go = new GameObject(poseName, typeof(RectTransform));
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(pose.offset.x, pose.offset.y);

            Directory.CreateDirectory(_prefabPath);
            PrefabUtility.CreatePrefab(_prefabPath + "/" + poseName + ".prefab", go);
            GameObject.DestroyImmediate(go);

            string posePath = _prefabPath + "/" + poseName + "/";
            Directory.CreateDirectory(posePath);

            GameObject backgroundGO = new GameObject("Background", typeof(RectTransform));
            AddImageLayers(pose.baseImages, backgroundGO);
            PrefabUtility.CreatePrefab(posePath + "Background.prefab", backgroundGO);
            GameObject.DestroyImmediate(backgroundGO);

            Directory.CreateDirectory(posePath + "OutfitsBot");
            Directory.CreateDirectory(posePath + "OutfitsTop");

            foreach (var outfitPair in pose.outfits)
            {
                AddOutfit(outfitPair.Key, outfitPair.Value, posePath);
            }

            Directory.CreateDirectory(posePath + "Expressions");

            foreach (var expData in pose.expressions)
            {
                AddExpression(expData.Key, expData.Value, posePath);
            }
        }

        private void AddOutfit(string outfitName, OutfitPair outfitData, string prefabBase)
        {
            GameObject outfitRootTop = new GameObject(outfitName, typeof(RectTransform));
            AddImageLayers(outfitData.top, outfitRootTop);
            PrefabUtility.CreatePrefab(prefabBase + "OutfitsTop/" + outfitName + ".prefab", outfitRootTop);
            GameObject.DestroyImmediate(outfitRootTop);

            GameObject outfitRootBottom = new GameObject(outfitName, typeof(RectTransform));
            AddImageLayers(outfitData.bottom, outfitRootBottom);
            PrefabUtility.CreatePrefab(prefabBase + "OutfitsBot/" + outfitName + ".prefab", outfitRootBottom);
            GameObject.DestroyImmediate(outfitRootBottom);
        }

        private void AddExpression(string expressionName, List<LayerInfo> layers, string prefabBase)
        {
            GameObject expressionRoot = new GameObject(expressionName, typeof(RectTransform));
            AddImageLayers(layers, expressionRoot);

            expressionName = TruncateExpressionName(expressionName);

            PrefabUtility.CreatePrefab(prefabBase + "Expressions/" + expressionName + ".prefab", expressionRoot);
            GameObject.DestroyImmediate(expressionRoot);
        }

        private string TruncateExpressionName(string expressionName)
        {
            int index = expressionName.LastIndexOf('_');
            if (index == -1)
            {
                return expressionName;
            }

            return expressionName.Substring(0, index);
        }

        private void AddImageLayers(List<LayerInfo> layers, GameObject parent)
        {
            foreach (var layer in layers)
            {
                if (!layer.isImage)
                {
                    continue;
                }

                GameObject layerGO = new GameObject(layer.name);

                RectTransform rt = layerGO.AddComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(layer.absolute.x, layer.absolute.y);
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, layer.absolute.width);
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, layer.absolute.height);

                Image image = layerGO.AddComponent<Image>();
                image.sprite = AssetDatabase.LoadAssetAtPath(_assetPath + "/" + layer.name + ".png", typeof(Sprite)) as Sprite;

                layerGO.transform.SetParent(parent.transform, false);
            }
        }

        public string ExtractLex(string path)
        {
            string tempDir = FileUtil.GetUniqueTempPathInProject();
            ZipUtils.UnzipToDir(path, tempDir);

            return tempDir;
        }

        public void ImportImages(string importFromPath, string importToPath)
        {
            var files = Directory.GetFiles(importFromPath, "*.png");
            foreach (var file in files)
            {
                if (!IsUsefulImage(file))
                {
                    continue;
                }

                string path = importToPath + "/" + Path.GetFileName(file);

                FileUtil.CopyFileOrDirectory(file, path);
                AssetDatabase.ImportAsset(path);
            }

            AssetDatabase.Refresh();
        }

        private void ParseLex(string lexDir, float scaleFactor)
        {
            var jsonFiles = Directory.GetFiles(lexDir, "*.json");
            if (jsonFiles.Length != 1)
            {
                throw new Exception("Could not locate json meta file");
            }

            JObject obj = JObject.Parse(File.ReadAllText(jsonFiles[0]));

            _parser.Parse(obj, scaleFactor);
            foreach (var warning in _parser.Warnings)
            {
                Debug.LogWarning(warning);
            }
        }

//        private Dictionary<string, PoseInfo> GetPoses(string lexDir, float scaleFactor)
//        {
//            var jsonFiles = Directory.GetFiles(lexDir, "*.json");
//            if (jsonFiles.Length != 1)
//            {
//                throw new Exception("Could not locate json meta file");
//            }
//
//            JObject obj = JObject.Parse(File.ReadAllText(jsonFiles[0]));
//
//            Dictionary<string, PoseInfo> poses = _parser.Parse(obj, scaleFactor);
//            foreach (var warning in _parser.Warnings)
//            {
//                Debug.LogWarning(warning);
//            }
//
//            Rect speakerFrame = _parser.SpeakerFrame;
//            Debug.Log("speaker rect is: " + speakerFrame);
//
//            return poses;
//        }

        private bool IsUsefulImage(string path)
        {
            string filename = Path.GetFileNameWithoutExtension(path);
            return !((filename == POSITION_LAYER_NAME) || (Regex.IsMatch(filename, SPEAKER_LAYER_REGEX)));
        }
    }
}