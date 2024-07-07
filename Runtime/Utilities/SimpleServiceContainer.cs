using System;
using System.Collections.Generic;
using Uninject.Interfaces;
using UnityEngine;

namespace Uninject.Utilities
{
    public class SimpleServiceContainer : IServiceContainer
    {
        private readonly Dictionary<Type, object> _singletons = new();
        
        public void BindSingleton<T>(T instance)
        {
            if (_singletons.ContainsKey(typeof(T)))
                Debug.LogWarning($"Singleton of type {typeof(T)} already exists. Overwriting.");
            _singletons[typeof(T)] = instance;
        }
        
        public T Resolve<T>()
        {
            if (!_singletons.ContainsKey(typeof(T)))
                throw new Exception($"Singleton of type {typeof(T)} not found.");
            return (T) _singletons[typeof(T)];
        }

        public object Resolve(Type type)
        {
            if (!_singletons.TryGetValue(type, out var instance))
                throw new Exception($"Singleton of type {type} not found.");
            return instance;
        }
    }
}