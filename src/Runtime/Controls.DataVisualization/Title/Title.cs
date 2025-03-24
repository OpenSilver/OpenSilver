// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls.DataVisualization
{
    /// <summary>
    /// Represents the title of a data visualization control.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public partial class Title : ContentControl
    {
        static Title()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Title), new PropertyMetadata(typeof(Title)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Title"/> class.
        /// </summary>
        public Title() { }
    }
}