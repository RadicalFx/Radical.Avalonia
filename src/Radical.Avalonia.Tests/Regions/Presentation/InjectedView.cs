using Avalonia.Controls;
using Radical.Avalonia.ComponentModel;

namespace Radical.Avalonia.Tests.Regions.Presentation
{
    [InjectViewInRegion(Named = "TestContentRegion")]
    public class InjectedView : UserControl
    {
    }
}
