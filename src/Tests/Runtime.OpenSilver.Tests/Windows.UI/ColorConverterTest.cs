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
            var test = colorConverter.CanConvertTo(typeof(InstanceDescriptor));
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
        public void ConvertFrom_Hex_ShouldReturnColor()
        {
            var colorConverter = new ColorConverter();
            var expected = new Color { A = 18, B = 120, G = 86, R = 52 };
            var test = colorConverter.ConvertFrom("#12345678");
            test.Should().Be(expected);
        }

        [TestMethod]
        public void ConvertFrom_NamedColor_ShouldReturnColor()
        {
            var colorConverter = new ColorConverter();
            var expected = new Color { A = 255, B = 0, G = 255, R = 255 };
            var test = colorConverter.ConvertFrom("Yellow");
            test.Should().Be(expected);
        }

        [TestMethod]
        public void ConvertFrom_SC_ShouldReturnColor()
        {
            var colorConverter = new ColorConverter();
            var expected = new Color { A = 0, B = 255, G = 255, R = 255 };
            throw new NotImplementedException();
            var test = colorConverter.ConvertFrom("sc#12345678");
            test.Should().Be(expected);
        }

        [TestMethod]
        public void ConvertFrom_InvalidColor_ShouldThrow_Exception()
        {
            var colorConverter = new ColorConverter();
            var invalidColor = "invalid color";
            Assert.ThrowsException<Exception>(() => colorConverter.ConvertFrom(invalidColor));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var colorConverter = new ColorConverter();
            Assert.ThrowsException<NotSupportedException>(() => colorConverter.ConvertFrom(null));
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
            Assert.ThrowsException<NotSupportedException>(() => colorConverter.ConvertTo(true, typeof(bool)));
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
