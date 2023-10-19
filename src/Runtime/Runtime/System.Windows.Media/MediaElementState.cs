
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

using System.Windows.Controls;

namespace System.Windows.Media
{
    /// <summary>
    /// Defines the potential states of a MediaElement object.
    /// </summary>
    public enum MediaElementState
    {
        /// <summary>
        /// The <see cref="MediaElement"/> contains no media. The <see cref="MediaElement"/> displays a transparent frame.
        /// </summary>
        Closed,

        /// <summary>
        /// The <see cref="MediaElement"/> is validating and attempting to open the Uniform Resource Identifier (URI) specified by its <see cref="MediaElement.Source"/>
        /// property. While in this state, the <see cref="MediaElement"/> queues any <see cref="MediaElement.Play"/>, <see cref="MediaElement.Pause"/>,
        /// or <see cref="MediaElement.Stop"/> commands it receives and processes them if the media is successfully opened.
        /// </summary>
        Opening,

        /// <summary>
        /// The <see cref="MediaElement"/> is loading the media for playback. Its <see cref="MediaElement.Position"/> does not advance
        /// during this state. If the <see cref="MediaElement"/> was already playing video, it continues to display the last displayed frame.
        /// </summary>
        Buffering,

        /// <summary>
        /// The <see cref="MediaElement"/> is playing the media specified by its source property. Its <see cref="MediaElement.Position"/> advances forward.
        /// </summary>
        Playing,

        /// <summary>
        /// The <see cref="MediaElement"/> does not advance its <see cref="MediaElement.Position"/>. If the <see cref="MediaElement"/> was playing video,
        /// it continues to display the current frame.
        /// </summary>
        Paused,

        /// <summary>
        /// The <see cref="MediaElement"/> contains media but is not playing or paused. Its <see cref="MediaElement.Position"/> is 0 and does not advance.
        /// If the loaded media is video, the <see cref="MediaElement"/> displays the first frame.
        /// </summary>
        Stopped,

        /// <summary>
        /// The <see cref="MediaElement"/> is in the process of ensuring that proper individualization components (only applicable when
        /// playing DRM protected content) are installed on the user's computer.
        /// </summary>
        Individualizing,

        /// <summary>
        /// The <see cref="MediaElement"/> is acquiring a license required to play DRM protected content. Once OnAcquireLicense has
        /// been called, the MediaElement will remain in this state until SetLicenseResponse has been called.
        /// </summary>
        AcquiringLicense,
    }
}