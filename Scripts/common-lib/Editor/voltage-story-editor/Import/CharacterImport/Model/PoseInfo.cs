using UnityEngine;
using System.Collections.Generic;

namespace Voltage.Story.Import.CharacterImport.Model
{
    internal class PoseInfo
    {
        public string name;
        public Vector2 offset;
        public List<LayerInfo> baseImages;
        public Dictionary<string, List<LayerInfo>> expressions;
        public Dictionary<string, OutfitPair> outfits;

        public PoseInfo()
        {
            baseImages = new List<LayerInfo>();
            expressions = new Dictionary<string, List<LayerInfo>>();
            outfits = new Dictionary<string, OutfitPair>();
        }
    }
}

