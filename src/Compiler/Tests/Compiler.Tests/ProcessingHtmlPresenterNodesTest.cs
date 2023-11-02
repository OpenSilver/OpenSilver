
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


using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSilver.Compiler;

namespace Compiler.Tests
{
    [TestClass]
    public class ProcessingHtmlPresenterNodesTest
    {
        [TestMethod]
        public void Process_Should_Accept_Empty_HtmlPresenter()
        {
            var xaml = @"
<sdk:Page x:Class=""XRSharpApplication32.MainPage""
          xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
          xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
          xmlns:sdk=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk""
          xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
          xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
          mc:Ignorable=""d"">
    <StackPanel>
        <native:HtmlPresenter xmlns:native=""using:CSHTML5.Native.Html.Controls""></native:HtmlPresenter>
        <native:HtmlPresenter xmlns:native=""using:CSHTML5.Native.Html.Controls"">
            <span>Test</span>
        </native:HtmlPresenter>
    </StackPanel>
</sdk:Page>
";

            var res = ProcessingHtmlPresenterNodes.Process(xaml);

            res.Should().Be(@"
<sdk:Page x:Class=""XRSharpApplication32.MainPage""
          xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
          xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
          xmlns:sdk=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk""
          xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
          xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
          mc:Ignorable=""d"">
    <StackPanel>
        <native:HtmlPresenter xmlns:native=""using:CSHTML5.Native.Html.Controls""></native:HtmlPresenter>
        <native:HtmlPresenter xmlns:native=""using:CSHTML5.Native.Html.Controls"">
            &lt;span&gt;Test&lt;/span&gt;
        </native:HtmlPresenter>
    </StackPanel>
</sdk:Page>
");
        }
    }
}
