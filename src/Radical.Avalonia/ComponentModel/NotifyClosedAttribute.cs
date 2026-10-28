using System;

namespace Radical.Avalonia.ComponentModel
{
    /// <summary>
    /// Applied to a ViewModel issues automatically a ViewModelClosed message.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
    public class NotifyClosedAttribute : Attribute
    {
    }
}
