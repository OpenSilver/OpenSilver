
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
    public void GetMethod_Should_Find_Public_Instance_Method()
    {
        var type = GetClassWithMembersType();

        var method = MonoCecilAssembliesInspectorImpl.FindMethodDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceMethod),
            MemberFlags.Public | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNotNull(method);
        Assert.IsNotNull(declaringType);
        Assert.IsTrue(method.IsPublic);
        Assert.IsFalse(method.IsStatic);
        Assert.AreEqual(nameof(ClassWithMembers.PublicInstanceMethod), method.Name);
        Assert.AreEqual(nameof(ClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetMethod_Should_Find_Public_Static_Method()
    {
        var type = GetClassWithMembersType();

        var method = MonoCecilAssembliesInspectorImpl.FindMethodDeep(
            type,
            nameof(ClassWithMembers.PublicStaticMethod),
            MemberFlags.Public | MemberFlags.Static,
            out var declaringType);

        Assert.IsNotNull(method);
        Assert.IsNotNull(declaringType);
        Assert.IsTrue(method.IsPublic);
        Assert.IsTrue(method.IsStatic);
        Assert.AreEqual(nameof(ClassWithMembers.PublicStaticMethod), method.Name);
        Assert.AreEqual(nameof(ClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetMethod_Should_Find_NonPublic_Instance_Method()
    {
        var type = GetClassWithMembersType();

        var method = MonoCecilAssembliesInspectorImpl.FindMethodDeep(
            type,
            nameof(ClassWithMembers.NonPublicInstanceMethod),
            MemberFlags.NonPublic | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNotNull(method);
        Assert.IsNotNull(declaringType);
        Assert.IsFalse(method.IsPublic);
        Assert.IsFalse(method.IsStatic);
        Assert.AreEqual(nameof(ClassWithMembers.NonPublicInstanceMethod), method.Name);
        Assert.AreEqual(nameof(ClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetMethod_Should_Not_Find_Static_Method_When_Only_Instance_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var method = MonoCecilAssembliesInspectorImpl.FindMethodDeep(
            type,
            nameof(ClassWithMembers.PublicStaticMethod),
            MemberFlags.Public | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNull(method);
        Assert.IsNull(declaringType);
    }

    [TestMethod]
    public void GetMethod_Should_Not_Find_Public_Method_When_Only_NonPublic_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var method = MonoCecilAssembliesInspectorImpl.FindMethodDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceMethod),
            MemberFlags.NonPublic | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNull(method);
        Assert.IsNull(declaringType);
    }

    [TestMethod]
    public void GetMethod_Should_Find_Inherited_Public_Method()
    {
        var type = GetClassWithMembersType();

        var method = MonoCecilAssembliesInspectorImpl.FindMethodDeep(
            type,
            nameof(BaseClassWithMembers.InheritedPublicInstanceMethod),
            MemberFlags.Public | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNotNull(method);
        Assert.IsNotNull(declaringType);
        Assert.IsTrue(method.IsPublic);
        Assert.IsFalse(method.IsStatic);
        Assert.AreEqual(nameof(BaseClassWithMembers.InheritedPublicInstanceMethod), method.Name);
        Assert.AreEqual(nameof(BaseClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetMethod_Should_Match_Name_When_IgnoreCase_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var method = MonoCecilAssembliesInspectorImpl.FindMethodDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceMethod).ToLowerInvariant(),
            MemberFlags.IgnoreCase | MemberFlags.Public | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNotNull(method);
        Assert.IsNotNull(declaringType);
        Assert.IsTrue(method.IsPublic);
        Assert.IsFalse(method.IsStatic);
        Assert.AreEqual(nameof(ClassWithMembers.PublicInstanceMethod), method.Name);
        Assert.AreEqual(nameof(ClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetMethod_Should_Return_Null_When_Neither_Static_Nor_Instance_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var method = MonoCecilAssembliesInspectorImpl.FindMethodDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceMethod),
            MemberFlags.Public | MemberFlags.NonPublic,
            out var declaringType);

        Assert.IsNull(method);
        Assert.IsNull(declaringType);
    }

    [TestMethod]
    public void GetMethod_Should_Return_Null_When_Neither_Public_Nor_NonPublic_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var method = MonoCecilAssembliesInspectorImpl.FindMethodDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceMethod),
            MemberFlags.Static | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNull(method);
        Assert.IsNull(declaringType);
    }

    [TestMethod]
    public void GetMethod_Should_Return_Null_When_Name_Does_Not_Exist()
    {
        var type = GetClassWithMembersType();

        var method = MonoCecilAssembliesInspectorImpl.FindMethodDeep(
            type,
            "MethodThatDoesNotExist",
            MemberFlags.Public | MemberFlags.NonPublic | MemberFlags.Static | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNull(method);
        Assert.IsNull(declaringType);
    }
}