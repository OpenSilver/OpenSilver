
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
using OpenSilver.Internal;
using OpenSilver.Internal.Xaml.Context;

namespace System.Windows
{
    /// <summary>
    /// Creates an element tree of elements.
    /// </summary>
    [ContentProperty(nameof(ContentPropertyUsefulOnlyDuringTheCompilation))]
    public abstract class FrameworkTemplate : DependencyObject
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

        internal bool ApplyTemplateContent(IInternalFrameworkElement container)
        {
            Debug.Assert(container != null, "Must have a non-null TemplatedParent.");

            if (Template != null)
            {
                IInternalFrameworkElement visualTree = Template.LoadContent(container);
                container.TemplateChild = visualTree;
                
                return visualTree != null;
            }
            else
            {
                return BuildVisualTree(container);
            }
        }

        internal virtual bool BuildVisualTree(IInternalFrameworkElement container)
        {
            return false;
        }

        /// <summary>
        /// Creates an instance of the Template. Intented to be called for templates that have no owner, such as DataTemplates (not ControlTemplates).
        /// </summary>
        /// <returns>The instantiated template.</returns>
        internal IInternalFrameworkElement INTERNAL_InstantiateFrameworkTemplate()
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

        [Obsolete(Helper.ObsoleteMemberMessage + " Please use the Template property instead.", true)]
        public void SetMethodToInstantiateFrameworkTemplate(Func<FrameworkElement, TemplateInstance> methodToInstantiateFrameworkTemplate)
        {
            throw new NotSupportedException("Deprecated. Please use the Template property instead.");
        }

        // The following property is used during the "InsertImplicitNodes" step of the compilation,
        // in conjunction with the "ContentProperty" attribute. The property is never used at runtime.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IUIElement ContentPropertyUsefulOnlyDuringTheCompilation
        {
            get { return (IUIElement)GetValue(ContentPropertyUsefulOnlyDuringTheCompilationProperty); }
            set { SetValue(ContentPropertyUsefulOnlyDuringTheCompilationProperty, value); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DependencyProperty ContentPropertyUsefulOnlyDuringTheCompilationProperty =
            DependencyProperty.Register(
                nameof(ContentPropertyUsefulOnlyDuringTheCompilation), 
                typeof(IUIElement), 
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

        internal static readonly DependencyProperty TemplateNameScopeProperty =
            DependencyProperty.RegisterAttached(
                "TemplateNameScope",
                typeof(INameScope),
                typeof(FrameworkTemplate),
                null);

        internal static INameScope GetTemplateNameScope(IInternalFrameworkElement fe)
        {
            if (fe is null)
            {
                throw new ArgumentNullException(nameof(fe));
            }

            return (INameScope)fe.GetValue(TemplateNameScopeProperty);
        }

        internal static void SetTemplateNameScope(IInternalFrameworkElement fe, INameScope namescope)
        {
            if (fe is null)
            {
                throw new ArgumentNullException(nameof(fe));
            }
            
            fe.SetValue(TemplateNameScopeProperty, namescope);
        }
    }

    internal sealed class TemplateContent
    {
        private readonly XamlContext _xamlContext;
        private readonly Func<IInternalFrameworkElement, XamlContext, IInternalFrameworkElement> _factory;

        internal TemplateContent(XamlContext xamlContext, Func<IInternalFrameworkElement, XamlContext, IInternalFrameworkElement> factory)
        {
            if (xamlContext == null)
            {
                throw new ArgumentNullException(nameof(xamlContext));
            }

            _xamlContext = new XamlContext(xamlContext);
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        internal IInternalFrameworkElement LoadContent(IInternalFrameworkElement owner)
        {
            XamlContext context = new XamlContext(_xamlContext)
            {
                ExternalNameScope = new NameScope(),
            };

            IInternalFrameworkElement rootElement = _factory(owner, context);
            
            if (owner == null)
            {
                if (NameScope.GetNameScope(rootElement) == null)
                {
                    NameScope.SetNameScope(rootElement, context.ExternalNameScope);
                }
            }
            else
            {
                FrameworkTemplate.SetTemplateNameScope(owner, context.ExternalNameScope);
            }

            return rootElement;
        }
    }
}
