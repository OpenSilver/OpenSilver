using System;
using System.Collections.Generic;
using System.Text;

namespace CSHTML5.Internal
{
    /// <summary>
    /// Abstract definition of a observer which monitors the resizing of elements.
    /// </summary>
    public interface IResizeObserver
    {
        /// <summary>
        /// Monitors the specified <paramref name="elementReference"/> for resize, and calls the specified action.
        /// </summary>
        /// <param name="elementReference">The element to observe.</param>
        /// <param name="callback">The action to call when resizing occurs.</param>
        void Observe(object elementReference, Action<string, string> callback);

        /// <summary>
        /// Remove the specified element from the list of observed elements.
        /// </summary>
        /// <param name="elementReference">The html element reference to unobserve.</param>
        void Unobserve(object elementReference);
    }

    public static class ResizeObserverFactory
    {
        /// <summary>
        /// Factory method which instantiates a <see cref="IResizeObserver"/>.
        /// </summary>
        /// <returns></returns>
        public static IResizeObserver Create()
        {
            if (Interop.IsRunningInTheSimulator_WorkAround)
            {
                return new ResizeSensor();
            }
            else
            {
                return new ResizeObserver();
            }
        }
    }

    internal class ResizeSensor : IResizeObserver
    {
        private const string ADD_SENSOR_JS_TEMPLATE = "new ResizeSensor($1, $2)";
        private const string REMOVE_SENSOR_JS_TEMPLATE = "$0.detach($1)";

        private readonly IDictionary<HtmlElementReferenceWrapper, object> _sensors;

        public ResizeSensor()
        {
            this._sensors = new Dictionary<HtmlElementReferenceWrapper, object>();
        }

        public void Observe(object elementReference, Action<string, string> callback)
        {
            var sensor = Interop.ExecuteJavaScript(ADD_SENSOR_JS_TEMPLATE, elementReference, callback);
            this._sensors.Add(new HtmlElementReferenceWrapper((INTERNAL_HtmlDomElementReference)elementReference), sensor);
        }

        public void Unobserve(object elementReference)
        {
            Interop.ExecuteJavaScript(REMOVE_SENSOR_JS_TEMPLATE, this._sensors[new HtmlElementReferenceWrapper((INTERNAL_HtmlDomElementReference)elementReference)], elementReference);
            this._sensors.Remove(new HtmlElementReferenceWrapper((INTERNAL_HtmlDomElementReference)elementReference));
        }

        /// <summary>
        /// Used to store a HtmlDomElementReference as a key in the dictionary.
        /// </summary>
        private class HtmlElementReferenceWrapper
        {
            public HtmlElementReferenceWrapper(INTERNAL_HtmlDomElementReference reference)
            {
                this.Reference = reference;
            }

            public INTERNAL_HtmlDomElementReference Reference { get; }

            public override int GetHashCode()
            {
                return Reference.UniqueIdentifier.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is null)
                {
                    return false;
                }

                if (obj is HtmlElementReferenceWrapper wrapper)
                {
                    return wrapper.Reference.Equals(this);
                }

                return false;
            }
        }
    }
    internal class ResizeObserver : IResizeObserver
    {
        private const string CREATE_OBSERVER_JS = "new ResizeObserverAdapter()";
        private const string ADD_OBSERVER_JS_TEMPLATE = "$0.observe($1, $2)";
        private const string REMOVE_OBSERVER_JS_TEMPLATE = "$0.unobserve($1)";

        private readonly object _observerJsReference;

        public ResizeObserver()
        {
            this._observerJsReference = Interop.ExecuteJavaScript(CREATE_OBSERVER_JS);
        }

        public void Observe(object elementReference, Action<string, string> callback)
        {
            Interop.ExecuteJavaScript(ADD_OBSERVER_JS_TEMPLATE, this._observerJsReference, elementReference, callback);
        }

        public void Unobserve(object elementReference)
        {
            Interop.ExecuteJavaScript(REMOVE_OBSERVER_JS_TEMPLATE, this._observerJsReference, elementReference);
        }
    }
}
