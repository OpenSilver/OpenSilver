
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
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
    internal class INTERNAL_DefaultButtonStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_27761b772dd042038ccd0a8411db434d = new global::System.Windows.Style();
                Style_27761b772dd042038ccd0a8411db434d.TargetType = typeof(global::System.Windows.Controls.Button);
                var Setter_293523e2a6504050b941ad632a73ae5c = new global::System.Windows.Setter();
                Setter_293523e2a6504050b941ad632a73ae5c.Property = global::System.Windows.Controls.Button.BackgroundProperty;
                Setter_293523e2a6504050b941ad632a73ae5c.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFE2E2E2");

                var Setter_406bdcfd4a14410d8fe9c2d742ce11a8 = new global::System.Windows.Setter();
                Setter_406bdcfd4a14410d8fe9c2d742ce11a8.Property = global::System.Windows.Controls.Button.ForegroundProperty;
                Setter_406bdcfd4a14410d8fe9c2d742ce11a8.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Black");

                var Setter_7f001f6302364f79bebb4e52102663f2 = new global::System.Windows.Setter();
                Setter_7f001f6302364f79bebb4e52102663f2.Property = global::System.Windows.Controls.Button.BorderThicknessProperty;
                Setter_7f001f6302364f79bebb4e52102663f2.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0");

                var Setter_4b74961b18e04e77a89e89845ea54b04 = new global::System.Windows.Setter();
                Setter_4b74961b18e04e77a89e89845ea54b04.Property = global::System.Windows.Controls.Button.PaddingProperty;
                Setter_4b74961b18e04e77a89e89845ea54b04.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"3");

                var Setter_873791a433394d8eab48fcc32e39ec13 = new global::System.Windows.Setter();
                Setter_873791a433394d8eab48fcc32e39ec13.Property = global::System.Windows.Controls.Button.CursorProperty;
                Setter_873791a433394d8eab48fcc32e39ec13.Value = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Hand");

                var Setter_b71ea50f9afd4027811f6263069a36af = new global::System.Windows.Setter();
                Setter_b71ea50f9afd4027811f6263069a36af.Property = global::System.Windows.Controls.Button.HorizontalContentAlignmentProperty;
                Setter_b71ea50f9afd4027811f6263069a36af.Value = global::System.Windows.HorizontalAlignment.Center;

                var Setter_d7c89c827d7a4946bfa675a647c5ce08 = new global::System.Windows.Setter();
                Setter_d7c89c827d7a4946bfa675a647c5ce08.Property = global::System.Windows.Controls.Button.VerticalContentAlignmentProperty;
                Setter_d7c89c827d7a4946bfa675a647c5ce08.Value = global::System.Windows.VerticalAlignment.Center;

                var Setter_749ae2eae0f5448eb6fa70b482754921 = new global::System.Windows.Setter();
                Setter_749ae2eae0f5448eb6fa70b482754921.Property = global::System.Windows.Controls.Button.TemplateProperty;
                var ControlTemplate_7304dcc3a19f4a88a9519ead8cebaa52 = new global::System.Windows.Controls.ControlTemplate();
                ControlTemplate_7304dcc3a19f4a88a9519ead8cebaa52.TargetType = typeof(global::System.Windows.Controls.Button);
                ControlTemplate_7304dcc3a19f4a88a9519ead8cebaa52.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_7304dcc3a19f4a88a9519ead8cebaa52);

                Setter_749ae2eae0f5448eb6fa70b482754921.Value = ControlTemplate_7304dcc3a19f4a88a9519ead8cebaa52;


                Style_27761b772dd042038ccd0a8411db434d.Setters.Add(Setter_293523e2a6504050b941ad632a73ae5c);
                Style_27761b772dd042038ccd0a8411db434d.Setters.Add(Setter_406bdcfd4a14410d8fe9c2d742ce11a8);
                Style_27761b772dd042038ccd0a8411db434d.Setters.Add(Setter_7f001f6302364f79bebb4e52102663f2);
                Style_27761b772dd042038ccd0a8411db434d.Setters.Add(Setter_4b74961b18e04e77a89e89845ea54b04);
                Style_27761b772dd042038ccd0a8411db434d.Setters.Add(Setter_873791a433394d8eab48fcc32e39ec13);
                Style_27761b772dd042038ccd0a8411db434d.Setters.Add(Setter_b71ea50f9afd4027811f6263069a36af);
                Style_27761b772dd042038ccd0a8411db434d.Setters.Add(Setter_d7c89c827d7a4946bfa675a647c5ce08);
                Style_27761b772dd042038ccd0a8411db434d.Setters.Add(Setter_749ae2eae0f5448eb6fa70b482754921);


                DefaultStyle = Style_27761b772dd042038ccd0a8411db434d;
            }

            return DefaultStyle;
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_8771c83003c344a28b6f5dd4717a6597(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_8771c83003c344a28b6f5dd4717a6597(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_8771c83003c344a28b6f5dd4717a6597(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_8771c83003c344a28b6f5dd4717a6597(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_741833e54c914a14b2f2f7ccc151a893(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_741833e54c914a14b2f2f7ccc151a893(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_741833e54c914a14b2f2f7ccc151a893(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_741833e54c914a14b2f2f7ccc151a893(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_65f99b6e906342699a1f750fbcebc34e(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_65f99b6e906342699a1f750fbcebc34e(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_65f99b6e906342699a1f750fbcebc34e(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_65f99b6e906342699a1f750fbcebc34e(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }

        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_7304dcc3a19f4a88a9519ead8cebaa52(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_bc29344b1ad94da8940026d4d5cd38a9 = new global::System.Windows.TemplateInstance();
            templateInstance_bc29344b1ad94da8940026d4d5cd38a9.TemplateOwner = templateOwner;
            var Border_b29d98a31ebf4d149d9d17a45a56fe8d = new global::System.Windows.Controls.Border();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("OuterBorder", Border_b29d98a31ebf4d149d9d17a45a56fe8d);
            Border_b29d98a31ebf4d149d9d17a45a56fe8d.Name = "OuterBorder";
            var VisualStateGroup_7733a25694e14f5e9c0d450df3e38327 = new global::System.Windows.VisualStateGroup();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_7733a25694e14f5e9c0d450df3e38327);
            VisualStateGroup_7733a25694e14f5e9c0d450df3e38327.Name = "CommonStates";
            var VisualState_4906afe15e68428cad9229c32bcb0ce8 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Normal", VisualState_4906afe15e68428cad9229c32bcb0ce8);
            VisualState_4906afe15e68428cad9229c32bcb0ce8.Name = "Normal";

            var VisualState_492656887ed1435791d3b36ac421d395 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("MouseOver", VisualState_492656887ed1435791d3b36ac421d395);
            VisualState_492656887ed1435791d3b36ac421d395.Name = "MouseOver";
            var Storyboard_1278967bd95b491ab90ecd94b23e532f = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_cca6ec7be7b641698ba5fff415012e37 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_cca6ec7be7b641698ba5fff415012e37, @"InnerBorder");
            var DiscreteObjectKeyFrame_13d6bfb0533e449c9b389732578aa9f8 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_13d6bfb0533e449c9b389732578aa9f8.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_13d6bfb0533e449c9b389732578aa9f8.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#11000000");

            ObjectAnimationUsingKeyFrames_cca6ec7be7b641698ba5fff415012e37.KeyFrames.Add(DiscreteObjectKeyFrame_13d6bfb0533e449c9b389732578aa9f8);


            Storyboard_1278967bd95b491ab90ecd94b23e532f.Children.Add(ObjectAnimationUsingKeyFrames_cca6ec7be7b641698ba5fff415012e37);


            VisualState_492656887ed1435791d3b36ac421d395.Storyboard = Storyboard_1278967bd95b491ab90ecd94b23e532f;


            var VisualState_c34bbc7099824fc09923b1264383dae9 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Pressed", VisualState_c34bbc7099824fc09923b1264383dae9);
            VisualState_c34bbc7099824fc09923b1264383dae9.Name = "Pressed";
            var Storyboard_2f2f64e746414388812f3a5d41886375 = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_20a0b97d4370479ba8230b28c255a095 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_20a0b97d4370479ba8230b28c255a095, @"InnerBorder");
            var DiscreteObjectKeyFrame_6d0dcd62810243138d8bcae92a5e24ab = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_6d0dcd62810243138d8bcae92a5e24ab.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_6d0dcd62810243138d8bcae92a5e24ab.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#22000000");

            ObjectAnimationUsingKeyFrames_20a0b97d4370479ba8230b28c255a095.KeyFrames.Add(DiscreteObjectKeyFrame_6d0dcd62810243138d8bcae92a5e24ab);


            Storyboard_2f2f64e746414388812f3a5d41886375.Children.Add(ObjectAnimationUsingKeyFrames_20a0b97d4370479ba8230b28c255a095);


            VisualState_c34bbc7099824fc09923b1264383dae9.Storyboard = Storyboard_2f2f64e746414388812f3a5d41886375;


            var VisualState_7fd412a6e58b41cf891531966de1d0ad = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_7fd412a6e58b41cf891531966de1d0ad);
            VisualState_7fd412a6e58b41cf891531966de1d0ad.Name = "Disabled";
            var Storyboard_b063f7ff0d614b0a9cfa97b2c0c256df = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_82d43e6e777f47afbad4984d60e5ca64 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_82d43e6e777f47afbad4984d60e5ca64, @"InnerBorder");
            var DiscreteObjectKeyFrame_5077045f9093441994378fdd9561d003 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_5077045f9093441994378fdd9561d003.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_5077045f9093441994378fdd9561d003.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#33FFFFFF");

            ObjectAnimationUsingKeyFrames_82d43e6e777f47afbad4984d60e5ca64.KeyFrames.Add(DiscreteObjectKeyFrame_5077045f9093441994378fdd9561d003);


            Storyboard_b063f7ff0d614b0a9cfa97b2c0c256df.Children.Add(ObjectAnimationUsingKeyFrames_82d43e6e777f47afbad4984d60e5ca64);


            VisualState_7fd412a6e58b41cf891531966de1d0ad.Storyboard = Storyboard_b063f7ff0d614b0a9cfa97b2c0c256df;


            VisualStateGroup_7733a25694e14f5e9c0d450df3e38327.States.Add(VisualState_4906afe15e68428cad9229c32bcb0ce8);
            VisualStateGroup_7733a25694e14f5e9c0d450df3e38327.States.Add(VisualState_492656887ed1435791d3b36ac421d395);
            VisualStateGroup_7733a25694e14f5e9c0d450df3e38327.States.Add(VisualState_c34bbc7099824fc09923b1264383dae9);
            VisualStateGroup_7733a25694e14f5e9c0d450df3e38327.States.Add(VisualState_7fd412a6e58b41cf891531966de1d0ad);


            ((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_7733a25694e14f5e9c0d450df3e38327);

            var Border_1a86cfa0349a42d29b702bf1258286f7 = new global::System.Windows.Controls.Border();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("InnerBorder", Border_1a86cfa0349a42d29b702bf1258286f7);
            Border_1a86cfa0349a42d29b702bf1258286f7.Name = "InnerBorder";
            var ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9 = new global::System.Windows.Controls.ContentPresenter();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("ContentPresenter", ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9);
            ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9.Name = "ContentPresenter";
            var Binding_91acea06e00c49548bc2f975f8c708a3 = new global::System.Windows.Data.Binding();
            Binding_91acea06e00c49548bc2f975f8c708a3.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"ContentTemplate");
            var RelativeSource_dd8eb7faf07344eea6aec3930ceb66b1 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_dd8eb7faf07344eea6aec3930ceb66b1.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_91acea06e00c49548bc2f975f8c708a3.RelativeSource = RelativeSource_dd8eb7faf07344eea6aec3930ceb66b1;


            Binding_91acea06e00c49548bc2f975f8c708a3.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;

            var Binding_c6b24d6bd0734b9e9420759f5f45d4a1 = new global::System.Windows.Data.Binding();
            Binding_c6b24d6bd0734b9e9420759f5f45d4a1.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Content");
            var RelativeSource_c31f3b94b8be4b09a12eb08eca31d6a9 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_c31f3b94b8be4b09a12eb08eca31d6a9.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_c6b24d6bd0734b9e9420759f5f45d4a1.RelativeSource = RelativeSource_c31f3b94b8be4b09a12eb08eca31d6a9;


            Binding_c6b24d6bd0734b9e9420759f5f45d4a1.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;

            var Binding_8c979377f5604169a95efa05a955e19b = new global::System.Windows.Data.Binding();
            Binding_8c979377f5604169a95efa05a955e19b.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Padding");
            var RelativeSource_af85d08ded73477390420376e1e77359 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_af85d08ded73477390420376e1e77359.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_8c979377f5604169a95efa05a955e19b.RelativeSource = RelativeSource_af85d08ded73477390420376e1e77359;


            Binding_8c979377f5604169a95efa05a955e19b.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;

            var Binding_ca16155824bb4cfea36ccddf1be3e1fd = new global::System.Windows.Data.Binding();
            Binding_ca16155824bb4cfea36ccddf1be3e1fd.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"HorizontalContentAlignment");
            var RelativeSource_4d4b6cabfcb74e1798a3485ac330f9e7 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_4d4b6cabfcb74e1798a3485ac330f9e7.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_ca16155824bb4cfea36ccddf1be3e1fd.RelativeSource = RelativeSource_4d4b6cabfcb74e1798a3485ac330f9e7;


            Binding_ca16155824bb4cfea36ccddf1be3e1fd.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;

            var Binding_55745993266e4648a57ee766c888aa6c = new global::System.Windows.Data.Binding();
            Binding_55745993266e4648a57ee766c888aa6c.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"VerticalContentAlignment");
            var RelativeSource_4c1e6090e877464e9ed478f3a93dd538 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_4c1e6090e877464e9ed478f3a93dd538.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_55745993266e4648a57ee766c888aa6c.RelativeSource = RelativeSource_4c1e6090e877464e9ed478f3a93dd538;


            Binding_55745993266e4648a57ee766c888aa6c.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;


            Border_1a86cfa0349a42d29b702bf1258286f7.Child = ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9;

            var Binding_f93a805babd140709d76fdd644e1ef09 = new global::System.Windows.Data.Binding();
            Binding_f93a805babd140709d76fdd644e1ef09.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_7d2de04a45484624b14aaaf0e919598d = new global::System.Windows.Data.RelativeSource();
            RelativeSource_7d2de04a45484624b14aaaf0e919598d.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_f93a805babd140709d76fdd644e1ef09.RelativeSource = RelativeSource_7d2de04a45484624b14aaaf0e919598d;


            Binding_f93a805babd140709d76fdd644e1ef09.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;


            Border_b29d98a31ebf4d149d9d17a45a56fe8d.Child = Border_1a86cfa0349a42d29b702bf1258286f7;

            var Binding_d9a26b9724f2467a85cb0e454c6bcc8c = new global::System.Windows.Data.Binding();
            Binding_d9a26b9724f2467a85cb0e454c6bcc8c.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_fe1b76a408534827a6f4e30fcb67dc31 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_fe1b76a408534827a6f4e30fcb67dc31.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_d9a26b9724f2467a85cb0e454c6bcc8c.RelativeSource = RelativeSource_fe1b76a408534827a6f4e30fcb67dc31;


            Binding_d9a26b9724f2467a85cb0e454c6bcc8c.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;

            var Binding_5fafa742b2eb4e38ab52d76d20f86b85 = new global::System.Windows.Data.Binding();
            Binding_5fafa742b2eb4e38ab52d76d20f86b85.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
            var RelativeSource_2e757e7ddc91493fa3e4f6c74b5fe9ac = new global::System.Windows.Data.RelativeSource();
            RelativeSource_2e757e7ddc91493fa3e4f6c74b5fe9ac.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_5fafa742b2eb4e38ab52d76d20f86b85.RelativeSource = RelativeSource_2e757e7ddc91493fa3e4f6c74b5fe9ac;


            Binding_5fafa742b2eb4e38ab52d76d20f86b85.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;

            var Binding_6db6e89a3d8f4adb9a524b38e19b0dd1 = new global::System.Windows.Data.Binding();
            Binding_6db6e89a3d8f4adb9a524b38e19b0dd1.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
            var RelativeSource_f24f2561b0ff4fe49751c88a42ab106b = new global::System.Windows.Data.RelativeSource();
            RelativeSource_f24f2561b0ff4fe49751c88a42ab106b.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_6db6e89a3d8f4adb9a524b38e19b0dd1.RelativeSource = RelativeSource_f24f2561b0ff4fe49751c88a42ab106b;


            Binding_6db6e89a3d8f4adb9a524b38e19b0dd1.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;



            ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9.SetBinding(global::System.Windows.Controls.ContentPresenter.ContentTemplateProperty, Binding_91acea06e00c49548bc2f975f8c708a3);
            ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9.SetBinding(global::System.Windows.Controls.ContentPresenter.ContentProperty, Binding_c6b24d6bd0734b9e9420759f5f45d4a1);
            ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9.SetBinding(global::System.Windows.FrameworkElement.MarginProperty, Binding_8c979377f5604169a95efa05a955e19b);
            ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9.SetBinding(global::System.Windows.FrameworkElement.HorizontalAlignmentProperty, Binding_ca16155824bb4cfea36ccddf1be3e1fd);
            ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9.SetBinding(global::System.Windows.FrameworkElement.VerticalAlignmentProperty, Binding_55745993266e4648a57ee766c888aa6c);
            Border_1a86cfa0349a42d29b702bf1258286f7.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_f93a805babd140709d76fdd644e1ef09);
            Border_b29d98a31ebf4d149d9d17a45a56fe8d.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_d9a26b9724f2467a85cb0e454c6bcc8c);
            Border_b29d98a31ebf4d149d9d17a45a56fe8d.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_5fafa742b2eb4e38ab52d76d20f86b85);
            Border_b29d98a31ebf4d149d9d17a45a56fe8d.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_6db6e89a3d8f4adb9a524b38e19b0dd1);

            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_cca6ec7be7b641698ba5fff415012e37,
                new global::System.Windows.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_8771c83003c344a28b6f5dd4717a6597,
                    setVisualStateProperty_8771c83003c344a28b6f5dd4717a6597,
                    setLocalVisualStateProperty_8771c83003c344a28b6f5dd4717a6597,
                    getVisualStateProperty_8771c83003c344a28b6f5dd4717a6597));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_cca6ec7be7b641698ba5fff415012e37, Border_1a86cfa0349a42d29b702bf1258286f7);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_20a0b97d4370479ba8230b28c255a095,
                new global::System.Windows.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_741833e54c914a14b2f2f7ccc151a893,
                    setVisualStateProperty_741833e54c914a14b2f2f7ccc151a893,
                    setLocalVisualStateProperty_741833e54c914a14b2f2f7ccc151a893,
                    getVisualStateProperty_741833e54c914a14b2f2f7ccc151a893));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_20a0b97d4370479ba8230b28c255a095, Border_1a86cfa0349a42d29b702bf1258286f7);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_82d43e6e777f47afbad4984d60e5ca64,
                new global::System.Windows.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_65f99b6e906342699a1f750fbcebc34e,
                    setVisualStateProperty_65f99b6e906342699a1f750fbcebc34e,
                    setLocalVisualStateProperty_65f99b6e906342699a1f750fbcebc34e,
                    getVisualStateProperty_65f99b6e906342699a1f750fbcebc34e));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_82d43e6e777f47afbad4984d60e5ca64, Border_1a86cfa0349a42d29b702bf1258286f7);

            templateInstance_bc29344b1ad94da8940026d4d5cd38a9.TemplateContent = Border_b29d98a31ebf4d149d9d17a45a56fe8d;
            return templateInstance_bc29344b1ad94da8940026d4d5cd38a9;
        }

    }
}
#else
namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultButtonStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_27761b772dd042038ccd0a8411db434d = new global::Windows.UI.Xaml.Style();
                Style_27761b772dd042038ccd0a8411db434d.TargetType = typeof(global::Windows.UI.Xaml.Controls.Button);
                var Setter_293523e2a6504050b941ad632a73ae5c = new global::Windows.UI.Xaml.Setter();
                Setter_293523e2a6504050b941ad632a73ae5c.Property = global::Windows.UI.Xaml.Controls.Button.BackgroundProperty;
                Setter_293523e2a6504050b941ad632a73ae5c.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFE2E2E2");

                var Setter_406bdcfd4a14410d8fe9c2d742ce11a8 = new global::Windows.UI.Xaml.Setter();
                Setter_406bdcfd4a14410d8fe9c2d742ce11a8.Property = global::Windows.UI.Xaml.Controls.Button.ForegroundProperty;
                Setter_406bdcfd4a14410d8fe9c2d742ce11a8.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Black");

                var Setter_7f001f6302364f79bebb4e52102663f2 = new global::Windows.UI.Xaml.Setter();
                Setter_7f001f6302364f79bebb4e52102663f2.Property = global::Windows.UI.Xaml.Controls.Button.BorderThicknessProperty;
                Setter_7f001f6302364f79bebb4e52102663f2.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0");

                var Setter_4b74961b18e04e77a89e89845ea54b04 = new global::Windows.UI.Xaml.Setter();
                Setter_4b74961b18e04e77a89e89845ea54b04.Property = global::Windows.UI.Xaml.Controls.Button.PaddingProperty;
                Setter_4b74961b18e04e77a89e89845ea54b04.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"3");

                var Setter_873791a433394d8eab48fcc32e39ec13 = new global::Windows.UI.Xaml.Setter();
                Setter_873791a433394d8eab48fcc32e39ec13.Property = global::Windows.UI.Xaml.Controls.Button.CursorProperty;
                Setter_873791a433394d8eab48fcc32e39ec13.Value = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Hand");

                var Setter_b71ea50f9afd4027811f6263069a36af = new global::Windows.UI.Xaml.Setter();
                Setter_b71ea50f9afd4027811f6263069a36af.Property = global::Windows.UI.Xaml.Controls.Button.HorizontalContentAlignmentProperty;
                Setter_b71ea50f9afd4027811f6263069a36af.Value = global::Windows.UI.Xaml.HorizontalAlignment.Center;

                var Setter_d7c89c827d7a4946bfa675a647c5ce08 = new global::Windows.UI.Xaml.Setter();
                Setter_d7c89c827d7a4946bfa675a647c5ce08.Property = global::Windows.UI.Xaml.Controls.Button.VerticalContentAlignmentProperty;
                Setter_d7c89c827d7a4946bfa675a647c5ce08.Value = global::Windows.UI.Xaml.VerticalAlignment.Center;

                var Setter_749ae2eae0f5448eb6fa70b482754921 = new global::Windows.UI.Xaml.Setter();
                Setter_749ae2eae0f5448eb6fa70b482754921.Property = global::Windows.UI.Xaml.Controls.Button.TemplateProperty;
                var ControlTemplate_7304dcc3a19f4a88a9519ead8cebaa52 = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_7304dcc3a19f4a88a9519ead8cebaa52.TargetType = typeof(global::Windows.UI.Xaml.Controls.Button);
                ControlTemplate_7304dcc3a19f4a88a9519ead8cebaa52.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_7304dcc3a19f4a88a9519ead8cebaa52);

                Setter_749ae2eae0f5448eb6fa70b482754921.Value = ControlTemplate_7304dcc3a19f4a88a9519ead8cebaa52;


                Style_27761b772dd042038ccd0a8411db434d.Setters.Add(Setter_293523e2a6504050b941ad632a73ae5c);
                Style_27761b772dd042038ccd0a8411db434d.Setters.Add(Setter_406bdcfd4a14410d8fe9c2d742ce11a8);
                Style_27761b772dd042038ccd0a8411db434d.Setters.Add(Setter_7f001f6302364f79bebb4e52102663f2);
                Style_27761b772dd042038ccd0a8411db434d.Setters.Add(Setter_4b74961b18e04e77a89e89845ea54b04);
                Style_27761b772dd042038ccd0a8411db434d.Setters.Add(Setter_873791a433394d8eab48fcc32e39ec13);
                Style_27761b772dd042038ccd0a8411db434d.Setters.Add(Setter_b71ea50f9afd4027811f6263069a36af);
                Style_27761b772dd042038ccd0a8411db434d.Setters.Add(Setter_d7c89c827d7a4946bfa675a647c5ce08);
                Style_27761b772dd042038ccd0a8411db434d.Setters.Add(Setter_749ae2eae0f5448eb6fa70b482754921);


                DefaultStyle = Style_27761b772dd042038ccd0a8411db434d;
            }

            return DefaultStyle;
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_8771c83003c344a28b6f5dd4717a6597(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_8771c83003c344a28b6f5dd4717a6597(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_8771c83003c344a28b6f5dd4717a6597(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_8771c83003c344a28b6f5dd4717a6597(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_741833e54c914a14b2f2f7ccc151a893(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_741833e54c914a14b2f2f7ccc151a893(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_741833e54c914a14b2f2f7ccc151a893(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_741833e54c914a14b2f2f7ccc151a893(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_65f99b6e906342699a1f750fbcebc34e(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_65f99b6e906342699a1f750fbcebc34e(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_65f99b6e906342699a1f750fbcebc34e(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_65f99b6e906342699a1f750fbcebc34e(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }

        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_7304dcc3a19f4a88a9519ead8cebaa52(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_bc29344b1ad94da8940026d4d5cd38a9 = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_bc29344b1ad94da8940026d4d5cd38a9.TemplateOwner = templateOwner;
            var Border_b29d98a31ebf4d149d9d17a45a56fe8d = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("OuterBorder", Border_b29d98a31ebf4d149d9d17a45a56fe8d);
            Border_b29d98a31ebf4d149d9d17a45a56fe8d.Name = "OuterBorder";
            var VisualStateGroup_7733a25694e14f5e9c0d450df3e38327 = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_7733a25694e14f5e9c0d450df3e38327);
            VisualStateGroup_7733a25694e14f5e9c0d450df3e38327.Name = "CommonStates";
            var VisualState_4906afe15e68428cad9229c32bcb0ce8 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Normal", VisualState_4906afe15e68428cad9229c32bcb0ce8);
            VisualState_4906afe15e68428cad9229c32bcb0ce8.Name = "Normal";

            var VisualState_492656887ed1435791d3b36ac421d395 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("PointerOver", VisualState_492656887ed1435791d3b36ac421d395);
            VisualState_492656887ed1435791d3b36ac421d395.Name = "PointerOver";
            var Storyboard_1278967bd95b491ab90ecd94b23e532f = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_cca6ec7be7b641698ba5fff415012e37 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_cca6ec7be7b641698ba5fff415012e37, @"InnerBorder");
            var DiscreteObjectKeyFrame_13d6bfb0533e449c9b389732578aa9f8 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_13d6bfb0533e449c9b389732578aa9f8.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_13d6bfb0533e449c9b389732578aa9f8.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#11000000");

            ObjectAnimationUsingKeyFrames_cca6ec7be7b641698ba5fff415012e37.KeyFrames.Add(DiscreteObjectKeyFrame_13d6bfb0533e449c9b389732578aa9f8);


            Storyboard_1278967bd95b491ab90ecd94b23e532f.Children.Add(ObjectAnimationUsingKeyFrames_cca6ec7be7b641698ba5fff415012e37);


            VisualState_492656887ed1435791d3b36ac421d395.Storyboard = Storyboard_1278967bd95b491ab90ecd94b23e532f;


            var VisualState_c34bbc7099824fc09923b1264383dae9 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Pressed", VisualState_c34bbc7099824fc09923b1264383dae9);
            VisualState_c34bbc7099824fc09923b1264383dae9.Name = "Pressed";
            var Storyboard_2f2f64e746414388812f3a5d41886375 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_20a0b97d4370479ba8230b28c255a095 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_20a0b97d4370479ba8230b28c255a095, @"InnerBorder");
            var DiscreteObjectKeyFrame_6d0dcd62810243138d8bcae92a5e24ab = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_6d0dcd62810243138d8bcae92a5e24ab.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_6d0dcd62810243138d8bcae92a5e24ab.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#22000000");

            ObjectAnimationUsingKeyFrames_20a0b97d4370479ba8230b28c255a095.KeyFrames.Add(DiscreteObjectKeyFrame_6d0dcd62810243138d8bcae92a5e24ab);


            Storyboard_2f2f64e746414388812f3a5d41886375.Children.Add(ObjectAnimationUsingKeyFrames_20a0b97d4370479ba8230b28c255a095);


            VisualState_c34bbc7099824fc09923b1264383dae9.Storyboard = Storyboard_2f2f64e746414388812f3a5d41886375;


            var VisualState_7fd412a6e58b41cf891531966de1d0ad = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_7fd412a6e58b41cf891531966de1d0ad);
            VisualState_7fd412a6e58b41cf891531966de1d0ad.Name = "Disabled";
            var Storyboard_b063f7ff0d614b0a9cfa97b2c0c256df = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_82d43e6e777f47afbad4984d60e5ca64 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_82d43e6e777f47afbad4984d60e5ca64, @"InnerBorder");
            var DiscreteObjectKeyFrame_5077045f9093441994378fdd9561d003 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_5077045f9093441994378fdd9561d003.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_5077045f9093441994378fdd9561d003.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#33FFFFFF");

            ObjectAnimationUsingKeyFrames_82d43e6e777f47afbad4984d60e5ca64.KeyFrames.Add(DiscreteObjectKeyFrame_5077045f9093441994378fdd9561d003);


            Storyboard_b063f7ff0d614b0a9cfa97b2c0c256df.Children.Add(ObjectAnimationUsingKeyFrames_82d43e6e777f47afbad4984d60e5ca64);


            VisualState_7fd412a6e58b41cf891531966de1d0ad.Storyboard = Storyboard_b063f7ff0d614b0a9cfa97b2c0c256df;


            VisualStateGroup_7733a25694e14f5e9c0d450df3e38327.States.Add(VisualState_4906afe15e68428cad9229c32bcb0ce8);
            VisualStateGroup_7733a25694e14f5e9c0d450df3e38327.States.Add(VisualState_492656887ed1435791d3b36ac421d395);
            VisualStateGroup_7733a25694e14f5e9c0d450df3e38327.States.Add(VisualState_c34bbc7099824fc09923b1264383dae9);
            VisualStateGroup_7733a25694e14f5e9c0d450df3e38327.States.Add(VisualState_7fd412a6e58b41cf891531966de1d0ad);


            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_7733a25694e14f5e9c0d450df3e38327);

            var Border_1a86cfa0349a42d29b702bf1258286f7 = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("InnerBorder", Border_1a86cfa0349a42d29b702bf1258286f7);
            Border_1a86cfa0349a42d29b702bf1258286f7.Name = "InnerBorder";
            var ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9 = new global::Windows.UI.Xaml.Controls.ContentPresenter();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ContentPresenter", ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9);
            ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9.Name = "ContentPresenter";
            var Binding_91acea06e00c49548bc2f975f8c708a3 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_91acea06e00c49548bc2f975f8c708a3.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"ContentTemplate");
            var RelativeSource_dd8eb7faf07344eea6aec3930ceb66b1 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_dd8eb7faf07344eea6aec3930ceb66b1.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_91acea06e00c49548bc2f975f8c708a3.RelativeSource = RelativeSource_dd8eb7faf07344eea6aec3930ceb66b1;


            Binding_91acea06e00c49548bc2f975f8c708a3.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;

            var Binding_c6b24d6bd0734b9e9420759f5f45d4a1 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_c6b24d6bd0734b9e9420759f5f45d4a1.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Content");
            var RelativeSource_c31f3b94b8be4b09a12eb08eca31d6a9 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_c31f3b94b8be4b09a12eb08eca31d6a9.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_c6b24d6bd0734b9e9420759f5f45d4a1.RelativeSource = RelativeSource_c31f3b94b8be4b09a12eb08eca31d6a9;


            Binding_c6b24d6bd0734b9e9420759f5f45d4a1.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;

            var Binding_8c979377f5604169a95efa05a955e19b = new global::Windows.UI.Xaml.Data.Binding();
            Binding_8c979377f5604169a95efa05a955e19b.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Padding");
            var RelativeSource_af85d08ded73477390420376e1e77359 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_af85d08ded73477390420376e1e77359.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_8c979377f5604169a95efa05a955e19b.RelativeSource = RelativeSource_af85d08ded73477390420376e1e77359;


            Binding_8c979377f5604169a95efa05a955e19b.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;

            var Binding_ca16155824bb4cfea36ccddf1be3e1fd = new global::Windows.UI.Xaml.Data.Binding();
            Binding_ca16155824bb4cfea36ccddf1be3e1fd.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"HorizontalContentAlignment");
            var RelativeSource_4d4b6cabfcb74e1798a3485ac330f9e7 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_4d4b6cabfcb74e1798a3485ac330f9e7.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_ca16155824bb4cfea36ccddf1be3e1fd.RelativeSource = RelativeSource_4d4b6cabfcb74e1798a3485ac330f9e7;


            Binding_ca16155824bb4cfea36ccddf1be3e1fd.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;

            var Binding_55745993266e4648a57ee766c888aa6c = new global::Windows.UI.Xaml.Data.Binding();
            Binding_55745993266e4648a57ee766c888aa6c.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"VerticalContentAlignment");
            var RelativeSource_4c1e6090e877464e9ed478f3a93dd538 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_4c1e6090e877464e9ed478f3a93dd538.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_55745993266e4648a57ee766c888aa6c.RelativeSource = RelativeSource_4c1e6090e877464e9ed478f3a93dd538;


            Binding_55745993266e4648a57ee766c888aa6c.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;


            Border_1a86cfa0349a42d29b702bf1258286f7.Child = ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9;

            var Binding_f93a805babd140709d76fdd644e1ef09 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_f93a805babd140709d76fdd644e1ef09.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_7d2de04a45484624b14aaaf0e919598d = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_7d2de04a45484624b14aaaf0e919598d.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_f93a805babd140709d76fdd644e1ef09.RelativeSource = RelativeSource_7d2de04a45484624b14aaaf0e919598d;


            Binding_f93a805babd140709d76fdd644e1ef09.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;


            Border_b29d98a31ebf4d149d9d17a45a56fe8d.Child = Border_1a86cfa0349a42d29b702bf1258286f7;

            var Binding_d9a26b9724f2467a85cb0e454c6bcc8c = new global::Windows.UI.Xaml.Data.Binding();
            Binding_d9a26b9724f2467a85cb0e454c6bcc8c.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_fe1b76a408534827a6f4e30fcb67dc31 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_fe1b76a408534827a6f4e30fcb67dc31.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_d9a26b9724f2467a85cb0e454c6bcc8c.RelativeSource = RelativeSource_fe1b76a408534827a6f4e30fcb67dc31;


            Binding_d9a26b9724f2467a85cb0e454c6bcc8c.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;

            var Binding_5fafa742b2eb4e38ab52d76d20f86b85 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_5fafa742b2eb4e38ab52d76d20f86b85.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_2e757e7ddc91493fa3e4f6c74b5fe9ac = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_2e757e7ddc91493fa3e4f6c74b5fe9ac.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_5fafa742b2eb4e38ab52d76d20f86b85.RelativeSource = RelativeSource_2e757e7ddc91493fa3e4f6c74b5fe9ac;


            Binding_5fafa742b2eb4e38ab52d76d20f86b85.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;

            var Binding_6db6e89a3d8f4adb9a524b38e19b0dd1 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_6db6e89a3d8f4adb9a524b38e19b0dd1.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_f24f2561b0ff4fe49751c88a42ab106b = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_f24f2561b0ff4fe49751c88a42ab106b.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_6db6e89a3d8f4adb9a524b38e19b0dd1.RelativeSource = RelativeSource_f24f2561b0ff4fe49751c88a42ab106b;


            Binding_6db6e89a3d8f4adb9a524b38e19b0dd1.TemplateOwner = templateInstance_bc29344b1ad94da8940026d4d5cd38a9;



            ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9.SetBinding(global::Windows.UI.Xaml.Controls.ContentPresenter.ContentTemplateProperty, Binding_91acea06e00c49548bc2f975f8c708a3);
            ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9.SetBinding(global::Windows.UI.Xaml.Controls.ContentPresenter.ContentProperty, Binding_c6b24d6bd0734b9e9420759f5f45d4a1);
            ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9.SetBinding(global::Windows.UI.Xaml.FrameworkElement.MarginProperty, Binding_8c979377f5604169a95efa05a955e19b);
            ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9.SetBinding(global::Windows.UI.Xaml.FrameworkElement.HorizontalAlignmentProperty, Binding_ca16155824bb4cfea36ccddf1be3e1fd);
            ContentPresenter_cd4db4fcddaa49838af0a6b180a903d9.SetBinding(global::Windows.UI.Xaml.FrameworkElement.VerticalAlignmentProperty, Binding_55745993266e4648a57ee766c888aa6c);
            Border_1a86cfa0349a42d29b702bf1258286f7.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_f93a805babd140709d76fdd644e1ef09);
            Border_b29d98a31ebf4d149d9d17a45a56fe8d.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_d9a26b9724f2467a85cb0e454c6bcc8c);
            Border_b29d98a31ebf4d149d9d17a45a56fe8d.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_5fafa742b2eb4e38ab52d76d20f86b85);
            Border_b29d98a31ebf4d149d9d17a45a56fe8d.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_6db6e89a3d8f4adb9a524b38e19b0dd1);

            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_cca6ec7be7b641698ba5fff415012e37,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_8771c83003c344a28b6f5dd4717a6597,
                    setVisualStateProperty_8771c83003c344a28b6f5dd4717a6597,
                    setLocalVisualStateProperty_8771c83003c344a28b6f5dd4717a6597,
                    getVisualStateProperty_8771c83003c344a28b6f5dd4717a6597));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_cca6ec7be7b641698ba5fff415012e37, Border_1a86cfa0349a42d29b702bf1258286f7);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_20a0b97d4370479ba8230b28c255a095,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_741833e54c914a14b2f2f7ccc151a893,
                    setVisualStateProperty_741833e54c914a14b2f2f7ccc151a893,
                    setLocalVisualStateProperty_741833e54c914a14b2f2f7ccc151a893,
                    getVisualStateProperty_741833e54c914a14b2f2f7ccc151a893));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_20a0b97d4370479ba8230b28c255a095, Border_1a86cfa0349a42d29b702bf1258286f7);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_82d43e6e777f47afbad4984d60e5ca64,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_65f99b6e906342699a1f750fbcebc34e,
                    setVisualStateProperty_65f99b6e906342699a1f750fbcebc34e,
                    setLocalVisualStateProperty_65f99b6e906342699a1f750fbcebc34e,
                    getVisualStateProperty_65f99b6e906342699a1f750fbcebc34e));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_82d43e6e777f47afbad4984d60e5ca64, Border_1a86cfa0349a42d29b702bf1258286f7);

            templateInstance_bc29344b1ad94da8940026d4d5cd38a9.TemplateContent = Border_b29d98a31ebf4d149d9d17a45a56fe8d;
            return templateInstance_bc29344b1ad94da8940026d4d5cd38a9;
        }

    }
}
#endif