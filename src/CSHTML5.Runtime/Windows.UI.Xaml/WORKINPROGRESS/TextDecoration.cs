﻿
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
#if MIGRATION
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml.Media.Animation;
#endif

#if MIGRATION
namespace System.Windows
{
    internal sealed partial class TextDecoration
    {
        internal TextDecoration(int decoration)
        {
            this.Decoration = decoration;
        }

        internal int Decoration { get; private set; }

        public override bool Equals(object o)
        {
            if (o is TextDecoration)
            {
                TextDecoration td = (TextDecoration)o;
                return this.Decoration == td.Decoration;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Decoration;
        }

        internal static bool Equals(TextDecoration left, TextDecoration right)
        {
            if (left == null)
            {
                return right == null;
            }
            return left.Equals(right);
        }
    }


#if no
    public sealed partial class TextDecoration : Animatable
    {
    }
#endif
}
#endif
