﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DotNetForHtml5.EmulatorWithoutJavascript.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        public int DisplaySize {
            get {
                return ((int)(this["DisplaySize"]));
            }
            set {
                this["DisplaySize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int DisplaySize_Phone_Orientation {
            get {
                return ((int)(this["DisplaySize_Phone_Orientation"]));
            }
            set {
                this["DisplaySize_Phone_Orientation"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int DisplaySize_Tablet_Orientation {
            get {
                return ((int)(this["DisplaySize_Tablet_Orientation"]));
            }
            set {
                this["DisplaySize_Tablet_Orientation"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DateOfAssemblyLastSuccessfullyCompiledToJavaScript {
            get {
                return ((string)(this["DateOfAssemblyLastSuccessfullyCompiledToJavaScript"]));
            }
            set {
                this["DateOfAssemblyLastSuccessfullyCompiledToJavaScript"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsBypassCORSErrors {
            get {
                return ((bool)(this["IsBypassCORSErrors"]));
            }
            set {
                this["IsBypassCORSErrors"] = value;
            }
        }
    }
}
