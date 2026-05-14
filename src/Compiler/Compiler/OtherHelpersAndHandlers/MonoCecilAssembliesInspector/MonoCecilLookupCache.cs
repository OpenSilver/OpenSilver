
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
using System.Collections;
using System.Collections.Concurrent;

namespace OpenSilver.Compiler;

/// <summary>
/// Signature of the static Find*Deep helpers exposed by <see cref="MonoCecilAssembliesInspectorImpl"/>.
/// Defined at namespace scope so the inspector and the cache can share it without a cyclic
/// reference and so static method-group conversions can be cached in <c>static readonly</c>
/// fields (avoiding a per-call allocation).
/// </summary>
internal delegate T FindMemberDeepFunc<T>(
    TypeDefinition elementType,
    string name,
    MemberFlags flags,
    out TypeReference ownerElementType);

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
    // Member-lookup caches. Keyed by (declaringType, name, flags). The owner TypeReference
    // is captured alongside the member because Find*Deep walks the inheritance chain and
    // returns the type where the member was actually defined.
    private readonly ConcurrentDictionary<(TypeDefinition, string, MemberFlags), (PropertyDefinition Member, TypeReference Owner)> _propertyCache = [];
    private readonly ConcurrentDictionary<(TypeDefinition, string, MemberFlags), (FieldDefinition Member, TypeReference Owner)> _fieldCache = [];
    private readonly ConcurrentDictionary<(TypeDefinition, string, MemberFlags), (EventDefinition Member, TypeReference Owner)> _eventCache = [];
    private readonly ConcurrentDictionary<(TypeDefinition, string, MemberFlags), (MethodDefinition Member, TypeReference Owner)> _methodCache = [];
    private readonly ConcurrentDictionary<(TypeDefinition, string, bool, bool), string> _enumValueCache = [];

    // Inheritance-walk caches. Each IsXxx predicate on the inspector ultimately calls one of
    // these two primitives, which traverse the entire base-class chain (and for interfaces,
    // every interface impl). On a single XAML file we see thousands of repeated walks over
    // the same (type, target) pair; the cache turns those into O(1) lookups.
    private readonly ConcurrentDictionary<(TypeDefinition Source, TypeDefinition Target), bool> _isSubclassOfCache = [];
    private readonly ConcurrentDictionary<(TypeDefinition Source, TypeDefinition Interface), bool> _doesImplementInterfaceCache = [];

    // Bundle of all caches above so Clear can reset them in a single loop.
    private readonly IDictionary[] _all;

    public MonoCecilLookupCache()
    {
        _all =
        [
            _propertyCache,
            _fieldCache,
            _eventCache,
            _methodCache,
            _enumValueCache,
            _isSubclassOfCache,
            _doesImplementInterfaceCache,
        ];
    }

    public void Clear()
    {
        foreach (var c in _all)
        {
            c.Clear();
        }
    }

    public PropertyDefinition FindProperty(
        TypeDefinition elementType, string name, MemberFlags flags,
        FindMemberDeepFunc<PropertyDefinition> finder, out TypeReference ownerElementType)
        => FindMember(_propertyCache, finder, elementType, name, flags, out ownerElementType);

    public FieldDefinition FindField(
        TypeDefinition elementType, string name, MemberFlags flags,
        FindMemberDeepFunc<FieldDefinition> finder, out TypeReference ownerElementType)
        => FindMember(_fieldCache, finder, elementType, name, flags, out ownerElementType);

    public EventDefinition FindEvent(
        TypeDefinition elementType, string name, MemberFlags flags,
        FindMemberDeepFunc<EventDefinition> finder, out TypeReference ownerElementType)
        => FindMember(_eventCache, finder, elementType, name, flags, out ownerElementType);

    public MethodDefinition FindMethod(
        TypeDefinition elementType, string name, MemberFlags flags,
        FindMemberDeepFunc<MethodDefinition> finder, out TypeReference ownerElementType)
        => FindMember(_methodCache, finder, elementType, name, flags, out ownerElementType);

    public bool IsSubclassOf(
        TypeDefinition type, TypeDefinition target,
        Func<TypeDefinition, TypeDefinition, bool> compute)
        => GetOrAddRelation(_isSubclassOfCache, compute, type, target);

    public bool DoesAnySubTypeImplementInterface(
        TypeDefinition type, TypeDefinition iface,
        Func<TypeDefinition, TypeDefinition, bool> compute)
        => GetOrAddRelation(_doesImplementInterfaceCache, compute, type, iface);

    public string GetEnumValue(
        TypeDefinition enumType, string name, bool ignoreCase, bool allowIntegerValue,
        Func<TypeDefinition, string, bool, bool, string> compute)
    {
        var key = (enumType, name, ignoreCase, allowIntegerValue);
        if (_enumValueCache.TryGetValue(key, out var cached))
        {
            return cached;
        }

        var result = compute(enumType, name, ignoreCase, allowIntegerValue);
        _enumValueCache.TryAdd(key, result);
        return result;
    }

    private static T FindMember<T>(
        ConcurrentDictionary<(TypeDefinition, string, MemberFlags), (T Member, TypeReference Owner)> cache,
        FindMemberDeepFunc<T> finder,
        TypeDefinition elementType, string name, MemberFlags flags,
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
        TypeDefinition source, TypeDefinition target)
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
}
