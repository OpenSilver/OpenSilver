
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



using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#else
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a control that a user can select (check) or clear (uncheck). A
    /// CheckBox can also report its value as indeterminate.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <CheckBox Content="Text of the CheckBox." Checked="CheckBox_Checked" Unchecked="CheckBox_Unchecked"/>
    /// </code>
    /// <code lang="C#">
    /// void CheckBox_Checked(object sender, RoutedEventArgs e)
    /// {
    ///     MessageBox.Show("You checked me.");
    /// }
    ///
    /// void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    /// {
    ///     MessageBox.Show("You unchecked me.");
    /// }
    /// </code>
    /// </example>
    public class CheckBox : ToggleButton
    {
        /// <summary>
        /// Initializes a new instance of the CheckBox class.
        /// </summary>
        public CheckBox()
        {
            if(!CSHTML5.Interop.IsRunningInTheSimulator)
            {
                _reactsToKeyboardEventsWhenFocused = false;
            }
        }

#if MIGRATION
        internal override void OnKeyDownWhenFocused(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Space))
#else
        internal override void OnKeyDownWhenFocused(object sender, Input.KeyRoutedEventArgs e)
        {
            if ((e.Key == Windows.System.VirtualKey.Space)) //didn't use the same as the one for ButtonBase just because in the browsers, the space key naturally does the trick while enter is ignored.
#endif
            {
                ToggleButton_Click(this, null);
            }
        }

        protected override void SetDefaultStyle() // Overridden in CheckBox and RadioButton
        {
            // No default style at the moment because we use the HTML5 native checkbox.
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            return INTERNAL_CheckBoxAndRadioButtonHelpers.CreateDomElement(this, "checkbox", parentRef, out domElementWhereToPlaceChildren);
        }

        protected override void UpdateDomBasedOnCheckedState(bool? isChecked)
        {
            INTERNAL_CheckBoxAndRadioButtonHelpers.UpdateDomBasedOnCheckedState(this, isChecked);
        }

        internal override void SubscribeToClickEventForChildContainerDiv(dynamic divWhereToPlaceChild, dynamic checkBoxDomElement)
        {
            if (INTERNAL_CheckBoxAndRadioButtonHelpers.IsRunningInJavaScript())
            {
#if !BRIDGE
                JSIL.Verbatim.Expression(@"
$0.addEventListener('click', function(e) {
    if($1.checked === true)
    {
        $1.checked = false;
    }
    else
    {
        $1.checked = true;
    }
    var evt = document.createEvent('Event');
    evt.initEvent('change', false, false);
    $1.dispatchEvent(evt);
}, false);", divWhereToPlaceChild, checkBoxDomElement);
#else
                Script.Write(@"
{0}.addEventListener('click', function(e) {
    if({1}.checked === true)
    {
        {1}.checked = false;
    }
    else
    {
        {1}.checked = true;
    }
    var evt = document.createEvent('Event');
    evt.initEvent('change', false, false);
    {1}.dispatchEvent(evt);
}, false);", divWhereToPlaceChild, checkBoxDomElement);
            
#endif
            }
#if !BRIDGE
            else
            {
                // ---- SIMULATOR ----
                string javaScriptToExecute = string.Format(@"
var divWhereToPlaceChild = document.getElementById(""{0}"");
divWhereToPlaceChild.addEventListener('click', function(e) {{
var checkBoxDomElement = document.getElementById(""{1}"");
    if(checkBoxDomElement.checked === true)
    {{
        checkBoxDomElement.checked = false;
    }}
    else
    {{
        checkBoxDomElement.checked = true;
    }}
    var evt = document.createEvent('Event');
    evt.initEvent('change', false, false);
    checkBoxDomElement.dispatchEvent(evt);
}}, false);", ((INTERNAL_HtmlDomElementReference)divWhereToPlaceChild).UniqueIdentifier, ((INTERNAL_HtmlDomElementReference)checkBoxDomElement).UniqueIdentifier);
                INTERNAL_HtmlDomManager.ExecuteJavaScript(javaScriptToExecute);
            }
#endif
        }
    }
}
