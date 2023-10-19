
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
    public class PointCollectionConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new PointCollectionConverter();

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
        public void ConvertFrom_String_Should_Return_PointCollection_1()
        {
            Converter.ConvertFrom("1, 1, 2, 2")
                .Should()
                .BeOfType<PointCollection>()
                .Which
                .Should()
                .HaveCount(2)
                .And
                .HaveElementAt(0, new Point(1, 1))
                .And
                .HaveElementAt(1, new Point(2, 2));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_PointCollection_2()
        {
            Converter.ConvertFrom(" 1 1  2 2")
                .Should()
                .BeOfType<PointCollection>()
                .Which
                .Should()
                .HaveCount(2)
                .And
                .HaveElementAt(0, new Point(1, 1))
                .And
                .HaveElementAt(1, new Point(2, 2));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_PointCollection_3()
        {
            Converter.ConvertFrom(" 1,1 , 2 2  ")
                .Should()
                .BeOfType<PointCollection>()
                .Which
                .Should()
                .HaveCount(2)
                .And
                .HaveElementAt(0, new Point(1, 1))
                .And
                .HaveElementAt(1, new Point(2, 2));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Throw_FormatException()
        {
            Assert.ThrowsException<FormatException>(
                () => Converter.ConvertFrom("1,1,1")
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
            var collection = new PointCollection() 
            { 
                new Point(1.0, 1.0), 
                new Point(2.0, -2.0) 
            };
            
            Converter.ConvertTo(collection, typeof(string))
                .Should()
                .Be(collection.ToString());
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => Converter.ConvertTo(new PointCollection(), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(true, typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_2()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(new PointCollection(), typeof(byte))
            );
        }
    }
}
