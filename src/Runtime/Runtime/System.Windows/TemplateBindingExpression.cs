

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
using System.Diagnostics;
using CSHTML5.Internal;
using OpenSilver.Internal.Data;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public class TemplateBindingExpression : Expression
    {
        private IPropertyChangedListener _listener;
        private bool _skipTypeCheck;

        internal Control Source { get; }
        internal DependencyProperty SourceProperty { get; }
        
        internal DependencyObject Target { get; private set; }
        internal DependencyProperty TargetProperty { get; private set; }

        internal TemplateBindingExpression(Control templatedParent, DependencyProperty sourceDP)
        {
            Source = templatedParent ?? throw new ArgumentNullException(nameof(templatedParent));
            SourceProperty = sourceDP ?? throw new ArgumentNullException(nameof(sourceDP));
        }

        internal override bool CanSetValue(DependencyObject d, DependencyProperty dp)
        {
            return false;
        }

        internal override object GetValue(DependencyObject d, DependencyProperty dp)
        {
            var value = Source.GetValue(SourceProperty);
            if (_skipTypeCheck || DependencyProperty.IsValueTypeValid(value, dp.PropertyType))
            {
                return value;
            }

            // Note: consider caching the default value as we should always have d == Target.
            return TargetProperty.GetMetadata(Target.GetType()).DefaultValue; 
        }

        internal override void OnAttach(DependencyObject d, DependencyProperty dp)
        {
            if (IsAttached)
                return;

            Debug.Assert(d != null);
            Debug.Assert(dp != null);

            IsAttached = true;

            Target = d;
            TargetProperty = dp;

            _skipTypeCheck = TargetProperty.PropertyType.IsAssignableFrom(SourceProperty.PropertyType);
            _listener = INTERNAL_PropertyStore.ListenToChanged(Source, SourceProperty, 
                (o, args) => Target.ApplyExpression(TargetProperty, this, false));
        }

        internal override void OnDetach(DependencyObject d, DependencyProperty dp)
        {
            if (!IsAttached)
                return;

            IsAttached = false;

            _skipTypeCheck = false;
            var listener = _listener;
            _listener = null;
            listener?.Detach();
        }

        internal override void SetValue(DependencyObject d, DependencyProperty dp, object value)
        {
            return;
        }
    }
}
