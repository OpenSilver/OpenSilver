
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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Markup;
using OpenSilver.Internal.Xaml.Context;

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
        private TemplateContent _template;
        private bool _isSealed;

        protected FrameworkTemplate()
        {
            CanBeInheritanceContext = false;
        }

        internal TemplateContent Template
        {
            get { return _template; }
            set { CheckSealed(); _template = value; }
        }

        internal bool ApplyTemplateContent(FrameworkElement container)
        {
            Debug.Assert(container != null, "Must have a non-null TemplatedParent.");

            if (Template != null)
            {
                FrameworkElement visualTree = Template.LoadContent(container);
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
            if (Template != null)
            {
                return Template.LoadContent(null);
            }
            else
            {
                return null;
            }
        }

        [Obsolete("Deprecated. Please use the Template property instead.", true)]
        public void SetMethodToInstantiateFrameworkTemplate(Func<FrameworkElement, TemplateInstance> methodToInstantiateFrameworkTemplate)
        {
            throw new NotSupportedException("Deprecated. Please use the Template property instead.");
        }

        // The following property is used during the "InsertImplicitNodes" step of the compilation,
        // in conjunction with the "ContentProperty" attribute. The property is never used at runtime.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UIElement ContentPropertyUsefulOnlyDuringTheCompilation
        {
            get { return (UIElement)GetValue(ContentPropertyUsefulOnlyDuringTheCompilationProperty); }
            set { SetValue(ContentPropertyUsefulOnlyDuringTheCompilationProperty, value); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DependencyProperty ContentPropertyUsefulOnlyDuringTheCompilationProperty =
            DependencyProperty.Register(
                nameof(ContentPropertyUsefulOnlyDuringTheCompilation), 
                typeof(UIElement), 
                typeof(FrameworkTemplate), 
                null);
        
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

        private void CheckSealed()
        {
            if (IsSealed())
            {
                throw new InvalidOperationException($"Cannot modify a sealed '{GetType().Name}'. Please create a new one instead.");
            }
        }
    }

    internal class TemplateContent
    {
        private readonly XamlContext _xamlContext;
        private readonly Func<FrameworkElement, XamlContext, FrameworkElement> _factory;

        internal TemplateContent(XamlContext xamlContext, Func<FrameworkElement, XamlContext, FrameworkElement> factory)
        {
            if (xamlContext == null)
            {
                throw new ArgumentNullException(nameof(xamlContext));
            }

            _xamlContext = new XamlContext(xamlContext);
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        internal FrameworkElement LoadContent(FrameworkElement owner)
        {
            return _factory(owner, new XamlContext(_xamlContext));
        }
    }
}
