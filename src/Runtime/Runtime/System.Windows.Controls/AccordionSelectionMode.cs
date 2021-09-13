

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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Defines the minimum and maximum number of selected items allowed in an Accordion control.
    /// </summary>
    public enum AccordionSelectionMode
    {
        /// <summary>
        /// Exactly one item must be selected in the Accordion.
        /// </summary>
        One,

        /// <summary>
        /// At least one item must be selected in the Accordion.
        /// </summary>
        OneOrMore,

        /// <summary>
        /// No more than one item can be selected in the accordion.
        /// </summary>
        ZeroOrOne,

        /// <summary>
        /// Any number of  items can be selected in the Accordion.
        /// </summary>
        ZeroOrMore
    }
}
