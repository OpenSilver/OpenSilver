
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

using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Stores per-instance trigger state for a FrameworkElement with a template.
/// This class handles triggers defined in ControlTemplate and DataTemplate.
/// </summary>
/// <remarks>
/// Memory optimization notes:
/// - Dictionaries are lazily initialized to avoid allocations when not needed
/// - Uses HashSet for active triggers instead of Dictionary with TriggerState objects
/// - SourceName-related maps are only allocated when SourceName is used (uncommon)
/// </remarks>
internal sealed class TemplateTriggerStorage
{
    private readonly FrameworkElement _templatedParent;
    private readonly FrameworkTemplate _template;
    private readonly INameScope _nameScope;

    // Active triggers set - uses HashSet instead of Dictionary<TriggerBase, TriggerState>
    private HashSet<TriggerBase> _activeTriggers;

    // Lazily initialized - only allocated when property triggers exist (without SourceName)
    private Dictionary<DependencyProperty, List<TriggerBase>> _propertyTriggerMap;

    // Lazily initialized - only allocated when DataTriggers exist
    private Dictionary<TriggerBase, List<DataTriggerBindingHelper>> _dataTriggerHelpers;

    // Lazily initialized - only allocated when EventTriggers exist
    private Dictionary<EventTrigger, (RoutedEventHandler Handler, FrameworkElement Source)> _eventTriggerHandlers;

    // Lazily initialized - only allocated when SourceName is used (uncommon)
    // Maps source elements to property triggers
    private Dictionary<DependencyObject, Dictionary<DependencyProperty, List<TriggerBase>>> _sourceElementTriggerMap;

    // Lazily initialized - only allocated when SourceName is used (uncommon)
    // Stores the resolved source element for each trigger with SourceName
    private Dictionary<TriggerBase, DependencyObject> _triggerSourceElements;

    private bool _isProcessingTriggers;

    internal TemplateTriggerStorage(FrameworkElement templatedParent, FrameworkTemplate template)
    {
        _templatedParent = templatedParent;
        _template = template;
        _nameScope = FrameworkTemplate.GetTemplateNameScope(templatedParent);
    }

    /// <summary>
    /// Gets a value indicating whether this template has triggers.
    /// </summary>
    private bool HasTriggers
    {
        get
        {
            return _template switch
            {
                ControlTemplate ct => ct.HasTriggers,
                DataTemplate dt => dt.HasTriggers,
                _ => false
            };
        }
    }

    /// <summary>
    /// Gets the triggers collection from the template.
    /// </summary>
    private StyleTriggerCollection GetTriggers()
    {
        return _template switch
        {
            ControlTemplate ct => ct.Triggers,
            DataTemplate dt => dt.Triggers,
            _ => null
        };
    }

    /// <summary>
    /// Initializes trigger tracking and evaluates initial states.
    /// </summary>
    internal void Initialize()
    {
        if (!HasTriggers)
        {
            return;
        }

        var triggers = GetTriggers();
        if (triggers is null)
        {
            return;
        }

        foreach (TriggerBase trigger in triggers)
        {
            SetupTrigger(trigger);
        }

        // Now evaluate all triggers to set initial state
        EvaluateAllTriggers();
    }

    private void SetupTrigger(TriggerBase trigger)
    {
        // Ensure the trigger is sealed (performs validation and type conversions)
        trigger.Seal();

        // Set up name resolvers for any storyboards in the trigger's actions
        // This must be done once so storyboards can find named elements in the template
        SetupTriggerStoryboards(trigger);

        switch (trigger)
        {
            case Trigger propertyTrigger:
                SetupPropertyTrigger(propertyTrigger);
                break;

            case DataTrigger dataTrigger:
                SetupDataTrigger(dataTrigger);
                break;

            case MultiTrigger multiTrigger:
                SetupMultiTrigger(multiTrigger);
                break;

            case MultiDataTrigger multiDataTrigger:
                SetupMultiDataTrigger(multiDataTrigger);
                break;

            case EventTrigger eventTrigger:
                SetupEventTrigger(eventTrigger);
                break;
        }
    }

    /// <summary>
    /// Sets up name resolvers for all storyboards in the trigger's actions.
    /// </summary>
    private void SetupTriggerStoryboards(TriggerBase trigger)
    {
        // Handle EventTrigger.Actions
        if (trigger is EventTrigger eventTrigger)
        {
            foreach (TriggerAction action in eventTrigger.Actions)
            {
                if (action is BeginStoryboard beginStoryboard && beginStoryboard.Storyboard is Storyboard storyboard)
                {
                    SetupStoryboardNameResolver(storyboard);
                }
            }
        }

        // Handle EnterActions
        if (trigger.HasEnterActions)
        {
            foreach (TriggerAction action in trigger.EnterActions)
            {
                if (action is BeginStoryboard beginStoryboard && beginStoryboard.Storyboard is Storyboard storyboard)
                {
                    SetupStoryboardNameResolver(storyboard);
                }
            }
        }

        // Handle ExitActions
        if (trigger.HasExitActions)
        {
            foreach (TriggerAction action in trigger.ExitActions)
            {
                if (action is BeginStoryboard beginStoryboard && beginStoryboard.Storyboard is Storyboard storyboard)
                {
                    SetupStoryboardNameResolver(storyboard);
                }
            }
        }
    }

    private void SetupPropertyTrigger(Trigger trigger)
    {
        if (trigger.Property is null)
        {
            return;
        }

        DependencyProperty dp = trigger.Property;

        // Check if this trigger has a SourceName
        if (!string.IsNullOrEmpty(trigger.SourceName))
        {
            // Resolve the source element by name
            DependencyObject sourceElement = ResolveNamedElement(trigger.SourceName);
            if (sourceElement is not null)
            {
                _triggerSourceElements ??= new Dictionary<TriggerBase, DependencyObject>();
                _triggerSourceElements[trigger] = sourceElement;
                RegisterSourceElementTrigger(sourceElement, dp, trigger);
            }
        }
        else
        {
            // Listen to property changes on the templated parent
            _propertyTriggerMap ??= new Dictionary<DependencyProperty, List<TriggerBase>>();
            if (!_propertyTriggerMap.TryGetValue(dp, out List<TriggerBase> triggers))
            {
                triggers = new List<TriggerBase>();
                _propertyTriggerMap[dp] = triggers;
            }
            triggers.Add(trigger);
        }
    }

    private void RegisterSourceElementTrigger(DependencyObject sourceElement, DependencyProperty dp, TriggerBase trigger)
    {
        _sourceElementTriggerMap ??= new Dictionary<DependencyObject, Dictionary<DependencyProperty, List<TriggerBase>>>();
        
        if (!_sourceElementTriggerMap.TryGetValue(sourceElement, out var propMap))
        {
            propMap = new Dictionary<DependencyProperty, List<TriggerBase>>();
            _sourceElementTriggerMap[sourceElement] = propMap;
        }

        if (!propMap.TryGetValue(dp, out var triggers))
        {
            triggers = new List<TriggerBase>();
            propMap[dp] = triggers;
        }

        if (!triggers.Contains(trigger))
        {
            triggers.Add(trigger);
        }
    }

    /// <summary>
    /// Resolves a named element from the template's name scope.
    /// If name is null/empty, returns the templated parent.
    /// </summary>
    private DependencyObject ResolveNamedElement(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return _templatedParent;
        }

        return _nameScope?.FindName(name) as DependencyObject;
    }

    private void SetupDataTrigger(DataTrigger trigger)
    {
        if (trigger.Binding is null)
        {
            return;
        }

        SetupDataTriggerBinding(trigger, trigger.Binding);
    }

    private void SetupMultiTrigger(MultiTrigger trigger)
    {
        foreach (Condition condition in trigger.Conditions)
        {
            if (condition.Property is not null)
            {
                DependencyProperty dp = condition.Property;

                // Check if this condition has a SourceName
                if (!string.IsNullOrEmpty(condition.SourceName))
                {
                    DependencyObject sourceElement = ResolveNamedElement(condition.SourceName);
                    if (sourceElement is not null)
                    {
                        RegisterSourceElementTrigger(sourceElement, dp, trigger);
                    }
                }
                else
                {
                    _propertyTriggerMap ??= new Dictionary<DependencyProperty, List<TriggerBase>>();
                    if (!_propertyTriggerMap.TryGetValue(dp, out List<TriggerBase> triggers))
                    {
                        triggers = new List<TriggerBase>();
                        _propertyTriggerMap[dp] = triggers;
                    }
                    if (!triggers.Contains(trigger))
                    {
                        triggers.Add(trigger);
                    }
                }
            }
        }
    }

    private void SetupMultiDataTrigger(MultiDataTrigger trigger)
    {
        foreach (Condition condition in trigger.Conditions)
        {
            if (condition.Binding is not null)
            {
                SetupDataTriggerBinding(trigger, condition.Binding);
            }
        }
    }

    private void SetupEventTrigger(EventTrigger trigger)
    {
        if (trigger.RoutedEvent is null)
        {
            return;
        }

        // Create a handler that invokes the trigger's actions
        RoutedEventHandler handler = (sender, e) =>
        {
            foreach (TriggerAction action in trigger.Actions)
            {
                action.Invoke((IInternalFrameworkElement)_templatedParent);
            }
        };

        // Resolve the source element (defaults to templated parent if SourceName is not set)
        FrameworkElement sourceElement;
        if (!string.IsNullOrEmpty(trigger.SourceName))
        {
            sourceElement = ResolveNamedElement(trigger.SourceName) as FrameworkElement;
            if (sourceElement is null)
            {
                return; // Source element not found
            }
        }
        else
        {
            sourceElement = _templatedParent;
        }

        _eventTriggerHandlers ??= new Dictionary<EventTrigger, (RoutedEventHandler, FrameworkElement)>();
        _eventTriggerHandlers[trigger] = (handler, sourceElement);
        sourceElement.AddHandler(trigger.RoutedEvent, handler, false);
    }

    private void SetupDataTriggerBinding(TriggerBase trigger, BindingBase bindingBase)
    {
        if (bindingBase is not Binding binding)
        {
            return;
        }

        // Note: DataTriggerBindingHelper inherits from FrameworkElement which has significant
        // memory overhead. See remarks on the class for details.
        var helper = new DataTriggerBindingHelper(this, trigger, _templatedParent);

        var listenerBinding = new Binding
        {
            Path = binding.Path,
            Mode = BindingMode.OneWay,
        };

        if (binding.Source is not null)
        {
            listenerBinding.Source = binding.Source;
        }
        if (binding.RelativeSource is not null)
        {
            listenerBinding.RelativeSource = binding.RelativeSource;
        }
        if (!string.IsNullOrEmpty(binding.ElementName))
        {
            listenerBinding.ElementName = binding.ElementName;
        }

        BindingExpression expr = (BindingExpression)listenerBinding.CreateBindingExpression(
            helper,
            DataTriggerBindingHelper.ValueProperty,
            null);

        helper.SetValue(DataTriggerBindingHelper.ValueProperty, expr);

        _dataTriggerHelpers ??= new Dictionary<TriggerBase, List<DataTriggerBindingHelper>>();
        if (!_dataTriggerHelpers.TryGetValue(trigger, out var helpers))
        {
            helpers = new List<DataTriggerBindingHelper>();
            _dataTriggerHelpers[trigger] = helpers;
        }
        helpers.Add(helper);
    }

    /// <summary>
    /// Called when a dependency property value changes on the templated parent.
    /// </summary>
    internal void OnPropertyChanged(DependencyProperty dp)
    {
        if (_isProcessingTriggers)
        {
            return;
        }

        if (_propertyTriggerMap is not null &&
            _propertyTriggerMap.TryGetValue(dp, out List<TriggerBase> triggers))
        {
            foreach (TriggerBase trigger in triggers)
            {
                EvaluateTrigger(trigger);
            }
        }
    }

    /// <summary>
    /// Called when a dependency property value changes on a source element (used for SourceName triggers).
    /// </summary>
    internal void OnSourceElementPropertyChanged(DependencyObject sourceElement, DependencyProperty dp)
    {
        if (_isProcessingTriggers)
        {
            return;
        }

        if (_sourceElementTriggerMap is not null &&
            _sourceElementTriggerMap.TryGetValue(sourceElement, out var propMap) &&
            propMap.TryGetValue(dp, out List<TriggerBase> triggers))
        {
            foreach (TriggerBase trigger in triggers)
            {
                EvaluateTrigger(trigger);
            }
        }
    }

    internal void OnDataTriggerValueChanged(TriggerBase trigger)
    {
        if (_isProcessingTriggers)
        {
            return;
        }

        EvaluateTrigger(trigger);
    }

    private void EvaluateAllTriggers()
    {
        if (!HasTriggers)
        {
            return;
        }

        var triggers = GetTriggers();
        if (triggers is null)
        {
            return;
        }

        _isProcessingTriggers = true;
        try
        {
            foreach (TriggerBase trigger in triggers)
            {
                EvaluateTrigger(trigger);
            }
        }
        finally
        {
            _isProcessingTriggers = false;
        }
    }

    private void EvaluateTrigger(TriggerBase trigger)
    {
        bool isActive = EvaluateTriggerCondition(trigger);
        bool wasActive = _activeTriggers?.Contains(trigger) ?? false;

        if (isActive != wasActive)
        {
            if (isActive)
            {
                _activeTriggers ??= new HashSet<TriggerBase>();
                _activeTriggers.Add(trigger);

                ApplyTriggerSetters(trigger);
                InvokeEnterActions(trigger);
            }
            else
            {
                _activeTriggers?.Remove(trigger);

                UnapplyTriggerSetters(trigger);
                InvokeExitActions(trigger);
            }
        }
    }

    private bool EvaluateTriggerCondition(TriggerBase trigger)
    {
        switch (trigger)
        {
            case Trigger propertyTrigger:
                // Use the source element if SourceName was specified, otherwise use templated parent
                DependencyObject sourceElement = _triggerSourceElements?.TryGetValue(trigger, out var src) == true
                    ? src
                    : _templatedParent;
                return propertyTrigger.Evaluate(sourceElement);

            case MultiTrigger multiTrigger:
                return EvaluateMultiTrigger(multiTrigger);

            case DataTrigger dataTrigger:
                return EvaluateDataTrigger(dataTrigger);

            case MultiDataTrigger multiDataTrigger:
                return EvaluateMultiDataTrigger(multiDataTrigger);

            default:
                return false;
        }
    }

    private bool EvaluateMultiTrigger(MultiTrigger trigger)
    {
        foreach (Condition condition in trigger.Conditions)
        {
            if (condition.Property is null)
            {
                return false;
            }

            // Resolve the source element for this condition
            DependencyObject sourceElement = !string.IsNullOrEmpty(condition.SourceName)
                ? ResolveNamedElement(condition.SourceName)
                : _templatedParent;

            if (sourceElement is null)
            {
                return false;
            }

            object currentValue = sourceElement.GetValue(condition.Property);
            if (!Trigger.Match(currentValue, condition.Value))
            {
                return false;
            }
        }

        return trigger.Conditions.Count > 0;
    }

    private bool EvaluateDataTrigger(DataTrigger trigger)
    {
        if (_dataTriggerHelpers is null ||
            !_dataTriggerHelpers.TryGetValue(trigger, out var helpers) || 
            helpers.Count == 0)
        {
            return false;
        }

        object currentValue = helpers[0].Value;
        return Trigger.Match(currentValue, trigger.Value);
    }

    private bool EvaluateMultiDataTrigger(MultiDataTrigger trigger)
    {
        if (_dataTriggerHelpers is null ||
            !_dataTriggerHelpers.TryGetValue(trigger, out var helpers))
        {
            return false;
        }

        int helperIndex = 0;
        foreach (Condition condition in trigger.Conditions)
        {
            if (condition.Binding is null || helperIndex >= helpers.Count)
            {
                return false;
            }

            object currentValue = helpers[helperIndex].Value;
            if (!Trigger.Match(currentValue, condition.Value))
            {
                return false;
            }
            helperIndex++;
        }

        return trigger.Conditions.Count > 0;
    }

    private void ApplyTriggerSetters(TriggerBase trigger)
    {
        SetterBaseCollection setters = GetTriggerSetters(trigger);
        if (setters is null)
        {
            return;
        }

        foreach (SetterBase setterBase in setters)
        {
            if (setterBase is Setter setter && setter.Property is not null)
            {
                // Resolve the target element
                DependencyObject target = ResolveNamedElement(setter.TargetName);
                if (target is null)
                {
                    continue;
                }

                object value = setter.ValueInternal;

                // Handle BindingBase values
                if (value is BindingBase bindingBase)
                {
                    value = bindingBase.CreateBindingExpression(target, setter.Property, null);
                }
                // Handle DynamicResourceExtension
                else if (value is DynamicResourceExtension dynamicResource)
                {
                    value = new ResourceReferenceExpression(dynamicResource.ResourceKey ??
                        throw new InvalidOperationException(Strings.MarkupExtensionResourceKey));
                }

                // Use ParentTemplateTrigger precedence - can override local values
                target.SetParentTemplateTriggerValue(setter.Property, value);
            }
        }
    }

    private void UnapplyTriggerSetters(TriggerBase trigger)
    {
        SetterBaseCollection setters = GetTriggerSetters(trigger);
        if (setters is null)
        {
            return;
        }

        foreach (SetterBase setterBase in setters)
        {
            if (setterBase is Setter setter && setter.Property is not null)
            {
                DependencyObject target = ResolveNamedElement(setter.TargetName);
                if (target is null)
                {
                    continue;
                }

                target.ClearParentTemplateTriggerValue(setter.Property);
            }
        }
    }

    private static SetterBaseCollection GetTriggerSetters(TriggerBase trigger)
    {
        return trigger switch
        {
            Trigger t => t.HasSetters ? t.Setters : null,
            DataTrigger dt => dt.HasSetters ? dt.Setters : null,
            MultiTrigger mt => mt.HasSetters ? mt.Setters : null,
            MultiDataTrigger mdt => mdt.HasSetters ? mdt.Setters : null,
            _ => null
        };
    }

    private void InvokeEnterActions(TriggerBase trigger)
    {
        if (!trigger.HasEnterActions)
        {
            return;
        }

        foreach (TriggerAction action in trigger.EnterActions)
        {
            if (action is BeginStoryboard beginStoryboard)
            {
                beginStoryboard.Storyboard?.Begin(_templatedParent);
            }
        }
    }

    private void InvokeExitActions(TriggerBase trigger)
    {
        if (!trigger.HasExitActions)
        {
            return;
        }

        foreach (TriggerAction action in trigger.ExitActions)
        {
            if (action is BeginStoryboard beginStoryboard)
            {
                beginStoryboard.Storyboard?.Begin(_templatedParent);
            }
        }
    }

    private void SetupStoryboardNameResolver(Storyboard storyboard)
    {
        // Create a name resolver that uses the template's name scope
        if (_templatedParent.TemplateChild is IInternalFrameworkElement templateRoot)
        {
            var nameResolver = new TemplateNameResolver(templateRoot);
            SetNameResolverRecursive(storyboard, nameResolver);
        }
    }

    private static void SetNameResolverRecursive(Timeline timeline, INameResolver nameResolver)
    {
        timeline.NameResolver = nameResolver;
        
        if (timeline is Storyboard storyboard)
        {
            foreach (Timeline child in storyboard.Children)
            {
                SetNameResolverRecursive(child, nameResolver);
            }
        }
    }

    internal void Cleanup()
    {
        // Unapply all active triggers
        if (_activeTriggers is not null)
        {
            foreach (TriggerBase trigger in _activeTriggers)
            {
                UnapplyTriggerSetters(trigger);
            }
            _activeTriggers.Clear();
        }

        // Remove event trigger handlers
        if (_eventTriggerHandlers is not null)
        {
            foreach (var kvp in _eventTriggerHandlers)
            {
                if (kvp.Key.RoutedEvent is not null)
                {
                    kvp.Value.Source.RemoveHandler(kvp.Key.RoutedEvent, kvp.Value.Handler);
                }
            }
            _eventTriggerHandlers.Clear();
        }

        _propertyTriggerMap?.Clear();
        _dataTriggerHelpers?.Clear();
        _sourceElementTriggerMap?.Clear();
        _triggerSourceElements?.Clear();
    }

    /// <summary>
    /// Helper class to receive data trigger binding values.
    /// </summary>
    /// <remarks>
    /// Memory consideration: FrameworkElement has significant overhead (~500+ bytes per instance).
    /// This is required because the binding system uses FrameworkElement.FindMentor for DataContext
    /// resolution. A lighter-weight approach would require changes to the binding infrastructure.
    /// </remarks>
    private sealed class DataTriggerBindingHelper : FrameworkElement
    {
        private readonly TemplateTriggerStorage _storage;
        private readonly TriggerBase _trigger;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(object),
                typeof(DataTriggerBindingHelper),
                new PropertyMetadata(null, OnValueChanged));

        public DataTriggerBindingHelper(TemplateTriggerStorage storage, TriggerBase trigger, FrameworkElement element)
        {
            _storage = storage;
            _trigger = trigger;

            var dcBinding = new Binding
            {
                Source = element,
                Path = new PropertyPath(nameof(DataContext)),
                Mode = BindingMode.OneWay
            };
            SetBinding(DataContextProperty, dcBinding);
        }

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValueInternal(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var helper = (DataTriggerBindingHelper)d;
            helper._storage.OnDataTriggerValueChanged(helper._trigger);
        }
    }
}
