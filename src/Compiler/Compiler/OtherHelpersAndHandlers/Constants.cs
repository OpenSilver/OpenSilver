using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler
{
    internal static class Constants
    {
        public const string LOWERCASE_CORE_ASSEMBLY_NAME = "csharpxamlforhtml5";
        public const string LOWERCASE_SYSTEM_ASSEMBLY_NAME = "csharpxamlforhtml5.system";

        public const string NAME_OF_CORE_ASSEMBLY = "CSharpXamlForHtml5";
        public const string NAME_OF_CORE_ASSEMBLY_USING_BRIDGE = "CSHTML5";
        public const string NAME_OF_CORE_ASSEMBLY_USING_BLAZOR = "OpenSilver.UwpCompatible";
        public const string NAME_OF_CORE_ASSEMBLY_SLMIGRATION = "SLMigration.CSharpXamlForHtml5";
        public const string NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BRIDGE = "CSHTML5.Migration";
        public const string NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BLAZOR = "OpenSilver";

        public const string PROFESSIONAL_EDITION_FEATURE_ID = "PROFESSIONAL_EDITION"; //Note: do not change this + it needs to have the same value as its equivalent in Licensing.WebServices.Constants
        public const string SL_MIGRATION_EDITION_FEATURE_ID = "SL_MIGRATION_EDITION"; //Note: do not change this + it needs to have the same value as its equivalent in Licensing.WebServices.Constants
        public const string ENTERPRISE_EDITION_FEATURE_ID = "ENTERPRISE_EDITION"; //Note: do not change this + it needs to have the same value as its equivalent in Licensing.WebServices.Constants
        public const string COMMERCIAL_EDITION_S_FEATURE_ID = "COMMERCIAL_EDITION_S"; //Note: do not change this + it needs to have the same value as its equivalent in Licensing.WebServices.Constants
        public const string COMMERCIAL_EDITION_L_FEATURE_ID = "COMMERCIAL_EDITION_L"; //Note: do not change this + it needs to have the same value as its equivalent in Licensing.WebServices.Constants
        public const string PREMIUM_SUPPORT_EDITION_FEATURE_ID = "PREMIUM_SUPPORT_EDITION"; //Note: do not change this + it needs to have the same value as its equivalent in Licensing.WebServices.Constants

        public const string PROFESSIONAL_EDITION_FRIENDLY_NAME = "Professional Edition";
        public const string SL_MIGRATION_EDITION_FRIENDLY_NAME = "Silverlight Migration Edition";
        public const string ENTERPRISE_EDITION_FRIENDLY_NAME = "Enterprise Edition";
        public const string COMMERCIAL_EDITION_S_FRIENDLY_NAME = "Commercial Edition (S)";
        public const string COMMERCIAL_EDITION_L_FRIENDLY_NAME = "Commercial Edition (L)";
        public const string PREMIUM_SUPPORT_EDITION_FRIENDLY_NAME = "Premium Support Edition";
        public const string COMMUNITY_EDITION_FRIENDLY_NAME = "Community Edition";
    }
}
