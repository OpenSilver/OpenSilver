
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
    internal class INTERNAL_DefaultMenuItemStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_673b3462cc1d46279d537b62fbf8eb12 = new global::System.Windows.Style();
                Style_673b3462cc1d46279d537b62fbf8eb12.TargetType = typeof(global::System.Windows.Controls.MenuItem);
                var Setter_0d5ae989770e46ebbaebc0c1051cf31f = new global::System.Windows.Setter();
                Setter_0d5ae989770e46ebbaebc0c1051cf31f.Property = global::System.Windows.Controls.MenuItem.BackgroundProperty;
                Setter_0d5ae989770e46ebbaebc0c1051cf31f.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFE2E2E2");

                var Setter_3c532e58008c40e9b4ddb798a6675002 = new global::System.Windows.Setter();
                Setter_3c532e58008c40e9b4ddb798a6675002.Property = global::System.Windows.Controls.MenuItem.ForegroundProperty;
                Setter_3c532e58008c40e9b4ddb798a6675002.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Black");

                var Setter_1e28931446ce4a6d9e852b8185a1c8c5 = new global::System.Windows.Setter();
                Setter_1e28931446ce4a6d9e852b8185a1c8c5.Property = global::System.Windows.Controls.MenuItem.BorderThicknessProperty;
                Setter_1e28931446ce4a6d9e852b8185a1c8c5.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0");

                var Setter_eca3221f40894e3f9301a6dcc1c116ca = new global::System.Windows.Setter();
                Setter_eca3221f40894e3f9301a6dcc1c116ca.Property = global::System.Windows.Controls.MenuItem.PaddingProperty;
                Setter_eca3221f40894e3f9301a6dcc1c116ca.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"12,4,12,4");

                var Setter_c99114c24d51477a9e5a9bd8adb367b0 = new global::System.Windows.Setter();
                Setter_c99114c24d51477a9e5a9bd8adb367b0.Property = global::System.Windows.Controls.MenuItem.CursorProperty;
                Setter_c99114c24d51477a9e5a9bd8adb367b0.Value = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Hand");

                var Setter_1f58ac41556745f6ba8e050070066263 = new global::System.Windows.Setter();
                Setter_1f58ac41556745f6ba8e050070066263.Property = global::System.Windows.Controls.MenuItem.HorizontalContentAlignmentProperty;
                Setter_1f58ac41556745f6ba8e050070066263.Value = global::System.Windows.HorizontalAlignment.Center;

                var Setter_aa3215a0fd3740e2b4e63213c527b07f = new global::System.Windows.Setter();
                Setter_aa3215a0fd3740e2b4e63213c527b07f.Property = global::System.Windows.Controls.MenuItem.VerticalContentAlignmentProperty;
                Setter_aa3215a0fd3740e2b4e63213c527b07f.Value = global::System.Windows.VerticalAlignment.Center;

                var Setter_d9cc7cd8190f4779b3560cca27f4b2e9 = new global::System.Windows.Setter();
                Setter_d9cc7cd8190f4779b3560cca27f4b2e9.Property = global::System.Windows.Controls.MenuItem.TemplateProperty;
                var ControlTemplate_579e33c07fa149ff8b8e365e5a254609 = new global::System.Windows.Controls.ControlTemplate();
                ControlTemplate_579e33c07fa149ff8b8e365e5a254609.TargetType = typeof(global::System.Windows.Controls.MenuItem);
                ControlTemplate_579e33c07fa149ff8b8e365e5a254609.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_579e33c07fa149ff8b8e365e5a254609);

                Setter_d9cc7cd8190f4779b3560cca27f4b2e9.Value = ControlTemplate_579e33c07fa149ff8b8e365e5a254609;


                Style_673b3462cc1d46279d537b62fbf8eb12.Setters.Add(Setter_0d5ae989770e46ebbaebc0c1051cf31f);
                Style_673b3462cc1d46279d537b62fbf8eb12.Setters.Add(Setter_3c532e58008c40e9b4ddb798a6675002);
                Style_673b3462cc1d46279d537b62fbf8eb12.Setters.Add(Setter_1e28931446ce4a6d9e852b8185a1c8c5);
                Style_673b3462cc1d46279d537b62fbf8eb12.Setters.Add(Setter_eca3221f40894e3f9301a6dcc1c116ca);
                Style_673b3462cc1d46279d537b62fbf8eb12.Setters.Add(Setter_c99114c24d51477a9e5a9bd8adb367b0);
                Style_673b3462cc1d46279d537b62fbf8eb12.Setters.Add(Setter_1f58ac41556745f6ba8e050070066263);
                Style_673b3462cc1d46279d537b62fbf8eb12.Setters.Add(Setter_aa3215a0fd3740e2b4e63213c527b07f);
                Style_673b3462cc1d46279d537b62fbf8eb12.Setters.Add(Setter_d9cc7cd8190f4779b3560cca27f4b2e9);


                DefaultStyle = Style_673b3462cc1d46279d537b62fbf8eb12;

            }

            return DefaultStyle;
        }



        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_acc3082b25d84c5da250958f925418a7(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_acc3082b25d84c5da250958f925418a7(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_acc3082b25d84c5da250958f925418a7(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_acc3082b25d84c5da250958f925418a7(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_56244a39b1ae4b02a554dd24b163925b(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_56244a39b1ae4b02a554dd24b163925b(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_56244a39b1ae4b02a554dd24b163925b(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_56244a39b1ae4b02a554dd24b163925b(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_f1c5e147f893404fab0b8f4d41c44784(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_f1c5e147f893404fab0b8f4d41c44784(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_f1c5e147f893404fab0b8f4d41c44784(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_f1c5e147f893404fab0b8f4d41c44784(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_579e33c07fa149ff8b8e365e5a254609(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_b4f9cb6794aa4ef0bae1153137af4c49 = new global::System.Windows.TemplateInstance();
            templateInstance_b4f9cb6794aa4ef0bae1153137af4c49.TemplateOwner = templateOwner;
            var Border_60dd6f5be58f478f87fd257911a770ab = new global::System.Windows.Controls.Border();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("OuterBorder", Border_60dd6f5be58f478f87fd257911a770ab);
            Border_60dd6f5be58f478f87fd257911a770ab.Name = "OuterBorder";
            var VisualStateGroup_1c2eebe39a0e414398c94afa3dc89afa = new global::System.Windows.VisualStateGroup();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_1c2eebe39a0e414398c94afa3dc89afa);
            VisualStateGroup_1c2eebe39a0e414398c94afa3dc89afa.Name = "CommonStates";
            var VisualState_6fb652461bc94c539502e5b6072b9ba9 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Normal", VisualState_6fb652461bc94c539502e5b6072b9ba9);
            VisualState_6fb652461bc94c539502e5b6072b9ba9.Name = "Normal";

            var VisualState_829d3e8e480c4320aec52fa6b041788e = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("MouseOver", VisualState_829d3e8e480c4320aec52fa6b041788e);
            VisualState_829d3e8e480c4320aec52fa6b041788e.Name = "MouseOver";
            var Storyboard_98bbe3611b21485aaabe8114517f654e = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_7d102680831b4a20aaa966a70fa17a5b = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_7d102680831b4a20aaa966a70fa17a5b, @"InnerBorder");
            var DiscreteObjectKeyFrame_36524061f88b4a1d95287bb13176f2da = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_36524061f88b4a1d95287bb13176f2da.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_36524061f88b4a1d95287bb13176f2da.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#11000000");

            ObjectAnimationUsingKeyFrames_7d102680831b4a20aaa966a70fa17a5b.KeyFrames.Add(DiscreteObjectKeyFrame_36524061f88b4a1d95287bb13176f2da);


            Storyboard_98bbe3611b21485aaabe8114517f654e.Children.Add(ObjectAnimationUsingKeyFrames_7d102680831b4a20aaa966a70fa17a5b);


            VisualState_829d3e8e480c4320aec52fa6b041788e.Storyboard = Storyboard_98bbe3611b21485aaabe8114517f654e;


            var VisualState_096f6de19c2e400383b58f94f68fabdb = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Pressed", VisualState_096f6de19c2e400383b58f94f68fabdb);
            VisualState_096f6de19c2e400383b58f94f68fabdb.Name = "Pressed";
            var Storyboard_d0ab6f12828841f9a0cbbc3986beb3a4 = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_05b3b23c72ec4c8c817d327424fd746e = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_05b3b23c72ec4c8c817d327424fd746e, @"InnerBorder");
            var DiscreteObjectKeyFrame_5e1a3daa7f284f56b8a2999a0d8824a2 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_5e1a3daa7f284f56b8a2999a0d8824a2.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_5e1a3daa7f284f56b8a2999a0d8824a2.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#22000000");

            ObjectAnimationUsingKeyFrames_05b3b23c72ec4c8c817d327424fd746e.KeyFrames.Add(DiscreteObjectKeyFrame_5e1a3daa7f284f56b8a2999a0d8824a2);


            Storyboard_d0ab6f12828841f9a0cbbc3986beb3a4.Children.Add(ObjectAnimationUsingKeyFrames_05b3b23c72ec4c8c817d327424fd746e);


            VisualState_096f6de19c2e400383b58f94f68fabdb.Storyboard = Storyboard_d0ab6f12828841f9a0cbbc3986beb3a4;


            var VisualState_f8f432816f264c24be73ccf0cfe5e7ba = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_f8f432816f264c24be73ccf0cfe5e7ba);
            VisualState_f8f432816f264c24be73ccf0cfe5e7ba.Name = "Disabled";
            var Storyboard_55be0a911bec40ee8a3f1f4d1f88e150 = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_aa800cbca6d44aba8ffb9c1e814a92c5 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_aa800cbca6d44aba8ffb9c1e814a92c5, @"InnerBorder");
            var DiscreteObjectKeyFrame_bf5de57a4eac49fcb2bb2744e45cd29a = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_bf5de57a4eac49fcb2bb2744e45cd29a.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_bf5de57a4eac49fcb2bb2744e45cd29a.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#33FFFFFF");

            ObjectAnimationUsingKeyFrames_aa800cbca6d44aba8ffb9c1e814a92c5.KeyFrames.Add(DiscreteObjectKeyFrame_bf5de57a4eac49fcb2bb2744e45cd29a);


            Storyboard_55be0a911bec40ee8a3f1f4d1f88e150.Children.Add(ObjectAnimationUsingKeyFrames_aa800cbca6d44aba8ffb9c1e814a92c5);


            VisualState_f8f432816f264c24be73ccf0cfe5e7ba.Storyboard = Storyboard_55be0a911bec40ee8a3f1f4d1f88e150;


            VisualStateGroup_1c2eebe39a0e414398c94afa3dc89afa.States.Add(VisualState_6fb652461bc94c539502e5b6072b9ba9);
            VisualStateGroup_1c2eebe39a0e414398c94afa3dc89afa.States.Add(VisualState_829d3e8e480c4320aec52fa6b041788e);
            VisualStateGroup_1c2eebe39a0e414398c94afa3dc89afa.States.Add(VisualState_096f6de19c2e400383b58f94f68fabdb);
            VisualStateGroup_1c2eebe39a0e414398c94afa3dc89afa.States.Add(VisualState_f8f432816f264c24be73ccf0cfe5e7ba);


            ((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_1c2eebe39a0e414398c94afa3dc89afa);

            var Border_ba3b9309913345758e9c4beda7aec2d3 = new global::System.Windows.Controls.Border();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("InnerBorder", Border_ba3b9309913345758e9c4beda7aec2d3);
            Border_ba3b9309913345758e9c4beda7aec2d3.Name = "InnerBorder";
            var StackPanel_6e4dbeb2578a4d73bfbd05ea47cbb2d8 = new global::System.Windows.Controls.StackPanel();
            StackPanel_6e4dbeb2578a4d73bfbd05ea47cbb2d8.Orientation = global::System.Windows.Controls.Orientation.Horizontal;
            var ContentPresenter_063fcb61987645619fd6323651ddb693 = new global::System.Windows.Controls.ContentPresenter();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("IconPresenter", ContentPresenter_063fcb61987645619fd6323651ddb693);
            ContentPresenter_063fcb61987645619fd6323651ddb693.Name = "IconPresenter";
            ContentPresenter_063fcb61987645619fd6323651ddb693.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0,0,10,0");
            var Binding_3f90e3d8a1314100aedef8168d68ba71 = new global::System.Windows.Data.Binding();
            Binding_3f90e3d8a1314100aedef8168d68ba71.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Icon");
            var RelativeSource_a5a1b17d86344a47a6ca66a109fb308c = new global::System.Windows.Data.RelativeSource();
            RelativeSource_a5a1b17d86344a47a6ca66a109fb308c.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_3f90e3d8a1314100aedef8168d68ba71.RelativeSource = RelativeSource_a5a1b17d86344a47a6ca66a109fb308c;


            Binding_3f90e3d8a1314100aedef8168d68ba71.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;

            var Binding_fe5a8669556b41c0b4ceb49ad93a1ea2 = new global::System.Windows.Data.Binding();
            Binding_fe5a8669556b41c0b4ceb49ad93a1ea2.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"VerticalContentAlignment");
            var RelativeSource_e0e77bf4265746099b2ccb08cefec20d = new global::System.Windows.Data.RelativeSource();
            RelativeSource_e0e77bf4265746099b2ccb08cefec20d.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_fe5a8669556b41c0b4ceb49ad93a1ea2.RelativeSource = RelativeSource_e0e77bf4265746099b2ccb08cefec20d;


            Binding_fe5a8669556b41c0b4ceb49ad93a1ea2.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;

            var Binding_a104b00949c34e19811307d0f0f31484 = new global::System.Windows.Data.Binding();
            Binding_a104b00949c34e19811307d0f0f31484.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"INTERNAL_IconVisibility");
            var RelativeSource_2f5f7e091f9949c2840c2a2f9019dec3 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_2f5f7e091f9949c2840c2a2f9019dec3.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_a104b00949c34e19811307d0f0f31484.RelativeSource = RelativeSource_2f5f7e091f9949c2840c2a2f9019dec3;


            Binding_a104b00949c34e19811307d0f0f31484.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;

            var Binding_b7d2f4e8681b4b2cb128078e13a86a35 = new global::System.Windows.Data.Binding();
            Binding_b7d2f4e8681b4b2cb128078e13a86a35.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"ContentTemplate");
            var RelativeSource_347fd44727134d15b047857495620fc5 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_347fd44727134d15b047857495620fc5.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_b7d2f4e8681b4b2cb128078e13a86a35.RelativeSource = RelativeSource_347fd44727134d15b047857495620fc5;


            Binding_b7d2f4e8681b4b2cb128078e13a86a35.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;


            var ContentPresenter_830ba134a90a41938baa1f4efc8ed400 = new global::System.Windows.Controls.ContentPresenter();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("ContentPresenter", ContentPresenter_830ba134a90a41938baa1f4efc8ed400);
            ContentPresenter_830ba134a90a41938baa1f4efc8ed400.Name = "ContentPresenter";
            var Binding_c43e7644e6b448df9a5d7f6aef5e04c4 = new global::System.Windows.Data.Binding();
            Binding_c43e7644e6b448df9a5d7f6aef5e04c4.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"ContentTemplate");
            var RelativeSource_d2d71d0ab43f48c9a40797ebe28c09bb = new global::System.Windows.Data.RelativeSource();
            RelativeSource_d2d71d0ab43f48c9a40797ebe28c09bb.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_c43e7644e6b448df9a5d7f6aef5e04c4.RelativeSource = RelativeSource_d2d71d0ab43f48c9a40797ebe28c09bb;


            Binding_c43e7644e6b448df9a5d7f6aef5e04c4.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;

            var Binding_82ab7e0ffcec4deca9fca4ffc3e9fc48 = new global::System.Windows.Data.Binding();
            Binding_82ab7e0ffcec4deca9fca4ffc3e9fc48.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Content");
            var RelativeSource_f475e154dba845da9ff2a675b1de9fcc = new global::System.Windows.Data.RelativeSource();
            RelativeSource_f475e154dba845da9ff2a675b1de9fcc.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_82ab7e0ffcec4deca9fca4ffc3e9fc48.RelativeSource = RelativeSource_f475e154dba845da9ff2a675b1de9fcc;


            Binding_82ab7e0ffcec4deca9fca4ffc3e9fc48.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;

            var Binding_525516a50f6d451abeb47305a58a153e = new global::System.Windows.Data.Binding();
            Binding_525516a50f6d451abeb47305a58a153e.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"HorizontalContentAlignment");
            var RelativeSource_08535aaa69334be29eaca7a9174c9142 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_08535aaa69334be29eaca7a9174c9142.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_525516a50f6d451abeb47305a58a153e.RelativeSource = RelativeSource_08535aaa69334be29eaca7a9174c9142;


            Binding_525516a50f6d451abeb47305a58a153e.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;

            var Binding_ce51f4c0c5c444369621212b7fbcdf21 = new global::System.Windows.Data.Binding();
            Binding_ce51f4c0c5c444369621212b7fbcdf21.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"VerticalContentAlignment");
            var RelativeSource_fbbc874fdde2406bae6871c2968f42ac = new global::System.Windows.Data.RelativeSource();
            RelativeSource_fbbc874fdde2406bae6871c2968f42ac.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_ce51f4c0c5c444369621212b7fbcdf21.RelativeSource = RelativeSource_fbbc874fdde2406bae6871c2968f42ac;


            Binding_ce51f4c0c5c444369621212b7fbcdf21.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;


            StackPanel_6e4dbeb2578a4d73bfbd05ea47cbb2d8.Children.Add(ContentPresenter_063fcb61987645619fd6323651ddb693);
            StackPanel_6e4dbeb2578a4d73bfbd05ea47cbb2d8.Children.Add(ContentPresenter_830ba134a90a41938baa1f4efc8ed400);

            var Binding_c90b65d5b8a54c58ad205122e9a5c6cd = new global::System.Windows.Data.Binding();
            Binding_c90b65d5b8a54c58ad205122e9a5c6cd.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Padding");
            var RelativeSource_57297909a5584a4fbc2667e724e0594a = new global::System.Windows.Data.RelativeSource();
            RelativeSource_57297909a5584a4fbc2667e724e0594a.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_c90b65d5b8a54c58ad205122e9a5c6cd.RelativeSource = RelativeSource_57297909a5584a4fbc2667e724e0594a;


            Binding_c90b65d5b8a54c58ad205122e9a5c6cd.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;


            Border_ba3b9309913345758e9c4beda7aec2d3.Child = StackPanel_6e4dbeb2578a4d73bfbd05ea47cbb2d8;

            var Binding_395052bafee64de6a46aa581219f5ccc = new global::System.Windows.Data.Binding();
            Binding_395052bafee64de6a46aa581219f5ccc.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_35399a2e910e4b00b1500ea882650192 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_35399a2e910e4b00b1500ea882650192.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_395052bafee64de6a46aa581219f5ccc.RelativeSource = RelativeSource_35399a2e910e4b00b1500ea882650192;


            Binding_395052bafee64de6a46aa581219f5ccc.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;


            Border_60dd6f5be58f478f87fd257911a770ab.Child = Border_ba3b9309913345758e9c4beda7aec2d3;

            var Binding_67214965a3c145e19ba94bb5398f0dba = new global::System.Windows.Data.Binding();
            Binding_67214965a3c145e19ba94bb5398f0dba.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_0b54914b142443338bcbd589662b21d2 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_0b54914b142443338bcbd589662b21d2.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_67214965a3c145e19ba94bb5398f0dba.RelativeSource = RelativeSource_0b54914b142443338bcbd589662b21d2;


            Binding_67214965a3c145e19ba94bb5398f0dba.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;

            var Binding_f9f3e21f016d4e068983e3c2075115a7 = new global::System.Windows.Data.Binding();
            Binding_f9f3e21f016d4e068983e3c2075115a7.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
            var RelativeSource_f4cabdb4ad984335adf9ce6e48443701 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_f4cabdb4ad984335adf9ce6e48443701.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_f9f3e21f016d4e068983e3c2075115a7.RelativeSource = RelativeSource_f4cabdb4ad984335adf9ce6e48443701;


            Binding_f9f3e21f016d4e068983e3c2075115a7.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;

            var Binding_2af2afd502a3468c9bc07ede1cc6a956 = new global::System.Windows.Data.Binding();
            Binding_2af2afd502a3468c9bc07ede1cc6a956.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
            var RelativeSource_c183e6946e6a45d4915763e097ca20b5 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_c183e6946e6a45d4915763e097ca20b5.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_2af2afd502a3468c9bc07ede1cc6a956.RelativeSource = RelativeSource_c183e6946e6a45d4915763e097ca20b5;


            Binding_2af2afd502a3468c9bc07ede1cc6a956.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;



            ContentPresenter_063fcb61987645619fd6323651ddb693.SetBinding(global::System.Windows.Controls.ContentControl.ContentProperty, Binding_3f90e3d8a1314100aedef8168d68ba71);
            ContentPresenter_063fcb61987645619fd6323651ddb693.SetBinding(global::System.Windows.FrameworkElement.VerticalAlignmentProperty, Binding_fe5a8669556b41c0b4ceb49ad93a1ea2);
            ContentPresenter_063fcb61987645619fd6323651ddb693.SetBinding(global::System.Windows.UIElement.VisibilityProperty, Binding_a104b00949c34e19811307d0f0f31484);
            ContentPresenter_063fcb61987645619fd6323651ddb693.SetBinding(global::System.Windows.Controls.ContentControl.ContentTemplateProperty, Binding_b7d2f4e8681b4b2cb128078e13a86a35);
            ContentPresenter_830ba134a90a41938baa1f4efc8ed400.SetBinding(global::System.Windows.Controls.ContentControl.ContentTemplateProperty, Binding_c43e7644e6b448df9a5d7f6aef5e04c4);
            ContentPresenter_830ba134a90a41938baa1f4efc8ed400.SetBinding(global::System.Windows.Controls.ContentControl.ContentProperty, Binding_82ab7e0ffcec4deca9fca4ffc3e9fc48);
            ContentPresenter_830ba134a90a41938baa1f4efc8ed400.SetBinding(global::System.Windows.FrameworkElement.HorizontalAlignmentProperty, Binding_525516a50f6d451abeb47305a58a153e);
            ContentPresenter_830ba134a90a41938baa1f4efc8ed400.SetBinding(global::System.Windows.FrameworkElement.VerticalAlignmentProperty, Binding_ce51f4c0c5c444369621212b7fbcdf21);
            StackPanel_6e4dbeb2578a4d73bfbd05ea47cbb2d8.SetBinding(global::System.Windows.FrameworkElement.MarginProperty, Binding_c90b65d5b8a54c58ad205122e9a5c6cd);
            Border_ba3b9309913345758e9c4beda7aec2d3.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_395052bafee64de6a46aa581219f5ccc);
            Border_60dd6f5be58f478f87fd257911a770ab.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_67214965a3c145e19ba94bb5398f0dba);
            Border_60dd6f5be58f478f87fd257911a770ab.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_f9f3e21f016d4e068983e3c2075115a7);
            Border_60dd6f5be58f478f87fd257911a770ab.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_2af2afd502a3468c9bc07ede1cc6a956);

            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_7d102680831b4a20aaa966a70fa17a5b,
                new global::System.Windows.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_acc3082b25d84c5da250958f925418a7,
                    setVisualStateProperty_acc3082b25d84c5da250958f925418a7,
                    setLocalVisualStateProperty_acc3082b25d84c5da250958f925418a7,
                    getVisualStateProperty_acc3082b25d84c5da250958f925418a7));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_7d102680831b4a20aaa966a70fa17a5b, Border_ba3b9309913345758e9c4beda7aec2d3);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_05b3b23c72ec4c8c817d327424fd746e,
                new global::System.Windows.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_56244a39b1ae4b02a554dd24b163925b,
                    setVisualStateProperty_56244a39b1ae4b02a554dd24b163925b,
                    setLocalVisualStateProperty_56244a39b1ae4b02a554dd24b163925b,
                    getVisualStateProperty_56244a39b1ae4b02a554dd24b163925b));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_05b3b23c72ec4c8c817d327424fd746e, Border_ba3b9309913345758e9c4beda7aec2d3);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_aa800cbca6d44aba8ffb9c1e814a92c5,
                new global::System.Windows.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_f1c5e147f893404fab0b8f4d41c44784,
                    setVisualStateProperty_f1c5e147f893404fab0b8f4d41c44784,
                    setLocalVisualStateProperty_f1c5e147f893404fab0b8f4d41c44784,
                    getVisualStateProperty_f1c5e147f893404fab0b8f4d41c44784));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_aa800cbca6d44aba8ffb9c1e814a92c5, Border_ba3b9309913345758e9c4beda7aec2d3);

            templateInstance_b4f9cb6794aa4ef0bae1153137af4c49.TemplateContent = Border_60dd6f5be58f478f87fd257911a770ab;
            return templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;
        }
    }
}
#else
namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultMenuItemStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_673b3462cc1d46279d537b62fbf8eb12 = new global::Windows.UI.Xaml.Style();
                Style_673b3462cc1d46279d537b62fbf8eb12.TargetType = typeof(global::Windows.UI.Xaml.Controls.MenuItem);
                var Setter_0d5ae989770e46ebbaebc0c1051cf31f = new global::Windows.UI.Xaml.Setter();
                Setter_0d5ae989770e46ebbaebc0c1051cf31f.Property = global::Windows.UI.Xaml.Controls.MenuItem.BackgroundProperty;
                Setter_0d5ae989770e46ebbaebc0c1051cf31f.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFE2E2E2");

                var Setter_3c532e58008c40e9b4ddb798a6675002 = new global::Windows.UI.Xaml.Setter();
                Setter_3c532e58008c40e9b4ddb798a6675002.Property = global::Windows.UI.Xaml.Controls.MenuItem.ForegroundProperty;
                Setter_3c532e58008c40e9b4ddb798a6675002.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Black");

                var Setter_1e28931446ce4a6d9e852b8185a1c8c5 = new global::Windows.UI.Xaml.Setter();
                Setter_1e28931446ce4a6d9e852b8185a1c8c5.Property = global::Windows.UI.Xaml.Controls.MenuItem.BorderThicknessProperty;
                Setter_1e28931446ce4a6d9e852b8185a1c8c5.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0");

                var Setter_eca3221f40894e3f9301a6dcc1c116ca = new global::Windows.UI.Xaml.Setter();
                Setter_eca3221f40894e3f9301a6dcc1c116ca.Property = global::Windows.UI.Xaml.Controls.MenuItem.PaddingProperty;
                Setter_eca3221f40894e3f9301a6dcc1c116ca.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"12,4,12,4");

                var Setter_c99114c24d51477a9e5a9bd8adb367b0 = new global::Windows.UI.Xaml.Setter();
                Setter_c99114c24d51477a9e5a9bd8adb367b0.Property = global::Windows.UI.Xaml.Controls.MenuItem.CursorProperty;
                Setter_c99114c24d51477a9e5a9bd8adb367b0.Value = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Hand");

                var Setter_1f58ac41556745f6ba8e050070066263 = new global::Windows.UI.Xaml.Setter();
                Setter_1f58ac41556745f6ba8e050070066263.Property = global::Windows.UI.Xaml.Controls.MenuItem.HorizontalContentAlignmentProperty;
                Setter_1f58ac41556745f6ba8e050070066263.Value = global::Windows.UI.Xaml.HorizontalAlignment.Center;

                var Setter_aa3215a0fd3740e2b4e63213c527b07f = new global::Windows.UI.Xaml.Setter();
                Setter_aa3215a0fd3740e2b4e63213c527b07f.Property = global::Windows.UI.Xaml.Controls.MenuItem.VerticalContentAlignmentProperty;
                Setter_aa3215a0fd3740e2b4e63213c527b07f.Value = global::Windows.UI.Xaml.VerticalAlignment.Center;

                var Setter_d9cc7cd8190f4779b3560cca27f4b2e9 = new global::Windows.UI.Xaml.Setter();
                Setter_d9cc7cd8190f4779b3560cca27f4b2e9.Property = global::Windows.UI.Xaml.Controls.MenuItem.TemplateProperty;
                var ControlTemplate_579e33c07fa149ff8b8e365e5a254609 = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_579e33c07fa149ff8b8e365e5a254609.TargetType = typeof(global::Windows.UI.Xaml.Controls.MenuItem);
                ControlTemplate_579e33c07fa149ff8b8e365e5a254609.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_579e33c07fa149ff8b8e365e5a254609);

                Setter_d9cc7cd8190f4779b3560cca27f4b2e9.Value = ControlTemplate_579e33c07fa149ff8b8e365e5a254609;


                Style_673b3462cc1d46279d537b62fbf8eb12.Setters.Add(Setter_0d5ae989770e46ebbaebc0c1051cf31f);
                Style_673b3462cc1d46279d537b62fbf8eb12.Setters.Add(Setter_3c532e58008c40e9b4ddb798a6675002);
                Style_673b3462cc1d46279d537b62fbf8eb12.Setters.Add(Setter_1e28931446ce4a6d9e852b8185a1c8c5);
                Style_673b3462cc1d46279d537b62fbf8eb12.Setters.Add(Setter_eca3221f40894e3f9301a6dcc1c116ca);
                Style_673b3462cc1d46279d537b62fbf8eb12.Setters.Add(Setter_c99114c24d51477a9e5a9bd8adb367b0);
                Style_673b3462cc1d46279d537b62fbf8eb12.Setters.Add(Setter_1f58ac41556745f6ba8e050070066263);
                Style_673b3462cc1d46279d537b62fbf8eb12.Setters.Add(Setter_aa3215a0fd3740e2b4e63213c527b07f);
                Style_673b3462cc1d46279d537b62fbf8eb12.Setters.Add(Setter_d9cc7cd8190f4779b3560cca27f4b2e9);


                DefaultStyle = Style_673b3462cc1d46279d537b62fbf8eb12;

            }

            return DefaultStyle;
        }



        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_acc3082b25d84c5da250958f925418a7(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_acc3082b25d84c5da250958f925418a7(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_acc3082b25d84c5da250958f925418a7(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_acc3082b25d84c5da250958f925418a7(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_56244a39b1ae4b02a554dd24b163925b(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_56244a39b1ae4b02a554dd24b163925b(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_56244a39b1ae4b02a554dd24b163925b(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_56244a39b1ae4b02a554dd24b163925b(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_f1c5e147f893404fab0b8f4d41c44784(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_f1c5e147f893404fab0b8f4d41c44784(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_f1c5e147f893404fab0b8f4d41c44784(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_f1c5e147f893404fab0b8f4d41c44784(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_579e33c07fa149ff8b8e365e5a254609(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_b4f9cb6794aa4ef0bae1153137af4c49 = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_b4f9cb6794aa4ef0bae1153137af4c49.TemplateOwner = templateOwner;
            var Border_60dd6f5be58f478f87fd257911a770ab = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("OuterBorder", Border_60dd6f5be58f478f87fd257911a770ab);
            Border_60dd6f5be58f478f87fd257911a770ab.Name = "OuterBorder";
            var VisualStateGroup_1c2eebe39a0e414398c94afa3dc89afa = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_1c2eebe39a0e414398c94afa3dc89afa);
            VisualStateGroup_1c2eebe39a0e414398c94afa3dc89afa.Name = "CommonStates";
            var VisualState_6fb652461bc94c539502e5b6072b9ba9 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Normal", VisualState_6fb652461bc94c539502e5b6072b9ba9);
            VisualState_6fb652461bc94c539502e5b6072b9ba9.Name = "Normal";

            var VisualState_829d3e8e480c4320aec52fa6b041788e = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("PointerOver", VisualState_829d3e8e480c4320aec52fa6b041788e);
            VisualState_829d3e8e480c4320aec52fa6b041788e.Name = "PointerOver";
            var Storyboard_98bbe3611b21485aaabe8114517f654e = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_7d102680831b4a20aaa966a70fa17a5b = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_7d102680831b4a20aaa966a70fa17a5b, @"InnerBorder");
            var DiscreteObjectKeyFrame_36524061f88b4a1d95287bb13176f2da = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_36524061f88b4a1d95287bb13176f2da.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_36524061f88b4a1d95287bb13176f2da.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#11000000");

            ObjectAnimationUsingKeyFrames_7d102680831b4a20aaa966a70fa17a5b.KeyFrames.Add(DiscreteObjectKeyFrame_36524061f88b4a1d95287bb13176f2da);


            Storyboard_98bbe3611b21485aaabe8114517f654e.Children.Add(ObjectAnimationUsingKeyFrames_7d102680831b4a20aaa966a70fa17a5b);


            VisualState_829d3e8e480c4320aec52fa6b041788e.Storyboard = Storyboard_98bbe3611b21485aaabe8114517f654e;


            var VisualState_096f6de19c2e400383b58f94f68fabdb = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Pressed", VisualState_096f6de19c2e400383b58f94f68fabdb);
            VisualState_096f6de19c2e400383b58f94f68fabdb.Name = "Pressed";
            var Storyboard_d0ab6f12828841f9a0cbbc3986beb3a4 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_05b3b23c72ec4c8c817d327424fd746e = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_05b3b23c72ec4c8c817d327424fd746e, @"InnerBorder");
            var DiscreteObjectKeyFrame_5e1a3daa7f284f56b8a2999a0d8824a2 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_5e1a3daa7f284f56b8a2999a0d8824a2.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_5e1a3daa7f284f56b8a2999a0d8824a2.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#22000000");

            ObjectAnimationUsingKeyFrames_05b3b23c72ec4c8c817d327424fd746e.KeyFrames.Add(DiscreteObjectKeyFrame_5e1a3daa7f284f56b8a2999a0d8824a2);


            Storyboard_d0ab6f12828841f9a0cbbc3986beb3a4.Children.Add(ObjectAnimationUsingKeyFrames_05b3b23c72ec4c8c817d327424fd746e);


            VisualState_096f6de19c2e400383b58f94f68fabdb.Storyboard = Storyboard_d0ab6f12828841f9a0cbbc3986beb3a4;


            var VisualState_f8f432816f264c24be73ccf0cfe5e7ba = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_f8f432816f264c24be73ccf0cfe5e7ba);
            VisualState_f8f432816f264c24be73ccf0cfe5e7ba.Name = "Disabled";
            var Storyboard_55be0a911bec40ee8a3f1f4d1f88e150 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_aa800cbca6d44aba8ffb9c1e814a92c5 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_aa800cbca6d44aba8ffb9c1e814a92c5, @"InnerBorder");
            var DiscreteObjectKeyFrame_bf5de57a4eac49fcb2bb2744e45cd29a = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_bf5de57a4eac49fcb2bb2744e45cd29a.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_bf5de57a4eac49fcb2bb2744e45cd29a.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#33FFFFFF");

            ObjectAnimationUsingKeyFrames_aa800cbca6d44aba8ffb9c1e814a92c5.KeyFrames.Add(DiscreteObjectKeyFrame_bf5de57a4eac49fcb2bb2744e45cd29a);


            Storyboard_55be0a911bec40ee8a3f1f4d1f88e150.Children.Add(ObjectAnimationUsingKeyFrames_aa800cbca6d44aba8ffb9c1e814a92c5);


            VisualState_f8f432816f264c24be73ccf0cfe5e7ba.Storyboard = Storyboard_55be0a911bec40ee8a3f1f4d1f88e150;


            VisualStateGroup_1c2eebe39a0e414398c94afa3dc89afa.States.Add(VisualState_6fb652461bc94c539502e5b6072b9ba9);
            VisualStateGroup_1c2eebe39a0e414398c94afa3dc89afa.States.Add(VisualState_829d3e8e480c4320aec52fa6b041788e);
            VisualStateGroup_1c2eebe39a0e414398c94afa3dc89afa.States.Add(VisualState_096f6de19c2e400383b58f94f68fabdb);
            VisualStateGroup_1c2eebe39a0e414398c94afa3dc89afa.States.Add(VisualState_f8f432816f264c24be73ccf0cfe5e7ba);


            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_1c2eebe39a0e414398c94afa3dc89afa);

            var Border_ba3b9309913345758e9c4beda7aec2d3 = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("InnerBorder", Border_ba3b9309913345758e9c4beda7aec2d3);
            Border_ba3b9309913345758e9c4beda7aec2d3.Name = "InnerBorder";
            var StackPanel_6e4dbeb2578a4d73bfbd05ea47cbb2d8 = new global::Windows.UI.Xaml.Controls.StackPanel();
            StackPanel_6e4dbeb2578a4d73bfbd05ea47cbb2d8.Orientation = global::Windows.UI.Xaml.Controls.Orientation.Horizontal;
            var ContentPresenter_063fcb61987645619fd6323651ddb693 = new global::Windows.UI.Xaml.Controls.ContentPresenter();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("IconPresenter", ContentPresenter_063fcb61987645619fd6323651ddb693);
            ContentPresenter_063fcb61987645619fd6323651ddb693.Name = "IconPresenter";
            ContentPresenter_063fcb61987645619fd6323651ddb693.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0,0,10,0");
            var Binding_3f90e3d8a1314100aedef8168d68ba71 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_3f90e3d8a1314100aedef8168d68ba71.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Icon");
            var RelativeSource_a5a1b17d86344a47a6ca66a109fb308c = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_a5a1b17d86344a47a6ca66a109fb308c.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_3f90e3d8a1314100aedef8168d68ba71.RelativeSource = RelativeSource_a5a1b17d86344a47a6ca66a109fb308c;


            Binding_3f90e3d8a1314100aedef8168d68ba71.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;

            var Binding_fe5a8669556b41c0b4ceb49ad93a1ea2 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_fe5a8669556b41c0b4ceb49ad93a1ea2.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"VerticalContentAlignment");
            var RelativeSource_e0e77bf4265746099b2ccb08cefec20d = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_e0e77bf4265746099b2ccb08cefec20d.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_fe5a8669556b41c0b4ceb49ad93a1ea2.RelativeSource = RelativeSource_e0e77bf4265746099b2ccb08cefec20d;


            Binding_fe5a8669556b41c0b4ceb49ad93a1ea2.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;

            var Binding_a104b00949c34e19811307d0f0f31484 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_a104b00949c34e19811307d0f0f31484.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"INTERNAL_IconVisibility");
            var RelativeSource_2f5f7e091f9949c2840c2a2f9019dec3 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_2f5f7e091f9949c2840c2a2f9019dec3.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_a104b00949c34e19811307d0f0f31484.RelativeSource = RelativeSource_2f5f7e091f9949c2840c2a2f9019dec3;


            Binding_a104b00949c34e19811307d0f0f31484.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;

            var Binding_b7d2f4e8681b4b2cb128078e13a86a35 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_b7d2f4e8681b4b2cb128078e13a86a35.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"ContentTemplate");
            var RelativeSource_347fd44727134d15b047857495620fc5 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_347fd44727134d15b047857495620fc5.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_b7d2f4e8681b4b2cb128078e13a86a35.RelativeSource = RelativeSource_347fd44727134d15b047857495620fc5;


            Binding_b7d2f4e8681b4b2cb128078e13a86a35.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;


            var ContentPresenter_830ba134a90a41938baa1f4efc8ed400 = new global::Windows.UI.Xaml.Controls.ContentPresenter();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ContentPresenter", ContentPresenter_830ba134a90a41938baa1f4efc8ed400);
            ContentPresenter_830ba134a90a41938baa1f4efc8ed400.Name = "ContentPresenter";
            var Binding_c43e7644e6b448df9a5d7f6aef5e04c4 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_c43e7644e6b448df9a5d7f6aef5e04c4.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"ContentTemplate");
            var RelativeSource_d2d71d0ab43f48c9a40797ebe28c09bb = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_d2d71d0ab43f48c9a40797ebe28c09bb.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_c43e7644e6b448df9a5d7f6aef5e04c4.RelativeSource = RelativeSource_d2d71d0ab43f48c9a40797ebe28c09bb;


            Binding_c43e7644e6b448df9a5d7f6aef5e04c4.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;

            var Binding_82ab7e0ffcec4deca9fca4ffc3e9fc48 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_82ab7e0ffcec4deca9fca4ffc3e9fc48.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Content");
            var RelativeSource_f475e154dba845da9ff2a675b1de9fcc = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_f475e154dba845da9ff2a675b1de9fcc.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_82ab7e0ffcec4deca9fca4ffc3e9fc48.RelativeSource = RelativeSource_f475e154dba845da9ff2a675b1de9fcc;


            Binding_82ab7e0ffcec4deca9fca4ffc3e9fc48.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;

            var Binding_525516a50f6d451abeb47305a58a153e = new global::Windows.UI.Xaml.Data.Binding();
            Binding_525516a50f6d451abeb47305a58a153e.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"HorizontalContentAlignment");
            var RelativeSource_08535aaa69334be29eaca7a9174c9142 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_08535aaa69334be29eaca7a9174c9142.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_525516a50f6d451abeb47305a58a153e.RelativeSource = RelativeSource_08535aaa69334be29eaca7a9174c9142;


            Binding_525516a50f6d451abeb47305a58a153e.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;

            var Binding_ce51f4c0c5c444369621212b7fbcdf21 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_ce51f4c0c5c444369621212b7fbcdf21.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"VerticalContentAlignment");
            var RelativeSource_fbbc874fdde2406bae6871c2968f42ac = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_fbbc874fdde2406bae6871c2968f42ac.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_ce51f4c0c5c444369621212b7fbcdf21.RelativeSource = RelativeSource_fbbc874fdde2406bae6871c2968f42ac;


            Binding_ce51f4c0c5c444369621212b7fbcdf21.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;


            StackPanel_6e4dbeb2578a4d73bfbd05ea47cbb2d8.Children.Add(ContentPresenter_063fcb61987645619fd6323651ddb693);
            StackPanel_6e4dbeb2578a4d73bfbd05ea47cbb2d8.Children.Add(ContentPresenter_830ba134a90a41938baa1f4efc8ed400);

            var Binding_c90b65d5b8a54c58ad205122e9a5c6cd = new global::Windows.UI.Xaml.Data.Binding();
            Binding_c90b65d5b8a54c58ad205122e9a5c6cd.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Padding");
            var RelativeSource_57297909a5584a4fbc2667e724e0594a = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_57297909a5584a4fbc2667e724e0594a.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_c90b65d5b8a54c58ad205122e9a5c6cd.RelativeSource = RelativeSource_57297909a5584a4fbc2667e724e0594a;


            Binding_c90b65d5b8a54c58ad205122e9a5c6cd.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;


            Border_ba3b9309913345758e9c4beda7aec2d3.Child = StackPanel_6e4dbeb2578a4d73bfbd05ea47cbb2d8;

            var Binding_395052bafee64de6a46aa581219f5ccc = new global::Windows.UI.Xaml.Data.Binding();
            Binding_395052bafee64de6a46aa581219f5ccc.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_35399a2e910e4b00b1500ea882650192 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_35399a2e910e4b00b1500ea882650192.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_395052bafee64de6a46aa581219f5ccc.RelativeSource = RelativeSource_35399a2e910e4b00b1500ea882650192;


            Binding_395052bafee64de6a46aa581219f5ccc.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;


            Border_60dd6f5be58f478f87fd257911a770ab.Child = Border_ba3b9309913345758e9c4beda7aec2d3;

            var Binding_67214965a3c145e19ba94bb5398f0dba = new global::Windows.UI.Xaml.Data.Binding();
            Binding_67214965a3c145e19ba94bb5398f0dba.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_0b54914b142443338bcbd589662b21d2 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_0b54914b142443338bcbd589662b21d2.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_67214965a3c145e19ba94bb5398f0dba.RelativeSource = RelativeSource_0b54914b142443338bcbd589662b21d2;


            Binding_67214965a3c145e19ba94bb5398f0dba.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;

            var Binding_f9f3e21f016d4e068983e3c2075115a7 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_f9f3e21f016d4e068983e3c2075115a7.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_f4cabdb4ad984335adf9ce6e48443701 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_f4cabdb4ad984335adf9ce6e48443701.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_f9f3e21f016d4e068983e3c2075115a7.RelativeSource = RelativeSource_f4cabdb4ad984335adf9ce6e48443701;


            Binding_f9f3e21f016d4e068983e3c2075115a7.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;

            var Binding_2af2afd502a3468c9bc07ede1cc6a956 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_2af2afd502a3468c9bc07ede1cc6a956.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_c183e6946e6a45d4915763e097ca20b5 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_c183e6946e6a45d4915763e097ca20b5.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_2af2afd502a3468c9bc07ede1cc6a956.RelativeSource = RelativeSource_c183e6946e6a45d4915763e097ca20b5;


            Binding_2af2afd502a3468c9bc07ede1cc6a956.TemplateOwner = templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;



            ContentPresenter_063fcb61987645619fd6323651ddb693.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentProperty, Binding_3f90e3d8a1314100aedef8168d68ba71);
            ContentPresenter_063fcb61987645619fd6323651ddb693.SetBinding(global::Windows.UI.Xaml.FrameworkElement.VerticalAlignmentProperty, Binding_fe5a8669556b41c0b4ceb49ad93a1ea2);
            ContentPresenter_063fcb61987645619fd6323651ddb693.SetBinding(global::Windows.UI.Xaml.UIElement.VisibilityProperty, Binding_a104b00949c34e19811307d0f0f31484);
            ContentPresenter_063fcb61987645619fd6323651ddb693.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentTemplateProperty, Binding_b7d2f4e8681b4b2cb128078e13a86a35);
            ContentPresenter_830ba134a90a41938baa1f4efc8ed400.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentTemplateProperty, Binding_c43e7644e6b448df9a5d7f6aef5e04c4);
            ContentPresenter_830ba134a90a41938baa1f4efc8ed400.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentProperty, Binding_82ab7e0ffcec4deca9fca4ffc3e9fc48);
            ContentPresenter_830ba134a90a41938baa1f4efc8ed400.SetBinding(global::Windows.UI.Xaml.FrameworkElement.HorizontalAlignmentProperty, Binding_525516a50f6d451abeb47305a58a153e);
            ContentPresenter_830ba134a90a41938baa1f4efc8ed400.SetBinding(global::Windows.UI.Xaml.FrameworkElement.VerticalAlignmentProperty, Binding_ce51f4c0c5c444369621212b7fbcdf21);
            StackPanel_6e4dbeb2578a4d73bfbd05ea47cbb2d8.SetBinding(global::Windows.UI.Xaml.FrameworkElement.MarginProperty, Binding_c90b65d5b8a54c58ad205122e9a5c6cd);
            Border_ba3b9309913345758e9c4beda7aec2d3.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_395052bafee64de6a46aa581219f5ccc);
            Border_60dd6f5be58f478f87fd257911a770ab.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_67214965a3c145e19ba94bb5398f0dba);
            Border_60dd6f5be58f478f87fd257911a770ab.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_f9f3e21f016d4e068983e3c2075115a7);
            Border_60dd6f5be58f478f87fd257911a770ab.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_2af2afd502a3468c9bc07ede1cc6a956);

            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_7d102680831b4a20aaa966a70fa17a5b,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_acc3082b25d84c5da250958f925418a7,
                    setVisualStateProperty_acc3082b25d84c5da250958f925418a7,
                    setLocalVisualStateProperty_acc3082b25d84c5da250958f925418a7,
                    getVisualStateProperty_acc3082b25d84c5da250958f925418a7));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_7d102680831b4a20aaa966a70fa17a5b, Border_ba3b9309913345758e9c4beda7aec2d3);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_05b3b23c72ec4c8c817d327424fd746e,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_56244a39b1ae4b02a554dd24b163925b,
                    setVisualStateProperty_56244a39b1ae4b02a554dd24b163925b,
                    setLocalVisualStateProperty_56244a39b1ae4b02a554dd24b163925b,
                    getVisualStateProperty_56244a39b1ae4b02a554dd24b163925b));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_05b3b23c72ec4c8c817d327424fd746e, Border_ba3b9309913345758e9c4beda7aec2d3);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_aa800cbca6d44aba8ffb9c1e814a92c5,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_f1c5e147f893404fab0b8f4d41c44784,
                    setVisualStateProperty_f1c5e147f893404fab0b8f4d41c44784,
                    setLocalVisualStateProperty_f1c5e147f893404fab0b8f4d41c44784,
                    getVisualStateProperty_f1c5e147f893404fab0b8f4d41c44784));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_aa800cbca6d44aba8ffb9c1e814a92c5, Border_ba3b9309913345758e9c4beda7aec2d3);

            templateInstance_b4f9cb6794aa4ef0bae1153137af4c49.TemplateContent = Border_60dd6f5be58f478f87fd257911a770ab;
            return templateInstance_b4f9cb6794aa4ef0bae1153137af4c49;
        }
    }
}

#endif