using System.IO;

namespace Voltage.Witches.Services
{

	public interface IBuildNumberService
	{
		string GetBuildVersion();
		string GetBaseBuildVersion();
	}


    public class BuildNumberService : IBuildNumberService
    {
        private IFilesystemService _filesystem;
        private const string ASSET_PATH = "buildNumber";

        public BuildNumberService(IFilesystemService filesystem)
        {
            _filesystem = filesystem;
        }

        private const string DEFAULT_BUILD = "1.0.0_d LOCAL";

        public string GetBuildVersion()
        {
            string buildVersion = string.Empty;
            try
            {
                buildVersion = _filesystem.ReadAllText(ASSET_PATH);
            }
            catch (DirectoryNotFoundException) { buildVersion = DEFAULT_BUILD; }
            catch (FileNotFoundException) { buildVersion = DEFAULT_BUILD; }

            return buildVersion;
        }

        public string GetBaseBuildVersion()
        {
            string rawVersion = GetBuildVersion();
            string[] tokens = rawVersion.Split(' ');
            return tokens[0];
        }
    }
}

