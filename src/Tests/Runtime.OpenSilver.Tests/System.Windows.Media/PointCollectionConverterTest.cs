
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
    public class PointCollectionConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new PointCollectionConverter();

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
        public void ConvertFrom_String_Should_Return_PointCollection_1()
        {
            object value = Converter.ConvertFrom("1, 1, 2, 2");

            Assert.IsInstanceOfType<PointCollection>(value);

            PointCollection pointCollection = value as PointCollection;

            Assert.AreEqual(2, pointCollection.Count);
            Assert.AreEqual(pointCollection[0], new Point(1, 1));
            Assert.AreEqual(pointCollection[1], new Point(2, 2));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_PointCollection_2()
        {
            object value = Converter.ConvertFrom(" 1 1  2 2");

            Assert.IsInstanceOfType<PointCollection>(value);

            PointCollection pointCollection = value as PointCollection;

            Assert.AreEqual(2, pointCollection.Count);
            Assert.AreEqual(pointCollection[0], new Point(1, 1));
            Assert.AreEqual(pointCollection[1], new Point(2, 2));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_PointCollection_3()
        {
            object value = Converter.ConvertFrom(" 1,1 , 2 2  ");

            Assert.IsInstanceOfType<PointCollection>(value);

            PointCollection pointCollection = value as PointCollection;

            Assert.AreEqual(2, pointCollection.Count);
            Assert.AreEqual(pointCollection[0], new Point(1, 1));
            Assert.AreEqual(pointCollection[1], new Point(2, 2));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Throw_InvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(
                () => Converter.ConvertFrom("1,1,1")
            );
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
            var collection = new PointCollection() 
            { 
                new Point(1.0, 1.0), 
                new Point(2.0, -2.0) 
            };
            
            Assert.AreEqual(Converter.ConvertTo(collection, typeof(string)), collection.ToString());
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => Converter.ConvertTo(new PointCollection(), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(true, typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_2()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(new PointCollection(), typeof(byte))
            );
        }
    }
}
