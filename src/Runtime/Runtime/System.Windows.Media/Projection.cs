
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

using System;

namespace System.Windows.Media
{
    /// <summary>
    /// Provides a base class for projections, which describe how to transform an object 
    /// in 3-D space using perspective transforms.
    /// </summary>
    public abstract class Projection : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Projection"/> class.
        /// </summary>
        protected Projection()
        {
        }

        /// <summary>
        /// Occurs when the projection changes.
        /// </summary>
        internal event EventHandler Changed;

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        protected void RaiseChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets the CSS transform string for this projection.
        /// </summary>
        /// <param name="elementWidth">The width of the element being projected.</param>
        /// <param name="elementHeight">The height of the element being projected.</param>
        /// <returns>A CSS transform string, or an empty string if no transform is applied.</returns>
        internal abstract string GetCssTransform(double elementWidth, double elementHeight);

        /// <summary>
        /// Gets the CSS perspective value for this projection.
        /// </summary>
        /// <returns>A CSS perspective value in pixels, or 0 if no perspective is applied.</returns>
        internal virtual double GetPerspective() => 1000; // Default perspective distance in pixels
    }
}

