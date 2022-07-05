// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{
    /// <summary>Represents a data point used for an area series.</summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(GroupName = "RevealStates", Name = "Shown")]
    [TemplateVisualState(GroupName = "RevealStates", Name = "Hidden")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Selected")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Unselected")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    public class AreaDataPoint : DataPoint
    {
        /// <summary>
        /// Initializes a new instance of the AreaDataPoint class.
        /// </summary>
        public AreaDataPoint()
        {
            this.DefaultStyleKey = (object)typeof(AreaDataPoint);
        }
    }
}