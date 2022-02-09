

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if !MIGRATION
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace System.Windows.Media.Effects
{
    /// <summary>
    /// Provides a custom bitmap effect.
    /// </summary>
    public abstract partial class Effect : DependencyObject//: Animatable
    {
        //Note: we inherit from DependencyObject but we should only inherit from Animatable

        internal UIElement _parentUIElement;
        internal virtual void SetParentUIElement(UIElement newParent)
        {
            _parentUIElement = newParent;
        }

        /// <summary>
        /// Initializes a new instance of the System.Windows.Media.Effects.Effect class.
        /// </summary>
        protected Effect() { }

        /// <summary>
        /// When overridden in a derived class, transforms mouse input and coordinate systems
        /// through the effect.
        /// </summary>
        /// <returns>
        /// The transform to apply. The default is the identity transform.
        /// </returns>
        [OpenSilver.NotImplemented]
        protected internal virtual GeneralTransform EffectMapping => new MatrixTransform();

        ///// <summary>
        ///// Gets a System.Windows.Media.Brush that, when it is used as an input for an
        ///// System.Windows.Media.Effects.Effect, causes the bitmap of the System.Windows.UIElement
        ///// that the System.Windows.Media.Effects.Effect is applied to be that input.
        ///// </summary>
        //public static Brush ImplicitInput { get; }

        //// Returns:
        ////     A modifiable clone of this instance. The returned clone is effectively a
        ////     deep copy of the current object. The clone's System.Windows.Freezable.IsFrozen
        ////     property is false.
        ///// <summary>
        ///// Creates a modifiable clone of this System.Windows.Media.Effects.Effect object,
        ///// making deep copies of this object's values. When copying this object's dependency
        ///// properties, this method copies resource references and data bindings (which
        ///// may no longer resolve), but not animations or their current values.
        ///// </summary>
        ///// <returns></returns>
        //public Effect Clone();

        //// Returns:
        ////     A modifiable clone of the current object. The cloned object's System.Windows.Freezable.IsFrozen
        ////     property will be false even if the source's System.Windows.Freezable.IsFrozen
        ////     property was true.
        ///// <summary>
        ///// Creates a modifiable clone of this System.Windows.Media.Effects.Effect object,
        ///// making deep copies of this object's current values. Resource references,
        ///// data bindings, and animations are not copied, but their current values are
        ///// copied.
        ///// </summary>
        ///// <returns></returns>
        //public Effect CloneCurrentValue();
    }
}