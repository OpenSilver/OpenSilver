
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

namespace System.Windows.Documents;

/// <summary>
/// Provides access to a rich set of OpenType typography properties.
/// </summary>
[OpenSilver.NotImplemented]
public static class Typography
{
    /// <summary>
    /// Identifies the Typography.StandardLigatures attached property.
    /// </summary>
    public static readonly DependencyProperty StandardLigaturesProperty =
        DependencyProperty.RegisterAttached(
            "StandardLigatures",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Returns the value of the Typography.StandardLigatures attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StandardLigatures property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StandardLigatures attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStandardLigatures(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StandardLigaturesProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.StandardLigatures attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StandardLigatures property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStandardLigatures(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StandardLigaturesProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.ContextualLigatures attached property.
    /// </summary>
    public static readonly DependencyProperty ContextualLigaturesProperty =
        DependencyProperty.RegisterAttached(
            "ContextualLigatures",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Returns the value of the Typography.ContextualLigatures attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.ContextualLigatures property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.ContextualLigatures attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetContextualLigatures(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(ContextualLigaturesProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.ContextualLigatures attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.ContextualLigatures property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetContextualLigatures(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(ContextualLigaturesProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.DiscretionaryLigatures attached property.
    /// </summary>
    public static readonly DependencyProperty DiscretionaryLigaturesProperty =
        DependencyProperty.RegisterAttached(
            "DiscretionaryLigatures",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.DiscretionaryLigatures attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.DiscretionaryLigatures property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.DiscretionaryLigatures attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetDiscretionaryLigatures(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(DiscretionaryLigaturesProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.DiscretionaryLigatures attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.DiscretionaryLigatures property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetDiscretionaryLigatures(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(DiscretionaryLigaturesProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.HistoricalLigatures attached property.
    /// </summary>
    public static readonly DependencyProperty HistoricalLigaturesProperty =
        DependencyProperty.RegisterAttached(
            "HistoricalLigatures",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.HistoricalLigatures attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.HistoricalLigatures property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.HistoricalLigatures attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetHistoricalLigatures(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(HistoricalLigaturesProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.HistoricalLigatures attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.HistoricalLigatures property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetHistoricalLigatures(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(HistoricalLigaturesProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.AnnotationAlternates attached property.
    /// </summary>
    public static readonly DependencyProperty AnnotationAlternatesProperty =
        DependencyProperty.RegisterAttached(
            "AnnotationAlternates",
            typeof(int),
            typeof(Typography),
            new PropertyMetadata(0));

    /// <summary>
    /// Returns the value of the Typography.AnnotationAlternates attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.AnnotationAlternates property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.AnnotationAlternates attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static int GetAnnotationAlternates(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (int)element.GetValue(AnnotationAlternatesProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.AnnotationAlternates attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.AnnotationAlternates property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetAnnotationAlternates(DependencyObject element, int value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(AnnotationAlternatesProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.ContextualAlternates attached property.
    /// </summary>
    public static readonly DependencyProperty ContextualAlternatesProperty =
        DependencyProperty.RegisterAttached(
            "ContextualAlternates",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Returns the value of the Typography.ContextualAlternates attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.ContextualAlternates property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.ContextualAlternates attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetContextualAlternates(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(ContextualAlternatesProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.ContextualAlternates attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.ContextualAlternates property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetContextualAlternates(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(ContextualAlternatesProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.HistoricalForms attached property.
    /// </summary>
    public static readonly DependencyProperty HistoricalFormsProperty =
        DependencyProperty.RegisterAttached(
            "HistoricalForms",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.HistoricalForms attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.HistoricalForms property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.HistoricalForms attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetHistoricalForms(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(HistoricalFormsProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.HistoricalForms attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.HistoricalForms property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetHistoricalForms(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(HistoricalFormsProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.Kerning attached property.
    /// </summary>
    public static readonly DependencyProperty KerningProperty =
        DependencyProperty.RegisterAttached(
            "Kerning",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Returns the value of the Typography.Kerning attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.Kerning property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.Kerning attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetKerning(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(KerningProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.Kerning attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.Kerning property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetKerning(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(KerningProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.CapitalSpacing attached property.
    /// </summary>
    public static readonly DependencyProperty CapitalSpacingProperty =
        DependencyProperty.RegisterAttached(
            "CapitalSpacing",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.CapitalSpacing attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.CapitalSpacing property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.CapitalSpacing attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetCapitalSpacing(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(CapitalSpacingProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.CapitalSpacing attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.CapitalSpacing property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetCapitalSpacing(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(CapitalSpacingProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.CaseSensitiveForms attached property.
    /// </summary>
    public static readonly DependencyProperty CaseSensitiveFormsProperty =
        DependencyProperty.RegisterAttached(
            "CaseSensitiveForms",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.CaseSensitiveForms attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.CaseSensitiveForms property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.CaseSensitiveForms attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetCaseSensitiveForms(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(CaseSensitiveFormsProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.CaseSensitiveForms attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.CaseSensitiveForms property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetCaseSensitiveForms(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(CaseSensitiveFormsProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet1 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet1Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet1",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet1 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet1 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet1 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet1(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet1Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet1 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet1 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet1(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet1Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet2 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet2Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet2",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet2 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet2 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet2 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet2(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet2Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet2 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet2 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet2(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet2Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet3 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet3Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet3",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet3 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet3 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet3 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet3(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet3Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet3 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet3 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet3(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet3Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet4 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet4Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet4",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet4 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet4 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet4 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet4(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet4Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet4 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet4 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet4(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet4Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet5 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet5Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet5",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet5 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet5 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet5 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet5(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet5Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet5 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet5 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet5(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet5Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet6 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet6Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet6",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet6 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet6 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet6 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet6(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet6Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet6 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet6 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet6(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet6Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet7 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet7Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet7",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet7 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet7 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet7 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet7(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet7Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet7 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet7 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet7(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet7Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet8 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet8Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet8",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet8 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet8 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet8 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet8(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet8Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet8 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet8 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet8(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet8Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet9 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet9Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet9",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet9 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet9 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet9 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet9(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet9Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet9 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet9 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet9(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet9Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet10 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet10Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet10",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet10 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet10 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet10 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet10(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet10Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet10 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet10 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet10(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet10Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet11 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet11Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet11",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet11 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet11 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet11 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet11(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet11Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet11 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet11 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet11(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet11Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet12 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet12Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet12",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet12 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet12 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet12 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet12(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet12Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet12 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet12 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet12(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet12Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet13 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet13Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet13",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet13 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet13 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet13 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet13(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet13Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet13 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet13 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet13(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet13Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet14 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet14Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet14",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet14 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet14 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet14 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet14(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet14Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet14 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet14 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet14(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet14Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet15 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet15Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet15",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet15 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet15 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet15 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet15(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet15Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet15 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet15 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet15(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet15Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet16 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet16Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet16",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet16 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet16 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet16 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet16(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet16Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet16 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet16 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet16(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet16Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet17 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet17Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet17",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet17 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet17 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet17 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet17(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet17Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet17 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet17 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet17(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet17Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet18 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet18Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet18",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet18 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet18 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet18 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet18(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet18Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet18 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet18 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet18(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet18Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet19 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet19Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet19",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet19 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet19 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet19 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet19(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet19Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet19 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet19 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet19(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet19Property, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticSet20 attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticSet20Property =
        DependencyProperty.RegisterAttached(
            "StylisticSet20",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.StylisticSet20 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticSet20 property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticSet20 attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetStylisticSet20(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(StylisticSet20Property);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticSet20 attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticSet20 property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticSet20(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticSet20Property, value);
    }

    /// <summary>
    /// Identifies the Typography.Fraction attached property.
    /// </summary>
    public static readonly DependencyProperty FractionProperty =
        DependencyProperty.RegisterAttached(
            "Fraction",
            typeof(FontFraction),
            typeof(Typography),
            new PropertyMetadata(FontFraction.Normal));

    /// <summary>
    /// Returns the value of the Typography.Fraction attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.Fraction property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.Fraction attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static FontFraction GetFraction(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (FontFraction)element.GetValue(FractionProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.Fraction attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.Fraction property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetFraction(DependencyObject element, FontFraction value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(FractionProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.SlashedZero attached property.
    /// </summary>
    public static readonly DependencyProperty SlashedZeroProperty =
        DependencyProperty.RegisterAttached(
            "SlashedZero",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.SlashedZero attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.SlashedZero property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.SlashedZero attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetSlashedZero(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(SlashedZeroProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.SlashedZero attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.SlashedZero property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetSlashedZero(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(SlashedZeroProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.MathematicalGreek attached property.
    /// </summary>
    public static readonly DependencyProperty MathematicalGreekProperty =
        DependencyProperty.RegisterAttached(
            "MathematicalGreek",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.MathematicalGreek attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.MathematicalGreek property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.MathematicalGreek attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetMathematicalGreek(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(MathematicalGreekProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.MathematicalGreek attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.MathematicalGreek property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetMathematicalGreek(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(MathematicalGreekProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.EastAsianExpertForms attached property.
    /// </summary>
    public static readonly DependencyProperty EastAsianExpertFormsProperty =
        DependencyProperty.RegisterAttached(
            "EastAsianExpertForms",
            typeof(bool),
            typeof(Typography),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Returns the value of the Typography.EastAsianExpertForms attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.EastAsianExpertForms property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.EastAsianExpertForms attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetEastAsianExpertForms(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(EastAsianExpertFormsProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.EastAsianExpertForms attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.EastAsianExpertForms property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetEastAsianExpertForms(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(EastAsianExpertFormsProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.Variants attached property.
    /// </summary>
    public static readonly DependencyProperty VariantsProperty =
        DependencyProperty.RegisterAttached(
            "Variants",
            typeof(FontVariants),
            typeof(Typography),
            new PropertyMetadata(FontVariants.Normal));

    /// <summary>
    /// Returns the value of the Typography.Variants attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.Variants property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.Variants attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static FontVariants GetVariants(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (FontVariants)element.GetValue(VariantsProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.Variants attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.Variants property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetVariants(DependencyObject element, FontVariants value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(VariantsProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.Capitals attached property.
    /// </summary>
    public static readonly DependencyProperty CapitalsProperty =
        DependencyProperty.RegisterAttached(
            "Capitals",
            typeof(FontCapitals),
            typeof(Typography),
            new PropertyMetadata(FontCapitals.Normal));

    /// <summary>
    /// Returns the value of the Typography.Capitals attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.Capitals property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.Capitals attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static FontCapitals GetCapitals(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (FontCapitals)element.GetValue(CapitalsProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.Capitals attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.Capitals property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetCapitals(DependencyObject element, FontCapitals value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(CapitalsProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.NumeralStyle attached property.
    /// </summary>
    public static readonly DependencyProperty NumeralStyleProperty =
        DependencyProperty.RegisterAttached(
            "NumeralStyle",
            typeof(FontNumeralStyle),
            typeof(Typography),
            new PropertyMetadata(FontNumeralStyle.Normal));

    /// <summary>
    /// Returns the value of the Typography.NumeralStyle attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.NumeralStyle property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.NumeralStyle attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static FontNumeralStyle GetNumeralStyle(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (FontNumeralStyle)element.GetValue(NumeralStyleProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.NumeralStyle attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.NumeralStyle property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetNumeralStyle(DependencyObject element, FontNumeralStyle value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(NumeralStyleProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.NumeralAlignment attached property.
    /// </summary>
    public static readonly DependencyProperty NumeralAlignmentProperty =
        DependencyProperty.RegisterAttached(
            "NumeralAlignment",
            typeof(FontNumeralAlignment),
            typeof(Typography),
            new PropertyMetadata(FontNumeralAlignment.Normal));

    /// <summary>
    /// Returns the value of the Typography.NumeralAlignment attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.NumeralAlignment property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.NumeralAlignment attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static FontNumeralAlignment GetNumeralAlignment(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (FontNumeralAlignment)element.GetValue(NumeralAlignmentProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.NumeralAlignment attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.NumeralAlignment property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetNumeralAlignment(DependencyObject element, FontNumeralAlignment value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(NumeralAlignmentProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.EastAsianWidths attached property.
    /// </summary>
    public static readonly DependencyProperty EastAsianWidthsProperty =
        DependencyProperty.RegisterAttached(
            "EastAsianWidths",
            typeof(FontEastAsianWidths),
            typeof(Typography),
            new PropertyMetadata(FontEastAsianWidths.Normal));

    /// <summary>
    /// Returns the value of the Typography.EastAsianWidths attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.EastAsianWidths property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.EastAsianWidths attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static FontEastAsianWidths GetEastAsianWidths(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (FontEastAsianWidths)element.GetValue(EastAsianWidthsProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.EastAsianWidths attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.EastAsianWidths property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetEastAsianWidths(DependencyObject element, FontEastAsianWidths value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(EastAsianWidthsProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.EastAsianLanguage attached property.
    /// </summary>
    public static readonly DependencyProperty EastAsianLanguageProperty =
        DependencyProperty.RegisterAttached(
            "EastAsianLanguage",
            typeof(FontEastAsianLanguage),
            typeof(Typography),
            new PropertyMetadata(FontEastAsianLanguage.Normal));

    /// <summary>
    /// Returns the value of the Typography.EastAsianLanguage attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.EastAsianLanguage property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.EastAsianLanguage attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static FontEastAsianLanguage GetEastAsianLanguage(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (FontEastAsianLanguage)element.GetValue(EastAsianLanguageProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.EastAsianLanguage attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.EastAsianLanguage property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetEastAsianLanguage(DependencyObject element, FontEastAsianLanguage value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(EastAsianLanguageProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.StandardSwashes attached property.
    /// </summary>
    public static readonly DependencyProperty StandardSwashesProperty =
        DependencyProperty.RegisterAttached(
            "StandardSwashes",
            typeof(int),
            typeof(Typography),
            new PropertyMetadata(0));

    /// <summary>
    /// Returns the value of the Typography.StandardSwashes attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StandardSwashes property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StandardSwashes attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static int GetStandardSwashes(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (int)element.GetValue(StandardSwashesProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.StandardSwashes attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StandardSwashes property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStandardSwashes(DependencyObject element, int value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StandardSwashesProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.ContextualSwashes attached property.
    /// </summary>
    public static readonly DependencyProperty ContextualSwashesProperty =
        DependencyProperty.RegisterAttached(
            "ContextualSwashes",
            typeof(int),
            typeof(Typography),
            new PropertyMetadata(0));

    /// <summary>
    /// Returns the value of the Typography.ContextualSwashes attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.ContextualSwashes property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.ContextualSwashes attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static int GetContextualSwashes(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (int)element.GetValue(ContextualSwashesProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.ContextualSwashes attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.ContextualSwashes property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetContextualSwashes(DependencyObject element, int value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(ContextualSwashesProperty, value);
    }

    /// <summary>
    /// Identifies the Typography.StylisticAlternates attached property.
    /// </summary>
    public static readonly DependencyProperty StylisticAlternatesProperty =
        DependencyProperty.RegisterAttached(
            "StylisticAlternates",
            typeof(int),
            typeof(Typography),
            new PropertyMetadata(0));

    /// <summary>
    /// Returns the value of the Typography.StylisticAlternates attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to retrieve the value of the Typography.StylisticAlternates property.
    /// </param>
    /// <returns>
    /// The current value of the Typography.StylisticAlternates attached property on the specified dependency object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static int GetStylisticAlternates(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (int)element.GetValue(StylisticAlternatesProperty);
    }

    /// <summary>
    /// Sets the value of the Typography.StylisticAlternates attached property for a specified dependency object.
    /// </summary>
    /// <param name="element">
    /// The dependency object for which to set the value of the Typography.StylisticAlternates property.
    /// </param>
    /// <param name="value">
    /// The new value to set the property to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetStylisticAlternates(DependencyObject element, int value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(StylisticAlternatesProperty, value);
    }
}
