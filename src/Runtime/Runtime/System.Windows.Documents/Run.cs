
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
using System.Windows.Markup;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    /// <summary>
    /// Represents a discrete section of formatted or unformatted text.
    /// </summary>
    [ContentProperty(nameof(Text))]
    public sealed class Run : Inline
    {
        /// <summary>
        /// Get or Set the Text property
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Identifies the Text dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register(
                nameof(Text), 
                typeof(string), 
                typeof(Run),
                new PropertyMetadata(string.Empty, OnTextChanged)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        INTERNAL_HtmlDomManager.SetContentString((Run)d, (string)newValue);
                    },
                });

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Run run = (Run)d;

            switch (run.GetLayoutParent())
            {
                case TextBlock tb:
                    tb.InvalidateCacheAndMeasure();
                    break;

                case FrameworkElement fe:
                    fe.InvalidateMeasure();
                    break;
            };
        }
    }
}
