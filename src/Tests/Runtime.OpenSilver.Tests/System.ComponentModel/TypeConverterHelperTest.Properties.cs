
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

namespace System.ComponentModel.Tests
{
    public partial class TypeConverterHelperTest
    {
        [TestMethod]
        public void GetProperties_Inheritance_1()
        {
            var properties = TypeConverterHelper.GetProperties(typeof(_MyType1));
            properties.Should().HaveCount(2);
            properties[nameof(_MyType1.IntProperty)].Should().NotBeNull();
            properties[nameof(_MyType1.DoubleProperty)].Should().NotBeNull();
        }

        [TestMethod]
        public void GetProperties_Inheritance_2()
        {
            var properties = TypeConverterHelper.GetProperties(typeof(_MyType2));
            properties.Should().HaveCount(3);
            properties[nameof(_MyType1.IntProperty)].Should().NotBeNull();
            properties[nameof(_MyType1.DoubleProperty)].Should().NotBeNull();
            properties[nameof(_MyType2.StringProperty)].Should().NotBeNull();
        }

        [TestMethod]
        public void GetProperties_Inheritance_3()
        {
            var properties = TypeConverterHelper.GetProperties(typeof(_MyType3));
            properties.Should().HaveCount(4);
            properties[nameof(_MyType1.IntProperty)].Should().NotBeNull();
            properties[nameof(_MyType1.DoubleProperty)].Should().NotBeNull();
            properties[nameof(_MyType2.StringProperty)].Should().NotBeNull();
            properties[nameof(_MyType3.DateTimeProperty)].Should().NotBeNull();
        }

        [TestMethod]
        public void GetProperties_When_New_Property_Hides_Base_Property()
        {
            var properties = TypeConverterHelper.GetProperties(typeof(_MyType5));
            properties.Should().HaveCount(1);
            properties[0].Name.Should().Be(nameof(_MyType5.FloatProperty));
            properties[0].ComponentType.Should().BeSameAs(typeof(_MyType5));
        }

        [TestMethod]
        public void GetProperties_When_Property_Is_Overridden()
        {
            var properties = TypeConverterHelper.GetProperties(typeof(_MyClass2));
            properties.Should().HaveCount(1);
            properties[0].Name.Should().Be(nameof(_MyClass2.ByteProperty));
            properties[0].ComponentType.Should().BeSameAs(typeof(_MyClass2));
        }

        [TestMethod]
        public void GetProperties_InternalConverter_When_No_TypeConverterAttribute()
        {
            TypeConverterHelper.GetProperties(typeof(_MyType1))[nameof(_MyType1.IntProperty)]
                .InternalConverter
                .Should()
                .BeNull();
        }

        [TestMethod]
        public void GetProperties_InternalConverter_When_TypeConverterAttribute()
        {
            TypeConverterHelper.GetProperties(typeof(_MyType3))[nameof(_MyType3.DateTimeProperty)]
                .InternalConverter
                .Should()
                .BeOfType<_MyDateTimeConverter>();
        }

        [TestMethod]
        public void GetProperties_InternalConverter_When_Property_Is_Overridden_And_Base_Has_TypeConverterAttribute()
        {
            TypeConverterHelper.GetProperties(typeof(_MyClass2))[nameof(_MyClass2.ByteProperty)]
               .InternalConverter
               .Should()
               .BeNull();
        }

        [TestMethod]
        public void GetProperties_InternalConverter_Should_Be_Cached()
        {
            TypeConverterHelper.GetProperties(typeof(_MyType1))[nameof(_MyType1.IntProperty)]
                .InternalConverter
                .Should()
                .BeSameAs(
                    TypeConverterHelper.GetProperties(typeof(_MyType1))[nameof(_MyType1.IntProperty)]
                    .InternalConverter
                );
        }

        [TestMethod]
        public void GetProperties_Converter_When_No_TypeConverterAttribute_And_PropertyType_Has_No_TypeConverter()
        {
            TypeConverterHelper.GetProperties(typeof(_MyClass5))[nameof(_MyClass5.MyType1Property)]
                .Converter
                .Should()
                .BeNull();
        }

        [TestMethod]
        public void GetProperties_Converter_When_No_TypeConverterAttribute_And_PropertyType_Has_TypeConverter()
        {
            TypeConverterHelper.GetProperties(typeof(_MyType1))[nameof(_MyType1.IntProperty)]
                .Converter
                .Should()
                .BeSameAs(TypeConverterHelper.GetConverter(typeof(int)));
        }

        [TestMethod]
        public void GetProperties_Converter_When_TypeConverterAttribute_And_PropertyType_Has_No_TypeConverter()
        {
            TypeConverterHelper.GetProperties(typeof(_MyClass3))[nameof(_MyClass3.ByteProperty)]
                .Converter
                .Should()
                .BeOfType<_MyByteConverter1>();
        }

        [TestMethod]
        public void GetProperties_Converter_When_TypeConverterAttribute_And_PropertyType_Has_TypeConverter()
        {
            TypeConverterHelper.GetProperties(typeof(_MyClass3))[nameof(_MyClass3.ByteProperty)]
                .Converter
                .Should()
                .BeOfType<_MyByteConverter1>();
        }

        private class _MyType1
        {
            public int IntProperty { get; set; }
        
            public double DoubleProperty { get; set; }
        }

        private class _MyType2 : _MyType1
        {
            public string StringProperty { get; set; }
        }

        private class _MyType3 : _MyType2
        {
            [TypeConverter(typeof(_MyDateTimeConverter))]
            public DateTime DateTimeProperty { get; set; }
        }

        private class _MyDateTimeConverter : TypeConverter { }

        private class _MyType4
        {
            public float FloatProperty { get; set; }
        }

        private class _MyType5 : _MyType4
        {
            public new float FloatProperty { get; set; }
        }

        private class _MyClass1
        {
            [TypeConverter(typeof(_MyByteConverter1))]
            public virtual byte ByteProperty { get; set; }
        }

        private class _MyClass2 : _MyClass1
        {
            public override byte ByteProperty
            {
                get => base.ByteProperty;
                set => base.ByteProperty = value;
            }
        }

        private class _MyClass3
        {
            [TypeConverter(typeof(_MyByteConverter1))]
            public virtual byte ByteProperty { get; set; }
        }

        private class _MyClass4 : _MyClass3
        {
            [TypeConverter(typeof(_MyByteConverter2))]
            public override byte ByteProperty
            {
                get => base.ByteProperty;
                set => base.ByteProperty = value;
            }
        }

        private class _MyClass5
        {
            public _MyType1 MyType1Property { get; set; }
        }

        private class _MyByteConverter1 : TypeConverter { }
        
        private class _MyByteConverter2 : TypeConverter { }
    }
}
