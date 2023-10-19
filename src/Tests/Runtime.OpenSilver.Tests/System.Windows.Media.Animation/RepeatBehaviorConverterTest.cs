
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

namespace System.Windows.Media.Animation.Tests
{
    [TestClass]
    public class RepeatBehaviorConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new RepeatBehaviorConverter();

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
        public void ConvertFrom_String_Forever_Should_Return_RepeatBehavior_Forever_1()
        {
            Converter.ConvertFrom("Forever")
                .Should()
                .Be(RepeatBehavior.Forever);
        }

        [TestMethod]
        public void ConvertFrom_String_Forever_Should_Return_RepeatBehavior_Forever_2()
        {
            Converter.ConvertFrom("  foreveR  ")
                .Should()
                .Be(RepeatBehavior.Forever);
        }

        [TestMethod]
        public void ConvertFrom_String_Iterations_Should_Return_RepeatBehavior_1()
        {
            Converter.ConvertFrom("100x")
                .Should()
                .Be(new RepeatBehavior(100));
        }

        [TestMethod]
        public void ConvertFrom_String_Iterations_Should_Return_RepeatBehavior_2()
        {
            Converter.ConvertFrom("   100x  ")
                .Should()
                .Be(new RepeatBehavior(100));
        }

        [TestMethod]
        public void ConvertFrom_String_Iterations_Should_Return_RepeatBehavior_3()
        {
            Converter.ConvertFrom("   100X  ")
                .Should()
                .Be(new RepeatBehavior(100));
        }

        [TestMethod]
        public void ConvertFrom_String_Iterations_Should_Return_RepeatBehavior_4()
        {
            Converter.ConvertFrom("   100XxxX  ")
                .Should()
                .Be(new RepeatBehavior(100));
        }

        [TestMethod]
        public void ConvertFrom_String_Duration_Should_Return_RepeatBehavior_1()
        {
            string s = "100";

            Converter.ConvertFrom(s)
                .Should()
                .Be(new RepeatBehavior(TimeSpan.Parse(s)));
        }

        [TestMethod]
        public void ConvertFrom_String_Duration_Should_Return_RepeatBehavior_2()
        {
            string s = "01:02:03.69420";

            Converter.ConvertFrom(s)
                .Should()
                .Be(new RepeatBehavior(TimeSpan.Parse(s)));
        }

        [TestMethod]
        public void ConvertFrom_String_Duration_Should_Return_RepeatBehavior_3()
        {
            string s = "   01:02:03.69420   ";

            Converter.ConvertFrom(s)
                .Should()
                .Be(new RepeatBehavior(TimeSpan.Parse(s)));
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
            Converter.ConvertTo(RepeatBehavior.Forever, typeof(string))
                .Should()
                .Be(RepeatBehavior.Forever.ToString());
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => Converter.ConvertTo(new RepeatBehavior(), null)
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
                () => Converter.ConvertTo(new RepeatBehavior(10), typeof(int))
            );
        }
    }
}
