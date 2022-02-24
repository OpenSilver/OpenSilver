﻿// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Represents a data point used for a line series.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = DataPoint.StateCommonNormal, GroupName = DataPoint.GroupCommonStates)]
    [TemplateVisualState(Name = DataPoint.StateCommonMouseOver, GroupName = DataPoint.GroupCommonStates)]
    [TemplateVisualState(Name = DataPoint.StateSelectionUnselected, GroupName = DataPoint.GroupSelectionStates)]
    [TemplateVisualState(Name = DataPoint.StateSelectionSelected, GroupName = DataPoint.GroupSelectionStates)]
    [TemplateVisualState(Name = DataPoint.StateRevealShown, GroupName = DataPoint.GroupRevealStates)]
    [TemplateVisualState(Name = DataPoint.StateRevealHidden, GroupName = DataPoint.GroupRevealStates)]
    public partial class LineDataPoint : DataPoint
    {
#if !SILVERLIGHT
        /// <summary>
        /// Initializes the static members of the LineDataPoint class.
        /// </summary>
        static LineDataPoint()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineDataPoint), new FrameworkPropertyMetadata(typeof(LineDataPoint)));
        }

#endif
        /// <summary>
        /// Initializes a new instance of the LineDataPoint class.
        /// </summary>
        public LineDataPoint()
        {
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(LineDataPoint);
#endif
        }
    }
}