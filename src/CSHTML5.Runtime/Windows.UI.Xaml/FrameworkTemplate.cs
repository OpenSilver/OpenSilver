

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
            if (templateOwner == null)
            {
                throw new ArgumentNullException("templateOwner");
            }

            if (_methodToInstantiateFrameworkTemplate != null)
            {
                templateOwner.INTERNAL_IsTemplated = true;

                // Instantiate the ControlTemplate:
                TemplateInstance templateInstance = _methodToInstantiateFrameworkTemplate(templateOwner);

                // Attach it:
#if REWORKLOADED
                templateOwner.AddVisualChild(templateInstance.TemplateContent);
#else
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(templateInstance.TemplateContent, templateOwner);
#endif

                // Raise the "OnApplyTemplate" property:
                if (templateOwner != null)
                {
                    templateOwner.RaiseOnApplyTemplate();
                }
                return templateInstance.TemplateContent;
            }
            else
            {
                templateOwner.INTERNAL_IsTemplated = false;
                return null;
            }
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
            DependencyProperty.Register("ContentPropertyUsefulOnlyDuringTheCompilation", typeof(UIElement), typeof(FrameworkTemplate), new PropertyMetadata(null));

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
