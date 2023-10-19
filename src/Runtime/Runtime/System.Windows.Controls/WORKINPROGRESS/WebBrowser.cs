
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

namespace System.Windows.Controls
{
    //
    // Summary:
    //     Hosts HTML content within the Silverlight plug-in.
    public partial class WebBrowser : FrameworkElement
    {
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(WebBrowser), null);

        //
        // Summary:
        //     Gets or sets the URI source of the HTML content to display in the System.Windows.Controls.WebBrowser
        //     control.
        //
        // Returns:
        //     The URI source of the HTML content to display in the System.Windows.Controls.WebBrowser
        //     control.
        [OpenSilver.NotImplemented]
        public Uri Source
        {
            get => (Uri)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        //
        // Summary:
        //     Saves the source for the HTML content currently displayed in the System.Windows.Controls.WebBrowser
        //     as a string.
        //
        // Returns:
        //     The string representation of the source for the currently displayed HTML content.
        //
        // Exceptions:
        //   T:System.Security.SecurityException:
        //     The HTML content to be saved is from a cross-domain location.
        [OpenSilver.NotImplemented]
        public string SaveToString()
        {
            return "";
        }

        /// <summary>
        /// Executes a script function that is implemented by the currently loaded document.
        /// </summary>
        /// <param name="scriptName">
        /// The name of the script function to execute.
        /// </param>
        /// <returns>
        /// The object returned by the Active Scripting call.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        [OpenSilver.NotImplemented]
        public object InvokeScript(string scriptName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes a script function that is defined in the currently loaded document.
        /// </summary>
        /// <param name="scriptName">
        /// The name of the script function to execute.
        /// </param>
        /// <param name="args">
        /// The parameters to pass to the script function.
        /// </param>
        /// <returns>
        /// The object returned by the Active Scripting call.
        /// </returns>
        [OpenSilver.NotImplemented]
        public object InvokeScript(string scriptName, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Occurs when the content contained in the WebBrowser control passes a string to the Silverlight plug-in by using JavaScript.
        /// </summary>
        [OpenSilver.NotImplemented]
        public event EventHandler<NotifyEventArgs> ScriptNotify;
    }
}
