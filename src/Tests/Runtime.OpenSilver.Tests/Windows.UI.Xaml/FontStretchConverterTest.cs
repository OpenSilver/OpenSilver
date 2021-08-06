using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Design.Serialization;

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.UI.Xaml.Tests
#endif
{
    #if WORKINPROGRESS
    [TestClass]
    public class FontStretchConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var fontStretchConverter = new FontStretchConverter();
            var test = fontStretchConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var fontStretchConverter = new FontStretchConverter();
            var test = fontStretchConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var fontStretchConverter = new FontStretchConverter();
            var test = fontStretchConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var fontStretchConverter = new FontStretchConverter();
            var test = fontStretchConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnFontStretch()
        {
            var fontStretchConverter = new FontStretchConverter();
            var test = fontStretchConverter.ConvertFrom("");
            test.Should().Be(new FontStretch());
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var fontStretchConverter = new FontStretchConverter();
            Assert.ThrowsException<NotSupportedException>(() => fontStretchConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var fontStretchConverter = new FontStretchConverter();
            Assert.ThrowsException<NotSupportedException>(() => fontStretchConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var fontStretchConverter = new FontStretchConverter();
            var test = fontStretchConverter.ConvertTo(new FontStretch(), typeof(string));
            test.Should().Be("");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var fontStretchConverter = new FontStretchConverter();
            Assert.ThrowsException<ArgumentNullException>(() => fontStretchConverter.ConvertTo(new FontStretch(), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var fontStretchConverter = new FontStretchConverter();
            var notSupportedType = typeof(bool);
            Assert.ThrowsException<NotSupportedException>(() => fontStretchConverter.ConvertTo(new FontStretch(), notSupportedType));
        }

        [TestMethod]
        public void ConvertTo_String_FromBool()
        {
            var cursorConverter = new FontStretchConverter();
            var test = cursorConverter.ConvertTo(true, typeof(string));
            test.Should().Be("True");
        }
    }
#endif
}
