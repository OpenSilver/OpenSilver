
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
    /// Indicates that a specific property, method, or event is accessible to JavaScript
    /// callers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false, Inherited = true)]
    public sealed class ScriptableMemberAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptableMemberAttribute"/> class.
        /// </summary>
        public ScriptableMemberAttribute() { }

        /// <summary>
        /// Controls the generation of Silverlight plug-in helper methods that can be used
        /// to create wrappers around managed objects.
        /// </summary>
        /// <returns>
        /// true if the HTML bridge feature should automatically generate helper methods
        /// in the scope of the registered scriptable type; otherwise, false. The default
        /// is true.
        /// </returns>
        public bool EnableCreateableTypes { get; set; }

        /// <summary>
        /// Gets or sets the name of a property, method, or event that is exposed to JavaScript
        /// code. By default, the script alias is the same as the name of the managed property,
        /// method, or event.
        /// </summary>
        /// <returns>
        /// The name of a property, method, or event.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The alias is set to an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The alias is set to null.
        /// </exception>
        public string ScriptAlias { get; set; }
    }
}
