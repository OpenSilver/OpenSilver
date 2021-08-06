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
            test.Should().Be(Cursors.None);
        }

        [TestMethod]
        public void ConvertFrom_InvalidCursorString_ShouldThrow_ArgumentException()
        {
            var cursorConverter = new CursorConverter();
            var invalidCursor = "invalidCursor";
            Assert.ThrowsException<ArgumentException>(() => cursorConverter.ConvertFrom(invalidCursor));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var cursorConverter = new CursorConverter();
            Assert.ThrowsException<NotSupportedException>(() => cursorConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var cursorConverter = new CursorConverter();
            var booleanValue = true;
            Assert.ThrowsException<NotSupportedException>(() => cursorConverter.ConvertFrom(booleanValue));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var cursorConverter = new CursorConverter();
            var test = cursorConverter.ConvertTo(Cursors.Arrow, typeof(string));
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
            Assert.ThrowsException<NotSupportedException>(() => cursorConverter.ConvertTo(Cursors.No, notSupportedType));
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
