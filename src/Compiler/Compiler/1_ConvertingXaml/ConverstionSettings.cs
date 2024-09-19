
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

namespace OpenSilver.Compiler
{
    internal sealed class ConversionSettings
    {
        private ConversionSettings(
            string assemblyName,
            IMetadata metadata,
            ICoreTypesConverter coreTypes,
            SystemTypesHelper systemTypes,
            bool enableImplicitAssemblyRedirection,
            XamlPreprocessorOptions options)
        {
            AssemblyName = assemblyName;
            Metadata = metadata;
            CoreTypes = coreTypes;
            SystemTypes = systemTypes;
            EnableImplicitAssemblyRedirection = enableImplicitAssemblyRedirection;
            Options = options;
        }

        public static ConversionSettings CreateCSharpSettings(string assembly, XamlPreprocessorOptions options) =>
            new(assembly, MetadatasCS.Silverlight, CoreTypesConverters.CSharp, SystemTypesHelper.CSharp, true, options);

        public static ConversionSettings CreateVisualBasicSettings(string assembly, XamlPreprocessorOptions options) =>
            new(assembly, MetadatasVB.Silverlight, CoreTypesConverters.VisualBasic, SystemTypesHelper.VisualBasic, true, options);

        public static ConversionSettings CreateFSharpSettings(string assembly, XamlPreprocessorOptions options) =>
            new(assembly, MetadatasFS.Silverlight, CoreTypesConverters.FSharp, SystemTypesHelper.FSharp, true, options);

        public string AssemblyName { get; }

        public IMetadata Metadata { get; }

        public ICoreTypesConverter CoreTypes { get; }

        public SystemTypesHelper SystemTypes { get; }

        public bool EnableImplicitAssemblyRedirection { get; }

        public XamlPreprocessorOptions Options { get; }
    }
}