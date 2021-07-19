using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Input.Tests
{
    [TestClass]
    public class CursorConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var cursorConverter = new CursorConverter();
            var test = cursorConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var cursorConverter = new CursorConverter();
            var test = cursorConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var cursorConverter = new CursorConverter();
            var test = cursorConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var cursorConverter = new CursorConverter();
            var test = cursorConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnCursor()
        {
            var cursorConverter = new CursorConverter();
            var test = cursorConverter.ConvertFrom("None");
            test.Should().Be(new Cursor(CursorType.None));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_ArgumentNullException()
        {
            var cursorConverter = new CursorConverter();
            Assert.ThrowsException<ArgumentNullException>(() => cursorConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var cursorConverter = new CursorConverter();
            Assert.ThrowsException<NotSupportedException>(() => cursorConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var cursorConverter = new CursorConverter();
            var test = cursorConverter.ConvertTo(new Cursor(CursorType.Arrow), typeof(string));
            test.Should().Be("Arrow");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var cursorConverter = new CursorConverter();
            Assert.ThrowsException<ArgumentNullException>(() => cursorConverter.ConvertTo(true, null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var cursorConverter = new CursorConverter();
            var notSupportedType = typeof(bool);
            Assert.ThrowsException<NotSupportedException>(() => cursorConverter.ConvertTo(new Cursor(CursorType.No), notSupportedType));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldReturn_EmptyString()
        {
            var cursorConverter = new CursorConverter();
            var test = cursorConverter.ConvertTo(null, typeof(string));
            test.Should().Be(string.Empty);
        }
    }
}
