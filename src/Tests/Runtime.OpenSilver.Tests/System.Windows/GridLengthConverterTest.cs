
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
using FluentAssertions;
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
            Converter.CanConvertFrom(typeof(string)).Should().BeTrue();
            Converter.CanConvertFrom(typeof(decimal)).Should().BeTrue();
            Converter.CanConvertFrom(typeof(float)).Should().BeTrue();
            Converter.CanConvertFrom(typeof(double)).Should().BeTrue();
            Converter.CanConvertFrom(typeof(short)).Should().BeTrue();
            Converter.CanConvertFrom(typeof(int)).Should().BeTrue();
            Converter.CanConvertFrom(typeof(long)).Should().BeTrue();
            Converter.CanConvertFrom(typeof(ushort)).Should().BeTrue();
            Converter.CanConvertFrom(typeof(uint)).Should().BeTrue();
            Converter.CanConvertFrom(typeof(ulong)).Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_String_Should_Return_True()
        {
            Converter.CanConvertTo(typeof(string))
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_Should_Return_False()
        {
            Converter.CanConvertTo(typeof(bool))
                .Should()
                .BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Pixels_1()
        {
            Converter.ConvertFrom("100")
                .Should()
                .Be(new GridLength(100));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Pixels_2()
        {
            Converter.ConvertFrom("100px")
                .Should()
                .Be(new GridLength(100));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Pixels_3()
        {
            Converter.ConvertFrom(".5")
                .Should()
                .Be(new GridLength(0.5));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Pixels_4()
        {
            Converter.ConvertFrom("..5")
                .Should()
                .Be(new GridLength(0.0));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Pixels_5()
        {
            Converter.ConvertFrom("100.420.hi")
                .Should()
                .Be(new GridLength(100.420));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Pixels_6()
        {
            Converter.ConvertFrom(" 100what ever ")
                .Should()
                .Be(new GridLength(100));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Auto_1()
        {
            Converter.ConvertFrom("Auto")
                .Should()
                .Be(GridLength.Auto);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Auto_2()
        {
            Converter.ConvertFrom("auTo")
                .Should()
                .Be(GridLength.Auto);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Star_1()
        {
            Converter.ConvertFrom("*")
                .Should()
                .Be(new GridLength(1.0, GridUnitType.Star));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Star_2()
        {
            Converter.ConvertFrom("  * ")
                .Should()
                .Be(new GridLength(1.0, GridUnitType.Star));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Star_3()
        {
            Converter.ConvertFrom(".*")
                .Should()
                .Be(new GridLength(0.0, GridUnitType.Star));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Star_4()
        {
            Converter.ConvertFrom("0.33*")
                .Should()
                .Be(new GridLength(0.33, GridUnitType.Star));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_GridLength_Star_5()
        {
            Converter.ConvertFrom(" 0.33.4a2bc0*")
                .Should()
                .Be(new GridLength(0.33, GridUnitType.Star));
        }

        [TestMethod]
        public void ConvertFrom_Double_NaN_Should_Return_GridLength_Auto()
        {
            Converter.ConvertFrom(double.NaN)
                .Should()
                .Be(GridLength.Auto);
        }

        [TestMethod]
        public void ConvertFrom_Numeric_Types_Should_Return_GridLength_Pixels()
        {
            Converter.ConvertFrom(100m).Should().Be(new GridLength(100));
            Converter.ConvertFrom(100f).Should().Be(new GridLength(100));
            Converter.ConvertFrom(100d).Should().Be(new GridLength(100));
            Converter.ConvertFrom((short)100).Should().Be(new GridLength(100));
            Converter.ConvertFrom(100).Should().Be(new GridLength(100));
            Converter.ConvertFrom(100L).Should().Be(new GridLength(100));
            Converter.ConvertFrom((ushort)100).Should().Be(new GridLength(100));
            Converter.ConvertFrom(100U).Should().Be(new GridLength(100));
            Converter.ConvertFrom(100UL).Should().Be(new GridLength(100));
        }

        [TestMethod]
        public void ConvertFrom_Null_Should_Throw_NotSupportedException()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertFrom(null)
            );
        }

        [TestMethod]
        public void ConvertFrom_Style_Should_Throw_InvalidCastException()
        {
            Assert.ThrowsException<InvalidCastException>(
                () => Converter.ConvertFrom(new Style())
            );
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            Converter.ConvertTo(new GridLength(100), typeof(string))
                .Should()
                .Be("100");
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => Converter.ConvertTo(new GridLength(100), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(true, typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_2()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(new GridLength(100), typeof(bool))
            );
        }
    }
}
