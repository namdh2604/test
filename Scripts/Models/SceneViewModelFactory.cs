using Voltage.Story.StoryDivisions;

namespace Voltage.Witches.Models
{
    public interface ISceneViewModelFactory
    {
        SceneViewModel Generate(SceneHeader header, bool completed, int bitProgress);
    }

    public class SceneViewModelFactory : ISceneViewModelFactory
    {
        private readonly RequirementEvaluator _reqEvaluator;

        public SceneViewModelFactory(RequirementEvaluator reqEvaluator)
        {
            _reqEvaluator = reqEvaluator;
        }

		public SceneViewModel Generate(SceneHeader header, bool completed, int bitProgress)
        {
            LockType lockStatus = _reqEvaluator.GetLockType(header);

			var progress = bitProgress;
			progress = (completed) ? 5 : bitProgress;

			return new SceneViewModel(header.Route, header.Arc, header.Scene, header.Description, header.PolaroidPath , lockStatus, progress, completed);	
        }
    }
}

