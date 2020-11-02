

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


#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a collection of collapsed and expanded AccordionItem controls.
    /// </summary>
    public class Accordion : ItemsControl, IUpdateVisualState
    {
        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unselects all the AccordionItems in the Accordion control.
        /// </summary>
        /// <remarks>
        /// If the Accordion SelectionMode is Zero or ZeroOrMore all
        /// AccordionItems would be Unselected. If SelectionMode is One or
        /// OneOrMode  than all items would be Unselected and selected. Only the
        /// first AccordionItem would still be selected.
        /// </remarks>
        public void UnselectAll()
        {
            UpdateAccordionItemsSelection(false);
        }

        /// <summary>
        /// Updates all accordionItems to be selected or unselected.
        /// </summary>
        /// <param name="selectedValue">
        /// True to select all items, false to unselect.
        /// </param>
        /// <remarks>
        /// Will not attempt to change a locked accordionItem.
        /// </remarks>
        private void UpdateAccordionItemsSelection(bool selectedValue)
        {
            throw new NotImplementedException();
        }
    }
}
#endif