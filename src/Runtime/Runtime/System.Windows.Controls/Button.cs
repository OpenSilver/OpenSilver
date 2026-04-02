
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
using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a templated button control that interprets a Click user interaction.
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
    public partial class Button : ButtonBase
    {
        static Button()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Button), new PropertyMetadata(typeof(Button)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// </summary>
        public Button() { }

        /// <summary>
        /// Returns a <see cref="ButtonAutomationPeer"/> for use by the Silverlight automation 
        /// infrastructure.
        /// </summary>
        /// <returns>
        /// <see cref="ButtonAutomationPeer"/> for the <see cref="Button"/> object.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            => new ButtonAutomationPeer(this);

        /// <summary>
        /// Identifies the <see cref="IsCancel"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty IsCancelProperty =
            DependencyProperty.Register(
                nameof(IsCancel),
                typeof(bool),
                typeof(Button),
                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets a value that indicates whether a <see cref="Button"/> is a Cancel button. A user 
        /// can activate the Cancel button by pressing the ESC key.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Button"/> is a Cancel button; otherwise, false. The default is false.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool IsCancel
        {
            get => (bool)GetValue(IsCancelProperty);
            set => SetValueInternal(IsCancelProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsDefault"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty IsDefaultProperty =
            DependencyProperty.Register(
                nameof(IsDefault),
                typeof(bool),
                typeof(Button),
                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets a value that indicates whether a <see cref="Button"/> is the default button. A user 
        /// invokes the default button by pressing the ENTER key.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Button"/> is the default button; otherwise, false. The default is false.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool IsDefault
        {
            get => (bool)GetValue(IsDefaultProperty);
            set => SetValueInternal(IsDefaultProperty, value);
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
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty IsDefaultedProperty = IsDefaultedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether a <see cref="Button"/> is the button that is activated 
        /// when a user presses ENTER.
        /// </summary>
        /// <returns>
        /// true if the button is activated when the user presses ENTER; otherwise, false. The default 
        /// is false.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool IsDefaulted => (bool)GetValue(IsDefaultedProperty);
    }
}
