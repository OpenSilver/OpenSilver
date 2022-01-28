using OpenSilver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
#if MIGRATION
using System.Windows;
namespace System.Windows.Controls
#else
using Windows.UI.Xaml;
namespace Windows.UI.Xaml.Controls
#endif
{
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]

    [TemplateVisualState(Name = StateValid, GroupName = GroupDomain)]
    [TemplateVisualState(Name = StateInvalid, GroupName = GroupDomain)]

    [TemplateVisualState(Name = VisualStates.StateEdit, GroupName = VisualStates.GroupInteractionMode)]
    [TemplateVisualState(Name = VisualStates.StateDisplay, GroupName = VisualStates.GroupInteractionMode)]

    [TemplateVisualState(Name = VisualStates.StateValid, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidFocused, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidUnfocused, GroupName = VisualStates.GroupValidation)]

    [TemplatePart(Name = ElementTextName, Type = typeof(TextBox))]
    [TemplatePart(Name = ElementSpinnerName, Type = typeof(Spinner))]
    [StyleTypedProperty(Property = SpinnerStyleName, StyleTargetType = typeof(ButtonSpinner))]
    [ContentProperty("Items")]
    [NotImplemented]
    public class DomainUpDown: UpDownBase
    {
        public const string GroupDomain = "DomainStates";
        public const string StateValid = "ValidDomain";
        public const string StateInvalid = "InvalidDomain";

    }
}
