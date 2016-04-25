using System;

namespace Voltage.Story.Models.Nodes.Controllers
{
    using Voltage.Witches.Models.Avatar;
    using Voltage.Story.StoryPlayer;
    using Voltage.Story.Models.Data;
    using Voltage.Story.General;
    using Voltage.Story.Views;
    using Voltage.Common.Logging;
    using Voltage.Story.Mapper;

    public class EventIllustrationNodeController : BaseNodeController
    {
        private ILayoutDisplay _display;
        private IParser<string> _textParser;
        private readonly AvatarNameUtility _avatarNameUtil;

        public EventIllustrationNodeController(ILayoutDisplay display, IParser<string> textParser, AvatarNameUtility avatarNameUtil, ILogger logger) : base(logger)
        {
            if (display == null)
            {
                throw new ArgumentNullException("display");
            }

            if (textParser == null)
            {
                throw new ArgumentException("textParser");
            }

            _display = display;
            _textParser = textParser;
            _avatarNameUtil = avatarNameUtil;
        }

        public override void Execute(INode node, IStoryPlayer storyPlayer)
        {
            EINode eiNode = GetNode(node);
			_display.DisplayEI(GetViewData(eiNode), null, (index) => {storyPlayer.Next();return true;});
        }

        private EINode GetNode(INode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            EINode eiNode = node as EINode;
            if (eiNode == null)
            {
                throw new Exception("Failure: expected EventIllustrationNode, received: " + node.GetType().Name);
            }

            return eiNode;
        }

        private EventIllustrationNodeViewData GetViewData(EINode node)
        {
            bool isAvatar = AvatarNameUtility.IsAvatarName(node.Speaker);
            EventIllustrationNodeViewData data = new EventIllustrationNodeViewData();
            data.image = node.image;
            data.speaker = (isAvatar) ? _avatarNameUtil.GetDisplayableName() : data.speaker;
                
            data.speechBox = node.speechBox;
            data.text = _textParser.Parse(node.text);
            data.speakerIsAvatar = isAvatar;

            return data;
        }
    }
}

