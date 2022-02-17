
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

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents metadata associated with a specific point in a media file.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class TimelineMarker : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineMarker"/> class.
        /// </summary>
        [OpenSilver.NotImplemented]
        public TimelineMarker() { }

        /// <summary>
        ///  The text value of the <see cref="TimelineMarker"/>. The default value is an empty string.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///  The time at which the <see cref="TimelineMarker"/> is reached. The default value is nulla null reference.
        /// </summary>
        [OpenSilver.NotImplemented]
        public TimeSpan Time { get; set; }

        /// <summary>
        ///  A string that describes the type of this <see cref="TimelineMarker"/>. The default value is an empty string.
        /// </summary>
        [OpenSilver.NotImplemented]
        public string Type { get; set; }
    }
}