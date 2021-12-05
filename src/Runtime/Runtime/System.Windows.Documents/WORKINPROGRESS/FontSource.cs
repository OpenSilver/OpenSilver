
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

using System.IO;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    //
    // Summary:
    //     Represents one or more fonts created from a stream.
    [OpenSilver.NotImplemented]
    public class FontSource
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Documents.FontSource class.
        //
        // Parameters:
        //   stream:
        //     The stream that contains the font source.
        [OpenSilver.NotImplemented]
        public FontSource(Stream stream)
        {
        }
    }
}
