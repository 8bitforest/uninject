using Uninject.Interfaces;
using UnityEngine;

namespace Uninject.MonoBehaviours
{
    public abstract class ServiceContainerProvider : MonoBehaviour
    {
        public abstract IServiceContainer ConstructServiceContainer();
    }
}