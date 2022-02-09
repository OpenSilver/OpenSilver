

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
            autoCompleteBox.ItemContainerGenerator.ContainerFromItem("AnItem")
                .As<SelectorItem>()
                .Visibility
                .Should()
                .Be(Visibility.Visible);
            autoCompleteBox.ItemContainerGenerator.ContainerFromItem("OtherItem")
                .As<SelectorItem>()
                .Visibility
                .Should()
                .Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void AutoCompleteBox_DropDownOpening_CallsDelegates()
        {
            AutoCompleteBox autoCompleteBox = new AutoCompleteBox();
            
            int dropDownOpeningCallCount = 0;
            autoCompleteBox.DropDownOpening += (sender, e) => dropDownOpeningCallCount++;
            TestSetup.AttachVisualChild(autoCompleteBox);

            autoCompleteBox.IsDropDownOpen = true;

            dropDownOpeningCallCount.Should()
                .Be(1, "should be called once as dropdown was opened");
        }

        [TestMethod]
        public void AutoCompleteBox_DropDownClosing_CallsDelegates()
        {
            AutoCompleteBox autoCompleteBox = new AutoCompleteBox();
            // IsDropDownOpen needs to initially be different than the value being set
            autoCompleteBox.IsDropDownOpen = true;

            int dropDownClosingCallCount = 0;
            autoCompleteBox.DropDownClosing += (sender, e) => dropDownClosingCallCount++;
            TestSetup.AttachVisualChild(autoCompleteBox);

            autoCompleteBox.IsDropDownOpen = false;

            dropDownClosingCallCount.Should()
                .Be(1, "should be called once as dropdown was closed");
        }

        [TestMethod]
        public void AutoCompleteBox_DropDownOpening_DoesNotCallsDelegatesWithNoValueChange()
        {
            AutoCompleteBox autoCompleteBox = new AutoCompleteBox();
            autoCompleteBox.IsDropDownOpen = true;

            int dropDownOpeningCallCount = 0;
            autoCompleteBox.DropDownOpening += (sender, e) => dropDownOpeningCallCount++;
            TestSetup.AttachVisualChild(autoCompleteBox);

            autoCompleteBox.IsDropDownOpen = true;

            dropDownOpeningCallCount.Should()
                .Be(0, "should not be called since dropdown was already open");
        }
    }
}
