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

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Tests
{
    [TestClass]
    public class RectTest
    {
        [TestMethod]
        public void Intersect_PartialOverlap()
        {
            var rect1 = new Rect(0, 0, 15, 10);
            var rect2 = new Rect(5, -5, 5, 20);

            rect1.Intersect(rect2);

            rect1.Should().Be(new Rect(5, 0, 5, 10));
        }

        [TestMethod]
        public void Intersect_TotalOverlap()
        {
            var rect1 = new Rect(0, 0, 10, 10);
            var rect2 = new Rect(0, 0, 20, 20);

            rect1.Intersect(rect2);
            rect1.Should().Be(new Rect(0, 0, 10, 10));

            // Test the other way around.
            rect1 = new Rect(0, 0, 10, 10);

            rect2.Intersect(rect1);
            rect2.Should().Be(new Rect(0, 0, 10, 10));
        }

        [TestMethod]
        public void Intersect_EmptyIntersection_ShouldReturnEmptyRect()
        {
            var rect1 = new Rect(10, 10, 10, 10);
            var rect2 = new Rect(0, 0, 5, 5);

            rect1.Intersect(rect2);

            rect1.Should().Be(Rect.Empty);
        }

        [TestMethod]
        public void Intersect_ZeroWidth_ShouldReturnEmptyRect()
        {
            var rect1 = new Rect(10, 10, 0, 10);
            var rect2 = new Rect(0, 0, 5, 5);

            rect1.Intersect(rect2);

            rect1.Should().Be(Rect.Empty);
        }
    }
}
