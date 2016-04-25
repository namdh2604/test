using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Voltage.Story.Models.Nodes.Factories
{
    public class EventIllustrationNodeFactory
    {
        public EventIllustrationNodeFactory()
        {
        }

        public EINode Create(JToken token, string id, INode previous)
        {
            // deserializing from a json text string would be much easier here,
            // but in the json, the text is an array of objects, rather than a string.
            // The easiest way to remain compatible with DialogueNode is to continue this, for now
            EINode node = new EINode();
            node.image = token.Value<string>("image");
            node.Speaker = token.Value<string>("speaker");
            node.speechBox = token.Value<string>("speechBox");
            node.text = token["text"].ToString();
            node.ID = id;
            node.Previous = previous;

            return node;
        }
    }
}

