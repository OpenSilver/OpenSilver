
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
        public void ConvertFrom_String_Should_Return_FontFamily()
        {
            object value = Converter.ConvertFrom("Verdana");

            Assert.IsInstanceOfType<FontFamily>(value);

            FontFamily fontFamily = value as FontFamily;

            Assert.AreEqual("Verdana", fontFamily.Source);
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
        public void ConvertTo_String_Should_Return_FontFamily_Source()
        {
            var ff = new FontFamily("Comic Sans");
            
            Assert.AreEqual(Converter.ConvertTo(ff, typeof(string)), ff.Source);
        }

        [TestMethod]
        public void ConvertTo_String_Should_Return_String_Empty()
        {
            Assert.AreEqual(Converter.ConvertTo(new FontFamily(null), typeof(string)), string.Empty);
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException_1()
        {
            Assert.Throws<ArgumentNullException>(
                () => Converter.ConvertTo(null, typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException_2()
        {
            Assert.Throws<ArgumentNullException>(
                () => Converter.ConvertTo(new FontFamily("Verdana"), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => Converter.ConvertTo("Not_A_FontFamily", typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(new FontFamily("Arial"), typeof(long))
            );
        }
    }
}
