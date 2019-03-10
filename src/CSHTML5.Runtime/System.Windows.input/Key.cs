
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================




#if DISABLED_BECAUSE_MISSES_CORRECT_MAPPING_OF_KEYCODES // cf. "VirtualKey.cs" for the current "Key" class implementation.


// cf. "VirtualKey.cs" for the non-migration version.

#if MIGRATION

using System;

namespace System.Windows.Input
{
    // Summary:
    //     Specifies the possible key values on a keyboard.
    public enum Key
    {
        // Summary:
        //     A special value indicating no key.
        None = 0,
        //
        // Summary:
        //     The BACKSPACE key.
        Back = 1,
        //
        // Summary:
        //     The TAB key.
        Tab = 2,
        //
        // Summary:
        //     The ENTER key.
        Enter = 3,
        //
        // Summary:
        //     The SHIFT key.
        Shift = 4,
        //
        // Summary:
        //     The CTRL (control) key.
        Ctrl = 5,
        //
        // Summary:
        //     The ALT key.
        Alt = 6,
        //
        // Summary:
        //     The CAPSLOCK key.
        CapsLock = 7,
        //
        // Summary:
        //     The ESC (also known as ESCAPE) key.
        Escape = 8,
        //
        // Summary:
        //     The SPACE key.
        Space = 9,
        //
        // Summary:
        //     The PAGEUP key.
        PageUp = 10,
        //
        // Summary:
        //     The PAGEDOWN key.
        PageDown = 11,
        //
        // Summary:
        //     The END key.
        End = 12,
        //
        // Summary:
        //     The HOME key.
        Home = 13,
        //
        // Summary:
        //     The left arrow key.
        Left = 14,
        //
        // Summary:
        //     The up arrow key.
        Up = 15,
        //
        // Summary:
        //     The right arrow key.
        Right = 16,
        //
        // Summary:
        //     The down arrow key.
        Down = 17,
        //
        // Summary:
        //     The INSERT key.
        Insert = 18,
        //
        // Summary:
        //     The DEL (also known as DELETE) key.
        Delete = 19,
        //
        // Summary:
        //     The 0 (zero) key.
        D0 = 20,
        //
        // Summary:
        //     The 1 key.
        D1 = 21,
        //
        // Summary:
        //     The 2 key.
        D2 = 22,
        //
        // Summary:
        //     The 3 key.
        D3 = 23,
        //
        // Summary:
        //     The 4 key.
        D4 = 24,
        //
        // Summary:
        //     The 5 key.
        D5 = 25,
        //
        // Summary:
        //     The 6 key.
        D6 = 26,
        //
        // Summary:
        //     The 7 key.
        D7 = 27,
        //
        // Summary:
        //     The 8 key.
        D8 = 28,
        //
        // Summary:
        //     The 9 key.
        D9 = 29,
        //
        // Summary:
        //     The A key.
        A = 30,
        //
        // Summary:
        //     The B key.
        B = 31,
        //
        // Summary:
        //     The C key.
        C = 32,
        //
        // Summary:
        //     The D key.
        D = 33,
        //
        // Summary:
        //     The E key.
        E = 34,
        //
        // Summary:
        //     The F key.
        F = 35,
        //
        // Summary:
        //     The G key.
        G = 36,
        //
        // Summary:
        //     The H key.
        H = 37,
        //
        // Summary:
        //     The I key.
        I = 38,
        //
        // Summary:
        //     The J key.
        J = 39,
        //
        // Summary:
        //     The K key.
        K = 40,
        //
        // Summary:
        //     The L key.
        L = 41,
        //
        // Summary:
        //     The M key.
        M = 42,
        //
        // Summary:
        //     The N key.
        N = 43,
        //
        // Summary:
        //     The O key.
        O = 44,
        //
        // Summary:
        //     The P key.
        P = 45,
        //
        // Summary:
        //     The Q key.
        Q = 46,
        //
        // Summary:
        //     The R key.
        R = 47,
        //
        // Summary:
        //     The S key.
        S = 48,
        //
        // Summary:
        //     The T key.
        T = 49,
        //
        // Summary:
        //     The U key.
        U = 50,
        //
        // Summary:
        //     The V key.
        V = 51,
        //
        // Summary:
        //     The W key.
        W = 52,
        //
        // Summary:
        //     The X key.
        X = 53,
        //
        // Summary:
        //     The Y key.
        Y = 54,
        //
        // Summary:
        //     The Z key.
        Z = 55,
        //
        // Summary:
        //     The F1 key.
        F1 = 56,
        //
        // Summary:
        //     The F2 key.
        F2 = 57,
        //
        // Summary:
        //     The F3 key.
        F3 = 58,
        //
        // Summary:
        //     The F4 key.
        F4 = 59,
        //
        // Summary:
        //     The F5 key.
        F5 = 60,
        //
        // Summary:
        //     The F6 key.
        F6 = 61,
        //
        // Summary:
        //     The F7 key.
        F7 = 62,
        //
        // Summary:
        //     The F8 key.
        F8 = 63,
        //
        // Summary:
        //     The F9 key.
        F9 = 64,
        //
        // Summary:
        //     The F10 key.
        F10 = 65,
        //
        // Summary:
        //     The F11 key.
        F11 = 66,
        //
        // Summary:
        //     The F12 key.
        F12 = 67,
        //
        // Summary:
        //     The 0 key on the number pad.
        NumPad0 = 68,
        //
        // Summary:
        //     The 1 key on the number pad.
        NumPad1 = 69,
        //
        // Summary:
        //     The 2 key on the number pad.
        NumPad2 = 70,
        //
        // Summary:
        //     The 3 key on the number pad.
        NumPad3 = 71,
        //
        // Summary:
        //     The 4 key on the number pad.
        NumPad4 = 72,
        //
        // Summary:
        //     The 5 key on the number pad.
        NumPad5 = 73,
        //
        // Summary:
        //     The 6 key on the number pad.
        NumPad6 = 74,
        //
        // Summary:
        //     The 7 key on the number pad.
        NumPad7 = 75,
        //
        // Summary:
        //     The 8 key on the number pad.
        NumPad8 = 76,
        //
        // Summary:
        //     The 9 key on the number pad.
        NumPad9 = 77,
        //
        // Summary:
        //     The * (MULTIPLY) key.
        Multiply = 78,
        //
        // Summary:
        //     The + (ADD) key.
        Add = 79,
        //
        // Summary:
        //     The - (SUBTRACT) key.
        Subtract = 80,
        //
        // Summary:
        //     The . (DECIMAL) key.
        Decimal = 81,
        //
        // Summary:
        //     The / (DIVIDE) key.
        Divide = 82,
        //
        // Summary:
        //     A special value indicating the key is out of range of this enumeration.
        Unknown = 255,
    }
}

#endif


#endif