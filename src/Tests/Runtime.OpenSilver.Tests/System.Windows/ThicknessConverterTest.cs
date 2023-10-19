
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
    public class ThicknessConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new ThicknessConverter();

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
        public void ConvertFrom_String_Should_Return_Thickness_1()
        {
            Converter.ConvertFrom("1")
                .Should()
                .Be(new Thickness(1));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Thickness_2()
        {
            Converter.ConvertFrom("  1 ")
                .Should()
                .Be(new Thickness(1));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Thickness_3()
        {
            Converter.ConvertFrom("1,2")
                .Should()
                .Be(new Thickness(1, 2, 1, 2));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Thickness_4()
        {
            Converter.ConvertFrom("1 2")
                .Should()
                .Be(new Thickness(1, 2, 1, 2));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Thickness_5()
        {
            Converter.ConvertFrom(" 1   ,2  ")
                .Should()
                .Be(new Thickness(1, 2, 1, 2));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Thickness_6()
        {
            Converter.ConvertFrom("1,2,3,4")
                .Should()
                .Be(new Thickness(1, 2, 3, 4));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Thickness_7()
        {
            Converter.ConvertFrom("1 2 3 4")
                .Should()
                .Be(new Thickness(1, 2, 3, 4));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Thickness_8()
        {
            Converter.ConvertFrom("   1,2  3  , 4  ")
                .Should()
                .Be(new Thickness(1, 2, 3, 4));
        }

        [TestMethod]
        public void ConvertFrom_Numeric_Types_Return_Thickness()
        {
            Converter.ConvertFrom(100m).Should().Be(new Thickness(100));
            Converter.ConvertFrom(100f).Should().Be(new Thickness(100));
            Converter.ConvertFrom(100d).Should().Be(new Thickness(100));
            Converter.ConvertFrom((short)100).Should().Be(new Thickness(100));
            Converter.ConvertFrom(100).Should().Be(new Thickness(100));
            Converter.ConvertFrom(100L).Should().Be(new Thickness(100));
            Converter.ConvertFrom((ushort)100).Should().Be(new Thickness(100));
            Converter.ConvertFrom(100U).Should().Be(new Thickness(100));
            Converter.ConvertFrom(100UL).Should().Be(new Thickness(100));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Throw_FormatException()
        {
            Assert.ThrowsException<FormatException>(
                () => Converter.ConvertFrom("1,2,3")
            );
        }

        [TestMethod]
        public void ConvertFrom_Null_Should_Throw_NotSupportedException()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertFrom(null)
            );
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            Converter.ConvertTo(new Thickness(1), typeof(string))
                .Should()
                .Be("1,1,1,1");
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException_1()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => Converter.ConvertTo(new Thickness(), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException_2()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => Converter.ConvertTo(null, typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(
                () => Converter.ConvertTo(true, typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Sould_Throw_NotSupportedException()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(new Thickness(), typeof(long))
            );
        }
    }
}
