
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSilver.Compiler;

namespace Compiler.Tests;

[TestClass]
public class MarkupExtensionDescriptorTest
{
    [TestMethod]
    public void TryParse_NameOnly()
    {
        var d = AssertValidParse("{Binding}");
        Assert.AreEqual("Binding", d.Name);
        Assert.IsNull(d.ContentProperty);
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_NameOnly_WithLeadingWhitespace()
    {
        var d = AssertValidParse("{ Binding}");
        Assert.AreEqual("Binding", d.Name);
        Assert.IsNull(d.ContentProperty);
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_NameOnly_WithTrailingWhitespace()
    {
        var d = AssertValidParse("{Binding }");
        Assert.AreEqual("Binding", d.Name);
        Assert.IsNull(d.ContentProperty);
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_NameOnly_WithSurroundingWhitespace()
    {
        var d = AssertValidParse("{ Binding }");
        Assert.AreEqual("Binding", d.Name);
        Assert.IsNull(d.ContentProperty);
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_NameOnly_WithMultipleSpaces()
    {
        var d = AssertValidParse("{   Binding   }");
        Assert.AreEqual("Binding", d.Name);
        Assert.IsNull(d.ContentProperty);
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_NameOnly_WithNamespacePrefix()
    {
        var d = AssertValidParse("{x:Null}");
        Assert.AreEqual("x:Null", d.Name);
        Assert.IsNull(d.ContentProperty);
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_NameOnly_StaticResource()
    {
        var d = AssertValidParse("{StaticResource}");
        Assert.AreEqual("StaticResource", d.Name);
        Assert.IsNull(d.ContentProperty);
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_NameOnly_TemplateBinding()
    {
        var d = AssertValidParse("{TemplateBinding}");
        Assert.AreEqual("TemplateBinding", d.Name);
        Assert.IsNull(d.ContentProperty);
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_ContentProperty_Simple()
    {
        var d = AssertValidParse("{StaticResource MyKey}");
        Assert.AreEqual("StaticResource", d.Name);
        Assert.AreEqual("MyKey", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_ContentProperty_WithNamespacePrefix()
    {
        var d = AssertValidParse("{x:Static Member}");
        Assert.AreEqual("x:Static", d.Name);
        Assert.AreEqual("Member", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_ContentProperty_TemplateBinding()
    {
        var d = AssertValidParse("{TemplateBinding Property}");
        Assert.AreEqual("TemplateBinding", d.Name);
        Assert.AreEqual("Property", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_ContentProperty_RelativeSource()
    {
        var d = AssertValidParse("{RelativeSource Self}");
        Assert.AreEqual("RelativeSource", d.Name);
        Assert.AreEqual("Self", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_ContentProperty_Quoted()
    {
        var d = AssertValidParse("{StaticResource 'My Key'}");
        Assert.AreEqual("StaticResource", d.Name);
        Assert.AreEqual("My Key", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_ContentProperty_QuotedWithComma()
    {
        var d = AssertValidParse("{StaticResource 'My,Key'}");
        Assert.AreEqual("StaticResource", d.Name);
        Assert.AreEqual("My,Key", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_ContentProperty_NestedExtension()
    {
        var d = AssertValidParse("{Binding {StaticResource Key}}");
        Assert.AreEqual("Binding", d.Name);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.ContentProperty);
        Assert.AreEqual("StaticResource", nested.Name);
        Assert.AreEqual("Key", Assert.IsExactInstanceOfType<string>(nested.ContentProperty));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_ContentProperty_EscapeBraces()
    {
        var d = AssertValidParse("{Binding {}literal text}");
        Assert.AreEqual("Binding", d.Name);
        Assert.AreEqual("literal text", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_ContentProperty_WithTrailingWhitespace()
    {
        var d = AssertValidParse("{StaticResource MyKey   }");
        Assert.AreEqual("StaticResource", d.Name);
        Assert.AreEqual("MyKey", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_SingleNamedProperty()
    {
        var d = AssertValidParse("{Binding Path=Name}");
        Assert.AreEqual("Binding", d.Name);
        Assert.IsNull(d.ContentProperty);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_TwoNamedProperties()
    {
        var d = AssertValidParse("{Binding Path=Name, Mode=TwoWay}");
        Assert.AreEqual("Binding", d.Name);
        Assert.IsNull(d.ContentProperty);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
        Assert.AreEqual("Mode", d.Properties[1].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
    }

    [TestMethod]
    public void TryParse_ThreeNamedProperties()
    {
        var d = AssertValidParse("{Binding Path=Name, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}");
        Assert.AreEqual("Binding", d.Name);
        Assert.IsNull(d.ContentProperty);
        Assert.HasCount(3, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
        Assert.AreEqual("Mode", d.Properties[1].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
        Assert.AreEqual("UpdateSourceTrigger", d.Properties[2].Name);
        Assert.AreEqual("PropertyChanged", Assert.IsExactInstanceOfType<string>(d.Properties[2].Value));
    }

    [TestMethod]
    public void TryParse_NamedProperty_WithSpacesAroundEquals()
    {
        var d = AssertValidParse("{Binding Path = Name}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_NamedProperty_WithQuotedValue()
    {
        var d = AssertValidParse("{Binding Path='hello, world'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("hello, world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_NamedProperty_WithQuotedValueContainingBraces()
    {
        var d = AssertValidParse("{Binding Path='{}{test}'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("{test}", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_NamedProperty_WithNestedExtensionValue()
    {
        var d = AssertValidParse("{Binding Converter={StaticResource MyConverter}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Converter", d.Properties[0].Name);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[0].Value);
        Assert.AreEqual("StaticResource", nested.Name);
        Assert.AreEqual("MyConverter", Assert.IsExactInstanceOfType<string>(nested.ContentProperty));
    }

    [TestMethod]
    public void TryParse_NamedProperty_ValueWithTrailingWhitespace()
    {
        var d = AssertValidParse("{Binding Path=Name   }");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_NamedProperty_WithEscapeBracesInValue()
    {
        var d = AssertValidParse("{Binding Path={}some literal}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("some literal", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_NamedProperty_TrailingComma()
    {
        var d = AssertValidParse("{Binding Path=Name,}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_NamedProperty_QuotedPropertyName()
    {
        var d = AssertValidParse("{Binding 'Path'=Name}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_NamedProperty_QuotedPropertyNameAndQuotedValue()
    {
        var d = AssertValidParse("{Binding 'Path'='hello, world'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("hello, world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_NamedProperty_QuotedPropertyNameWithNestedValue()
    {
        var d = AssertValidParse("{Binding 'Source'={StaticResource Key}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Source", d.Properties[0].Name);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[0].Value);
        Assert.AreEqual("StaticResource", nested.Name);
        Assert.AreEqual("Key", Assert.IsExactInstanceOfType<string>(nested.ContentProperty));
    }

    [TestMethod]
    public void TryParse_NamedProperty_MultipleQuotedNames()
    {
        var d = AssertValidParse("{Binding 'Path'=Name, 'Mode'=TwoWay}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
        Assert.AreEqual("Mode", d.Properties[1].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
    }

    [TestMethod]
    public void TryParse_ContentPropertyAndQuotedPropertyName()
    {
        var d = AssertValidParse("{Binding MyContent, 'Mode'=TwoWay}");
        Assert.AreEqual("Binding", d.Name);
        Assert.AreEqual("MyContent", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Mode", d.Properties[0].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_ContentPropertyAndOneNamedProperty()
    {
        var d = AssertValidParse("{Binding Name, Mode=TwoWay}");
        Assert.AreEqual("Binding", d.Name);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Mode", d.Properties[0].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_ContentPropertyAndTwoNamedProperties()
    {
        var d = AssertValidParse("{Binding Name, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}");
        Assert.AreEqual("Binding", d.Name);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("Mode", d.Properties[0].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
        Assert.AreEqual("UpdateSourceTrigger", d.Properties[1].Name);
        Assert.AreEqual("PropertyChanged", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
    }

    [TestMethod]
    public void TryParse_ContentPropertyAfterNamedProperty()
    {
        var d = AssertValidParse("{Binding Path=Name, Content}");
        Assert.AreEqual("Binding", d.Name);
        Assert.AreEqual("Content", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_QuotedContentPropertyAndNamedProperty()
    {
        var d = AssertValidParse("{Binding 'My Path', Mode=TwoWay}");
        Assert.AreEqual("Binding", d.Name);
        Assert.AreEqual("My Path", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Mode", d.Properties[0].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_NestedContentPropertyAndNamedProperty()
    {
        var d = AssertValidParse("{Binding {StaticResource Key}, Mode=TwoWay}");
        Assert.AreEqual("Binding", d.Name);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.ContentProperty);
        Assert.AreEqual("StaticResource", nested.Name);
        Assert.AreEqual("Key", Assert.IsExactInstanceOfType<string>(nested.ContentProperty));
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Mode", d.Properties[0].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_NestedExtensionInProperty()
    {
        var d = AssertValidParse("{Binding Path=DataContext, RelativeSource={RelativeSource Self}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("DataContext", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
        Assert.AreEqual("RelativeSource", d.Properties[1].Name);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[1].Value);
        Assert.AreEqual("RelativeSource", nested.Name);
        Assert.AreEqual("Self", Assert.IsExactInstanceOfType<string>(nested.ContentProperty));
    }

    [TestMethod]
    public void TryParse_DeeplyNestedExtensions()
    {
        var d = AssertValidParse("{Binding RelativeSource={RelativeSource AncestorType={x:Type Window}}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("RelativeSource", d.Properties[0].Name);
        var rs = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[0].Value);
        Assert.AreEqual("RelativeSource", rs.Name);
        Assert.HasCount(1, rs.Properties);
        Assert.AreEqual("AncestorType", rs.Properties[0].Name);
        var xType = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(rs.Properties[0].Value);
        Assert.AreEqual("x:Type", xType.Name);
        Assert.AreEqual("Window", Assert.IsExactInstanceOfType<string>(xType.ContentProperty));
    }

    [TestMethod]
    public void TryParse_MultiplePropertiesWithNestedExtension()
    {
        var d = AssertValidParse("{Binding Path=Text, Converter={StaticResource MyConverter}, Mode=OneWay}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(3, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Text", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
        Assert.AreEqual("Converter", d.Properties[1].Name);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[1].Value);
        Assert.AreEqual("StaticResource", nested.Name);
        Assert.AreEqual("MyConverter", Assert.IsExactInstanceOfType<string>(nested.ContentProperty));
        Assert.AreEqual("Mode", d.Properties[2].Name);
        Assert.AreEqual("OneWay", Assert.IsExactInstanceOfType<string>(d.Properties[2].Value));
    }

    [TestMethod]
    public void TryParse_NestedExtensionWithQuotedValue()
    {
        var d = AssertValidParse("{Binding Converter={StaticResource 'My Converter'}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Converter", d.Properties[0].Name);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[0].Value);
        Assert.AreEqual("StaticResource", nested.Name);
        Assert.AreEqual("My Converter", Assert.IsExactInstanceOfType<string>(nested.ContentProperty));
    }

    [TestMethod]
    public void TryParse_TwoNestedExtensions()
    {
        var d = AssertValidParse("{Binding Converter={StaticResource Conv}, ConverterParameter={StaticResource Param}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("Converter", d.Properties[0].Name);
        var conv = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[0].Value);
        Assert.AreEqual("StaticResource", conv.Name);
        Assert.AreEqual("Conv", Assert.IsExactInstanceOfType<string>(conv.ContentProperty));
        Assert.AreEqual("ConverterParameter", d.Properties[1].Name);
        var param = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[1].Value);
        Assert.AreEqual("StaticResource", param.Name);
        Assert.AreEqual("Param", Assert.IsExactInstanceOfType<string>(param.ContentProperty));
    }

    [TestMethod]
    public void TryParse_ComplexNestedWithContentAndProperties()
    {
        var d = AssertValidParse("{Binding Path=DataContext, RelativeSource={RelativeSource AncestorType={x:Type Window}}, Mode=TwoWay}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(3, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("DataContext", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
        Assert.AreEqual("RelativeSource", d.Properties[1].Name);
        var rs = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[1].Value);
        Assert.AreEqual("RelativeSource", rs.Name);
        Assert.HasCount(1, rs.Properties);
        Assert.AreEqual("AncestorType", rs.Properties[0].Name);
        var xType = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(rs.Properties[0].Value);
        Assert.AreEqual("x:Type", xType.Name);
        Assert.AreEqual("Window", Assert.IsExactInstanceOfType<string>(xType.ContentProperty));
        Assert.AreEqual("Mode", d.Properties[2].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[2].Value));
    }

    [TestMethod]
    public void TryParse_ValueContainingDots()
    {
        var d = AssertValidParse("{Binding Path=Items.Count}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Items.Count", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_ValueContainingBrackets()
    {
        var d = AssertValidParse("{Binding Path=Items[0]}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Items[0]", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_ValueContainingParentheses()
    {
        var d = AssertValidParse("{Binding Path=(local:MyProp)}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("(local:MyProp)", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_ContentProperty_ValueWithDots()
    {
        var d = AssertValidParse("{Binding Items.Count}");
        Assert.AreEqual("Binding", d.Name);
        Assert.AreEqual("Items.Count", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_EmptyString() => AssertInvalidParse("");

    [TestMethod]
    public void TryParse_PlainText() => AssertInvalidParse("Hello");

    [TestMethod]
    public void TryParse_PlainTextWithSpaces() => AssertInvalidParse("Hello World");

    [TestMethod]
    public void TryParse_EscapeSequence() => AssertInvalidParse("{}");

    [TestMethod]
    public void TryParse_EscapeSequenceWithText() => AssertInvalidParse("{}text");

    [TestMethod]
    public void TryParse_MissingClosingBrace() => AssertInvalidParse("{Binding");

    [TestMethod]
    public void TryParse_MissingClosingBrace_WithProperty() => AssertInvalidParse("{Binding Path=Name");

    [TestMethod]
    public void TryParse_ExtraTextAfterClosingBrace() => AssertInvalidParse("{Binding}extra");

    [TestMethod]
    public void TryParse_ExtraWhitespaceAfterClosingBrace() => AssertInvalidParse("{Binding} ");

    [TestMethod]
    public void TryParse_EmptyBracesWithSpace() => AssertInvalidParse("{ }");

    [TestMethod]
    public void TryParse_TwoContentProperties() => AssertInvalidParse("{Binding Foo, Bar}");

    [TestMethod]
    public void TryParse_TwoContentProperties_WithNamedProperty() => AssertInvalidParse("{Binding Foo, Bar, Mode=TwoWay}");

    [TestMethod]
    public void TryParse_UnterminatedQuotedValue() => AssertInvalidParse("{Binding Path='unclosed}");

    [TestMethod]
    public void TryParse_UnterminatedQuotedValue_NoClosingBrace() => AssertInvalidParse("{Binding Path='unclosed");

    [TestMethod]
    public void TryParse_MissingNestedClosingBrace() => AssertInvalidParse("{Binding Path={StaticResource Key}");

    [TestMethod]
    public void TryParse_OnlyOpeningBrace() => AssertInvalidParse("{");

    [TestMethod]
    public void TryParse_MultipleBracePairs() => AssertInvalidParse("{Binding}{Binding}");

    [TestMethod]
    public void TryParse_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => MarkupExtensionDescriptor.TryParse(null, out _));
    }

    [TestMethod]
    public void TryParse_Backslash_EscapesCommaInPropertyValue()
    {
        var d = AssertValidParse(@"{Binding Path=hello\,world}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("hello,world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_Backslash_EscapesClosingBraceInPropertyValue()
    {
        var d = AssertValidParse(@"{Binding Path=hello\}world}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("hello}world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_Backslash_EscapesEqualsSign()
    {
        var d = AssertValidParse(@"{Binding hello\=world}");
        Assert.AreEqual("Binding", d.Name);
        Assert.AreEqual("hello=world", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_Backslash_EscapesQuoteInQuotedString()
    {
        var d = AssertValidParse(@"{Binding Path='hello\'world'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("hello'world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_Backslash_DoubleBackslashProducesSingleBackslash()
    {
        var d = AssertValidParse(@"{Binding Path=hello\\world}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual(@"hello\world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_Backslash_TripleBackslashProducesBackslashAndEscapesNext()
    {
        var d = AssertValidParse(@"{Binding Path=hello\\\,world}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual(@"hello\,world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_Backslash_QuadrupleBackslashProducesTwoBackslashes()
    {
        var d = AssertValidParse(@"{Binding Path=hello\\\\world}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual(@"hello\\world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_Backslash_DoubleBackslashBeforeClosingBraceIsNotEscaped()
    {
        AssertInvalidParse(@"{Binding Path=hello\\}world}");
    }

    [TestMethod]
    public void TryParse_Backslash_DoubleBackslashBeforeCommaIsDelimiter()
    {
        var d = AssertValidParse(@"{Binding Path=hello\\, Mode=TwoWay}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual(@"hello\", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
        Assert.AreEqual("Mode", d.Properties[1].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
    }

    [TestMethod]
    public void TryParse_Backslash_EscapesCommaInContentProperty()
    {
        var d = AssertValidParse(@"{StaticResource My\,Key}");
        Assert.AreEqual("StaticResource", d.Name);
        Assert.AreEqual("My,Key", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_Backslash_DoubleBackslashInQuotedString()
    {
        var d = AssertValidParse(@"{Binding Path='hello\\world'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual(@"hello\world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_Backslash_EscapesOpeningBraceInPropertyValue()
    {
        var d = AssertValidParse(@"{Binding Path=\{test}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("{test", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_Backslash_StringFormatWithComma()
    {
        var d = AssertValidParse(@"{Binding Path=Amount, StringFormat=\{0:C\}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Amount", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
        Assert.AreEqual("StringFormat", d.Properties[1].Name);
        Assert.AreEqual("{0:C}", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
    }

    [TestMethod]
    public void TryParse_Backslash_StringFormatCurrencyWithText()
    {
        var d = AssertValidParse(@"{Binding Path=Price, StringFormat=Total: \{0:C2\}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("StringFormat", d.Properties[1].Name);
        Assert.AreEqual("Total: {0:C2}", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
    }

    [TestMethod]
    public void TryParse_Backslash_StringFormatMultiplePlaceholders()
    {
        var d = AssertValidParse(@"{Binding Path=Name, StringFormat=\{0\} - \{1\}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("StringFormat", d.Properties[1].Name);
        Assert.AreEqual("{0} - {1}", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
    }

    [TestMethod]
    public void TryParse_Backslash_StringFormatWithEscapedCommaInFormat()
    {
        var d = AssertValidParse(@"{Binding Path=Value, StringFormat=\{0:N2\}\, items}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("StringFormat", d.Properties[1].Name);
        Assert.AreEqual("{0:N2}, items", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
    }

    [TestMethod]
    public void TryParse_Backslash_StringFormatWithBackslashInText()
    {
        var d = AssertValidParse(@"{Binding Path=Dir, StringFormat=C:\\Users\\\{0\}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("StringFormat", d.Properties[1].Name);
        Assert.AreEqual(@"C:\Users\{0}", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
    }

    [TestMethod]
    public void TryParse_Backslash_ContentPropertyWithEscapedBraces()
    {
        var d = AssertValidParse(@"{Binding \{0\}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.AreEqual("{0}", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_QuotedNestedExtension()
    {
        var d = AssertValidParse("{Binding Path=Value, StringFormat='{0:C}'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("StringFormat", d.Properties[1].Name);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[1].Value);
        Assert.AreEqual("0:C", nested.Name);
    }

    [TestMethod]
    public void TryParse_QuotedNestedExtensionInContentProperty()
    {
        var d = AssertValidParse("{Binding '{StaticResource Key}'}");
        Assert.AreEqual("Binding", d.Name);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.ContentProperty);
        Assert.AreEqual("StaticResource", nested.Name);
        Assert.AreEqual("Key", Assert.IsExactInstanceOfType<string>(nested.ContentProperty));
    }

    [TestMethod]
    public void TryParse_QuotedEscapeBracesProducesLiteralString()
    {
        var d = AssertValidParse("{Binding Path='{}{test}'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("{test}", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_QuotedPlainString()
    {
        var d = AssertValidParse("{Binding Path='hello world'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("hello world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_QuotedNestedExtensionWithProperties()
    {
        var d = AssertValidParse("{Binding Converter='{StaticResource MyConverter}'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Converter", d.Properties[0].Name);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[0].Value);
        Assert.AreEqual("StaticResource", nested.Name);
        Assert.AreEqual("MyConverter", Assert.IsExactInstanceOfType<string>(nested.ContentProperty));
    }

    [TestMethod]
    public void TryParse_QuotedEscapeBracesWithBackslash()
    {
        var d = AssertValidParse(@"{Binding StringFormat='{}\{0\}'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("StringFormat", d.Properties[0].Name);
        Assert.AreEqual("{0}", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_Backslash_EscapedExtensionName()
    {
        var d = AssertValidParse(@"{Stat\icResource Value}");
        Assert.AreEqual("StaticResource", d.Name);
        Assert.AreEqual("Value", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void TryParse_Backslash_EscapedEqualsInPropertyName()
    {
        var d = AssertValidParse(@"{Binding A\=B=Value}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("A=B", d.Properties[0].Name);
        Assert.AreEqual("Value", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_Backslash_EscapedBackslashInPropertyName()
    {
        var d = AssertValidParse(@"{Binding A\\B=Value}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual(@"A\B", d.Properties[0].Name);
        Assert.AreEqual("Value", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void TryParse_Backslash_EscapedCommaInExtensionName()
    {
        var d = AssertValidParse(@"{My\,Extension Value}");
        Assert.AreEqual("My,Extension", d.Name);
        Assert.AreEqual("Value", Assert.IsExactInstanceOfType<string>(d.ContentProperty));
        Assert.HasCount(0, d.Properties);
    }

    private static MarkupExtensionDescriptor AssertValidParse(string input)
    {
        bool result = MarkupExtensionDescriptor.TryParse(input, out var descriptor);
        Assert.IsTrue(result, $"TryParse should return true for \"{input}\"");
        return descriptor;
    }

    private static void AssertInvalidParse(string input)
    {
        bool result = MarkupExtensionDescriptor.TryParse(input, out _);
        Assert.IsFalse(result, $"TryParse should return false for \"{input}\"");
    }
}
