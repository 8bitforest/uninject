using System;
using JetBrains.Annotations;

namespace Uninject.Attributes
{
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public class InjectComponentAttribute : Attribute { }
    
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public class InjectComponentsAttribute : Attribute { }

    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public class InjectChildComponentAttribute : Attribute { }
    
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public class InjectChildComponentsAttribute : Attribute { }
}