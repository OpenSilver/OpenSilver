
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
using System.Diagnostics;
using System.Windows.Markup;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
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
    public abstract partial class FrameworkTemplate : DependencyObject
    {
        private Func<FrameworkElement, TemplateInstance> _methodToInstantiateFrameworkTemplate;

        protected FrameworkTemplate()
        {
            this.CanBeInheritanceContext = false;
        }

        internal bool ApplyTemplateContent(FrameworkElement container)
        {
            Debug.Assert(container != null, "Must have a non-null TemplatedParent.");

            if (_methodToInstantiateFrameworkTemplate != null)
            {
                FrameworkElement visualTree = _methodToInstantiateFrameworkTemplate(container).TemplateContent;
                container.TemplateChild = visualTree;
                
                return visualTree != null;
            }
            else
            {
                return BuildVisualTree(container);
            }
        }

        internal virtual bool BuildVisualTree(FrameworkElement container)
        {
            return false;
        }

        /// <summary>
        /// Creates an instance of the Template. Intented to be called for templates that have no owner, such as DataTemplates (not ControlTemplates).
        /// </summary>
        /// <returns>The instantiated template.</returns>
        internal FrameworkElement INTERNAL_InstantiateFrameworkTemplate()
        {
            if (_methodToInstantiateFrameworkTemplate != null)
            {
                TemplateInstance templateInstance = _methodToInstantiateFrameworkTemplate(null);
                return templateInstance.TemplateContent;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the method that will create the tree of elements.
        /// </summary>
        /// <param name="methodToInstantiateFrameworkTemplate">The method that will create the tree of elements.</param>
        public void SetMethodToInstantiateFrameworkTemplate(Func<FrameworkElement, TemplateInstance> methodToInstantiateFrameworkTemplate)
        {
            if (_isSealed)
            {
                throw new InvalidOperationException($"Cannot modify a sealed '{GetType().Name}'. Please create a new one instead.");
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
