using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Data.Tests
#else
namespace Windows.UI.Xaml.Data.Tests
#endif
{
    [TestClass]
    public class RelativeSourceConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var relativeSourceConverter = new RelativeSourceConverter();
            var test = relativeSourceConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var relativeSourceConverter = new RelativeSourceConverter();
            var test = relativeSourceConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var relativeSourceConverter = new RelativeSourceConverter();
            var test = relativeSourceConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var relativeSourceConverter = new RelativeSourceConverter();
            var test = relativeSourceConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String()
        {
            var relativeSourceConverter = new RelativeSourceConverter();
            var expected = new RelativeSource { Mode = RelativeSourceMode.Self };
            var test = relativeSourceConverter.ConvertFrom("Self");
            test.Should().Be(expected);
        }

        [TestMethod]
        public void ConvertFrom_InvalidRelativeMode_ShouldThrow_FormatException()
        {
            var relativeSourceConverter = new RelativeSourceConverter();
            var invalidRelativeSource = "invalid relative source";
            Assert.ThrowsException<FormatException>(() => relativeSourceConverter.ConvertFrom(invalidRelativeSource));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var relativeSourceConverter = new RelativeSourceConverter();
            Assert.ThrowsException<NotSupportedException>(() => relativeSourceConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var relativeSourceConverter = new RelativeSourceConverter();
            Assert.ThrowsException<NotSupportedException>(() => relativeSourceConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var relativeSourceConverter = new RelativeSourceConverter();
            var test = relativeSourceConverter.ConvertTo(new RelativeSource { Mode = RelativeSourceMode.Self }, typeof(string));
            test.Should().Be("Self");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var relativeSourceConverter = new RelativeSourceConverter();
            Assert.ThrowsException<ArgumentNullException>(() => relativeSourceConverter.ConvertTo(null, null));
            Assert.ThrowsException<ArgumentNullException>(() => relativeSourceConverter.ConvertTo(new RelativeSource(), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var relativeSourceConverter = new RelativeSourceConverter();
            Assert.ThrowsException<NotSupportedException>(() => relativeSourceConverter.ConvertTo(true, typeof(bool)));
            Assert.ThrowsException<NotSupportedException>(() => relativeSourceConverter.ConvertTo(new RelativeSource(), typeof(bool)));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentException()
        {
            var relativeSourceConverter = new RelativeSourceConverter();
            Assert.ThrowsException<ArgumentException>(() => relativeSourceConverter.ConvertTo(new RelativeSource { Mode = RelativeSourceMode.FindAncestor }, typeof(string)));
        }
    }
}
