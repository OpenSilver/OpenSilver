
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

#if MIGRATION
using System.Windows.Threading;
#else
using Windows.UI.Core;
#endif

namespace System.Windows.Browser
{
    /// <summary>
    /// Defines the core behavior for the <see cref="HtmlObject"/> class, and
    /// provides a base class for browser Document Object Model (DOM) access types.
    /// </summary>
    public partial class ScriptObject : IDynamicMetaObjectProvider
    {
#if false
        /// <summary>
        /// Frees resources and performs other cleanup operations before the scriptable object
        /// is reclaimed by garbage collection.
        /// </summary>
        ~ScriptObject()
        {

        }
#endif

        /// <summary>
        /// Gets an instance of the dispatcher.
        /// </summary>
        /// <returns>
        /// The dispatcher associated with the user interface (UI) thread.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [OpenSilver.NotImplemented]
#if MIGRATION
        public Dispatcher Dispatcher { get; }
#else
        public CoreDispatcher Dispatcher { get; }
#endif

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
        [OpenSilver.NotImplemented]
        public bool CheckAccess()
        {
            return false;
        }

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
        /// name is an empty string.-or-name contains an embedded null character (\0).
        /// </exception>
        public virtual object GetProperty(string name)
        {
            var result = default(object);

            if (name is null)
            {
                throw new ArgumentNullException(name);
            }
            else if (string.IsNullOrEmpty(name) || name.Contains("\\0"))
            {
                throw new ArgumentException($"{nameof(name)} is an empty string or contains an embedded null character (\0)");
            }

            var jsObject = CSHTML5.Interop.ExecuteJavaScript(name);

            if (!jsObject.ToString().Equals("undefined"))
            {
                result = jsObject;
            }

            // TODO: Check if the underlying ScriptObject is a managed type

            return result;
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
        /// name is an empty string.-or-name contains an embedded null character (\0).-or-The
        /// method does not exist or is not scriptable.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The underlying method invocation results in an error. The .NET Framework attempts
        /// to return the error text that is associated with the error.
        /// </exception>
		[OpenSilver.NotImplemented]
        public virtual object Invoke(string name, params object[] args)
        {
            return null;
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
        /// <exception cref="ArgumentNullException">
        /// name is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// name is an empty string.-or-name contains an embedded null character (\0).-or-The
        /// method does not exist or is not scriptable.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="ScriptObject"/> is not a method.-or-The underlying
        /// method invocation results in an error.
        /// </exception>
		[OpenSilver.NotImplemented]
        public virtual object InvokeSelf(params object[] args)
        {
            return null;
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
		[OpenSilver.NotImplemented]
        public void SetProperty(int index, object value)
        {
        }
        
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
        /// name is an empty string.-or-name contains an embedded null character (\0).
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// A type mismatch exists between the supplied type and the target property.-or-The
        /// property is not settable.-or-All other failures.
        /// </exception>
		[OpenSilver.NotImplemented]
        public virtual void SetProperty(string name, object value)
        {
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
    }
}
