
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
using OpenSilver.Internal.Data;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Markup;

namespace System.Windows.Data;

/// <summary>
/// Represents the base class for <see cref="BindingExpression"/>, and <see cref="MultiBindingExpression"/>.
/// </summary>
public abstract class BindingExpressionBase : Expression
{
    [Flags]
    internal enum PrivateFlags
    {
        iSourceToTarget = 0x00000001,
        iTargetToSource = 0x00000002,
        iPropDefault = 0x00000004,
        iInTransfer = 0x00000008,
        iInUpdate = 0x00000010,
        iNeedDataTransfer = 0x00000020,   // used by MultiBindingExpression
        iTransferDeferred = 0x00000040,   // used by MultiBindingExpression
        iUpdateOnLostFocus = 0x00000080,
        iUpdateExplicitly = 0x00000100,
        iUpdateOnPropertyChanged = 0x00000200,
        iUpdateDefault = iUpdateExplicitly | iUpdateOnLostFocus | iUpdateOnPropertyChanged,
        iNeedsUpdate = 0x00000400,
        iDetaching = 0x00000800,
        iInMultiBindingExpression = 0x00001000,
        iNotifyOnValidationError = 0x00002000,
        iAttaching = 0x00004000,
        iValidatesOnExceptions = 0x00008000,
        iValidatesOnDataErrors = 0x00010000,
        iValidatesOnNotifyDataErrors = 0x00020000,

        iPropagationMask = iSourceToTarget | iTargetToSource | iPropDefault,
        iUpdateMask = iUpdateOnPropertyChanged | iUpdateOnLostFocus | iUpdateExplicitly,
    }

    /// <summary>
    /// NoTarget DependencyProperty, a placeholder used by BindingExpressions with no target property
    /// </summary>
    internal static readonly DependencyProperty NoTargetProperty =
        DependencyProperty.RegisterAttached(
            "NoTarget",
            typeof(object),
            typeof(BindingExpressionBase),
            null);

    /// <summary> Sentinel meaning "field has its default value" </summary>
    internal static readonly object DefaultValueObject = new NamedObject("DefaultValue");

    private PrivateFlags _flags;
    private PrivateFlags _defaultFlags;
    private PropertyChangeListener _targetPropertyListener;
    private PropertyChangeListener _languageChangedListener;
    private object _culture = DefaultValueObject;

    private BindingStatus _status;

    internal BindingExpressionBase(BindingBase binding, BindingExpressionBase parent)
    {
        ParentBindingBase = binding;
        ParentBindingExpressionBase = parent;

        _flags = (PrivateFlags)binding.Flags;

        if (parent is not null)
        {
            Type type = parent.GetType();
            if (type == typeof(MultiBindingExpression))
            {
                ChangeFlag(PrivateFlags.iInMultiBindingExpression, true);
            }
        }
    }

    /// <summary> Create an untargeted BindingExpression </summary>
    internal static BindingExpressionBase CreateUntargetedBindingExpression(DependencyObject d, BindingBase binding) =>
        binding.CreateBindingExpression(d, NoTargetProperty, null);

    /// <summary>
    /// Gets the status of the binding expression.
    /// </summary>
    /// <returns>
    /// A <see cref="BindingStatus"/> value that describes the status of the binding expression.
    /// </returns>
    public BindingStatus Status => _status;

    /// <summary>
    /// Gets the <see cref="BindingBase"/> object from which this <see cref="BindingExpressionBase"/> object is created.
    /// </summary>
    /// <returns>
    /// The <see cref="BindingBase"/> object from which this <see cref="BindingExpressionBase"/> object is created.
    /// </returns>
    internal BindingBase ParentBindingBase { get; }

    /// <summary>
    /// Gets the element that is the binding target object of this binding expression.
    /// </summary>
    /// <returns>
    /// The element that is the binding target object of this binding expression.
    /// </returns>
    internal DependencyObject Target { get; private set; }

    /// <summary>
    /// Gets the binding target property of this binding expression.
    /// </summary>
    /// <returns>
    /// The binding target property of this binding expression.
    /// </returns>
    internal DependencyProperty TargetProperty { get; private set; }

    /// <summary> The parent MultiBindingExpression (if any) </summary>
    internal BindingExpressionBase ParentBindingExpressionBase { get; }

    /// <summary> The default value of the target property </summary>
    internal object DefaultValue => TargetProperty.GetDefaultValue(Target);

    /// <summary> True if this binding expression is attaching </summary>
    internal bool IsAttaching
    {
        get => TestFlag(PrivateFlags.iAttaching);
        private set => ChangeFlag(PrivateFlags.iAttaching, value);
    }

    /// <summary> True if this binding expression is detaching </summary>
    internal bool IsDetaching
    {
        get => TestFlag(PrivateFlags.iDetaching);
        private set => ChangeFlag(PrivateFlags.iDetaching, value);
    }

    /// <summary> True if this binding expression is detached </summary>
    internal bool IsDetached => _status == BindingStatus.Detached;

    /// <summary> True if this binding expression updates the target </summary>
    internal bool IsDynamic =>
        TestFlag(PrivateFlags.iSourceToTarget) && (!IsInMultiBindingExpression || ParentBindingExpressionBase.IsDynamic);

    /// <summary> True if this binding expression updates the source </summary>
    internal bool IsReflective =>
        TestFlag(PrivateFlags.iTargetToSource) && (!IsInMultiBindingExpression || ParentBindingExpressionBase.IsReflective);

    /// <summary> True if this binding expression is OneWayToSource </summary>
    internal bool IsOneWayToSource => (_flags & PrivateFlags.iPropagationMask) == PrivateFlags.iTargetToSource;

    /// <summary> True if this binding expression updates on PropertyChanged </summary>
    internal bool IsUpdateOnPropertyChanged => TestFlag(PrivateFlags.iUpdateOnPropertyChanged);

    /// <summary> True if this binding expression updates on LostFocus </summary>
    internal bool IsUpdateOnLostFocus => TestFlag(PrivateFlags.iUpdateOnLostFocus);

    /// <summary> True if this binding expression is deferring a target update </summary>
    internal bool TransferIsDeferred
    {
        get => TestFlag(PrivateFlags.iTransferDeferred);
        set => ChangeFlag(PrivateFlags.iTransferDeferred, value);
    }

    /// <summary> True if this binding expression is updating the target </summary>
    internal bool IsInTransfer
    {
        get => TestFlag(PrivateFlags.iInTransfer);
        set => ChangeFlag(PrivateFlags.iInTransfer, value);
    }

    /// <summary> True if this binding expression is updating the source </summary>
    internal bool IsInUpdate
    {
        get => TestFlag(PrivateFlags.iInUpdate);
        set => ChangeFlag(PrivateFlags.iInUpdate, value);
    }

    /// <summary> True if this binding expression has a pending target update </summary>
    internal bool NeedsDataTransfer
    {
        get => TestFlag(PrivateFlags.iNeedDataTransfer);
        set => ChangeFlag(PrivateFlags.iNeedDataTransfer, value);
    }

    /// <summary> True if this binding expression has a pending source update </summary>
    internal bool NeedsUpdate
    {
        get => TestFlag(PrivateFlags.iNeedsUpdate);
        set => ChangeFlag(PrivateFlags.iNeedsUpdate, value);
    }

    /// <summary> True if this binding expression belongs to a MultiBinding </summary>
    internal bool IsInMultiBindingExpression => TestFlag(PrivateFlags.iInMultiBindingExpression);

    /// <summary> True if this binding expression belongs to a PriorityBinding or MultiBinding </summary>
    internal bool IsInBindingExpressionCollection => TestFlag(PrivateFlags.iInMultiBindingExpression);

    /// <summary> True if this binding expression validates on exceptions </summary>
    internal bool ValidatesOnExceptions => TestFlag(PrivateFlags.iValidatesOnExceptions);

    /// <summary> True if this binding expression validates on data errors </summary>
    internal bool ValidatesOnDataErrors => TestFlag(PrivateFlags.iValidatesOnDataErrors);

    /// <summary> True if this binding expression validates on notify data errors </summary>
    internal bool ValidatesOnNotifyDataErrors => TestFlag(PrivateFlags.iValidatesOnNotifyDataErrors);

    internal bool UsesLanguage => ParentBindingBase.ConverterCultureInternal is null;

    /// <summary>
    /// Sends the current binding target value to the binding source in <see cref="BindingMode.TwoWay"/>
    /// or <see cref="BindingMode.OneWayToSource"/> bindings.
    /// </summary>
    public virtual void UpdateSource() { }

    /// <summary>
    /// Compute the culture, either from the parent Binding, or from the target element.
    /// </summary>
    internal CultureInfo GetCulture()
    {
        if (_culture == DefaultValueObject)
        {
            // explicit culture set in Binding
            _culture = ParentBindingBase.ConverterCultureInternal;

            // if that doesn't work, use target element's xml:lang property
            if (_culture is null)
            {
                if (Target is not null && Target.GetValue(FrameworkElement.LanguageProperty) is XmlLanguage xmlLanguage)
                {
                    _culture = xmlLanguage.GetSpecificCulture();
                }
            }
        }

        return (CultureInfo)_culture;
    }

    /// <summary> Culture has changed.  Re-fetch the value with the new culture. </summary>
    private void InvalidateCulture() => _culture = DefaultValueObject;

    /// <summary>
    /// Invalidate the given child expression.
    /// </summary>
    internal abstract void InvalidateChild(BindingExpressionBase bindingExpression);

    // transfer a value from the source to the target
    internal void Invalidate()
    {
        // don't invalidate during Attach.  The property engine does it already.
        if (IsAttaching) return;

        if (TargetProperty != NoTargetProperty)
        {
            Target.ApplyExpression(TargetProperty, this);
        }
    }

    internal void RaiseValueChanged() => ValueChanged?.Invoke(this, EventArgs.Empty);

    internal event EventHandler ValueChanged;

    internal sealed override void OnAttach(DependencyObject d, DependencyProperty dp) => Attach(d, dp);

    internal void Attach(DependencyObject d, DependencyProperty dp)
    {
        IsAttaching = true;
        AttachOverride(d, dp);
        IsAttaching = false;
    }

    /// <summary> Attach the BindingExpression to its target element </summary>
    /// <remarks>
    /// This method must be called once during the initialization.
    /// </remarks>
    /// <param name="d">The target element </param>
    internal void Attach(DependencyObject d) => Attach(d, NoTargetProperty);

    /// <summary>
    /// Attach the binding expression to the given target object and property.
    /// Derived classes should call base.AttachOverride before doing their work,
    /// and should continue only if it returns true.
    /// </summary>
    internal virtual void AttachOverride(DependencyObject d, DependencyProperty dp)
    {
        Target = d;
        TargetProperty = dp;

        SetStatus(BindingStatus.Unattached);

        DetermineEffectiveValidatesOnNotifyDataErrors();

        if (UsesLanguage)
        {
            _languageChangedListener = PropertyChangeListener.CreateListener(Target, FrameworkElement.LanguageProperty, OnLanguageChanged);
        }

        // Listen to changes on the Target if the Binding is TwoWay:
        if (IsReflective && IsUpdateOnPropertyChanged)
        {
            if (IsUpdateOnLostFocus && Target is UIElement uie)
            {
                uie.LostFocus += new RoutedEventHandler(OnTargetLostFocus);
            }

            _targetPropertyListener = PropertyChangeListener.CreateListener(Target, TargetProperty, OnTargetPropertyChanged);
        }
    }

    internal sealed override void OnDetach(DependencyObject d, DependencyProperty dp) => Detach();

    internal void Detach()
    {
        IsDetaching = true;
        DetachOverride();
        IsDetaching = false;
    }

    /// <summary>
    /// Detach the binding expression from its target object and property.
    /// Derived classes should call base.DetachOverride after doing their work.
    /// </summary>
    internal virtual void DetachOverride()
    {
        _languageChangedListener?.Dispose();
        _languageChangedListener = null;

        _targetPropertyListener?.Dispose();
        _targetPropertyListener = null;

        if (IsUpdateOnLostFocus && Target is UIElement uie)
        {
            uie.LostFocus -= new RoutedEventHandler(OnTargetLostFocus);
        }

        Target = null;
        TargetProperty = null;

        _flags = _defaultFlags;
        SetStatus(BindingStatus.Detached);
    }

    internal void SetStatus(BindingStatus status) => _status = status;

    private void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => InvalidateCulture();

    private void OnTargetLostFocus(object sender, RoutedEventArgs e) => Update();

    private void OnTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        if (IsInTransfer || IsInUpdate)
        {
            return;
        }

        NeedsUpdate = true;

        if (IsUpdateOnLostFocus && IsTargetFocused())
        {
            return;
        }

        Update();
    }

    private bool IsTargetFocused() => Target is UIElement uie && uie.IsKeyboardFocusWithin;

    internal bool Validate(object value)
    {
        if (value == DependencyProperty.UnsetValue)
        {
            SetStatus(BindingStatus.UpdateSourceError);
            return false;
        }

        return true;
    }

    internal virtual void Update() { }

    // Return the object from which the given value was obtained, if possible
    internal abstract object GetSourceItem(object newValue);

    /// <summary>
    /// Create a format that is suitable for String.Format
    /// </summary>
    internal string GetEffectiveStringFormat()
    {
        if (ParentBindingBase.StringFormat is not string stringFormat)
        {
            return null;
        }

        if (stringFormat.IndexOf('{') < 0)
        {
            stringFormat = @"{0:" + stringFormat + @"}";
        }
        return stringFormat;
    }

    internal static void HandleException(Exception ex)
    {
        if (Application.Current.Host.Settings.EnableBindingErrorsLogging)
        {
            Debug.WriteLine(ex.ToString());
        }
        if (Application.Current.Host.Settings.EnableBindingErrorsThrowing)
        {
            throw ex;
        }
    }

    /// <summary> Begin a source update </summary>
    internal void BeginSourceUpdate() => ChangeFlag(PrivateFlags.iInUpdate, true);

    /// <summary> End a source update </summary>
    internal void EndSourceUpdate() => ChangeFlag(PrivateFlags.iInUpdate | PrivateFlags.iNeedsUpdate, false);

    internal void ResolvePropertyDefaultSettings(BindingMode mode, UpdateSourceTrigger updateTrigger, FrameworkPropertyMetadata fwMetaData)
    {
        // resolve "property-default" dataflow
        if (mode == BindingMode.Default)
        {
            PrivateFlags f = PrivateFlags.iSourceToTarget;
            if (fwMetaData is not null && fwMetaData.BindsTwoWayByDefault)
            {
                f = PrivateFlags.iSourceToTarget | PrivateFlags.iTargetToSource;
            }

            ChangeFlag(PrivateFlags.iPropagationMask, false);
            ChangeFlag(f, true);
        }

        Debug.Assert((_flags & PrivateFlags.iPropagationMask) != PrivateFlags.iPropDefault, "BindingExpression should not have Default propagation");

        // resolve "property-default" update trigger
        if (updateTrigger == UpdateSourceTrigger.Default)
        {
            UpdateSourceTrigger ust = GetDefaultUpdateSourceTrigger(fwMetaData);

            SetUpdateSourceTrigger(ust);
        }

        Debug.Assert((_flags & PrivateFlags.iUpdateMask) != PrivateFlags.iUpdateDefault, "BindingExpression should not have Default update trigger");
    }

    // return the effective update trigger, used when binding doesn't set one explicitly
    private UpdateSourceTrigger GetDefaultUpdateSourceTrigger(FrameworkPropertyMetadata fwMetaData)
    {
        if (IsInMultiBindingExpression)
        {
            return UpdateSourceTrigger.Explicit;
        }

        return fwMetaData?.DefaultUpdateSourceTrigger ?? UpdateSourceTrigger.PropertyChanged;
    }

    private void SetUpdateSourceTrigger(UpdateSourceTrigger ust)
    {
        ChangeFlag(PrivateFlags.iUpdateMask, false);
        ChangeFlag((PrivateFlags)BindingBase.FlagsFrom(ust), true);
    }

    internal void SaveDefaultFlags() => _defaultFlags = _flags;

    internal Type GetEffectiveTargetType()
    {
        Type targetType = TargetProperty.PropertyType;
        BindingExpressionBase be = ParentBindingExpressionBase;

        while (be is not null)
        {
            if (be is MultiBindingExpression)
            {
                // for descendants of a MultiBinding, the effective target
                // type is Object.
                targetType = typeof(object);
                break;
            }

            be = be.ParentBindingExpressionBase;
        }

        return targetType;
    }

    private void DetermineEffectiveValidatesOnNotifyDataErrors()
    {
        bool result = ParentBindingBase.ValidatesOnNotifyDataErrorsInternal;
        BindingExpressionBase beb = ParentBindingExpressionBase;
        while (result && beb is not null)
        {
            result = beb.ValidatesOnNotifyDataErrors;
            beb = beb.ParentBindingExpressionBase;
        }
        ChangeFlag(PrivateFlags.iValidatesOnNotifyDataErrors, result);
    }

    private bool TestFlag(PrivateFlags flag) => (_flags & flag) != 0;

    private void ChangeFlag(PrivateFlags flag, bool value)
    {
        if (value)
        {
            _flags |= flag;
        }
        else
        {
            _flags &= ~flag;
        }
    }
}
