using System;
using System.Collections.Generic;
using System.Text;
#if MIGRATION
using System.Windows;
namespace System.Windows.Controls.Primitives
#else
using Windows.UI.Xaml
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Inactive", GroupName = "ActiveStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "CalendarButtonUnfocused", GroupName = "CalendarButtonFocusStates")]
    [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "CalendarButtonFocused", GroupName = "CalendarButtonFocusStates")]
    [TemplateVisualState(Name = "Unselected", GroupName = "SelectionStates")]
    [TemplateVisualState(Name = "Selected", GroupName = "SelectionStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Active", GroupName = "ActiveStates")]
    public class CalendarButton: Button
    {
    }
}
