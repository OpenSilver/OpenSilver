
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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

namespace OpenSilver.Compiler;

/// <summary>
/// Owns all per-compile memorization caches used by <see cref="MonoCecilAssembliesInspectorImpl"/>.
///
/// Every cache here is keyed (directly or indirectly) on <see cref="TypeDefinition"/> instances
/// that may belong to an assembly that gets unloaded mid-session, so a stale entry would silently
/// leak memory. Centralizing the caches behind one type lets <see cref="Clear"/> wipe them all
/// atomically and keeps the inspector free of cache plumbing.
///
/// Each public method takes the compute delegate as a parameter so the inspector can pass cached
/// <c>static readonly</c> method-group delegates (no per-call allocations) while the cache itself
/// stays decoupled from inspector state and from <see cref="TypeReferenceHelper"/>.
/// </summary>
internal sealed class MonoCecilLookupCache
{
    private delegate T FindMemberDeepFunc<T>(
        TypeDefinition elementType,
        string name,
        MemberFlags flags,
        out TypeReference ownerElementType);

    private readonly EnumHelper _enumHelper;

    private readonly ConcurrentDictionary<(TypeDefinition, string, MemberFlags), (PropertyDefinition Member, TypeReference Owner)> _propertyCache = [];
    private readonly ConcurrentDictionary<(TypeDefinition, string, MemberFlags), (FieldDefinition Member, TypeReference Owner)> _fieldCache = [];
    private readonly ConcurrentDictionary<(TypeDefinition, string, MemberFlags), (EventDefinition Member, TypeReference Owner)> _eventCache = [];
    private readonly ConcurrentDictionary<(TypeDefinition, string, MemberFlags), (MethodDefinition Member, TypeReference Owner)> _methodCache = [];
    private readonly ConcurrentDictionary<(TypeDefinition, string, bool, bool), string> _enumValueCache = [];

    private readonly ConcurrentDictionary<(TypeDefinition Source, TypeDefinition Target), bool> _isSubclassOfCache = [];
    private readonly ConcurrentDictionary<(TypeDefinition Source, TypeDefinition Interface), bool> _doesImplementInterfaceCache = [];

    public MonoCecilLookupCache(SupportedLanguage language)
    {
        _enumHelper = EnumHelper.Create(this, language);
    }

    public void Clear()
    {
        _propertyCache.Clear();
        _fieldCache.Clear();
        _eventCache.Clear();
        _methodCache.Clear();
        _enumValueCache.Clear();
        _isSubclassOfCache.Clear();
        _doesImplementInterfaceCache.Clear();
    }

    public PropertyDefinition FindProperty(TypeDefinition elementType, string name, MemberFlags flags, out TypeReference ownerElementType)
        => FindMember(_propertyCache, FindPropertyDeep, elementType, name, flags, out ownerElementType);

    public FieldDefinition FindField(TypeDefinition elementType, string name, MemberFlags flags, out TypeReference ownerElementType)
        => FindMember(_fieldCache, FindFieldDeep, elementType, name, flags, out ownerElementType);

    public EventDefinition FindEvent(TypeDefinition elementType, string name, MemberFlags flags, out TypeReference ownerElementType)
        => FindMember(_eventCache, FindEventDeep, elementType, name, flags, out ownerElementType);

    public MethodDefinition FindMethod(TypeDefinition elementType, string name, MemberFlags flags, out TypeReference ownerElementType)
        => FindMember(_methodCache, FindMethodDeep, elementType, name, flags, out ownerElementType);

    public bool IsSubclassOf(TypeDefinition type, TypeDefinition target)
        => GetOrAddRelation(_isSubclassOfCache, TypeDefinitionExtensions.IsSubclassOf, type, target);

    public bool DoesAnySubTypeImplementInterface(TypeDefinition type, TypeDefinition iface)
        => GetOrAddRelation(_doesImplementInterfaceCache, TypeDefinitionExtensions.DoesAnySubTypeImplementInterface, type, iface);

    public bool IsEnum(TypeDefinition type) => _enumHelper.IsEnum(type);

    public string GetEnumValue(TypeDefinition enumType, string name, bool ignoreCase, bool allowIntegerValue)
    {
        var key = (enumType, name, ignoreCase, allowIntegerValue);
        if (_enumValueCache.TryGetValue(key, out var cached))
        {
            return cached;
        }

        var result = _enumHelper.GetEnumValue(enumType, name, ignoreCase, allowIntegerValue);
        _enumValueCache.TryAdd(key, result);
        return result;
    }

    private static T FindMember<T>(
        ConcurrentDictionary<(TypeDefinition, string, MemberFlags), (T Member, TypeReference Owner)> cache,
        FindMemberDeepFunc<T> finder,
        TypeDefinition elementType,
        string name,
        MemberFlags flags,
        out TypeReference ownerElementType) where T : class
    {
        if (elementType is null)
        {
            ownerElementType = null;
            return null;
        }

        var key = (elementType, name, flags);
        if (cache.TryGetValue(key, out var cached))
        {
            ownerElementType = cached.Owner;
            return cached.Member;
        }

        var result = finder(elementType, name, flags, out ownerElementType);
        cache.TryAdd(key, (result, ownerElementType));
        return result;
    }

    private static bool GetOrAddRelation(
        ConcurrentDictionary<(TypeDefinition, TypeDefinition), bool> cache,
        Func<TypeDefinition, TypeDefinition, bool> compute,
        TypeDefinition source,
        TypeDefinition target)
    {
        if (source is null || target is null)
        {
            return false;
        }

        var key = (source, target);
        if (cache.TryGetValue(key, out bool cached))
        {
            return cached;
        }

        bool result = compute(source, target);
        cache.TryAdd(key, result);
        return result;
    }

    private static PropertyDefinition FindPropertyDeep(
        TypeDefinition elementType,
        string propertyName,
        MemberFlags flags,
        out TypeReference ownerElementType)
    {
        bool ignoreCaseFlag = TestMemberFlag(flags, MemberFlags.IgnoreCase);
        bool staticFlag = TestMemberFlag(flags, MemberFlags.Static);
        bool instanceFlag = TestMemberFlag(flags, MemberFlags.Instance);
        bool publicFlag = TestMemberFlag(flags, MemberFlags.Public);
        bool nonPublicFlag = TestMemberFlag(flags, MemberFlags.NonPublic);

        if ((!staticFlag && !instanceFlag) || (!publicFlag && !nonPublicFlag))
        {
            ownerElementType = null;
            return null;
        }

        ownerElementType = elementType;

        while (ownerElementType is not null)
        {
            var resolved = ownerElementType.ResolveOrThrow();

            foreach (var property in resolved.Properties)
            {
                if (string.Compare(property.Name, propertyName, ignoreCaseFlag) != 0)
                {
                    continue;
                }

                if (staticFlag != instanceFlag && staticFlag != IsStatic(property))
                {
                    continue;
                }

                if (publicFlag != nonPublicFlag && publicFlag != IsPublic(property))
                {
                    continue;
                }

                return property;
            }

            ownerElementType = resolved.BaseType?.PopulateGeneric(elementType, ownerElementType);
        }

        return null;

        static bool IsStatic(PropertyDefinition property)
        {
            return property.GetMethod is not null && property.GetMethod.IsStatic ||
                   property.SetMethod is not null && property.SetMethod.IsStatic;
        }

        static bool IsPublic(PropertyDefinition property)
        {
            return property.GetMethod is not null && property.GetMethod.IsPublic ||
                   property.SetMethod is not null && property.SetMethod.IsPublic;
        }
    }

    private static FieldDefinition FindFieldDeep(
        TypeDefinition elementType,
        string name,
        MemberFlags flags,
        out TypeReference ownerElementType)
    {
        bool ignoreCaseFlag = TestMemberFlag(flags, MemberFlags.IgnoreCase);
        bool staticFlag = TestMemberFlag(flags, MemberFlags.Static);
        bool instanceFlag = TestMemberFlag(flags, MemberFlags.Instance);
        bool publicFlag = TestMemberFlag(flags, MemberFlags.Public);
        bool nonPublicFlag = TestMemberFlag(flags, MemberFlags.NonPublic);

        if ((!staticFlag && !instanceFlag) || (!publicFlag && !nonPublicFlag))
        {
            ownerElementType = null;
            return null;
        }

        ownerElementType = elementType;

        while (ownerElementType is not null)
        {
            var resolved = ownerElementType.ResolveOrThrow();

            foreach (var field in resolved.Fields)
            {
                if (string.Compare(field.Name, name, ignoreCaseFlag) != 0)
                {
                    continue;
                }

                if (staticFlag != instanceFlag && staticFlag != field.IsStatic)
                {
                    continue;
                }

                if (publicFlag != nonPublicFlag && publicFlag != field.IsPublic)
                {
                    continue;
                }

                return field;
            }

            ownerElementType = resolved.BaseType?.PopulateGeneric(elementType, ownerElementType);
        }

        return null;
    }

    private static EventDefinition FindEventDeep(
        TypeDefinition elementType,
        string eventName,
        MemberFlags flags,
        out TypeReference ownerElementType)
    {
        bool ignoreCaseFlag = TestMemberFlag(flags, MemberFlags.IgnoreCase);
        bool staticFlag = TestMemberFlag(flags, MemberFlags.Static);
        bool instanceFlag = TestMemberFlag(flags, MemberFlags.Instance);
        bool publicFlag = TestMemberFlag(flags, MemberFlags.Public);
        bool nonPublicFlag = TestMemberFlag(flags, MemberFlags.NonPublic);

        if ((!staticFlag && !instanceFlag) || (!publicFlag && !nonPublicFlag))
        {
            ownerElementType = null;
            return null;
        }

        ownerElementType = elementType;

        while (ownerElementType is not null)
        {
            var resolved = ownerElementType.ResolveOrThrow();

            foreach (var eventDefinition in resolved.Events)
            {
                if (string.Compare(eventDefinition.Name, eventName, ignoreCaseFlag) != 0)
                {
                    continue;
                }

                if (staticFlag != instanceFlag && staticFlag != IsStatic(eventDefinition))
                {
                    continue;
                }

                if (publicFlag != nonPublicFlag && publicFlag != IsPublic(eventDefinition))
                {
                    continue;
                }

                return eventDefinition;
            }

            ownerElementType = resolved.BaseType?.PopulateGeneric(elementType, ownerElementType);
        }

        return null;

        static bool IsStatic(EventDefinition eventDefinition)
        {
            return eventDefinition.AddMethod is not null && eventDefinition.AddMethod.IsStatic ||
                   eventDefinition.RemoveMethod is not null && eventDefinition.RemoveMethod.IsStatic;
        }

        static bool IsPublic(EventDefinition eventDefinition)
        {
            return eventDefinition.AddMethod is not null && eventDefinition.AddMethod.IsPublic ||
                   eventDefinition.RemoveMethod is not null && eventDefinition.RemoveMethod.IsPublic;
        }
    }

    private static MethodDefinition FindMethodDeep(
        TypeDefinition elementType,
        string methodName,
        MemberFlags flags,
        out TypeReference ownerElementType)
    {
        bool ignoreCaseFlag = TestMemberFlag(flags, MemberFlags.IgnoreCase);
        bool staticFlag = TestMemberFlag(flags, MemberFlags.Static);
        bool instanceFlag = TestMemberFlag(flags, MemberFlags.Instance);
        bool publicFlag = TestMemberFlag(flags, MemberFlags.Public);
        bool nonPublicFlag = TestMemberFlag(flags, MemberFlags.NonPublic);

        if ((!staticFlag && !instanceFlag) || (!publicFlag && !nonPublicFlag))
        {
            ownerElementType = null;
            return null;
        }

        ownerElementType = elementType;

        while (ownerElementType is not null)
        {
            var resolved = ownerElementType.ResolveOrThrow();

            foreach (var method in resolved.Methods)
            {
                if (string.Compare(method.Name, methodName, ignoreCaseFlag) != 0)
                {
                    continue;
                }

                if (staticFlag != instanceFlag && staticFlag != method.IsStatic)
                {
                    continue;
                }

                if (publicFlag != nonPublicFlag && publicFlag != method.IsPublic)
                {
                    continue;
                }

                return method;
            }

            ownerElementType = resolved.BaseType?.PopulateGeneric(elementType, ownerElementType);
        }

        return null;
    }

    private static bool TestMemberFlag(MemberFlags flags, MemberFlags value) => (flags & value) == value;

    private abstract partial class EnumHelper
    {
        private sealed class EnumHelperCS : EnumHelper
        {
            private readonly MonoCecilLookupCache _cache;

            public EnumHelperCS(MonoCecilLookupCache cache)
            {
                _cache = cache;
            }

            public override string GetEnumValue(TypeDefinition enumType, string name, bool ignoreCase, bool allowIntegerValue)
            {
                Debug.Assert(enumType is not null && IsEnum(enumType));

                name = name.Trim();

                MemberFlags flags = ignoreCase ?
                    MemberFlags.IgnoreCase | MemberFlags.Public | MemberFlags.Static :
                    MemberFlags.Public | MemberFlags.Static;

                var field = _cache.FindField(
                    enumType,
                    name,
                    flags,
                    out _);

                if (field is not null)
                {
                    return $"global::{TypeReferenceHelper.CSharp.ConvertToString(enumType)}.{field.Name}";
                }
                if (allowIntegerValue)
                {
                    if (long.TryParse(name, out long l))
                    {
                        return $"(global::{TypeReferenceHelper.CSharp.ConvertToString(enumType)}){l}";
                    }
                    if (ulong.TryParse(name, out ulong ul))
                    {
                        return $"(global::{TypeReferenceHelper.CSharp.ConvertToString(enumType)}){ul}";
                    }
                }
                return null;
            }
        }

        private sealed class EnumHelperVB : EnumHelper
        {
            private readonly MonoCecilLookupCache _cache;

            public EnumHelperVB(MonoCecilLookupCache cache)
            {
                _cache = cache;
            }

            public override string GetEnumValue(TypeDefinition enumType, string name, bool ignoreCase, bool allowIntegerValue)
            {
                Debug.Assert(enumType is not null && IsEnum(enumType));

                name = name.Trim();

                MemberFlags flags = ignoreCase ?
                    MemberFlags.IgnoreCase | MemberFlags.Public | MemberFlags.Static :
                    MemberFlags.Public | MemberFlags.Static;

                var field = _cache.FindField(
                    enumType,
                    name,
                    flags,
                    out _);

                if (field is not null)
                {
                    return $"Global.{TypeReferenceHelper.VisualBasic.ConvertToString(enumType)}.{field.Name}";
                }
                if (allowIntegerValue)
                {
                    if (long.TryParse(name, out long l))
                    {
                        return $"CType({l}, Global.{TypeReferenceHelper.VisualBasic.ConvertToString(enumType)})";
                    }
                    if (ulong.TryParse(name, out ulong ul))
                    {
                        return $"CType({ul}, Global.{TypeReferenceHelper.VisualBasic.ConvertToString(enumType)})";
                    }
                }
                return null;
            }
        }

        private sealed class EnumHelperFS : EnumHelper
        {
            private readonly MonoCecilLookupCache _cache;

            public EnumHelperFS(MonoCecilLookupCache cache)
            {
                _cache = cache;
            }

            public override string GetEnumValue(TypeDefinition enumType, string name, bool ignoreCase, bool allowIntegerValue)
            {
                Debug.Assert(enumType is not null && IsEnum(enumType));

                name = name.Trim();

                MemberFlags flags = ignoreCase ?
                    MemberFlags.IgnoreCase | MemberFlags.Public | MemberFlags.Static :
                    MemberFlags.Public | MemberFlags.Static;

                MemberReference member = _cache.FindField(
                    enumType,
                    name,
                    flags,
                    out _);

                member ??= _cache.FindProperty(
                    enumType,
                    name,
                    MemberFlags.Public | MemberFlags.NonPublic | MemberFlags.Static,
                    out _);

                if (member is not null)
                {
                    return $"global.{TypeReferenceHelper.FSharp.ConvertToString(enumType)}.{member.Name}";
                }

                if (allowIntegerValue)
                {
                    if (long.TryParse(name, out long l))
                    {
                        return $"enum<global.{TypeReferenceHelper.FSharp.ConvertToString(enumType)}> {l}";
                    }
                    if (ulong.TryParse(name, out ulong ul))
                    {
                        return $"enum<global.{TypeReferenceHelper.FSharp.ConvertToString(enumType)}> {ul}";
                    }
                }
                return null;
            }

            public override bool IsEnum(TypeDefinition type)
            {
                return base.IsEnum(type) || type.CustomAttributes.Any(attr => attr.AttributeType.FullName == "Microsoft.FSharp.Core.CompilationMappingAttribute");
            }
        }

        public static EnumHelper Create(MonoCecilLookupCache cache, SupportedLanguage language)
        {
            return language switch
            {
                SupportedLanguage.CSharp => new EnumHelperCS(cache),
                SupportedLanguage.VBNet => new EnumHelperVB(cache),
                SupportedLanguage.FSharp => new EnumHelperFS(cache),
                _ => throw new InvalidCompilerTypeException(),
            };
        }

        public abstract string GetEnumValue(TypeDefinition enumType, string name, bool ignoreCase, bool allowIntegerValue);

        public virtual bool IsEnum(TypeDefinition type)
        {
            Debug.Assert(type is not null);
            return type.IsEnum;
        }
    }
}
