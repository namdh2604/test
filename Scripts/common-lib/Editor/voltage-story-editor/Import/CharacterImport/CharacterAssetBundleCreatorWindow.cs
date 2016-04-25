// TODO #5.0# -- This class is likely no longer necessary

//using UnityEngine;
//using UnityEditor;
//
//using Voltage.Story.Layout;
//using System.IO;
//using System.Collections.Generic;
//
//namespace Voltage.Story.Import.CharacterImport
//{
//	public class CharacterAssetBundleCreatorWindow : EditorWindow
//	{
//		[MenuItem("Import/Character/Create Bundle From Selection")]
//        private static void Init()
//        {
//            CharacterAssetBundleCreatorWindow window = EditorWindow.GetWindow<CharacterAssetBundleCreatorWindow>("Character Asset Bundle Builder");
//            window.LoadPrefs();
//            window.Show();
//        }
//
//		private Object _folder;
//		private string _destinationPath;
//		private Dictionary<BuildTarget, bool> _buildTargets;
//		private CharacterAssetBundleCreator _creator;
//
//		public CharacterAssetBundleCreatorWindow()
//		{
//			_buildTargets = new Dictionary<BuildTarget, bool>();
//			_buildTargets[BuildTarget.Android] = false;
//			_buildTargets[BuildTarget.iOS] = false;
//			_buildTargets[BuildTarget.WebPlayer] = false;
//
//			_creator = new CharacterAssetBundleCreator();
//		}
//
//		protected void OnGUI()
//		{
//			_destinationPath = EditorGUILayout.TextField("Destination", _destinationPath);
//            if (GUILayout.Button("Set"))
//            {
//                GUI.FocusControl(string.Empty);
//                string path = EditorUtility.OpenFolderPanel("Set Destination", _destinationPath, string.Empty);
//                if (path != string.Empty)
//                {
//                    _destinationPath = path;
//                }
//            }
//
//			Object newfolder = EditorGUILayout.ObjectField(_folder, typeof(Object), false);
//			if ((newfolder != _folder) && IsFolder(newfolder))
//			{
//				_folder = newfolder;
//			}
//
//			DisplayTargets();
//
//			if (GUILayout.Button("Generate"))
//			{
//				if (CheckRequirements())
//				{
//					SavePrefs();
//					List<BuildTarget> activeTargets = GetActiveTargets();
//					string source = AssetDatabase.GetAssetPath(_folder);
//					_creator.CreateAssetBundle(source, _destinationPath, activeTargets);
//				}
//			}
//		}
//
//		private void DisplayTargets()
//		{
//			_buildTargets[BuildTarget.Android] = GUILayout.Toggle(_buildTargets[BuildTarget.Android], "Android");
//			_buildTargets[BuildTarget.iOS] = GUILayout.Toggle(_buildTargets[BuildTarget.iOS], "iOS");
//			_buildTargets[BuildTarget.WebPlayer] = GUILayout.Toggle(_buildTargets[BuildTarget.WebPlayer], "Standalone/Web");
//		}
//
//		private List<BuildTarget> GetActiveTargets()
//		{
//			List<BuildTarget> activeTargets = new List<BuildTarget>();
//
//			foreach (var pair in _buildTargets)
//			{
//				if (pair.Value)
//				{
//					activeTargets.Add(pair.Key);
//				}
//			}
//
//			return activeTargets;
//		}
//
//		private bool IsFolder(Object candidate)
//		{
//			string path = AssetDatabase.GetAssetPath(candidate);
//			return Directory.Exists(path);
//		}
//
//		private bool CheckRequirements()
//		{
//			if ((_destinationPath.Length == 0) || (!Directory.Exists(_destinationPath)))
//			{
//				DisplayError("Invalid destination specified");
//			}
//			else if (_folder == null)
//			{
//				DisplayError("Invalid source asset folder");
//			}
//			else if (GetActiveTargets().Count == 0)
//			{
//				DisplayError("No Active Targets Selected. Please select at least one target platform");
//			}
//			else
//			{
//				return true;
//			}
//
//			return false;
//		}
//
//		private void DisplayError(string msg)
//		{
//			EditorUtility.DisplayDialog("Error", msg, "OK");
//		}
//
//		private const string SCRIPT_NAME = "AssetBundleCreator";
//		private const string KEY_DESTINATION = SCRIPT_NAME + "_Destination";
//		private const string KEY_FOLDER_GUID = SCRIPT_NAME + "_Folder";
//		private const string KEY_ANDROID = SCRIPT_NAME + "_BuildAndroid";
//		private const string KEY_IOS = SCRIPT_NAME + "_BuildIOS";
//		private const string KEY_STANDALONE = SCRIPT_NAME + "_BuildStandalone";
//
//        private void LoadPrefs()
//        {
//			_destinationPath = EditorPrefs.GetString(KEY_DESTINATION);
//			_buildTargets[BuildTarget.Android] = EditorPrefs.GetBool(KEY_ANDROID);
//			_buildTargets[BuildTarget.iOS] = EditorPrefs.GetBool(KEY_IOS);
//			_buildTargets[BuildTarget.WebPlayer] = EditorPrefs.GetBool(KEY_STANDALONE);
//
//			string guid = EditorPrefs.GetString(KEY_FOLDER_GUID);
//			if (guid != "")
//			{
//				string path = AssetDatabase.GUIDToAssetPath(guid);
//				_folder = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
//			}
//        }
//
//		private void SavePrefs()
//		{
//			EditorPrefs.SetString(KEY_DESTINATION, _destinationPath);
//			EditorPrefs.SetBool(KEY_ANDROID, _buildTargets[BuildTarget.Android]);
//			EditorPrefs.SetBool(KEY_IOS, _buildTargets[BuildTarget.iOS]);
//			EditorPrefs.SetBool(KEY_STANDALONE, _buildTargets[BuildTarget.WebPlayer]);
//
//			string path = AssetDatabase.GetAssetPath(_folder);
//			string guid = AssetDatabase.AssetPathToGUID(path);
//			EditorPrefs.SetString(KEY_FOLDER_GUID, guid);
//		}
//	}
//}
//
