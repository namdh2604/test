// TODO #5.0# -- This class is probably no longer necessary

//using System;
//using UnityEngine;
//using UnityEditor;
//using System.Linq;
//using System.Collections.Generic;
//using System.IO;
//
//namespace Voltage.Story.Import.CharacterImport
//{
//	using Voltage.Story.Import.CharacterImport.Helpers;
//
//	public class CharacterAssetBundleCreator
//	{
//		private const string CHARACTER_ROOT = "Assets/Characters";
//
//		public void CreateAssetBundle(string source, string destinationPath, List<BuildTarget> platforms)
//		{
//			VerifyPaths(source, destinationPath);
//
//			if (platforms.Count == 0)
//			{
//				return;
//			}
//
//			string charName = CharacterBundleUtils.GetCharNameFromPath(source);
//
//            string[] guids = AssetDatabase.FindAssets("t:GameObject t:CharacterLayoutData", new string[] {source});
//            var paths = guids.Select(x => AssetDatabase.GUIDToAssetPath(x));
//            var objects = paths.Select(x => AssetDatabase.LoadAssetAtPath(x, typeof(UnityEngine.Object))).ToArray();
//            var names = paths.Select(x => CharacterBundleUtils.NormalizePath(x, source)).ToArray();
//
//			foreach (BuildTarget platform in platforms)
//			{
//				string destination = destinationPath + "/" + charName + ".unity3d";
//	            BuildPipeline.BuildAssetBundleExplicitAssetNames(objects, names, destination, BuildAssetBundleOptions.CollectDependencies, platform);
//			}
//		}
//
//		private void VerifyPaths(string source, string destinationPath)
//		{
//			if (!Directory.Exists(source))
//			{
//				throw new Exception("Source directory " + source + " does not exist");
//			}
//
//			if (!Directory.Exists(destinationPath))
//			{
//				throw new Exception("Destination directory " + destinationPath + "does not exist");
//			}
//		}
//	}
//}
