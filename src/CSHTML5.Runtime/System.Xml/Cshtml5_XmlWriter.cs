

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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace System.Xml
{
    // allows to instanciate XmlWritter without a complete implementation (setting equivalent to indent = false)
    internal partial class Cshtml5_XmlWriter : XmlWriter
    {
        private string _data;

        private string _header;

        private List<string> _elementsName; // stack will be better

        private bool _open;

        private bool _openTag; // This means that we are in a tag. (no matter whether in an attribute or not, so between '<' and the corresponding '>').

        private bool _isInAttributeValue; // This means that we are in a place like '<Element Attr="HERE'. This means that we can Write a string (with WriteString) there without needing to close the tag.

        private XDocument _xmlDoc;

        private Stream _stream;

        private StreamWriter _writter;

        public Cshtml5_XmlWriter(XDocument xmlDoc)
        {
            _xmlDoc = xmlDoc;
            _stream = null;
            Init();
        }

        public Cshtml5_XmlWriter(Stream stream)
        {
            _xmlDoc = null;
            _stream = stream;

            _writter = new StreamWriter(_stream);

            Init();
        }

        private void Init()
        {
            _header = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine;
            _data = "";
            _elementsName = new List<string>();
            _open = true;
            _openTag = false;
            _isInAttributeValue = false;
        }

        public new void WriteElementString(string localName, string value)
        {
            WriteStartElement(null, localName, null);
            WriteString(value);
            WriteEndElement();
        }

        public new void WriteAttributeString(string localName, string value)
        {
            WriteStartAttribute(null, localName, null);
            WriteString(value);
            WriteEndAttribute();
        }

        public override void WriteString(string text)
        {
            if (!_isInAttributeValue && _openTag)
            {
                AddData(">");
                _openTag = false;
            }

            AddData(text);
        }

        // Writes the attribute with the specified local name (basically writes: " localName=").
        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            CheckOpen();

            _isInAttributeValue = true;

            AddData(" " + localName + "=" + "\"");
        }

        public override void WriteEndAttribute()
        {
            // we don't write the '>' caracter, to allow multiple attributes.
            // the '>' caracter will be automatically added if a new element is written
            AddData("\" ");
            _isInAttributeValue = false;
        }

        // closes the last tag in the list by writing </name>
        public override void WriteEndElement()
        {
            CheckOpen();

            CloseTagIfOpen(false);

            string Element;

            if (_elementsName.Count > 0)
            {
                Element = _elementsName[_elementsName.Count - 1];

                _elementsName.RemoveAt(_elementsName.Count - 1);

                AddData("</" + Element + ">" + Environment.NewLine);
            }
        }

        // closes the document so it is not possible to add anything after that
        public override void Close()
        {
            if (_open)
            {
                CloseAllTag();

                _open = false;

                if (_xmlDoc != null)
                    _xmlDoc = XDocument.Parse(_data);
            }
            else
            {
                throw new InvalidOperationException("XmlWriter is already close !");
            }
        }

        // Writes a new tag with a name, but does not close it so it is possible to add information to it
        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            CheckOpen();

            CloseTagIfOpen(true);

            AddData("<" + localName);

            _elementsName.Add(localName);

            _openTag = true;
        }

        public new void WriteStartElement(string localName)
        {
            WriteStartElement(null, localName, null);
        }

        //todo: the wpf one does not do that but this solution is better for debugging
        public override string ToString()
        {
            return _data;
        }

        // check if the XmlWriter is open
        private void CheckOpen()
        {
            if (!_open)
            {
                throw new InvalidOperationException("Error to write in XmlWriter: The document is close !");
            }
        }

        private void CloseTagIfOpen(bool LineReturn)
        {
            if (_openTag)
            {
                AddData(">");

                if (LineReturn)
                    AddData(Environment.NewLine);
            }

            _isInAttributeValue = false;
            _openTag = false;
        }

        private void CloseAllTag()
        {
            while (_elementsName.Count > 0)
            {
                WriteEndElement();
            }
        }

        private void AddData(string newData)
        {
            _data += newData;

            //todo : always Close tag (probably not?)
            if (_stream != null)
            {
                _writter.Write(newData);
                _writter.Flush();
            }
        }

        public static XmlWriter Create(Stream output)
        {
            return new Cshtml5_XmlWriter(output);
        }

        public override void WriteStartDocument()
        {
            throw new NotImplementedException();
        }

        public override void WriteEndDocument()
        {
            throw new NotImplementedException();
        }

        #region Unimplemented_Override

        public override void WriteStartDocument(bool standalone) { }

        public override void WriteDocType(string name, string pubid, string sysid, string subset) { }

        public override void WriteFullEndElement() { }

        public override void WriteCData(string text) { }

        public override void WriteComment(string text) { }

        public override void WriteProcessingInstruction(string name, string text) { }

        public override void WriteEntityRef(string name) { }

        public override void WriteCharEntity(char ch) { }

        public override void WriteWhitespace(string ws) { }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar) { }

        public override void WriteChars(char[] buffer, int index, int count) { }

        public override void WriteRaw(char[] buffer, int index, int count) { }

        public override void WriteRaw(string data) { }

        public override void WriteBase64(byte[] buffer, int index, int count) { }

        public override WriteState WriteState { get { throw new NotImplementedException(); } }

        public override void Flush() { }

        public override string LookupPrefix(string ns) { throw new NotImplementedException(); }


        #endregion
    }
}
