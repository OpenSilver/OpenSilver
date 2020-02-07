
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



#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    /// <summary>
    /// An abstract class used as the base class for the also-abstract Block and Inline classes. 
    /// TextElement supports common API for classes involved in the XAML text object model,
    /// such as properties that control text size, font families and so on.
    /// </summary>
    public abstract class TextElement : Control
    {
#if WORKINPROGRESS
        public static readonly DependencyProperty CharacterSpacingProperty = DependencyProperty.Register("CharacterSpacing", typeof(int), typeof(TextElement), null);
        public int CharacterSpacing
        {
            get { return (int)this.GetValue(CharacterSpacingProperty); }
            set { this.SetValue(CharacterSpacingProperty, value); }
        }
#endif
    }
}
