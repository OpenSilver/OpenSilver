using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.UI.Xaml.Tests
#endif
{
    [TestClass]
    public class RoutedEventConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var routedEventConverter = new RoutedEventConverter();
            var test = routedEventConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var routedEventConverter = new RoutedEventConverter();
            var test = routedEventConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var routedEventConverter = new RoutedEventConverter();
            var test = routedEventConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnRoutedEvent()
        {
            var routedEventConverter = new RoutedEventConverter();
            var test = routedEventConverter.ConvertFrom("");
            test.Should().Be("");
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var routedEventConverter = new RoutedEventConverter();
            Assert.ThrowsException<NotSupportedException>(() => routedEventConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var routedEventConverter = new RoutedEventConverter();
            Assert.ThrowsException<NotSupportedException>(() => routedEventConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var routedEventConverter = new RoutedEventConverter();
            Assert.ThrowsException<ArgumentNullException>(() => routedEventConverter.ConvertTo(new RoutedEvent("event"), null));
            Assert.ThrowsException<ArgumentNullException>(() => routedEventConverter.ConvertTo(null, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var routedEventConverter = new RoutedEventConverter();
            Assert.ThrowsException<NotSupportedException>(() => routedEventConverter.ConvertTo(new RoutedEvent("event"), typeof(string)));
        }
    }
}
