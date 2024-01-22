
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

using System.Security;

namespace System.Windows.Printing
{
    /// <summary>
    /// Provides printing capabilities for a Silverlight application.
    /// </summary>
    /// <remarks>
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
    /// </remarks>
    public partial class PrintDocument : DependencyObject
    {
        private static object _printDocumentNative;

        static PrintDocument()
        {
            try
            {
                InitializePrintDocumentNative();
            }
            catch
            {
                // If this static constructor is called before the JSInterop is set up, then
                // this will crash. Handle it and delay this object's construction to later.
            }
        }        

        private PrintOperation _operation;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintDocument"/> class.
        /// </summary>
        public PrintDocument()
        {
        }

        /// <summary>
        /// Gets the identifier for the <see cref="PrintedPageCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PrintedPageCountProperty =
            DependencyProperty.Register(
                nameof(PrintedPageCount),
                typeof(int),
                typeof(PrintDocument),
                new PropertyMetadata(0));

        /// <summary>
        /// Gets the number of pages that have printed.
        /// </summary>
        /// <returns>
        /// The number of pages that have printed.
        /// </returns>
        public int PrintedPageCount
        {
            get => (int)GetValue(PrintedPageCountProperty);
            private set => SetValueInternal(PrintedPageCountProperty, value);
        }

        /// <summary>
        /// Occurs after the <see cref="Print(string)"/> method is called and the print 
        /// dialog box successfully returns, but before the <see cref="PrintPage"/>
        /// event is raised.
        /// </summary>
        public event EventHandler<BeginPrintEventArgs> BeginPrint;

        /// <summary>
        /// Occurs when the printing operation is passed to the print spooler or when the
        /// print operation is cancelled by the application author.
        /// </summary>
        public event EventHandler<EndPrintEventArgs> EndPrint;

        /// <summary>
        /// Occurs when each page is printing.
        /// </summary>
        public event EventHandler<PrintPageEventArgs> PrintPage;

        /// <summary>
        /// Starts the printing process for the specified document by opening the print dialog box.
        /// </summary>
        /// <param name="documentName">
        /// The name of the document to print.
        /// </param>
        /// <exception cref="SecurityException">
        /// The print operation is not user-initiated.
        /// </exception>
        public void Print(string documentName)
        {
            InitializePrintDocumentNative();
            EndPendingOperation();

            _operation = new PrintOperation(this);
            _operation.Print(documentName);
        }

        /// <summary>
        /// Starts the vector printing process for the specified document by optionally opening
        /// the print dialog box or printing directly to the default printer for trusted
        /// applications.
        /// </summary>
        /// <param name="documentName">
        /// The name of the document to print.
        /// </param>
        /// <param name="printerFallbackSettings">
        /// The settings to use to enable vector printing for printers with limited support.
        /// </param>
        /// <param name="useDefaultPrinter">
        /// Whether or not to automatically print to the default printer for the computer
        /// without showing a print dialog. This parameter can only be true in trusted applications,
        /// otherwise an exception will occur.
        /// </param>
        /// <exception cref="SecurityException">
        /// The print operation is not user-initiated.-or-useDefaultPrinter is set to true
        /// and the application is not a trusted application.
        /// </exception>
        [OpenSilver.NotImplemented]
        public void Print(string documentName, PrinterFallbackSettings printerFallbackSettings, bool useDefaultPrinter = false)
        {
        }

        /// <summary>
        /// Starts the bitmap printing process for the specified document by opening the
        /// print dialog box.
        /// </summary>
        /// <param name="documentName">
        /// The name of the document to print.
        /// </param>
        [OpenSilver.NotImplemented]
        public void PrintBitmap(string documentName)
        {
        }

        private void EndPendingOperation()
        {
            if (_operation is not null)
            {
                _operation.Dispose();
                _operation = null;
                PrintedPageCount = 0;
            }
        }

        private static void InitializePrintDocumentNative()
        {
            _printDocumentNative ??= OpenSilver.Interop.ExecuteJavaScript(
@"(function () {
  function addStyle() {
    var element = document.createElement('style')
    element.innerHTML += `
#print-container {
  display: none;
}
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
}`;
    document.head.prepend(element);
  };

  function prepareDocument() {
    addStyle();
    var elements = document.getElementsByClassName('print-section');
    let el = document.createElement('div');
    el.id = 'print-container';      

    for (var i = 0; i < elements.length; i++) {
      el.innerHTML = el.innerHTML + elements[i].outerHTML;
    }

    document.body.prepend(el);

    var new_elements = el.getElementsByClassName('print-section');
    for (var i = 0; i < new_elements.length; i++) {
      new_elements[i].classList.add('print-document-section-to-print');
    }
  };

  return {
  	print: function (title, callback) {
      const t = document.title;
      window.addEventListener('beforeprint', function (e) {
        document.title = title;
  	    prepareDocument();
  	  }, { once: true });
  	  var mql = window.matchMedia('print');
  	  const endprint = function (event) {
        if (!event.matches) {
  		  callback();
  		  document.getElementsByTagName('style')[0].remove();
  		  document.getElementById('print-container').remove();
  		  document.title = t;
  		  mql.removeEventListener('change', endprint);
        }
      };
  	  mql.addEventListener('change', endprint);
  	  window.print();
  	}
  };
})();");
        }
    }
}
