
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

#if !MIGRATION
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace System.Windows.Media.Effects
{
    /// <summary>
    /// Provides a base class for all bitmap effects.
    /// </summary>
    public abstract class Effect : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Effect"/> class.
        /// </summary>
        protected Effect() { }

        /// <summary>
        /// Gets a <see cref="Brush"/> that, when it is used as an input for an <see cref="Effect"/>,
        /// causes the bitmap of the <see cref="UIElement"/> that the <see cref="Effect"/>
        /// is applied to be that input.
        /// </summary>
        /// <returns>
        /// The <see cref="Brush"/> that acts as the input.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static Brush ImplicitInput { get; }

        /// <summary>
        /// When overridden in a derived class, transforms mouse input and coordinate systems
        /// through the effect.
        /// </summary>
        /// <returns>
        /// The transform to apply. The default is the identity transform.
        /// </returns>
        [OpenSilver.NotImplemented]
        protected internal virtual GeneralTransform EffectMapping => new MatrixTransform();

        internal event EventHandler Changed;

        internal void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);

        internal virtual void Render(UIElement renderTarget) { }

        internal virtual void Clean(UIElement renderTarget) { }
    }
}