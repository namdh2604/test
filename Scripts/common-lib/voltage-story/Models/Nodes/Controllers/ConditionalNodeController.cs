using System;

namespace Voltage.Story.Models.Nodes.Controllers
{
	using Voltage.Story.StoryPlayer;

	using Voltage.Story.General;
	using Voltage.Story.Expressions;
    using Voltage.Witches.Exceptions;

	public class ConditionalNodeController : BaseNodeController // NOTE: maybe just INodeController
	{
		public IParser<ExpressionState> Parser { get; private set; }
		public IExpressionFactory Factory { get; private set; }

		public Action OnFailure { get; private set; }

		public ConditionalNodeController (IParser<ExpressionState> parser, IExpressionFactory factory, Action onFailure)	// pass in ILogger ???
		{
			Parser = parser;
			Factory = factory;

			OnFailure = onFailure;
		}

		public override void Execute(INode node, IStoryPlayer storyPlayer)
		{	
			ConditionalNode conditionalNode = node as ConditionalNode;
            if (conditionalNode == null)
            {
                throw new WitchesException("Invalid node being processed as conditional");
            }

			for (int i=0; i < conditionalNode.Branches.Count; i++)
			{
				BranchNode branch = conditionalNode.Branches[i] as BranchNode;
				if(ProcessBranch(branch) && storyPlayer != null)
				{
					storyPlayer.Next(i);
                    return;
				}
			}

            storyPlayer.Next();
		}

		private bool ProcessBranch (BranchNode branch)
		{
            if (branch == null)
            {
                throw new WitchesException("Invalid branch node");
            }

			ExpressionState state = Parser.Parse(branch.Expression);
			IExpression expression = Factory.CreateExpression(state);

			if(expression != null)
			{
				return expression.Evaluate();
			}

			return false;
		}
	}
}
