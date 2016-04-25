using System.IO;

namespace Voltage.Witches.User
{

    public interface IPlayerWriter
    {
        void Save(PlayerDataStore playerData);
        PlayerDataStore Load();
        bool HasExistingData { get; }
    }

    // This class is intended to take care of reading and writing the player object from the filesystem
    // It leaves parsing the internal representation to the serializer
    public class PlayerWriter : IPlayerWriter
    {
        private readonly IPlayerDataSerializer _serializer;
        private readonly string _path;

        public PlayerWriter(IPlayerDataSerializer serializer, string path)
        {
            _serializer = serializer;
            _path = path;
        }

        public void Save(PlayerDataStore playerData)
        {
            string rawData = _serializer.Serialize(playerData, true);

            File.WriteAllText(_path, rawData);
        }

        public PlayerDataStore Load()
        {
            if (!File.Exists(_path))
            {
                return null;
            }

            string rawData = File.ReadAllText(_path);

            return _serializer.Deserialize(rawData);
        }

        public bool HasExistingData
        {
            get { return File.Exists(_path); }
        }
    }
}

