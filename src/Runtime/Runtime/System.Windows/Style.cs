
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
using System.Diagnostics;
using System.Windows.Markup;
using System.Xaml.Markup;
using PropertyValue = (int PropertyIndex, object ValueInternal);

namespace System.Windows;

/// <summary>
/// Contains property setters that can be shared between instances of a type.
/// </summary>
[DictionaryKeyProperty(nameof(TargetType))]
[ContentProperty(nameof(Setters))]
public class Style : DependencyObject //was sealed but we unsealed it because telerik has xaml files with styles as their roots (and the file we generate from xaml files create a type that inherits the type of the root of the xaml).
{
    private bool _sealed;
    private SetterBaseCollection _setters;
    private Type _targetType;
    private Style _basedOn;

    // Style tables (includes based-on data)
    internal new Dictionary<int, object> EffectiveValues { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Style"/> class.
    /// </summary>
    public Style()
    {
        // Note: In WPF, Style inherits from DispatcherObject rather than DependencyObject.
        // For this reason we explicitly set the two properties below to ensure that a Style
        // has no inherited context, and can't provide itself as inherited context.
        CanBeInheritanceContext = false;
        IsInheritanceContextSealed = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Style"/> class to use on the specified <see cref="Type"/>.
    /// </summary>
    /// <param name="targetType">
    /// The type to which the style will apply.
    /// </param>
    public Style(Type targetType) : this()
    {
        TargetType = targetType;
    }

    /// <summary>
    /// Gets a value that indicates whether the style is read-only and cannot be changed.
    /// </summary>
    /// <returns>
    /// true if the style is read-only; otherwise, false.
    /// </returns>
    public bool IsSealed => _sealed;

    /// <summary>
    /// Gets or sets the type for which the style is intended.
    /// </summary>
    /// <returns>
    /// The type of object to which the style is applied.
    /// </returns>
    [Ambient]
    public Type TargetType
    {
        get => _targetType;
        set
        {
            if (_sealed)
            {
                throw new InvalidOperationException("Cannot modify a 'Style' after it is sealed.");
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _targetType = value;
        }
    }

    /// <summary>
    /// Gets or sets a defined style that is the basis of the current style.
    /// </summary>
    /// <returns>
    /// A defined style that is the basis of the current style. The default value is null.
    /// </returns>
    public Style BasedOn
    {
        get => _basedOn;
        set
        {
            if (_sealed)
            {
                throw new InvalidOperationException("Cannot modify a 'Style' after it is sealed.");
            }

            if (value == this)
            {
                // Basing on self is not allowed.  This is a degenerate case
                // of circular reference chain, the full check for circular
                // reference is done in Seal().
                throw new ArgumentException("A Style cannot be based on itself.");
            }

            _basedOn = value;
        }
    }

    /// <summary>
    /// Gets a collection of <see cref="Setter"/> objects.
    /// </summary>
    /// <returns>
    /// A collection of <see cref="Setter"/> objects. The default is an empty collection.
    /// </returns>
    public SetterBaseCollection Setters
    {
        get
        {
            if (_setters == null)
            {
                _setters = new SetterBaseCollection();

                // If the style has been sealed prior to this the newly
                // created SetterBaseCollection also needs to be sealed
                if (_sealed)
                {
                    _setters.Seal();
                }
            }
            return _setters;
        }
    }

    /// <summary>
    /// Locks the style so that the <see cref="TargetType"/> property or any <see cref="Setter"/>
    /// in the <see cref="Setters"/> collection cannot be changed.
    /// </summary>
    public void Seal()
    {
        // 99% case - Style is already sealed.
        if (_sealed)
        {
            return;
        }

        // Most parameter checking is done as "upstream" as possible, but some
        //  can't be checked until Style is sealed.
        if (_targetType == null)
        {
            throw new InvalidOperationException("Must have non-null value for 'TargetType'.");
        }

        if (_basedOn != null)
        {
            if (_basedOn.TargetType == null || !_basedOn.TargetType.IsAssignableFrom(_targetType))
            {
                throw new InvalidOperationException($"Can only base on a Style with target type that is base type '{_targetType.Name}'.");
            }
        }

        // Will throw InvalidOperationException if we find a loop of
        // BasedOn references.  (A.BasedOn = B, B.BasedOn = C, C.BasedOn = A)
        CheckForCircularBasedOnReferences();

        // Seal BasedOn Style chain
        _basedOn?.Seal();

        // Seal setters
        _setters?.Seal();

        //
        // Build shared tables
        //

        // Process all PropertyValues (all are "Self") in the Style
        // chain (base added first)
        ProcessSelfStyles();

        // All done, seal self and call it a day.
        _sealed = true;
    }

    internal void CheckTargetType(object element)
    {
        if (TargetType == null)
        {
            throw new InvalidOperationException("Must have non-null value for TargetType.");
        }

        Type elementType = element.GetType();
        if (!TargetType.IsAssignableFrom(elementType))
        {
            throw new InvalidOperationException(
                $"'{TargetType.Name}' TargetType does not match type of element '{elementType.Name}'.");
        }
    }

    /// <summary>
    /// This method checks to see if the BasedOn hierarchy contains
    /// a loop in the chain of references.
    /// </summary>
    /// <remarks>
    /// Classic "when did we enter the cycle" problem where we don't know
    /// what to start remembering and what to check against.  Brute-
    /// force approach here is to remember everything with a stack
    /// and do a linear comparison through everything.  Since the Style
    /// BasedOn hierarchy is not expected to be large, this should be OK.
    /// </remarks>
    private void CheckForCircularBasedOnReferences()
    {
        if (HasCircularBasedOnReferences(this))
        {
            // Uh-oh.  We've seen this Style before.  This means
            //  the BasedOn hierarchy contains a loop.
            throw new InvalidOperationException("This Style's hierarchy of BasedOn references contains a loop.");
        }

        // This does not really check for circular reference in all circumstances. This is accurate
        // only if the basedOn styles have no circular references. In our case, it is safe because we
        // seal basedOn styles first.
        static bool HasCircularBasedOnReferences(Style s)
        {
            for (Style basedOn = s._basedOn; basedOn is not null; basedOn = basedOn._basedOn)
            {
                if (basedOn == s)
                {
                    return true;
                }
            }

            return false;
        }
    }

    private void ProcessSelfStyles()
    {
        if (Setters.Count == 0)
        {
            EffectiveValues = _basedOn?.EffectiveValues ?? new(0);
            return;
        }

        // Process all Setters set on the selfStyle. This stores all the property
        // setters on the current styles into PropertyValues list, so it can be used
        // by ProcessSelfStyle in the next step. The EventSetters for the current
        // and all the basedOn styles are merged into the EventHandlersStore on the
        // current style.
        (PropertyValue[] propertyValues, int length) = ProcessSetters();

        int initialCapacity = Math.Max(length, _basedOn?.EffectiveValues.Count ?? 0);
        Dictionary<int, object> effectiveValues = new(initialCapacity);

        // Walk down to bottom of based-on chain

        if (_basedOn is not null)
        {
            foreach (var propertyValue in _basedOn.EffectiveValues)
            {
                effectiveValues[propertyValue.Key] = propertyValue.Value;
            }
        }

        // Merge in "self" PropertyValues while walking back up the tree
        // "Based-on" style "self" rules are always added first (lower priority)
        for (int i = 0; i < length; i++)
        {
            PropertyValue propertyValue = propertyValues[i];
            effectiveValues[propertyValue.PropertyIndex] = propertyValue.ValueInternal;
        }

        EffectiveValues = effectiveValues;
    }

    // Iterates through the setters collection and adds the EventSetter information into
    // an EventHandlersStore for easy and fast retrieval during event routing. Also adds
    // an entry in the EventDependents list for EventhandlersStore holding the TargetType's
    // events.
    private (PropertyValue[] PropertyValues, int Length) ProcessSetters()
    {
        Debug.Assert(Setters.Count > 0);

        var setters = Setters.InternalItems;

        int length = 0;
        var propertyValues = new PropertyValue[setters.Count];

        foreach (var setterBase in setters)
        {
            // Setters are folded into the PropertyValues table only for the current style. The
            // processing of BasedOn Style properties will occur in subsequent call to ProcessSelfStyle
            if (setterBase is Setter setter)
            {
                UpdatePropertyValueList(setter.Property, setter.Value, propertyValues, ref length);
            }
        }

        return (propertyValues, length);

        // Given a set of values for the PropertyValue struct, put that in
        // to the PropertyValueList, overwriting any existing entry.
        static void UpdatePropertyValueList(DependencyProperty dp, object value, PropertyValue[] propertyValues, ref int length)
        {
            // Check for existing value on dp
            int existingIndex = -1;
            for (int i = 0; i < length; i++)
            {
                if (propertyValues[i].PropertyIndex == dp.GlobalIndex)
                {
                    existingIndex = i;
                    break;
                }
            }

            if (existingIndex >= 0)
            {
                // Overwrite existing value for dp
                ref PropertyValue propertyValue = ref propertyValues[existingIndex];
                propertyValue.ValueInternal = value;
            }
            else
            {
                // Store original data
                propertyValues[length++] = (dp.GlobalIndex, value);
            }
        }
    }
}
