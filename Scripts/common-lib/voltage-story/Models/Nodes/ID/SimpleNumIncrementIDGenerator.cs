namespace Voltage.Story.Models.Nodes.ID
{
	public class SimpleNumIncrementIDGenerator : IIDGenerator<string,INode>	// NOTE: you need to create a NEW IDMaker instance for every scene
	{
		public int NodeCount { get; private set; }
		private readonly string _padding;

		public SimpleNumIncrementIDGenerator(int startValue=0, int padding=5)
		{
			_padding = string.Format ("D{0}", padding);
			NodeCount = startValue;
		}

		public string GenerateID () { return GenerateID (null); }
		public string GenerateID (INode node)
		{
			string id = NodeCount.ToString (_padding);
			NodeCount += 1;

			return id;
		}

	}

	public class RouteArcSceneNumIncrementIDGenerator : IIDGenerator<string,INode>
	{
		public int NodeCount { get; private set; }
		private readonly string _padding;

		public string Route { get; private set; }
		public string Arc { get; private set; }
		public string Scene { get; private set; }

		public RouteArcSceneNumIncrementIDGenerator (string route, string arc, string scene, int startValue=0, int padding=5)
		{
			Route = route;
			Arc = arc;
			Scene = scene;

			_padding = string.Format ("D{0}", padding);
			NodeCount = startValue;
		}

		public string GenerateID () { return GenerateID (null); }
		public string GenerateID (INode node)
		{
			string id = NodeCount.ToString (_padding);
			NodeCount += 1;

			return string.Format("{0}_{1}_{2}_{3}", Route, Arc, Scene, id);
		}
	}
}

