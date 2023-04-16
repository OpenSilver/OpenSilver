
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

namespace System.Windows.Browser
{
    /// <summary>
    /// Gets a reference to the JavaScript object that raised the event.
    /// </summary>
    /// <param name="sender">
    /// A reference to the JavaScript object that raised the event.
    /// </param>
    /// <param name="args">
    /// A reference to the complex type that is passed as an event argument.
    /// </param>
    public delegate void ScriptEventHandler(ScriptObject sender, ScriptObject args);
}