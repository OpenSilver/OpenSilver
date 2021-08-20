using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

#if MIGRATION
namespace System.Windows.Media.Tests
#else
namespace Windows.UI.Xaml.Media.Tests
#endif
{
    [TestClass]
    public class PointCollectionConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var pointCollectionConverter = new PointCollectionConverter();
            var test = pointCollectionConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var pointCollectionConverter = new PointCollectionConverter();
            var test = pointCollectionConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var pointCollectionConverter = new PointCollectionConverter();
            var test = pointCollectionConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var pointCollectionConverter = new PointCollectionConverter();
            var test = pointCollectionConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnPointCollection()
        {
            var pointCollectionConverter = new PointCollectionConverter();
            var test = pointCollectionConverter.ConvertFrom("1, 1, 2, 2");
            test.Should().BeOfType<PointCollection>();

            var pc = (PointCollection)test;
            pc.Count.Should().Be(2);
            pc[0].Should().Be(new Point(1, 1));
            pc[1].Should().Be(new Point(2, 2));
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldThrow_FormatException()
        {
            var pointCollectionConverter = new PointCollectionConverter();
            Assert.ThrowsException<FormatException>(() => pointCollectionConverter.ConvertFrom("1,1,1"));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var pointCollectionConverter = new PointCollectionConverter();
            Assert.ThrowsException<NotSupportedException>(() => pointCollectionConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var pointCollectionConverter = new PointCollectionConverter();
            Assert.ThrowsException<NotSupportedException>(() => pointCollectionConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var pointCollectionConverter = new PointCollectionConverter();
            var test = pointCollectionConverter.ConvertTo(new PointCollection(), typeof(string));
            test.Should().Be(new PointCollection().ToString());
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var pointCollectionConverter = new PointCollectionConverter();
            Assert.ThrowsException<ArgumentNullException>(() => pointCollectionConverter.ConvertTo(new PointCollection(), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var pointCollectionConverter = new PointCollectionConverter();
            Assert.ThrowsException<NotSupportedException>(() => pointCollectionConverter.ConvertTo(true, typeof(bool)));
            Assert.ThrowsException<NotSupportedException>(() => pointCollectionConverter.ConvertTo(new PointCollection(), typeof(bool)));
        }
    }
}
