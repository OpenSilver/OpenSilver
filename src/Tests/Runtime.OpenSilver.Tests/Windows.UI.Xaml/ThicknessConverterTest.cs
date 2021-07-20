using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Design.Serialization;

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.UI.Xaml.Tests
#endif
{
    [TestClass]
    public class ThicknessConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var thicknessConverter = new ThicknessConverter();
            var test = thicknessConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var thicknessConverter = new ThicknessConverter();
            var test = thicknessConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var thicknessConverter = new ThicknessConverter();
            var test = thicknessConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var thicknessConverter = new ThicknessConverter();
            var test = thicknessConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnUniformThickness()
        {
            var thicknessConverter = new ThicknessConverter();
            var test = thicknessConverter.ConvertFrom("1");
            test.Should().Be(new Thickness(1));
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnLeftRightTopBottomThickness()
        {
            var thicknessConverter = new ThicknessConverter();
            var test = thicknessConverter.ConvertFrom("1,2");
            test.Should().Be(new Thickness(1,2,1,2));
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnThickness()
        {
            var thicknessConverter = new ThicknessConverter();
            var test = thicknessConverter.ConvertFrom("1,2,3,4");
            test.Should().Be(new Thickness(1,2,3,4));
        }

        [TestMethod]
        public void ConvertFrom_InvalidThickness_ShouldThrow_FormatException()
        {
            var thicknessConverter = new ThicknessConverter();
            Assert.ThrowsException<FormatException>(() => thicknessConverter.ConvertFrom("1,2,3"));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var thicknessConverter = new ThicknessConverter();
            Assert.ThrowsException<NotSupportedException>(() => thicknessConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var thicknessConverter = new ThicknessConverter();
            Assert.ThrowsException<NotSupportedException>(() => thicknessConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var thicknessConverter = new ThicknessConverter();
            var test = thicknessConverter.ConvertTo(new Thickness(1), typeof(string));
            test.Should().Be("1,1,1,1");
        }

        [TestMethod]
        public void ConvertTo_InstanceDescriptor()
        {
            var thicknessConverter = new ThicknessConverter();
            var test = thicknessConverter.ConvertTo(new Thickness(1), typeof(InstanceDescriptor));
            test.Should().BeOfType(typeof(InstanceDescriptor));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var thicknessConverter = new ThicknessConverter();
            Assert.ThrowsException<ArgumentNullException>(() => thicknessConverter.ConvertTo(null, typeof(string)));
            Assert.ThrowsException<ArgumentNullException>(() => thicknessConverter.ConvertTo(new Thickness(), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentException()
        {
            var thicknessConverter = new ThicknessConverter();
            Assert.ThrowsException<ArgumentException>(() => thicknessConverter.ConvertTo(true, typeof(string)));
            Assert.ThrowsException<ArgumentException>(() => thicknessConverter.ConvertTo(new Thickness(), typeof(bool)));
        }
    }
}
