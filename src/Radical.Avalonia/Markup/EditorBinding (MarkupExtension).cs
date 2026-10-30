using Avalonia.Data;

namespace Radical.Avalonia.Markup
{
    /// <summary>
    /// A binding for editors that updates the source when the target property changes.
    /// </summary>
    public class EditorBinding : BindingDecoratorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorBinding"/> class.
        /// </summary>
        public EditorBinding()
            : base()
        {
            InitDefaults();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorBinding"/> class.
        /// </summary>
        /// <param name="path">The binding path.</param>
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