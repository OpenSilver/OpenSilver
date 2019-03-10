
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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Define an expected VisualState in the contract between a Control and its
    /// ControlTemplate for use with the VisualStateManager.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class TemplateVisualStateAttribute : Attribute
    {
        /// <summary>
        /// Name of the VisualState.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the VisualStateGroup containing this state.
        /// </summary>
        public string GroupName
        {
            get;
            set;
        }
    }
}
