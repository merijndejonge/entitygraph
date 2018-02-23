using System;
using System.Linq;
using System.Reflection;

namespace OpenSoftware.EntityGraphCore
{
    // ReSharper disable once UnusedTypeParameter
    public partial class EntityGraph<TEntity> : IDisposable
    {
        public void Dispose()
        {
            var type = GetType();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var disposers = type.GetMethods(flags).Where(m => m.IsDefined(typeof(DisposeAttribute), true));

            foreach (var disposer in disposers)
            {
                disposer.Invoke(this, new object[] { });
            }
        }
    }
}