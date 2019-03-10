
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


#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    ///  Represents a button that allows a user to select a single option from a group
    ///  of options.
    /// </summary>
    public class RadioButton : ToggleButton
    {
        /// <summary>
        /// Initializes a new instance of the RadioButton class.
        /// </summary>
        public RadioButton()
        {
            _reactsToKeyboardEventsWhenFocused = false; //todo: change this if there is a Template and if a Template is added later on, make sure to start the mechanic properly (see where it is defined/used).
        }

        protected override void SetDefaultStyle() // Overridden in CheckBox and RadioButton
        {
            // No default style at the moment because we use the HTML5 native checkbox.
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            return INTERNAL_CheckBoxAndRadioButtonHelpers.CreateDomElement(this, "radio", parentRef, out domElementWhereToPlaceChildren);
        }

#if !BRIDGE
        [JSIL.Meta.JSReplacement("true")]
#else
        [Template("true")]
#endif
        private static bool IsRunningInJavaScript() //must be static to work properly
        {
            return false;
        }


        internal override void SubscribeToClickEventForChildContainerDiv(dynamic divWhereToPlaceChild, dynamic checkBoxDomElement)
        {
            if (INTERNAL_CheckBoxAndRadioButtonHelpers.IsRunningInJavaScript())
            {
                //Note: if checked is true then we don't do anything, we only want to check the radioButton when it was not beforehand. 
#if !BRIDGE               
                JSIL.Verbatim.Expression(@"
$0.addEventListener('click', function(e) {
    if($1.checked === true)
    {
        
    }
    else
    {
        $1.checked = true;
        var evt = document.createEvent('Event');
        evt.initEvent('change', false, false);
        $1.dispatchEvent(evt);
    }
}, false);", divWhereToPlaceChild, checkBoxDomElement);
#else
                Script.Write(@"
$0.addEventListener('click', function(e) {
    if($1.checked === true)
    {
        
    }
    else
    {
        $1.checked = true;
        var evt = document.createEvent('Event');
        evt.initEvent('change', false, false);
        $1.dispatchEvent(evt);
    }
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
var radioButtonDomElement = document.getElementById(""{1}"");
    if(radioButtonDomElement.checked === true)
    {{
    }}
    else
    {{
        radioButtonDomElement.checked = true;
        var evt = document.createEvent('Event');
        evt.initEvent('change', false, false);
        radioButtonDomElement.dispatchEvent(evt);
    }}
}}, false);", ((INTERNAL_HtmlDomElementReference)divWhereToPlaceChild).UniqueIdentifier, ((INTERNAL_HtmlDomElementReference)checkBoxDomElement).UniqueIdentifier);
                INTERNAL_HtmlDomManager.ExecuteJavaScript(javaScriptToExecute);
            }
#endif
        }


        #region new version of handling IsChecked

        
        internal void ChangedEventReceived()
        {
            bool newIsChecked = Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("$0.checked", INTERNAL_OptionalSpecifyDomElementConcernedByFocus)); //Note: this should be sufficient since a RadioButton cannot be in an indeterminate state.
            if(IsChecked != newIsChecked)
            {
                IsChecked = newIsChecked;
            }
            if (newIsChecked == true)
            {
                UnselectOtherRadioButtonsFromSameGroup();
            }
            else
            {
                //if the RadioButton has just been unchecked, we set isLastChecked to undefined so that we do not trigger the "change" event for no reason when another radioButton will be selected:
                if (INTERNAL_CheckBoxAndRadioButtonHelpers.IsRunningInJavaScript())
                {
#if !BRIDGE
                    JSIL.Verbatim.Expression(@"$0.isLastChecked = undefined;", INTERNAL_OptionalSpecifyDomElementConcernedByFocus);
#else
                    Script.Write(@"{0}.isLastChecked = undefined;", INTERNAL_OptionalSpecifyDomElementConcernedByFocus);
#endif
                }
                else
                {
                    string javaScriptCodeToExecute = string.Format(@"document.getElementById(""{0}"").isLastChecked = undefined;", ((INTERNAL_HtmlDomElementReference)INTERNAL_OptionalSpecifyDomElementConcernedByFocus).UniqueIdentifier);
                    INTERNAL_HtmlDomManager.ExecuteJavaScriptWithResult(javaScriptCodeToExecute);
                }
            }
        }

        private void UnselectOtherRadioButtonsFromSameGroup()
        {
            string groupName = GroupName;
            if (string.IsNullOrWhiteSpace(groupName))
            {
                groupName = ((UIElement)INTERNAL_VisualParent).INTERNAL_ChildrenRadioButtonDefaultName;
            }

            //if the RadioButton has just been checked, we trigger the change event on the RadioButton of the same group that was checked before:
            if (INTERNAL_CheckBoxAndRadioButtonHelpers.IsRunningInJavaScript())
            {
#if !BRIDGE
                JSIL.Verbatim.Expression(@"
                        var radios = document.getElementsByName( $0 );
                        for(var i = 0; i < radios.length; i++ ) {
                            if( radios[i].isLastChecked === true && radios[i] !== $1) {
                                radios[i].checked = false;
                                radios[i].isLastChecked = undefined;
                                var evt = document.createEvent('Event');
                                evt.initEvent('change', false, false);
                                radios[i].dispatchEvent(evt);
                            }
                        }
                        $1.isLastChecked = true;
                    ", groupName, INTERNAL_OptionalSpecifyDomElementConcernedByFocus);
#else

                Script.Write(@"
var radios = document.getElementsByName( $0 );
                        for(var i = 0; i < radios.length; i++ ) {
                            if( radios[i].isLastChecked === true && radios[i] !== $1) {
                                radios[i].checked = false;
                                radios[i].isLastChecked = undefined;
                                var evt = document.createEvent('Event');
                                evt.initEvent('change', false, false);
                                radios[i].dispatchEvent(evt);
                            }
                        }
                        $1.isLastChecked = true;
                    ", groupName, INTERNAL_OptionalSpecifyDomElementConcernedByFocus);
#endif
            }
#if !BRIDGE
            else
            {
                string javaScriptCodeToExecute = string.Format(@"
                        var radios = document.getElementsByName( ""{0}"" );
                        var newlySelectedRadio = document.getElementById(""{1}"");
                            for(var i = 0; i < radios.length; i++ ) {{
                                if( radios[i].isLastChecked === true  && radios[i] !== newlySelectedRadio) {{
                                    radios[i].checked = false;
                                    radios[i].isLastChecked = undefined;
                                    var evt = document.createEvent('Event');
                                    evt.initEvent('change', false, false);
                                    radios[i].dispatchEvent(evt);
                                }}
                            }}
                            newlySelectedRadio.isLastChecked = true;
                        ", groupName, ((INTERNAL_HtmlDomElementReference)INTERNAL_OptionalSpecifyDomElementConcernedByFocus).UniqueIdentifier);
                INTERNAL_HtmlDomManager.ExecuteJavaScriptWithResult(javaScriptCodeToExecute);
            }
#endif
        }

        protected override void UpdateDomBasedOnCheckedState(bool? isChecked)
        {
            INTERNAL_CheckBoxAndRadioButtonHelpers.UpdateDomBasedOnCheckedState(this, isChecked);
            //specifically sending the "change" event on the element that was changed programmatically so that it can then do the rest (setting the other radiobuttons' IsChecked to false and stuff).
            CSHTML5.Interop.ExecuteJavaScript(@"var evt = document.createEvent('Event');
                                    evt.initEvent('change', false, false);
                                    $0.dispatchEvent(evt);", INTERNAL_OptionalSpecifyDomElementConcernedByFocus);
        }

#endregion

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            bool isUsingNativeHtml5RadioButtonRatherThanAControlTemplate = (this.INTERNAL_OptionalSpecifyDomElementConcernedByFocus != null); // Faster than checking if (radioButton.Template != null) because the latter is a DependencyProperty.
            if (isUsingNativeHtml5RadioButtonRatherThanAControlTemplate)
            {
                if (string.IsNullOrWhiteSpace(GroupName))
                {
                    INTERNAL_HtmlDomManager.SetDomElementAttribute(this.INTERNAL_OptionalSpecifyDomElementConcernedByFocus, "name", ((UIElement)INTERNAL_VisualParent).INTERNAL_ChildrenRadioButtonDefaultName, forceSimulatorExecuteImmediately: true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the name that specifies which RadioButton controls are mutually
        /// exclusive.
        /// </summary>
        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }
        /// <summary>
        /// Identifies the GroupName dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(RadioButton), new PropertyMetadata(null, GroupName_Changed));

        private static void GroupName_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadioButton radio = (RadioButton)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(radio))
            {
                bool isUsingNativeHtml5RadioButtonRatherThanAControlTemplate = (radio.INTERNAL_OptionalSpecifyDomElementConcernedByFocus != null); // Faster than checking if (radioButton.Template != null) because the latter is a DependencyProperty.
                if (isUsingNativeHtml5RadioButtonRatherThanAControlTemplate)
                {
                    if (!string.IsNullOrWhiteSpace((string)e.NewValue))
                    {
                        INTERNAL_HtmlDomManager.SetDomElementAttribute(radio.INTERNAL_OptionalSpecifyDomElementConcernedByFocus, "name", radio.GroupName, forceSimulatorExecuteImmediately: true);
                    }
                    else
                    {
                        INTERNAL_HtmlDomManager.SetDomElementAttribute(radio.INTERNAL_OptionalSpecifyDomElementConcernedByFocus, "name", ((UIElement)radio.INTERNAL_VisualParent).INTERNAL_ChildrenRadioButtonDefaultName, forceSimulatorExecuteImmediately: true);
                    }
                }
            }
        }
    }
}