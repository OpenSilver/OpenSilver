
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

namespace System.Windows;

/// <summary>
/// Provides property metadata for non-framework properties that do have rendering/user interface impact at 
/// the core level.
/// </summary>
public class UIPropertyMetadata : PropertyMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UIPropertyMetadata"/> class.
    /// </summary>
    public UIPropertyMetadata() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UIPropertyMetadata"/> class, with the specified default 
    /// value for the property.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value of the dependency property, usually provided as a value of some specific type.
    /// </param>
    public UIPropertyMetadata(object defaultValue)
        : base(defaultValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UIPropertyMetadata"/> class, with the specified 
    /// PropertyChanged callback.
    /// </summary>
    /// <param name="propertyChangedCallback">
    /// Reference to a handler implementation that is to be called by the property system whenever the effective 
    /// value of the property changes.
    /// </param>
    public UIPropertyMetadata(PropertyChangedCallback propertyChangedCallback)
        : base(propertyChangedCallback)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UIPropertyMetadata"/> class, with the specified 
    /// PropertyChanged callback.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value of the dependency property, usually provided as a value of some specific type.
    /// </param>
    /// <param name="propertyChangedCallback">
    /// Reference to a handler implementation that is to be called by the property system whenever the effective 
    /// value of the property changes.
    /// </param>
    public UIPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback)
        : base(defaultValue, propertyChangedCallback)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UIPropertyMetadata"/> class, with the specified default value
    /// and callbacks.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value of the dependency property, usually provided as a value of some specific type.
    /// </param>
    /// <param name="propertyChangedCallback">
    /// Reference to a handler implementation that is to be called by the property system whenever the effective 
    /// value of the property changes.
    /// </param>
    /// <param name="coerceValueCallback">
    /// Reference to a handler implementation that is to be called whenever the property system calls 
    /// <see cref="DependencyObject.CoerceValue(DependencyProperty)"/> against this property.
    /// </param>
    public UIPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
        : base(defaultValue, propertyChangedCallback, coerceValueCallback)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="UIPropertyMetadata"/> class, with the specified default value 
    /// and callbacks, and a Boolean used to disable animations on the property.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value of the dependency property, usually provided as a value of some specific type.
    /// </param>
    /// <param name="propertyChangedCallback">
    /// Reference to a handler implementation that is to be called by the property system whenever the effective 
    /// value of the property changes.
    /// </param>
    /// <param name="coerceValueCallback">
    /// Reference to a handler implementation that is to be called whenever the property system calls 
    /// <see cref="DependencyObject.CoerceValue(DependencyProperty)"/> against this property.
    /// </param>
    /// <param name="isAnimationProhibited">
    /// Set to true to prevent the property system from animating the property that this metadata is applied to. 
    /// Such properties will raise run time exceptions if animations of them are attempted. The default is false.
    /// </param>
    public UIPropertyMetadata(
        object defaultValue,
        PropertyChangedCallback propertyChangedCallback,
        CoerceValueCallback coerceValueCallback,
        bool isAnimationProhibited)
        : base(defaultValue, propertyChangedCallback, coerceValueCallback)
    {
        IsAnimationProhibited = isAnimationProhibited;
    }

    /// <summary>
    /// Gets or sets a value declaring whether animations should be disabled on the dependency property where the 
    /// containing metadata instance is applied.
    /// </summary>
    /// <returns>
    /// true indicates that animations are disallowed; false indicates that animations are allowed. The default is 
    /// false (animations allowed).
    /// </returns>
    public bool IsAnimationProhibited
    {
        get { return ReadFlag(MetadataFlags.UI_IsAnimationProhibitedID); }
        set { CheckSealed(); WriteFlag(MetadataFlags.UI_IsAnimationProhibitedID, value); }
    }
}
