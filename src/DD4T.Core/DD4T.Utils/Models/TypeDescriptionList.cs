using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DD4T.Core.DD4T.Utils.Models
{
    public class TypeDescriptionList : List<TypeDescription>
    {
        public void Add(Type interfaceType, Type implementationType, LifeCycle lifeCycle = LifeCycle.SingleInstance)
        {
            this.Add(new TypeDescription()
            {
                Interface = interfaceType,
                Implementation = implementationType,
                LifeCycle = lifeCycle
            });
        }
    }

    public struct TypeDescription
    {
        public Type Interface { get; set; }
        public Type Implementation { get; set; }
        public LifeCycle LifeCycle { get; set; }
    }

    public enum LifeCycle
    {
        PerRequest,
        SingleInstance,
        PerLifeTime,
        PerDependency
    }
}