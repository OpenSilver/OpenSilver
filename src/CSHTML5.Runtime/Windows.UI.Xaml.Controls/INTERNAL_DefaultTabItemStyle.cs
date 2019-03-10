
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Controls
{
    internal class INTERNAL_DefaultTabItemStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_95cfbe95b93f453c8f7af65ff3d99e8f = new global::System.Windows.Style();
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.TargetType = typeof(global::System.Windows.Controls.TabItem);
                var Setter_7af520bb291b4f93bf7bc687724aedf0 = new global::System.Windows.Setter();
                Setter_7af520bb291b4f93bf7bc687724aedf0.Property = global::System.Windows.Controls.TabItem.BackgroundProperty;
                Setter_7af520bb291b4f93bf7bc687724aedf0.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"White");

                var Setter_f027524263264347abe938ddaccfc1a1 = new global::System.Windows.Setter();
                Setter_f027524263264347abe938ddaccfc1a1.Property = global::System.Windows.Controls.TabItem.BorderBrushProperty;
                Setter_f027524263264347abe938ddaccfc1a1.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFDDDDDD");

                var Setter_407e53bf9198479080a4430bebc78308 = new global::System.Windows.Setter();
                Setter_407e53bf9198479080a4430bebc78308.Property = global::System.Windows.Controls.TabItem.BorderThicknessProperty;
                Setter_407e53bf9198479080a4430bebc78308.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"2");

                var Setter_582836c32d554d1ea11fb5f18e3d8a42 = new global::System.Windows.Setter();
                Setter_582836c32d554d1ea11fb5f18e3d8a42.Property = global::System.Windows.Controls.TabItem.CursorProperty;
                Setter_582836c32d554d1ea11fb5f18e3d8a42.Value = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Hand");

                var Setter_45bb5804039b4614babe5fd76b8e641c = new global::System.Windows.Setter();
                Setter_45bb5804039b4614babe5fd76b8e641c.Property = global::System.Windows.Controls.TabItem.ForegroundProperty;
                Setter_45bb5804039b4614babe5fd76b8e641c.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Black");

                var Setter_69e7128639184661adfc1fd5fadde4cc = new global::System.Windows.Setter();
                Setter_69e7128639184661adfc1fd5fadde4cc.Property = global::System.Windows.Controls.TabItem.HorizontalContentAlignmentProperty;
                Setter_69e7128639184661adfc1fd5fadde4cc.Value = global::System.Windows.HorizontalAlignment.Stretch;

                var Setter_22ddfce0f4934110a3bbff27462049d0 = new global::System.Windows.Setter();
                Setter_22ddfce0f4934110a3bbff27462049d0.Property = global::System.Windows.Controls.TabItem.VerticalContentAlignmentProperty;
                Setter_22ddfce0f4934110a3bbff27462049d0.Value = global::System.Windows.VerticalAlignment.Stretch;

                var Setter_4aa03ecd73154df6969293a8d3239af8 = new global::System.Windows.Setter();
                Setter_4aa03ecd73154df6969293a8d3239af8.Property = global::System.Windows.Controls.TabItem.MarginProperty;
                Setter_4aa03ecd73154df6969293a8d3239af8.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0,0,5,0");

                var Setter_10f5a0a3d6e145deae947ba1487fba31 = new global::System.Windows.Setter();
                Setter_10f5a0a3d6e145deae947ba1487fba31.Property = global::System.Windows.Controls.TabItem.PaddingProperty;
                Setter_10f5a0a3d6e145deae947ba1487fba31.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"6,0,6,3");

                var Setter_44bdfd1ae5ce4fedb64c1dfea251d3c2 = new global::System.Windows.Setter();
                Setter_44bdfd1ae5ce4fedb64c1dfea251d3c2.Property = global::System.Windows.Controls.TabItem.SelectedBackgroundProperty;
                Setter_44bdfd1ae5ce4fedb64c1dfea251d3c2.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"White");

                var Setter_0baf9845141545b7a5501b39f6c85811 = new global::System.Windows.Setter();
                Setter_0baf9845141545b7a5501b39f6c85811.Property = global::System.Windows.Controls.TabItem.SelectedForegroundProperty;
                Setter_0baf9845141545b7a5501b39f6c85811.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Black");

                var Setter_2cf8cc1a9d76429ca9584e6295c44736 = new global::System.Windows.Setter();
                Setter_2cf8cc1a9d76429ca9584e6295c44736.Property = global::System.Windows.Controls.TabItem.SelectedAccentProperty;
                Setter_2cf8cc1a9d76429ca9584e6295c44736.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Blue");

                var Setter_4f66919b2e374844aa5985d932398b89 = new global::System.Windows.Setter();
                Setter_4f66919b2e374844aa5985d932398b89.Property = global::System.Windows.Controls.TabItem.TemplateProperty;
                var ControlTemplate_e5e83c6b034b42ac9c9f912628611c6e = new global::System.Windows.Controls.ControlTemplate();
                ControlTemplate_e5e83c6b034b42ac9c9f912628611c6e.TargetType = typeof(global::System.Windows.Controls.TabItem);
                ControlTemplate_e5e83c6b034b42ac9c9f912628611c6e.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_e5e83c6b034b42ac9c9f912628611c6e);

                Setter_4f66919b2e374844aa5985d932398b89.Value = ControlTemplate_e5e83c6b034b42ac9c9f912628611c6e;


                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_7af520bb291b4f93bf7bc687724aedf0);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_f027524263264347abe938ddaccfc1a1);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_407e53bf9198479080a4430bebc78308);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_582836c32d554d1ea11fb5f18e3d8a42);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_45bb5804039b4614babe5fd76b8e641c);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_69e7128639184661adfc1fd5fadde4cc);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_22ddfce0f4934110a3bbff27462049d0);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_4aa03ecd73154df6969293a8d3239af8);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_10f5a0a3d6e145deae947ba1487fba31);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_44bdfd1ae5ce4fedb64c1dfea251d3c2);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_0baf9845141545b7a5501b39f6c85811);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_2cf8cc1a9d76429ca9584e6295c44736);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_4f66919b2e374844aa5985d932398b89);


                DefaultStyle = Style_95cfbe95b93f453c8f7af65ff3d99e8f;
            }

            return DefaultStyle;
        }

        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_b89ffbe13c2348999b5b8639a41775d6(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_b89ffbe13c2348999b5b8639a41775d6(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_b89ffbe13c2348999b5b8639a41775d6(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_b89ffbe13c2348999b5b8639a41775d6(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_e5e83c6b034b42ac9c9f912628611c6e(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_3da87ad11c964413a3705dab8af9e658 = new global::System.Windows.TemplateInstance();
            templateInstance_3da87ad11c964413a3705dab8af9e658.TemplateOwner = templateOwner;
            var StackPanel_c3d071e2b99344e3b5e3afcfd722d31b = new global::System.Windows.Controls.StackPanel();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Root", StackPanel_c3d071e2b99344e3b5e3afcfd722d31b);
            StackPanel_c3d071e2b99344e3b5e3afcfd722d31b.Name = "Root";
            var VisualStateGroup_29024679c1894937b3928740793536eb = new global::System.Windows.VisualStateGroup();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_29024679c1894937b3928740793536eb);
            VisualStateGroup_29024679c1894937b3928740793536eb.Name = "CommonStates";
            var VisualState_e8742488491a4e0697dc253e48ea7280 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Normal", VisualState_e8742488491a4e0697dc253e48ea7280);
            VisualState_e8742488491a4e0697dc253e48ea7280.Name = "Normal";

            var VisualState_8e3cf897eeaf42abbebbe0fff1d4e116 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("MouseOver", VisualState_8e3cf897eeaf42abbebbe0fff1d4e116);
            VisualState_8e3cf897eeaf42abbebbe0fff1d4e116.Name = "MouseOver";
            var Storyboard_d99ff9dac79d4298b4c87513f019ee8a = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_c3ababf68a5a40efa04943e4c65482b5 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            ObjectAnimationUsingKeyFrames_c3ababf68a5a40efa04943e4c65482b5.Duration = (global::System.Windows.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Duration), @"0");
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_c3ababf68a5a40efa04943e4c65482b5, @"PointerOverBorder");
            var DiscreteObjectKeyFrame_8b8db01deb5447018bcd29b4d83be750 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_8b8db01deb5447018bcd29b4d83be750.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_8b8db01deb5447018bcd29b4d83be750.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#FFCFCFCF");

            ObjectAnimationUsingKeyFrames_c3ababf68a5a40efa04943e4c65482b5.KeyFrames.Add(DiscreteObjectKeyFrame_8b8db01deb5447018bcd29b4d83be750);


            Storyboard_d99ff9dac79d4298b4c87513f019ee8a.Children.Add(ObjectAnimationUsingKeyFrames_c3ababf68a5a40efa04943e4c65482b5);


            VisualState_8e3cf897eeaf42abbebbe0fff1d4e116.Storyboard = Storyboard_d99ff9dac79d4298b4c87513f019ee8a;


            var VisualState_c70211b4d7cb4a319b606345de0130f3 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_c70211b4d7cb4a319b606345de0130f3);
            VisualState_c70211b4d7cb4a319b606345de0130f3.Name = "Disabled";

            VisualStateGroup_29024679c1894937b3928740793536eb.States.Add(VisualState_e8742488491a4e0697dc253e48ea7280);
            VisualStateGroup_29024679c1894937b3928740793536eb.States.Add(VisualState_8e3cf897eeaf42abbebbe0fff1d4e116);
            VisualStateGroup_29024679c1894937b3928740793536eb.States.Add(VisualState_c70211b4d7cb4a319b606345de0130f3);


            var VisualStateGroup_f12469de104b4634888a746b830803e9 = new global::System.Windows.VisualStateGroup();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("SelectionStates", VisualStateGroup_f12469de104b4634888a746b830803e9);
            VisualStateGroup_f12469de104b4634888a746b830803e9.Name = "SelectionStates";
            var VisualState_025faba6c17c42378217aef5db2812d6 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Unselected", VisualState_025faba6c17c42378217aef5db2812d6);
            VisualState_025faba6c17c42378217aef5db2812d6.Name = "Unselected";

            var VisualState_acff7d85a7f2493eac0bbe01fd883f79 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Selected", VisualState_acff7d85a7f2493eac0bbe01fd883f79);
            VisualState_acff7d85a7f2493eac0bbe01fd883f79.Name = "Selected";

            VisualStateGroup_f12469de104b4634888a746b830803e9.States.Add(VisualState_025faba6c17c42378217aef5db2812d6);
            VisualStateGroup_f12469de104b4634888a746b830803e9.States.Add(VisualState_acff7d85a7f2493eac0bbe01fd883f79);


            ((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_29024679c1894937b3928740793536eb);
            ((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_f12469de104b4634888a746b830803e9);

            var Border_1c9f7d37162a4e2398694a5d95518710 = new global::System.Windows.Controls.Border();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("TemplateTopSelected", Border_1c9f7d37162a4e2398694a5d95518710);
            Border_1c9f7d37162a4e2398694a5d95518710.Name = "TemplateTopSelected";
            Border_1c9f7d37162a4e2398694a5d95518710.Visibility = global::System.Windows.Visibility.Collapsed;
            Border_1c9f7d37162a4e2398694a5d95518710.BorderThickness = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1,0,1,0");
            Border_1c9f7d37162a4e2398694a5d95518710.CornerRadius = (global::System.Windows.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.CornerRadius), @"3,3,0,0");
            Border_1c9f7d37162a4e2398694a5d95518710.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");
            var StackPanel_ea3097e279f24d96b531d7ff938602a7 = new global::System.Windows.Controls.StackPanel();
            var Border_52581094754d48e6a1096a407162082f = new global::System.Windows.Controls.Border();
            Border_52581094754d48e6a1096a407162082f.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"3");
            Border_52581094754d48e6a1096a407162082f.CornerRadius = (global::System.Windows.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.CornerRadius), @"3,3,0,0");
            var Binding_9ade1692dc3c41b787ee53a098ce2dc3 = new global::System.Windows.Data.Binding();
            Binding_9ade1692dc3c41b787ee53a098ce2dc3.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"SelectedAccent");
            var RelativeSource_4e8c241f19cb4c37b1d90e859e77d422 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_4e8c241f19cb4c37b1d90e859e77d422.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_9ade1692dc3c41b787ee53a098ce2dc3.RelativeSource = RelativeSource_4e8c241f19cb4c37b1d90e859e77d422;


            Binding_9ade1692dc3c41b787ee53a098ce2dc3.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;


            var ContentControl_1d2f190c866e40f2a77daad51dfcb782 = new global::System.Windows.Controls.ContentControl();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("HeaderTopSelected", ContentControl_1d2f190c866e40f2a77daad51dfcb782);
            ContentControl_1d2f190c866e40f2a77daad51dfcb782.Name = "HeaderTopSelected";
            var Binding_cb9ff518b0304751a3b895b8887bc448 = new global::System.Windows.Data.Binding();
            Binding_cb9ff518b0304751a3b895b8887bc448.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"SelectedForeground");
            var RelativeSource_6956fd27ca774504acc85aa1235895aa = new global::System.Windows.Data.RelativeSource();
            RelativeSource_6956fd27ca774504acc85aa1235895aa.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_cb9ff518b0304751a3b895b8887bc448.RelativeSource = RelativeSource_6956fd27ca774504acc85aa1235895aa;


            Binding_cb9ff518b0304751a3b895b8887bc448.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;

            var Binding_bf02fee274314e7daf3f8a7ac0298a21 = new global::System.Windows.Data.Binding();
            Binding_bf02fee274314e7daf3f8a7ac0298a21.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Padding");
            var RelativeSource_7cd35505f00c4ae58e8089b5c2fe910d = new global::System.Windows.Data.RelativeSource();
            RelativeSource_7cd35505f00c4ae58e8089b5c2fe910d.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_bf02fee274314e7daf3f8a7ac0298a21.RelativeSource = RelativeSource_7cd35505f00c4ae58e8089b5c2fe910d;


            Binding_bf02fee274314e7daf3f8a7ac0298a21.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;


            StackPanel_ea3097e279f24d96b531d7ff938602a7.Children.Add(Border_52581094754d48e6a1096a407162082f);
            StackPanel_ea3097e279f24d96b531d7ff938602a7.Children.Add(ContentControl_1d2f190c866e40f2a77daad51dfcb782);


            Border_1c9f7d37162a4e2398694a5d95518710.Child = StackPanel_ea3097e279f24d96b531d7ff938602a7;

            var Binding_a1ee06c689264ec2854399504a411bd9 = new global::System.Windows.Data.Binding();
            Binding_a1ee06c689264ec2854399504a411bd9.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
            var RelativeSource_281574b47b8e49fbac0c04e15117efd7 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_281574b47b8e49fbac0c04e15117efd7.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_a1ee06c689264ec2854399504a411bd9.RelativeSource = RelativeSource_281574b47b8e49fbac0c04e15117efd7;


            Binding_a1ee06c689264ec2854399504a411bd9.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;

            var Binding_a3ece6dde1794a22a6a0a7ed7d3b9372 = new global::System.Windows.Data.Binding();
            Binding_a3ece6dde1794a22a6a0a7ed7d3b9372.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"SelectedBackground");
            var RelativeSource_657fae32242f4f8cb6ce9c559be8b964 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_657fae32242f4f8cb6ce9c559be8b964.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_a3ece6dde1794a22a6a0a7ed7d3b9372.RelativeSource = RelativeSource_657fae32242f4f8cb6ce9c559be8b964;


            Binding_a3ece6dde1794a22a6a0a7ed7d3b9372.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;


            var Border_1f378c14ffa94fb58dbef81e1ab0b975 = new global::System.Windows.Controls.Border();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("TemplateTopUnselected", Border_1f378c14ffa94fb58dbef81e1ab0b975);
            Border_1f378c14ffa94fb58dbef81e1ab0b975.Name = "TemplateTopUnselected";
            Border_1f378c14ffa94fb58dbef81e1ab0b975.Visibility = global::System.Windows.Visibility.Collapsed;
            Border_1f378c14ffa94fb58dbef81e1ab0b975.BorderThickness = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1,1,1,0");
            Border_1f378c14ffa94fb58dbef81e1ab0b975.CornerRadius = (global::System.Windows.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.CornerRadius), @"3,3,0,0");
            var StackPanel_f8649fb6054a43368d71a1011b55eb10 = new global::System.Windows.Controls.StackPanel();
            var Border_2ecffd2b4dda4eb7a5a7a821f6e41f4c = new global::System.Windows.Controls.Border();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("PointerOverBorder", Border_2ecffd2b4dda4eb7a5a7a821f6e41f4c);
            Border_2ecffd2b4dda4eb7a5a7a821f6e41f4c.Name = "PointerOverBorder";
            Border_2ecffd2b4dda4eb7a5a7a821f6e41f4c.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"2");
            Border_2ecffd2b4dda4eb7a5a7a821f6e41f4c.CornerRadius = (global::System.Windows.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.CornerRadius), @"3,3,0,0");
            Border_2ecffd2b4dda4eb7a5a7a821f6e41f4c.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Transparent");

            var ContentControl_142ea7c5a80b448784d9d96001cc0662 = new global::System.Windows.Controls.ContentControl();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("HeaderTopUnselected", ContentControl_142ea7c5a80b448784d9d96001cc0662);
            ContentControl_142ea7c5a80b448784d9d96001cc0662.Name = "HeaderTopUnselected";
            var Binding_77b1e0a7455c4dddb40b2e6eab7af099 = new global::System.Windows.Data.Binding();
            Binding_77b1e0a7455c4dddb40b2e6eab7af099.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Foreground");
            var RelativeSource_5eac48ccd8c94b2b9cb54f8f82daea94 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_5eac48ccd8c94b2b9cb54f8f82daea94.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_77b1e0a7455c4dddb40b2e6eab7af099.RelativeSource = RelativeSource_5eac48ccd8c94b2b9cb54f8f82daea94;


            Binding_77b1e0a7455c4dddb40b2e6eab7af099.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;

            var Binding_fa27688241824117955982db410156ee = new global::System.Windows.Data.Binding();
            Binding_fa27688241824117955982db410156ee.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Padding");
            var RelativeSource_5ae045248cd54c41a314348fe8b948d8 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_5ae045248cd54c41a314348fe8b948d8.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_fa27688241824117955982db410156ee.RelativeSource = RelativeSource_5ae045248cd54c41a314348fe8b948d8;


            Binding_fa27688241824117955982db410156ee.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;


            StackPanel_f8649fb6054a43368d71a1011b55eb10.Children.Add(Border_2ecffd2b4dda4eb7a5a7a821f6e41f4c);
            StackPanel_f8649fb6054a43368d71a1011b55eb10.Children.Add(ContentControl_142ea7c5a80b448784d9d96001cc0662);


            Border_1f378c14ffa94fb58dbef81e1ab0b975.Child = StackPanel_f8649fb6054a43368d71a1011b55eb10;

            var Binding_fef019e77bb542188a7a72373aa38261 = new global::System.Windows.Data.Binding();
            Binding_fef019e77bb542188a7a72373aa38261.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
            var RelativeSource_9afba0e8ef2b4bf2bdef0fc2cef2aa96 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_9afba0e8ef2b4bf2bdef0fc2cef2aa96.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_fef019e77bb542188a7a72373aa38261.RelativeSource = RelativeSource_9afba0e8ef2b4bf2bdef0fc2cef2aa96;


            Binding_fef019e77bb542188a7a72373aa38261.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;

            var Binding_3e5444e2342b4dfa820fd80c24481e6f = new global::System.Windows.Data.Binding();
            Binding_3e5444e2342b4dfa820fd80c24481e6f.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_15b195ed5b364effb9916850f81d5f0c = new global::System.Windows.Data.RelativeSource();
            RelativeSource_15b195ed5b364effb9916850f81d5f0c.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_3e5444e2342b4dfa820fd80c24481e6f.RelativeSource = RelativeSource_15b195ed5b364effb9916850f81d5f0c;


            Binding_3e5444e2342b4dfa820fd80c24481e6f.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;

            var Binding_c3d87c85c7fe429c905d62c8d78e1b0f = new global::System.Windows.Data.Binding();
            Binding_c3d87c85c7fe429c905d62c8d78e1b0f.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Cursor");
            var RelativeSource_50a435e405d84a27a5f97afb7f95cca0 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_50a435e405d84a27a5f97afb7f95cca0.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_c3d87c85c7fe429c905d62c8d78e1b0f.RelativeSource = RelativeSource_50a435e405d84a27a5f97afb7f95cca0;


            Binding_c3d87c85c7fe429c905d62c8d78e1b0f.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;


            StackPanel_c3d071e2b99344e3b5e3afcfd722d31b.Children.Add(Border_1c9f7d37162a4e2398694a5d95518710);
            StackPanel_c3d071e2b99344e3b5e3afcfd722d31b.Children.Add(Border_1f378c14ffa94fb58dbef81e1ab0b975);



            Border_52581094754d48e6a1096a407162082f.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_9ade1692dc3c41b787ee53a098ce2dc3);
            ContentControl_1d2f190c866e40f2a77daad51dfcb782.SetBinding(global::System.Windows.Controls.Control.ForegroundProperty, Binding_cb9ff518b0304751a3b895b8887bc448);
            ContentControl_1d2f190c866e40f2a77daad51dfcb782.SetBinding(global::System.Windows.FrameworkElement.MarginProperty, Binding_bf02fee274314e7daf3f8a7ac0298a21);
            Border_1c9f7d37162a4e2398694a5d95518710.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_a1ee06c689264ec2854399504a411bd9);
            Border_1c9f7d37162a4e2398694a5d95518710.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_a3ece6dde1794a22a6a0a7ed7d3b9372);
            ContentControl_142ea7c5a80b448784d9d96001cc0662.SetBinding(global::System.Windows.Controls.Control.ForegroundProperty, Binding_77b1e0a7455c4dddb40b2e6eab7af099);
            ContentControl_142ea7c5a80b448784d9d96001cc0662.SetBinding(global::System.Windows.FrameworkElement.MarginProperty, Binding_fa27688241824117955982db410156ee);
            Border_1f378c14ffa94fb58dbef81e1ab0b975.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_fef019e77bb542188a7a72373aa38261);
            Border_1f378c14ffa94fb58dbef81e1ab0b975.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_3e5444e2342b4dfa820fd80c24481e6f);
            Border_1f378c14ffa94fb58dbef81e1ab0b975.SetBinding(global::System.Windows.FrameworkElement.CursorProperty, Binding_c3d87c85c7fe429c905d62c8d78e1b0f);

            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_c3ababf68a5a40efa04943e4c65482b5,
                new global::System.Windows.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_b89ffbe13c2348999b5b8639a41775d6,
                    setVisualStateProperty_b89ffbe13c2348999b5b8639a41775d6,
                    setLocalVisualStateProperty_b89ffbe13c2348999b5b8639a41775d6,
                    getVisualStateProperty_b89ffbe13c2348999b5b8639a41775d6));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_c3ababf68a5a40efa04943e4c65482b5, Border_2ecffd2b4dda4eb7a5a7a821f6e41f4c);

            templateInstance_3da87ad11c964413a3705dab8af9e658.TemplateContent = StackPanel_c3d071e2b99344e3b5e3afcfd722d31b;
            return templateInstance_3da87ad11c964413a3705dab8af9e658;
        }

    }
}
#else
namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultTabItemStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_95cfbe95b93f453c8f7af65ff3d99e8f = new global::Windows.UI.Xaml.Style();
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.TargetType = typeof(global::Windows.UI.Xaml.Controls.TabItem);
                var Setter_7af520bb291b4f93bf7bc687724aedf0 = new global::Windows.UI.Xaml.Setter();
                Setter_7af520bb291b4f93bf7bc687724aedf0.Property = global::Windows.UI.Xaml.Controls.TabItem.BackgroundProperty;
                Setter_7af520bb291b4f93bf7bc687724aedf0.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"White");

                var Setter_f027524263264347abe938ddaccfc1a1 = new global::Windows.UI.Xaml.Setter();
                Setter_f027524263264347abe938ddaccfc1a1.Property = global::Windows.UI.Xaml.Controls.TabItem.BorderBrushProperty;
                Setter_f027524263264347abe938ddaccfc1a1.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFDDDDDD");

                var Setter_407e53bf9198479080a4430bebc78308 = new global::Windows.UI.Xaml.Setter();
                Setter_407e53bf9198479080a4430bebc78308.Property = global::Windows.UI.Xaml.Controls.TabItem.BorderThicknessProperty;
                Setter_407e53bf9198479080a4430bebc78308.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"2");

                var Setter_582836c32d554d1ea11fb5f18e3d8a42 = new global::Windows.UI.Xaml.Setter();
                Setter_582836c32d554d1ea11fb5f18e3d8a42.Property = global::Windows.UI.Xaml.Controls.TabItem.CursorProperty;
                Setter_582836c32d554d1ea11fb5f18e3d8a42.Value = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Hand");

                var Setter_45bb5804039b4614babe5fd76b8e641c = new global::Windows.UI.Xaml.Setter();
                Setter_45bb5804039b4614babe5fd76b8e641c.Property = global::Windows.UI.Xaml.Controls.TabItem.ForegroundProperty;
                Setter_45bb5804039b4614babe5fd76b8e641c.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Black");

                var Setter_69e7128639184661adfc1fd5fadde4cc = new global::Windows.UI.Xaml.Setter();
                Setter_69e7128639184661adfc1fd5fadde4cc.Property = global::Windows.UI.Xaml.Controls.TabItem.HorizontalContentAlignmentProperty;
                Setter_69e7128639184661adfc1fd5fadde4cc.Value = global::Windows.UI.Xaml.HorizontalAlignment.Stretch;

                var Setter_22ddfce0f4934110a3bbff27462049d0 = new global::Windows.UI.Xaml.Setter();
                Setter_22ddfce0f4934110a3bbff27462049d0.Property = global::Windows.UI.Xaml.Controls.TabItem.VerticalContentAlignmentProperty;
                Setter_22ddfce0f4934110a3bbff27462049d0.Value = global::Windows.UI.Xaml.VerticalAlignment.Stretch;

                var Setter_4aa03ecd73154df6969293a8d3239af8 = new global::Windows.UI.Xaml.Setter();
                Setter_4aa03ecd73154df6969293a8d3239af8.Property = global::Windows.UI.Xaml.Controls.TabItem.MarginProperty;
                Setter_4aa03ecd73154df6969293a8d3239af8.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0,0,5,0");

                var Setter_10f5a0a3d6e145deae947ba1487fba31 = new global::Windows.UI.Xaml.Setter();
                Setter_10f5a0a3d6e145deae947ba1487fba31.Property = global::Windows.UI.Xaml.Controls.TabItem.PaddingProperty;
                Setter_10f5a0a3d6e145deae947ba1487fba31.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"6,0,6,3");

                var Setter_44bdfd1ae5ce4fedb64c1dfea251d3c2 = new global::Windows.UI.Xaml.Setter();
                Setter_44bdfd1ae5ce4fedb64c1dfea251d3c2.Property = global::Windows.UI.Xaml.Controls.TabItem.SelectedBackgroundProperty;
                Setter_44bdfd1ae5ce4fedb64c1dfea251d3c2.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"White");

                var Setter_0baf9845141545b7a5501b39f6c85811 = new global::Windows.UI.Xaml.Setter();
                Setter_0baf9845141545b7a5501b39f6c85811.Property = global::Windows.UI.Xaml.Controls.TabItem.SelectedForegroundProperty;
                Setter_0baf9845141545b7a5501b39f6c85811.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Black");

                var Setter_2cf8cc1a9d76429ca9584e6295c44736 = new global::Windows.UI.Xaml.Setter();
                Setter_2cf8cc1a9d76429ca9584e6295c44736.Property = global::Windows.UI.Xaml.Controls.TabItem.SelectedAccentProperty;
                Setter_2cf8cc1a9d76429ca9584e6295c44736.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Blue");

                var Setter_4f66919b2e374844aa5985d932398b89 = new global::Windows.UI.Xaml.Setter();
                Setter_4f66919b2e374844aa5985d932398b89.Property = global::Windows.UI.Xaml.Controls.TabItem.TemplateProperty;
                var ControlTemplate_e5e83c6b034b42ac9c9f912628611c6e = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_e5e83c6b034b42ac9c9f912628611c6e.TargetType = typeof(global::Windows.UI.Xaml.Controls.TabItem);
                ControlTemplate_e5e83c6b034b42ac9c9f912628611c6e.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_e5e83c6b034b42ac9c9f912628611c6e);

                Setter_4f66919b2e374844aa5985d932398b89.Value = ControlTemplate_e5e83c6b034b42ac9c9f912628611c6e;


                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_7af520bb291b4f93bf7bc687724aedf0);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_f027524263264347abe938ddaccfc1a1);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_407e53bf9198479080a4430bebc78308);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_582836c32d554d1ea11fb5f18e3d8a42);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_45bb5804039b4614babe5fd76b8e641c);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_69e7128639184661adfc1fd5fadde4cc);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_22ddfce0f4934110a3bbff27462049d0);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_4aa03ecd73154df6969293a8d3239af8);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_10f5a0a3d6e145deae947ba1487fba31);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_44bdfd1ae5ce4fedb64c1dfea251d3c2);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_0baf9845141545b7a5501b39f6c85811);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_2cf8cc1a9d76429ca9584e6295c44736);
                Style_95cfbe95b93f453c8f7af65ff3d99e8f.Setters.Add(Setter_4f66919b2e374844aa5985d932398b89);


                DefaultStyle = Style_95cfbe95b93f453c8f7af65ff3d99e8f;
            }

            return DefaultStyle;
        }

        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_b89ffbe13c2348999b5b8639a41775d6(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_b89ffbe13c2348999b5b8639a41775d6(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_b89ffbe13c2348999b5b8639a41775d6(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_b89ffbe13c2348999b5b8639a41775d6(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_e5e83c6b034b42ac9c9f912628611c6e(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_3da87ad11c964413a3705dab8af9e658 = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_3da87ad11c964413a3705dab8af9e658.TemplateOwner = templateOwner;
            var StackPanel_c3d071e2b99344e3b5e3afcfd722d31b = new global::Windows.UI.Xaml.Controls.StackPanel();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Root", StackPanel_c3d071e2b99344e3b5e3afcfd722d31b);
            StackPanel_c3d071e2b99344e3b5e3afcfd722d31b.Name = "Root";
            var VisualStateGroup_29024679c1894937b3928740793536eb = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_29024679c1894937b3928740793536eb);
            VisualStateGroup_29024679c1894937b3928740793536eb.Name = "CommonStates";
            var VisualState_e8742488491a4e0697dc253e48ea7280 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Normal", VisualState_e8742488491a4e0697dc253e48ea7280);
            VisualState_e8742488491a4e0697dc253e48ea7280.Name = "Normal";

            var VisualState_8e3cf897eeaf42abbebbe0fff1d4e116 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("MouseOver", VisualState_8e3cf897eeaf42abbebbe0fff1d4e116);
            VisualState_8e3cf897eeaf42abbebbe0fff1d4e116.Name = "MouseOver";
            var Storyboard_d99ff9dac79d4298b4c87513f019ee8a = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_c3ababf68a5a40efa04943e4c65482b5 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            ObjectAnimationUsingKeyFrames_c3ababf68a5a40efa04943e4c65482b5.Duration = (global::Windows.UI.Xaml.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Duration), @"0");
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_c3ababf68a5a40efa04943e4c65482b5, @"PointerOverBorder");
            var DiscreteObjectKeyFrame_8b8db01deb5447018bcd29b4d83be750 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_8b8db01deb5447018bcd29b4d83be750.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_8b8db01deb5447018bcd29b4d83be750.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#FFCFCFCF");

            ObjectAnimationUsingKeyFrames_c3ababf68a5a40efa04943e4c65482b5.KeyFrames.Add(DiscreteObjectKeyFrame_8b8db01deb5447018bcd29b4d83be750);


            Storyboard_d99ff9dac79d4298b4c87513f019ee8a.Children.Add(ObjectAnimationUsingKeyFrames_c3ababf68a5a40efa04943e4c65482b5);


            VisualState_8e3cf897eeaf42abbebbe0fff1d4e116.Storyboard = Storyboard_d99ff9dac79d4298b4c87513f019ee8a;


            var VisualState_c70211b4d7cb4a319b606345de0130f3 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_c70211b4d7cb4a319b606345de0130f3);
            VisualState_c70211b4d7cb4a319b606345de0130f3.Name = "Disabled";

            VisualStateGroup_29024679c1894937b3928740793536eb.States.Add(VisualState_e8742488491a4e0697dc253e48ea7280);
            VisualStateGroup_29024679c1894937b3928740793536eb.States.Add(VisualState_8e3cf897eeaf42abbebbe0fff1d4e116);
            VisualStateGroup_29024679c1894937b3928740793536eb.States.Add(VisualState_c70211b4d7cb4a319b606345de0130f3);


            var VisualStateGroup_f12469de104b4634888a746b830803e9 = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("SelectionStates", VisualStateGroup_f12469de104b4634888a746b830803e9);
            VisualStateGroup_f12469de104b4634888a746b830803e9.Name = "SelectionStates";
            var VisualState_025faba6c17c42378217aef5db2812d6 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Unselected", VisualState_025faba6c17c42378217aef5db2812d6);
            VisualState_025faba6c17c42378217aef5db2812d6.Name = "Unselected";

            var VisualState_acff7d85a7f2493eac0bbe01fd883f79 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Selected", VisualState_acff7d85a7f2493eac0bbe01fd883f79);
            VisualState_acff7d85a7f2493eac0bbe01fd883f79.Name = "Selected";

            VisualStateGroup_f12469de104b4634888a746b830803e9.States.Add(VisualState_025faba6c17c42378217aef5db2812d6);
            VisualStateGroup_f12469de104b4634888a746b830803e9.States.Add(VisualState_acff7d85a7f2493eac0bbe01fd883f79);


            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_29024679c1894937b3928740793536eb);
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_f12469de104b4634888a746b830803e9);

            var Border_1c9f7d37162a4e2398694a5d95518710 = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("TemplateTopSelected", Border_1c9f7d37162a4e2398694a5d95518710);
            Border_1c9f7d37162a4e2398694a5d95518710.Name = "TemplateTopSelected";
            Border_1c9f7d37162a4e2398694a5d95518710.Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
            Border_1c9f7d37162a4e2398694a5d95518710.BorderThickness = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1,0,1,0");
            Border_1c9f7d37162a4e2398694a5d95518710.CornerRadius = (global::Windows.UI.Xaml.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.CornerRadius), @"3,3,0,0");
            Border_1c9f7d37162a4e2398694a5d95518710.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");
            var StackPanel_ea3097e279f24d96b531d7ff938602a7 = new global::Windows.UI.Xaml.Controls.StackPanel();
            var Border_52581094754d48e6a1096a407162082f = new global::Windows.UI.Xaml.Controls.Border();
            Border_52581094754d48e6a1096a407162082f.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"3");
            Border_52581094754d48e6a1096a407162082f.CornerRadius = (global::Windows.UI.Xaml.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.CornerRadius), @"3,3,0,0");
            var Binding_9ade1692dc3c41b787ee53a098ce2dc3 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_9ade1692dc3c41b787ee53a098ce2dc3.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"SelectedAccent");
            var RelativeSource_4e8c241f19cb4c37b1d90e859e77d422 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_4e8c241f19cb4c37b1d90e859e77d422.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_9ade1692dc3c41b787ee53a098ce2dc3.RelativeSource = RelativeSource_4e8c241f19cb4c37b1d90e859e77d422;


            Binding_9ade1692dc3c41b787ee53a098ce2dc3.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;


            var ContentControl_1d2f190c866e40f2a77daad51dfcb782 = new global::Windows.UI.Xaml.Controls.ContentControl();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("HeaderTopSelected", ContentControl_1d2f190c866e40f2a77daad51dfcb782);
            ContentControl_1d2f190c866e40f2a77daad51dfcb782.Name = "HeaderTopSelected";
            var Binding_cb9ff518b0304751a3b895b8887bc448 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_cb9ff518b0304751a3b895b8887bc448.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"SelectedForeground");
            var RelativeSource_6956fd27ca774504acc85aa1235895aa = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_6956fd27ca774504acc85aa1235895aa.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_cb9ff518b0304751a3b895b8887bc448.RelativeSource = RelativeSource_6956fd27ca774504acc85aa1235895aa;


            Binding_cb9ff518b0304751a3b895b8887bc448.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;

            var Binding_bf02fee274314e7daf3f8a7ac0298a21 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_bf02fee274314e7daf3f8a7ac0298a21.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Padding");
            var RelativeSource_7cd35505f00c4ae58e8089b5c2fe910d = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_7cd35505f00c4ae58e8089b5c2fe910d.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_bf02fee274314e7daf3f8a7ac0298a21.RelativeSource = RelativeSource_7cd35505f00c4ae58e8089b5c2fe910d;


            Binding_bf02fee274314e7daf3f8a7ac0298a21.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;


            StackPanel_ea3097e279f24d96b531d7ff938602a7.Children.Add(Border_52581094754d48e6a1096a407162082f);
            StackPanel_ea3097e279f24d96b531d7ff938602a7.Children.Add(ContentControl_1d2f190c866e40f2a77daad51dfcb782);


            Border_1c9f7d37162a4e2398694a5d95518710.Child = StackPanel_ea3097e279f24d96b531d7ff938602a7;

            var Binding_a1ee06c689264ec2854399504a411bd9 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_a1ee06c689264ec2854399504a411bd9.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_281574b47b8e49fbac0c04e15117efd7 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_281574b47b8e49fbac0c04e15117efd7.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_a1ee06c689264ec2854399504a411bd9.RelativeSource = RelativeSource_281574b47b8e49fbac0c04e15117efd7;


            Binding_a1ee06c689264ec2854399504a411bd9.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;

            var Binding_a3ece6dde1794a22a6a0a7ed7d3b9372 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_a3ece6dde1794a22a6a0a7ed7d3b9372.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"SelectedBackground");
            var RelativeSource_657fae32242f4f8cb6ce9c559be8b964 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_657fae32242f4f8cb6ce9c559be8b964.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_a3ece6dde1794a22a6a0a7ed7d3b9372.RelativeSource = RelativeSource_657fae32242f4f8cb6ce9c559be8b964;


            Binding_a3ece6dde1794a22a6a0a7ed7d3b9372.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;


            var Border_1f378c14ffa94fb58dbef81e1ab0b975 = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("TemplateTopUnselected", Border_1f378c14ffa94fb58dbef81e1ab0b975);
            Border_1f378c14ffa94fb58dbef81e1ab0b975.Name = "TemplateTopUnselected";
            Border_1f378c14ffa94fb58dbef81e1ab0b975.Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
            Border_1f378c14ffa94fb58dbef81e1ab0b975.BorderThickness = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1,1,1,0");
            Border_1f378c14ffa94fb58dbef81e1ab0b975.CornerRadius = (global::Windows.UI.Xaml.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.CornerRadius), @"3,3,0,0");
            var StackPanel_f8649fb6054a43368d71a1011b55eb10 = new global::Windows.UI.Xaml.Controls.StackPanel();
            var Border_2ecffd2b4dda4eb7a5a7a821f6e41f4c = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("PointerOverBorder", Border_2ecffd2b4dda4eb7a5a7a821f6e41f4c);
            Border_2ecffd2b4dda4eb7a5a7a821f6e41f4c.Name = "PointerOverBorder";
            Border_2ecffd2b4dda4eb7a5a7a821f6e41f4c.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"2");
            Border_2ecffd2b4dda4eb7a5a7a821f6e41f4c.CornerRadius = (global::Windows.UI.Xaml.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.CornerRadius), @"3,3,0,0");
            Border_2ecffd2b4dda4eb7a5a7a821f6e41f4c.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Transparent");

            var ContentControl_142ea7c5a80b448784d9d96001cc0662 = new global::Windows.UI.Xaml.Controls.ContentControl();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("HeaderTopUnselected", ContentControl_142ea7c5a80b448784d9d96001cc0662);
            ContentControl_142ea7c5a80b448784d9d96001cc0662.Name = "HeaderTopUnselected";
            var Binding_77b1e0a7455c4dddb40b2e6eab7af099 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_77b1e0a7455c4dddb40b2e6eab7af099.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Foreground");
            var RelativeSource_5eac48ccd8c94b2b9cb54f8f82daea94 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_5eac48ccd8c94b2b9cb54f8f82daea94.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_77b1e0a7455c4dddb40b2e6eab7af099.RelativeSource = RelativeSource_5eac48ccd8c94b2b9cb54f8f82daea94;


            Binding_77b1e0a7455c4dddb40b2e6eab7af099.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;

            var Binding_fa27688241824117955982db410156ee = new global::Windows.UI.Xaml.Data.Binding();
            Binding_fa27688241824117955982db410156ee.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Padding");
            var RelativeSource_5ae045248cd54c41a314348fe8b948d8 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_5ae045248cd54c41a314348fe8b948d8.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_fa27688241824117955982db410156ee.RelativeSource = RelativeSource_5ae045248cd54c41a314348fe8b948d8;


            Binding_fa27688241824117955982db410156ee.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;


            StackPanel_f8649fb6054a43368d71a1011b55eb10.Children.Add(Border_2ecffd2b4dda4eb7a5a7a821f6e41f4c);
            StackPanel_f8649fb6054a43368d71a1011b55eb10.Children.Add(ContentControl_142ea7c5a80b448784d9d96001cc0662);


            Border_1f378c14ffa94fb58dbef81e1ab0b975.Child = StackPanel_f8649fb6054a43368d71a1011b55eb10;

            var Binding_fef019e77bb542188a7a72373aa38261 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_fef019e77bb542188a7a72373aa38261.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_9afba0e8ef2b4bf2bdef0fc2cef2aa96 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_9afba0e8ef2b4bf2bdef0fc2cef2aa96.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_fef019e77bb542188a7a72373aa38261.RelativeSource = RelativeSource_9afba0e8ef2b4bf2bdef0fc2cef2aa96;


            Binding_fef019e77bb542188a7a72373aa38261.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;

            var Binding_3e5444e2342b4dfa820fd80c24481e6f = new global::Windows.UI.Xaml.Data.Binding();
            Binding_3e5444e2342b4dfa820fd80c24481e6f.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_15b195ed5b364effb9916850f81d5f0c = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_15b195ed5b364effb9916850f81d5f0c.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_3e5444e2342b4dfa820fd80c24481e6f.RelativeSource = RelativeSource_15b195ed5b364effb9916850f81d5f0c;


            Binding_3e5444e2342b4dfa820fd80c24481e6f.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;

            var Binding_c3d87c85c7fe429c905d62c8d78e1b0f = new global::Windows.UI.Xaml.Data.Binding();
            Binding_c3d87c85c7fe429c905d62c8d78e1b0f.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Cursor");
            var RelativeSource_50a435e405d84a27a5f97afb7f95cca0 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_50a435e405d84a27a5f97afb7f95cca0.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_c3d87c85c7fe429c905d62c8d78e1b0f.RelativeSource = RelativeSource_50a435e405d84a27a5f97afb7f95cca0;


            Binding_c3d87c85c7fe429c905d62c8d78e1b0f.TemplateOwner = templateInstance_3da87ad11c964413a3705dab8af9e658;


            StackPanel_c3d071e2b99344e3b5e3afcfd722d31b.Children.Add(Border_1c9f7d37162a4e2398694a5d95518710);
            StackPanel_c3d071e2b99344e3b5e3afcfd722d31b.Children.Add(Border_1f378c14ffa94fb58dbef81e1ab0b975);



            Border_52581094754d48e6a1096a407162082f.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_9ade1692dc3c41b787ee53a098ce2dc3);
            ContentControl_1d2f190c866e40f2a77daad51dfcb782.SetBinding(global::Windows.UI.Xaml.Controls.Control.ForegroundProperty, Binding_cb9ff518b0304751a3b895b8887bc448);
            ContentControl_1d2f190c866e40f2a77daad51dfcb782.SetBinding(global::Windows.UI.Xaml.FrameworkElement.MarginProperty, Binding_bf02fee274314e7daf3f8a7ac0298a21);
            Border_1c9f7d37162a4e2398694a5d95518710.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_a1ee06c689264ec2854399504a411bd9);
            Border_1c9f7d37162a4e2398694a5d95518710.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_a3ece6dde1794a22a6a0a7ed7d3b9372);
            ContentControl_142ea7c5a80b448784d9d96001cc0662.SetBinding(global::Windows.UI.Xaml.Controls.Control.ForegroundProperty, Binding_77b1e0a7455c4dddb40b2e6eab7af099);
            ContentControl_142ea7c5a80b448784d9d96001cc0662.SetBinding(global::Windows.UI.Xaml.FrameworkElement.MarginProperty, Binding_fa27688241824117955982db410156ee);
            Border_1f378c14ffa94fb58dbef81e1ab0b975.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_fef019e77bb542188a7a72373aa38261);
            Border_1f378c14ffa94fb58dbef81e1ab0b975.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_3e5444e2342b4dfa820fd80c24481e6f);
            Border_1f378c14ffa94fb58dbef81e1ab0b975.SetBinding(global::Windows.UI.Xaml.FrameworkElement.CursorProperty, Binding_c3d87c85c7fe429c905d62c8d78e1b0f);

            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_c3ababf68a5a40efa04943e4c65482b5,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_b89ffbe13c2348999b5b8639a41775d6,
                    setVisualStateProperty_b89ffbe13c2348999b5b8639a41775d6,
                    setLocalVisualStateProperty_b89ffbe13c2348999b5b8639a41775d6,
                    getVisualStateProperty_b89ffbe13c2348999b5b8639a41775d6));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_c3ababf68a5a40efa04943e4c65482b5, Border_2ecffd2b4dda4eb7a5a7a821f6e41f4c);

            templateInstance_3da87ad11c964413a3705dab8af9e658.TemplateContent = StackPanel_c3d071e2b99344e3b5e3afcfd722d31b;
            return templateInstance_3da87ad11c964413a3705dab8af9e658;
        }

    }
}
#endif