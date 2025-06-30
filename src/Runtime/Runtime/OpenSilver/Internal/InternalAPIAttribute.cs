
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

namespace OpenSilver.Internal;

/// <summary>
/// Indicates that an internal member is used by an external project, despite not being part of 
/// the public API. This attribute serves as a marker to prevent accidental refactoring or removal 
/// of internal methods that are externally consumed.
/// Apply this attribute to internal methods, properties, or types that are relied upon by other 
/// projects, especially those residing in separate repositories.
/// </summary>
[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
internal sealed class InternalAPIAttribute : Attribute
{
    public InternalAPIAttribute(ExternalProject project)
    {
        Project = project;
    }

    public ExternalProject Project { get; }
}

/// <summary>
/// Enumerates the known external projects that rely on OpenSilver's internal APIs.
/// </summary>
internal enum ExternalProject
{
    XRSharp,
}
