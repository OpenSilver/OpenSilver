
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
        public IMetadata Metadata { get; set; }

        public ICoreTypesConverter CoreTypesConverter { get; set; }

        public bool EnableImplicitAssemblyRedirection { get; set; }
    }

    internal static class ConversionSettingsCS
    {
        public static ConversionSettings Silverlight { get; } =
            new ConversionSettings
            {
                Metadata = MetadatasCS.Silverlight,
                CoreTypesConverter = CoreTypesConvertersCS.Silverlight,
                EnableImplicitAssemblyRedirection = true,
            };

        public static ConversionSettings UWP { get; } =
            new ConversionSettings
            {
                Metadata = MetadatasCS.UWP,
                CoreTypesConverter = CoreTypesConvertersCS.UWP,
                EnableImplicitAssemblyRedirection = false,
            };
    }

    internal static class ConversionSettingsVB
    {
        public static ConversionSettings Silverlight { get; } =
            new ConversionSettings
            {
                Metadata = MetadatasVB.Silverlight,
                CoreTypesConverter = CoreTypesConvertersVB.Silverlight,
                EnableImplicitAssemblyRedirection = true,
            };

        public static ConversionSettings UWP { get; } =
            new ConversionSettings
            {
                Metadata = MetadatasVB.UWP,
                CoreTypesConverter = CoreTypesConvertersVB.UWP,
                EnableImplicitAssemblyRedirection = false,
            };
    }
}