
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Media.Effects
{
    /// <summary>
    /// Provides a custom bitmap effect.
    /// </summary>
    public abstract class Effect : DependencyObject//: Animatable
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

        //// Returns:
        ////     The transform to apply. The default is the identity transform.
        ///// <summary>
        ///// When overridden in a derived class, transforms mouse input and coordinate
        ///// systems through the effect.
        ///// </summary>
        //protected internal virtual GeneralTransform EffectMapping { get; }

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