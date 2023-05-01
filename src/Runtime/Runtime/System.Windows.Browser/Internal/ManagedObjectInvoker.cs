
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

using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using JSTYPE = System.Windows.Browser.ScriptObject.JSTYPE;
using JSParam = System.Windows.Browser.ScriptObject.JSParam;

namespace System.Windows.Browser.Internal;

internal sealed class ManagedObjectInvoker
{
    private const BindingFlags DeclaredOnlyFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private static readonly MethodInfo _onEvent =
        typeof(ScriptObject).GetMethod(
            nameof(ScriptObject.OnEvent),
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private readonly object _managedObject;
    private readonly Dictionary<string, List<Delegate>> _eventHandlers = new();

    public ManagedObjectInvoker(ManagedObject o)
    {
        _managedObject = o.ManagedObject;
    }

    public string GetPropertyValue(string name)
    {
        if (GetScriptableProperty(name) is PropertyInfo pi)
        {
            object o;

            try
            {
                o = pi.GetValue(_managedObject);
            }
            catch (Exception ex)
            {
                return MarshalError(ex);
            }

            return MarshalValue(o);
        }

        if (IsKnownMethod(name) || HasScriptableMethod(name))
        {
            return MarshalMember(name);
        }

        return MarshalNotFoundError();
    }

    public string SetPropertyValue(string name, string value)
    {
        if (GetScriptableProperty(name) is PropertyInfo pi)
        {
            JSParam jsValue = JsonSerializer.Deserialize<JSParam>(value);
            try
            {
                pi.SetValue(_managedObject, ConvertJSArgument(jsValue, pi.PropertyType));
                return MarshalVoid();
            }
            catch (Exception ex)
            {
                return MarshalError(ex);
            }
        }
        
        return MarshalNotFoundError();
    }

    public string InvokeMethod(string name, string args)
    {
        JSParam[] jsArgs = JsonSerializer.Deserialize<JSParam[]>(args);

        if (jsArgs.Length == 0 && name == "toString")
        {
            return MarshalValue(_managedObject.ToString());
        }

        MethodInfo method = GetScriptableMethod(name, jsArgs);
        if (method is null)
        {
            return MarshalNotFoundError();
        }

        object o;

        try
        {
            object[] arguments = ConvertJSArguments(jsArgs, method);
            o = method.Invoke(_managedObject, arguments);
        }
        catch (Exception ex)
        {
            return MarshalError(ex);
        }

        return MarshalValue(o);
    }

    public string InvokeSelf(string args)
    {
        JSParam[] jsArgs = JsonSerializer.Deserialize<JSParam[]>(args);

        if (_managedObject is not Delegate d)
        {
            return MarshalError("Object must be a Delegate.");
        }

        object o;

        try
        {
            object[] arguments = ConvertJSArguments(jsArgs, d.Method);
            o = d.DynamicInvoke(arguments);
        }
        catch (Exception ex)
        {
            return MarshalError(ex);
        }

        return MarshalValue(o);
    }

    public string AddEventHandler(string name, string handler, bool add)
    {
        if (GetScriptableEvent(name) is EventInfo eventInfo)
        {
            ScriptObject so = ScriptObject.Unwrap(JsonSerializer.Deserialize<JSParam>(handler)) as ScriptObject;

            try
            {
                Delegate d = GetDelegateFromScriptObject(so, eventInfo.EventHandlerType);
                if (add)
                {
                    eventInfo.AddEventHandler(_managedObject, d);
                }
                else
                {
                    eventInfo.RemoveEventHandler(_managedObject, d);
                }
            }
            catch (Exception ex)
            {
                return MarshalError(ex);
            }
        }

        return MarshalVoid();
    }

    private Delegate GetDelegateFromScriptObject(ScriptObject so, Type eventType)
    {
        if (so.ManagedObject is Delegate d)
        {
            return d;
        }

        return Delegate.CreateDelegate(eventType, so, _onEvent);
    }

    private static object[] ConvertJSArguments(JSParam[] jsArgs, MethodInfo method)
    {
        ParameterInfo[] parameters = method.GetParameters();
        object[] arguments = new object[jsArgs.Length];

        for (int i = 0; i < jsArgs.Length; i++)
        {
            arguments[i] = ConvertJSArgument(jsArgs[i], parameters[i].ParameterType);
        }

        return arguments;
    }

    private static object ConvertJSArgument(JSParam param, Type type)
    {
        return param.Type switch
        {
            JSTYPE.STRING => Convert.ChangeType(param.Value, type, CultureInfo.InvariantCulture),
            JSTYPE.INTEGER => Convert.ChangeType(int.Parse(param.Value, CultureInfo.InvariantCulture), type, CultureInfo.InvariantCulture),
            JSTYPE.DOUBLE => Convert.ChangeType(double.Parse(param.Value, CultureInfo.InvariantCulture), type, CultureInfo.InvariantCulture),
            JSTYPE.BOOLEAN => Convert.ChangeType(bool.Parse(param.Value), type, CultureInfo.InvariantCulture),
            JSTYPE.OBJECT => ScriptObject.GetOrCreateScriptObject(param.Value),
            JSTYPE.HTMLELEMENT => ScriptObject.GetOrCreateHtmlElement(param.Value),
            JSTYPE.HTMLCOLLECTION => ScriptObject.GetOrCreateScriptObjectCollection(param.Value),
            JSTYPE.HTMLDOCUMENT => HtmlPage.Document,
            JSTYPE.HTMLWINDOW => HtmlPage.Window,
            _ => null,
        };
    }

    private PropertyInfo GetScriptableProperty(string name)
    {
        PropertyInfo prop = null;

        Type baseType = _managedObject.GetType();
        Type objectType = typeof(object);

        do
        {
            bool isScriptableType = IsScriptableType(baseType);

            foreach (PropertyInfo p in baseType.GetProperties(DeclaredOnlyFlags))
            {
                string scriptMemberName = p.Name;

                ScriptableMemberAttribute attr = p.GetCustomAttribute<ScriptableMemberAttribute>();
                if (attr is not null && !string.IsNullOrEmpty(attr.ScriptAlias))
                {
                    scriptMemberName = attr.ScriptAlias;
                }

                if ((isScriptableType || attr is not null) && scriptMemberName == name)
                {
                    prop = p;
                    break;
                }
            }

            baseType = baseType.BaseType;
        }
        while (prop is null && baseType != objectType);

        return prop;
    }

    private static bool IsKnownMethod(string name)
    {
        return name == "toString";
    }

    private bool HasScriptableMethod(string name)
    {
        Type baseType = _managedObject.GetType();
        Type objectType = typeof(object);

        do
        {
            bool isScriptableType = IsScriptableType(baseType);

            foreach (MethodInfo m in baseType.GetMethods(DeclaredOnlyFlags))
            {
                string scriptMemberName = m.Name;

                ScriptableMemberAttribute attr = m.GetCustomAttribute<ScriptableMemberAttribute>();
                if (attr is not null && !string.IsNullOrEmpty(attr.ScriptAlias))
                {
                    scriptMemberName = attr.ScriptAlias;
                }

                if ((isScriptableType || attr is not null) && scriptMemberName == name)
                {
                    return true;
                }
            }

            baseType = baseType.BaseType;
        }
        while (baseType != objectType);

        return false;
    }

    private MethodInfo GetScriptableMethod(string name, JSParam[] arguments)
    {
        MethodInfo method = null;

        Type baseType = _managedObject.GetType();
        Type objectType = typeof(object);

        do
        {
            bool isScriptableType = IsScriptableType(baseType);

            foreach (MethodInfo m in baseType.GetMethods(DeclaredOnlyFlags))
            {
                string scriptMemberName = m.Name;

                ScriptableMemberAttribute attr = m.GetCustomAttribute<ScriptableMemberAttribute>();
                if (attr is not null && !string.IsNullOrEmpty(attr.ScriptAlias))
                {
                    scriptMemberName = attr.ScriptAlias;
                }

                if ((isScriptableType || attr is not null) && scriptMemberName == name
                    && CanInvokeMethodWithArguments(m, arguments))
                {
                    method = m;
                    break;
                }
            }

            baseType = baseType.BaseType;
        }
        while (method is null && baseType != objectType);

        return method;
    }

    private EventInfo GetScriptableEvent(string name)
    {
        EventInfo ev = null;

        Type baseType = _managedObject.GetType();
        Type objectType = typeof(object);

        do
        {
            bool isScriptableType = IsScriptableType(baseType);

            foreach (EventInfo e in baseType.GetEvents(DeclaredOnlyFlags))
            {
                string scriptMemberName = e.Name;

                ScriptableMemberAttribute attr = e.GetCustomAttribute<ScriptableMemberAttribute>();
                if (attr is not null && !string.IsNullOrEmpty(attr.ScriptAlias))
                {
                    scriptMemberName = attr.ScriptAlias;
                }

                if ((isScriptableType || attr is not null) && scriptMemberName == name)
                {
                    ev = e;
                    break;
                }
            }

            baseType = baseType.BaseType;
        }
        while (ev is null && baseType != objectType);

        return ev;
    }    

    private static bool CanInvokeMethodWithArguments(MethodInfo method, JSParam[] arguments)
    {
        ParameterInfo[] parameters = method.GetParameters();
        if (parameters.Length != arguments.Length)
        {
            return false;
        }

        for (int i = 0; i < parameters.Length; i++)
        {
            if (!AreTypesCompatible(parameters[i].ParameterType, arguments[i].Type))
            {
                return false;
            }
        }

        return true;
    }

    private static bool AreTypesCompatible(Type type, JSTYPE jsType)
    {
        if (type == typeof(object))
        {
            return true;
        }

        return jsType switch
        {
            JSTYPE.STRING or JSTYPE.INTEGER or JSTYPE.DOUBLE or JSTYPE.BOOLEAN => typeof(IConvertible).IsAssignableFrom(type),
            JSTYPE.HTMLELEMENT => type.IsAssignableFrom(typeof(HtmlElement)),
            JSTYPE.HTMLCOLLECTION => type.IsAssignableFrom(typeof(ScriptObjectCollection)),
            JSTYPE.HTMLDOCUMENT => type.IsAssignableFrom(typeof(HtmlDocument)),
            JSTYPE.HTMLWINDOW => type.IsAssignableFrom(typeof(HtmlWindow)),
            _ => true,
        };
    }

    private static bool IsScriptableType(Type type) => type.IsDefined(typeof(ScriptableTypeAttribute), false);

    private static string MarshalValue(object o) => MarshalAsString(INTEROP_RESULT.OBJECT, o);

    private static string MarshalMember(string memberName) => MarshalAsString(INTEROP_RESULT.MEMBER, memberName);

    private static string MarshalVoid() => MarshalAsString(INTEROP_RESULT.VOID, string.Empty);

    private static string MarshalError(string errorMessage) => MarshalAsString(INTEROP_RESULT.ERROR, errorMessage);

    private static string MarshalError(Exception ex)
    {
        while (ex.InnerException is not null)
        {
            ex = ex.InnerException;
        }

        return MarshalError(ex.ToString());
    }

    private static string MarshalNotFoundError() => MarshalError("Object doesn't support this property or method");

    private static string MarshalAsString(INTEROP_RESULT type, object value)
        => $"({{ type: {(int)type}, value: {ScriptObject.ConvertToJavaScriptParam(value)} }})";

    private enum INTEROP_RESULT
    {
        ERROR = 0,
        VOID = 1,
        OBJECT = 2,
        MEMBER = 3,
    };
}
