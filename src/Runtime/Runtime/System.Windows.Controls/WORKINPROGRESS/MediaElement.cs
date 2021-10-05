

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
using System.IO;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents an object that contains audio, video, or both.
    /// </summary>
    public partial class MediaElement
    {
        /// <summary>
        /// Stops and resets media to be played from the beginning.
        /// </summary>
        [OpenSilver.NotImplemented]
        public void Stop()
        {
            throw new NotImplementedException();
        }

        [OpenSilver.NotImplemented]
        public TimeSpan BufferingTime { get; set; }

        [OpenSilver.NotImplemented]
        public event RoutedEventHandler MediaEnded;

        [OpenSilver.NotImplemented]
        public TimeSpan Position { get; set; }

        [OpenSilver.NotImplemented]
        public Duration NaturalDuration { get; }

        [OpenSilver.NotImplemented]
        public void SetSource(Stream stream)
        {
        }
    }
}
