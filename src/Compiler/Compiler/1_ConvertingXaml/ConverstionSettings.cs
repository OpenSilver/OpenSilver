
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
            bool enableImplicitAssemblyRedirection)
        {
            AssemblyName = assemblyName;
            Metadata = metadata;
            CoreTypes = coreTypes;
            SystemTypes = systemTypes;
            EnableImplicitAssemblyRedirection = enableImplicitAssemblyRedirection;
        }

        public static ConversionSettings CreateCSharpSettings(string assembly) =>
            new(assembly, MetadatasCS.Silverlight, CoreTypesConverters.CSharp, SystemTypesHelper.CSharp, true);

        public static ConversionSettings CreateVisualBasicSettings(string assembly) =>
            new(assembly, MetadatasVB.Silverlight, CoreTypesConverters.VisualBasic, SystemTypesHelper.VisualBasic, true);

        public static ConversionSettings CreateFSharpSettings(string assembly) =>
            new(assembly, MetadatasFS.Silverlight, CoreTypesConverters.FSharp, SystemTypesHelper.FSharp, true);

        public string AssemblyName { get; }

        public IMetadata Metadata { get; }

        public ICoreTypesConverter CoreTypes { get; }

        public SystemTypesHelper SystemTypes { get; }

        public bool EnableImplicitAssemblyRedirection { get; }
    }
}