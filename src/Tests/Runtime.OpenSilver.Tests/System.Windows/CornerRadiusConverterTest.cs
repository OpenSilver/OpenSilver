
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
    public class CornerRadiusConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new CornerRadiusConverter();

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
        public void ConvertFrom_String_Should_Return_CornerRadius_1()
        {
            Converter.ConvertFrom("1,2,3,4")
                .Should()
                .Be(new CornerRadius(1, 2, 3, 4));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_CornerRadius_2()
        {
            Converter.ConvertFrom("1 2 3 4")
                .Should()
                .Be(new CornerRadius(1, 2, 3, 4));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_CornerRadius_3()
        {
            Converter.ConvertFrom(" 1, 2 ,3  4  ")
                .Should()
                .Be(new CornerRadius(1, 2, 3, 4));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_CornerRadius_Uniform_1()
        {
            Converter.ConvertFrom("1")
                .Should()
                .Be(new CornerRadius(1));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_CornerRadius_Uniform_2()
        {
            Converter.ConvertFrom(" 1  ")
                .Should()
                .Be(new CornerRadius(1));
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
        public void ConvertFrom_Style_Should_Throw_InvalidCastException()
        {
            Assert.ThrowsException<InvalidCastException>(
                () => Converter.ConvertFrom(new Style())
            );
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            Converter.ConvertTo(new CornerRadius(1, 2, 3, 4), typeof(string))
                .Should()
                .Be("1,2,3,4");
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException_1()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => Converter.ConvertTo(null, typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException_2()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => Converter.ConvertTo(new CornerRadius(), null)
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
        public void ConvertTo_Should_Throw_NotSupportedException()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(new CornerRadius(1), typeof(bool))
            );
        }
    }
}
