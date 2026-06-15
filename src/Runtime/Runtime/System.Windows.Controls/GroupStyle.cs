
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
using OpenSilver.Internal.Xaml.Context;
using System.ComponentModel;

namespace System.Windows.Controls;

/// <summary>
/// Defines how you want the group to look at each level.
/// </summary>
[OpenSilver.NotImplemented]
public class GroupStyle : INotifyPropertyChanged
{
    /// <summary>
    /// Identifies the default <see cref="ItemsPanelTemplate"/> that creates the panel used to layout the items.
    /// </summary>
    public static readonly ItemsPanelTemplate DefaultGroupPanel;

    private ItemsPanelTemplate _panel;
    private Style _containerStyle;
    private StyleSelector _containerStyleSelector;
    private DataTemplate _headerTemplate;
    private DataTemplateSelector _headerTemplateSelector;
    private bool _hidesIfEmpty;
    private int _alternationCount;

    static GroupStyle()
    {
        var template = new ItemsPanelTemplate
        {
            Template = new CompiledTemplateContent(
                new XamlContext(),
                static (owner, context) =>
                {
                    var panel = new StackPanel();
                    panel.SetTemplatedParent(context.TemplateOwnerReference);
                    return panel;
                }),
        };

        template.Seal();

        DefaultGroupPanel = template;

        Default = new GroupStyle();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupStyle"/> class.
    /// </summary>
    public GroupStyle() { }

    /// <summary>
    /// Gets the default style of the group.
    /// </summary>
    /// <returns>
    /// The default style of the group.
    /// </returns>
    public static GroupStyle Default { get; }

    /// <summary>
    /// Gets or sets a template that creates the panel used to layout the items.
    /// </summary>
    /// <returns>
    /// An <see cref="ItemsPanelTemplate"/> object that creates the panel used to layout the items.
    /// </returns>
    public ItemsPanelTemplate Panel
    {
        get => _panel;
        set
        {
            _panel = value;
            OnPropertyChanged(nameof(Panel));
        }
    }

    /// <summary>
    /// Gets or sets the style that is applied to the <see cref="GroupItem"/> generated for each item.
    /// </summary>
    /// <returns>
    /// The style that is applied to the <see cref="GroupItem"/> generated for each item. The default is null.
    /// </returns>
    public Style ContainerStyle
    {
        get => _containerStyle;
        set
        {
            _containerStyle = value;
            OnPropertyChanged(nameof(ContainerStyle));
        }
    }

    /// <summary>
    /// Enables the application writer to provide custom selection logic for a style to apply to each generated
    /// <see cref="GroupItem"/>.
    /// </summary>
    /// <returns>
    /// An object that derives from <see cref="StyleSelector"/>. The default is null.
    /// </returns>
    public StyleSelector ContainerStyleSelector
    {
        get => _containerStyleSelector;
        set
        {
            _containerStyleSelector = value;
            OnPropertyChanged(nameof(ContainerStyleSelector));
        }
    }

    /// <summary>
    /// Gets or sets the template that is used to display the group header.
    /// </summary>
    /// <returns>
    /// A <see cref="DataTemplate"/> object that is used to display the group header. The default is null.
    /// </returns>
    public DataTemplate HeaderTemplate
    {
        get => _headerTemplate;
        set
        {
            _headerTemplate = value;
            OnPropertyChanged(nameof(HeaderTemplate));
        }
    }

    /// <summary>
    /// Enables the application writer to provide custom selection logic for a template that is used to display 
    /// the group header.
    /// </summary>
    /// <returns>
    /// An object that derives from <see cref="DataTemplateSelector"/>. The default is null.
    /// </returns>
    public DataTemplateSelector HeaderTemplateSelector
    {
        get => _headerTemplateSelector;
        set
        {
            _headerTemplateSelector = value;
            OnPropertyChanged(nameof(HeaderTemplateSelector));
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether items corresponding to empty groups should be displayed.
    /// </summary>
    /// <returns>
    /// true to not display empty groups; otherwise, false. The default is false.
    /// </returns>
    public bool HidesIfEmpty
    {
        get => _hidesIfEmpty;
        set
        {
            _hidesIfEmpty = value;
            OnPropertyChanged(nameof(HidesIfEmpty));
        }
    }

    /// <summary>
    /// Gets or sets the number of alternating <see cref="GroupItem"/> objects.
    /// </summary>
    /// <returns>
    /// The number of alternating <see cref="GroupItem"/> objects.
    /// </returns>
    public int AlternationCount
    {
        get => _alternationCount;
        set
        {
            _alternationCount = value;
            OnPropertyChanged(nameof(AlternationCount));
        }
    }

    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
        add => PropertyChanged += value;
        remove => PropertyChanged -= value;
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    protected event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event using the provided arguments.
    /// </summary>
    /// <param name="e">
    /// Arguments of the event being raised.
    /// </param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

    private void OnPropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
}
