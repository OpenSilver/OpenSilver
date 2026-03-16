
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
    public class GeometryConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new GeometryConverter();

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
        public void ConvertFrom_String_Should_Return_PathGeometry()
        {
            var test = Converter.ConvertFrom("M 10,10 20,0 20,10 L 50,30 50,40 20,40");

            var pg = (PathGeometry)test;

            Assert.AreEqual(1, pg.Figures.Count);
            Assert.AreEqual(1, pg.Figures[0].Segments.Count);
            Assert.IsInstanceOfType<PolyLineSegment>(pg.Figures[0].Segments[0]);

            var segments = (PolyLineSegment)pg.Figures[0].Segments[0];
            
            Assert.AreEqual(5, segments.Points.Count);
            Assert.AreEqual(segments.Points[0], new Point(20, 0));
            Assert.AreEqual(segments.Points[1], new Point(20, 10));
            Assert.AreEqual(segments.Points[2], new Point(50, 30));
            Assert.AreEqual(segments.Points[3], new Point(50, 40));
            Assert.AreEqual(segments.Points[4], new Point(20, 40));
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
            var geo = new PathGeometry();
            
            Assert.AreEqual(Converter.ConvertTo(geo, typeof(string)), geo.ToString());
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => Converter.ConvertTo(new PathGeometry(), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(420, typeof(string))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_2()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(new PathGeometry(), typeof(bool))
            );
        }
    }
}
