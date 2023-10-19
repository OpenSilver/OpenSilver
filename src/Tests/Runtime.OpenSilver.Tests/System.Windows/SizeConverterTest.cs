
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
    public class SizeConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new SizeConverter();

        [TestMethod]
        public void CanConvertFrom_String_Should_Return_True()
        {
            Converter.CanConvertFrom(typeof(string))
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_Should_Return_False()
        {
            Converter.CanConvertFrom(typeof(bool))
                .Should()
                .BeFalse();
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
        public void ConvertFrom_String_Should_Return_Size_1()
        {
            Converter.ConvertFrom("100,100")
                .Should()
                .Be(new Size(100, 100));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Size_2()
        {
            Converter.ConvertFrom(" 100,   100   ")
                .Should()
                .Be(new Size(100, 100));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Size_3()
        {
            Converter.ConvertFrom(" 100  100   ")
                .Should()
                .Be(new Size(100, 100));
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
        public void ConvertFrom_String_Should_Throw_FormatException()
        {
            Assert.ThrowsException<FormatException>(
                () => Converter.ConvertFrom("100, 100, 100")
            );
        }

        [TestMethod]
        public void ConvertTo_String_1()
        {
            Converter.ConvertTo(Size.Empty, typeof(string))
                .Should()
                .Be("Empty");
        }

        [TestMethod]
        public void ConvertTo_String_2()
        {
            Converter.ConvertTo(new Size(100, 100), typeof(string))
                .Should()
                .Be("100,100");
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => Converter.ConvertTo(new Size(1, 1), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(new Size(1, 1), typeof(bool))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_2()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(new Rect(1, 1, 1, 1), typeof(string))
            );
        }
    }
}
