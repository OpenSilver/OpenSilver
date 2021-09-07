

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


using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a spinner control that includes two Buttons.
    /// </summary>
    /// <remarks>
    /// ButtonSpinner inherits from Spinner.
    /// It adds two button template parts and a content property.
    /// </remarks>
    [ContentProperty("Content")]
    [OpenSilver.NotImplemented]
    public class ButtonSpinner : Spinner
    {
        #region public object Content
        /// <summary>
        /// Gets or sets the content that is contained within the button spinner.
        /// </summary>
        [OpenSilver.NotImplemented]
        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        /// <summary>
        /// Identifies the Content dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                "Content",
                typeof(object),
                typeof(ButtonSpinner),
                new PropertyMetadata(null, OnContentPropertyChanged));

        /// <summary>
        /// ContentProperty property changed handler.
        /// </summary>
        /// <param name="d">ButtonSpinner that changed its Content.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = d as ButtonSpinner;
            source.OnContentChanged(e.OldValue, e.NewValue);
        }
        #endregion public object Content

        /// <summary>
        /// Occurs when the Content property value changed.
        /// </summary>
        /// <param name="oldValue">The old value of the Content property.</param>
        /// <param name="newValue">The new value of the Content property.</param>
        [OpenSilver.NotImplemented]
        protected virtual void OnContentChanged(object oldValue, object newValue)
        {
        }
    }
}
