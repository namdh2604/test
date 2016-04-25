using System;
using System.IO;
using UnityEngine;

namespace Voltage.Story.Import.AnchorImport
{
	/**
	 * An import process for photoshop documents containing a single anchor layer.
	 * This process will use this anchor metadata to create left and right character anchors,
	 * attached to the provided parent transform.  An example of a target source document would be
	 * witches-art/trunk/Characters/main_characters/placement_mockup_main.psd
	 * Note: This currently does no scaling, so the source document resolution should match your target resolution
	 **/
    public class UGUIAnchorImport
    {
        public UGUIAnchorImport()
        {
        }

        public void DoImport(string lexPath, RectTransform parent)
        {
			Vector2 coords = ComputeAnchorCoords(lexPath);
			CreateAnchors(coords, parent);
        }

		private Vector2 ComputeAnchorCoords(string lexPath)
		{
			string path = LexUtils.ExtractLexToTempPath(lexPath);
			string lexDataPath = GetLexDataPath(path);
			Parser lexParser = new Parser(File.ReadAllText(lexDataPath));
			return lexParser.GetAnchorCoordinates();
		}

		private string GetLexDataPath(string containingFolderPath)
		{
            var files = Directory.GetFiles(containingFolderPath, Parser.CONFIG_FILE_PATTERN);
            if (files.Length != 1)
            {
				throw new Exception("No Lex Data found!");
            }

			return files[0];
		}

		private void CreateAnchors(Vector2 coords, RectTransform parent)
		{
			CreateAnchor("Left Character", coords, parent);
			GameObject rightAnchor = CreateAnchor("Right Character", new Vector2(-coords.x, coords.y), parent);
			// mirror the right anchor
			rightAnchor.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f));
		}

		private GameObject CreateAnchor(string name, Vector2 coords, RectTransform parent)
		{
			GameObject anchor = new GameObject(name);
			RectTransform rt = anchor.AddComponent<RectTransform>();
			// anchored to the horizontal center and the vertical bottom
			rt.anchorMin = new Vector2(0.5f, 0.0f);
			rt.anchorMax = new Vector2(0.5f, 0.0f);
			rt.anchoredPosition = coords;

			rt.SetParent(parent, false);

			return anchor;
		}
    }
}