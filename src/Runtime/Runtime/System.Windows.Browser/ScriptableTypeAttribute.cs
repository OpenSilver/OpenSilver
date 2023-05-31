
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
    /// Indicates that all public properties, methods, and events on a managed type are
    /// available to JavaScript code when they are registered by using the 
    /// <see cref="HtmlPage.RegisterCreateableType(string, Type)"/> method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class ScriptableTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptableTypeAttribute"/> class.
        /// </summary>
        public ScriptableTypeAttribute() { }
    }
}
