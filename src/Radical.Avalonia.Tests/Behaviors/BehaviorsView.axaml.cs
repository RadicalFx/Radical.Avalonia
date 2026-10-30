using Avalonia.Controls;
using Radical.Avalonia.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Radical.Avalonia.Tests.Behaviors
{
    public partial class BehaviorsView : UserControl
    {
        public BehaviorsView()
        {
            InitializeComponent();
        }
    }

    public class BehaviorsViewModel : INotifyPropertyChanged
    {
        string focusedElement;

        public BehaviorsViewModel()
        {
            SearchCommand = DelegateCommand.Create().OnExecute(p => Executions.Add(p));
            SaveCommand = DelegateCommand.Create().OnExecute(p => Executions.Add(p));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<object> Executions { get; } = new List<object>();

        public ICommand SearchCommand { get; }

        public ICommand SaveCommand { get; }

        public string FocusedElement
        {
            get => focusedElement;
            set
            {
                focusedElement = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FocusedElement)));
            }
        }
    }
}
