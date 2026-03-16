
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
    public class SizeConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new SizeConverter();

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
        public void ConvertFrom_String_Should_Return_Size_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("100,100"), new Size(100, 100));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Size_2()
        {
            Assert.AreEqual(Converter.ConvertFrom(" 100,   100   "), new Size(100, 100));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Size_3()
        {
            Assert.AreEqual(Converter.ConvertFrom(" 100  100   "), new Size(100, 100));
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
        public void ConvertFrom_String_Should_Throw_InvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(
                () => Converter.ConvertFrom("100, 100, 100")
            );
        }

        [TestMethod]
        public void ConvertTo_String_1()
        {
            Assert.AreEqual("Empty", Converter.ConvertTo(Size.Empty, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_String_2()
        {
            Assert.AreEqual("100,100", Converter.ConvertTo(new Size(100, 100), typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => Converter.ConvertTo(new Size(1, 1), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(new Size(1, 1), typeof(bool))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_2()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(new Rect(1, 1, 1, 1), typeof(string))
            );
        }
    }
}
