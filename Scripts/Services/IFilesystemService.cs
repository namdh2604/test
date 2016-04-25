namespace Voltage.Witches.Services
{
    /* An interface to create an abstraction for filesystem operations */
    public interface IFilesystemService
    {
        /*
         * Lists all files starting with the given prefix. This will recurse through child directories
         * path: A platform-agnostic path to the resource
         * pattern: a wild-card pattern (ex:*.json)
         */ 
        string[] ListAllFiles(string path, string pattern);

        /*
         * path: A platform-agnostic path to the resource
         */
        string ReadAllText(string path);
    }
}

