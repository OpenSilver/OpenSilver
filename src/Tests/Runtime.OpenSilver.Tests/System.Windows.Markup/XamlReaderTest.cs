
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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xaml;

namespace System.Windows.Markup.Tests;

[TestClass]
public class XamlReaderTest
{
    [TestMethod]
    public void Parse_Should_Resolve_Namespace_Declared_On_Ancestor()
    {
        var grid = (Grid)XamlReader.Parse(
            """
            <Grid xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                  xmlns:local="clr-namespace:System.Windows.Shapes;assembly=OpenSilver">
                <Border>
                    <Border.Resources>
                        <Style x:Key="style" TargetType="local:Rectangle" />
                        <x:Type x:Key="type" TypeName="local:Ellipse" />
                    </Border.Resources>
                </Border>
            </Grid>
            """);

        var border = (Border)grid.Children[0];

        var type = Assert.IsInstanceOfType<Type>(border.Resources["type"]);
        Assert.AreEqual(typeof(Rectangle), GetStyleTargetType(border));
        Assert.AreEqual(typeof(Ellipse), type);
    }

    [TestMethod]
    public void Parse_Nested_Namespace_Declaration_Should_Shadow_Outer_Declaration()
    {
        var grid = (Grid)XamlReader.Parse(
            """
            <Grid xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                  xmlns:local="clr-namespace:System.Windows.Controls;assembly=OpenSilver">
                <Border xmlns:local="clr-namespace:System.Windows.Shapes;assembly=OpenSilver">
                    <Border.Resources>
                        <Style x:Key="style" TargetType="local:Rectangle" />
                    </Border.Resources>
                </Border>
            </Grid>
            """);

        Assert.AreEqual(typeof(Rectangle), GetStyleTargetType((Border)grid.Children[0]));
    }

    [TestMethod]
    public void Parse_Namespace_Declaration_Should_Not_Leak_To_Siblings()
    {
        Assert.ThrowsExactly<XamlObjectWriterException>(() => XamlReader.Parse(
            """
            <Grid xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Border xmlns:local="clr-namespace:System.Windows.Shapes;assembly=OpenSilver" />
                <Border>
                    <Border.Resources>
                        <Style x:Key="style" TargetType="local:Rectangle" />
                    </Border.Resources>
                </Border>
            </Grid>
            """));
    }

    [TestMethod]
    public void Parse_Namespace_Declared_On_First_Item_Of_Implicit_Collection()
    {
        var border = (Border)XamlReader.Parse(
            """
            <Border xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Border.Resources>
                    <Style xmlns:local="clr-namespace:System.Windows.Shapes;assembly=OpenSilver"
                           x:Key="style"
                           TargetType="local:Rectangle" />
                </Border.Resources>
            </Border>
            """);

        Assert.AreEqual(typeof(Rectangle), GetStyleTargetType(border));
    }

    [TestMethod]
    public void LoadContent_Should_Use_Namespaces_Declared_Outside_Template()
    {
        var grid = (Grid)XamlReader.Parse(
            """
            <Grid xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                  xmlns:local="clr-namespace:System.Windows.Shapes;assembly=OpenSilver">
                <Grid.Resources>
                    <DataTemplate x:Key="template">
                        <Border>
                            <Border.Resources>
                                <Style x:Key="style" TargetType="local:Rectangle" />
                            </Border.Resources>
                        </Border>
                    </DataTemplate>
                </Grid.Resources>
            </Grid>
            """);

        var template = (DataTemplate)grid.Resources["template"];
        var border = (Border)template.LoadContent();

        Assert.AreEqual(typeof(Rectangle), GetStyleTargetType(border));
        Assert.AreEqual(typeof(Rectangle), GetStyleTargetType(border));
    }

    [TestMethod]
    public void LoadContent_Namespace_Declared_In_Template_Should_Shadow_Outer_Declaration()
    {
        var grid = (Grid)XamlReader.Parse(
            """
            <Grid xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                  xmlns:local="clr-namespace:System.Windows.Controls;assembly=OpenSilver">
                <Grid.Resources>
                    <DataTemplate x:Key="template">
                        <Border xmlns:local="clr-namespace:System.Windows.Shapes;assembly=OpenSilver">
                            <Border.Resources>
                                <Style x:Key="style" TargetType="local:Rectangle" />
                            </Border.Resources>
                        </Border>
                    </DataTemplate>
                </Grid.Resources>
            </Grid>
            """);

        var border = (Border)((DataTemplate)grid.Resources["template"]).LoadContent();

        Assert.AreEqual(typeof(Rectangle), GetStyleTargetType(border));
    }

    [TestMethod]
    public void LoadContent_Should_Not_See_Namespace_Declared_On_Sibling()
    {
        var grid = (Grid)XamlReader.Parse(
            """
            <Grid xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid.Children>
                    <Border xmlns:local="clr-namespace:System.Windows.Shapes;assembly=OpenSilver" />
                </Grid.Children>
                <Grid.Resources>
                    <DataTemplate x:Key="template">
                        <Border>
                            <Border.Resources>
                                <Style x:Key="style" TargetType="local:Rectangle" />
                            </Border.Resources>
                        </Border>
                    </DataTemplate>
                </Grid.Resources>
            </Grid>
            """);

        var template = (DataTemplate)grid.Resources["template"];

        Assert.ThrowsExactly<XamlObjectWriterException>(template.LoadContent);
    }

    [TestMethod]
    public void LoadContent_Nested_Template_Should_Use_Outer_Namespaces()
    {
        var grid = (Grid)XamlReader.Parse(
            """
            <Grid xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                  xmlns:local="clr-namespace:System.Windows.Shapes;assembly=OpenSilver">
                <Grid.Resources>
                    <DataTemplate x:Key="template">
                        <ContentControl xmlns:inner="clr-namespace:System.Windows.Shapes;assembly=OpenSilver">
                            <ContentControl.ContentTemplate>
                                <DataTemplate>
                                    <Border>
                                        <Border.Resources>
                                            <Style x:Key="style" TargetType="local:Rectangle" />
                                            <Style x:Key="style2" TargetType="inner:Ellipse" />
                                        </Border.Resources>
                                    </Border>
                                </DataTemplate>
                            </ContentControl.ContentTemplate>
                        </ContentControl>
                    </DataTemplate>
                </Grid.Resources>
            </Grid>
            """);

        var contentControl = (ContentControl)((DataTemplate)grid.Resources["template"]).LoadContent();
        var border = (Border)contentControl.ContentTemplate.LoadContent();

        Assert.AreEqual(typeof(Rectangle), GetStyleTargetType(border));
        Assert.AreEqual(typeof(Ellipse), GetStyleTargetType(border, "style2"));
    }

    [TestMethod]
    public void LoadContent_Should_Find_StaticResource_Declared_Outside_Template()
    {
        var grid = (Grid)XamlReader.Parse(
            """
            <Grid xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid.Resources>
                    <SolidColorBrush x:Key="brush" Color="Red" />
                    <DataTemplate x:Key="template">
                        <Border Background="{StaticResource brush}" />
                    </DataTemplate>
                </Grid.Resources>
            </Grid>
            """);

        var border = (Border)((DataTemplate)grid.Resources["template"]).LoadContent();

        var brush = Assert.IsExactInstanceOfType<SolidColorBrush>(grid.Resources["brush"]);
        Assert.AreSame(brush, border.Background);
    }

    [TestMethod]
    public void LoadContent_Should_Find_ThemeResource_Declared_Outside_Template()
    {
        var grid = (Grid)XamlReader.Parse(
            """
            <Grid xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid.Resources>
                    <SolidColorBrush x:Key="brush" Color="Red" />
                    <DataTemplate x:Key="template">
                        <Border Background="{ThemeResource brush}" />
                    </DataTemplate>
                </Grid.Resources>
            </Grid>
            """);

        var border = (Border)((DataTemplate)grid.Resources["template"]).LoadContent();

        var brush = Assert.IsExactInstanceOfType<SolidColorBrush>(grid.Resources["brush"]);
        Assert.AreSame(brush, border.Background);
    }

    [TestMethod]
    public void LoadContent_Should_Find_StaticResource_Declared_In_Style_Resources()
    {
        var grid = (Grid)XamlReader.Parse(
            """
            <Grid xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid.Resources>
                    <Style x:Key="style" TargetType="ContentControl">
                        <Style.Resources>
                            <SolidColorBrush x:Key="brush" Color="Red" />
                        </Style.Resources>
                        <Setter Property="Template">
                            <Setter.Value>
                                <ControlTemplate TargetType="ContentControl">
                                    <Border Background="{StaticResource brush}" />
                                </ControlTemplate>
                            </Setter.Value>
                        </Setter>
                    </Style>
                </Grid.Resources>
            </Grid>
            """);

        var style = (Style)grid.Resources["style"];
        var template = (ControlTemplate)((Setter)style.Setters[0]).Value;
        var border = (Border)template.LoadContent();

        var brush = Assert.IsExactInstanceOfType<SolidColorBrush>(style.Resources["brush"]);
        Assert.AreSame(brush, border.Background);
    }

    [TestMethod]
    public void LoadContent_Nested_Template_Should_Find_StaticResource_Declared_Outside()
    {
        var grid = (Grid)XamlReader.Parse(
            """
            <Grid xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid.Resources>
                    <SolidColorBrush x:Key="brush" Color="Red" />
                    <DataTemplate x:Key="template">
                        <ContentControl>
                            <ContentControl.Resources>
                                <SolidColorBrush x:Key="brush2" Color="Blue" />
                            </ContentControl.Resources>
                            <ContentControl.ContentTemplate>
                                <DataTemplate>
                                    <Border Background="{StaticResource brush}"
                                            BorderBrush="{StaticResource brush2}" />
                                </DataTemplate>
                            </ContentControl.ContentTemplate>
                        </ContentControl>
                    </DataTemplate>
                </Grid.Resources>
            </Grid>
            """);

        var contentControl = (ContentControl)((DataTemplate)grid.Resources["template"]).LoadContent();
        var border = (Border)contentControl.ContentTemplate.LoadContent();

        var brush = Assert.IsExactInstanceOfType<SolidColorBrush>(grid.Resources["brush"]);
        var brush2 = Assert.IsExactInstanceOfType<SolidColorBrush>(contentControl.Resources["brush2"]);
        Assert.AreSame(brush, border.Background);
        Assert.AreSame(brush2, border.BorderBrush);
    }

    [TestMethod]
    public void LoadContent_TypeExtension_Should_Use_Namespaces_Declared_Outside_Template()
    {
        var grid = (Grid)XamlReader.Parse(
            """
            <Grid xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                  xmlns:local="clr-namespace:System.Windows.Shapes;assembly=OpenSilver">
                <Grid.Resources>
                    <DataTemplate x:Key="template">
                        <ContentControl Tag="{x:Type local:Rectangle}" />
                    </DataTemplate>
                </Grid.Resources>
            </Grid>
            """);

        var contentControl = (ContentControl)((DataTemplate)grid.Resources["template"]).LoadContent();

        Assert.AreEqual(typeof(Rectangle), contentControl.Tag);
    }

    private static Type GetStyleTargetType(FrameworkElement element, string key = "style")
        => ((Style)element.Resources[key]).TargetType;
}
