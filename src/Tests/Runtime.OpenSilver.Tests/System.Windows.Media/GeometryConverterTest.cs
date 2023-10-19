
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
    public class GeometryConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new GeometryConverter();

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
        public void ConvertFrom_String_Should_Return_PathGeometry()
        {
            var test = Converter.ConvertFrom("M 10,10 20,0 20,10 L 50,30 50,40 20,40");

            var pg = (PathGeometry)test;

            pg.Figures.Count.Should().Be(1);
            pg.Figures[0].Segments.Count.Should().Be(1);
            pg.Figures[0].Segments[0].Should().BeOfType<PolyLineSegment>();

            var segments = (PolyLineSegment)pg.Figures[0].Segments[0];
            
            segments.Points.Count.Should().Be(5);
            segments.Points[0].Should().Be(new Point(20, 0));
            segments.Points[1].Should().Be(new Point(20, 10));
            segments.Points[2].Should().Be(new Point(50, 30));
            segments.Points[3].Should().Be(new Point(50, 40));
            segments.Points[4].Should().Be(new Point(20, 40));
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
            var geo = new PathGeometry();
            
            Converter.ConvertTo(geo, typeof(string))
                .Should()
                .Be(geo.ToString());
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => Converter.ConvertTo(new PathGeometry(), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(420, typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_2()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(new PathGeometry(), typeof(bool))
            );
        }
    }
}
