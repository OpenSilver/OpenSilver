using System;
using System.Text.Json;

namespace Runtime.OpenSilver.Tests
{
    public class ExecuteJavascriptEventArgs : EventArgs
    {
        public bool Handled { get; set; }

        public JsonElement Result { get; set; }

        public string Javascript { get; set; }
    }
}