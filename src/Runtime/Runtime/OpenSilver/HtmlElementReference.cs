
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

using OpenSilver.Internal;
using System;

namespace OpenSilver;

public readonly struct HtmlElementReference : IJavaScriptConvertible
{
    public HtmlElementReference(string uid)
    {
        ArgumentException.ThrowIfNullOrEmpty(uid);
        Uid = uid;
    }

    public readonly string Uid;

    public bool IsConnected => !string.IsNullOrEmpty(Uid);

    internal void SetAttribute(string name, string value) => SetAttributeImpl(name, $"\"{value}\"");

    internal void SetAttribute(string name, double value) => SetAttributeImpl(name, value.ToInvariantString());

    internal void SetAttribute(string name, int value) => SetAttributeImpl(name, value.ToInvariantString());

    internal void SetAttribute(string name, bool value) => SetAttributeImpl(name, value ? "true" : "false");

    private void SetAttributeImpl(string name, string value) =>
        Interop.ExecuteJavaScriptVoidAsync($"osjs.setAttr('{Uid}','{name}',{value})");

    internal void RemoveAttribute(string name) =>
        Interop.ExecuteJavaScriptVoidAsync($"osjs.unsetAttr('{Uid}','{name}')");

    internal void SetCssStyleProperty(string propertyName, string value) =>
        Interop.ExecuteJavaScriptVoidAsync($"osjs.setCSS('{Uid}','{propertyName}','{value}')");

    internal void SetProperty(string name, double value) => SetPropertyImpl(name, value.ToInvariantString());

    internal void SetProperty(string name, string value) => SetPropertyImpl(name, $"\"{value}\"");

    private void SetPropertyImpl(string name, string value) =>
        Interop.ExecuteJavaScriptVoidAsync($"osjs.setProp('{Uid}','{name}',{value})");

    string IJavaScriptConvertible.ToJavaScriptString() => $"document.getElementById(\"{Uid}\")";

    public bool Equals(HtmlElementReference other) => Uid == other.Uid;

    public override bool Equals(object obj) => obj is HtmlElementReference other && Equals(other);

    public override int GetHashCode() => Uid?.GetHashCode() ?? 0;

    public static bool operator ==(HtmlElementReference left, HtmlElementReference right) => left.Equals(right);

    public static bool operator !=(HtmlElementReference left, HtmlElementReference right) => !left.Equals(right);
}
