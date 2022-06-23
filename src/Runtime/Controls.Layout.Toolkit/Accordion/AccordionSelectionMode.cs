// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Defines the minimum and maximum number of selected items allowed in an Accordion control.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
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