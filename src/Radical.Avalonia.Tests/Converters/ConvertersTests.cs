using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Avalonia.Converters;
using System.Globalization;

namespace Radical.Avalonia.Tests.Converters
{
    [TestClass]
    public class ConvertersTests
    {
        enum Color { Red, Green }

        [TestMethod, TestCategory("Converters")]
        public void BooleanToVisibilityConverter_should_convert_to_IsVisible_values()
        {
            var sut = new BooleanToVisibilityConverter();

            Assert.AreEqual(true, sut.Convert(true, typeof(bool), null, CultureInfo.InvariantCulture));
            Assert.AreEqual(false, sut.Convert(false, typeof(bool), null, CultureInfo.InvariantCulture));
            Assert.AreEqual(false, sut.Convert((bool?)null, typeof(bool), null, CultureInfo.InvariantCulture));
            Assert.AreEqual(false, sut.Convert(true, typeof(bool), "true", CultureInfo.InvariantCulture));
            Assert.AreEqual(true, sut.ConvertBack(true, typeof(bool), null, CultureInfo.InvariantCulture));
        }

        [TestMethod, TestCategory("Converters")]
        public void NullToVisibilityConverter_should_hide_null_values()
        {
            var sut = new NullToVisibilityConverter();

            Assert.AreEqual(false, sut.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture));
            Assert.AreEqual(true, sut.Convert(new object(), typeof(bool), null, CultureInfo.InvariantCulture));
            Assert.AreEqual(true, sut.Convert(null, typeof(bool), "true", CultureInfo.InvariantCulture));
            Assert.AreEqual(false, sut.Convert(new object(), typeof(bool), "true", CultureInfo.InvariantCulture));
        }

        [TestMethod, TestCategory("Converters")]
        public void NotConverter_should_negate_values()
        {
            var sut = new NotConverter();

            Assert.AreEqual(false, sut.Convert(true, typeof(bool), null, CultureInfo.InvariantCulture));
            Assert.AreEqual(true, sut.ConvertBack(false, typeof(bool), null, CultureInfo.InvariantCulture));
        }

        [TestMethod, TestCategory("Converters")]
        public void LinqFirstOrDefaultConverter_should_return_the_first_element()
        {
            var sut = new LinqFirstOrDefaultConverter();

            Assert.AreEqual(1, sut.Convert(new[] { 1, 2 }, typeof(object), null, CultureInfo.InvariantCulture));
            Assert.IsNull(sut.Convert(new int[0], typeof(object), null, CultureInfo.InvariantCulture));
        }

        [TestMethod, TestCategory("Converters")]
        public void EnumBooleanConverter_should_convert_back_to_the_parameter_only_when_checked()
        {
            var sut = new EnumBooleanConverter();

            Assert.AreEqual(Color.Green, sut.ConvertBack(true, typeof(Color), Color.Green, CultureInfo.InvariantCulture));
            Assert.AreSame(global::Avalonia.Data.BindingOperations.DoNothing, sut.ConvertBack(false, typeof(Color), Color.Green, CultureInfo.InvariantCulture));
        }

        [TestMethod, TestCategory("Converters")]
        public void Singleton_converters_should_provide_a_single_instance_per_type()
        {
            var first = new NotConverter().ProvideValue(null);
            var second = new NotConverter().ProvideValue(null);

            Assert.AreSame(first, second);
        }

        [AvaloniaTestMethod, TestCategory("Converters")]
        public void Singleton_converters_should_work_as_markup_extensions()
        {
            var view = new ConvertersView { DataContext = new { IsHidden = true, Item = (object)null } };

            Assert.IsFalse(view.NotTarget.IsVisible);
            Assert.IsFalse(view.NullTarget.IsVisible);
        }

        [AvaloniaTestMethod, TestCategory("Converters")]
        public void BinaryImageConverter_should_convert_bytes_to_a_bitmap()
        {
            // 1x1 transparent PNG
            var png = System.Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=");
            var sut = new BinaryImageConverter();

            var image = sut.Convert(png, typeof(global::Avalonia.Media.IImage), null, CultureInfo.InvariantCulture);

            Assert.IsInstanceOfType<global::Avalonia.Media.Imaging.Bitmap>(image);
        }
    }
}
