

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
using System.Collections.Generic;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.System
#endif
{
    /// <summary>
    /// Specifies the values for each virtual key.
    /// </summary>
#if MIGRATION
    public enum Key : int //todo: the numbers do not match those of silverlight (example: in Silverlight, 1 corresponds to Back.
#else
    public enum VirtualKey : int
#endif
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
        //
        // Summary:
        //     The cancel key or button.
        Cancel = 3,
        //
        // Summary:
        //     The middle mouse button.
        MiddleButton = 4,
        //
        // Summary:
        //     An additional "extended" device key or button (for example, an additional
        //     mouse button).
        XButton1 = 5,
        //
        // Summary:
        //     An additional "extended" device key or button (for example, an additional
        //     mouse button).
        XButton2 = 6,
        //
        // Summary:
        //     The virtual back key or button.
        Back = 8,
        //
        // Summary:
        //     The Tab key.
        Tab = 9,
        //
        // Summary:
        //     The Clear key or button.
        Clear = 12,
        //
        // Summary:
        //     The Enter key.
        Enter = 13,
        //
        // Summary:
        //     The Shift key. This is the general Shift case, applicable to key layouts
        //     with only one Shift key or that do not need to differentiate between left
        //     Shift and right Shift keystrokes.
        Shift = 16,
        //
#if MIGRATION
        // Summary:
        //     The Ctrl key. This is the general Ctrl case, applicable to key layouts with
        //     only one Ctrl key or that do not need to differentiate between left Ctrl
        //     and right Ctrl keystrokes.
        Ctrl = 17,
        //
        // Summary:
        //     The menu key or button.
        Alt = 18,
#else
        Control = 17,
        Menu = 18,
#endif
        //
        // Summary:
        //     The Pause key or button.
        Pause = 19,
        //
        // Summary:
        //     The Caps Lock key or button.
#if MIGRATION
        CapsLock = 20,
#else
        CapitalLock = 20,
#endif
        //
        // Summary:
        //     The Kana symbol key-shift button.
        Kana = 21,
        //
        // Summary:
        //     The Hangul symbol key-shift button.
        Hangul = 21,
        //
        // Summary:
        //     The Junja symbol key-shift button.
        Junja = 23,
        //
        // Summary:
        //     The Final symbol key-shift button.
        Final = 24,
        //
        // Summary:
        //     The Kanji symbol key-shift button.
        Kanji = 25,
        //
        // Summary:
        //     The Hanja symbol key shift button.
        Hanja = 25,
        //
        // Summary:
        //     The Esc key.
        Escape = 27,
        //
        // Summary:
        //     The convert button or key.
        Convert = 28,
        //
        // Summary:
        //     The nonconvert button or key.
        NonConvert = 29,
        //
        // Summary:
        //     The accept button or key.
        Accept = 30,
        //
        // Summary:
        //     The mode change key.
        ModeChange = 31,
        //
        // Summary:
        //     The Spacebar key or button.
        Space = 32,
        //
        // Summary:
        //     The Page Up key.
        PageUp = 33,
        //
        // Summary:
        //     The Page Down key.
        PageDown = 34,
        //
        // Summary:
        //     The End key.
        End = 35,
        //
        // Summary:
        //     The Home key.
        Home = 36,
        //
        // Summary:
        //     The Left Arrow key.
        Left = 37,
        //
        // Summary:
        //     The Up Arrow key.
        Up = 38,
        //
        // Summary:
        //     The Right Arrow key.
        Right = 39,
        //
        // Summary:
        //     The Down Arrow key.
        Down = 40,
        //
        // Summary:
        //     The Select key or button.
        Select = 41,
        //
        // Summary:
        //     The Print key or button.
        Print = 42,
        //
        // Summary:
        //     The execute key or button.
        Execute = 43,
        //
        // Summary:
        //     The snapshot key or button.
        Snapshot = 44,
        //
        // Summary:
        //     The Insert key.
        Insert = 45,
        //
        // Summary:
        //     The Delete key.
        Delete = 46,
        //
        // Summary:
        //     The Help key or button.
        Help = 47,
        //
        // Summary:
        //     The number "0" key.
#if MIGRATION
        D0 = 48,
#else
        Number0 = 48,
#endif
        //
        // Summary:
        //     The number "1" key.
#if MIGRATION
        D1 = 49,
#else
        Number1 = 49,
#endif
        //
        // Summary:
        //     The number "2" key.
#if MIGRATION
        D2 = 50,
#else
        Number2 = 50,
#endif
        //
        // Summary:
        //     The number "3" key.
#if MIGRATION
        D3 = 51,
#else
        Number3 = 51,
#endif
        //
        // Summary:
        //     The number "4" key.
#if MIGRATION
        D4 = 52,
#else
        Number4 = 52,
#endif
        //
        // Summary:
        //     The number "5" key.
#if MIGRATION
        D5 = 53,
#else
        Number5 = 53,
#endif
        //
        // Summary:
        //     The number "6" key.
#if MIGRATION
        D6 = 54,
#else
        Number6 = 54,
#endif
        //
        // Summary:
        //     The number "7" key.
#if MIGRATION
        D7 = 55,
#else
        Number7 = 55,
#endif
        //
        // Summary:
        //     The number "8" key.
#if MIGRATION
        D8 = 56,
#else
        Number8 = 56,
#endif
        //
        // Summary:
        //     The number "9" key.
#if MIGRATION
        D9 = 57,
#else
        Number9 = 57,
#endif
        //
        // Summary:
        //     The letter "A" key.
        A = 65,
        //
        // Summary:
        //     The letter "B" key.
        B = 66,
        //
        // Summary:
        //     The letter "C" key.
        C = 67,
        //
        // Summary:
        //     The letter "D" key.
        D = 68,
        //
        // Summary:
        //     The letter "E" key.
        E = 69,
        //
        // Summary:
        //     The letter "F" key.
        F = 70,
        //
        // Summary:
        //     The letter "G" key.
        G = 71,
        //
        // Summary:
        //     The letter "H" key.
        H = 72,
        //
        // Summary:
        //     The letter "I" key.
        I = 73,
        //
        // Summary:
        //     The letter "J" key.
        J = 74,
        //
        // Summary:
        //     The letter "K" key.
        K = 75,
        //
        // Summary:
        //     The letter "L" key.
        L = 76,
        //
        // Summary:
        //     The letter "M" key.
        M = 77,
        //
        // Summary:
        //     The letter "N" key.
        N = 78,
        //
        // Summary:
        //     The letter "O" key.
        O = 79,
        //
        // Summary:
        //     The letter "P" key.
        P = 80,
        //
        // Summary:
        //     The letter "Q" key.
        Q = 81,
        //
        // Summary:
        //     The letter "R" key.
        R = 82,
        //
        // Summary:
        //     The letter "S" key.
        S = 83,
        //
        // Summary:
        //     The letter "T" key.
        T = 84,
        //
        // Summary:
        //     The letter "U" key.
        U = 85,
        //
        // Summary:
        //     The letter "V" key.
        V = 86,
        //
        // Summary:
        //     The letter "W" key.
        W = 87,
        //
        // Summary:
        //     The letter "X" key.
        X = 88,
        //
        // Summary:
        //     The letter "Y" key.
        Y = 89,
        //
        // Summary:
        //     The letter "Z" key.
        Z = 90,
        //
        // Summary:
        //     The left Windows key.
        LeftWindows = 91,
        //
        // Summary:
        //     The right Windows key.
        RightWindows = 92,
        //
        // Summary:
        //     The application key or button.
        Application = 93,
        //
        // Summary:
        //     The sleep key or button.
        Sleep = 95,
        //
        // Summary:
        //     The number "0" key as located on a numeric pad.
#if MIGRATION
        NumPad0 = 96,
#else
        NumberPad0 = 96,
#endif
        //
        // Summary:
        //     The number "1" key as located on a numeric pad.
#if MIGRATION
        NumPad1 = 97,
#else
        NumberPad1 = 97,
#endif
        //
        // Summary:
        //     The number "2" key as located on a numeric pad.
#if MIGRATION
        NumPad2 = 98,
#else
        NumberPad2 = 98,
#endif
        //
        // Summary:
        //     The number "3" key as located on a numeric pad.
#if MIGRATION
        NumPad3 = 99,
#else
        NumberPad3 = 99,
#endif
        //
        // Summary:
        //     The number "4" key as located on a numeric pad.
#if MIGRATION
        NumPad4 = 100,
#else
        NumberPad4 = 100,
#endif
        //
        // Summary:
        //     The number "5" key as located on a numeric pad.
#if MIGRATION
        NumPad5 = 101,
#else
        NumberPad5 = 101,
#endif
        //
        // Summary:
        //     The number "6" key as located on a numeric pad.
#if MIGRATION
        NumPad6 = 102,
#else
        NumberPad6 = 102,
#endif
        //
        // Summary:
        //     The number "7" key as located on a numeric pad.
#if MIGRATION
        NumPad7 = 103,
#else
        NumberPad7 = 103,
#endif
        //
        // Summary:
        //     The number "8" key as located on a numeric pad.
#if MIGRATION
        NumPad8 = 104,
#else
        NumberPad8 = 104,
#endif
        //
        // Summary:
        //     The number "9" key as located on a numeric pad.
#if MIGRATION
        NumPad9 = 105,
#else
        NumberPad9 = 105,
#endif
        //
        // Summary:
        //     The multiply (*) operation key as located on a numeric pad.
        Multiply = 106,
        //
        // Summary:
        //     The add (+) operation key as located on a numeric pad.
        Add = 107,
        //
        // Summary:
        //     The separator key as located on a numeric pad.
        Separator = 108,
        //
        // Summary:
        //     The subtract (-) operation key as located on a numeric pad.
        Subtract = 109,
        //
        // Summary:
        //     The decimal (.) key as located on a numeric pad.
        Decimal = 110,
        //
        // Summary:
        //     The divide (/) operation key as located on a numeric pad.
        Divide = 111,
        //
        // Summary:
        //     The F1 function key.
        F1 = 112,
        //
        // Summary:
        //     The F2 function key.
        F2 = 113,
        //
        // Summary:
        //     The F3 function key.
        F3 = 114,
        //
        // Summary:
        //     The F4 function key.
        F4 = 115,
        //
        // Summary:
        //     The F5 function key.
        F5 = 116,
        //
        // Summary:
        //     The F6 function key.
        F6 = 117,
        //
        // Summary:
        //     The F7 function key.
        F7 = 118,
        //
        // Summary:
        //     The F8 function key.
        F8 = 119,
        //
        // Summary:
        //     The F9 function key.
        F9 = 120,
        //
        // Summary:
        //     The F10 function key.
        F10 = 121,
        //
        // Summary:
        //     The F11 function key.
        F11 = 122,
        //
        // Summary:
        //     The F12 function key.
        F12 = 123,
        //
        // Summary:
        //     The F13 function key.
        F13 = 124,
        //
        // Summary:
        //     The F14 function key.
        F14 = 125,
        //
        // Summary:
        //     The F15 function key.
        F15 = 126,
        //
        // Summary:
        //     The F16 function key.
        F16 = 127,
        //
        // Summary:
        //     The F17 function key.
        F17 = 128,
        //
        // Summary:
        //     The F18 function key.
        F18 = 129,
        //
        // Summary:
        //     The F19 function key.
        F19 = 130,
        //
        // Summary:
        //     The F20 function key.
        F20 = 131,
        //
        // Summary:
        //     The F21 function key.
        F21 = 132,
        //
        // Summary:
        //     The F22 function key.
        F22 = 133,
        //
        // Summary:
        //     The F23 function key.
        F23 = 134,
        //
        // Summary:
        //     The F24 function key.
        F24 = 135,
        //
        // Summary:
        //     The Num Lock key.
        NumberKeyLock = 144,
        //
        // Summary:
        //     The Scroll Lock (ScrLk) key.
        Scroll = 145,
#if !MIGRATION
        //
        // Summary:
        //     The left Shift key.
        LeftShift = 160,
        //
        // Summary:
        //     The right Shift key.
        RightShift = 161,
        //
        // Summary:
        //     The left Ctrl key.
        LeftControl = 162,
        //
        // Summary:
        //     The right Ctrl key.
        RightControl = 163,
        //
        // Summary:
        //     The left menu key.
        LeftMenu = 164,
        //
        // Summary:
        //     The right menu key.
        RightMenu = 165,
        //
        // Summary:
        //     The comma key.
        Comma = 188,
        //
        // Summary:
        //     The period key.
        Period = 190,
#endif
        //      An unknown key.
        Unknown = int.MaxValue,
    }
    internal static class INTERNAL_VirtualKeysHelpers
    {
        internal static bool IsUnknownKey(int intValue)
        {
            if (intValue < 0 || intValue > 165)
            {
#if MIGRATION
                return true;
#else
                if (intValue != 188 && intValue != 190)
                {
                    return true;
                }
#endif
            }
            HashSet<int> unknownKeys = new HashSet<int>() { 7, 10, 11, 14, 15, 22, 26, 58, 59, 60, 61, 62, 63, 64, 94, 136, 137, 138, 139, 140, 141, 142, 143, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159 };
            if (unknownKeys.Contains(intValue))
            {
                return true;
            }
            return false;
        }

#if MIGRATION
        internal static Key GetKeyFromKeyCode(int keyCode)
#else
        internal static VirtualKey GetKeyFromKeyCode(int keyCode)
#endif
        {
            if (keyCode == 59) // The keyCode for the period in Firefox is 59 while it is 190 for IE, Chrome and Edge.
            {
                keyCode = 190;
            }
#if MIGRATION
            var key = (INTERNAL_VirtualKeysHelpers.IsUnknownKey(keyCode) ? Key.Unknown : (Key)keyCode);
#else
            var key = (INTERNAL_VirtualKeysHelpers.IsUnknownKey(keyCode) ? VirtualKey.Unknown : (VirtualKey)keyCode);
#endif

            return key;
        }

#if MIGRATION
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
#endif
    }

}