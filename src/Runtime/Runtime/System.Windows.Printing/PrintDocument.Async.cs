
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

using System.Threading.Tasks;

namespace System.Windows.Printing;

public interface IPrintDocumentAsync
{
    Task PrintAsync(string documentName);

    event Func<object, BeginPrintEventArgs, Task> BeginPrintAsync;

    event Func<object, PrintPageEventArgs, Task> PrintPageAsync;

    event Func<object, EndPrintEventArgs, Task> EndPrintAsync;
}

public partial class PrintDocument : IPrintDocumentAsync
{
    private Func<object, BeginPrintEventArgs, Task> _beginPrintAsync;
    private Func<object, PrintPageEventArgs, Task> _printPageAsync;
    private Func<object, EndPrintEventArgs, Task> _endPrintAsync;

    Task IPrintDocumentAsync.PrintAsync(string documentName)
    {
        InitializePrintDocumentNative();
        EndPendingOperation();

        _operation = new PrintOperation(this);
        return _operation.PrintAsync(documentName);
    }

    event Func<object, BeginPrintEventArgs, Task> IPrintDocumentAsync.BeginPrintAsync
    {
        add => _beginPrintAsync += value;
        remove => _beginPrintAsync -= value;
    }

    event Func<object, PrintPageEventArgs, Task> IPrintDocumentAsync.PrintPageAsync
    {
        add => _printPageAsync += value;
        remove => _printPageAsync -= value;
    }

    event Func<object, EndPrintEventArgs, Task> IPrintDocumentAsync.EndPrintAsync
    {
        add => _endPrintAsync += value;
        remove => _endPrintAsync -= value;
    }
}
