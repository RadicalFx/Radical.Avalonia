using Radical.Conversions;
using Avalonia;
using Avalonia.Controls;

namespace Radical.Avalonia.Regions
{
    /// <summary>
    /// An elements region hosted by a Panel.
    /// </summary>
    public sealed class PanelRegion : ElementsRegion<Panel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PanelRegion"/> class.
        /// </summary>
        public PanelRegion()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PanelRegion"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public PanelRegion(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Called after the add operation.
        /// </summary>
        /// <param name="view">The view.</param>
        protected override void OnAdd(AvaloniaObject view)
        {
            Element.Children.Add((Control)view);
        }

        /// <summary>
        /// Called before the remove operation.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="reason">The reason.</param>
        protected override void OnRemove(AvaloniaObject view, RemoveReason reason)
        {
            if (view is Control e && Element.Children.Contains(e))
            {
                Element.Children.Remove(e);
            }
        }
    }
}