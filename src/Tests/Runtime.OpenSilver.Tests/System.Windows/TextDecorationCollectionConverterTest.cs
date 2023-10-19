
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
    public class TextDecorationCollectionConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new TextDecorationCollectionConverter();

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
        public void ConvertFrom_String_Should_Return_TextDecorationCollection_1()
        {
            Converter.ConvertFrom("Underline")
                .Should()
                .Be(TextDecorations.Underline);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_TextDecorationCollection_2()
        {
            Converter.ConvertFrom("underline")
                .Should()
                .Be(TextDecorations.Underline);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_TextDecorationCollection_3()
        {
            Converter.ConvertFrom("Strikethrough")
                .Should()
                .Be(TextDecorations.Strikethrough);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_TextDecorationCollection_4()
        {
            Converter.ConvertFrom("strikethrough")
                .Should()
                .Be(TextDecorations.Strikethrough);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_TextDecorationCollection_5()
        {
            Converter.ConvertFrom("OverLine")
                .Should()
                .Be(TextDecorations.OverLine);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_TextDecorationCollection_6()
        {
            Converter.ConvertFrom("overline")
                .Should()
                .Be(TextDecorations.OverLine);
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
                () => Converter.ConvertFrom("invalid text decoration")
            );
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            Converter.ConvertTo(TextDecorations.Underline, typeof(string))
                .Should()
                .Be(TextDecorations.Underline.ToString());
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => Converter.ConvertTo(TextDecorations.OverLine, null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(TextDecorations.OverLine, typeof(long))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_2()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(new PropertyPath("."), typeof(string))
            );
        }
    }
}
