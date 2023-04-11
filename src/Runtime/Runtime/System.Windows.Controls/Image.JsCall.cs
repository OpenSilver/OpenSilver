using OpenSilver;

namespace System.Windows.Controls;

public sealed partial class Image
{
    private enum JsCall
    {
        [JsCall(@"void;
{sImage}.addEventListener('load', function (e) {{ {sLoadCallback}(); }});
{sImage}.addEventListener('error', function (e) {{ {sErrorCallback}(); }});
")]
        AttachToDomEvents,
    }
}