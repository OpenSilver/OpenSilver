// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.


#if MIGRATION
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
#endif

using System.ComponentModel;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a <see cref="T:System.Windows.Controls.DataGrid" /> column that hosts textual content in its cells.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [StyleTypedProperty(Property = "ElementStyle", StyleTargetType = typeof(TextBlock))]
    [StyleTypedProperty(Property = "EditingElementStyle", StyleTargetType = typeof(TextBox))]
    public class DataGridTextColumn : DataGridBoundColumn
    {
#region Constants

        private const string DATAGRIDTEXTCOLUMN_fontFamilyName = "FontFamily";
        private const string DATAGRIDTEXTCOLUMN_fontSizeName = "FontSize";
        private const string DATAGRIDTEXTCOLUMN_fontStyleName = "FontStyle";
        private const string DATAGRIDTEXTCOLUMN_fontWeightName = "FontWeight";
        private const string DATAGRIDTEXTCOLUMN_foregroundName = "Foreground";

#endregion Constants

#region Data

        private double? _fontSize;
        private FontStyle? _fontStyle;
        private FontWeight? _fontWeight;
        private Brush _foreground;

#endregion Data

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.DataGridTextColumn" /> class.
        /// </summary>
        public DataGridTextColumn()
        {
            this.BindingTarget = TextBox.TextProperty;
        }

#region DependencyProperties

#region FontFamily
        /// <summary>
        /// Gets or sets the font name.
        /// </summary>
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Identifies the FontFamily dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register(
                "FontFamily",
                typeof(FontFamily),
                typeof(DataGridTextColumn),
                new PropertyMetadata(OnFontFamilyPropertyChanged));

        private static void OnFontFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridTextColumn textColumn = (DataGridTextColumn)d;
            textColumn.NotifyPropertyChanged(DATAGRIDTEXTCOLUMN_fontFamilyName);
        }
#endregion FontFamily

#endregion DependencyProperties

#region Public Properties

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        // Use DefaultValue here so undo in the Designer will set this to NaN
        [DefaultValue(double.NaN)]
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
                    NotifyPropertyChanged(DATAGRIDTEXTCOLUMN_fontSizeName);
                }
            }
        }

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        public FontStyle FontStyle
        {
            get
            {
                return this._fontStyle ?? FontStyles.Normal;
            }
            set
            {
                if (this._fontStyle != value)
                {
                    this._fontStyle = value;
                    NotifyPropertyChanged(DATAGRIDTEXTCOLUMN_fontStyleName);
                }
            }
        }

        /// <summary>
        /// Gets or sets the font weight or thickness.
        /// </summary>
        public FontWeight FontWeight
        {
            get
            {
                return this._fontWeight ?? FontWeights.Normal;
            }
            set
            {
                if (this._fontWeight != value)
                {
                    this._fontWeight = value;
                    NotifyPropertyChanged(DATAGRIDTEXTCOLUMN_fontWeightName);
                }
            }
        }

        /// <summary>
        /// Gets or sets a brush that describes the foreground of the column cells.
        /// </summary>
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
                    NotifyPropertyChanged(DATAGRIDTEXTCOLUMN_foregroundName);
                }
            }
        }

#endregion Public Properties

#region Internal Properties

#endregion Internal Properties

#region Protected Methods

        /// <summary>
        /// Causes the column cell being edited to revert to the specified value.
        /// </summary>
        /// <param name="editingElement">The element that the column displays for a cell in editing mode.</param>
        /// <param name="uneditedValue">The previous, unedited value in the cell being edited.</param>
        protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
        {
            TextBox textBox = editingElement as TextBox;
            if (textBox != null)
            {
                string uneditedString = uneditedValue as string;
                textBox.Text = uneditedString ?? string.Empty;
            }
        }
        
        /// <summary>
        /// Gets a <see cref="T:System.Windows.Controls.TextBox" /> control that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.
        /// </summary>
        /// <param name="cell">The cell that will contain the generated element.</param>
        /// <param name="dataItem">The data item represented by the row that contains the intended cell.</param>
        /// <returns>A new <see cref="T:System.Windows.Controls.TextBox" /> control that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.</returns>
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            TextBox textBox = new TextBox();
            textBox.VerticalAlignment = VerticalAlignment.Stretch;
            textBox.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);

            if (DependencyProperty.UnsetValue != ReadLocalValue(DataGridTextColumn.FontFamilyProperty))
            {
                textBox.FontFamily = this.FontFamily;
            }
            if (this._fontSize.HasValue)
            {
                textBox.FontSize = this._fontSize.Value;
            }
            if (this._fontStyle.HasValue)
            {
                textBox.FontStyle = this._fontStyle.Value;
            }
            if (this._fontWeight.HasValue)
            {
                textBox.FontWeight = this._fontWeight.Value;
            }
            if (this._foreground != null)
            {
                textBox.Foreground = this._foreground;
            }
            if (this.Binding != null || !DesignerProperties.IsInDesignTool)
            {
                textBox.SetBinding(this.BindingTarget, this.Binding);
            }
            return textBox;
        }
        
        /// <summary>
        /// Gets a read-only <see cref="T:System.Windows.Controls.TextBlock" /> element that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.
        /// </summary>
        /// <param name="cell">The cell that will contain the generated element.</param>
        /// <param name="dataItem">The data item represented by the row that contains the intended cell.</param>
        /// <returns>A new, read-only <see cref="T:System.Windows.Controls.TextBlock" /> element that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.</returns>
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            TextBlock textBlockElement = new TextBlock();
            textBlockElement.Margin = new Thickness(4);
            textBlockElement.VerticalAlignment = VerticalAlignment.Center;
            if (DependencyProperty.UnsetValue != ReadLocalValue(DataGridTextColumn.FontFamilyProperty))
            {
                textBlockElement.FontFamily = this.FontFamily;
            }
            if (this._fontSize.HasValue)
            {
                textBlockElement.FontSize = this._fontSize.Value;
            }
            if (this._fontStyle.HasValue)
            {
                textBlockElement.FontStyle = this._fontStyle.Value;
            }
            if (this._fontWeight.HasValue)
            {
                textBlockElement.FontWeight = this._fontWeight.Value;
            }
            if (this._foreground != null)
            {
                textBlockElement.Foreground = this._foreground;
            }
            if (this.Binding != null || !DesignerProperties.IsInDesignTool)
            {
                textBlockElement.SetBinding(TextBlock.TextProperty, this.Binding);
            }
            return textBlockElement;
        }

        /// <summary>
        /// Called when the cell in the column enters editing mode.
        /// </summary>
        /// <param name="editingElement">The element that the column displays for a cell in editing mode.</param>
        /// <param name="editingEventArgs">Information about the user gesture that is causing a cell to enter editing mode.</param>
        /// <returns>The unedited value. </returns>
        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            TextBox textBox = editingElement as TextBox;
            if (textBox != null)
            {
                string uneditedText = textBox.Text;
                int len = uneditedText.Length;
                KeyEventArgs keyEventArgs = editingEventArgs as KeyEventArgs;
                if (keyEventArgs != null && keyEventArgs.Key == Key.F2)
                {
                    // Put caret at the end of the text
                    textBox.Select(len, len);
                }
                else
                {
                    // Select all text
                    textBox.Select(0, len);
                }
                return uneditedText;
            }
            return string.Empty;
        }

        /// <summary>
        /// Called by the DataGrid control when this column asks for its elements to be
        /// updated, because a property changed.
        /// </summary>
        protected internal override void RefreshCellContent(FrameworkElement element, string propertyName)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            TextBox textBox = element as TextBox;
            if (textBox == null)
            {
                TextBlock textBlock = element as TextBlock;
                if (textBlock == null)
                {
                    throw DataGridError.DataGrid.ValueIsNotAnInstanceOfEitherOr("element", typeof(TextBox), typeof(TextBlock));
                }
                if (propertyName == DATAGRIDTEXTCOLUMN_fontFamilyName)
                {
                    textBlock.FontFamily = this.FontFamily;
                }
                else if (propertyName == DATAGRIDTEXTCOLUMN_fontSizeName)
                {
                    SetTextFontSize(textBlock, TextBlock.FontSizeProperty);
                }
                else if (propertyName == DATAGRIDTEXTCOLUMN_fontStyleName)
                {
                    textBlock.FontStyle = this.FontStyle;
                }
                else if (propertyName == DATAGRIDTEXTCOLUMN_fontWeightName)
                {
                    textBlock.FontWeight = this.FontWeight;
                }
                else if (propertyName == DATAGRIDTEXTCOLUMN_foregroundName)
                {
                    textBlock.Foreground = this.Foreground;
                }
                else
                {
                    if (this.FontFamily != null)
                    {
                        textBlock.FontFamily = this.FontFamily;
                    }
                    SetTextFontSize(textBlock, TextBlock.FontSizeProperty);
                    textBlock.FontStyle = this.FontStyle;
                    textBlock.FontWeight = this.FontWeight;
                    if (this.Foreground != null)
                    {
                        textBlock.Foreground = this.Foreground;
                    }
                }
                return;
            }
            if (propertyName == DATAGRIDTEXTCOLUMN_fontFamilyName)
            {
                textBox.FontFamily = this.FontFamily;
            }
            else if (propertyName == DATAGRIDTEXTCOLUMN_fontSizeName)
            {
                SetTextFontSize(textBox, TextBox.FontSizeProperty);
            }
            else if (propertyName == DATAGRIDTEXTCOLUMN_fontStyleName)
            {
                textBox.FontStyle = this.FontStyle;
            }
            else if (propertyName == DATAGRIDTEXTCOLUMN_fontWeightName)
            {
                textBox.FontWeight = this.FontWeight;
            }
            else if (propertyName == DATAGRIDTEXTCOLUMN_foregroundName)
            {
                textBox.Foreground = this.Foreground;
            }
            else
            {
                if (this.FontFamily != null)
                {
                    textBox.FontFamily = this.FontFamily;
                }
                SetTextFontSize(textBox, TextBox.FontSizeProperty);
                textBox.FontStyle = this.FontStyle;
                textBox.FontWeight = this.FontWeight;
                if (this.Foreground != null)
                {
                    textBox.Foreground = this.Foreground;
                }
            }
        }

#endregion Protected Methods

#region Internal Methods

#endregion Internal Methods

#region Private Methods

        private void SetTextFontSize(DependencyObject textElement, DependencyProperty fontSizeProperty)
        {
            double newFontSize = this.FontSize;
            if (double.IsNaN(newFontSize))
            {
                textElement.ClearValue(fontSizeProperty);
            }
            else
            {
                textElement.SetValue(fontSizeProperty, newFontSize);
            }
        }

#endregion Private Methods
    }
}
