

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
