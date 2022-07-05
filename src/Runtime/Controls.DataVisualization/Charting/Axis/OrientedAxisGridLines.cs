
using System.Collections.Generic;
using System.Linq;
using System.Windows.Shapes;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// This control draws gridlines with the help of an axis.
    /// </summary>
    internal class OrientedAxisGridLines : DisplayAxisGridLines
    {
        /// <summary>A pool of grid lines.</summary>
        private ObjectPool<Line> _gridLinePool;

        /// <summary>
        /// Initializes a new instance of the OrientedAxisGridLines class.
        /// </summary>
        /// <param name="displayAxis">The axis to draw grid lines for.</param>
        public OrientedAxisGridLines(DisplayAxis displayAxis)
          : base(displayAxis)
        {
            this._gridLinePool = new ObjectPool<Line>((Func<Line>)(() =>
            {
                return new Line()
                {
                    Style = this.Axis.GridLineStyle
                };
            }));
        }

        /// <summary>Draws the grid lines.</summary>
        protected override void Invalidate()
        {
            this._gridLinePool.Reset();
            try
            {
                IList<UnitValue> list = (IList<UnitValue>)this.Axis.InternalGetMajorGridLinePositions().ToList<UnitValue>();
                this.Children.Clear();
                double num1 = Math.Max(Math.Round(this.ActualHeight - 1.0), 0.0);
                double num2 = Math.Max(Math.Round(this.ActualWidth - 1.0), 0.0);
                for (int index = 0; index < list.Count; ++index)
                {
                    double d = list[index].Value;
                    if (!double.IsNaN(d))
                    {
                        Line line = this._gridLinePool.Next();
                        if (this.Axis.Orientation == AxisOrientation.Y)
                        {
                            line.Y1 = line.Y2 = num1 - Math.Round(d - line.StrokeThickness / 2.0);
                            line.X1 = 0.0;
                            line.X2 = num2;
                        }
                        else if (this.Axis.Orientation == AxisOrientation.X)
                        {
                            line.X1 = line.X2 = Math.Round(d - line.StrokeThickness / 2.0);
                            line.Y1 = 0.0;
                            line.Y2 = num1;
                        }
                        if (line.StrokeThickness % 2.0 > 0.0)
                        {
                            line.SetValue(Canvas.LeftProperty, (object)0.5);
                            line.SetValue(Canvas.TopProperty, (object)0.5);
                        }
                        this.Children.Add((UIElement)line);
                    }
                }
            }
            finally
            {
                this._gridLinePool.Done();
            }
        }
    }
}

