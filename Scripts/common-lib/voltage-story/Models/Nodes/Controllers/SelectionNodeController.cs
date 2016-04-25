using System;
using System.Collections.Generic;

//
//using Voltage.Story.Models.Nodes;
//using Voltage.Story.Models.Nodes.Controllers;
//

namespace Voltage.Story.Models.Nodes.Controllers
{
    using Voltage.Story.Models.Nodes.Extensions;
    using Voltage.Witches.Models.Avatar;

	using Voltage.Common.Logging;

	using Voltage.Story.StoryPlayer;
	using Voltage.Story.Views;

	using Voltage.Story.Mapper;
	using Voltage.Story.Models.Data;

	using Voltage.Story.General;
	using Voltage.Story.Text;

	using Voltage.Story.Effects;
    using Voltage.Witches.Story;

	public class SelectionNodeController : BaseNodeController
	{
		public ILayoutDisplay LayoutDisplay { get; private set; }
        private readonly StoryMusicPlayer _musicPlayer;
		public IParser<string> TextParser { get; private set; }
        private readonly AvatarNameUtility _avatarNameUtil;

		public SelectionNodeController(IMapping<string> variableMapper, ILayoutDisplay display, StoryMusicPlayer musicPlayer, AvatarNameUtility avatarNameUtil, ILogger logger) : base(logger)
		{
			LayoutDisplay = display;
            _musicPlayer = musicPlayer;
            _avatarNameUtil = avatarNameUtil;

			TextParser = new VariableTextParser (variableMapper, Logger);		// TODO: maybe pass in TextParser instead???
		}
		
		public override void Execute(INode node, IStoryPlayer storyPlayer)
		{
			SelectionNode selectionNode = node as SelectionNode;		// FIXME: remove cast
            _musicPlayer.Play(selectionNode.Music);

			if (selectionNode != null && LayoutDisplay != null && storyPlayer != null)
			{
				LayoutDisplay.DisplaySelection(GetViewData(selectionNode), null, CreateSelectionHandler(storyPlayer, selectionNode));
			}
		}

		protected virtual SelectionNodeViewData GetViewData (SelectionNode selectionNode)
		{
			SelectionNodeViewData data = new SelectionNodeViewData ();
			
			if (TextParser != null && selectionNode != null)
			{
				data.Options = new List<INode>(selectionNode.Branches).ConvertAll((node) => node as OptionNode).ConvertAll((option) => TextParser.Parse (option.Text));	// FIXME: hmm not pretty or safe...at least make this into a function instead of a liner

                // NOTE: This is not a great pattern. Having to switch off types here is definitely a smell, but its hard to extract this logic.
                // In the future, nodes could be wrapped to include their controller and view data
                IHasBackdrop previousDisplayableNode = selectionNode.FindNodeBefore(x => x is IHasBackdrop) as IHasBackdrop;
                if (previousDisplayableNode == null)
                {
                    return data;
                }
                else if (previousDisplayableNode.GetType() == typeof(DialogueNode))
				{
                    DialogueNode dialogueNode = previousDisplayableNode as DialogueNode;
                    data.DialogueNode = DialogueNodeController.GetViewData(dialogueNode, TextParser, _avatarNameUtil);
				}
                else if (previousDisplayableNode.GetType() == typeof(EstablishingNode))
                {
                    EstablishingNode establishingNode = previousDisplayableNode as EstablishingNode;
                    data.DialogueNode = EstablishingNodeController.GetViewData(establishingNode);
                }
				else
				{
                    throw new Exception("Unrecognized Backdrop Node: " + previousDisplayableNode.GetType());
				}
			}
			else
			{
				Logger.Log ("SelectionNodeController::GetViewData >>> No VariableMapper or Node", LogLevel.WARNING);
			}
			
			return data ;
		}





		private Func<int,bool> CreateSelectionHandler(IStoryPlayer player, SelectionNode node)
		{
			return delegate(int selectedIndex) 
			{
//				if(node != null)
//				{
//					ResolveEffects(node.Branches[selectedIndex] as OptionNode);
//				}

				OnSelection(player.CurrentScene, node, selectedIndex);
				player.Next(selectedIndex);
				return true;
			};
		}

//		private void ResolveEffects(OptionNode optionNode)
//		{
//			if(optionNode != null && Resolver != null)
//			{
//				System.Console.WriteLine ("resolving");
//				Resolver.Resolve(optionNode.Effects);
//			}
//		}

        protected virtual void OnSelection(Voltage.Story.StoryDivisions.Scene scene, SelectionNode node, int selectedIndex)
		{
			Logger.Log ("SelectionNodeController::OnSelection: " + selectedIndex, LogLevel.INFO);
//			if(node != null)
//			{
//				ResolveEffects(node.Branches[selectedIndex]);
//			}
		}
	}
}

