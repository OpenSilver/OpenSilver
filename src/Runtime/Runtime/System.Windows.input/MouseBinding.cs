
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

using System.ComponentModel;
using OpenSilver.Internal;

namespace System.Windows.Input;

/// <summary>
/// Binds a <see cref="MouseGesture"/> to a <see cref="RoutedCommand"/> (or another <see cref="ICommand"/> implementation).
/// </summary>
public class MouseBinding : InputBinding
{
    private bool _settingGesture = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseBinding"/> class.
    /// </summary>
    public MouseBinding() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseBinding"/> class, using the specified command and mouse gesture.
    /// </summary>
    /// <param name="command">
    /// The command associated with the gesture.
    /// </param>
    /// <param name="gesture">
    /// The gesture associated with the command.
    /// </param>
    public MouseBinding(ICommand command, MouseGesture gesture)
        : base(command, gesture)
    {
        SynchronizePropertiesFromGesture(gesture);

        // Hooking the handler explicitly becuase base constructor uses _gesture
        // It cannot use Gesture property itself because it is a virtual
        gesture.PropertyChanged += new PropertyChangedEventHandler(OnMouseGesturePropertyChanged);
    }

    /// <summary>
    /// Gets or sets the gesture associated with this <see cref="MouseBinding"/>.
    /// </summary>
    /// <returns>
    /// The gesture.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// the value gesture is not being set to a <see cref="MouseGesture"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Attempted to set this property on a sealed <see cref="MouseBinding"/>.
    /// </exception>
    [TypeConverter(typeof(MouseGestureConverter))]
    public override InputGesture Gesture
    {
        get => base.Gesture;
        set
        {
            if (value is not MouseGesture mouseGesture)
            {
                throw new ArgumentException(string.Format(Strings.InputBinding_ExpectedInputGesture, typeof(MouseGesture)));
            }

            MouseGesture oldMouseGesture = Gesture as MouseGesture;

            base.Gesture = mouseGesture;

            SynchronizePropertiesFromGesture(mouseGesture);
            if (oldMouseGesture != mouseGesture)
            {
                if (oldMouseGesture is not null)
                {
                    oldMouseGesture.PropertyChanged -= new PropertyChangedEventHandler(OnMouseGesturePropertyChanged);
                }

                mouseGesture.PropertyChanged += new PropertyChangedEventHandler(OnMouseGesturePropertyChanged);
            }
        }
    }

    /// <summary>
    /// Identifies the <see cref="MouseAction"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MouseActionProperty =
        DependencyProperty.Register(
            nameof(MouseAction),
            typeof(MouseAction),
            typeof(MouseBinding),
            new UIPropertyMetadata(MouseAction.None, OnMouseActionPropertyChanged));

    /// <summary>
    /// Gets or sets the <see cref="MouseAction"/> associated with this <see cref="MouseBinding"/>.
    /// </summary>
    /// <returns>
    /// The mouse action. The default is <see cref="MouseAction.None"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Attempted to set this property on a sealed <see cref="MouseBinding"/>.
    /// </exception>
    public MouseAction MouseAction
    {
        get => (MouseAction)GetValue(MouseActionProperty);
        set => SetValueInternal(MouseActionProperty, value);
    }

    private static void OnMouseActionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        MouseBinding mouseBinding = (MouseBinding)d;
        mouseBinding.SynchronizeGestureFromProperties((MouseAction)e.NewValue);
    }

    private void SynchronizePropertiesFromGesture(MouseGesture mouseGesture)
    {
        if (_settingGesture)
        {
            return;
        }

        _settingGesture = true;
        try
        {
            MouseAction = mouseGesture.MouseAction;
        }
        finally
        {
            _settingGesture = false;
        }
    }

    /// <summary>
    ///     Synchronized Gesture from properties
    /// </summary>
    private void SynchronizeGestureFromProperties(MouseAction mouseAction)
    {
        if (_settingGesture)
        {
            return;
        }

        _settingGesture = true;
        try
        {
            if (Gesture is null)
            {
                Gesture = new MouseGesture(mouseAction);
            }
            else
            {
                ((MouseGesture)Gesture).MouseAction = mouseAction;
            }
        }
        finally
        {
            _settingGesture = false;
        }
    }

    private void OnMouseGesturePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (string.Equals(e.PropertyName, nameof(MouseAction), StringComparison.Ordinal))
        {
            if (Gesture is MouseGesture mouseGesture)
            {
                SynchronizePropertiesFromGesture(mouseGesture);
            }
        }
    }
}
