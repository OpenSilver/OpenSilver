// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Windows.Controls
{
    /// <summary>
    /// The source of the error, for error management
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum ValidationSummaryItemType
    {
        /// <summary>
        /// The error came from object level validation
        /// </summary>
        ObjectError = 1,

        /// <summary>
        /// The error came from the binding engine, which exposes only a single error at a time
        /// </summary>
        PropertyError = 2
    }
}
