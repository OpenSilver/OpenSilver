
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

using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Markup;
using System.Xaml.Markup;
using OpenSilver.Internal;
using OpenSilver.Internal.Xaml;

namespace System.Windows
{
    /// <summary>
    /// Creates an element tree of elements.
    /// </summary>
    [ContentProperty(nameof(Template))]
    public abstract class FrameworkTemplate : DependencyObject
    {
        private ITemplateContent _template;
        private bool _isSealed;

        protected FrameworkTemplate()
        {
            CanBeInheritanceContext = false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [XamlDeferLoad(typeof(TemplateContentLoader), typeof(IFrameworkElement))]
        public ITemplateContent Template
        {
            get => _template;
            set { CheckSealed(); _template = value; }
        }

        internal bool ApplyTemplateContent(FrameworkElement container)
        {
            Debug.Assert(container is not null, "Must have a non-null TemplatedParent.");

            if (Template is not null)
            {
                FrameworkElement visualTree = (FrameworkElement)Template.LoadContent(container);
                container.TemplateChild = visualTree;

                return visualTree is not null;
            }
            else
            {
                return BuildVisualTree(container);
            }
        }

        internal bool ApplyTemplateContent(IInternalFrameworkElement container)
        {
            Debug.Assert(container is not null, "Must have a non-null TemplatedParent.");

            if (Template is not null)
            {
                IFrameworkElement visualTree = Template.LoadContent(container);
                container.TemplateChild = visualTree;
                
                return visualTree is not null;
            }
            else
            {
                return BuildVisualTree(container);
            }
        }

        internal virtual bool BuildVisualTree(IFrameworkElement container) => false;

        // The following property is used during the "InsertImplicitNodes" step of the compilation,
        // in conjunction with the "ContentProperty" attribute. The property is never used at runtime.
        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IUIElement ContentPropertyUsefulOnlyDuringTheCompilation
        {
            get { return (IUIElement)GetValue(ContentPropertyUsefulOnlyDuringTheCompilationProperty); }
            set { SetValueInternal(ContentPropertyUsefulOnlyDuringTheCompilationProperty, value); }
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
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
        public void Seal() => _isSealed = true;

        /// <summary>
        /// Gets a value that indicates whether this object is in an immutable state
        /// so it cannot be changed.
        /// </summary>
        /// <returns>true if this object is in an immutable state; otherwise, false.</returns>
        public bool IsSealed() => _isSealed;

        private protected void CheckSealed()
        {
            if (IsSealed())
            {
                throw new InvalidOperationException($"Cannot modify a '{GetType().Name}' after it is sealed.");
            }
        }

        internal static readonly DependencyProperty TemplateNameScopeProperty =
            DependencyProperty.RegisterAttached(
                "TemplateNameScope",
                typeof(INameScope),
                typeof(FrameworkTemplate),
                null);

        internal static INameScope GetTemplateNameScope(IFrameworkElement fe)
        {
            if (fe is null)
            {
                throw new ArgumentNullException(nameof(fe));
            }

            return (INameScope)fe.GetValue(TemplateNameScopeProperty);
        }

        internal static void SetTemplateNameScope(IFrameworkElement fe, INameScope namescope)
        {
            if (fe is null)
            {
                throw new ArgumentNullException(nameof(fe));
            }
            
            fe.SetValue(TemplateNameScopeProperty, namescope);
        }
    }
}
