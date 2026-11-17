
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

using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace OpenSilver;

public static class Features
{
    public static class DOM
    {
        /// <summary>
        /// Determine if the runtime should assign the <see cref="Type.FullName"/> as a class to DOM elements
        /// for each <see cref="UIElement"/>.
        /// </summary>
        public static bool AssignClass { get; set; } = IsDebug();

        /// <summary>
        /// Determine if the runtime should assign the <i>dataId</i> attribute to DOM elements of each 
        /// <see cref="FrameworkElement"/> with a non null or empty <see cref="FrameworkElement.Name"/> property.
        /// </summary>
        public static bool AssignName { get; set; } = IsDebug();

        /// <summary>
        /// Determine if the runtime should allow calling the <i>dump</i> method from DOM elements.
        /// </summary>
        public static bool EnableObjectDump { get; set; } = IsDebug();
    }

    public static class Interop
    {
        /// <summary>
        /// When enabled, JavaScript interop will be separated by "<b>;\n</b>" rather than "<s>;</s>".
        /// </summary>
        public static bool UseNewLineSeparator { get; set; } = IsDebug();
    }

    private static bool IsDebug()
    {
        if (_isDebugging.HasValue)
        {
            return _isDebugging.Value;
        }

        Application app = Application.Current;
        if (app is not null)
        {
            DebuggableAttribute attr = app.GetType().Assembly.GetCustomAttribute<DebuggableAttribute>();
            _isDebugging = attr != null && attr.IsJITTrackingEnabled;
            return _isDebugging.Value;
        }

        return false;
    }

    private static bool? _isDebugging;
}
