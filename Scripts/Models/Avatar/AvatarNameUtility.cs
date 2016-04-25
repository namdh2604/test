namespace Voltage.Witches.Models.Avatar
{
    public class AvatarNameUtility
    {
        private readonly Player _player;
        private const string BUNDLE_NAME = "USER_FIRST";

        public AvatarNameUtility(Player player)
        {
            _player = player;
        }

        public static bool IsAvatarName(string name)
        {
            return (name == BUNDLE_NAME);
        }

        public string GetDisplayableName()
        {
            return _player.FirstName;
        }

        public static string GetBundleName()
        {
            return BUNDLE_NAME;
        }
    }
}
