
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
/// Appears as the root of the visual subtree generated for a group.
/// </summary>
public class GroupItem : ContentControl
{
    static GroupItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupItem), new PropertyMetadata(typeof(GroupItem)));

        // GroupItems should not be focusable by default
        FocusableProperty.OverrideMetadata(typeof(GroupItem), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupItem"/> class.
    /// </summary>
    public GroupItem() { }

    internal ItemContainerGenerator Generator { get; set; }

    internal Panel ItemsHost { get; set; }

    internal void PrepareItemContainer(object item, ItemsControl parentItemsControl)
    {
        if (Generator is null)
        {
            return;     // user-declared GroupItem - ignore
        }

        // If a GroupItem is being recycled set back IsItemsHost
        ItemsHost?.IsItemsHost = true;

        // Release any previous containers. Also ensures Items and GroupStyle are hooked up correctly
        Generator.Release();

        ItemContainerGenerator generator = Generator.Parent;
        GroupStyle groupStyle = generator.GroupStyle;

        // apply the container style
        Style style = groupStyle.ContainerStyle;

        // no ContainerStyle set, try ContainerStyleSelector
        style ??= groupStyle.ContainerStyleSelector?.SelectStyle(item, this);

        // apply the style, if found
        if (style != null)
        {
            // verify style is appropriate before applying it
            if (!style.TargetType.IsInstanceOfType(this))
            {
                throw new InvalidOperationException(string.Format(Strings.StyleForWrongType, style.TargetType.Name, GetType().Name));
            }

            Style = style;
            IsStyleSetFromGenerator = true;
        }

        // forward the header template information
        if (ContentIsItem || HasDefaultValue(ContentProperty))
        {
            Content = item;
            ContentIsItem = true;
        }

        if (HasDefaultValue(ContentTemplateProperty))
        {
            ContentTemplate = groupStyle.HeaderTemplate;
        }

        if (HasDefaultValue(ContentTemplateSelectorProperty))
        {
            ContentTemplateSelector = groupStyle.HeaderTemplateSelector;
        }

        if (HasDefaultValue(ContentStringFormatProperty))
        {
            ContentStringFormat = groupStyle.HeaderStringFormat;
        }
    }

    internal void ClearItemContainer(object item, ItemsControl parentItemsControl)
    {
        if (Generator is null)
        {
            return;     // user-declared GroupItem - ignore
        }

        Generator.Release();

        ClearContentControl(item);
    }
}
