using System;
using System.Collections.Generic;
using VgcApis.Interfaces.ComponentOf;

namespace VgcApis.BaseClasses
{
    public class ComponentOf<TParent> :
        Disposable,
        IComponentOf<TParent>
        where TParent : class, IParentComponent
    {

        TParent parent = null;
        readonly List<object> children = new List<object>();

        readonly object componentLocker = new object();
        public ComponentOf() { }

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
        sealed protected override void Cleanup()
        {
            CleanupBeforeChildrenDispose();
            DisposeChilderenInReverseOrder();
            CleanupAfterChildrenDisposed();
        }

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
    }
}
