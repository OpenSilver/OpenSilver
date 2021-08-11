
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
    internal class INTERNAL_DefaultDatePickerStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {





var Style_b5aebdee816740e59e57f5f4bffa3962 = new global::System.Windows.Style();
Style_b5aebdee816740e59e57f5f4bffa3962.TargetType = typeof(global::System.Windows.Controls.DatePicker);
var Setter_e478bf012c5c46dc8b3ab4245ab5ea44 = new global::System.Windows.Setter();
Setter_e478bf012c5c46dc8b3ab4245ab5ea44.Property = global::System.Windows.Controls.DatePicker.BorderBrushProperty;
Setter_e478bf012c5c46dc8b3ab4245ab5ea44.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Gray");

var Setter_1e571f504c744c9ea7b3d8ddd7fb3bc9 = new global::System.Windows.Setter();
Setter_1e571f504c744c9ea7b3d8ddd7fb3bc9.Property = global::System.Windows.Controls.DatePicker.BorderThicknessProperty;
Setter_1e571f504c744c9ea7b3d8ddd7fb3bc9.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");

var Setter_6e2ce2f2fdda41f881af4c062b4b6934 = new global::System.Windows.Setter();
Setter_6e2ce2f2fdda41f881af4c062b4b6934.Property = global::System.Windows.Controls.DatePicker.TemplateProperty;
var ControlTemplate_774828d401f24f19ae1284661393c433 = new global::System.Windows.Controls.ControlTemplate();
ControlTemplate_774828d401f24f19ae1284661393c433.TargetType = typeof(global::System.Windows.Controls.DatePicker);
ControlTemplate_774828d401f24f19ae1284661393c433.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_774828d401f24f19ae1284661393c433);

Setter_6e2ce2f2fdda41f881af4c062b4b6934.Value = ControlTemplate_774828d401f24f19ae1284661393c433;


Style_b5aebdee816740e59e57f5f4bffa3962.Setters.Add(Setter_e478bf012c5c46dc8b3ab4245ab5ea44);
Style_b5aebdee816740e59e57f5f4bffa3962.Setters.Add(Setter_1e571f504c744c9ea7b3d8ddd7fb3bc9);
Style_b5aebdee816740e59e57f5f4bffa3962.Setters.Add(Setter_6e2ce2f2fdda41f881af4c062b4b6934);


               DefaultStyle = Style_b5aebdee816740e59e57f5f4bffa3962;
            }
            return DefaultStyle;






    
        }



        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_774828d401f24f19ae1284661393c433(global::System.Windows.FrameworkElement templateOwner)
        {
var templateInstance_342dda018fca426091c96e0702ac85fb = new global::System.Windows.TemplateInstance();
templateInstance_342dda018fca426091c96e0702ac85fb.TemplateOwner = templateOwner;
var Grid_d2c5f67656be49198b6e3efcfc9cc0d2 = new global::System.Windows.Controls.Grid();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Root", Grid_d2c5f67656be49198b6e3efcfc9cc0d2);
Grid_d2c5f67656be49198b6e3efcfc9cc0d2.Name = "Root";
var ColumnDefinition_ddc4243eeb7f4fd18e920ded79e45556 = new global::System.Windows.Controls.ColumnDefinition();
ColumnDefinition_ddc4243eeb7f4fd18e920ded79e45556.Width = (global::System.Windows.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.GridLength), @"*");

var ColumnDefinition_4e2f238f21944fc592e92958b03da088 = new global::System.Windows.Controls.ColumnDefinition();
ColumnDefinition_4e2f238f21944fc592e92958b03da088.Width = (global::System.Windows.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.GridLength), @"Auto");

Grid_d2c5f67656be49198b6e3efcfc9cc0d2.ColumnDefinitions.Add(ColumnDefinition_ddc4243eeb7f4fd18e920ded79e45556);
Grid_d2c5f67656be49198b6e3efcfc9cc0d2.ColumnDefinitions.Add(ColumnDefinition_4e2f238f21944fc592e92958b03da088);

var TextBox_0a7e5081c37049afb9e46a1ac2fe362e = new global::System.Windows.Controls.TextBox();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("TextBox", TextBox_0a7e5081c37049afb9e46a1ac2fe362e);
TextBox_0a7e5081c37049afb9e46a1ac2fe362e.Name = "TextBox";
global::System.Windows.Controls.Grid.SetColumn(TextBox_0a7e5081c37049afb9e46a1ac2fe362e,(global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"0"));
var Binding_eb3d83ca7da44eb18c9db7e502c3b63a = new global::System.Windows.Data.Binding();
Binding_eb3d83ca7da44eb18c9db7e502c3b63a.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
var RelativeSource_323ad6d163484146883d96cb9cc2eb9e = new global::System.Windows.Data.RelativeSource();
RelativeSource_323ad6d163484146883d96cb9cc2eb9e.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_eb3d83ca7da44eb18c9db7e502c3b63a.RelativeSource = RelativeSource_323ad6d163484146883d96cb9cc2eb9e;


Binding_eb3d83ca7da44eb18c9db7e502c3b63a.TemplateOwner = templateInstance_342dda018fca426091c96e0702ac85fb;

var Binding_4c9a4ab572414c8ea08d360bbb994c8d = new global::System.Windows.Data.Binding();
Binding_4c9a4ab572414c8ea08d360bbb994c8d.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
var RelativeSource_08803430b3334fb6860652dcecfd229b = new global::System.Windows.Data.RelativeSource();
RelativeSource_08803430b3334fb6860652dcecfd229b.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_4c9a4ab572414c8ea08d360bbb994c8d.RelativeSource = RelativeSource_08803430b3334fb6860652dcecfd229b;


Binding_4c9a4ab572414c8ea08d360bbb994c8d.TemplateOwner = templateInstance_342dda018fca426091c96e0702ac85fb;

var Binding_19f0d877cb1f46b9ba5b51e587a8a91f = new global::System.Windows.Data.Binding();
Binding_19f0d877cb1f46b9ba5b51e587a8a91f.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
var RelativeSource_4da5f4bc2a4240c5b3d0b9c0218eba0a = new global::System.Windows.Data.RelativeSource();
RelativeSource_4da5f4bc2a4240c5b3d0b9c0218eba0a.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_19f0d877cb1f46b9ba5b51e587a8a91f.RelativeSource = RelativeSource_4da5f4bc2a4240c5b3d0b9c0218eba0a;


Binding_19f0d877cb1f46b9ba5b51e587a8a91f.TemplateOwner = templateInstance_342dda018fca426091c96e0702ac85fb;

var Binding_9a36a78445064710ab65f1d658bf125b = new global::System.Windows.Data.Binding();
Binding_9a36a78445064710ab65f1d658bf125b.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Padding");
var RelativeSource_0cdb916f603946958c43864dd854b034 = new global::System.Windows.Data.RelativeSource();
RelativeSource_0cdb916f603946958c43864dd854b034.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_9a36a78445064710ab65f1d658bf125b.RelativeSource = RelativeSource_0cdb916f603946958c43864dd854b034;


Binding_9a36a78445064710ab65f1d658bf125b.TemplateOwner = templateInstance_342dda018fca426091c96e0702ac85fb;


var Button_8764eb0dba1f477a89ef2fce3ec219de = new global::System.Windows.Controls.Button();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Button", Button_8764eb0dba1f477a89ef2fce3ec219de);
Button_8764eb0dba1f477a89ef2fce3ec219de.Name = "Button";
global::System.Windows.Controls.Grid.SetColumn(Button_8764eb0dba1f477a89ef2fce3ec219de,(global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"1"));
Button_8764eb0dba1f477a89ef2fce3ec219de.Content = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"▼");
Button_8764eb0dba1f477a89ef2fce3ec219de.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"30");
Button_8764eb0dba1f477a89ef2fce3ec219de.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"2,0,2,0");
var Binding_9bdb992db4ca466bbc8e2934d73a9d36 = new global::System.Windows.Data.Binding();
Binding_9bdb992db4ca466bbc8e2934d73a9d36.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Foreground");
var RelativeSource_4601b43081ef4624818772ad158addf4 = new global::System.Windows.Data.RelativeSource();
RelativeSource_4601b43081ef4624818772ad158addf4.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_9bdb992db4ca466bbc8e2934d73a9d36.RelativeSource = RelativeSource_4601b43081ef4624818772ad158addf4;


Binding_9bdb992db4ca466bbc8e2934d73a9d36.TemplateOwner = templateInstance_342dda018fca426091c96e0702ac85fb;

var Binding_53f6accb6bee4d689a7ffd667aaa847a = new global::System.Windows.Data.Binding();
Binding_53f6accb6bee4d689a7ffd667aaa847a.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
var RelativeSource_475062fdac094166a83ecc21e5476083 = new global::System.Windows.Data.RelativeSource();
RelativeSource_475062fdac094166a83ecc21e5476083.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_53f6accb6bee4d689a7ffd667aaa847a.RelativeSource = RelativeSource_475062fdac094166a83ecc21e5476083;


Binding_53f6accb6bee4d689a7ffd667aaa847a.TemplateOwner = templateInstance_342dda018fca426091c96e0702ac85fb;

var Binding_69533b84f62846bf917e1fa02dede28a = new global::System.Windows.Data.Binding();
Binding_69533b84f62846bf917e1fa02dede28a.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
var RelativeSource_7a862635a2824423b9375dbcca95d183 = new global::System.Windows.Data.RelativeSource();
RelativeSource_7a862635a2824423b9375dbcca95d183.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_69533b84f62846bf917e1fa02dede28a.RelativeSource = RelativeSource_7a862635a2824423b9375dbcca95d183;


Binding_69533b84f62846bf917e1fa02dede28a.TemplateOwner = templateInstance_342dda018fca426091c96e0702ac85fb;


var Popup_1a590b878d2d4275850bbfd54ef47c0d = new global::System.Windows.Controls.Primitives.Popup();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Popup", Popup_1a590b878d2d4275850bbfd54ef47c0d);
Popup_1a590b878d2d4275850bbfd54ef47c0d.Name = "Popup";
Popup_1a590b878d2d4275850bbfd54ef47c0d.VerticalAlignment = global::System.Windows.VerticalAlignment.Bottom;

Grid_d2c5f67656be49198b6e3efcfc9cc0d2.Children.Add(TextBox_0a7e5081c37049afb9e46a1ac2fe362e);
Grid_d2c5f67656be49198b6e3efcfc9cc0d2.Children.Add(Button_8764eb0dba1f477a89ef2fce3ec219de);
Grid_d2c5f67656be49198b6e3efcfc9cc0d2.Children.Add(Popup_1a590b878d2d4275850bbfd54ef47c0d);



TextBox_0a7e5081c37049afb9e46a1ac2fe362e.SetBinding(global::System.Windows.Controls.Control.BackgroundProperty, Binding_eb3d83ca7da44eb18c9db7e502c3b63a);
TextBox_0a7e5081c37049afb9e46a1ac2fe362e.SetBinding(global::System.Windows.Controls.Control.BorderBrushProperty, Binding_4c9a4ab572414c8ea08d360bbb994c8d);
TextBox_0a7e5081c37049afb9e46a1ac2fe362e.SetBinding(global::System.Windows.Controls.Control.BorderThicknessProperty, Binding_19f0d877cb1f46b9ba5b51e587a8a91f);
TextBox_0a7e5081c37049afb9e46a1ac2fe362e.SetBinding(global::System.Windows.Controls.Control.PaddingProperty, Binding_9a36a78445064710ab65f1d658bf125b);
Button_8764eb0dba1f477a89ef2fce3ec219de.SetBinding(global::System.Windows.Controls.Control.ForegroundProperty, Binding_9bdb992db4ca466bbc8e2934d73a9d36);
Button_8764eb0dba1f477a89ef2fce3ec219de.SetBinding(global::System.Windows.Controls.Control.BorderBrushProperty, Binding_53f6accb6bee4d689a7ffd667aaa847a);
Button_8764eb0dba1f477a89ef2fce3ec219de.SetBinding(global::System.Windows.Controls.Control.BorderThicknessProperty, Binding_69533b84f62846bf917e1fa02dede28a);

templateInstance_342dda018fca426091c96e0702ac85fb.TemplateContent = Grid_d2c5f67656be49198b6e3efcfc9cc0d2;
return templateInstance_342dda018fca426091c96e0702ac85fb;
        }



        }
}
#else

namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultDatePickerStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {





                var Style_b5aebdee816740e59e57f5f4bffa3962 = new global::Windows.UI.Xaml.Style();
                Style_b5aebdee816740e59e57f5f4bffa3962.TargetType = typeof(global::Windows.UI.Xaml.Controls.DatePicker);
                var Setter_e478bf012c5c46dc8b3ab4245ab5ea44 = new global::Windows.UI.Xaml.Setter();
                Setter_e478bf012c5c46dc8b3ab4245ab5ea44.Property = global::Windows.UI.Xaml.Controls.DatePicker.BorderBrushProperty;
                Setter_e478bf012c5c46dc8b3ab4245ab5ea44.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Gray");

                var Setter_1e571f504c744c9ea7b3d8ddd7fb3bc9 = new global::Windows.UI.Xaml.Setter();
                Setter_1e571f504c744c9ea7b3d8ddd7fb3bc9.Property = global::Windows.UI.Xaml.Controls.DatePicker.BorderThicknessProperty;
                Setter_1e571f504c744c9ea7b3d8ddd7fb3bc9.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");

                var Setter_6e2ce2f2fdda41f881af4c062b4b6934 = new global::Windows.UI.Xaml.Setter();
                Setter_6e2ce2f2fdda41f881af4c062b4b6934.Property = global::Windows.UI.Xaml.Controls.DatePicker.TemplateProperty;
                var ControlTemplate_774828d401f24f19ae1284661393c433 = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_774828d401f24f19ae1284661393c433.TargetType = typeof(global::Windows.UI.Xaml.Controls.DatePicker);
                ControlTemplate_774828d401f24f19ae1284661393c433.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_774828d401f24f19ae1284661393c433);

                Setter_6e2ce2f2fdda41f881af4c062b4b6934.Value = ControlTemplate_774828d401f24f19ae1284661393c433;


                Style_b5aebdee816740e59e57f5f4bffa3962.Setters.Add(Setter_e478bf012c5c46dc8b3ab4245ab5ea44);
                Style_b5aebdee816740e59e57f5f4bffa3962.Setters.Add(Setter_1e571f504c744c9ea7b3d8ddd7fb3bc9);
                Style_b5aebdee816740e59e57f5f4bffa3962.Setters.Add(Setter_6e2ce2f2fdda41f881af4c062b4b6934);


                DefaultStyle = Style_b5aebdee816740e59e57f5f4bffa3962;
            }
            return DefaultStyle;







        }



        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_774828d401f24f19ae1284661393c433(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_342dda018fca426091c96e0702ac85fb = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_342dda018fca426091c96e0702ac85fb.TemplateOwner = templateOwner;
            var Grid_d2c5f67656be49198b6e3efcfc9cc0d2 = new global::Windows.UI.Xaml.Controls.Grid();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Root", Grid_d2c5f67656be49198b6e3efcfc9cc0d2);
            Grid_d2c5f67656be49198b6e3efcfc9cc0d2.Name = "Root";
            var ColumnDefinition_ddc4243eeb7f4fd18e920ded79e45556 = new global::Windows.UI.Xaml.Controls.ColumnDefinition();
            ColumnDefinition_ddc4243eeb7f4fd18e920ded79e45556.Width = (global::Windows.UI.Xaml.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.GridLength), @"*");

            var ColumnDefinition_4e2f238f21944fc592e92958b03da088 = new global::Windows.UI.Xaml.Controls.ColumnDefinition();
            ColumnDefinition_4e2f238f21944fc592e92958b03da088.Width = (global::Windows.UI.Xaml.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.GridLength), @"Auto");

            Grid_d2c5f67656be49198b6e3efcfc9cc0d2.ColumnDefinitions.Add(ColumnDefinition_ddc4243eeb7f4fd18e920ded79e45556);
            Grid_d2c5f67656be49198b6e3efcfc9cc0d2.ColumnDefinitions.Add(ColumnDefinition_4e2f238f21944fc592e92958b03da088);

            var TextBox_0a7e5081c37049afb9e46a1ac2fe362e = new global::Windows.UI.Xaml.Controls.TextBox();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("TextBox", TextBox_0a7e5081c37049afb9e46a1ac2fe362e);
            TextBox_0a7e5081c37049afb9e46a1ac2fe362e.Name = "TextBox";
            global::Windows.UI.Xaml.Controls.Grid.SetColumn(TextBox_0a7e5081c37049afb9e46a1ac2fe362e, (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"0"));
            var Binding_eb3d83ca7da44eb18c9db7e502c3b63a = new global::Windows.UI.Xaml.Data.Binding();
            Binding_eb3d83ca7da44eb18c9db7e502c3b63a.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_323ad6d163484146883d96cb9cc2eb9e = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_323ad6d163484146883d96cb9cc2eb9e.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_eb3d83ca7da44eb18c9db7e502c3b63a.RelativeSource = RelativeSource_323ad6d163484146883d96cb9cc2eb9e;


            Binding_eb3d83ca7da44eb18c9db7e502c3b63a.TemplateOwner = templateInstance_342dda018fca426091c96e0702ac85fb;

            var Binding_4c9a4ab572414c8ea08d360bbb994c8d = new global::Windows.UI.Xaml.Data.Binding();
            Binding_4c9a4ab572414c8ea08d360bbb994c8d.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_08803430b3334fb6860652dcecfd229b = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_08803430b3334fb6860652dcecfd229b.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_4c9a4ab572414c8ea08d360bbb994c8d.RelativeSource = RelativeSource_08803430b3334fb6860652dcecfd229b;


            Binding_4c9a4ab572414c8ea08d360bbb994c8d.TemplateOwner = templateInstance_342dda018fca426091c96e0702ac85fb;

            var Binding_19f0d877cb1f46b9ba5b51e587a8a91f = new global::Windows.UI.Xaml.Data.Binding();
            Binding_19f0d877cb1f46b9ba5b51e587a8a91f.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_4da5f4bc2a4240c5b3d0b9c0218eba0a = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_4da5f4bc2a4240c5b3d0b9c0218eba0a.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_19f0d877cb1f46b9ba5b51e587a8a91f.RelativeSource = RelativeSource_4da5f4bc2a4240c5b3d0b9c0218eba0a;


            Binding_19f0d877cb1f46b9ba5b51e587a8a91f.TemplateOwner = templateInstance_342dda018fca426091c96e0702ac85fb;

            var Binding_9a36a78445064710ab65f1d658bf125b = new global::Windows.UI.Xaml.Data.Binding();
            Binding_9a36a78445064710ab65f1d658bf125b.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Padding");
            var RelativeSource_0cdb916f603946958c43864dd854b034 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_0cdb916f603946958c43864dd854b034.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_9a36a78445064710ab65f1d658bf125b.RelativeSource = RelativeSource_0cdb916f603946958c43864dd854b034;


            Binding_9a36a78445064710ab65f1d658bf125b.TemplateOwner = templateInstance_342dda018fca426091c96e0702ac85fb;


            var Button_8764eb0dba1f477a89ef2fce3ec219de = new global::Windows.UI.Xaml.Controls.Button();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Button", Button_8764eb0dba1f477a89ef2fce3ec219de);
            Button_8764eb0dba1f477a89ef2fce3ec219de.Name = "Button";
            global::Windows.UI.Xaml.Controls.Grid.SetColumn(Button_8764eb0dba1f477a89ef2fce3ec219de, (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"1"));
            Button_8764eb0dba1f477a89ef2fce3ec219de.Content = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"▼");
            Button_8764eb0dba1f477a89ef2fce3ec219de.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"30");
            Button_8764eb0dba1f477a89ef2fce3ec219de.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"2,0,2,0");
            var Binding_9bdb992db4ca466bbc8e2934d73a9d36 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_9bdb992db4ca466bbc8e2934d73a9d36.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Foreground");
            var RelativeSource_4601b43081ef4624818772ad158addf4 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_4601b43081ef4624818772ad158addf4.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_9bdb992db4ca466bbc8e2934d73a9d36.RelativeSource = RelativeSource_4601b43081ef4624818772ad158addf4;


            Binding_9bdb992db4ca466bbc8e2934d73a9d36.TemplateOwner = templateInstance_342dda018fca426091c96e0702ac85fb;

            var Binding_53f6accb6bee4d689a7ffd667aaa847a = new global::Windows.UI.Xaml.Data.Binding();
            Binding_53f6accb6bee4d689a7ffd667aaa847a.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_475062fdac094166a83ecc21e5476083 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_475062fdac094166a83ecc21e5476083.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_53f6accb6bee4d689a7ffd667aaa847a.RelativeSource = RelativeSource_475062fdac094166a83ecc21e5476083;


            Binding_53f6accb6bee4d689a7ffd667aaa847a.TemplateOwner = templateInstance_342dda018fca426091c96e0702ac85fb;

            var Binding_69533b84f62846bf917e1fa02dede28a = new global::Windows.UI.Xaml.Data.Binding();
            Binding_69533b84f62846bf917e1fa02dede28a.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_7a862635a2824423b9375dbcca95d183 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_7a862635a2824423b9375dbcca95d183.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_69533b84f62846bf917e1fa02dede28a.RelativeSource = RelativeSource_7a862635a2824423b9375dbcca95d183;


            Binding_69533b84f62846bf917e1fa02dede28a.TemplateOwner = templateInstance_342dda018fca426091c96e0702ac85fb;


            var Popup_1a590b878d2d4275850bbfd54ef47c0d = new global::Windows.UI.Xaml.Controls.Primitives.Popup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Popup", Popup_1a590b878d2d4275850bbfd54ef47c0d);
            Popup_1a590b878d2d4275850bbfd54ef47c0d.Name = "Popup";
            Popup_1a590b878d2d4275850bbfd54ef47c0d.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Bottom;

            Grid_d2c5f67656be49198b6e3efcfc9cc0d2.Children.Add(TextBox_0a7e5081c37049afb9e46a1ac2fe362e);
            Grid_d2c5f67656be49198b6e3efcfc9cc0d2.Children.Add(Button_8764eb0dba1f477a89ef2fce3ec219de);
            Grid_d2c5f67656be49198b6e3efcfc9cc0d2.Children.Add(Popup_1a590b878d2d4275850bbfd54ef47c0d);



            TextBox_0a7e5081c37049afb9e46a1ac2fe362e.SetBinding(global::Windows.UI.Xaml.Controls.Control.BackgroundProperty, Binding_eb3d83ca7da44eb18c9db7e502c3b63a);
            TextBox_0a7e5081c37049afb9e46a1ac2fe362e.SetBinding(global::Windows.UI.Xaml.Controls.Control.BorderBrushProperty, Binding_4c9a4ab572414c8ea08d360bbb994c8d);
            TextBox_0a7e5081c37049afb9e46a1ac2fe362e.SetBinding(global::Windows.UI.Xaml.Controls.Control.BorderThicknessProperty, Binding_19f0d877cb1f46b9ba5b51e587a8a91f);
            TextBox_0a7e5081c37049afb9e46a1ac2fe362e.SetBinding(global::Windows.UI.Xaml.Controls.Control.PaddingProperty, Binding_9a36a78445064710ab65f1d658bf125b);
            Button_8764eb0dba1f477a89ef2fce3ec219de.SetBinding(global::Windows.UI.Xaml.Controls.Control.ForegroundProperty, Binding_9bdb992db4ca466bbc8e2934d73a9d36);
            Button_8764eb0dba1f477a89ef2fce3ec219de.SetBinding(global::Windows.UI.Xaml.Controls.Control.BorderBrushProperty, Binding_53f6accb6bee4d689a7ffd667aaa847a);
            Button_8764eb0dba1f477a89ef2fce3ec219de.SetBinding(global::Windows.UI.Xaml.Controls.Control.BorderThicknessProperty, Binding_69533b84f62846bf917e1fa02dede28a);

            templateInstance_342dda018fca426091c96e0702ac85fb.TemplateContent = Grid_d2c5f67656be49198b6e3efcfc9cc0d2;
            return templateInstance_342dda018fca426091c96e0702ac85fb;
        }



    }
}
#endif