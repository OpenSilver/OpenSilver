
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

namespace System.Windows.Media.Tests
{
    [TestClass]
    public class FontFamilyConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new FontFamilyConverter();

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
        public void ConvertFrom_String_Should_Return_FontFamily()
        {
            Converter.ConvertFrom("Verdana")
                .Should()
                .BeOfType<FontFamily>()
                .Which
                .Source
                .Should()
                .Be("Verdana");
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
        public void ConvertTo_String_Should_Return_FontFamily_Source()
        {
            var ff = new FontFamily("Comic Sans");
            
            Converter.ConvertTo(ff, typeof(string))
                .Should()
                .Be(ff.Source);
        }

        [TestMethod]
        public void ConvertTo_String_Should_Return_String_Empty()
        {
            Converter.ConvertTo(new FontFamily(null), typeof(string))
                .Should()
                .Be(string.Empty);
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
                () => Converter.ConvertTo(new FontFamily("Verdana"), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(
                () => Converter.ConvertTo("Not_A_FontFamily", typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(new FontFamily("Arial"), typeof(long))
            );
        }
    }
}
