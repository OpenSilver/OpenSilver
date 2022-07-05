namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>Represents a data point used for a scatter series.</summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Selected")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Unselected")]
    [TemplateVisualState(GroupName = "RevealStates", Name = "Hidden")]
    [TemplateVisualState(GroupName = "RevealStates", Name = "Shown")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
    public class ScatterDataPoint : DataPoint
    {
        /// <summary>
        /// Initializes a new instance of the ScatterDataPoint class.
        /// </summary>
        public ScatterDataPoint()
        {
            this.DefaultStyleKey = (object)typeof(ScatterDataPoint);
        }
    }
}