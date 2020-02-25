﻿
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using CSHTML5.Internal;

#if !MIGRATION
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Creates an element tree of elements.
    /// </summary>
    [ContentProperty("ContentPropertyUsefulOnlyDuringTheCompilation")]
    public partial class FrameworkTemplate : DependencyObject
    {
        internal Func<Control, TemplateInstance> _methodToInstantiateFrameworkTemplate;

        /// <summary>
        /// Creates an instance of the Template. Intented to be called for templates that have no owner, such as DataTemplates (not ControlTemplates).
        /// </summary>
        /// <returns>The instantiated template.</returns>
        internal FrameworkElement INTERNAL_InstantiateFrameworkTemplate()
        {
            if (_methodToInstantiateFrameworkTemplate != null)
            {
                TemplateInstance templateInstance = _methodToInstantiateFrameworkTemplate(null); // We pass "null" in case of DataTemplate (unlike ControlTemplate), because there is no "templateOwner" for a DataTemplate.
                return templateInstance.TemplateContent;
            }
            else
                throw new Exception("The FrameworkTemplate was not properly initialized.");
        }

        /// <summary>
        /// Creates an instance of the Template, attaches it to the Visual Tree, and calls "OnApplyTemplate". This method is intented to be called for ControlTemplates only (not DataTemplates).
        /// </summary>
        /// <param name="templateOwner">The owner of the template is the control to which the template is applied.</param>
        /// <returns>The instantiated control template.</returns>
        internal FrameworkElement INTERNAL_InstantiateAndAttachControlTemplate(Control templateOwner)
        {
#if CSHTML5BLAZOR && DEBUG
            Console.WriteLine("OPEN SILVER DEBUG: FrameworkTemplate: INTERNAL_InstantiateAndAttachControlTemplate:" +
                " template owner = " + templateOwner +
                " template owner hash = " + (templateOwner != null ? templateOwner.GetHashCode().ToString() : ""));
#endif
            if (_methodToInstantiateFrameworkTemplate != null)
            {
                if (templateOwner != null)
                {
                    // Instantiate the ControlTemplate:
                    TemplateInstance templateInstance = _methodToInstantiateFrameworkTemplate(templateOwner);

                    // Attach it:
                    INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(templateInstance.TemplateContent, templateOwner);

                    // Raise the "OnApplyTemplate" property:
                    if (templateOwner != null)
                    {
                        templateOwner.RaiseOnApplyTemplate();
                    }
#if CSHTML5BLAZOR && DEBUG
                    else
                    {
                        Console.WriteLine("OPEN SILVER DEBUG: FrameworkTemplate: INTERNAL_InstantiateAndAttachControlTemplate: template owner is null");
                    }
#endif
                    return templateInstance.TemplateContent;
                }
                else
                    throw new ArgumentNullException("templateOwner");
            }
            else
                throw new Exception("The FrameworkTemplate was not properly initialized.");
        }

        /// <summary>
        /// Sets the method that will create the tree of elements.
        /// </summary>
        /// <param name="methodToInstantiateFrameworkTemplate">The method that will create the tree of elements.</param>
        public void SetMethodToInstantiateFrameworkTemplate(Func<Control, TemplateInstance> methodToInstantiateFrameworkTemplate)
        {
            if (_isSealed)
            {
                throw new InvalidOperationException("Cannot modify a sealed " + this.GetType().Name + ". Please create a new one instead.");
            }
            _methodToInstantiateFrameworkTemplate = methodToInstantiateFrameworkTemplate;
        }

        // The following property is used during the "InsertImplicitNodes" step of the compilation, in conjunction with the "ContentProperty" attribute. The property is never used at runtime.
        /// <exclude/>
        public UIElement ContentPropertyUsefulOnlyDuringTheCompilation
        {
            get { return (UIElement)GetValue(ContentPropertyUsefulOnlyDuringTheCompilationProperty); }
            set { SetValue(ContentPropertyUsefulOnlyDuringTheCompilationProperty, value); }
        }
        /// <exclude/>
        public static readonly DependencyProperty ContentPropertyUsefulOnlyDuringTheCompilationProperty =
            DependencyProperty.Register("ContentPropertyUsefulOnlyDuringTheCompilation", typeof(UIElement), typeof(FrameworkTemplate), new PropertyMetadata(null, null));

        bool _isSealed = false;
        /// <summary>
        /// Locks the template so it cannot be changed.
        /// </summary>
        public void Seal()
        {
            _isSealed = true;
        }
       
        /// <summary>
        /// Gets a value that indicates whether this object is in an immutable state
        /// so it cannot be changed.
        /// </summary>
        /// <returns>true if this object is in an immutable state; otherwise, false.</returns>
        public bool IsSealed()
        {
            return _isSealed;
        }

    }
}
