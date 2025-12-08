
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
    /// </summary>
    private static readonly ConditionalWeakTable<DependencyObject, InstanceRenderState> _instanceStates = new();

    /// <summary>
    /// Counter for generating unique template IDs.
    /// </summary>
    private static int _templateIdCounter;

    /// <summary>
    /// Stack of render contexts to handle nested element rendering.
    /// </summary>
    [ThreadStatic]
    private static Stack<ElementRenderContext> _contextStack;

    /// <summary>
    /// Maps DOM element IDs to their render contexts for quick lookup during CSS setting.
    /// </summary>
    [ThreadStatic]
    private static Dictionary<string, ElementRenderContext> _activeContexts;

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

        // Get the templated parent
        DependencyObject templatedParent = fe.TemplatedParent;
        if (templatedParent is null)
        {
            return null;
        }

        // Get the template
        FrameworkTemplate template = GetElementTemplate(fe);
        if (template is null)
        {
            return null;
        }

        // Get DOM element ID
        string elementId = element.OuterDiv?.UniqueIdentifier;
        if (string.IsNullOrEmpty(elementId))
        {
            return null;
        }

        // Get or create caches
        TemplateCssCache templateCache = _templateCache.GetOrCreateValue(template);
        InstanceRenderState instanceState = _instanceStates.GetOrCreateValue(templatedParent);
        int elementIndex = instanceState.GetNextElementIndex();

        // Initialize thread-static collections if needed
        _contextStack ??= new Stack<ElementRenderContext>();
        _activeContexts ??= new Dictionary<string, ElementRenderContext>();

        // Create context
        var context = new ElementRenderContext
        {
            ElementId = elementId,
            TemplateCache = templateCache,
            ElementIndex = elementIndex
        };

        // Check if we have a CSS class for this element position
        if (templateCache.TryGetCssClassName(elementIndex, out string className))
        {
            context.IsLearning = false;
        }
        else
        {
            context.IsLearning = true;
            context.RecordedProperties = new Dictionary<string, string>();
            className = null;
        }

        // Push context onto stack and register for quick lookup
        _contextStack.Push(context);
        _activeContexts[elementId] = context;

        return className;
    }

    /// <summary>
    /// Called when a CSS property is about to be set on an element.
    /// Returns true if the property should be skipped (already in CSS class).
    /// </summary>
    public static bool TryRecordOrSkipCss(string elementId, string propertyName, string value)
    {
        if (_activeContexts is null || string.IsNullOrEmpty(elementId))
        {
            return false;
        }

        if (!_activeContexts.TryGetValue(elementId, out ElementRenderContext ctx))
        {
            return false;
        }

        if (ctx.IsLearning)
        {
            // Learning mode - record the CSS property
            ctx.RecordedProperties[propertyName] = value;
            return false; // Don't skip, let it render normally
        }
        else
        {
            // Skip if property is in the cached CSS class.
            // Even if the value differs (e.g., from TemplateBinding), the inline style
            // will be set on the NEXT render after properties are processed.
            // For now, we skip the CSS call. If value truly differs, the inline style
            // from a later TemplateBinding update will override the class style.
            return ctx.TemplateCache.HasProperty(ctx.ElementIndex, propertyName);
        }
    }

    /// <summary>
    /// Called when an element finishes rendering.
    /// </summary>
    public static void OnElementRenderEnd(UIElement element)
    {
        if (_contextStack is null || _contextStack.Count == 0)
        {
            return;
        }

        var ctx = _contextStack.Pop();

        // Remove from active contexts
        if (_activeContexts is not null && ctx.ElementId is not null)
        {
            _activeContexts.Remove(ctx.ElementId);
        }

        // If learning mode completed, create CSS class
        if (ctx.IsLearning && ctx.RecordedProperties?.Count > 0)
        {
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

    internal static int GenerateTemplateId() => ++_templateIdCounter;

    private sealed class ElementRenderContext
    {
        public string ElementId { get; init; }
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
/// </summary>
internal sealed class TemplateCssCache
{
    private readonly int _templateId = TemplateCssHelper.GenerateTemplateId();
    private readonly Dictionary<int, ElementCssClass> _elementClasses = new();
    private readonly object _lock = new();

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

    public bool HasProperty(int elementIndex, string propertyName)
    {
        lock (_lock)
        {
            if (_elementClasses.TryGetValue(elementIndex, out ElementCssClass elementClass))
            {
                return elementClass.HasProperty(propertyName);
            }
            return false;
        }
    }

    public void CreateCssClass(int elementIndex, Dictionary<string, string> properties)
    {
        lock (_lock)
        {
            if (_elementClasses.ContainsKey(elementIndex))
            {
                return;
            }

            string className = $"os-tpl-{_templateId}-{elementIndex}";
            _elementClasses[elementIndex] = new ElementCssClass(className, properties);
            InjectCss(className, properties);
        }
    }

    private static void InjectCss(string className, Dictionary<string, string> properties)
    {
        var cssBuilder = new StringBuilder();
        cssBuilder.Append('.');
        cssBuilder.Append(className);
        cssBuilder.Append('{');

        foreach (var prop in properties)
        {
            cssBuilder.Append(ToKebabCase(prop.Key));
            cssBuilder.Append(':');
            cssBuilder.Append(prop.Value);
            cssBuilder.Append(';');
        }

        cssBuilder.Append('}');

        string css = cssBuilder.ToString().Replace("'", "\\'").Replace("\\", "\\\\");
        OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
            $"(function(){{var s=document.getElementById('os-template-css');if(!s){{s=document.createElement('style');s.id='os-template-css';document.head.appendChild(s);}}s.textContent+='{css}';}})()");
    }

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

internal sealed class ElementCssClass
{
    public string ClassName { get; }
    private readonly HashSet<string> _propertyNames;

    public ElementCssClass(string className, Dictionary<string, string> properties)
    {
        ClassName = className;
        _propertyNames = new HashSet<string>(properties.Keys);
    }

    public bool HasProperty(string name) => _propertyNames.Contains(name);
}
