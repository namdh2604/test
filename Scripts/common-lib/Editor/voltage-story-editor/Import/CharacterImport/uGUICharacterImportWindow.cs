using UnityEditor;
using UnityEngine;

using System.IO;

using Voltage.Common.Utilities;

namespace Voltage.Story.Import.CharacterImport
{
	public class UGUICharacterImportWindow : EditorWindow
	{
	    private string _path;
	    private string _destinationPath;
	    private float _scaleFactor;

	    private const string KEY_DATA_DIR = "Lex_CharDataDir";
	    private const string KEY_DATA_DEST_DIR = "Lex_CharDataDestDir";

        [MenuItem("Import/Character/Single Import")]
	    static void Init()
	    {
	        UGUICharacterImportWindow window = EditorWindow.GetWindow<UGUICharacterImportWindow>("UGUI Character Import");
	        window.LoadPrefs();
	        window.Show();
	    }

	    void OnGUI()
	    {
	        _path = EditorGUILayout.TextField("Lex: ", _path);
			GUIUtils.RightJustify(delegate() {
		        if (GUILayout.Button("Set Lex", GUILayout.MaxWidth(100)))
		        {
		            GUI.FocusControl(string.Empty);
		            string path = EditorUtility.OpenFilePanel("Locate Lex", _path, "lex");
		            if (path != string.Empty)
		            {
		                _path = path;
		            }
		        }
			});

			EditorGUILayout.Space();

	        _destinationPath = EditorGUILayout.TextField("Destination: ", _destinationPath);
			GUIUtils.RightJustify(delegate() {
		        if (GUILayout.Button("Set Destination", GUILayout.MaxWidth(100)))
		        {
		            GUI.FocusControl(string.Empty);
		            string path = EditorUtility.SaveFolderPanel("Character Export Destination", _destinationPath, string.Empty);

		            if (path != string.Empty)
		            {
		                _destinationPath = path;
		            }
		        }
			});

			EditorGUILayout.Space();

	        _scaleFactor = EditorGUILayout.FloatField("Scale Factor: ", _scaleFactor);

			EditorGUILayout.Space();

			GUIUtils.Center(delegate() {
		        if (GUILayout.Button("Run", GUILayout.MaxWidth(100)))
		        {
		            if (!CheckRequirements())
		            {
		                return;
		            }

		            SavePrefs();
		            UGUICharacterImport importer = new UGUICharacterImport();
		            importer.doImport(_path, _destinationPath, Path.GetFileNameWithoutExtension(_path), _scaleFactor);
		        }
			});
	    }

	    private bool CheckRequirements()
	    {
	        string error = string.Empty;

	        if (!File.Exists(_path))
	        {
	            error = "Lex file does not exist!";
	        }

	        if (!Directory.Exists(_destinationPath))
	        {
	            error = "Destination Path does not exist!";
	        }

	        if (error != string.Empty)
	        {
	            EditorUtility.DisplayDialog("Error", error, "OK");
	            return false;
	        }

	        return true;
	    }

	    void SavePrefs()
	    {
	        EditorPrefs.SetString(KEY_DATA_DIR, _path);
	        EditorPrefs.SetString(KEY_DATA_DEST_DIR, _destinationPath);
	    }

	    void LoadPrefs()
	    {
	        _path = EditorPrefs.GetString(KEY_DATA_DIR, "");
	        _destinationPath = EditorPrefs.GetString(KEY_DATA_DEST_DIR, "");
	        _scaleFactor = 1.0f;
	    }
	}
}