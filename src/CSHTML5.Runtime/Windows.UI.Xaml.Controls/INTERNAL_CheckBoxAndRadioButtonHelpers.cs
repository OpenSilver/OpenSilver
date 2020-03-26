

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


using CSHTML5.Internal;
using System;
#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
using CSHTML5;
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
    internal static class INTERNAL_CheckBoxAndRadioButtonHelpers
    {
#if !BRIDGE
        [JSReplacement("true")]
#else
        [Template("true")]
#endif
        internal static bool IsRunningInJavaScript()
        {
            return false;
        }

        internal static object CreateDomElement(ToggleButton checkBoxOrRadioButton, string domInputType, object parentRef, out object domElementWhereToPlaceChildren)
        {
            object outerDiv;
            var outerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, checkBoxOrRadioButton, out outerDiv);

            object divInBetween;
            var divInBetweenStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", outerDiv, checkBoxOrRadioButton, out divInBetween);

            object innerElement;
            var innerElementStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("input", divInBetween, checkBoxOrRadioButton, out innerElement);

            INTERNAL_HtmlDomManager.SetDomElementAttribute(innerElement, "type", domInputType, forceSimulatorExecuteImmediately: true);
            //we simulate a Horizontal StackPanel (1/2):
            divInBetweenStyle.position = "relative";
            divInBetweenStyle.display = "table-cell";
            divInBetweenStyle.height = "100%";

            checkBoxOrRadioButton.INTERNAL_OptionalSpecifyDomElementConcernedByFocus = innerElement;
            checkBoxOrRadioButton.INTERNAL_OptionalSpecifyDomElementConcernedByIsEnabled = innerElement;

            object divWhereToPlaceChild;
            var divWhereToPlaceChildStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", outerDiv, checkBoxOrRadioButton, out divWhereToPlaceChild);
            //we simulate a Horizontal StackPanel (2/2):
            divWhereToPlaceChildStyle.position = "relative";
            divWhereToPlaceChildStyle.display = "table-cell";
            divWhereToPlaceChildStyle.height = "100%";


            if (INTERNAL_HtmlDomManager.IsInternetExplorer() && domInputType == "checkbox")
            {
                //Note: we add these event handlers because IE does not fire the change event when clicking to go out of the Indeterminate state.
                INTERNAL_EventsHelper.AttachToDomEvents("click", innerElement, (Action<object>)(e =>
                {
                    IsCheckedValueChanged(checkBoxOrRadioButton);
                }));
                INTERNAL_EventsHelper.AttachToDomEvents("click", divWhereToPlaceChild, (Action<object>)(e =>
                {
                    IsCheckedValueChanged(checkBoxOrRadioButton);
                }));
            }
            else
            {
                SubscribeToBasicEventsForRadioButton(checkBoxOrRadioButton, innerElement, divWhereToPlaceChild);
            }

            domElementWhereToPlaceChildren = divWhereToPlaceChild;
            return outerDiv;
        }

        internal static void SubscribeToBasicEventsForRadioButton(ToggleButton checkBoxOrRadioButton, object innerElement, object divWhereToPlaceChild)
        {
            checkBoxOrRadioButton.SubscribeToClickEventForChildContainerDiv(divWhereToPlaceChild, innerElement);
            if (checkBoxOrRadioButton is RadioButton)
            {
                INTERNAL_EventsHelper.AttachToDomEvents("change", innerElement, (Action<object>)(e =>
                {
                    ((RadioButton)checkBoxOrRadioButton).ChangedEventReceived();
                }));
            }
            else
            {
                INTERNAL_EventsHelper.AttachToDomEvents("change", innerElement, (Action<object>)(e =>
                {
                    IsCheckedValueChanged(checkBoxOrRadioButton);
                }));
            }
        }

        internal static void IsCheckedValueChanged(ToggleButton checkBoxOrRadioButton)
        {
            // We arrive here because the DOM has signaled a changed in the Checked state.

            // First, we ensure that it is the user who has clicked, not a programmatic change of the DOM:
            if (!checkBoxOrRadioButton.INTERNAL_IsCodeProgrammaticallyChangingIsChecked)
            {
                // Then we make sure the control is in the visual tree:
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(checkBoxOrRadioButton))
                {
                    // We get the value:
                    //todo: find out why we got the value from the dom element directly (was it for performance purposes?)
                    //Note: I switched to getting the c# value because we have no way of knowing if the previous value was false or indeterminate from the dom only (clicking sets indeterminate to false so we cannot know if it was tru before clicking)

                    //bool isChecked = (INTERNAL_HtmlDomManager.GetDomElementAttribute(checkBoxOrRadioButton.INTERNAL_OptionalSpecifyDomElementConcernedByFocus, "checked") == true);
                    bool? formerIsChecked = checkBoxOrRadioButton.IsChecked;
                    checkBoxOrRadioButton.INTERNAL_IsCodeProgrammaticallyChangingIsChecked = true; //To prevent reentrance (infinite loop) when user clicks.
                    if (formerIsChecked == null)
                    {
                        checkBoxOrRadioButton.SetLocalValue(ToggleButton.IsCheckedProperty, false); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay
                    }
                    else
                    {
                        if (checkBoxOrRadioButton.IsThreeState && formerIsChecked == true)
                        {
                            checkBoxOrRadioButton.SetLocalValue(ToggleButton.IsCheckedProperty, null); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay
                        }
                        else
                        {
                            checkBoxOrRadioButton.SetLocalValue(ToggleButton.IsCheckedProperty, !(bool)formerIsChecked); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay
                        }
                    }
                    checkBoxOrRadioButton.INTERNAL_IsCodeProgrammaticallyChangingIsChecked = false;
                }
            }
        }

        internal static void UpdateDomBasedOnCheckedState(ToggleButton checkBoxOrRadioButton, bool? isChecked)
        {
            if (isChecked == null)
            {
                INTERNAL_HtmlDomManager.SetDomElementProperty(checkBoxOrRadioButton.INTERNAL_OptionalSpecifyDomElementConcernedByFocus, "indeterminate", true, forceSimulatorExecuteImmediately: true);
            }
            else if (isChecked == false)
            {
                //we make sure the checkbox is not stuck on indeterminate
                INTERNAL_HtmlDomManager.SetDomElementProperty(checkBoxOrRadioButton.INTERNAL_OptionalSpecifyDomElementConcernedByFocus, "indeterminate", false, forceSimulatorExecuteImmediately: true);

                //INTERNAL_HtmlDomManager.RemoveDomElementAttribute(checkBoxOrRadioButton.INTERNAL_OptionalSpecifyDomElementConcernedByFocus, "checked", forceSimulatorExecuteImmediately: true);
                //Note: I think the checked property of the checkbox is not considered as an attribute in js.
                INTERNAL_HtmlDomManager.SetDomElementProperty(checkBoxOrRadioButton.INTERNAL_OptionalSpecifyDomElementConcernedByFocus, "checked", false, forceSimulatorExecuteImmediately: true);
            }
            else
            {
                INTERNAL_HtmlDomManager.SetDomElementProperty(checkBoxOrRadioButton.INTERNAL_OptionalSpecifyDomElementConcernedByFocus, "indeterminate", false, forceSimulatorExecuteImmediately: true);
                INTERNAL_HtmlDomManager.SetDomElementProperty(checkBoxOrRadioButton.INTERNAL_OptionalSpecifyDomElementConcernedByFocus, "checked", true, forceSimulatorExecuteImmediately: true);
            }
        }
    }
}
