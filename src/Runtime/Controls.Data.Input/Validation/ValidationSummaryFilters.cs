// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// An enum to specify the error filtering level.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [Flags]
    public enum ValidationSummaryFilters
    {
        /// <summary>
        /// None of the errors are displayed in the ValidationSummary
        /// </summary>
        None = 0,

        /// <summary>
        /// ValidationSummary only displays the object level errors
        /// </summary>
        ObjectErrors = 1,

        /// <summary>
        /// ValidationSummary only displays the property level errors
        /// </summary>
        PropertyErrors = 2,

        /// <summary>
        /// ValidationSummary displays all errors
        /// </summary>
        All = ObjectErrors | PropertyErrors
    }
}
