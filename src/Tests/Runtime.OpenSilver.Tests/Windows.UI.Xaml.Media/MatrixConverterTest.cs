using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Media.Tests
#else
namespace Windows.UI.Xaml.Media.Tests
#endif
{
    [TestClass]
    public class MatrixConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var matrixConverter = new MatrixConverter();
            var test = matrixConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var matrixConverter = new MatrixConverter();
            var test = matrixConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var matrixConverter = new MatrixConverter();
            var test = matrixConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var matrixConverter = new MatrixConverter();
            var test = matrixConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnMatrix()
        {
            var matrixConverter = new MatrixConverter();
            var expected = new Matrix(1, 2, 3, 4, 5, 6);
            var test = matrixConverter.ConvertFrom("1,2,3,4,5,6");
            test.Should().Be(expected);
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_FormatException()
        {
            var matrixConverter = new MatrixConverter();
            Assert.ThrowsException<FormatException>(() => matrixConverter.ConvertFrom("1"));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var matrixConverter = new MatrixConverter();
            Assert.ThrowsException<NotSupportedException>(() => matrixConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var matrixConverter = new MatrixConverter();
            Assert.ThrowsException<NotSupportedException>(() => matrixConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var matrixConverter = new MatrixConverter();
            var test = matrixConverter.ConvertTo(new Matrix(1, 2, 3, 4, 5, 6), typeof(string));
            test.Should().Be("1,2,3,4,5,6");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var matrixConverter = new MatrixConverter();
            Assert.ThrowsException<ArgumentNullException>(() => matrixConverter.ConvertTo(new Matrix(), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var matrixConverter = new MatrixConverter();
            Assert.ThrowsException<NotSupportedException>(() => matrixConverter.ConvertTo(true, typeof(bool)));
            Assert.ThrowsException<NotSupportedException>(() => matrixConverter.ConvertTo(new Matrix(), typeof(bool)));
        }
    }
}
