
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
        CoreTypesConverter coreTypes,
        SystemTypesHelper systemTypes,
        TypeReferenceHelper typeReferenceHelper,
        XamlPreprocessorOptions options)
    {
        AssemblyName = assemblyName;
        Inspector = inspector;
        CoreTypes = coreTypes;
        SystemTypes = systemTypes;
        TypeReferenceHelper = typeReferenceHelper;
        XamlNameParser = new XamlNameParser(assemblyName);
        NameProvider = new();
        Options = options;
    }

    public string AssemblyName { get; }

    public AssembliesInspector Inspector { get; }

    public CoreTypesConverter CoreTypes { get; }

    public SystemTypesHelper SystemTypes { get; }

    public TypeReferenceHelper TypeReferenceHelper { get; }

    public XamlNameParser XamlNameParser { get; }

    public NameProvider NameProvider { get; }

    public XamlPreprocessorOptions Options { get; }
}