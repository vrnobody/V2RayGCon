namespace V2RayGCon.Model.Data
{
    public class PluginInfoItem
    {
        public string filename { get; set; }
        public string name { get; set; }
        public string version { get; set; }
        public string description { get; set; }
        public bool isUse { get; set; }

        public PluginInfoItem()
        {
            filename = string.Empty;
            name = string.Empty;
            version = string.Empty;
            description = string.Empty;
            isUse = false;
        }
    }
}
