﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace CSHTML5.Internal
{
    internal interface IResizeObserverAdapter
    {
        bool IsObserved { get; }

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
        /// Factory method which instantiates a <see cref="IResizeObserverAdapter"/>.
        /// </summary>
        /// <returns></returns>
        public static IResizeObserverAdapter Create()
        {
            if (Application.Current.Host.Settings.UseResizeSensor || OpenSilver.Interop.IsRunningInTheSimulator_WorkAround || IsRunningOnInternetExplorer())
            {
                return new ResizeSensorAdapter();
            }
            else
            {
                return new ResizeObserverAdapter();
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
        /// An implementation of the <see cref="IResizeObserverAdapter"/> using the ResizeSensor js library.
        /// </summary>
        private class ResizeSensorAdapter : IResizeObserverAdapter
        {
            private const string ADD_SENSOR_JS_TEMPLATE = "new ResizeSensor($0, $1)";
            private const string REMOVE_SENSOR_JS_TEMPLATE = "$0.detach($1)";

            private bool _isObserved;
            private object _resizeSensor;

            /// <summary>
            /// Initializes a new instance of the <see cref="ResizeSensorAdapter"/>.
            /// </summary>
            public ResizeSensorAdapter()
            {
            }

            public bool IsObserved => _isObserved;

            /// <inheritdoc />
            public void Observe(object elementReference, Action<Size> callback)
            {
                if (!_isObserved)
                {
                    _isObserved = true;

                    _resizeSensor = OpenSilver.Interop.ExecuteJavaScript(
                        ADD_SENSOR_JS_TEMPLATE, 
                        elementReference, 
                        new Action<string>((string arg) => callback(ParseSize(arg)))
                    );
                }
            }

            /// <inheritdoc />
            public void Unobserve(object elementReference)
            {
                if (_isObserved)
                {
                    _isObserved = false;
                    object sensor = _resizeSensor;
                    _resizeSensor = null;

                    OpenSilver.Interop.ExecuteJavaScript(
                        REMOVE_SENSOR_JS_TEMPLATE,
                        sensor,
                        elementReference
                    );
                }
            }
        }

        /// <summary>
        /// An implementation of the <see cref="IResizeObserverAdapter"/> using the standard ResizeObserver API.
        /// </summary>
        private class ResizeObserverAdapter : IResizeObserverAdapter
        {
            private const string CREATE_OBSERVER_JS = "new ResizeObserverAdapter()";
            private const string ADD_OBSERVER_JS_TEMPLATE = "$0.observe($1, $2)";
            private const string REMOVE_OBSERVER_JS_TEMPLATE = "$0.unobserve($1)";

            // Holds the reference to the observer js object.
            private static object _observerJsReference;

            private bool _isObserved;

            /// <summary>
            /// Initializes a new instance of the <see cref="ResizeObserverAdapter"/>.
            /// </summary>
            public ResizeObserverAdapter()
            {
            }

            public bool IsObserved => _isObserved;

            /// <inheritdoc />
            public void Observe(object elementReference, Action<Size> callback)
            {
                EnsureResizeObserverInitialized();

                if (!_isObserved)
                {
                    _isObserved = true;

                    OpenSilver.Interop.ExecuteJavaScript(
                        ADD_OBSERVER_JS_TEMPLATE,
                        _observerJsReference,
                        elementReference,
                        new Action<string>((string arg) => callback(ParseSize(arg)))
                    );
                }
            }

            /// <inheritdoc />
            public void Unobserve(object elementReference)
            {
                EnsureResizeObserverInitialized();

                if (_isObserved)
                {
                    _isObserved = false;

                    OpenSilver.Interop.ExecuteJavaScript(
                        REMOVE_OBSERVER_JS_TEMPLATE,
                        _observerJsReference,
                        elementReference
                    );
                }
            }

            private static void EnsureResizeObserverInitialized()
            {
                if (_observerJsReference == null)
                {
                    _observerJsReference = OpenSilver.Interop.ExecuteJavaScript(CREATE_OBSERVER_JS);
                }
            }
        }
    }
}
