

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a templated button control that interprets a Click user interaction.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <Button Content="Click me" Margin="0,5,0,0" Foreground="White" Background="#FFE44D26" HorizontalAlignment="Left" Click="MyButton_Click"/>
    /// </code>
    /// <code lang="C#">
    /// void MyButton_Click(object sender, RoutedEventArgs e)
    /// {
    ///     MessageBox.Show("You clicked me.");
    ///     Window.Current.IsEnabled = false;
    /// }
    /// </code>
    /// </example>
    public partial class Button : ButtonBase
    {
        public Button()
        {
            DefaultStyleKey = typeof(Button);
        }

        /// <summary>
        /// Returns a <see cref="ButtonAutomationPeer"/> for use by the Silverlight automation 
        /// infrastructure.
        /// </summary>
        /// <returns>
        /// <see cref="ButtonAutomationPeer"/> for the <see cref="Button"/> object.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            => new ButtonAutomationPeer(this);
    }
}
