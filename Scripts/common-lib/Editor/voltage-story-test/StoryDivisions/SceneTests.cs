using NUnit.Framework;
using Voltage.Story.StoryDivisions;

namespace Unit.Story.StoryDivisions
{
    [TestFixture]
    public class SceneTests
    {
        [Test]
        public void GetPath_SceneWithoutVersion_EndsWithSceneName()
        {
            Scene scene = new Scene("route", "arc", "scene");
            Assert.AreEqual("route/arc/scene", scene.Path);
        }

        [Test]
        public void GetPath_SceneWithVersion_IncludesVersion()
        {
            Scene scene = new Scene("route", "arc", "scene", "version");
            Assert.AreEqual("route/arc/scene/version", scene.Path);
        }

    }
}

