
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
using System.Globalization;
using OpenSilver.Internal;

namespace System.Windows.Input;

/// <summary>
/// Defines a keyboard combination that can be used to invoke a command.
/// </summary>
[TypeConverter(typeof(KeyGestureConverter))]
public class KeyGesture : InputGesture
{
    private const char MULTIPLEGESTURE_DELIMITER = ';';

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyGesture"/> class with the specified <see cref="Input.Key"/>.
    /// </summary>
    /// <param name="key">
    /// The key associated with this gesture.
    /// </param>
    /// <exception cref="InvalidEnumArgumentException">
    /// <paramref name="key"/> is not a valid <see cref="Input.Key"/>.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <paramref name="key"/> is not a valid <see cref="KeyGesture"/>.
    /// </exception>
    public KeyGesture(Key key)
        : this(key, ModifierKeys.None)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyGesture"/> class with the specified <see cref="Input.Key"/>
    /// and <see cref="ModifierKeys"/>.
    /// </summary>
    /// <param name="key">
    /// The key associated with the gesture.
    /// </param>
    /// <param name="modifiers">
    /// The modifier keys associated with the gesture.
    /// </param>
    /// <exception cref="InvalidEnumArgumentException">
    /// <paramref name="modifiers"/> is not a valid <see cref="ModifierKeys"/> -or- <paramref name="key"/> is not a valid <see cref="Input.Key"/>.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <paramref name="key"/> and <paramref name="modifiers"/> do not form a valid <see cref="KeyGesture"/>.
    /// </exception>
    public KeyGesture(Key key, ModifierKeys modifiers)
        : this(key, modifiers, string.Empty, true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyGesture"/> class with the specified <see cref="Input.Key"/>,
    /// <see cref="ModifierKeys"/>, and display string.
    /// </summary>
    /// <param name="key">
    /// The key associated with the gesture.
    /// </param>
    /// <param name="modifiers">
    /// The modifier keys associated with the gesture.
    /// </param>
    /// <param name="displayString">
    /// A string representation of the <see cref="KeyGesture"/>.
    /// </param>
    /// <exception cref="InvalidEnumArgumentException">
    /// <paramref name="modifiers"/> is not a valid <see cref="ModifierKeys"/> -or- <paramref name="key"/> is not a valid <see cref="Input.Key"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="displayString"/> is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <paramref name="key"/> and <paramref name="modifiers"/> do not form a valid <see cref="KeyGesture"/>.
    /// </exception>
    public KeyGesture(Key key, ModifierKeys modifiers, string displayString)
        : this(key, modifiers, displayString, true)
    {
    }


    /// <summary>
    /// Internal constructor used by KeyBinding to avoid key and modifier validation
    /// This allows setting KeyBinding.Key and KeyBinding.Modifiers without regard
    /// to order.
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="modifiers">Modifiers</param>
    /// <param name="validateGesture">If true, throws an exception if the key and modifier are not valid</param>
    internal KeyGesture(Key key, ModifierKeys modifiers, bool validateGesture)
        : this(key, modifiers, string.Empty, validateGesture)
    {
    }

    /// <summary>
    /// Private constructor that does the real work.
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="modifiers">Modifiers</param>
    /// <param name="displayString">display string</param>
    /// <param name="validateGesture">If true, throws an exception if the key and modifier are not valid</param>
    private KeyGesture(Key key, ModifierKeys modifiers, string displayString, bool validateGesture)
    {
        if (!ModifierKeysConverter.IsDefinedModifierKeys(modifiers))
        {
            throw new InvalidEnumArgumentException(nameof(modifiers), (int)modifiers, typeof(ModifierKeys));
        }

        if (!KeyConverter.IsDefinedKey(key))
        {
            throw new InvalidEnumArgumentException(nameof(key), (int)key, typeof(Key));
        }

        if (displayString is null)
        {
            throw new ArgumentNullException(nameof(displayString));
        }

        if (validateGesture && !IsValid(key, modifiers))
        {
            throw new NotSupportedException(string.Format(Strings.KeyGesture_Invalid, modifiers, key));
        }

        Modifiers = modifiers;
        Key = key;
        DisplayString = displayString;
    }

    /// <summary>
    /// Gets a string representation of this <see cref="KeyGesture"/>.
    /// </summary>
    /// <returns>
    /// The display string for this <see cref="KeyGesture"/>. The default value is <see cref="string.Empty"/>.
    /// </returns>
    public string DisplayString { get; }

    /// <summary>
    /// Gets the key associated with this <see cref="KeyGesture"/>.
    /// </summary>
    /// <returns>
    /// The key associated with the gesture. The default value is <see cref="Key.None"/>.
    /// </returns>
    public Key Key { get; }

    /// <summary>
    /// Gets the modifier keys associated with this <see cref="KeyGesture"/>.
    /// </summary>
    /// <returns>
    /// The modifier keys associated with the gesture. The default value is <see cref="ModifierKeys.None"/>.
    /// </returns>
    public ModifierKeys Modifiers { get; }

    /// <summary>
    /// Returns a string that can be used to display the <see cref="KeyGesture"/>.
    /// </summary>
    /// <param name="culture">
    /// The culture specific information.
    /// </param>
    /// <returns>
    /// The string to display
    /// </returns>
    public string GetDisplayStringForCulture(CultureInfo culture)
    {
        // return the DisplayString, if it was set by the ctor
        if (!string.IsNullOrEmpty(DisplayString))
        {
            return DisplayString;
        }

        // otherwise use the type converter
        return KeyGestureConverter.ToString(this);
    }

    /// <summary>
    /// Determines whether this <see cref="KeyGesture"/> matches the input associated with the specified 
    /// <see cref="InputEventArgs"/> object.
    /// </summary>
    /// <param name="targetElement">
    /// The target.
    /// </param>
    /// <param name="inputEventArgs">
    /// The input event data to compare this gesture to.
    /// </param>
    /// <returns>
    /// true if the event data matches this <see cref="KeyGesture"/>; otherwise, false.
    /// </returns>
    public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
    {
        if (inputEventArgs is KeyEventArgs keyEventArgs)
        {
            return Key == keyEventArgs.Key && Modifiers == Keyboard.Modifiers;
        }
        return false;
    }

    /// <summary>
    /// Is Valid Keyboard input to process for commands
    /// </summary>
    internal static bool IsValid(Key key, ModifierKeys modifiers)
    {
        //
        //  Don't enforce any rules on the Function keys or on the number pad keys.
        //
        if (!((key >= Key.F1 && key <= Key.F24) || (key >= Key.NumPad0 && key <= Key.Divide)))
        {
            //
            //  We check whether Control/Alt/Windows key is down for modifiers. We don't check
            //  for shift at this time as Shift with any combination is already covered in above check.
            //  Shift alone as modifier case, we defer to the next condition to avoid conflicing with
            //  TextInput.

            if ((modifiers & (ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Windows)) != 0)
            {
                return true;
            }
            else if ((key >= Key.D0 && key <= Key.D9) || (key >= Key.A && key <= Key.Z))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Decode the strings keyGestures and displayStrings, creating a sequence
    /// of KeyGestures.  Add each KeyGesture to the given InputGestureCollection.
    /// The two input strings typically come from a resource file.
    /// </summary>
    internal static void AddGesturesFromResourceStrings(string keyGestures, string displayStrings, InputGestureCollection gestures)
    {
        while (!string.IsNullOrEmpty(keyGestures))
        {
            string keyGestureToken;
            string keyDisplayString;

            // break apart first gesture from the rest
            int index = keyGestures.IndexOf(MULTIPLEGESTURE_DELIMITER);
            if (index >= 0)
            {
                // multiple gestures exist
                keyGestureToken = keyGestures.Substring(0, index);
                keyGestures = keyGestures.Substring(index + 1);
            }
            else
            {
                keyGestureToken = keyGestures;
                keyGestures = string.Empty;
            }

            // similarly, break apart first display string from the rest
            index = displayStrings.IndexOf(MULTIPLEGESTURE_DELIMITER);
            if (index >= 0)
            {
                // multiple display strings exist
                keyDisplayString = displayStrings.Substring(0, index);
                displayStrings = displayStrings.Substring(index + 1);
            }
            else
            {
                keyDisplayString = displayStrings;
                displayStrings = string.Empty;
            }

            if (CreateFromResourceStrings(keyGestureToken, keyDisplayString) is KeyGesture keyGesture)
            {
                gestures.Add(keyGesture);
            }
        }
    }

    internal static KeyGesture CreateFromResourceStrings(string keyGestureToken, string keyDisplayString)
    {
        // combine the gesture and the display string, producing a string
        // that the type converter will recognize
        if (!string.IsNullOrEmpty(keyDisplayString))
        {
            keyGestureToken += KeyGestureConverter.DISPLAYSTRING_SEPARATOR + keyDisplayString;
        }

        return KeyGestureConverter.FromString(null, CultureInfo.InvariantCulture, keyGestureToken);
    }
}
