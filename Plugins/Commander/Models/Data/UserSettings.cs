using System.Collections.Generic;

namespace Commander.Models.Data
{
    public class UserSettings
    {
        #region public properties
        public List<ProcParam> procParams;

        #endregion

        public UserSettings()
        {
            procParams = new List<ProcParam>();
        }
    }
}
