
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using OpenSilver.Tests;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace System.Windows.Media.Tests
{
    [TestClass]
    public class ImageSourceConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new ImageSourceConverter();

        [TestMethod]
        public void CanConvertFrom_Should_Return_True()
        {
            Converter.CanConvertFrom(typeof(string)).Should().BeTrue();
            Converter.CanConvertFrom(typeof(Uri)).Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Should_Return_False()
        {
            Converter.CanConvertTo(typeof(string)).Should().BeFalse();
            Converter.CanConvertTo(typeof(bool)).Should().BeFalse();
            Converter.CanConvertTo(typeof(int)).Should().BeFalse();
            Converter.CanConvertTo(typeof(Uri)).Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_BitmapImage()
        {
            string source = "ms-appx:/Images/Logo1.png";

            Converter.ConvertFrom(source)
                .Should()
                .BeOfType<BitmapImage>()
                .Which
                .UriSource
                .Should()
                .NotBeNull()
                .And
                .Subject
                .As<Uri>()
                .AbsoluteUri
                .Should()
                .Be(source);
        }

        [TestMethod]
        public void ConvertFrom_Null_Should_Throw_NotSupportedException()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertFrom(null)
            );
        }

        [TestMethod]
        public void ConvertFrom_Bool_Should_Throw_NotSupportedException()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertFrom(true)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotImplementedException_1()
        {
            Assert.ThrowsException<NotImplementedException>(
                () => Converter.ConvertTo(null, typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotImplementedException_2()
        {
            Assert.ThrowsException<NotImplementedException>(
                () => Converter.ConvertTo(new BitmapImage(new Uri("ms-appx:/Images/Logo1.png", UriKind.Absolute)), typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotImplementedException_3()
        {
            Assert.ThrowsException<NotImplementedException>(
                () => Converter.ConvertTo(new BitmapImage(new Uri("ms-appx:/Images/Logo1.png", UriKind.Absolute)), typeof(decimal))
            );
        }
    }
}
