// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls
{
    /// <summary>
    /// Determines the behavior of a DomainUpDown control when a user sets a 
    /// value not included in the domain.
    /// </summary>
    public enum InvalidInputAction
    {
        /// <summary>
        /// Once a user sets a value not included in the domain, the DomainUpDown 
        /// control will use the FallbackItem property as the selected item. If 
        /// FallbackItem is not specified the first item in the DomainUpDown Items 
        /// collection will be used.
        /// </summary>
        UseFallbackItem = 0,

        /// <summary>
        /// Once a user sets a value not included in the domain, the 
        /// DomainUpDown control cannot lose focus.
        /// </summary>
        /// <remarks>There are situations that the control can not regain focus.</remarks>
        TextBoxCannotLoseFocus = 1,
    }
}
