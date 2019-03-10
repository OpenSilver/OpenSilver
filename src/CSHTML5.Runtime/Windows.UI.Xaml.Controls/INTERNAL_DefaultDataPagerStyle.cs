
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
    internal class INTERNAL_DefaultDataPagerStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {





var Style_48a9c2aac1b04f5cb334bba0fd1621cb = new global::System.Windows.Style();
Style_48a9c2aac1b04f5cb334bba0fd1621cb.TargetType = typeof(global::System.Windows.Controls.DataPager);
var Setter_ed6e2f4d7175424d8bc471dc4418eaf6 = new global::System.Windows.Setter();
Setter_ed6e2f4d7175424d8bc471dc4418eaf6.Property = global::System.Windows.Controls.DataPager.BackgroundProperty;
Setter_ed6e2f4d7175424d8bc471dc4418eaf6.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFF2F3F4");

var Setter_33166b0c3bd2454697a6d5921071a7f5 = new global::System.Windows.Setter();
Setter_33166b0c3bd2454697a6d5921071a7f5.Property = global::System.Windows.Controls.DataPager.BorderBrushProperty;
Setter_33166b0c3bd2454697a6d5921071a7f5.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Gray");

var Setter_c6aed2d9832c458d9520400e7c14701a = new global::System.Windows.Setter();
Setter_c6aed2d9832c458d9520400e7c14701a.Property = global::System.Windows.Controls.DataPager.BorderThicknessProperty;
Setter_c6aed2d9832c458d9520400e7c14701a.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");

var Setter_1bb7437e412c409183cde6250da23c62 = new global::System.Windows.Setter();
Setter_1bb7437e412c409183cde6250da23c62.Property = global::System.Windows.Controls.DataPager.HorizontalContentAlignmentProperty;
Setter_1bb7437e412c409183cde6250da23c62.Value = global::System.Windows.HorizontalAlignment.Right;

var Setter_46e707e78f91413190b404c80f5cb905 = new global::System.Windows.Setter();
Setter_46e707e78f91413190b404c80f5cb905.Property = global::System.Windows.Controls.DataPager.NumericButtonStyleProperty;
var Style_ed7f2c6193174303806b992d6e7b9201 = new global::System.Windows.Style();
Style_ed7f2c6193174303806b992d6e7b9201.TargetType = typeof(global::System.Windows.Controls.Primitives.ToggleButton);
var Setter_70b001c20fb34d6fbe658188fddc4b5c = new global::System.Windows.Setter();
Setter_70b001c20fb34d6fbe658188fddc4b5c.Property = global::System.Windows.Controls.Primitives.ToggleButton.MinHeightProperty;
Setter_70b001c20fb34d6fbe658188fddc4b5c.Value = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");

var Setter_79231c144ded4368a05a1dcadcaa6d14 = new global::System.Windows.Setter();
Setter_79231c144ded4368a05a1dcadcaa6d14.Property = global::System.Windows.Controls.Primitives.ToggleButton.MinWidthProperty;
Setter_79231c144ded4368a05a1dcadcaa6d14.Value = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");

var Setter_0b4fbcafdc8945109fda24290a750d89 = new global::System.Windows.Setter();
Setter_0b4fbcafdc8945109fda24290a750d89.Property = global::System.Windows.Controls.Primitives.ToggleButton.HorizontalAlignmentProperty;
Setter_0b4fbcafdc8945109fda24290a750d89.Value = global::System.Windows.HorizontalAlignment.Right;

var Setter_1f244a5e00904369931958694b87db58 = new global::System.Windows.Setter();
Setter_1f244a5e00904369931958694b87db58.Property = global::System.Windows.Controls.Primitives.ToggleButton.VerticalAlignmentProperty;
Setter_1f244a5e00904369931958694b87db58.Value = global::System.Windows.VerticalAlignment.Center;

var Setter_d1b64aecb8a7456091240c8d2b293e17 = new global::System.Windows.Setter();
Setter_d1b64aecb8a7456091240c8d2b293e17.Property = global::System.Windows.Controls.Primitives.ToggleButton.BackgroundProperty;
Setter_d1b64aecb8a7456091240c8d2b293e17.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#00000000");

var Setter_fe20409de6fb4b31ac5c303492679632 = new global::System.Windows.Setter();
Setter_fe20409de6fb4b31ac5c303492679632.Property = global::System.Windows.Controls.Primitives.ToggleButton.BorderThicknessProperty;
Setter_fe20409de6fb4b31ac5c303492679632.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");

var Setter_066bd31647d24fb0adbe856c49adf5d6 = new global::System.Windows.Setter();
Setter_066bd31647d24fb0adbe856c49adf5d6.Property = global::System.Windows.Controls.Primitives.ToggleButton.PaddingProperty;
Setter_066bd31647d24fb0adbe856c49adf5d6.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");

var Setter_913db9820abb4904b65ac65c27441595 = new global::System.Windows.Setter();
Setter_913db9820abb4904b65ac65c27441595.Property = global::System.Windows.Controls.Primitives.ToggleButton.TemplateProperty;
var ControlTemplate_ec87bd0495374250bda6c17ada3699b5 = new global::System.Windows.Controls.ControlTemplate();
ControlTemplate_ec87bd0495374250bda6c17ada3699b5.TargetType = typeof(global::System.Windows.Controls.Primitives.ToggleButton);
ControlTemplate_ec87bd0495374250bda6c17ada3699b5.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_ec87bd0495374250bda6c17ada3699b5);

Setter_913db9820abb4904b65ac65c27441595.Value = ControlTemplate_ec87bd0495374250bda6c17ada3699b5;


Style_ed7f2c6193174303806b992d6e7b9201.Setters.Add(Setter_70b001c20fb34d6fbe658188fddc4b5c);
Style_ed7f2c6193174303806b992d6e7b9201.Setters.Add(Setter_79231c144ded4368a05a1dcadcaa6d14);
Style_ed7f2c6193174303806b992d6e7b9201.Setters.Add(Setter_0b4fbcafdc8945109fda24290a750d89);
Style_ed7f2c6193174303806b992d6e7b9201.Setters.Add(Setter_1f244a5e00904369931958694b87db58);
Style_ed7f2c6193174303806b992d6e7b9201.Setters.Add(Setter_d1b64aecb8a7456091240c8d2b293e17);
Style_ed7f2c6193174303806b992d6e7b9201.Setters.Add(Setter_fe20409de6fb4b31ac5c303492679632);
Style_ed7f2c6193174303806b992d6e7b9201.Setters.Add(Setter_066bd31647d24fb0adbe856c49adf5d6);
Style_ed7f2c6193174303806b992d6e7b9201.Setters.Add(Setter_913db9820abb4904b65ac65c27441595);


Setter_46e707e78f91413190b404c80f5cb905.Value = Style_ed7f2c6193174303806b992d6e7b9201;


var Setter_14827f7092e74b0fb6f99ffe408a59e5 = new global::System.Windows.Setter();
Setter_14827f7092e74b0fb6f99ffe408a59e5.Property = global::System.Windows.Controls.DataPager.TemplateProperty;
var ControlTemplate_25729cbfa664423389a71921ef5f672f = new global::System.Windows.Controls.ControlTemplate();
ControlTemplate_25729cbfa664423389a71921ef5f672f.TargetType = typeof(global::System.Windows.Controls.DataPager);
ControlTemplate_25729cbfa664423389a71921ef5f672f.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_25729cbfa664423389a71921ef5f672f);

Setter_14827f7092e74b0fb6f99ffe408a59e5.Value = ControlTemplate_25729cbfa664423389a71921ef5f672f;


Style_48a9c2aac1b04f5cb334bba0fd1621cb.Setters.Add(Setter_ed6e2f4d7175424d8bc471dc4418eaf6);
Style_48a9c2aac1b04f5cb334bba0fd1621cb.Setters.Add(Setter_33166b0c3bd2454697a6d5921071a7f5);
Style_48a9c2aac1b04f5cb334bba0fd1621cb.Setters.Add(Setter_c6aed2d9832c458d9520400e7c14701a);
Style_48a9c2aac1b04f5cb334bba0fd1621cb.Setters.Add(Setter_1bb7437e412c409183cde6250da23c62);
Style_48a9c2aac1b04f5cb334bba0fd1621cb.Setters.Add(Setter_46e707e78f91413190b404c80f5cb905);
Style_48a9c2aac1b04f5cb334bba0fd1621cb.Setters.Add(Setter_14827f7092e74b0fb6f99ffe408a59e5);


               DefaultStyle = Style_48a9c2aac1b04f5cb334bba0fd1621cb;
            }
            return DefaultStyle;






    
        }



public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_bd0c50e0aa094ae88569d0a5bc3c07e1 (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_bd0c50e0aa094ae88569d0a5bc3c07e1 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_bd0c50e0aa094ae88569d0a5bc3c07e1 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_bd0c50e0aa094ae88569d0a5bc3c07e1 (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_a456d3d23a5d4f9482dde4f726c3e361 (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_a456d3d23a5d4f9482dde4f726c3e361 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_a456d3d23a5d4f9482dde4f726c3e361 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_a456d3d23a5d4f9482dde4f726c3e361 (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_2996c86077b74db1a44e79b444cd3d90 (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_2996c86077b74db1a44e79b444cd3d90 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_2996c86077b74db1a44e79b444cd3d90 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_2996c86077b74db1a44e79b444cd3d90 (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_ec87bd0495374250bda6c17ada3699b5(global::System.Windows.FrameworkElement templateOwner)
        {
var templateInstance_775e6fcf22224272900c5397b4c5f5f6 = new global::System.Windows.TemplateInstance();
templateInstance_775e6fcf22224272900c5397b4c5f5f6.TemplateOwner = templateOwner;
var Grid_94469a9e5eed451bac82ae6b7812395d = new global::System.Windows.Controls.Grid();
var VisualStateGroup_3de8c62808724aa8adf0baa139318a0c = new global::System.Windows.VisualStateGroup();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_3de8c62808724aa8adf0baa139318a0c);
VisualStateGroup_3de8c62808724aa8adf0baa139318a0c.Name = "CommonStates";
var VisualState_56bda82cbdbb4281a202da2b874b4056 = new global::System.Windows.VisualState();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Normal", VisualState_56bda82cbdbb4281a202da2b874b4056);
VisualState_56bda82cbdbb4281a202da2b874b4056.Name = "Normal";

var VisualState_117aff25cef1480e9f47add7ddf01a9d = new global::System.Windows.VisualState();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("MouseOver", VisualState_117aff25cef1480e9f47add7ddf01a9d);
VisualState_117aff25cef1480e9f47add7ddf01a9d.Name = "MouseOver";

var VisualState_f381c250f2004d6fa8e607077108baf6 = new global::System.Windows.VisualState();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Pressed", VisualState_f381c250f2004d6fa8e607077108baf6);
VisualState_f381c250f2004d6fa8e607077108baf6.Name = "Pressed";

var VisualState_48d4766d0dc044c3a93d66b68fa24ee7 = new global::System.Windows.VisualState();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_48d4766d0dc044c3a93d66b68fa24ee7);
VisualState_48d4766d0dc044c3a93d66b68fa24ee7.Name = "Disabled";
var Storyboard_da2c2b3acf6a4b58bd788c98c45884de = new global::System.Windows.Media.Animation.Storyboard();
var ObjectAnimationUsingKeyFrames_1fe72b56484c4726829e2975a58545aa = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_1fe72b56484c4726829e2975a58545aa,@"contentPresenter");
var DiscreteObjectKeyFrame_7671f14c5bd54fc58d34172803d6c65c = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_7671f14c5bd54fc58d34172803d6c65c.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_7671f14c5bd54fc58d34172803d6c65c.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"0.5");

ObjectAnimationUsingKeyFrames_1fe72b56484c4726829e2975a58545aa.KeyFrames.Add(DiscreteObjectKeyFrame_7671f14c5bd54fc58d34172803d6c65c);


Storyboard_da2c2b3acf6a4b58bd788c98c45884de.Children.Add(ObjectAnimationUsingKeyFrames_1fe72b56484c4726829e2975a58545aa);


VisualState_48d4766d0dc044c3a93d66b68fa24ee7.Storyboard = Storyboard_da2c2b3acf6a4b58bd788c98c45884de;


VisualStateGroup_3de8c62808724aa8adf0baa139318a0c.States.Add(VisualState_56bda82cbdbb4281a202da2b874b4056);
VisualStateGroup_3de8c62808724aa8adf0baa139318a0c.States.Add(VisualState_117aff25cef1480e9f47add7ddf01a9d);
VisualStateGroup_3de8c62808724aa8adf0baa139318a0c.States.Add(VisualState_f381c250f2004d6fa8e607077108baf6);
VisualStateGroup_3de8c62808724aa8adf0baa139318a0c.States.Add(VisualState_48d4766d0dc044c3a93d66b68fa24ee7);


var VisualStateGroup_2c6df731bb2b4507a1167fa1abea234f = new global::System.Windows.VisualStateGroup();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("CheckStates", VisualStateGroup_2c6df731bb2b4507a1167fa1abea234f);
VisualStateGroup_2c6df731bb2b4507a1167fa1abea234f.Name = "CheckStates";
var VisualState_cf22006de00c430498374793f9ad96a0 = new global::System.Windows.VisualState();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Checked", VisualState_cf22006de00c430498374793f9ad96a0);
VisualState_cf22006de00c430498374793f9ad96a0.Name = "Checked";
var Storyboard_a0bc8d22367a492bae782a11fdd6ea96 = new global::System.Windows.Media.Animation.Storyboard();
var ObjectAnimationUsingKeyFrames_af59477110984b5091cdc6b15c52582e = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_af59477110984b5091cdc6b15c52582e,@"CheckedStateOuterBorder");
var DiscreteObjectKeyFrame_c5ba0615754c47ccb7b5664930b1aafd = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_c5ba0615754c47ccb7b5664930b1aafd.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_c5ba0615754c47ccb7b5664930b1aafd.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"1");

ObjectAnimationUsingKeyFrames_af59477110984b5091cdc6b15c52582e.KeyFrames.Add(DiscreteObjectKeyFrame_c5ba0615754c47ccb7b5664930b1aafd);


Storyboard_a0bc8d22367a492bae782a11fdd6ea96.Children.Add(ObjectAnimationUsingKeyFrames_af59477110984b5091cdc6b15c52582e);


VisualState_cf22006de00c430498374793f9ad96a0.Storyboard = Storyboard_a0bc8d22367a492bae782a11fdd6ea96;


var VisualState_e1607bef63f04bfcafaf0237ea33555e = new global::System.Windows.VisualState();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Unchecked", VisualState_e1607bef63f04bfcafaf0237ea33555e);
VisualState_e1607bef63f04bfcafaf0237ea33555e.Name = "Unchecked";

VisualStateGroup_2c6df731bb2b4507a1167fa1abea234f.States.Add(VisualState_cf22006de00c430498374793f9ad96a0);
VisualStateGroup_2c6df731bb2b4507a1167fa1abea234f.States.Add(VisualState_e1607bef63f04bfcafaf0237ea33555e);


var VisualStateGroup_e9416c575d9e4fb2956b9d79dca58c8e = new global::System.Windows.VisualStateGroup();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("FocusStates", VisualStateGroup_e9416c575d9e4fb2956b9d79dca58c8e);
VisualStateGroup_e9416c575d9e4fb2956b9d79dca58c8e.Name = "FocusStates";
var VisualState_ea422e7edb314713a12e0e83acf2fe1e = new global::System.Windows.VisualState();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Focused", VisualState_ea422e7edb314713a12e0e83acf2fe1e);
VisualState_ea422e7edb314713a12e0e83acf2fe1e.Name = "Focused";
var Storyboard_e11ffccdd62d4e4680b7e953f3342549 = new global::System.Windows.Media.Animation.Storyboard();
var ObjectAnimationUsingKeyFrames_7378f61104fd4800be233d764be77e28 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_7378f61104fd4800be233d764be77e28,@"FocusVisualElement");
var DiscreteObjectKeyFrame_bdd2b367c0054bb79d3811cc067ee0dc = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_bdd2b367c0054bb79d3811cc067ee0dc.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_bdd2b367c0054bb79d3811cc067ee0dc.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"1");

ObjectAnimationUsingKeyFrames_7378f61104fd4800be233d764be77e28.KeyFrames.Add(DiscreteObjectKeyFrame_bdd2b367c0054bb79d3811cc067ee0dc);


Storyboard_e11ffccdd62d4e4680b7e953f3342549.Children.Add(ObjectAnimationUsingKeyFrames_7378f61104fd4800be233d764be77e28);


VisualState_ea422e7edb314713a12e0e83acf2fe1e.Storyboard = Storyboard_e11ffccdd62d4e4680b7e953f3342549;


var VisualState_36200ecff6464e9483cba882eed695bd = new global::System.Windows.VisualState();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Unfocused", VisualState_36200ecff6464e9483cba882eed695bd);
VisualState_36200ecff6464e9483cba882eed695bd.Name = "Unfocused";

VisualStateGroup_e9416c575d9e4fb2956b9d79dca58c8e.States.Add(VisualState_ea422e7edb314713a12e0e83acf2fe1e);
VisualStateGroup_e9416c575d9e4fb2956b9d79dca58c8e.States.Add(VisualState_36200ecff6464e9483cba882eed695bd);


((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_3de8c62808724aa8adf0baa139318a0c);
((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_2c6df731bb2b4507a1167fa1abea234f);
((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_e9416c575d9e4fb2956b9d79dca58c8e);

var Border_eecf9e336ab745d9b8f95b9c2b9b4e49 = new global::System.Windows.Controls.Border();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("CheckedStateOuterBorder", Border_eecf9e336ab745d9b8f95b9c2b9b4e49);
Border_eecf9e336ab745d9b8f95b9c2b9b4e49.Name = "CheckedStateOuterBorder";
Border_eecf9e336ab745d9b8f95b9c2b9b4e49.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#7FA9A9A9");
Border_eecf9e336ab745d9b8f95b9c2b9b4e49.BorderBrush = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#00FFFFFF");
Border_eecf9e336ab745d9b8f95b9c2b9b4e49.CornerRadius = (global::System.Windows.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.CornerRadius), @"3");
Border_eecf9e336ab745d9b8f95b9c2b9b4e49.Opacity = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");
var Binding_bcd8811d47304f19bccb473b35582756 = new global::System.Windows.Data.Binding();
Binding_bcd8811d47304f19bccb473b35582756.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
var RelativeSource_41d2837eea7045099cb6515febf42613 = new global::System.Windows.Data.RelativeSource();
RelativeSource_41d2837eea7045099cb6515febf42613.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_bcd8811d47304f19bccb473b35582756.RelativeSource = RelativeSource_41d2837eea7045099cb6515febf42613;


Binding_bcd8811d47304f19bccb473b35582756.TemplateOwner = templateInstance_775e6fcf22224272900c5397b4c5f5f6;


var Border_4da38deed2994c11bd59fa76e919bbc3 = new global::System.Windows.Controls.Border();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("OuterBtnBorder", Border_4da38deed2994c11bd59fa76e919bbc3);
Border_4da38deed2994c11bd59fa76e919bbc3.Name = "OuterBtnBorder";
Border_4da38deed2994c11bd59fa76e919bbc3.BorderBrush = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#00FFFFFF");
Border_4da38deed2994c11bd59fa76e919bbc3.CornerRadius = (global::System.Windows.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.CornerRadius), @"3");
var Border_ce1451764756494cb093c87f1a72735c = new global::System.Windows.Controls.Border();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("InnerBtnBorder", Border_ce1451764756494cb093c87f1a72735c);
Border_ce1451764756494cb093c87f1a72735c.Name = "InnerBtnBorder";
Border_ce1451764756494cb093c87f1a72735c.BorderBrush = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#00CCD1D6");
Border_ce1451764756494cb093c87f1a72735c.CornerRadius = (global::System.Windows.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.CornerRadius), @"2");
var ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64 = new global::System.Windows.Controls.ContentPresenter();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("contentPresenter", ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64);
ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64.Name = "contentPresenter";
ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Center;
ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
var Binding_e55036387c2c4bccbc03759fc54486e4 = new global::System.Windows.Data.Binding();
Binding_e55036387c2c4bccbc03759fc54486e4.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Content");
var RelativeSource_1ab0218e66c449ba8129df37262d41bb = new global::System.Windows.Data.RelativeSource();
RelativeSource_1ab0218e66c449ba8129df37262d41bb.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_e55036387c2c4bccbc03759fc54486e4.RelativeSource = RelativeSource_1ab0218e66c449ba8129df37262d41bb;


Binding_e55036387c2c4bccbc03759fc54486e4.TemplateOwner = templateInstance_775e6fcf22224272900c5397b4c5f5f6;

var Binding_1f3f6b2d019c4a9a89fe716bbee3e15a = new global::System.Windows.Data.Binding();
Binding_1f3f6b2d019c4a9a89fe716bbee3e15a.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"ContentTemplate");
var RelativeSource_24e59aedc39f41b4aaf1aee6eda60a83 = new global::System.Windows.Data.RelativeSource();
RelativeSource_24e59aedc39f41b4aaf1aee6eda60a83.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_1f3f6b2d019c4a9a89fe716bbee3e15a.RelativeSource = RelativeSource_24e59aedc39f41b4aaf1aee6eda60a83;


Binding_1f3f6b2d019c4a9a89fe716bbee3e15a.TemplateOwner = templateInstance_775e6fcf22224272900c5397b4c5f5f6;


Border_ce1451764756494cb093c87f1a72735c.Child = ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64;

var Binding_b03a2e7d26cd47a59601d390d4841c75 = new global::System.Windows.Data.Binding();
Binding_b03a2e7d26cd47a59601d390d4841c75.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
var RelativeSource_87058a93505542889ed8e17caf9435f7 = new global::System.Windows.Data.RelativeSource();
RelativeSource_87058a93505542889ed8e17caf9435f7.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_b03a2e7d26cd47a59601d390d4841c75.RelativeSource = RelativeSource_87058a93505542889ed8e17caf9435f7;


Binding_b03a2e7d26cd47a59601d390d4841c75.TemplateOwner = templateInstance_775e6fcf22224272900c5397b4c5f5f6;


Border_4da38deed2994c11bd59fa76e919bbc3.Child = Border_ce1451764756494cb093c87f1a72735c;

var Binding_f4ac443209c84eb39534e21ffd629afb = new global::System.Windows.Data.Binding();
Binding_f4ac443209c84eb39534e21ffd629afb.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
var RelativeSource_a247992ddf834f06a760fd7885c82640 = new global::System.Windows.Data.RelativeSource();
RelativeSource_a247992ddf834f06a760fd7885c82640.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_f4ac443209c84eb39534e21ffd629afb.RelativeSource = RelativeSource_a247992ddf834f06a760fd7885c82640;


Binding_f4ac443209c84eb39534e21ffd629afb.TemplateOwner = templateInstance_775e6fcf22224272900c5397b4c5f5f6;

var Binding_c726a1cbcb3a4645bd1a5b6906e5af4e = new global::System.Windows.Data.Binding();
Binding_c726a1cbcb3a4645bd1a5b6906e5af4e.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
var RelativeSource_8c5fa5c757b34043aa7d8f199a7b8132 = new global::System.Windows.Data.RelativeSource();
RelativeSource_8c5fa5c757b34043aa7d8f199a7b8132.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_c726a1cbcb3a4645bd1a5b6906e5af4e.RelativeSource = RelativeSource_8c5fa5c757b34043aa7d8f199a7b8132;


Binding_c726a1cbcb3a4645bd1a5b6906e5af4e.TemplateOwner = templateInstance_775e6fcf22224272900c5397b4c5f5f6;


var Border_3581775925684bed938ba5e5f2413265 = new global::System.Windows.Controls.Border();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("FocusVisualElement", Border_3581775925684bed938ba5e5f2413265);
Border_3581775925684bed938ba5e5f2413265.Name = "FocusVisualElement";
Border_3581775925684bed938ba5e5f2413265.BorderBrush = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FF6DBDD1");
Border_3581775925684bed938ba5e5f2413265.CornerRadius = (global::System.Windows.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.CornerRadius), @"2");
Border_3581775925684bed938ba5e5f2413265.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");
Border_3581775925684bed938ba5e5f2413265.Opacity = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");
var Binding_8a60146f7e0249f39a63e498869c3f54 = new global::System.Windows.Data.Binding();
Binding_8a60146f7e0249f39a63e498869c3f54.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
var RelativeSource_19b621559c4e48cc9ab22ebff5820ec5 = new global::System.Windows.Data.RelativeSource();
RelativeSource_19b621559c4e48cc9ab22ebff5820ec5.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_8a60146f7e0249f39a63e498869c3f54.RelativeSource = RelativeSource_19b621559c4e48cc9ab22ebff5820ec5;


Binding_8a60146f7e0249f39a63e498869c3f54.TemplateOwner = templateInstance_775e6fcf22224272900c5397b4c5f5f6;

var Binding_e84a71b43bbe4b75aeb101ccdce6de3b = new global::System.Windows.Data.Binding();
Binding_e84a71b43bbe4b75aeb101ccdce6de3b.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
var RelativeSource_8fb5cabb9e694cb68c0abc9afd77c4c0 = new global::System.Windows.Data.RelativeSource();
RelativeSource_8fb5cabb9e694cb68c0abc9afd77c4c0.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_e84a71b43bbe4b75aeb101ccdce6de3b.RelativeSource = RelativeSource_8fb5cabb9e694cb68c0abc9afd77c4c0;


Binding_e84a71b43bbe4b75aeb101ccdce6de3b.TemplateOwner = templateInstance_775e6fcf22224272900c5397b4c5f5f6;


Grid_94469a9e5eed451bac82ae6b7812395d.Children.Add(Border_eecf9e336ab745d9b8f95b9c2b9b4e49);
Grid_94469a9e5eed451bac82ae6b7812395d.Children.Add(Border_4da38deed2994c11bd59fa76e919bbc3);
Grid_94469a9e5eed451bac82ae6b7812395d.Children.Add(Border_3581775925684bed938ba5e5f2413265);



Border_eecf9e336ab745d9b8f95b9c2b9b4e49.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_bcd8811d47304f19bccb473b35582756);
ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64.SetBinding(global::System.Windows.Controls.ContentControl.ContentProperty, Binding_e55036387c2c4bccbc03759fc54486e4);
ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64.SetBinding(global::System.Windows.Controls.ContentControl.ContentTemplateProperty, Binding_1f3f6b2d019c4a9a89fe716bbee3e15a);
Border_ce1451764756494cb093c87f1a72735c.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_b03a2e7d26cd47a59601d390d4841c75);
Border_4da38deed2994c11bd59fa76e919bbc3.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_f4ac443209c84eb39534e21ffd629afb);
Border_4da38deed2994c11bd59fa76e919bbc3.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_c726a1cbcb3a4645bd1a5b6906e5af4e);
Border_3581775925684bed938ba5e5f2413265.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_8a60146f7e0249f39a63e498869c3f54);
Border_3581775925684bed938ba5e5f2413265.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_e84a71b43bbe4b75aeb101ccdce6de3b);

global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_1fe72b56484c4726829e2975a58545aa,
    new global::System.Windows.PropertyPath(
        "Opacity",
        "Opacity",
        accessVisualStateProperty_bd0c50e0aa094ae88569d0a5bc3c07e1,
        setVisualStateProperty_bd0c50e0aa094ae88569d0a5bc3c07e1,
        setLocalVisualStateProperty_bd0c50e0aa094ae88569d0a5bc3c07e1,
        getVisualStateProperty_bd0c50e0aa094ae88569d0a5bc3c07e1));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_1fe72b56484c4726829e2975a58545aa, ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_af59477110984b5091cdc6b15c52582e,
    new global::System.Windows.PropertyPath(
        "Opacity",
        "Opacity",
        accessVisualStateProperty_a456d3d23a5d4f9482dde4f726c3e361,
        setVisualStateProperty_a456d3d23a5d4f9482dde4f726c3e361,
        setLocalVisualStateProperty_a456d3d23a5d4f9482dde4f726c3e361,
        getVisualStateProperty_a456d3d23a5d4f9482dde4f726c3e361));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_af59477110984b5091cdc6b15c52582e, Border_eecf9e336ab745d9b8f95b9c2b9b4e49);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_7378f61104fd4800be233d764be77e28,
    new global::System.Windows.PropertyPath(
        "Opacity",
        "Opacity",
        accessVisualStateProperty_2996c86077b74db1a44e79b444cd3d90,
        setVisualStateProperty_2996c86077b74db1a44e79b444cd3d90,
        setLocalVisualStateProperty_2996c86077b74db1a44e79b444cd3d90,
        getVisualStateProperty_2996c86077b74db1a44e79b444cd3d90));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_7378f61104fd4800be233d764be77e28, Border_3581775925684bed938ba5e5f2413265);

templateInstance_775e6fcf22224272900c5397b4c5f5f6.TemplateContent = Grid_94469a9e5eed451bac82ae6b7812395d;
return templateInstance_775e6fcf22224272900c5397b4c5f5f6;
        }



public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_e4c067ed50db4a1493459f6d6c44200f (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_e4c067ed50db4a1493459f6d6c44200f (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_e4c067ed50db4a1493459f6d6c44200f (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_e4c067ed50db4a1493459f6d6c44200f (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_25a037c483174582b5b8ff6e591fa34f (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_25a037c483174582b5b8ff6e591fa34f (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_25a037c483174582b5b8ff6e591fa34f (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_25a037c483174582b5b8ff6e591fa34f (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_310181e043004846999fcd1fd825a8b6 (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_310181e043004846999fcd1fd825a8b6 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_310181e043004846999fcd1fd825a8b6 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_310181e043004846999fcd1fd825a8b6 (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_92bb952c168248e48af0c8f0c2f4476c (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_92bb952c168248e48af0c8f0c2f4476c (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_92bb952c168248e48af0c8f0c2f4476c (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_92bb952c168248e48af0c8f0c2f4476c (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_31a81299a7e94144bfbb2d7c3098a50e (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_31a81299a7e94144bfbb2d7c3098a50e (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_31a81299a7e94144bfbb2d7c3098a50e (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_31a81299a7e94144bfbb2d7c3098a50e (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_e54f799f2101425eae897770a57938ca (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_e54f799f2101425eae897770a57938ca (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_e54f799f2101425eae897770a57938ca (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_e54f799f2101425eae897770a57938ca (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_de55a2edc4954dc2957579cc3b6081bc (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_de55a2edc4954dc2957579cc3b6081bc (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_de55a2edc4954dc2957579cc3b6081bc (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_de55a2edc4954dc2957579cc3b6081bc (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_c36c66a63b084180872fe4f6bd226cfe (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_c36c66a63b084180872fe4f6bd226cfe (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_c36c66a63b084180872fe4f6bd226cfe (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_c36c66a63b084180872fe4f6bd226cfe (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_ad8d0039b8a4499f9fd1032501ea81d5 (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_ad8d0039b8a4499f9fd1032501ea81d5 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_ad8d0039b8a4499f9fd1032501ea81d5 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_ad8d0039b8a4499f9fd1032501ea81d5 (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_b8d7583c61994c808ab5aa00dbc46386 (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_b8d7583c61994c808ab5aa00dbc46386 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_b8d7583c61994c808ab5aa00dbc46386 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_b8d7583c61994c808ab5aa00dbc46386 (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_f1d0cdc46f5b4110b414c009ffb63c0a (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_f1d0cdc46f5b4110b414c009ffb63c0a (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_f1d0cdc46f5b4110b414c009ffb63c0a (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_f1d0cdc46f5b4110b414c009ffb63c0a (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_cf7c1d4daafe484bbc4736b387b61ab9 (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_cf7c1d4daafe484bbc4736b387b61ab9 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_cf7c1d4daafe484bbc4736b387b61ab9 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_cf7c1d4daafe484bbc4736b387b61ab9 (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_4064e88d2988479aacc4d848ee5f2e52 (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_4064e88d2988479aacc4d848ee5f2e52 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_4064e88d2988479aacc4d848ee5f2e52 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_4064e88d2988479aacc4d848ee5f2e52 (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_7670580d752a45d0972587bb549a9ab6 (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_7670580d752a45d0972587bb549a9ab6 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_7670580d752a45d0972587bb549a9ab6 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_7670580d752a45d0972587bb549a9ab6 (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_6e581dd54e344d7e94f57c0978c290e6 (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_6e581dd54e344d7e94f57c0978c290e6 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_6e581dd54e344d7e94f57c0978c290e6 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_6e581dd54e344d7e94f57c0978c290e6 (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_996462726a7744be8bb086c73f96ff57 (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_996462726a7744be8bb086c73f96ff57 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_996462726a7744be8bb086c73f96ff57 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_996462726a7744be8bb086c73f96ff57 (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_fb5773338a444cbc8bd2cc90ff66f7b2 (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_fb5773338a444cbc8bd2cc90ff66f7b2 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_fb5773338a444cbc8bd2cc90ff66f7b2 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_fb5773338a444cbc8bd2cc90ff66f7b2 (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_ddc25f31b9da44e9a6e85d92302d793a (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_ddc25f31b9da44e9a6e85d92302d793a (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_ddc25f31b9da44e9a6e85d92302d793a (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_ddc25f31b9da44e9a6e85d92302d793a (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_586d9fee14b642098172624535a9b0e1 (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_586d9fee14b642098172624535a9b0e1 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_586d9fee14b642098172624535a9b0e1 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_586d9fee14b642098172624535a9b0e1 (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_cd347efb57c54637b1df77c18e921404 (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_cd347efb57c54637b1df77c18e921404 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_cd347efb57c54637b1df77c18e921404 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_cd347efb57c54637b1df77c18e921404 (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_95f930ac5c8e479bbb22a702196e76c5 (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_95f930ac5c8e479bbb22a702196e76c5 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_95f930ac5c8e479bbb22a702196e76c5 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_95f930ac5c8e479bbb22a702196e76c5 (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_5a58a72b322f4b59ba82da3ba51062b7 (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_5a58a72b322f4b59ba82da3ba51062b7 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_5a58a72b322f4b59ba82da3ba51062b7 (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_5a58a72b322f4b59ba82da3ba51062b7 (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_25729cbfa664423389a71921ef5f672f(global::System.Windows.FrameworkElement templateOwner)
        {
var templateInstance_02c86289167443ce9f58fee5a926ce26 = new global::System.Windows.TemplateInstance();
templateInstance_02c86289167443ce9f58fee5a926ce26.TemplateOwner = templateOwner;
var Grid_7f4ee1c24187438c85695ea996c43dd0 = new global::System.Windows.Controls.Grid();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Root", Grid_7f4ee1c24187438c85695ea996c43dd0);
Grid_7f4ee1c24187438c85695ea996c43dd0.Name = "Root";
Grid_7f4ee1c24187438c85695ea996c43dd0.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Transparent");

var VisualStateGroup_0db846b4082747c39cfb7fc007499e8b = new global::System.Windows.VisualStateGroup();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("DisplayModeStates", VisualStateGroup_0db846b4082747c39cfb7fc007499e8b);
VisualStateGroup_0db846b4082747c39cfb7fc007499e8b.Name = "DisplayModeStates";
var VisualState_cdb8e9e4f55d4b4980098a1336bd34c1 = new global::System.Windows.VisualState();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("FirstLastNumeric", VisualState_cdb8e9e4f55d4b4980098a1336bd34c1);
VisualState_cdb8e9e4f55d4b4980098a1336bd34c1.Name = "FirstLastNumeric";
var Storyboard_d45d98aa52954b6399c6d81cc7e494d1 = new global::System.Windows.Media.Animation.Storyboard();
var ObjectAnimationUsingKeyFrames_1b4d05d2581149419affafa47f0da001 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_1b4d05d2581149419affafa47f0da001,@"NextPageButton");
var DiscreteObjectKeyFrame_dac9434873a445fc83d770765671947b = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_dac9434873a445fc83d770765671947b.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_dac9434873a445fc83d770765671947b.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_1b4d05d2581149419affafa47f0da001.KeyFrames.Add(DiscreteObjectKeyFrame_dac9434873a445fc83d770765671947b);


var ObjectAnimationUsingKeyFrames_f554248a2dae432db7f6ebb0e9052863 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_f554248a2dae432db7f6ebb0e9052863,@"PreviousPageButton");
var DiscreteObjectKeyFrame_350c0ce1301b41e186ae0d4c48392ae0 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_350c0ce1301b41e186ae0d4c48392ae0.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_350c0ce1301b41e186ae0d4c48392ae0.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_f554248a2dae432db7f6ebb0e9052863.KeyFrames.Add(DiscreteObjectKeyFrame_350c0ce1301b41e186ae0d4c48392ae0);


var ObjectAnimationUsingKeyFrames_492215f6c9d140c38c7a8a9b81f1f512 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_492215f6c9d140c38c7a8a9b81f1f512,@"CurrentPageTextBox");
var DiscreteObjectKeyFrame_06e4a74f23cf4078b76c06e3b07fd556 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_06e4a74f23cf4078b76c06e3b07fd556.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_06e4a74f23cf4078b76c06e3b07fd556.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_492215f6c9d140c38c7a8a9b81f1f512.KeyFrames.Add(DiscreteObjectKeyFrame_06e4a74f23cf4078b76c06e3b07fd556);


var ObjectAnimationUsingKeyFrames_52c8493a678348afac140010c5c94cd4 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_52c8493a678348afac140010c5c94cd4,@"PageDisplay");
var DiscreteObjectKeyFrame_11c82e7300bf4d288e77cb53b57c3218 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_11c82e7300bf4d288e77cb53b57c3218.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_11c82e7300bf4d288e77cb53b57c3218.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_52c8493a678348afac140010c5c94cd4.KeyFrames.Add(DiscreteObjectKeyFrame_11c82e7300bf4d288e77cb53b57c3218);


Storyboard_d45d98aa52954b6399c6d81cc7e494d1.Children.Add(ObjectAnimationUsingKeyFrames_1b4d05d2581149419affafa47f0da001);
Storyboard_d45d98aa52954b6399c6d81cc7e494d1.Children.Add(ObjectAnimationUsingKeyFrames_f554248a2dae432db7f6ebb0e9052863);
Storyboard_d45d98aa52954b6399c6d81cc7e494d1.Children.Add(ObjectAnimationUsingKeyFrames_492215f6c9d140c38c7a8a9b81f1f512);
Storyboard_d45d98aa52954b6399c6d81cc7e494d1.Children.Add(ObjectAnimationUsingKeyFrames_52c8493a678348afac140010c5c94cd4);


VisualState_cdb8e9e4f55d4b4980098a1336bd34c1.Storyboard = Storyboard_d45d98aa52954b6399c6d81cc7e494d1;


var VisualState_97b3411c5d2340b0a2fab167217199ee = new global::System.Windows.VisualState();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("FirstLastPreviousNext", VisualState_97b3411c5d2340b0a2fab167217199ee);
VisualState_97b3411c5d2340b0a2fab167217199ee.Name = "FirstLastPreviousNext";
var Storyboard_22afc7f25c8044208500f6ce1158e4aa = new global::System.Windows.Media.Animation.Storyboard();
var ObjectAnimationUsingKeyFrames_83430102a5a64417ab18dad8fd48f51e = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_83430102a5a64417ab18dad8fd48f51e,@"NumericButtonPanel");
var DiscreteObjectKeyFrame_e5330c14fc9047729967981f23d6e47f = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_e5330c14fc9047729967981f23d6e47f.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_e5330c14fc9047729967981f23d6e47f.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_83430102a5a64417ab18dad8fd48f51e.KeyFrames.Add(DiscreteObjectKeyFrame_e5330c14fc9047729967981f23d6e47f);


Storyboard_22afc7f25c8044208500f6ce1158e4aa.Children.Add(ObjectAnimationUsingKeyFrames_83430102a5a64417ab18dad8fd48f51e);


VisualState_97b3411c5d2340b0a2fab167217199ee.Storyboard = Storyboard_22afc7f25c8044208500f6ce1158e4aa;


var VisualState_c714f0d40de84daaba56d7144967c7c0 = new global::System.Windows.VisualState();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("FirstLastPreviousNextNumeric", VisualState_c714f0d40de84daaba56d7144967c7c0);
VisualState_c714f0d40de84daaba56d7144967c7c0.Name = "FirstLastPreviousNextNumeric";
var Storyboard_9379582d77104abb9c300009b32ae91a = new global::System.Windows.Media.Animation.Storyboard();
var ObjectAnimationUsingKeyFrames_90cde9f5245749d4af1f42fb1eae22d5 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_90cde9f5245749d4af1f42fb1eae22d5,@"CurrentPageTextBox");
var DiscreteObjectKeyFrame_74022c29a31a47d8a61a66f33db523c9 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_74022c29a31a47d8a61a66f33db523c9.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_74022c29a31a47d8a61a66f33db523c9.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_90cde9f5245749d4af1f42fb1eae22d5.KeyFrames.Add(DiscreteObjectKeyFrame_74022c29a31a47d8a61a66f33db523c9);


var ObjectAnimationUsingKeyFrames_abf89c881cc345c785eb2672a963604b = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_abf89c881cc345c785eb2672a963604b,@"PageDisplay");
var DiscreteObjectKeyFrame_56eb956d4f8f445f96fbcca7707c9a69 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_56eb956d4f8f445f96fbcca7707c9a69.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_56eb956d4f8f445f96fbcca7707c9a69.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_abf89c881cc345c785eb2672a963604b.KeyFrames.Add(DiscreteObjectKeyFrame_56eb956d4f8f445f96fbcca7707c9a69);


Storyboard_9379582d77104abb9c300009b32ae91a.Children.Add(ObjectAnimationUsingKeyFrames_90cde9f5245749d4af1f42fb1eae22d5);
Storyboard_9379582d77104abb9c300009b32ae91a.Children.Add(ObjectAnimationUsingKeyFrames_abf89c881cc345c785eb2672a963604b);


VisualState_c714f0d40de84daaba56d7144967c7c0.Storyboard = Storyboard_9379582d77104abb9c300009b32ae91a;


var VisualState_f590894a41ca483287bf48c453e50c42 = new global::System.Windows.VisualState();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Numeric", VisualState_f590894a41ca483287bf48c453e50c42);
VisualState_f590894a41ca483287bf48c453e50c42.Name = "Numeric";
var Storyboard_d5db8acaf377420984ff5e99d0c3fad4 = new global::System.Windows.Media.Animation.Storyboard();
var ObjectAnimationUsingKeyFrames_b0cdd1499d1d4362841d0a2240d2a7bc = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_b0cdd1499d1d4362841d0a2240d2a7bc,@"FirstPageButton");
var DiscreteObjectKeyFrame_91e4e8c443e04d748aa9abca06ebd097 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_91e4e8c443e04d748aa9abca06ebd097.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_91e4e8c443e04d748aa9abca06ebd097.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_b0cdd1499d1d4362841d0a2240d2a7bc.KeyFrames.Add(DiscreteObjectKeyFrame_91e4e8c443e04d748aa9abca06ebd097);


var ObjectAnimationUsingKeyFrames_bbda902d40a547d897d1f607045f5776 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_bbda902d40a547d897d1f607045f5776,@"LastPageButton");
var DiscreteObjectKeyFrame_f074522b081740e0bdb93b895769bc10 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_f074522b081740e0bdb93b895769bc10.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_f074522b081740e0bdb93b895769bc10.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_bbda902d40a547d897d1f607045f5776.KeyFrames.Add(DiscreteObjectKeyFrame_f074522b081740e0bdb93b895769bc10);


var ObjectAnimationUsingKeyFrames_37259e069f894df4a5bb3a1aaf08cf9a = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_37259e069f894df4a5bb3a1aaf08cf9a,@"NextPageButton");
var DiscreteObjectKeyFrame_76a6d39ac2ed4f40a86313046cebeee6 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_76a6d39ac2ed4f40a86313046cebeee6.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_76a6d39ac2ed4f40a86313046cebeee6.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_37259e069f894df4a5bb3a1aaf08cf9a.KeyFrames.Add(DiscreteObjectKeyFrame_76a6d39ac2ed4f40a86313046cebeee6);


var ObjectAnimationUsingKeyFrames_4a6eef40a2704fa294e717216b1a641f = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_4a6eef40a2704fa294e717216b1a641f,@"PreviousPageButton");
var DiscreteObjectKeyFrame_bbcd8b558192456582eb02e9baf98fbb = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_bbcd8b558192456582eb02e9baf98fbb.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_bbcd8b558192456582eb02e9baf98fbb.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_4a6eef40a2704fa294e717216b1a641f.KeyFrames.Add(DiscreteObjectKeyFrame_bbcd8b558192456582eb02e9baf98fbb);


var ObjectAnimationUsingKeyFrames_5d6117e1885f4282bdaf3569ead6bd51 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_5d6117e1885f4282bdaf3569ead6bd51,@"CurrentPageTextBox");
var DiscreteObjectKeyFrame_3758392e1ad34c94a7b90ce85eac46c6 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_3758392e1ad34c94a7b90ce85eac46c6.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_3758392e1ad34c94a7b90ce85eac46c6.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_5d6117e1885f4282bdaf3569ead6bd51.KeyFrames.Add(DiscreteObjectKeyFrame_3758392e1ad34c94a7b90ce85eac46c6);


var ObjectAnimationUsingKeyFrames_f60cb089a8db45c3a5571057b20675e2 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_f60cb089a8db45c3a5571057b20675e2,@"PageDisplay");
var DiscreteObjectKeyFrame_3420ee16bf5c431c8f2d56f87909ddf2 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_3420ee16bf5c431c8f2d56f87909ddf2.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_3420ee16bf5c431c8f2d56f87909ddf2.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_f60cb089a8db45c3a5571057b20675e2.KeyFrames.Add(DiscreteObjectKeyFrame_3420ee16bf5c431c8f2d56f87909ddf2);


var ObjectAnimationUsingKeyFrames_560ac39113924ef0bdcbd3fed2a0872b = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_560ac39113924ef0bdcbd3fed2a0872b,@"Separator1");
var DiscreteObjectKeyFrame_f3ac2e78d2dd49eab9fc31e18d807b55 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_f3ac2e78d2dd49eab9fc31e18d807b55.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_f3ac2e78d2dd49eab9fc31e18d807b55.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_560ac39113924ef0bdcbd3fed2a0872b.KeyFrames.Add(DiscreteObjectKeyFrame_f3ac2e78d2dd49eab9fc31e18d807b55);


var ObjectAnimationUsingKeyFrames_ed971e57f6b242a18353cff9222679ea = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_ed971e57f6b242a18353cff9222679ea,@"Separator2");
var DiscreteObjectKeyFrame_3c054f5957f54bb9b41bc7b7021de821 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_3c054f5957f54bb9b41bc7b7021de821.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_3c054f5957f54bb9b41bc7b7021de821.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_ed971e57f6b242a18353cff9222679ea.KeyFrames.Add(DiscreteObjectKeyFrame_3c054f5957f54bb9b41bc7b7021de821);


Storyboard_d5db8acaf377420984ff5e99d0c3fad4.Children.Add(ObjectAnimationUsingKeyFrames_b0cdd1499d1d4362841d0a2240d2a7bc);
Storyboard_d5db8acaf377420984ff5e99d0c3fad4.Children.Add(ObjectAnimationUsingKeyFrames_bbda902d40a547d897d1f607045f5776);
Storyboard_d5db8acaf377420984ff5e99d0c3fad4.Children.Add(ObjectAnimationUsingKeyFrames_37259e069f894df4a5bb3a1aaf08cf9a);
Storyboard_d5db8acaf377420984ff5e99d0c3fad4.Children.Add(ObjectAnimationUsingKeyFrames_4a6eef40a2704fa294e717216b1a641f);
Storyboard_d5db8acaf377420984ff5e99d0c3fad4.Children.Add(ObjectAnimationUsingKeyFrames_5d6117e1885f4282bdaf3569ead6bd51);
Storyboard_d5db8acaf377420984ff5e99d0c3fad4.Children.Add(ObjectAnimationUsingKeyFrames_f60cb089a8db45c3a5571057b20675e2);
Storyboard_d5db8acaf377420984ff5e99d0c3fad4.Children.Add(ObjectAnimationUsingKeyFrames_560ac39113924ef0bdcbd3fed2a0872b);
Storyboard_d5db8acaf377420984ff5e99d0c3fad4.Children.Add(ObjectAnimationUsingKeyFrames_ed971e57f6b242a18353cff9222679ea);


VisualState_f590894a41ca483287bf48c453e50c42.Storyboard = Storyboard_d5db8acaf377420984ff5e99d0c3fad4;


var VisualState_829b0a5021a149708619a389d66a0c39 = new global::System.Windows.VisualState();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("PreviousNext", VisualState_829b0a5021a149708619a389d66a0c39);
VisualState_829b0a5021a149708619a389d66a0c39.Name = "PreviousNext";
var Storyboard_103859abbe934fc381dc6b63ebb052cf = new global::System.Windows.Media.Animation.Storyboard();
var ObjectAnimationUsingKeyFrames_ed84f6c7f82a4d9aa9c5629f8173ea6a = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_ed84f6c7f82a4d9aa9c5629f8173ea6a,@"FirstPageButton");
var DiscreteObjectKeyFrame_826fa88ff77743e78aa402b68505e49f = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_826fa88ff77743e78aa402b68505e49f.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_826fa88ff77743e78aa402b68505e49f.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_ed84f6c7f82a4d9aa9c5629f8173ea6a.KeyFrames.Add(DiscreteObjectKeyFrame_826fa88ff77743e78aa402b68505e49f);


var ObjectAnimationUsingKeyFrames_6d6892eae02f41fdbb17fb652f164b51 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_6d6892eae02f41fdbb17fb652f164b51,@"LastPageButton");
var DiscreteObjectKeyFrame_9ec0342da2404093a8cc2ea4ed0fe49b = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_9ec0342da2404093a8cc2ea4ed0fe49b.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_9ec0342da2404093a8cc2ea4ed0fe49b.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_6d6892eae02f41fdbb17fb652f164b51.KeyFrames.Add(DiscreteObjectKeyFrame_9ec0342da2404093a8cc2ea4ed0fe49b);


var ObjectAnimationUsingKeyFrames_68d4da37033d42bd91210437952956a6 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_68d4da37033d42bd91210437952956a6,@"NumericButtonPanel");
var DiscreteObjectKeyFrame_2f401ba39ca14d6a946fc0a8efc842ad = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_2f401ba39ca14d6a946fc0a8efc842ad.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_2f401ba39ca14d6a946fc0a8efc842ad.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_68d4da37033d42bd91210437952956a6.KeyFrames.Add(DiscreteObjectKeyFrame_2f401ba39ca14d6a946fc0a8efc842ad);


Storyboard_103859abbe934fc381dc6b63ebb052cf.Children.Add(ObjectAnimationUsingKeyFrames_ed84f6c7f82a4d9aa9c5629f8173ea6a);
Storyboard_103859abbe934fc381dc6b63ebb052cf.Children.Add(ObjectAnimationUsingKeyFrames_6d6892eae02f41fdbb17fb652f164b51);
Storyboard_103859abbe934fc381dc6b63ebb052cf.Children.Add(ObjectAnimationUsingKeyFrames_68d4da37033d42bd91210437952956a6);


VisualState_829b0a5021a149708619a389d66a0c39.Storyboard = Storyboard_103859abbe934fc381dc6b63ebb052cf;


var VisualState_3279838841a8491daa77f95b6c19c5bb = new global::System.Windows.VisualState();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("PreviousNextNumeric", VisualState_3279838841a8491daa77f95b6c19c5bb);
VisualState_3279838841a8491daa77f95b6c19c5bb.Name = "PreviousNextNumeric";
var Storyboard_e70052c434af4cc98f2408c7c41b009e = new global::System.Windows.Media.Animation.Storyboard();
var ObjectAnimationUsingKeyFrames_38c69a90cb5e46ee92bebdf846289a63 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_38c69a90cb5e46ee92bebdf846289a63,@"FirstPageButton");
var DiscreteObjectKeyFrame_f8b7397f41834592be6ca8dfbcd6e8c8 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_f8b7397f41834592be6ca8dfbcd6e8c8.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_f8b7397f41834592be6ca8dfbcd6e8c8.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_38c69a90cb5e46ee92bebdf846289a63.KeyFrames.Add(DiscreteObjectKeyFrame_f8b7397f41834592be6ca8dfbcd6e8c8);


var ObjectAnimationUsingKeyFrames_86eabba1ffa44c05bb9b346f05b81a87 = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_86eabba1ffa44c05bb9b346f05b81a87,@"LastPageButton");
var DiscreteObjectKeyFrame_132df7ce1efd4e8fb6e62554fc1e178a = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_132df7ce1efd4e8fb6e62554fc1e178a.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_132df7ce1efd4e8fb6e62554fc1e178a.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_86eabba1ffa44c05bb9b346f05b81a87.KeyFrames.Add(DiscreteObjectKeyFrame_132df7ce1efd4e8fb6e62554fc1e178a);


var ObjectAnimationUsingKeyFrames_fe428118ce17448d998640ba1444a29d = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_fe428118ce17448d998640ba1444a29d,@"CurrentPageTextBox");
var DiscreteObjectKeyFrame_88f08d89b015443392ad3ae0f86e3799 = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_88f08d89b015443392ad3ae0f86e3799.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_88f08d89b015443392ad3ae0f86e3799.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_fe428118ce17448d998640ba1444a29d.KeyFrames.Add(DiscreteObjectKeyFrame_88f08d89b015443392ad3ae0f86e3799);


var ObjectAnimationUsingKeyFrames_82ef26e9f4c147dcaa14228a2c663bba = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_82ef26e9f4c147dcaa14228a2c663bba,@"PageDisplay");
var DiscreteObjectKeyFrame_212e668927c94a99981ae81a8fc3db2d = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_212e668927c94a99981ae81a8fc3db2d.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_212e668927c94a99981ae81a8fc3db2d.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

ObjectAnimationUsingKeyFrames_82ef26e9f4c147dcaa14228a2c663bba.KeyFrames.Add(DiscreteObjectKeyFrame_212e668927c94a99981ae81a8fc3db2d);


Storyboard_e70052c434af4cc98f2408c7c41b009e.Children.Add(ObjectAnimationUsingKeyFrames_38c69a90cb5e46ee92bebdf846289a63);
Storyboard_e70052c434af4cc98f2408c7c41b009e.Children.Add(ObjectAnimationUsingKeyFrames_86eabba1ffa44c05bb9b346f05b81a87);
Storyboard_e70052c434af4cc98f2408c7c41b009e.Children.Add(ObjectAnimationUsingKeyFrames_fe428118ce17448d998640ba1444a29d);
Storyboard_e70052c434af4cc98f2408c7c41b009e.Children.Add(ObjectAnimationUsingKeyFrames_82ef26e9f4c147dcaa14228a2c663bba);


VisualState_3279838841a8491daa77f95b6c19c5bb.Storyboard = Storyboard_e70052c434af4cc98f2408c7c41b009e;


VisualStateGroup_0db846b4082747c39cfb7fc007499e8b.States.Add(VisualState_cdb8e9e4f55d4b4980098a1336bd34c1);
VisualStateGroup_0db846b4082747c39cfb7fc007499e8b.States.Add(VisualState_97b3411c5d2340b0a2fab167217199ee);
VisualStateGroup_0db846b4082747c39cfb7fc007499e8b.States.Add(VisualState_c714f0d40de84daaba56d7144967c7c0);
VisualStateGroup_0db846b4082747c39cfb7fc007499e8b.States.Add(VisualState_f590894a41ca483287bf48c453e50c42);
VisualStateGroup_0db846b4082747c39cfb7fc007499e8b.States.Add(VisualState_829b0a5021a149708619a389d66a0c39);
VisualStateGroup_0db846b4082747c39cfb7fc007499e8b.States.Add(VisualState_3279838841a8491daa77f95b6c19c5bb);


((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_0db846b4082747c39cfb7fc007499e8b);

var Border_43ffe661f57c450eb0b620a2fc29eb41 = new global::System.Windows.Controls.Border();
Border_43ffe661f57c450eb0b620a2fc29eb41.MinHeight = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"24");
Border_43ffe661f57c450eb0b620a2fc29eb41.VerticalAlignment = global::System.Windows.VerticalAlignment.Bottom;
Border_43ffe661f57c450eb0b620a2fc29eb41.CornerRadius = (global::System.Windows.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.CornerRadius), @"2");
var StackPanel_2e89527adb5d4bd69edc2233216c02ec = new global::System.Windows.Controls.StackPanel();
StackPanel_2e89527adb5d4bd69edc2233216c02ec.Orientation = global::System.Windows.Controls.Orientation.Horizontal;
StackPanel_2e89527adb5d4bd69edc2233216c02ec.VerticalAlignment = global::System.Windows.VerticalAlignment.Stretch;
var Button_38b956b481c84006a0b890678a8b9425 = new global::System.Windows.Controls.Button();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("FirstPageButton", Button_38b956b481c84006a0b890678a8b9425);
Button_38b956b481c84006a0b890678a8b9425.Name = "FirstPageButton";
Button_38b956b481c84006a0b890678a8b9425.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");
Button_38b956b481c84006a0b890678a8b9425.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");
Button_38b956b481c84006a0b890678a8b9425.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#00000000");
Button_38b956b481c84006a0b890678a8b9425.Foreground = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FF000000");
Button_38b956b481c84006a0b890678a8b9425.BorderBrush = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFFFFFFF");
Button_38b956b481c84006a0b890678a8b9425.BorderThickness = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");
Button_38b956b481c84006a0b890678a8b9425.Padding = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");
Button_38b956b481c84006a0b890678a8b9425.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Right;
Button_38b956b481c84006a0b890678a8b9425.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
var Grid_f4ec4519247e4d82a8d43b366375802d = new global::System.Windows.Controls.Grid();
Grid_f4ec4519247e4d82a8d43b366375802d.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
Grid_f4ec4519247e4d82a8d43b366375802d.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"8");
var Path_23ec9dcad4794c85aa465687641eca68 = new global::System.Windows.Shapes.Path();
Path_23ec9dcad4794c85aa465687641eca68.Stretch = global::System.Windows.Media.Stretch.Fill;
Path_23ec9dcad4794c85aa465687641eca68.Data = (global::System.Windows.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Geometry), @"M0,1 L1,0 L1,2 Z");
Path_23ec9dcad4794c85aa465687641eca68.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"5");
Path_23ec9dcad4794c85aa465687641eca68.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
Path_23ec9dcad4794c85aa465687641eca68.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Right;
var Binding_63648c0deec54d989a0cf65c99648baf = new global::System.Windows.Data.Binding();
Binding_63648c0deec54d989a0cf65c99648baf.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Foreground");
var RelativeSource_92ba41ad89da404aab7f2f39b82d8f73 = new global::System.Windows.Data.RelativeSource();
RelativeSource_92ba41ad89da404aab7f2f39b82d8f73.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_63648c0deec54d989a0cf65c99648baf.RelativeSource = RelativeSource_92ba41ad89da404aab7f2f39b82d8f73;


Binding_63648c0deec54d989a0cf65c99648baf.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


var Rectangle_0af080efd3b0400c812a86dde0d8ac70 = new global::System.Windows.Shapes.Rectangle();
Rectangle_0af080efd3b0400c812a86dde0d8ac70.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"2");
Rectangle_0af080efd3b0400c812a86dde0d8ac70.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
Rectangle_0af080efd3b0400c812a86dde0d8ac70.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Left;
var Binding_1e6a8f803d604c338dfcae2e111f2ac0 = new global::System.Windows.Data.Binding();
Binding_1e6a8f803d604c338dfcae2e111f2ac0.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Foreground");
var RelativeSource_f86c441a95ad4167a525cab717e0bead = new global::System.Windows.Data.RelativeSource();
RelativeSource_f86c441a95ad4167a525cab717e0bead.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_1e6a8f803d604c338dfcae2e111f2ac0.RelativeSource = RelativeSource_f86c441a95ad4167a525cab717e0bead;


Binding_1e6a8f803d604c338dfcae2e111f2ac0.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


Grid_f4ec4519247e4d82a8d43b366375802d.Children.Add(Path_23ec9dcad4794c85aa465687641eca68);
Grid_f4ec4519247e4d82a8d43b366375802d.Children.Add(Rectangle_0af080efd3b0400c812a86dde0d8ac70);


Button_38b956b481c84006a0b890678a8b9425.Content = Grid_f4ec4519247e4d82a8d43b366375802d;


var Button_8ee8e6f33b0d4426a5b1fcc96ad58dea = new global::System.Windows.Controls.Button();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("PreviousPageButton", Button_8ee8e6f33b0d4426a5b1fcc96ad58dea);
Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.Name = "PreviousPageButton";
Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#00000000");
Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.Foreground = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FF000000");
Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.BorderBrush = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFFFFFFF");
Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.BorderThickness = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");
Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.Padding = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");
Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");
Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");
Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Right;
Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
var Path_94c1c35de2834123a12a262069111d05 = new global::System.Windows.Shapes.Path();
Path_94c1c35de2834123a12a262069111d05.Stretch = global::System.Windows.Media.Stretch.Fill;
Path_94c1c35de2834123a12a262069111d05.Data = (global::System.Windows.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Geometry), @"M0,1 L1,0 L1,2 Z");
Path_94c1c35de2834123a12a262069111d05.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"5");
Path_94c1c35de2834123a12a262069111d05.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
Path_94c1c35de2834123a12a262069111d05.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Center;
Path_94c1c35de2834123a12a262069111d05.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
var Binding_9e0e956b45c84c0aa1a2ab5b60ed608b = new global::System.Windows.Data.Binding();
Binding_9e0e956b45c84c0aa1a2ab5b60ed608b.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Foreground");
var RelativeSource_62560e8945b84b7eaa95c8c65d2dd915 = new global::System.Windows.Data.RelativeSource();
RelativeSource_62560e8945b84b7eaa95c8c65d2dd915.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_9e0e956b45c84c0aa1a2ab5b60ed608b.RelativeSource = RelativeSource_62560e8945b84b7eaa95c8c65d2dd915;


Binding_9e0e956b45c84c0aa1a2ab5b60ed608b.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.Content = Path_94c1c35de2834123a12a262069111d05;


var Border_639202ef7dc9498cae85775b174e4c53 = new global::System.Windows.Controls.Border();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Separator1", Border_639202ef7dc9498cae85775b174e4c53);
Border_639202ef7dc9498cae85775b174e4c53.Name = "Separator1";
Border_639202ef7dc9498cae85775b174e4c53.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"2");
Border_639202ef7dc9498cae85775b174e4c53.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFCCD1D6");
Border_639202ef7dc9498cae85775b174e4c53.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0,3,0,3");
var Binding_d9b162045aef4367a8c0b21e3857e5c7 = new global::System.Windows.Data.Binding();
Binding_d9b162045aef4367a8c0b21e3857e5c7.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
var RelativeSource_2ee92e38f2b14e7c8ea7003907779225 = new global::System.Windows.Data.RelativeSource();
RelativeSource_2ee92e38f2b14e7c8ea7003907779225.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_d9b162045aef4367a8c0b21e3857e5c7.RelativeSource = RelativeSource_2ee92e38f2b14e7c8ea7003907779225;


Binding_d9b162045aef4367a8c0b21e3857e5c7.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


var StackPanel_4236c3c4298248c4a0a31e2e08687dff = new global::System.Windows.Controls.StackPanel();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("NumericButtonPanel", StackPanel_4236c3c4298248c4a0a31e2e08687dff);
StackPanel_4236c3c4298248c4a0a31e2e08687dff.Name = "NumericButtonPanel";
StackPanel_4236c3c4298248c4a0a31e2e08687dff.Orientation = global::System.Windows.Controls.Orientation.Horizontal;
StackPanel_4236c3c4298248c4a0a31e2e08687dff.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");

var StackPanel_38cdc3083ec24095b99adf32cec3f3a9 = new global::System.Windows.Controls.StackPanel();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("PageDisplay", StackPanel_38cdc3083ec24095b99adf32cec3f3a9);
StackPanel_38cdc3083ec24095b99adf32cec3f3a9.Name = "PageDisplay";
StackPanel_38cdc3083ec24095b99adf32cec3f3a9.Orientation = global::System.Windows.Controls.Orientation.Horizontal;
var TextBlock_fa22ea2fd0594848a8c2170dff3f9b0d = new global::System.Windows.Controls.TextBlock();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("CurrentPagePrefixTextBlock", TextBlock_fa22ea2fd0594848a8c2170dff3f9b0d);
TextBlock_fa22ea2fd0594848a8c2170dff3f9b0d.Name = "CurrentPagePrefixTextBlock";
TextBlock_fa22ea2fd0594848a8c2170dff3f9b0d.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"Auto");
TextBlock_fa22ea2fd0594848a8c2170dff3f9b0d.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
TextBlock_fa22ea2fd0594848a8c2170dff3f9b0d.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"4,0,0,0");
var Binding_ed9bff1ba4844238b637335bb3adf5cf = new global::System.Windows.Data.Binding();
Binding_ed9bff1ba4844238b637335bb3adf5cf.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Foreground");
var RelativeSource_ce4c6ef602424234b50cde5f03ab3bb2 = new global::System.Windows.Data.RelativeSource();
RelativeSource_ce4c6ef602424234b50cde5f03ab3bb2.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_ed9bff1ba4844238b637335bb3adf5cf.RelativeSource = RelativeSource_ce4c6ef602424234b50cde5f03ab3bb2;


Binding_ed9bff1ba4844238b637335bb3adf5cf.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


var TextBox_fadfe3e43eee4532ae383105252b8d5d = new global::System.Windows.Controls.TextBox();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("CurrentPageTextBox", TextBox_fadfe3e43eee4532ae383105252b8d5d);
TextBox_fadfe3e43eee4532ae383105252b8d5d.Name = "CurrentPageTextBox";
TextBox_fadfe3e43eee4532ae383105252b8d5d.TextWrapping = global::System.Windows.TextWrapping.Wrap;
TextBox_fadfe3e43eee4532ae383105252b8d5d.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"40");
TextBox_fadfe3e43eee4532ae383105252b8d5d.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"Auto");
TextBox_fadfe3e43eee4532ae383105252b8d5d.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
TextBox_fadfe3e43eee4532ae383105252b8d5d.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"4,2,4,2");
var Binding_092500823ad24f3c86009d157ac5dcce = new global::System.Windows.Data.Binding();
Binding_092500823ad24f3c86009d157ac5dcce.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Foreground");
var RelativeSource_e0ed835ed30941c4b175ed7add4497ee = new global::System.Windows.Data.RelativeSource();
RelativeSource_e0ed835ed30941c4b175ed7add4497ee.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_092500823ad24f3c86009d157ac5dcce.RelativeSource = RelativeSource_e0ed835ed30941c4b175ed7add4497ee;


Binding_092500823ad24f3c86009d157ac5dcce.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;

var Binding_d9bad0296bb94c968d97f95959083fca = new global::System.Windows.Data.Binding();
Binding_d9bad0296bb94c968d97f95959083fca.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
var RelativeSource_0c4c05ac32f347358b5566aba86a89fa = new global::System.Windows.Data.RelativeSource();
RelativeSource_0c4c05ac32f347358b5566aba86a89fa.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_d9bad0296bb94c968d97f95959083fca.RelativeSource = RelativeSource_0c4c05ac32f347358b5566aba86a89fa;


Binding_d9bad0296bb94c968d97f95959083fca.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


var TextBlock_ddc27ca47a454a24aefc969d4bdc2182 = new global::System.Windows.Controls.TextBlock();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("CurrentPageSuffixTextBlock", TextBlock_ddc27ca47a454a24aefc969d4bdc2182);
TextBlock_ddc27ca47a454a24aefc969d4bdc2182.Name = "CurrentPageSuffixTextBlock";
TextBlock_ddc27ca47a454a24aefc969d4bdc2182.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"Auto");
TextBlock_ddc27ca47a454a24aefc969d4bdc2182.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
TextBlock_ddc27ca47a454a24aefc969d4bdc2182.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0,0,4,0");
var Binding_4044c8f06ada47b6b654278df8d8a6f6 = new global::System.Windows.Data.Binding();
Binding_4044c8f06ada47b6b654278df8d8a6f6.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Foreground");
var RelativeSource_54716356df524a3d8c9cfec9b9d2d819 = new global::System.Windows.Data.RelativeSource();
RelativeSource_54716356df524a3d8c9cfec9b9d2d819.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_4044c8f06ada47b6b654278df8d8a6f6.RelativeSource = RelativeSource_54716356df524a3d8c9cfec9b9d2d819;


Binding_4044c8f06ada47b6b654278df8d8a6f6.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


StackPanel_38cdc3083ec24095b99adf32cec3f3a9.Children.Add(TextBlock_fa22ea2fd0594848a8c2170dff3f9b0d);
StackPanel_38cdc3083ec24095b99adf32cec3f3a9.Children.Add(TextBox_fadfe3e43eee4532ae383105252b8d5d);
StackPanel_38cdc3083ec24095b99adf32cec3f3a9.Children.Add(TextBlock_ddc27ca47a454a24aefc969d4bdc2182);


var Border_69af78d4c920434791175ccf44c0b961 = new global::System.Windows.Controls.Border();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Separator2", Border_69af78d4c920434791175ccf44c0b961);
Border_69af78d4c920434791175ccf44c0b961.Name = "Separator2";
Border_69af78d4c920434791175ccf44c0b961.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"2");
Border_69af78d4c920434791175ccf44c0b961.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFCCD1D6");
Border_69af78d4c920434791175ccf44c0b961.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0,3,0,3");
var Binding_c3c52295d24545cf9d424280c8bea170 = new global::System.Windows.Data.Binding();
Binding_c3c52295d24545cf9d424280c8bea170.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
var RelativeSource_8fc685af76444c288fb1f6c6bedc684b = new global::System.Windows.Data.RelativeSource();
RelativeSource_8fc685af76444c288fb1f6c6bedc684b.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_c3c52295d24545cf9d424280c8bea170.RelativeSource = RelativeSource_8fc685af76444c288fb1f6c6bedc684b;


Binding_c3c52295d24545cf9d424280c8bea170.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


var Button_9de873ee7702417f9a415751e6737b1a = new global::System.Windows.Controls.Button();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("NextPageButton", Button_9de873ee7702417f9a415751e6737b1a);
Button_9de873ee7702417f9a415751e6737b1a.Name = "NextPageButton";
Button_9de873ee7702417f9a415751e6737b1a.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#00000000");
Button_9de873ee7702417f9a415751e6737b1a.Foreground = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FF000000");
Button_9de873ee7702417f9a415751e6737b1a.BorderBrush = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFFFFFFF");
Button_9de873ee7702417f9a415751e6737b1a.BorderThickness = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");
Button_9de873ee7702417f9a415751e6737b1a.Padding = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");
Button_9de873ee7702417f9a415751e6737b1a.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");
Button_9de873ee7702417f9a415751e6737b1a.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");
Button_9de873ee7702417f9a415751e6737b1a.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Right;
Button_9de873ee7702417f9a415751e6737b1a.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
var Path_4f9b5bba365a483bb478d9f9ac4ec930 = new global::System.Windows.Shapes.Path();
Path_4f9b5bba365a483bb478d9f9ac4ec930.Stretch = global::System.Windows.Media.Stretch.Fill;
Path_4f9b5bba365a483bb478d9f9ac4ec930.Data = (global::System.Windows.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Geometry), @"M0,0 L1,1 L0,2 Z");
Path_4f9b5bba365a483bb478d9f9ac4ec930.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"5");
Path_4f9b5bba365a483bb478d9f9ac4ec930.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
Path_4f9b5bba365a483bb478d9f9ac4ec930.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Center;
Path_4f9b5bba365a483bb478d9f9ac4ec930.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
var Binding_8af020caca124b559dfc0349558ec4b6 = new global::System.Windows.Data.Binding();
Binding_8af020caca124b559dfc0349558ec4b6.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Foreground");
var RelativeSource_ed5de920d86443e49076d92682e48e18 = new global::System.Windows.Data.RelativeSource();
RelativeSource_ed5de920d86443e49076d92682e48e18.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_8af020caca124b559dfc0349558ec4b6.RelativeSource = RelativeSource_ed5de920d86443e49076d92682e48e18;


Binding_8af020caca124b559dfc0349558ec4b6.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


Button_9de873ee7702417f9a415751e6737b1a.Content = Path_4f9b5bba365a483bb478d9f9ac4ec930;


var Button_f2704b84b9c24610bf8082cfa31d98b8 = new global::System.Windows.Controls.Button();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("LastPageButton", Button_f2704b84b9c24610bf8082cfa31d98b8);
Button_f2704b84b9c24610bf8082cfa31d98b8.Name = "LastPageButton";
Button_f2704b84b9c24610bf8082cfa31d98b8.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#00000000");
Button_f2704b84b9c24610bf8082cfa31d98b8.Foreground = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FF000000");
Button_f2704b84b9c24610bf8082cfa31d98b8.BorderBrush = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFFFFFFF");
Button_f2704b84b9c24610bf8082cfa31d98b8.BorderThickness = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");
Button_f2704b84b9c24610bf8082cfa31d98b8.Padding = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");
Button_f2704b84b9c24610bf8082cfa31d98b8.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");
Button_f2704b84b9c24610bf8082cfa31d98b8.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");
Button_f2704b84b9c24610bf8082cfa31d98b8.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Right;
Button_f2704b84b9c24610bf8082cfa31d98b8.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
var Grid_1cecb19f596b4d96be2ec17d74b49067 = new global::System.Windows.Controls.Grid();
Grid_1cecb19f596b4d96be2ec17d74b49067.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
Grid_1cecb19f596b4d96be2ec17d74b49067.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"8");
var Path_85ed49895fe7483aa73bc92beee55e70 = new global::System.Windows.Shapes.Path();
Path_85ed49895fe7483aa73bc92beee55e70.Stretch = global::System.Windows.Media.Stretch.Fill;
Path_85ed49895fe7483aa73bc92beee55e70.Data = (global::System.Windows.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Geometry), @"M0,0 L1,1 L0,2 Z");
Path_85ed49895fe7483aa73bc92beee55e70.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"5");
Path_85ed49895fe7483aa73bc92beee55e70.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
Path_85ed49895fe7483aa73bc92beee55e70.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Left;
var Binding_a7b96098bf2c4760b28062b881d18f24 = new global::System.Windows.Data.Binding();
Binding_a7b96098bf2c4760b28062b881d18f24.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Foreground");
var RelativeSource_abf3915587cf4438adcc20a22d9d5346 = new global::System.Windows.Data.RelativeSource();
RelativeSource_abf3915587cf4438adcc20a22d9d5346.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_a7b96098bf2c4760b28062b881d18f24.RelativeSource = RelativeSource_abf3915587cf4438adcc20a22d9d5346;


Binding_a7b96098bf2c4760b28062b881d18f24.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


var Rectangle_c9683c1bc950412d8772b863b9d408cc = new global::System.Windows.Shapes.Rectangle();
Rectangle_c9683c1bc950412d8772b863b9d408cc.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"2");
Rectangle_c9683c1bc950412d8772b863b9d408cc.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
Rectangle_c9683c1bc950412d8772b863b9d408cc.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Right;
var Binding_62c4e695061d458d8ab29dd04e1adec6 = new global::System.Windows.Data.Binding();
Binding_62c4e695061d458d8ab29dd04e1adec6.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Foreground");
var RelativeSource_f1f7fa3953ee466aaead95427cf9c797 = new global::System.Windows.Data.RelativeSource();
RelativeSource_f1f7fa3953ee466aaead95427cf9c797.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_62c4e695061d458d8ab29dd04e1adec6.RelativeSource = RelativeSource_f1f7fa3953ee466aaead95427cf9c797;


Binding_62c4e695061d458d8ab29dd04e1adec6.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


Grid_1cecb19f596b4d96be2ec17d74b49067.Children.Add(Path_85ed49895fe7483aa73bc92beee55e70);
Grid_1cecb19f596b4d96be2ec17d74b49067.Children.Add(Rectangle_c9683c1bc950412d8772b863b9d408cc);


Button_f2704b84b9c24610bf8082cfa31d98b8.Content = Grid_1cecb19f596b4d96be2ec17d74b49067;


StackPanel_2e89527adb5d4bd69edc2233216c02ec.Children.Add(Button_38b956b481c84006a0b890678a8b9425);
StackPanel_2e89527adb5d4bd69edc2233216c02ec.Children.Add(Button_8ee8e6f33b0d4426a5b1fcc96ad58dea);
StackPanel_2e89527adb5d4bd69edc2233216c02ec.Children.Add(Border_639202ef7dc9498cae85775b174e4c53);
StackPanel_2e89527adb5d4bd69edc2233216c02ec.Children.Add(StackPanel_4236c3c4298248c4a0a31e2e08687dff);
StackPanel_2e89527adb5d4bd69edc2233216c02ec.Children.Add(StackPanel_38cdc3083ec24095b99adf32cec3f3a9);
StackPanel_2e89527adb5d4bd69edc2233216c02ec.Children.Add(Border_69af78d4c920434791175ccf44c0b961);
StackPanel_2e89527adb5d4bd69edc2233216c02ec.Children.Add(Button_9de873ee7702417f9a415751e6737b1a);
StackPanel_2e89527adb5d4bd69edc2233216c02ec.Children.Add(Button_f2704b84b9c24610bf8082cfa31d98b8);

var Binding_b651b5541e11467bbad2c3d971bb9e76 = new global::System.Windows.Data.Binding();
Binding_b651b5541e11467bbad2c3d971bb9e76.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"HorizontalContentAlignment");
var RelativeSource_9c404097039e4194a50402edc421686a = new global::System.Windows.Data.RelativeSource();
RelativeSource_9c404097039e4194a50402edc421686a.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_b651b5541e11467bbad2c3d971bb9e76.RelativeSource = RelativeSource_9c404097039e4194a50402edc421686a;


Binding_b651b5541e11467bbad2c3d971bb9e76.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


Border_43ffe661f57c450eb0b620a2fc29eb41.Child = StackPanel_2e89527adb5d4bd69edc2233216c02ec;

var Binding_cb5f54dc0196427d87db4c7e1086faeb = new global::System.Windows.Data.Binding();
Binding_cb5f54dc0196427d87db4c7e1086faeb.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
var RelativeSource_f53f717c479c4bf59bef7d478ecfbd33 = new global::System.Windows.Data.RelativeSource();
RelativeSource_f53f717c479c4bf59bef7d478ecfbd33.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_cb5f54dc0196427d87db4c7e1086faeb.RelativeSource = RelativeSource_f53f717c479c4bf59bef7d478ecfbd33;


Binding_cb5f54dc0196427d87db4c7e1086faeb.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;

var Binding_52c1e6bd3a0c4432bdf049f8bb660a7d = new global::System.Windows.Data.Binding();
Binding_52c1e6bd3a0c4432bdf049f8bb660a7d.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
var RelativeSource_592f64390d8c4cbf9bdbe1ad4b8b5d0f = new global::System.Windows.Data.RelativeSource();
RelativeSource_592f64390d8c4cbf9bdbe1ad4b8b5d0f.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_52c1e6bd3a0c4432bdf049f8bb660a7d.RelativeSource = RelativeSource_592f64390d8c4cbf9bdbe1ad4b8b5d0f;


Binding_52c1e6bd3a0c4432bdf049f8bb660a7d.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;

var Binding_69c4761a3f0d4ab7b2d29190241c6cff = new global::System.Windows.Data.Binding();
Binding_69c4761a3f0d4ab7b2d29190241c6cff.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
var RelativeSource_d7ab337308754e7383c1077f31013ade = new global::System.Windows.Data.RelativeSource();
RelativeSource_d7ab337308754e7383c1077f31013ade.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_69c4761a3f0d4ab7b2d29190241c6cff.RelativeSource = RelativeSource_d7ab337308754e7383c1077f31013ade;


Binding_69c4761a3f0d4ab7b2d29190241c6cff.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;

var Binding_346c587373a34059860dbbba28cd13db = new global::System.Windows.Data.Binding();
Binding_346c587373a34059860dbbba28cd13db.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Padding");
var RelativeSource_6038fc116d254f208b7dd64029d7974e = new global::System.Windows.Data.RelativeSource();
RelativeSource_6038fc116d254f208b7dd64029d7974e.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_346c587373a34059860dbbba28cd13db.RelativeSource = RelativeSource_6038fc116d254f208b7dd64029d7974e;


Binding_346c587373a34059860dbbba28cd13db.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


Grid_7f4ee1c24187438c85695ea996c43dd0.Children.Add(Border_43ffe661f57c450eb0b620a2fc29eb41);



Path_23ec9dcad4794c85aa465687641eca68.SetBinding(global::System.Windows.Shapes.Shape.FillProperty, Binding_63648c0deec54d989a0cf65c99648baf);
Rectangle_0af080efd3b0400c812a86dde0d8ac70.SetBinding(global::System.Windows.Shapes.Shape.FillProperty, Binding_1e6a8f803d604c338dfcae2e111f2ac0);
Path_94c1c35de2834123a12a262069111d05.SetBinding(global::System.Windows.Shapes.Shape.FillProperty, Binding_9e0e956b45c84c0aa1a2ab5b60ed608b);
Border_639202ef7dc9498cae85775b174e4c53.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_d9b162045aef4367a8c0b21e3857e5c7);
TextBlock_fa22ea2fd0594848a8c2170dff3f9b0d.SetBinding(global::System.Windows.Controls.Control.ForegroundProperty, Binding_ed9bff1ba4844238b637335bb3adf5cf);
TextBox_fadfe3e43eee4532ae383105252b8d5d.SetBinding(global::System.Windows.Controls.Control.ForegroundProperty, Binding_092500823ad24f3c86009d157ac5dcce);
TextBox_fadfe3e43eee4532ae383105252b8d5d.SetBinding(global::System.Windows.Controls.Control.BorderBrushProperty, Binding_d9bad0296bb94c968d97f95959083fca);
TextBlock_ddc27ca47a454a24aefc969d4bdc2182.SetBinding(global::System.Windows.Controls.Control.ForegroundProperty, Binding_4044c8f06ada47b6b654278df8d8a6f6);
Border_69af78d4c920434791175ccf44c0b961.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_c3c52295d24545cf9d424280c8bea170);
Path_4f9b5bba365a483bb478d9f9ac4ec930.SetBinding(global::System.Windows.Shapes.Shape.FillProperty, Binding_8af020caca124b559dfc0349558ec4b6);
Path_85ed49895fe7483aa73bc92beee55e70.SetBinding(global::System.Windows.Shapes.Shape.FillProperty, Binding_a7b96098bf2c4760b28062b881d18f24);
Rectangle_c9683c1bc950412d8772b863b9d408cc.SetBinding(global::System.Windows.Shapes.Shape.FillProperty, Binding_62c4e695061d458d8ab29dd04e1adec6);
StackPanel_2e89527adb5d4bd69edc2233216c02ec.SetBinding(global::System.Windows.FrameworkElement.HorizontalAlignmentProperty, Binding_b651b5541e11467bbad2c3d971bb9e76);
Border_43ffe661f57c450eb0b620a2fc29eb41.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_cb5f54dc0196427d87db4c7e1086faeb);
Border_43ffe661f57c450eb0b620a2fc29eb41.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_52c1e6bd3a0c4432bdf049f8bb660a7d);
Border_43ffe661f57c450eb0b620a2fc29eb41.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_69c4761a3f0d4ab7b2d29190241c6cff);
Border_43ffe661f57c450eb0b620a2fc29eb41.SetBinding(global::System.Windows.Controls.Border.PaddingProperty, Binding_346c587373a34059860dbbba28cd13db);

global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_1b4d05d2581149419affafa47f0da001,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_e4c067ed50db4a1493459f6d6c44200f,
        setVisualStateProperty_e4c067ed50db4a1493459f6d6c44200f,
        setLocalVisualStateProperty_e4c067ed50db4a1493459f6d6c44200f,
        getVisualStateProperty_e4c067ed50db4a1493459f6d6c44200f));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_1b4d05d2581149419affafa47f0da001, Button_9de873ee7702417f9a415751e6737b1a);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_f554248a2dae432db7f6ebb0e9052863,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_25a037c483174582b5b8ff6e591fa34f,
        setVisualStateProperty_25a037c483174582b5b8ff6e591fa34f,
        setLocalVisualStateProperty_25a037c483174582b5b8ff6e591fa34f,
        getVisualStateProperty_25a037c483174582b5b8ff6e591fa34f));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_f554248a2dae432db7f6ebb0e9052863, Button_8ee8e6f33b0d4426a5b1fcc96ad58dea);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_492215f6c9d140c38c7a8a9b81f1f512,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_310181e043004846999fcd1fd825a8b6,
        setVisualStateProperty_310181e043004846999fcd1fd825a8b6,
        setLocalVisualStateProperty_310181e043004846999fcd1fd825a8b6,
        getVisualStateProperty_310181e043004846999fcd1fd825a8b6));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_492215f6c9d140c38c7a8a9b81f1f512, TextBox_fadfe3e43eee4532ae383105252b8d5d);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_52c8493a678348afac140010c5c94cd4,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_92bb952c168248e48af0c8f0c2f4476c,
        setVisualStateProperty_92bb952c168248e48af0c8f0c2f4476c,
        setLocalVisualStateProperty_92bb952c168248e48af0c8f0c2f4476c,
        getVisualStateProperty_92bb952c168248e48af0c8f0c2f4476c));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_52c8493a678348afac140010c5c94cd4, StackPanel_38cdc3083ec24095b99adf32cec3f3a9);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_83430102a5a64417ab18dad8fd48f51e,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_31a81299a7e94144bfbb2d7c3098a50e,
        setVisualStateProperty_31a81299a7e94144bfbb2d7c3098a50e,
        setLocalVisualStateProperty_31a81299a7e94144bfbb2d7c3098a50e,
        getVisualStateProperty_31a81299a7e94144bfbb2d7c3098a50e));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_83430102a5a64417ab18dad8fd48f51e, StackPanel_4236c3c4298248c4a0a31e2e08687dff);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_90cde9f5245749d4af1f42fb1eae22d5,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_e54f799f2101425eae897770a57938ca,
        setVisualStateProperty_e54f799f2101425eae897770a57938ca,
        setLocalVisualStateProperty_e54f799f2101425eae897770a57938ca,
        getVisualStateProperty_e54f799f2101425eae897770a57938ca));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_90cde9f5245749d4af1f42fb1eae22d5, TextBox_fadfe3e43eee4532ae383105252b8d5d);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_abf89c881cc345c785eb2672a963604b,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_de55a2edc4954dc2957579cc3b6081bc,
        setVisualStateProperty_de55a2edc4954dc2957579cc3b6081bc,
        setLocalVisualStateProperty_de55a2edc4954dc2957579cc3b6081bc,
        getVisualStateProperty_de55a2edc4954dc2957579cc3b6081bc));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_abf89c881cc345c785eb2672a963604b, StackPanel_38cdc3083ec24095b99adf32cec3f3a9);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_b0cdd1499d1d4362841d0a2240d2a7bc,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_c36c66a63b084180872fe4f6bd226cfe,
        setVisualStateProperty_c36c66a63b084180872fe4f6bd226cfe,
        setLocalVisualStateProperty_c36c66a63b084180872fe4f6bd226cfe,
        getVisualStateProperty_c36c66a63b084180872fe4f6bd226cfe));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_b0cdd1499d1d4362841d0a2240d2a7bc, Button_38b956b481c84006a0b890678a8b9425);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_bbda902d40a547d897d1f607045f5776,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_ad8d0039b8a4499f9fd1032501ea81d5,
        setVisualStateProperty_ad8d0039b8a4499f9fd1032501ea81d5,
        setLocalVisualStateProperty_ad8d0039b8a4499f9fd1032501ea81d5,
        getVisualStateProperty_ad8d0039b8a4499f9fd1032501ea81d5));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_bbda902d40a547d897d1f607045f5776, Button_f2704b84b9c24610bf8082cfa31d98b8);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_37259e069f894df4a5bb3a1aaf08cf9a,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_b8d7583c61994c808ab5aa00dbc46386,
        setVisualStateProperty_b8d7583c61994c808ab5aa00dbc46386,
        setLocalVisualStateProperty_b8d7583c61994c808ab5aa00dbc46386,
        getVisualStateProperty_b8d7583c61994c808ab5aa00dbc46386));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_37259e069f894df4a5bb3a1aaf08cf9a, Button_9de873ee7702417f9a415751e6737b1a);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_4a6eef40a2704fa294e717216b1a641f,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_f1d0cdc46f5b4110b414c009ffb63c0a,
        setVisualStateProperty_f1d0cdc46f5b4110b414c009ffb63c0a,
        setLocalVisualStateProperty_f1d0cdc46f5b4110b414c009ffb63c0a,
        getVisualStateProperty_f1d0cdc46f5b4110b414c009ffb63c0a));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_4a6eef40a2704fa294e717216b1a641f, Button_8ee8e6f33b0d4426a5b1fcc96ad58dea);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_5d6117e1885f4282bdaf3569ead6bd51,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_cf7c1d4daafe484bbc4736b387b61ab9,
        setVisualStateProperty_cf7c1d4daafe484bbc4736b387b61ab9,
        setLocalVisualStateProperty_cf7c1d4daafe484bbc4736b387b61ab9,
        getVisualStateProperty_cf7c1d4daafe484bbc4736b387b61ab9));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_5d6117e1885f4282bdaf3569ead6bd51, TextBox_fadfe3e43eee4532ae383105252b8d5d);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_f60cb089a8db45c3a5571057b20675e2,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_4064e88d2988479aacc4d848ee5f2e52,
        setVisualStateProperty_4064e88d2988479aacc4d848ee5f2e52,
        setLocalVisualStateProperty_4064e88d2988479aacc4d848ee5f2e52,
        getVisualStateProperty_4064e88d2988479aacc4d848ee5f2e52));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_f60cb089a8db45c3a5571057b20675e2, StackPanel_38cdc3083ec24095b99adf32cec3f3a9);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_560ac39113924ef0bdcbd3fed2a0872b,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_7670580d752a45d0972587bb549a9ab6,
        setVisualStateProperty_7670580d752a45d0972587bb549a9ab6,
        setLocalVisualStateProperty_7670580d752a45d0972587bb549a9ab6,
        getVisualStateProperty_7670580d752a45d0972587bb549a9ab6));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_560ac39113924ef0bdcbd3fed2a0872b, Border_639202ef7dc9498cae85775b174e4c53);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_ed971e57f6b242a18353cff9222679ea,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_6e581dd54e344d7e94f57c0978c290e6,
        setVisualStateProperty_6e581dd54e344d7e94f57c0978c290e6,
        setLocalVisualStateProperty_6e581dd54e344d7e94f57c0978c290e6,
        getVisualStateProperty_6e581dd54e344d7e94f57c0978c290e6));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_ed971e57f6b242a18353cff9222679ea, Border_69af78d4c920434791175ccf44c0b961);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_ed84f6c7f82a4d9aa9c5629f8173ea6a,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_996462726a7744be8bb086c73f96ff57,
        setVisualStateProperty_996462726a7744be8bb086c73f96ff57,
        setLocalVisualStateProperty_996462726a7744be8bb086c73f96ff57,
        getVisualStateProperty_996462726a7744be8bb086c73f96ff57));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_ed84f6c7f82a4d9aa9c5629f8173ea6a, Button_38b956b481c84006a0b890678a8b9425);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_6d6892eae02f41fdbb17fb652f164b51,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_fb5773338a444cbc8bd2cc90ff66f7b2,
        setVisualStateProperty_fb5773338a444cbc8bd2cc90ff66f7b2,
        setLocalVisualStateProperty_fb5773338a444cbc8bd2cc90ff66f7b2,
        getVisualStateProperty_fb5773338a444cbc8bd2cc90ff66f7b2));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_6d6892eae02f41fdbb17fb652f164b51, Button_f2704b84b9c24610bf8082cfa31d98b8);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_68d4da37033d42bd91210437952956a6,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_ddc25f31b9da44e9a6e85d92302d793a,
        setVisualStateProperty_ddc25f31b9da44e9a6e85d92302d793a,
        setLocalVisualStateProperty_ddc25f31b9da44e9a6e85d92302d793a,
        getVisualStateProperty_ddc25f31b9da44e9a6e85d92302d793a));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_68d4da37033d42bd91210437952956a6, StackPanel_4236c3c4298248c4a0a31e2e08687dff);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_38c69a90cb5e46ee92bebdf846289a63,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_586d9fee14b642098172624535a9b0e1,
        setVisualStateProperty_586d9fee14b642098172624535a9b0e1,
        setLocalVisualStateProperty_586d9fee14b642098172624535a9b0e1,
        getVisualStateProperty_586d9fee14b642098172624535a9b0e1));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_38c69a90cb5e46ee92bebdf846289a63, Button_38b956b481c84006a0b890678a8b9425);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_86eabba1ffa44c05bb9b346f05b81a87,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_cd347efb57c54637b1df77c18e921404,
        setVisualStateProperty_cd347efb57c54637b1df77c18e921404,
        setLocalVisualStateProperty_cd347efb57c54637b1df77c18e921404,
        getVisualStateProperty_cd347efb57c54637b1df77c18e921404));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_86eabba1ffa44c05bb9b346f05b81a87, Button_f2704b84b9c24610bf8082cfa31d98b8);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_fe428118ce17448d998640ba1444a29d,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_95f930ac5c8e479bbb22a702196e76c5,
        setVisualStateProperty_95f930ac5c8e479bbb22a702196e76c5,
        setLocalVisualStateProperty_95f930ac5c8e479bbb22a702196e76c5,
        getVisualStateProperty_95f930ac5c8e479bbb22a702196e76c5));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_fe428118ce17448d998640ba1444a29d, TextBox_fadfe3e43eee4532ae383105252b8d5d);


global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_82ef26e9f4c147dcaa14228a2c663bba,
    new global::System.Windows.PropertyPath(
        "Visibility",
        "Visibility",
        accessVisualStateProperty_5a58a72b322f4b59ba82da3ba51062b7,
        setVisualStateProperty_5a58a72b322f4b59ba82da3ba51062b7,
        setLocalVisualStateProperty_5a58a72b322f4b59ba82da3ba51062b7,
        getVisualStateProperty_5a58a72b322f4b59ba82da3ba51062b7));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_82ef26e9f4c147dcaa14228a2c663bba, StackPanel_38cdc3083ec24095b99adf32cec3f3a9);

templateInstance_02c86289167443ce9f58fee5a926ce26.TemplateContent = Grid_7f4ee1c24187438c85695ea996c43dd0;
return templateInstance_02c86289167443ce9f58fee5a926ce26;
        }



        }
}
#else

namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultDataPagerStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {





                var Style_48a9c2aac1b04f5cb334bba0fd1621cb = new global::Windows.UI.Xaml.Style();
                Style_48a9c2aac1b04f5cb334bba0fd1621cb.TargetType = typeof(global::Windows.UI.Xaml.Controls.DataPager);
                var Setter_ed6e2f4d7175424d8bc471dc4418eaf6 = new global::Windows.UI.Xaml.Setter();
                Setter_ed6e2f4d7175424d8bc471dc4418eaf6.Property = global::Windows.UI.Xaml.Controls.DataPager.BackgroundProperty;
                Setter_ed6e2f4d7175424d8bc471dc4418eaf6.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFF2F3F4");

                var Setter_33166b0c3bd2454697a6d5921071a7f5 = new global::Windows.UI.Xaml.Setter();
                Setter_33166b0c3bd2454697a6d5921071a7f5.Property = global::Windows.UI.Xaml.Controls.DataPager.BorderBrushProperty;
                Setter_33166b0c3bd2454697a6d5921071a7f5.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Gray");

                var Setter_c6aed2d9832c458d9520400e7c14701a = new global::Windows.UI.Xaml.Setter();
                Setter_c6aed2d9832c458d9520400e7c14701a.Property = global::Windows.UI.Xaml.Controls.DataPager.BorderThicknessProperty;
                Setter_c6aed2d9832c458d9520400e7c14701a.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");

                var Setter_1bb7437e412c409183cde6250da23c62 = new global::Windows.UI.Xaml.Setter();
                Setter_1bb7437e412c409183cde6250da23c62.Property = global::Windows.UI.Xaml.Controls.DataPager.HorizontalContentAlignmentProperty;
                Setter_1bb7437e412c409183cde6250da23c62.Value = global::Windows.UI.Xaml.HorizontalAlignment.Right;

                var Setter_46e707e78f91413190b404c80f5cb905 = new global::Windows.UI.Xaml.Setter();
                Setter_46e707e78f91413190b404c80f5cb905.Property = global::Windows.UI.Xaml.Controls.DataPager.NumericButtonStyleProperty;
                var Style_ed7f2c6193174303806b992d6e7b9201 = new global::Windows.UI.Xaml.Style();
                Style_ed7f2c6193174303806b992d6e7b9201.TargetType = typeof(global::Windows.UI.Xaml.Controls.Primitives.ToggleButton);
                var Setter_70b001c20fb34d6fbe658188fddc4b5c = new global::Windows.UI.Xaml.Setter();
                Setter_70b001c20fb34d6fbe658188fddc4b5c.Property = global::Windows.UI.Xaml.Controls.Primitives.ToggleButton.MinHeightProperty;
                Setter_70b001c20fb34d6fbe658188fddc4b5c.Value = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");

                var Setter_79231c144ded4368a05a1dcadcaa6d14 = new global::Windows.UI.Xaml.Setter();
                Setter_79231c144ded4368a05a1dcadcaa6d14.Property = global::Windows.UI.Xaml.Controls.Primitives.ToggleButton.MinWidthProperty;
                Setter_79231c144ded4368a05a1dcadcaa6d14.Value = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");

                var Setter_0b4fbcafdc8945109fda24290a750d89 = new global::Windows.UI.Xaml.Setter();
                Setter_0b4fbcafdc8945109fda24290a750d89.Property = global::Windows.UI.Xaml.Controls.Primitives.ToggleButton.HorizontalAlignmentProperty;
                Setter_0b4fbcafdc8945109fda24290a750d89.Value = global::Windows.UI.Xaml.HorizontalAlignment.Right;

                var Setter_1f244a5e00904369931958694b87db58 = new global::Windows.UI.Xaml.Setter();
                Setter_1f244a5e00904369931958694b87db58.Property = global::Windows.UI.Xaml.Controls.Primitives.ToggleButton.VerticalAlignmentProperty;
                Setter_1f244a5e00904369931958694b87db58.Value = global::Windows.UI.Xaml.VerticalAlignment.Center;

                var Setter_d1b64aecb8a7456091240c8d2b293e17 = new global::Windows.UI.Xaml.Setter();
                Setter_d1b64aecb8a7456091240c8d2b293e17.Property = global::Windows.UI.Xaml.Controls.Primitives.ToggleButton.BackgroundProperty;
                Setter_d1b64aecb8a7456091240c8d2b293e17.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#00000000");

                var Setter_fe20409de6fb4b31ac5c303492679632 = new global::Windows.UI.Xaml.Setter();
                Setter_fe20409de6fb4b31ac5c303492679632.Property = global::Windows.UI.Xaml.Controls.Primitives.ToggleButton.BorderThicknessProperty;
                Setter_fe20409de6fb4b31ac5c303492679632.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");

                var Setter_066bd31647d24fb0adbe856c49adf5d6 = new global::Windows.UI.Xaml.Setter();
                Setter_066bd31647d24fb0adbe856c49adf5d6.Property = global::Windows.UI.Xaml.Controls.Primitives.ToggleButton.PaddingProperty;
                Setter_066bd31647d24fb0adbe856c49adf5d6.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");

                var Setter_913db9820abb4904b65ac65c27441595 = new global::Windows.UI.Xaml.Setter();
                Setter_913db9820abb4904b65ac65c27441595.Property = global::Windows.UI.Xaml.Controls.Primitives.ToggleButton.TemplateProperty;
                var ControlTemplate_ec87bd0495374250bda6c17ada3699b5 = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_ec87bd0495374250bda6c17ada3699b5.TargetType = typeof(global::Windows.UI.Xaml.Controls.Primitives.ToggleButton);
                ControlTemplate_ec87bd0495374250bda6c17ada3699b5.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_ec87bd0495374250bda6c17ada3699b5);

                Setter_913db9820abb4904b65ac65c27441595.Value = ControlTemplate_ec87bd0495374250bda6c17ada3699b5;


                Style_ed7f2c6193174303806b992d6e7b9201.Setters.Add(Setter_70b001c20fb34d6fbe658188fddc4b5c);
                Style_ed7f2c6193174303806b992d6e7b9201.Setters.Add(Setter_79231c144ded4368a05a1dcadcaa6d14);
                Style_ed7f2c6193174303806b992d6e7b9201.Setters.Add(Setter_0b4fbcafdc8945109fda24290a750d89);
                Style_ed7f2c6193174303806b992d6e7b9201.Setters.Add(Setter_1f244a5e00904369931958694b87db58);
                Style_ed7f2c6193174303806b992d6e7b9201.Setters.Add(Setter_d1b64aecb8a7456091240c8d2b293e17);
                Style_ed7f2c6193174303806b992d6e7b9201.Setters.Add(Setter_fe20409de6fb4b31ac5c303492679632);
                Style_ed7f2c6193174303806b992d6e7b9201.Setters.Add(Setter_066bd31647d24fb0adbe856c49adf5d6);
                Style_ed7f2c6193174303806b992d6e7b9201.Setters.Add(Setter_913db9820abb4904b65ac65c27441595);


                Setter_46e707e78f91413190b404c80f5cb905.Value = Style_ed7f2c6193174303806b992d6e7b9201;


                var Setter_14827f7092e74b0fb6f99ffe408a59e5 = new global::Windows.UI.Xaml.Setter();
                Setter_14827f7092e74b0fb6f99ffe408a59e5.Property = global::Windows.UI.Xaml.Controls.DataPager.TemplateProperty;
                var ControlTemplate_25729cbfa664423389a71921ef5f672f = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_25729cbfa664423389a71921ef5f672f.TargetType = typeof(global::Windows.UI.Xaml.Controls.DataPager);
                ControlTemplate_25729cbfa664423389a71921ef5f672f.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_25729cbfa664423389a71921ef5f672f);

                Setter_14827f7092e74b0fb6f99ffe408a59e5.Value = ControlTemplate_25729cbfa664423389a71921ef5f672f;


                Style_48a9c2aac1b04f5cb334bba0fd1621cb.Setters.Add(Setter_ed6e2f4d7175424d8bc471dc4418eaf6);
                Style_48a9c2aac1b04f5cb334bba0fd1621cb.Setters.Add(Setter_33166b0c3bd2454697a6d5921071a7f5);
                Style_48a9c2aac1b04f5cb334bba0fd1621cb.Setters.Add(Setter_c6aed2d9832c458d9520400e7c14701a);
                Style_48a9c2aac1b04f5cb334bba0fd1621cb.Setters.Add(Setter_1bb7437e412c409183cde6250da23c62);
                Style_48a9c2aac1b04f5cb334bba0fd1621cb.Setters.Add(Setter_46e707e78f91413190b404c80f5cb905);
                Style_48a9c2aac1b04f5cb334bba0fd1621cb.Setters.Add(Setter_14827f7092e74b0fb6f99ffe408a59e5);


                DefaultStyle = Style_48a9c2aac1b04f5cb334bba0fd1621cb;
            }
            return DefaultStyle;







        }



        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_bd0c50e0aa094ae88569d0a5bc3c07e1(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_bd0c50e0aa094ae88569d0a5bc3c07e1(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_bd0c50e0aa094ae88569d0a5bc3c07e1(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_bd0c50e0aa094ae88569d0a5bc3c07e1(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_a456d3d23a5d4f9482dde4f726c3e361(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_a456d3d23a5d4f9482dde4f726c3e361(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_a456d3d23a5d4f9482dde4f726c3e361(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_a456d3d23a5d4f9482dde4f726c3e361(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_2996c86077b74db1a44e79b444cd3d90(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_2996c86077b74db1a44e79b444cd3d90(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_2996c86077b74db1a44e79b444cd3d90(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_2996c86077b74db1a44e79b444cd3d90(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Opacity").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("OpacityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_ec87bd0495374250bda6c17ada3699b5(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_775e6fcf22224272900c5397b4c5f5f6 = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_775e6fcf22224272900c5397b4c5f5f6.TemplateOwner = templateOwner;
            var Grid_94469a9e5eed451bac82ae6b7812395d = new global::Windows.UI.Xaml.Controls.Grid();
            var VisualStateGroup_3de8c62808724aa8adf0baa139318a0c = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_3de8c62808724aa8adf0baa139318a0c);
            VisualStateGroup_3de8c62808724aa8adf0baa139318a0c.Name = "CommonStates";
            var VisualState_56bda82cbdbb4281a202da2b874b4056 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Normal", VisualState_56bda82cbdbb4281a202da2b874b4056);
            VisualState_56bda82cbdbb4281a202da2b874b4056.Name = "Normal";

            var VisualState_117aff25cef1480e9f47add7ddf01a9d = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("MouseOver", VisualState_117aff25cef1480e9f47add7ddf01a9d);
            VisualState_117aff25cef1480e9f47add7ddf01a9d.Name = "MouseOver";

            var VisualState_f381c250f2004d6fa8e607077108baf6 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Pressed", VisualState_f381c250f2004d6fa8e607077108baf6);
            VisualState_f381c250f2004d6fa8e607077108baf6.Name = "Pressed";

            var VisualState_48d4766d0dc044c3a93d66b68fa24ee7 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_48d4766d0dc044c3a93d66b68fa24ee7);
            VisualState_48d4766d0dc044c3a93d66b68fa24ee7.Name = "Disabled";
            var Storyboard_da2c2b3acf6a4b58bd788c98c45884de = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_1fe72b56484c4726829e2975a58545aa = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_1fe72b56484c4726829e2975a58545aa, @"contentPresenter");
            var DiscreteObjectKeyFrame_7671f14c5bd54fc58d34172803d6c65c = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_7671f14c5bd54fc58d34172803d6c65c.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_7671f14c5bd54fc58d34172803d6c65c.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"0.5");

            ObjectAnimationUsingKeyFrames_1fe72b56484c4726829e2975a58545aa.KeyFrames.Add(DiscreteObjectKeyFrame_7671f14c5bd54fc58d34172803d6c65c);


            Storyboard_da2c2b3acf6a4b58bd788c98c45884de.Children.Add(ObjectAnimationUsingKeyFrames_1fe72b56484c4726829e2975a58545aa);


            VisualState_48d4766d0dc044c3a93d66b68fa24ee7.Storyboard = Storyboard_da2c2b3acf6a4b58bd788c98c45884de;


            VisualStateGroup_3de8c62808724aa8adf0baa139318a0c.States.Add(VisualState_56bda82cbdbb4281a202da2b874b4056);
            VisualStateGroup_3de8c62808724aa8adf0baa139318a0c.States.Add(VisualState_117aff25cef1480e9f47add7ddf01a9d);
            VisualStateGroup_3de8c62808724aa8adf0baa139318a0c.States.Add(VisualState_f381c250f2004d6fa8e607077108baf6);
            VisualStateGroup_3de8c62808724aa8adf0baa139318a0c.States.Add(VisualState_48d4766d0dc044c3a93d66b68fa24ee7);


            var VisualStateGroup_2c6df731bb2b4507a1167fa1abea234f = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CheckStates", VisualStateGroup_2c6df731bb2b4507a1167fa1abea234f);
            VisualStateGroup_2c6df731bb2b4507a1167fa1abea234f.Name = "CheckStates";
            var VisualState_cf22006de00c430498374793f9ad96a0 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Checked", VisualState_cf22006de00c430498374793f9ad96a0);
            VisualState_cf22006de00c430498374793f9ad96a0.Name = "Checked";
            var Storyboard_a0bc8d22367a492bae782a11fdd6ea96 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_af59477110984b5091cdc6b15c52582e = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_af59477110984b5091cdc6b15c52582e, @"CheckedStateOuterBorder");
            var DiscreteObjectKeyFrame_c5ba0615754c47ccb7b5664930b1aafd = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_c5ba0615754c47ccb7b5664930b1aafd.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_c5ba0615754c47ccb7b5664930b1aafd.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"1");

            ObjectAnimationUsingKeyFrames_af59477110984b5091cdc6b15c52582e.KeyFrames.Add(DiscreteObjectKeyFrame_c5ba0615754c47ccb7b5664930b1aafd);


            Storyboard_a0bc8d22367a492bae782a11fdd6ea96.Children.Add(ObjectAnimationUsingKeyFrames_af59477110984b5091cdc6b15c52582e);


            VisualState_cf22006de00c430498374793f9ad96a0.Storyboard = Storyboard_a0bc8d22367a492bae782a11fdd6ea96;


            var VisualState_e1607bef63f04bfcafaf0237ea33555e = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Unchecked", VisualState_e1607bef63f04bfcafaf0237ea33555e);
            VisualState_e1607bef63f04bfcafaf0237ea33555e.Name = "Unchecked";

            VisualStateGroup_2c6df731bb2b4507a1167fa1abea234f.States.Add(VisualState_cf22006de00c430498374793f9ad96a0);
            VisualStateGroup_2c6df731bb2b4507a1167fa1abea234f.States.Add(VisualState_e1607bef63f04bfcafaf0237ea33555e);


            var VisualStateGroup_e9416c575d9e4fb2956b9d79dca58c8e = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("FocusStates", VisualStateGroup_e9416c575d9e4fb2956b9d79dca58c8e);
            VisualStateGroup_e9416c575d9e4fb2956b9d79dca58c8e.Name = "FocusStates";
            var VisualState_ea422e7edb314713a12e0e83acf2fe1e = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Focused", VisualState_ea422e7edb314713a12e0e83acf2fe1e);
            VisualState_ea422e7edb314713a12e0e83acf2fe1e.Name = "Focused";
            var Storyboard_e11ffccdd62d4e4680b7e953f3342549 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_7378f61104fd4800be233d764be77e28 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_7378f61104fd4800be233d764be77e28, @"FocusVisualElement");
            var DiscreteObjectKeyFrame_bdd2b367c0054bb79d3811cc067ee0dc = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_bdd2b367c0054bb79d3811cc067ee0dc.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_bdd2b367c0054bb79d3811cc067ee0dc.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"1");

            ObjectAnimationUsingKeyFrames_7378f61104fd4800be233d764be77e28.KeyFrames.Add(DiscreteObjectKeyFrame_bdd2b367c0054bb79d3811cc067ee0dc);


            Storyboard_e11ffccdd62d4e4680b7e953f3342549.Children.Add(ObjectAnimationUsingKeyFrames_7378f61104fd4800be233d764be77e28);


            VisualState_ea422e7edb314713a12e0e83acf2fe1e.Storyboard = Storyboard_e11ffccdd62d4e4680b7e953f3342549;


            var VisualState_36200ecff6464e9483cba882eed695bd = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Unfocused", VisualState_36200ecff6464e9483cba882eed695bd);
            VisualState_36200ecff6464e9483cba882eed695bd.Name = "Unfocused";

            VisualStateGroup_e9416c575d9e4fb2956b9d79dca58c8e.States.Add(VisualState_ea422e7edb314713a12e0e83acf2fe1e);
            VisualStateGroup_e9416c575d9e4fb2956b9d79dca58c8e.States.Add(VisualState_36200ecff6464e9483cba882eed695bd);


            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_3de8c62808724aa8adf0baa139318a0c);
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_2c6df731bb2b4507a1167fa1abea234f);
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_e9416c575d9e4fb2956b9d79dca58c8e);

            var Border_eecf9e336ab745d9b8f95b9c2b9b4e49 = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CheckedStateOuterBorder", Border_eecf9e336ab745d9b8f95b9c2b9b4e49);
            Border_eecf9e336ab745d9b8f95b9c2b9b4e49.Name = "CheckedStateOuterBorder";
            Border_eecf9e336ab745d9b8f95b9c2b9b4e49.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#7FA9A9A9");
            Border_eecf9e336ab745d9b8f95b9c2b9b4e49.BorderBrush = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#00FFFFFF");
            Border_eecf9e336ab745d9b8f95b9c2b9b4e49.CornerRadius = (global::Windows.UI.Xaml.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.CornerRadius), @"3");
            Border_eecf9e336ab745d9b8f95b9c2b9b4e49.Opacity = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");
            var Binding_bcd8811d47304f19bccb473b35582756 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_bcd8811d47304f19bccb473b35582756.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_41d2837eea7045099cb6515febf42613 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_41d2837eea7045099cb6515febf42613.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_bcd8811d47304f19bccb473b35582756.RelativeSource = RelativeSource_41d2837eea7045099cb6515febf42613;


            Binding_bcd8811d47304f19bccb473b35582756.TemplateOwner = templateInstance_775e6fcf22224272900c5397b4c5f5f6;


            var Border_4da38deed2994c11bd59fa76e919bbc3 = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("OuterBtnBorder", Border_4da38deed2994c11bd59fa76e919bbc3);
            Border_4da38deed2994c11bd59fa76e919bbc3.Name = "OuterBtnBorder";
            Border_4da38deed2994c11bd59fa76e919bbc3.BorderBrush = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#00FFFFFF");
            Border_4da38deed2994c11bd59fa76e919bbc3.CornerRadius = (global::Windows.UI.Xaml.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.CornerRadius), @"3");
            var Border_ce1451764756494cb093c87f1a72735c = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("InnerBtnBorder", Border_ce1451764756494cb093c87f1a72735c);
            Border_ce1451764756494cb093c87f1a72735c.Name = "InnerBtnBorder";
            Border_ce1451764756494cb093c87f1a72735c.BorderBrush = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#00CCD1D6");
            Border_ce1451764756494cb093c87f1a72735c.CornerRadius = (global::Windows.UI.Xaml.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.CornerRadius), @"2");
            var ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64 = new global::Windows.UI.Xaml.Controls.ContentPresenter();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("contentPresenter", ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64);
            ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64.Name = "contentPresenter";
            ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center;
            ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            var Binding_e55036387c2c4bccbc03759fc54486e4 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_e55036387c2c4bccbc03759fc54486e4.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Content");
            var RelativeSource_1ab0218e66c449ba8129df37262d41bb = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_1ab0218e66c449ba8129df37262d41bb.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_e55036387c2c4bccbc03759fc54486e4.RelativeSource = RelativeSource_1ab0218e66c449ba8129df37262d41bb;


            Binding_e55036387c2c4bccbc03759fc54486e4.TemplateOwner = templateInstance_775e6fcf22224272900c5397b4c5f5f6;

            var Binding_1f3f6b2d019c4a9a89fe716bbee3e15a = new global::Windows.UI.Xaml.Data.Binding();
            Binding_1f3f6b2d019c4a9a89fe716bbee3e15a.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"ContentTemplate");
            var RelativeSource_24e59aedc39f41b4aaf1aee6eda60a83 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_24e59aedc39f41b4aaf1aee6eda60a83.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_1f3f6b2d019c4a9a89fe716bbee3e15a.RelativeSource = RelativeSource_24e59aedc39f41b4aaf1aee6eda60a83;


            Binding_1f3f6b2d019c4a9a89fe716bbee3e15a.TemplateOwner = templateInstance_775e6fcf22224272900c5397b4c5f5f6;


            Border_ce1451764756494cb093c87f1a72735c.Child = ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64;

            var Binding_b03a2e7d26cd47a59601d390d4841c75 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_b03a2e7d26cd47a59601d390d4841c75.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_87058a93505542889ed8e17caf9435f7 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_87058a93505542889ed8e17caf9435f7.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_b03a2e7d26cd47a59601d390d4841c75.RelativeSource = RelativeSource_87058a93505542889ed8e17caf9435f7;


            Binding_b03a2e7d26cd47a59601d390d4841c75.TemplateOwner = templateInstance_775e6fcf22224272900c5397b4c5f5f6;


            Border_4da38deed2994c11bd59fa76e919bbc3.Child = Border_ce1451764756494cb093c87f1a72735c;

            var Binding_f4ac443209c84eb39534e21ffd629afb = new global::Windows.UI.Xaml.Data.Binding();
            Binding_f4ac443209c84eb39534e21ffd629afb.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_a247992ddf834f06a760fd7885c82640 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_a247992ddf834f06a760fd7885c82640.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_f4ac443209c84eb39534e21ffd629afb.RelativeSource = RelativeSource_a247992ddf834f06a760fd7885c82640;


            Binding_f4ac443209c84eb39534e21ffd629afb.TemplateOwner = templateInstance_775e6fcf22224272900c5397b4c5f5f6;

            var Binding_c726a1cbcb3a4645bd1a5b6906e5af4e = new global::Windows.UI.Xaml.Data.Binding();
            Binding_c726a1cbcb3a4645bd1a5b6906e5af4e.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_8c5fa5c757b34043aa7d8f199a7b8132 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_8c5fa5c757b34043aa7d8f199a7b8132.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_c726a1cbcb3a4645bd1a5b6906e5af4e.RelativeSource = RelativeSource_8c5fa5c757b34043aa7d8f199a7b8132;


            Binding_c726a1cbcb3a4645bd1a5b6906e5af4e.TemplateOwner = templateInstance_775e6fcf22224272900c5397b4c5f5f6;


            var Border_3581775925684bed938ba5e5f2413265 = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("FocusVisualElement", Border_3581775925684bed938ba5e5f2413265);
            Border_3581775925684bed938ba5e5f2413265.Name = "FocusVisualElement";
            Border_3581775925684bed938ba5e5f2413265.BorderBrush = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FF6DBDD1");
            Border_3581775925684bed938ba5e5f2413265.CornerRadius = (global::Windows.UI.Xaml.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.CornerRadius), @"2");
            Border_3581775925684bed938ba5e5f2413265.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");
            Border_3581775925684bed938ba5e5f2413265.Opacity = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");
            var Binding_8a60146f7e0249f39a63e498869c3f54 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_8a60146f7e0249f39a63e498869c3f54.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_19b621559c4e48cc9ab22ebff5820ec5 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_19b621559c4e48cc9ab22ebff5820ec5.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_8a60146f7e0249f39a63e498869c3f54.RelativeSource = RelativeSource_19b621559c4e48cc9ab22ebff5820ec5;


            Binding_8a60146f7e0249f39a63e498869c3f54.TemplateOwner = templateInstance_775e6fcf22224272900c5397b4c5f5f6;

            var Binding_e84a71b43bbe4b75aeb101ccdce6de3b = new global::Windows.UI.Xaml.Data.Binding();
            Binding_e84a71b43bbe4b75aeb101ccdce6de3b.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_8fb5cabb9e694cb68c0abc9afd77c4c0 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_8fb5cabb9e694cb68c0abc9afd77c4c0.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_e84a71b43bbe4b75aeb101ccdce6de3b.RelativeSource = RelativeSource_8fb5cabb9e694cb68c0abc9afd77c4c0;


            Binding_e84a71b43bbe4b75aeb101ccdce6de3b.TemplateOwner = templateInstance_775e6fcf22224272900c5397b4c5f5f6;


            Grid_94469a9e5eed451bac82ae6b7812395d.Children.Add(Border_eecf9e336ab745d9b8f95b9c2b9b4e49);
            Grid_94469a9e5eed451bac82ae6b7812395d.Children.Add(Border_4da38deed2994c11bd59fa76e919bbc3);
            Grid_94469a9e5eed451bac82ae6b7812395d.Children.Add(Border_3581775925684bed938ba5e5f2413265);



            Border_eecf9e336ab745d9b8f95b9c2b9b4e49.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_bcd8811d47304f19bccb473b35582756);
            ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentProperty, Binding_e55036387c2c4bccbc03759fc54486e4);
            ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentTemplateProperty, Binding_1f3f6b2d019c4a9a89fe716bbee3e15a);
            Border_ce1451764756494cb093c87f1a72735c.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_b03a2e7d26cd47a59601d390d4841c75);
            Border_4da38deed2994c11bd59fa76e919bbc3.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_f4ac443209c84eb39534e21ffd629afb);
            Border_4da38deed2994c11bd59fa76e919bbc3.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_c726a1cbcb3a4645bd1a5b6906e5af4e);
            Border_3581775925684bed938ba5e5f2413265.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_8a60146f7e0249f39a63e498869c3f54);
            Border_3581775925684bed938ba5e5f2413265.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_e84a71b43bbe4b75aeb101ccdce6de3b);

            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_1fe72b56484c4726829e2975a58545aa,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Opacity",
                    "Opacity",
                    accessVisualStateProperty_bd0c50e0aa094ae88569d0a5bc3c07e1,
                    setVisualStateProperty_bd0c50e0aa094ae88569d0a5bc3c07e1,
                    setLocalVisualStateProperty_bd0c50e0aa094ae88569d0a5bc3c07e1,
                    getVisualStateProperty_bd0c50e0aa094ae88569d0a5bc3c07e1));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_1fe72b56484c4726829e2975a58545aa, ContentPresenter_2a8c9e29c25241de9757cb3d81f65c64);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_af59477110984b5091cdc6b15c52582e,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Opacity",
                    "Opacity",
                    accessVisualStateProperty_a456d3d23a5d4f9482dde4f726c3e361,
                    setVisualStateProperty_a456d3d23a5d4f9482dde4f726c3e361,
                    setLocalVisualStateProperty_a456d3d23a5d4f9482dde4f726c3e361,
                    getVisualStateProperty_a456d3d23a5d4f9482dde4f726c3e361));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_af59477110984b5091cdc6b15c52582e, Border_eecf9e336ab745d9b8f95b9c2b9b4e49);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_7378f61104fd4800be233d764be77e28,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Opacity",
                    "Opacity",
                    accessVisualStateProperty_2996c86077b74db1a44e79b444cd3d90,
                    setVisualStateProperty_2996c86077b74db1a44e79b444cd3d90,
                    setLocalVisualStateProperty_2996c86077b74db1a44e79b444cd3d90,
                    getVisualStateProperty_2996c86077b74db1a44e79b444cd3d90));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_7378f61104fd4800be233d764be77e28, Border_3581775925684bed938ba5e5f2413265);

            templateInstance_775e6fcf22224272900c5397b4c5f5f6.TemplateContent = Grid_94469a9e5eed451bac82ae6b7812395d;
            return templateInstance_775e6fcf22224272900c5397b4c5f5f6;
        }



        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_e4c067ed50db4a1493459f6d6c44200f(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_e4c067ed50db4a1493459f6d6c44200f(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_e4c067ed50db4a1493459f6d6c44200f(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_e4c067ed50db4a1493459f6d6c44200f(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_25a037c483174582b5b8ff6e591fa34f(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_25a037c483174582b5b8ff6e591fa34f(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_25a037c483174582b5b8ff6e591fa34f(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_25a037c483174582b5b8ff6e591fa34f(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_310181e043004846999fcd1fd825a8b6(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_310181e043004846999fcd1fd825a8b6(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_310181e043004846999fcd1fd825a8b6(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_310181e043004846999fcd1fd825a8b6(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_92bb952c168248e48af0c8f0c2f4476c(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_92bb952c168248e48af0c8f0c2f4476c(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_92bb952c168248e48af0c8f0c2f4476c(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_92bb952c168248e48af0c8f0c2f4476c(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_31a81299a7e94144bfbb2d7c3098a50e(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_31a81299a7e94144bfbb2d7c3098a50e(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_31a81299a7e94144bfbb2d7c3098a50e(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_31a81299a7e94144bfbb2d7c3098a50e(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_e54f799f2101425eae897770a57938ca(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_e54f799f2101425eae897770a57938ca(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_e54f799f2101425eae897770a57938ca(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_e54f799f2101425eae897770a57938ca(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_de55a2edc4954dc2957579cc3b6081bc(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_de55a2edc4954dc2957579cc3b6081bc(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_de55a2edc4954dc2957579cc3b6081bc(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_de55a2edc4954dc2957579cc3b6081bc(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_c36c66a63b084180872fe4f6bd226cfe(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_c36c66a63b084180872fe4f6bd226cfe(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_c36c66a63b084180872fe4f6bd226cfe(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_c36c66a63b084180872fe4f6bd226cfe(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_ad8d0039b8a4499f9fd1032501ea81d5(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_ad8d0039b8a4499f9fd1032501ea81d5(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_ad8d0039b8a4499f9fd1032501ea81d5(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_ad8d0039b8a4499f9fd1032501ea81d5(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_b8d7583c61994c808ab5aa00dbc46386(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_b8d7583c61994c808ab5aa00dbc46386(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_b8d7583c61994c808ab5aa00dbc46386(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_b8d7583c61994c808ab5aa00dbc46386(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_f1d0cdc46f5b4110b414c009ffb63c0a(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_f1d0cdc46f5b4110b414c009ffb63c0a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_f1d0cdc46f5b4110b414c009ffb63c0a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_f1d0cdc46f5b4110b414c009ffb63c0a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_cf7c1d4daafe484bbc4736b387b61ab9(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_cf7c1d4daafe484bbc4736b387b61ab9(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_cf7c1d4daafe484bbc4736b387b61ab9(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_cf7c1d4daafe484bbc4736b387b61ab9(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_4064e88d2988479aacc4d848ee5f2e52(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_4064e88d2988479aacc4d848ee5f2e52(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_4064e88d2988479aacc4d848ee5f2e52(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_4064e88d2988479aacc4d848ee5f2e52(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_7670580d752a45d0972587bb549a9ab6(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_7670580d752a45d0972587bb549a9ab6(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_7670580d752a45d0972587bb549a9ab6(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_7670580d752a45d0972587bb549a9ab6(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_6e581dd54e344d7e94f57c0978c290e6(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_6e581dd54e344d7e94f57c0978c290e6(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_6e581dd54e344d7e94f57c0978c290e6(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_6e581dd54e344d7e94f57c0978c290e6(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_996462726a7744be8bb086c73f96ff57(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_996462726a7744be8bb086c73f96ff57(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_996462726a7744be8bb086c73f96ff57(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_996462726a7744be8bb086c73f96ff57(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_fb5773338a444cbc8bd2cc90ff66f7b2(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_fb5773338a444cbc8bd2cc90ff66f7b2(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_fb5773338a444cbc8bd2cc90ff66f7b2(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_fb5773338a444cbc8bd2cc90ff66f7b2(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_ddc25f31b9da44e9a6e85d92302d793a(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_ddc25f31b9da44e9a6e85d92302d793a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_ddc25f31b9da44e9a6e85d92302d793a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_ddc25f31b9da44e9a6e85d92302d793a(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_586d9fee14b642098172624535a9b0e1(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_586d9fee14b642098172624535a9b0e1(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_586d9fee14b642098172624535a9b0e1(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_586d9fee14b642098172624535a9b0e1(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_cd347efb57c54637b1df77c18e921404(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_cd347efb57c54637b1df77c18e921404(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_cd347efb57c54637b1df77c18e921404(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_cd347efb57c54637b1df77c18e921404(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_95f930ac5c8e479bbb22a702196e76c5(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_95f930ac5c8e479bbb22a702196e76c5(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_95f930ac5c8e479bbb22a702196e76c5(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_95f930ac5c8e479bbb22a702196e76c5(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_5a58a72b322f4b59ba82da3ba51062b7(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_5a58a72b322f4b59ba82da3ba51062b7(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_5a58a72b322f4b59ba82da3ba51062b7(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_5a58a72b322f4b59ba82da3ba51062b7(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("Visibility").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("VisibilityProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_25729cbfa664423389a71921ef5f672f(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_02c86289167443ce9f58fee5a926ce26 = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_02c86289167443ce9f58fee5a926ce26.TemplateOwner = templateOwner;
            var Grid_7f4ee1c24187438c85695ea996c43dd0 = new global::Windows.UI.Xaml.Controls.Grid();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Root", Grid_7f4ee1c24187438c85695ea996c43dd0);
            Grid_7f4ee1c24187438c85695ea996c43dd0.Name = "Root";
            Grid_7f4ee1c24187438c85695ea996c43dd0.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Transparent");

            var VisualStateGroup_0db846b4082747c39cfb7fc007499e8b = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("DisplayModeStates", VisualStateGroup_0db846b4082747c39cfb7fc007499e8b);
            VisualStateGroup_0db846b4082747c39cfb7fc007499e8b.Name = "DisplayModeStates";
            var VisualState_cdb8e9e4f55d4b4980098a1336bd34c1 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("FirstLastNumeric", VisualState_cdb8e9e4f55d4b4980098a1336bd34c1);
            VisualState_cdb8e9e4f55d4b4980098a1336bd34c1.Name = "FirstLastNumeric";
            var Storyboard_d45d98aa52954b6399c6d81cc7e494d1 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_1b4d05d2581149419affafa47f0da001 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_1b4d05d2581149419affafa47f0da001, @"NextPageButton");
            var DiscreteObjectKeyFrame_dac9434873a445fc83d770765671947b = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_dac9434873a445fc83d770765671947b.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_dac9434873a445fc83d770765671947b.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_1b4d05d2581149419affafa47f0da001.KeyFrames.Add(DiscreteObjectKeyFrame_dac9434873a445fc83d770765671947b);


            var ObjectAnimationUsingKeyFrames_f554248a2dae432db7f6ebb0e9052863 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_f554248a2dae432db7f6ebb0e9052863, @"PreviousPageButton");
            var DiscreteObjectKeyFrame_350c0ce1301b41e186ae0d4c48392ae0 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_350c0ce1301b41e186ae0d4c48392ae0.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_350c0ce1301b41e186ae0d4c48392ae0.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_f554248a2dae432db7f6ebb0e9052863.KeyFrames.Add(DiscreteObjectKeyFrame_350c0ce1301b41e186ae0d4c48392ae0);


            var ObjectAnimationUsingKeyFrames_492215f6c9d140c38c7a8a9b81f1f512 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_492215f6c9d140c38c7a8a9b81f1f512, @"CurrentPageTextBox");
            var DiscreteObjectKeyFrame_06e4a74f23cf4078b76c06e3b07fd556 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_06e4a74f23cf4078b76c06e3b07fd556.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_06e4a74f23cf4078b76c06e3b07fd556.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_492215f6c9d140c38c7a8a9b81f1f512.KeyFrames.Add(DiscreteObjectKeyFrame_06e4a74f23cf4078b76c06e3b07fd556);


            var ObjectAnimationUsingKeyFrames_52c8493a678348afac140010c5c94cd4 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_52c8493a678348afac140010c5c94cd4, @"PageDisplay");
            var DiscreteObjectKeyFrame_11c82e7300bf4d288e77cb53b57c3218 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_11c82e7300bf4d288e77cb53b57c3218.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_11c82e7300bf4d288e77cb53b57c3218.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_52c8493a678348afac140010c5c94cd4.KeyFrames.Add(DiscreteObjectKeyFrame_11c82e7300bf4d288e77cb53b57c3218);


            Storyboard_d45d98aa52954b6399c6d81cc7e494d1.Children.Add(ObjectAnimationUsingKeyFrames_1b4d05d2581149419affafa47f0da001);
            Storyboard_d45d98aa52954b6399c6d81cc7e494d1.Children.Add(ObjectAnimationUsingKeyFrames_f554248a2dae432db7f6ebb0e9052863);
            Storyboard_d45d98aa52954b6399c6d81cc7e494d1.Children.Add(ObjectAnimationUsingKeyFrames_492215f6c9d140c38c7a8a9b81f1f512);
            Storyboard_d45d98aa52954b6399c6d81cc7e494d1.Children.Add(ObjectAnimationUsingKeyFrames_52c8493a678348afac140010c5c94cd4);


            VisualState_cdb8e9e4f55d4b4980098a1336bd34c1.Storyboard = Storyboard_d45d98aa52954b6399c6d81cc7e494d1;


            var VisualState_97b3411c5d2340b0a2fab167217199ee = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("FirstLastPreviousNext", VisualState_97b3411c5d2340b0a2fab167217199ee);
            VisualState_97b3411c5d2340b0a2fab167217199ee.Name = "FirstLastPreviousNext";
            var Storyboard_22afc7f25c8044208500f6ce1158e4aa = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_83430102a5a64417ab18dad8fd48f51e = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_83430102a5a64417ab18dad8fd48f51e, @"NumericButtonPanel");
            var DiscreteObjectKeyFrame_e5330c14fc9047729967981f23d6e47f = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_e5330c14fc9047729967981f23d6e47f.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_e5330c14fc9047729967981f23d6e47f.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_83430102a5a64417ab18dad8fd48f51e.KeyFrames.Add(DiscreteObjectKeyFrame_e5330c14fc9047729967981f23d6e47f);


            Storyboard_22afc7f25c8044208500f6ce1158e4aa.Children.Add(ObjectAnimationUsingKeyFrames_83430102a5a64417ab18dad8fd48f51e);


            VisualState_97b3411c5d2340b0a2fab167217199ee.Storyboard = Storyboard_22afc7f25c8044208500f6ce1158e4aa;


            var VisualState_c714f0d40de84daaba56d7144967c7c0 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("FirstLastPreviousNextNumeric", VisualState_c714f0d40de84daaba56d7144967c7c0);
            VisualState_c714f0d40de84daaba56d7144967c7c0.Name = "FirstLastPreviousNextNumeric";
            var Storyboard_9379582d77104abb9c300009b32ae91a = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_90cde9f5245749d4af1f42fb1eae22d5 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_90cde9f5245749d4af1f42fb1eae22d5, @"CurrentPageTextBox");
            var DiscreteObjectKeyFrame_74022c29a31a47d8a61a66f33db523c9 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_74022c29a31a47d8a61a66f33db523c9.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_74022c29a31a47d8a61a66f33db523c9.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_90cde9f5245749d4af1f42fb1eae22d5.KeyFrames.Add(DiscreteObjectKeyFrame_74022c29a31a47d8a61a66f33db523c9);


            var ObjectAnimationUsingKeyFrames_abf89c881cc345c785eb2672a963604b = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_abf89c881cc345c785eb2672a963604b, @"PageDisplay");
            var DiscreteObjectKeyFrame_56eb956d4f8f445f96fbcca7707c9a69 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_56eb956d4f8f445f96fbcca7707c9a69.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_56eb956d4f8f445f96fbcca7707c9a69.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_abf89c881cc345c785eb2672a963604b.KeyFrames.Add(DiscreteObjectKeyFrame_56eb956d4f8f445f96fbcca7707c9a69);


            Storyboard_9379582d77104abb9c300009b32ae91a.Children.Add(ObjectAnimationUsingKeyFrames_90cde9f5245749d4af1f42fb1eae22d5);
            Storyboard_9379582d77104abb9c300009b32ae91a.Children.Add(ObjectAnimationUsingKeyFrames_abf89c881cc345c785eb2672a963604b);


            VisualState_c714f0d40de84daaba56d7144967c7c0.Storyboard = Storyboard_9379582d77104abb9c300009b32ae91a;


            var VisualState_f590894a41ca483287bf48c453e50c42 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Numeric", VisualState_f590894a41ca483287bf48c453e50c42);
            VisualState_f590894a41ca483287bf48c453e50c42.Name = "Numeric";
            var Storyboard_d5db8acaf377420984ff5e99d0c3fad4 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_b0cdd1499d1d4362841d0a2240d2a7bc = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_b0cdd1499d1d4362841d0a2240d2a7bc, @"FirstPageButton");
            var DiscreteObjectKeyFrame_91e4e8c443e04d748aa9abca06ebd097 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_91e4e8c443e04d748aa9abca06ebd097.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_91e4e8c443e04d748aa9abca06ebd097.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_b0cdd1499d1d4362841d0a2240d2a7bc.KeyFrames.Add(DiscreteObjectKeyFrame_91e4e8c443e04d748aa9abca06ebd097);


            var ObjectAnimationUsingKeyFrames_bbda902d40a547d897d1f607045f5776 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_bbda902d40a547d897d1f607045f5776, @"LastPageButton");
            var DiscreteObjectKeyFrame_f074522b081740e0bdb93b895769bc10 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_f074522b081740e0bdb93b895769bc10.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_f074522b081740e0bdb93b895769bc10.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_bbda902d40a547d897d1f607045f5776.KeyFrames.Add(DiscreteObjectKeyFrame_f074522b081740e0bdb93b895769bc10);


            var ObjectAnimationUsingKeyFrames_37259e069f894df4a5bb3a1aaf08cf9a = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_37259e069f894df4a5bb3a1aaf08cf9a, @"NextPageButton");
            var DiscreteObjectKeyFrame_76a6d39ac2ed4f40a86313046cebeee6 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_76a6d39ac2ed4f40a86313046cebeee6.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_76a6d39ac2ed4f40a86313046cebeee6.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_37259e069f894df4a5bb3a1aaf08cf9a.KeyFrames.Add(DiscreteObjectKeyFrame_76a6d39ac2ed4f40a86313046cebeee6);


            var ObjectAnimationUsingKeyFrames_4a6eef40a2704fa294e717216b1a641f = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_4a6eef40a2704fa294e717216b1a641f, @"PreviousPageButton");
            var DiscreteObjectKeyFrame_bbcd8b558192456582eb02e9baf98fbb = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_bbcd8b558192456582eb02e9baf98fbb.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_bbcd8b558192456582eb02e9baf98fbb.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_4a6eef40a2704fa294e717216b1a641f.KeyFrames.Add(DiscreteObjectKeyFrame_bbcd8b558192456582eb02e9baf98fbb);


            var ObjectAnimationUsingKeyFrames_5d6117e1885f4282bdaf3569ead6bd51 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_5d6117e1885f4282bdaf3569ead6bd51, @"CurrentPageTextBox");
            var DiscreteObjectKeyFrame_3758392e1ad34c94a7b90ce85eac46c6 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_3758392e1ad34c94a7b90ce85eac46c6.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_3758392e1ad34c94a7b90ce85eac46c6.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_5d6117e1885f4282bdaf3569ead6bd51.KeyFrames.Add(DiscreteObjectKeyFrame_3758392e1ad34c94a7b90ce85eac46c6);


            var ObjectAnimationUsingKeyFrames_f60cb089a8db45c3a5571057b20675e2 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_f60cb089a8db45c3a5571057b20675e2, @"PageDisplay");
            var DiscreteObjectKeyFrame_3420ee16bf5c431c8f2d56f87909ddf2 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_3420ee16bf5c431c8f2d56f87909ddf2.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_3420ee16bf5c431c8f2d56f87909ddf2.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_f60cb089a8db45c3a5571057b20675e2.KeyFrames.Add(DiscreteObjectKeyFrame_3420ee16bf5c431c8f2d56f87909ddf2);


            var ObjectAnimationUsingKeyFrames_560ac39113924ef0bdcbd3fed2a0872b = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_560ac39113924ef0bdcbd3fed2a0872b, @"Separator1");
            var DiscreteObjectKeyFrame_f3ac2e78d2dd49eab9fc31e18d807b55 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_f3ac2e78d2dd49eab9fc31e18d807b55.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_f3ac2e78d2dd49eab9fc31e18d807b55.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_560ac39113924ef0bdcbd3fed2a0872b.KeyFrames.Add(DiscreteObjectKeyFrame_f3ac2e78d2dd49eab9fc31e18d807b55);


            var ObjectAnimationUsingKeyFrames_ed971e57f6b242a18353cff9222679ea = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_ed971e57f6b242a18353cff9222679ea, @"Separator2");
            var DiscreteObjectKeyFrame_3c054f5957f54bb9b41bc7b7021de821 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_3c054f5957f54bb9b41bc7b7021de821.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_3c054f5957f54bb9b41bc7b7021de821.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_ed971e57f6b242a18353cff9222679ea.KeyFrames.Add(DiscreteObjectKeyFrame_3c054f5957f54bb9b41bc7b7021de821);


            Storyboard_d5db8acaf377420984ff5e99d0c3fad4.Children.Add(ObjectAnimationUsingKeyFrames_b0cdd1499d1d4362841d0a2240d2a7bc);
            Storyboard_d5db8acaf377420984ff5e99d0c3fad4.Children.Add(ObjectAnimationUsingKeyFrames_bbda902d40a547d897d1f607045f5776);
            Storyboard_d5db8acaf377420984ff5e99d0c3fad4.Children.Add(ObjectAnimationUsingKeyFrames_37259e069f894df4a5bb3a1aaf08cf9a);
            Storyboard_d5db8acaf377420984ff5e99d0c3fad4.Children.Add(ObjectAnimationUsingKeyFrames_4a6eef40a2704fa294e717216b1a641f);
            Storyboard_d5db8acaf377420984ff5e99d0c3fad4.Children.Add(ObjectAnimationUsingKeyFrames_5d6117e1885f4282bdaf3569ead6bd51);
            Storyboard_d5db8acaf377420984ff5e99d0c3fad4.Children.Add(ObjectAnimationUsingKeyFrames_f60cb089a8db45c3a5571057b20675e2);
            Storyboard_d5db8acaf377420984ff5e99d0c3fad4.Children.Add(ObjectAnimationUsingKeyFrames_560ac39113924ef0bdcbd3fed2a0872b);
            Storyboard_d5db8acaf377420984ff5e99d0c3fad4.Children.Add(ObjectAnimationUsingKeyFrames_ed971e57f6b242a18353cff9222679ea);


            VisualState_f590894a41ca483287bf48c453e50c42.Storyboard = Storyboard_d5db8acaf377420984ff5e99d0c3fad4;


            var VisualState_829b0a5021a149708619a389d66a0c39 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("PreviousNext", VisualState_829b0a5021a149708619a389d66a0c39);
            VisualState_829b0a5021a149708619a389d66a0c39.Name = "PreviousNext";
            var Storyboard_103859abbe934fc381dc6b63ebb052cf = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_ed84f6c7f82a4d9aa9c5629f8173ea6a = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_ed84f6c7f82a4d9aa9c5629f8173ea6a, @"FirstPageButton");
            var DiscreteObjectKeyFrame_826fa88ff77743e78aa402b68505e49f = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_826fa88ff77743e78aa402b68505e49f.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_826fa88ff77743e78aa402b68505e49f.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_ed84f6c7f82a4d9aa9c5629f8173ea6a.KeyFrames.Add(DiscreteObjectKeyFrame_826fa88ff77743e78aa402b68505e49f);


            var ObjectAnimationUsingKeyFrames_6d6892eae02f41fdbb17fb652f164b51 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_6d6892eae02f41fdbb17fb652f164b51, @"LastPageButton");
            var DiscreteObjectKeyFrame_9ec0342da2404093a8cc2ea4ed0fe49b = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_9ec0342da2404093a8cc2ea4ed0fe49b.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_9ec0342da2404093a8cc2ea4ed0fe49b.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_6d6892eae02f41fdbb17fb652f164b51.KeyFrames.Add(DiscreteObjectKeyFrame_9ec0342da2404093a8cc2ea4ed0fe49b);


            var ObjectAnimationUsingKeyFrames_68d4da37033d42bd91210437952956a6 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_68d4da37033d42bd91210437952956a6, @"NumericButtonPanel");
            var DiscreteObjectKeyFrame_2f401ba39ca14d6a946fc0a8efc842ad = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_2f401ba39ca14d6a946fc0a8efc842ad.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_2f401ba39ca14d6a946fc0a8efc842ad.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_68d4da37033d42bd91210437952956a6.KeyFrames.Add(DiscreteObjectKeyFrame_2f401ba39ca14d6a946fc0a8efc842ad);


            Storyboard_103859abbe934fc381dc6b63ebb052cf.Children.Add(ObjectAnimationUsingKeyFrames_ed84f6c7f82a4d9aa9c5629f8173ea6a);
            Storyboard_103859abbe934fc381dc6b63ebb052cf.Children.Add(ObjectAnimationUsingKeyFrames_6d6892eae02f41fdbb17fb652f164b51);
            Storyboard_103859abbe934fc381dc6b63ebb052cf.Children.Add(ObjectAnimationUsingKeyFrames_68d4da37033d42bd91210437952956a6);


            VisualState_829b0a5021a149708619a389d66a0c39.Storyboard = Storyboard_103859abbe934fc381dc6b63ebb052cf;


            var VisualState_3279838841a8491daa77f95b6c19c5bb = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("PreviousNextNumeric", VisualState_3279838841a8491daa77f95b6c19c5bb);
            VisualState_3279838841a8491daa77f95b6c19c5bb.Name = "PreviousNextNumeric";
            var Storyboard_e70052c434af4cc98f2408c7c41b009e = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_38c69a90cb5e46ee92bebdf846289a63 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_38c69a90cb5e46ee92bebdf846289a63, @"FirstPageButton");
            var DiscreteObjectKeyFrame_f8b7397f41834592be6ca8dfbcd6e8c8 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_f8b7397f41834592be6ca8dfbcd6e8c8.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_f8b7397f41834592be6ca8dfbcd6e8c8.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_38c69a90cb5e46ee92bebdf846289a63.KeyFrames.Add(DiscreteObjectKeyFrame_f8b7397f41834592be6ca8dfbcd6e8c8);


            var ObjectAnimationUsingKeyFrames_86eabba1ffa44c05bb9b346f05b81a87 = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_86eabba1ffa44c05bb9b346f05b81a87, @"LastPageButton");
            var DiscreteObjectKeyFrame_132df7ce1efd4e8fb6e62554fc1e178a = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_132df7ce1efd4e8fb6e62554fc1e178a.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_132df7ce1efd4e8fb6e62554fc1e178a.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_86eabba1ffa44c05bb9b346f05b81a87.KeyFrames.Add(DiscreteObjectKeyFrame_132df7ce1efd4e8fb6e62554fc1e178a);


            var ObjectAnimationUsingKeyFrames_fe428118ce17448d998640ba1444a29d = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_fe428118ce17448d998640ba1444a29d, @"CurrentPageTextBox");
            var DiscreteObjectKeyFrame_88f08d89b015443392ad3ae0f86e3799 = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_88f08d89b015443392ad3ae0f86e3799.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_88f08d89b015443392ad3ae0f86e3799.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_fe428118ce17448d998640ba1444a29d.KeyFrames.Add(DiscreteObjectKeyFrame_88f08d89b015443392ad3ae0f86e3799);


            var ObjectAnimationUsingKeyFrames_82ef26e9f4c147dcaa14228a2c663bba = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_82ef26e9f4c147dcaa14228a2c663bba, @"PageDisplay");
            var DiscreteObjectKeyFrame_212e668927c94a99981ae81a8fc3db2d = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_212e668927c94a99981ae81a8fc3db2d.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_212e668927c94a99981ae81a8fc3db2d.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Collapsed");

            ObjectAnimationUsingKeyFrames_82ef26e9f4c147dcaa14228a2c663bba.KeyFrames.Add(DiscreteObjectKeyFrame_212e668927c94a99981ae81a8fc3db2d);


            Storyboard_e70052c434af4cc98f2408c7c41b009e.Children.Add(ObjectAnimationUsingKeyFrames_38c69a90cb5e46ee92bebdf846289a63);
            Storyboard_e70052c434af4cc98f2408c7c41b009e.Children.Add(ObjectAnimationUsingKeyFrames_86eabba1ffa44c05bb9b346f05b81a87);
            Storyboard_e70052c434af4cc98f2408c7c41b009e.Children.Add(ObjectAnimationUsingKeyFrames_fe428118ce17448d998640ba1444a29d);
            Storyboard_e70052c434af4cc98f2408c7c41b009e.Children.Add(ObjectAnimationUsingKeyFrames_82ef26e9f4c147dcaa14228a2c663bba);


            VisualState_3279838841a8491daa77f95b6c19c5bb.Storyboard = Storyboard_e70052c434af4cc98f2408c7c41b009e;


            VisualStateGroup_0db846b4082747c39cfb7fc007499e8b.States.Add(VisualState_cdb8e9e4f55d4b4980098a1336bd34c1);
            VisualStateGroup_0db846b4082747c39cfb7fc007499e8b.States.Add(VisualState_97b3411c5d2340b0a2fab167217199ee);
            VisualStateGroup_0db846b4082747c39cfb7fc007499e8b.States.Add(VisualState_c714f0d40de84daaba56d7144967c7c0);
            VisualStateGroup_0db846b4082747c39cfb7fc007499e8b.States.Add(VisualState_f590894a41ca483287bf48c453e50c42);
            VisualStateGroup_0db846b4082747c39cfb7fc007499e8b.States.Add(VisualState_829b0a5021a149708619a389d66a0c39);
            VisualStateGroup_0db846b4082747c39cfb7fc007499e8b.States.Add(VisualState_3279838841a8491daa77f95b6c19c5bb);


            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_0db846b4082747c39cfb7fc007499e8b);

            var Border_43ffe661f57c450eb0b620a2fc29eb41 = new global::Windows.UI.Xaml.Controls.Border();
            Border_43ffe661f57c450eb0b620a2fc29eb41.MinHeight = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"24");
            Border_43ffe661f57c450eb0b620a2fc29eb41.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Bottom;
            Border_43ffe661f57c450eb0b620a2fc29eb41.CornerRadius = (global::Windows.UI.Xaml.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.CornerRadius), @"2");
            var StackPanel_2e89527adb5d4bd69edc2233216c02ec = new global::Windows.UI.Xaml.Controls.StackPanel();
            StackPanel_2e89527adb5d4bd69edc2233216c02ec.Orientation = global::Windows.UI.Xaml.Controls.Orientation.Horizontal;
            StackPanel_2e89527adb5d4bd69edc2233216c02ec.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Stretch;
            var Button_38b956b481c84006a0b890678a8b9425 = new global::Windows.UI.Xaml.Controls.Button();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("FirstPageButton", Button_38b956b481c84006a0b890678a8b9425);
            Button_38b956b481c84006a0b890678a8b9425.Name = "FirstPageButton";
            Button_38b956b481c84006a0b890678a8b9425.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");
            Button_38b956b481c84006a0b890678a8b9425.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");
            Button_38b956b481c84006a0b890678a8b9425.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#00000000");
            Button_38b956b481c84006a0b890678a8b9425.Foreground = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FF000000");
            Button_38b956b481c84006a0b890678a8b9425.BorderBrush = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFFFFFFF");
            Button_38b956b481c84006a0b890678a8b9425.BorderThickness = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");
            Button_38b956b481c84006a0b890678a8b9425.Padding = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");
            Button_38b956b481c84006a0b890678a8b9425.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Right;
            Button_38b956b481c84006a0b890678a8b9425.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            var Grid_f4ec4519247e4d82a8d43b366375802d = new global::Windows.UI.Xaml.Controls.Grid();
            Grid_f4ec4519247e4d82a8d43b366375802d.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Grid_f4ec4519247e4d82a8d43b366375802d.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"8");
            var Path_23ec9dcad4794c85aa465687641eca68 = new global::Windows.UI.Xaml.Shapes.Path();
            Path_23ec9dcad4794c85aa465687641eca68.Stretch = global::Windows.UI.Xaml.Media.Stretch.Fill;
            Path_23ec9dcad4794c85aa465687641eca68.Data = (global::Windows.UI.Xaml.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Geometry), @"M0,1 L1,0 L1,2 Z");
            Path_23ec9dcad4794c85aa465687641eca68.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"5");
            Path_23ec9dcad4794c85aa465687641eca68.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_23ec9dcad4794c85aa465687641eca68.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Right;
            var Binding_63648c0deec54d989a0cf65c99648baf = new global::Windows.UI.Xaml.Data.Binding();
            Binding_63648c0deec54d989a0cf65c99648baf.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Foreground");
            var RelativeSource_92ba41ad89da404aab7f2f39b82d8f73 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_92ba41ad89da404aab7f2f39b82d8f73.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_63648c0deec54d989a0cf65c99648baf.RelativeSource = RelativeSource_92ba41ad89da404aab7f2f39b82d8f73;


            Binding_63648c0deec54d989a0cf65c99648baf.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


            var Rectangle_0af080efd3b0400c812a86dde0d8ac70 = new global::Windows.UI.Xaml.Shapes.Rectangle();
            Rectangle_0af080efd3b0400c812a86dde0d8ac70.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"2");
            Rectangle_0af080efd3b0400c812a86dde0d8ac70.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Rectangle_0af080efd3b0400c812a86dde0d8ac70.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Left;
            var Binding_1e6a8f803d604c338dfcae2e111f2ac0 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_1e6a8f803d604c338dfcae2e111f2ac0.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Foreground");
            var RelativeSource_f86c441a95ad4167a525cab717e0bead = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_f86c441a95ad4167a525cab717e0bead.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_1e6a8f803d604c338dfcae2e111f2ac0.RelativeSource = RelativeSource_f86c441a95ad4167a525cab717e0bead;


            Binding_1e6a8f803d604c338dfcae2e111f2ac0.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


            Grid_f4ec4519247e4d82a8d43b366375802d.Children.Add(Path_23ec9dcad4794c85aa465687641eca68);
            Grid_f4ec4519247e4d82a8d43b366375802d.Children.Add(Rectangle_0af080efd3b0400c812a86dde0d8ac70);


            Button_38b956b481c84006a0b890678a8b9425.Content = Grid_f4ec4519247e4d82a8d43b366375802d;


            var Button_8ee8e6f33b0d4426a5b1fcc96ad58dea = new global::Windows.UI.Xaml.Controls.Button();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("PreviousPageButton", Button_8ee8e6f33b0d4426a5b1fcc96ad58dea);
            Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.Name = "PreviousPageButton";
            Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#00000000");
            Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.Foreground = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FF000000");
            Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.BorderBrush = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFFFFFFF");
            Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.BorderThickness = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");
            Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.Padding = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");
            Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");
            Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");
            Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Right;
            Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            var Path_94c1c35de2834123a12a262069111d05 = new global::Windows.UI.Xaml.Shapes.Path();
            Path_94c1c35de2834123a12a262069111d05.Stretch = global::Windows.UI.Xaml.Media.Stretch.Fill;
            Path_94c1c35de2834123a12a262069111d05.Data = (global::Windows.UI.Xaml.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Geometry), @"M0,1 L1,0 L1,2 Z");
            Path_94c1c35de2834123a12a262069111d05.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"5");
            Path_94c1c35de2834123a12a262069111d05.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_94c1c35de2834123a12a262069111d05.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center;
            Path_94c1c35de2834123a12a262069111d05.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            var Binding_9e0e956b45c84c0aa1a2ab5b60ed608b = new global::Windows.UI.Xaml.Data.Binding();
            Binding_9e0e956b45c84c0aa1a2ab5b60ed608b.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Foreground");
            var RelativeSource_62560e8945b84b7eaa95c8c65d2dd915 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_62560e8945b84b7eaa95c8c65d2dd915.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_9e0e956b45c84c0aa1a2ab5b60ed608b.RelativeSource = RelativeSource_62560e8945b84b7eaa95c8c65d2dd915;


            Binding_9e0e956b45c84c0aa1a2ab5b60ed608b.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


            Button_8ee8e6f33b0d4426a5b1fcc96ad58dea.Content = Path_94c1c35de2834123a12a262069111d05;


            var Border_639202ef7dc9498cae85775b174e4c53 = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Separator1", Border_639202ef7dc9498cae85775b174e4c53);
            Border_639202ef7dc9498cae85775b174e4c53.Name = "Separator1";
            Border_639202ef7dc9498cae85775b174e4c53.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"2");
            Border_639202ef7dc9498cae85775b174e4c53.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFCCD1D6");
            Border_639202ef7dc9498cae85775b174e4c53.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0,3,0,3");
            var Binding_d9b162045aef4367a8c0b21e3857e5c7 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_d9b162045aef4367a8c0b21e3857e5c7.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_2ee92e38f2b14e7c8ea7003907779225 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_2ee92e38f2b14e7c8ea7003907779225.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_d9b162045aef4367a8c0b21e3857e5c7.RelativeSource = RelativeSource_2ee92e38f2b14e7c8ea7003907779225;


            Binding_d9b162045aef4367a8c0b21e3857e5c7.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


            var StackPanel_4236c3c4298248c4a0a31e2e08687dff = new global::Windows.UI.Xaml.Controls.StackPanel();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("NumericButtonPanel", StackPanel_4236c3c4298248c4a0a31e2e08687dff);
            StackPanel_4236c3c4298248c4a0a31e2e08687dff.Name = "NumericButtonPanel";
            StackPanel_4236c3c4298248c4a0a31e2e08687dff.Orientation = global::Windows.UI.Xaml.Controls.Orientation.Horizontal;
            StackPanel_4236c3c4298248c4a0a31e2e08687dff.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");

            var StackPanel_38cdc3083ec24095b99adf32cec3f3a9 = new global::Windows.UI.Xaml.Controls.StackPanel();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("PageDisplay", StackPanel_38cdc3083ec24095b99adf32cec3f3a9);
            StackPanel_38cdc3083ec24095b99adf32cec3f3a9.Name = "PageDisplay";
            StackPanel_38cdc3083ec24095b99adf32cec3f3a9.Orientation = global::Windows.UI.Xaml.Controls.Orientation.Horizontal;
            var TextBlock_fa22ea2fd0594848a8c2170dff3f9b0d = new global::Windows.UI.Xaml.Controls.TextBlock();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CurrentPagePrefixTextBlock", TextBlock_fa22ea2fd0594848a8c2170dff3f9b0d);
            TextBlock_fa22ea2fd0594848a8c2170dff3f9b0d.Name = "CurrentPagePrefixTextBlock";
            TextBlock_fa22ea2fd0594848a8c2170dff3f9b0d.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"Auto");
            TextBlock_fa22ea2fd0594848a8c2170dff3f9b0d.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            TextBlock_fa22ea2fd0594848a8c2170dff3f9b0d.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"4,0,0,0");
            var Binding_ed9bff1ba4844238b637335bb3adf5cf = new global::Windows.UI.Xaml.Data.Binding();
            Binding_ed9bff1ba4844238b637335bb3adf5cf.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Foreground");
            var RelativeSource_ce4c6ef602424234b50cde5f03ab3bb2 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_ce4c6ef602424234b50cde5f03ab3bb2.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_ed9bff1ba4844238b637335bb3adf5cf.RelativeSource = RelativeSource_ce4c6ef602424234b50cde5f03ab3bb2;


            Binding_ed9bff1ba4844238b637335bb3adf5cf.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


            var TextBox_fadfe3e43eee4532ae383105252b8d5d = new global::Windows.UI.Xaml.Controls.TextBox();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CurrentPageTextBox", TextBox_fadfe3e43eee4532ae383105252b8d5d);
            TextBox_fadfe3e43eee4532ae383105252b8d5d.Name = "CurrentPageTextBox";
            TextBox_fadfe3e43eee4532ae383105252b8d5d.TextWrapping = global::Windows.UI.Xaml.TextWrapping.Wrap;
            TextBox_fadfe3e43eee4532ae383105252b8d5d.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"40");
            TextBox_fadfe3e43eee4532ae383105252b8d5d.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"Auto");
            TextBox_fadfe3e43eee4532ae383105252b8d5d.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            TextBox_fadfe3e43eee4532ae383105252b8d5d.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"4,2,4,2");
            var Binding_092500823ad24f3c86009d157ac5dcce = new global::Windows.UI.Xaml.Data.Binding();
            Binding_092500823ad24f3c86009d157ac5dcce.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Foreground");
            var RelativeSource_e0ed835ed30941c4b175ed7add4497ee = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_e0ed835ed30941c4b175ed7add4497ee.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_092500823ad24f3c86009d157ac5dcce.RelativeSource = RelativeSource_e0ed835ed30941c4b175ed7add4497ee;


            Binding_092500823ad24f3c86009d157ac5dcce.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;

            var Binding_d9bad0296bb94c968d97f95959083fca = new global::Windows.UI.Xaml.Data.Binding();
            Binding_d9bad0296bb94c968d97f95959083fca.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_0c4c05ac32f347358b5566aba86a89fa = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_0c4c05ac32f347358b5566aba86a89fa.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_d9bad0296bb94c968d97f95959083fca.RelativeSource = RelativeSource_0c4c05ac32f347358b5566aba86a89fa;


            Binding_d9bad0296bb94c968d97f95959083fca.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


            var TextBlock_ddc27ca47a454a24aefc969d4bdc2182 = new global::Windows.UI.Xaml.Controls.TextBlock();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CurrentPageSuffixTextBlock", TextBlock_ddc27ca47a454a24aefc969d4bdc2182);
            TextBlock_ddc27ca47a454a24aefc969d4bdc2182.Name = "CurrentPageSuffixTextBlock";
            TextBlock_ddc27ca47a454a24aefc969d4bdc2182.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"Auto");
            TextBlock_ddc27ca47a454a24aefc969d4bdc2182.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            TextBlock_ddc27ca47a454a24aefc969d4bdc2182.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0,0,4,0");
            var Binding_4044c8f06ada47b6b654278df8d8a6f6 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_4044c8f06ada47b6b654278df8d8a6f6.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Foreground");
            var RelativeSource_54716356df524a3d8c9cfec9b9d2d819 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_54716356df524a3d8c9cfec9b9d2d819.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_4044c8f06ada47b6b654278df8d8a6f6.RelativeSource = RelativeSource_54716356df524a3d8c9cfec9b9d2d819;


            Binding_4044c8f06ada47b6b654278df8d8a6f6.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


            StackPanel_38cdc3083ec24095b99adf32cec3f3a9.Children.Add(TextBlock_fa22ea2fd0594848a8c2170dff3f9b0d);
            StackPanel_38cdc3083ec24095b99adf32cec3f3a9.Children.Add(TextBox_fadfe3e43eee4532ae383105252b8d5d);
            StackPanel_38cdc3083ec24095b99adf32cec3f3a9.Children.Add(TextBlock_ddc27ca47a454a24aefc969d4bdc2182);


            var Border_69af78d4c920434791175ccf44c0b961 = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Separator2", Border_69af78d4c920434791175ccf44c0b961);
            Border_69af78d4c920434791175ccf44c0b961.Name = "Separator2";
            Border_69af78d4c920434791175ccf44c0b961.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"2");
            Border_69af78d4c920434791175ccf44c0b961.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFCCD1D6");
            Border_69af78d4c920434791175ccf44c0b961.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0,3,0,3");
            var Binding_c3c52295d24545cf9d424280c8bea170 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_c3c52295d24545cf9d424280c8bea170.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_8fc685af76444c288fb1f6c6bedc684b = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_8fc685af76444c288fb1f6c6bedc684b.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_c3c52295d24545cf9d424280c8bea170.RelativeSource = RelativeSource_8fc685af76444c288fb1f6c6bedc684b;


            Binding_c3c52295d24545cf9d424280c8bea170.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


            var Button_9de873ee7702417f9a415751e6737b1a = new global::Windows.UI.Xaml.Controls.Button();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("NextPageButton", Button_9de873ee7702417f9a415751e6737b1a);
            Button_9de873ee7702417f9a415751e6737b1a.Name = "NextPageButton";
            Button_9de873ee7702417f9a415751e6737b1a.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#00000000");
            Button_9de873ee7702417f9a415751e6737b1a.Foreground = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FF000000");
            Button_9de873ee7702417f9a415751e6737b1a.BorderBrush = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFFFFFFF");
            Button_9de873ee7702417f9a415751e6737b1a.BorderThickness = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");
            Button_9de873ee7702417f9a415751e6737b1a.Padding = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");
            Button_9de873ee7702417f9a415751e6737b1a.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");
            Button_9de873ee7702417f9a415751e6737b1a.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");
            Button_9de873ee7702417f9a415751e6737b1a.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Right;
            Button_9de873ee7702417f9a415751e6737b1a.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            var Path_4f9b5bba365a483bb478d9f9ac4ec930 = new global::Windows.UI.Xaml.Shapes.Path();
            Path_4f9b5bba365a483bb478d9f9ac4ec930.Stretch = global::Windows.UI.Xaml.Media.Stretch.Fill;
            Path_4f9b5bba365a483bb478d9f9ac4ec930.Data = (global::Windows.UI.Xaml.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Geometry), @"M0,0 L1,1 L0,2 Z");
            Path_4f9b5bba365a483bb478d9f9ac4ec930.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"5");
            Path_4f9b5bba365a483bb478d9f9ac4ec930.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_4f9b5bba365a483bb478d9f9ac4ec930.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center;
            Path_4f9b5bba365a483bb478d9f9ac4ec930.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            var Binding_8af020caca124b559dfc0349558ec4b6 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_8af020caca124b559dfc0349558ec4b6.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Foreground");
            var RelativeSource_ed5de920d86443e49076d92682e48e18 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_ed5de920d86443e49076d92682e48e18.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_8af020caca124b559dfc0349558ec4b6.RelativeSource = RelativeSource_ed5de920d86443e49076d92682e48e18;


            Binding_8af020caca124b559dfc0349558ec4b6.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


            Button_9de873ee7702417f9a415751e6737b1a.Content = Path_4f9b5bba365a483bb478d9f9ac4ec930;


            var Button_f2704b84b9c24610bf8082cfa31d98b8 = new global::Windows.UI.Xaml.Controls.Button();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("LastPageButton", Button_f2704b84b9c24610bf8082cfa31d98b8);
            Button_f2704b84b9c24610bf8082cfa31d98b8.Name = "LastPageButton";
            Button_f2704b84b9c24610bf8082cfa31d98b8.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#00000000");
            Button_f2704b84b9c24610bf8082cfa31d98b8.Foreground = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FF000000");
            Button_f2704b84b9c24610bf8082cfa31d98b8.BorderBrush = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFFFFFFF");
            Button_f2704b84b9c24610bf8082cfa31d98b8.BorderThickness = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");
            Button_f2704b84b9c24610bf8082cfa31d98b8.Padding = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");
            Button_f2704b84b9c24610bf8082cfa31d98b8.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");
            Button_f2704b84b9c24610bf8082cfa31d98b8.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"20");
            Button_f2704b84b9c24610bf8082cfa31d98b8.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Right;
            Button_f2704b84b9c24610bf8082cfa31d98b8.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            var Grid_1cecb19f596b4d96be2ec17d74b49067 = new global::Windows.UI.Xaml.Controls.Grid();
            Grid_1cecb19f596b4d96be2ec17d74b49067.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Grid_1cecb19f596b4d96be2ec17d74b49067.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"8");
            var Path_85ed49895fe7483aa73bc92beee55e70 = new global::Windows.UI.Xaml.Shapes.Path();
            Path_85ed49895fe7483aa73bc92beee55e70.Stretch = global::Windows.UI.Xaml.Media.Stretch.Fill;
            Path_85ed49895fe7483aa73bc92beee55e70.Data = (global::Windows.UI.Xaml.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Geometry), @"M0,0 L1,1 L0,2 Z");
            Path_85ed49895fe7483aa73bc92beee55e70.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"5");
            Path_85ed49895fe7483aa73bc92beee55e70.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_85ed49895fe7483aa73bc92beee55e70.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Left;
            var Binding_a7b96098bf2c4760b28062b881d18f24 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_a7b96098bf2c4760b28062b881d18f24.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Foreground");
            var RelativeSource_abf3915587cf4438adcc20a22d9d5346 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_abf3915587cf4438adcc20a22d9d5346.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_a7b96098bf2c4760b28062b881d18f24.RelativeSource = RelativeSource_abf3915587cf4438adcc20a22d9d5346;


            Binding_a7b96098bf2c4760b28062b881d18f24.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


            var Rectangle_c9683c1bc950412d8772b863b9d408cc = new global::Windows.UI.Xaml.Shapes.Rectangle();
            Rectangle_c9683c1bc950412d8772b863b9d408cc.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"2");
            Rectangle_c9683c1bc950412d8772b863b9d408cc.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Rectangle_c9683c1bc950412d8772b863b9d408cc.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Right;
            var Binding_62c4e695061d458d8ab29dd04e1adec6 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_62c4e695061d458d8ab29dd04e1adec6.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Foreground");
            var RelativeSource_f1f7fa3953ee466aaead95427cf9c797 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_f1f7fa3953ee466aaead95427cf9c797.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_62c4e695061d458d8ab29dd04e1adec6.RelativeSource = RelativeSource_f1f7fa3953ee466aaead95427cf9c797;


            Binding_62c4e695061d458d8ab29dd04e1adec6.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


            Grid_1cecb19f596b4d96be2ec17d74b49067.Children.Add(Path_85ed49895fe7483aa73bc92beee55e70);
            Grid_1cecb19f596b4d96be2ec17d74b49067.Children.Add(Rectangle_c9683c1bc950412d8772b863b9d408cc);


            Button_f2704b84b9c24610bf8082cfa31d98b8.Content = Grid_1cecb19f596b4d96be2ec17d74b49067;


            StackPanel_2e89527adb5d4bd69edc2233216c02ec.Children.Add(Button_38b956b481c84006a0b890678a8b9425);
            StackPanel_2e89527adb5d4bd69edc2233216c02ec.Children.Add(Button_8ee8e6f33b0d4426a5b1fcc96ad58dea);
            StackPanel_2e89527adb5d4bd69edc2233216c02ec.Children.Add(Border_639202ef7dc9498cae85775b174e4c53);
            StackPanel_2e89527adb5d4bd69edc2233216c02ec.Children.Add(StackPanel_4236c3c4298248c4a0a31e2e08687dff);
            StackPanel_2e89527adb5d4bd69edc2233216c02ec.Children.Add(StackPanel_38cdc3083ec24095b99adf32cec3f3a9);
            StackPanel_2e89527adb5d4bd69edc2233216c02ec.Children.Add(Border_69af78d4c920434791175ccf44c0b961);
            StackPanel_2e89527adb5d4bd69edc2233216c02ec.Children.Add(Button_9de873ee7702417f9a415751e6737b1a);
            StackPanel_2e89527adb5d4bd69edc2233216c02ec.Children.Add(Button_f2704b84b9c24610bf8082cfa31d98b8);

            var Binding_b651b5541e11467bbad2c3d971bb9e76 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_b651b5541e11467bbad2c3d971bb9e76.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"HorizontalContentAlignment");
            var RelativeSource_9c404097039e4194a50402edc421686a = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_9c404097039e4194a50402edc421686a.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_b651b5541e11467bbad2c3d971bb9e76.RelativeSource = RelativeSource_9c404097039e4194a50402edc421686a;


            Binding_b651b5541e11467bbad2c3d971bb9e76.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


            Border_43ffe661f57c450eb0b620a2fc29eb41.Child = StackPanel_2e89527adb5d4bd69edc2233216c02ec;

            var Binding_cb5f54dc0196427d87db4c7e1086faeb = new global::Windows.UI.Xaml.Data.Binding();
            Binding_cb5f54dc0196427d87db4c7e1086faeb.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_f53f717c479c4bf59bef7d478ecfbd33 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_f53f717c479c4bf59bef7d478ecfbd33.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_cb5f54dc0196427d87db4c7e1086faeb.RelativeSource = RelativeSource_f53f717c479c4bf59bef7d478ecfbd33;


            Binding_cb5f54dc0196427d87db4c7e1086faeb.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;

            var Binding_52c1e6bd3a0c4432bdf049f8bb660a7d = new global::Windows.UI.Xaml.Data.Binding();
            Binding_52c1e6bd3a0c4432bdf049f8bb660a7d.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_592f64390d8c4cbf9bdbe1ad4b8b5d0f = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_592f64390d8c4cbf9bdbe1ad4b8b5d0f.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_52c1e6bd3a0c4432bdf049f8bb660a7d.RelativeSource = RelativeSource_592f64390d8c4cbf9bdbe1ad4b8b5d0f;


            Binding_52c1e6bd3a0c4432bdf049f8bb660a7d.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;

            var Binding_69c4761a3f0d4ab7b2d29190241c6cff = new global::Windows.UI.Xaml.Data.Binding();
            Binding_69c4761a3f0d4ab7b2d29190241c6cff.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_d7ab337308754e7383c1077f31013ade = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_d7ab337308754e7383c1077f31013ade.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_69c4761a3f0d4ab7b2d29190241c6cff.RelativeSource = RelativeSource_d7ab337308754e7383c1077f31013ade;


            Binding_69c4761a3f0d4ab7b2d29190241c6cff.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;

            var Binding_346c587373a34059860dbbba28cd13db = new global::Windows.UI.Xaml.Data.Binding();
            Binding_346c587373a34059860dbbba28cd13db.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Padding");
            var RelativeSource_6038fc116d254f208b7dd64029d7974e = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_6038fc116d254f208b7dd64029d7974e.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_346c587373a34059860dbbba28cd13db.RelativeSource = RelativeSource_6038fc116d254f208b7dd64029d7974e;


            Binding_346c587373a34059860dbbba28cd13db.TemplateOwner = templateInstance_02c86289167443ce9f58fee5a926ce26;


            Grid_7f4ee1c24187438c85695ea996c43dd0.Children.Add(Border_43ffe661f57c450eb0b620a2fc29eb41);



            Path_23ec9dcad4794c85aa465687641eca68.SetBinding(global::Windows.UI.Xaml.Shapes.Shape.FillProperty, Binding_63648c0deec54d989a0cf65c99648baf);
            Rectangle_0af080efd3b0400c812a86dde0d8ac70.SetBinding(global::Windows.UI.Xaml.Shapes.Shape.FillProperty, Binding_1e6a8f803d604c338dfcae2e111f2ac0);
            Path_94c1c35de2834123a12a262069111d05.SetBinding(global::Windows.UI.Xaml.Shapes.Shape.FillProperty, Binding_9e0e956b45c84c0aa1a2ab5b60ed608b);
            Border_639202ef7dc9498cae85775b174e4c53.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_d9b162045aef4367a8c0b21e3857e5c7);
            TextBlock_fa22ea2fd0594848a8c2170dff3f9b0d.SetBinding(global::Windows.UI.Xaml.Controls.Control.ForegroundProperty, Binding_ed9bff1ba4844238b637335bb3adf5cf);
            TextBox_fadfe3e43eee4532ae383105252b8d5d.SetBinding(global::Windows.UI.Xaml.Controls.Control.ForegroundProperty, Binding_092500823ad24f3c86009d157ac5dcce);
            TextBox_fadfe3e43eee4532ae383105252b8d5d.SetBinding(global::Windows.UI.Xaml.Controls.Control.BorderBrushProperty, Binding_d9bad0296bb94c968d97f95959083fca);
            TextBlock_ddc27ca47a454a24aefc969d4bdc2182.SetBinding(global::Windows.UI.Xaml.Controls.Control.ForegroundProperty, Binding_4044c8f06ada47b6b654278df8d8a6f6);
            Border_69af78d4c920434791175ccf44c0b961.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_c3c52295d24545cf9d424280c8bea170);
            Path_4f9b5bba365a483bb478d9f9ac4ec930.SetBinding(global::Windows.UI.Xaml.Shapes.Shape.FillProperty, Binding_8af020caca124b559dfc0349558ec4b6);
            Path_85ed49895fe7483aa73bc92beee55e70.SetBinding(global::Windows.UI.Xaml.Shapes.Shape.FillProperty, Binding_a7b96098bf2c4760b28062b881d18f24);
            Rectangle_c9683c1bc950412d8772b863b9d408cc.SetBinding(global::Windows.UI.Xaml.Shapes.Shape.FillProperty, Binding_62c4e695061d458d8ab29dd04e1adec6);
            StackPanel_2e89527adb5d4bd69edc2233216c02ec.SetBinding(global::Windows.UI.Xaml.FrameworkElement.HorizontalAlignmentProperty, Binding_b651b5541e11467bbad2c3d971bb9e76);
            Border_43ffe661f57c450eb0b620a2fc29eb41.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_cb5f54dc0196427d87db4c7e1086faeb);
            Border_43ffe661f57c450eb0b620a2fc29eb41.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_52c1e6bd3a0c4432bdf049f8bb660a7d);
            Border_43ffe661f57c450eb0b620a2fc29eb41.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_69c4761a3f0d4ab7b2d29190241c6cff);
            Border_43ffe661f57c450eb0b620a2fc29eb41.SetBinding(global::Windows.UI.Xaml.Controls.Border.PaddingProperty, Binding_346c587373a34059860dbbba28cd13db);

            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_1b4d05d2581149419affafa47f0da001,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_e4c067ed50db4a1493459f6d6c44200f,
                    setVisualStateProperty_e4c067ed50db4a1493459f6d6c44200f,
                    setLocalVisualStateProperty_e4c067ed50db4a1493459f6d6c44200f,
                    getVisualStateProperty_e4c067ed50db4a1493459f6d6c44200f));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_1b4d05d2581149419affafa47f0da001, Button_9de873ee7702417f9a415751e6737b1a);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_f554248a2dae432db7f6ebb0e9052863,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_25a037c483174582b5b8ff6e591fa34f,
                    setVisualStateProperty_25a037c483174582b5b8ff6e591fa34f,
                    setLocalVisualStateProperty_25a037c483174582b5b8ff6e591fa34f,
                    getVisualStateProperty_25a037c483174582b5b8ff6e591fa34f));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_f554248a2dae432db7f6ebb0e9052863, Button_8ee8e6f33b0d4426a5b1fcc96ad58dea);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_492215f6c9d140c38c7a8a9b81f1f512,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_310181e043004846999fcd1fd825a8b6,
                    setVisualStateProperty_310181e043004846999fcd1fd825a8b6,
                    setLocalVisualStateProperty_310181e043004846999fcd1fd825a8b6,
                    getVisualStateProperty_310181e043004846999fcd1fd825a8b6));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_492215f6c9d140c38c7a8a9b81f1f512, TextBox_fadfe3e43eee4532ae383105252b8d5d);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_52c8493a678348afac140010c5c94cd4,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_92bb952c168248e48af0c8f0c2f4476c,
                    setVisualStateProperty_92bb952c168248e48af0c8f0c2f4476c,
                    setLocalVisualStateProperty_92bb952c168248e48af0c8f0c2f4476c,
                    getVisualStateProperty_92bb952c168248e48af0c8f0c2f4476c));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_52c8493a678348afac140010c5c94cd4, StackPanel_38cdc3083ec24095b99adf32cec3f3a9);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_83430102a5a64417ab18dad8fd48f51e,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_31a81299a7e94144bfbb2d7c3098a50e,
                    setVisualStateProperty_31a81299a7e94144bfbb2d7c3098a50e,
                    setLocalVisualStateProperty_31a81299a7e94144bfbb2d7c3098a50e,
                    getVisualStateProperty_31a81299a7e94144bfbb2d7c3098a50e));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_83430102a5a64417ab18dad8fd48f51e, StackPanel_4236c3c4298248c4a0a31e2e08687dff);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_90cde9f5245749d4af1f42fb1eae22d5,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_e54f799f2101425eae897770a57938ca,
                    setVisualStateProperty_e54f799f2101425eae897770a57938ca,
                    setLocalVisualStateProperty_e54f799f2101425eae897770a57938ca,
                    getVisualStateProperty_e54f799f2101425eae897770a57938ca));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_90cde9f5245749d4af1f42fb1eae22d5, TextBox_fadfe3e43eee4532ae383105252b8d5d);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_abf89c881cc345c785eb2672a963604b,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_de55a2edc4954dc2957579cc3b6081bc,
                    setVisualStateProperty_de55a2edc4954dc2957579cc3b6081bc,
                    setLocalVisualStateProperty_de55a2edc4954dc2957579cc3b6081bc,
                    getVisualStateProperty_de55a2edc4954dc2957579cc3b6081bc));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_abf89c881cc345c785eb2672a963604b, StackPanel_38cdc3083ec24095b99adf32cec3f3a9);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_b0cdd1499d1d4362841d0a2240d2a7bc,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_c36c66a63b084180872fe4f6bd226cfe,
                    setVisualStateProperty_c36c66a63b084180872fe4f6bd226cfe,
                    setLocalVisualStateProperty_c36c66a63b084180872fe4f6bd226cfe,
                    getVisualStateProperty_c36c66a63b084180872fe4f6bd226cfe));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_b0cdd1499d1d4362841d0a2240d2a7bc, Button_38b956b481c84006a0b890678a8b9425);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_bbda902d40a547d897d1f607045f5776,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_ad8d0039b8a4499f9fd1032501ea81d5,
                    setVisualStateProperty_ad8d0039b8a4499f9fd1032501ea81d5,
                    setLocalVisualStateProperty_ad8d0039b8a4499f9fd1032501ea81d5,
                    getVisualStateProperty_ad8d0039b8a4499f9fd1032501ea81d5));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_bbda902d40a547d897d1f607045f5776, Button_f2704b84b9c24610bf8082cfa31d98b8);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_37259e069f894df4a5bb3a1aaf08cf9a,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_b8d7583c61994c808ab5aa00dbc46386,
                    setVisualStateProperty_b8d7583c61994c808ab5aa00dbc46386,
                    setLocalVisualStateProperty_b8d7583c61994c808ab5aa00dbc46386,
                    getVisualStateProperty_b8d7583c61994c808ab5aa00dbc46386));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_37259e069f894df4a5bb3a1aaf08cf9a, Button_9de873ee7702417f9a415751e6737b1a);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_4a6eef40a2704fa294e717216b1a641f,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_f1d0cdc46f5b4110b414c009ffb63c0a,
                    setVisualStateProperty_f1d0cdc46f5b4110b414c009ffb63c0a,
                    setLocalVisualStateProperty_f1d0cdc46f5b4110b414c009ffb63c0a,
                    getVisualStateProperty_f1d0cdc46f5b4110b414c009ffb63c0a));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_4a6eef40a2704fa294e717216b1a641f, Button_8ee8e6f33b0d4426a5b1fcc96ad58dea);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_5d6117e1885f4282bdaf3569ead6bd51,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_cf7c1d4daafe484bbc4736b387b61ab9,
                    setVisualStateProperty_cf7c1d4daafe484bbc4736b387b61ab9,
                    setLocalVisualStateProperty_cf7c1d4daafe484bbc4736b387b61ab9,
                    getVisualStateProperty_cf7c1d4daafe484bbc4736b387b61ab9));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_5d6117e1885f4282bdaf3569ead6bd51, TextBox_fadfe3e43eee4532ae383105252b8d5d);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_f60cb089a8db45c3a5571057b20675e2,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_4064e88d2988479aacc4d848ee5f2e52,
                    setVisualStateProperty_4064e88d2988479aacc4d848ee5f2e52,
                    setLocalVisualStateProperty_4064e88d2988479aacc4d848ee5f2e52,
                    getVisualStateProperty_4064e88d2988479aacc4d848ee5f2e52));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_f60cb089a8db45c3a5571057b20675e2, StackPanel_38cdc3083ec24095b99adf32cec3f3a9);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_560ac39113924ef0bdcbd3fed2a0872b,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_7670580d752a45d0972587bb549a9ab6,
                    setVisualStateProperty_7670580d752a45d0972587bb549a9ab6,
                    setLocalVisualStateProperty_7670580d752a45d0972587bb549a9ab6,
                    getVisualStateProperty_7670580d752a45d0972587bb549a9ab6));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_560ac39113924ef0bdcbd3fed2a0872b, Border_639202ef7dc9498cae85775b174e4c53);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_ed971e57f6b242a18353cff9222679ea,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_6e581dd54e344d7e94f57c0978c290e6,
                    setVisualStateProperty_6e581dd54e344d7e94f57c0978c290e6,
                    setLocalVisualStateProperty_6e581dd54e344d7e94f57c0978c290e6,
                    getVisualStateProperty_6e581dd54e344d7e94f57c0978c290e6));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_ed971e57f6b242a18353cff9222679ea, Border_69af78d4c920434791175ccf44c0b961);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_ed84f6c7f82a4d9aa9c5629f8173ea6a,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_996462726a7744be8bb086c73f96ff57,
                    setVisualStateProperty_996462726a7744be8bb086c73f96ff57,
                    setLocalVisualStateProperty_996462726a7744be8bb086c73f96ff57,
                    getVisualStateProperty_996462726a7744be8bb086c73f96ff57));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_ed84f6c7f82a4d9aa9c5629f8173ea6a, Button_38b956b481c84006a0b890678a8b9425);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_6d6892eae02f41fdbb17fb652f164b51,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_fb5773338a444cbc8bd2cc90ff66f7b2,
                    setVisualStateProperty_fb5773338a444cbc8bd2cc90ff66f7b2,
                    setLocalVisualStateProperty_fb5773338a444cbc8bd2cc90ff66f7b2,
                    getVisualStateProperty_fb5773338a444cbc8bd2cc90ff66f7b2));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_6d6892eae02f41fdbb17fb652f164b51, Button_f2704b84b9c24610bf8082cfa31d98b8);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_68d4da37033d42bd91210437952956a6,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_ddc25f31b9da44e9a6e85d92302d793a,
                    setVisualStateProperty_ddc25f31b9da44e9a6e85d92302d793a,
                    setLocalVisualStateProperty_ddc25f31b9da44e9a6e85d92302d793a,
                    getVisualStateProperty_ddc25f31b9da44e9a6e85d92302d793a));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_68d4da37033d42bd91210437952956a6, StackPanel_4236c3c4298248c4a0a31e2e08687dff);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_38c69a90cb5e46ee92bebdf846289a63,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_586d9fee14b642098172624535a9b0e1,
                    setVisualStateProperty_586d9fee14b642098172624535a9b0e1,
                    setLocalVisualStateProperty_586d9fee14b642098172624535a9b0e1,
                    getVisualStateProperty_586d9fee14b642098172624535a9b0e1));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_38c69a90cb5e46ee92bebdf846289a63, Button_38b956b481c84006a0b890678a8b9425);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_86eabba1ffa44c05bb9b346f05b81a87,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_cd347efb57c54637b1df77c18e921404,
                    setVisualStateProperty_cd347efb57c54637b1df77c18e921404,
                    setLocalVisualStateProperty_cd347efb57c54637b1df77c18e921404,
                    getVisualStateProperty_cd347efb57c54637b1df77c18e921404));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_86eabba1ffa44c05bb9b346f05b81a87, Button_f2704b84b9c24610bf8082cfa31d98b8);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_fe428118ce17448d998640ba1444a29d,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_95f930ac5c8e479bbb22a702196e76c5,
                    setVisualStateProperty_95f930ac5c8e479bbb22a702196e76c5,
                    setLocalVisualStateProperty_95f930ac5c8e479bbb22a702196e76c5,
                    getVisualStateProperty_95f930ac5c8e479bbb22a702196e76c5));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_fe428118ce17448d998640ba1444a29d, TextBox_fadfe3e43eee4532ae383105252b8d5d);


            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_82ef26e9f4c147dcaa14228a2c663bba,
                new global::Windows.UI.Xaml.PropertyPath(
                    "Visibility",
                    "Visibility",
                    accessVisualStateProperty_5a58a72b322f4b59ba82da3ba51062b7,
                    setVisualStateProperty_5a58a72b322f4b59ba82da3ba51062b7,
                    setLocalVisualStateProperty_5a58a72b322f4b59ba82da3ba51062b7,
                    getVisualStateProperty_5a58a72b322f4b59ba82da3ba51062b7));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_82ef26e9f4c147dcaa14228a2c663bba, StackPanel_38cdc3083ec24095b99adf32cec3f3a9);

            templateInstance_02c86289167443ce9f58fee5a926ce26.TemplateContent = Grid_7f4ee1c24187438c85695ea996c43dd0;
            return templateInstance_02c86289167443ce9f58fee5a926ce26;
        }



    }
}
#endif