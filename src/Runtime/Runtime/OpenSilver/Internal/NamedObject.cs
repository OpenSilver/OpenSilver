using System;
using System.Globalization;

namespace OpenSilver.Internal;

/// <summary>
/// An instance of this class can be used wherever you might otherwise use
/// "new Object()".  The name will show up in the debugger, instead of
/// merely "{object}"
/// </summary>
internal sealed class NamedObject
{
    private string _name;

    public NamedObject(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }
        
        _name = name;
    }

    public override string ToString()
    {
        if (_name[0] != '{')
        {
            // lazily add {} around the name, to avoid allocating a string
            // until it's actually needed
            _name = string.Format(CultureInfo.InvariantCulture, "{{{0}}}", _name);
        }

        return _name;
    }
}
