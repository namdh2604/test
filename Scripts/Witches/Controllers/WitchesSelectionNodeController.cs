namespace Voltage.Witches.Controllers
{
    using Voltage.Witches.Models.Avatar;
    using Voltage.Witches.Models;
    using Voltage.Story.Models.Nodes;
    using Voltage.Story.Models.Nodes.Controllers;
    using Voltage.Story.Mapper;
    using Voltage.Story.Views;
    using Voltage.Story.Effects;
    using Voltage.Common.Logging;
    using Voltage.Witches.Story;

    public class WitchesSelectionNodeController : SelectionNodeController
    {
        private readonly Player _player;
        private readonly IEffectResolver _effectResolver;

        public WitchesSelectionNodeController(Player player, IEffectResolver effectResolver, IMapping<string> variableMapper, AvatarNameUtility avatarNameUtil,
            ILayoutDisplay display, StoryMusicPlayer musicPlayer, ILogger logger) 
            : base(variableMapper, display, musicPlayer, avatarNameUtil, logger)
        {
            _player = player;
            _effectResolver = effectResolver;
        }

        protected override void OnSelection(Voltage.Story.StoryDivisions.Scene scene, SelectionNode node, int selectedIndex)
        {
            base.OnSelection(scene, node, selectedIndex);

            OptionNode selectedNode = node.GetBranch(selectedIndex) as OptionNode;
            _effectResolver.Resolve(selectedNode.Effects);
            _player.RecordSelection(scene, node, selectedIndex);

            _player.Serialize();
        }

    }
}

