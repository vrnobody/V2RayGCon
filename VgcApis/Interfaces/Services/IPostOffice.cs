using VgcApis.Models.Datas;
using VgcApis.Interfaces.PostOfficeComponents;

namespace VgcApis.Interfaces.Services
{
    public interface IPostOffice
    {
        #region snap cache

        bool SnapCacheRemove(string token);

        string SnapCacheApply();

        bool SnapCacheCreate(string token);

        object SnapCacheGet(string token, string key);

        bool SnapCacheSet(string token, string key, object value);

        #endregion

        #region mailbox
        ILuaMailBox CreateMailBox(string address, int capacity);

        bool RemoveMailBox(ILuaMailBox mailbox);
        bool Send(string address, LuaMail mail);
        bool SendAndWait(string address, VgcApis.Models.Datas.LuaMail mail);
        bool ValidateMailBox(ILuaMailBox mailbox);
        #endregion
    }
}
