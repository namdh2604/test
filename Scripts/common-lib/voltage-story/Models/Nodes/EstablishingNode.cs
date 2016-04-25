using System.Collections.Generic;

namespace Voltage.Story.Models.Nodes
{
    using Voltage.Story.Models.Nodes.Helpers;

    public class EstablishingNode : INode, IHasCharacters, IHasBackdrop, IHasMusic
    {
        public INode Next { get; set; }
        public INode Previous { get; set; }

        public CharacterAttribute LeftCharacter { get; set; }
        public CharacterAttribute RightCharacter { get; set; }
        public string Background { get; set; }
        public string Music { get; set; }

        public string ID { get; set; }

        public override string ToString()
        {
            return string.Format("[EstablishingNode: LeftCharacter={0}, RightCharacter={1}, Background={2}, ID={3}]", LeftCharacter, RightCharacter, Background, ID);
        }
    }
}

