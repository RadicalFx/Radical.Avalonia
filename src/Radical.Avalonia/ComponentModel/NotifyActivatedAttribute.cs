using System;

namespace Radical.Avalonia.ComponentModel
{
    /// <summary>
    /// Applied to a ViewModel issues automatically a ViewModelActivated message.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
    public class NotifyActivatedAttribute : Attribute
    {
    }
}
