
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


#if MIGRATION
namespace System.Windows.Controls.Primitives
{
    internal class INTERNAL_DefaultThumbStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_96f68aa06ac5423ea04363f0e1cc6b17 = new global::System.Windows.Style();
                Style_96f68aa06ac5423ea04363f0e1cc6b17.TargetType = typeof(global::System.Windows.Controls.Primitives.Thumb);
                var Setter_68c4f256583c417cb62ebdfad52e19a7 = new global::System.Windows.Setter();
                Setter_68c4f256583c417cb62ebdfad52e19a7.Property = global::System.Windows.Controls.Primitives.Thumb.BackgroundProperty;
                Setter_68c4f256583c417cb62ebdfad52e19a7.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFE2E2E2");

                var Setter_7a6a7f16478c4a10a08e61f3d67c17e7 = new global::System.Windows.Setter();
                Setter_7a6a7f16478c4a10a08e61f3d67c17e7.Property = global::System.Windows.Controls.Primitives.Thumb.BorderThicknessProperty;
                Setter_7a6a7f16478c4a10a08e61f3d67c17e7.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0");

                var Setter_ecba2487d1094c93901b8dce0cf4f2b0 = new global::System.Windows.Setter();
                Setter_ecba2487d1094c93901b8dce0cf4f2b0.Property = global::System.Windows.Controls.Primitives.Thumb.TemplateProperty;
                var ControlTemplate_c8e6fa19521344109414b9d44ea94918 = new global::System.Windows.Controls.ControlTemplate();
                ControlTemplate_c8e6fa19521344109414b9d44ea94918.TargetType = typeof(global::System.Windows.Controls.Primitives.Thumb);
                ControlTemplate_c8e6fa19521344109414b9d44ea94918.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_c8e6fa19521344109414b9d44ea94918);

                Setter_ecba2487d1094c93901b8dce0cf4f2b0.Value = ControlTemplate_c8e6fa19521344109414b9d44ea94918;


                Style_96f68aa06ac5423ea04363f0e1cc6b17.Setters.Add(Setter_68c4f256583c417cb62ebdfad52e19a7);
                Style_96f68aa06ac5423ea04363f0e1cc6b17.Setters.Add(Setter_7a6a7f16478c4a10a08e61f3d67c17e7);
                Style_96f68aa06ac5423ea04363f0e1cc6b17.Setters.Add(Setter_ecba2487d1094c93901b8dce0cf4f2b0);


                DefaultStyle = Style_96f68aa06ac5423ea04363f0e1cc6b17;
            }

            return DefaultStyle;
        }



        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_4e4d6374fec24e15b0023e9a82203bf4(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_4e4d6374fec24e15b0023e9a82203bf4(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_4e4d6374fec24e15b0023e9a82203bf4(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_4e4d6374fec24e15b0023e9a82203bf4(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_8f8058186681457e8d459ce9c6e9f66b(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_8f8058186681457e8d459ce9c6e9f66b(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_8f8058186681457e8d459ce9c6e9f66b(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_8f8058186681457e8d459ce9c6e9f66b(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_7b2742bb384447bfb447e66f73afa03a(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_7b2742bb384447bfb447e66f73afa03a(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_7b2742bb384447bfb447e66f73afa03a(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_7b2742bb384447bfb447e66f73afa03a(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_72659532984249a9bd2e30737c7ed5a8(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_72659532984249a9bd2e30737c7ed5a8(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_72659532984249a9bd2e30737c7ed5a8(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_72659532984249a9bd2e30737c7ed5a8(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_c832abb588b24cef9d0c63e2a898bd8e(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_c832abb588b24cef9d0c63e2a898bd8e(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_c832abb588b24cef9d0c63e2a898bd8e(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_c832abb588b24cef9d0c63e2a898bd8e(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_1757611cf6164cc5afbb34df94b52850(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_1757611cf6164cc5afbb34df94b52850(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_1757611cf6164cc5afbb34df94b52850(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_1757611cf6164cc5afbb34df94b52850(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_c8e6fa19521344109414b9d44ea94918(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_e173eb2aae5246999057bfa4ae154a20 = new global::System.Windows.TemplateInstance();
            templateInstance_e173eb2aae5246999057bfa4ae154a20.TemplateOwner = templateOwner;
            var Grid_6823781fbae44ec3ae1881f9639427c0 = new global::System.Windows.Controls.Grid();
            var VisualStateGroup_df0179b92f0f4bd299de2ea203a6a00c = new global::System.Windows.VisualStateGroup();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_df0179b92f0f4bd299de2ea203a6a00c);
            VisualStateGroup_df0179b92f0f4bd299de2ea203a6a00c.Name = "CommonStates";
            var VisualState_8f1d1fe38ea74a56bf764a937ed3e83e = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Normal", VisualState_8f1d1fe38ea74a56bf764a937ed3e83e);
            VisualState_8f1d1fe38ea74a56bf764a937ed3e83e.Name = "Normal";

            var VisualState_a05a8180b5e34367aa476d4c55e72b53 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("MouseOver", VisualState_a05a8180b5e34367aa476d4c55e72b53);
            VisualState_a05a8180b5e34367aa476d4c55e72b53.Name = "MouseOver";
            var Storyboard_9f3ee418257044abad2960a5a02c506b = new global::System.Windows.Media.Animation.Storyboard();
            var DoubleAnimation_b9f9e8f1a5a84ec28f20d795a4f4308a = new global::System.Windows.Media.Animation.DoubleAnimation();
            DoubleAnimation_b9f9e8f1a5a84ec28f20d795a4f4308a.Duration = (global::System.Windows.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Duration), @"0");
            DoubleAnimation_b9f9e8f1a5a84ec28f20d795a4f4308a.To = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"1");
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(DoubleAnimation_b9f9e8f1a5a84ec28f20d795a4f4308a, @"BackgroundPointerOver");

            var DoubleAnimation_9600c41cc1eb41798dd72e92d9798a9f = new global::System.Windows.Media.Animation.DoubleAnimation();
            DoubleAnimation_9600c41cc1eb41798dd72e92d9798a9f.Duration = (global::System.Windows.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Duration), @"0");
            DoubleAnimation_9600c41cc1eb41798dd72e92d9798a9f.To = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(DoubleAnimation_9600c41cc1eb41798dd72e92d9798a9f, @"Background");

            Storyboard_9f3ee418257044abad2960a5a02c506b.Children.Add(DoubleAnimation_b9f9e8f1a5a84ec28f20d795a4f4308a);
            Storyboard_9f3ee418257044abad2960a5a02c506b.Children.Add(DoubleAnimation_9600c41cc1eb41798dd72e92d9798a9f);


            VisualState_a05a8180b5e34367aa476d4c55e72b53.Storyboard = Storyboard_9f3ee418257044abad2960a5a02c506b;


            var VisualState_ca5b55d3d46c409c90a86e18928482f1 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Pressed", VisualState_ca5b55d3d46c409c90a86e18928482f1);
            VisualState_ca5b55d3d46c409c90a86e18928482f1.Name = "Pressed";
            var Storyboard_2bb450174cb346498fcf33d32b6f274f = new global::System.Windows.Media.Animation.Storyboard();
            var DoubleAnimation_8d6711a3eba649a9be2dda911d715527 = new global::System.Windows.Media.Animation.DoubleAnimation();
            DoubleAnimation_8d6711a3eba649a9be2dda911d715527.Duration = (global::System.Windows.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Duration), @"0");
            DoubleAnimation_8d6711a3eba649a9be2dda911d715527.To = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"1");
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(DoubleAnimation_8d6711a3eba649a9be2dda911d715527, @"BackgroundPressed");

            var DoubleAnimation_5bcbf8c30cec4cd5892e160b593e9be3 = new global::System.Windows.Media.Animation.DoubleAnimation();
            DoubleAnimation_5bcbf8c30cec4cd5892e160b593e9be3.Duration = (global::System.Windows.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Duration), @"0");
            DoubleAnimation_5bcbf8c30cec4cd5892e160b593e9be3.To = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(DoubleAnimation_5bcbf8c30cec4cd5892e160b593e9be3, @"Background");

            Storyboard_2bb450174cb346498fcf33d32b6f274f.Children.Add(DoubleAnimation_8d6711a3eba649a9be2dda911d715527);
            Storyboard_2bb450174cb346498fcf33d32b6f274f.Children.Add(DoubleAnimation_5bcbf8c30cec4cd5892e160b593e9be3);


            VisualState_ca5b55d3d46c409c90a86e18928482f1.Storyboard = Storyboard_2bb450174cb346498fcf33d32b6f274f;


            var VisualState_aafe8f6e1d2d4073a29b0855d2c0d69c = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_aafe8f6e1d2d4073a29b0855d2c0d69c);
            VisualState_aafe8f6e1d2d4073a29b0855d2c0d69c.Name = "Disabled";
            var Storyboard_f9e39415f22e4a8cac43eb1bdb4c8bac = new global::System.Windows.Media.Animation.Storyboard();
            var DoubleAnimation_f9831317b68747c58e5a5213fa56d01e = new global::System.Windows.Media.Animation.DoubleAnimation();
            DoubleAnimation_f9831317b68747c58e5a5213fa56d01e.Duration = (global::System.Windows.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Duration), @"0");
            DoubleAnimation_f9831317b68747c58e5a5213fa56d01e.To = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"1");
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(DoubleAnimation_f9831317b68747c58e5a5213fa56d01e, @"BackgroundDisabled");

            var DoubleAnimation_bdde83f3759744959b6de99a1903aa77 = new global::System.Windows.Media.Animation.DoubleAnimation();
            DoubleAnimation_bdde83f3759744959b6de99a1903aa77.Duration = (global::System.Windows.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Duration), @"0");
            DoubleAnimation_bdde83f3759744959b6de99a1903aa77.To = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(DoubleAnimation_bdde83f3759744959b6de99a1903aa77, @"Background");

            Storyboard_f9e39415f22e4a8cac43eb1bdb4c8bac.Children.Add(DoubleAnimation_f9831317b68747c58e5a5213fa56d01e);
            Storyboard_f9e39415f22e4a8cac43eb1bdb4c8bac.Children.Add(DoubleAnimation_bdde83f3759744959b6de99a1903aa77);


            VisualState_aafe8f6e1d2d4073a29b0855d2c0d69c.Storyboard = Storyboard_f9e39415f22e4a8cac43eb1bdb4c8bac;


            VisualStateGroup_df0179b92f0f4bd299de2ea203a6a00c.States.Add(VisualState_8f1d1fe38ea74a56bf764a937ed3e83e);
            VisualStateGroup_df0179b92f0f4bd299de2ea203a6a00c.States.Add(VisualState_a05a8180b5e34367aa476d4c55e72b53);
            VisualStateGroup_df0179b92f0f4bd299de2ea203a6a00c.States.Add(VisualState_ca5b55d3d46c409c90a86e18928482f1);
            VisualStateGroup_df0179b92f0f4bd299de2ea203a6a00c.States.Add(VisualState_aafe8f6e1d2d4073a29b0855d2c0d69c);


            ((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_df0179b92f0f4bd299de2ea203a6a00c);

            var Border_213e93fb993146df8dc25081b4bca296 = new global::System.Windows.Controls.Border();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Background", Border_213e93fb993146df8dc25081b4bca296);
            Border_213e93fb993146df8dc25081b4bca296.Name = "Background";
            var Binding_e2e38253117543f2b4c1a43b6a368232 = new global::System.Windows.Data.Binding();
            Binding_e2e38253117543f2b4c1a43b6a368232.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_cd6a9748a12c4aaeb6859ab0074f8b02 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_cd6a9748a12c4aaeb6859ab0074f8b02.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_e2e38253117543f2b4c1a43b6a368232.RelativeSource = RelativeSource_cd6a9748a12c4aaeb6859ab0074f8b02;


            Binding_e2e38253117543f2b4c1a43b6a368232.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;

            var Binding_c0eff2e736e747289a380985b09acef5 = new global::System.Windows.Data.Binding();
            Binding_c0eff2e736e747289a380985b09acef5.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
            var RelativeSource_9847ed462b194338ac9ab4952192df0c = new global::System.Windows.Data.RelativeSource();
            RelativeSource_9847ed462b194338ac9ab4952192df0c.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_c0eff2e736e747289a380985b09acef5.RelativeSource = RelativeSource_9847ed462b194338ac9ab4952192df0c;


            Binding_c0eff2e736e747289a380985b09acef5.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;

            var Binding_48ecc0d1902b411fa2435fb061808c3d = new global::System.Windows.Data.Binding();
            Binding_48ecc0d1902b411fa2435fb061808c3d.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
            var RelativeSource_85064d4f6341401dacfcb93153689301 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_85064d4f6341401dacfcb93153689301.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_48ecc0d1902b411fa2435fb061808c3d.RelativeSource = RelativeSource_85064d4f6341401dacfcb93153689301;


            Binding_48ecc0d1902b411fa2435fb061808c3d.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;


            var Border_4daad864e9a64015a672cacc4cb83412 = new global::System.Windows.Controls.Border();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("BackgroundPointerOver", Border_4daad864e9a64015a672cacc4cb83412);
            Border_4daad864e9a64015a672cacc4cb83412.Name = "BackgroundPointerOver";
            Border_4daad864e9a64015a672cacc4cb83412.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#11000000");
            Border_4daad864e9a64015a672cacc4cb83412.Opacity = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");
            var Binding_68beccaa074a46d09a143d687cc3b151 = new global::System.Windows.Data.Binding();
            Binding_68beccaa074a46d09a143d687cc3b151.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
            var RelativeSource_1c4dc12d7b2a4434945889e5c3c1561e = new global::System.Windows.Data.RelativeSource();
            RelativeSource_1c4dc12d7b2a4434945889e5c3c1561e.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_68beccaa074a46d09a143d687cc3b151.RelativeSource = RelativeSource_1c4dc12d7b2a4434945889e5c3c1561e;


            Binding_68beccaa074a46d09a143d687cc3b151.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;

            var Binding_e81ad729880c46b9a01c6d7fb8b353c7 = new global::System.Windows.Data.Binding();
            Binding_e81ad729880c46b9a01c6d7fb8b353c7.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
            var RelativeSource_8fa792d2b94a4b4dae296568451569a1 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_8fa792d2b94a4b4dae296568451569a1.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_e81ad729880c46b9a01c6d7fb8b353c7.RelativeSource = RelativeSource_8fa792d2b94a4b4dae296568451569a1;


            Binding_e81ad729880c46b9a01c6d7fb8b353c7.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;


            var Border_4b29416d30db4307b8b8910ddc03be0d = new global::System.Windows.Controls.Border();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("BackgroundPressed", Border_4b29416d30db4307b8b8910ddc03be0d);
            Border_4b29416d30db4307b8b8910ddc03be0d.Name = "BackgroundPressed";
            Border_4b29416d30db4307b8b8910ddc03be0d.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#22000000");
            Border_4b29416d30db4307b8b8910ddc03be0d.Opacity = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");
            var Binding_efe76c02f3bb4b08b8d79fff00514389 = new global::System.Windows.Data.Binding();
            Binding_efe76c02f3bb4b08b8d79fff00514389.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
            var RelativeSource_0cf1173a131842108651a4df3ec79575 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_0cf1173a131842108651a4df3ec79575.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_efe76c02f3bb4b08b8d79fff00514389.RelativeSource = RelativeSource_0cf1173a131842108651a4df3ec79575;


            Binding_efe76c02f3bb4b08b8d79fff00514389.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;

            var Binding_656a147c0f7e4b3c827d142e564d50e3 = new global::System.Windows.Data.Binding();
            Binding_656a147c0f7e4b3c827d142e564d50e3.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
            var RelativeSource_0d7d402b573d48a6b6a748c3b3fe238d = new global::System.Windows.Data.RelativeSource();
            RelativeSource_0d7d402b573d48a6b6a748c3b3fe238d.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_656a147c0f7e4b3c827d142e564d50e3.RelativeSource = RelativeSource_0d7d402b573d48a6b6a748c3b3fe238d;


            Binding_656a147c0f7e4b3c827d142e564d50e3.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;


            var Border_10fec0a9e1b24bf0a8e82141e3653e2f = new global::System.Windows.Controls.Border();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("BackgroundDisabled", Border_10fec0a9e1b24bf0a8e82141e3653e2f);
            Border_10fec0a9e1b24bf0a8e82141e3653e2f.Name = "BackgroundDisabled";
            Border_10fec0a9e1b24bf0a8e82141e3653e2f.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#33FFFFFF");
            Border_10fec0a9e1b24bf0a8e82141e3653e2f.Opacity = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");
            var Binding_380e51af20794eba9a64200c086b5c83 = new global::System.Windows.Data.Binding();
            Binding_380e51af20794eba9a64200c086b5c83.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
            var RelativeSource_567359a1681e425c94349c8ce35a63c6 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_567359a1681e425c94349c8ce35a63c6.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_380e51af20794eba9a64200c086b5c83.RelativeSource = RelativeSource_567359a1681e425c94349c8ce35a63c6;


            Binding_380e51af20794eba9a64200c086b5c83.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;

            var Binding_d4bf21d77f9845a6938465dc95e45ad4 = new global::System.Windows.Data.Binding();
            Binding_d4bf21d77f9845a6938465dc95e45ad4.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
            var RelativeSource_0bd30d52ccb34a94befa99e25a99c5bb = new global::System.Windows.Data.RelativeSource();
            RelativeSource_0bd30d52ccb34a94befa99e25a99c5bb.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_d4bf21d77f9845a6938465dc95e45ad4.RelativeSource = RelativeSource_0bd30d52ccb34a94befa99e25a99c5bb;


            Binding_d4bf21d77f9845a6938465dc95e45ad4.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;


            Grid_6823781fbae44ec3ae1881f9639427c0.Children.Add(Border_213e93fb993146df8dc25081b4bca296);
            Grid_6823781fbae44ec3ae1881f9639427c0.Children.Add(Border_4daad864e9a64015a672cacc4cb83412);
            Grid_6823781fbae44ec3ae1881f9639427c0.Children.Add(Border_4b29416d30db4307b8b8910ddc03be0d);
            Grid_6823781fbae44ec3ae1881f9639427c0.Children.Add(Border_10fec0a9e1b24bf0a8e82141e3653e2f);



            Border_213e93fb993146df8dc25081b4bca296.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_e2e38253117543f2b4c1a43b6a368232);
            Border_213e93fb993146df8dc25081b4bca296.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_c0eff2e736e747289a380985b09acef5);
            Border_213e93fb993146df8dc25081b4bca296.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_48ecc0d1902b411fa2435fb061808c3d);
            Border_4daad864e9a64015a672cacc4cb83412.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_68beccaa074a46d09a143d687cc3b151);
            Border_4daad864e9a64015a672cacc4cb83412.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_e81ad729880c46b9a01c6d7fb8b353c7);
            Border_4b29416d30db4307b8b8910ddc03be0d.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_efe76c02f3bb4b08b8d79fff00514389);
            Border_4b29416d30db4307b8b8910ddc03be0d.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_656a147c0f7e4b3c827d142e564d50e3);
            Border_10fec0a9e1b24bf0a8e82141e3653e2f.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_380e51af20794eba9a64200c086b5c83);
            Border_10fec0a9e1b24bf0a8e82141e3653e2f.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_d4bf21d77f9845a6938465dc95e45ad4);

            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(DoubleAnimation_b9f9e8f1a5a84ec28f20d795a4f4308a,
                new global::System.Windows.PropertyPath(
                    "Opacity",
                    "Opacity",
                    accessVisualStateProperty_4e4d6374fec24e15b0023e9a82203bf4,
                    setVisualStateProperty_4e4d6374fec24e15b0023e9a82203bf4,
                    setLocalVisualStateProperty_4e4d6374fec24e15b0023e9a82203bf4,
                    getVisualStateProperty_4e4d6374fec24e15b0023e9a82203bf4));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(DoubleAnimation_b9f9e8f1a5a84ec28f20d795a4f4308a, Border_4daad864e9a64015a672cacc4cb83412);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(DoubleAnimation_9600c41cc1eb41798dd72e92d9798a9f,
                new global::System.Windows.PropertyPath(
                    "Opacity",
                    "Opacity",
                    accessVisualStateProperty_8f8058186681457e8d459ce9c6e9f66b,
                    setVisualStateProperty_8f8058186681457e8d459ce9c6e9f66b,
                    setLocalVisualStateProperty_8f8058186681457e8d459ce9c6e9f66b,
                    getVisualStateProperty_8f8058186681457e8d459ce9c6e9f66b));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(DoubleAnimation_9600c41cc1eb41798dd72e92d9798a9f, Border_213e93fb993146df8dc25081b4bca296);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(DoubleAnimation_8d6711a3eba649a9be2dda911d715527,
                new global::System.Windows.PropertyPath(
                    "Opacity",
                    "Opacity",
                    accessVisualStateProperty_7b2742bb384447bfb447e66f73afa03a,
                    setVisualStateProperty_7b2742bb384447bfb447e66f73afa03a,
                    setLocalVisualStateProperty_7b2742bb384447bfb447e66f73afa03a,
                    getVisualStateProperty_7b2742bb384447bfb447e66f73afa03a));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(DoubleAnimation_8d6711a3eba649a9be2dda911d715527, Border_4b29416d30db4307b8b8910ddc03be0d);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(DoubleAnimation_5bcbf8c30cec4cd5892e160b593e9be3,
                new global::System.Windows.PropertyPath(
                    "Opacity",
                    "Opacity",
                    accessVisualStateProperty_72659532984249a9bd2e30737c7ed5a8,
                    setVisualStateProperty_72659532984249a9bd2e30737c7ed5a8,
                    setLocalVisualStateProperty_72659532984249a9bd2e30737c7ed5a8,
                    getVisualStateProperty_72659532984249a9bd2e30737c7ed5a8));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(DoubleAnimation_5bcbf8c30cec4cd5892e160b593e9be3, Border_213e93fb993146df8dc25081b4bca296);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(DoubleAnimation_f9831317b68747c58e5a5213fa56d01e,
                new global::System.Windows.PropertyPath(
                    "Opacity",
                    "Opacity",
                    accessVisualStateProperty_c832abb588b24cef9d0c63e2a898bd8e,
                    setVisualStateProperty_c832abb588b24cef9d0c63e2a898bd8e,
                    setLocalVisualStateProperty_c832abb588b24cef9d0c63e2a898bd8e,
                    getVisualStateProperty_c832abb588b24cef9d0c63e2a898bd8e));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(DoubleAnimation_f9831317b68747c58e5a5213fa56d01e, Border_10fec0a9e1b24bf0a8e82141e3653e2f);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(DoubleAnimation_bdde83f3759744959b6de99a1903aa77,
                new global::System.Windows.PropertyPath(
                    "Opacity",
                    "Opacity",
                    accessVisualStateProperty_1757611cf6164cc5afbb34df94b52850,
                    setVisualStateProperty_1757611cf6164cc5afbb34df94b52850,
                    setLocalVisualStateProperty_1757611cf6164cc5afbb34df94b52850,
                    getVisualStateProperty_1757611cf6164cc5afbb34df94b52850));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(DoubleAnimation_bdde83f3759744959b6de99a1903aa77, Border_213e93fb993146df8dc25081b4bca296);

            templateInstance_e173eb2aae5246999057bfa4ae154a20.TemplateContent = Grid_6823781fbae44ec3ae1881f9639427c0;
            return templateInstance_e173eb2aae5246999057bfa4ae154a20;
        }
    }
}
#else
namespace Windows.UI.Xaml.Controls.Primitives
{
    internal class INTERNAL_DefaultThumbStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_96f68aa06ac5423ea04363f0e1cc6b17 = new global::Windows.UI.Xaml.Style();
                Style_96f68aa06ac5423ea04363f0e1cc6b17.TargetType = typeof(global::Windows.UI.Xaml.Controls.Primitives.Thumb);
                var Setter_68c4f256583c417cb62ebdfad52e19a7 = new global::Windows.UI.Xaml.Setter();
                Setter_68c4f256583c417cb62ebdfad52e19a7.Property = global::Windows.UI.Xaml.Controls.Primitives.Thumb.BackgroundProperty;
                Setter_68c4f256583c417cb62ebdfad52e19a7.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFE2E2E2");

                var Setter_7a6a7f16478c4a10a08e61f3d67c17e7 = new global::Windows.UI.Xaml.Setter();
                Setter_7a6a7f16478c4a10a08e61f3d67c17e7.Property = global::Windows.UI.Xaml.Controls.Primitives.Thumb.BorderThicknessProperty;
                Setter_7a6a7f16478c4a10a08e61f3d67c17e7.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0");

                var Setter_ecba2487d1094c93901b8dce0cf4f2b0 = new global::Windows.UI.Xaml.Setter();
                Setter_ecba2487d1094c93901b8dce0cf4f2b0.Property = global::Windows.UI.Xaml.Controls.Primitives.Thumb.TemplateProperty;
                var ControlTemplate_c8e6fa19521344109414b9d44ea94918 = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_c8e6fa19521344109414b9d44ea94918.TargetType = typeof(global::Windows.UI.Xaml.Controls.Primitives.Thumb);
                ControlTemplate_c8e6fa19521344109414b9d44ea94918.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_c8e6fa19521344109414b9d44ea94918);

                Setter_ecba2487d1094c93901b8dce0cf4f2b0.Value = ControlTemplate_c8e6fa19521344109414b9d44ea94918;


                Style_96f68aa06ac5423ea04363f0e1cc6b17.Setters.Add(Setter_68c4f256583c417cb62ebdfad52e19a7);
                Style_96f68aa06ac5423ea04363f0e1cc6b17.Setters.Add(Setter_7a6a7f16478c4a10a08e61f3d67c17e7);
                Style_96f68aa06ac5423ea04363f0e1cc6b17.Setters.Add(Setter_ecba2487d1094c93901b8dce0cf4f2b0);


                DefaultStyle = Style_96f68aa06ac5423ea04363f0e1cc6b17;
            }

            return DefaultStyle;
        }



        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_4e4d6374fec24e15b0023e9a82203bf4(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_4e4d6374fec24e15b0023e9a82203bf4(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_4e4d6374fec24e15b0023e9a82203bf4(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_4e4d6374fec24e15b0023e9a82203bf4(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_8f8058186681457e8d459ce9c6e9f66b(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_8f8058186681457e8d459ce9c6e9f66b(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_8f8058186681457e8d459ce9c6e9f66b(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_8f8058186681457e8d459ce9c6e9f66b(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_7b2742bb384447bfb447e66f73afa03a(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_7b2742bb384447bfb447e66f73afa03a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_7b2742bb384447bfb447e66f73afa03a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_7b2742bb384447bfb447e66f73afa03a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_72659532984249a9bd2e30737c7ed5a8(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_72659532984249a9bd2e30737c7ed5a8(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_72659532984249a9bd2e30737c7ed5a8(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_72659532984249a9bd2e30737c7ed5a8(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_c832abb588b24cef9d0c63e2a898bd8e(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_c832abb588b24cef9d0c63e2a898bd8e(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_c832abb588b24cef9d0c63e2a898bd8e(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_c832abb588b24cef9d0c63e2a898bd8e(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_1757611cf6164cc5afbb34df94b52850(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_1757611cf6164cc5afbb34df94b52850(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_1757611cf6164cc5afbb34df94b52850(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_1757611cf6164cc5afbb34df94b52850(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_c8e6fa19521344109414b9d44ea94918(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_e173eb2aae5246999057bfa4ae154a20 = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_e173eb2aae5246999057bfa4ae154a20.TemplateOwner = templateOwner;
            var Grid_6823781fbae44ec3ae1881f9639427c0 = new global::Windows.UI.Xaml.Controls.Grid();
            var VisualStateGroup_df0179b92f0f4bd299de2ea203a6a00c = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_df0179b92f0f4bd299de2ea203a6a00c);
            VisualStateGroup_df0179b92f0f4bd299de2ea203a6a00c.Name = "CommonStates";
            var VisualState_8f1d1fe38ea74a56bf764a937ed3e83e = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Normal", VisualState_8f1d1fe38ea74a56bf764a937ed3e83e);
            VisualState_8f1d1fe38ea74a56bf764a937ed3e83e.Name = "Normal";

            var VisualState_a05a8180b5e34367aa476d4c55e72b53 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("PointerOver", VisualState_a05a8180b5e34367aa476d4c55e72b53);
            VisualState_a05a8180b5e34367aa476d4c55e72b53.Name = "PointerOver";
            var Storyboard_9f3ee418257044abad2960a5a02c506b = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var DoubleAnimation_b9f9e8f1a5a84ec28f20d795a4f4308a = new global::Windows.UI.Xaml.Media.Animation.DoubleAnimation();
            DoubleAnimation_b9f9e8f1a5a84ec28f20d795a4f4308a.Duration = (global::Windows.UI.Xaml.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Duration), @"0");
            DoubleAnimation_b9f9e8f1a5a84ec28f20d795a4f4308a.To = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"1");
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(DoubleAnimation_b9f9e8f1a5a84ec28f20d795a4f4308a, @"BackgroundPointerOver");

            var DoubleAnimation_9600c41cc1eb41798dd72e92d9798a9f = new global::Windows.UI.Xaml.Media.Animation.DoubleAnimation();
            DoubleAnimation_9600c41cc1eb41798dd72e92d9798a9f.Duration = (global::Windows.UI.Xaml.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Duration), @"0");
            DoubleAnimation_9600c41cc1eb41798dd72e92d9798a9f.To = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(DoubleAnimation_9600c41cc1eb41798dd72e92d9798a9f, @"Background");

            Storyboard_9f3ee418257044abad2960a5a02c506b.Children.Add(DoubleAnimation_b9f9e8f1a5a84ec28f20d795a4f4308a);
            Storyboard_9f3ee418257044abad2960a5a02c506b.Children.Add(DoubleAnimation_9600c41cc1eb41798dd72e92d9798a9f);


            VisualState_a05a8180b5e34367aa476d4c55e72b53.Storyboard = Storyboard_9f3ee418257044abad2960a5a02c506b;


            var VisualState_ca5b55d3d46c409c90a86e18928482f1 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Pressed", VisualState_ca5b55d3d46c409c90a86e18928482f1);
            VisualState_ca5b55d3d46c409c90a86e18928482f1.Name = "Pressed";
            var Storyboard_2bb450174cb346498fcf33d32b6f274f = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var DoubleAnimation_8d6711a3eba649a9be2dda911d715527 = new global::Windows.UI.Xaml.Media.Animation.DoubleAnimation();
            DoubleAnimation_8d6711a3eba649a9be2dda911d715527.Duration = (global::Windows.UI.Xaml.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Duration), @"0");
            DoubleAnimation_8d6711a3eba649a9be2dda911d715527.To = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"1");
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(DoubleAnimation_8d6711a3eba649a9be2dda911d715527, @"BackgroundPressed");

            var DoubleAnimation_5bcbf8c30cec4cd5892e160b593e9be3 = new global::Windows.UI.Xaml.Media.Animation.DoubleAnimation();
            DoubleAnimation_5bcbf8c30cec4cd5892e160b593e9be3.Duration = (global::Windows.UI.Xaml.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Duration), @"0");
            DoubleAnimation_5bcbf8c30cec4cd5892e160b593e9be3.To = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(DoubleAnimation_5bcbf8c30cec4cd5892e160b593e9be3, @"Background");

            Storyboard_2bb450174cb346498fcf33d32b6f274f.Children.Add(DoubleAnimation_8d6711a3eba649a9be2dda911d715527);
            Storyboard_2bb450174cb346498fcf33d32b6f274f.Children.Add(DoubleAnimation_5bcbf8c30cec4cd5892e160b593e9be3);


            VisualState_ca5b55d3d46c409c90a86e18928482f1.Storyboard = Storyboard_2bb450174cb346498fcf33d32b6f274f;


            var VisualState_aafe8f6e1d2d4073a29b0855d2c0d69c = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_aafe8f6e1d2d4073a29b0855d2c0d69c);
            VisualState_aafe8f6e1d2d4073a29b0855d2c0d69c.Name = "Disabled";
            var Storyboard_f9e39415f22e4a8cac43eb1bdb4c8bac = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var DoubleAnimation_f9831317b68747c58e5a5213fa56d01e = new global::Windows.UI.Xaml.Media.Animation.DoubleAnimation();
            DoubleAnimation_f9831317b68747c58e5a5213fa56d01e.Duration = (global::Windows.UI.Xaml.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Duration), @"0");
            DoubleAnimation_f9831317b68747c58e5a5213fa56d01e.To = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"1");
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(DoubleAnimation_f9831317b68747c58e5a5213fa56d01e, @"BackgroundDisabled");

            var DoubleAnimation_bdde83f3759744959b6de99a1903aa77 = new global::Windows.UI.Xaml.Media.Animation.DoubleAnimation();
            DoubleAnimation_bdde83f3759744959b6de99a1903aa77.Duration = (global::Windows.UI.Xaml.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Duration), @"0");
            DoubleAnimation_bdde83f3759744959b6de99a1903aa77.To = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(DoubleAnimation_bdde83f3759744959b6de99a1903aa77, @"Background");

            Storyboard_f9e39415f22e4a8cac43eb1bdb4c8bac.Children.Add(DoubleAnimation_f9831317b68747c58e5a5213fa56d01e);
            Storyboard_f9e39415f22e4a8cac43eb1bdb4c8bac.Children.Add(DoubleAnimation_bdde83f3759744959b6de99a1903aa77);


            VisualState_aafe8f6e1d2d4073a29b0855d2c0d69c.Storyboard = Storyboard_f9e39415f22e4a8cac43eb1bdb4c8bac;


            VisualStateGroup_df0179b92f0f4bd299de2ea203a6a00c.States.Add(VisualState_8f1d1fe38ea74a56bf764a937ed3e83e);
            VisualStateGroup_df0179b92f0f4bd299de2ea203a6a00c.States.Add(VisualState_a05a8180b5e34367aa476d4c55e72b53);
            VisualStateGroup_df0179b92f0f4bd299de2ea203a6a00c.States.Add(VisualState_ca5b55d3d46c409c90a86e18928482f1);
            VisualStateGroup_df0179b92f0f4bd299de2ea203a6a00c.States.Add(VisualState_aafe8f6e1d2d4073a29b0855d2c0d69c);


            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_df0179b92f0f4bd299de2ea203a6a00c);

            var Border_213e93fb993146df8dc25081b4bca296 = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Background", Border_213e93fb993146df8dc25081b4bca296);
            Border_213e93fb993146df8dc25081b4bca296.Name = "Background";
            var Binding_e2e38253117543f2b4c1a43b6a368232 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_e2e38253117543f2b4c1a43b6a368232.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_cd6a9748a12c4aaeb6859ab0074f8b02 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_cd6a9748a12c4aaeb6859ab0074f8b02.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_e2e38253117543f2b4c1a43b6a368232.RelativeSource = RelativeSource_cd6a9748a12c4aaeb6859ab0074f8b02;


            Binding_e2e38253117543f2b4c1a43b6a368232.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;

            var Binding_c0eff2e736e747289a380985b09acef5 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_c0eff2e736e747289a380985b09acef5.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_9847ed462b194338ac9ab4952192df0c = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_9847ed462b194338ac9ab4952192df0c.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_c0eff2e736e747289a380985b09acef5.RelativeSource = RelativeSource_9847ed462b194338ac9ab4952192df0c;


            Binding_c0eff2e736e747289a380985b09acef5.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;

            var Binding_48ecc0d1902b411fa2435fb061808c3d = new global::Windows.UI.Xaml.Data.Binding();
            Binding_48ecc0d1902b411fa2435fb061808c3d.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_85064d4f6341401dacfcb93153689301 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_85064d4f6341401dacfcb93153689301.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_48ecc0d1902b411fa2435fb061808c3d.RelativeSource = RelativeSource_85064d4f6341401dacfcb93153689301;


            Binding_48ecc0d1902b411fa2435fb061808c3d.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;


            var Border_4daad864e9a64015a672cacc4cb83412 = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("BackgroundPointerOver", Border_4daad864e9a64015a672cacc4cb83412);
            Border_4daad864e9a64015a672cacc4cb83412.Name = "BackgroundPointerOver";
            Border_4daad864e9a64015a672cacc4cb83412.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#11000000");
            Border_4daad864e9a64015a672cacc4cb83412.Opacity = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");
            var Binding_68beccaa074a46d09a143d687cc3b151 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_68beccaa074a46d09a143d687cc3b151.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_1c4dc12d7b2a4434945889e5c3c1561e = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_1c4dc12d7b2a4434945889e5c3c1561e.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_68beccaa074a46d09a143d687cc3b151.RelativeSource = RelativeSource_1c4dc12d7b2a4434945889e5c3c1561e;


            Binding_68beccaa074a46d09a143d687cc3b151.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;

            var Binding_e81ad729880c46b9a01c6d7fb8b353c7 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_e81ad729880c46b9a01c6d7fb8b353c7.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_8fa792d2b94a4b4dae296568451569a1 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_8fa792d2b94a4b4dae296568451569a1.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_e81ad729880c46b9a01c6d7fb8b353c7.RelativeSource = RelativeSource_8fa792d2b94a4b4dae296568451569a1;


            Binding_e81ad729880c46b9a01c6d7fb8b353c7.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;


            var Border_4b29416d30db4307b8b8910ddc03be0d = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("BackgroundPressed", Border_4b29416d30db4307b8b8910ddc03be0d);
            Border_4b29416d30db4307b8b8910ddc03be0d.Name = "BackgroundPressed";
            Border_4b29416d30db4307b8b8910ddc03be0d.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#22000000");
            Border_4b29416d30db4307b8b8910ddc03be0d.Opacity = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");
            var Binding_efe76c02f3bb4b08b8d79fff00514389 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_efe76c02f3bb4b08b8d79fff00514389.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_0cf1173a131842108651a4df3ec79575 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_0cf1173a131842108651a4df3ec79575.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_efe76c02f3bb4b08b8d79fff00514389.RelativeSource = RelativeSource_0cf1173a131842108651a4df3ec79575;


            Binding_efe76c02f3bb4b08b8d79fff00514389.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;

            var Binding_656a147c0f7e4b3c827d142e564d50e3 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_656a147c0f7e4b3c827d142e564d50e3.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_0d7d402b573d48a6b6a748c3b3fe238d = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_0d7d402b573d48a6b6a748c3b3fe238d.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_656a147c0f7e4b3c827d142e564d50e3.RelativeSource = RelativeSource_0d7d402b573d48a6b6a748c3b3fe238d;


            Binding_656a147c0f7e4b3c827d142e564d50e3.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;


            var Border_10fec0a9e1b24bf0a8e82141e3653e2f = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("BackgroundDisabled", Border_10fec0a9e1b24bf0a8e82141e3653e2f);
            Border_10fec0a9e1b24bf0a8e82141e3653e2f.Name = "BackgroundDisabled";
            Border_10fec0a9e1b24bf0a8e82141e3653e2f.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#33FFFFFF");
            Border_10fec0a9e1b24bf0a8e82141e3653e2f.Opacity = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");
            var Binding_380e51af20794eba9a64200c086b5c83 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_380e51af20794eba9a64200c086b5c83.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_567359a1681e425c94349c8ce35a63c6 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_567359a1681e425c94349c8ce35a63c6.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_380e51af20794eba9a64200c086b5c83.RelativeSource = RelativeSource_567359a1681e425c94349c8ce35a63c6;


            Binding_380e51af20794eba9a64200c086b5c83.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;

            var Binding_d4bf21d77f9845a6938465dc95e45ad4 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_d4bf21d77f9845a6938465dc95e45ad4.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_0bd30d52ccb34a94befa99e25a99c5bb = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_0bd30d52ccb34a94befa99e25a99c5bb.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_d4bf21d77f9845a6938465dc95e45ad4.RelativeSource = RelativeSource_0bd30d52ccb34a94befa99e25a99c5bb;


            Binding_d4bf21d77f9845a6938465dc95e45ad4.TemplateOwner = templateInstance_e173eb2aae5246999057bfa4ae154a20;


            Grid_6823781fbae44ec3ae1881f9639427c0.Children.Add(Border_213e93fb993146df8dc25081b4bca296);
            Grid_6823781fbae44ec3ae1881f9639427c0.Children.Add(Border_4daad864e9a64015a672cacc4cb83412);
            Grid_6823781fbae44ec3ae1881f9639427c0.Children.Add(Border_4b29416d30db4307b8b8910ddc03be0d);
            Grid_6823781fbae44ec3ae1881f9639427c0.Children.Add(Border_10fec0a9e1b24bf0a8e82141e3653e2f);



            Border_213e93fb993146df8dc25081b4bca296.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_e2e38253117543f2b4c1a43b6a368232);
            Border_213e93fb993146df8dc25081b4bca296.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_c0eff2e736e747289a380985b09acef5);
            Border_213e93fb993146df8dc25081b4bca296.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_48ecc0d1902b411fa2435fb061808c3d);
            Border_4daad864e9a64015a672cacc4cb83412.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_68beccaa074a46d09a143d687cc3b151);
            Border_4daad864e9a64015a672cacc4cb83412.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_e81ad729880c46b9a01c6d7fb8b353c7);
            Border_4b29416d30db4307b8b8910ddc03be0d.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_efe76c02f3bb4b08b8d79fff00514389);
            Border_4b29416d30db4307b8b8910ddc03be0d.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_656a147c0f7e4b3c827d142e564d50e3);
            Border_10fec0a9e1b24bf0a8e82141e3653e2f.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_380e51af20794eba9a64200c086b5c83);
            Border_10fec0a9e1b24bf0a8e82141e3653e2f.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_d4bf21d77f9845a6938465dc95e45ad4);

            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(DoubleAnimation_b9f9e8f1a5a84ec28f20d795a4f4308a,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Opacity",
                    "Opacity",
                    accessVisualStateProperty_4e4d6374fec24e15b0023e9a82203bf4,
                    setVisualStateProperty_4e4d6374fec24e15b0023e9a82203bf4,
                    setLocalVisualStateProperty_4e4d6374fec24e15b0023e9a82203bf4,
                    getVisualStateProperty_4e4d6374fec24e15b0023e9a82203bf4));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(DoubleAnimation_b9f9e8f1a5a84ec28f20d795a4f4308a, Border_4daad864e9a64015a672cacc4cb83412);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(DoubleAnimation_9600c41cc1eb41798dd72e92d9798a9f,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Opacity",
                    "Opacity",
                    accessVisualStateProperty_8f8058186681457e8d459ce9c6e9f66b,
                    setVisualStateProperty_8f8058186681457e8d459ce9c6e9f66b,
                    setLocalVisualStateProperty_8f8058186681457e8d459ce9c6e9f66b,
                    getVisualStateProperty_8f8058186681457e8d459ce9c6e9f66b));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(DoubleAnimation_9600c41cc1eb41798dd72e92d9798a9f, Border_213e93fb993146df8dc25081b4bca296);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(DoubleAnimation_8d6711a3eba649a9be2dda911d715527,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Opacity",
                    "Opacity",
                    accessVisualStateProperty_7b2742bb384447bfb447e66f73afa03a,
                    setVisualStateProperty_7b2742bb384447bfb447e66f73afa03a,
                    setLocalVisualStateProperty_7b2742bb384447bfb447e66f73afa03a,
                    getVisualStateProperty_7b2742bb384447bfb447e66f73afa03a));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(DoubleAnimation_8d6711a3eba649a9be2dda911d715527, Border_4b29416d30db4307b8b8910ddc03be0d);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(DoubleAnimation_5bcbf8c30cec4cd5892e160b593e9be3,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Opacity",
                    "Opacity",
                    accessVisualStateProperty_72659532984249a9bd2e30737c7ed5a8,
                    setVisualStateProperty_72659532984249a9bd2e30737c7ed5a8,
                    setLocalVisualStateProperty_72659532984249a9bd2e30737c7ed5a8,
                    getVisualStateProperty_72659532984249a9bd2e30737c7ed5a8));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(DoubleAnimation_5bcbf8c30cec4cd5892e160b593e9be3, Border_213e93fb993146df8dc25081b4bca296);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(DoubleAnimation_f9831317b68747c58e5a5213fa56d01e,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Opacity",
                    "Opacity",
                    accessVisualStateProperty_c832abb588b24cef9d0c63e2a898bd8e,
                    setVisualStateProperty_c832abb588b24cef9d0c63e2a898bd8e,
                    setLocalVisualStateProperty_c832abb588b24cef9d0c63e2a898bd8e,
                    getVisualStateProperty_c832abb588b24cef9d0c63e2a898bd8e));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(DoubleAnimation_f9831317b68747c58e5a5213fa56d01e, Border_10fec0a9e1b24bf0a8e82141e3653e2f);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(DoubleAnimation_bdde83f3759744959b6de99a1903aa77,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Opacity",
                    "Opacity",
                    accessVisualStateProperty_1757611cf6164cc5afbb34df94b52850,
                    setVisualStateProperty_1757611cf6164cc5afbb34df94b52850,
                    setLocalVisualStateProperty_1757611cf6164cc5afbb34df94b52850,
                    getVisualStateProperty_1757611cf6164cc5afbb34df94b52850));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(DoubleAnimation_bdde83f3759744959b6de99a1903aa77, Border_213e93fb993146df8dc25081b4bca296);

            templateInstance_e173eb2aae5246999057bfa4ae154a20.TemplateContent = Grid_6823781fbae44ec3ae1881f9639427c0;
            return templateInstance_e173eb2aae5246999057bfa4ae154a20;
        }
    }
}
#endif