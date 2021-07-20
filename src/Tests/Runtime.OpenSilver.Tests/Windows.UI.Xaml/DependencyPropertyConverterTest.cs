using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.UI.Xaml.Tests
#endif
{
    [TestClass]
    public class DependencyPropertyConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var dependencyPropertyConverter = new DependencyPropertyConverter();
            var test = dependencyPropertyConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var dependencyPropertyConverter = new DependencyPropertyConverter();
            var test = dependencyPropertyConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldFalse()
        {
            var dependencyPropertyConverter = new DependencyPropertyConverter();
            var test = dependencyPropertyConverter.CanConvertTo(typeof(string));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnNull()
        {
            var dependencyPropertyConverter = new DependencyPropertyConverter();
            var test = dependencyPropertyConverter.ConvertFrom("0,0,100,100");
            test.Should().BeNull();
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var dependencyPropertyConverter = new DependencyPropertyConverter();
            Assert.ThrowsException<NotSupportedException>(() => dependencyPropertyConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var dependencyPropertyConverter = new DependencyPropertyConverter();
            Assert.ThrowsException<NotSupportedException>(() => dependencyPropertyConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var dependencyPropertyConverter = new DependencyPropertyConverter();
            Assert.ThrowsException<NotSupportedException>(() => dependencyPropertyConverter.ConvertTo(new DependencyProperty(), typeof(bool)));
        }
    }
}
