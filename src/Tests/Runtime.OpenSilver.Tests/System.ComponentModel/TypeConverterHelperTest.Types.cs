
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

using System.Globalization;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NullableConverter2 = System.ComponentModel.TypeConverterHelper.NullableConverter2;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using OpenSilver.Internal.Helpers;

namespace System.ComponentModel.Tests
{
    [TestClass]
    public partial class TypeConverterHelperTest
    {
        [TestMethod]
        public void GetConverter_When_Cursor()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(Cursor));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<CursorConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_KeyTime()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(KeyTime));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<KeyTimeConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_RepeatBehavior()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(RepeatBehavior));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<RepeatBehaviorConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Brush()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(Brush));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<BrushConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_SolidColorBrush()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(SolidColorBrush));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<BrushConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Color()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(Color));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<ColorConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_DoubleCollection()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(DoubleCollection));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<DoubleCollectionConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_FontFamily()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(FontFamily));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<FontFamilyConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Geometry()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(Geometry));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<GeometryConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_PathGeometry()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(PathGeometry));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<GeometryConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Matrix()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(Matrix));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<MatrixConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_PointCollection()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(PointCollection));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<PointCollectionConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Transform()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(Transform));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<TransformConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_MatrixTransform()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(MatrixTransform));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<TransformConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_CacheMode()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(CacheMode));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<CacheModeConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_CornerRadius()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(CornerRadius));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<CornerRadiusConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Duration()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(Duration));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<DurationConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_FontWeight()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(FontWeight));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<FontWeightConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_GridLength()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(GridLength));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<GridLengthConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Point()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(Point));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<PointConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_PropertyPath()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(PropertyPath));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<PropertyPathConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Rect()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(Rect));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<RectConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Size()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(Size));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<SizeConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Thickness()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(Thickness));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<ThicknessConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_FontStretch()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(FontStretch));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<FontStretchConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_FontStyle()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(FontStyle));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<FontStyleConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_TextDecorationCollection()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(TextDecorationCollection));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<TextDecorationCollectionConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Boolean()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(bool));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<BooleanConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Byte()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(byte));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<ByteConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_SByte()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(sbyte));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<SByteConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Char()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(char));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<CharConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Double()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(double));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<DoubleConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_String()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(string));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<StringConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Int16()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(short));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<Int16Converter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Int32()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(int));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<Int32Converter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Int64()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(long));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<Int64Converter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Single()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(float));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<SingleConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_UInt16()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(ushort));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<UInt16Converter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_UInt32()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(uint));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<UInt32Converter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_UInt64()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(ulong));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<UInt64Converter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_CultureInfo()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(CultureInfo));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<CultureInfoConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Derived_From_CultureInfo()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(CultureInfo2));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<CultureInfoConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_DateTime()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(DateTime));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<DateTimeConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_DateTimeOffset()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(DateTimeOffset));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<DateTimeOffsetConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Decimal()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(decimal));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<DecimalConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_TimeSpan()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(TimeSpan));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<TimeSpanConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Guid()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(Guid));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<GuidConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Uri()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(Uri));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<UriTypeConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Derived_From_Uri()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(Uri2));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<UriTypeConverter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_Enum()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(MyEnum1));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<EnumConverter>(converter);

            Type enumType = ((Type)typeof(EnumConverter)
                .GetProperty("EnumType", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(converter));

            Assert.IsNotNull(enumType);
            Assert.AreSame(typeof(MyEnum1), enumType);
        }

        [TestMethod]
        public void GetConverter_When_Nullable_Of_IntrinsicType()
        {
            var converter = TypeConverterHelper.GetConverter(typeof(int?));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<NullableConverter2>(converter);

            Assert.AreSame(typeof(int?), converter.As<NullableConverter2>().NullableType);
            Assert.AreSame(typeof(int), converter.As<NullableConverter2>().UnderlyingType);
            Assert.IsNotNull(converter.As<NullableConverter2>().UnderlyingTypeConverter);
            Assert.IsInstanceOfType<Int32Converter>(converter.As<NullableConverter2>().UnderlyingTypeConverter);
        }

        [TestMethod]
        public void GetConverter_When_Nullable_Of_CoreType()
        {
            var converter = TypeConverterHelper.GetConverter(typeof(Point?));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<NullableConverter2>(converter);

            Assert.AreSame(typeof(Point?), converter.As<NullableConverter2>().NullableType);
            Assert.AreSame(typeof(Point), converter.As<NullableConverter2>().UnderlyingType);
            Assert.IsNotNull(converter.As<NullableConverter2>().UnderlyingTypeConverter);
            Assert.IsInstanceOfType<PointConverter>(converter.As<NullableConverter2>().UnderlyingTypeConverter);
        }

        [TestMethod]
        public void GetConverter_When_Nullable_Of_TypeConverterAttribute()
        {
            var converter = TypeConverterHelper.GetConverter(typeof(MyStruct1?));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<NullableConverter2>(converter);

            Assert.AreSame(typeof(MyStruct1?), converter.As<NullableConverter2>().NullableType);
            Assert.AreSame(typeof(MyStruct1), converter.As<NullableConverter2>().UnderlyingType);
            Assert.IsNotNull(converter.As<NullableConverter2>().UnderlyingTypeConverter);
            Assert.IsInstanceOfType<MyStruct1Converter>(converter.As<NullableConverter2>().UnderlyingTypeConverter);
        }

        [TestMethod]
        public void GetConverter_When_ICommand()
        {
            Assert.IsInstanceOfType<CommandConverter>(TypeConverterHelper.GetConverter(typeof(ICommand)));
        }

        [TestMethod]
        public void GetConverter_When_TypeConverterAttribute_1()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(MyClass1));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<MyClass1Converter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_TypeConverterAttribute_2()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(MyClass2));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<MyClass1Converter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_TypeConverterAttribute_3()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(MyClass3));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<MyClass3Converter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_TypeConverterAttribute_4()
        {
            TypeConverter converter = TypeConverterHelper.GetConverter(typeof(IMyClass4));

            Assert.IsNotNull(converter);
            Assert.IsInstanceOfType<IMyClass4Converter>(converter);
        }

        [TestMethod]
        public void GetConverter_When_TypeConverterAttribute_5()
        {
            Assert.IsNull(TypeConverterHelper.GetConverter(typeof(MyClass4)));
        }

        [TestMethod]
        public void GetConverter_When_No_TypeConverterAttribute()
        {
            Assert.IsNull(TypeConverterHelper.GetConverter(typeof(MyClass5)));
        }

        [TestMethod]
        public void GetConverter_Should_Cache_Intrinsic_TypeConverter()
        {
            Assert.AreSame(TypeConverterHelper.GetConverter(typeof(long)), TypeConverterHelper.GetConverter(typeof(long)));
        }

        [TestMethod]
        public void GetConverter_Should_Cache_Nullable_TypeConverter()
        {
            Assert.AreSame(TypeConverterHelper.GetConverter(typeof(MyStruct1?)), TypeConverterHelper.GetConverter(typeof(MyStruct1?)));
        }

        [TestMethod]
        public void GetConverter_Should_Cache_Enum_TypeConverter()
        {
            Assert.AreSame(TypeConverterHelper.GetConverter(typeof(MyEnum1)), TypeConverterHelper.GetConverter(typeof(MyEnum1)));
        }

        [TestMethod]
        public void GetConverter_Should_Cache_TypeConverterAttribute_TypeConverter_1()
        {
            Assert.AreSame(TypeConverterHelper.GetConverter(typeof(MyClass1)), TypeConverterHelper.GetConverter(typeof(MyClass1)));
        }

        [TestMethod]
        public void GetConverter_Should_Cache_TypeConverterAttribute_TypeConverter_2()
        {
            Assert.AreSame(TypeConverterHelper.GetConverter(typeof(MyClass1)), TypeConverterHelper.GetConverter(typeof(MyClass2)));
        }

        private class CultureInfo2 : CultureInfo
        {
            public CultureInfo2(string name) : base(name) { }
        }

        private class Uri2 : Uri
        {
            public Uri2(string uriString) : base(uriString) { }
        }

        private enum MyEnum1 { A, B, C, }

        [TypeConverter(typeof(MyStruct1Converter))]
        private struct MyStruct1 { }

        private class MyStruct1Converter : TypeConverter { }

        [TypeConverter(typeof(MyClass1Converter))]
        private class MyClass1 { }

        private class MyClass1Converter : TypeConverter { }

        private class MyClass2 : MyClass1 { }

        [TypeConverter(typeof(MyClass3Converter))]
        private class MyClass3 : MyClass1 { }

        private class MyClass3Converter : TypeConverter { }

        [TypeConverter(typeof(IMyClass4Converter))]
        private interface IMyClass4 { }

        private class IMyClass4Converter : TypeConverter { }

        private class MyClass4 : IMyClass4 { }

        private class MyClass5 { }
    }
}
