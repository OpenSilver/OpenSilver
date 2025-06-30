// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows.Data;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Specifies the types of framework-level property behavior that pertain to a particular
/// dependency property.
/// </summary>
[Flags]
public enum FrameworkPropertyMetadataOptions : int
{
    /// <summary>
    /// No options are specified; the dependency property uses the default behavior of the WPF property system.
    /// </summary>
    None = 0x000,

    /// <summary>The measure pass of layout compositions is affected by value changes to this dependency property.</summary>
    AffectsMeasure = 0x001,

    /// <summary>
    /// The arrange pass of layout composition is affected by value changes to this dependency property.
    /// </summary>
    AffectsArrange = 0x002,

    /// <summary>
    /// The measure pass on the parent element is affected by value changes to this dependency property.
    /// </summary>
    AffectsParentMeasure = 0x004,

    /// <summary>
    /// The arrange pass on the parent element is affected by value changes to this dependency property.
    /// </summary>
    AffectsParentArrange = 0x008,

    /// <summary>
    /// Some aspect of rendering or layout composition (other than measure or arrange) is affected by value changes to this dependency property.
    /// </summary>
    AffectsRender = 0x010,

    /// <summary>
    /// The values of this dependency property are inherited by child elements.
    /// </summary>
    Inherits = 0x020,

    /// <summary>
    /// The values of this dependency property span separated trees for purposes of property value inheritance.
    /// </summary>
    [OpenSilver.NotImplemented]
    OverridesInheritanceBehavior = 0x040,

    /// <summary>
    /// Data binding to this dependency property is not allowed.
    /// </summary>
    NotDataBindable = 0x080,

    /// <summary>
    /// The <see cref="BindingMode"/> for data bindings on this dependency property defaults to <see cref="BindingMode.TwoWay"/>.
    /// </summary>
    BindsTwoWayByDefault = 0x100,

    /// <summary>
    /// The values of this dependency property should be saved or restored by journaling processes, or when navigating by Uniform resource identifiers (URIs).
    /// </summary>
    [OpenSilver.NotImplemented]
    Journal = 0x400,

    /// <summary>
    /// The subproperties on the value of this dependency property do not affect any aspect of rendering.
    /// </summary>
    [OpenSilver.NotImplemented]
    SubPropertiesDoNotAffectRender = 0x800,
}

/// <summary>
/// Reports or applies metadata for a dependency property, specifically adding framework-specific
/// property system characteristics.
/// </summary>
public class FrameworkPropertyMetadata : UIPropertyMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FrameworkPropertyMetadata"/> class.
    /// </summary>
    public FrameworkPropertyMetadata()
        : base()
    {
        Initialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameworkPropertyMetadata"/> class
    /// with the specified default value.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value of the dependency property, usually provided as a value of
    /// a specific type.
    /// </param>
    /// <exception cref="ArgumentException">
    /// defaultValue is set to <see cref="DependencyProperty.UnsetValue"/>.
    /// </exception>
    public FrameworkPropertyMetadata(object defaultValue)
        : base(defaultValue)
    {
        Initialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameworkPropertyMetadata"/> class
    /// with the specified <see cref="PropertyChangedCallback"/> callback.
    /// </summary>
    /// <param name="propertyChangedCallback">
    /// A reference to a handler implementation that the property system will call whenever
    /// the effective value of the property changes.
    /// </param>
    public FrameworkPropertyMetadata(PropertyChangedCallback propertyChangedCallback)
        : base(propertyChangedCallback)
    {
        Initialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameworkPropertyMetadata"/> class
    /// with the specified callbacks.
    /// </summary>
    /// <param name="propertyChangedCallback">
    /// A reference to a handler implementation that the property system will call whenever
    /// the effective value of the property changes.
    /// </param>
    /// <param name="coerceValueCallback">
    /// A reference to a handler implementation will be called whenever the property
    /// system calls <see cref="DependencyObject.CoerceValue(DependencyProperty)"/>
    /// for this dependency property.
    /// </param>
    public FrameworkPropertyMetadata(PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
        : base(propertyChangedCallback)
    {
        Initialize();
        CoerceValueCallback = coerceValueCallback;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameworkPropertyMetadata"/> class
    /// with the provided default value and specified <see cref="PropertyChangedCallback"/>
    /// callback.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value of the dependency property, usually provided as a value of
    /// a specific type.
    /// </param>
    /// <param name="propertyChangedCallback">
    /// A reference to a handler implementation that the property system will call whenever
    /// the effective value of the property changes.
    /// </param>
    /// <exception cref="ArgumentException">
    /// defaultValue is set to <see cref="DependencyProperty.UnsetValue"/>.
    /// </exception>
    public FrameworkPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback)
        : base(defaultValue, propertyChangedCallback)
    {
        Initialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameworkPropertyMetadata"/> class
    /// with the provided default value and specified callbacks.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value of the dependency property, usually provided as a specific type.
    /// </param>
    /// <param name="propertyChangedCallback">
    /// A reference to a handler implementation that the property system will call whenever
    /// the effective value of the property changes.
    /// </param>
    /// <param name="coerceValueCallback">
    /// A reference to a handler implementation that will be called whenever the property
    /// system calls <see cref="DependencyObject.CoerceValue(DependencyProperty)"/>
    /// for this dependency property.
    /// </param>
    /// <exception cref="ArgumentException">
    /// defaultValue is set to <see cref="DependencyProperty.UnsetValue"/>.
    /// </exception>
    public FrameworkPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
        : base(defaultValue, propertyChangedCallback, coerceValueCallback)
    {
        Initialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameworkPropertyMetadata"/> class
    /// with the provided default value and framework-level metadata options.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value of the dependency property, usually provided as a value of
    /// a specific type.
    /// </param>
    /// <param name="flags">
    /// The metadata option flags (a combination of <see cref="FrameworkPropertyMetadataOptions"/>
    /// values). These options specify characteristics of the dependency property that
    /// interact with systems such as layout or data binding.
    /// </param>
    /// <exception cref="ArgumentException">
    /// defaultValue is set to <see cref="DependencyProperty.UnsetValue"/>.
    /// </exception>
    public FrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags)
        : base(defaultValue)
    {
        TranslateFlags(flags);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameworkPropertyMetadata"/> class
    /// with the provided default value and framework metadata options, and specified
    /// <see cref="PropertyChangedCallback"/> callback.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value of the dependency property, usually provided as a value of
    /// a specific type.
    /// </param>
    /// <param name="flags">
    /// The metadata option flags (a combination of <see cref="FrameworkPropertyMetadataOptions"/>
    /// values). These options specify characteristics of the dependency property that 
    /// interact with systems such as layout or data binding.
    /// </param>
    /// <param name="propertyChangedCallback">
    /// A reference to a handler implementation that the property system will call whenever
    /// the effective value of the property changes.
    /// </param>
    /// <exception cref="ArgumentException">
    /// defaultValue is set to <see cref="DependencyProperty.UnsetValue"/>.
    /// </exception>
    public FrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback)
        : base(defaultValue, propertyChangedCallback)
    {
        TranslateFlags(flags);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameworkPropertyMetadata"/> class
    /// with the provided default value and framework metadata options, and specified
    /// callbacks.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value of the dependency property, usually provided as a specific type.
    /// </param>
    /// <param name="flags">
    /// The metadata option flags (a combination of <see cref="FrameworkPropertyMetadataOptions"/>
    /// values). These options specify characteristics of the dependency property that
    /// interact with systems such as layout or data binding.
    /// </param>
    /// <param name="propertyChangedCallback">
    /// A reference to a handler implementation that the property system will call whenever
    /// the effective value of the property changes.
    /// </param>
    /// <param name="coerceValueCallback">
    /// A reference to a handler implementation that will be called whenever the property
    /// system calls <see cref="DependencyObject.CoerceValue(DependencyProperty)"/>
    /// against this property.
    /// </param>
    /// <exception cref="ArgumentException">
    /// defaultValue is set to <see cref="DependencyProperty.UnsetValue"/>.
    /// </exception>
    public FrameworkPropertyMetadata(
        object defaultValue,
        FrameworkPropertyMetadataOptions flags,
        PropertyChangedCallback propertyChangedCallback,
        CoerceValueCallback coerceValueCallback)
        : base(defaultValue, propertyChangedCallback, coerceValueCallback)
    {
        TranslateFlags(flags);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameworkPropertyMetadata"/> class with the provided default 
    /// value and framework metadata options, specified callbacks, and a Boolean that can be used to prevent 
    /// animation of the property.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value of the dependency property, usually provided as a specific type.
    /// </param>
    /// <param name="flags">
    /// The metadata option flags (a combination of <see cref="FrameworkPropertyMetadataOptions"/> values). These 
    /// options specify characteristics of the dependency property that interact with systems such as layout or 
    /// data binding.
    /// </param>
    /// <param name="propertyChangedCallback">
    /// A reference to a handler implementation that the property system will call whenever the effective value of 
    /// the property changes.
    /// </param>
    /// <param name="coerceValueCallback">
    /// A reference to a handler implementation that will be called whenever the property system calls 
    /// <see cref="DependencyObject.CoerceValue(DependencyProperty)"/> on this dependency property.
    /// </param>
    /// <param name="isAnimationProhibited">
    /// true to prevent the property system from animating the property that this metadata is applied to. Such 
    /// properties will raise a run-time exception originating from the property system if animations of them are 
    /// attempted. false to permit animating the property. The default is false.
    /// </param>
    /// <exception cref="ArgumentException">
    /// defaultValue is set to <see cref="DependencyProperty.UnsetValue"/>.
    /// </exception>
    public FrameworkPropertyMetadata(
        object defaultValue,
        FrameworkPropertyMetadataOptions flags,
        PropertyChangedCallback propertyChangedCallback,
        CoerceValueCallback coerceValueCallback,
        bool isAnimationProhibited)
        : base(defaultValue, propertyChangedCallback, coerceValueCallback, isAnimationProhibited)
    {
        TranslateFlags(flags);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameworkPropertyMetadata"/> class with the provided 
    /// default value and framework metadata options, specified callbacks, and a data-binding update trigger
    /// default.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value of the dependency property, usually provided as a specific type.
    /// </param>
    /// <param name="flags">
    /// The metadata option flags (a combination of <see cref="FrameworkPropertyMetadataOptions"/>
    /// values). These options specify characteristics of the dependency property that interact with 
    /// systems such as layout or data binding.
    /// </param>
    /// <param name="propertyChangedCallback">
    /// A reference to a handler implementation that the property system will call whenever the effective 
    /// value of the property changes.
    /// </param>
    /// <param name="coerceValueCallback">
    /// A reference to a handler implementation that will be called whenever the property system calls 
    /// <see cref="DependencyObject.CoerceValue(DependencyProperty)"/> against this property.
    /// </param>
    /// <param name="defaultUpdateSourceTrigger">
    /// The <see cref="UpdateSourceTrigger"/> to use when bindings for this property are applied that have 
    /// their <see cref="UpdateSourceTrigger"/> set to <see cref="UpdateSourceTrigger.Default"/>.
    /// </param>
    /// <exception cref="ArgumentException">
    /// defaultValue is set to <see cref="DependencyProperty.UnsetValue"/>.
    /// </exception>
    public FrameworkPropertyMetadata(
        object defaultValue,
        FrameworkPropertyMetadataOptions flags,
        PropertyChangedCallback propertyChangedCallback,
        CoerceValueCallback coerceValueCallback,
        UpdateSourceTrigger defaultUpdateSourceTrigger)
        : this(defaultValue, flags, propertyChangedCallback, coerceValueCallback, false, defaultUpdateSourceTrigger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameworkPropertyMetadata"/> class with the provided 
    /// default value and framework metadata options, specified callbacks, a Boolean that can be used to 
    /// prevent animation of the property, and a data-binding update trigger default.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value of the dependency property, usually provided as a specific type.
    /// </param>
    /// <param name="flags">
    /// The metadata option flags (a combination of <see cref="FrameworkPropertyMetadataOptions"/> values). 
    /// These options specify characteristics of the dependency property that interact with systems such as 
    /// layout or data binding.
    /// </param>
    /// <param name="propertyChangedCallback">
    /// A reference to a handler implementation that the property system will call whenever the effective 
    /// value of the property changes.
    /// </param>
    /// <param name="coerceValueCallback">
    /// A reference to a handler implementation that will be called whenever the property system calls 
    /// <see cref="DependencyObject.CoerceValue(DependencyProperty)"/> against this property.
    /// </param>
    /// <param name="isAnimationProhibited">
    /// true to prevent the property system from animating the property that this metadata is applied to. 
    /// Such properties will raise a run-time exception originating from the property system if animations 
    /// of them are attempted. The default is false.
    /// </param>
    /// <param name="defaultUpdateSourceTrigger">
    /// The <see cref="UpdateSourceTrigger"/> to use when bindings for this property are applied that have
    /// their <see cref="UpdateSourceTrigger"/> set to <see cref="UpdateSourceTrigger.Default"/>.
    /// </param>
    /// <exception cref="ArgumentException">
    /// defaultValue is set to <see cref="DependencyProperty.UnsetValue"/>.
    /// </exception>
    public FrameworkPropertyMetadata(
        object defaultValue,
        FrameworkPropertyMetadataOptions flags,
        PropertyChangedCallback propertyChangedCallback,
        CoerceValueCallback coerceValueCallback,
        bool isAnimationProhibited,
        UpdateSourceTrigger defaultUpdateSourceTrigger)
        : base(defaultValue, propertyChangedCallback, coerceValueCallback, isAnimationProhibited)
    {
        if (!IsValidUpdateSourceTrigger(defaultUpdateSourceTrigger))
        {
            throw new InvalidEnumArgumentException(nameof(defaultUpdateSourceTrigger), (int)defaultUpdateSourceTrigger, typeof(UpdateSourceTrigger));
        }

        if (defaultUpdateSourceTrigger == UpdateSourceTrigger.Default)
        {
            throw new ArgumentException(Strings.NoDefaultUpdateSourceTrigger, nameof(defaultUpdateSourceTrigger));
        }

        TranslateFlags(flags);
        DefaultUpdateSourceTrigger = defaultUpdateSourceTrigger;
    }

    /// <summary>
    /// Gets or sets a value that indicates whether a dependency property potentially
    /// affects the measure pass during layout engine operations.
    /// </summary>
    /// <returns>
    /// true if the dependency property on which this metadata exists potentially affects
    /// the measure pass; otherwise, false. The default is false.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The metadata has already been applied to a dependency property operation, so
    /// that metadata is sealed and properties of the metadata cannot be set.
    /// </exception>
    public bool AffectsMeasure
    {
        get { return ReadFlag(MetadataFlags.FW_AffectsMeasureID); }
        set { CheckSealed(); WriteFlag(MetadataFlags.FW_AffectsMeasureID, value); }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether a dependency property potentially
    /// affects the arrange pass during layout engine operations.
    /// </summary>
    /// <returns>
    /// true if the dependency property on which this metadata exists potentially affects
    /// the arrange pass; otherwise, false. The default is false.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The metadata has already been applied to a dependency property operation, so
    /// that metadata is sealed and properties of the metadata cannot be set.
    /// </exception>
    public bool AffectsArrange
    {
        get { return ReadFlag(MetadataFlags.FW_AffectsArrangeID); }
        set { CheckSealed(); WriteFlag(MetadataFlags.FW_AffectsArrangeID, value); }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether a dependency property potentially
    /// affects the measure pass of its parent element's layout during layout engine
    /// operations.
    /// </summary>
    /// <returns>
    /// true if the dependency property on which this metadata exists potentially affects
    /// the measure pass specifically on its parent element; otherwise, false.The default
    /// is false.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The metadata has already been applied to a dependency property operation, so
    /// that metadata is sealed and properties of the metadata cannot be set.
    /// </exception>
    public bool AffectsParentMeasure
    {
        get { return ReadFlag(MetadataFlags.FW_AffectsParentMeasureID); }
        set { CheckSealed(); WriteFlag(MetadataFlags.FW_AffectsParentMeasureID, value); }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether a dependency property potentially
    /// affects the arrange pass of its parent element's layout during layout engine
    /// operations.
    /// </summary>
    /// <returns>
    /// true if the dependency property on which this metadata exists potentially affects
    /// the arrange pass specifically on its parent element; otherwise, false. The default
    /// is false.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The metadata has already been applied to a dependency property operation, so
    /// that metadata is sealed and properties of the metadata cannot be set.
    /// </exception>
    public bool AffectsParentArrange
    {
        get { return ReadFlag(MetadataFlags.FW_AffectsParentArrangeID); }
        set { CheckSealed(); WriteFlag(MetadataFlags.FW_AffectsParentArrangeID, value); }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether a dependency property potentially
    /// affects the general layout in some way that does not specifically influence arrangement
    /// or measurement, but would require a redraw.
    /// </summary>
    /// <returns>
    /// true if the dependency property on which this metadata exists affects rendering;
    /// otherwise, false. The default is false.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The metadata has already been applied to a dependency property operation, so
    /// that metadata is sealed and properties of the metadata cannot be set.
    /// </exception>
    public bool AffectsRender
    {
        get { return ReadFlag(MetadataFlags.FW_AffectsRenderID); }
        set { CheckSealed(); WriteFlag(MetadataFlags.FW_AffectsRenderID, value); }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the property value inheritance evaluation 
    /// should span across certain content boundaries in the logical tree of elements.
    /// </summary>
    /// <returns>
    /// true if the property value inheritance should span across certain content boundaries; 
    /// otherwise, false. The default is false.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The metadata has already been applied to a dependency property operation, so that metadata
    /// is sealed and properties of the metadata cannot be set.
    /// </exception>
    [OpenSilver.NotImplemented]
    public bool OverridesInheritanceBehavior
    {
        get { return ReadFlag(MetadataFlags.FW_OverridesInheritanceBehaviorID); }
        set
        {
            CheckSealed();
            WriteFlag(MetadataFlags.FW_OverridesInheritanceBehaviorID, value);
            SetModified(MetadataFlags.FW_OverridesInheritanceBehaviorModifiedID);
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the dependency property supports data binding.
    /// </summary>
    /// <returns>
    /// true if the property does not support data binding; otherwise, false. The default is false.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The metadata has already been applied to a dependency property operation, so that metadata 
    /// is sealed and properties of the metadata cannot be set.
    /// </exception>
    public bool IsNotDataBindable
    {
        get { return ReadFlag(MetadataFlags.FW_IsNotDataBindableID); }
        set { CheckSealed(); WriteFlag(MetadataFlags.FW_IsNotDataBindableID, value); }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the property binds two-way by default.
    /// </summary>
    /// <returns>
    /// true if the dependency property on which this metadata exists binds two-way by default;
    /// otherwise, false. The default is false.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The metadata has already been applied to a dependency property operation, so that metadata
    /// is sealed and properties of the metadata cannot be set.
    /// </exception>
    public bool BindsTwoWayByDefault
    {
        get { return ReadFlag(MetadataFlags.FW_BindsTwoWayByDefaultID); }
        set { CheckSealed(); WriteFlag(MetadataFlags.FW_BindsTwoWayByDefaultID, value); }
    }

    /// <summary>
    /// Gets or sets the default for <see cref="UpdateSourceTrigger"/> to use when bindings for the 
    /// property with this metadata are applied, which have their <see cref="UpdateSourceTrigger"/>
    /// set to <see cref="UpdateSourceTrigger.Default"/>.
    /// </summary>
    /// <returns>
    /// A value of the enumeration, other than <see cref="UpdateSourceTrigger.Default"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// This property is set to <see cref="UpdateSourceTrigger.Default"/>; the value you set is 
    /// supposed to become the default when requested by bindings.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The metadata has already been applied to a dependency property operation, so that metadata 
    /// is sealed and properties of the metadata cannot be set.
    /// </exception>
    public UpdateSourceTrigger DefaultUpdateSourceTrigger
    {
        // FW_DefaultUpdateSourceTriggerEnumBit1        = 0x40000000,
        // FW_DefaultUpdateSourceTriggerEnumBit2        = 0x80000000,
        get { return (UpdateSourceTrigger)(((uint)_flags >> 30) & 0x3); }
        set
        {
            CheckSealed();
            if (!IsValidUpdateSourceTrigger(value))
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(UpdateSourceTrigger));
            }
            if (value == UpdateSourceTrigger.Default)
            {
                throw new ArgumentException(Strings.NoDefaultUpdateSourceTrigger, nameof(value));
            }
            // FW_DefaultUpdateSourceTriggerEnumBit1        = 0x40000000,
            // FW_DefaultUpdateSourceTriggerEnumBit2        = 0x80000000,
            _flags = (MetadataFlags)(((uint)_flags & 0x3FFFFFFF) | ((uint)value) << 30);
            SetModified(MetadataFlags.FW_DefaultUpdateSourceTriggerModifiedID);
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether this property contains journaling information 
    /// that applications can or should store as part of a journaling implementation.
    /// </summary>
    /// <returns>
    /// true if journaling should be performed on the dependency property that this metadata is 
    /// applied to; otherwise, false. The default is false.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The metadata has already been applied to a dependency property operation, so that metadata 
    /// is sealed and properties of the metadata cannot be set.
    /// </exception>
    [OpenSilver.NotImplemented]
    public bool Journal
    {
        get { return ReadFlag(MetadataFlags.FW_ShouldBeJournaledID); }
        set
        {
            CheckSealed();
            WriteFlag(MetadataFlags.FW_ShouldBeJournaledID, value);
            SetModified(MetadataFlags.FW_ShouldBeJournaledModifiedID);
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether sub-properties of the dependency property do 
    /// not affect the rendering of the containing object.
    /// </summary>
    /// <returns>
    /// true if changes to sub-property values do not affect rendering if changed; otherwise, false. 
    /// The default is false.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The metadata has already been applied to a dependency property operation, so that metadata 
    /// is sealed and properties of the metadata cannot be set.
    /// </exception>
    [OpenSilver.NotImplemented]
    public bool SubPropertiesDoNotAffectRender
    {
        get { return ReadFlag(MetadataFlags.FW_SubPropertiesDoNotAffectRenderID); }
        set
        {
            CheckSealed();
            WriteFlag(MetadataFlags.FW_SubPropertiesDoNotAffectRenderID, value);
            SetModified(MetadataFlags.FW_SubPropertiesDoNotAffectRenderModifiedID);
        }
    }

    /// <summary>
    /// Gets a value that indicates whether data binding is supported for the dependency property.
    /// </summary>
    /// <returns>
    /// true if data binding is supported on the dependency property to which this metadata applies;
    /// otherwise, false. The default is true.
    /// </returns>
    public bool IsDataBindingAllowed
    {
        get { return !ReadFlag(MetadataFlags.FW_IsNotDataBindableID) && !ReadOnly; }
    }

    /// <summary>
    /// Does the represent the metadata for a ReadOnly property
    /// </summary>
    private bool ReadOnly
    {
        get { return ReadFlag(MetadataFlags.FW_ReadOnlyID); }
        set { CheckSealed(); WriteFlag(MetadataFlags.FW_ReadOnlyID, value); }
    }

    /// <summary>
    /// Creates a new instance of this property metadata.  This method is used
    /// when metadata needs to be cloned.  After CreateInstance is called the
    /// framework will call Merge to merge metadata into the new instance.
    /// Deriving classes must override this and return a new instance of
    /// themselves.
    /// </summary>
    internal override PropertyMetadata CreateInstance()
    {
        return new FrameworkPropertyMetadata();
    }

    /// <summary>
    /// Enables a merge of the source metadata with base metadata.
    /// </summary>
    /// <param name="baseMetadata">
    /// The base metadata to merge.
    /// </param>
    /// <param name="dp">
    /// The dependency property this metadata is being applied to.
    /// </param>
    protected override void Merge(PropertyMetadata baseMetadata, DependencyProperty dp)
    {
        // Does parameter validation
        base.Merge(baseMetadata, dp);

        // Source type is guaranteed to be the same type or base type
        if (baseMetadata is FrameworkPropertyMetadata fbaseMetadata)
        {
            // Merge source metadata into this

            // Modify metadata merge state fields directly (not through accessors
            // so that "modified" bits remain intact

            // Merge state
            // Defaults to false, derived classes can only enable
            WriteFlag(MetadataFlags.FW_AffectsMeasureID, ReadFlag(MetadataFlags.FW_AffectsMeasureID) | fbaseMetadata.AffectsMeasure);
            WriteFlag(MetadataFlags.FW_AffectsArrangeID, ReadFlag(MetadataFlags.FW_AffectsArrangeID) | fbaseMetadata.AffectsArrange);
            WriteFlag(MetadataFlags.FW_AffectsParentMeasureID, ReadFlag(MetadataFlags.FW_AffectsParentMeasureID) | fbaseMetadata.AffectsParentMeasure);
            WriteFlag(MetadataFlags.FW_AffectsParentArrangeID, ReadFlag(MetadataFlags.FW_AffectsParentArrangeID) | fbaseMetadata.AffectsParentArrange);
            WriteFlag(MetadataFlags.FW_AffectsRenderID, ReadFlag(MetadataFlags.FW_AffectsRenderID) | fbaseMetadata.AffectsRender);
            WriteFlag(MetadataFlags.FW_BindsTwoWayByDefaultID, ReadFlag(MetadataFlags.FW_BindsTwoWayByDefaultID) | fbaseMetadata.BindsTwoWayByDefault);
            WriteFlag(MetadataFlags.FW_IsNotDataBindableID, ReadFlag(MetadataFlags.FW_IsNotDataBindableID) | fbaseMetadata.IsNotDataBindable);

            // Override state
            if (!IsModified(MetadataFlags.FW_SubPropertiesDoNotAffectRenderModifiedID))
            {
                WriteFlag(MetadataFlags.FW_SubPropertiesDoNotAffectRenderID, fbaseMetadata.SubPropertiesDoNotAffectRender);
            }

            if (!IsModified(MetadataFlags.FW_InheritsModifiedID))
            {
                IsInherited = fbaseMetadata.Inherits;
            }

            if (!IsModified(MetadataFlags.FW_OverridesInheritanceBehaviorModifiedID))
            {
                WriteFlag(MetadataFlags.FW_OverridesInheritanceBehaviorID, fbaseMetadata.OverridesInheritanceBehavior);
            }

            if (!IsModified(MetadataFlags.FW_ShouldBeJournaledModifiedID))
            {
                WriteFlag(MetadataFlags.FW_ShouldBeJournaledID, fbaseMetadata.Journal);
            }

            if (!IsModified(MetadataFlags.FW_DefaultUpdateSourceTriggerModifiedID))
            {
                // FW_DefaultUpdateSourceTriggerEnumBit1        = 0x40000000,
                // FW_DefaultUpdateSourceTriggerEnumBit2        = 0x80000000,
                _flags = (MetadataFlags)(((uint)_flags & 0x3FFFFFFF) | ((uint)fbaseMetadata.DefaultUpdateSourceTrigger) << 30);
            }
        }
    }

    /// <summary>
    /// Called when this metadata has been applied to a property, which indicates that
    /// the metadata is being sealed.
    /// </summary>
    /// <param name="dp">
    /// The dependency property to which the metadata has been applied.
    /// </param>
    /// <param name="targetType">
    /// The type associated with this metadata if this is type-specific metadata. If
    /// this is default metadata, this value can be null.
    /// </param>
    protected override void OnApply(DependencyProperty dp, Type targetType)
    {
        // Remember if this is the metadata for a ReadOnly property
        ReadOnly = dp.ReadOnly;

        base.OnApply(dp, targetType);
    }

    private void Initialize()
    {
        // FW_DefaultUpdateSourceTriggerEnumBit1        = 0x40000000,
        // FW_DefaultUpdateSourceTriggerEnumBit2        = 0x80000000,
        _flags = (MetadataFlags)(((uint)_flags & 0x3FFFFFFF) | ((uint)UpdateSourceTrigger.PropertyChanged) << 30);
    }

    private static bool IsFlagSet(FrameworkPropertyMetadataOptions flag, FrameworkPropertyMetadataOptions flags)
    {
        return (flags & flag) != 0;
    }

    private void TranslateFlags(FrameworkPropertyMetadataOptions flags)
    {
        Initialize();

        // Convert flags to state sets. If a flag is set, then,
        // the value is set on the respective property. Otherwise,
        // the state remains unset

        // This means that state is cumulative across base classes
        // on a merge where appropriate

        if (IsFlagSet(FrameworkPropertyMetadataOptions.AffectsMeasure, flags))
        {
            AffectsMeasure = true;
        }

        if (IsFlagSet(FrameworkPropertyMetadataOptions.AffectsArrange, flags))
        {
            AffectsArrange = true;
        }

        if (IsFlagSet(FrameworkPropertyMetadataOptions.AffectsParentMeasure, flags))
        {
            AffectsParentMeasure = true;
        }

        if (IsFlagSet(FrameworkPropertyMetadataOptions.AffectsParentArrange, flags))
        {
            AffectsParentArrange = true;
        }

        if (IsFlagSet(FrameworkPropertyMetadataOptions.AffectsRender, flags))
        {
            AffectsRender = true;
        }

        if (IsFlagSet(FrameworkPropertyMetadataOptions.Inherits, flags))
        {
            IsInherited = true;
        }

        if (IsFlagSet(FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior, flags))
        {
            OverridesInheritanceBehavior = true;
        }

        if (IsFlagSet(FrameworkPropertyMetadataOptions.NotDataBindable, flags))
        {
            IsNotDataBindable = true;
        }

        if (IsFlagSet(FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, flags))
        {
            BindsTwoWayByDefault = true;
        }

        if (IsFlagSet(FrameworkPropertyMetadataOptions.Journal, flags))
        {
            Journal = true;
        }

        if (IsFlagSet(FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender, flags))
        {
            SubPropertiesDoNotAffectRender = true;
        }
    }

    internal void SetModified(MetadataFlags id) { WriteFlag(id, true); }

    internal bool IsModified(MetadataFlags id) { return ReadFlag(id); }

    // return false if this is an invalid value for UpdateSourceTrigger
    private static bool IsValidUpdateSourceTrigger(UpdateSourceTrigger value)
    {
        return value == UpdateSourceTrigger.Default ||
               value == UpdateSourceTrigger.PropertyChanged ||
               value == UpdateSourceTrigger.LostFocus ||
               value == UpdateSourceTrigger.Explicit;
    }
}
