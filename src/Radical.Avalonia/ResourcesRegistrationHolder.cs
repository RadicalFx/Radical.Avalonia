using System;
using System.Collections.Generic;

namespace Radical.Avalonia
{
    class ResourcesRegistrationHolder
    {
        public IDictionary<Type, HashSet<Type>> Registrations { get; } = new Dictionary<Type, HashSet<Type>>();
    }
}