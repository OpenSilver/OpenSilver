
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
using System.Runtime.CompilerServices;

namespace System.Windows.Controls;

/// <summary>
/// Describes the presentation of grouped items in an <see cref="ItemsControl"/>.
/// </summary>
[OpenSilver.NotImplemented]
public class GroupStyle : INotifyPropertyChanged
{
    private DataTemplate _headerTemplate;
    private Style _containerStyle;

    /// <inheritdoc />
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Gets or sets the template that is used to display the grouping header.
    /// </summary>
    [OpenSilver.NotImplemented]
    public DataTemplate HeaderTemplate
    {
        get => _headerTemplate;
        set
        {
            if (_headerTemplate != value)
            {
                _headerTemplate = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the style that is applied to the grouping item that is generated for each item.
    /// </summary>
    [OpenSilver.NotImplemented]
    public Style ContainerStyle
    {
        get => _containerStyle;
        set
        {
            if (_containerStyle != value)
            {
                _containerStyle = value;
                OnPropertyChanged();
            }
        }
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
