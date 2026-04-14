
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

using Experimental;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSilver.Compiler;

namespace Compiler.Tests;

public partial class MonoCecilAssembliesInspectorTest
{
    [TestMethod]
    public void GetField_Should_Find_Public_Instance_Field()
    {
        var type = GetClassWithMembersType();

        var field = MonoCecilAssembliesInspectorImpl.FindFieldDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceField),
            MemberFlags.Public | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNotNull(field);
        Assert.IsNotNull(declaringType);
        Assert.IsTrue(field.IsPublic);
        Assert.IsFalse(field.IsStatic);
        Assert.AreEqual(nameof(ClassWithMembers.PublicInstanceField), field.Name);
        Assert.AreEqual(nameof(ClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetField_Should_Find_Public_Static_Field()
    {
        var type = GetClassWithMembersType();

        var field = MonoCecilAssembliesInspectorImpl.FindFieldDeep(
            type,
            nameof(ClassWithMembers.PublicStaticField),
            MemberFlags.Public | MemberFlags.Static,
            out var declaringType);

        Assert.IsNotNull(field);
        Assert.IsNotNull(declaringType);
        Assert.IsTrue(field.IsPublic);
        Assert.IsTrue(field.IsStatic);
        Assert.AreEqual(nameof(ClassWithMembers.PublicStaticField), field.Name);
        Assert.AreEqual(nameof(ClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetField_Should_Find_NonPublic_Instance_Field()
    {
        var type = GetClassWithMembersType();

        var field = MonoCecilAssembliesInspectorImpl.FindFieldDeep(
            type,
            nameof(ClassWithMembers.NonPublicInstanceField),
            MemberFlags.NonPublic | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNotNull(field);
        Assert.IsNotNull(declaringType);
        Assert.IsFalse(field.IsPublic);
        Assert.IsFalse(field.IsStatic);
        Assert.AreEqual(nameof(ClassWithMembers.NonPublicInstanceField), field.Name);
        Assert.AreEqual(nameof(ClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetField_Should_Not_Find_Public_Field_When_Only_NonPublic_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var field = MonoCecilAssembliesInspectorImpl.FindFieldDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceField),
            MemberFlags.NonPublic | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNull(field);
        Assert.IsNull(declaringType);
    }

    [TestMethod]
    public void GetField_Should_Not_Find_Static_Field_When_Only_Instance_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var field = MonoCecilAssembliesInspectorImpl.FindFieldDeep(
            type,
            nameof(ClassWithMembers.PublicStaticField),
            MemberFlags.Public | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNull(field);
        Assert.IsNull(declaringType);
    }

    [TestMethod]
    public void GetField_Should_Find_Inherited_Public_Field()
    {
        var type = GetClassWithMembersType();

        var field = MonoCecilAssembliesInspectorImpl.FindFieldDeep(
            type,
            nameof(BaseClassWithMembers.InheritedPublicInstanceField),
            MemberFlags.Public | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNotNull(field);
        Assert.IsNotNull(declaringType);
        Assert.IsTrue(field.IsPublic);
        Assert.IsFalse(field.IsStatic);
        Assert.AreEqual(nameof(BaseClassWithMembers.InheritedPublicInstanceField), field.Name);
        Assert.AreEqual(nameof(BaseClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetField_Should_Match_Either_Visibility_When_Both_Flags_Are_Set()
    {
        var type = GetClassWithMembersType();

        var publicField = MonoCecilAssembliesInspectorImpl.FindFieldDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceField),
            MemberFlags.Public | MemberFlags.NonPublic | MemberFlags.Instance,
            out var declaringTypePublicField);

        var nonPublicField = MonoCecilAssembliesInspectorImpl.FindFieldDeep(
            type,
            nameof(ClassWithMembers.NonPublicInstanceField),
            MemberFlags.Public | MemberFlags.NonPublic | MemberFlags.Instance,
            out var declaringTypeNonPublicField);

        Assert.IsNotNull(publicField);
        Assert.IsNotNull(declaringTypePublicField);
        Assert.IsTrue(publicField.IsPublic);
        Assert.IsFalse(publicField.IsStatic);
        Assert.AreEqual(nameof(ClassWithMembers.PublicInstanceField), publicField.Name);
        Assert.AreEqual(nameof(ClassWithMembers), declaringTypePublicField.Name);

        Assert.IsNotNull(nonPublicField);
        Assert.IsNotNull(declaringTypeNonPublicField);
        Assert.IsFalse(nonPublicField.IsPublic);
        Assert.IsFalse(nonPublicField.IsStatic);
        Assert.AreEqual(nameof(ClassWithMembers.NonPublicInstanceField), nonPublicField.Name);
        Assert.AreEqual(nameof(ClassWithMembers), declaringTypeNonPublicField.Name);
    }

    [TestMethod]
    public void GetField_Should_Be_Case_Sensitive_By_Default()
    {
        var type = GetClassWithMembersType();

        var field = MonoCecilAssembliesInspectorImpl.FindFieldDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceField).ToLowerInvariant(),
            MemberFlags.Public | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNull(field);
        Assert.IsNull(declaringType);
    }

    [TestMethod]
    public void GetField_Should_Match_Name_When_IgnoreCase_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var field = MonoCecilAssembliesInspectorImpl.FindFieldDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceField).ToLowerInvariant(),
            MemberFlags.IgnoreCase | MemberFlags.Public | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNotNull(field);
        Assert.IsNotNull(declaringType);
        Assert.IsTrue(field.IsPublic);
        Assert.IsFalse(field.IsStatic);
        Assert.AreEqual(nameof(ClassWithMembers.PublicInstanceField), field.Name);
        Assert.AreEqual(nameof(ClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetField_Should_Return_Null_When_Neither_Static_Nor_Instance_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var field = MonoCecilAssembliesInspectorImpl.FindFieldDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceField),
            MemberFlags.Public,
            out var declaringType);

        Assert.IsNull(field);
        Assert.IsNull(declaringType);
    }

    [TestMethod]
    public void GetField_Should_Return_Null_When_Neither_Public_Nor_NonPublic_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var field = MonoCecilAssembliesInspectorImpl.FindFieldDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceField),
            MemberFlags.Instance,
            out var declaringType);

        Assert.IsNull(field);
        Assert.IsNull(declaringType);
    }

    [TestMethod]
    public void GetField_Should_Return_Null_When_Name_Does_Not_Exist()
    {
        var type = GetClassWithMembersType();

        var field = MonoCecilAssembliesInspectorImpl.FindFieldDeep(
            type,
            "FieldThatDoesNotExist",
            MemberFlags.Public | MemberFlags.NonPublic | MemberFlags.Static | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNull(field);
        Assert.IsNull(declaringType);
    }
}
