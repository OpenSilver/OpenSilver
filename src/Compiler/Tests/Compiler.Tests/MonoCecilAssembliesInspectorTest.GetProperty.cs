
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
using Mono.Cecil;
using OpenSilver.Compiler;

namespace Compiler.Tests;

public partial class MonoCecilAssembliesInspectorTest
{
    [TestMethod]
    public void GetProperty_Should_Find_Public_Instance_Property()
    {
        var type = GetClassWithMembersType();

        var property = CreateMonoCecilInspector().FindPropertyDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceProperty),
            MemberFlags.Public | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNotNull(property);
        Assert.IsNotNull(declaringType);
        Assert.IsTrue(property.IsPublic());
        Assert.IsFalse(property.IsStatic());
        Assert.AreEqual(nameof(ClassWithMembers.PublicInstanceProperty), property.Name);
        Assert.AreEqual(nameof(ClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetProperty_Should_Find_Public_Static_Property()
    {
        var type = GetClassWithMembersType();

        var property = CreateMonoCecilInspector().FindPropertyDeep(
            type,
            nameof(ClassWithMembers.PublicStaticProperty),
            MemberFlags.Public | MemberFlags.Static,
            out var declaringType);

        Assert.IsNotNull(property);
        Assert.IsNotNull(declaringType);
        Assert.IsTrue(property.IsPublic());
        Assert.IsTrue(property.IsStatic());
        Assert.AreEqual(nameof(ClassWithMembers.PublicStaticProperty), property.Name);
        Assert.AreEqual(nameof(ClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetProperty_Should_Find_NonPublic_Instance_Property()
    {
        var type = GetClassWithMembersType();

        var property = CreateMonoCecilInspector().FindPropertyDeep(
            type,
            nameof(ClassWithMembers.NonPublicInstanceProperty),
            MemberFlags.NonPublic | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNotNull(property);
        Assert.IsNotNull(declaringType);
        Assert.IsFalse(property.IsPublic());
        Assert.IsFalse(property.IsStatic());
        Assert.AreEqual(nameof(ClassWithMembers.NonPublicInstanceProperty), property.Name);
        Assert.AreEqual(nameof(ClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetProperty_Should_Not_Find_Static_Property_When_Only_Instance_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var property = CreateMonoCecilInspector().FindPropertyDeep(
            type,
            nameof(ClassWithMembers.PublicStaticProperty),
            MemberFlags.Public | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNull(property);
        Assert.IsNull(declaringType);
    }

    [TestMethod]
    public void GetProperty_Should_Not_Find_Public_Property_When_Only_NonPublic_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var property = CreateMonoCecilInspector().FindPropertyDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceProperty),
            MemberFlags.NonPublic | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNull(property);
        Assert.IsNull(declaringType);
    }

    [TestMethod]
    public void GetProperty_Should_Find_Inherited_Public_Property()
    {
        var type = GetClassWithMembersType();

        var property = CreateMonoCecilInspector().FindPropertyDeep(
            type,
            nameof(BaseClassWithMembers.InheritedPublicInstanceProperty),
            MemberFlags.Public | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNotNull(property);
        Assert.IsNotNull(declaringType);
        Assert.IsTrue(property.IsPublic());
        Assert.IsFalse(property.IsStatic());
        Assert.AreEqual(nameof(BaseClassWithMembers.InheritedPublicInstanceProperty), property.Name);
        Assert.AreEqual(nameof(BaseClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetProperty_Should_Match_Name_When_IgnoreCase_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var property = CreateMonoCecilInspector().FindPropertyDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceProperty).ToUpperInvariant(),
            MemberFlags.IgnoreCase | MemberFlags.Public | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNotNull(property);
        Assert.IsNotNull(declaringType);
        Assert.IsTrue(property.IsPublic());
        Assert.IsFalse(property.IsStatic());
        Assert.AreEqual(nameof(ClassWithMembers.PublicInstanceProperty), property.Name);
        Assert.AreEqual(nameof(ClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetProperty_Should_Return_Null_When_Neither_Static_Nor_Instance_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var property = CreateMonoCecilInspector().FindPropertyDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceProperty),
            MemberFlags.Public | MemberFlags.NonPublic,
            out var declaringType);

        Assert.IsNull(property);
        Assert.IsNull(declaringType);
    }

    [TestMethod]
    public void GetProperty_Should_Return_Null_When_Neither_Public_Nor_NonPublic_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var property = CreateMonoCecilInspector().FindPropertyDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceProperty),
            MemberFlags.Static | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNull(property);
        Assert.IsNull(declaringType);
    }

    [TestMethod]
    public void GetProperty_Should_Return_Null_When_Name_Does_Not_Exist()
    {
        var type = GetClassWithMembersType();

        var property = CreateMonoCecilInspector().FindPropertyDeep(
            type,
            "PropertyThatDoesNotExist",
            MemberFlags.Public | MemberFlags.NonPublic | MemberFlags.Static | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNull(property);
        Assert.IsNull(declaringType);
    }
}

file static class Helpers
{
    public static bool IsStatic(this PropertyDefinition property)
    {
        return property.GetMethod is not null && property.GetMethod.IsStatic ||
               property.SetMethod is not null && property.SetMethod.IsStatic;
    }

    public static bool IsPublic(this PropertyDefinition property)
    {
        return property.GetMethod is not null && property.GetMethod.IsPublic ||
               property.SetMethod is not null && property.SetMethod.IsPublic;
    }
}
