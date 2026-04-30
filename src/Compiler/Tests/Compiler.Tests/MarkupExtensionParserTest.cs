
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
using OpenSilver.Compiler;

namespace Compiler.Tests;

[TestClass]
public class MarkupExtensionParserTest
{
    [TestMethod]
    public void Parse_NameOnly()
    {
        var d = Parse("{Binding}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(0, d.ConstructorArguments);
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_NameOnly_WithLeadingWhitespace()
    {
        var d = Parse("{ Binding}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(0, d.ConstructorArguments);
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_NameOnly_WithTrailingWhitespace()
    {
        var d = Parse("{Binding }");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(0, d.ConstructorArguments);
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_NameOnly_WithSurroundingWhitespace()
    {
        var d = Parse("{ Binding }");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(0, d.ConstructorArguments);
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_NameOnly_WithMultipleSpaces()
    {
        var d = Parse("{   Binding   }");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(0, d.ConstructorArguments);
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_NameOnly_WithNamespacePrefix()
    {
        var d = Parse("{x:Null}");
        Assert.AreEqual("x:Null", d.Name);
        Assert.HasCount(0, d.ConstructorArguments);
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_NameOnly_StaticResource()
    {
        var d = Parse("{StaticResource}");
        Assert.AreEqual("StaticResource", d.Name);
        Assert.HasCount(0, d.ConstructorArguments);
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_NameOnly_TemplateBinding()
    {
        var d = Parse("{TemplateBinding}");
        Assert.AreEqual("TemplateBinding", d.Name);
        Assert.HasCount(0, d.ConstructorArguments);
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_ContentProperty_Simple()
    {
        var d = Parse("{StaticResource MyKey}");
        Assert.AreEqual("StaticResource", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("MyKey", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_ContentProperty_WithNamespacePrefix()
    {
        var d = Parse("{x:Static Member}");
        Assert.AreEqual("x:Static", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("Member", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_ContentProperty_TemplateBinding()
    {
        var d = Parse("{TemplateBinding Property}");
        Assert.AreEqual("TemplateBinding", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("Property", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_ContentProperty_RelativeSource()
    {
        var d = Parse("{RelativeSource Self}");
        Assert.AreEqual("RelativeSource", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("Self", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_ContentProperty_Quoted()
    {
        var d = Parse("{StaticResource 'My Key'}");
        Assert.AreEqual("StaticResource", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("My Key", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_ContentProperty_QuotedWithComma()
    {
        var d = Parse("{StaticResource 'My,Key'}");
        Assert.AreEqual("StaticResource", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("My,Key", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_ContentProperty_NestedExtension()
    {
        var d = Parse("{Binding {StaticResource Key}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.ConstructorArguments[0]);
        Assert.AreEqual("StaticResource", nested.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("Key", Assert.IsExactInstanceOfType<string>(nested.ConstructorArguments[0]));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_ContentProperty_EscapeBraces()
    {
        var d = Parse("{Binding {}literal text}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("literal text", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_ContentProperty_WithTrailingWhitespace()
    {
        var d = Parse("{StaticResource MyKey   }");
        Assert.AreEqual("StaticResource", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("MyKey", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_SingleNamedProperty()
    {
        var d = Parse("{Binding Path=Name}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(0, d.ConstructorArguments);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_TwoNamedProperties()
    {
        var d = Parse("{Binding Path=Name, Mode=TwoWay}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(0, d.ConstructorArguments);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
        Assert.AreEqual("Mode", d.Properties[1].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
    }

    [TestMethod]
    public void Parse_ThreeNamedProperties()
    {
        var d = Parse("{Binding Path=Name, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(0, d.ConstructorArguments);
        Assert.HasCount(3, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
        Assert.AreEqual("Mode", d.Properties[1].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
        Assert.AreEqual("UpdateSourceTrigger", d.Properties[2].Name);
        Assert.AreEqual("PropertyChanged", Assert.IsExactInstanceOfType<string>(d.Properties[2].Value));
    }

    [TestMethod]
    public void Parse_NamedProperty_WithSpacesAroundEquals()
    {
        var d = Parse("{Binding Path = Name}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_NamedProperty_WithQuotedValue()
    {
        var d = Parse("{Binding Path='hello, world'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("hello, world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_NamedProperty_WithQuotedValueContainingBraces()
    {
        var d = Parse("{Binding Path='{}{test}'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("{test}", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_NamedProperty_WithNestedExtensionValue()
    {
        var d = Parse("{Binding Converter={StaticResource MyConverter}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Converter", d.Properties[0].Name);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[0].Value);
        Assert.AreEqual("StaticResource", nested.Name);
        Assert.HasCount(1, nested.ConstructorArguments);
        Assert.AreEqual("MyConverter", Assert.IsExactInstanceOfType<string>(nested.ConstructorArguments[0]));
    }

    [TestMethod]
    public void Parse_NamedProperty_ValueWithTrailingWhitespace()
    {
        var d = Parse("{Binding Path=Name   }");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_NamedProperty_WithEscapeBracesInValue()
    {
        var d = Parse("{Binding Path={}some literal}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("some literal", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_NamedProperty_TrailingComma()
    {
        var d = Parse("{Binding Path=Name,}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_ContentPropertyAndOneNamedProperty()
    {
        var d = Parse("{Binding Name, Mode=TwoWay}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Mode", d.Properties[0].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_ContentPropertyAndTwoNamedProperties()
    {
        var d = Parse("{Binding Name, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("Name", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("Mode", d.Properties[0].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
        Assert.AreEqual("UpdateSourceTrigger", d.Properties[1].Name);
        Assert.AreEqual("PropertyChanged", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
    }

    [TestMethod]
    public void Parse_QuotedContentPropertyAndNamedProperty()
    {
        var d = Parse("{Binding 'My Path', Mode=TwoWay}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("My Path", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Mode", d.Properties[0].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_NestedContentPropertyAndNamedProperty()
    {
        var d = Parse("{Binding {StaticResource Key}, Mode=TwoWay}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.ConstructorArguments[0]);
        Assert.AreEqual("StaticResource", nested.Name);
        Assert.HasCount(1, nested.ConstructorArguments);
        Assert.AreEqual("Key", Assert.IsExactInstanceOfType<string>(nested.ConstructorArguments[0]));
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Mode", d.Properties[0].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_NestedExtensionInProperty()
    {
        var d = Parse("{Binding Path=DataContext, RelativeSource={RelativeSource Self}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("DataContext", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
        Assert.AreEqual("RelativeSource", d.Properties[1].Name);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[1].Value);
        Assert.AreEqual("RelativeSource", nested.Name);
        Assert.HasCount(1, nested.ConstructorArguments);
        Assert.AreEqual("Self", Assert.IsExactInstanceOfType<string>(nested.ConstructorArguments[0]));
    }

    [TestMethod]
    public void Parse_DeeplyNestedExtensions()
    {
        var d = Parse("{Binding RelativeSource={RelativeSource AncestorType={x:Type Window}}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("RelativeSource", d.Properties[0].Name);
        var rs = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[0].Value);
        Assert.AreEqual("RelativeSource", rs.Name);
        Assert.HasCount(1, rs.Properties);
        Assert.AreEqual("AncestorType", rs.Properties[0].Name);
        var xType = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(rs.Properties[0].Value);
        Assert.AreEqual("x:Type", xType.Name);
        Assert.HasCount(1, xType.ConstructorArguments);
        Assert.AreEqual("Window", Assert.IsExactInstanceOfType<string>(xType.ConstructorArguments[0]));
    }

    [TestMethod]
    public void Parse_MultiplePropertiesWithNestedExtension()
    {
        var d = Parse("{Binding Path=Text, Converter={StaticResource MyConverter}, Mode=OneWay}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(3, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Text", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
        Assert.AreEqual("Converter", d.Properties[1].Name);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[1].Value);
        Assert.AreEqual("StaticResource", nested.Name);
        Assert.HasCount(1, nested.ConstructorArguments);
        Assert.AreEqual("MyConverter", Assert.IsExactInstanceOfType<string>(nested.ConstructorArguments[0]));
        Assert.AreEqual("Mode", d.Properties[2].Name);
        Assert.AreEqual("OneWay", Assert.IsExactInstanceOfType<string>(d.Properties[2].Value));
    }

    [TestMethod]
    public void Parse_NestedExtensionWithQuotedValue()
    {
        var d = Parse("{Binding Converter={StaticResource 'My Converter'}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Converter", d.Properties[0].Name);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[0].Value);
        Assert.AreEqual("StaticResource", nested.Name);
        Assert.HasCount(1, nested.ConstructorArguments);
        Assert.AreEqual("My Converter", Assert.IsExactInstanceOfType<string>(nested.ConstructorArguments[0]));
    }

    [TestMethod]
    public void Parse_TwoNestedExtensions()
    {
        var d = Parse("{Binding Converter={StaticResource Conv}, ConverterParameter={StaticResource Param}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("Converter", d.Properties[0].Name);
        var conv = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[0].Value);
        Assert.AreEqual("StaticResource", conv.Name);
        Assert.HasCount(1, conv.ConstructorArguments);
        Assert.AreEqual("Conv", Assert.IsExactInstanceOfType<string>(conv.ConstructorArguments[0]));
        Assert.AreEqual("ConverterParameter", d.Properties[1].Name);
        var param = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[1].Value);
        Assert.AreEqual("StaticResource", param.Name);
        Assert.HasCount(1, param.ConstructorArguments);
        Assert.AreEqual("Param", Assert.IsExactInstanceOfType<string>(param.ConstructorArguments[0]));
    }

    [TestMethod]
    public void Parse_ComplexNestedWithContentAndProperties()
    {
        var d = Parse("{Binding Path=DataContext, RelativeSource={RelativeSource AncestorType={x:Type Window}}, Mode=TwoWay}");
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
        Assert.HasCount(1, xType.ConstructorArguments);
        Assert.AreEqual("Window", Assert.IsExactInstanceOfType<string>(xType.ConstructorArguments[0]));
        Assert.AreEqual("Mode", d.Properties[2].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[2].Value));
    }

    [TestMethod]
    public void Parse_ValueContainingDots()
    {
        var d = Parse("{Binding Path=Items.Count}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Items.Count", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_ValueContainingBrackets()
    {
        var d = Parse("{Binding Path=Items[0]}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Items[0]", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_ValueContainingParentheses()
    {
        var d = Parse("{Binding Path=(local:MyProp)}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("(local:MyProp)", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_ContentProperty_ValueWithDots()
    {
        var d = Parse("{Binding Items.Count}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("Items.Count", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_ValueContainsCurlyBraces_And_IsQuoted()
    {
        var d = Parse("{Binding Solution.Name, StringFormat='Solution {0}'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("Solution.Name", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("StringFormat", d.Properties[0].Name);
        Assert.AreEqual("Solution {0}", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_ValueContainsCurlyBraces_And_IsNotQuoted()
    {
        var d = Parse("{Binding Solution.Name, StringFormat=Solution {0}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("Solution.Name", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("StringFormat", d.Properties[0].Name);
        Assert.AreEqual("Solution {0}", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_TwoContentProperties()
    {
        var d = Parse("{Binding Foo, Bar}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.ConstructorArguments);
        Assert.AreEqual("Foo", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.AreEqual("Bar", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[1]));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_TwoContentProperties_WithNamedProperty()
    {
        var d = Parse("{Binding Foo, Bar, Mode=TwoWay}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.ConstructorArguments);
        Assert.AreEqual("Foo", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.AreEqual("Bar", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[1]));
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Mode", d.Properties[0].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_Backslash_EscapesCommaInPropertyValue()
    {
        var d = Parse(@"{Binding Path=hello\,world}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("hello,world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_Backslash_EscapesClosingBraceInPropertyValue()
    {
        var d = Parse(@"{Binding Path=hello\}world}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("hello}world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_Backslash_EscapesEqualsSign()
    {
        var d = Parse(@"{Binding hello\=world}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("hello=world", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_Backslash_EscapesQuoteInQuotedString()
    {
        var d = Parse(@"{Binding Path='hello\'world'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("hello'world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_Backslash_DoubleBackslashProducesSingleBackslash()
    {
        var d = Parse(@"{Binding Path=hello\\world}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual(@"hello\world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_Backslash_TripleBackslashProducesBackslashAndEscapesNext()
    {
        var d = Parse(@"{Binding Path=hello\\\,world}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual(@"hello\,world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_Backslash_QuadrupleBackslashProducesTwoBackslashes()
    {
        var d = Parse(@"{Binding Path=hello\\\\world}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual(@"hello\\world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_Backslash_DoubleBackslashBeforeCommaIsDelimiter()
    {
        var d = Parse(@"{Binding Path=hello\\, Mode=TwoWay}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual(@"hello\", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
        Assert.AreEqual("Mode", d.Properties[1].Name);
        Assert.AreEqual("TwoWay", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
    }

    [TestMethod]
    public void Parse_Backslash_EscapesCommaInContentProperty()
    {
        var d = Parse(@"{StaticResource My\,Key}");
        Assert.AreEqual("StaticResource", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("My,Key", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_Backslash_DoubleBackslashInQuotedString()
    {
        var d = Parse(@"{Binding Path='hello\\world'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual(@"hello\world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_Backslash_EscapesOpeningBraceInPropertyValue()
    {
        var d = Parse(@"{Binding Path=\{test}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("{test", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_Backslash_StringFormatWithComma()
    {
        var d = Parse(@"{Binding Path=Amount, StringFormat=\{0:C\}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("Amount", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
        Assert.AreEqual("StringFormat", d.Properties[1].Name);
        Assert.AreEqual("{0:C}", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
    }

    [TestMethod]
    public void Parse_Backslash_StringFormatCurrencyWithText()
    {
        var d = Parse(@"{Binding Path=Price, StringFormat=Total: \{0:C2\}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("StringFormat", d.Properties[1].Name);
        Assert.AreEqual("Total: {0:C2}", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
    }

    [TestMethod]
    public void Parse_Backslash_StringFormatMultiplePlaceholders()
    {
        var d = Parse(@"{Binding Path=Name, StringFormat=\{0\} - \{1\}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("StringFormat", d.Properties[1].Name);
        Assert.AreEqual("{0} - {1}", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
    }

    [TestMethod]
    public void Parse_Backslash_StringFormatWithEscapedCommaInFormat()
    {
        var d = Parse(@"{Binding Path=Value, StringFormat=\{0:N2\}\, items}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("StringFormat", d.Properties[1].Name);
        Assert.AreEqual("{0:N2}, items", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
    }

    [TestMethod]
    public void Parse_Backslash_StringFormatWithBackslashInText()
    {
        var d = Parse(@"{Binding Path=Dir, StringFormat=C:\\Users\\\{0\}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("StringFormat", d.Properties[1].Name);
        Assert.AreEqual(@"C:\Users\{0}", Assert.IsExactInstanceOfType<string>(d.Properties[1].Value));
    }

    [TestMethod]
    public void Parse_Backslash_ContentPropertyWithEscapedBraces()
    {
        var d = Parse(@"{Binding \{0\}}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        Assert.AreEqual("{0}", Assert.IsExactInstanceOfType<string>(d.ConstructorArguments[0]));
        Assert.HasCount(0, d.Properties);
    }

    [TestMethod]
    public void Parse_QuotedNestedExtension()
    {
        var d = Parse("{Binding Path=Value, StringFormat='{0:C}'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(2, d.Properties);
        Assert.AreEqual("StringFormat", d.Properties[1].Name);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[1].Value);
        Assert.AreEqual("0:C", nested.Name);
    }

    [TestMethod]
    public void Parse_QuotedNestedExtensionInContentProperty()
    {
        var d = Parse("{Binding '{StaticResource Key}'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.ConstructorArguments);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.ConstructorArguments[0]);
        Assert.AreEqual("StaticResource", nested.Name);
        Assert.HasCount(1, nested.ConstructorArguments);
        Assert.AreEqual("Key", Assert.IsExactInstanceOfType<string>(nested.ConstructorArguments[0]));
    }

    [TestMethod]
    public void Parse_QuotedEscapeBracesProducesLiteralString()
    {
        var d = Parse("{Binding Path='{}{test}'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("{test}", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_QuotedPlainString()
    {
        var d = Parse("{Binding Path='hello world'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Path", d.Properties[0].Name);
        Assert.AreEqual("hello world", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_QuotedNestedExtensionWithProperties()
    {
        var d = Parse("{Binding Converter='{StaticResource MyConverter}'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("Converter", d.Properties[0].Name);
        var nested = Assert.IsExactInstanceOfType<MarkupExtensionDescriptor>(d.Properties[0].Value);
        Assert.AreEqual("StaticResource", nested.Name);
        Assert.HasCount(1, nested.ConstructorArguments);
        Assert.AreEqual("MyConverter", Assert.IsExactInstanceOfType<string>(nested.ConstructorArguments[0]));
    }

    [TestMethod]
    public void Parse_QuotedEscapeBracesWithBackslash()
    {
        var d = Parse(@"{Binding StringFormat='{}\{0\}'}");
        Assert.AreEqual("Binding", d.Name);
        Assert.HasCount(1, d.Properties);
        Assert.AreEqual("StringFormat", d.Properties[0].Name);
        Assert.AreEqual("{0}", Assert.IsExactInstanceOfType<string>(d.Properties[0].Value));
    }

    [TestMethod]
    public void Parse_EmptyString() => AssertInvalidParse("");

    [TestMethod]
    public void Parse_PlainText() => AssertInvalidParse("Hello");

    [TestMethod]
    public void Parse_PlainTextWithSpaces() => AssertInvalidParse("Hello World");

    [TestMethod]
    public void Parse_EscapeSequence() => AssertInvalidParse("{}");

    [TestMethod]
    public void Parse_EscapeSequenceWithText() => AssertInvalidParse("{}text");

    [TestMethod]
    public void Parse_MissingClosingBrace() => AssertInvalidParse("{Binding");

    [TestMethod]
    public void Parse_MissingClosingBrace_WithProperty() => AssertInvalidParse("{Binding Path=Name");

    [TestMethod]
    public void Parse_ExtraTextAfterClosingBrace() => AssertInvalidParse("{Binding}extra");

    [TestMethod]
    public void Parse_ExtraWhitespaceAfterClosingBrace() => AssertInvalidParse("{Binding} ");

    [TestMethod]
    public void Parse_EmptyBracesWithSpace() => AssertInvalidParse("{ }");

    [TestMethod]
    public void Parse_UnterminatedQuotedValue() => AssertInvalidParse("{Binding Path='unclosed}");

    [TestMethod]
    public void Parse_UnterminatedQuotedValue_NoClosingBrace() => AssertInvalidParse("{Binding Path='unclosed");

    [TestMethod]
    public void Parse_MissingNestedClosingBrace() => AssertInvalidParse("{Binding Path={StaticResource Key}");

    [TestMethod]
    public void Parse_OnlyOpeningBrace() => AssertInvalidParse("{");

    [TestMethod]
    public void Parse_MultipleBracePairs() => AssertInvalidParse("{Binding}{Binding}");

    [TestMethod]
    public void Parse_ContentPropertyAfterNamedProperty() => AssertInvalidParse("{Binding Path=Name, Content}");

    [TestMethod]
    public void Parse_Backslash_DoubleBackslashBeforeClosingBraceIsNotEscaped() => AssertInvalidParse(@"{Binding Path=hello\\}world}");

    private static void AssertInvalidParse(string input)
    {
        Assert.Throws<XamlParseException>(() => Parse(input));
    }

    private static MarkupExtensionDescriptor Parse(string input)
    {
        var parser = new MarkupExtensionParser(new XamlParserContext());
        return parser.Parse(input, 0, 0);
    }
}
