using System.Collections.Generic;

namespace Commander.Models.Data
{
    public class UserSettings
    {
        #region public properties
        public List<CmderParam> cmderParams;
        #endregion

        public UserSettings()
        {
            cmderParams = new List<CmderParam>();
        }
    }
}
