
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

using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace System.Windows.Controls;

/// <summary>
/// Represents a button control that displays a hyperlink.
/// </summary>
/// <example>
/// <code lang="XAML" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
/// <StackPanel x:Name="MyStackPanel">
///     <HyperlinkButton Content="here" NavigateUri="http://www.myaddress.com" Foreground="Blue"/>
/// </StackPanel>
/// </code>
/// <code lang="C#">
/// HyperlinkButton hyperlinkButton = new HyperlinkButton() { Content = "here", NavigateUri = new Uri("http://www.myaddress.com"), Foreground = new SolidColorBrush(Windows.UI.Colors.Blue) };
/// MyStackPanel.Children.Add(hyperlinkButton);
/// </code>
/// </example>
[TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
[TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
[TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
[TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
[TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
[TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
public class HyperlinkButton : ButtonBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HyperlinkButton"/> class.
    /// </summary>
    public HyperlinkButton()
    {
        DefaultStyleKey = typeof(HyperlinkButton);
    }

    /// <summary>
    /// Provides handling for the <see cref="ButtonBase.Click"/> event.
    /// </summary>
    protected override void OnClick()
    {
        base.OnClick();

        if (NavigateUri is Uri navigateUri)
        {
            Hyperlink.Navigate(this, navigateUri, TargetName);
        }
    }

    /// <summary>
    /// Returns a <see cref="HyperlinkButtonAutomationPeer"/> for use by the Silverlight automation 
    /// infrastructure.
    /// </summary>
    /// <returns>
    /// A <see cref="HyperlinkButtonAutomationPeer"/> for the hyperlink button object.
    /// </returns>
    protected override AutomationPeer OnCreateAutomationPeer()
        => new HyperlinkButtonAutomationPeer(this);

    /// <summary>
    /// Gets or sets the URI to navigate to when the <see cref="HyperlinkButton"/> is clicked.
    /// </summary>
    public Uri NavigateUri
    {
        get => (Uri)GetValue(NavigateUriProperty);
        set => SetValueInternal(NavigateUriProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="NavigateUri"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NavigateUriProperty =
        DependencyProperty.Register(
            nameof(NavigateUri),
            typeof(Uri),
            typeof(HyperlinkButton),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the name of the target window or frame that the Web page should open in, 
    /// or the name of the object within the Silverlight application to navigate to.
    /// </summary>
    public string TargetName
    {
        get => (string)GetValue(TargetNameProperty);
        set => SetValueInternal(TargetNameProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="TargetName"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TargetNameProperty =
        DependencyProperty.Register(
            nameof(TargetName),
            typeof(string),
            typeof(HyperlinkButton),
            new PropertyMetadata(string.Empty));
}