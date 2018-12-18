using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace DependencyInjectionContainer
{
    public class DependenciesConfiguration
    {
        private ConcurrentDictionary<Type, List<Dependency>> dictionary;

        public DependenciesConfiguration()
        {
            dictionary = new ConcurrentDictionary<Type, List<Dependency>>();
        }

        public void Register(Type Interface, Type realization, bool isSingleton = false)
        {
            if (!realization.IsInterface && !realization.IsAbstract && (realization.IsGenericTypeDefinition || Interface.IsAssignableFrom(realization)))
            {
                var dependency = new Dependency(Interface, realization, isSingleton);
                List<Dependency> dependencies;

                if (!dictionary.TryGetValue(Interface, out dependencies))
                {
                    lock (dictionary)
                    {
                        if (!dictionary.TryGetValue(Interface, out dependencies))
                        {
                            dictionary.TryAdd(Interface, new List<Dependency>() { dependency });
                            return;
                        }
                    }
                }

                if (!dependencies.Contains(dependency))
                {
                    lock (dependencies)
                    {
                        if (!dependencies.Contains(dependency))
                        {
                            dependencies.Add(dependency);
                        }
                    }
                }
            }
        }

        public Dependency GetDependency(Type Interface)
        {
            List<Dependency> dependencies;
            return dictionary.TryGetValue(Interface, out dependencies) ? dependencies.Last() : null;
        }

        public IEnumerable<Dependency> GetDependencies(Type Interface)
        {
            List<Dependency> dependencies;
            return dictionary.TryGetValue(Interface, out dependencies) ? dependencies : null;
        }
    }
}
