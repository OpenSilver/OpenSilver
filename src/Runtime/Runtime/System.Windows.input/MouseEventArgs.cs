
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
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using CSHTML5.Internal;
using OpenSilver.Internal;

namespace System.Windows.Input;

/// <summary>
/// Provides event data for pointer message events related to specific user interface
/// elements, such as PointerPressed.
/// </summary>
public class MouseEventArgs : InputEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MouseEventArgs"/> class.
    /// </summary>
    public MouseEventArgs() { }

    internal MouseEventArgs(bool isTouchDevice, ModifierKeys keyModifiers, double x, double y)
    {
        IsTouchEvent = isTouchDevice;
        KeyModifiers = keyModifiers;
        _pointerAbsoluteX = x;
        _pointerAbsoluteY = y;
    }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) =>
        ((MouseEventHandler)genericHandler)(genericTarget, this);

    internal double _pointerAbsoluteX;
    internal double _pointerAbsoluteY;

    internal bool IsTouchEvent { get; private set; }

    /// <summary>
    /// Gets or sets a value that marks the routed event as handled, and prevents
    /// most handlers along the event route from handling the same event again.
    /// </summary>
    public new bool Handled
    {
        get => base.Handled;
        set => base.Handled = value;
    }

    /// <summary>
    /// Gets a value that indicates which key modifiers were active at the time that
    /// the pointer event was initiated.
    /// </summary>
    public ModifierKeys KeyModifiers { get; }

    /// <summary>
    /// Gets an object that reports stylus device information, such as the collection
    /// of stylus points associated with the input.
    /// </summary>
    /// <returns>
    /// The stylus device information object.
    /// </returns>
    public StylusDevice StylusDevice => new StylusDevice(this);

    /// <summary>
    /// Gets a reference to a pointer token.
    /// </summary>
    public Pointer Pointer { get; internal set; }

    /// <summary>
    /// Gets the number of times the button was clicked.
    /// </summary>
    public int ClickCount { get; internal set; }

    /// <summary>
    /// Returns the pointer position for this event occurrence, optionally evaluated
    /// against a coordinate origin of a supplied UIElement.
    /// </summary>
    /// <param name="relativeTo">
    /// Any UIElement-derived object that is connected to the same object tree. To
    /// specify the object relative to the overall coordinate system, use a relativeTo value
    /// of null.
    /// </param>
    /// <returns>
    /// A PointerPoint value that represents the pointer point associated with this
    /// event. If null was passed as relativeTo, the coordinates are in the frame
    /// of reference of the overall window. If a non-null relativeTo was passed,
    /// the coordinates are relative to the object referenced by relativeTo.
    /// </returns>
    public Point GetPosition(UIElement relativeTo)
        => GetPosition(new Point(_pointerAbsoluteX, _pointerAbsoluteY), relativeTo);

    internal static Point GetPosition(Point origin, UIElement relativeTo)
    {
        if (relativeTo is Popup popup)
        {
            relativeTo = popup.IsOpen ? popup.Child : null;
        }

        if (relativeTo is null)
        {
            //-----------------------------------
            // Return the absolute pointer coordinates:
            //-----------------------------------
            return origin;
        }
        else if (INTERNAL_VisualTreeManager.IsElementInVisualTree(relativeTo))
        {
            //-----------------------------------
            // Returns the pointer coordinates relative to the "relativeTo" element:
            //-----------------------------------

            Matrix m = relativeTo.InternalTransformToAncestor(null);
            if (m.HasInverse)
            {
                m.Invert();
            }
            return m.Transform(origin);
        }

        return new Point(0.0, 0.0);
    }

    [Obsolete(Helper.ObsoleteMemberMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal void SetPointerAbsolutePosition(object jsEventArg, Window window)
    {
        string sEvent = OpenSilver.Interop.GetVariableStringForJS(jsEventArg);
        IsTouchEvent = OpenSilver.Interop.ExecuteJavaScriptBoolean($"{sEvent}.pointerType === 'touch'", false);
        _pointerAbsoluteX = OpenSilver.Interop.ExecuteJavaScriptDouble($"{sEvent}.pageX", false);
        _pointerAbsoluteY = OpenSilver.Interop.ExecuteJavaScriptDouble($"{sEvent}.pageY", false);

        //---------------------------------------
        // Adjust the absolute coordinates to take into account the fact that the XAML Window is not necessary un the top-left corner of the HTML page:
        //---------------------------------------
        if (window != null)
        {
            // Get the XAML Window root position relative to the page and substracts it
            string sElement = OpenSilver.Interop.GetVariableStringForJS(window.OuterDiv);
            _pointerAbsoluteX -= OpenSilver.Interop.ExecuteJavaScriptDouble(
                $"{sElement}.getBoundingClientRect().left - document.body.getBoundingClientRect().left", false);
            _pointerAbsoluteY -= OpenSilver.Interop.ExecuteJavaScriptDouble(
                $"{sElement}.getBoundingClientRect().top - document.body.getBoundingClientRect().top", false);
        }
    }
}