
/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

namespace OpenSilver.Compiler;

internal sealed class ConversionSettings
{
    public ConversionSettings(
        string assemblyName,
        AssembliesInspector inspector,
        ICoreTypesConverter coreTypes,
        SystemTypesHelper systemTypes,
        XamlPreprocessorOptions options)
    {
        AssemblyName = assemblyName;
        Inspector = inspector;
        CoreTypes = coreTypes;
        SystemTypes = systemTypes;
        Options = options;
    }

    public string AssemblyName { get; }

    public AssembliesInspector Inspector { get; }

    public ICoreTypesConverter CoreTypes { get; }

    public SystemTypesHelper SystemTypes { get; }

    public XamlPreprocessorOptions Options { get; }
}