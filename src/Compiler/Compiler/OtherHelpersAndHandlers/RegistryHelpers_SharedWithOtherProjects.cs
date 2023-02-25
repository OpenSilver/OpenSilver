
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

using Microsoft.Win32;

namespace OpenSilver
{
    internal static class RegistryHelpers
    {
        public static void SaveSetting(string name, string value)
        {
            RegistryKey reg_key = Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey sub_key = reg_key.CreateSubKey("CSharpXamlForHtml5");
            sub_key.SetValue(name, value);
        }

        public static void DeleteSetting(string name)
        {
            RegistryKey reg_key = Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey sub_key = reg_key.CreateSubKey("CSharpXamlForHtml5");
            sub_key.DeleteValue(name, false);
        }

        public static string GetSetting(string name, string defaultValue)
        {
            // Returns the defaultValue if the key is not found.

            RegistryKey reg_key = Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey sub_key = reg_key.CreateSubKey("CSharpXamlForHtml5");
            object value = sub_key.GetValue(name, defaultValue);
            if (value != null)
                return value.ToString();
            else
                return null;
        }

    }
}
