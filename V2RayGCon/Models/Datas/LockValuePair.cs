namespace V2RayGCon.Models.Datas
{
    public class LockValuePair<TValue>
    {
        public object rwLock;
        public TValue content;

        public LockValuePair()
        {
            rwLock = new object();
            content = default(TValue);
        }
    }
}
