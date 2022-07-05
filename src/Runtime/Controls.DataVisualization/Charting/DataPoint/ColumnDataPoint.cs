namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>Represents a data point used for a column series.</summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Unselected")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Selected")]
    [TemplateVisualState(GroupName = "RevealStates", Name = "Shown")]
    [TemplateVisualState(GroupName = "RevealStates", Name = "Hidden")]
    public class ColumnDataPoint : DataPoint
    {
        /// <summary>
        /// Initializes a new instance of the ColumnDataPoint class.
        /// </summary>
        public ColumnDataPoint()
        {
            this.DefaultStyleKey = (object)typeof(ColumnDataPoint);
        }
    }
}