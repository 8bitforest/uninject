using Uninject.MonoBehaviours;
using UnityEngine;

namespace Uninject.Extensions
{
    public static class MonoBehaviourExtensions 
    {
        public static void Inject(this MonoBehaviour behaviour)
        {
            InjectionManager.Inject(behaviour);
        }
    }
}