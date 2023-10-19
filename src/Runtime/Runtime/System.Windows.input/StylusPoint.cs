
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

namespace System.Windows.Input
{
    /// <summary>
    /// Represents a single point collected while the user is entering ink strokes with
    /// the stylus or mouse.
    /// </summary>
    public struct StylusPoint
    {
        private double _x;
        private double _y;
        private float _pressurefactor;

        /// <summary>
        /// Initializes a new instance of the <see cref="StylusPoint" /> class.
        /// </summary>
        /// <param name="x">
        /// The x-coordinate of the <see cref="StylusPoint" /> in a pixel grid.
        /// </param>
        /// <param name="y">
        /// The y-coordinate of the <see cref="StylusPoint" /> in a pixel grid.
        /// </param>
        public StylusPoint(double x, double y)
        {
            _x = x;
            _y = y;
            _pressurefactor = 0.5f;
        }

        /// <summary>
        /// Gets or sets the value for the x-coordinate of the <see cref="StylusPoint" />.
        /// </summary>
        /// <returns>
        /// The x-coordinate of the <see cref="StylusPoint" /> in a pixel grid. The default is 0.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="X" /> is set a value that evaluates to infinity or a value that is not a number.
        /// </exception>
        public double X
        {
            get => _x;
            set
            {
                if (double.IsNaN(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(X));
                }

                _x = value;
            }
        }

        /// <summary>
        /// Gets or sets the value for the y-coordinate of the <see cref="StylusPoint" />.
        /// </summary>
        /// <returns>
        /// The y-coordinate of the <see cref="StylusPoint" /> in a pixel grid. The default is 0.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="Y" /> is set a value that evaluates to infinity or a value that is not a number.
        /// </exception>
        public double Y
        {
            get => _y;
            set
            {
                if (double.IsNaN(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(Y));
                }

                _y = value;
            }
        }

        /// <summary>
        /// Gets or sets the pressure factor of the stylus on the screen.
        /// </summary>
        /// <returns>
        /// The pressure factor of the stylus on the screen. The default is 0.5.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="PressureFactor" />is set to a value that is less than 0 or greater than 1.0.
        /// </exception>
        public float PressureFactor
        {
            get
            {
                if (_pressurefactor > 1.0)
                {
                    return 1f;
                }

                if (_pressurefactor < 0.0)
                {
                    return 0.0f;
                }

                return _pressurefactor;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(PressureFactor));
                }

                _pressurefactor = value;
            }
        }
    }
}
