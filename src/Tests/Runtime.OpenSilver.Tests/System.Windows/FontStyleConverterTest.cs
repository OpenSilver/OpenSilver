
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
    public class FontStyleConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new FontStyleConverter();

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
        public void ConvertFrom_String_Should_Return_FontStyle_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("Normal"), FontStyles.Normal);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_FontStyle_2()
        {
           Assert.AreEqual(Converter.ConvertFrom("normal"), FontStyles.Normal);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_FontStyle_3()
        {
            Assert.AreEqual(Converter.ConvertFrom("Oblique"), FontStyles.Oblique);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_FontStyle_4()
        {
            Assert.AreEqual(Converter.ConvertFrom("oblique"), FontStyles.Oblique);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_FontStyle_5()
        {
            Assert.AreEqual(Converter.ConvertFrom("Italic"), FontStyles.Italic);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_FontStyle_6()
        {
            Assert.AreEqual(Converter.ConvertFrom("italic"), FontStyles.Italic);
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
        public void ConvertFrom_String_Should_Throw_NotSupportedException()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertFrom("invalid font style")
            );
        }

        [TestMethod]
        public void ConvertTo_String_1()
        {
            Assert.AreEqual("Normal", Converter.ConvertTo(FontStyles.Normal, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_String_2()
        {
            Assert.AreEqual("Oblique", Converter.ConvertTo(FontStyles.Oblique, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_String_3()
        {
            Assert.AreEqual("Italic", Converter.ConvertTo(FontStyles.Italic, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => Converter.ConvertTo(new FontStyle(), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(new FontStyle(), typeof(bool))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_2()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(420, typeof(string))
            );
        }
    }
}
