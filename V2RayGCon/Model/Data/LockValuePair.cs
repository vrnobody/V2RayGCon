namespace V2RayGCon.Model.Data
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
