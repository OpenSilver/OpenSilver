
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
    public class ColorConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new ColorConverter();

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
        public void ConvertFrom_Hex8_Should_Return_Color_1()
        {
            Converter.ConvertFrom("#FFF5F5F5")
                .Should()
                .Be(Colors.WhiteSmoke);
        }

        [TestMethod]
        public void ConvertFrom_Hex8_Should_Return_Color_2()
        {
            Converter.ConvertFrom("   #FfF5f5F5   ")
                .Should()
                .Be(Colors.WhiteSmoke);
        }

        [TestMethod]
        public void ConvertFrom_Hex6_Should_Return_Color_1()
        {
            Converter.ConvertFrom("#F5F5F5")
                .Should()
                .Be(Colors.WhiteSmoke);
        }

        [TestMethod]
        public void ConvertFrom_Hex6_Should_Return_Color_2()
        {
            Converter.ConvertFrom("    #f5F5f5 ")
                .Should()
                .Be(Colors.WhiteSmoke);
        }

        [TestMethod]
        public void ConvertFrom_Hex4_Should_Return_Color_1()
        {
            Converter.ConvertFrom("#9ABC")
                .Should()
                .Be(Color.FromArgb((byte)(9 * 16 + 9), (byte)(10 * 16 + 10), (byte)(11 * 16 + 11), (byte)(12 * 16 + 12)));
        }

        [TestMethod]
        public void ConvertFrom_Hex4_Should_Return_Color_2()
        {
            Converter.ConvertFrom("#9Abc  ")
                .Should()
                .Be(Color.FromArgb((byte)(9 * 16 + 9), (byte)(10 * 16 + 10), (byte)(11 * 16 + 11), (byte)(12 * 16 + 12)));
        }

        [TestMethod]
        public void ConvertFrom_Hex3_Should_Return_Color_1()
        {
            Converter.ConvertFrom("#ABC")
                .Should()
                .Be(Color.FromArgb((byte)255, (byte)(10 * 16 + 10), (byte)(11 * 16 + 11), (byte)(12 * 16 + 12)));
        }

        [TestMethod]
        public void ConvertFrom_Hex3_Should_Return_Color_2()
        {
            Converter.ConvertFrom("    #AbC")
                .Should()
                .Be(Color.FromArgb((byte)255, (byte)(10 * 16 + 10), (byte)(11 * 16 + 11), (byte)(12 * 16 + 12)));
        }

        [TestMethod]
        public void ConvertFrom_Sc4_Should_Return_Color_1()
        {
            Converter.ConvertFrom("sc#0.5, 0.6, 0.7, 0.8")
                .Should()
                .Be(Color.FromScRgb(0.5f, 0.6f, 0.7f, 0.8f));
        }

        [TestMethod]
        public void ConvertFrom_Sc4_Should_Return_Color_2()
        {
            Converter.ConvertFrom(" sc#0.5, 0.6, 0.7, 0.8   ")
                .Should()
                .Be(Color.FromScRgb(0.5f, 0.6f, 0.7f, 0.8f));
        }

        [TestMethod]
        public void ConvertFrom_Sc3_Should_Return_Color_1()
        {
            Converter.ConvertFrom("sc#0.6, 0.7, 0.8")
                .Should()
                .Be(Color.FromScRgb(1.0f, 0.6f, 0.7f, 0.8f));
        }

        [TestMethod]
        public void ConvertFrom_Sc3_Should_Return_Color_2()
        {
            Converter.ConvertFrom(" sc#    0.6   0.7,   .8   ")
                .Should()
                .Be(Color.FromScRgb(1.0f, 0.6f, 0.7f, 0.8f));
        }

        [TestMethod]
        public void ConvertFrom_Sc_Should_Throw_FormatException()
        {
            Assert.ThrowsException<FormatException>(
                () => Converter.ConvertFrom("SC#0.5, 1.0, 0.0, 1.0")
            );
        }

        [TestMethod]
        public void ConvertFrom_NamedColor_Should_Return_Color_1()
        {
            Converter.ConvertFrom("Yellow")
                .Should()
                .Be(Colors.Yellow);
        }

        [TestMethod]
        public void ConvertFrom_NamedColor_Should_Return_Color_2()
        {
            Converter.ConvertFrom("   yELlow")
                .Should()
                .Be(Colors.Yellow);
        }

        [TestMethod]
        public void ConvertFrom_InvalidColor_Should_Throw_FormatException()
        {
            Assert.ThrowsException<FormatException>(
                () => Converter.ConvertFrom("invalid color")
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
        public void ConvertFrom_Bool_Should_Throw_ArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(
                () => Converter.ConvertFrom(true)
            );
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var color = Colors.Brown;
            
            Converter.ConvertTo(color, typeof(string))
                .Should()
                .Be(color.ToString());
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => Converter.ConvertTo(new Color(), null)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_1()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(new Color(), typeof(bool))
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotSupportedException_2()
        {
            Assert.ThrowsException<NotSupportedException>(
                () => Converter.ConvertTo(true, typeof(string))
            );
        }
    }
}
