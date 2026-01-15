
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
using System.Runtime.CompilerServices;

namespace OpenSilver.Internal.Data;

internal sealed class DynamicPropertyAccessor : PropertyAccessor
{
    private CallSite<Func<CallSite, object, object>> _getter;
    private CallSite<Action<CallSite, object, object>> _setter;

    public DynamicPropertyAccessor(string propertyName)
    {
        PropertyName = propertyName;
    }

    public override Type PropertyType => typeof(object);

    public override string PropertyName { get; }

    public override object GetValue(object component)
    {
        if (_getter is null)
        {
            var binder = new TrivialGetMemberBinder(PropertyName);
            _getter = CallSite<Func<CallSite, object, object>>.Create(binder);
        }

        return _getter.Target(_getter, component);
    }

    public override void SetValue(object component, object value)
    {
        if (_setter is null)
        {
            var binder = new TrivialSetMemberBinder(PropertyName);
            _setter = CallSite<Action<CallSite, object, object>>.Create(binder);
        }

        _setter.Target(_setter, component, value);
    }

    private sealed class TrivialGetMemberBinder : GetMemberBinder
    {
        public TrivialGetMemberBinder(string propertyName)
            : base(propertyName, false)
        {
        }

        public override DynamicMetaObject FallbackGetMember(
            DynamicMetaObject target,
            DynamicMetaObject errorSuggestion)
        {
            return errorSuggestion ?? TrivialBinderHelper.ThrowMissingMemberExpression(target, Name, ReturnType);
        }
    }

    private sealed class TrivialSetMemberBinder : SetMemberBinder
    {
        public TrivialSetMemberBinder(string propertyName)
            : base(propertyName, false)
        {
        }

        public override DynamicMetaObject FallbackSetMember(
            DynamicMetaObject target,
            DynamicMetaObject value,
            DynamicMetaObject errorSuggestion)
        {
            return errorSuggestion ?? TrivialBinderHelper.ThrowMissingMemberExpression(target, Name, ReturnType);
        }
    }
}
