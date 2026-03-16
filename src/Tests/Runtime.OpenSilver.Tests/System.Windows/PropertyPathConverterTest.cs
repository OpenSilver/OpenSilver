
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
using OpenSilver.Internal.Helpers;

namespace System.Windows.Tests
{
    [TestClass]
    public class PropertyPathConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
           new PropertyPathConverter();

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
        public void CanConvertTo_Should_Return_False()
        {
            Assert.IsFalse(Converter.CanConvertTo(typeof(string)));
            Assert.IsFalse(Converter.CanConvertTo(typeof(int)));
            Assert.IsFalse(Converter.CanConvertTo(typeof(bool)));
            Assert.IsFalse(Converter.CanConvertTo(typeof(double)));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_PropertyPath_1()
        {
            string path = "Test.Path";

            object value = Converter.ConvertFrom(path);

            Assert.IsInstanceOfType<PropertyPath>(value);
            Assert.AreEqual(value.As<PropertyPath>().Path, path);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_PropertyPath_2()
        {
            object value = Converter.ConvertFrom(null);

            Assert.IsInstanceOfType<PropertyPath>(value);
            Assert.IsNull(value.As<PropertyPath>().Path);
        }

        [TestMethod]
        public void ConvertFrom_Bool_Should_Throw_NotSupportedException()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertFrom(true)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotImplementedException()
        {
            Assert.Throws<NotImplementedException>(
                () => Converter.ConvertTo(new PropertyPath("My.Path"), typeof(string))
            );
        }
    }
}