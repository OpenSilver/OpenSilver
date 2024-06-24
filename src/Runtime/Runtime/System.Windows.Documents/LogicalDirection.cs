
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

namespace System.Windows.Documents;

/// <summary>
/// Specifies a logical direction in which to perform certain text operations, such
/// as inserting, retrieving, or navigating through text relative to a specified
/// position (a <see cref="TextPointer"/>).
/// </summary>
public enum LogicalDirection
{
    /// <summary>
    /// Backward, or from right to left.
    /// </summary>
    Backward = 0,
    /// <summary>
    /// Forward, or from left to right.
    /// </summary>
    Forward = 1
}
