
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
    public class FontStyleConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new FontStyleConverter();

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
        public void ConvertFrom_String_Should_Return_FontStyle_1()
        {
            Converter.ConvertFrom("Normal")
                .Should()
                .Be(FontStyles.Normal);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_FontStyle_2()
        {
            Converter.ConvertFrom("normal")
                .Should()
                .Be(FontStyles.Normal);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_FontStyle_3()
        {
            Converter.ConvertFrom("Oblique")
                .Should()
                .Be(FontStyles.Oblique);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_FontStyle_4()
        {
            Converter.ConvertFrom("oblique")
                .Should()
                .Be(FontStyles.Oblique);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_FontStyle_5()
        {
            Converter.ConvertFrom("Italic")
                .Should()
                .Be(FontStyles.Italic);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_FontStyle_6()
        {
            Converter.ConvertFrom("italic")
                .Should()
                .Be(FontStyles.Italic);
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
        public void ConvertFrom_String_Should_Throw_NotSupportedException()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertFrom("invalid font style")
            );
        }

        [TestMethod]
        public void ConvertTo_String_1()
        {
            Converter.ConvertTo(FontStyles.Normal, typeof(string))
                .Should()
                .Be("Normal");
        }

        [TestMethod]
        public void ConvertTo_String_2()
        {
            Converter.ConvertTo(FontStyles.Oblique, typeof(string))
                .Should()
                .Be("Oblique");
        }

        [TestMethod]
        public void ConvertTo_String_3()
        {
            Converter.ConvertTo(FontStyles.Italic, typeof(string))
                .Should()
                .Be("Italic");
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => Converter.ConvertTo(new FontStyle(), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(new FontStyle(), typeof(bool))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_2()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(420, typeof(string))
            );
        }
    }
}
