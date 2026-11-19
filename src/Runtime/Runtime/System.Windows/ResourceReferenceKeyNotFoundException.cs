
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

using OpenSilver.Internal;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace System.Windows;

/// <summary>
/// The exception that is thrown when a resource reference key cannot be found during parsing or serialization 
/// of markup extension resources.
/// </summary>
public class ResourceReferenceKeyNotFoundException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceReferenceKeyNotFoundException"/> class.
    /// </summary>
    public ResourceReferenceKeyNotFoundException()
    {
        Key = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceReferenceKeyNotFoundException"/> class with the 
    /// specified error message and resource key.
    /// </summary>
    /// <param name="message">
    /// A possible descriptive message.
    /// </param>
    /// <param name="resourceKey">
    /// The key that was not found.
    /// </param>
    public ResourceReferenceKeyNotFoundException(string message, object resourceKey)
        : base(message)
    {
        Key = resourceKey;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceReferenceKeyNotFoundException"/> class with the 
    /// specified serialization information and streaming context.
    /// </summary>
    /// <param name="info">
    /// Specific information from the serialization process.
    /// </param>
    /// <param name="context">
    /// The context at the time the exception was thrown.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(Obsoletions.LegacyFormatterImplMessage, DiagnosticId = Obsoletions.LegacyFormatterImplDiagId)]
    protected ResourceReferenceKeyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Key = info.GetValue(nameof(Key), typeof(object));
    }

    /// <summary>
    /// Gets the key that was not found and caused the exception to be thrown.
    /// </summary>
    /// <returns>
    /// The resource key.
    /// </returns>
    public object Key { get; }

    /// <summary>
    /// Reports specifics of the exception to debuggers or dialogs.
    /// </summary>
    /// <param name="info">
    /// Specific information from the serialization process.
    /// </param>
    /// <param name="context">
    /// The context at the time the exception was thrown.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(Obsoletions.LegacyFormatterImplMessage, DiagnosticId = Obsoletions.LegacyFormatterImplDiagId)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(Key), Key);
    }
}
