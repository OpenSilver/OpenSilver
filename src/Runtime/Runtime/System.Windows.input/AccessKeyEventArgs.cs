
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

namespace System.Windows.Input;

/// <summary>
/// Provides information for access keys events.
/// </summary>
public class AccessKeyEventArgs : EventArgs
{
    internal AccessKeyEventArgs(string key, bool isMultiple, bool userInitiated)
    {
        Key = key;
        IsMultiple = isMultiple;
        UserInitiated = userInitiated;
    }

    /// <summary>
    /// Gets the access keys that was pressed.
    /// </summary>
    /// <returns>
    /// The access key.
    /// </returns>
    public string Key { get; }

    /// <summary>
    /// Gets a value that indicates whether other elements are invoked by the key.
    /// </summary>
    /// <returns>
    /// true if other elements are invoked; otherwise, false.
    /// </returns>
    public bool IsMultiple { get; }

    internal bool UserInitiated { get; private set; }

    internal void ClearUserInitiated() => UserInitiated = false;
}
