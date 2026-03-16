
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
using OpenSilver.Internal.Helpers;

namespace System.Windows.Tests
{
    [TestClass]
    public class NullableBoolConverterTest : TypeConverterTestBase
    {
        protected override TypeConverter Converter { get; } =
            new NullableBoolConverter();

        [TestMethod]
        public void CanConvertFrom_Should_Return_True()
        {
            Assert.IsTrue(Converter.CanConvertFrom(typeof(string)));
            Assert.IsTrue(Converter.CanConvertFrom(typeof(bool)));
            Assert.IsTrue(Converter.CanConvertFrom(typeof(bool?)));
        }

        [TestMethod]
        public void CanConvertFrom_Int32_Should_Return_False()
        {
            Assert.IsFalse(Converter.CanConvertFrom(typeof(int)));
        }

        [TestMethod]
        public void CanConvertTo_Should_Return_False()
        {
            Assert.IsFalse(Converter.CanConvertTo(typeof(string)));
            Assert.IsFalse(Converter.CanConvertTo(typeof(int)));
            Assert.IsFalse(Converter.CanConvertTo(typeof(bool)));
            Assert.IsFalse(Converter.CanConvertTo(typeof(bool?)));
            Assert.IsFalse(Converter.CanConvertTo(typeof(long)));
        }

        [TestMethod]
        public void ConvertFrom_Null()
        {
            Assert.IsNull(Converter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_String_Empty()
        {
            Assert.IsNull(Converter.ConvertFrom(string.Empty));
        }

        [TestMethod]
        public void ConvertFrom_String_True_1()
        {
            bool? value = Converter.ConvertFrom("true").As<bool?>();

            Assert.IsTrue(value.HasValue);
            Assert.IsTrue(value.Value);
        }

        [TestMethod]
        public void ConvertFrom_String_True_2()
        {
            bool? value = Converter.ConvertFrom("  true   ").As<bool?>();

            Assert.IsTrue(value.HasValue);
            Assert.IsTrue(value.Value);
        }

        [TestMethod]
        public void ConvertFrom_String_True_3()
        {
            bool? value = Converter.ConvertFrom("   trUe    ").As<bool?>();

            Assert.IsTrue(value.HasValue);
            Assert.IsTrue(value.Value);
        }

        [TestMethod]
        public void ConvertFrom_String_False_1()
        {
            bool? value = Converter.ConvertFrom("false").As<bool?>();

            Assert.IsTrue(value.HasValue);
            Assert.IsFalse(value.Value);
        }

        [TestMethod]
        public void ConvertFrom_String_False_2()
        {
            bool? value = Converter.ConvertFrom(" false   ").As<bool?>();

            Assert.IsTrue(value.HasValue);
            Assert.IsFalse(value.Value);
        }

        [TestMethod]
        public void ConvertFrom_String_False_3()
        {
            bool? value = Converter.ConvertFrom("   FAlse    ").As<bool?>();

            Assert.IsTrue(value.HasValue);
            Assert.IsFalse(value.Value);
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Throw_FormatException_1()
        {
            Assert.Throws<FormatException>(
                () => Converter.ConvertFrom("   ")
            );
        }

        [TestMethod]
        public void ConvertFrom_String_Should_Throw_FormatException_2()
        {
            Assert.Throws<FormatException>(
                () => Converter.ConvertFrom("not_a_bool")
            );
        }

        [TestMethod]
        public void ConvertFrom_Bool_True()
        {
            bool? value = Converter.ConvertFrom(true).As<bool?>();

            Assert.IsTrue(value.HasValue);
            Assert.IsTrue(value.Value);
        }

        [TestMethod]
        public void ConvertFrom_Bool_False()
        {
            bool? value = Converter.ConvertFrom(false).As<bool?>();

            Assert.IsTrue(value.HasValue);
            Assert.IsFalse(value.Value);
        }

        [TestMethod]
        public void ConvertFrom_Nullable_Of_Bool_True()
        {
            bool? value = Converter.ConvertFrom((bool?)true).As<bool?>();

            Assert.IsTrue(value.HasValue);
            Assert.IsTrue(value.Value);
        }

        [TestMethod]
        public void ConvertFrom_Nullable_Of_Bool_False()
        {
            bool? value = Converter.ConvertFrom((bool?)false).As<bool?>();

            Assert.IsTrue(value.HasValue);
            Assert.IsFalse(value.Value);
        }

        [TestMethod]
        public void ConvertFrom_Should_Throw_NotSupportedException()
        {
            Assert.Throws<NotSupportedException>(
                () => Converter.ConvertFrom(420)
            );
        }

        [TestMethod]
        public void ConvertTo_Should_Throw_NotImplementedException()
        {
            Assert.Throws<NotImplementedException>(
                () => Converter.ConvertTo((bool?)true, typeof(string))
            );
        }
    }
}
