using System.Collections.Generic;

namespace Voltage.Story.Import.CharacterImport.Model
{
    public class LayerInfo
    {
        public string name;
        public bool isImage;
        public int opacity;
        public UnityEngine.Rect absolute;
        public List<LayerInfo> children;
    }
}

