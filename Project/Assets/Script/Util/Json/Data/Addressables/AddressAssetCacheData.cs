namespace Backend.Util.Json.Data.Addressables
{
    [System.Serializable]
    public class AddressAssetCacheData
    {
        public string Path { get; set; } = string.Empty;

        public string Namespace { get; set; } = string.Empty;

        public string ClassName { get; set; } = string.Empty;

        public AddressAssetData AddressAssetData { get; set; } = new();
    }
}
