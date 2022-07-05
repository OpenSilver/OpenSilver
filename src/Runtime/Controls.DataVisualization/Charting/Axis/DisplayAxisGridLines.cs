using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;



namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// This control draws gridlines with the help of an axis.
    /// </summary>
    internal abstract class DisplayAxisGridLines : Canvas, IAxisListener
    {
        /// <summary>
        /// The field that stores the axis that the grid lines are connected to.
        /// </summary>
        private DisplayAxis _axis;

        /// <summary>Gets the axis that the grid lines are connected to.</summary>
        public DisplayAxis Axis
        {
            get
            {
                return this._axis;
            }
            private set
            {
                if (this._axis == value)
                    return;
                DisplayAxis axis = this._axis;
                this._axis = value;
                if (axis != this._axis)
                    this.OnAxisPropertyChanged(axis, value);
            }
        }

        /// <summary>AxisProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnAxisPropertyChanged(DisplayAxis oldValue, DisplayAxis newValue)
        {
            Debug.Assert(newValue != null, "Don't set the axis property to null.");
            if (newValue != null)
                newValue.RegisteredListeners.Add((IAxisListener)this);
            if (oldValue == null)
                return;
            oldValue.RegisteredListeners.Remove((IAxisListener)this);
        }

        /// <summary>
        /// Instantiates a new instance of the DisplayAxisGridLines class.
        /// </summary>
        /// <param name="axis">The axis used by the DisplayAxisGridLines.</param>
        public DisplayAxisGridLines(DisplayAxis axis)
        {
            this.Axis = axis;
            this.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
        }

        /// <summary>
        /// Redraws grid lines when the size of the control changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private new void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Invalidate();
        }

        /// <summary>Redraws grid lines when the axis is invalidated.</summary>
        /// <param name="axis">The invalidated axis.</param>
        public void AxisInvalidated(IAxis axis)
        {
            this.Invalidate();
        }

        /// <summary>Draws the grid lines.</summary>
        protected abstract void Invalidate();
    }
}
