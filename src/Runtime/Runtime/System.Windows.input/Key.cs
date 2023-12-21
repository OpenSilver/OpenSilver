
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

using System.Collections.Generic;

namespace System.Windows.Input
{
    /// <summary>
    /// Specifies the values for each virtual key.
    /// </summary>
    public enum Key : int //todo: the numbers do not match those of silverlight (example: in Silverlight, 1 corresponds to Back.
    {
        /// <summary>
        /// No virtual key value.
        /// </summary>
        None = 0,
        /// <summary>
        /// The left mouse button.
        /// </summary>
        LeftButton = 1,
        /// <summary>
        /// The right mouse button.
        /// </summary>
        RightButton = 2,
        /// <summary>
        /// The cancel key or button.
        /// </summary>
        Cancel = 3,
        /// <summary>
        /// The middle mouse button.
        /// </summary>
        MiddleButton = 4,
        /// <summary>
        /// An additional "extended" device key or button (for example, an additional
        /// mouse button).
        /// </summary>
        XButton1 = 5,
        /// <summary>
        /// An additional "extended" device key or button (for example, an additional
        /// mouse button).
        /// </summary>
        XButton2 = 6,
        /// <summary>
        /// The virtual back key or button.
        /// </summary>
        Back = 8,
        /// <summary>
        /// The Tab key.
        /// </summary>
        Tab = 9,
        /// <summary>
        /// The Clear key or button.
        /// </summary>
        Clear = 12,
        /// <summary>
        /// The Enter key.
        /// </summary>
        Enter = 13,
        /// <summary>
        /// The Shift key. This is the general Shift case, applicable to key layouts
        /// with only one Shift key or that do not need to differentiate between left
        /// Shift and right Shift keystrokes.
        /// </summary>
        Shift = 16,
        /// <summary>
        /// The Ctrl key. This is the general Ctrl case, applicable to key layouts with
        /// only one Ctrl key or that do not need to differentiate between left Ctrl
        /// and right Ctrl keystrokes.
        /// </summary>
        Ctrl = 17,
        /// <summary>
        /// The menu key or button.
        /// </summary>
        Alt = 18,
        /// <summary>
        /// The Pause key or button.
        /// </summary>
        Pause = 19,
        /// <summary>
        /// The Caps Lock key or button.
        /// </summary>
        CapsLock = 20,
        /// <summary>
        /// The Kana symbol key-shift button.
        /// </summary>
        Kana = 21,
        /// <summary>
        /// The Hangul symbol key-shift button.
        /// </summary>
        Hangul = 21,
        /// <summary>
        /// The Junja symbol key-shift button.
        /// </summary>
        Junja = 23,
        /// <summary>
        /// The Final symbol key-shift button.
        /// </summary>
        Final = 24,
        /// <summary>
        /// The Kanji symbol key-shift button.
        /// </summary>
        Kanji = 25,
        /// <summary>
        /// The Hanja symbol key shift button.
        /// </summary>
        Hanja = 25,
        /// <summary>
        /// The Esc key.
        /// </summary>
        Escape = 27,
        /// <summary>
        /// The convert button or key.
        /// </summary>
        Convert = 28,
        /// <summary>
        /// The nonconvert button or key.
        /// </summary>
        NonConvert = 29,
        /// <summary>
        /// The accept button or key.
        /// </summary>
        Accept = 30,
        /// <summary>
        /// The mode change key.
        /// </summary>
        ModeChange = 31,
        /// <summary>
        /// The Spacebar key or button.
        /// </summary>
        Space = 32,
        /// <summary>
        /// The Page Up key.
        /// </summary>
        PageUp = 33,
        /// <summary>
        /// The Page Down key.
        /// </summary>
        PageDown = 34,
        /// <summary>
        /// The End key.
        /// </summary>
        End = 35,
        /// <summary>
        /// The Home key.
        /// </summary>
        Home = 36,
        /// <summary>
        /// The Left Arrow key.
        /// </summary>
        Left = 37,
        /// <summary>
        /// The Up Arrow key.
        /// </summary>
        Up = 38,
        /// <summary>
        /// The Right Arrow key.
        /// </summary>
        Right = 39,
        /// <summary>
        /// The Down Arrow key.
        /// </summary>
        Down = 40,
        /// <summary>
        /// The Select key or button.
        /// </summary>
        Select = 41,
        /// <summary>
        /// The Print key or button.
        /// </summary>
        Print = 42,
        /// <summary>
        /// The execute key or button.
        /// </summary>
        Execute = 43,
        /// <summary>
        /// The snapshot key or button.
        /// </summary>
        Snapshot = 44,
        /// <summary>
        /// The Insert key.
        /// </summary>
        Insert = 45,
        /// <summary>
        /// The Delete key.
        /// </summary>
        Delete = 46,
        /// <summary>
        /// The Help key or button.
        /// </summary>
        Help = 47,
        /// <summary>
        /// The number "0" key.
        /// </summary>
        D0 = 48,
        /// <summary>
        /// The number "1" key.
        /// </summary>
        D1 = 49,
        /// <summary>
        /// The number "2" key.
        /// </summary>
        D2 = 50,
        /// <summary>
        /// The number "3" key.
        /// </summary>
        D3 = 51,
        /// <summary>
        /// The number "4" key.
        /// </summary>
        D4 = 52,
        /// <summary>
        /// The number "5" key.
        /// </summary>
        D5 = 53,
        /// <summary>
        /// The number "6" key.
        /// </summary>
        D6 = 54,
        /// <summary>
        /// The number "7" key.
        /// </summary>
        D7 = 55,
        /// <summary>
        /// The number "8" key.
        /// </summary>
        D8 = 56,
        /// <summary>
        /// The number "9" key.
        /// </summary>
        D9 = 57,
        /// <summary>
        /// The letter "A" key.
        /// </summary>
        A = 65,
        /// <summary>
        /// The letter "B" key.
        /// </summary>
        B = 66,
        /// <summary>
        /// The letter "C" key.
        /// </summary>
        C = 67,
        /// <summary>
        /// The letter "D" key.
        /// </summary>
        D = 68,
        /// <summary>
        /// The letter "E" key.
        /// </summary>
        E = 69,
        /// <summary>
        /// The letter "F" key.
        /// </summary>
        F = 70,
        /// <summary>
        /// The letter "G" key.
        /// </summary>
        G = 71,
        /// <summary>
        /// The letter "H" key.
        /// </summary>
        H = 72,
        /// <summary>
        /// The letter "I" key.
        /// </summary>
        I = 73,
        /// <summary>
        /// The letter "J" key.
        /// </summary>
        J = 74,
        /// <summary>
        /// The letter "K" key.
        /// </summary>
        K = 75,
        /// <summary>
        /// The letter "L" key.
        /// </summary>
        L = 76,
        /// <summary>
        /// The letter "M" key.
        /// </summary>
        M = 77,
        /// <summary>
        /// The letter "N" key.
        /// </summary>
        N = 78,
        /// <summary>
        /// The letter "O" key.
        /// </summary>
        O = 79,
        /// <summary>
        /// The letter "P" key.
        /// </summary>
        P = 80,
        /// <summary>
        /// The letter "Q" key.
        /// </summary>
        Q = 81,
        /// <summary>
        /// The letter "R" key.
        /// </summary>
        R = 82,
        /// <summary>
        /// The letter "S" key.
        /// </summary>
        S = 83,
        /// <summary>
        /// The letter "T" key.
        /// </summary>
        T = 84,
        /// <summary>
        /// The letter "U" key.
        /// </summary>
        U = 85,
        /// <summary>
        /// The letter "V" key.
        /// </summary>
        V = 86,
        /// <summary>
        /// The letter "W" key.
        /// </summary>
        W = 87,
        /// <summary>
        /// The letter "X" key.
        /// </summary>
        X = 88,
        /// <summary>
        /// The letter "Y" key.
        /// </summary>
        Y = 89,
        /// <summary>
        /// The letter "Z" key.
        /// </summary>
        Z = 90,
        /// <summary>
        /// The left Windows key.
        /// </summary>
        LeftWindows = 91,
        /// <summary>
        /// The right Windows key.
        /// </summary>
        RightWindows = 92,
        /// <summary>
        /// The application key or button.
        /// </summary>
        Application = 93,
        /// <summary>
        /// The sleep key or button.
        /// </summary>
        Sleep = 95,
        /// <summary>
        /// The number "0" key as located on a numeric pad.
        /// </summary>
        NumPad0 = 96,
        /// <summary>
        /// The number "1" key as located on a numeric pad.
        /// </summary>
        NumPad1 = 97,
        /// <summary>
        /// The number "2" key as located on a numeric pad.
        /// </summary>
        NumPad2 = 98,
        /// <summary>
        /// The number "3" key as located on a numeric pad.
        /// </summary>
        NumPad3 = 99,
        /// <summary>
        /// The number "4" key as located on a numeric pad.
        /// </summary>
        NumPad4 = 100,
        /// <summary>
        /// The number "5" key as located on a numeric pad.
        /// </summary>
        NumPad5 = 101,
        /// <summary>
        /// The number "6" key as located on a numeric pad.
        /// </summary>
        NumPad6 = 102,
        /// <summary>
        /// The number "7" key as located on a numeric pad.
        /// </summary>
        NumPad7 = 103,
        /// <summary>
        /// The number "8" key as located on a numeric pad.
        /// </summary>
        NumPad8 = 104,
        /// <summary>
        /// The number "9" key as located on a numeric pad.
        /// </summary>
        NumPad9 = 105,
        /// <summary>
        /// The multiply (*) operation key as located on a numeric pad.
        /// </summary>
        Multiply = 106,
        /// <summary>
        /// The add (+) operation key as located on a numeric pad.
        /// </summary>
        Add = 107,
        /// <summary>
        /// The separator key as located on a numeric pad.
        /// </summary>
        Separator = 108,
        /// <summary>
        /// The subtract (-) operation key as located on a numeric pad.
        /// </summary>
        Subtract = 109,
        /// <summary>
        /// The decimal (.) key as located on a numeric pad.
        /// </summary>
        Decimal = 110,
        /// <summary>
        /// The divide (/) operation key as located on a numeric pad.
        /// </summary>
        Divide = 111,
        /// <summary>
        /// The F1 function key.
        /// </summary>
        F1 = 112,
        /// <summary>
        /// The F2 function key.
        /// </summary>
        F2 = 113,
        /// <summary>
        /// The F3 function key.
        /// </summary>
        F3 = 114,
        /// <summary>
        /// The F4 function key.
        /// </summary>
        F4 = 115,
        /// <summary>
        /// The F5 function key.
        /// </summary>
        F5 = 116,
        /// <summary>
        /// The F6 function key.
        /// </summary>
        F6 = 117,
        /// <summary>
        /// The F7 function key.
        /// </summary>
        F7 = 118,
        /// <summary>
        /// The F8 function key.
        /// </summary>
        F8 = 119,
        /// <summary>
        /// The F9 function key.
        /// </summary>
        F9 = 120,
        /// <summary>
        /// The F10 function key.
        /// </summary>
        F10 = 121,
        /// <summary>
        /// The F11 function key.
        /// </summary>
        F11 = 122,
        /// <summary>
        /// The F12 function key.
        /// </summary>
        F12 = 123,
        /// <summary>
        /// The F13 function key.
        /// </summary>
        F13 = 124,
        /// <summary>
        /// The F14 function key.
        /// </summary>
        F14 = 125,
        /// <summary>
        /// The F15 function key.
        /// </summary>
        F15 = 126,
        /// <summary>
        /// The F16 function key.
        /// </summary>
        F16 = 127,
        /// <summary>
        /// The F17 function key.
        /// </summary>
        F17 = 128,
        /// <summary>
        /// The F18 function key.
        /// </summary>
        F18 = 129,
        /// <summary>
        /// The F19 function key.
        /// </summary>
        F19 = 130,
        /// <summary>
        /// The F20 function key.
        /// </summary>
        F20 = 131,
        /// <summary>
        /// The F21 function key.
        /// </summary>
        F21 = 132,
        /// <summary>
        /// The F22 function key.
        /// </summary>
        F22 = 133,
        /// <summary>
        /// The F23 function key.
        /// </summary>
        F23 = 134,
        /// <summary>
        /// The F24 function key.
        /// </summary>
        F24 = 135,
        /// <summary>
        /// The Num Lock key.
        /// </summary>
        NumberKeyLock = 144,
        /// <summary>
        /// The Scroll Lock (ScrLk) key.
        /// </summary>
        Scroll = 145,
        /// <summary>
        /// An unknown key.
        /// </summary>
        Unknown = int.MaxValue,
    }

    internal static class VirtualKeysHelpers
    {
        private static readonly HashSet<int> _unknownKeys =
            new()
            { 
                7, 10, 11, 14, 15, 22, 26, 58, 59, 60, 61, 62,
                63, 64, 94, 136, 137, 138, 139, 140, 141, 142,
                143, 146, 147, 148, 149, 150, 151, 152, 153,
                154, 155, 156, 157, 158, 159
            };

        internal static bool IsUnknownKey(int intValue)
        {
            return intValue < 0 || intValue > 165 || _unknownKeys.Contains(intValue);
        }

        internal static Key GetKeyFromKeyCode(int keyCode)
        {
            if (keyCode == 59) // The keyCode for the period in Firefox is 59 while it is 190 for IE, Chrome and Edge.
            {
                keyCode = 190;
            }
            var key = (IsUnknownKey(keyCode) ? Key.Unknown : (Key)keyCode);

            return key;
        }

        internal static int FixKeyCodeForSilverlight(int keycode)
        {
            // Silverlight does not distinguish between left and right ctrl/alt/shift keys, so we need to "redirect" those keys to the generic ones.

            int fixedKeyCode = keycode;
            //left Shift key or right Shift key.
            if (fixedKeyCode == 160 || fixedKeyCode == 161)
            {
                fixedKeyCode = 16;
            }
            //left Ctrl key or right Ctrl key
            else if (fixedKeyCode == 162 || fixedKeyCode == 163)
            {
                fixedKeyCode = 17;
            }
            //left Alt key of right Alt key
            else if (fixedKeyCode == 164 || fixedKeyCode == 165)
            {
                fixedKeyCode = 18;
            }
            return fixedKeyCode;
        }
    }
}