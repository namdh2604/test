using System;
using Voltage.Common.Logging;
using Voltage.Story.User;
using Voltage.Story.General;
using Voltage.Story.StoryPlayer;
using Voltage.Story.Models.Nodes;
using Voltage.Story.Views;
using Voltage.Witches.Models.Avatar;
using Voltage.Witches.Story;

namespace Voltage.Story.Models.Nodes.Controllers
{
//	using Voltage.Witches.Models;
	using Voltage.Story.Mapper;
	using Voltage.Story.Models.Data;
	using Voltage.Story.Text;
    using Voltage.Story.Models.Nodes.Helpers;

	public class DialogueNodeController : BaseNodeController		
	{
		public ILayoutDisplay LayoutDisplay { get; private set; }
        private readonly StoryMusicPlayer _musicPlayer;
		public IParser<string> TextParser { get; private set; }
        private readonly AvatarNameUtility _avatarNameUtil;

        private const string THOUGHT_BUBBLE = "Thought Center";

		private IStoryPlayer _storyPlayer;

        public DialogueNodeController(IMapping<string> variableMapper, ILayoutDisplay display, StoryMusicPlayer musicPlayer, AvatarNameUtility avatarNameUtil, ILogger logger) : base(logger)
		{
            _avatarNameUtil = avatarNameUtil;
			TextParser = new VariableTextParser (variableMapper, Logger);	// TODO: maybe pass in TextParser instead???
			LayoutDisplay = display;
            _musicPlayer = musicPlayer;
		}

		public override void Execute(INode node, IStoryPlayer storyPlayer)
		{
			DialogueNode dialogueNode = node as DialogueNode;		// FIXME: remove this cast
            _musicPlayer.Play(dialogueNode.Music);

			if(dialogueNode != null && LayoutDisplay != null && storyPlayer != null)
			{
				_storyPlayer = storyPlayer;
                DialogueNodeViewData viewData = GetViewData(dialogueNode, TextParser, _avatarNameUtil);
	
				LayoutDisplay.DisplayDialogue(viewData, null, (index) => {storyPlayer.Next();return true;});
			}
		}

		public void HideSpeechBox()
		{
			if (_storyPlayer.CurrentNode == null & _storyPlayer.CurrentNode.Next == null) {
				LayoutDisplay.HideSpeechBox ();
			}
		}

        public static DialogueNodeViewData GetViewData (DialogueNode dialogueNode, IParser<string> textParser, AvatarNameUtility avatarNameUtil)
		{
            if ((dialogueNode == null) || (textParser == null))
            {
                throw new ArgumentNullException();
            }

			DialogueNodeViewData data = new DialogueNodeViewData();
            bool isAvatar = AvatarNameUtility.IsAvatarName(dialogueNode.Speaker);

            // HACK - Hung Nguyen
            // Production has a huge number of thought center setup up incorrectly.  This change will force all thought center to be avatar thought.
            // If production changes this behavior need to remove this change.
            if (dialogueNode.SpeechBox == THOUGHT_BUBBLE)
            {
                isAvatar = true;
            }

            data.Speaker = isAvatar ? avatarNameUtil.GetDisplayableName() : dialogueNode.Speaker;

            data.Text = textParser.Parse(dialogueNode.Text);
            data.SpeakerIsAvatar = isAvatar;

            Options options = new Options();
            options.Background = dialogueNode.Background;
            options.SpeechBox = dialogueNode.SpeechBox;

            options.Left = CreateCharacter(dialogueNode.LeftCharacter);
            options.Right = CreateCharacter(dialogueNode.RightCharacter);

			data.Data = options;

			return data;
		}

        private static CharOptions CreateCharacter(CharacterAttribute character)
        {
            CharOptions options = new CharOptions();
            if (character == null)
            {
                options.Enabled = false;
            }
            else
            {
                options.Enabled = character.Enabled;
                options.Name = character.Name;
                options.Pose = character.Pose;
                options.Outfit = character.Outfit;
                options.Expression = character.Expression;
            }

            return options;
        }
	}
		

}
