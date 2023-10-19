
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

using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Browser.Internal;
using System.Windows.Threading;
using CSHTML5.Internal;

namespace System.Windows.Browser
{
    /// <summary>
    /// Defines the core behavior for the <see cref="HtmlObject"/> class, and
    /// provides a base class for browser Document Object Model (DOM) access types.
    /// </summary>
    public class ScriptObject : IDynamicMetaObjectProvider
    {
        private readonly IJSObjectRef _jsObjectRef;

        static ScriptObject()
        {
            string getMemberCallback = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(
                JavaScriptCallback.Create(GetMemberFromJS, true));
            string setPropertyCallback = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(
                JavaScriptCallback.Create(SetPropertyFromJS, true));
            string invokeMethodCallback = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(
                JavaScriptCallback.Create(InvokeMethodFromJS, true));
            string addEventListenerCallback = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(
                JavaScriptCallback.Create(AddEventListenerFromJS, true));

            OpenSilver.Interop.ExecuteJavaScriptVoid(
                $"document.browserService.initialize({getMemberCallback}, {setPropertyCallback}, {invokeMethodCallback}, {addEventListenerCallback});");
        }

        internal ScriptObject(IJSObjectRef jsObject)
            : this(jsObject, null)
        {
        }

        internal ScriptObject(IJSObjectRef jsObject, object managedObject)
        {
            _jsObjectRef = jsObject;
            ManagedObject = managedObject;
        }

        /// <summary>
        /// Frees resources and performs other cleanup operations before the scriptable object
        /// is reclaimed by garbage collection.
        /// </summary>
        ~ScriptObject() => Dispose(false);

        /// <summary>
        /// Gets an instance of the dispatcher.
        /// </summary>
        /// <returns>
        /// The dispatcher associated with the user interface (UI) thread.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Dispatcher Dispatcher => Dispatcher.CurrentDispatcher;

        /// <summary>
        /// Gets the underlying managed object reference of the <see cref="ScriptObject"/>.
        /// </summary>
        /// <returns>
        /// A managed object reference if the current <see cref="ScriptObject"/>
        /// wraps a managed type; otherwise, null.
        /// </returns>
        public object ManagedObject { get; }

        /// <summary>
        /// Determines whether the current thread is the browser's UI thread.
        /// </summary>
        /// <returns>
        /// true if the current thread is the browser's UI thread; false if it is a background
        /// thread.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool CheckAccess() => Dispatcher.CurrentDispatcher.CheckAccess();

        /// <summary>
        /// Converts the current scriptable object to a specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type to convert the current scriptable object to.
        /// </typeparam>
        /// <returns>
        /// An object of type T.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The conversion failed or is not supported.
        /// </exception>
        [OpenSilver.NotImplemented]
        public T ConvertTo<T>()
        {
            return default(T);
        }

        /// <summary>
        /// Gets the value of a property that is identified by ordinal number on the current
        /// scriptable object.
        /// </summary>
        /// <param name="index">
        /// The ordinal number of the property.
        /// </param>
        /// <returns>
        /// A null reference if the property does not exist or
        /// if the underlying <see cref="ScriptObject"/> is a managed type.
        /// </returns>
        public object GetProperty(int index)
        {
            return GetProperty(index.ToString());
        }

        /// <summary>
        /// Gets the value of a property that is identified by name on the current scriptable
        /// object.
        /// </summary>
        /// <param name="name">
        /// The name of the property.
        /// </param>
        /// <returns>
        /// A null reference if the property does not exist or
        /// if the underlying <see cref="ScriptObject"/> is a managed type.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// name is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// name is an empty string.-or-name contains an embedded null character (<see cref="char.MinValue"/>).
        /// </exception>
        public virtual object GetProperty(string name)
        {
            ValidateParameter(name);

            string sJSObj = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_jsObjectRef);

            JSParam result = JsonSerializer.Deserialize<JSParam>(
                OpenSilver.Interop.ExecuteJavaScriptString(
                    $"document.browserService.getProperty({sJSObj}, '{name}');"));

            return Unwrap(result);
        }

        /// <summary>
        /// Invokes a method on the current scriptable object, and optionally passes in one
        /// or more method parameters.
        /// </summary>
        /// <param name="name">
        /// The method to invoke.
        /// </param>
        /// <param name="args">
        /// Parameters to be passed to the method.
        /// </param>
        /// <returns>
        /// An object that represents the return value from the underlying JavaScript method.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// name is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// name is an empty string.-or-name contains an embedded null character (<see cref="char.MinValue"/>).
        /// -or-The method does not exist or is not scriptable.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The underlying method invocation results in an error. The .NET Framework attempts
        /// to return the error text that is associated with the error.
        /// </exception>
        public virtual object Invoke(string name, params object[] args)
        {
            ValidateParameter(name);

            string sJSObj = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_jsObjectRef);
            string sParams = args is null ? string.Empty : string.Join(",", args.Select(a => ConvertToJavaScriptParam(a)));

            JSParam result = JsonSerializer.Deserialize<JSParam>(
                OpenSilver.Interop.ExecuteJavaScriptString(
                    $"document.browserService.invoke({sJSObj}, '{name}', {sParams});"));

            return Unwrap(result);
        }

        /// <summary>
        /// Invokes the current <see cref="ScriptObject"/> and assumes that it represents
        /// a JavaScript method.
        /// </summary>
        /// <param name="args">
        /// Parameters to be passed to the underlying JavaScript method.
        /// </param>
        /// <returns>
        /// An object that represents the return value from the underlying JavaScript method.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="ScriptObject"/> is not a method.-or-The underlying
        /// method invocation results in an error.
        /// </exception>
        public virtual object InvokeSelf(params object[] args)
        {
            string sJSObj = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_jsObjectRef);
            string sParams = args is null ? string.Empty : string.Join(",", args.Select(a => ConvertToJavaScriptParam(a)));

            JSParam result = JsonSerializer.Deserialize<JSParam>(
                OpenSilver.Interop.ExecuteJavaScriptString(
                    $"document.browserService.invokeSelf({sJSObj}, {sParams});"));

            return Unwrap(result);
        }

        /// <summary>
        /// Sets the value of a property that is identified by ordinal number on the current
        /// scriptable object.
        /// </summary>
        /// <param name="index">
        /// The ordinal number of the property.
        /// </param>
        /// <param name="value">
        /// The value to set the property to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// index is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// index identifies an empty string.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// A type mismatch exists between the supplied type and the target property.-or-The
        /// property is not settable.-or-All other failures.
        /// </exception>
        public void SetProperty(int index, object value) => SetProperty(index.ToString(), value);
        
        /// <summary>
        /// Sets a property that is identified by name on the current scriptable object.
        /// </summary>
        /// <param name="name">
        /// The name of the property.
        /// </param>
        /// <param name="value">
        /// The value to set the property to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// name is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// name is an empty string.-or-name contains an embedded null character (<see cref="char.MinValue"/>).
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// A type mismatch exists between the supplied type and the target property.-or-The
        /// property is not settable.-or-All other failures.
        /// </exception>
        public virtual void SetProperty(string name, object value)
        {
            ValidateParameter(name);

            string sJSObj = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_jsObjectRef);
            string sValue = ConvertToJavaScriptParam(value);

            JSParam result = JsonSerializer.Deserialize<JSParam>(
                OpenSilver.Interop.ExecuteJavaScriptString(
                    $"document.browserService.setProperty({sJSObj}, '{name}', {sValue});"));

            Exception ex = UnwrapException(result);
            if (ex != null)
            {
                throw ex;
            }
        }

#if false
        /// <summary>
        /// Initializes a scriptable object.
        /// </summary>
        /// <param name="handle">
        /// The handle to the object to initialize.
        /// </param>
        /// <param name="identity">
        /// The identity of the object.
        /// </param>
        /// <param name="addReference">
        /// true to specify that the HTML Bridge should assign a reference count to this
        /// object; otherwise, false.
        /// </param>
        /// <param name="releaseReferenceOnDispose">
        /// true to release the reference on dispose; otherwise, false.
        /// </param>
		[OpenSilver.NotImplemented]
        protected void Initialize(IntPtr handle, IntPtr identity, bool addReference, bool releaseReferenceOnDispose)
        {
        }
#endif

        /// <summary>
        /// Converts the current scriptable object to a specified type, with serialization
        /// support.
        /// </summary>
        /// <param name="targetType">
        /// The type to convert the current scriptable object to.
        /// </param>
        /// <param name="allowSerialization">
        /// A flag which enables the current scriptable object to be serialized.
        /// </param>
        /// <returns>
        /// An object of type targetType.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The conversion failed or is not supported.
        /// </exception>
		[OpenSilver.NotImplemented]
        protected internal virtual object ConvertTo(Type targetType, bool allowSerialization)
        {
            return null;
        }

        /// <summary>
        /// Returns the <see cref="DynamicMetaObject"/> responsible for binding operations
        /// performed on this object.
        /// </summary>
        /// <param name="parameter">
        /// The expression tree representation of the runtime value.
        /// </param>
        /// <returns>
        /// The <see cref="DynamicMetaObject"/> to bind this object.
        /// </returns>
        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression parameter)
        {
            return null;
        }

        internal void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            _jsObjectRef?.Dispose();

            if (ManagedObject is not null)
            {
                _managedObjects.Remove(ManagedObject);
            }
        }

        internal void OnEvent(object sender, EventArgs e) => InvokeSelf(sender, e);

        internal static void ValidateParameter(string name, [CallerArgumentExpression(nameof(name))] string paramName = null)
        {
            CheckNullOrEmpty(name, paramName);
            CheckInvalidCharacters(name, paramName);
        }

        internal static void CheckNullOrEmpty(string name, [CallerArgumentExpression(nameof(name))] string paramName = null)
        {
            if (name is null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (name.Length == 0)
            {
                throw new ArgumentException($"'{paramName}' cannot be empty", paramName);
            }
        }

        internal static void CheckInvalidCharacters(string name, [CallerArgumentExpression(nameof(name))] string paramName = null)
        {
            if (name.IndexOf(char.MinValue) > -1)
            {
                throw new ArgumentException("Invalid identifier. Identifiers may not contain null-terminators.", paramName);
            }
        }

        internal static string ConvertToJavaScriptParam(object value)
        {
            object o = value switch
            {
                ScriptObject so => so._jsObjectRef,
                string or char or Guid => value.ToString(),
                double or float or decimal or int or uint or short or ushort or long or ulong or byte or sbyte or Enum => Convert.ToDouble(value, CultureInfo.InvariantCulture),
                bool or null => value,
                object => OpenSilver.Interop.IsRunningInTheSimulator ? null : GetOrCreateManagedObject(value)._jsObjectRef,
            };

            return CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(o);
        }

        private static ManagedObject GetOrCreateManagedObject(object obj)
        {
            lock (_managedObjects)
            {
                return GetManagedObject(obj) ?? CreateManagedObject(obj);
            }
        }

        private static ManagedObject GetManagedObject(object obj)
            => (_managedObjects.TryGetValue(obj, out WeakReference<ManagedObject> wr) &&
                wr.TryGetTarget(out ManagedObject managedObject))
                ? managedObject
                : null;

        private static ManagedObject CreateManagedObject(object obj)
        {
            string jsRef = OpenSilver.Interop.ExecuteJavaScriptString(
                $"document.browserService.registerManagedObject({(obj is Delegate ? "true" : "false")});");
            var managedObject = new ManagedObject(new JSObjectRef(jsRef), obj);
            RegisterScriptObject(jsRef, managedObject);
            RegisterManagedObject(obj, managedObject);
            return managedObject;
        }

        internal static object Unwrap(JSParam result)
        {
            return result.Type switch
            {
                JSTYPE.ERROR => throw new InvalidOperationException(result.Value),
                JSTYPE.VOID => null,
                JSTYPE.STRING => result.Value,
                JSTYPE.INTEGER => int.Parse(result.Value, CultureInfo.InvariantCulture),
                JSTYPE.DOUBLE => double.Parse(result.Value, CultureInfo.InvariantCulture),
                JSTYPE.BOOLEAN => bool.Parse(result.Value),
                JSTYPE.OBJECT => GetOrCreateScriptObject(result.Value),
                JSTYPE.HTMLELEMENT => GetOrCreateHtmlElement(result.Value),
                JSTYPE.HTMLCOLLECTION => GetOrCreateScriptObjectCollection(result.Value),
                JSTYPE.HTMLWINDOW => HtmlPage.Window,
                JSTYPE.HTMLDOCUMENT => HtmlPage.Document,
                _ => throw new InvalidOperationException(),
            };
        }

        private static Exception UnwrapException(JSParam result)
        {
            return result.Type switch
            {
                JSTYPE.ERROR => new InvalidOperationException(result.Value),
                _ => null,
            };
        }

        private static T GetOrCreateScriptObject<T>(string id, object managedObject, Func<string, object, T> creationFunc)
            where T : ScriptObject
        {
            lock (_objects)
            {
                T o = (T)GetScriptObject(id);
                if (o is null)
                {
                    o = creationFunc(id, managedObject);
                    RegisterScriptObject(id, o);
                }

                return o;
            }
        }

        private static void RegisterScriptObject(string id, ScriptObject scriptObject)
            => _objects[id] = new WeakReference<ScriptObject>(scriptObject);

        private static void RegisterManagedObject(object obj, ManagedObject managedObject)
            => _managedObjects[obj] = new WeakReference<ManagedObject>(managedObject);

        internal static ScriptObject GetOrCreateScriptObject(string id)
        {
            return GetOrCreateScriptObject(id, null, CreateScriptObject);

            static ScriptObject CreateScriptObject(string id, object managedObj) => new(new JSObjectRef(id), managedObj);
        }

        internal static HtmlElement GetOrCreateHtmlElement(string id)
        {
            return GetOrCreateScriptObject(id, null, CreateHtmlElement);

            static HtmlElement CreateHtmlElement(string id, object managedObject) => new(new JSObjectRef(id));
        }

        internal static ScriptObjectCollection GetOrCreateScriptObjectCollection(string id)
        {
            return GetOrCreateScriptObject(id, null, CreateScriptObjectCollection);

            static ScriptObjectCollection CreateScriptObjectCollection(string id, object managedObject) => new(new JSObjectRef(id));
        }

        private static ScriptObject GetScriptObject(string id)
            => (_objects.TryGetValue(id, out WeakReference<ScriptObject> wr) && wr.TryGetTarget(out ScriptObject o)) ? o : null;

        private static ManagedObject GetManagedObject(string id, bool throwIfNotFound = false)
        {
            if (_objects.TryGetValue(id, out WeakReference<ScriptObject> wr)
                && wr.TryGetTarget(out ScriptObject o)
                && o is ManagedObject managedObject)
            {
                return managedObject;
            }

            if (throwIfNotFound)
            {
                throw new InvalidOperationException();
            }

            return null;
        }

        internal static void UnregisterScriptObject(string id)
        {
            lock (_objects)
            {
                _objects.Remove(id);
            }
        }

        private static string GetMemberFromJS(string caller, string memberName)
            => GetManagedObject(caller, true).Invoker.GetPropertyValue(memberName);            

        private static string SetPropertyFromJS(string caller, string propertyName, string value)
            => GetManagedObject(caller, true).Invoker.SetPropertyValue(propertyName, value);

        private static string InvokeMethodFromJS(string caller, string methodName, string args)
        {
            ManagedObject managedObject = GetManagedObject(caller, true);

            if (string.IsNullOrEmpty(methodName))
            {
                return managedObject.Invoker.InvokeSelf(args);
            }
            else
            {
                return managedObject.Invoker.InvokeMethod(methodName, args);
            }
        }

        private static string AddEventListenerFromJS(string caller, string eventName, string handler, bool add)
            => GetManagedObject(caller, true).Invoker.AddEventHandler(eventName, handler, add);

        private static readonly Dictionary<string, WeakReference<ScriptObject>> _objects = new();
        private static readonly Dictionary<object, WeakReference<ManagedObject>> _managedObjects = new();

        internal struct JSParam
        {
            public JSTYPE Type { get; set; }
            public string Value { get; set; }
        }

        internal enum JSTYPE
        {
            ERROR = -1,
            VOID = 0,
            STRING = 1,
            INTEGER = 2,
            DOUBLE = 3,
            BOOLEAN = 4,
            OBJECT = 5,
            HTMLELEMENT = 6,
            HTMLCOLLECTION = 7,
            HTMLDOCUMENT = 8,
            HTMLWINDOW = 9,
        }
    }
}
