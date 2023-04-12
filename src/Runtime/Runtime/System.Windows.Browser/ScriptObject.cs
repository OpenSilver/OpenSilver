
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
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Threading;
#else
using Dispatcher = Windows.UI.Core.CoreDispatcher;
#endif

namespace System.Windows.Browser
{
    /// <summary>
    /// Defines the core behavior for the <see cref="HtmlObject"/> class, and
    /// provides a base class for browser Document Object Model (DOM) access types.
    /// </summary>
    public class ScriptObject : IDynamicMetaObjectProvider
    {
        private readonly IJSObjectRef _jsObjectRef;

        internal ScriptObject(IJSObjectRef jsObject)
        {
            _jsObjectRef = jsObject;
        }

        /// <summary>
        /// Frees resources and performs other cleanup operations before the scriptable object
        /// is reclaimed by garbage collection.
        /// </summary>
        ~ScriptObject()
        {
            _jsObjectRef?.Dispose();
        }

        /// <summary>
        /// Gets an instance of the dispatcher.
        /// </summary>
        /// <returns>
        /// The dispatcher associated with the user interface (UI) thread.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Dispatcher Dispatcher => Dispatcher.INTERNAL_GetCurrentDispatcher();

        /// <summary>
        /// Gets the underlying managed object reference of the <see cref="ScriptObject"/>.
        /// </summary>
        /// <returns>
        /// A managed object reference if the current <see cref="ScriptObject"/>
        /// wraps a managed type; otherwise, null.
        /// </returns>
		[OpenSilver.NotImplemented]
        public object ManagedObject { get; }

        /// <summary>
        /// Determines whether the current thread is the browser's UI thread.
        /// </summary>
        /// <returns>
        /// true if the current thread is the browser's UI thread; false if it is a background
        /// thread.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool CheckAccess() => Dispatcher.INTERNAL_GetCurrentDispatcher().CheckAccess();

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
            ValidateName(name);

            string sJSObj = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_jsObjectRef);

            JSResult result = JsonSerializer.Deserialize<JSResult>(
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
            ValidateName(name);

            string sJSObj = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_jsObjectRef);
            string sParams = args is null ? string.Empty : string.Join(",", args.Select(a => ConvertToJavaScriptParam(a)));

            JSResult result = JsonSerializer.Deserialize<JSResult>(
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

            JSResult result = JsonSerializer.Deserialize<JSResult>(
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
            ValidateName(name);

            string sJSObj = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_jsObjectRef);
            string sValue = ConvertToJavaScriptParam(value);

            JSResult result = JsonSerializer.Deserialize<JSResult>(
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

        private static void ValidateName(string name, [CallerArgumentExpression(nameof(name))] string paramName = null)
        {
            if (name is null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (name.Length == 0)
            {
                throw new ArgumentException($"'{paramName}' cannot be empty", paramName);
            }

            if (name.IndexOf(char.MinValue) > -1)
            {
                throw new ArgumentException("Invalid identifier. Identifiers may not contain null-terminators.", paramName);
            }
        }

        private static string ConvertToJavaScriptParam(object value)
        {
            object o = value switch
            {
                ScriptObject so => so._jsObjectRef,
                _ => value,
            };

            return CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(o);
        }

        private static object Unwrap(JSResult result)
        {
            return result.Type switch
            {
                TYPES.ERROR => throw new InvalidOperationException(result.Value),
                TYPES.VOID => null,
                TYPES.STRING => result.Value,
                TYPES.INTEGER => int.Parse(result.Value, CultureInfo.InvariantCulture),
                TYPES.DOUBLE => double.Parse(result.Value, CultureInfo.InvariantCulture),
                TYPES.BOOLEAN => bool.Parse(result.Value),
                TYPES.OBJECT => GetOrCreateScriptObject(result.Value, CreateScriptObject),
                TYPES.HTMLELEMENT => GetOrCreateScriptObject(result.Value, CreateHtmlElement),
                TYPES.HTMLCOLLECTION => GetOrCreateScriptObject(result.Value, CreateScriptObjectCollection),
                TYPES.HTMLWINDOW => HtmlPage.Window,
                TYPES.HTMLDOCUMENT => HtmlPage.Document,
                _ => throw new InvalidOperationException(),
            };

            static ScriptObject CreateScriptObject(string id) => new(new JSObjectRef(id));
            static HtmlElement CreateHtmlElement(string id) => new(new JSObjectRef(id));
            static ScriptObjectCollection CreateScriptObjectCollection(string id) => new(new JSObjectRef(id));
        }

        private static Exception UnwrapException(JSResult result)
        {
            return result.Type switch
            {
                TYPES.ERROR => new InvalidOperationException(result.Value),
                _ => null,
            };
        }

        private static ScriptObject GetOrCreateScriptObject(string id, Func<string, ScriptObject> creationFunc)
        {
            lock (_objects)
            {
                if (!_objects.TryGetValue(id, out WeakReference<ScriptObject> wr) || !wr.TryGetTarget(out ScriptObject o))
                {
                    o = creationFunc(id);
                    _objects[id] = new WeakReference<ScriptObject>(o);
                }

                return o;
            }
        }

        private static readonly Dictionary<string, WeakReference<ScriptObject>> _objects = new();

        private struct JSResult
        {
            public TYPES Type { get; set; }
            public string Value { get; set; }
        }

        private enum TYPES
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

    internal interface IJSObjectRef : IJavaScriptConvertible, IDisposable { }

    internal sealed class WindowRef : IJSObjectRef
    {
        private const string Window = "window";

        public WindowRef() { }

        public string ToJavaScriptString() => Window;

        public void Dispose() { }
    }

    internal sealed class DocumentRef : IJSObjectRef
    {
        private const string Document = "document";

        public DocumentRef() { }

        public string ToJavaScriptString() => Document;

        public void Dispose() { }
    }

    internal sealed class JSObjectRef : IJSObjectRef
    {
        private readonly string _jsRef;

        public JSObjectRef(string jsRef)
        {
            _jsRef = jsRef;
        }

        public string ToJavaScriptString() => $"document.browserService.getObject('{_jsRef}')";

        public void Dispose()
        {
            OpenSilver.Interop.ExecuteJavaScriptVoid($"document.browserService.releaseObject('{_jsRef}');");
        }
    }
}
