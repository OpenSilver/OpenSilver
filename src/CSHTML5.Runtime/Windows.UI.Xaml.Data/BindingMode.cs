
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


#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    /// <summary>
    /// Describes how the data propagates in a binding.
    /// </summary>
    public enum BindingMode
    {
        /// <summary>
        /// Updates the target property when the binding is created. Changes to the source object can also propagate to the target.
        /// </summary>
        OneWay = 1,
        /// <summary>
        /// Updates the target property when the binding is created.
        /// </summary>
        OneTime = 2,
        /// <summary>
        /// Updates either the target or the source object when either changes. When the binding is created, the target property is updated from the source.
        /// </summary>
        TwoWay = 3,

    }
}