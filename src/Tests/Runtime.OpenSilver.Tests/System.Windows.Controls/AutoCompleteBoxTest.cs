

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
using Runtime.OpenSilver.Tests;
using System.Collections.Generic;
using System.Linq;

#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.System;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
#endif

#if MIGRATION
namespace System.Windows.Controls.Tests
#else
namespace Windows.UI.Xaml.Controls.Tests
#endif
{
    [TestClass]
    public class AutoCompleteBoxTest
    {
        [TestMethod]
        public void AutoCompleteBox_TextChanged_ShouldFilterDropDown()
        {
            IList<string> items = new List<string>
            {
                "AnItem",
                "OtherItem"
            };

            AutoCompleteBox autoCompleteBox = new AutoCompleteBox();
            autoCompleteBox.ItemsSource = items;

            TestSetup.AttachVisualChild(autoCompleteBox);
            (autoCompleteBox.GetTemplateChild("Text") as TextBox).Text = "an";

            TestSetup.SleepWhile(() => !autoCompleteBox.IsDropDownOpen,
                "DropDown is not open after text change");
            autoCompleteBox.ItemsHost.Children
                .Cast<SelectorItem>()
                .First(c => c.Content.ToString() == "AnItem")
                .Visibility
                .Should()
                .Be(Visibility.Visible);
            autoCompleteBox.ItemsHost.Children
                .Cast<SelectorItem>()
                .First(c => c.Content.ToString() == "OtherItem")
                .Visibility
                .Should()
                .Be(Visibility.Collapsed);
        }
    }
}
