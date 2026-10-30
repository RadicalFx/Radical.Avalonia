using Avalonia.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Radical.Avalonia.Tests.Behaviors
{
    public partial class AdornersView : UserControl
    {
        public AdornersView()
        {
            InitializeComponent();
        }
    }

    public class AdornersViewModel : INotifyPropertyChanged
    {
        bool isBusy;
        bool isOverlayVisible;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> Items { get; } = new ObservableCollection<string>();

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBusy)));
            }
        }

        public bool IsOverlayVisible
        {
            get => isOverlayVisible;
            set
            {
                isOverlayVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOverlayVisible)));
            }
        }
    }
}
