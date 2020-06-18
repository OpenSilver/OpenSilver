

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
    public abstract partial class TextElement : Control
    {
        internal override void UpdateTabIndex(bool isTabStop, int tabIndex)
        {
            // we don't do anything since TextElement is not supposed to be a Control in the first place
            // and it is not supposed to be counted in tabbing
            return;
        }

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
