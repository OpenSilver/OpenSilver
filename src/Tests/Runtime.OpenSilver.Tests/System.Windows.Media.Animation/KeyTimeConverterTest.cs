
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

namespace System.Windows.Media.Animation.Tests
{
    [TestClass]
    public class KeyTimeConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new KeyTimeConverter();

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
        public void ConvertFrom_String_Should_Return_KeyTime()
        {
            Assert.AreEqual(Converter.ConvertFrom("3"), KeyTime.FromTimeSpan(TimeSpan.FromDays(3)));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_KeyTime_Uniform()
        {
            Assert.AreEqual(Converter.ConvertFrom("Uniform"), KeyTime.Uniform);
            Assert.AreEqual(Converter.ConvertFrom("  Uniform   "), KeyTime.Uniform);
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
        public void ConvertFrom_Paced_Should_Throw_NotSupportedException()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertFrom("Paced")
            );
        }

        [TestMethod]
        public void ConvertFrom_Percent_Should_Throw_NotSupportedException()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertFrom("%")
            );
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            Assert.AreEqual("3.00:00:00", Converter.ConvertTo(KeyTime.FromTimeSpan(TimeSpan.FromDays(3)), typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(null, typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_2()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(true, typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_3()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(KeyTime.FromTimeSpan(TimeSpan.FromDays(1)), typeof(bool))
            );
        }
    }
}
