using System;

namespace Voltage.Story.Models.Nodes.Controllers
{
    using Voltage.Story.StoryPlayer;
    using Voltage.Story.Views;
    using Voltage.Common.Logging;
    using Voltage.Story.Models.Data;
    using Voltage.Story.Models.Nodes.Helpers;
    using Voltage.Witches.Story;

    public class EstablishingNodeController : BaseNodeController
    {
        private readonly ILayoutDisplay _display;
        private readonly StoryMusicPlayer _musicPlayer;

        public EstablishingNodeController(ILayoutDisplay display, StoryMusicPlayer musicPlayer, ILogger logger) : base(logger)
        {
            if (display == null)
            {
                throw new ArgumentNullException("display");
            }

            _display = display;
            _musicPlayer = musicPlayer;
        }

        public override void Execute(INode node, IStoryPlayer storyPlayer)
        {
            EstablishingNode establishingNode = GetNode(node);
            _musicPlayer.Play(establishingNode.Music);
			_display.DisplayDialogue(GetViewData(establishingNode), null, (index) => {storyPlayer.Next();return true;});
        }

        private EstablishingNode GetNode(INode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            EstablishingNode resolvedNode = node as EstablishingNode;
            if (resolvedNode == null)
            {
                throw new Exception("Failure: expected EstablishingNode, received: " + node.GetType().Name);
            }

            return resolvedNode;
        }

        public static DialogueNodeViewData GetViewData(EstablishingNode node)
        {
            DialogueNodeViewData data = new DialogueNodeViewData();
            data.Text = null;
            data.Speaker = null;
            data.Data = new Options();
            Options options = new Options();
            options.Background = node.Background;
            options.Left = GetCharacterOptions(node.LeftCharacter);
            options.Right = GetCharacterOptions(node.RightCharacter);
            data.Data = options;

            return data;
        }

        private static CharOptions GetCharacterOptions(CharacterAttribute rawAttributes)
        {
            if (rawAttributes == null)
            {
                CharOptions minOptions = new CharOptions();
                minOptions.Enabled = false;

                return minOptions;
            }

            CharOptions result = new CharOptions();
            result.Enabled = rawAttributes.Enabled;
            result.Name = rawAttributes.Name;
            result.Pose = rawAttributes.Pose;
            result.Outfit = rawAttributes.Outfit;
            result.Expression = rawAttributes.Expression;

            return result;
        }
    }
}

