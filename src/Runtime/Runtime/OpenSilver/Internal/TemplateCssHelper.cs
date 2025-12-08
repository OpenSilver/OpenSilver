
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
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using CSHTML5.Internal;

namespace OpenSilver.Internal;

/// <summary>
/// Optimizes template rendering by generating CSS classes for template styles.
/// On first instantiation of a template, CSS properties are recorded per element position.
/// On subsequent instantiations, a CSS class is applied instead of individual CSS calls.
/// </summary>
internal static class TemplateCssHelper
{
    /// <summary>
    /// Stores CSS class information per template.
    /// Key is the FrameworkTemplate instance.
    /// </summary>
    private static readonly ConditionalWeakTable<FrameworkTemplate, TemplateCssCache> _templateCache = new();

    /// <summary>
    /// Tracks element rendering order per templated parent.
    /// This allows us to assign consistent element indices within each template instance.
    /// </summary>
    private static readonly ConditionalWeakTable<DependencyObject, InstanceRenderState> _instanceStates = new();

    /// <summary>
    /// Counter for generating unique template IDs.
    /// </summary>
    private static int _templateIdCounter;

    /// <summary>
    /// Current element being rendered (thread-static for thread safety).
    /// </summary>
    [ThreadStatic]
    private static ElementRenderContext _currentContext;

    /// <summary>
    /// Called when an element that is part of a template starts rendering.
    /// Returns the CSS class name to apply, or null if this is the first render (learning mode).
    /// </summary>
    public static string OnElementRenderStart(UIElement element)
    {
        if (element is not FrameworkElement fe)
        {
            return null;
        }

        // Get the templated parent - this identifies which control instance this element belongs to
        DependencyObject templatedParent = fe.TemplatedParent;
        if (templatedParent is null)
        {
            return null;
        }

        // Get the template this element belongs to
        FrameworkTemplate template = GetElementTemplate(fe);
        if (template is null)
        {
            return null;
        }

        // Get or create cache for this template
        TemplateCssCache templateCache = _templateCache.GetOrCreateValue(template);

        // Get or create render state for this template instance
        InstanceRenderState instanceState = _instanceStates.GetOrCreateValue(templatedParent);
        
        // Get element index for this element within the instance
        int elementIndex = instanceState.GetNextElementIndex();

        // Set up rendering context
        _currentContext = new ElementRenderContext
        {
            ElementId = element.OuterDiv?.UniqueIdentifier,
            Template = template,
            TemplateCache = templateCache,
            ElementIndex = elementIndex
        };

        // Check if we have a CSS class for this element position
        if (templateCache.TryGetCssClassName(elementIndex, out string className))
        {
            // CSS class exists - mark context as not learning
            _currentContext.IsLearning = false;
            return className;
        }

        // First time seeing this element position - enable learning mode
        _currentContext.IsLearning = true;
        _currentContext.RecordedProperties = new Dictionary<string, string>();
        return null;
    }

    /// <summary>
    /// Called when a CSS property is about to be set on an element.
    /// Returns true if the property should be skipped (already in CSS class with same value).
    /// </summary>
    public static bool TryRecordOrSkipCss(string elementId, string propertyName, string value)
    {
        var ctx = _currentContext;
        if (ctx is null || ctx.ElementId != elementId)
        {
            return false; // Not in template context or different element
        }

        if (ctx.IsLearning)
        {
            // Learning mode - record the CSS property (last value wins if set multiple times)
            if (ctx.RecordedProperties is not null)
            {
                ctx.RecordedProperties[propertyName] = value;
            }
            return false; // Don't skip, let it render normally
        }
        else
        {
            // Only skip if property is in the cached CSS class AND value matches.
            // This ensures TemplateBinding values (which may differ) are not skipped.
            return ctx.TemplateCache.ShouldSkipCss(ctx.ElementIndex, propertyName, value);
        }
    }

    /// <summary>
    /// Called when an element finishes rendering.
    /// If learning mode was active, creates a CSS class for this element position.
    /// </summary>
    public static void OnElementRenderEnd(UIElement element)
    {
        var ctx = _currentContext;
        if (ctx is null)
        {
            return;
        }

        // Clear context first
        _currentContext = null;

        if (ctx.IsLearning && ctx.RecordedProperties?.Count > 0)
        {
            // Generate CSS class for this element position
            ctx.TemplateCache.CreateCssClass(ctx.ElementIndex, ctx.RecordedProperties);
        }
    }

    /// <summary>
    /// Gets the template that an element belongs to.
    /// </summary>
    private static FrameworkTemplate GetElementTemplate(FrameworkElement element)
    {
        DependencyObject templatedParent = element.TemplatedParent;
        if (templatedParent is null)
        {
            return null;
        }

        if (templatedParent is Control control)
        {
            return control.Template;
        }
        if (templatedParent is ContentPresenter cp)
        {
            return cp.ContentTemplate;
        }
        if (templatedParent is ItemsPresenter ip)
        {
            return ip.Template;
        }

        return null;
    }

    /// <summary>
    /// Generates a unique template ID.
    /// </summary>
    internal static int GenerateTemplateId() => ++_templateIdCounter;

    /// <summary>
    /// Context for the element currently being rendered.
    /// </summary>
    private sealed class ElementRenderContext
    {
        public string ElementId { get; init; }
        public FrameworkTemplate Template { get; init; }
        public TemplateCssCache TemplateCache { get; init; }
        public int ElementIndex { get; init; }
        public bool IsLearning { get; set; }
        public Dictionary<string, string> RecordedProperties { get; set; }
    }
}

/// <summary>
/// Tracks element rendering order for a single template instance.
/// </summary>
internal sealed class InstanceRenderState
{
    private int _elementIndex;

    public int GetNextElementIndex() => _elementIndex++;
}

/// <summary>
/// Stores CSS class information for a template.
/// Shared across all instances of the same template.
/// </summary>
internal sealed class TemplateCssCache
{
    private readonly int _templateId = TemplateCssHelper.GenerateTemplateId();
    private readonly Dictionary<int, ElementCssClass> _elementClasses = new();
    private readonly object _lock = new();

    /// <summary>
    /// Tries to get the CSS class name for an element position.
    /// </summary>
    public bool TryGetCssClassName(int elementIndex, out string className)
    {
        lock (_lock)
        {
            if (_elementClasses.TryGetValue(elementIndex, out ElementCssClass elementClass))
            {
                className = elementClass.ClassName;
                return true;
            }
            className = null;
            return false;
        }
    }

    /// <summary>
    /// Checks if a CSS property should be skipped (exists in class with same value).
    /// </summary>
    public bool ShouldSkipCss(int elementIndex, string propertyName, string value)
    {
        lock (_lock)
        {
            if (_elementClasses.TryGetValue(elementIndex, out ElementCssClass elementClass))
            {
                return elementClass.HasPropertyWithValue(propertyName, value);
            }
            return false;
        }
    }

    /// <summary>
    /// Creates a CSS class for an element position with the given properties.
    /// </summary>
    public void CreateCssClass(int elementIndex, Dictionary<string, string> properties)
    {
        lock (_lock)
        {
            // Don't recreate if already exists
            if (_elementClasses.ContainsKey(elementIndex))
            {
                return;
            }

            string className = $"os-tpl-{_templateId}-{elementIndex}";
            
            _elementClasses[elementIndex] = new ElementCssClass(className, properties);

            // Generate and inject CSS
            InjectCss(className, properties);
        }
    }

    /// <summary>
    /// Generates CSS string and injects it into the document.
    /// </summary>
    private static void InjectCss(string className, Dictionary<string, string> properties)
    {
        var cssBuilder = new StringBuilder();
        cssBuilder.Append('.');
        cssBuilder.Append(className);
        cssBuilder.Append('{');

        foreach (var prop in properties)
        {
            // Convert camelCase to kebab-case
            cssBuilder.Append(ToKebabCase(prop.Key));
            cssBuilder.Append(':');
            cssBuilder.Append(prop.Value);
            cssBuilder.Append(';');
        }

        cssBuilder.Append('}');

        string css = cssBuilder.ToString().Replace("'", "\\'");
        OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
            $"(function(){{var s=document.getElementById('os-template-css');if(!s){{s=document.createElement('style');s.id='os-template-css';document.head.appendChild(s);}}s.textContent+='{css}';}})()");
    }

    /// <summary>
    /// Converts camelCase to kebab-case.
    /// </summary>
    private static string ToKebabCase(string camelCase)
    {
        if (string.IsNullOrEmpty(camelCase))
        {
            return camelCase;
        }

        var result = new StringBuilder(camelCase.Length + 4);
        for (int i = 0; i < camelCase.Length; i++)
        {
            char c = camelCase[i];
            if (char.IsUpper(c))
            {
                if (i > 0)
                {
                    result.Append('-');
                }
                result.Append(char.ToLowerInvariant(c));
            }
            else
            {
                result.Append(c);
            }
        }
        return result.ToString();
    }
}

/// <summary>
/// Stores information about a CSS class for a single element position.
/// </summary>
internal sealed class ElementCssClass
{
    public string ClassName { get; }
    private readonly Dictionary<string, string> _properties;

    public ElementCssClass(string className, Dictionary<string, string> properties)
    {
        ClassName = className;
        // Store a copy of the properties
        _properties = new Dictionary<string, string>(properties);
    }

    /// <summary>
    /// Checks if the class has a property with the specified value.
    /// Returns true only if both property name AND value match.
    /// </summary>
    public bool HasPropertyWithValue(string name, string value)
    {
        return _properties.TryGetValue(name, out string cachedValue) 
            && string.Equals(cachedValue, value, StringComparison.Ordinal);
    }
}
