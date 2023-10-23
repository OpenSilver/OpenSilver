
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
using System.Globalization;

namespace System.Windows.Media.Animation.Tests
{
    [TestClass]
    public class KeySplineConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new KeySplineConverter();

        [TestMethod]
        public void CanConvertFrom_String_Should_Return_True()
        {
            Converter.CanConvertFrom(typeof(string)).Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Should_Return_False()
        {
            Converter.CanConvertFrom(typeof(bool)).Should().BeFalse();
            Converter.CanConvertFrom(typeof(int)).Should().BeFalse();
            Converter.CanConvertFrom(typeof(object)).Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_Should_Return_True()
        {
            Converter.CanConvertTo(typeof(string)).Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Should_Return_False()
        {
            Converter.CanConvertTo(typeof(char)).Should().BeFalse();
            Converter.CanConvertTo(typeof(long)).Should().BeFalse();
            Converter.CanConvertTo(typeof(object)).Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_KeySpline_1()
        {
            KeySpline keySpline = Converter.ConvertFrom("0 0.5 0.5 1")
                .Should()
                .BeOfType<KeySpline>()
                .Subject;

            keySpline.ControlPoint1.X.Should().Be(0);
            keySpline.ControlPoint1.Y.Should().Be(0.5);
            keySpline.ControlPoint2.X.Should().Be(0.5);
            keySpline.ControlPoint2.Y.Should().Be(1);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_KeySpline_2()
        {
            KeySpline keySpline = Converter.ConvertFrom("0.1,0.5,0.7,0.9")
                .Should()
                .BeOfType<KeySpline>()
                .Subject;

            keySpline.ControlPoint1.X.Should().Be(0.1);
            keySpline.ControlPoint1.Y.Should().Be(0.5);
            keySpline.ControlPoint2.X.Should().Be(0.7);
            keySpline.ControlPoint2.Y.Should().Be(0.9);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_KeySpline_3()
        {
            KeySpline keySpline = Converter.ConvertFrom("  0.2,   0   0.7  ,  0.8")
                .Should()
                .BeOfType<KeySpline>()
                .Subject;

            keySpline.ControlPoint1.X.Should().Be(0.2);
            keySpline.ControlPoint1.Y.Should().Be(0);
            keySpline.ControlPoint2.X.Should().Be(0.7);
            keySpline.ControlPoint2.Y.Should().Be(0.8);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_KeySpline_4()
        {
            KeySpline keySpline = Converter.ConvertFrom("")
                .Should()
                .BeOfType<KeySpline>()
                .Subject;

            keySpline.ControlPoint1.X.Should().Be(0);
            keySpline.ControlPoint1.Y.Should().Be(0);
            keySpline.ControlPoint2.X.Should().Be(1);
            keySpline.ControlPoint2.Y.Should().Be(1);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Throw_NotSupportedException()
        {
            Assert.ThrowsException<NotSupportedException>(() => Converter.ConvertFrom(" "));
            Assert.ThrowsException<NotSupportedException>(() => Converter.ConvertFrom("0.2"));
            Assert.ThrowsException<NotSupportedException>(() => Converter.ConvertFrom("0.2 0.2"));
            Assert.ThrowsException<NotSupportedException>(() => Converter.ConvertFrom("0.2 0.2 0.3"));
        }

        [TestMethod]
        public void ConvertFrom_Should_Throw_NotSupportedException()
        {
            Assert.ThrowsException<NotSupportedException>(() => Converter.ConvertFrom(true));
            Assert.ThrowsException<NotSupportedException>(() => Converter.ConvertFrom(123456789));
            Assert.ThrowsException<NotSupportedException>(() => Converter.ConvertFrom(null));
            Assert.ThrowsException<NotSupportedException>(() => Converter.ConvertFrom(new string[0]));
        }

        [TestMethod]
        public void ConvertTo_Should_Return_String()
        {
            string separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;

            Converter.ConvertTo(new KeySpline(), typeof(string))
                .Should()
                .Be($"0{separator}0{separator}1{separator}1");

            Converter.ConvertTo(new KeySpline(0.2, 0.3, 0.4, 0.5), typeof(string))
                .Should()
                .Be($"{0.2}{separator}{0.3}{separator}{0.4}{separator}{0.5}");
        }
    }
}
