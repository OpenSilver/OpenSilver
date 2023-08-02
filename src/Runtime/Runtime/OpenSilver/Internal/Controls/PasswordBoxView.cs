
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
using CSHTML5.Internal;

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endif

namespace OpenSilver.Internal.Controls;

internal sealed class PasswordBoxView : TextViewBase<PasswordBox>
{
    internal PasswordBoxView(PasswordBox host)
        : base(host)
    {
    }

    public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren) =>
        AddPasswordInputDomElement(parentRef, out domElementWhereToPlaceChildren);

    protected internal override void INTERNAL_OnAttachedToVisualTree()
    {
        base.INTERNAL_OnAttachedToVisualTree();

        SetPasswordNative(Host.Password);

        if (FocusManager.GetFocusedElement() == Host)
        {
            InputManager.SetFocusNative(InputDiv);
        }
    }

    protected sealed override void OnInput()
    {
        Host.UpdatePasswordProperty(GetPassword());
        InvalidateMeasure();
    }

    protected sealed override Size MeasureContent(Size constraint)
    {
        int pwdLength = Host.Password.Length;

        return INTERNAL_ParentWindow.TextMeasurementService.MeasureText(
            ((INTERNAL_HtmlDomElementReference)INTERNAL_OuterDomElement).UniqueIdentifier,
            "pre",
            string.Empty,
            constraint.Width,
            pwdLength > 0 ? new string('•', pwdLength) : "M");
    }

    internal void SelectNative()
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && InputDiv is not null)
        {
            string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(InputDiv);
            Interop.ExecuteJavaScriptVoid($"{sElement}.select();");
        }
    }

    internal void OnMaxLengthChanged(int maxLength)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && InputDiv is not null)
        {
            INTERNAL_HtmlDomManager.SetDomElementProperty(InputDiv, "maxLength", maxLength);
        }
    }

    internal void SetPasswordNative(string text)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && InputDiv is not null)
        {
            string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(InputDiv);
            Interop.ExecuteJavaScriptVoid(
                $"{sElement}.value = \"{INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(text)}\";");

            InvalidateMeasure();
        }
    }

    private object AddPasswordInputDomElement(object parentRef, out object domElementWhereToPlaceChildren)
    {
        var passwordFieldStyle = INTERNAL_HtmlDomManager.CreateDomLayoutElementAppendItAndGetStyle(
            "input", parentRef, this, out object passwordField);

        domElementWhereToPlaceChildren = passwordField; // Note: this value is used by the Padding_Changed method to set the padding of the PasswordBox.

        passwordFieldStyle.border = "transparent"; // This removes the border. We do not need it since we are templated
        passwordFieldStyle.outline = "none";
        passwordFieldStyle.backgroundColor = "transparent";
        passwordFieldStyle.fontFamily = "inherit"; // Not inherited by default for "input" DOM elements
        passwordFieldStyle.fontSize = "inherit"; // Not inherited by default for "input" DOM elements
        passwordFieldStyle.color = "inherit"; //This is to inherit the foreground value from parent div.
        passwordFieldStyle.letterSpacing = "inherit"; // Not inherited by default for "input" DOM elements
        passwordFieldStyle.width = "100%";
        passwordFieldStyle.height = "100%";
        passwordFieldStyle.padding = "0px";

        INTERNAL_HtmlDomManager.SetDomElementAttribute(passwordField, "type", "password");

        // disable native tab navigation
        INTERNAL_HtmlDomManager.SetDomElementAttribute(passwordField, "tabindex", "-1");

        return passwordField;
    }

    private string GetPassword()
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && InputDiv is not null)
        {
            string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(InputDiv);
            return Interop.ExecuteJavaScriptString($"{sElement}.value;") ?? string.Empty;
        }

        return string.Empty;
    }
}