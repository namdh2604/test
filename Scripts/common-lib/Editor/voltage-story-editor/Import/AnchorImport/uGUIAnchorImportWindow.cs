using UnityEngine;
using UnityEditor;

using UnityEngine.UI;
using Voltage.Common.Utilities;

namespace Voltage.Story.Import.AnchorImport
{
	public class UGUIAnchorImportWindow : EditorWindow
	{
		[MenuItem("Import/UGUI Anchors")]
		private static void Init()
		{
			UGUIAnchorImportWindow window = EditorWindow.GetWindow<UGUIAnchorImportWindow>("Import Character Anchors");
			window.LoadPrefs();
            window.Show();
		}

        private Canvas _targetCanvas;
        private string _path;
        private UGUIAnchorImport _importer;

		private const string IMPORT_PREFIX = "AnchorImport";
		private const string KEY_PATH = IMPORT_PREFIX + "_path";
		private const string KEY_CANVAS_REF = IMPORT_PREFIX + "_canvasRef";

		void OnEnable()
		{
			if (_importer == null)
			{
				_importer = new UGUIAnchorImport();
			}
		}

		void LoadPrefs()
		{
			_path = EditorPrefs.GetString(KEY_PATH);
			int refId = EditorPrefs.GetInt(KEY_CANVAS_REF);
			_targetCanvas = EditorUtility.InstanceIDToObject(refId) as Canvas;
		}

		void SavePrefs()
		{
			EditorPrefs.SetString(KEY_PATH, _path);
			EditorPrefs.SetInt(KEY_CANVAS_REF, _targetCanvas.GetInstanceID());
		}

		private void OnGUI()
		{
            _path = EditorGUILayout.TextField("Lex Path", _path);
			GUIUtils.RightJustify(delegate() {
                if (GUILayout.Button("Set Lex", GUILayout.MaxWidth(100)))
                {
                    GUI.FocusControl(string.Empty);
                    string resultPath = EditorUtility.OpenFilePanel("Set Lex Source", _path, "lex");
                    if (resultPath != string.Empty)
                    {
                        _path = resultPath;
                    }
                }
			});

            EditorGUILayout.Space();

            Canvas newCanvas = EditorGUILayout.ObjectField("CanvasTarget", _targetCanvas, typeof(Canvas), true) as Canvas;
			if (newCanvas != null)
			{
				_targetCanvas = newCanvas;
			}

            EditorGUILayout.Space();

			GUIUtils.Center(delegate() {
                if (GUILayout.Button("Run", GUILayout.MaxWidth(100)))
                {
					RunCommand();
                }
			});
		}

		private void RunCommand()
		{
			if (_path == string.Empty || _path == null)
			{
				EditorUtility.DisplayDialog("Error", "Missing valid lex pathname", "OK");
				return;
			}

			if (_targetCanvas == null)
			{
				EditorUtility.DisplayDialog("Error", "Missing parent canvas", "OK");
				return;
			}

            _importer.DoImport(_path, _targetCanvas.transform as RectTransform);
			SavePrefs();
			EditorUtility.DisplayDialog("Success", "Anchors created", "OK");
		}
	}
}