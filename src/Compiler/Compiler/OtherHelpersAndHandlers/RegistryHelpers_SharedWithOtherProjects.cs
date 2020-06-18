using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5
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
