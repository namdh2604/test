using System.Collections.Generic;

namespace Voltage.Story.StoryDivisions
{
    using Voltage.Witches.Models.MissionRequirements;
    using Voltage.Story.Mapper;
    using Voltage.Story.Expressions;
    using Voltage.Story.General;

	public interface ISceneHeaderFactory
	{
        SceneHeader Create(string scenePath);
	}

}

