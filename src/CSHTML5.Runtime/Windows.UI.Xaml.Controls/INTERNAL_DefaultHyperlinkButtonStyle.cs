
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
    internal class INTERNAL_DefaultHyperlinkButtonStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {





var Style_e003a0b9f29841eebfa042065da7e216 = new global::System.Windows.Style();
Style_e003a0b9f29841eebfa042065da7e216.TargetType = typeof(global::System.Windows.Controls.HyperlinkButton);
var Setter_c7b92e9855044bddbb8fb691e162871e = new global::System.Windows.Setter();
Setter_c7b92e9855044bddbb8fb691e162871e.Property = global::System.Windows.Controls.HyperlinkButton.ForegroundProperty;
Setter_c7b92e9855044bddbb8fb691e162871e.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#0000EE");

var Setter_18b6ab88073746b98b528065bd1d5e73 = new global::System.Windows.Setter();
Setter_18b6ab88073746b98b528065bd1d5e73.Property = global::System.Windows.Controls.HyperlinkButton.CursorProperty;
Setter_18b6ab88073746b98b528065bd1d5e73.Value = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Hand");

var Setter_cdcbcf54a3664ba3be43f678edd8f76c = new global::System.Windows.Setter();
Setter_cdcbcf54a3664ba3be43f678edd8f76c.Property = global::System.Windows.Controls.HyperlinkButton.HorizontalContentAlignmentProperty;
Setter_cdcbcf54a3664ba3be43f678edd8f76c.Value = global::System.Windows.HorizontalAlignment.Center;

var Setter_0c5519b8341d4862b119adbd9ad4e137 = new global::System.Windows.Setter();
Setter_0c5519b8341d4862b119adbd9ad4e137.Property = global::System.Windows.Controls.HyperlinkButton.VerticalContentAlignmentProperty;
Setter_0c5519b8341d4862b119adbd9ad4e137.Value = global::System.Windows.VerticalAlignment.Center;

var Setter_2c5f7fb2ef0945df93203939c5516a35 = new global::System.Windows.Setter();
Setter_2c5f7fb2ef0945df93203939c5516a35.Property = global::System.Windows.Controls.HyperlinkButton.TemplateProperty;
var ControlTemplate_16cc557f3d994fedb699a532399edd99 = new global::System.Windows.Controls.ControlTemplate();
ControlTemplate_16cc557f3d994fedb699a532399edd99.TargetType = typeof(global::System.Windows.Controls.Button);
ControlTemplate_16cc557f3d994fedb699a532399edd99.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_16cc557f3d994fedb699a532399edd99);

Setter_2c5f7fb2ef0945df93203939c5516a35.Value = ControlTemplate_16cc557f3d994fedb699a532399edd99;


Style_e003a0b9f29841eebfa042065da7e216.Setters.Add(Setter_c7b92e9855044bddbb8fb691e162871e);
Style_e003a0b9f29841eebfa042065da7e216.Setters.Add(Setter_18b6ab88073746b98b528065bd1d5e73);
Style_e003a0b9f29841eebfa042065da7e216.Setters.Add(Setter_cdcbcf54a3664ba3be43f678edd8f76c);
Style_e003a0b9f29841eebfa042065da7e216.Setters.Add(Setter_0c5519b8341d4862b119adbd9ad4e137);
Style_e003a0b9f29841eebfa042065da7e216.Setters.Add(Setter_2c5f7fb2ef0945df93203939c5516a35);


               DefaultStyle = Style_e003a0b9f29841eebfa042065da7e216;
            }
            return DefaultStyle;






    
        }



public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::System.Windows.DependencyObject, global::System.Windows.DependencyProperty, int?>> accessVisualStateProperty_50aaaf9819334ab9a5e132c1f664445d (global::System.Windows.DependencyObject rootTargetObjectInstance)
{
  
yield break;
}


public static void setVisualStateProperty_50aaaf9819334ab9a5e132c1f664445d (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("FontWeight").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("FontWeightProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetVisualStateValue(property, value);
}


public static void setLocalVisualStateProperty_50aaaf9819334ab9a5e132c1f664445d (global::System.Windows.DependencyObject finalTargetInstance, object value)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("FontWeight").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("FontWeightProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  (finalTargetInstance).SetLocalValue(property, value);
}


public static global::System.Object getVisualStateProperty_50aaaf9819334ab9a5e132c1f664445d (global::System.Windows.DependencyObject finalTargetInstance)
{
  
global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("FontWeight").DeclaringType;
global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("FontWeightProperty");
global::System.Windows.DependencyProperty property = (global::System.Windows.DependencyProperty)propertyField.GetValue(null);

  return finalTargetInstance.GetVisualStateValue(property);
}


        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_16cc557f3d994fedb699a532399edd99(global::System.Windows.FrameworkElement templateOwner)
        {
var templateInstance_a0e8a84e8f9642d7af6474bc16917921 = new global::System.Windows.TemplateInstance();
templateInstance_a0e8a84e8f9642d7af6474bc16917921.TemplateOwner = templateOwner;
var Border_10e588adc34d463e88e9a24615356189 = new global::System.Windows.Controls.Border();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("OuterBorder", Border_10e588adc34d463e88e9a24615356189);
Border_10e588adc34d463e88e9a24615356189.Name = "OuterBorder";
var VisualStateGroup_3fba7c45fabe4f1cbfcc7ae7c4821711 = new global::System.Windows.VisualStateGroup();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_3fba7c45fabe4f1cbfcc7ae7c4821711);
VisualStateGroup_3fba7c45fabe4f1cbfcc7ae7c4821711.Name = "CommonStates";
var VisualState_6188c6a6110a4a3aac0f336a84ac4c47 = new global::System.Windows.VisualState();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Normal", VisualState_6188c6a6110a4a3aac0f336a84ac4c47);
VisualState_6188c6a6110a4a3aac0f336a84ac4c47.Name = "Normal";

var VisualState_6efcc0ae9ec6490daeeecb7447ebe26c = new global::System.Windows.VisualState();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("MouseOver", VisualState_6efcc0ae9ec6490daeeecb7447ebe26c);
VisualState_6efcc0ae9ec6490daeeecb7447ebe26c.Name = "MouseOver";
var Storyboard_1129b222f91a4e0384851fd5d69daba7 = new global::System.Windows.Media.Animation.Storyboard();
var ObjectAnimationUsingKeyFrames_e1dac6177a8a47deab31a1ce5b746eeb = new global::System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames();
global::System.Windows.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_e1dac6177a8a47deab31a1ce5b746eeb,@"ContentPresenter");
var DiscreteObjectKeyFrame_cf2dd01ca8964e7b9a9b6e69a46a84bf = new global::System.Windows.Media.Animation.DiscreteObjectKeyFrame();
DiscreteObjectKeyFrame_cf2dd01ca8964e7b9a9b6e69a46a84bf.KeyTime = (global::System.Windows.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Animation.KeyTime), @"0");
DiscreteObjectKeyFrame_cf2dd01ca8964e7b9a9b6e69a46a84bf.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Bold");

ObjectAnimationUsingKeyFrames_e1dac6177a8a47deab31a1ce5b746eeb.KeyFrames.Add(DiscreteObjectKeyFrame_cf2dd01ca8964e7b9a9b6e69a46a84bf);


Storyboard_1129b222f91a4e0384851fd5d69daba7.Children.Add(ObjectAnimationUsingKeyFrames_e1dac6177a8a47deab31a1ce5b746eeb);


VisualState_6efcc0ae9ec6490daeeecb7447ebe26c.Storyboard = Storyboard_1129b222f91a4e0384851fd5d69daba7;


VisualStateGroup_3fba7c45fabe4f1cbfcc7ae7c4821711.States.Add(VisualState_6188c6a6110a4a3aac0f336a84ac4c47);
VisualStateGroup_3fba7c45fabe4f1cbfcc7ae7c4821711.States.Add(VisualState_6efcc0ae9ec6490daeeecb7447ebe26c);


((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_3fba7c45fabe4f1cbfcc7ae7c4821711);

var ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967 = new global::System.Windows.Controls.ContentPresenter();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("ContentPresenter", ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967);
ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967.Name = "ContentPresenter";
var Binding_6bb8bbac5ac54b81b16b9be917227385 = new global::System.Windows.Data.Binding();
Binding_6bb8bbac5ac54b81b16b9be917227385.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"ContentTemplate");
var RelativeSource_47b31b183d8e47adb9a35a388f07fabd = new global::System.Windows.Data.RelativeSource();
RelativeSource_47b31b183d8e47adb9a35a388f07fabd.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_6bb8bbac5ac54b81b16b9be917227385.RelativeSource = RelativeSource_47b31b183d8e47adb9a35a388f07fabd;


Binding_6bb8bbac5ac54b81b16b9be917227385.TemplateOwner = templateInstance_a0e8a84e8f9642d7af6474bc16917921;

var Binding_8a75d1571a8244bd8e95461db8849c48 = new global::System.Windows.Data.Binding();
Binding_8a75d1571a8244bd8e95461db8849c48.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Content");
var RelativeSource_ade8eedcfb864b2cbbb8e9fb28faa8bd = new global::System.Windows.Data.RelativeSource();
RelativeSource_ade8eedcfb864b2cbbb8e9fb28faa8bd.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_8a75d1571a8244bd8e95461db8849c48.RelativeSource = RelativeSource_ade8eedcfb864b2cbbb8e9fb28faa8bd;


Binding_8a75d1571a8244bd8e95461db8849c48.TemplateOwner = templateInstance_a0e8a84e8f9642d7af6474bc16917921;

var Binding_8828adb43bc24f55ad49006edbc12c11 = new global::System.Windows.Data.Binding();
Binding_8828adb43bc24f55ad49006edbc12c11.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Padding");
var RelativeSource_e1a5e52a532c46eb9bcf33a7178b2496 = new global::System.Windows.Data.RelativeSource();
RelativeSource_e1a5e52a532c46eb9bcf33a7178b2496.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_8828adb43bc24f55ad49006edbc12c11.RelativeSource = RelativeSource_e1a5e52a532c46eb9bcf33a7178b2496;


Binding_8828adb43bc24f55ad49006edbc12c11.TemplateOwner = templateInstance_a0e8a84e8f9642d7af6474bc16917921;

var Binding_972ec641775546f8a5b27154d8558633 = new global::System.Windows.Data.Binding();
Binding_972ec641775546f8a5b27154d8558633.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"HorizontalContentAlignment");
var RelativeSource_7fe48b98bbff4030a9961fb8f09d0f86 = new global::System.Windows.Data.RelativeSource();
RelativeSource_7fe48b98bbff4030a9961fb8f09d0f86.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_972ec641775546f8a5b27154d8558633.RelativeSource = RelativeSource_7fe48b98bbff4030a9961fb8f09d0f86;


Binding_972ec641775546f8a5b27154d8558633.TemplateOwner = templateInstance_a0e8a84e8f9642d7af6474bc16917921;

var Binding_9482972fee474aa0a8ea57a49beb12a8 = new global::System.Windows.Data.Binding();
Binding_9482972fee474aa0a8ea57a49beb12a8.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"VerticalContentAlignment");
var RelativeSource_59cb809f461141dc985482166ed8a44b = new global::System.Windows.Data.RelativeSource();
RelativeSource_59cb809f461141dc985482166ed8a44b.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_9482972fee474aa0a8ea57a49beb12a8.RelativeSource = RelativeSource_59cb809f461141dc985482166ed8a44b;


Binding_9482972fee474aa0a8ea57a49beb12a8.TemplateOwner = templateInstance_a0e8a84e8f9642d7af6474bc16917921;


Border_10e588adc34d463e88e9a24615356189.Child = ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967;

var Binding_fe79b24b7748493b9b2c7dcacb549a90 = new global::System.Windows.Data.Binding();
Binding_fe79b24b7748493b9b2c7dcacb549a90.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
var RelativeSource_82fa26a5a8b4435baf362e14d938d7b5 = new global::System.Windows.Data.RelativeSource();
RelativeSource_82fa26a5a8b4435baf362e14d938d7b5.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_fe79b24b7748493b9b2c7dcacb549a90.RelativeSource = RelativeSource_82fa26a5a8b4435baf362e14d938d7b5;


Binding_fe79b24b7748493b9b2c7dcacb549a90.TemplateOwner = templateInstance_a0e8a84e8f9642d7af6474bc16917921;

var Binding_9573cfa6a53c445a9951bdfca936beb2 = new global::System.Windows.Data.Binding();
Binding_9573cfa6a53c445a9951bdfca936beb2.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
var RelativeSource_0319c934045b4528bc539bd320506136 = new global::System.Windows.Data.RelativeSource();
RelativeSource_0319c934045b4528bc539bd320506136.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_9573cfa6a53c445a9951bdfca936beb2.RelativeSource = RelativeSource_0319c934045b4528bc539bd320506136;


Binding_9573cfa6a53c445a9951bdfca936beb2.TemplateOwner = templateInstance_a0e8a84e8f9642d7af6474bc16917921;

var Binding_4acf08365a8c4e62870ca62f77e9b8df = new global::System.Windows.Data.Binding();
Binding_4acf08365a8c4e62870ca62f77e9b8df.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
var RelativeSource_327d9513ca4a484e8bde58261850d555 = new global::System.Windows.Data.RelativeSource();
RelativeSource_327d9513ca4a484e8bde58261850d555.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_4acf08365a8c4e62870ca62f77e9b8df.RelativeSource = RelativeSource_327d9513ca4a484e8bde58261850d555;


Binding_4acf08365a8c4e62870ca62f77e9b8df.TemplateOwner = templateInstance_a0e8a84e8f9642d7af6474bc16917921;



ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967.SetBinding(global::System.Windows.Controls.ContentControl.ContentTemplateProperty, Binding_6bb8bbac5ac54b81b16b9be917227385);
ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967.SetBinding(global::System.Windows.Controls.ContentControl.ContentProperty, Binding_8a75d1571a8244bd8e95461db8849c48);
ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967.SetBinding(global::System.Windows.FrameworkElement.MarginProperty, Binding_8828adb43bc24f55ad49006edbc12c11);
ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967.SetBinding(global::System.Windows.FrameworkElement.HorizontalAlignmentProperty, Binding_972ec641775546f8a5b27154d8558633);
ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967.SetBinding(global::System.Windows.FrameworkElement.VerticalAlignmentProperty, Binding_9482972fee474aa0a8ea57a49beb12a8);
Border_10e588adc34d463e88e9a24615356189.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_fe79b24b7748493b9b2c7dcacb549a90);
Border_10e588adc34d463e88e9a24615356189.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_9573cfa6a53c445a9951bdfca936beb2);
Border_10e588adc34d463e88e9a24615356189.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_4acf08365a8c4e62870ca62f77e9b8df);

global::System.Windows.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_e1dac6177a8a47deab31a1ce5b746eeb,
    new global::System.Windows.PropertyPath(
        "FontWeight",
        "FontWeight",
        accessVisualStateProperty_50aaaf9819334ab9a5e132c1f664445d,
        setVisualStateProperty_50aaaf9819334ab9a5e132c1f664445d,
        setLocalVisualStateProperty_50aaaf9819334ab9a5e132c1f664445d,
        getVisualStateProperty_50aaaf9819334ab9a5e132c1f664445d));
global::System.Windows.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_e1dac6177a8a47deab31a1ce5b746eeb, ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967);

templateInstance_a0e8a84e8f9642d7af6474bc16917921.TemplateContent = Border_10e588adc34d463e88e9a24615356189;
return templateInstance_a0e8a84e8f9642d7af6474bc16917921;
        }



        }
}
#else

namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultHyperlinkButtonStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {





                var Style_e003a0b9f29841eebfa042065da7e216 = new global::Windows.UI.Xaml.Style();
                Style_e003a0b9f29841eebfa042065da7e216.TargetType = typeof(global::Windows.UI.Xaml.Controls.HyperlinkButton);
                var Setter_c7b92e9855044bddbb8fb691e162871e = new global::Windows.UI.Xaml.Setter();
                Setter_c7b92e9855044bddbb8fb691e162871e.Property = global::Windows.UI.Xaml.Controls.HyperlinkButton.ForegroundProperty;
                Setter_c7b92e9855044bddbb8fb691e162871e.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#0000EE");

                var Setter_18b6ab88073746b98b528065bd1d5e73 = new global::Windows.UI.Xaml.Setter();
                Setter_18b6ab88073746b98b528065bd1d5e73.Property = global::Windows.UI.Xaml.Controls.HyperlinkButton.CursorProperty;
                Setter_18b6ab88073746b98b528065bd1d5e73.Value = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Hand");

                var Setter_cdcbcf54a3664ba3be43f678edd8f76c = new global::Windows.UI.Xaml.Setter();
                Setter_cdcbcf54a3664ba3be43f678edd8f76c.Property = global::Windows.UI.Xaml.Controls.HyperlinkButton.HorizontalContentAlignmentProperty;
                Setter_cdcbcf54a3664ba3be43f678edd8f76c.Value = global::Windows.UI.Xaml.HorizontalAlignment.Center;

                var Setter_0c5519b8341d4862b119adbd9ad4e137 = new global::Windows.UI.Xaml.Setter();
                Setter_0c5519b8341d4862b119adbd9ad4e137.Property = global::Windows.UI.Xaml.Controls.HyperlinkButton.VerticalContentAlignmentProperty;
                Setter_0c5519b8341d4862b119adbd9ad4e137.Value = global::Windows.UI.Xaml.VerticalAlignment.Center;

                var Setter_2c5f7fb2ef0945df93203939c5516a35 = new global::Windows.UI.Xaml.Setter();
                Setter_2c5f7fb2ef0945df93203939c5516a35.Property = global::Windows.UI.Xaml.Controls.HyperlinkButton.TemplateProperty;
                var ControlTemplate_16cc557f3d994fedb699a532399edd99 = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_16cc557f3d994fedb699a532399edd99.TargetType = typeof(global::Windows.UI.Xaml.Controls.Button);
                ControlTemplate_16cc557f3d994fedb699a532399edd99.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_16cc557f3d994fedb699a532399edd99);

                Setter_2c5f7fb2ef0945df93203939c5516a35.Value = ControlTemplate_16cc557f3d994fedb699a532399edd99;


                Style_e003a0b9f29841eebfa042065da7e216.Setters.Add(Setter_c7b92e9855044bddbb8fb691e162871e);
                Style_e003a0b9f29841eebfa042065da7e216.Setters.Add(Setter_18b6ab88073746b98b528065bd1d5e73);
                Style_e003a0b9f29841eebfa042065da7e216.Setters.Add(Setter_cdcbcf54a3664ba3be43f678edd8f76c);
                Style_e003a0b9f29841eebfa042065da7e216.Setters.Add(Setter_0c5519b8341d4862b119adbd9ad4e137);
                Style_e003a0b9f29841eebfa042065da7e216.Setters.Add(Setter_2c5f7fb2ef0945df93203939c5516a35);


                DefaultStyle = Style_e003a0b9f29841eebfa042065da7e216;
            }
            return DefaultStyle;







        }



        public static global::System.Collections.Generic.IEnumerable<global::System.Tuple<global::Windows.UI.Xaml.DependencyObject, global::Windows.UI.Xaml.DependencyProperty, int?>> accessVisualStateProperty_50aaaf9819334ab9a5e132c1f664445d(global::Windows.UI.Xaml.DependencyObject rootTargetObjectInstance)
        {

            yield break;
        }


        public static void setVisualStateProperty_50aaaf9819334ab9a5e132c1f664445d(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("FontWeight").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("FontWeightProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetVisualStateValue(property, value);
        }


        public static void setLocalVisualStateProperty_50aaaf9819334ab9a5e132c1f664445d(global::Windows.UI.Xaml.DependencyObject finalTargetInstance, object value)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("FontWeight").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("FontWeightProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            (finalTargetInstance).SetLocalValue(property, value);
        }


        public static global::System.Object getVisualStateProperty_50aaaf9819334ab9a5e132c1f664445d(global::Windows.UI.Xaml.DependencyObject finalTargetInstance)
        {

            global::System.Type finalTargetInstanceType = finalTargetInstance.GetType();
            global::System.Type propertyDeclaringType = finalTargetInstanceType.GetProperty("FontWeight").DeclaringType;
            global::System.Reflection.FieldInfo propertyField = propertyDeclaringType.GetField("FontWeightProperty");
            global::Windows.UI.Xaml.DependencyProperty property = (global::Windows.UI.Xaml.DependencyProperty)propertyField.GetValue(null);

            return finalTargetInstance.GetVisualStateValue(property);
        }


        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_16cc557f3d994fedb699a532399edd99(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_a0e8a84e8f9642d7af6474bc16917921 = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_a0e8a84e8f9642d7af6474bc16917921.TemplateOwner = templateOwner;
            var Border_10e588adc34d463e88e9a24615356189 = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("OuterBorder", Border_10e588adc34d463e88e9a24615356189);
            Border_10e588adc34d463e88e9a24615356189.Name = "OuterBorder";
            var VisualStateGroup_3fba7c45fabe4f1cbfcc7ae7c4821711 = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_3fba7c45fabe4f1cbfcc7ae7c4821711);
            VisualStateGroup_3fba7c45fabe4f1cbfcc7ae7c4821711.Name = "CommonStates";
            var VisualState_6188c6a6110a4a3aac0f336a84ac4c47 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Normal", VisualState_6188c6a6110a4a3aac0f336a84ac4c47);
            VisualState_6188c6a6110a4a3aac0f336a84ac4c47.Name = "Normal";

            var VisualState_6efcc0ae9ec6490daeeecb7447ebe26c = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("PointerOver", VisualState_6efcc0ae9ec6490daeeecb7447ebe26c);
            VisualState_6efcc0ae9ec6490daeeecb7447ebe26c.Name = "PointerOver";
            var Storyboard_1129b222f91a4e0384851fd5d69daba7 = new global::Windows.UI.Xaml.Media.Animation.Storyboard();
            var ObjectAnimationUsingKeyFrames_e1dac6177a8a47deab31a1ce5b746eeb = new global::Windows.UI.Xaml.Media.Animation.ObjectAnimationUsingKeyFrames();
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetName(ObjectAnimationUsingKeyFrames_e1dac6177a8a47deab31a1ce5b746eeb, @"ContentPresenter");
            var DiscreteObjectKeyFrame_cf2dd01ca8964e7b9a9b6e69a46a84bf = new global::Windows.UI.Xaml.Media.Animation.DiscreteObjectKeyFrame();
            DiscreteObjectKeyFrame_cf2dd01ca8964e7b9a9b6e69a46a84bf.KeyTime = (global::Windows.UI.Xaml.Media.Animation.KeyTime)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Animation.KeyTime), @"0");
            DiscreteObjectKeyFrame_cf2dd01ca8964e7b9a9b6e69a46a84bf.Value = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"Bold");

            ObjectAnimationUsingKeyFrames_e1dac6177a8a47deab31a1ce5b746eeb.KeyFrames.Add(DiscreteObjectKeyFrame_cf2dd01ca8964e7b9a9b6e69a46a84bf);


            Storyboard_1129b222f91a4e0384851fd5d69daba7.Children.Add(ObjectAnimationUsingKeyFrames_e1dac6177a8a47deab31a1ce5b746eeb);


            VisualState_6efcc0ae9ec6490daeeecb7447ebe26c.Storyboard = Storyboard_1129b222f91a4e0384851fd5d69daba7;


            VisualStateGroup_3fba7c45fabe4f1cbfcc7ae7c4821711.States.Add(VisualState_6188c6a6110a4a3aac0f336a84ac4c47);
            VisualStateGroup_3fba7c45fabe4f1cbfcc7ae7c4821711.States.Add(VisualState_6efcc0ae9ec6490daeeecb7447ebe26c);


            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_3fba7c45fabe4f1cbfcc7ae7c4821711);

            var ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967 = new global::Windows.UI.Xaml.Controls.ContentPresenter();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ContentPresenter", ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967);
            ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967.Name = "ContentPresenter";
            var Binding_6bb8bbac5ac54b81b16b9be917227385 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_6bb8bbac5ac54b81b16b9be917227385.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"ContentTemplate");
            var RelativeSource_47b31b183d8e47adb9a35a388f07fabd = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_47b31b183d8e47adb9a35a388f07fabd.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_6bb8bbac5ac54b81b16b9be917227385.RelativeSource = RelativeSource_47b31b183d8e47adb9a35a388f07fabd;


            Binding_6bb8bbac5ac54b81b16b9be917227385.TemplateOwner = templateInstance_a0e8a84e8f9642d7af6474bc16917921;

            var Binding_8a75d1571a8244bd8e95461db8849c48 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_8a75d1571a8244bd8e95461db8849c48.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Content");
            var RelativeSource_ade8eedcfb864b2cbbb8e9fb28faa8bd = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_ade8eedcfb864b2cbbb8e9fb28faa8bd.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_8a75d1571a8244bd8e95461db8849c48.RelativeSource = RelativeSource_ade8eedcfb864b2cbbb8e9fb28faa8bd;


            Binding_8a75d1571a8244bd8e95461db8849c48.TemplateOwner = templateInstance_a0e8a84e8f9642d7af6474bc16917921;

            var Binding_8828adb43bc24f55ad49006edbc12c11 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_8828adb43bc24f55ad49006edbc12c11.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Padding");
            var RelativeSource_e1a5e52a532c46eb9bcf33a7178b2496 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_e1a5e52a532c46eb9bcf33a7178b2496.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_8828adb43bc24f55ad49006edbc12c11.RelativeSource = RelativeSource_e1a5e52a532c46eb9bcf33a7178b2496;


            Binding_8828adb43bc24f55ad49006edbc12c11.TemplateOwner = templateInstance_a0e8a84e8f9642d7af6474bc16917921;

            var Binding_972ec641775546f8a5b27154d8558633 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_972ec641775546f8a5b27154d8558633.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"HorizontalContentAlignment");
            var RelativeSource_7fe48b98bbff4030a9961fb8f09d0f86 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_7fe48b98bbff4030a9961fb8f09d0f86.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_972ec641775546f8a5b27154d8558633.RelativeSource = RelativeSource_7fe48b98bbff4030a9961fb8f09d0f86;


            Binding_972ec641775546f8a5b27154d8558633.TemplateOwner = templateInstance_a0e8a84e8f9642d7af6474bc16917921;

            var Binding_9482972fee474aa0a8ea57a49beb12a8 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_9482972fee474aa0a8ea57a49beb12a8.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"VerticalContentAlignment");
            var RelativeSource_59cb809f461141dc985482166ed8a44b = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_59cb809f461141dc985482166ed8a44b.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_9482972fee474aa0a8ea57a49beb12a8.RelativeSource = RelativeSource_59cb809f461141dc985482166ed8a44b;


            Binding_9482972fee474aa0a8ea57a49beb12a8.TemplateOwner = templateInstance_a0e8a84e8f9642d7af6474bc16917921;


            Border_10e588adc34d463e88e9a24615356189.Child = ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967;

            var Binding_fe79b24b7748493b9b2c7dcacb549a90 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_fe79b24b7748493b9b2c7dcacb549a90.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_82fa26a5a8b4435baf362e14d938d7b5 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_82fa26a5a8b4435baf362e14d938d7b5.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_fe79b24b7748493b9b2c7dcacb549a90.RelativeSource = RelativeSource_82fa26a5a8b4435baf362e14d938d7b5;


            Binding_fe79b24b7748493b9b2c7dcacb549a90.TemplateOwner = templateInstance_a0e8a84e8f9642d7af6474bc16917921;

            var Binding_9573cfa6a53c445a9951bdfca936beb2 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_9573cfa6a53c445a9951bdfca936beb2.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_0319c934045b4528bc539bd320506136 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_0319c934045b4528bc539bd320506136.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_9573cfa6a53c445a9951bdfca936beb2.RelativeSource = RelativeSource_0319c934045b4528bc539bd320506136;


            Binding_9573cfa6a53c445a9951bdfca936beb2.TemplateOwner = templateInstance_a0e8a84e8f9642d7af6474bc16917921;

            var Binding_4acf08365a8c4e62870ca62f77e9b8df = new global::Windows.UI.Xaml.Data.Binding();
            Binding_4acf08365a8c4e62870ca62f77e9b8df.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_327d9513ca4a484e8bde58261850d555 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_327d9513ca4a484e8bde58261850d555.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_4acf08365a8c4e62870ca62f77e9b8df.RelativeSource = RelativeSource_327d9513ca4a484e8bde58261850d555;


            Binding_4acf08365a8c4e62870ca62f77e9b8df.TemplateOwner = templateInstance_a0e8a84e8f9642d7af6474bc16917921;



            ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentTemplateProperty, Binding_6bb8bbac5ac54b81b16b9be917227385);
            ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentProperty, Binding_8a75d1571a8244bd8e95461db8849c48);
            ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967.SetBinding(global::Windows.UI.Xaml.FrameworkElement.MarginProperty, Binding_8828adb43bc24f55ad49006edbc12c11);
            ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967.SetBinding(global::Windows.UI.Xaml.FrameworkElement.HorizontalAlignmentProperty, Binding_972ec641775546f8a5b27154d8558633);
            ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967.SetBinding(global::Windows.UI.Xaml.FrameworkElement.VerticalAlignmentProperty, Binding_9482972fee474aa0a8ea57a49beb12a8);
            Border_10e588adc34d463e88e9a24615356189.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_fe79b24b7748493b9b2c7dcacb549a90);
            Border_10e588adc34d463e88e9a24615356189.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_9573cfa6a53c445a9951bdfca936beb2);
            Border_10e588adc34d463e88e9a24615356189.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_4acf08365a8c4e62870ca62f77e9b8df);

            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(ObjectAnimationUsingKeyFrames_e1dac6177a8a47deab31a1ce5b746eeb,
                new global::Windows.UI.Xaml.PropertyPath(
                    "FontWeight",
                    "FontWeight",
                    accessVisualStateProperty_50aaaf9819334ab9a5e132c1f664445d,
                    setVisualStateProperty_50aaaf9819334ab9a5e132c1f664445d,
                    setLocalVisualStateProperty_50aaaf9819334ab9a5e132c1f664445d,
                    getVisualStateProperty_50aaaf9819334ab9a5e132c1f664445d));
            global::Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(ObjectAnimationUsingKeyFrames_e1dac6177a8a47deab31a1ce5b746eeb, ContentPresenter_2d6617b470214a2cbebfee7ac5ec8967);

            templateInstance_a0e8a84e8f9642d7af6474bc16917921.TemplateContent = Border_10e588adc34d463e88e9a24615356189;
            return templateInstance_a0e8a84e8f9642d7af6474bc16917921;
        }



    }
}
#endif