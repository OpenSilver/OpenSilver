
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
    internal class INTERNAL_DefaultExpanderStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_e18d4da2dd3c4bbf8768898bfa851954 = new global::System.Windows.Style();
                Style_e18d4da2dd3c4bbf8768898bfa851954.TargetType = typeof(global::System.Windows.Controls.Expander);
                var Setter_64490ae3891743fe8b043a852565098c = new global::System.Windows.Setter();
                Setter_64490ae3891743fe8b043a852565098c.Property = global::System.Windows.Controls.Expander.TemplateProperty;
                var ControlTemplate_282840830e7944d68a811b527d311e2c = new global::System.Windows.Controls.ControlTemplate();
                ControlTemplate_282840830e7944d68a811b527d311e2c.TargetType = typeof(global::System.Windows.Controls.Expander);
                ControlTemplate_282840830e7944d68a811b527d311e2c.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_282840830e7944d68a811b527d311e2c);

                Setter_64490ae3891743fe8b043a852565098c.Value = ControlTemplate_282840830e7944d68a811b527d311e2c;


                Style_e18d4da2dd3c4bbf8768898bfa851954.Setters.Add(Setter_64490ae3891743fe8b043a852565098c);


                DefaultStyle = Style_e18d4da2dd3c4bbf8768898bfa851954;
            }

            return DefaultStyle;
        }

        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_dbbd711c3837416bbff9f458bf97df01(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_dbbd711c3837416bbff9f458bf97df01(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_dbbd711c3837416bbff9f458bf97df01(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_dbbd711c3837416bbff9f458bf97df01(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_0d25bb136c494f1c814cf265ae6c46a2(global::System.Windows.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_0d25bb136c494f1c814cf265ae6c46a2(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Data").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("DataProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_0d25bb136c494f1c814cf265ae6c46a2(global::System.Windows.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Data").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("DataProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_0d25bb136c494f1c814cf265ae6c46a2(global::System.Windows.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Data").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("DataProperty");
            global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_d55708b383dc41858869da62dd4e36e8(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_58d8310d50a84a90be72820313f75282 = new global::System.Windows.TemplateInstance();
            templateInstance_58d8310d50a84a90be72820313f75282.TemplateOwner = templateOwner;
            var Border_5eac7e0779e74cfb9062f33697e7f325 = new global::System.Windows.Controls.Border();
            var VisualStateGroup_3a8780deef94410db04dd291a0c16db8 = new global::System.Windows.VisualStateGroup();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_3a8780deef94410db04dd291a0c16db8);
            VisualStateGroup_3a8780deef94410db04dd291a0c16db8.Name = "CommonStates";
            var VisualState_80b0591284c849eaac457f3385056973 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Normal", VisualState_80b0591284c849eaac457f3385056973);
            VisualState_80b0591284c849eaac457f3385056973.Name = "Normal";

            var VisualState_21057ca6e38b4fd69728a4f411712738 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Checked", VisualState_21057ca6e38b4fd69728a4f411712738);
            VisualState_21057ca6e38b4fd69728a4f411712738.Name = "Checked";
            var Storyboard_e1aae4577331467db095b74296164842 = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_cddcb3f5596b49979bf7fa3f6a66515a = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            ObjectAnimationUsingKeyFrames_cddcb3f5596b49979bf7fa3f6a66515a.Duration = (global::System.Windows.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Duration), @"0");
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_cddcb3f5596b49979bf7fa3f6a66515a, @"arrow");
            var DiscreteObjectKeyFrame_7baf762f058a4fcbba424a8a1129ce70 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_7baf762f058a4fcbba424a8a1129ce70.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_7baf762f058a4fcbba424a8a1129ce70.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"M 1,1.5 L 4.5,5 L 8,1.5");

            ObjectAnimationUsingKeyFrames_cddcb3f5596b49979bf7fa3f6a66515a.KeyFrames.Add(DiscreteObjectKeyFrame_7baf762f058a4fcbba424a8a1129ce70);


            Storyboard_e1aae4577331467db095b74296164842.Children.Add(ObjectAnimationUsingKeyFrames_cddcb3f5596b49979bf7fa3f6a66515a);


            VisualState_21057ca6e38b4fd69728a4f411712738.Storyboard = Storyboard_e1aae4577331467db095b74296164842;


            VisualStateGroup_3a8780deef94410db04dd291a0c16db8.States.Add(VisualState_80b0591284c849eaac457f3385056973);
            VisualStateGroup_3a8780deef94410db04dd291a0c16db8.States.Add(VisualState_21057ca6e38b4fd69728a4f411712738);


            var VisualStateGroup_478d20513a73449e85051de57480c12a = new global::System.Windows.VisualStateGroup();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("FocusStates", VisualStateGroup_478d20513a73449e85051de57480c12a);
            VisualStateGroup_478d20513a73449e85051de57480c12a.Name = "FocusStates";
            var VisualState_4682fea4a3c9464e8216d47bf0770c24 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Focused", VisualState_4682fea4a3c9464e8216d47bf0770c24);
            VisualState_4682fea4a3c9464e8216d47bf0770c24.Name = "Focused";

            var VisualState_de3c98c088f54efd9849b5c901aebe01 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Unfocused", VisualState_de3c98c088f54efd9849b5c901aebe01);
            VisualState_de3c98c088f54efd9849b5c901aebe01.Name = "Unfocused";

            VisualStateGroup_478d20513a73449e85051de57480c12a.States.Add(VisualState_4682fea4a3c9464e8216d47bf0770c24);
            VisualStateGroup_478d20513a73449e85051de57480c12a.States.Add(VisualState_de3c98c088f54efd9849b5c901aebe01);


            ((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_3a8780deef94410db04dd291a0c16db8);
            ((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_478d20513a73449e85051de57480c12a);

            var StackPanel_750876653e714d01865a28e4997d80f3 = new global::System.Windows.Controls.StackPanel();
            StackPanel_750876653e714d01865a28e4997d80f3.Orientation = global::System.Windows.Controls.Orientation.Horizontal;
            StackPanel_750876653e714d01865a28e4997d80f3.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"5,0,0,0");
            var Path_d6053ffa07f445e4a42d748c99cd47d2 = new global::System.Windows.Shapes.Path();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("arrow", Path_d6053ffa07f445e4a42d748c99cd47d2);
            Path_d6053ffa07f445e4a42d748c99cd47d2.Name = "arrow";
            Path_d6053ffa07f445e4a42d748c99cd47d2.Visibility = global::System.Windows.Visibility.Visible;
            Path_d6053ffa07f445e4a42d748c99cd47d2.Stroke = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FF555555");
            Path_d6053ffa07f445e4a42d748c99cd47d2.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_d6053ffa07f445e4a42d748c99cd47d2.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_d6053ffa07f445e4a42d748c99cd47d2.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0,0,3,0");
            Path_d6053ffa07f445e4a42d748c99cd47d2.StrokeThickness = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"3");
            Path_d6053ffa07f445e4a42d748c99cd47d2.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Center;
            Path_d6053ffa07f445e4a42d748c99cd47d2.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
            Path_d6053ffa07f445e4a42d748c99cd47d2.Stretch = global::System.Windows.Media.Stretch.Fill;
            Path_d6053ffa07f445e4a42d748c99cd47d2.Data = (global::System.Windows.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Geometry), @"M 2,1 L 5.5,4.5 L 2,8");

            var ContentPresenter_daf36d638dc7438082dda8b3d2f9f663 = new global::System.Windows.Controls.ContentPresenter();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("header", ContentPresenter_daf36d638dc7438082dda8b3d2f9f663);
            ContentPresenter_daf36d638dc7438082dda8b3d2f9f663.Name = "header";
            ContentPresenter_daf36d638dc7438082dda8b3d2f9f663.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"4,0,0,0");
            ContentPresenter_daf36d638dc7438082dda8b3d2f9f663.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Stretch;
            ContentPresenter_daf36d638dc7438082dda8b3d2f9f663.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
            var Binding_484ee92ae04a42e3a6140a7f1a5e3fa8 = new global::System.Windows.Data.Binding();
            Binding_484ee92ae04a42e3a6140a7f1a5e3fa8.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Content");
            var RelativeSource_c0a5c2f849244ba4ade85d771c63e35d = new global::System.Windows.Data.RelativeSource();
            RelativeSource_c0a5c2f849244ba4ade85d771c63e35d.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_484ee92ae04a42e3a6140a7f1a5e3fa8.RelativeSource = RelativeSource_c0a5c2f849244ba4ade85d771c63e35d;


            Binding_484ee92ae04a42e3a6140a7f1a5e3fa8.TemplateOwner = templateInstance_58d8310d50a84a90be72820313f75282;

            var Binding_0f38b6d81ee24744be54cd5aaab95809 = new global::System.Windows.Data.Binding();
            Binding_0f38b6d81ee24744be54cd5aaab95809.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"ContentTemplate");
            var RelativeSource_c87f2626fe2648c5b3778675677b78d7 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_c87f2626fe2648c5b3778675677b78d7.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_0f38b6d81ee24744be54cd5aaab95809.RelativeSource = RelativeSource_c87f2626fe2648c5b3778675677b78d7;


            Binding_0f38b6d81ee24744be54cd5aaab95809.TemplateOwner = templateInstance_58d8310d50a84a90be72820313f75282;


            StackPanel_750876653e714d01865a28e4997d80f3.Children.Add(Path_d6053ffa07f445e4a42d748c99cd47d2);
            StackPanel_750876653e714d01865a28e4997d80f3.Children.Add(ContentPresenter_daf36d638dc7438082dda8b3d2f9f663);


            Border_5eac7e0779e74cfb9062f33697e7f325.Child = StackPanel_750876653e714d01865a28e4997d80f3;

            var Binding_e4a7e0753c7146d6b470414b8cf2a2b2 = new global::System.Windows.Data.Binding();
            Binding_e4a7e0753c7146d6b470414b8cf2a2b2.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Padding");
            var RelativeSource_285bdbfc229844c0b4bd6d93e2b6c580 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_285bdbfc229844c0b4bd6d93e2b6c580.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_e4a7e0753c7146d6b470414b8cf2a2b2.RelativeSource = RelativeSource_285bdbfc229844c0b4bd6d93e2b6c580;


            Binding_e4a7e0753c7146d6b470414b8cf2a2b2.TemplateOwner = templateInstance_58d8310d50a84a90be72820313f75282;



            ContentPresenter_daf36d638dc7438082dda8b3d2f9f663.SetBinding(global::System.Windows.Controls.ContentPresenter.ContentProperty, Binding_484ee92ae04a42e3a6140a7f1a5e3fa8);
            ContentPresenter_daf36d638dc7438082dda8b3d2f9f663.SetBinding(global::System.Windows.Controls.ContentPresenter.ContentTemplateProperty, Binding_0f38b6d81ee24744be54cd5aaab95809);
            Border_5eac7e0779e74cfb9062f33697e7f325.SetBinding(global::System.Windows.Controls.Border.PaddingProperty, Binding_e4a7e0753c7146d6b470414b8cf2a2b2);

            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_cddcb3f5596b49979bf7fa3f6a66515a,
                new global::System.Windows.PropertyPath(
                    "Data",
                    "Data",
                    accessVisualStateProperty_0d25bb136c494f1c814cf265ae6c46a2,
                    setVisualStateProperty_0d25bb136c494f1c814cf265ae6c46a2,
                    setLocalVisualStateProperty_0d25bb136c494f1c814cf265ae6c46a2,
                    getVisualStateProperty_0d25bb136c494f1c814cf265ae6c46a2));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_cddcb3f5596b49979bf7fa3f6a66515a, Path_d6053ffa07f445e4a42d748c99cd47d2);

            templateInstance_58d8310d50a84a90be72820313f75282.TemplateContent = Border_5eac7e0779e74cfb9062f33697e7f325;
            return templateInstance_58d8310d50a84a90be72820313f75282;
        }



        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_282840830e7944d68a811b527d311e2c(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_b5e23e08ba2947618b6f2702818d9477 = new global::System.Windows.TemplateInstance();
            templateInstance_b5e23e08ba2947618b6f2702818d9477.TemplateOwner = templateOwner;
            var Border_442cacacc68b4ac28e01ff0719febbc2 = new global::System.Windows.Controls.Border();
            Border_442cacacc68b4ac28e01ff0719febbc2.CornerRadius = (global::System.Windows.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.CornerRadius), @"3");
            var VisualStateGroup_438bab165d4344a18b73ed6bd4315a03 = new global::System.Windows.VisualStateGroup();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_438bab165d4344a18b73ed6bd4315a03);
            VisualStateGroup_438bab165d4344a18b73ed6bd4315a03.Name = "CommonStates";
            var VisualState_25a376f5ff0e45569ec353e686645358 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Normal", VisualState_25a376f5ff0e45569ec353e686645358);
            VisualState_25a376f5ff0e45569ec353e686645358.Name = "Normal";

            var VisualState_44b2f2e73bb543f693ddcf1ce8ddf264 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_44b2f2e73bb543f693ddcf1ce8ddf264);
            VisualState_44b2f2e73bb543f693ddcf1ce8ddf264.Name = "Disabled";

            VisualStateGroup_438bab165d4344a18b73ed6bd4315a03.States.Add(VisualState_25a376f5ff0e45569ec353e686645358);
            VisualStateGroup_438bab165d4344a18b73ed6bd4315a03.States.Add(VisualState_44b2f2e73bb543f693ddcf1ce8ddf264);


            var VisualStateGroup_68f1955695cd4ce2b29a3d07070bd5a0 = new global::System.Windows.VisualStateGroup();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("FocusStates", VisualStateGroup_68f1955695cd4ce2b29a3d07070bd5a0);
            VisualStateGroup_68f1955695cd4ce2b29a3d07070bd5a0.Name = "FocusStates";
            var VisualState_ad10dc625e0744e78367516e015c422c = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Focused", VisualState_ad10dc625e0744e78367516e015c422c);
            VisualState_ad10dc625e0744e78367516e015c422c.Name = "Focused";

            var VisualState_369940c29243416789800e3ec0b72695 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Unfocused", VisualState_369940c29243416789800e3ec0b72695);
            VisualState_369940c29243416789800e3ec0b72695.Name = "Unfocused";

            VisualStateGroup_68f1955695cd4ce2b29a3d07070bd5a0.States.Add(VisualState_ad10dc625e0744e78367516e015c422c);
            VisualStateGroup_68f1955695cd4ce2b29a3d07070bd5a0.States.Add(VisualState_369940c29243416789800e3ec0b72695);


            var VisualStateGroup_09c325b83f374fa0870593e89dbe5c64 = new global::System.Windows.VisualStateGroup();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("ExpansionStates", VisualStateGroup_09c325b83f374fa0870593e89dbe5c64);
            VisualStateGroup_09c325b83f374fa0870593e89dbe5c64.Name = "ExpansionStates";
            var VisualState_6f1c838616ac4c4f93268e269fabfc27 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Collapsed", VisualState_6f1c838616ac4c4f93268e269fabfc27);
            VisualState_6f1c838616ac4c4f93268e269fabfc27.Name = "Collapsed";

            var VisualState_22dee1c0e8c046b4bdddae4a78900e72 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Expanded", VisualState_22dee1c0e8c046b4bdddae4a78900e72);
            VisualState_22dee1c0e8c046b4bdddae4a78900e72.Name = "Expanded";
            var Storyboard_983a186d9f2f48d699f0c7ff29e16ba8 = new global::System.Windows.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_75001fba46a34df1b1dbaf16a38655d7 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
            ObjectAnimationUsingKeyFrames_75001fba46a34df1b1dbaf16a38655d7.Duration = (global::System.Windows.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Duration), @"0");
            global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_75001fba46a34df1b1dbaf16a38655d7, @"ExpandSite");
            var DiscreteObjectKeyFrame_c2818a666f5742ef93c792daa1f47d4f = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_c2818a666f5742ef93c792daa1f47d4f.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_c2818a666f5742ef93c792daa1f47d4f.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Visible");

            ObjectAnimationUsingKeyFrames_75001fba46a34df1b1dbaf16a38655d7.KeyFrames.Add(DiscreteObjectKeyFrame_c2818a666f5742ef93c792daa1f47d4f);


            Storyboard_983a186d9f2f48d699f0c7ff29e16ba8.Children.Add(ObjectAnimationUsingKeyFrames_75001fba46a34df1b1dbaf16a38655d7);


            VisualState_22dee1c0e8c046b4bdddae4a78900e72.Storyboard = Storyboard_983a186d9f2f48d699f0c7ff29e16ba8;


            VisualStateGroup_09c325b83f374fa0870593e89dbe5c64.States.Add(VisualState_6f1c838616ac4c4f93268e269fabfc27);
            VisualStateGroup_09c325b83f374fa0870593e89dbe5c64.States.Add(VisualState_22dee1c0e8c046b4bdddae4a78900e72);


            ((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_438bab165d4344a18b73ed6bd4315a03);
            ((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_68f1955695cd4ce2b29a3d07070bd5a0);
            ((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_09c325b83f374fa0870593e89dbe5c64);

            var StackPanel_2788134dad9b42ad95097c9e2da93890 = new global::System.Windows.Controls.StackPanel();
            var ToggleButton_e8e72dc63a814ec8a0035e766f516955 = new global::System.Windows.Controls.Primitives.ToggleButton();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("ExpanderButton", ToggleButton_e8e72dc63a814ec8a0035e766f516955);
            ToggleButton_e8e72dc63a814ec8a0035e766f516955.Name = "ExpanderButton";
            ToggleButton_e8e72dc63a814ec8a0035e766f516955.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");
            ToggleButton_e8e72dc63a814ec8a0035e766f516955.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Hand");
            var ControlTemplate_d55708b383dc41858869da62dd4e36e8 = new global::System.Windows.Controls.ControlTemplate();
            ControlTemplate_d55708b383dc41858869da62dd4e36e8.TargetType = typeof(global::System.Windows.Controls.Primitives.ToggleButton);
            ControlTemplate_d55708b383dc41858869da62dd4e36e8.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_d55708b383dc41858869da62dd4e36e8);

            ToggleButton_e8e72dc63a814ec8a0035e766f516955.Template = ControlTemplate_d55708b383dc41858869da62dd4e36e8;

            var Binding_d708842f2790486fbe698a11ff17e661 = new global::System.Windows.Data.Binding();
            Binding_d708842f2790486fbe698a11ff17e661.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Padding");
            var RelativeSource_5ea5cff268fe4628b017e608d12202c5 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_5ea5cff268fe4628b017e608d12202c5.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_d708842f2790486fbe698a11ff17e661.RelativeSource = RelativeSource_5ea5cff268fe4628b017e608d12202c5;


            Binding_d708842f2790486fbe698a11ff17e661.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;

            var Binding_e890fddf6f414776a0ee9cad8a3c4c27 = new global::System.Windows.Data.Binding();
            Binding_e890fddf6f414776a0ee9cad8a3c4c27.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Header");
            var RelativeSource_03c279b26b954458a6dc61fa13f86171 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_03c279b26b954458a6dc61fa13f86171.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_e890fddf6f414776a0ee9cad8a3c4c27.RelativeSource = RelativeSource_03c279b26b954458a6dc61fa13f86171;


            Binding_e890fddf6f414776a0ee9cad8a3c4c27.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;

            var Binding_0c4a42e9d6e843ce850f6f6c0b859ec2 = new global::System.Windows.Data.Binding();
            Binding_0c4a42e9d6e843ce850f6f6c0b859ec2.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"HeaderTemplate");
            var RelativeSource_bce73f367d4348d599de27212ca51896 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_bce73f367d4348d599de27212ca51896.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_0c4a42e9d6e843ce850f6f6c0b859ec2.RelativeSource = RelativeSource_bce73f367d4348d599de27212ca51896;


            Binding_0c4a42e9d6e843ce850f6f6c0b859ec2.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;


            var ContentControl_46243c6a201241c39b3f8d385fda3f80 = new global::System.Windows.Controls.ContentControl();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("ExpandSite", ContentControl_46243c6a201241c39b3f8d385fda3f80);
            ContentControl_46243c6a201241c39b3f8d385fda3f80.Name = "ExpandSite";
            ContentControl_46243c6a201241c39b3f8d385fda3f80.Visibility = global::System.Windows.Visibility.Collapsed;
            var Binding_55f6db0f11f848609f0c39be26b080db = new global::System.Windows.Data.Binding();
            Binding_55f6db0f11f848609f0c39be26b080db.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Padding");
            var RelativeSource_29bb11d9a9674b8db02db8f65a715189 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_29bb11d9a9674b8db02db8f65a715189.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_55f6db0f11f848609f0c39be26b080db.RelativeSource = RelativeSource_29bb11d9a9674b8db02db8f65a715189;


            Binding_55f6db0f11f848609f0c39be26b080db.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;

            var Binding_7d7bad8fb36d4302af994b2e53858f0b = new global::System.Windows.Data.Binding();
            Binding_7d7bad8fb36d4302af994b2e53858f0b.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Content");
            var RelativeSource_034908797e92484d83fc3766ea76e187 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_034908797e92484d83fc3766ea76e187.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_7d7bad8fb36d4302af994b2e53858f0b.RelativeSource = RelativeSource_034908797e92484d83fc3766ea76e187;


            Binding_7d7bad8fb36d4302af994b2e53858f0b.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;

            var Binding_3df7b20233be44bfae58a56d02f7191d = new global::System.Windows.Data.Binding();
            Binding_3df7b20233be44bfae58a56d02f7191d.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"ContentTemplate");
            var RelativeSource_dff97c9756a346ca972866a951a04100 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_dff97c9756a346ca972866a951a04100.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_3df7b20233be44bfae58a56d02f7191d.RelativeSource = RelativeSource_dff97c9756a346ca972866a951a04100;


            Binding_3df7b20233be44bfae58a56d02f7191d.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;


            StackPanel_2788134dad9b42ad95097c9e2da93890.Children.Add(ToggleButton_e8e72dc63a814ec8a0035e766f516955);
            StackPanel_2788134dad9b42ad95097c9e2da93890.Children.Add(ContentControl_46243c6a201241c39b3f8d385fda3f80);


            Border_442cacacc68b4ac28e01ff0719febbc2.Child = StackPanel_2788134dad9b42ad95097c9e2da93890;

            var Binding_d02694e0ade448e8b5ff3abeaeda8542 = new global::System.Windows.Data.Binding();
            Binding_d02694e0ade448e8b5ff3abeaeda8542.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_4d1e6af5065844159beae248718b796b = new global::System.Windows.Data.RelativeSource();
            RelativeSource_4d1e6af5065844159beae248718b796b.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_d02694e0ade448e8b5ff3abeaeda8542.RelativeSource = RelativeSource_4d1e6af5065844159beae248718b796b;


            Binding_d02694e0ade448e8b5ff3abeaeda8542.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;

            var Binding_12b7eec25cbe45709bccbc1c74061b5b = new global::System.Windows.Data.Binding();
            Binding_12b7eec25cbe45709bccbc1c74061b5b.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
            var RelativeSource_f54f1a8020324090a123056a9e230652 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_f54f1a8020324090a123056a9e230652.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_12b7eec25cbe45709bccbc1c74061b5b.RelativeSource = RelativeSource_f54f1a8020324090a123056a9e230652;


            Binding_12b7eec25cbe45709bccbc1c74061b5b.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;

            var Binding_89b2106919c6465e85b3ea5bf1390bae = new global::System.Windows.Data.Binding();
            Binding_89b2106919c6465e85b3ea5bf1390bae.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
            var RelativeSource_9c5248b5bc9e4953bb15e5af4b054611 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_9c5248b5bc9e4953bb15e5af4b054611.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_89b2106919c6465e85b3ea5bf1390bae.RelativeSource = RelativeSource_9c5248b5bc9e4953bb15e5af4b054611;


            Binding_89b2106919c6465e85b3ea5bf1390bae.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;



            ToggleButton_e8e72dc63a814ec8a0035e766f516955.SetBinding(global::System.Windows.Controls.Control.PaddingProperty, Binding_d708842f2790486fbe698a11ff17e661);
            ToggleButton_e8e72dc63a814ec8a0035e766f516955.SetBinding(global::System.Windows.Controls.ContentControl.ContentProperty, Binding_e890fddf6f414776a0ee9cad8a3c4c27);
            ToggleButton_e8e72dc63a814ec8a0035e766f516955.SetBinding(global::System.Windows.Controls.ContentControl.ContentTemplateProperty, Binding_0c4a42e9d6e843ce850f6f6c0b859ec2);
            ContentControl_46243c6a201241c39b3f8d385fda3f80.SetBinding(global::System.Windows.FrameworkElement.MarginProperty, Binding_55f6db0f11f848609f0c39be26b080db);
            ContentControl_46243c6a201241c39b3f8d385fda3f80.SetBinding(global::System.Windows.Controls.ContentControl.ContentProperty, Binding_7d7bad8fb36d4302af994b2e53858f0b);
            ContentControl_46243c6a201241c39b3f8d385fda3f80.SetBinding(global::System.Windows.Controls.ContentControl.ContentTemplateProperty, Binding_3df7b20233be44bfae58a56d02f7191d);
            Border_442cacacc68b4ac28e01ff0719febbc2.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_d02694e0ade448e8b5ff3abeaeda8542);
            Border_442cacacc68b4ac28e01ff0719febbc2.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_12b7eec25cbe45709bccbc1c74061b5b);
            Border_442cacacc68b4ac28e01ff0719febbc2.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_89b2106919c6465e85b3ea5bf1390bae);

            global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_75001fba46a34df1b1dbaf16a38655d7,
                new global::System.Windows.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_dbbd711c3837416bbff9f458bf97df01,
                    setVisualStateProperty_dbbd711c3837416bbff9f458bf97df01,
                    setLocalVisualStateProperty_dbbd711c3837416bbff9f458bf97df01,
                    getVisualStateProperty_dbbd711c3837416bbff9f458bf97df01));
            global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_75001fba46a34df1b1dbaf16a38655d7, ContentControl_46243c6a201241c39b3f8d385fda3f80);

            templateInstance_b5e23e08ba2947618b6f2702818d9477.TemplateContent = Border_442cacacc68b4ac28e01ff0719febbc2;
            return templateInstance_b5e23e08ba2947618b6f2702818d9477;
        }



    }
}
#else
namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultExpanderStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_e18d4da2dd3c4bbf8768898bfa851954 = new global::Windows.UI.Xaml.Style();
                Style_e18d4da2dd3c4bbf8768898bfa851954.TargetType = typeof(global::Windows.UI.Xaml.Controls.Expander);
                var Setter_64490ae3891743fe8b043a852565098c = new global::Windows.UI.Xaml.Setter();
                Setter_64490ae3891743fe8b043a852565098c.Property = global::Windows.UI.Xaml.Controls.Expander.TemplateProperty;
                var ControlTemplate_282840830e7944d68a811b527d311e2c = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_282840830e7944d68a811b527d311e2c.TargetType = typeof(global::Windows.UI.Xaml.Controls.Expander);
                ControlTemplate_282840830e7944d68a811b527d311e2c.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_282840830e7944d68a811b527d311e2c);

                Setter_64490ae3891743fe8b043a852565098c.Value = ControlTemplate_282840830e7944d68a811b527d311e2c;


                Style_e18d4da2dd3c4bbf8768898bfa851954.Setters.Add(Setter_64490ae3891743fe8b043a852565098c);


                DefaultStyle = Style_e18d4da2dd3c4bbf8768898bfa851954;
            }
            return DefaultStyle;
        }

        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_dbbd711c3837416bbff9f458bf97df01(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_dbbd711c3837416bbff9f458bf97df01(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_dbbd711c3837416bbff9f458bf97df01(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }

        public static global::System.Object getVisualStateProperty_dbbd711c3837416bbff9f458bf97df01(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_0d25bb136c494f1c814cf265ae6c46a2(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_0d25bb136c494f1c814cf265ae6c46a2(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Data").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("DataProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }

        public static void setLocalVisualStateProperty_0d25bb136c494f1c814cf265ae6c46a2(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Data").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("DataProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_0d25bb136c494f1c814cf265ae6c46a2(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Data").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("DataProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_d55708b383dc41858869da62dd4e36e8(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_58d8310d50a84a90be72820313f75282 = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_58d8310d50a84a90be72820313f75282.TemplateOwner = templateOwner;
            var Border_5eac7e0779e74cfb9062f33697e7f325 = new global::Windows.UI.Xaml.Controls.Border();
            var VisualStateGroup_3a8780deef94410db04dd291a0c16db8 = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_3a8780deef94410db04dd291a0c16db8);
            VisualStateGroup_3a8780deef94410db04dd291a0c16db8.Name = "CommonStates";
            var VisualState_80b0591284c849eaac457f3385056973 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Normal", VisualState_80b0591284c849eaac457f3385056973);
            VisualState_80b0591284c849eaac457f3385056973.Name = "Normal";

            var VisualState_21057ca6e38b4fd69728a4f411712738 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Checked", VisualState_21057ca6e38b4fd69728a4f411712738);
            VisualState_21057ca6e38b4fd69728a4f411712738.Name = "Checked";
            var Storyboard_e1aae4577331467db095b74296164842 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_cddcb3f5596b49979bf7fa3f6a66515a = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            ObjectAnimationUsingKeyFrames_cddcb3f5596b49979bf7fa3f6a66515a.Duration = (global::Windows.UI.Xaml.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Duration), @"0");
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_cddcb3f5596b49979bf7fa3f6a66515a, @"arrow");
            var DiscreteObjectKeyFrame_7baf762f058a4fcbba424a8a1129ce70 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_7baf762f058a4fcbba424a8a1129ce70.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_7baf762f058a4fcbba424a8a1129ce70.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"M 1,1.5 L 4.5,5 L 8,1.5");

            ObjectAnimationUsingKeyFrames_cddcb3f5596b49979bf7fa3f6a66515a.KeyFrames.Add(DiscreteObjectKeyFrame_7baf762f058a4fcbba424a8a1129ce70);


            Storyboard_e1aae4577331467db095b74296164842.Children.Add(ObjectAnimationUsingKeyFrames_cddcb3f5596b49979bf7fa3f6a66515a);


            VisualState_21057ca6e38b4fd69728a4f411712738.Storyboard = Storyboard_e1aae4577331467db095b74296164842;


            VisualStateGroup_3a8780deef94410db04dd291a0c16db8.States.Add(VisualState_80b0591284c849eaac457f3385056973);
            VisualStateGroup_3a8780deef94410db04dd291a0c16db8.States.Add(VisualState_21057ca6e38b4fd69728a4f411712738);


            var VisualStateGroup_478d20513a73449e85051de57480c12a = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("FocusStates", VisualStateGroup_478d20513a73449e85051de57480c12a);
            VisualStateGroup_478d20513a73449e85051de57480c12a.Name = "FocusStates";
            var VisualState_4682fea4a3c9464e8216d47bf0770c24 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Focused", VisualState_4682fea4a3c9464e8216d47bf0770c24);
            VisualState_4682fea4a3c9464e8216d47bf0770c24.Name = "Focused";

            var VisualState_de3c98c088f54efd9849b5c901aebe01 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Unfocused", VisualState_de3c98c088f54efd9849b5c901aebe01);
            VisualState_de3c98c088f54efd9849b5c901aebe01.Name = "Unfocused";

            VisualStateGroup_478d20513a73449e85051de57480c12a.States.Add(VisualState_4682fea4a3c9464e8216d47bf0770c24);
            VisualStateGroup_478d20513a73449e85051de57480c12a.States.Add(VisualState_de3c98c088f54efd9849b5c901aebe01);


            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_3a8780deef94410db04dd291a0c16db8);
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_478d20513a73449e85051de57480c12a);

            var StackPanel_750876653e714d01865a28e4997d80f3 = new global::Windows.UI.Xaml.Controls.StackPanel();
            StackPanel_750876653e714d01865a28e4997d80f3.Orientation = global::Windows.UI.Xaml.Controls.Orientation.Horizontal;
            StackPanel_750876653e714d01865a28e4997d80f3.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"5,0,0,0");
            var Path_d6053ffa07f445e4a42d748c99cd47d2 = new global::Windows.UI.Xaml.Shapes.Path();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("arrow", Path_d6053ffa07f445e4a42d748c99cd47d2);
            Path_d6053ffa07f445e4a42d748c99cd47d2.Name = "arrow";
            Path_d6053ffa07f445e4a42d748c99cd47d2.Visibility = global::Windows.UI.Xaml.Visibility.Visible;
            Path_d6053ffa07f445e4a42d748c99cd47d2.Stroke = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FF555555");
            Path_d6053ffa07f445e4a42d748c99cd47d2.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_d6053ffa07f445e4a42d748c99cd47d2.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_d6053ffa07f445e4a42d748c99cd47d2.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0,0,3,0");
            Path_d6053ffa07f445e4a42d748c99cd47d2.StrokeThickness = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"3");
            Path_d6053ffa07f445e4a42d748c99cd47d2.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center;
            Path_d6053ffa07f445e4a42d748c99cd47d2.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            Path_d6053ffa07f445e4a42d748c99cd47d2.Stretch = global::Windows.UI.Xaml.Media.Stretch.Fill;
            Path_d6053ffa07f445e4a42d748c99cd47d2.Data = (global::Windows.UI.Xaml.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Geometry), @"M 2,1 L 5.5,4.5 L 2,8");

            var ContentPresenter_daf36d638dc7438082dda8b3d2f9f663 = new global::Windows.UI.Xaml.Controls.ContentPresenter();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("header", ContentPresenter_daf36d638dc7438082dda8b3d2f9f663);
            ContentPresenter_daf36d638dc7438082dda8b3d2f9f663.Name = "header";
            ContentPresenter_daf36d638dc7438082dda8b3d2f9f663.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"4,0,0,0");
            ContentPresenter_daf36d638dc7438082dda8b3d2f9f663.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch;
            ContentPresenter_daf36d638dc7438082dda8b3d2f9f663.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            var Binding_484ee92ae04a42e3a6140a7f1a5e3fa8 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_484ee92ae04a42e3a6140a7f1a5e3fa8.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Content");
            var RelativeSource_c0a5c2f849244ba4ade85d771c63e35d = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_c0a5c2f849244ba4ade85d771c63e35d.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_484ee92ae04a42e3a6140a7f1a5e3fa8.RelativeSource = RelativeSource_c0a5c2f849244ba4ade85d771c63e35d;


            Binding_484ee92ae04a42e3a6140a7f1a5e3fa8.TemplateOwner = templateInstance_58d8310d50a84a90be72820313f75282;

            var Binding_0f38b6d81ee24744be54cd5aaab95809 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_0f38b6d81ee24744be54cd5aaab95809.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"ContentTemplate");
            var RelativeSource_c87f2626fe2648c5b3778675677b78d7 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_c87f2626fe2648c5b3778675677b78d7.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_0f38b6d81ee24744be54cd5aaab95809.RelativeSource = RelativeSource_c87f2626fe2648c5b3778675677b78d7;


            Binding_0f38b6d81ee24744be54cd5aaab95809.TemplateOwner = templateInstance_58d8310d50a84a90be72820313f75282;


            StackPanel_750876653e714d01865a28e4997d80f3.Children.Add(Path_d6053ffa07f445e4a42d748c99cd47d2);
            StackPanel_750876653e714d01865a28e4997d80f3.Children.Add(ContentPresenter_daf36d638dc7438082dda8b3d2f9f663);


            Border_5eac7e0779e74cfb9062f33697e7f325.Child = StackPanel_750876653e714d01865a28e4997d80f3;

            var Binding_e4a7e0753c7146d6b470414b8cf2a2b2 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_e4a7e0753c7146d6b470414b8cf2a2b2.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Padding");
            var RelativeSource_285bdbfc229844c0b4bd6d93e2b6c580 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_285bdbfc229844c0b4bd6d93e2b6c580.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_e4a7e0753c7146d6b470414b8cf2a2b2.RelativeSource = RelativeSource_285bdbfc229844c0b4bd6d93e2b6c580;


            Binding_e4a7e0753c7146d6b470414b8cf2a2b2.TemplateOwner = templateInstance_58d8310d50a84a90be72820313f75282;



            ContentPresenter_daf36d638dc7438082dda8b3d2f9f663.SetBinding(global::Windows.UI.Xaml.Controls.ContentPresenter.ContentProperty, Binding_484ee92ae04a42e3a6140a7f1a5e3fa8);
            ContentPresenter_daf36d638dc7438082dda8b3d2f9f663.SetBinding(global::Windows.UI.Xaml.Controls.ContentPresenter.ContentTemplateProperty, Binding_0f38b6d81ee24744be54cd5aaab95809);
            Border_5eac7e0779e74cfb9062f33697e7f325.SetBinding(global::Windows.UI.Xaml.Controls.Border.PaddingProperty, Binding_e4a7e0753c7146d6b470414b8cf2a2b2);

            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_cddcb3f5596b49979bf7fa3f6a66515a,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Data",
                    "Data",
                    accessVisualStateProperty_0d25bb136c494f1c814cf265ae6c46a2,
                    setVisualStateProperty_0d25bb136c494f1c814cf265ae6c46a2,
                    setLocalVisualStateProperty_0d25bb136c494f1c814cf265ae6c46a2,
                    getVisualStateProperty_0d25bb136c494f1c814cf265ae6c46a2));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_cddcb3f5596b49979bf7fa3f6a66515a, Path_d6053ffa07f445e4a42d748c99cd47d2);

            templateInstance_58d8310d50a84a90be72820313f75282.TemplateContent = Border_5eac7e0779e74cfb9062f33697e7f325;
            return templateInstance_58d8310d50a84a90be72820313f75282;
        }



        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_282840830e7944d68a811b527d311e2c(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_b5e23e08ba2947618b6f2702818d9477 = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_b5e23e08ba2947618b6f2702818d9477.TemplateOwner = templateOwner;
            var Border_442cacacc68b4ac28e01ff0719febbc2 = new global::Windows.UI.Xaml.Controls.Border();
            Border_442cacacc68b4ac28e01ff0719febbc2.CornerRadius = (global::Windows.UI.Xaml.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.CornerRadius), @"3");
            var VisualStateGroup_438bab165d4344a18b73ed6bd4315a03 = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_438bab165d4344a18b73ed6bd4315a03);
            VisualStateGroup_438bab165d4344a18b73ed6bd4315a03.Name = "CommonStates";
            var VisualState_25a376f5ff0e45569ec353e686645358 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Normal", VisualState_25a376f5ff0e45569ec353e686645358);
            VisualState_25a376f5ff0e45569ec353e686645358.Name = "Normal";

            var VisualState_44b2f2e73bb543f693ddcf1ce8ddf264 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_44b2f2e73bb543f693ddcf1ce8ddf264);
            VisualState_44b2f2e73bb543f693ddcf1ce8ddf264.Name = "Disabled";

            VisualStateGroup_438bab165d4344a18b73ed6bd4315a03.States.Add(VisualState_25a376f5ff0e45569ec353e686645358);
            VisualStateGroup_438bab165d4344a18b73ed6bd4315a03.States.Add(VisualState_44b2f2e73bb543f693ddcf1ce8ddf264);


            var VisualStateGroup_68f1955695cd4ce2b29a3d07070bd5a0 = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("FocusStates", VisualStateGroup_68f1955695cd4ce2b29a3d07070bd5a0);
            VisualStateGroup_68f1955695cd4ce2b29a3d07070bd5a0.Name = "FocusStates";
            var VisualState_ad10dc625e0744e78367516e015c422c = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Focused", VisualState_ad10dc625e0744e78367516e015c422c);
            VisualState_ad10dc625e0744e78367516e015c422c.Name = "Focused";

            var VisualState_369940c29243416789800e3ec0b72695 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Unfocused", VisualState_369940c29243416789800e3ec0b72695);
            VisualState_369940c29243416789800e3ec0b72695.Name = "Unfocused";

            VisualStateGroup_68f1955695cd4ce2b29a3d07070bd5a0.States.Add(VisualState_ad10dc625e0744e78367516e015c422c);
            VisualStateGroup_68f1955695cd4ce2b29a3d07070bd5a0.States.Add(VisualState_369940c29243416789800e3ec0b72695);


            var VisualStateGroup_09c325b83f374fa0870593e89dbe5c64 = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ExpansionStates", VisualStateGroup_09c325b83f374fa0870593e89dbe5c64);
            VisualStateGroup_09c325b83f374fa0870593e89dbe5c64.Name = "ExpansionStates";
            var VisualState_6f1c838616ac4c4f93268e269fabfc27 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Collapsed", VisualState_6f1c838616ac4c4f93268e269fabfc27);
            VisualState_6f1c838616ac4c4f93268e269fabfc27.Name = "Collapsed";

            var VisualState_22dee1c0e8c046b4bdddae4a78900e72 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Expanded", VisualState_22dee1c0e8c046b4bdddae4a78900e72);
            VisualState_22dee1c0e8c046b4bdddae4a78900e72.Name = "Expanded";
            var Storyboard_983a186d9f2f48d699f0c7ff29e16ba8 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_75001fba46a34df1b1dbaf16a38655d7 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            ObjectAnimationUsingKeyFrames_75001fba46a34df1b1dbaf16a38655d7.Duration = (global::Windows.UI.Xaml.Duration)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Duration), @"0");
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_75001fba46a34df1b1dbaf16a38655d7, @"ExpandSite");
            var DiscreteObjectKeyFrame_c2818a666f5742ef93c792daa1f47d4f = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_c2818a666f5742ef93c792daa1f47d4f.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_c2818a666f5742ef93c792daa1f47d4f.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Visible");

            ObjectAnimationUsingKeyFrames_75001fba46a34df1b1dbaf16a38655d7.KeyFrames.Add(DiscreteObjectKeyFrame_c2818a666f5742ef93c792daa1f47d4f);


            Storyboard_983a186d9f2f48d699f0c7ff29e16ba8.Children.Add(ObjectAnimationUsingKeyFrames_75001fba46a34df1b1dbaf16a38655d7);


            VisualState_22dee1c0e8c046b4bdddae4a78900e72.Storyboard = Storyboard_983a186d9f2f48d699f0c7ff29e16ba8;


            VisualStateGroup_09c325b83f374fa0870593e89dbe5c64.States.Add(VisualState_6f1c838616ac4c4f93268e269fabfc27);
            VisualStateGroup_09c325b83f374fa0870593e89dbe5c64.States.Add(VisualState_22dee1c0e8c046b4bdddae4a78900e72);


            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_438bab165d4344a18b73ed6bd4315a03);
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_68f1955695cd4ce2b29a3d07070bd5a0);
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_09c325b83f374fa0870593e89dbe5c64);

            var StackPanel_2788134dad9b42ad95097c9e2da93890 = new global::Windows.UI.Xaml.Controls.StackPanel();
            var ToggleButton_e8e72dc63a814ec8a0035e766f516955 = new global::Windows.UI.Xaml.Controls.Primitives.ToggleButton();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ExpanderButton", ToggleButton_e8e72dc63a814ec8a0035e766f516955);
            ToggleButton_e8e72dc63a814ec8a0035e766f516955.Name = "ExpanderButton";
            ToggleButton_e8e72dc63a814ec8a0035e766f516955.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");
            ToggleButton_e8e72dc63a814ec8a0035e766f516955.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Hand");
            var ControlTemplate_d55708b383dc41858869da62dd4e36e8 = new global::Windows.UI.Xaml.Controls.ControlTemplate();
            ControlTemplate_d55708b383dc41858869da62dd4e36e8.TargetType = typeof(global::Windows.UI.Xaml.Controls.Primitives.ToggleButton);
            ControlTemplate_d55708b383dc41858869da62dd4e36e8.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_d55708b383dc41858869da62dd4e36e8);

            ToggleButton_e8e72dc63a814ec8a0035e766f516955.Template = ControlTemplate_d55708b383dc41858869da62dd4e36e8;

            var Binding_d708842f2790486fbe698a11ff17e661 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_d708842f2790486fbe698a11ff17e661.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Padding");
            var RelativeSource_5ea5cff268fe4628b017e608d12202c5 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_5ea5cff268fe4628b017e608d12202c5.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_d708842f2790486fbe698a11ff17e661.RelativeSource = RelativeSource_5ea5cff268fe4628b017e608d12202c5;


            Binding_d708842f2790486fbe698a11ff17e661.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;

            var Binding_e890fddf6f414776a0ee9cad8a3c4c27 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_e890fddf6f414776a0ee9cad8a3c4c27.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Header");
            var RelativeSource_03c279b26b954458a6dc61fa13f86171 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_03c279b26b954458a6dc61fa13f86171.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_e890fddf6f414776a0ee9cad8a3c4c27.RelativeSource = RelativeSource_03c279b26b954458a6dc61fa13f86171;


            Binding_e890fddf6f414776a0ee9cad8a3c4c27.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;

            var Binding_0c4a42e9d6e843ce850f6f6c0b859ec2 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_0c4a42e9d6e843ce850f6f6c0b859ec2.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"HeaderTemplate");
            var RelativeSource_bce73f367d4348d599de27212ca51896 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_bce73f367d4348d599de27212ca51896.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_0c4a42e9d6e843ce850f6f6c0b859ec2.RelativeSource = RelativeSource_bce73f367d4348d599de27212ca51896;


            Binding_0c4a42e9d6e843ce850f6f6c0b859ec2.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;


            var ContentControl_46243c6a201241c39b3f8d385fda3f80 = new global::Windows.UI.Xaml.Controls.ContentControl();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ExpandSite", ContentControl_46243c6a201241c39b3f8d385fda3f80);
            ContentControl_46243c6a201241c39b3f8d385fda3f80.Name = "ExpandSite";
            ContentControl_46243c6a201241c39b3f8d385fda3f80.Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
            var Binding_55f6db0f11f848609f0c39be26b080db = new global::Windows.UI.Xaml.Data.Binding();
            Binding_55f6db0f11f848609f0c39be26b080db.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Padding");
            var RelativeSource_29bb11d9a9674b8db02db8f65a715189 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_29bb11d9a9674b8db02db8f65a715189.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_55f6db0f11f848609f0c39be26b080db.RelativeSource = RelativeSource_29bb11d9a9674b8db02db8f65a715189;


            Binding_55f6db0f11f848609f0c39be26b080db.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;

            var Binding_7d7bad8fb36d4302af994b2e53858f0b = new global::Windows.UI.Xaml.Data.Binding();
            Binding_7d7bad8fb36d4302af994b2e53858f0b.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Content");
            var RelativeSource_034908797e92484d83fc3766ea76e187 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_034908797e92484d83fc3766ea76e187.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_7d7bad8fb36d4302af994b2e53858f0b.RelativeSource = RelativeSource_034908797e92484d83fc3766ea76e187;


            Binding_7d7bad8fb36d4302af994b2e53858f0b.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;

            var Binding_3df7b20233be44bfae58a56d02f7191d = new global::Windows.UI.Xaml.Data.Binding();
            Binding_3df7b20233be44bfae58a56d02f7191d.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"ContentTemplate");
            var RelativeSource_dff97c9756a346ca972866a951a04100 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_dff97c9756a346ca972866a951a04100.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_3df7b20233be44bfae58a56d02f7191d.RelativeSource = RelativeSource_dff97c9756a346ca972866a951a04100;


            Binding_3df7b20233be44bfae58a56d02f7191d.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;


            StackPanel_2788134dad9b42ad95097c9e2da93890.Children.Add(ToggleButton_e8e72dc63a814ec8a0035e766f516955);
            StackPanel_2788134dad9b42ad95097c9e2da93890.Children.Add(ContentControl_46243c6a201241c39b3f8d385fda3f80);


            Border_442cacacc68b4ac28e01ff0719febbc2.Child = StackPanel_2788134dad9b42ad95097c9e2da93890;

            var Binding_d02694e0ade448e8b5ff3abeaeda8542 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_d02694e0ade448e8b5ff3abeaeda8542.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_4d1e6af5065844159beae248718b796b = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_4d1e6af5065844159beae248718b796b.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_d02694e0ade448e8b5ff3abeaeda8542.RelativeSource = RelativeSource_4d1e6af5065844159beae248718b796b;


            Binding_d02694e0ade448e8b5ff3abeaeda8542.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;

            var Binding_12b7eec25cbe45709bccbc1c74061b5b = new global::Windows.UI.Xaml.Data.Binding();
            Binding_12b7eec25cbe45709bccbc1c74061b5b.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_f54f1a8020324090a123056a9e230652 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_f54f1a8020324090a123056a9e230652.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_12b7eec25cbe45709bccbc1c74061b5b.RelativeSource = RelativeSource_f54f1a8020324090a123056a9e230652;


            Binding_12b7eec25cbe45709bccbc1c74061b5b.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;

            var Binding_89b2106919c6465e85b3ea5bf1390bae = new global::Windows.UI.Xaml.Data.Binding();
            Binding_89b2106919c6465e85b3ea5bf1390bae.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_9c5248b5bc9e4953bb15e5af4b054611 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_9c5248b5bc9e4953bb15e5af4b054611.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_89b2106919c6465e85b3ea5bf1390bae.RelativeSource = RelativeSource_9c5248b5bc9e4953bb15e5af4b054611;


            Binding_89b2106919c6465e85b3ea5bf1390bae.TemplateOwner = templateInstance_b5e23e08ba2947618b6f2702818d9477;



            ToggleButton_e8e72dc63a814ec8a0035e766f516955.SetBinding(global::Windows.UI.Xaml.Controls.Control.PaddingProperty, Binding_d708842f2790486fbe698a11ff17e661);
            ToggleButton_e8e72dc63a814ec8a0035e766f516955.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentProperty, Binding_e890fddf6f414776a0ee9cad8a3c4c27);
            ToggleButton_e8e72dc63a814ec8a0035e766f516955.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentTemplateProperty, Binding_0c4a42e9d6e843ce850f6f6c0b859ec2);
            ContentControl_46243c6a201241c39b3f8d385fda3f80.SetBinding(global::Windows.UI.Xaml.FrameworkElement.MarginProperty, Binding_55f6db0f11f848609f0c39be26b080db);
            ContentControl_46243c6a201241c39b3f8d385fda3f80.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentProperty, Binding_7d7bad8fb36d4302af994b2e53858f0b);
            ContentControl_46243c6a201241c39b3f8d385fda3f80.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentTemplateProperty, Binding_3df7b20233be44bfae58a56d02f7191d);
            Border_442cacacc68b4ac28e01ff0719febbc2.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_d02694e0ade448e8b5ff3abeaeda8542);
            Border_442cacacc68b4ac28e01ff0719febbc2.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_12b7eec25cbe45709bccbc1c74061b5b);
            Border_442cacacc68b4ac28e01ff0719febbc2.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_89b2106919c6465e85b3ea5bf1390bae);

            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_75001fba46a34df1b1dbaf16a38655d7,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_dbbd711c3837416bbff9f458bf97df01,
                    setVisualStateProperty_dbbd711c3837416bbff9f458bf97df01,
                    setLocalVisualStateProperty_dbbd711c3837416bbff9f458bf97df01,
                    getVisualStateProperty_dbbd711c3837416bbff9f458bf97df01));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_75001fba46a34df1b1dbaf16a38655d7, ContentControl_46243c6a201241c39b3f8d385fda3f80);

            templateInstance_b5e23e08ba2947618b6f2702818d9477.TemplateContent = Border_442cacacc68b4ac28e01ff0719febbc2;
            return templateInstance_b5e23e08ba2947618b6f2702818d9477;
        }



    }
}
#endif