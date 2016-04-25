using UnityEngine;
using UnityEditor;

namespace Voltage.Story.Import.CharacterImport
{
    public class UGUIBatchCharImportWindow : EditorWindow
    {
        string _fromPath, _toPath;
        const string KEY_FROM_DIR = "batchCharGUI_From";
        const string KEY_TO_DIR = "batchCharGUI_To";

        [MenuItem("Import/Character/Batch Import")]
        private static void Init()
        {
            UGUIBatchCharImportWindow window = EditorWindow.GetWindow<UGUIBatchCharImportWindow>("Batch Import");
            window.LoadPrefs();
            window.Show();
        }

        void OnGUI()
        {
            _fromPath = EditorGUILayout.TextField("Import Folder: ", _fromPath);
            if (GUILayout.Button("Set"))
            {
                GUI.FocusControl(string.Empty);
                string path = EditorUtility.OpenFolderPanel("Locate Lex", _fromPath, string.Empty);
                if (path != string.Empty)
                {
                    _fromPath = path;
                }
            }

            _toPath = EditorGUILayout.TextField("Destination: ", _toPath);
            if (GUILayout.Button("Set"))
            {
                GUI.FocusControl(string.Empty);
                string path = EditorUtility.SaveFolderPanel("Character Export Destination", _toPath, string.Empty);

                if (path != string.Empty)
                {
                    _toPath = path;
                }
            }

            if (GUILayout.Button("Run"))
            {
                if (!CheckRequirements())
                {
                    return;
                }

                SavePrefs();
                UGUICharacterImport.ImportAll(_fromPath, _toPath);
            }
        }

        private void LoadPrefs()
        {
            _fromPath = EditorPrefs.GetString(KEY_FROM_DIR);
            _toPath = EditorPrefs.GetString(KEY_TO_DIR);
        }

        private void SavePrefs()
        {
            EditorPrefs.SetString(KEY_FROM_DIR, _fromPath);
            EditorPrefs.SetString(KEY_TO_DIR, _toPath);
        }

        private bool CheckRequirements()
        {
            return ((_fromPath != string.Empty) && (_toPath != string.Empty));
        }
    }
}