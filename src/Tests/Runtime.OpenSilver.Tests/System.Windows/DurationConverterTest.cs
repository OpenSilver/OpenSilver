
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
    public class DurationConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new DurationConverter();

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
        public void CanConvertTo_Should_Return_False_1()
        {
            Converter.CanConvertTo(typeof(string))
                .Should()
                .BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_Should_Return_False_2()
        {
            Converter.CanConvertTo(typeof(bool))
                .Should()
                .BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Duration()
        {
            Converter.ConvertFrom("1")
                .Should()
                .Be(new Duration(TimeSpan.FromDays(1)));
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
        public void ConvertTo_Should_Throw_NotImplementedException()
        {
            Assert.ThrowsException<NotImplementedException>(
                () => Converter.ConvertTo(new Duration(TimeSpan.FromDays(3)), typeof(string))
            );
        }
    }
}
