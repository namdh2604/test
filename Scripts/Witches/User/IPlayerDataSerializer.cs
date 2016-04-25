namespace Voltage.Witches.User
{
    public interface IPlayerDataSerializer
    {
        string Serialize(PlayerDataStore playerData, bool prettyPrint=false);
        PlayerDataStore Deserialize(string rawData);
    }
}

