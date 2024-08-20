
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

using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using OpenSilver.Internal.Data;

namespace System.Windows.Data;

public abstract class BindingExpressionBase : Expression
{
    [Flags]
    internal enum PrivateFlags
    {
        iSourceToTarget = 0x00000001,
        iTargetToSource = 0x00000002,
        iInTransfer = 0x00000004,
        iInUpdate = 0x00000008,
        iNeedDataTransfer = 0x00000010,   // used by MultiBindingExpression
        iTransferDeferred = 0x00000020,   // used by MultiBindingExpression
        iUpdateOnLostFocus = 0x00000040,
        iUpdateExplicitly = 0x00000080,
        iUpdateOnPropertyChanged = 0x00000100,
        iNeedsUpdate = 0x00000200,
        iDetaching = 0x00000400,
        iInMultiBindingExpression = 0x00000800,
        iNotifyOnValidationError = 0x00001000,
        iAttaching = 0x00002000,
        iValidatesOnExceptions = 0x00004000,
        iValidatesOnDataErrors = 0x00008000,
        iValidatesOnNotifyDataErrors = 0x00010000,

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

    private PrivateFlags _flags;
    private DependencyPropertyChangedListener _targetPropertyListener;

    internal BindingExpressionBase(BindingBase binding, BindingExpressionBase parent)
    {
        ParentBindingBase = binding;
        ParentBindingExpressionBase = parent;

        _flags = (PrivateFlags)binding.Flags;
    }

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

    /// <summary> True if this binding expression updates the target </summary>
    internal bool IsDynamic =>
        TestFlag(PrivateFlags.iSourceToTarget) && (!IsInMultiBindingExpression || ParentBindingExpressionBase.IsDynamic);

    /// <summary> True if this binding expression updates the source </summary>
    internal bool IsReflective =>
        TestFlag(PrivateFlags.iTargetToSource) && (!IsInMultiBindingExpression || ParentBindingExpressionBase.IsReflective);

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
    internal bool IsInMultiBindingExpression
    {
        get => TestFlag(PrivateFlags.iInMultiBindingExpression);
        set => ChangeFlag(PrivateFlags.iInMultiBindingExpression, value);
    }

    /// <summary> True if this binding expression validates on exceptions </summary>
    internal bool ValidatesOnExceptions => TestFlag(PrivateFlags.iValidatesOnExceptions);

    /// <summary> True if this binding expression validates on data errors </summary>
    internal bool ValidatesOnDataErrors => TestFlag(PrivateFlags.iValidatesOnDataErrors);

    /// <summary> True if this binding expression validates on notify data errors </summary>
    internal bool ValidatesOnNotifyDataErrors => TestFlag(PrivateFlags.iValidatesOnNotifyDataErrors);

    /// <summary>
    /// Invalidate the given child expression.
    /// </summary>
    internal abstract void InvalidateChild(BindingExpressionBase bindingExpression);

    // transfer a value from the source to the target
    internal void Invalidate()
    {
        // don't invalidate during Attach.  The property engine does it already.
        if (IsAttaching) return;

        Target.ApplyExpression(TargetProperty, this);
    }

    internal sealed override void OnAttach(DependencyObject d, DependencyProperty dp) => Attach(d, dp);

    internal void Attach(DependencyObject d, DependencyProperty dp)
    {
        IsAttaching = true;
        AttachOverride(d, dp);
        IsAttaching = false;
    }

    /// <summary>
    /// Attach the binding expression to the given target object and property.
    /// Derived classes should call base.AttachOverride before doing their work,
    /// and should continue only if it returns true.
    /// </summary>
    internal virtual void AttachOverride(DependencyObject d, DependencyProperty dp)
    {
        Target = d;
        TargetProperty = dp;

        ResolvePropertyDefaultSettings(ParentBindingBase.UpdateSourceTriggerInternal, Target, TargetProperty);
        DetermineEffectiveValidatesOnNotifyDataErrors();

        // Listen to changes on the Target if the Binding is TwoWay:
        if (IsReflective && IsUpdateOnPropertyChanged)
        {
            if (IsUpdateOnLostFocus && Target is UIElement uie)
            {
                uie.LostFocus += new RoutedEventHandler(OnTargetLostFocus);
            }

            _targetPropertyListener = new DependencyPropertyChangedListener(Target, TargetProperty, OnTargetPropertyChanged);
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
        if (_targetPropertyListener != null)
        {
            _targetPropertyListener.Dispose();
            _targetPropertyListener = null;
        }

        if (IsUpdateOnLostFocus && Target is UIElement uie)
        {
            uie.LostFocus -= new RoutedEventHandler(OnTargetLostFocus);
        }

        Target = null;
        TargetProperty = null;

        _flags = (PrivateFlags)ParentBindingBase.Flags;
    }

    private void OnTargetLostFocus(object sender, RoutedEventArgs e) => Update();

    private void OnTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        if (IsInTransfer || IsInUpdate)
        {
            return;
        }

        NeedsUpdate = true;

        if (IsUpdateOnLostFocus && ReferenceEquals(FocusManager.GetFocusedElement(), Target))
        {
            return;
        }

        Update();
    }

    internal virtual void Update() { }

    /// <summary>
    /// Create a format that is suitable for String.Format
    /// </summary>
    /// <param name="stringFormat"></param>
    /// <returns></returns>
    internal static string GetEffectiveStringFormat(string stringFormat)
    {
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

    private void ResolvePropertyDefaultSettings(UpdateSourceTrigger updateTrigger, DependencyObject target, DependencyProperty targetProperty)
    {
        // resolve "property-default" update trigger
        if (updateTrigger == UpdateSourceTrigger.Default)
        {
            UpdateSourceTrigger ust = GetDefaultUpdateSourceTrigger(target, targetProperty);

            SetUpdateSourceTrigger(ust);
        }
    }

    // return the effective update trigger, used when binding doesn't set one explicitly
    private UpdateSourceTrigger GetDefaultUpdateSourceTrigger(DependencyObject target, DependencyProperty targetProperty)
    {
        if (IsInMultiBindingExpression)
        {
            return UpdateSourceTrigger.Explicit;
        }

        if ((target is TextBox && targetProperty == TextBox.TextProperty) ||
            (target is PasswordBox && targetProperty == PasswordBox.PasswordProperty))
        {
            return UpdateSourceTrigger.LostFocus;
        }

        return UpdateSourceTrigger.PropertyChanged;
    }

    private void SetUpdateSourceTrigger(UpdateSourceTrigger ust)
    {
        ChangeFlag(PrivateFlags.iUpdateMask, false);
        ChangeFlag((PrivateFlags)BindingBase.FlagsFrom(ust), true);
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
