using Avalonia;
using Avalonia.Controls;

namespace Radical.Avalonia.ComponentModel
{
    /// <summary>
    /// A region that holds a single, replaceable, content.
    /// </summary>
    public interface IContentRegion : IRegion
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        AvaloniaObject Content { get; set; }
    }
}
