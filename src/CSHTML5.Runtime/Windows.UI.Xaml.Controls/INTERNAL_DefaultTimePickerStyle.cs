
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
    internal class INTERNAL_DefaultTimePickerStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {





var Style_8b54b07bd61b412b9fb040355536950e = new global::System.Windows.Style();
Style_8b54b07bd61b412b9fb040355536950e.TargetType = typeof(global::System.Windows.Controls.TimePicker);
var Setter_87ea2dadcf054fbfad00f78a4ee02588 = new global::System.Windows.Setter();
Setter_87ea2dadcf054fbfad00f78a4ee02588.Property = global::System.Windows.Controls.TimePicker.BorderBrushProperty;
Setter_87ea2dadcf054fbfad00f78a4ee02588.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Gray");

var Setter_651c2d1de5a046ac881cd9be11c65388 = new global::System.Windows.Setter();
Setter_651c2d1de5a046ac881cd9be11c65388.Property = global::System.Windows.Controls.TimePicker.BorderThicknessProperty;
Setter_651c2d1de5a046ac881cd9be11c65388.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");

var Setter_65956154e98e421a91c4b63791260723 = new global::System.Windows.Setter();
Setter_65956154e98e421a91c4b63791260723.Property = global::System.Windows.Controls.TimePicker.TemplateProperty;
var ControlTemplate_2edd8eda64044039bbbb3e6198beb231 = new global::System.Windows.Controls.ControlTemplate();
ControlTemplate_2edd8eda64044039bbbb3e6198beb231.TargetType = typeof(global::System.Windows.Controls.TimePicker);
ControlTemplate_2edd8eda64044039bbbb3e6198beb231.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_2edd8eda64044039bbbb3e6198beb231);

Setter_65956154e98e421a91c4b63791260723.Value = ControlTemplate_2edd8eda64044039bbbb3e6198beb231;


Style_8b54b07bd61b412b9fb040355536950e.Setters.Add(Setter_87ea2dadcf054fbfad00f78a4ee02588);
Style_8b54b07bd61b412b9fb040355536950e.Setters.Add(Setter_651c2d1de5a046ac881cd9be11c65388);
Style_8b54b07bd61b412b9fb040355536950e.Setters.Add(Setter_65956154e98e421a91c4b63791260723);


               DefaultStyle = Style_8b54b07bd61b412b9fb040355536950e;
            }
            return DefaultStyle;






    
        }



        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_2edd8eda64044039bbbb3e6198beb231(global::System.Windows.FrameworkElement templateOwner)
        {
var templateInstance_e63a3324cd3840b8bb6a590db0a94283 = new global::System.Windows.TemplateInstance();
templateInstance_e63a3324cd3840b8bb6a590db0a94283.TemplateOwner = templateOwner;
var Grid_83fdab7a48894fd18f82be1c873d4679 = new global::System.Windows.Controls.Grid();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Root", Grid_83fdab7a48894fd18f82be1c873d4679);
Grid_83fdab7a48894fd18f82be1c873d4679.Name = "Root";
var ColumnDefinition_40e1206d34ca4895abfe888b8af328dc = new global::System.Windows.Controls.ColumnDefinition();
ColumnDefinition_40e1206d34ca4895abfe888b8af328dc.Width = (global::System.Windows.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.GridLength), @"*");

var ColumnDefinition_527f5ebe8b8a487083fa30b3655a5eb4 = new global::System.Windows.Controls.ColumnDefinition();
ColumnDefinition_527f5ebe8b8a487083fa30b3655a5eb4.Width = (global::System.Windows.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.GridLength), @"Auto");

Grid_83fdab7a48894fd18f82be1c873d4679.ColumnDefinitions.Add(ColumnDefinition_40e1206d34ca4895abfe888b8af328dc);
Grid_83fdab7a48894fd18f82be1c873d4679.ColumnDefinitions.Add(ColumnDefinition_527f5ebe8b8a487083fa30b3655a5eb4);

var TextBox_e8ad548eda0047e9a6f219e5890ad050 = new global::System.Windows.Controls.TextBox();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("TextBox", TextBox_e8ad548eda0047e9a6f219e5890ad050);
TextBox_e8ad548eda0047e9a6f219e5890ad050.Name = "TextBox";
global::System.Windows.Controls.Grid.SetColumn(TextBox_e8ad548eda0047e9a6f219e5890ad050,(global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"0"));
var Binding_9bfb5d7cf4aa44ac854b34116f9b6d66 = new global::System.Windows.Data.Binding();
Binding_9bfb5d7cf4aa44ac854b34116f9b6d66.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
var RelativeSource_f4ee9cb21d2a4eb1bc400b2c26fd0dcc = new global::System.Windows.Data.RelativeSource();
RelativeSource_f4ee9cb21d2a4eb1bc400b2c26fd0dcc.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_9bfb5d7cf4aa44ac854b34116f9b6d66.RelativeSource = RelativeSource_f4ee9cb21d2a4eb1bc400b2c26fd0dcc;


Binding_9bfb5d7cf4aa44ac854b34116f9b6d66.TemplateOwner = templateInstance_e63a3324cd3840b8bb6a590db0a94283;

var Binding_c7b34b15a8fd4ff4acf94b73893e4529 = new global::System.Windows.Data.Binding();
Binding_c7b34b15a8fd4ff4acf94b73893e4529.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
var RelativeSource_adefd8da691644fcabf12ad850c9e6a9 = new global::System.Windows.Data.RelativeSource();
RelativeSource_adefd8da691644fcabf12ad850c9e6a9.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_c7b34b15a8fd4ff4acf94b73893e4529.RelativeSource = RelativeSource_adefd8da691644fcabf12ad850c9e6a9;


Binding_c7b34b15a8fd4ff4acf94b73893e4529.TemplateOwner = templateInstance_e63a3324cd3840b8bb6a590db0a94283;

var Binding_b7b85e330b574a7ead3934864e6aef11 = new global::System.Windows.Data.Binding();
Binding_b7b85e330b574a7ead3934864e6aef11.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
var RelativeSource_64d04ff10418401993520dd22b42ddbd = new global::System.Windows.Data.RelativeSource();
RelativeSource_64d04ff10418401993520dd22b42ddbd.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_b7b85e330b574a7ead3934864e6aef11.RelativeSource = RelativeSource_64d04ff10418401993520dd22b42ddbd;


Binding_b7b85e330b574a7ead3934864e6aef11.TemplateOwner = templateInstance_e63a3324cd3840b8bb6a590db0a94283;

var Binding_2ba4d05177cf40298950abe15a9ae41e = new global::System.Windows.Data.Binding();
Binding_2ba4d05177cf40298950abe15a9ae41e.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Padding");
var RelativeSource_6d1393c985644dea9ce7a991de1f4f55 = new global::System.Windows.Data.RelativeSource();
RelativeSource_6d1393c985644dea9ce7a991de1f4f55.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_2ba4d05177cf40298950abe15a9ae41e.RelativeSource = RelativeSource_6d1393c985644dea9ce7a991de1f4f55;


Binding_2ba4d05177cf40298950abe15a9ae41e.TemplateOwner = templateInstance_e63a3324cd3840b8bb6a590db0a94283;


var Button_98e020b5b2e247c4a65dde2bcf95be2e = new global::System.Windows.Controls.Button();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Button", Button_98e020b5b2e247c4a65dde2bcf95be2e);
Button_98e020b5b2e247c4a65dde2bcf95be2e.Name = "Button";
global::System.Windows.Controls.Grid.SetColumn(Button_98e020b5b2e247c4a65dde2bcf95be2e,(global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"1"));
Button_98e020b5b2e247c4a65dde2bcf95be2e.Content = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"▼");
Button_98e020b5b2e247c4a65dde2bcf95be2e.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"30");
Button_98e020b5b2e247c4a65dde2bcf95be2e.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"2,0,2,0");
var Binding_fcba859302854aab9b9c439d2e871267 = new global::System.Windows.Data.Binding();
Binding_fcba859302854aab9b9c439d2e871267.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Foreground");
var RelativeSource_8013d701695b46ad94dd5d59e00a2cb0 = new global::System.Windows.Data.RelativeSource();
RelativeSource_8013d701695b46ad94dd5d59e00a2cb0.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_fcba859302854aab9b9c439d2e871267.RelativeSource = RelativeSource_8013d701695b46ad94dd5d59e00a2cb0;


Binding_fcba859302854aab9b9c439d2e871267.TemplateOwner = templateInstance_e63a3324cd3840b8bb6a590db0a94283;

var Binding_9df19b0897774b359d6300faa9e11666 = new global::System.Windows.Data.Binding();
Binding_9df19b0897774b359d6300faa9e11666.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
var RelativeSource_e7ff9ba4dbf841fa948dc73a29d77a53 = new global::System.Windows.Data.RelativeSource();
RelativeSource_e7ff9ba4dbf841fa948dc73a29d77a53.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_9df19b0897774b359d6300faa9e11666.RelativeSource = RelativeSource_e7ff9ba4dbf841fa948dc73a29d77a53;


Binding_9df19b0897774b359d6300faa9e11666.TemplateOwner = templateInstance_e63a3324cd3840b8bb6a590db0a94283;

var Binding_60e905380dcb41449831e462e6f80c83 = new global::System.Windows.Data.Binding();
Binding_60e905380dcb41449831e462e6f80c83.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
var RelativeSource_8d6049fc3c4147bdb7282c50459d6651 = new global::System.Windows.Data.RelativeSource();
RelativeSource_8d6049fc3c4147bdb7282c50459d6651.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_60e905380dcb41449831e462e6f80c83.RelativeSource = RelativeSource_8d6049fc3c4147bdb7282c50459d6651;


Binding_60e905380dcb41449831e462e6f80c83.TemplateOwner = templateInstance_e63a3324cd3840b8bb6a590db0a94283;


var Popup_178cd792b23d46ccbcf76958fd333e1d = new global::System.Windows.Controls.Primitives.Popup();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Popup", Popup_178cd792b23d46ccbcf76958fd333e1d);
Popup_178cd792b23d46ccbcf76958fd333e1d.Name = "Popup";
Popup_178cd792b23d46ccbcf76958fd333e1d.VerticalAlignment = global::System.Windows.VerticalAlignment.Bottom;

Grid_83fdab7a48894fd18f82be1c873d4679.Children.Add(TextBox_e8ad548eda0047e9a6f219e5890ad050);
Grid_83fdab7a48894fd18f82be1c873d4679.Children.Add(Button_98e020b5b2e247c4a65dde2bcf95be2e);
Grid_83fdab7a48894fd18f82be1c873d4679.Children.Add(Popup_178cd792b23d46ccbcf76958fd333e1d);



TextBox_e8ad548eda0047e9a6f219e5890ad050.SetBinding(global::System.Windows.Controls.Control.BackgroundProperty, Binding_9bfb5d7cf4aa44ac854b34116f9b6d66);
TextBox_e8ad548eda0047e9a6f219e5890ad050.SetBinding(global::System.Windows.Controls.Control.BorderBrushProperty, Binding_c7b34b15a8fd4ff4acf94b73893e4529);
TextBox_e8ad548eda0047e9a6f219e5890ad050.SetBinding(global::System.Windows.Controls.Control.BorderThicknessProperty, Binding_b7b85e330b574a7ead3934864e6aef11);
TextBox_e8ad548eda0047e9a6f219e5890ad050.SetBinding(global::System.Windows.Controls.Control.PaddingProperty, Binding_2ba4d05177cf40298950abe15a9ae41e);
Button_98e020b5b2e247c4a65dde2bcf95be2e.SetBinding(global::System.Windows.Controls.Control.ForegroundProperty, Binding_fcba859302854aab9b9c439d2e871267);
Button_98e020b5b2e247c4a65dde2bcf95be2e.SetBinding(global::System.Windows.Controls.Control.BorderBrushProperty, Binding_9df19b0897774b359d6300faa9e11666);
Button_98e020b5b2e247c4a65dde2bcf95be2e.SetBinding(global::System.Windows.Controls.Control.BorderThicknessProperty, Binding_60e905380dcb41449831e462e6f80c83);

templateInstance_e63a3324cd3840b8bb6a590db0a94283.TemplateContent = Grid_83fdab7a48894fd18f82be1c873d4679;
return templateInstance_e63a3324cd3840b8bb6a590db0a94283;
        }



        }
}
#else

namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultTimePickerStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {





                var Style_8b54b07bd61b412b9fb040355536950e = new global::Windows.UI.Xaml.Style();
                Style_8b54b07bd61b412b9fb040355536950e.TargetType = typeof(global::Windows.UI.Xaml.Controls.TimePicker);
                var Setter_87ea2dadcf054fbfad00f78a4ee02588 = new global::Windows.UI.Xaml.Setter();
                Setter_87ea2dadcf054fbfad00f78a4ee02588.Property = global::Windows.UI.Xaml.Controls.TimePicker.BorderBrushProperty;
                Setter_87ea2dadcf054fbfad00f78a4ee02588.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Gray");

                var Setter_651c2d1de5a046ac881cd9be11c65388 = new global::Windows.UI.Xaml.Setter();
                Setter_651c2d1de5a046ac881cd9be11c65388.Property = global::Windows.UI.Xaml.Controls.TimePicker.BorderThicknessProperty;
                Setter_651c2d1de5a046ac881cd9be11c65388.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");

                var Setter_65956154e98e421a91c4b63791260723 = new global::Windows.UI.Xaml.Setter();
                Setter_65956154e98e421a91c4b63791260723.Property = global::Windows.UI.Xaml.Controls.TimePicker.TemplateProperty;
                var ControlTemplate_2edd8eda64044039bbbb3e6198beb231 = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_2edd8eda64044039bbbb3e6198beb231.TargetType = typeof(global::Windows.UI.Xaml.Controls.TimePicker);
                ControlTemplate_2edd8eda64044039bbbb3e6198beb231.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_2edd8eda64044039bbbb3e6198beb231);

                Setter_65956154e98e421a91c4b63791260723.Value = ControlTemplate_2edd8eda64044039bbbb3e6198beb231;


                Style_8b54b07bd61b412b9fb040355536950e.Setters.Add(Setter_87ea2dadcf054fbfad00f78a4ee02588);
                Style_8b54b07bd61b412b9fb040355536950e.Setters.Add(Setter_651c2d1de5a046ac881cd9be11c65388);
                Style_8b54b07bd61b412b9fb040355536950e.Setters.Add(Setter_65956154e98e421a91c4b63791260723);


                DefaultStyle = Style_8b54b07bd61b412b9fb040355536950e;
            }
            return DefaultStyle;







        }



        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_2edd8eda64044039bbbb3e6198beb231(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_e63a3324cd3840b8bb6a590db0a94283 = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_e63a3324cd3840b8bb6a590db0a94283.TemplateOwner = templateOwner;
            var Grid_83fdab7a48894fd18f82be1c873d4679 = new global::Windows.UI.Xaml.Controls.Grid();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Root", Grid_83fdab7a48894fd18f82be1c873d4679);
            Grid_83fdab7a48894fd18f82be1c873d4679.Name = "Root";
            var ColumnDefinition_40e1206d34ca4895abfe888b8af328dc = new global::Windows.UI.Xaml.Controls.ColumnDefinition();
            ColumnDefinition_40e1206d34ca4895abfe888b8af328dc.Width = (global::Windows.UI.Xaml.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.GridLength), @"*");

            var ColumnDefinition_527f5ebe8b8a487083fa30b3655a5eb4 = new global::Windows.UI.Xaml.Controls.ColumnDefinition();
            ColumnDefinition_527f5ebe8b8a487083fa30b3655a5eb4.Width = (global::Windows.UI.Xaml.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.GridLength), @"Auto");

            Grid_83fdab7a48894fd18f82be1c873d4679.ColumnDefinitions.Add(ColumnDefinition_40e1206d34ca4895abfe888b8af328dc);
            Grid_83fdab7a48894fd18f82be1c873d4679.ColumnDefinitions.Add(ColumnDefinition_527f5ebe8b8a487083fa30b3655a5eb4);

            var TextBox_e8ad548eda0047e9a6f219e5890ad050 = new global::Windows.UI.Xaml.Controls.TextBox();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("TextBox", TextBox_e8ad548eda0047e9a6f219e5890ad050);
            TextBox_e8ad548eda0047e9a6f219e5890ad050.Name = "TextBox";
            global::Windows.UI.Xaml.Controls.Grid.SetColumn(TextBox_e8ad548eda0047e9a6f219e5890ad050, (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"0"));
            var Binding_9bfb5d7cf4aa44ac854b34116f9b6d66 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_9bfb5d7cf4aa44ac854b34116f9b6d66.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_f4ee9cb21d2a4eb1bc400b2c26fd0dcc = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_f4ee9cb21d2a4eb1bc400b2c26fd0dcc.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_9bfb5d7cf4aa44ac854b34116f9b6d66.RelativeSource = RelativeSource_f4ee9cb21d2a4eb1bc400b2c26fd0dcc;


            Binding_9bfb5d7cf4aa44ac854b34116f9b6d66.TemplateOwner = templateInstance_e63a3324cd3840b8bb6a590db0a94283;

            var Binding_c7b34b15a8fd4ff4acf94b73893e4529 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_c7b34b15a8fd4ff4acf94b73893e4529.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_adefd8da691644fcabf12ad850c9e6a9 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_adefd8da691644fcabf12ad850c9e6a9.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_c7b34b15a8fd4ff4acf94b73893e4529.RelativeSource = RelativeSource_adefd8da691644fcabf12ad850c9e6a9;


            Binding_c7b34b15a8fd4ff4acf94b73893e4529.TemplateOwner = templateInstance_e63a3324cd3840b8bb6a590db0a94283;

            var Binding_b7b85e330b574a7ead3934864e6aef11 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_b7b85e330b574a7ead3934864e6aef11.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_64d04ff10418401993520dd22b42ddbd = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_64d04ff10418401993520dd22b42ddbd.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_b7b85e330b574a7ead3934864e6aef11.RelativeSource = RelativeSource_64d04ff10418401993520dd22b42ddbd;


            Binding_b7b85e330b574a7ead3934864e6aef11.TemplateOwner = templateInstance_e63a3324cd3840b8bb6a590db0a94283;

            var Binding_2ba4d05177cf40298950abe15a9ae41e = new global::Windows.UI.Xaml.Data.Binding();
            Binding_2ba4d05177cf40298950abe15a9ae41e.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Padding");
            var RelativeSource_6d1393c985644dea9ce7a991de1f4f55 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_6d1393c985644dea9ce7a991de1f4f55.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_2ba4d05177cf40298950abe15a9ae41e.RelativeSource = RelativeSource_6d1393c985644dea9ce7a991de1f4f55;


            Binding_2ba4d05177cf40298950abe15a9ae41e.TemplateOwner = templateInstance_e63a3324cd3840b8bb6a590db0a94283;


            var Button_98e020b5b2e247c4a65dde2bcf95be2e = new global::Windows.UI.Xaml.Controls.Button();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Button", Button_98e020b5b2e247c4a65dde2bcf95be2e);
            Button_98e020b5b2e247c4a65dde2bcf95be2e.Name = "Button";
            global::Windows.UI.Xaml.Controls.Grid.SetColumn(Button_98e020b5b2e247c4a65dde2bcf95be2e, (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"1"));
            Button_98e020b5b2e247c4a65dde2bcf95be2e.Content = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"▼");
            Button_98e020b5b2e247c4a65dde2bcf95be2e.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"30");
            Button_98e020b5b2e247c4a65dde2bcf95be2e.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"2,0,2,0");
            var Binding_fcba859302854aab9b9c439d2e871267 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_fcba859302854aab9b9c439d2e871267.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Foreground");
            var RelativeSource_8013d701695b46ad94dd5d59e00a2cb0 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_8013d701695b46ad94dd5d59e00a2cb0.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_fcba859302854aab9b9c439d2e871267.RelativeSource = RelativeSource_8013d701695b46ad94dd5d59e00a2cb0;


            Binding_fcba859302854aab9b9c439d2e871267.TemplateOwner = templateInstance_e63a3324cd3840b8bb6a590db0a94283;

            var Binding_9df19b0897774b359d6300faa9e11666 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_9df19b0897774b359d6300faa9e11666.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_e7ff9ba4dbf841fa948dc73a29d77a53 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_e7ff9ba4dbf841fa948dc73a29d77a53.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_9df19b0897774b359d6300faa9e11666.RelativeSource = RelativeSource_e7ff9ba4dbf841fa948dc73a29d77a53;


            Binding_9df19b0897774b359d6300faa9e11666.TemplateOwner = templateInstance_e63a3324cd3840b8bb6a590db0a94283;

            var Binding_60e905380dcb41449831e462e6f80c83 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_60e905380dcb41449831e462e6f80c83.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_8d6049fc3c4147bdb7282c50459d6651 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_8d6049fc3c4147bdb7282c50459d6651.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_60e905380dcb41449831e462e6f80c83.RelativeSource = RelativeSource_8d6049fc3c4147bdb7282c50459d6651;


            Binding_60e905380dcb41449831e462e6f80c83.TemplateOwner = templateInstance_e63a3324cd3840b8bb6a590db0a94283;


            var Popup_178cd792b23d46ccbcf76958fd333e1d = new global::Windows.UI.Xaml.Controls.Primitives.Popup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Popup", Popup_178cd792b23d46ccbcf76958fd333e1d);
            Popup_178cd792b23d46ccbcf76958fd333e1d.Name = "Popup";
            Popup_178cd792b23d46ccbcf76958fd333e1d.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Bottom;

            Grid_83fdab7a48894fd18f82be1c873d4679.Children.Add(TextBox_e8ad548eda0047e9a6f219e5890ad050);
            Grid_83fdab7a48894fd18f82be1c873d4679.Children.Add(Button_98e020b5b2e247c4a65dde2bcf95be2e);
            Grid_83fdab7a48894fd18f82be1c873d4679.Children.Add(Popup_178cd792b23d46ccbcf76958fd333e1d);



            TextBox_e8ad548eda0047e9a6f219e5890ad050.SetBinding(global::Windows.UI.Xaml.Controls.Control.BackgroundProperty, Binding_9bfb5d7cf4aa44ac854b34116f9b6d66);
            TextBox_e8ad548eda0047e9a6f219e5890ad050.SetBinding(global::Windows.UI.Xaml.Controls.Control.BorderBrushProperty, Binding_c7b34b15a8fd4ff4acf94b73893e4529);
            TextBox_e8ad548eda0047e9a6f219e5890ad050.SetBinding(global::Windows.UI.Xaml.Controls.Control.BorderThicknessProperty, Binding_b7b85e330b574a7ead3934864e6aef11);
            TextBox_e8ad548eda0047e9a6f219e5890ad050.SetBinding(global::Windows.UI.Xaml.Controls.Control.PaddingProperty, Binding_2ba4d05177cf40298950abe15a9ae41e);
            Button_98e020b5b2e247c4a65dde2bcf95be2e.SetBinding(global::Windows.UI.Xaml.Controls.Control.ForegroundProperty, Binding_fcba859302854aab9b9c439d2e871267);
            Button_98e020b5b2e247c4a65dde2bcf95be2e.SetBinding(global::Windows.UI.Xaml.Controls.Control.BorderBrushProperty, Binding_9df19b0897774b359d6300faa9e11666);
            Button_98e020b5b2e247c4a65dde2bcf95be2e.SetBinding(global::Windows.UI.Xaml.Controls.Control.BorderThicknessProperty, Binding_60e905380dcb41449831e462e6f80c83);

            templateInstance_e63a3324cd3840b8bb6a590db0a94283.TemplateContent = Grid_83fdab7a48894fd18f82be1c873d4679;
            return templateInstance_e63a3324cd3840b8bb6a590db0a94283;
        }



    }
}
#endif