
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
    public class TransformConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new TransformConverter();

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
        public void ConvertFrom_String_Should_Return_MatrixTransform_1()
        {
            object value = Converter.ConvertFrom("1,2,3,4,5,6");

            Assert.IsInstanceOfType<MatrixTransform>(value);

            MatrixTransform transform = value as MatrixTransform;

            Assert.AreEqual(transform.Matrix, new Matrix(1, 2, 3, 4, 5, 6));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_MatrixTransform_2()
        {
            object value = Converter.ConvertFrom("1 2 3 4 5 6");

            Assert.IsInstanceOfType<MatrixTransform>(value);

            MatrixTransform transform = value as MatrixTransform;

            Assert.AreEqual(transform.Matrix, new Matrix(1, 2, 3, 4, 5, 6));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_MatrixTransform_3()
        {
            object value = Converter.ConvertFrom("  1 ,2,3, 4  5, 6  ");

            Assert.IsInstanceOfType<MatrixTransform>(value);

            MatrixTransform transform = value as MatrixTransform;

            Assert.AreEqual(transform.Matrix, new Matrix(1, 2, 3, 4, 5, 6));
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
        public void ConvertTo_String()
        {
            var transform = new MatrixTransform();

            Assert.AreEqual(Converter.ConvertTo(transform, typeof(string)), transform.ToString());
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => Converter.ConvertTo(new MatrixTransform(), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(new Matrix(), typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_2()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(new MatrixTransform(), typeof(Geometry))
            );
        }
    }
}
