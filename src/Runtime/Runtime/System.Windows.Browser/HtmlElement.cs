
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
using System.Security;
using System.Windows.Browser.Internal;

namespace System.Windows.Browser
{
    /// <summary>
    /// Represents an HTML element in the Document Object Model (DOM) of a Web page.
    /// </summary>
    public sealed class HtmlElement : HtmlObject
    {
        internal HtmlElement(IJSObjectRef jsInstance)
            : base(jsInstance)
        {
        }

        /// <summary>
        /// Gets a read-only collection of HTML elements that are immediate descendants of
        /// the current HTML element.
        /// </summary>
        /// <returns>
        /// A collection of HTML elements. If the current HTML element has no children, the
        /// returned collection is empty.
        /// </returns>
        public ScriptObjectCollection Children => (ScriptObjectCollection)GetProperty("childNodes");

        /// <summary>
        /// Gets the identifier of the current HTML element.
        /// </summary>
        /// <returns>
        /// An HTML element ID string if the current element has an identifier; otherwise,
        /// an empty string.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The property is set to an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The property is set to null.
        /// </exception>
        public string Id
        {
            get => (string)GetProperty("id");
            set
            {
                ValidateParameter(value);
                SetProperty("id", value);
            }
        }

        /// <summary>
        /// Gets a reference to the parent of the current HTML element.
        /// </summary>
        /// <returns>
        /// An HTML element reference if the element has a parent; otherwise, null.
        /// </returns>
        public HtmlElement Parent => (HtmlElement)GetProperty("parentElement");

        /// <summary>
        /// Gets the HTML tag name of the current HTML element.
        /// </summary>
        /// <returns>
        /// An HTML element tag name, such as div or span.
        /// </returns>
        public string TagName => ((string)GetProperty("tagName"))?.ToLower() ?? string.Empty;

        /// <summary>
        /// Gets or sets the cascading style sheet (CSS) class string for the current
        /// HTML element.
        /// </summary>
        /// <returns>
        /// A CSS class string if the element is associated with a CSS class; otherwise,
        /// an empty string.
        /// </returns>
        public string CssClass
        {
            get => (string)GetProperty("className") ?? string.Empty;
            set
            {
                CheckInvalidCharacters(value);
                SetProperty("className", value ?? string.Empty);
            }
        }

        /// <summary>
        /// Sets the browser focus to the current HTML element.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// All errors.
        /// </exception>
        [SecuritySafeCritical]
        public void Focus() => Invoke("focus");

        /// <summary>
        /// Adds an element to the end of the child collection for the current HTML element.
        /// </summary>
        /// <param name="element">
        /// The element to be added.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// element is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// element is a reference to this <see cref="HtmlElement"/>.
        /// </exception>
        public void AppendChild(HtmlElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (this == element)
            {
                throw new InvalidOperationException("An element cannot call appendChild on itself.");
            }

            Invoke("appendChild", element);
        }

        /// <summary>
        /// Adds an element at a specified location in the child element collection for the
        /// current HTML element.
        /// </summary>
        /// <param name="element">
        /// The element to be added.
        /// </param>
        /// <param name="referenceElement">
        /// The location to insert the element. The element is added before referenceElement.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// referenceElement is not in the child element collection of the current HTML element.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// element is null.
        /// </exception>
        [SecuritySafeCritical]
        public void AppendChild(HtmlElement element, HtmlElement referenceElement)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            Invoke("insertBefore", element, referenceElement);
        }

        /// <summary>
        /// Removes the specified element from the child element collection of the current
        /// HTML element.
        /// </summary>
        /// <param name="element">
        /// The element to remove.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// element is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// All other errors.
        /// </exception>
        [SecuritySafeCritical]
        public void RemoveChild(HtmlElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            Invoke("removeChild", element);
        }

        /// <summary>
        /// Sets the value of an attribute on the current HTML element.
        /// </summary>
        /// <param name="name">
        /// The attribute whose value will be set.
        /// </param>
        /// <param name="value">
        /// The attribute's new value.
        /// </param>
        /// <exception cref="ArgumentException">
        /// name is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// name is null.
        /// </exception>
        public void SetAttribute(string name, string value)
        {
            CheckNullOrEmpty(name);

            Invoke("setAttribute", name, value);
        }

        /// <summary>
        /// Gets the value of the specified attribute on the current HTML element.
        /// </summary>
        /// <param name="name">
        /// The name of an attribute.
        /// </param>
        /// <returns>
        /// An attribute on the current HTML element.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// name is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// name is null.
        /// </exception>
        public string GetAttribute(string name)
        {
            CheckNullOrEmpty(name);

            return (string)Invoke("getAttribute", name);
        }

        /// <summary>
        /// Retrieves the specified style attribute for the current HTML element.
        /// </summary>
        /// <param name="name">
        /// The name of the style attribute to retrieve.
        /// </param>
        /// <returns>
        /// The style attribute for the current HTML element.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// name is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// name is null.
        /// </exception>
        public string GetStyleAttribute(string name)
        {
            CheckNullOrEmpty(name);

            ScriptObject style = (ScriptObject)GetProperty("style");
            return (string)style.GetProperty(name);
        }

        /// <summary>
        /// Sets the value of a style attribute on the current HTML element.
        /// </summary>
        /// <param name="name">
        /// The style attribute whose value will be set.
        /// </param>
        /// <param name="value">
        /// The style attribute's new value.
        /// </param>
        /// <exception cref="ArgumentException">
        /// name is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// name is null.
        /// </exception>
        public void SetStyleAttribute(string name, string value)
        {
            CheckNullOrEmpty(name);

            ScriptObject style = (ScriptObject)GetProperty("style");
            style.SetProperty(name, value);
        }

        /// <summary>
        /// Removes a style attribute on the current HTML element.
        /// </summary>
        /// <param name="name">
        /// The style attribute to be removed.
        /// </param>
        /// <exception cref="ArgumentException">
        /// name is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// name is null.
        /// </exception>
        public void RemoveStyleAttribute(string name)
        {
            CheckNullOrEmpty(name);

            ScriptObject style = (ScriptObject)GetProperty("style");
            style.SetProperty(name, string.Empty);
        }

        /// <summary>
        /// Removes an attribute from the current HTML element.
        /// </summary>
        /// <param name="name">
        /// The name of the attribute to remove.
        /// </param>
        /// <exception cref="ArgumentException">
        /// name is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// name is null.
        /// </exception>
        public void RemoveAttribute(string name)
        {
            CheckNullOrEmpty(name);

            Invoke("removeAttribute", name);
        }
    }
}
