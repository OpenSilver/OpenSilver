
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

namespace System.Windows.Resources;

/// <summary>
/// Provides resource stream information for application resources or other packages obtained 
/// through the <see cref="Application.GetResourceStream(StreamResourceInfo, Uri)"/> method.
/// </summary>
public class StreamResourceInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamResourceInfo"/> class.
    /// </summary>
    public StreamResourceInfo() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamResourceInfo"/> class.
    /// </summary>
    /// <param name="stream">
    /// The stream to use to create the resource.
    /// </param>
    /// <param name="contentType">
    /// The MIME type of the content.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// stream is null.
    /// </exception>
    public StreamResourceInfo(Stream stream, string contentType)
    {
        Stream = stream ?? throw new ArgumentNullException(nameof(stream));
        ContentType = contentType;
    }

    /// <summary>
    /// Gets the MIME type of the content in the stream.
    /// </summary>
    /// <returns>
    /// The MIME type of the content in the stream, as a string.
    /// </returns>
    public string ContentType { get; }

    /// <summary>
    /// Gets the stream that is contained by the resource.
    /// </summary>
    /// <returns>
    /// The stream that is contained by the resource.
    /// </returns>
    public Stream Stream { get; }
}
