
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


using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Experimental;
using OpenSilver.Compiler.OtherHelpersAndHandlers.MonoCecilAssembliesInspector;
using OpenSilver.Compiler;
using System.Windows;
using System.Windows.Controls;

namespace Compiler.Tests
{
    [TestClass]
    public class MonoCecilAssembliesInspectorTest
    {
        private const string GlobalPrefix = "global::";
        private const string ExperimentalSubjectName = "Experimental";
        private const string ExperimentalSubjectDll = ExperimentalSubjectName + ".dll";
        private const string ExperimentalNamespace = "Experimental";
        private const string Content = "Content";

        private static readonly MonoCecilAssembliesInspectorImpl MonoCecilVersion = new(SupportedLanguage.CSharp);

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            MonoCecilVersion.LoadAssembly(ExperimentalSubjectDll, true);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            MonoCecilVersion.Dispose();
        }

        [TestMethod]
        public void GetAssemblyQualifiedNameOfXamlType_Should_Return_Name()
        {
            var res = MonoCecilVersion.GetAssemblyQualifiedNameOfXamlType("http://schemas.microsoft.com/winfx/2006/xaml/presentation", nameof(Validation), null);

            res.Should().Be(typeof(Validation).FullName + ", OpenSilver");
        }

        [TestMethod]
        public void GetAttachedPropertyGetMethodInfo_Should_Find_AttachedPropertyGetMethod()
        {
            MonoCecilVersion.GetAttachedPropertyGetMethodInfo(nameof(ToolTipService.GetPlacementTarget), "http://schemas.microsoft.com/winfx/2006/xaml/presentation", nameof(ToolTipService),
                out var declaringTypeName, out var returnValueNamespaceName, out var returnValueLocalTypeName, out var isTypeString, out var isTypeEnum);

            declaringTypeName.Should().Be(GlobalPrefix + typeof(ToolTipService).FullName);
            returnValueNamespaceName.Should().Be(typeof(UIElement).Namespace);
            returnValueLocalTypeName.Should().Be(nameof(UIElement));
            isTypeString.Should().BeFalse();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void IsTypeAnEnum_Should_Return_True_For_Enum()
        {
            var res = MonoCecilVersion.IsTypeAnEnum(ExperimentalSubjectName, nameof(PlanetStructure));
            res.Should().BeTrue();
        }

        [TestMethod]
        public void GetPropertyOrFieldTypeInfo_Should_Return_Info_For_Generic_Parameters()
        {
            MonoCecilVersion.GetPropertyOrFieldTypeInfo(nameof(DerivedClassGenericType.MyProperty), ExperimentalNamespace, nameof(DerivedClassGenericType),
                out var propertyNamespaceName, out var propertyLocalTypeName, out var propertyAssemblyName,
                out var isTypeString,
                out var isTypeEnum);
            propertyNamespaceName.Should().Be(typeof(string).Namespace);
            propertyLocalTypeName.Should().Be(nameof(String));
            propertyAssemblyName.Should().Be(typeof(string).Assembly.GetName().Name);
            isTypeString.Should().BeTrue();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void GetPropertyOrFieldTypeInfo_Should_Return_Info_For_Property_With_Nested_Generic()
        {
            MonoCecilVersion.GetPropertyOrFieldTypeInfo(nameof(DerivedClassGenericType.PropertyWithNestedGeneric), ExperimentalNamespace, nameof(DerivedClassGenericType),
                out var propertyNamespaceName, out var propertyLocalTypeName, out var propertyAssemblyName,
                out var isTypeString,
                out var isTypeEnum);
            propertyNamespaceName.Should().Be(ExperimentalNamespace);
            propertyLocalTypeName.Should().Be("AnotherGenericType<global::Experimental.AnotherGenericType<global::System.Int32>>");
            propertyAssemblyName.Should().Be(ExperimentalSubjectName);
            isTypeString.Should().BeFalse();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void GetPropertyOrFieldTypeInfo_Should_Return_Info_For_Nested_Enum_Field()
        {
            MonoCecilVersion.GetPropertyOrFieldTypeInfo(nameof(ClassWithField.Behavior), ExperimentalNamespace, nameof(ClassWithField),
                out var propertyNamespaceName, out var propertyLocalTypeName, out var propertyAssemblyName,
                out var isTypeString,
                out var isTypeEnum);
            propertyNamespaceName.Should().Be(typeof(ClassWithNestedEnum).FullName);
            propertyLocalTypeName.Should().Be(nameof(ClassWithNestedEnum.InputBehavior));
            propertyAssemblyName.Should().Be(ExperimentalSubjectName);
            isTypeString.Should().BeFalse();
            isTypeEnum.Should().BeTrue();
        }

        [TestMethod]
        public void GetPropertyOrFieldInfo_Should_Return_Info_For_Generic_Property()
        {
            MonoCecilVersion.GetPropertyOrFieldInfo(nameof(DerivedClassGenericType.MyProperty), ExperimentalNamespace, nameof(DerivedClassGenericType),
                out var memberDeclaringTypeName, out var memberTypeNamespace, out var memberTypeName,
                out var isTypeString, out var isTypeEnum);
            memberDeclaringTypeName.Should().Be("global::Experimental.GenericType<global::System.Double, global::System.Int32, global::System.String>");
            memberTypeNamespace.Should().Be(typeof(string).Namespace);
            memberTypeName.Should().Be(nameof(String));
            isTypeString.Should().BeTrue();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void GetPropertyOrFieldInfo_Should_Return_Info_For_Property()
        {
            MonoCecilVersion.GetPropertyOrFieldInfo(nameof(DerivedClassGenericType.MyNonGenericProperty), ExperimentalNamespace, nameof(DerivedClassGenericType),
                out var memberDeclaringTypeName, out var memberTypeNamespace, out var memberTypeName,
                out var isTypeString, out var isTypeEnum);
            memberDeclaringTypeName.Should().Be("global::Experimental.GenericType<global::System.Double, global::System.Int32, global::System.String>");
            memberTypeNamespace.Should().Be(typeof(int).Namespace);
            memberTypeName.Should().Be(nameof(Int32));
            isTypeString.Should().BeFalse();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void GetFiled_Should_Return_Full_TypeName()
        {
            var res = MonoCecilVersion.GetField(nameof(DerivedClassGenericType.MyField), ExperimentalNamespace, nameof(DerivedClassGenericType), null);
            res.Should().Be("global::Experimental.DerivedClassGenericType.MyField");
        }

        [TestMethod]
        public void GetAttachedPropertyGetMethodInfo_Should_Return_Get_Method_From_Generic_Type()
        {
            MonoCecilVersion.GetAttachedPropertyGetMethodInfo(nameof(DerivedClassGenericType.GetHasSomething), ExperimentalNamespace, nameof(DerivedClassGenericType),
                out var declaringTypeName, out var returnValueNamespaceName, out var returnValueLocalTypeName,
                out var isTypeString, out var isTypeEnum);
            declaringTypeName.Should().Be("global::Experimental.GenericType<global::System.Double, global::System.Int32, global::System.String>");
            returnValueNamespaceName.Should().Be(typeof(string).Namespace);
            returnValueLocalTypeName.Should().Be(nameof(String));
            isTypeString.Should().BeTrue();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void GetEnumValue_Should_Handle_Nested_Enum_Type()
        {
            var res = MonoCecilVersion.GetEnumValue(nameof(ClassWithNestedEnum.InputBehavior.SelectFromList).ToLower(),typeof(ClassWithNestedEnum).FullName, nameof(ClassWithNestedEnum.InputBehavior), null, true, true);

            res.Should().Be($"{GlobalPrefix}{typeof(ClassWithNestedEnum).FullName}.{nameof(ClassWithNestedEnum.InputBehavior)}.{nameof(ClassWithNestedEnum.InputBehavior.SelectFromList)}");
        }

        [TestMethod]
        public void GetEnumValue_Should_Handle_Integer_Input_Value()
        {
            var res = MonoCecilVersion.GetEnumValue("1", typeof(ClassWithNestedEnum).FullName, nameof(ClassWithNestedEnum.InputBehavior), null, true, true);

            res.Should().Be($"({GlobalPrefix}{typeof(ClassWithNestedEnum).FullName}.{nameof(ClassWithNestedEnum.InputBehavior)})1");
        }

        [TestMethod]
        public void GetEnumValue_Should_Return_Value_For_Enum_Without_Namespace()
        {
            var res = MonoCecilVersion.GetEnumValue(nameof(EnumWithoutNamespace.Item), "", nameof(EnumWithoutNamespace), null, true, false);

            res.Should().Be($"{GlobalPrefix}{typeof(EnumWithoutNamespace).FullName}.{nameof(EnumWithoutNamespace.Item)}");
        }

        [TestMethod]
        public void GetContentPropertyName_Should_Return_Value()
        {
            var res = MonoCecilVersion.GetContentPropertyName(typeof(ContentControl).Namespace, nameof(ContentControl));
            res.Should().Be(Content);
        }

        [TestMethod]
        public void GetMethodReturnValueTypeInfo_Should_Return_Info_For_Generic_Return_Type()
        {
            MonoCecilVersion.GetMethodReturnValueTypeInfo(
                nameof(DerivedClassGenericType.MethodWithGenericReturnType),
                ExperimentalNamespace, nameof(DerivedClassGenericType), out var returnValueNamespace,
                out var returnValueTypeName, out var returnValueAssemblyName, out var isTypeString,
                out var isTypeEnum);
            returnValueNamespace.Should().Be(typeof(string).Namespace);
            returnValueTypeName.Should().Be(nameof(String));
            returnValueAssemblyName.Should().Be(typeof(string).Assembly.GetName().Name);
            isTypeString.Should().BeTrue();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void GetMethodReturnValueTypeInfo_Should_Return_Info_For_Method_With_GenericTypeResult()
        {
            MonoCecilVersion.GetMethodReturnValueTypeInfo(
                nameof(DerivedClassGenericType.MethodReturnsAnotherGeneric),
                ExperimentalNamespace, nameof(DerivedClassGenericType), out var returnValueNamespace,
                out var returnValueTypeName, out var returnValueAssemblyName, out var isTypeString,
                out var isTypeEnum);
            returnValueNamespace.Should().Be(ExperimentalNamespace);
            returnValueTypeName.Should().Be("AnotherGenericType<global::System.String>");
            returnValueAssemblyName.Should().Be(ExperimentalSubjectName);
            isTypeString.Should().BeFalse();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void IsPropertyOrFieldACollection_Should_Return_True_For_List()
        {
            var res = MonoCecilVersion.IsPropertyOrFieldACollection(nameof(DerivedClassGenericType.ListField),
                ExperimentalNamespace, nameof(DerivedClassGenericType));
            res.Should().BeTrue();
        }

        [TestMethod]
        public void IsPropertyOrFieldACollection_Should_Return_True_For_IList()
        {
            var res = MonoCecilVersion.IsPropertyOrFieldACollection(nameof(DerivedClassGenericType.IListField),
                ExperimentalNamespace, nameof(DerivedClassGenericType));
            res.Should().BeTrue();
        }

        [TestMethod]
        public void IsPropertyOrFieldACollection_Should_Return_True_For_Dictionary()
        {
            var res = MonoCecilVersion.IsPropertyOrFieldACollection(nameof(DerivedClassGenericType.DictionaryField),
                ExperimentalNamespace, nameof(DerivedClassGenericType));
            res.Should().BeTrue();
        }

        [TestMethod]
        public void IsPropertyOrFieldACollection_Should_Return_False_For_String()
        {
            var res = MonoCecilVersion.IsPropertyOrFieldACollection(nameof(DerivedClassGenericType.MyField),
                ExperimentalNamespace, nameof(DerivedClassGenericType));
            res.Should().BeFalse();
        }
    }
}
