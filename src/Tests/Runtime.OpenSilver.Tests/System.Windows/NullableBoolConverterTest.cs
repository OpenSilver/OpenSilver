
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
    public class NullableBoolConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new NullableBoolConverter();

        [TestMethod]
        public void CanConvertFrom_Should_Return_True()
        {
            Converter.CanConvertFrom(typeof(string)).Should().BeTrue();
            Converter.CanConvertFrom(typeof(bool)).Should().BeTrue();
            Converter.CanConvertFrom(typeof(bool?)).Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Int32_Should_Return_False()
        {
            Converter.CanConvertFrom(typeof(int))
                .Should()
                .BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_Should_Return_False()
        {
            Converter.CanConvertTo(typeof(string)).Should().BeFalse();
            Converter.CanConvertTo(typeof(int)).Should().BeFalse();
            Converter.CanConvertTo(typeof(bool)).Should().BeFalse();
            Converter.CanConvertTo(typeof(bool?)).Should().BeFalse();
            Converter.CanConvertTo(typeof(long)).Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_Null()
        {
            Converter.ConvertFrom(null)
                .Should()
                .BeNull();
        }

        [TestMethod]
        public void ConvertFrom_String_Empty()
        {
            Converter.ConvertFrom(string.Empty)
                .Should()
                .BeNull();
        }

        [TestMethod]
        public void ConvertFrom_String_True_1()
        {
            Converter.ConvertFrom("true")
                .As<bool?>()
                .Should()
                .HaveValue()
                .And
                .BeTrue();
        }

        [TestMethod]
        public void ConvertFrom_String_True_2()
        {
            Converter.ConvertFrom("  true   ")
                .As<bool?>()
                .Should()
                .HaveValue()
                .And
                .BeTrue();
        }

        [TestMethod]
        public void ConvertFrom_String_True_3()
        {
            Converter.ConvertFrom("   trUe    ")
                .As<bool?>()
                .Should()
                .HaveValue()
                .And
                .BeTrue();
        }

        [TestMethod]
        public void ConvertFrom_String_False_1()
        {
            Converter.ConvertFrom("false")
              .As<bool?>()
              .Should()
              .HaveValue()
              .And
              .BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_False_2()
        {
            Converter.ConvertFrom(" false   ")
              .As<bool?>()
              .Should()
              .HaveValue()
              .And
              .BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_False_3()
        {
            Converter.ConvertFrom("   FAlse    ")
              .As<bool?>()
              .Should()
              .HaveValue()
              .And
              .BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Throw_FormatException_1()
        {
            Assert.ThrowsException<FormatException>(
                () => Converter.ConvertFrom("   ")
            );
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Throw_FormatException_2()
        {
            Assert.ThrowsException<FormatException>(
                () => Converter.ConvertFrom("not_a_bool")
            );
        }

        [TestMethod]
        public void ConvertFrom_Bool_True()
        {
            Converter.ConvertFrom(true)
                .As<bool?>()
                .Should()
                .HaveValue()
                .And
                .BeTrue();
        }

        [TestMethod]
        public void ConvertFrom_Bool_False()
        {
            Converter.ConvertFrom(false)
                .As<bool?>()
                .Should()
                .HaveValue()
                .And
                .BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_Nullable_Of_Bool_True()
        {
            Converter.ConvertFrom((bool?)true)
                .As<bool?>()
                .Should()
                .HaveValue()
                .And
                .BeTrue();
        }

        [TestMethod]
        public void ConvertFrom_Nullable_Of_Bool_False()
        {
            Converter.ConvertFrom((bool?)false)
                .As<bool?>()
                .Should()
                .HaveValue()
                .And
                .BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_Should_Throw_NotSupportedException()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertFrom(420)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotImplementedException()
        {
            Assert.ThrowsException<NotImplementedException>(
                () => Converter.ConvertTo((bool?)true, typeof(string))
            );
        }
    }
}
