using System;
using System.Reflection;
using Uninject.Attributes;
using Uninject.Interfaces;
using UnityEngine;

namespace Uninject.MonoBehaviours
{
    public class InjectionManager : MonoBehaviour
    {
        [SerializeField] private ServiceContainerProvider serviceContainerProvider;

        public IServiceContainer Container { get; set; }

        private static InjectionManager _instance;

        private void Awake()
        {
            _instance = this;
            Container ??= serviceContainerProvider?.ConstructServiceContainer() ??
                          throw new Exception("No container provider was found.");
        }

        public static void Inject(MonoBehaviour behaviour)
        {
            var injectComponentAttributeType = typeof(InjectComponentAttribute);
            var injectChildComponentAttributeType = typeof(InjectChildComponentAttribute);
            var injectDependencyAttributeType = typeof(InjectDependencyAttribute);
            var allFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (var field in behaviour.GetType().GetFields(allFlags))
            {
                if (field.GetCustomAttribute(injectComponentAttributeType) is InjectComponentAttribute)
                    field.SetValue(behaviour, ResolveComponent(behaviour, field.FieldType));
                else if (field.GetCustomAttribute(injectChildComponentAttributeType) is InjectChildComponentAttribute)
                    field.SetValue(behaviour, ResolveChildComponent(behaviour, field.FieldType));
                else if (field.GetCustomAttribute(injectDependencyAttributeType) is InjectDependencyAttribute)
                    field.SetValue(behaviour, ResolveDependency(field.FieldType));
            }

            foreach (var property in behaviour.GetType().GetProperties(allFlags))
            {
                if (property.GetCustomAttribute(injectComponentAttributeType) is InjectComponentAttribute)
                    property.SetValue(behaviour, ResolveComponent(behaviour, property.PropertyType));
                else if (property.GetCustomAttribute(injectChildComponentAttributeType) is InjectChildComponentAttribute)
                    property.SetValue(behaviour, ResolveChildComponent(behaviour, property.PropertyType));
                else if (property.GetCustomAttribute(injectDependencyAttributeType) is InjectDependencyAttribute)
                    property.SetValue(behaviour, ResolveDependency(property.PropertyType));
            }
        }
        
        private static object ResolveComponent(MonoBehaviour behaviour, Type type)
        {
            return behaviour.GetComponent(type);
        }
        
        private static object ResolveChildComponent(MonoBehaviour behaviour, Type type)
        {
            return behaviour.GetComponentInChildren(type);
        }
        
        private static object ResolveDependency(Type type)
        {
            return _instance.Container.Resolve(type);
        }
    }
}