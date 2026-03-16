
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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Tests
{
    public partial class TypeConverterHelperTest
    {
        [TestMethod]
        public void GetProperties_Inheritance_1()
        {
            var properties = TypeConverterHelper.GetProperties(typeof(_MyType1));

            Assert.AreEqual(2, properties.Count);
            Assert.IsNotNull(properties[nameof(_MyType1.IntProperty)]);
            Assert.IsNotNull(properties[nameof(_MyType1.DoubleProperty)]);
        }

        [TestMethod]
        public void GetProperties_Inheritance_2()
        {
            var properties = TypeConverterHelper.GetProperties(typeof(_MyType2));

            Assert.AreEqual(3, properties.Count);
            Assert.IsNotNull(properties[nameof(_MyType1.IntProperty)]);
            Assert.IsNotNull(properties[nameof(_MyType1.DoubleProperty)]);
            Assert.IsNotNull(properties[nameof(_MyType2.StringProperty)]);
        }

        [TestMethod]
        public void GetProperties_Inheritance_3()
        {
            var properties = TypeConverterHelper.GetProperties(typeof(_MyType3));

            Assert.AreEqual(4, properties.Count);
            Assert.IsNotNull(properties[nameof(_MyType1.IntProperty)]);
            Assert.IsNotNull(properties[nameof(_MyType1.DoubleProperty)]);
            Assert.IsNotNull(properties[nameof(_MyType2.StringProperty)]);
            Assert.IsNotNull(properties[nameof(_MyType3.DateTimeProperty)]);
        }

        [TestMethod]
        public void GetProperties_When_New_Property_Hides_Base_Property()
        {
            var properties = TypeConverterHelper.GetProperties(typeof(_MyType5));

            Assert.AreEqual(1, properties.Count);
            Assert.AreEqual(nameof(_MyType5.FloatProperty), properties[0].Name);
            Assert.AreSame(typeof(_MyType5), properties[0].ComponentType);
        }

        [TestMethod]
        public void GetProperties_When_Property_Is_Overridden()
        {
            var properties = TypeConverterHelper.GetProperties(typeof(_MyClass2));

            Assert.AreEqual(1, properties.Count);
            Assert.AreEqual(nameof(_MyClass2.ByteProperty), properties[0].Name);
            Assert.AreSame(typeof(_MyClass2), properties[0].ComponentType);
        }

        [TestMethod]
        public void GetProperties_InternalConverter_When_No_TypeConverterAttribute()
        {
            Assert.IsNull(TypeConverterHelper.GetProperties(typeof(_MyType1))[nameof(_MyType1.IntProperty)].InternalConverter);
        }

        [TestMethod]
        public void GetProperties_InternalConverter_When_TypeConverterAttribute()
        {
            Assert.IsInstanceOfType<_MyDateTimeConverter>(
                TypeConverterHelper.GetProperties(typeof(_MyType3))[nameof(_MyType3.DateTimeProperty)].InternalConverter);
        }

        [TestMethod]
        public void GetProperties_InternalConverter_When_Property_Is_Overridden_And_Base_Has_TypeConverterAttribute()
        {
            Assert.IsNull(TypeConverterHelper.GetProperties(typeof(_MyClass2))[nameof(_MyClass2.ByteProperty)].InternalConverter);
        }

        [TestMethod]
        public void GetProperties_InternalConverter_Should_Be_Cached()
        {
            Assert.AreSame(
                TypeConverterHelper.GetProperties(typeof(_MyType1))[nameof(_MyType1.IntProperty)].InternalConverter,
                TypeConverterHelper.GetProperties(typeof(_MyType1))[nameof(_MyType1.IntProperty)].InternalConverter);
        }

        [TestMethod]
        public void GetProperties_Converter_When_No_TypeConverterAttribute_And_PropertyType_Has_No_TypeConverter()
        {
            Assert.IsNull(TypeConverterHelper.GetProperties(typeof(_MyClass5))[nameof(_MyClass5.MyType1Property)].Converter);
        }

        [TestMethod]
        public void GetProperties_Converter_When_No_TypeConverterAttribute_And_PropertyType_Has_TypeConverter()
        {
            Assert.AreSame(
                TypeConverterHelper.GetProperties(typeof(_MyType1))[nameof(_MyType1.IntProperty)].Converter,
                TypeConverterHelper.GetConverter(typeof(int)));
        }

        [TestMethod]
        public void GetProperties_Converter_When_TypeConverterAttribute_And_PropertyType_Has_No_TypeConverter()
        {
            Assert.IsInstanceOfType<_MyByteConverter1>(
                TypeConverterHelper.GetProperties(typeof(_MyClass3))[nameof(_MyClass3.ByteProperty)].Converter);
        }

        [TestMethod]
        public void GetProperties_Converter_When_TypeConverterAttribute_And_PropertyType_Has_TypeConverter()
        {
            Assert.IsInstanceOfType<_MyByteConverter1>(
                TypeConverterHelper.GetProperties(typeof(_MyClass3))[nameof(_MyClass3.ByteProperty)].Converter);
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
