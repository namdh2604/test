
using System;
using System.Collections.Generic;

namespace Voltage.Story.StoryDivisions
{
	public class Arc : BaseIdentifiableContainer<Scene>
	{

		public IList<Scene> Scenes { get { return new List<Scene> (SceneMap.Values); } }
		public IDictionary<string,Scene> SceneMap { get { return ElementMap; } }

		public Scene GetScene (string scene)
		{
			if(SceneMap != null)
			{
				if(SceneMap.ContainsKey(scene))
				{
					return SceneMap[scene];
				}
			}
			
			return default(Scene);
		}


		public Arc (string name, params Scene[] scenes) : base (name) //, scenes) {}
		{
			if(scenes != null)
			{
				foreach (Scene scene in scenes) 
				{
					ElementMap.Add (scene.Name, scene);
				}
			}
		}


		public IList<Scene> SortedScenesBy(Comparison<Scene> comparer)		// Func<T,T,int>
		{
//			if(Scenes != null && Scenes.Count > 0)
			{
				List<Scene> scenes = new List<Scene>(Scenes);
				scenes.Sort(comparer);
				return scenes;
			}
		}

		public IList<Scene> SortedScenesByID	// NOTE: likely alphabetical anyway
		{
			get
			{
//				if(Scenes != null && Scenes.Count > 0)
				{
					return SortedScenesBy((sceneA,sceneB) => sceneA.ID.CompareTo(sceneB.ID));
				}
			}
		}

		public IList<Scene> SortedScenesByName
		{
			get
			{
//				if(Scenes != null && Scenes.Count > 0)
				{
					return SortedScenesBy((sceneA,sceneB) => sceneA.Name.CompareTo(sceneB.Name));
				}
			}
		}



	}
}





