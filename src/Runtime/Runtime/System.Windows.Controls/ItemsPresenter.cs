
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
using System.Windows.Media;

namespace System.Windows.Controls;

/// <summary>
/// Specifies where items are placed in a control, usually an <see cref="ItemsControl"/>.
/// </summary>
public class ItemsPresenter : FrameworkElement
{
    private ItemsPanelTemplate _templateCache;

    internal sealed override FrameworkElement TemplateChild
    {
        get { return base.TemplateChild; }
        set
        {
            if (value is not null)
            {
                if (value is not Panel panel)
                {
                    throw new InvalidOperationException(string.Format(Strings.ItemsPanelNotAPanel, value.GetType()));
                }
                panel.IsItemsHost = true;
            }

            base.TemplateChild = value;
        }
    }

    internal ItemsControl Owner { get; private set; }

    internal ItemContainerGenerator Generator { get; private set; }

    // Internal Helper so the FrameworkElement could see this property
    internal override FrameworkTemplate TemplateInternal => Template;

    // Internal Helper so the FrameworkElement could see the template cache
    internal override FrameworkTemplate TemplateCache
    {
        get => _templateCache;
        set => _templateCache = (ItemsPanelTemplate)value;
    }

    internal override void OnTemplateChangedInternal(FrameworkTemplate oldTemplate, FrameworkTemplate newTemplate)
    {
        OnTemplateChanged((ItemsPanelTemplate)oldTemplate, (ItemsPanelTemplate)newTemplate);
    }

    /// <summary>
    /// TemplateProperty
    /// </summary>
    private static readonly DependencyProperty TemplateProperty =
        DependencyProperty.Register(
            nameof(Template),
            typeof(ItemsPanelTemplate),
            typeof(ItemsPresenter),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTemplateChanged));

    /// <summary>
    /// Template Property
    /// </summary>
    private ItemsPanelTemplate Template
    {
        get => _templateCache;
        set => SetValueInternal(TemplateProperty, value);
    }

    private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ip = (ItemsPresenter)d;
        var oldTemplate = (ItemsPanelTemplate)e.OldValue;
        var newTemplate = (ItemsPanelTemplate)e.NewValue;

        ip.ClearPanel();
        StyleHelper.UpdateTemplateCache(ip, oldTemplate, newTemplate, TemplateProperty);
    }

    /// <summary>
    /// Called when the control template changes.
    /// </summary>
    /// <param name="oldTemplate">
    /// Value of the old template.
    /// </param>
    /// <param name="newTemplate">
    /// Value of the new template.
    /// </param>
    protected virtual void OnTemplateChanged(ItemsPanelTemplate oldTemplate, ItemsPanelTemplate newTemplate) { }

    private void ClearPanel()
    {
        if (TemplateChild is Panel oldPanel)
        {
            oldPanel.IsItemsHost = false;
        }
    }

    internal void DetachFromOwner()
    {
        UseGenerator(null);
        ClearPanel();
    }

    private void AttachToOwner()
    {
        DependencyObject templatedParent = TemplatedParent;
        ItemsControl owner = templatedParent as ItemsControl;
        ItemContainerGenerator generator;

        if (owner is not null)
        {
            // top-level presenter - get information from ItemsControl
            generator = owner.ItemContainerGenerator;
        }
        else
        {
            // subgroup presenter - get information from GroupItem
            GroupItem parentGI = templatedParent as GroupItem;

            if (FromGroupItem(parentGI) is ItemsPresenter parentIP)
            {
                owner = parentIP.Owner;
            }

            generator = parentGI?.Generator;
        }

        Owner = owner;
        UseGenerator(generator);

        // create the panel, based either on ItemsControl.ItemsPanel or GroupStyle.Panel
        ItemsPanelTemplate template = null;
        GroupStyle groupStyle = Generator?.GroupStyle;

        if (groupStyle is not null)
        {
            // If GroupStyle.Panel is set then we dont honor ItemsControl.IsVirtualizing
            template = groupStyle.Panel;

            // create default Panels
            template ??= GroupStyle.DefaultStackPanel;
        }
        else
        {
            // Its a leaf-level ItemsPresenter, therefore pick ItemsControl.ItemsPanel
            template = Owner?.ItemsPanel;
        }

        Template = template;
    }

    private void UseGenerator(ItemContainerGenerator generator)
    {
        if (generator == Generator) return;

        Generator?.PanelChanged -= new EventHandler(OnPanelChanged);
        Generator = generator;
        Generator?.PanelChanged += new EventHandler(OnPanelChanged);
    }

    private void OnPanelChanged(object sender, EventArgs e)
    {
        // something has changed that affects the ItemsPresenter.
        // Re-measure.  This will recalculate everything from scratch.
        InvalidateMeasure();

        // If we're under a ScrollViewer then its ScrollContentPresenter needs to
        // be updated to work with the new panel.
        if (Parent is ScrollViewer)
        {
            // If our logical parent is a ScrollViewer then the visual parent is a ScrollContentPresenter.
            if (VisualTreeHelper.GetParent(this) is ScrollContentPresenter scp)
            {
                scp.HookupScrollingComponents();
            }
        }
    }

    internal static ItemsPresenter FromPanel(Panel panel)
    {
        if (panel is null)
        {
            return null;
        }

        return panel.TemplatedParent as ItemsPresenter;
    }

    internal static ItemsPresenter FromGroupItem(GroupItem groupItem)
    {
        if (groupItem is null)
        {
            return null;
        }

        if (VisualTreeHelper.GetParent(groupItem) is not UIElement parent)
        {
            return null;
        }

        return VisualTreeHelper.GetParent(parent) as ItemsPresenter;
    }

    /// <summary>
    /// Called when the Template's tree is about to be generated
    /// </summary>
    internal override void OnPreApplyTemplate()
    {
        base.OnPreApplyTemplate();
        AttachToOwner();
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        // verify that the template produced a panel with no children
        if (TemplateChild is not Panel panel || panel.HasChildren)
        {
            throw new InvalidOperationException(Strings.ItemsPanelNotSingleNode);
        }

        OnPanelChanged(this, EventArgs.Empty);

        base.OnApplyTemplate();
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
    {
        int count = VisualChildrenCount;

        if (count > 0)
        {
            if (GetVisualChild(0) is UIElement child)
            {
                child.Measure(availableSize);
                return child.DesiredSize;
            }
        }

        return new Size();
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        int count = VisualChildrenCount;

        if (count > 0)
        {
            if (GetVisualChild(0) is UIElement child)
            {
                child.Arrange(new Rect(finalSize));
            }
        }
        return finalSize;
    }
}