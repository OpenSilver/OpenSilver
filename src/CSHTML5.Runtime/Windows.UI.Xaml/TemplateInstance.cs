
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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Defines a generated instance of a FrameworkTemplate.
    /// </summary>
    public class TemplateInstance
    {
        private UIElement _templateOwner;

        /// <summary>
        /// Gets or sets the element that contains the FrameworkTemplate. Do not use this Property.
        /// </summary>
        /// <exclude/>
        public UIElement TemplateOwner
        {
            get { return _templateOwner; }
            set { _templateOwner = value; }
        }

        private FrameworkElement _templateContent;
        /// <summary>
        /// Gets or sets the visual subtree that has been generated for the FrameworkTemplate.
        /// Note: this should only be used inside the methods put as parameter in FrameworkTemplate.SetMethodToInstantiateFrameworkTemplate, to define the root of the Template's generated subtree.
        /// </summary>
        public FrameworkElement TemplateContent
        {
            get { return _templateContent; }
            set { _templateContent = value; }
        }
    }
}
