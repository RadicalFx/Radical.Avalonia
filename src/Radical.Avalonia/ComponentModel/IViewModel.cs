using Avalonia;
using System.ComponentModel;

namespace Radical.Avalonia.ComponentModel
{
    /// <summary>
    /// Defines the base contract that a Topics.Presentation
    /// ViewModel must respect.
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the view. The view property is intended only for
        /// infrastructural purpose. It is required to hold the one-to-one
        /// relation between the view and the view model.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        AvaloniaObject View { get; set; }
    }
}
