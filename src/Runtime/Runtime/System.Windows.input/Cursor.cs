
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

using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using OpenSilver.Internal;

namespace System.Windows.Input;

/// <summary>
/// Represents the image used for the mouse pointer.
/// </summary>
[TypeConverter(typeof(CursorConverter))]
public sealed class Cursor : IDisposable
{
    private static string[] HtmlCursors { get; }

    static Cursor()
    {
        HtmlCursors = new string[Cursors._cursorTypeCount];
        HtmlCursors[(int)CursorType.None] = "none";
        HtmlCursors[(int)CursorType.No] = "not-allowed";
        HtmlCursors[(int)CursorType.Arrow] = "default";
        HtmlCursors[(int)CursorType.AppStarting] = "progress";
        HtmlCursors[(int)CursorType.Cross] = "crosshair";
        HtmlCursors[(int)CursorType.Help] = "help";
        HtmlCursors[(int)CursorType.IBeam] = "text";
        HtmlCursors[(int)CursorType.SizeAll] = "move";
        HtmlCursors[(int)CursorType.SizeNESW] = "nesw-resize";
        HtmlCursors[(int)CursorType.SizeNS] = "ns-resize";
        HtmlCursors[(int)CursorType.SizeNWSE] = "nwse-resize";
        HtmlCursors[(int)CursorType.SizeWE] = "ew-resize";
        HtmlCursors[(int)CursorType.UpArrow] = "auto"; // not implemented
        HtmlCursors[(int)CursorType.Wait] = "wait";
        HtmlCursors[(int)CursorType.Hand] = "pointer";
        HtmlCursors[(int)CursorType.Pen] = "auto"; // not implemented
        HtmlCursors[(int)CursorType.ScrollNS] = "auto"; // not implemented
        HtmlCursors[(int)CursorType.ScrollWE] = "auto"; // not implemented
        HtmlCursors[(int)CursorType.ScrollAll] = "all-scroll";
        HtmlCursors[(int)CursorType.ScrollN] = "auto"; // not implemented
        HtmlCursors[(int)CursorType.ScrollS] = "auto"; // not implemented
        HtmlCursors[(int)CursorType.ScrollW] = "auto"; // not implemented
        HtmlCursors[(int)CursorType.ScrollE] = "auto"; // not implemented
        HtmlCursors[(int)CursorType.ScrollNW] = "auto"; // not implemented
        HtmlCursors[(int)CursorType.ScrollNE] = "auto"; // not implemented
        HtmlCursors[(int)CursorType.ScrollSW] = "auto"; // not implemented
        HtmlCursors[(int)CursorType.ScrollSE] = "auto"; // not implemented
        HtmlCursors[(int)CursorType.ArrowCD] = "auto"; // not implemented
        HtmlCursors[(int)CursorType.Stylus] = "auto"; // not implemented
        HtmlCursors[(int)CursorType.Eraser] = "auto"; // not implemented
    }

    private readonly CursorType _cursorType = CursorType.None;
    private readonly CursorHandle _cursorHandle;

    /// <summary>
    /// Constructor for Standard Cursors, needn't be public as Stock Cursors
    /// are exposed in Cursors class.
    /// </summary>
    internal Cursor(CursorType cursorType)
    {
        GC.SuppressFinalize(this);

        if (IsValidCursorType(cursorType))
        {
            _cursorType = cursorType;
        }
        else
        {
            throw new ArgumentException(string.Format(Strings.InvalidCursorType, cursorType));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Cursor"/> class from the specified <see cref="Stream"/>.
    /// </summary>
    /// <param name="cursorStream">
    /// The <see cref="Stream"/> that contains the cursor.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="cursorStream"/> is null.
    /// </exception>
    public Cursor(Stream cursorStream)
    {
        ArgumentNullException.ThrowIfNull(cursorStream);

        _cursorHandle = CursorHandle.Create(cursorStream);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Cursor"/> class.
    /// </summary>
    /// <param name="cursorStream">
    /// The <see cref="Stream"/> that contains the cursor.
    /// </param>
    /// <param name="scaleWithDpi">
    /// true if to scale with dpi; otherwise, false.
    /// </param>
    [OpenSilver.NotImplemented]
    public Cursor(Stream cursorStream, bool scaleWithDpi)
        : this(cursorStream)
    {
    }

    ~Cursor() => Dispose(false);

    /// <summary>
    /// CursorType - Cursor Type Enumeration
    /// </summary>
    /// <value></value>
    internal CursorType CursorType => _cursorType;

    private static bool IsValidCursorType(CursorType cursorType)
    {
        return (int)cursorType >= (int)CursorType.None && (int)cursorType <= (int)CursorType.Eraser;
    }

    /// <summary>
    /// Releases the resources used by the <see cref="Cursor"/> class.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing) => _cursorHandle.ReleaseResource();

    /// <summary>
    /// Returns the string representation of the <see cref="Cursor"/>.
    /// </summary>
    /// <returns>
    /// The string representation of the cursor. This corresponds to the active <see cref="Cursors"/>
    /// property name.
    /// </returns>
    public override string ToString()
    {
        // Get the string representation fo the cursor type enumeration.
        return Enum.GetName(typeof(CursorType), _cursorType);
    }

    internal string ToHtmlString()
    {
        if (_cursorHandle.IsValid)
        {
            return $"url({_cursorHandle.Url}), auto";
        }

        return HtmlCursors[(int)_cursorType];
    }

    private struct CursorHandle
    {
        private string _url;

        public static CursorHandle Create(Stream stream)
        {
            Debug.Assert(stream is not null);

            string url = OpenSilver.Interop.ExecuteJavaScriptString($"osjs.cursors.create('{ReadFile(stream)}')");
            return new CursorHandle(url);
        }

        private CursorHandle(string url)
        {
            _url = url;
        }

        public readonly string Url => _url;

        public readonly bool IsValid => !string.IsNullOrEmpty(_url);

        public void ReleaseResource()
        {
            if (IsValid)
            {
                (string url, _url) = (_url, null);
                OpenSilver.Interop.RevokeObjectURLAsync(url);
            }
        }

        private static string ReadFile(Stream stream)
        {
            Debug.Assert(stream is not null);

            using MemoryStream memoryStream = new();
            stream.CopyTo(memoryStream);
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }
    }
}