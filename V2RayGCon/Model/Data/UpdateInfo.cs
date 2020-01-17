using System.Collections.Generic;

namespace V2RayGCon.Model.Data
{
    internal sealed class UpdateInfo
    {
        public string version;
        public string md5;
        public List<string> warnings, changes;

        public UpdateInfo()
        {
            version = string.Empty;
            md5 = string.Empty;
            warnings = new List<string>();
            changes = new List<string>();
        }
    }
}
