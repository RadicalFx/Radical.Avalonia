using System;

namespace Radical.Avalonia.ComponentModel
{
    /// <summary>
    /// Marks a class or interface as a component of the toolkit.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Interface )]
    public class ToolkitComponentAttribute : Attribute
    {

    }
}
