using System;

namespace Uninject.Interfaces
{
    public interface IServiceContainer
    {
        public object Resolve(Type type);
    }
}