
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

using System.ComponentModel;
using OpenSilver.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Tests
{
    [TestClass]
    public class CornerRadiusConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new CornerRadiusConverter();

        [TestMethod]
        public void CanConvertFrom_Should_Return_True()
        {
            Assert.IsTrue(Converter.CanConvertFrom(typeof(string)));
            Assert.IsTrue(Converter.CanConvertFrom(typeof(decimal)));
            Assert.IsTrue(Converter.CanConvertFrom(typeof(float)));
            Assert.IsTrue(Converter.CanConvertFrom(typeof(double)));
            Assert.IsTrue(Converter.CanConvertFrom(typeof(short)));
            Assert.IsTrue(Converter.CanConvertFrom(typeof(int)));
            Assert.IsTrue(Converter.CanConvertFrom(typeof(long)));
            Assert.IsTrue(Converter.CanConvertFrom(typeof(ushort)));
            Assert.IsTrue(Converter.CanConvertFrom(typeof(uint)));
            Assert.IsTrue(Converter.CanConvertFrom(typeof(ulong)));
        }

        [TestMethod]
        public void CanConvertTo_String_Should_Return_True()
        {
            Assert.IsTrue(Converter.CanConvertTo(typeof(string)));
        }

        [TestMethod]
        public void CanConvertTo_Bool_Should_Return_False()
        {
            Assert.IsFalse(Converter.CanConvertTo(typeof(bool)));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_CornerRadius_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("1,2,3,4"), new CornerRadius(1, 2, 3, 4));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_CornerRadius_2()
        {
            Assert.AreEqual(Converter.ConvertFrom("1 2 3 4"), new CornerRadius(1, 2, 3, 4));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_CornerRadius_3()
        {
            Assert.AreEqual(Converter.ConvertFrom(" 1, 2 ,3  4  "), new CornerRadius(1, 2, 3, 4));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_CornerRadius_Uniform_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("1"), new CornerRadius(1));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_CornerRadius_Uniform_2()
        {
            Assert.AreEqual(Converter.ConvertFrom(" 1  "), new CornerRadius(1));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Throw_FormatException()
        {
            Assert.Throws<FormatException>(
                () => Converter.ConvertFrom("1,2,3")
            );
        }

        [TestMethod]
        public void ConvertFrom_Null_Should_Throw_NotSupportedException()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertFrom(null)
            );
        }

        [TestMethod]
        public void ConvertFrom_Style_Should_Throw_InvalidCastException()
        {
            Assert.Throws<InvalidCastException>(
                () => Converter.ConvertFrom(new Style())
            );
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            Assert.AreEqual("1,2,3,4", Converter.ConvertTo(new CornerRadius(1, 2, 3, 4), typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException_1()
        {
            Assert.Throws<ArgumentNullException>(
                () => Converter.ConvertTo(null, typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException_2()
        {
            Assert.Throws<ArgumentNullException>(
                () => Converter.ConvertTo(new CornerRadius(), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => Converter.ConvertTo(true, typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(new CornerRadius(1), typeof(bool))
            );
        }
    }
}
