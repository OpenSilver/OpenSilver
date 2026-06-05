
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

using OpenSilver.Internal;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace System.Windows.Controls;

/// <summary>
/// Represents a Windows button control, which reacts to the <see cref="ButtonBase.Click"/> event.
/// </summary>
/// <example>
/// <code lang="XAML">
/// <Button Content="Click me" Margin="0,5,0,0" Foreground="White" Background="#FFE44D26" HorizontalAlignment="Left" Click="MyButton_Click"/>
/// </code>
/// <code lang="C#">
/// void MyButton_Click(object sender, RoutedEventArgs e)
/// {
///     MessageBox.Show("You clicked me.");
///     Window.Current.IsEnabled = false;
/// }
/// </code>
/// </example>
public class Button : ButtonBase
{
    internal const string DefaultAccessKey = "\x000D";
    internal const string CancelAccessKey = "\x001B";

    private static readonly UncommonField<KeyboardFocusChangedEventHandler> FocusChangedEventHandlerField = new();

    static Button()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Button), new PropertyMetadata(typeof(Button)));
        IsEnabledProperty.OverrideMetadata(typeof(Button), new PropertyMetadata(OnIsEnabledChanged));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Button"/> class.
    /// </summary>
    public Button() { }

    /// <summary>
    /// Identifies the <see cref="IsCancel"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsCancelProperty =
        DependencyProperty.Register(
            nameof(IsCancel),
            typeof(bool),
            typeof(Button),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, OnIsCancelChanged));

    /// <summary>
    /// Gets or sets a value that indicates whether a <see cref="Button"/> is a Cancel button. A user 
    /// can activate the Cancel button by pressing the ESC key.
    /// </summary>
    /// <returns>
    /// true if the <see cref="Button"/> is a Cancel button; otherwise, false. The default is false.
    /// </returns>
    public bool IsCancel
    {
        get => (bool)GetValue(IsCancelProperty);
        set => SetValueInternal(IsCancelProperty, value);
    }

    private static void OnIsCancelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var b = (Button)d;
        if ((bool)e.NewValue)
        {
            AccessKeyManager.Register(CancelAccessKey, b);
        }
        else
        {
            AccessKeyManager.Unregister(CancelAccessKey, b);
        }
    }

    /// <summary>
    /// Identifies the <see cref="IsDefault"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsDefaultProperty =
        DependencyProperty.Register(
            nameof(IsDefault),
            typeof(bool),
            typeof(Button),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, OnIsDefaultChanged));

    /// <summary>
    /// Gets or sets a value that indicates whether a <see cref="Button"/> is the default button. A user 
    /// invokes the default button by pressing the ENTER key.
    /// </summary>
    /// <returns>
    /// true if the <see cref="Button"/> is the default button; otherwise, false. The default is false.
    /// </returns>
    public bool IsDefault
    {
        get => (bool)GetValue(IsDefaultProperty);
        set => SetValueInternal(IsDefaultProperty, value);
    }

    private static void OnIsDefaultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var b = (Button)d;
        KeyboardFocusChangedEventHandler focusChangedEventHandler = FocusChangedEventHandlerField.GetValue(b);
        if (focusChangedEventHandler is null)
        {
            focusChangedEventHandler = new KeyboardFocusChangedEventHandler(b.OnFocusChanged);
            FocusChangedEventHandlerField.SetValue(b, focusChangedEventHandler);
        }

        if ((bool)e.NewValue)
        {
            AccessKeyManager.Register(DefaultAccessKey, b);
            KeyboardNavigation.Current.FocusChanged += focusChangedEventHandler;
            b.UpdateIsDefaulted(Keyboard.FocusedElement);
        }
        else
        {
            AccessKeyManager.Unregister(DefaultAccessKey, b);
            KeyboardNavigation.Current.FocusChanged -= focusChangedEventHandler;
            b.UpdateIsDefaulted(null);
        }
    }

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // This value is cached in FE, so all we have to do here is look at the new value
        var b = (Button)d;

        // If it's not a default button we don't need to update the IsDefaulted property
        if (b.IsDefault)
        {
            b.UpdateIsDefaulted(Keyboard.FocusedElement);
        }
    }

    private static readonly DependencyPropertyKey IsDefaultedPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(IsDefaulted),
            typeof(bool),
            typeof(Button),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Identifies the <see cref="IsDefaulted"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsDefaultedProperty = IsDefaultedPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value that indicates whether a <see cref="Button"/> is the button that is activated 
    /// when a user presses ENTER.
    /// </summary>
    /// <returns>
    /// true if the button is activated when the user presses ENTER; otherwise, false. The default 
    /// is false.
    /// </returns>
    public bool IsDefaulted => (bool)GetValue(IsDefaultedProperty);

    /// <summary>
    /// Returns a <see cref="ButtonAutomationPeer"/> for use by the Silverlight automation 
    /// infrastructure.
    /// </summary>
    /// <returns>
    /// <see cref="ButtonAutomationPeer"/> for the <see cref="Button"/> object.
    /// </returns>
    protected override AutomationPeer OnCreateAutomationPeer() => new ButtonAutomationPeer(this);

    private void OnFocusChanged(object sender, KeyboardFocusChangedEventArgs e) => UpdateIsDefaulted(Keyboard.FocusedElement);

    private void UpdateIsDefaulted(IInputElement focus)
    {
        // If it's not a default button, or nothing is focused, or it's disabled then it's not defaulted.
        if (!IsDefault || focus is null || !IsEnabled)
        {
            SetValueInternal(IsDefaultedPropertyKey, BooleanBoxes.FalseBox);
            return;
        }

        DependencyObject focusDO = focus as DependencyObject;

        // If the focused thing is not in this scope then IsDefaulted = false
        object isDefaulted = BooleanBoxes.FalseBox;

        try
        {
            // Step 1: Determine the AccessKey scope from currently focused element
            var e = new AccessKeyPressedEventArgs();
            focus.RaiseEvent(e);
            object focusScope = e.Scope;

            // Step 2: Determine the AccessKey scope from this button
            e = new AccessKeyPressedEventArgs();
            RaiseEvent(e);
            object thisScope = e.Scope;

            // Step 3: Compare scopes
            if (thisScope == focusScope && (focusDO is null || !(bool)focusDO.GetValue(KeyboardNavigation.AcceptsReturnProperty)))
            {
                isDefaulted = BooleanBoxes.TrueBox;
            }
        }
        finally
        {
            SetValueInternal(IsDefaultedPropertyKey, isDefaulted);
        }
    }
}
