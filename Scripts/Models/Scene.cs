using System.Collections.Generic;

namespace Voltage.Witches.Models
{
	using Voltage.Story.Expressions;

	//TODO This will become deprecated, it'll probably clash with Micahel's code
	public enum LOCK_TYPE
	{
		NONE = 0,
		PROGRESS = 1,
		FAVORABILITY = 2
	}

	public interface IScene
	{
		int Id { get; }
		string Name { get; }
		string SceneInfo { get; }
		bool IsLocked { get; }
		LOCK_TYPE LockType { get; }
	}

	public class Scene : IScene
	{
		public int Id { get; protected set; }
		public string Name { get; protected set; }
		public string SceneInfo { get; protected set; }
		public bool IsLocked { get; protected set; }
		public LOCK_TYPE LockType { get; protected set; }
		public List<IExpression> Requirements { get; protected set; }

		public Scene(int id, string name, string sceneInfo, LOCK_TYPE? lockType)
		{
			Id = id;
			Name = name;
			SceneInfo = sceneInfo;

			if(!lockType.HasValue)
			{
				IsLocked = false;
				LockType = LOCK_TYPE.NONE;
			}
			else
			{
				LockType = lockType.Value;
				IsLocked = false;
				if(LockType != LOCK_TYPE.NONE)
				{
					IsLocked = true;
				}
			}
		}

		public void SetRequirements(List<IExpression> reqs)
		{
			Requirements = reqs;
		}

		public void UnlockScene()
		{
			LockType = LOCK_TYPE.NONE;
			IsLocked = false;
		}
	}
}