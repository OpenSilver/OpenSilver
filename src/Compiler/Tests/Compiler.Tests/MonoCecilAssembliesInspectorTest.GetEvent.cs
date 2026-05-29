
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
    public void GetEvent_Should_Find_Public_Instance_Event()
    {
        var type = GetClassWithMembersType();

        var eventDefinition = CreateMonoCecilInspector().FindEventDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceEvent),
            MemberFlags.Public | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNotNull(eventDefinition);
        Assert.IsNotNull(declaringType);
        Assert.IsTrue(eventDefinition.IsPublic());
        Assert.IsFalse(eventDefinition.IsStatic());
        Assert.AreEqual(nameof(ClassWithMembers.PublicInstanceEvent), eventDefinition.Name);
        Assert.AreEqual(nameof(ClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetEvent_Should_Find_Public_Static_Event()
    {
        var type = GetClassWithMembersType();

        var eventDefinition = CreateMonoCecilInspector().FindEventDeep(
            type,
            nameof(ClassWithMembers.PublicStaticEvent),
            MemberFlags.Public | MemberFlags.Static,
            out var declaringType);

        Assert.IsNotNull(eventDefinition);
        Assert.IsNotNull(declaringType);
        Assert.IsTrue(eventDefinition.IsPublic());
        Assert.IsTrue(eventDefinition.IsStatic());
        Assert.AreEqual(nameof(ClassWithMembers.PublicStaticEvent), eventDefinition.Name);
        Assert.AreEqual(nameof(ClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetEvent_Should_Find_NonPublic_Instance_Event()
    {
        var type = GetClassWithMembersType();

        var eventDefinition = CreateMonoCecilInspector().FindEventDeep(
            type,
            nameof(ClassWithMembers.NonPublicInstanceEvent),
            MemberFlags.NonPublic | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNotNull(eventDefinition);
        Assert.IsNotNull(declaringType);
        Assert.IsFalse(eventDefinition.IsPublic());
        Assert.IsFalse(eventDefinition.IsStatic());
        Assert.AreEqual(nameof(ClassWithMembers.NonPublicInstanceEvent), eventDefinition.Name);
        Assert.AreEqual(nameof(ClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetEvent_Should_Not_Find_Public_Event_When_Only_NonPublic_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var eventDefinition = CreateMonoCecilInspector().FindEventDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceEvent),
            MemberFlags.NonPublic | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNull(eventDefinition);
        Assert.IsNull(declaringType);
    }

    [TestMethod]
    public void GetEvent_Should_Not_Find_Static_Event_When_Only_Instance_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var eventDefinition = CreateMonoCecilInspector().FindEventDeep(
            type,
            nameof(ClassWithMembers.PublicStaticEvent),
            MemberFlags.Public | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNull(eventDefinition);
        Assert.IsNull(declaringType);
    }

    [TestMethod]
    public void GetEvent_Should_Find_Inherited_Public_Event()
    {
        var type = GetClassWithMembersType();

        var eventDefinition = CreateMonoCecilInspector().FindEventDeep(
            type,
            nameof(BaseClassWithMembers.InheritedPublicInstanceEvent),
            MemberFlags.Public | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNotNull(eventDefinition);
        Assert.IsNotNull(declaringType);
        Assert.IsTrue(eventDefinition.IsPublic());
        Assert.IsFalse(eventDefinition.IsStatic());
        Assert.AreEqual(nameof(BaseClassWithMembers.InheritedPublicInstanceEvent), eventDefinition.Name);
        Assert.AreEqual(nameof(BaseClassWithMembers), declaringType.Name);
    }

    [TestMethod]
    public void GetEvent_Should_Return_Null_When_Neither_Static_Nor_Instance_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var eventDefinition = CreateMonoCecilInspector().FindEventDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceEvent),
            MemberFlags.Public | MemberFlags.NonPublic,
            out var declaringType);

        Assert.IsNull(eventDefinition);
        Assert.IsNull(declaringType);
    }

    [TestMethod]
    public void GetEvent_Should_Return_Null_When_Neither_Public_Nor_NonPublic_Flag_Is_Set()
    {
        var type = GetClassWithMembersType();

        var eventDefinition = CreateMonoCecilInspector().FindEventDeep(
            type,
            nameof(ClassWithMembers.PublicInstanceEvent),
            MemberFlags.Static | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNull(eventDefinition);
        Assert.IsNull(declaringType);
    }

    [TestMethod]
    public void GetEvent_Should_Return_Null_When_Name_Does_Not_Exist()
    {
        var type = GetClassWithMembersType();

        var eventDefinition = CreateMonoCecilInspector().FindEventDeep(
            type,
            "EventThatDoesNotExist",
            MemberFlags.Public | MemberFlags.NonPublic | MemberFlags.Static | MemberFlags.Instance,
            out var declaringType);

        Assert.IsNull(eventDefinition);
        Assert.IsNull(declaringType);
    }
}

file static class Helpers
{
    public static bool IsStatic(this EventDefinition eventDefinition)
    {
        return eventDefinition.AddMethod is not null && eventDefinition.AddMethod.IsStatic ||
               eventDefinition.RemoveMethod is not null && eventDefinition.RemoveMethod.IsStatic;
    }

    public static bool IsPublic(this EventDefinition eventDefinition)
    {
        return eventDefinition.AddMethod is not null && eventDefinition.AddMethod.IsPublic ||
               eventDefinition.RemoveMethod is not null && eventDefinition.RemoveMethod.IsPublic;
    }
}