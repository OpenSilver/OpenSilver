
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
    /// This enum describes when updates (target-to-source data flow)
    /// happen in a given Binding.
    /// </summary>
    public enum UpdateSourceTrigger
    {
        /// <summary>
        /// Obtain trigger from target property default
        /// </summary>
        Default,

        /// <summary>
        /// Update whenever the target property changes
        /// </summary>
        PropertyChanged,

        //// <summary>
        //// Update only when target element loses focus, or when Binding deactivates
        //// </summary>
        //LostFocus,

        /// <summary>
        /// Update only by explicit call to BindingExpression.UpdateSource()
        /// </summary>
        Explicit
    }
}