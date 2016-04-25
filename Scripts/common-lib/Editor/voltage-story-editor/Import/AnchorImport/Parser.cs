using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine;

namespace Voltage.Story.Import.AnchorImport
{
	/**
	 * Parses the lex data format for anchor metadata
	 **/
	public class Parser
	{
        public const string CONFIG_FILE_PATTERN = "*.json";
        private const string ANCHOR_LAYER_NAME = "Rectangle_1"; // This will likely change in the future -- it is currently what is used in the reference psds, but not the character files themselves

		private JObject _root;

		public Parser(string lexdata)
		{
			_root = JObject.Parse(lexdata);
		}

		public Vector2 GetAnchorCoordinates()
		{
            // locate the anchor layer
            JToken images = _root["images"];
            JToken anchorLayer = FindChild(images, child => child["name"].ToString() == ANCHOR_LAYER_NAME);
            if (anchorLayer == null)
            {
				throw new Exception("Could not locate anchor information");
            }

            float width = _root["container"].Value<float>("width");
            float height = _root["container"].Value<float>("height");

            JToken anchorCoords = anchorLayer["absolute"];

            // calculate the anchor's center x and y
            float x = anchorCoords.Value<float>("x") + anchorCoords.Value<float>("width") / 2.0f;
            float y = height - (anchorCoords.Value<float>("y") + anchorCoords.Value<float>("height") / 2.0f);

            // calculate the anchor's x & y adjusted to (center, bottom)
            x = x - (width / 2.0f);

			return new Vector2(x, y);
		}

        private static JToken FindChild(JToken root, Predicate<JToken> pred)
        {
            foreach (var child in root.Children())
            {
                if (pred(child))
                {
                    return child;
                }
            }

            return null;
        }
	}
}

