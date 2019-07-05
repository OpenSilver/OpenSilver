
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml;
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
    public class Button : ButtonBase
    {
        public Button()
        {
            this.DefaultStyleKey = typeof(Button);
        }
    }
}
