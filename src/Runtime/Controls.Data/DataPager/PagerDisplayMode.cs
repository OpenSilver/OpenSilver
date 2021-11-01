//-----------------------------------------------------------------------
// <copyright file="PagerDisplayMode.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// PagerDisplayMode Enum
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum PagerDisplayMode
    {
        /// <summary>
        /// Shows the First and Last buttons + the numeric display
        /// </summary>
        FirstLastNumeric,

        /// <summary>
        /// Shows the First, Last, Previous, Next buttons
        /// </summary>
        FirstLastPreviousNext,

        /// <summary>
        /// Shows the First, Last, Previous, Next buttons + the numeric display
        /// </summary>
        FirstLastPreviousNextNumeric,

        /// <summary>
        /// Shows the numeric display
        /// </summary>
        Numeric,

        /// <summary>
        /// Shows the Previous and Next buttons
        /// </summary>
        PreviousNext,

        /// <summary>
        /// Shows the Previous and Next buttons + the numeric display
        /// </summary>
        PreviousNextNumeric
    }
}
