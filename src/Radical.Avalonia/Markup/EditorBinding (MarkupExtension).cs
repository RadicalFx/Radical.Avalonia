using Avalonia.Data;

namespace Radical.Avalonia.Markup
{
    public class EditorBinding : BindingDecoratorBase
    {
        public EditorBinding()
            : base()
        {
            InitDefaults();
        }

        public EditorBinding(string path)
            : base(path)
        {
            InitDefaults();
        }

        void InitDefaults()
        {
            // Avalonia reports INotifyDataErrorInfo and exception validation
            // errors for properties that enable data validation (e.g. TextBox.Text)
            // without additional binding settings.
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }
    }
}