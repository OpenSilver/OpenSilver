using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using System.Windows;
#endif

#if MIGRATION
namespace System.Windows.Printing
#else
namespace Windows.UI.Xaml.Printing
#endif
{
    /// <summary>
    /// 1. All elements are collected using the PrintPage event and then 'print-section' class is added to all of them.
    /// 2. A separate 'print-section' div is created inside 'beforeprint' event and all 'print-section' elements
    ///    added to it with different class name to be printed later - 'print-document-section-to-print'
    /// 3. 'print-section' is added as the first child of the body element.
    /// 4. @media print is used to show 'print-document-section-to-print' elements and hide the rest.
    /// 
    /// The reason it recreates and adds the elements to 'body' is to avoid wrong positioning depending on other
    /// relative elements. One other solution would be to use 'fixed' position however it breaks paging (break-after).
    /// 'beforeprint' event prepares everything needed for printing and 'afterprint' cleans up things.
    /// 
    /// 'beforeprint' and 'afterprint' events registered only once and @media print rule is added/removed by
    /// JavaScript which guarantees no side effects after print.
    /// </summary>
    public partial class PrintDocument : DependencyObject
    {
        //
        // Summary:
        //     Gets the identifier for the System.Windows.Printing.PrintDocument.PrintedPageCount
        //     dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.Printing.PrintDocument.PrintedPageCount
        //     dependency property.
        public static readonly DependencyProperty PrintedPageCountProperty = DependencyProperty.Register(
            nameof(PrintedPageCount),
            typeof(int),
            typeof(PrintDocument),
            new PropertyMetadata(0));

        private List<UIElement> elements;
        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Printing.PrintDocument class.
        public PrintDocument()
        {
            elements = new List<UIElement>();
        }

        //
        // Summary:
        //     Gets the number of pages that have printed.
        //
        // Returns:
        //     The number of pages that have printed.
        public int PrintedPageCount { get; }

        //
        // Summary:
        //     Occurs after the System.Windows.Printing.PrintDocument.Print(System.String) method
        //     is called and the print dialog box successfully returns, but before the System.Windows.Printing.PrintDocument.PrintPage
        //     event is raised.
        public event EventHandler<BeginPrintEventArgs> BeginPrint;

        //
        // Summary:
        //     Occurs when the printing operation is passed to the print spooler or when the
        //     print operation is cancelled by the application author.
        public event EventHandler<EndPrintEventArgs> EndPrint;

        //
        // Summary:
        //     Occurs when each page is printing.
        public event EventHandler<PrintPageEventArgs> PrintPage;

        //
        // Summary:
        //     Starts the printing process for the specified document by opening the print dialog
        //     box.
        //
        // Parameters:
        //   documentName:
        //     The name of the document to print.
        //
        // Exceptions:
        //   T:System.Security.SecurityException:
        //     The print operation is not user-initiated.
        public void Print(string documentName)
        {
            elements.Clear();
            if (PrintPage != null)
            {
                // In Silverlight PrintPage event is used to get all elements that need to be printed.
                GetElements();
            }
            else
            {
                // If PrintPage was not registered before, there is a chance that it can be registered in BeginPrint
                if (BeginPrint != null)
                {
                    BeginPrint(this, new BeginPrintEventArgs());

                    if (PrintPage != null)
                    {
                        GetElements();
                    }
                }

                // If no element is selected to print then just call EndPrint
                if (elements.Count == 0 && EndPrint != null)
                {
                    EndPrint(this, new EndPrintEventArgs());
                }
            }

            if (elements.Count > 0)
            {
                AddEventListeners(documentName);
                LoadNotLoadedElements(() =>
                {
                    PrintPrivate();
                });
            }
        }

        private void GetElements()
        {
            PrintPageEventArgs e = new PrintPageEventArgs();

            int i = 0;
            // In Silverlight it will stop calling PrintPage after Print if HasMorePages is false AND PageVisual is null
            while (true)
            {
                i++;
                e.HasMorePages = false;
                PrintPage(this, e);

                if (e.PageVisual != null)
                {
                    elements.Add(e.PageVisual);

                    if (!e.HasMorePages)
                        break;
                }

                // Avoid infinite loop
                if (i > 1000)
                    break;
            }
        }

        private void PrintPrivate()
        {
            AddPrintSection();
            OpenSilver.Interop.ExecuteJavaScript("window.print()");
            RemovePrintSection(elements);
        }

        private void LoadNotLoadedElements(Action callback)
        {
            List<UIElement> NotLoadedElements = new List<UIElement>();
            foreach (var e in elements)
            {
                if (!e._isLoaded)
                {
                    NotLoadedElements.Add(e);
                }
            }

            if (NotLoadedElements.Count == 0)
            {
                callback();
                return;
            }

            var temporaryPopup = new Popup() { VerticalOffset = 10000 };
            temporaryPopup.IsOpen = true;

            var stackPanel = new StackPanel();
            foreach (var e in NotLoadedElements)
            {
                stackPanel.Children.Add(e);
            }

            stackPanel.Loaded += (s, e) =>
            {
#if MIGRATION
                Dispatcher
#else
                CoreDispatcher
#endif
                .INTERNAL_GetCurrentDispatcher().BeginInvoke(() =>
                {
                    foreach (var el in NotLoadedElements)
                    {
                        OpenSilver.Interop.ExecuteJavaScript(@"$0.classList.add(""print-section"")", el.INTERNAL_OuterDomElement);
                    }

                    // Even though the Loaded event is fired, sometimes we need to wait little bit more.
                    OpenSilver.Interop.ExecuteJavaScriptAsync("setTimeout($0, 100)",
                        (Action)(() =>
                        {
                            callback();
                            temporaryPopup.IsOpen = false;
                        }));
                });
            };

            // Put the container into the popup, and open the popup:
            temporaryPopup.Child = stackPanel;
            temporaryPopup.IsOpen = true;
        }

        private void AddPrintSection()
        {
            // Add 'print-section' class for elements we want to print
            foreach (var e in elements)
            {
                OpenSilver.Interop.ExecuteJavaScript(@"$0.classList.add(""print-section"")", e.INTERNAL_OuterDomElement);
            }
        }

        private void RemovePrintSection(List<UIElement> elements)
        {
            // Remove 'print-section' class for elements we want to print
            foreach (var e in elements)
            {
                OpenSilver.Interop.ExecuteJavaScript(@"$0.classList.remove(""print-section"")", e.INTERNAL_OuterDomElement);
            }
            elements.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentName"></param>
        private void AddEventListeners(string documentName)
        {
            Action beginCallback = () =>
            {
                BeginPrint?.Invoke(this, new BeginPrintEventArgs());
            };

            Action endCallback = () =>
            {
                EndPrint?.Invoke(this, new EndPrintEventArgs());
            };

            // Having all in one call makes it easier to copy and test it in separate HTML project.
            _ = OpenSilver.Interop.ExecuteJavaScript(@"
// Backup title to restore later
var title = document.title;

window.addEventListener('beforeprint', (event) => {
    document.title = $2;
    var beginCallback = $0; beginCallback();
    addStyle();
    var elements = document.getElementsByClassName('print-section');
    let el = document.createElement('div');
    el.id = 'print-container';
    el.style.display = 'none';

    for (var i = 0; i < elements.length; i++) {
        el.innerHTML = el.innerHTML + elements[i].outerHTML;
    }
    
    document.body.prepend(el);
    
    var new_elements = el.getElementsByClassName('print-section');
    for (var i = 0; i < new_elements.length; i++) {
        new_elements[i].classList.add('print-document-section-to-print');
    }
}, { once: true });

window.addEventListener('afterprint', (event) => {
    var endCallback = $1; endCallback();
    document.getElementsByTagName('style')[0].remove();
    document.getElementById('print-container').remove();
    document.title = title;
}, { once: true });

function addStyle() {
    var element = document.createElement('style')
    element.innerHTML += `
@media print {
    /* Added new class for PrintDocument */
    .print-document-section-to-print, .print-document-section-to-print * {
        visibility: visible;
    }

    .print-document-section-to-print {
        left: 0 !important;
        top: 0 !important;
        margin: 0 !important;
        border: 0 !important;
        padding: 0 !important;
        height: 100%;
        width: 100%;
    }

    /* Page break for all elements except the last one */
    .print-document-section-to-print:not(:last-child) {
        break-after: page !important;
    }

    #print-container {
        display: inline !important;
    }
    
    body>*:not(#print-container) {
        display: none !important;
    }
}
`;
    document.head.prepend(element);
}
", beginCallback, endCallback, documentName);
        }
    }
}
