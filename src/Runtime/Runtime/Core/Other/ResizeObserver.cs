using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace CSHTML5.Internal
{
    /// <summary>
    /// Abstract definition of a observer which monitors the resizing of elements.
    /// </summary>
    internal interface IResizeObserver
    {
        /// <summary>
        /// Monitors the specified <paramref name="elementReference"/> for resize, and calls the specified action.
        /// </summary>
        /// <param name="elementReference">The element to observe.</param>
        /// <param name="callback">The action to call when resizing occurs.</param>
        void Observe(object elementReference, Action<Size> callback);

        /// <summary>
        /// Remove the specified element from the list of observed elements.
        /// </summary>
        /// <param name="elementReference">The html element reference to unobserve.</param>
        void Unobserve(object elementReference);
    }

    internal static class ResizeObserverFactory
    {
        /// <summary>
        /// Factory method which instantiates a <see cref="IResizeObserver"/>.
        /// </summary>
        /// <returns></returns>
        public static IResizeObserver Create()
        {
            if (Application.Current.Host.Settings.UseResizeSensor || Interop.IsRunningInTheSimulator_WorkAround || IsRunningOnInternetExplorer())
            {
                return new ResizeSensor();
            }
            else
            {
                return ResizeObserver.Instance;
            }
        }

        /// <summary>
        /// Helper method used to parse size string "Height|Width".
        /// </summary>
        /// <param name="argSize">The size string to parse.</param>
        /// <returns>The parsed <see cref="Size"/>, or <see cref="Size.Empty"/> if the parse fails.</returns>
        internal static Size ParseSize(string argSize)
        {
            int sepIndex = argSize != null ? argSize.IndexOf('|') : -1;

            if (sepIndex == -1)
            {
                return Size.Empty;
            }

            string actualWidthAsString = argSize.Substring(0, sepIndex);
            string actualHeightAsString = argSize.Substring(sepIndex + 1);
            double actualWidth = double.Parse(actualWidthAsString, global::System.Globalization.CultureInfo.InvariantCulture);
            double actualHeight = double.Parse(actualHeightAsString, global::System.Globalization.CultureInfo.InvariantCulture);
            return new Size(actualWidth, actualHeight);
        }

#if BRIDGE
        [Bridge.Template("window.IE_VERSION")]
#endif
        private static bool IsRunningOnInternetExplorer()
        {
            return false;
        }

        /// <summary>
        /// An implementation of the <see cref="IResizeObserver"/> using the ResizeSensor js library.
        /// </summary>
        private class ResizeSensor : IResizeObserver
        {
            private const string ADD_SENSOR_JS_TEMPLATE = "new ResizeSensor($0, $1)";
            private const string REMOVE_SENSOR_JS_TEMPLATE = "$0.detach($1)";

            // Contains all the registered ResizeSensors and their corresponding identifiers.
            private readonly IDictionary<string, object> _sensors;

            /// <summary>
            /// Initializes a new instance of the <see cref="ResizeSensor"/>.
            /// </summary>
            public ResizeSensor()
            {
                this._sensors = new Dictionary<string, object>();
            }

            /// <inheritdoc />
            public void Observe(object elementReference, Action<Size> callback)
            {
                var sensor = Interop.ExecuteJavaScript(ADD_SENSOR_JS_TEMPLATE, elementReference, new Action<string>((string arg) => callback(ParseSize(arg))));
                this._sensors.Add(((INTERNAL_HtmlDomElementReference)elementReference).UniqueIdentifier, sensor);
            }

            /// <inheritdoc />
            public void Unobserve(object elementReference)
            {
                Interop.ExecuteJavaScript(REMOVE_SENSOR_JS_TEMPLATE, this._sensors[((INTERNAL_HtmlDomElementReference)elementReference).UniqueIdentifier], elementReference);
                this._sensors.Remove(((INTERNAL_HtmlDomElementReference)elementReference).UniqueIdentifier);
            }
        }

        /// <summary>
        /// An implementation of the <see cref="IResizeObserver"/> using the standard ResizeObserver API.
        /// </summary>
        private class ResizeObserver : IResizeObserver
        {
            private const string CREATE_OBSERVER_JS = "new ResizeObserverAdapter()";
            private const string ADD_OBSERVER_JS_TEMPLATE = "$0.observe($1, $2)";
            private const string REMOVE_OBSERVER_JS_TEMPLATE = "$0.unobserve($1)";

            private static ResizeObserver _instance;
            public static ResizeObserver Instance => _instance ?? (_instance = new ResizeObserver());

            // Holds the reference to the observer js object.
            private readonly object _observerJsReference;

            /// <summary>
            /// Initializes a new instance of the <see cref="ResizeObserver"/>.
            /// </summary>
            private ResizeObserver()
            {
                this._observerJsReference = Interop.ExecuteJavaScript(CREATE_OBSERVER_JS);
            }

            /// <inheritdoc />
            public void Observe(object elementReference, Action<Size> callback)
            {
                Interop.ExecuteJavaScript(ADD_OBSERVER_JS_TEMPLATE, this._observerJsReference, elementReference, new Action<string>((string arg) => callback(ParseSize(arg))));
            }

            /// <inheritdoc />
            public void Unobserve(object elementReference)
            {
                Interop.ExecuteJavaScript(REMOVE_OBSERVER_JS_TEMPLATE, this._observerJsReference, elementReference);
            }
        }
    }
}
