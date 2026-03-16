
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
    public class ThicknessConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new ThicknessConverter();

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
        public void ConvertFrom_String_Should_Return_Thickness_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("1"), new Thickness(1));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Thickness_2()
        {
            Assert.AreEqual(Converter.ConvertFrom("  1 "), new Thickness(1));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Thickness_3()
        {
            Assert.AreEqual(Converter.ConvertFrom("1,2"), new Thickness(1, 2, 1, 2));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Thickness_4()
        {
            Assert.AreEqual(Converter.ConvertFrom("1 2"), new Thickness(1, 2, 1, 2));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Thickness_5()
        {
            Assert.AreEqual(Converter.ConvertFrom(" 1   ,2  "), new Thickness(1, 2, 1, 2));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Thickness_6()
        {
            Assert.AreEqual(Converter.ConvertFrom("1,2,3,4"), new Thickness(1, 2, 3, 4));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Thickness_7()
        {
            Assert.AreEqual(Converter.ConvertFrom("1 2 3 4"), new Thickness(1, 2, 3, 4));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Thickness_8()
        {
            Assert.AreEqual(Converter.ConvertFrom("   1,2  3  , 4  "), new Thickness(1, 2, 3, 4));
        }

        [TestMethod]
        public void ConvertFrom_Numeric_Types_Return_Thickness()
        {
            Assert.AreEqual(Converter.ConvertFrom(100m), new Thickness(100));
            Assert.AreEqual(Converter.ConvertFrom(100f), new Thickness(100));
            Assert.AreEqual(Converter.ConvertFrom(100d), new Thickness(100));
            Assert.AreEqual(Converter.ConvertFrom((short)100), new Thickness(100));
            Assert.AreEqual(Converter.ConvertFrom(100), new Thickness(100));
            Assert.AreEqual(Converter.ConvertFrom(100L), new Thickness(100));
            Assert.AreEqual(Converter.ConvertFrom((ushort)100), new Thickness(100));
            Assert.AreEqual(Converter.ConvertFrom(100U), new Thickness(100));
            Assert.AreEqual(Converter.ConvertFrom(100UL), new Thickness(100));
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
        public void ConvertTo_String()
        {
            Assert.AreEqual("1,1,1,1", Converter.ConvertTo(new Thickness(1), typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException_1()
        {
            Assert.Throws<ArgumentNullException>(
                () => Converter.ConvertTo(new Thickness(), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException_2()
        {
            Assert.Throws<ArgumentNullException>(
                () => Converter.ConvertTo(null, typeof(string))
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
        public void ConvertTo_Sould_Throw_NotSupportedException()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(new Thickness(), typeof(long))
            );
        }
    }
}
