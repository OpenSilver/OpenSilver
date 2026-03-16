
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
            Assert.IsTrue(Converter.CanConvertFrom(typeof(string)));
            Assert.IsTrue(Converter.CanConvertFrom(typeof(Uri)));
        }

        [TestMethod]
        public void CanConvertTo_Should_Return_False()
        {
            Assert.IsFalse(Converter.CanConvertTo(typeof(string)));
            Assert.IsFalse(Converter.CanConvertTo(typeof(bool)));
            Assert.IsFalse(Converter.CanConvertTo(typeof(int)));
            Assert.IsFalse(Converter.CanConvertTo(typeof(Uri)));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_BitmapImage()
        {
            string source = "ms-appx:/Images/Logo1.png";

            object value = Converter.ConvertFrom(source);

            Assert.IsInstanceOfType<BitmapImage>(value);

            BitmapImage bmi = value as BitmapImage;

            Assert.IsNotNull(bmi.UriSource);
            Assert.AreEqual(bmi.UriSource.AbsoluteUri, source);
        }

        [TestMethod]
        public void ConvertFrom_Null_Should_Throw_NotSupportedException()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertFrom(null)
            );
        }

        [TestMethod]
        public void ConvertFrom_Bool_Should_Throw_NotSupportedException()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertFrom(true)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotImplementedException_1()
        {
            Assert.Throws<NotImplementedException>(
                () => Converter.ConvertTo(null, typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotImplementedException_2()
        {
            Assert.Throws<NotImplementedException>(
                () => Converter.ConvertTo(new BitmapImage(new Uri("ms-appx:/Images/Logo1.png", UriKind.Absolute)), typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotImplementedException_3()
        {
            Assert.Throws<NotImplementedException>(
                () => Converter.ConvertTo(new BitmapImage(new Uri("ms-appx:/Images/Logo1.png", UriKind.Absolute)), typeof(decimal))
            );
        }
    }
}
