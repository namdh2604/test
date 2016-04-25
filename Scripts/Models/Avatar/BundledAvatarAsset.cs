namespace Voltage.Witches.Models.Avatar
{
    public class BundledAvatarAsset
    {
        public string LayerName { get; set; }
        public string BundleName { get; set; }

        public BundledAvatarAsset(string layerName, string bundleName)
        {
            LayerName = layerName;
            BundleName = bundleName;
        }
    }
}
