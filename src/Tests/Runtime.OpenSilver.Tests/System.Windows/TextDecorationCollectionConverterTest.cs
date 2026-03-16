
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
    public class TextDecorationCollectionConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new TextDecorationCollectionConverter();

        [TestMethod]
        public void CanConvertFrom_String_Should_Return_True()
        {
            Assert.IsTrue(Converter.CanConvertFrom(typeof(string)));
        }

        [TestMethod]
        public void CanConvertFrom_Bool_Should_Return_False()
        {
            Assert.IsFalse(Converter.CanConvertFrom(typeof(bool)));
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
        public void ConvertFrom_String_Should_Return_TextDecorationCollection_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("Underline"), TextDecorations.Underline);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_TextDecorationCollection_2()
        {
            Assert.AreEqual(Converter.ConvertFrom("underline"), TextDecorations.Underline);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_TextDecorationCollection_3()
        {
            Assert.AreEqual(Converter.ConvertFrom("Strikethrough"), TextDecorations.Strikethrough);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_TextDecorationCollection_4()
        {
            Assert.AreEqual(Converter.ConvertFrom("strikethrough"), TextDecorations.Strikethrough);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_TextDecorationCollection_5()
        {
            Assert.AreEqual(Converter.ConvertFrom("OverLine"), TextDecorations.OverLine);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_TextDecorationCollection_6()
        {
            Assert.AreEqual(Converter.ConvertFrom("overline"), TextDecorations.OverLine);
        }

        [TestMethod]
        public void ConvertFrom_Null_Should_Throw_NotSupportedException()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertFrom(null)
            );
        }

        [TestMethod]
        public void ConvertFrom_Bool_Should_Throw_NotSupportedException()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertFrom(true)
            );
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Throw_FormatException()
        {
            Assert.Throws<FormatException>(
                () => Converter.ConvertFrom("invalid text decoration")
            );
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            Assert.AreEqual(Converter.ConvertTo(TextDecorations.Underline, typeof(string)), TextDecorations.Underline.ToString());
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => Converter.ConvertTo(TextDecorations.OverLine, null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(TextDecorations.OverLine, typeof(long))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_2()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(new PropertyPath("."), typeof(string))
            );
        }
    }
}
