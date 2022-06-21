using System;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    public struct StylusPoint
    {
        internal double _x;
        internal double _y;
        internal float _pressurefactor;

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Input.StylusPoint" /> class. </summary>
        /// <param name="x">The x-coordinate of the <see cref="T:System.Windows.Input.StylusPoint" /> in a pixel grid.</param>
        /// <param name="y">The y-coordinate of the <see cref="T:System.Windows.Input.StylusPoint" /> in a pixel grid.</param>
        public StylusPoint(double x, double y)
        {
            this._x = x;
            this._y = y;
            this._pressurefactor = 0.5f;
        }

        /// <summary>Gets or sets the value for the x-coordinate of the <see cref="T:System.Windows.Input.StylusPoint" />.</summary>
        /// <returns>The x-coordinate of the <see cref="T:System.Windows.Input.StylusPoint" /> in a pixel grid. The default is 0.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <see cref="P:System.Windows.Input.StylusPoint.X" /> is set a value that evaluates to infinity or a value that is not a number.</exception>
        public double X
        {
            get
            {
                return this._x;
            }
            set
            {
                if (double.IsNaN(value))
                    throw new ArgumentOutOfRangeException(nameof(X));
                this._x = value;
            }
        }

        /// <summary>Gets or sets the value for the y-coordinate of the <see cref="T:System.Windows.Input.StylusPoint" />.</summary>
        /// <returns>The y-coordinate of the <see cref="T:System.Windows.Input.StylusPoint" /> in a pixel grid. The default is 0.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <see cref="P:System.Windows.Input.StylusPoint.Y" /> is set a value that evaluates to infinity or a value that is not a number.</exception>
        public double Y
        {
            get
            {
                return this._y;
            }
            set
            {
                if (double.IsNaN(value))
                    throw new ArgumentOutOfRangeException(nameof(Y));
                this._y = value;
            }
        }

        /// <summary>Gets or sets the pressure factor of the stylus on the screen.</summary>
        /// <returns>The pressure factor of the stylus on the screen. The default is 0.5.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <see cref="P:System.Windows.Input.StylusPoint.PressureFactor" />is set to a value that is less than 0 or greater than 1.0.</exception>
        public float PressureFactor
        {
            get
            {
                if ((double)this._pressurefactor > 1.0)
                    return 1f;
                if ((double)this._pressurefactor < 0.0)
                    return 0.0f;
                return this._pressurefactor;
            }
            set
            {
                if ((double)value < 0.0 || (double)value > 1.0)
                    throw new ArgumentOutOfRangeException(nameof(PressureFactor));
                this._pressurefactor = value;
            }
        }
    }
}
