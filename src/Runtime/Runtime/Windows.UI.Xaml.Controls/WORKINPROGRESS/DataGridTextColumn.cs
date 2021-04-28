

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


#if WORKINPROGRESS

using System;
using System.ComponentModel;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{ 
    /// <summary>
    /// Represents a System.Windows.Controls.DataGrid column that hosts textual content
    /// in its cells.
    /// </summary>
    public partial class DataGridTextColumn
    {
        #region FontFamily
        /// <summary>
        /// Gets or sets the font name.
        /// </summary>
        [OpenSilver.NotImplemented]
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Identifies the FontFamily dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
#if WORKINPROGRESS
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register(
                "FontFamily",
                typeof(FontFamily),
                typeof(DataGridTextColumn),
                new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsMeasure, OnFontFamilyPropertyChanged));
#else
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register(
                "FontFamily",
                typeof(FontFamily),
                typeof(DataGridTextColumn),
                new PropertyMetadata(OnFontFamilyPropertyChanged));
#endif

        private static void OnFontFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //DataGridTextColumn textColumn = (DataGridTextColumn)d;
            //textColumn.NotifyPropertyChanged(DATAGRIDTEXTCOLUMN_fontFamilyName);
        }
        #endregion FontFamily

        private double? _fontSize;

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        // Use DefaultValue here so undo in the Designer will set this to NaN
        [DefaultValue(double.NaN)]
        [OpenSilver.NotImplemented]
        public double FontSize
        {
            get
            {
                return this._fontSize ?? Double.NaN;
            }
            set
            {
                if (this._fontSize != value)
                {
                    this._fontSize = value;
                    //NotifyPropertyChanged(DATAGRIDTEXTCOLUMN_fontSizeName);
                }
            }
        }

        private Brush _foreground;

        /// <summary>
        /// Gets or sets a brush that describes the foreground of the column cells.
        /// </summary>
        [OpenSilver.NotImplemented]
        public Brush Foreground
        {
            get
            {
                return this._foreground;
            }
            set
            {
                if (this._foreground != value)
                {
                    this._foreground = value;
                    //NotifyPropertyChanged(DATAGRIDTEXTCOLUMN_foregroundName);
                }
            }
        }

        //
        // Summary:
        //     Gets or sets the style that is used when rendering the element that the column
        //     displays for a cell in editing mode.
        //
        // Returns:
        //     The style that is used when rendering an editing element. The default is null.
        [OpenSilver.NotImplemented]
        public Style EditingElementStyle { get; set; }
    }
}
#endif