using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Design.Serialization;
using System.Windows.Media;

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.UI.Tests
#endif
{
    [TestClass]
    public class ColorConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var colorConverter = new ColorConverter();
            var test = colorConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var colorConverter = new ColorConverter();
            var test = colorConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var colorConverter = new ColorConverter();
            var test = colorConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var colorConverter = new ColorConverter();
            var test = colorConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnColor()
        {
            var colorConverter = new ColorConverter();
            var expected = new Color { A = 18, B = 120, G = 86, R = 52 };
            var test = colorConverter.ConvertFrom("#12345678");
            test.Should().Be(expected);
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_ArgumentNullException()
        {
            var colorConverter = new ColorConverter();
            Assert.ThrowsException<ArgumentNullException>(() => colorConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var colorConverter = new ColorConverter();
            Assert.ThrowsException<NotSupportedException>(() => colorConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var colorConverter = new ColorConverter();
            var color = new Color { A = 18, B = 120, G = 86, R = 52 };
            var test = colorConverter.ConvertTo(color, typeof(string));
            throw new NotImplementedException();
            test.Should().Be("#12345678");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var colorConverter = new ColorConverter();
            Assert.ThrowsException<ArgumentNullException>(() => colorConverter.ConvertTo(new Color(), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var colorConverter = new ColorConverter();
            var notSupportedType = typeof(bool);
            Assert.ThrowsException<NotSupportedException>(() => colorConverter.ConvertTo(new Color(), notSupportedType));
        }

        [TestMethod]
        public void ConvertTo_String_FromBool()
        {
            var cursorConverter = new ColorConverter();
            var test = cursorConverter.ConvertTo(true, typeof(string));
            test.Should().Be("True");
        }

        [TestMethod]
        public void ConvertTo_InstanceDescriptor()
        {
            var cursorConverter = new ColorConverter();
            var color = new Color { A = 18, B = 120, G = 86, R = 52 };
            var test = cursorConverter.ConvertTo(color, typeof(InstanceDescriptor));
            test.Should().BeOfType(typeof(InstanceDescriptor));
        }
    }
}
