
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
using System.Globalization;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
#endif

namespace OpenSilver.Internal
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
            if (Application.Current.Host.Settings.UseResizeSensor || Interop.IsRunningInTheSimulator)
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
        private static Size ParseSize(string argSize)
        {
            int sepIndex = argSize != null ? argSize.IndexOf('|') : -1;

            if (sepIndex == -1)
            {
                return Size.Empty;
            }

            string actualWidthAsString = argSize.Substring(0, sepIndex);
            string actualHeightAsString = argSize.Substring(sepIndex + 1);
            double actualWidth = Math.Floor(double.Parse(actualWidthAsString, CultureInfo.InvariantCulture));
            double actualHeight = Math.Floor(double.Parse(actualHeightAsString, CultureInfo.InvariantCulture));
            return new Size(actualWidth, actualHeight);
        }

        /// <summary>
        /// An implementation of the <see cref="IResizeObserverAdapter"/> using the ResizeSensor js library.
        /// </summary>
        private class ResizeSensorAdapter : IResizeObserverAdapter
        {
            private bool _isObserved;
            private object _resizeSensor;
            private JavaScriptCallback _sizeChangedCallback;

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
                    _sizeChangedCallback = JavaScriptCallback.Create((string arg) => callback(ParseSize(arg)), true);

                    string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(elementReference);
                    string sAction = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_sizeChangedCallback);

                    _resizeSensor = Interop.ExecuteJavaScript($"new ResizeSensor({sElement}, {sAction})");
                }
            }

            /// <inheritdoc />
            public void Unobserve(object elementReference)
            {
                if (_isObserved)
                {
                    _isObserved = false;

                    _sizeChangedCallback.Dispose();
                    _sizeChangedCallback = null;

                    string sSensor = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_resizeSensor);
                    _resizeSensor = null;

                    string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(elementReference);
                    Interop.ExecuteJavaScriptVoid($"{sSensor}.detach({sElement})");
                }
            }
        }

        /// <summary>
        /// An implementation of the <see cref="IResizeObserverAdapter"/> using the standard ResizeObserver API.
        /// </summary>
        private class ResizeObserverAdapter : IResizeObserverAdapter
        {
            // Holds the reference to the observer js object.
            private static object _observerJsReference;

            private bool _isObserved;
            private JavaScriptCallback _sizeChangedCallback;

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
                    _sizeChangedCallback = JavaScriptCallback.Create((string arg) => callback(ParseSize(arg)), true);

                    string sReference = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_observerJsReference);
                    string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(elementReference);
                    string sAction = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_sizeChangedCallback);

                    Interop.ExecuteJavaScriptVoid($"{sReference}.observe({sElement}, {sAction})");
                }
            }

            /// <inheritdoc />
            public void Unobserve(object elementReference)
            {
                EnsureResizeObserverInitialized();

                if (_isObserved)
                {
                    _isObserved = false;
                    _sizeChangedCallback.Dispose();
                    _sizeChangedCallback = null;

                    string sReference = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_observerJsReference);
                    string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(elementReference);
                    Interop.ExecuteJavaScriptVoid($"{sReference}.unobserve({sElement})");
                }
            }

            private static void EnsureResizeObserverInitialized()
                => _observerJsReference ??= Interop.ExecuteJavaScript("new ResizeObserverAdapter()");
        }
    }
}
