using Avalonia.Input;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Input;
using System.ComponentModel;

namespace Radical.Avalonia.Tests.Markup
{
    public class CommandsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public CommandsViewModel()
        {
            RefreshCommand = DelegateCommand.Create()
                .OnExecute(_ => Refreshed = true)
                .AddKeyGesture(Key.F5);
        }

        public int? SavedValue { get; private set; }

        public bool Refreshed { get; private set; }

        public IDelegateCommand RefreshCommand { get; }

        bool canSave = true;
        public bool CanSave
        {
            get => canSave;
            set
            {
                canSave = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanSave)));
            }
        }

        string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        [KeyBinding(Key.S, Modifiers = KeyModifiers.Control)]
        public void Save(int value)
        {
            SavedValue = value;
        }
    }
}
