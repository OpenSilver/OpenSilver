

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
#if !MIGRATION
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides the base implementation for controls that contain a single
    /// content element and a header.
    /// </summary>
    /// <remarks>
    /// HeaderedContentControl adds Header and HeaderTemplatefeatures to a
    /// ContentControl. HasHeader and HeaderTemplateSelector are removed for
    /// lack of support and consistency with other Silverlight controls.
    /// </remarks>
    /// <QualityBand>Stable</QualityBand>
    public partial class HeaderedContentControl : ContentControl
    {
        #region public object Header
        /// <summary>
        /// Gets or sets the content for the header of the control.
        /// </summary>
        /// <value>
        /// The content for the header of the control. The default value is
        /// null.
        /// </value>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.HeaderedContentControl.Header" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the
        /// <see cref="P:System.Windows.Controls.HeaderedContentControl.Header" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty HeaderProperty =
                DependencyProperty.Register(
                        "Header",
                        typeof(object),
                        typeof(HeaderedContentControl),
                        new PropertyMetadata(null, OnHeaderPropertyChanged)
                        { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// HeaderProperty property changed handler.
        /// </summary>
        /// <param name="d">HeaderedContentControl whose Header property is changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs, which contains the old and new value.</param>
        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderedContentControl ctrl = (HeaderedContentControl)d;
            ctrl.OnHeaderChanged(e.OldValue, e.NewValue);
        }
        #endregion public object Header

        #region public DataTemplate HeaderTemplate
        /// <summary>
        /// Gets or sets the template that is used to display the content of the
        /// control's header.
        /// </summary>
        /// <value>
        /// The template that is used to display the content of the control's
        /// header. The default is null.
        /// </value>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.HeaderedContentControl.HeaderTemplate" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the
        /// <see cref="P:System.Windows.Controls.HeaderedContentControl.HeaderTemplate" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty HeaderTemplateProperty =
                DependencyProperty.Register(
                        "HeaderTemplate",
                        typeof(DataTemplate),
                        typeof(HeaderedContentControl),
                        new PropertyMetadata(null, OnHeaderTemplatePropertyChanged)
                        { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// HeaderTemplateProperty property changed handler.
        /// </summary>
        /// <param name="d">HeaderedContentControl whose HeaderTemplate property is changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs, which contains the old and new value.</param>
        private static void OnHeaderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderedContentControl ctrl = (HeaderedContentControl)d;
            ctrl.OnHeaderTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }
        #endregion public DataTemplate HeaderTemplate

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Controls.HeaderedContentControl" />
        /// class.
        /// </summary>
        public HeaderedContentControl()
        {
            //DefaultStyleKey = typeof(HeaderedContentControl);
        }

        /// <summary>
        /// Called when the value of the
        /// <see cref="P:System.Windows.Controls.HeaderedContentControl.Header" />
        /// property changes.
        /// </summary>
        /// <param name="oldHeader">
        /// The old value of the
        /// <see cref="P:System.Windows.Controls.HeaderedContentControl.Header" />
        /// property.
        /// </param>
        /// <param name="newHeader">
        /// The new value of the
        /// <see cref="P:System.Windows.Controls.HeaderedContentControl.Header" />
        /// property.
        /// </param>
        protected virtual void OnHeaderChanged(object oldHeader, object newHeader)
        {
        }

        /// <summary>
        /// Called when the value of the
        /// <see cref="P:System.Windows.Controls.HeaderedContentControl.HeaderTemplate" />
        /// property changes.
        /// </summary>
        /// <param name="oldHeaderTemplate">
        /// The old value of the
        /// <see cref="P:System.Windows.Controls.HeaderedContentControl.HeaderTemplate" />
        /// property.
        /// </param>
        /// <param name="newHeaderTemplate">
        /// The new value of the
        /// <see cref="P:System.Windows.Controls.HeaderedContentControl.HeaderTemplate" />
        /// property.
        /// </param>
        protected virtual void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
        {
        }
    }
}
