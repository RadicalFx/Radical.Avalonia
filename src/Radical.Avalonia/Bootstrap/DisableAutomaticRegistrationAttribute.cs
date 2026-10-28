using System;

namespace Radical.Avalonia.Bootstrap
{
    /// <summary>
    /// Instructs the automatic registration process to ignore
    /// a type marked with this attribute.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class )]
    public class DisableAutomaticRegistrationAttribute : Attribute
    {
    }
}
