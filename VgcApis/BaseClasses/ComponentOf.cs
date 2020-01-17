using System;
using System.Collections.Generic;
using VgcApis.Interfaces.ComponentOf;

namespace VgcApis.BaseClasses
{
    public class ComponentOf<TParent> :
        IComponentOf<TParent>
        where TParent : class, IParentComponent
    {

        TParent parent = null;
        readonly List<object> children = new List<object>();

        readonly object componentLocker = new object();
        public ComponentOf()
        {

        }

        #region properties

        #endregion

        #region public methods
        public virtual void Prepare() { }

        public TParent GetParent() => parent;

        public IReadOnlyCollection<object> GetChildren()
        {
            lock (componentLocker)
            {
                return children.AsReadOnly();
            }
        }

        public IReadOnlyCollection<object> GetSiblings()
        {
            var parent = GetParent();
            return parent?.GetChildren();
        }

        public TSibling GetSibling<TSibling>() where TSibling : class
        {
            var siblins = GetSiblings();

            foreach (var sib in siblins)
            {
                if (sib is TSibling)
                {
                    return sib as TSibling;
                }
            }

            return null;
        }

        public TChild GetChild<TChild>() where TChild : class
        {
            lock (componentLocker)
            {
                foreach (var c in children)
                {
                    if (c is TChild)
                    {
                        return c as TChild;
                    }
                }
            }
            return null;
        }

        public void SetParent(object parent)
        {
            lock (componentLocker)
            {
                this.parent = parent as TParent;
            }
        }

        public void AddChild<TChild>(TChild child)
            where TChild : class, IDisposable
        {
            lock (componentLocker)
            {
                if (children.Contains(child))
                {
                    throw new ArgumentException($"Child {child.GetType().FullName} already exist!");
                }
                children.Add(child);
                (child as IChildComponent).SetParent(this);
            }
        }

        #endregion

        #region protected methods
        protected virtual void CleanupBeforeChildrenDispose() { }
        protected virtual void CleanupAfterChildrenDisposed() { }
        #endregion

        #region private methods
        private void DisposeChilderenInReverseOrder()
        {
            for (int i = children.Count - 1; i >= 0; i--)
            {
                (children[i] as IDisposable)?.Dispose();
            }
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CleanupBeforeChildrenDispose();
                    DisposeChilderenInReverseOrder();
                    CleanupAfterChildrenDisposed();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~Disposable() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
