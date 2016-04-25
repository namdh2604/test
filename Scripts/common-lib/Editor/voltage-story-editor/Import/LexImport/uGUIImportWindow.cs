using UnityEngine;
using UnityEditor;
using System.IO;

using Diagnostics = System.Diagnostics;

namespace Voltage.Story.Import.LexImport
{
    public class UGUIImportWindow : EditorWindow
    {
        private string _path;
		private float _scaleFactor = 1f;

        const string KEY_DATA_DIR = "Lex_DataDir";

        [MenuItem("Import/Lex Import")]
        static void Init()
        {
            UGUIImportWindow window = EditorWindow.GetWindow<UGUIImportWindow>("UGUI Import");
            window.LoadPrefs();
            window.Show();
        }

        void OnGUI()
        {
            _path = EditorGUILayout.TextField("Lex: ", _path);
            if (GUILayout.Button("Set Lex"))
            {
                GUI.FocusControl(string.Empty);
                string path = EditorUtility.OpenFilePanel("Open Lex Archive", _path, "lex");
                if (path != string.Empty)
                {
                    _path = path;
                }
            }

			_scaleFactor = EditorGUILayout.FloatField("Scale Factor: ", _scaleFactor);	// FIXME: Check for <= zero input
			EditorGUILayout.Space();

            if (GUILayout.Button("Run"))
            {
                SavePrefs();
                string destination = "Resources";

                UGUIImporter importer = new UGUIImporter();
                importer.doImport(_path, destination, _scaleFactor);
            }
        }

        void SavePrefs()
        {
            EditorPrefs.SetString(KEY_DATA_DIR, _path);
        }

        void LoadPrefs()
        {
            _path = EditorPrefs.GetString(KEY_DATA_DIR, "");
        }

    }
}