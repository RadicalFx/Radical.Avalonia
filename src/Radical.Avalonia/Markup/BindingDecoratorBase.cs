using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Radical.Avalonia.Markup
{
    /// <summary>
    /// A base class for custom markup extension which provides properties
    /// that can be found on regular <see cref="Binding"/> markup extension.
    /// </summary>
    public abstract class BindingDecoratorBase : MarkupExtension
    {
        /// <summary>
        /// The decorated binding class.
        /// </summary>
        private Binding binding = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingDecoratorBase"/> class.
        /// </summary>
        protected BindingDecoratorBase()
        {
            binding = new Binding();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingDecoratorBase"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        protected BindingDecoratorBase(string path)
        {
            binding = new Binding(path);
        }

        /// <summary>
        /// The decorated binding class.
        /// </summary>
        [Browsable(false)]
        public Binding Binding
        {
            get { return binding; }
            set { binding = value; }
        }

        /// <summary>
        /// The delay, in milliseconds, before updating the binding source.
        /// </summary>
        [DefaultValue(0)]
        public int Delay
        {
            get { return binding.Delay; }
            set { binding.Delay = value; }
        }

        /// <summary>
        /// The converter used by the binding.
        /// </summary>
        [DefaultValue(null)]
        public IValueConverter Converter
        {
            get { return binding.Converter; }
            set { binding.Converter = value; }
        }

        /// <summary>
        /// The culture passed to the converter.
        /// </summary>
        [DefaultValue(null)]
        public CultureInfo ConverterCulture
        {
            get { return binding.ConverterCulture; }
            set { binding.ConverterCulture = value; }
        }

        /// <summary>
        /// The parameter passed to the converter.
        /// </summary>
        [DefaultValue(null)]
        public object ConverterParameter
        {
            get { return binding.ConverterParameter; }
            set { binding.ConverterParameter = value; }
        }

        /// <summary>
        /// The name of the element to use as the binding source.
        /// </summary>
        [DefaultValue(null)]
        public string ElementName
        {
            get { return binding.ElementName; }
            set { binding.ElementName = value; }
        }

        /// <summary>
        /// The value used when the binding is unable to return a value.
        /// </summary>
        public object FallbackValue
        {
            get { return binding.FallbackValue; }
            set { binding.FallbackValue = value; }
        }

        /// <summary>
        /// The binding mode.
        /// </summary>
        public BindingMode Mode
        {
            get { return binding.Mode; }
            set { binding.Mode = value; }
        }

        /// <summary>
        /// The binding path.
        /// </summary>
        public string Path
        {
            get { return binding.Path; }
            set { binding.Path = value; }
        }

        /// <summary>
        /// The binding priority.
        /// </summary>
        public BindingPriority Priority
        {
            get { return binding.Priority; }
            set { binding.Priority = value; }
        }

        /// <summary>
        /// The relative binding source.
        /// </summary>
        [DefaultValue(null)]
        public RelativeSource RelativeSource
        {
            get { return binding.RelativeSource; }
            set { binding.RelativeSource = value; }
        }

        /// <summary>
        /// The binding source.
        /// </summary>
        public object Source
        {
            get { return binding.Source; }
            set { binding.Source = value; }
        }

        /// <summary>
        /// The string format applied to the value.
        /// </summary>
        [DefaultValue(null)]
        public string StringFormat
        {
            get { return binding.StringFormat; }
            set { binding.StringFormat = value; }
        }

        /// <summary>
        /// The value used when the source value is null.
        /// </summary>
        public object TargetNullValue
        {
            get { return binding.TargetNullValue; }
            set { binding.TargetNullValue = value; }
        }

        /// <summary>
        /// The timing of binding source updates.
        /// </summary>
        public UpdateSourceTrigger UpdateSourceTrigger
        {
            get { return binding.UpdateSourceTrigger; }
            set { binding.UpdateSourceTrigger = value; }
        }

        /// <summary>
        /// This basic implementation returns the decorated <see cref="Binding"/>,
        /// that Avalonia applies to the target property.
        /// </summary>
        /// <param name="provider">Object that can provide services for the markup
        /// extension.</param>
        /// <returns>The decorated binding.</returns>
        public override object ProvideValue(IServiceProvider provider)
        {
            return binding;
        }

        /// <summary>
        /// Validates a service provider that was submitted to the <see cref="ProvideValue"/>
        /// method. This method checks whether the provider is null (happens at design time),
        /// whether it provides an <see cref="IProvideValueTarget"/> service, and whether
        /// the service's <see cref="IProvideValueTarget.TargetObject"/> and
        /// <see cref="IProvideValueTarget.TargetProperty"/> properties are valid
        /// <see cref="AvaloniaObject"/> and <see cref="AvaloniaProperty"/>
        /// instances.
        /// </summary>
        /// <param name="provider">The provider to be validated.</param>
        /// <param name="target">The binding target of the binding.</param>
        /// <param name="property">The target property of the binding.</param>
        /// <returns>True if the provider supports all that's needed.</returns>
        protected bool TryGetTargetItems(IServiceProvider provider, out AvaloniaObject target, out AvaloniaProperty property)
        {
            return TryGetTargetItems<AvaloniaObject>(provider, out target, out property);
        }

        /// <summary>
        /// Validates a service provider that was submitted to the <see cref="ProvideValue"/>
        /// method, requiring a target object of the given type.
        /// </summary>
        /// <typeparam name="T">The expected type of the binding target.</typeparam>
        /// <param name="provider">The provider to be validated.</param>
        /// <param name="target">The binding target of the binding.</param>
        /// <param name="property">The target property of the binding.</param>
        /// <returns>True if the provider supports all that's needed.</returns>
        protected virtual bool TryGetTargetItems<T>(IServiceProvider provider, out T target, out AvaloniaProperty property)
            where T : AvaloniaObject
        {
            target = null;
            property = null;
            if (provider == null) return false;

            if (!(provider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget service)) return false;

            target = service.TargetObject as T;
            property = service.TargetProperty as AvaloniaProperty;
            return target != null && property != null;
        }
    }
}
