using System.Reflection;

namespace Radical.Avalonia
{
    static class ObjectExtensions
    {
        public static PropertyInfo GetProperty(this object entity, string propertyName) 
        {
            return entity.GetType().GetProperty(propertyName);
        }
    }
}
