using System;

namespace Voltage.Story.Models.Nodes
{
	public class EINode : INode, IHasSpeaker
    {
        public INode Next { get; set; }
        public INode Previous { get; set; }

        public string image;
		public string Speaker { get; set; }
        public string speechBox;
        public string text;

        public string ID { get; set; }

        public override string ToString()
        {
            return string.Format("[EINode: image:{0}, speaker:{1}, speechBox:{2}, text:{3}, ID={4}]", image, Speaker, speechBox, text, ID);
        }
    }
}

