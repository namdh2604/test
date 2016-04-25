using System.Collections.Generic;

namespace Voltage.Story.Import.CharacterImport.Model
{
    internal class OutfitPair
    {
        public List<LayerInfo> top;
        public List<LayerInfo> bottom;

        public OutfitPair()
        {
            top = new List<LayerInfo>();
            bottom = new List<LayerInfo>();
        }
    }
}

