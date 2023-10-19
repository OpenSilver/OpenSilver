
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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NullableConverter2 = System.ComponentModel.TypeConverterHelper.NullableConverter2;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace System.ComponentModel.Tests
{
    [TestClass]
    public partial class TypeConverterHelperTest
    {
        [TestMethod]
        public void GetConverter_When_Cursor()
        {
            TypeConverterHelper.GetConverter(typeof(Cursor))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<CursorConverter>();
        }

        [TestMethod]
        public void GetConverter_When_KeyTime()
        {
            TypeConverterHelper.GetConverter(typeof(KeyTime))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<KeyTimeConverter>();
        }

        [TestMethod]
        public void GetConverter_When_RepeatBehavior()
        {
            TypeConverterHelper.GetConverter(typeof(RepeatBehavior))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<RepeatBehaviorConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Brush()
        {
            TypeConverterHelper.GetConverter(typeof(Brush))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<BrushConverter>();
        }

        [TestMethod]
        public void GetConverter_When_SolidColorBrush()
        {
            TypeConverterHelper.GetConverter(typeof(SolidColorBrush))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<BrushConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Color()
        {
            TypeConverterHelper.GetConverter(typeof(Color))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<ColorConverter>();
        }

        [TestMethod]
        public void GetConverter_When_DoubleCollection()
        {
            TypeConverterHelper.GetConverter(typeof(DoubleCollection))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<DoubleCollectionConverter>();
        }

        [TestMethod]
        public void GetConverter_When_FontFamily()
        {
            TypeConverterHelper.GetConverter(typeof(FontFamily))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<FontFamilyConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Geometry()
        {
            TypeConverterHelper.GetConverter(typeof(Geometry))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<GeometryConverter>();
        }

        [TestMethod]
        public void GetConverter_When_PathGeometry()
        {
            TypeConverterHelper.GetConverter(typeof(PathGeometry))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<GeometryConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Matrix()
        {
            TypeConverterHelper.GetConverter(typeof(Matrix))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<MatrixConverter>();
        }

        [TestMethod]
        public void GetConverter_When_PointCollection()
        {
            TypeConverterHelper.GetConverter(typeof(PointCollection))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<PointCollectionConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Transform()
        {
            TypeConverterHelper.GetConverter(typeof(Transform))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<TransformConverter>();
        }

        [TestMethod]
        public void GetConverter_When_MatrixTransform()
        {
            TypeConverterHelper.GetConverter(typeof(MatrixTransform))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<TransformConverter>();
        }

        [TestMethod]
        public void GetConverter_When_CacheMode()
        {
            TypeConverterHelper.GetConverter(typeof(CacheMode))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<CacheModeConverter>();
        }

        [TestMethod]
        public void GetConverter_When_CornerRadius()
        {
            TypeConverterHelper.GetConverter(typeof(CornerRadius))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<CornerRadiusConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Duration()
        {
            TypeConverterHelper.GetConverter(typeof(Duration))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<DurationConverter>();
        }

        [TestMethod]
        public void GetConverter_When_FontWeight()
        {
            TypeConverterHelper.GetConverter(typeof(FontWeight))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<FontWeightConverter>();
        }

        [TestMethod]
        public void GetConverter_When_GridLength()
        {
            TypeConverterHelper.GetConverter(typeof(GridLength))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<GridLengthConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Point()
        {
            TypeConverterHelper.GetConverter(typeof(Point))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<PointConverter>();
        }

        [TestMethod]
        public void GetConverter_When_PropertyPath()
        {
            TypeConverterHelper.GetConverter(typeof(PropertyPath))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<PropertyPathConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Rect()
        {
            TypeConverterHelper.GetConverter(typeof(Rect))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<RectConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Size()
        {
            TypeConverterHelper.GetConverter(typeof(Size))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<SizeConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Thickness()
        {
            TypeConverterHelper.GetConverter(typeof(Thickness))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<ThicknessConverter>();
        }

        [TestMethod]
        public void GetConverter_When_FontStretch()
        {
            TypeConverterHelper.GetConverter(typeof(FontStretch))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<FontStretchConverter>();
        }

        [TestMethod]
        public void GetConverter_When_FontStyle()
        {
            TypeConverterHelper.GetConverter(typeof(FontStyle))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<FontStyleConverter>();
        }

        [TestMethod]
        public void GetConverter_When_TextDecorationCollection()
        {
            TypeConverterHelper.GetConverter(typeof(TextDecorationCollection))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<TextDecorationCollectionConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Boolean()
        {
            TypeConverterHelper.GetConverter(typeof(bool))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<BooleanConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Byte()
        {
            TypeConverterHelper.GetConverter(typeof(byte))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<ByteConverter>();
        }

        [TestMethod]
        public void GetConverter_When_SByte()
        {
            TypeConverterHelper.GetConverter(typeof(sbyte))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<SByteConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Char()
        {
            TypeConverterHelper.GetConverter(typeof(char))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<CharConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Double()
        {
            TypeConverterHelper.GetConverter(typeof(double))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<DoubleConverter>();
        }

        [TestMethod]
        public void GetConverter_When_String()
        {
            TypeConverterHelper.GetConverter(typeof(string))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<StringConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Int16()
        {
            TypeConverterHelper.GetConverter(typeof(short))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<Int16Converter>();
        }

        [TestMethod]
        public void GetConverter_When_Int32()
        {
            TypeConverterHelper.GetConverter(typeof(int))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<Int32Converter>();
        }

        [TestMethod]
        public void GetConverter_When_Int64()
        {
            TypeConverterHelper.GetConverter(typeof(long))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<Int64Converter>();
        }

        [TestMethod]
        public void GetConverter_When_Single()
        {
            TypeConverterHelper.GetConverter(typeof(float))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<SingleConverter>();
        }

        [TestMethod]
        public void GetConverter_When_UInt16()
        {
            TypeConverterHelper.GetConverter(typeof(ushort))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<UInt16Converter>();
        }

        [TestMethod]
        public void GetConverter_When_UInt32()
        {
            TypeConverterHelper.GetConverter(typeof(uint))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<UInt32Converter>();
        }

        [TestMethod]
        public void GetConverter_When_UInt64()
        {
            TypeConverterHelper.GetConverter(typeof(ulong))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<UInt64Converter>();
        }

        [TestMethod]
        public void GetConverter_When_CultureInfo()
        {
            TypeConverterHelper.GetConverter(typeof(CultureInfo))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<CultureInfoConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Derived_From_CultureInfo()
        {
            TypeConverterHelper.GetConverter(typeof(CultureInfo2))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<CultureInfoConverter>();
        }

        [TestMethod]
        public void GetConverter_When_DateTime()
        {
            TypeConverterHelper.GetConverter(typeof(DateTime))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<DateTimeConverter>();
        }

        [TestMethod]
        public void GetConverter_When_DateTimeOffset()
        {
            TypeConverterHelper.GetConverter(typeof(DateTimeOffset))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<DateTimeOffsetConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Decimal()
        {
            TypeConverterHelper.GetConverter(typeof(decimal))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<DecimalConverter>();
        }

        [TestMethod]
        public void GetConverter_When_TimeSpan()
        {
            TypeConverterHelper.GetConverter(typeof(TimeSpan))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<TimeSpanConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Guid()
        {
            TypeConverterHelper.GetConverter(typeof(Guid))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<GuidConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Uri()
        {
            TypeConverterHelper.GetConverter(typeof(Uri))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<UriTypeConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Derived_From_Uri()
        {
            TypeConverterHelper.GetConverter(typeof(Uri2))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<UriTypeConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Enum()
        {
            var converter = TypeConverterHelper.GetConverter(typeof(MyEnum1));
            converter.Should()
                .NotBeNull()
                .And
                .BeOfType<EnumConverter>();

            ((Type)typeof(EnumConverter)
                .GetProperty("EnumType", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(converter))
                .Should()
                .NotBeNull()
                .And
                .BeSameAs(typeof(MyEnum1));
        }

        [TestMethod]
        public void GetConverter_When_Nullable_Of_IntrinsicType()
        {
            var converter = TypeConverterHelper.GetConverter(typeof(int?));
            converter.Should()
                .NotBeNull()
                .And
                .BeOfType<NullableConverter2>();

            converter.As<NullableConverter2>()
                .NullableType
                .Should()
                .BeSameAs(typeof(int?));

            converter.As<NullableConverter2>()
                .UnderlyingType
                .Should()
                .BeSameAs(typeof(int));

            converter.As<NullableConverter2>()
                .UnderlyingTypeConverter
                .Should()
                .NotBeNull()
                .And
                .BeOfType<Int32Converter>();
        }

        [TestMethod]
        public void GetConverter_When_Nullable_Of_CoreType()
        {
            var converter = TypeConverterHelper.GetConverter(typeof(Point?));
            converter.Should()
                .NotBeNull()
                .And
                .BeOfType<NullableConverter2>();

            converter.As<NullableConverter2>()
                .NullableType
                .Should()
                .BeSameAs(typeof(Point?));

            converter.As<NullableConverter2>()
                .UnderlyingType
                .Should()
                .BeSameAs(typeof(Point));

            converter.As<NullableConverter2>()
                .UnderlyingTypeConverter
                .Should()
                .NotBeNull()
                .And
                .BeOfType<PointConverter>();
        }

        [TestMethod]
        public void GetConverter_When_Nullable_Of_TypeConverterAttribute()
        {
            var converter = TypeConverterHelper.GetConverter(typeof(MyStruct1?));
            converter.Should()
                .NotBeNull()
                .And
                .BeOfType<NullableConverter2>();

            converter.As<NullableConverter2>()
                .NullableType
                .Should()
                .BeSameAs(typeof(MyStruct1?));

            converter.As<NullableConverter2>()
                .UnderlyingType
                .Should()
                .BeSameAs(typeof(MyStruct1));

            converter.As<NullableConverter2>()
                .UnderlyingTypeConverter
                .Should()
                .NotBeNull()
                .And
                .BeOfType<MyStruct1Converter>();
        }

        [TestMethod]
        public void GetConverter_When_ICommand()
        {
            TypeConverterHelper.GetConverter(typeof(ICommand))
                .Should()
                .BeNull();
        }

        [TestMethod]
        public void GetConverter_When_TypeConverterAttribute_1()
        {
            TypeConverterHelper.GetConverter(typeof(MyClass1))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<MyClass1Converter>();
        }

        [TestMethod]
        public void GetConverter_When_TypeConverterAttribute_2()
        {
            TypeConverterHelper.GetConverter(typeof(MyClass2))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<MyClass1Converter>();
        }

        [TestMethod]
        public void GetConverter_When_TypeConverterAttribute_3()
        {
            TypeConverterHelper.GetConverter(typeof(MyClass3))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<MyClass3Converter>();
        }

        [TestMethod]
        public void GetConverter_When_TypeConverterAttribute_4()
        {
            TypeConverterHelper.GetConverter(typeof(IMyClass4))
                .Should()
                .NotBeNull()
                .And
                .BeOfType<IMyClass4Converter>();
        }

        [TestMethod]
        public void GetConverter_When_TypeConverterAttribute_5()
        {
            TypeConverterHelper.GetConverter(typeof(MyClass4))
                .Should()
                .BeNull();
        }

        [TestMethod]
        public void GetConverter_When_No_TypeConverterAttribute()
        {
            TypeConverterHelper.GetConverter(typeof(MyClass5))
                .Should()
                .BeNull();
        }

        [TestMethod]
        public void GetConverter_Should_Cache_Intrinsic_TypeConverter()
        {
            TypeConverterHelper.GetConverter(typeof(long))
                .Should()
                .BeSameAs(TypeConverterHelper.GetConverter(typeof(long)));
        }

        [TestMethod]
        public void GetConverter_Should_Cache_Nullable_TypeConverter()
        {
            TypeConverterHelper.GetConverter(typeof(MyStruct1?))
                .Should()
                .BeSameAs(TypeConverterHelper.GetConverter(typeof(MyStruct1?)));
        }

        [TestMethod]
        public void GetConverter_Should_Cache_Enum_TypeConverter()
        {
            TypeConverterHelper.GetConverter(typeof(MyEnum1))
                .Should()
                .BeSameAs(TypeConverterHelper.GetConverter(typeof(MyEnum1)));
        }

        [TestMethod]
        public void GetConverter_Should_Cache_TypeConverterAttribute_TypeConverter_1()
        {
            TypeConverterHelper.GetConverter(typeof(MyClass1))
                .Should()
                .BeSameAs(TypeConverterHelper.GetConverter(typeof(MyClass1)));
        }

        [TestMethod]
        public void GetConverter_Should_Cache_TypeConverterAttribute_TypeConverter_2()
        {
            TypeConverterHelper.GetConverter(typeof(MyClass1))
                .Should()
                .BeSameAs(TypeConverterHelper.GetConverter(typeof(MyClass2)));
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
