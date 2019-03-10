
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
            this.INTERNAL_SetDefaultStyle(INTERNAL_DefaultButtonStyle.GetDefaultStyle());
        }
    }
}
