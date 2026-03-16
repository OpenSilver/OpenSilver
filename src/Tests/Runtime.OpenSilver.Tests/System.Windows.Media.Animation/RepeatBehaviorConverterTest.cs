
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
        public void ConvertFrom_String_Forever_Should_Return_RepeatBehavior_Forever_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("Forever"), RepeatBehavior.Forever);
        }

        [TestMethod]
        public void ConvertFrom_String_Forever_Should_Return_RepeatBehavior_Forever_2()
        {
            Assert.AreEqual(Converter.ConvertFrom("  foreveR  "), RepeatBehavior.Forever);
        }

        [TestMethod]
        public void ConvertFrom_String_Iterations_Should_Return_RepeatBehavior_1()
        {
            Assert.AreEqual(Converter.ConvertFrom("100x"), new RepeatBehavior(100));
        }

        [TestMethod]
        public void ConvertFrom_String_Iterations_Should_Return_RepeatBehavior_2()
        {
            Assert.AreEqual(Converter.ConvertFrom("   100x  "), new RepeatBehavior(100));
        }

        [TestMethod]
        public void ConvertFrom_String_Iterations_Should_Return_RepeatBehavior_3()
        {
            Assert.AreEqual(Converter.ConvertFrom("   100X  "), new RepeatBehavior(100));
        }

        [TestMethod]
        public void ConvertFrom_String_Iterations_Should_Return_RepeatBehavior_4()
        {
            Assert.AreEqual(Converter.ConvertFrom("   100XxxX  "), new RepeatBehavior(100));
        }

        [TestMethod]
        public void ConvertFrom_String_Duration_Should_Return_RepeatBehavior_1()
        {
            string s = "100";

            Assert.AreEqual(Converter.ConvertFrom(s), new RepeatBehavior(TimeSpan.Parse(s)));
        }

        [TestMethod]
        public void ConvertFrom_String_Duration_Should_Return_RepeatBehavior_2()
        {
            string s = "01:02:03.69420";

            Assert.AreEqual(Converter.ConvertFrom(s), new RepeatBehavior(TimeSpan.Parse(s)));
        }

        [TestMethod]
        public void ConvertFrom_String_Duration_Should_Return_RepeatBehavior_3()
        {
            string s = "   01:02:03.69420   ";

            Assert.AreEqual(Converter.ConvertFrom(s), new RepeatBehavior(TimeSpan.Parse(s)));
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
            Assert.AreEqual(Converter.ConvertTo(RepeatBehavior.Forever, typeof(string)), RepeatBehavior.Forever.ToString());
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => Converter.ConvertTo(new RepeatBehavior(), null)
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
                () => Converter.ConvertTo(new RepeatBehavior(10), typeof(int))
            );
        }
    }
}
