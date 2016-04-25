using NUnit.Framework;

using Voltage.Story.Import.CharacterImport.Helpers;

namespace Tests.Story.Import.CharacterImport.Helpers
{
	[TestFixture]
	public class CharacterBundleUtilsTests
	{
		[Test]
		public void NormalizePath_WithPrefixedPath_TruncatesPrefix()
		{
			string prefix = "Assets/Characters";
			string inputPath = "Assets/Characters/Alix";
			string result = CharacterBundleUtils.NormalizePath(inputPath, prefix);

			Assert.AreEqual("Alix", result);
		}

		[Test]
		public void GetCharNameFromPath_WithAssetPath_RetrievesCorrectName()
		{
			string assetPath = "Assets/Characters/Alix";
			string result = CharacterBundleUtils.GetCharNameFromPath(assetPath);

			Assert.AreEqual("Alix", result);
		}
	}
}

