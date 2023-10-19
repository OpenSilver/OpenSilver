
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

using System.Windows;

namespace OpenSilver.Internal;

/// <summary>
/// Represents a method used as a callback that validates the effective value of a
/// <see cref="DependencyProperty"/>.
/// </summary>
internal delegate bool ValidateValueCallback(object value);
