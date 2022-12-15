using System;
using System.Globalization;

namespace CSHTML5.Internal
{
    /// <summary>
    /// An instance of this class can be used wherever you might otherwise use
    /// "new Object()".  The name will show up in the debugger, instead of
    /// merely "{object}"
    /// </summary>
    internal sealed class INTERNAL_NamedObject
    {
        private string _name;

        public INTERNAL_NamedObject(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            this._name = name;
        }

        public override string ToString()
        {
            if (this._name[0] != '{')
            {
                // lazily add {} around the name, to avoid allocating a string
                // until it's actually needed
                this._name = string.Format(CultureInfo.InvariantCulture, "{{{0}}}", this._name);
            }
            return this._name;
        }
    }
}
