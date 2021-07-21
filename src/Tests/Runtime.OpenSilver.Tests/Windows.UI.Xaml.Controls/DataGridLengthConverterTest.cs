using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Design.Serialization;

#if MIGRATION
namespace System.Windows.Controls.Tests
#else
namespace Windows.UI.Xaml.Controls.Tests
#endif
{
    [TestClass]
    public class DataGridLengthConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var dataGridLengthConverter = new DataGridLengthConverter();
            var test = dataGridLengthConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var dataGridLengthConverter = new DataGridLengthConverter();
            var test = dataGridLengthConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var dataGridLengthConverter = new DataGridLengthConverter();
            var test = dataGridLengthConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var dataGridLengthConverter = new DataGridLengthConverter();
            var test = dataGridLengthConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnAutoDataGridLength()
        {
            var dataGridLengthConverter = new DataGridLengthConverter();
            var test = dataGridLengthConverter.ConvertFrom("Auto");
            test.Should().Be(new DataGridLength(1, DataGridLengthUnitType.Auto));
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnStarDataGridLength()
        {
            var dataGridLengthConverter = new DataGridLengthConverter();
            var test = dataGridLengthConverter.ConvertFrom("*");
            test.Should().Be(new DataGridLength(1, DataGridLengthUnitType.Star));
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnPixelDataGridLength()
        {
            var dataGridLengthConverter = new DataGridLengthConverter();
            var test = dataGridLengthConverter.ConvertFrom("100");
            test.Should().Be(new DataGridLength(100));
        }

        [TestMethod]
        public void ConvertFrom_InvalidDataGridLength_ShouldThrow_Exception()
        {
            var dataGridLengthConverter = new DataGridLengthConverter();
            var invalidDataGridLength = "invalid data grid length";
            Assert.ThrowsException<Exception>(() => dataGridLengthConverter.ConvertFrom(invalidDataGridLength));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var dataGridLengthConverter = new DataGridLengthConverter();
            Assert.ThrowsException<NotSupportedException>(() => dataGridLengthConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var dataGridLengthConverter = new DataGridLengthConverter();
            Assert.ThrowsException<NotSupportedException>(() => dataGridLengthConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldReturnPixels()
        {
            var dataGridLengthConverter = new DataGridLengthConverter();
            var test = dataGridLengthConverter.ConvertTo(new DataGridLength(100), typeof(string));
            test.Should().Be("100");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldReturnAuto()
        {
            var dataGridLengthConverter = new DataGridLengthConverter();
            var test = dataGridLengthConverter.ConvertTo(new DataGridLength(100, DataGridLengthUnitType.Auto), typeof(string));
            test.Should().Be("Auto");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldReturnStar()
        {
            var dataGridLengthConverter = new DataGridLengthConverter();
            var test = dataGridLengthConverter.ConvertTo(new DataGridLength(1, DataGridLengthUnitType.Star), typeof(string));
            test.Should().Be("*");
        }

        [TestMethod]
        public void ConvertTo_InstanceDescriptor()
        {
            var dataGridLengthConverter = new DataGridLengthConverter();
            var test = dataGridLengthConverter.ConvertTo(new DataGridLength(), typeof(InstanceDescriptor));
            test.Should().BeOfType(typeof(InstanceDescriptor));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var dataGridLengthConverter = new DataGridLengthConverter();
            Assert.ThrowsException<ArgumentNullException>(() => dataGridLengthConverter.ConvertTo(new DataGridLength(), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var dataGridLengthConverter = new DataGridLengthConverter();
            Assert.ThrowsException<NotSupportedException>(() => dataGridLengthConverter.ConvertTo(null, typeof(bool)));
            Assert.ThrowsException<NotSupportedException>(() => dataGridLengthConverter.ConvertTo(true, typeof(bool)));
            Assert.ThrowsException<NotSupportedException>(() => dataGridLengthConverter.ConvertTo(new DataGridLength(), typeof(bool)));
        }
    }
}
