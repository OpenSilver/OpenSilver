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

namespace System.Windows.Controls;

/// <summary>
/// Describes the presentation of grouped items in an <see cref="ItemsControl"/>.
/// </summary>
[OpenSilver.NotImplemented]
public sealed class GroupStyle : DependencyObject
{
    public DataTemplate HeaderTemplate { get; set; }

    public Style ContainerStyle { get; set; }
}
