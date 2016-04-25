namespace Voltage.Story.Models.Nodes
{
    using Voltage.Story.Models.Nodes.Helpers;

    public interface IHasCharacters
    {
        CharacterAttribute LeftCharacter { get; }
        CharacterAttribute RightCharacter { get; }
    }
}

