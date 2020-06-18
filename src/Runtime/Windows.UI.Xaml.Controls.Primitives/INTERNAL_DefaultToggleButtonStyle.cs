
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
    internal class INTERNAL_DefaultToggleButtonStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_1766cd45f4114caab929a4040c7e5b66 = new global::System.Windows.Style();
                Style_1766cd45f4114caab929a4040c7e5b66.TargetType = typeof(global::System.Windows.Controls.Primitives.ToggleButton);
                var Setter_1aecc6235bd2473dae20c12f0da7fe02 = new global::System.Windows.Setter();
                Setter_1aecc6235bd2473dae20c12f0da7fe02.Property = global::System.Windows.Controls.Primitives.ToggleButton.BackgroundProperty;
                Setter_1aecc6235bd2473dae20c12f0da7fe02.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFE2E2E2");

                var Setter_a405e10599f3416d912ccd03ba0ed2a3 = new global::System.Windows.Setter();
                Setter_a405e10599f3416d912ccd03ba0ed2a3.Property = global::System.Windows.Controls.Primitives.ToggleButton.ForegroundProperty;
                Setter_a405e10599f3416d912ccd03ba0ed2a3.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Black");

                var Setter_e58113ee28a14fc8a50e32c4690ebd06 = new global::System.Windows.Setter();
                Setter_e58113ee28a14fc8a50e32c4690ebd06.Property = global::System.Windows.Controls.Primitives.ToggleButton.BorderThicknessProperty;
                Setter_e58113ee28a14fc8a50e32c4690ebd06.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0");

                var Setter_c2841f6a39724f308ec0e15a679a5229 = new global::System.Windows.Setter();
                Setter_c2841f6a39724f308ec0e15a679a5229.Property = global::System.Windows.Controls.Primitives.ToggleButton.PaddingProperty;
                Setter_c2841f6a39724f308ec0e15a679a5229.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"12,4,12,4");

                var Setter_139988636fd54cdaac6071028a3f56f2 = new global::System.Windows.Setter();
                Setter_139988636fd54cdaac6071028a3f56f2.Property = global::System.Windows.Controls.Primitives.ToggleButton.CursorProperty;
                Setter_139988636fd54cdaac6071028a3f56f2.Value = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Hand");

                var Setter_d240066348aa45d29abe85ddfdfe8953 = new global::System.Windows.Setter();
                Setter_d240066348aa45d29abe85ddfdfe8953.Property = global::System.Windows.Controls.Primitives.ToggleButton.HorizontalContentAlignmentProperty;
                Setter_d240066348aa45d29abe85ddfdfe8953.Value = global::System.Windows.HorizontalAlignment.Center;

                var Setter_8fa78a0109c848f89228783e3bfbda8e = new global::System.Windows.Setter();
                Setter_8fa78a0109c848f89228783e3bfbda8e.Property = global::System.Windows.Controls.Primitives.ToggleButton.VerticalContentAlignmentProperty;
                Setter_8fa78a0109c848f89228783e3bfbda8e.Value = global::System.Windows.VerticalAlignment.Center;

                var Setter_491c520659714419b27d0f79cf8b1d32 = new global::System.Windows.Setter();
                Setter_491c520659714419b27d0f79cf8b1d32.Property = global::System.Windows.Controls.Primitives.ToggleButton.TemplateProperty;
                var ControlTemplate_4824bc99f1f14e57ac1c085c3a1c1321 = new global::System.Windows.Controls.ControlTemplate();
                ControlTemplate_4824bc99f1f14e57ac1c085c3a1c1321.TargetType = typeof(global::System.Windows.Controls.Primitives.ToggleButton);
                ControlTemplate_4824bc99f1f14e57ac1c085c3a1c1321.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_4824bc99f1f14e57ac1c085c3a1c1321);

                Setter_491c520659714419b27d0f79cf8b1d32.Value = ControlTemplate_4824bc99f1f14e57ac1c085c3a1c1321;


                Style_1766cd45f4114caab929a4040c7e5b66.Setters.Add(Setter_1aecc6235bd2473dae20c12f0da7fe02);
                Style_1766cd45f4114caab929a4040c7e5b66.Setters.Add(Setter_a405e10599f3416d912ccd03ba0ed2a3);
                Style_1766cd45f4114caab929a4040c7e5b66.Setters.Add(Setter_e58113ee28a14fc8a50e32c4690ebd06);
                Style_1766cd45f4114caab929a4040c7e5b66.Setters.Add(Setter_c2841f6a39724f308ec0e15a679a5229);
                Style_1766cd45f4114caab929a4040c7e5b66.Setters.Add(Setter_139988636fd54cdaac6071028a3f56f2);
                Style_1766cd45f4114caab929a4040c7e5b66.Setters.Add(Setter_d240066348aa45d29abe85ddfdfe8953);
                Style_1766cd45f4114caab929a4040c7e5b66.Setters.Add(Setter_8fa78a0109c848f89228783e3bfbda8e);
                Style_1766cd45f4114caab929a4040c7e5b66.Setters.Add(Setter_491c520659714419b27d0f79cf8b1d32);


                DefaultStyle = Style_1766cd45f4114caab929a4040c7e5b66;
            }

            return DefaultStyle;
        }

        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_4053b15dd4ee4d1397186015aef32799(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_4053b15dd4ee4d1397186015aef32799(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_4053b15dd4ee4d1397186015aef32799(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_4053b15dd4ee4d1397186015aef32799(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_2185526c11084008b4d96c9abeb9645a(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_2185526c11084008b4d96c9abeb9645a(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_2185526c11084008b4d96c9abeb9645a(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_2185526c11084008b4d96c9abeb9645a(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_6cf63a228a7648e3b09d349e50f10891(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_6cf63a228a7648e3b09d349e50f10891(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_6cf63a228a7648e3b09d349e50f10891(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_6cf63a228a7648e3b09d349e50f10891(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_f1f1a394431e4e8391168272c7730f8a(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_f1f1a394431e4e8391168272c7730f8a(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_f1f1a394431e4e8391168272c7730f8a(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_f1f1a394431e4e8391168272c7730f8a(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_5ff70c31b6ca47ec92195e328b383a7a(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_5ff70c31b6ca47ec92195e328b383a7a(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_5ff70c31b6ca47ec92195e328b383a7a(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_5ff70c31b6ca47ec92195e328b383a7a(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_e6c6dc7ea17d4a1fb8de1088ab5ba7fb(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_e6c6dc7ea17d4a1fb8de1088ab5ba7fb(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_e6c6dc7ea17d4a1fb8de1088ab5ba7fb(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_e6c6dc7ea17d4a1fb8de1088ab5ba7fb(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_0cd646131198415198ed08a08586b3c8(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }

        public static void setVisualStateProperty_0cd646131198415198ed08a08586b3c8(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_0cd646131198415198ed08a08586b3c8(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_0cd646131198415198ed08a08586b3c8(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_0d8762d01568423da62f5c866eeb0199(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_0d8762d01568423da62f5c866eeb0199(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_0d8762d01568423da62f5c866eeb0199(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_0d8762d01568423da62f5c866eeb0199(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_e4b4395223e3450fb04826f9f30828ab(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_e4b4395223e3450fb04826f9f30828ab(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_e4b4395223e3450fb04826f9f30828ab(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_e4b4395223e3450fb04826f9f30828ab(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_750f3d33ac674ccab0bd2eac596d0d3e(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_750f3d33ac674ccab0bd2eac596d0d3e(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_750f3d33ac674ccab0bd2eac596d0d3e(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_750f3d33ac674ccab0bd2eac596d0d3e(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_4824bc99f1f14e57ac1c085c3a1c1321(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_523b354795e748a6a0c81ac8bd051b6a = new global::System.Windows.TemplateInstance();
            templateInstance_523b354795e748a6a0c81ac8bd051b6a.TemplateOwner = templateOwner;
            var Border_a87e17151acc48a89a823a0f683e1fed = new global::System.Windows.Controls.Border();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("OuterBorder", Border_a87e17151acc48a89a823a0f683e1fed);
            Border_a87e17151acc48a89a823a0f683e1fed.Name = "OuterBorder";
            var VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f = new global::System.Windows.VisualStateGroup();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.Name = "CommonStates";
            var VisualState_48b9e3edd8b242539c8432e626559e9a = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Normal", VisualState_48b9e3edd8b242539c8432e626559e9a);
            VisualState_48b9e3edd8b242539c8432e626559e9a.Name = "Normal";

            var VisualState_684653ffba544a559aa056c8b7c79846 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("MouseOver", VisualState_684653ffba544a559aa056c8b7c79846);
            VisualState_684653ffba544a559aa056c8b7c79846.Name = "MouseOver";
            var Storyboard_dee5e87903934d7cb6a776c10f9050d3 = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_1a7954717d81491899f6850738c30338 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_1a7954717d81491899f6850738c30338, @"InnerBorder");
            var DiscreteObjectKeyFrame_0c24c6d734e842da999fd6797b4343bb = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_0c24c6d734e842da999fd6797b4343bb.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_0c24c6d734e842da999fd6797b4343bb.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#11000000");

            ObjectAnimationUsingKeyFrames_1a7954717d81491899f6850738c30338.KeyFrames.Add(DiscreteObjectKeyFrame_0c24c6d734e842da999fd6797b4343bb);


            Storyboard_dee5e87903934d7cb6a776c10f9050d3.Children.Add(ObjectAnimationUsingKeyFrames_1a7954717d81491899f6850738c30338);


            VisualState_684653ffba544a559aa056c8b7c79846.Storyboard = Storyboard_dee5e87903934d7cb6a776c10f9050d3;


            var VisualState_99b521ca378f49729b15cc4157a429d1 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Pressed", VisualState_99b521ca378f49729b15cc4157a429d1);
            VisualState_99b521ca378f49729b15cc4157a429d1.Name = "Pressed";
            var Storyboard_089bb4f88cfb4eb6914b2eec480bb937 = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_7aac0c45ca9846799f661d1d1e26d5a9 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_7aac0c45ca9846799f661d1d1e26d5a9, @"InnerBorder");
            var DiscreteObjectKeyFrame_33a0ea8820d94d278a74c46c8303f99b = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_33a0ea8820d94d278a74c46c8303f99b.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_33a0ea8820d94d278a74c46c8303f99b.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#44000000");

            ObjectAnimationUsingKeyFrames_7aac0c45ca9846799f661d1d1e26d5a9.KeyFrames.Add(DiscreteObjectKeyFrame_33a0ea8820d94d278a74c46c8303f99b);


            Storyboard_089bb4f88cfb4eb6914b2eec480bb937.Children.Add(ObjectAnimationUsingKeyFrames_7aac0c45ca9846799f661d1d1e26d5a9);


            VisualState_99b521ca378f49729b15cc4157a429d1.Storyboard = Storyboard_089bb4f88cfb4eb6914b2eec480bb937;


            var VisualState_1daa3b7feead4eb0a119e0fb3b51593b = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_1daa3b7feead4eb0a119e0fb3b51593b);
            VisualState_1daa3b7feead4eb0a119e0fb3b51593b.Name = "Disabled";
            var Storyboard_0be07567d87f4bfa96c42a05e7aca71b = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_7ffbc1f738364c4da17d691b1174720e = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_7ffbc1f738364c4da17d691b1174720e, @"InnerBorder");
            var DiscreteObjectKeyFrame_29621ab2fc2847a787153f8531ab7a38 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_29621ab2fc2847a787153f8531ab7a38.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_29621ab2fc2847a787153f8531ab7a38.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Transparent");

            ObjectAnimationUsingKeyFrames_7ffbc1f738364c4da17d691b1174720e.KeyFrames.Add(DiscreteObjectKeyFrame_29621ab2fc2847a787153f8531ab7a38);


            Storyboard_0be07567d87f4bfa96c42a05e7aca71b.Children.Add(ObjectAnimationUsingKeyFrames_7ffbc1f738364c4da17d691b1174720e);


            VisualState_1daa3b7feead4eb0a119e0fb3b51593b.Storyboard = Storyboard_0be07567d87f4bfa96c42a05e7aca71b;


            var VisualState_09be70cea64c4a478df2dec5a2580ff3 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Checked", VisualState_09be70cea64c4a478df2dec5a2580ff3);
            VisualState_09be70cea64c4a478df2dec5a2580ff3.Name = "Checked";
            var Storyboard_6c454763aefc493b97eaf9466488c993 = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_a345c63cb00f47b3930be6088ec8f206 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_a345c63cb00f47b3930be6088ec8f206, @"InnerBorder");
            var DiscreteObjectKeyFrame_5ce95b24c17343229602aea4bb073cd2 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_5ce95b24c17343229602aea4bb073cd2.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_5ce95b24c17343229602aea4bb073cd2.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#33000000");

            ObjectAnimationUsingKeyFrames_a345c63cb00f47b3930be6088ec8f206.KeyFrames.Add(DiscreteObjectKeyFrame_5ce95b24c17343229602aea4bb073cd2);


            Storyboard_6c454763aefc493b97eaf9466488c993.Children.Add(ObjectAnimationUsingKeyFrames_a345c63cb00f47b3930be6088ec8f206);


            VisualState_09be70cea64c4a478df2dec5a2580ff3.Storyboard = Storyboard_6c454763aefc493b97eaf9466488c993;


            var VisualState_79f02f22b7854653892bb18b8aeb665c = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("CheckedPointerOver", VisualState_79f02f22b7854653892bb18b8aeb665c);
            VisualState_79f02f22b7854653892bb18b8aeb665c.Name = "CheckedPointerOver";
            var Storyboard_9fea9234e26d4d3faf2bf7d71d4b8202 = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_7d6bf9a54ea242fbacdbd3a58e8e6aa6 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_7d6bf9a54ea242fbacdbd3a58e8e6aa6, @"InnerBorder");
            var DiscreteObjectKeyFrame_f7e33f638a7d4189a1e315b1e1017d4f = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_f7e33f638a7d4189a1e315b1e1017d4f.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_f7e33f638a7d4189a1e315b1e1017d4f.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#22000000");

            ObjectAnimationUsingKeyFrames_7d6bf9a54ea242fbacdbd3a58e8e6aa6.KeyFrames.Add(DiscreteObjectKeyFrame_f7e33f638a7d4189a1e315b1e1017d4f);


            Storyboard_9fea9234e26d4d3faf2bf7d71d4b8202.Children.Add(ObjectAnimationUsingKeyFrames_7d6bf9a54ea242fbacdbd3a58e8e6aa6);


            VisualState_79f02f22b7854653892bb18b8aeb665c.Storyboard = Storyboard_9fea9234e26d4d3faf2bf7d71d4b8202;


            var VisualState_47a5eb3e6ca14a2db5674ae42e0e4c08 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("CheckedPressed", VisualState_47a5eb3e6ca14a2db5674ae42e0e4c08);
            VisualState_47a5eb3e6ca14a2db5674ae42e0e4c08.Name = "CheckedPressed";
            var Storyboard_c9b7a0bd755f4abf8d89ee6ed690f27f = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_7ee10829af72466a86f946725fcd70e1 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_7ee10829af72466a86f946725fcd70e1, @"InnerBorder");
            var DiscreteObjectKeyFrame_6f535523ae684fcd864f7e71d04f2a88 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_6f535523ae684fcd864f7e71d04f2a88.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_6f535523ae684fcd864f7e71d04f2a88.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#44000000");

            ObjectAnimationUsingKeyFrames_7ee10829af72466a86f946725fcd70e1.KeyFrames.Add(DiscreteObjectKeyFrame_6f535523ae684fcd864f7e71d04f2a88);


            Storyboard_c9b7a0bd755f4abf8d89ee6ed690f27f.Children.Add(ObjectAnimationUsingKeyFrames_7ee10829af72466a86f946725fcd70e1);


            VisualState_47a5eb3e6ca14a2db5674ae42e0e4c08.Storyboard = Storyboard_c9b7a0bd755f4abf8d89ee6ed690f27f;


            var VisualState_ca014aa41bee416f877d67dadbcffa5b = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("CheckedDisabled", VisualState_ca014aa41bee416f877d67dadbcffa5b);
            VisualState_ca014aa41bee416f877d67dadbcffa5b.Name = "CheckedDisabled";
            var Storyboard_9f0836cde7e24d33b750edf5067de935 = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_ffae47ffb5604efb9a84ee8790ad7256 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_ffae47ffb5604efb9a84ee8790ad7256, @"InnerBorder");
            var DiscreteObjectKeyFrame_c67488c19b8f4a9c97cb9ce46fdebbc4 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_c67488c19b8f4a9c97cb9ce46fdebbc4.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_c67488c19b8f4a9c97cb9ce46fdebbc4.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#33000000");

            ObjectAnimationUsingKeyFrames_ffae47ffb5604efb9a84ee8790ad7256.KeyFrames.Add(DiscreteObjectKeyFrame_c67488c19b8f4a9c97cb9ce46fdebbc4);


            Storyboard_9f0836cde7e24d33b750edf5067de935.Children.Add(ObjectAnimationUsingKeyFrames_ffae47ffb5604efb9a84ee8790ad7256);


            VisualState_ca014aa41bee416f877d67dadbcffa5b.Storyboard = Storyboard_9f0836cde7e24d33b750edf5067de935;


            var VisualState_ab9735ea608d45ad91223123fc22e05a = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Indeterminate", VisualState_ab9735ea608d45ad91223123fc22e05a);
            VisualState_ab9735ea608d45ad91223123fc22e05a.Name = "Indeterminate";

            var VisualState_9bb802fd2f2b4da284bafc905c3d3c12 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("IndeterminatePointerOver", VisualState_9bb802fd2f2b4da284bafc905c3d3c12);
            VisualState_9bb802fd2f2b4da284bafc905c3d3c12.Name = "IndeterminatePointerOver";
            var Storyboard_828ce909eaad47ff9320ca12f528a6c1 = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_77cc5eeb488d43d1898deb233d8e9b82 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_77cc5eeb488d43d1898deb233d8e9b82, @"InnerBorder");
            var DiscreteObjectKeyFrame_72115f2bd3e34dedaa764aa60d8299b4 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_72115f2bd3e34dedaa764aa60d8299b4.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_72115f2bd3e34dedaa764aa60d8299b4.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#11000000");

            ObjectAnimationUsingKeyFrames_77cc5eeb488d43d1898deb233d8e9b82.KeyFrames.Add(DiscreteObjectKeyFrame_72115f2bd3e34dedaa764aa60d8299b4);


            Storyboard_828ce909eaad47ff9320ca12f528a6c1.Children.Add(ObjectAnimationUsingKeyFrames_77cc5eeb488d43d1898deb233d8e9b82);


            VisualState_9bb802fd2f2b4da284bafc905c3d3c12.Storyboard = Storyboard_828ce909eaad47ff9320ca12f528a6c1;


            var VisualState_9362048624bd43a2a8a08def95ff2db2 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("IndeterminatePressed", VisualState_9362048624bd43a2a8a08def95ff2db2);
            VisualState_9362048624bd43a2a8a08def95ff2db2.Name = "IndeterminatePressed";
            var Storyboard_8e38e57e25194fa98bc933c498dac3a9 = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_966dcdbf7f3940039694ca2f353628ab = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_966dcdbf7f3940039694ca2f353628ab, @"InnerBorder");
            var DiscreteObjectKeyFrame_ac871e7483004c3eb72dff738ef9cae2 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_ac871e7483004c3eb72dff738ef9cae2.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_ac871e7483004c3eb72dff738ef9cae2.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#22000000");

            ObjectAnimationUsingKeyFrames_966dcdbf7f3940039694ca2f353628ab.KeyFrames.Add(DiscreteObjectKeyFrame_ac871e7483004c3eb72dff738ef9cae2);


            Storyboard_8e38e57e25194fa98bc933c498dac3a9.Children.Add(ObjectAnimationUsingKeyFrames_966dcdbf7f3940039694ca2f353628ab);


            VisualState_9362048624bd43a2a8a08def95ff2db2.Storyboard = Storyboard_8e38e57e25194fa98bc933c498dac3a9;


            var VisualState_7c672ea0b6d04eecb82cd68a0f1b315d = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("IndeterminateDisabled", VisualState_7c672ea0b6d04eecb82cd68a0f1b315d);
            VisualState_7c672ea0b6d04eecb82cd68a0f1b315d.Name = "IndeterminateDisabled";
            var Storyboard_cb45bbe4399448019b040fcd0183de37 = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_968a0218eb264e37b2af66a34af6b43f = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_968a0218eb264e37b2af66a34af6b43f, @"InnerBorder");
            var DiscreteObjectKeyFrame_49739a3267574367942e188801a8e817 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_49739a3267574367942e188801a8e817.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_49739a3267574367942e188801a8e817.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Transparent");

            ObjectAnimationUsingKeyFrames_968a0218eb264e37b2af66a34af6b43f.KeyFrames.Add(DiscreteObjectKeyFrame_49739a3267574367942e188801a8e817);


            Storyboard_cb45bbe4399448019b040fcd0183de37.Children.Add(ObjectAnimationUsingKeyFrames_968a0218eb264e37b2af66a34af6b43f);


            VisualState_7c672ea0b6d04eecb82cd68a0f1b315d.Storyboard = Storyboard_cb45bbe4399448019b040fcd0183de37;


            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_48b9e3edd8b242539c8432e626559e9a);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_684653ffba544a559aa056c8b7c79846);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_99b521ca378f49729b15cc4157a429d1);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_1daa3b7feead4eb0a119e0fb3b51593b);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_09be70cea64c4a478df2dec5a2580ff3);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_79f02f22b7854653892bb18b8aeb665c);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_47a5eb3e6ca14a2db5674ae42e0e4c08);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_ca014aa41bee416f877d67dadbcffa5b);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_ab9735ea608d45ad91223123fc22e05a);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_9bb802fd2f2b4da284bafc905c3d3c12);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_9362048624bd43a2a8a08def95ff2db2);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_7c672ea0b6d04eecb82cd68a0f1b315d);


            ((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f);

            var Border_618ef8ba03af451087fbc33c3ff886fd = new global::System.Windows.Controls.Border();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("InnerBorder", Border_618ef8ba03af451087fbc33c3ff886fd);
            Border_618ef8ba03af451087fbc33c3ff886fd.Name = "InnerBorder";
            var ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b = new global::System.Windows.Controls.ContentPresenter();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("ContentPresenter", ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b);
            ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b.Name = "ContentPresenter";
            var Binding_bd9a4b1326c24277b03c58728e4f1d57 = new global::System.Windows.Data.Binding();
            Binding_bd9a4b1326c24277b03c58728e4f1d57.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"ContentTemplate");
            var RelativeSource_b2e45ade370640979e9160898fec36ac = new global::System.Windows.Data.RelativeSource();
            RelativeSource_b2e45ade370640979e9160898fec36ac.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_bd9a4b1326c24277b03c58728e4f1d57.RelativeSource = RelativeSource_b2e45ade370640979e9160898fec36ac;


            Binding_bd9a4b1326c24277b03c58728e4f1d57.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;

            var Binding_a3dcd8ccd7cf499e9b557bdf02f01328 = new global::System.Windows.Data.Binding();
            Binding_a3dcd8ccd7cf499e9b557bdf02f01328.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Content");
            var RelativeSource_103ac3ab7ffb4fad96701830251c3db3 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_103ac3ab7ffb4fad96701830251c3db3.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_a3dcd8ccd7cf499e9b557bdf02f01328.RelativeSource = RelativeSource_103ac3ab7ffb4fad96701830251c3db3;


            Binding_a3dcd8ccd7cf499e9b557bdf02f01328.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;

            var Binding_2467ac46051541889740ebd38456e109 = new global::System.Windows.Data.Binding();
            Binding_2467ac46051541889740ebd38456e109.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Padding");
            var RelativeSource_d3e089d6d0e44bd5a2c855ef84b0a818 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_d3e089d6d0e44bd5a2c855ef84b0a818.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_2467ac46051541889740ebd38456e109.RelativeSource = RelativeSource_d3e089d6d0e44bd5a2c855ef84b0a818;


            Binding_2467ac46051541889740ebd38456e109.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;

            var Binding_bf05e8f495f44e5e995154b104bb8070 = new global::System.Windows.Data.Binding();
            Binding_bf05e8f495f44e5e995154b104bb8070.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"HorizontalContentAlignment");
            var RelativeSource_edb026b51d1d40f593a4865dca8626ea = new global::System.Windows.Data.RelativeSource();
            RelativeSource_edb026b51d1d40f593a4865dca8626ea.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_bf05e8f495f44e5e995154b104bb8070.RelativeSource = RelativeSource_edb026b51d1d40f593a4865dca8626ea;


            Binding_bf05e8f495f44e5e995154b104bb8070.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;

            var Binding_7fbc735a025e48ec9dc4077b05f07eda = new global::System.Windows.Data.Binding();
            Binding_7fbc735a025e48ec9dc4077b05f07eda.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"VerticalContentAlignment");
            var RelativeSource_db9efb05a5b54f6eba8239b9473d3c04 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_db9efb05a5b54f6eba8239b9473d3c04.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_7fbc735a025e48ec9dc4077b05f07eda.RelativeSource = RelativeSource_db9efb05a5b54f6eba8239b9473d3c04;


            Binding_7fbc735a025e48ec9dc4077b05f07eda.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;


            Border_618ef8ba03af451087fbc33c3ff886fd.Child = ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b;

            var Binding_c197526fad1349559d83de37906189d2 = new global::System.Windows.Data.Binding();
            Binding_c197526fad1349559d83de37906189d2.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_e19f9349bcad4558bf06237f7285883b = new global::System.Windows.Data.RelativeSource();
            RelativeSource_e19f9349bcad4558bf06237f7285883b.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_c197526fad1349559d83de37906189d2.RelativeSource = RelativeSource_e19f9349bcad4558bf06237f7285883b;


            Binding_c197526fad1349559d83de37906189d2.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;


            Border_a87e17151acc48a89a823a0f683e1fed.Child = Border_618ef8ba03af451087fbc33c3ff886fd;

            var Binding_7580d02f07d04069b9e18d24fe79f910 = new global::System.Windows.Data.Binding();
            Binding_7580d02f07d04069b9e18d24fe79f910.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_7bdd6c3991844c3dbd0e55cce344929c = new global::System.Windows.Data.RelativeSource();
            RelativeSource_7bdd6c3991844c3dbd0e55cce344929c.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_7580d02f07d04069b9e18d24fe79f910.RelativeSource = RelativeSource_7bdd6c3991844c3dbd0e55cce344929c;


            Binding_7580d02f07d04069b9e18d24fe79f910.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;

            var Binding_f7647f9dbc354a0b9ef46d8fe5e43563 = new global::System.Windows.Data.Binding();
            Binding_f7647f9dbc354a0b9ef46d8fe5e43563.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
            var RelativeSource_20eb68f70c5b4c4496bc8f5d3668a787 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_20eb68f70c5b4c4496bc8f5d3668a787.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_f7647f9dbc354a0b9ef46d8fe5e43563.RelativeSource = RelativeSource_20eb68f70c5b4c4496bc8f5d3668a787;


            Binding_f7647f9dbc354a0b9ef46d8fe5e43563.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;

            var Binding_27886b8d45874035b5fc859ce0c9e79e = new global::System.Windows.Data.Binding();
            Binding_27886b8d45874035b5fc859ce0c9e79e.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
            var RelativeSource_5458bd550fe34d65973b97e783a04f3f = new global::System.Windows.Data.RelativeSource();
            RelativeSource_5458bd550fe34d65973b97e783a04f3f.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_27886b8d45874035b5fc859ce0c9e79e.RelativeSource = RelativeSource_5458bd550fe34d65973b97e783a04f3f;


            Binding_27886b8d45874035b5fc859ce0c9e79e.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;



            ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b.SetBinding(global::System.Windows.Controls.ContentPresenter.ContentTemplateProperty, Binding_bd9a4b1326c24277b03c58728e4f1d57);
            ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b.SetBinding(global::System.Windows.Controls.ContentPresenter.ContentProperty, Binding_a3dcd8ccd7cf499e9b557bdf02f01328);
            ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b.SetBinding(global::System.Windows.FrameworkElement.MarginProperty, Binding_2467ac46051541889740ebd38456e109);
            ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b.SetBinding(global::System.Windows.FrameworkElement.HorizontalAlignmentProperty, Binding_bf05e8f495f44e5e995154b104bb8070);
            ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b.SetBinding(global::System.Windows.FrameworkElement.VerticalAlignmentProperty, Binding_7fbc735a025e48ec9dc4077b05f07eda);
            Border_618ef8ba03af451087fbc33c3ff886fd.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_c197526fad1349559d83de37906189d2);
            Border_a87e17151acc48a89a823a0f683e1fed.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_7580d02f07d04069b9e18d24fe79f910);
            Border_a87e17151acc48a89a823a0f683e1fed.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_f7647f9dbc354a0b9ef46d8fe5e43563);
            Border_a87e17151acc48a89a823a0f683e1fed.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_27886b8d45874035b5fc859ce0c9e79e);

            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_1a7954717d81491899f6850738c30338,
                new global::System.Windows.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_4053b15dd4ee4d1397186015aef32799,
                    setVisualStateProperty_4053b15dd4ee4d1397186015aef32799,
                    setLocalVisualStateProperty_4053b15dd4ee4d1397186015aef32799,
                    getVisualStateProperty_4053b15dd4ee4d1397186015aef32799));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_1a7954717d81491899f6850738c30338, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_7aac0c45ca9846799f661d1d1e26d5a9,
                new global::System.Windows.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_2185526c11084008b4d96c9abeb9645a,
                    setVisualStateProperty_2185526c11084008b4d96c9abeb9645a,
                    setLocalVisualStateProperty_2185526c11084008b4d96c9abeb9645a,
                    getVisualStateProperty_2185526c11084008b4d96c9abeb9645a));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_7aac0c45ca9846799f661d1d1e26d5a9, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_7ffbc1f738364c4da17d691b1174720e,
                new global::System.Windows.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_6cf63a228a7648e3b09d349e50f10891,
                    setVisualStateProperty_6cf63a228a7648e3b09d349e50f10891,
                    setLocalVisualStateProperty_6cf63a228a7648e3b09d349e50f10891,
                    getVisualStateProperty_6cf63a228a7648e3b09d349e50f10891));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_7ffbc1f738364c4da17d691b1174720e, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_a345c63cb00f47b3930be6088ec8f206,
                new global::System.Windows.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_f1f1a394431e4e8391168272c7730f8a,
                    setVisualStateProperty_f1f1a394431e4e8391168272c7730f8a,
                    setLocalVisualStateProperty_f1f1a394431e4e8391168272c7730f8a,
                    getVisualStateProperty_f1f1a394431e4e8391168272c7730f8a));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_a345c63cb00f47b3930be6088ec8f206, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_7d6bf9a54ea242fbacdbd3a58e8e6aa6,
                new global::System.Windows.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_5ff70c31b6ca47ec92195e328b383a7a,
                    setVisualStateProperty_5ff70c31b6ca47ec92195e328b383a7a,
                    setLocalVisualStateProperty_5ff70c31b6ca47ec92195e328b383a7a,
                    getVisualStateProperty_5ff70c31b6ca47ec92195e328b383a7a));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_7d6bf9a54ea242fbacdbd3a58e8e6aa6, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_7ee10829af72466a86f946725fcd70e1,
                new global::System.Windows.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_e6c6dc7ea17d4a1fb8de1088ab5ba7fb,
                    setVisualStateProperty_e6c6dc7ea17d4a1fb8de1088ab5ba7fb,
                    setLocalVisualStateProperty_e6c6dc7ea17d4a1fb8de1088ab5ba7fb,
                    getVisualStateProperty_e6c6dc7ea17d4a1fb8de1088ab5ba7fb));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_7ee10829af72466a86f946725fcd70e1, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_ffae47ffb5604efb9a84ee8790ad7256,
                new global::System.Windows.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_0cd646131198415198ed08a08586b3c8,
                    setVisualStateProperty_0cd646131198415198ed08a08586b3c8,
                    setLocalVisualStateProperty_0cd646131198415198ed08a08586b3c8,
                    getVisualStateProperty_0cd646131198415198ed08a08586b3c8));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_ffae47ffb5604efb9a84ee8790ad7256, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_77cc5eeb488d43d1898deb233d8e9b82,
                new global::System.Windows.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_0d8762d01568423da62f5c866eeb0199,
                    setVisualStateProperty_0d8762d01568423da62f5c866eeb0199,
                    setLocalVisualStateProperty_0d8762d01568423da62f5c866eeb0199,
                    getVisualStateProperty_0d8762d01568423da62f5c866eeb0199));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_77cc5eeb488d43d1898deb233d8e9b82, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_966dcdbf7f3940039694ca2f353628ab,
                new global::System.Windows.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_e4b4395223e3450fb04826f9f30828ab,
                    setVisualStateProperty_e4b4395223e3450fb04826f9f30828ab,
                    setLocalVisualStateProperty_e4b4395223e3450fb04826f9f30828ab,
                    getVisualStateProperty_e4b4395223e3450fb04826f9f30828ab));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_966dcdbf7f3940039694ca2f353628ab, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_968a0218eb264e37b2af66a34af6b43f,
                new global::System.Windows.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_750f3d33ac674ccab0bd2eac596d0d3e,
                    setVisualStateProperty_750f3d33ac674ccab0bd2eac596d0d3e,
                    setLocalVisualStateProperty_750f3d33ac674ccab0bd2eac596d0d3e,
                    getVisualStateProperty_750f3d33ac674ccab0bd2eac596d0d3e));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_968a0218eb264e37b2af66a34af6b43f, Border_618ef8ba03af451087fbc33c3ff886fd);

            templateInstance_523b354795e748a6a0c81ac8bd051b6a.TemplateContent = Border_a87e17151acc48a89a823a0f683e1fed;
            return templateInstance_523b354795e748a6a0c81ac8bd051b6a;
        }

    }
}
#else
namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultToggleButtonStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_1766cd45f4114caab929a4040c7e5b66 = new global::Windows.UI.Xaml.Style();
                Style_1766cd45f4114caab929a4040c7e5b66.TargetType = typeof(global::Windows.UI.Xaml.Controls.Primitives.ToggleButton);
                var Setter_1aecc6235bd2473dae20c12f0da7fe02 = new global::Windows.UI.Xaml.Setter();
                Setter_1aecc6235bd2473dae20c12f0da7fe02.Property = global::Windows.UI.Xaml.Controls.Primitives.ToggleButton.BackgroundProperty;
                Setter_1aecc6235bd2473dae20c12f0da7fe02.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFE2E2E2");

                var Setter_a405e10599f3416d912ccd03ba0ed2a3 = new global::Windows.UI.Xaml.Setter();
                Setter_a405e10599f3416d912ccd03ba0ed2a3.Property = global::Windows.UI.Xaml.Controls.Primitives.ToggleButton.ForegroundProperty;
                Setter_a405e10599f3416d912ccd03ba0ed2a3.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Black");

                var Setter_e58113ee28a14fc8a50e32c4690ebd06 = new global::Windows.UI.Xaml.Setter();
                Setter_e58113ee28a14fc8a50e32c4690ebd06.Property = global::Windows.UI.Xaml.Controls.Primitives.ToggleButton.BorderThicknessProperty;
                Setter_e58113ee28a14fc8a50e32c4690ebd06.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0");

                var Setter_c2841f6a39724f308ec0e15a679a5229 = new global::Windows.UI.Xaml.Setter();
                Setter_c2841f6a39724f308ec0e15a679a5229.Property = global::Windows.UI.Xaml.Controls.Primitives.ToggleButton.PaddingProperty;
                Setter_c2841f6a39724f308ec0e15a679a5229.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"12,4,12,4");

                var Setter_139988636fd54cdaac6071028a3f56f2 = new global::Windows.UI.Xaml.Setter();
                Setter_139988636fd54cdaac6071028a3f56f2.Property = global::Windows.UI.Xaml.Controls.Primitives.ToggleButton.CursorProperty;
                Setter_139988636fd54cdaac6071028a3f56f2.Value = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Hand");

                var Setter_d240066348aa45d29abe85ddfdfe8953 = new global::Windows.UI.Xaml.Setter();
                Setter_d240066348aa45d29abe85ddfdfe8953.Property = global::Windows.UI.Xaml.Controls.Primitives.ToggleButton.HorizontalContentAlignmentProperty;
                Setter_d240066348aa45d29abe85ddfdfe8953.Value = global::Windows.UI.Xaml.HorizontalAlignment.Center;

                var Setter_8fa78a0109c848f89228783e3bfbda8e = new global::Windows.UI.Xaml.Setter();
                Setter_8fa78a0109c848f89228783e3bfbda8e.Property = global::Windows.UI.Xaml.Controls.Primitives.ToggleButton.VerticalContentAlignmentProperty;
                Setter_8fa78a0109c848f89228783e3bfbda8e.Value = global::Windows.UI.Xaml.VerticalAlignment.Center;

                var Setter_491c520659714419b27d0f79cf8b1d32 = new global::Windows.UI.Xaml.Setter();
                Setter_491c520659714419b27d0f79cf8b1d32.Property = global::Windows.UI.Xaml.Controls.Primitives.ToggleButton.TemplateProperty;
                var ControlTemplate_4824bc99f1f14e57ac1c085c3a1c1321 = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_4824bc99f1f14e57ac1c085c3a1c1321.TargetType = typeof(global::Windows.UI.Xaml.Controls.Primitives.ToggleButton);
                ControlTemplate_4824bc99f1f14e57ac1c085c3a1c1321.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_4824bc99f1f14e57ac1c085c3a1c1321);

                Setter_491c520659714419b27d0f79cf8b1d32.Value = ControlTemplate_4824bc99f1f14e57ac1c085c3a1c1321;


                Style_1766cd45f4114caab929a4040c7e5b66.Setters.Add(Setter_1aecc6235bd2473dae20c12f0da7fe02);
                Style_1766cd45f4114caab929a4040c7e5b66.Setters.Add(Setter_a405e10599f3416d912ccd03ba0ed2a3);
                Style_1766cd45f4114caab929a4040c7e5b66.Setters.Add(Setter_e58113ee28a14fc8a50e32c4690ebd06);
                Style_1766cd45f4114caab929a4040c7e5b66.Setters.Add(Setter_c2841f6a39724f308ec0e15a679a5229);
                Style_1766cd45f4114caab929a4040c7e5b66.Setters.Add(Setter_139988636fd54cdaac6071028a3f56f2);
                Style_1766cd45f4114caab929a4040c7e5b66.Setters.Add(Setter_d240066348aa45d29abe85ddfdfe8953);
                Style_1766cd45f4114caab929a4040c7e5b66.Setters.Add(Setter_8fa78a0109c848f89228783e3bfbda8e);
                Style_1766cd45f4114caab929a4040c7e5b66.Setters.Add(Setter_491c520659714419b27d0f79cf8b1d32);


                DefaultStyle = Style_1766cd45f4114caab929a4040c7e5b66;
            }

            return DefaultStyle;
        }

        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_4053b15dd4ee4d1397186015aef32799(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_4053b15dd4ee4d1397186015aef32799(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_4053b15dd4ee4d1397186015aef32799(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_4053b15dd4ee4d1397186015aef32799(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_2185526c11084008b4d96c9abeb9645a(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_2185526c11084008b4d96c9abeb9645a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_2185526c11084008b4d96c9abeb9645a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_2185526c11084008b4d96c9abeb9645a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_6cf63a228a7648e3b09d349e50f10891(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_6cf63a228a7648e3b09d349e50f10891(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_6cf63a228a7648e3b09d349e50f10891(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_6cf63a228a7648e3b09d349e50f10891(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_f1f1a394431e4e8391168272c7730f8a(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_f1f1a394431e4e8391168272c7730f8a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_f1f1a394431e4e8391168272c7730f8a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_f1f1a394431e4e8391168272c7730f8a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_5ff70c31b6ca47ec92195e328b383a7a(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_5ff70c31b6ca47ec92195e328b383a7a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_5ff70c31b6ca47ec92195e328b383a7a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_5ff70c31b6ca47ec92195e328b383a7a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_e6c6dc7ea17d4a1fb8de1088ab5ba7fb(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_e6c6dc7ea17d4a1fb8de1088ab5ba7fb(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_e6c6dc7ea17d4a1fb8de1088ab5ba7fb(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_e6c6dc7ea17d4a1fb8de1088ab5ba7fb(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_0cd646131198415198ed08a08586b3c8(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_0cd646131198415198ed08a08586b3c8(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_0cd646131198415198ed08a08586b3c8(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_0cd646131198415198ed08a08586b3c8(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_0d8762d01568423da62f5c866eeb0199(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_0d8762d01568423da62f5c866eeb0199(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_0d8762d01568423da62f5c866eeb0199(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_0d8762d01568423da62f5c866eeb0199(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_e4b4395223e3450fb04826f9f30828ab(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_e4b4395223e3450fb04826f9f30828ab(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_e4b4395223e3450fb04826f9f30828ab(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_e4b4395223e3450fb04826f9f30828ab(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_750f3d33ac674ccab0bd2eac596d0d3e(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_750f3d33ac674ccab0bd2eac596d0d3e(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_750f3d33ac674ccab0bd2eac596d0d3e(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_750f3d33ac674ccab0bd2eac596d0d3e(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Background").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("BackgroundProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_4824bc99f1f14e57ac1c085c3a1c1321(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_523b354795e748a6a0c81ac8bd051b6a = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_523b354795e748a6a0c81ac8bd051b6a.TemplateOwner = templateOwner;
            var Border_a87e17151acc48a89a823a0f683e1fed = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("OuterBorder", Border_a87e17151acc48a89a823a0f683e1fed);
            Border_a87e17151acc48a89a823a0f683e1fed.Name = "OuterBorder";
            var VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.Name = "CommonStates";
            var VisualState_48b9e3edd8b242539c8432e626559e9a = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Normal", VisualState_48b9e3edd8b242539c8432e626559e9a);
            VisualState_48b9e3edd8b242539c8432e626559e9a.Name = "Normal";

            var VisualState_684653ffba544a559aa056c8b7c79846 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("PointerOver", VisualState_684653ffba544a559aa056c8b7c79846);
            VisualState_684653ffba544a559aa056c8b7c79846.Name = "PointerOver";
            var Storyboard_dee5e87903934d7cb6a776c10f9050d3 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_1a7954717d81491899f6850738c30338 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_1a7954717d81491899f6850738c30338, @"InnerBorder");
            var DiscreteObjectKeyFrame_0c24c6d734e842da999fd6797b4343bb = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_0c24c6d734e842da999fd6797b4343bb.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_0c24c6d734e842da999fd6797b4343bb.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#11000000");

            ObjectAnimationUsingKeyFrames_1a7954717d81491899f6850738c30338.KeyFrames.Add(DiscreteObjectKeyFrame_0c24c6d734e842da999fd6797b4343bb);


            Storyboard_dee5e87903934d7cb6a776c10f9050d3.Children.Add(ObjectAnimationUsingKeyFrames_1a7954717d81491899f6850738c30338);


            VisualState_684653ffba544a559aa056c8b7c79846.Storyboard = Storyboard_dee5e87903934d7cb6a776c10f9050d3;


            var VisualState_99b521ca378f49729b15cc4157a429d1 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Pressed", VisualState_99b521ca378f49729b15cc4157a429d1);
            VisualState_99b521ca378f49729b15cc4157a429d1.Name = "Pressed";
            var Storyboard_089bb4f88cfb4eb6914b2eec480bb937 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_7aac0c45ca9846799f661d1d1e26d5a9 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_7aac0c45ca9846799f661d1d1e26d5a9, @"InnerBorder");
            var DiscreteObjectKeyFrame_33a0ea8820d94d278a74c46c8303f99b = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_33a0ea8820d94d278a74c46c8303f99b.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_33a0ea8820d94d278a74c46c8303f99b.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#44000000");

            ObjectAnimationUsingKeyFrames_7aac0c45ca9846799f661d1d1e26d5a9.KeyFrames.Add(DiscreteObjectKeyFrame_33a0ea8820d94d278a74c46c8303f99b);


            Storyboard_089bb4f88cfb4eb6914b2eec480bb937.Children.Add(ObjectAnimationUsingKeyFrames_7aac0c45ca9846799f661d1d1e26d5a9);


            VisualState_99b521ca378f49729b15cc4157a429d1.Storyboard = Storyboard_089bb4f88cfb4eb6914b2eec480bb937;


            var VisualState_1daa3b7feead4eb0a119e0fb3b51593b = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_1daa3b7feead4eb0a119e0fb3b51593b);
            VisualState_1daa3b7feead4eb0a119e0fb3b51593b.Name = "Disabled";
            var Storyboard_0be07567d87f4bfa96c42a05e7aca71b = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_7ffbc1f738364c4da17d691b1174720e = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_7ffbc1f738364c4da17d691b1174720e, @"InnerBorder");
            var DiscreteObjectKeyFrame_29621ab2fc2847a787153f8531ab7a38 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_29621ab2fc2847a787153f8531ab7a38.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_29621ab2fc2847a787153f8531ab7a38.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Transparent");

            ObjectAnimationUsingKeyFrames_7ffbc1f738364c4da17d691b1174720e.KeyFrames.Add(DiscreteObjectKeyFrame_29621ab2fc2847a787153f8531ab7a38);


            Storyboard_0be07567d87f4bfa96c42a05e7aca71b.Children.Add(ObjectAnimationUsingKeyFrames_7ffbc1f738364c4da17d691b1174720e);


            VisualState_1daa3b7feead4eb0a119e0fb3b51593b.Storyboard = Storyboard_0be07567d87f4bfa96c42a05e7aca71b;


            var VisualState_09be70cea64c4a478df2dec5a2580ff3 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Checked", VisualState_09be70cea64c4a478df2dec5a2580ff3);
            VisualState_09be70cea64c4a478df2dec5a2580ff3.Name = "Checked";
            var Storyboard_6c454763aefc493b97eaf9466488c993 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_a345c63cb00f47b3930be6088ec8f206 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_a345c63cb00f47b3930be6088ec8f206, @"InnerBorder");
            var DiscreteObjectKeyFrame_5ce95b24c17343229602aea4bb073cd2 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_5ce95b24c17343229602aea4bb073cd2.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_5ce95b24c17343229602aea4bb073cd2.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#33000000");

            ObjectAnimationUsingKeyFrames_a345c63cb00f47b3930be6088ec8f206.KeyFrames.Add(DiscreteObjectKeyFrame_5ce95b24c17343229602aea4bb073cd2);


            Storyboard_6c454763aefc493b97eaf9466488c993.Children.Add(ObjectAnimationUsingKeyFrames_a345c63cb00f47b3930be6088ec8f206);


            VisualState_09be70cea64c4a478df2dec5a2580ff3.Storyboard = Storyboard_6c454763aefc493b97eaf9466488c993;


            var VisualState_79f02f22b7854653892bb18b8aeb665c = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CheckedPointerOver", VisualState_79f02f22b7854653892bb18b8aeb665c);
            VisualState_79f02f22b7854653892bb18b8aeb665c.Name = "CheckedPointerOver";
            var Storyboard_9fea9234e26d4d3faf2bf7d71d4b8202 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_7d6bf9a54ea242fbacdbd3a58e8e6aa6 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_7d6bf9a54ea242fbacdbd3a58e8e6aa6, @"InnerBorder");
            var DiscreteObjectKeyFrame_f7e33f638a7d4189a1e315b1e1017d4f = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_f7e33f638a7d4189a1e315b1e1017d4f.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_f7e33f638a7d4189a1e315b1e1017d4f.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#22000000");

            ObjectAnimationUsingKeyFrames_7d6bf9a54ea242fbacdbd3a58e8e6aa6.KeyFrames.Add(DiscreteObjectKeyFrame_f7e33f638a7d4189a1e315b1e1017d4f);


            Storyboard_9fea9234e26d4d3faf2bf7d71d4b8202.Children.Add(ObjectAnimationUsingKeyFrames_7d6bf9a54ea242fbacdbd3a58e8e6aa6);


            VisualState_79f02f22b7854653892bb18b8aeb665c.Storyboard = Storyboard_9fea9234e26d4d3faf2bf7d71d4b8202;


            var VisualState_47a5eb3e6ca14a2db5674ae42e0e4c08 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CheckedPressed", VisualState_47a5eb3e6ca14a2db5674ae42e0e4c08);
            VisualState_47a5eb3e6ca14a2db5674ae42e0e4c08.Name = "CheckedPressed";
            var Storyboard_c9b7a0bd755f4abf8d89ee6ed690f27f = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_7ee10829af72466a86f946725fcd70e1 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_7ee10829af72466a86f946725fcd70e1, @"InnerBorder");
            var DiscreteObjectKeyFrame_6f535523ae684fcd864f7e71d04f2a88 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_6f535523ae684fcd864f7e71d04f2a88.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_6f535523ae684fcd864f7e71d04f2a88.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#44000000");

            ObjectAnimationUsingKeyFrames_7ee10829af72466a86f946725fcd70e1.KeyFrames.Add(DiscreteObjectKeyFrame_6f535523ae684fcd864f7e71d04f2a88);


            Storyboard_c9b7a0bd755f4abf8d89ee6ed690f27f.Children.Add(ObjectAnimationUsingKeyFrames_7ee10829af72466a86f946725fcd70e1);


            VisualState_47a5eb3e6ca14a2db5674ae42e0e4c08.Storyboard = Storyboard_c9b7a0bd755f4abf8d89ee6ed690f27f;


            var VisualState_ca014aa41bee416f877d67dadbcffa5b = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CheckedDisabled", VisualState_ca014aa41bee416f877d67dadbcffa5b);
            VisualState_ca014aa41bee416f877d67dadbcffa5b.Name = "CheckedDisabled";
            var Storyboard_9f0836cde7e24d33b750edf5067de935 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_ffae47ffb5604efb9a84ee8790ad7256 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_ffae47ffb5604efb9a84ee8790ad7256, @"InnerBorder");
            var DiscreteObjectKeyFrame_c67488c19b8f4a9c97cb9ce46fdebbc4 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_c67488c19b8f4a9c97cb9ce46fdebbc4.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_c67488c19b8f4a9c97cb9ce46fdebbc4.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#33000000");

            ObjectAnimationUsingKeyFrames_ffae47ffb5604efb9a84ee8790ad7256.KeyFrames.Add(DiscreteObjectKeyFrame_c67488c19b8f4a9c97cb9ce46fdebbc4);


            Storyboard_9f0836cde7e24d33b750edf5067de935.Children.Add(ObjectAnimationUsingKeyFrames_ffae47ffb5604efb9a84ee8790ad7256);


            VisualState_ca014aa41bee416f877d67dadbcffa5b.Storyboard = Storyboard_9f0836cde7e24d33b750edf5067de935;


            var VisualState_ab9735ea608d45ad91223123fc22e05a = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Indeterminate", VisualState_ab9735ea608d45ad91223123fc22e05a);
            VisualState_ab9735ea608d45ad91223123fc22e05a.Name = "Indeterminate";

            var VisualState_9bb802fd2f2b4da284bafc905c3d3c12 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("IndeterminatePointerOver", VisualState_9bb802fd2f2b4da284bafc905c3d3c12);
            VisualState_9bb802fd2f2b4da284bafc905c3d3c12.Name = "IndeterminatePointerOver";
            var Storyboard_828ce909eaad47ff9320ca12f528a6c1 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_77cc5eeb488d43d1898deb233d8e9b82 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_77cc5eeb488d43d1898deb233d8e9b82, @"InnerBorder");
            var DiscreteObjectKeyFrame_72115f2bd3e34dedaa764aa60d8299b4 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_72115f2bd3e34dedaa764aa60d8299b4.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_72115f2bd3e34dedaa764aa60d8299b4.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#11000000");

            ObjectAnimationUsingKeyFrames_77cc5eeb488d43d1898deb233d8e9b82.KeyFrames.Add(DiscreteObjectKeyFrame_72115f2bd3e34dedaa764aa60d8299b4);


            Storyboard_828ce909eaad47ff9320ca12f528a6c1.Children.Add(ObjectAnimationUsingKeyFrames_77cc5eeb488d43d1898deb233d8e9b82);


            VisualState_9bb802fd2f2b4da284bafc905c3d3c12.Storyboard = Storyboard_828ce909eaad47ff9320ca12f528a6c1;


            var VisualState_9362048624bd43a2a8a08def95ff2db2 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("IndeterminatePressed", VisualState_9362048624bd43a2a8a08def95ff2db2);
            VisualState_9362048624bd43a2a8a08def95ff2db2.Name = "IndeterminatePressed";
            var Storyboard_8e38e57e25194fa98bc933c498dac3a9 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_966dcdbf7f3940039694ca2f353628ab = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_966dcdbf7f3940039694ca2f353628ab, @"InnerBorder");
            var DiscreteObjectKeyFrame_ac871e7483004c3eb72dff738ef9cae2 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_ac871e7483004c3eb72dff738ef9cae2.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_ac871e7483004c3eb72dff738ef9cae2.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"#22000000");

            ObjectAnimationUsingKeyFrames_966dcdbf7f3940039694ca2f353628ab.KeyFrames.Add(DiscreteObjectKeyFrame_ac871e7483004c3eb72dff738ef9cae2);


            Storyboard_8e38e57e25194fa98bc933c498dac3a9.Children.Add(ObjectAnimationUsingKeyFrames_966dcdbf7f3940039694ca2f353628ab);


            VisualState_9362048624bd43a2a8a08def95ff2db2.Storyboard = Storyboard_8e38e57e25194fa98bc933c498dac3a9;


            var VisualState_7c672ea0b6d04eecb82cd68a0f1b315d = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("IndeterminateDisabled", VisualState_7c672ea0b6d04eecb82cd68a0f1b315d);
            VisualState_7c672ea0b6d04eecb82cd68a0f1b315d.Name = "IndeterminateDisabled";
            var Storyboard_cb45bbe4399448019b040fcd0183de37 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_968a0218eb264e37b2af66a34af6b43f = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_968a0218eb264e37b2af66a34af6b43f, @"InnerBorder");
            var DiscreteObjectKeyFrame_49739a3267574367942e188801a8e817 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_49739a3267574367942e188801a8e817.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_49739a3267574367942e188801a8e817.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Transparent");

            ObjectAnimationUsingKeyFrames_968a0218eb264e37b2af66a34af6b43f.KeyFrames.Add(DiscreteObjectKeyFrame_49739a3267574367942e188801a8e817);


            Storyboard_cb45bbe4399448019b040fcd0183de37.Children.Add(ObjectAnimationUsingKeyFrames_968a0218eb264e37b2af66a34af6b43f);


            VisualState_7c672ea0b6d04eecb82cd68a0f1b315d.Storyboard = Storyboard_cb45bbe4399448019b040fcd0183de37;


            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_48b9e3edd8b242539c8432e626559e9a);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_684653ffba544a559aa056c8b7c79846);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_99b521ca378f49729b15cc4157a429d1);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_1daa3b7feead4eb0a119e0fb3b51593b);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_09be70cea64c4a478df2dec5a2580ff3);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_79f02f22b7854653892bb18b8aeb665c);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_47a5eb3e6ca14a2db5674ae42e0e4c08);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_ca014aa41bee416f877d67dadbcffa5b);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_ab9735ea608d45ad91223123fc22e05a);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_9bb802fd2f2b4da284bafc905c3d3c12);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_9362048624bd43a2a8a08def95ff2db2);
            VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f.States.Add(VisualState_7c672ea0b6d04eecb82cd68a0f1b315d);


            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_9cc4c1a506914a7da1137da9d880ad8f);

            var Border_618ef8ba03af451087fbc33c3ff886fd = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("InnerBorder", Border_618ef8ba03af451087fbc33c3ff886fd);
            Border_618ef8ba03af451087fbc33c3ff886fd.Name = "InnerBorder";
            var ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b = new global::Windows.UI.Xaml.Controls.ContentPresenter();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ContentPresenter", ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b);
            ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b.Name = "ContentPresenter";
            var Binding_bd9a4b1326c24277b03c58728e4f1d57 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_bd9a4b1326c24277b03c58728e4f1d57.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"ContentTemplate");
            var RelativeSource_b2e45ade370640979e9160898fec36ac = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_b2e45ade370640979e9160898fec36ac.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_bd9a4b1326c24277b03c58728e4f1d57.RelativeSource = RelativeSource_b2e45ade370640979e9160898fec36ac;


            Binding_bd9a4b1326c24277b03c58728e4f1d57.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;

            var Binding_a3dcd8ccd7cf499e9b557bdf02f01328 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_a3dcd8ccd7cf499e9b557bdf02f01328.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Content");
            var RelativeSource_103ac3ab7ffb4fad96701830251c3db3 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_103ac3ab7ffb4fad96701830251c3db3.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_a3dcd8ccd7cf499e9b557bdf02f01328.RelativeSource = RelativeSource_103ac3ab7ffb4fad96701830251c3db3;


            Binding_a3dcd8ccd7cf499e9b557bdf02f01328.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;

            var Binding_2467ac46051541889740ebd38456e109 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_2467ac46051541889740ebd38456e109.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Padding");
            var RelativeSource_d3e089d6d0e44bd5a2c855ef84b0a818 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_d3e089d6d0e44bd5a2c855ef84b0a818.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_2467ac46051541889740ebd38456e109.RelativeSource = RelativeSource_d3e089d6d0e44bd5a2c855ef84b0a818;


            Binding_2467ac46051541889740ebd38456e109.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;

            var Binding_bf05e8f495f44e5e995154b104bb8070 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_bf05e8f495f44e5e995154b104bb8070.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"HorizontalContentAlignment");
            var RelativeSource_edb026b51d1d40f593a4865dca8626ea = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_edb026b51d1d40f593a4865dca8626ea.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_bf05e8f495f44e5e995154b104bb8070.RelativeSource = RelativeSource_edb026b51d1d40f593a4865dca8626ea;


            Binding_bf05e8f495f44e5e995154b104bb8070.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;

            var Binding_7fbc735a025e48ec9dc4077b05f07eda = new global::Windows.UI.Xaml.Data.Binding();
            Binding_7fbc735a025e48ec9dc4077b05f07eda.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"VerticalContentAlignment");
            var RelativeSource_db9efb05a5b54f6eba8239b9473d3c04 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_db9efb05a5b54f6eba8239b9473d3c04.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_7fbc735a025e48ec9dc4077b05f07eda.RelativeSource = RelativeSource_db9efb05a5b54f6eba8239b9473d3c04;


            Binding_7fbc735a025e48ec9dc4077b05f07eda.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;


            Border_618ef8ba03af451087fbc33c3ff886fd.Child = ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b;

            var Binding_c197526fad1349559d83de37906189d2 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_c197526fad1349559d83de37906189d2.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_e19f9349bcad4558bf06237f7285883b = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_e19f9349bcad4558bf06237f7285883b.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_c197526fad1349559d83de37906189d2.RelativeSource = RelativeSource_e19f9349bcad4558bf06237f7285883b;


            Binding_c197526fad1349559d83de37906189d2.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;


            Border_a87e17151acc48a89a823a0f683e1fed.Child = Border_618ef8ba03af451087fbc33c3ff886fd;

            var Binding_7580d02f07d04069b9e18d24fe79f910 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_7580d02f07d04069b9e18d24fe79f910.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_7bdd6c3991844c3dbd0e55cce344929c = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_7bdd6c3991844c3dbd0e55cce344929c.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_7580d02f07d04069b9e18d24fe79f910.RelativeSource = RelativeSource_7bdd6c3991844c3dbd0e55cce344929c;


            Binding_7580d02f07d04069b9e18d24fe79f910.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;

            var Binding_f7647f9dbc354a0b9ef46d8fe5e43563 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_f7647f9dbc354a0b9ef46d8fe5e43563.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_20eb68f70c5b4c4496bc8f5d3668a787 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_20eb68f70c5b4c4496bc8f5d3668a787.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_f7647f9dbc354a0b9ef46d8fe5e43563.RelativeSource = RelativeSource_20eb68f70c5b4c4496bc8f5d3668a787;


            Binding_f7647f9dbc354a0b9ef46d8fe5e43563.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;

            var Binding_27886b8d45874035b5fc859ce0c9e79e = new global::Windows.UI.Xaml.Data.Binding();
            Binding_27886b8d45874035b5fc859ce0c9e79e.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_5458bd550fe34d65973b97e783a04f3f = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_5458bd550fe34d65973b97e783a04f3f.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_27886b8d45874035b5fc859ce0c9e79e.RelativeSource = RelativeSource_5458bd550fe34d65973b97e783a04f3f;


            Binding_27886b8d45874035b5fc859ce0c9e79e.TemplateOwner = templateInstance_523b354795e748a6a0c81ac8bd051b6a;



            ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b.SetBinding(global::Windows.UI.Xaml.Controls.ContentPresenter.ContentTemplateProperty, Binding_bd9a4b1326c24277b03c58728e4f1d57);
            ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b.SetBinding(global::Windows.UI.Xaml.Controls.ContentPresenter.ContentProperty, Binding_a3dcd8ccd7cf499e9b557bdf02f01328);
            ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b.SetBinding(global::Windows.UI.Xaml.FrameworkElement.MarginProperty, Binding_2467ac46051541889740ebd38456e109);
            ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b.SetBinding(global::Windows.UI.Xaml.FrameworkElement.HorizontalAlignmentProperty, Binding_bf05e8f495f44e5e995154b104bb8070);
            ContentPresenter_1ba8532ea1324ec4a11f602e5aac5b9b.SetBinding(global::Windows.UI.Xaml.FrameworkElement.VerticalAlignmentProperty, Binding_7fbc735a025e48ec9dc4077b05f07eda);
            Border_618ef8ba03af451087fbc33c3ff886fd.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_c197526fad1349559d83de37906189d2);
            Border_a87e17151acc48a89a823a0f683e1fed.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_7580d02f07d04069b9e18d24fe79f910);
            Border_a87e17151acc48a89a823a0f683e1fed.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_f7647f9dbc354a0b9ef46d8fe5e43563);
            Border_a87e17151acc48a89a823a0f683e1fed.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_27886b8d45874035b5fc859ce0c9e79e);

            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_1a7954717d81491899f6850738c30338,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_4053b15dd4ee4d1397186015aef32799,
                    setVisualStateProperty_4053b15dd4ee4d1397186015aef32799,
                    setLocalVisualStateProperty_4053b15dd4ee4d1397186015aef32799,
                    getVisualStateProperty_4053b15dd4ee4d1397186015aef32799));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_1a7954717d81491899f6850738c30338, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_7aac0c45ca9846799f661d1d1e26d5a9,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_2185526c11084008b4d96c9abeb9645a,
                    setVisualStateProperty_2185526c11084008b4d96c9abeb9645a,
                    setLocalVisualStateProperty_2185526c11084008b4d96c9abeb9645a,
                    getVisualStateProperty_2185526c11084008b4d96c9abeb9645a));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_7aac0c45ca9846799f661d1d1e26d5a9, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_7ffbc1f738364c4da17d691b1174720e,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_6cf63a228a7648e3b09d349e50f10891,
                    setVisualStateProperty_6cf63a228a7648e3b09d349e50f10891,
                    setLocalVisualStateProperty_6cf63a228a7648e3b09d349e50f10891,
                    getVisualStateProperty_6cf63a228a7648e3b09d349e50f10891));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_7ffbc1f738364c4da17d691b1174720e, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_a345c63cb00f47b3930be6088ec8f206,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_f1f1a394431e4e8391168272c7730f8a,
                    setVisualStateProperty_f1f1a394431e4e8391168272c7730f8a,
                    setLocalVisualStateProperty_f1f1a394431e4e8391168272c7730f8a,
                    getVisualStateProperty_f1f1a394431e4e8391168272c7730f8a));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_a345c63cb00f47b3930be6088ec8f206, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_7d6bf9a54ea242fbacdbd3a58e8e6aa6,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_5ff70c31b6ca47ec92195e328b383a7a,
                    setVisualStateProperty_5ff70c31b6ca47ec92195e328b383a7a,
                    setLocalVisualStateProperty_5ff70c31b6ca47ec92195e328b383a7a,
                    getVisualStateProperty_5ff70c31b6ca47ec92195e328b383a7a));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_7d6bf9a54ea242fbacdbd3a58e8e6aa6, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_7ee10829af72466a86f946725fcd70e1,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_e6c6dc7ea17d4a1fb8de1088ab5ba7fb,
                    setVisualStateProperty_e6c6dc7ea17d4a1fb8de1088ab5ba7fb,
                    setLocalVisualStateProperty_e6c6dc7ea17d4a1fb8de1088ab5ba7fb,
                    getVisualStateProperty_e6c6dc7ea17d4a1fb8de1088ab5ba7fb));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_7ee10829af72466a86f946725fcd70e1, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_ffae47ffb5604efb9a84ee8790ad7256,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_0cd646131198415198ed08a08586b3c8,
                    setVisualStateProperty_0cd646131198415198ed08a08586b3c8,
                    setLocalVisualStateProperty_0cd646131198415198ed08a08586b3c8,
                    getVisualStateProperty_0cd646131198415198ed08a08586b3c8));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_ffae47ffb5604efb9a84ee8790ad7256, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_77cc5eeb488d43d1898deb233d8e9b82,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_0d8762d01568423da62f5c866eeb0199,
                    setVisualStateProperty_0d8762d01568423da62f5c866eeb0199,
                    setLocalVisualStateProperty_0d8762d01568423da62f5c866eeb0199,
                    getVisualStateProperty_0d8762d01568423da62f5c866eeb0199));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_77cc5eeb488d43d1898deb233d8e9b82, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_966dcdbf7f3940039694ca2f353628ab,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_e4b4395223e3450fb04826f9f30828ab,
                    setVisualStateProperty_e4b4395223e3450fb04826f9f30828ab,
                    setLocalVisualStateProperty_e4b4395223e3450fb04826f9f30828ab,
                    getVisualStateProperty_e4b4395223e3450fb04826f9f30828ab));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_966dcdbf7f3940039694ca2f353628ab, Border_618ef8ba03af451087fbc33c3ff886fd);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_968a0218eb264e37b2af66a34af6b43f,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Background",
                    "Background",
                    accessVisualStateProperty_750f3d33ac674ccab0bd2eac596d0d3e,
                    setVisualStateProperty_750f3d33ac674ccab0bd2eac596d0d3e,
                    setLocalVisualStateProperty_750f3d33ac674ccab0bd2eac596d0d3e,
                    getVisualStateProperty_750f3d33ac674ccab0bd2eac596d0d3e));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_968a0218eb264e37b2af66a34af6b43f, Border_618ef8ba03af451087fbc33c3ff886fd);

            templateInstance_523b354795e748a6a0c81ac8bd051b6a.TemplateContent = Border_a87e17151acc48a89a823a0f683e1fed;
            return templateInstance_523b354795e748a6a0c81ac8bd051b6a;
        }

    }
}
#endif