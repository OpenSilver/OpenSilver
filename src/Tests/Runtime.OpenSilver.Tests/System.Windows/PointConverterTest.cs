
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
    public class PointConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new PointConverter();

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
        public void ConvertFrom_String_Should_Return_Point_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("1,1"), new Point(1, 1));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Point_2()
        {
            Assert.AreEqual(Converter.ConvertFrom("  1, 1  "), new Point(1, 1));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Point_3()
        {
            Assert.AreEqual(Converter.ConvertFrom("  1   1"), new Point(1, 1));
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
                () => Converter.ConvertFrom("1,1,1")
            );
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            Assert.AreEqual("1,1", Converter.ConvertTo(new Point(1, 1), typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => Converter.ConvertTo(new Point(1, 1), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(new Point(1, 1), typeof(bool))
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
