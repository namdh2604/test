using UnityEditor;

using Voltage.Common.Utilities;

namespace Voltage.Story.Import
{
	public static class LexUtils
	{
        /* extracts the lex file, located at the input path, to a randomly generated, temporary Unity folder.
         * Returns the path to this temporary folder */
        public static string ExtractLexToTempPath(string path)
        {
            string tempDir = FileUtil.GetUniqueTempPathInProject();
            ZipUtils.UnzipToDir(path, tempDir);

            return tempDir;
        }
	}
}
