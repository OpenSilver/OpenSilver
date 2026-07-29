// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using OpenSilver.Internal;
using OpenSilver.Internal.Xaml.Context;
using System.Threading;
using System.Windows.Automation.Peers;

namespace System.Windows.Controls.Primitives;

/// <summary>
/// Represents a control that displays items and information in a horizontal bar in an application window.
/// </summary>
[StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(StatusBarItem))]
public class StatusBar : ItemsControl
{
    static StatusBar()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusBar), new FrameworkPropertyMetadata(typeof(StatusBar)));
        IsTabStopProperty.OverrideMetadata(typeof(StatusBar), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        var template = new ItemsPanelTemplate
        {
            Template = new CompiledTemplateContent(
                new XamlContext(),
                static (owner, context) =>
                {
                    var panel = new DockPanel();
                    panel.SetTemplatedParent(context.TemplateOwnerReference);
                    return panel;
                }),
        };
        template.Seal();

        ItemsPanelProperty.OverrideMetadata(typeof(StatusBar), new FrameworkPropertyMetadata(template));
    }

    private static SystemThemeKey _separatorStyleKey;
    private object _currentItem;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatusBar"/> class.
    /// </summary>
    public StatusBar() { }

    /// <summary>
    /// The key that represents the style to use for <see cref="Separator"/> objects in the 
    /// <see cref="StatusBar"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="ResourceKey"/> that references the style to use for <see cref="Separator"/> 
    /// object.
    /// </returns>
    public static ResourceKey SeparatorStyleKey
    {
        get
        {
            if (_separatorStyleKey is null)
            {
                Interlocked.CompareExchange(ref _separatorStyleKey, new SystemThemeKey(SystemResourceKeyID.StatusBarSeparatorStyle), null);
            }

            return _separatorStyleKey;
        }
    }

    /// <summary>
    /// Identifies the <see cref="UsesItemContainerTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UsesItemContainerTemplateProperty =
        MenuBase.UsesItemContainerTemplateProperty.AddOwner(typeof(StatusBar));

    /// <summary>
    /// Gets or sets a value that indicates whether the menu selects different item containers,
    /// depending on the type of the item in the underlying collection or some other heuristic.
    /// </summary>
    /// <returns>
    /// true the menu selects different item containers; otherwise, false. The registered default
    /// is false.
    /// </returns>
    public bool UsesItemContainerTemplate
    {
        get => (bool)GetValue(UsesItemContainerTemplateProperty);
        set => SetValueInternal(UsesItemContainerTemplateProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ItemContainerTemplateSelector"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ItemContainerTemplateSelectorProperty =
        MenuBase.ItemContainerTemplateSelectorProperty.AddOwner(
            typeof(StatusBar),
            new FrameworkPropertyMetadata(MenuBase.ItemContainerTemplateSelectorProperty.DefaultMetadata.DefaultValue));

    /// <summary>
    /// Gets or sets the custom logic for choosing a template used to display each item.
    /// </summary>
    /// <returns>
    /// A custom object that provides logic and returns an item container.
    /// </returns>
    public ItemContainerTemplateSelector ItemContainerTemplateSelector
    {
        get => (ItemContainerTemplateSelector)GetValue(ItemContainerTemplateSelectorProperty);
        set => SetValueInternal(ItemContainerTemplateSelectorProperty, value);
    }

    /// <summary>
    /// Determines if the specified item is (or is eligible to be) its own container.
    /// </summary>
    /// <param name="item">
    /// The specified object to evaluate.
    /// </param>
    /// <returns>
    /// Returns true if the item is (or is eligible to be) its own container; otherwise, false.
    /// </returns>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        bool ret = item is StatusBarItem || item is Separator;
        if (!ret)
        {
            _currentItem = item;
        }
        return ret;
    }

    /// <summary>
    /// Creates a new <see cref="StatusBarItem"/>.
    /// </summary>
    /// <returns>
    /// The element used to display the specified item.
    /// </returns>
    protected override DependencyObject GetContainerForItemOverride()
    {
        (object currentItem, _currentItem) = (_currentItem, null);

        if (UsesItemContainerTemplate)
        {
            if (ItemContainerTemplateSelector.SelectTemplate(currentItem, this) is DataTemplate itemContainerTemplate)
            {
                object itemContainer = itemContainerTemplate.LoadContent();
                if (itemContainer is StatusBarItem || itemContainer is Separator)
                {
                    return itemContainer as DependencyObject;
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format(Strings.InvalidItemContainer, GetType().Name, nameof(StatusBarItem), nameof(Separator), itemContainer));
                }
            }
        }

        return new StatusBarItem();
    }

    /// <summary>
    /// Prepares an item for display in the <see cref="StatusBar"/>.
    /// </summary>
    /// <param name="element">
    /// The item to display in the <see cref="StatusBar"/>.
    /// </param>
    /// <param name="item">
    /// The content of the item to display.
    /// </param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);

        if (element is Separator separator)
        {
            ValueSource vs = DependencyPropertyHelper.GetValueSource(separator, StyleProperty);
            if (vs.BaseValueSource <= BaseValueSource.StyleTrigger)
            {
                separator.SetResourceReference(StyleProperty, SeparatorStyleKey);
            }
            separator.DefaultStyleKey = SeparatorStyleKey;
        }
    }

    /// <summary>
    /// Determines whether to apply the <see cref="Style"/> for an item in the <see cref="StatusBar"/>
    /// to an object.
    /// </summary>
    /// <param name="container">
    /// The container for the item.
    /// </param>
    /// <param name="item">
    /// The object to evaluate.
    /// </param>
    /// <returns>
    /// true if the item is not a <see cref="Separator"/>; otherwise, false.
    /// </returns>
    protected override bool ShouldApplyItemContainerStyle(DependencyObject container, object item)
    {
        if (item is Separator)
        {
            return false;
        }
        else
        {
            return base.ShouldApplyItemContainerStyle(container, item);
        }
    }

    /// <summary>
    /// Specifies an <see cref="AutomationPeer"/> for the <see cref="StatusBar"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="StatusBarAutomationPeer"/> for this <see cref="StatusBar"/>.
    /// </returns>
    protected override AutomationPeer OnCreateAutomationPeer() => new StatusBarAutomationPeer(this);
}
