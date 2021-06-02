using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Represents a weak reference, which references an object while still allowing
    /// that object to be reclaimed by garbage collection.
    /// </summary>
    public class WeakReference
    {
        // todo: the TrackResurrection part is ignored as of right now. (we should probably remove anything related to it until it is implemented ?)

        object _jsWeakRef = null;

        // Exceptions:
        //   System.NotImplementedException:
        //     This constructor is not implemented.
        /// <summary>
        /// Initializes a new instance of the <see cref="System.WeakReference"/> class. This constructor
        /// overload cannot be implemented in Silverlight-based applications.
        /// </summary>
        protected WeakReference() { }
     
        /// <summary>
        /// Initializes a new instance of the <see cref="System.WeakReference"/> class, referencing
        /// the specified object.
        /// </summary>
        /// <param name="target">The object to track or null.</param>
        public WeakReference(object target)
        {
            Target = target;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="System.WeakReference"/> class, referencing
        /// the specified object and using the specified resurrection tracking.
        /// </summary>
        /// <param name="target">An object to track.</param>
        /// <param name="trackResurrection">
        /// Indicates when to stop tracking the object. If true, the object is tracked
        /// after finalization; if false, the object is only tracked until finalization.
        /// </param>
        public WeakReference(object target, bool trackResurrection)
        {
            Target = target;
            TrackResurrection = trackResurrection;
        }

        /// <summary>
        ///  Gets an indication whether the object referenced by the current <see cref="System.WeakReference"/>
        ///  object has been garbage collected.
        /// </summary>
        public virtual bool IsAlive
        {
            get
            {
                bool ret = false;
                if (_jsWeakRef != null)
                {
                    ret = Bridge.Script.Write<bool>("({0}.deref() != null && {0}.deref() != undefined)", _jsWeakRef);
                }
                return ret;
            }
        }

        // Exceptions:
        //   System.InvalidOperationException:
        //     The reference to the target object is invalid. This exception can be thrown
        //     while setting this property if the value is a null reference or if the object
        //     has been finalized during the set operation.
        /// <summary>
        /// Gets or sets the object (the target) referenced by the current <see cref="System.WeakReference"/>
        /// object.
        /// </summary>
        public virtual object Target
        {
            get
            {
                object ret = null;
                if (_jsWeakRef != null)
                {
                    var jsWeakRef = _jsWeakRef;
                    ret = Bridge.Script.Write<dynamic>(@"(function () {
    var returnValue = {0}.deref();
    if(!returnValue) {
        returnValue = null;
    }
    return returnValue;
})()", jsWeakRef);
                }
                return ret;
            }
            set
            {
                if (value != null)
                {
                    _jsWeakRef = Bridge.Script.Write<dynamic>("new WeakRef({0})", value);
                }
                else
                {
                    _jsWeakRef = null;
                }
            }
        }

        /// <summary>
        /// Gets an indication whether the object referenced by the current <see cref="System.WeakReference"/>
        /// object is tracked after it is finalized.
        /// </summary>
        public virtual bool TrackResurrection { get; }
    }
}