
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
    public class GridLengthConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new GridLengthConverter();

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
        public void ConvertFrom_String_Should_Return_GridLength_Pixels_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("100"), new GridLength(100));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Pixels_2()
        {
            Assert.AreEqual(Converter.ConvertFrom("100px"), new GridLength(100));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Pixels_3()
        {
            Assert.AreEqual(Converter.ConvertFrom(".5"), new GridLength(0.5));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Pixels_4()
        {
            Assert.AreEqual(Converter.ConvertFrom("..5"), new GridLength(0.0));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Pixels_5()
        {
            Assert.AreEqual(Converter.ConvertFrom("100.420.hi"), new GridLength(100.420));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Pixels_6()
        {
            Assert.AreEqual(Converter.ConvertFrom(" 100what ever "), new GridLength(100));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Auto_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("Auto"), GridLength.Auto);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Auto_2()
        {
            Assert.AreEqual(Converter.ConvertFrom("auTo"), GridLength.Auto);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Star_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("*"), new GridLength(1.0, GridUnitType.Star));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Star_2()
        {
            Assert.AreEqual(Converter.ConvertFrom("  * "), new GridLength(1.0, GridUnitType.Star));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Star_3()
        {
            Assert.AreEqual(Converter.ConvertFrom(".*"), new GridLength(0.0, GridUnitType.Star));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Star_4()
        {
            Assert.AreEqual(Converter.ConvertFrom("0.33*"), new GridLength(0.33, GridUnitType.Star));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Star_5()
        {
            Assert.AreEqual(Converter.ConvertFrom(" 0.33.4a2bc0*"), new GridLength(0.33, GridUnitType.Star));
        }

        [TestMethod]
        public void ConvertFrom_Double_NaN_Should_Return_GridLength_Auto()
        {
            Assert.AreEqual(Converter.ConvertFrom(double.NaN), GridLength.Auto);
        }

        [TestMethod]
        public void ConvertFrom_Numeric_Types_Should_Return_GridLength_Pixels()
        {
            Assert.AreEqual(Converter.ConvertFrom(100m), new GridLength(100));
            Assert.AreEqual(Converter.ConvertFrom(100f), new GridLength(100));
            Assert.AreEqual(Converter.ConvertFrom(100d), new GridLength(100));
            Assert.AreEqual(Converter.ConvertFrom((short)100), new GridLength(100));
            Assert.AreEqual(Converter.ConvertFrom(100), new GridLength(100));
            Assert.AreEqual(Converter.ConvertFrom(100L), new GridLength(100));
            Assert.AreEqual(Converter.ConvertFrom((ushort)100), new GridLength(100));
            Assert.AreEqual(Converter.ConvertFrom(100U), new GridLength(100));
            Assert.AreEqual(Converter.ConvertFrom(100UL), new GridLength(100));
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
            Assert.AreEqual("100", Converter.ConvertTo(new GridLength(100), typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => Converter.ConvertTo(new GridLength(100), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(true, typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_2()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(new GridLength(100), typeof(bool))
            );
        }
    }
}
