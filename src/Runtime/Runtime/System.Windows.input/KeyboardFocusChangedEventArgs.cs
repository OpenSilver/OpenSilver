using System.Windows;

namespace System.Windows.Input;

public delegate void KeyboardFocusChangedEventHandler(object sender, KeyboardFocusChangedEventArgs e);

public class KeyboardFocusChangedEventArgs : RoutedEventArgs
{
    public KeyboardFocusChangedEventArgs()
    {
    }

    public KeyboardFocusChangedEventArgs(IInputElement oldFocus, IInputElement newFocus)
    {
        OldFocus = oldFocus;
        NewFocus = newFocus;
    }

    public IInputElement OldFocus { get; }

    public IInputElement NewFocus { get; }
}
