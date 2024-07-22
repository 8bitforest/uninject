using System;
using System.Collections.Generic;
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

        private const BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private delegate object Resolver(MonoBehaviour behaviour, Type type);

        private static readonly Dictionary<Type, Resolver> Resolvers = new()
        {
            { typeof(InjectComponentAttribute), ResolveComponent },
            { typeof(InjectComponentsAttribute), ResolveComponents },
            { typeof(InjectChildComponentAttribute), ResolveChildComponent },
            { typeof(InjectChildComponentsAttribute), ResolveChildComponents },
            { typeof(InjectDependencyAttribute), (_, type) => ResolveDependency(type) }
        };

        private static InjectionManager _instance;

        private void Awake()
        {
            _instance = this;
            Container ??= serviceContainerProvider?.ConstructServiceContainer() ??
                          throw new Exception("No container provider was found.");
        }


        public static void Inject(MonoBehaviour behaviour)
        {
            var type = behaviour.GetType();
            foreach (var field in type.GetFields(AllFlags))
                InjectMember(behaviour, field, field.FieldType);
            foreach (var property in type.GetProperties(AllFlags))
                InjectMember(behaviour, property, property.PropertyType);
        }

        private static void InjectMember(MonoBehaviour behaviour, MemberInfo member, Type memberType)
        {
            foreach (var attributeType in Resolvers.Keys)
            {
                if (Attribute.IsDefined(member, attributeType))
                {
                    var value = Resolvers[attributeType](behaviour, memberType);
                    if (member is FieldInfo field)
                        field.SetValue(behaviour, value);
                    else if (member is PropertyInfo property)
                        property.SetValue(behaviour, value);

                    break;
                }
            }
        }

        private static object ResolveComponent(MonoBehaviour behaviour, Type type)
        {
            return behaviour.GetComponent(type);
        }

        private static object ResolveComponents(MonoBehaviour behaviour, Type type)
        {
            if (!type.IsArray) Debug.LogError("InjectComponentsAttribute can only be used on arrays.");
            var method = typeof(MonoBehaviour).GetMethod(nameof(MonoBehaviour.GetComponents), Array.Empty<Type>());
            var generic = method!.MakeGenericMethod(type.GetElementType());
            return generic.Invoke(behaviour, null);
        }

        private static object ResolveChildComponent(MonoBehaviour behaviour, Type type)
        {
            return behaviour.GetComponentInChildren(type);
        }

        private static object ResolveChildComponents(MonoBehaviour behaviour, Type type)
        {
            if (!type.IsArray) Debug.LogError("InjectChildComponentsAttribute can only be used on arrays.");
            var method =
                typeof(MonoBehaviour).GetMethod(nameof(MonoBehaviour.GetComponentsInChildren), Array.Empty<Type>());
            var generic = method!.MakeGenericMethod(type.GetElementType());
            return generic.Invoke(behaviour, null);
        }

        private static object ResolveDependency(Type type)
        {
            return _instance.Container.Resolve(type);
        }
    }
}