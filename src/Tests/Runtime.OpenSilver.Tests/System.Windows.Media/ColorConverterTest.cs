
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
    public class ColorConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new ColorConverter();

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
        public void ConvertFrom_Hex8_Should_Return_Color_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("#FFF5F5F5"), Colors.WhiteSmoke);
        }

        [TestMethod]
        public void ConvertFrom_Hex8_Should_Return_Color_2()
        {
            Assert.AreEqual(Converter.ConvertFrom("   #FfF5f5F5   "), Colors.WhiteSmoke);
        }

        [TestMethod]
        public void ConvertFrom_Hex6_Should_Return_Color_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("#F5F5F5"), Colors.WhiteSmoke);
        }

        [TestMethod]
        public void ConvertFrom_Hex6_Should_Return_Color_2()
        {
            Assert.AreEqual(Converter.ConvertFrom("    #f5F5f5 "), Colors.WhiteSmoke);
        }

        [TestMethod]
        public void ConvertFrom_Hex4_Should_Return_Color_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("#9ABC"), Color.FromArgb(9 * 16 + 9, 10 * 16 + 10, 11 * 16 + 11, 12 * 16 + 12));
        }

        [TestMethod]
        public void ConvertFrom_Hex4_Should_Return_Color_2()
        {
            Assert.AreEqual(Converter.ConvertFrom("#9Abc  "), Color.FromArgb(9 * 16 + 9, 10 * 16 + 10, 11 * 16 + 11, 12 * 16 + 12));
        }

        [TestMethod]
        public void ConvertFrom_Hex3_Should_Return_Color_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("#ABC"), Color.FromArgb(255, 10 * 16 + 10, 11 * 16 + 11, 12 * 16 + 12));
        }

        [TestMethod]
        public void ConvertFrom_Hex3_Should_Return_Color_2()
        {
            Assert.AreEqual(Converter.ConvertFrom("    #AbC"), Color.FromArgb(255, 10 * 16 + 10, 11 * 16 + 11, 12 * 16 + 12));
        }

        [TestMethod]
        public void ConvertFrom_Sc4_Should_Return_Color_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("sc#0.5, 0.6, 0.7, 0.8"), Color.FromScRgb(0.5f, 0.6f, 0.7f, 0.8f));
        }

        [TestMethod]
        public void ConvertFrom_Sc4_Should_Return_Color_2()
        {
            Assert.AreEqual(Converter.ConvertFrom(" sc#0.5, 0.6, 0.7, 0.8   "), Color.FromScRgb(0.5f, 0.6f, 0.7f, 0.8f));
        }

        [TestMethod]
        public void ConvertFrom_Sc3_Should_Return_Color_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("sc#0.6, 0.7, 0.8"), Color.FromScRgb(1.0f, 0.6f, 0.7f, 0.8f));
        }

        [TestMethod]
        public void ConvertFrom_Sc3_Should_Return_Color_2()
        {
            Assert.AreEqual(Converter.ConvertFrom(" sc#    0.6   0.7,   .8   "), Color.FromScRgb(1.0f, 0.6f, 0.7f, 0.8f));
        }

        [TestMethod]
        public void ConvertFrom_Sc_Should_Throw_FormatException()
        {
            Assert.Throws<FormatException>(
                () => Converter.ConvertFrom("SC#0.5, 1.0, 0.0, 1.0")
            );
        }

        [TestMethod]
        public void ConvertFrom_NamedColor_Should_Return_Color_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("Yellow"), Colors.Yellow);
        }

        [TestMethod]
        public void ConvertFrom_NamedColor_Should_Return_Color_2()
        {
            Assert.AreEqual(Converter.ConvertFrom("   yELlow"), Colors.Yellow);
        }

        [TestMethod]
        public void ConvertFrom_InvalidColor_Should_Throw_FormatException()
        {
            Assert.Throws<FormatException>(
                () => Converter.ConvertFrom("invalid color")
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
        public void ConvertFrom_Bool_Should_Throw_ArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => Converter.ConvertFrom(true)
            );
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var color = Colors.Brown;

            Assert.AreEqual(Converter.ConvertTo(color, typeof(string)), color.ToString());
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => Converter.ConvertTo(new Color(), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(new Color(), typeof(bool))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_2()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertTo(true, typeof(string))
            );
        }
    }
}
