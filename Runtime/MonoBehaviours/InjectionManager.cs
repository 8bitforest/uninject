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
            var injectDependencyAttributeType = typeof(InjectDependencyAttribute);
            var allFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (var field in behaviour.GetType().GetFields(allFlags))
            {
                if (field.GetCustomAttribute(injectComponentAttributeType) is InjectComponentAttribute)
                    field.SetValue(behaviour, behaviour.GetComponent(field.FieldType));
                else if (field.GetCustomAttribute(injectDependencyAttributeType) is InjectDependencyAttribute)
                    field.SetValue(behaviour, _instance.Container.Resolve(field.FieldType));
            }

            foreach (var property in behaviour.GetType().GetProperties(allFlags))
            {
                if (property.GetCustomAttribute(injectComponentAttributeType) is InjectComponentAttribute)
                    property.SetValue(behaviour, behaviour.GetComponent(property.PropertyType));
                else if (property.GetCustomAttribute(injectDependencyAttributeType) is InjectDependencyAttribute)
                    property.SetValue(behaviour, _instance.Container.Resolve(property.PropertyType));
            }
        }
    }
}