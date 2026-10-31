using Avalonia.Input;
using Radical.Avalonia.ComponentModel;
using Radical.Avalonia.Input;
using System.ComponentModel;
using System.Threading.Tasks;

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

        public TaskCompletionSource<bool> SaveAsyncGate { get; } = new TaskCompletionSource<bool>();

        public int? SavedAsyncValue { get; private set; }

        public async Task SaveAsync(int value)
        {
            await SaveAsyncGate.Task;
            SavedAsyncValue = value;
        }

        [KeyBinding(Key.S, Modifiers = KeyModifiers.Control)]
        public void Save(int value)
        {
            SavedValue = value;
        }
    }
}
