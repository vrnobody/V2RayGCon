using System.Collections.Generic;

namespace Pacman.Models.Data
{
    public class UserSettings
    {
        #region public properties
        public List<Package> packages;

        #endregion

        public UserSettings()
        {
            packages = new List<Package>();
        }
    }
}
