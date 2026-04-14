
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

using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace OpenSilver.Compiler
{
    public class AssembliesInspector : IDisposable
    {
        private readonly MonoCecilAssembliesInspectorImpl _monoCecilVersion;

        public AssembliesInspector(string assemblyName, SupportedLanguage compilerType)
        {
            _monoCecilVersion = new MonoCecilAssembliesInspectorImpl(assemblyName, compilerType);
        }

        public void Dispose() => _monoCecilVersion.Dispose();

        public AssemblyDefinition LoadAssembly(string assemblyPath) => _monoCecilVersion.LoadAssembly(assemblyPath);

        public AssemblyDefinition LoadAssembly(Stream stream) => _monoCecilVersion.LoadAssembly(stream);

        public void UnloadAssembly(AssemblyDefinition assembly) => _monoCecilVersion.UnloadAssembly(assembly);

        public string GetContentPropertyName(TypeDefinition type, IXmlLineInfo lineInfo) => _monoCecilVersion.GetContentPropertyName(type, lineInfo);

        public bool IsElementAMarkupExtension(TypeDefinition type) => _monoCecilVersion.IsElementAMarkupExtension(type);

        public bool IsFrameworkTemplateTemplateProperty(string propertyName, string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo)
            => _monoCecilVersion.IsFrameworkTemplateTemplateProperty(propertyName, namespaceName, typeName, assemblyName, lineInfo);

        public bool IsFrameworkTemplateTemplateProperty(MemberReference memberReference)
            => _monoCecilVersion.IsFrameworkTemplateTemplateProperty(memberReference);

        public bool IsDependencyObject(TypeDefinition type) => _monoCecilVersion.IsDependencyObject(type);

        public bool IsApplication(TypeDefinition type) => _monoCecilVersion.IsApplication(type);

        public bool IsResourceDictionary(TypeDefinition type) => _monoCecilVersion.IsResourceDictionary(type);

        public bool IsStyle(TypeDefinition type) => _monoCecilVersion.IsStyle(type);

        public bool IsFrameworkTemplate(TypeDefinition type) => _monoCecilVersion.IsFrameworkTemplate(type);

        public bool IsDataTemplate(TypeDefinition type) => _monoCecilVersion.IsDataTemplate(type);

        public bool IsControlTemplate(TypeDefinition type) => _monoCecilVersion.IsControlTemplate(type);

        public bool IsContentPresenter(TypeDefinition type) => _monoCecilVersion.IsContentPresenter(type);

        public bool IsContentControl(TypeDefinition type) => _monoCecilVersion.IsContentControl(type);

        public bool IsRelativeSource(TypeDefinition type) => _monoCecilVersion.IsRelativeSource(type);

        public bool IsBindingBase(TypeReference type) => _monoCecilVersion.IsBindingBase(type);

        public bool IsBinding(TypeDefinition type) => _monoCecilVersion.IsBinding(type);

        public bool IsMultiBinding(TypeDefinition type) => _monoCecilVersion.IsMultiBinding(type);

        public bool IsTemplateBindingExtension(TypeDefinition type) => _monoCecilVersion.IsTemplateBindingExtension(type);

        public bool IsNullExtension(TypeDefinition type) => _monoCecilVersion.IsNullExtension(type);

        public bool IsStaticExtension(TypeDefinition type) => _monoCecilVersion.IsStaticExtension(type);

        public bool IsTypeExtension(TypeDefinition type) => _monoCecilVersion.IsTypeExtension(type);

        public bool IsStaticResourceExtension(TypeDefinition type) => _monoCecilVersion.IsStaticResourceExtension(type);

        public bool IsThemeResourceExtension(TypeDefinition type) => _monoCecilVersion.IsThemeResourceExtension(type);

        public bool IsDynamicResourceExtension(TypeDefinition type) => _monoCecilVersion.IsDynamicResourceExtension(type);

        public bool IsResponsiveExtension(TypeDefinition type) => _monoCecilVersion.IsResponsiveExtensionType(type);

        public bool IsIList(TypeDefinition type) => _monoCecilVersion.IsIList(type);

        public bool IsIDictionary(TypeDefinition type) => _monoCecilVersion.IsIDictionary(type);

        public bool IsIFrameworkElement(TypeDefinition type) => _monoCecilVersion.IsIFrameworkElement(type);

        public bool IsIUIElement(TypeDefinition type) => _monoCecilVersion.IsIUIElement(type);

        public TypeDefinition GetTypeDefinition(string namespaceName, string typeName, string assemblyName, IXmlLineInfo lineInfo, bool throwIfNull = true)
            => _monoCecilVersion.FindType(namespaceName, typeName, assemblyName, lineInfo, !throwIfNull);

        public string GetEnumValue(TypeDefinition enumType, string name, bool ignoreCase, bool allowIntegerValue)
            => _monoCecilVersion.GetEnumValue(enumType, name, ignoreCase, allowIntegerValue);

        public IEnumerable<string> GetEnumValues(TypeDefinition enumType, string name, bool ignoreCase, bool allowIntegerValue, IXmlLineInfo lineInfo)
            => _monoCecilVersion.GetEnumValues(enumType, name, ignoreCase, allowIntegerValue, lineInfo);

        public (FieldDefinition Field, TypeReference DeclaringType) GetField(TypeDefinition type, string name, MemberFlags flags)
        {
            FieldDefinition field = MonoCecilAssembliesInspectorImpl.FindFieldDeep(
                type, name, flags, out TypeReference declaringType);

            return (field, declaringType);
        }

        public (PropertyDefinition Property, TypeReference DeclaringType) GetProperty(TypeDefinition type, string name, MemberFlags flags)
        {
            PropertyDefinition property = MonoCecilAssembliesInspectorImpl.FindPropertyDeep(
                type, name, flags, out TypeReference declaringType);

            return (property, declaringType);
        }

        public EventDefinition GetEvent(TypeDefinition type, string eventName, MemberFlags flags)
            => MonoCecilAssembliesInspectorImpl.FindEventDeep(type, eventName, flags, out _);

        public MethodDefinition GetMethod(TypeDefinition type, string methodName, MemberFlags flags)
            => MonoCecilAssembliesInspectorImpl.FindMethodDeep(type, methodName, flags, out _);

        public bool HasTypeConverter(MemberReference member) => MonoCecilAssembliesInspectorImpl.HasTypeConverter(member);

        public MemberReference GetMemberFromType(
            string memberName,
            TypeDefinition fromType,
            MemberKind lookupFlags,
            out TypeReference declaringType,
            out TypeReference memberType,
            out MemberKind memberKind)
        {
            return _monoCecilVersion.GetMemberFromType(memberName, fromType, lookupFlags, out declaringType, out memberType, out memberKind);
        }
    }
}
