

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


using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
#if MIGRATION
using System.Windows.Media.Imaging;
#else
using Windows.UI.Xaml.Media.Imaging;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Provides an object source type for Source and ImageSource.
    /// </summary>
    [TypeConverter(typeof(ImageSourceConverter))]
#if WORKINPROGRESS
    [ContentProperty("UriSource")]
#endif
    public partial class ImageSource : DependencyObject
    {
#if WORKINPROGRESS
        //todo: the following property is used in conjuction with the [ContentProperty("UriSource")] attribute to allow to compile the following syntax: <ImageSource x:Key="AppointmentItem_Exception">/Telerik.Windows.Controls.ScheduleView;component/Themes/Images/AppointmentRecurrence.png</ImageSource>
        //However, it is currently not functional because a BitmapImage needs to be created instead of an ImageSource.
        [OpenSilver.NotImplemented]
        public Uri UriSource { get; set; }
#endif

    }
}