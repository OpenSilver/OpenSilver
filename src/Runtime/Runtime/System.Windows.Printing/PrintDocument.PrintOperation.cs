
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
using System.Collections.Generic;
using System.Diagnostics;
using CSHTML5;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Printing
#else
namespace Windows.UI.Xaml.Printing
#endif
{
    public partial class PrintDocument : DependencyObject
    {
        private sealed class PrintOperation : IDisposable
        {
            private readonly PrintDocument _printDocument;

            private readonly List<UIElement> _elements;
            private Popup _popup;
            private StackPanel _panel;
            private JavaScriptCallback _printJSCallback;
            private JavaScriptCallback _endPrintJSCallback;

            public PrintOperation(PrintDocument printDocument)
            {
                Debug.Assert(printDocument != null);
                _printDocument = printDocument;
                _elements = new List<UIElement>();
            }

            public void Print(string documentName)
            {
                OnBeginPrint(new BeginPrintEventArgs());

                if (_printDocument.PrintPage != null)
                {
                    // In Silverlight PrintPage event is used to get all elements
                    // that need to be printed.
                    // In Silverlight it will stop calling PrintPage after Print
                    // if HasMorePages is false AND PageVisual is null.
                    // We set a limit to 1000 iterations to avoid infinite loops.
                    for (int i = 0; i < 1000; i++)
                    {
                        var e = new PrintPageEventArgs();
                        OnPrintPage(e);
                        if (e.PageVisual != null)
                        {
                            _elements.Add(e.PageVisual);
                            _printDocument.PrintedPageCount = _elements.Count;
                            if (!e.HasMorePages)
                                break;
                        }
                    }
                }

                if (_elements.Count == 0)
                {
                    OnEndPrint(new EndPrintEventArgs());
                    _printDocument.EndPendingOperation();
                }
                else
                {
                    LoadNotLoadedElements(() => PrintNative(documentName));
                }
            }

            public void Dispose()
            {
                _elements.Clear();
                ClearPopup();
                if (_printJSCallback != null)
                {
                    _printJSCallback.Dispose();
                    _printJSCallback = null;
                }
                if (_endPrintJSCallback != null)
                {
                    _endPrintJSCallback.Dispose();
                    _endPrintJSCallback = null;
                }
            }

            private void OnBeginPrint(BeginPrintEventArgs e)
                => _printDocument.BeginPrint?.Invoke(_printDocument, e);

            private void OnPrintPage(PrintPageEventArgs e)
                => _printDocument.PrintPage?.Invoke(_printDocument, e);

            private void OnEndPrint(EndPrintEventArgs e)
                => _printDocument.EndPrint?.Invoke(_printDocument, e);

            private void PrintNative(string documentName)
            {
                AddPrintSection();

                _endPrintJSCallback = JavaScriptCallbackHelper.CreateSelfDisposedJavaScriptCallback(
                    () =>
                    {
                        _endPrintJSCallback = null;
                        OnEndPrint(new EndPrintEventArgs());
                        _printDocument.EndPendingOperation();
                    });
                string sPrint = INTERNAL_InteropImplementation.GetVariableStringForJS(_printDocumentNative);
                string sTitle = INTERNAL_InteropImplementation.GetVariableStringForJS(documentName);
                string sCallback = INTERNAL_InteropImplementation.GetVariableStringForJS(_endPrintJSCallback);
                OpenSilver.Interop.ExecuteJavaScriptVoid($"{sPrint}.print({sTitle}, {sCallback});");

                RemovePrintSection();
            }

            private void AddPrintSection()
            {
                // Add 'print-section' class for elements we want to print
                foreach (UIElement e in _elements)
                {
                    OpenSilver.Interop.ExecuteJavaScriptVoid(
                        $"{INTERNAL_InteropImplementation.GetVariableStringForJS(e.INTERNAL_OuterDomElement)}.classList.add(\"print-section\")");
                }
            }

            private void RemovePrintSection()
            {
                // Remove 'print-section' class for elements we want to print
                foreach (UIElement e in _elements)
                {
                    OpenSilver.Interop.ExecuteJavaScriptVoid(
                        $"{INTERNAL_InteropImplementation.GetVariableStringForJS(e.INTERNAL_OuterDomElement)}.classList.remove(\"print-section\")");
                }
            }

            private void ClearPopup()
            {
                if (_popup != null)
                {
                    _popup.IsOpen = false;
                    _popup.Child = null;
                    _popup = null;
                }
                if (_panel != null)
                {
                    _panel.Children.Clear();
                    _panel = null;
                }
            }

            private void LoadNotLoadedElements(Action callback)
            {
                var unloadedElements = new List<UIElement>();
                foreach (UIElement e in _elements)
                {
                    if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(e))
                    {
                        unloadedElements.Add(e);
                    }
                }

                if (unloadedElements.Count == 0)
                {
                    callback();
                    return;
                }

                _panel = new StackPanel();
                foreach (UIElement e in unloadedElements)
                {
                    _panel.Children.Add(e);
                }

                _panel.Loaded += (s, e) =>
                {
                    _printDocument.Dispatcher.BeginInvoke(() =>
                    {
                        foreach (UIElement el in unloadedElements)
                        {
                            OpenSilver.Interop.ExecuteJavaScriptVoid(
                                $"{INTERNAL_InteropImplementation.GetVariableStringForJS(el.INTERNAL_OuterDomElement)}.classList.add(\"print-section\")");
                        }

                        _printJSCallback = JavaScriptCallbackHelper.CreateSelfDisposedJavaScriptCallback(() =>
                        {
                            callback();
                            ClearPopup();
                            _printJSCallback = null;
                        });

                        string sCallback = INTERNAL_InteropImplementation.GetVariableStringForJS(_printJSCallback);
                        // Even though the Loaded event is fired, sometimes we need to wait little bit more.
                        OpenSilver.Interop.ExecuteJavaScriptFastAsync($"setTimeout({sCallback}, 100)");
                    });
                };

                _popup = new Popup
                {
                    VerticalOffset = 10000,
                    Child = _panel,
                    IsOpen = true,
                };
            }
        }
    }
}
