
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

using OpenSilver.Internal;
using OpenSilver.Internal.Xaml;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Markup;
using System.Xaml.Markup;

namespace System.Windows;

/// <summary>
/// Creates an element tree of elements.
/// </summary>
[ContentProperty(nameof(Template))]
public abstract class FrameworkTemplate : DependencyObject, ISealable
{
    private TemplateContent _template;
    private ResourceDictionary _resources;
    private bool _isSealed;

    protected FrameworkTemplate()
    {
        CanBeInheritanceContext = false;
    }

    /// <summary>
    /// Gets or sets the collection of resources that can be used within the scope of this template.
    /// </summary>
    /// <returns>
    /// The resources that can be used within the scope of this template.
    /// </returns>
    [Ambient]
    public ResourceDictionary Resources
    {
        get
        {
            if (_resources is null)
            {
                _resources = new ResourceDictionary();

                if (IsSealed())
                {
                    _resources.IsReadOnly = true;
                }
            }

            return _resources;
        }
        set
        {
            CheckSealed();

            _resources = value;
        }
    }

    internal bool HasResources => _resources is not null && !_resources.IsEmpty;

    internal object FindResource(object resourceKey)
    {
        if (_resources is not null && _resources.TryGetResource(resourceKey, out object value))
        {
            return value;
        }
        return DependencyProperty.UnsetValue;
    }

    internal virtual Type TargetTypeInternal => null;

    internal virtual TriggerCollection TriggersInternal => null;

    /// <summary>
    /// Gets or sets a reference to the object that records or plays the XAML nodes for
    /// the template when the template is defined or applied by a writer.
    /// </summary>
    /// <returns>
    /// A reference to the object that records or plays the XAML nodes for the template.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Ambient]
    [XamlDeferLoad(typeof(TemplateContentLoader), typeof(IFrameworkElement))]
    public TemplateContent Template
    {
        get => _template;
        set
        {
            CheckSealed();

            if (_template is not null)
            {
                throw new XamlParseException(Strings.TemplateContentSetTwice);
            }

            ArgumentNullException.ThrowIfNull(value);

            value.OwnerTemplate = this;
            _template = value;
        }
    }

    internal bool ApplyTemplateContent(FrameworkElement container) => StyleHelper.ApplyTemplateContent(container, this);

    internal bool ApplyTemplateContent<T>(T container) where T : DependencyObject, IInternalFrameworkElement
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
    public new void Seal()
    {
        if (_isSealed) return;

        // Seal triggers
        TriggersInternal?.Seal();

        // Seal Resource Dictionary
        _resources?.IsReadOnly = true;

        _template?.Seal();

        _isSealed = true;
    }

    /// <summary>
    /// Gets a value that indicates whether this object is in an immutable state
    /// so it cannot be changed.
    /// </summary>
    /// <returns>true if this object is in an immutable state; otherwise, false.</returns>
    public new bool IsSealed() => _isSealed;

    private protected void CheckSealed()
    {
        if (IsSealed())
        {
            throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, GetType().Name));
        }
    }

    /// <summary>
    /// Finds the element associated with the specified name defined within this template.
    /// </summary>
    /// <param name="name">
    /// The string name.
    /// </param>
    /// <param name="templatedParent">
    /// The context of the <see cref="FrameworkElement"/> where this template is applied.
    /// </param>
    /// <returns>
    /// The element associated with the specified name.
    /// </returns>
    public object FindName(string name, FrameworkElement templatedParent)
    {
        ArgumentNullException.ThrowIfNull(templatedParent);

        if (this != templatedParent.TemplateInternal)
        {
            throw new InvalidOperationException(Strings.TemplateFindNameInInvalidElement);
        }

        if (GetTemplateNameScope(templatedParent) is INameScope nameScope)
        {
            return nameScope.FindName(name);
        }

        return null;
    }

    internal static readonly UncommonField<INameScope> TemplateNameScopeField = new();

    internal static INameScope GetTemplateNameScope(DependencyObject templatedParent)
    {
        Debug.Assert(templatedParent is IFrameworkElement);
        return TemplateNameScopeField.GetValue(templatedParent);
    }

    internal static void SetTemplateNameScope(DependencyObject templatedParent, INameScope namescope)
    {
        Debug.Assert(templatedParent is IFrameworkElement);
        TemplateNameScopeField.SetValue(templatedParent, namescope);
    }

    bool ISealable.CanSeal => true;

    bool ISealable.IsSealed => IsSealed();

    void ISealable.Seal() => Seal();
}
