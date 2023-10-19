
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

namespace System.Windows.Media.Tests
{
    [TestClass]
    public class MatrixConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new MatrixConverter();

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
        public void CanConvertTo_String_Should_Return_True()
        {
            Converter.CanConvertTo(typeof(string))
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_Should_Return_False()
        {
            Converter.CanConvertTo(typeof(bool))
                .Should()
                .BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Matrix_Identity()
        {
            Converter.ConvertFrom("Identity")
                .Should()
                .Be(Matrix.Identity);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Matrix_1()
        {
            Converter.ConvertFrom("1,2,3,4,5,6")
                .Should()
                .Be(new Matrix(1, 2, 3, 4, 5, 6));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Matrix_2()
        {
            Converter.ConvertFrom("1 2 3 4 5 6")
                .Should()
                .Be(new Matrix(1, 2, 3, 4, 5, 6));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_Matrix_3()
        {
            Converter.ConvertFrom("  1,2 3, 4 5  6 ")
                .Should()
                .Be(new Matrix(1, 2, 3, 4, 5, 6));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Throw_FormatException()
        {
            Assert.ThrowsException<FormatException>(
                () => Converter.ConvertFrom("1")
            );
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
        public void ConvertTo_String()
        {
            Converter.ConvertTo(new Matrix(1, 2, 3, 4, 5, 6), typeof(string))
                .Should()
                .Be("1,2,3,4,5,6");
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => Converter.ConvertTo(new Matrix(), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo("Hi", typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_2()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(new Matrix(), typeof(bool))
            );
        }
    }
}
