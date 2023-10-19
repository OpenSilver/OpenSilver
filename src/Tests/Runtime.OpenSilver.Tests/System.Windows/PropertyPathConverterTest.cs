
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
    public class PropertyPathConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
           new PropertyPathConverter();

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
        public void CanConvertTo_Should_Return_False()
        {
            Converter.CanConvertTo(typeof(string)).Should().BeFalse();
            Converter.CanConvertTo(typeof(int)).Should().BeFalse();
            Converter.CanConvertTo(typeof(bool)).Should().BeFalse();
            Converter.CanConvertTo(typeof(double)).Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_PropertyPath_1()
        {
            string path = "Test.Path";
            Converter.ConvertFrom(path)
                .Should()
                .BeOfType<PropertyPath>()
                .Which
                .Path
                .Should()
                .Be(path);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_PropertyPath_2()
        {
            Converter.ConvertFrom(null)
                .Should()
                .BeOfType<PropertyPath>()
                .Which
                .Path
                .Should()
                .BeNull();
        }

        [TestMethod]
        public void ConvertFrom_Bool_Should_Throw_NotSupportedException()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertFrom(true)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotImplementedException()
        {
            Assert.ThrowsException<NotImplementedException>(
                () => Converter.ConvertTo(new PropertyPath("My.Path"), typeof(string))
            );
        }
    }
}