namespace Voltage.Story.Import.CharacterImport.Helpers
{
	public static class CharacterBundleUtils
	{
        public static string NormalizePath(string path, string prefix)
        {
            return path.Substring(prefix.Length + 1);
        }

		public static string GetCharNameFromPath(string path)
		{
			int startIndex = path.LastIndexOf("/") + 1;
			return path.Substring(startIndex);
		}
	}
}

