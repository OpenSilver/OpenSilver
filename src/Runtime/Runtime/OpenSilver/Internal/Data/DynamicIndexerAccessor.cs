
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

using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace OpenSilver.Internal.Data;

internal sealed class DynamicIndexerAccessor : IndexerAccessor
{
    public const string DynamicIndexerName = "Items";

    private static DynamicIndexerAccessor[] _accessors = new DynamicIndexerAccessor[1];
    private static readonly object _lock = new();

    private readonly CallSite _getterCallSite;
    private readonly CallSite _setterCallSite;
    private readonly MulticastDelegate _getterDelegate;
    private readonly MulticastDelegate _setterDelegate;

    private DynamicIndexerAccessor(int rank)
    {
        var getBinder = new TrivialGetIndexBinder(rank);
        var setBinder = new TrivialSetIndexBinder(rank);

        Type delegateType, callsiteType;
        MethodInfo createMethod;
        FieldInfo targetField;
        Type[] typeArgs;
        int i;

        // getter delegate type:  Func<CallSite, object, ..., object>
        typeArgs = new Type[rank + 3];
        typeArgs[0] = typeof(CallSite);
        for (i = 1; i <= rank + 2; ++i)
        {
            typeArgs[i] = typeof(object);
        }
        delegateType = Expression.GetDelegateType(typeArgs);

        // getter CallSite:  CallSite<Func<CallSite, object, ..., object>>.Create(getBinder)
        callsiteType = typeof(CallSite<>).MakeGenericType([delegateType]);
        createMethod = callsiteType.GetMethod(nameof(CallSite<>.Create), [typeof(CallSiteBinder)]);
        _getterCallSite = (CallSite)createMethod.Invoke(null, [getBinder]);

        // getter delegate:  _getterCallSite.Target
        targetField = callsiteType.GetField(nameof(CallSite<>.Target));
        _getterDelegate = (MulticastDelegate)targetField.GetValue(_getterCallSite);

        // setter delegate type:  Action<CallSite, object, ..., object>
        typeArgs = new Type[rank + 4];
        typeArgs[0] = typeof(CallSite);
        typeArgs[rank + 3] = typeof(void);
        for (i = 1; i <= rank + 2; ++i)
        {
            typeArgs[i] = typeof(object);
        }
        delegateType = Expression.GetDelegateType(typeArgs);

        // setter CallSite:  CallSite<Func<CallSite, object, ..., object>>.Create(setBinder)
        callsiteType = typeof(CallSite<>).MakeGenericType([delegateType]);
        createMethod = callsiteType.GetMethod(nameof(CallSite<>.Create), [typeof(CallSiteBinder)]);
        _setterCallSite = (CallSite)createMethod.Invoke(null, [setBinder]);

        // setter delegate:  _setterCallSite.Target
        targetField = callsiteType.GetField(nameof(CallSite<>.Target));
        _setterDelegate = (MulticastDelegate)targetField.GetValue(_setterCallSite);
    }

    public static DynamicIndexerAccessor GetIndexerAccessor(int rank)
    {
        if (_accessors.Length < rank || _accessors[rank - 1] is null)
        {
            lock (_lock)
            {
                if (_accessors.Length < rank)
                {
                    var newAccessors = new DynamicIndexerAccessor[rank];
                    Array.Copy(_accessors, 0, newAccessors, 0, _accessors.Length);
                    _accessors = newAccessors;
                }

                _accessors[rank - 1] ??= new DynamicIndexerAccessor(rank);
            }
        }

        return _accessors[rank - 1];
    }

    public override Type PropertyType => typeof(object);

    public override string GetPropertyName(string index) => $"{DynamicIndexerName}[{index}]";

    public override object GetValue(object component, object[] args)
    {
        int rank = args.Length;
        var delegateArgs = new object[rank + 2];
        delegateArgs[0] = _getterCallSite;
        delegateArgs[1] = component;
        Array.Copy(args, 0, delegateArgs, 2, rank);

        return _getterDelegate.DynamicInvoke(delegateArgs);
    }

    public override void SetValue(object component, object[] args, object value)
    {
        int rank = args.Length;
        var delegateArgs = new object[rank + 3];
        delegateArgs[0] = _setterCallSite;
        delegateArgs[1] = component;
        Array.Copy(args, 0, delegateArgs, 2, rank);
        delegateArgs[rank + 2] = value;

        _setterDelegate.DynamicInvoke(delegateArgs);
    }

    private sealed class TrivialGetIndexBinder : GetIndexBinder
    {
        public TrivialGetIndexBinder(int rank)
            : base(new CallInfo(rank))
        {
        }

        public override DynamicMetaObject FallbackGetIndex(
            DynamicMetaObject target,
            DynamicMetaObject[] indexes,
            DynamicMetaObject errorSuggestion)
        {
            return errorSuggestion ?? TrivialBinderHelper.ThrowMissingMemberExpression(target, DynamicIndexerName, ReturnType);
        }
    }

    private sealed class TrivialSetIndexBinder : SetIndexBinder
    {
        public TrivialSetIndexBinder(int rank)
            : base(new CallInfo(rank))
        {
        }

        public override DynamicMetaObject FallbackSetIndex(
            DynamicMetaObject target,
            DynamicMetaObject[] indexes,
            DynamicMetaObject value,
            DynamicMetaObject errorSuggestion)
        {
            return errorSuggestion ?? TrivialBinderHelper.ThrowMissingMemberExpression(target, DynamicIndexerName, ReturnType);
        }
    }
}
