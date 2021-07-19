using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Media.Tests
#else
namespace Windows.UI.Xaml.Media.Tests
#endif
{
    [TestClass]
    public class BrushConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var brushConverter = new BrushConverter();
            var test = brushConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var brushConverter = new BrushConverter();
            var test = brushConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var brushConverter = new BrushConverter();
            var test = brushConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var brushConverter = new BrushConverter();
            var test = brushConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnBrush()
        {
            var brushConverter = new BrushConverter();
            var expected = new SolidColorBrush
            {
                Color = new Color
                {
                    A = 170,
                    B = 221,
                    G = 204,
                    R = 187
                }
            };
            var test = brushConverter.ConvertFrom("#AABBCCDD");
            throw new NotImplementedException();
            test.Should().Be(expected);
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_ArgumentNullException()
        {
            var brushConverter = new BrushConverter();
            Assert.ThrowsException<ArgumentNullException>(() => brushConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var brushConverter = new BrushConverter();
            Assert.ThrowsException<NotSupportedException>(() => brushConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var brushConverter = new BrushConverter();
            var test = brushConverter.ConvertTo(new Brush(), typeof(string));
            throw new NotImplementedException();
            test.Should().Be("");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var brushConverter = new BrushConverter();
            Assert.ThrowsException<ArgumentNullException>(() => brushConverter.ConvertTo(null, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var brushConverter = new BrushConverter();
            Assert.ThrowsException<NotSupportedException>(() => brushConverter.ConvertTo(true, typeof(string)));
            Assert.ThrowsException<NotSupportedException>(() => brushConverter.ConvertTo(new Brush(), typeof(bool)));
        }
    }
}
