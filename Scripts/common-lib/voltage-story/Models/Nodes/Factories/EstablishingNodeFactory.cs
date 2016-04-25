using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Voltage.Story.Models.Nodes;
using Voltage.Story.Models.Nodes.Extensions;
using Voltage.Story.Models.Nodes.Helpers;

namespace Voltage.Story.Models.Nodes.Factories
{
    public class EstablishingNodeFactory
    {
        public EstablishingNodeFactory()
        {
        }

        public EstablishingNode Create(JToken token, string id, INode previous)
        {
            EstablishingNode node = new EstablishingNode();
            node.ID = id;
            node.Previous = previous;

            node.Background = GetBackground(previous, token["data"]);
            node.Music = DialogueNode.GetMusic(previous, token["data"]);
            ResolveCharacters(previous, token["data"], node);

            return node;
        }

        private string GetBackground(INode previous, JToken data)
        {
            if ((data != null) && (data["background"] != null))
            {
                return data.Value<string>("background");
            }

            IHasBackdrop node = previous.FindNodeBefore(x => x is IHasBackdrop) as IHasBackdrop;
            if (node == null)
            {
                throw new Exception("No valid background found!");
            }

            return node.Background;
        }

        private void ResolveCharacters(INode previous, JToken data, EstablishingNode currentNode)
        {
            IHasCharacters previousCharacterData = previous.FindNodeBefore(x => x is IHasCharacters) as IHasCharacters;

            JToken leftProperties = (data != null) ? data["left"] : null;
            JToken rightProperties = (data != null) ? data["right"] : null;

			CharacterAttribute leftChar = (previousCharacterData == null) ? null : previousCharacterData.LeftCharacter;
			CharacterAttribute rightChar = (previousCharacterData == null) ? null : previousCharacterData.RightCharacter;

			currentNode.LeftCharacter = CombineAttributes(leftChar, leftProperties);
			currentNode.RightCharacter = CombineAttributes(rightChar, rightProperties);
        }

        private CharacterAttribute CombineAttributes(CharacterAttribute original, JToken newProperties)
        {
            CharacterAttribute result = null;
            if (original != null)
            {
                result = (CharacterAttribute)original.Clone();
            }
            else
            {
                result = new CharacterAttribute();
                
            }

            if (newProperties == null)
            {
                return result;
            }

            if (newProperties["enabled"] != null)
            {
                result.Enabled = newProperties.Value<bool>("enabled");
            }

            if (newProperties["name"] != null)
            {
                result.Name = newProperties.Value<string>("name");
            }

            if (newProperties["outfit"] != null)
            {
                result.Outfit = newProperties.Value<string>("outfit");
            }

            if (newProperties["pose"] != null)
            {
                result.Pose = newProperties.Value<string>("pose");
            }

            if (newProperties["expression"] != null)
            {
                result.Expression = newProperties.Value<string>("expression");
            }

            return result;
        }
    }
}

