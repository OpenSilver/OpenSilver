
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

    /// <summary>
    /// TemplateProperty
    /// </summary>
    internal static readonly DependencyProperty TemplateProperty =
        DependencyProperty.Register(
            nameof(Template),
            typeof(ItemsPanelTemplate),
            typeof(ItemsPresenter),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTemplateChanged));

    /// <summary>
    /// Template Property
    /// </summary>
    internal ItemsPanelTemplate Template
    {
        get => _templateCache;
        set => SetValueInternal(TemplateProperty, value);
    }

    private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ip = (ItemsPresenter)d;
        ip.ClearPanel();
        UpdateTemplateCache(ip, (FrameworkTemplate)e.OldValue, (FrameworkTemplate)e.NewValue, TemplateProperty);
    }

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
        ItemsControl owner = TemplatedParent as ItemsControl;

        // top-level presenter - get information from ItemsControl
        ItemContainerGenerator generator = owner?.ItemContainerGenerator;

        Owner = owner;
        UseGenerator(generator);

        // create the panel, based on ItemsControl.ItemsPanel
        ItemsPanelTemplate template = Owner?.ItemsPanel;

        Template = template;
    }

    private void UseGenerator(ItemContainerGenerator generator)
    {
        if (generator == Generator) return;

        if (Generator is not null)
        {
            Generator.PanelChanged -= new EventHandler(OnPanelChanged);
        }

        Generator = generator;

        if (Generator is not null)
        {
            Generator.PanelChanged += new EventHandler(OnPanelChanged);
        }
    }

    private void OnPanelChanged(object sender, EventArgs e)
    {
        // something has changed that affects the ItemsPresenter.
        // Re-measure.  This will recalculate everything from scratch.
        InvalidateMeasure();
    }

    internal static ItemsPresenter FromPanel(Panel panel)
    {
        if (panel is null)
        {
            return null;
        }

        return panel.TemplatedParent as ItemsPresenter;
    }

    /// <summary>
    /// Called when the Template's tree is about to be generated
    /// </summary>
    internal override void OnPreApplyTemplate()
    {
        base.OnPreApplyTemplate();
        AttachToOwner();
    }

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