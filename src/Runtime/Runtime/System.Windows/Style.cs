

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
using System.Diagnostics;
using System.Windows.Markup;
using System.Xaml.Markup;
using System.Windows.Data;

namespace System.Windows
{
    /// <summary>
    /// Contains property setters that can be shared between instances of a type.
    /// </summary>
    [DictionaryKeyProperty(nameof(TargetType))]
    [ContentProperty(nameof(Setters))]
    public partial class Style : DependencyObject //was sealed but we unsealed it because telerik has xaml files with styles as their roots (and the file we generate from xaml files create a type that inherits the type of the root of the xaml).
    {
        #region Data

        private bool _sealed;
        private SetterBaseCollection _setters;
        private Type _targetType;
        private Style _basedOn;
        private int _modified = 0;
        private const int TargetTypeID = 0x01;
        internal const int BasedOnID = 0x02;

        // Original Style data (not including based-on data)
        internal List<PropertyValue> PropertyValues
        {
            get;
            private set;
        }

        // Style tables (includes based-on data)
        internal Dictionary<DependencyProperty, object> EffectiveValues
        {
            get;
            private set;
        }

        #endregion Data

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Style class, with no initial TargetType and an empty Setters collection.
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
        /// Initializes a new instance of the Style class, with an initial TargetType and an empty Setters collection.
        /// </summary>
        /// <param name="targetType">The type on which the Style will be used.</param>
        public Style(Type targetType) : this()
        {
            TargetType = targetType;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        ///     Style mutability state
        /// </summary>
        /// <remarks>
        ///     A style is sealed when another style is basing on it, or,
        ///     when it's applied
        /// </remarks>
        public bool IsSealed
        {
            get { return _sealed; }
        }

        /// <summary>
        ///     Type that this style is intended
        /// </summary>
        /// <remarks>
        ///     By default, the target type is FrameworkElement
        /// </remarks>
        [Ambient]
        public Type TargetType
        {
            get
            {
                return _targetType;
            }

            set
            {
                if (_sealed)
                {
                    throw new InvalidOperationException(string.Format("Cannot modify a '{0}' after it is sealed.", "Style"));
                }

                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _targetType = value;

                SetModified(TargetTypeID);
            }
        }

        /// <summary>
        ///     Style to base on
        /// </summary>
        public Style BasedOn
        {
            get
            {
                return _basedOn;
            }
            set
            {
                if (_sealed)
                {
                    throw new InvalidOperationException(string.Format("Cannot modify a '{0}' after it is sealed.", "Style"));
                }

                if (value == this)
                {
                    // Basing on self is not allowed.  This is a degenerate case
                    //  of circular reference chain, the full check for circular
                    //  reference is done in Seal().
                    throw new ArgumentException("A Style cannot be based on itself.");
                }

                _basedOn = value;

                SetModified(BasedOnID);
            }
        }

        /// <summary>
        ///     The collection of property setters for the target type
        /// </summary>

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

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// This Style is now immutable
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
                throw new InvalidOperationException(string.Format("Must have non-null value for '{0}'.", "TargetType"));
            }

            if (_basedOn != null)
            {
                if (_basedOn.TargetType == null ||
                    !_basedOn.TargetType.IsAssignableFrom(_targetType))
                {
                    throw new InvalidOperationException(string.Format("Can only base on a Style with target type that is base type '{0}'.", _targetType.Name));
                }
            }

            // Seal setters
            if (_setters != null)
            {
                _setters.Seal();
            }

            // Will throw InvalidOperationException if we find a loop of
            //  BasedOn references.  (A.BasedOn = B, B.BasedOn = C, C.BasedOn = A)
            CheckForCircularBasedOnReferences();

            // Seal BasedOn Style chain
            if (_basedOn != null)
            {
                _basedOn.Seal();
            }

            //
            // Build shared tables
            //

            // Process all Setters set on the selfStyle. This stores all the property
            // setters on the current styles into PropertyValues list, so it can be used
            // by ProcessSelfStyle in the next step. The EventSetters for the current
            // and all the basedOn styles are merged into the EventHandlersStore on the
            // current style.
            ProcessSetters(this);

            // Process all PropertyValues (all are "Self") in the Style
            // chain (base added first)
            EffectiveValues = new Dictionary<DependencyProperty, object>();
            ProcessSelfStyles(this);

            // All done, seal self and call it a day.
            _sealed = true;
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        ///     Given a set of values for the PropertyValue struct, put that in
        /// to the PropertyValueList, overwriting any existing entry.
        /// </summary>
        private void UpdatePropertyValueList(DependencyProperty dp, object value)
        {
            // Check for existing value on dp
            int existingIndex = -1;
            for (int i = 0; i < PropertyValues.Count; i++)
            {
                if (PropertyValues[i].Property == dp)
                {
                    existingIndex = i;
                    break;
                }
            }

            if (existingIndex >= 0)
            {
                // Overwrite existing value for dp
                PropertyValue propertyValue = PropertyValues[existingIndex];
                propertyValue.ValueInternal = value;
                // Put back modified struct
                PropertyValues[existingIndex] = propertyValue;
            }
            else
            {
                // Store original data
                PropertyValue propertyValue = new PropertyValue();
                propertyValue.Property = dp;
                propertyValue.ValueInternal = value;

                PropertyValues.Add(propertyValue);
            }
        }

        internal void CheckTargetType(object element)
        {
            // Note: we do not do this verification in CSHTML5 in order to remain compatible
            // with old applications created with prior versions of CSHTML5 (eg. Client_FB).
            if (TargetType == null)
            {
                throw new InvalidOperationException("Must have non-null value for TargetType.");
            }

            Type elementType = element.GetType();
            if (!TargetType.IsAssignableFrom(elementType))
            {
                throw new InvalidOperationException(string.Format("'{0}' TargetType does not match type of element '{1}'.",
                                                    this.TargetType.Name,
                                                    elementType.Name));
            }
        }

        /// <summary>
        ///     This method checks to see if the BasedOn hierarchy contains
        /// a loop in the chain of references.
        /// </summary>
        /// <remarks>
        /// Classic "when did we enter the cycle" problem where we don't know
        ///  what to start remembering and what to check against.  Brute-
        ///  force approach here is to remember everything with a stack
        ///  and do a linear comparison through everything.  Since the Style
        ///  BasedOn hierarchy is not expected to be large, this should be OK.
        /// </remarks>
        private void CheckForCircularBasedOnReferences()
        {
            Stack<Style> basedOnHierarchy = new Stack<Style>(10);  // 10 because that's the default value (see MSDN) and the perf team wants us to specify something.
            Style latestBasedOn = this;

            while (latestBasedOn != null)
            {
                if (basedOnHierarchy.Contains(latestBasedOn))
                {
                    // Uh-oh.  We've seen this Style before.  This means
                    //  the BasedOn hierarchy contains a loop.
                    throw new InvalidOperationException("This Style's hierarchy of BasedOn references contains a loop.");

                    // Debugging note: If we stop here, the basedOnHierarchy
                    //  object is still alive and we can browse through it to
                    //  see what we've explored.  (This does not apply if
                    //  somebody catches this exception and re-throws.)
                }

                // Haven't seen it, push on stack and go to next level.
                basedOnHierarchy.Push(latestBasedOn);
                latestBasedOn = latestBasedOn.BasedOn;
            }

            return;
        }

        // Iterates through the setters collection and adds the EventSetter information into
        // an EventHandlersStore for easy and fast retrieval during event routing. Also adds
        // an entry in the EventDependents list for EventhandlersStore holding the TargetType's
        // events.
        private void ProcessSetters(Style style)
        {
            // Walk down to bottom of based-on chain
            if (style == null)
            {
                return;
            }

            style.Setters.Seal(); // Does not mark individual setters as sealed, that's up to the loop below.

            if (style == this)
            {
                // On-demand create the PropertyValues list, so that we can specify the right size.
                if (PropertyValues == null || PropertyValues.Count == 0)
                {
                    PropertyValues = new List<PropertyValue>(style.Setters.Count);
                }

                for (int i = 0; i < style.Setters.Count; i++)
                {
                    SetterBase setterBase = style.Setters[i];
                    Debug.Assert(setterBase != null, "Setter collection must contain non-null instances of SetterBase");

                    // Setters are folded into the PropertyValues table only for the current style. The
                    // processing of BasedOn Style properties will occur in subsequent call to ProcessSelfStyle
                    Setter setter = setterBase as Setter;
                    if (setter != null)
                    {
                        UpdatePropertyValueList(setter.Property, setter.Value);
                    }
                }
            }
            
            ProcessSetters(style._basedOn);
        }

        private void ProcessSelfStyles(Style style)
        {
            // Walk down to bottom of based-on chain
            if (style == null)
            {
                return;
            }

            ProcessSelfStyles(style._basedOn);

            // Merge in "self" PropertyValues while walking back up the tree
            // "Based-on" style "self" rules are always added first (lower priority)
            for (int i = 0; i < style.PropertyValues.Count; i++)
            {
                PropertyValue propertyValue = style.PropertyValues[i];
                EffectiveValues[propertyValue.Property] = propertyValue.ValueInternal;
            }
        }

        private void SetModified(int id) { _modified |= id; }
        internal bool IsModified(int id) { return (id & _modified) != 0; }

        #endregion Internal Methods
    }

    internal struct PropertyValue
    {
        internal DependencyProperty Property;
        internal object ValueInternal;
    }
}
