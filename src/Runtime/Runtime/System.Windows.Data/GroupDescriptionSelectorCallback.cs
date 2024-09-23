
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

namespace System.Windows.Data;

/// <summary>
/// Represents a method that is used to provide custom logic to select the <see cref="GroupDescription"/> 
/// based on the parent group and its level.
/// </summary>
/// <param name="group">
/// The parent group.
/// </param>
/// <param name="level">
/// The level of group.
/// </param>
/// <returns>
/// The selected <see cref="GroupDescription"/> based on the parent group and its level.
/// </returns>
public delegate GroupDescription GroupDescriptionSelectorCallback(CollectionViewGroup group, int level);