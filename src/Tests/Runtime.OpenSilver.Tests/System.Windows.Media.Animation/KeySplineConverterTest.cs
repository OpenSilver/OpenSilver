
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
            Assert.IsTrue(Converter.CanConvertFrom(typeof(string)));
        }

        [TestMethod]
        public void CanConvertFrom_Should_Return_False()
        {
            Assert.IsFalse(Converter.CanConvertFrom(typeof(bool)));
            Assert.IsFalse(Converter.CanConvertFrom(typeof(int)));
            Assert.IsFalse(Converter.CanConvertFrom(typeof(object)));
        }

        [TestMethod]
        public void CanConvertTo_String_Should_Return_True()
        {
            Assert.IsTrue(Converter.CanConvertTo(typeof(string)));
        }

        [TestMethod]
        public void CanConvertTo_Should_Return_False()
        {
            Assert.IsFalse(Converter.CanConvertTo(typeof(char)));
            Assert.IsFalse(Converter.CanConvertTo(typeof(long)));
            Assert.IsFalse(Converter.CanConvertTo(typeof(object)));
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_KeySpline_1()
        {
            object value = Converter.ConvertFrom("0 0.5 0.5 1");

            Assert.IsInstanceOfType<KeySpline>(value);

            KeySpline keySpline = value as KeySpline;

            Assert.AreEqual(0, keySpline.ControlPoint1.X);
            Assert.AreEqual(0.5, keySpline.ControlPoint1.Y);
            Assert.AreEqual(0.5, keySpline.ControlPoint2.X);
            Assert.AreEqual(1, keySpline.ControlPoint2.Y);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_KeySpline_2()
        {
            object value = Converter.ConvertFrom("0.1,0.5,0.7,0.9");

            Assert.IsInstanceOfType<KeySpline>(value);

            KeySpline keySpline = value as KeySpline;

            Assert.AreEqual(0.1, keySpline.ControlPoint1.X);
            Assert.AreEqual(0.5, keySpline.ControlPoint1.Y);
            Assert.AreEqual(0.7, keySpline.ControlPoint2.X);
            Assert.AreEqual(0.9, keySpline.ControlPoint2.Y);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_KeySpline_3()
        {
            object value = Converter.ConvertFrom("  0.2,   0   0.7  ,  0.8");

            Assert.IsInstanceOfType<KeySpline>(value);

            KeySpline keySpline = value as KeySpline;

            Assert.AreEqual(0.2, keySpline.ControlPoint1.X);
            Assert.AreEqual(0, keySpline.ControlPoint1.Y);
            Assert.AreEqual(0.7, keySpline.ControlPoint2.X);
            Assert.AreEqual(0.8, keySpline.ControlPoint2.Y);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Return_KeySpline_4()
        {
            object value = Converter.ConvertFrom("");

            Assert.IsInstanceOfType<KeySpline>(value);

            KeySpline keySpline = value as KeySpline;

            Assert.AreEqual(0, keySpline.ControlPoint1.X);
            Assert.AreEqual(0, keySpline.ControlPoint1.Y);
            Assert.AreEqual(1, keySpline.ControlPoint2.X);
            Assert.AreEqual(1, keySpline.ControlPoint2.Y);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Throw_InvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => Converter.ConvertFrom(" "));
            Assert.Throws<InvalidOperationException>(() => Converter.ConvertFrom("0.2"));
            Assert.Throws<InvalidOperationException>(() => Converter.ConvertFrom("0.2 0.2"));
            Assert.Throws<InvalidOperationException>(() => Converter.ConvertFrom("0.2 0.2 0.3"));
        }

        [TestMethod]
        public void ConvertFrom_Should_Throw_NotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => Converter.ConvertFrom(true));
            Assert.Throws<NotSupportedException>(() => Converter.ConvertFrom(123456789));
            Assert.Throws<NotSupportedException>(() => Converter.ConvertFrom(null));
            Assert.Throws<NotSupportedException>(() => Converter.ConvertFrom(new string[0]));
        }

        [TestMethod]
        public void ConvertTo_Should_Return_String()
        {
            string separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;

            Assert.AreEqual(Converter.ConvertTo(new KeySpline(), typeof(string)), $"0{separator}0{separator}1{separator}1");
            Assert.AreEqual(Converter.ConvertTo(new KeySpline(0.2, 0.3, 0.4, 0.5), typeof(string)), $"{0.2}{separator}{0.3}{separator}{0.4}{separator}{0.5}");
        }
    }
}
