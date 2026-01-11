using System.Collections.Generic;

namespace Composer.Models
{
    public class UserSettings
    {
        #region public properties
        public string curPackageName = "";
        public List<PackageItem> packages = new List<PackageItem>();
        #endregion

        public UserSettings() { }

        #region public methods
        public void Normalize()
        {
            if (this.packages == null)
            {
                this.packages = new List<PackageItem>();
            }
        }

        #endregion
    }
}
