﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Media.Tests
#else
namespace Windows.UI.Xaml.Media.Tests
#endif
{
    [TestClass]
    public class DoubleCollectionConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var doubleCollectionConverter = new DoubleCollectionConverter();
            var test = doubleCollectionConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var doubleCollectionConverter = new DoubleCollectionConverter();
            var test = doubleCollectionConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var doubleCollectionConverter = new DoubleCollectionConverter();
            var test = doubleCollectionConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var doubleCollectionConverter = new DoubleCollectionConverter();
            var test = doubleCollectionConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnDoubleCollection()
        {
            var doubleCollectionConverter = new DoubleCollectionConverter();
            var expected = new DoubleCollection { 1.0, 2.0 };
            var test = doubleCollectionConverter.ConvertFrom("1.0, 2.0");
            test.Should().Be(expected);
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_ArgumentNullException()
        {
            var doubleCollectionConverter = new DoubleCollectionConverter();
            Assert.ThrowsException<ArgumentNullException>(() => doubleCollectionConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var doubleCollectionConverter = new DoubleCollectionConverter();
            Assert.ThrowsException<NotSupportedException>(() => doubleCollectionConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var doubleCollectionConverter = new DoubleCollectionConverter();
            var test = doubleCollectionConverter.ConvertTo(new DoubleCollection { 1.0, 2.0 }, typeof(string));
            test.Should().Be("1.0, 2.0");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var doubleCollectionConverter = new DoubleCollectionConverter();
            Assert.ThrowsException<ArgumentNullException>(() => doubleCollectionConverter.ConvertTo(null, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var doubleCollectionConverter = new DoubleCollectionConverter();
            Assert.ThrowsException<NotSupportedException>(() => doubleCollectionConverter.ConvertTo(true, typeof(string)));
            Assert.ThrowsException<NotSupportedException>(() => doubleCollectionConverter.ConvertTo(new DoubleCollection(), typeof(bool)));
        }
    }
}
