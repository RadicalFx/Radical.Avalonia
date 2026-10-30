using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Radical.Avalonia.Internals
{
    /// <summary>
    /// Avalonia allows a single adorner per element via <see cref="AdornerLayer.SetAdorner"/>,
    /// which takes care of adding and removing it as the element enters and leaves
    /// the visual tree. To allow more than one adorner on the same element (e.g. a busy
    /// indicator on a highlighted region), adorners are hosted in a per element panel.
    /// </summary>
    static class Adorners
    {
        sealed class AdornersHost : Panel
        {
        }

        public static void Add(Visual element, Control adorner)
        {
            if (AdornerLayer.GetAdorner(element) is not AdornersHost host)
            {
                host = new AdornersHost();
                AdornerLayer.SetAdorner(element, host);
            }

            host.Children.Add(adorner);
        }

        public static void Remove(Visual element, Control adorner)
        {
            if (AdornerLayer.GetAdorner(element) is AdornersHost host)
            {
                host.Children.Remove(adorner);
                if (host.Children.Count == 0)
                {
                    AdornerLayer.SetAdorner(element, null);
                }
            }
        }

        public static IEnumerable<Control> Get(Visual element)
        {
            return AdornerLayer.GetAdorner(element) is AdornersHost host
                ? host.Children.ToArray()
                : Enumerable.Empty<Control>();
        }
    }
}
