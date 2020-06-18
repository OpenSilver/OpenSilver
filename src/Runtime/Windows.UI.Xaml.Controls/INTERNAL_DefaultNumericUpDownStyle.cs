
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
    internal class INTERNAL_DefaultNumericUpDownStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_f361332eae77440ebfdd14313474583d = new global::System.Windows.Style();
                Style_f361332eae77440ebfdd14313474583d.TargetType = typeof(global::System.Windows.Controls.NumericUpDown);
                var Setter_5751504983754c61b38e63b01545cdaf = new global::System.Windows.Setter();
                Setter_5751504983754c61b38e63b01545cdaf.Property = global::System.Windows.Controls.NumericUpDown.TemplateProperty;
                var ControlTemplate_955230d423104401a73819b07b3c50ba = new global::System.Windows.Controls.ControlTemplate();
                ControlTemplate_955230d423104401a73819b07b3c50ba.TargetType = typeof(global::System.Windows.Controls.NumericUpDown);
                ControlTemplate_955230d423104401a73819b07b3c50ba.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_955230d423104401a73819b07b3c50ba);

                Setter_5751504983754c61b38e63b01545cdaf.Value = ControlTemplate_955230d423104401a73819b07b3c50ba;


                Style_f361332eae77440ebfdd14313474583d.Setters.Add(Setter_5751504983754c61b38e63b01545cdaf);


                DefaultStyle = Style_f361332eae77440ebfdd14313474583d;
            }

            return DefaultStyle;
        }

        static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_955230d423104401a73819b07b3c50ba(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_68d80f27031e4ed6aaed255212d7f79e = new global::System.Windows.TemplateInstance();
            templateInstance_68d80f27031e4ed6aaed255212d7f79e.TemplateOwner = templateOwner;
            var Border_cf3ab70b1fc34acc8131c5bc5ce5cef8 = new global::System.Windows.Controls.Border();
            Border_cf3ab70b1fc34acc8131c5bc5ce5cef8.VerticalAlignment = global::System.Windows.VerticalAlignment.Top;
            var Grid_7d29615f184042acac6b3036319bc19c = new global::System.Windows.Controls.Grid();
            var ColumnDefinition_d8fd457f39254c1ba422ede05740b2b3 = new global::System.Windows.Controls.ColumnDefinition();

            var ColumnDefinition_848f59343d934ff999f2cb691df6f9c1 = new global::System.Windows.Controls.ColumnDefinition();
            ColumnDefinition_848f59343d934ff999f2cb691df6f9c1.Width = (global::System.Windows.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.GridLength), @"Auto");

            var ColumnDefinition_87a32d2ee21b45d2a4566748e7a995ab = new global::System.Windows.Controls.ColumnDefinition();
            ColumnDefinition_87a32d2ee21b45d2a4566748e7a995ab.Width = (global::System.Windows.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.GridLength), @"Auto");

            Grid_7d29615f184042acac6b3036319bc19c.ColumnDefinitions.Add(ColumnDefinition_d8fd457f39254c1ba422ede05740b2b3);
            Grid_7d29615f184042acac6b3036319bc19c.ColumnDefinitions.Add(ColumnDefinition_848f59343d934ff999f2cb691df6f9c1);
            Grid_7d29615f184042acac6b3036319bc19c.ColumnDefinitions.Add(ColumnDefinition_87a32d2ee21b45d2a4566748e7a995ab);

            var Grid_eec27f24f79c402082f984059523461b = new global::System.Windows.Controls.Grid();
            global::System.Windows.Controls.Grid.SetColumn(Grid_eec27f24f79c402082f984059523461b, (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"0"));
            var Rectangle_05c2bbd0edb34cf298bfa53827030826 = new global::System.Windows.Shapes.Rectangle();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("PART_ValueBar", Rectangle_05c2bbd0edb34cf298bfa53827030826);
            Rectangle_05c2bbd0edb34cf298bfa53827030826.Name = "PART_ValueBar";
            Rectangle_05c2bbd0edb34cf298bfa53827030826.IsHitTestVisible = (global::System.Boolean)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Boolean), @"False");
            Rectangle_05c2bbd0edb34cf298bfa53827030826.Fill = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFFFFFFF");

            var UpDownTextBox_cc8ef7e728054a9583597c184b24380f = new global::System.Windows.Controls.Primitives.UpDownTextBox();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("PART_ValueTextBox", UpDownTextBox_cc8ef7e728054a9583597c184b24380f);
            UpDownTextBox_cc8ef7e728054a9583597c184b24380f.Name = "PART_ValueTextBox";
            UpDownTextBox_cc8ef7e728054a9583597c184b24380f.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Stretch;
            var Binding_73b75de7ad0341d38883fc34e8e8403c = new global::System.Windows.Data.Binding();
            Binding_73b75de7ad0341d38883fc34e8e8403c.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"FontFamily");
            var RelativeSource_9725343727aa41d8b96833126c84ca00 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_9725343727aa41d8b96833126c84ca00.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_73b75de7ad0341d38883fc34e8e8403c.RelativeSource = RelativeSource_9725343727aa41d8b96833126c84ca00;


            Binding_73b75de7ad0341d38883fc34e8e8403c.TemplateOwner = templateInstance_68d80f27031e4ed6aaed255212d7f79e;

            var Binding_58edf7e603de45479408b7528f708b7e = new global::System.Windows.Data.Binding();
            Binding_58edf7e603de45479408b7528f708b7e.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"FontSize");
            var RelativeSource_c64087ae46864b469b548a01d65a6a81 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_c64087ae46864b469b548a01d65a6a81.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_58edf7e603de45479408b7528f708b7e.RelativeSource = RelativeSource_c64087ae46864b469b548a01d65a6a81;


            Binding_58edf7e603de45479408b7528f708b7e.TemplateOwner = templateInstance_68d80f27031e4ed6aaed255212d7f79e;

            var Binding_4c408bfb3b164c2cb946d3e08c4caa9d = new global::System.Windows.Data.Binding();
            Binding_4c408bfb3b164c2cb946d3e08c4caa9d.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Foreground");
            var RelativeSource_84cea802454748beb50a780c193354fe = new global::System.Windows.Data.RelativeSource();
            RelativeSource_84cea802454748beb50a780c193354fe.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_4c408bfb3b164c2cb946d3e08c4caa9d.RelativeSource = RelativeSource_84cea802454748beb50a780c193354fe;


            Binding_4c408bfb3b164c2cb946d3e08c4caa9d.TemplateOwner = templateInstance_68d80f27031e4ed6aaed255212d7f79e;


            Grid_eec27f24f79c402082f984059523461b.Children.Add(Rectangle_05c2bbd0edb34cf298bfa53827030826);
            Grid_eec27f24f79c402082f984059523461b.Children.Add(UpDownTextBox_cc8ef7e728054a9583597c184b24380f);


            var RepeatButton_0e2351b63a0f457390eecdb05ce03a55 = new global::System.Windows.Controls.Primitives.RepeatButton();
            global::System.Windows.Controls.Grid.SetColumn(RepeatButton_0e2351b63a0f457390eecdb05ce03a55, (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"1"));
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("PART_DecrementButton", RepeatButton_0e2351b63a0f457390eecdb05ce03a55);
            RepeatButton_0e2351b63a0f457390eecdb05ce03a55.Name = "PART_DecrementButton";
            RepeatButton_0e2351b63a0f457390eecdb05ce03a55.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"4,0,0,0");
            RepeatButton_0e2351b63a0f457390eecdb05ce03a55.Content = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"➖");

            var RepeatButton_18e85dba18194e44836515fd50abfc1b = new global::System.Windows.Controls.Primitives.RepeatButton();
            global::System.Windows.Controls.Grid.SetColumn(RepeatButton_18e85dba18194e44836515fd50abfc1b, (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"2"));
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("PART_IncrementButton", RepeatButton_18e85dba18194e44836515fd50abfc1b);
            RepeatButton_18e85dba18194e44836515fd50abfc1b.Name = "PART_IncrementButton";
            RepeatButton_18e85dba18194e44836515fd50abfc1b.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"3,0,0,0");
            RepeatButton_18e85dba18194e44836515fd50abfc1b.Content = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"➕");

            Grid_7d29615f184042acac6b3036319bc19c.Children.Add(Grid_eec27f24f79c402082f984059523461b);
            Grid_7d29615f184042acac6b3036319bc19c.Children.Add(RepeatButton_0e2351b63a0f457390eecdb05ce03a55);
            Grid_7d29615f184042acac6b3036319bc19c.Children.Add(RepeatButton_18e85dba18194e44836515fd50abfc1b);


            Border_cf3ab70b1fc34acc8131c5bc5ce5cef8.Child = Grid_7d29615f184042acac6b3036319bc19c;

            var Binding_ea65ee99d4654121b2aff2d9b511688a = new global::System.Windows.Data.Binding();
            Binding_ea65ee99d4654121b2aff2d9b511688a.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_d207b136a95a417fa4510f0ec5291512 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_d207b136a95a417fa4510f0ec5291512.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_ea65ee99d4654121b2aff2d9b511688a.RelativeSource = RelativeSource_d207b136a95a417fa4510f0ec5291512;


            Binding_ea65ee99d4654121b2aff2d9b511688a.TemplateOwner = templateInstance_68d80f27031e4ed6aaed255212d7f79e;

            var Binding_e32828ec685846a8883905a81353b781 = new global::System.Windows.Data.Binding();
            Binding_e32828ec685846a8883905a81353b781.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
            var RelativeSource_5c4f7c024ad140a1980d06eb06db160d = new global::System.Windows.Data.RelativeSource();
            RelativeSource_5c4f7c024ad140a1980d06eb06db160d.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_e32828ec685846a8883905a81353b781.RelativeSource = RelativeSource_5c4f7c024ad140a1980d06eb06db160d;


            Binding_e32828ec685846a8883905a81353b781.TemplateOwner = templateInstance_68d80f27031e4ed6aaed255212d7f79e;

            var Binding_51730f076d984cb18192cce69cbe801e = new global::System.Windows.Data.Binding();
            Binding_51730f076d984cb18192cce69cbe801e.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
            var RelativeSource_b0b9ebdb74b5491892e6c9529964f430 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_b0b9ebdb74b5491892e6c9529964f430.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_51730f076d984cb18192cce69cbe801e.RelativeSource = RelativeSource_b0b9ebdb74b5491892e6c9529964f430;


            Binding_51730f076d984cb18192cce69cbe801e.TemplateOwner = templateInstance_68d80f27031e4ed6aaed255212d7f79e;



            UpDownTextBox_cc8ef7e728054a9583597c184b24380f.SetBinding(global::System.Windows.Controls.Control.FontFamilyProperty, Binding_73b75de7ad0341d38883fc34e8e8403c);
            UpDownTextBox_cc8ef7e728054a9583597c184b24380f.SetBinding(global::System.Windows.Controls.Control.FontSizeProperty, Binding_58edf7e603de45479408b7528f708b7e);
            UpDownTextBox_cc8ef7e728054a9583597c184b24380f.SetBinding(global::System.Windows.Controls.Control.ForegroundProperty, Binding_4c408bfb3b164c2cb946d3e08c4caa9d);
            Border_cf3ab70b1fc34acc8131c5bc5ce5cef8.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_ea65ee99d4654121b2aff2d9b511688a);
            Border_cf3ab70b1fc34acc8131c5bc5ce5cef8.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_e32828ec685846a8883905a81353b781);
            Border_cf3ab70b1fc34acc8131c5bc5ce5cef8.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_51730f076d984cb18192cce69cbe801e);

            templateInstance_68d80f27031e4ed6aaed255212d7f79e.TemplateContent = Border_cf3ab70b1fc34acc8131c5bc5ce5cef8;
            return templateInstance_68d80f27031e4ed6aaed255212d7f79e;
        }
    }
}
#else
namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultNumericUpDownStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_f361332eae77440ebfdd14313474583d = new global::Windows.UI.Xaml.Style();
                Style_f361332eae77440ebfdd14313474583d.TargetType = typeof(global::Windows.UI.Xaml.Controls.NumericUpDown);
                var Setter_5751504983754c61b38e63b01545cdaf = new global::Windows.UI.Xaml.Setter();
                Setter_5751504983754c61b38e63b01545cdaf.Property = global::Windows.UI.Xaml.Controls.NumericUpDown.TemplateProperty;
                var ControlTemplate_955230d423104401a73819b07b3c50ba = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_955230d423104401a73819b07b3c50ba.TargetType = typeof(global::Windows.UI.Xaml.Controls.NumericUpDown);
                ControlTemplate_955230d423104401a73819b07b3c50ba.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_955230d423104401a73819b07b3c50ba);

                Setter_5751504983754c61b38e63b01545cdaf.Value = ControlTemplate_955230d423104401a73819b07b3c50ba;


                Style_f361332eae77440ebfdd14313474583d.Setters.Add(Setter_5751504983754c61b38e63b01545cdaf);


                DefaultStyle = Style_f361332eae77440ebfdd14313474583d;
            }

            return DefaultStyle;
        }

        static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_955230d423104401a73819b07b3c50ba(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_68d80f27031e4ed6aaed255212d7f79e = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_68d80f27031e4ed6aaed255212d7f79e.TemplateOwner = templateOwner;
            var Border_cf3ab70b1fc34acc8131c5bc5ce5cef8 = new global::Windows.UI.Xaml.Controls.Border();
            Border_cf3ab70b1fc34acc8131c5bc5ce5cef8.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Top;
            var Grid_7d29615f184042acac6b3036319bc19c = new global::Windows.UI.Xaml.Controls.Grid();
            var ColumnDefinition_d8fd457f39254c1ba422ede05740b2b3 = new global::Windows.UI.Xaml.Controls.ColumnDefinition();

            var ColumnDefinition_848f59343d934ff999f2cb691df6f9c1 = new global::Windows.UI.Xaml.Controls.ColumnDefinition();
            ColumnDefinition_848f59343d934ff999f2cb691df6f9c1.Width = (global::Windows.UI.Xaml.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.GridLength), @"Auto");

            var ColumnDefinition_87a32d2ee21b45d2a4566748e7a995ab = new global::Windows.UI.Xaml.Controls.ColumnDefinition();
            ColumnDefinition_87a32d2ee21b45d2a4566748e7a995ab.Width = (global::Windows.UI.Xaml.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.GridLength), @"Auto");

            Grid_7d29615f184042acac6b3036319bc19c.ColumnDefinitions.Add(ColumnDefinition_d8fd457f39254c1ba422ede05740b2b3);
            Grid_7d29615f184042acac6b3036319bc19c.ColumnDefinitions.Add(ColumnDefinition_848f59343d934ff999f2cb691df6f9c1);
            Grid_7d29615f184042acac6b3036319bc19c.ColumnDefinitions.Add(ColumnDefinition_87a32d2ee21b45d2a4566748e7a995ab);

            var Grid_eec27f24f79c402082f984059523461b = new global::Windows.UI.Xaml.Controls.Grid();
            global::Windows.UI.Xaml.Controls.Grid.SetColumn(Grid_eec27f24f79c402082f984059523461b, (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"0"));
            var Rectangle_05c2bbd0edb34cf298bfa53827030826 = new global::Windows.UI.Xaml.Shapes.Rectangle();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("PART_ValueBar", Rectangle_05c2bbd0edb34cf298bfa53827030826);
            Rectangle_05c2bbd0edb34cf298bfa53827030826.Name = "PART_ValueBar";
            Rectangle_05c2bbd0edb34cf298bfa53827030826.IsHitTestVisible = (global::System.Boolean)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Boolean), @"False");
            Rectangle_05c2bbd0edb34cf298bfa53827030826.Fill = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFFFFFFF");

            var UpDownTextBox_cc8ef7e728054a9583597c184b24380f = new global::Windows.UI.Xaml.Controls.Primitives.UpDownTextBox();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("PART_ValueTextBox", UpDownTextBox_cc8ef7e728054a9583597c184b24380f);
            UpDownTextBox_cc8ef7e728054a9583597c184b24380f.Name = "PART_ValueTextBox";
            UpDownTextBox_cc8ef7e728054a9583597c184b24380f.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch;
            var Binding_73b75de7ad0341d38883fc34e8e8403c = new global::Windows.UI.Xaml.Data.Binding();
            Binding_73b75de7ad0341d38883fc34e8e8403c.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"FontFamily");
            var RelativeSource_9725343727aa41d8b96833126c84ca00 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_9725343727aa41d8b96833126c84ca00.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_73b75de7ad0341d38883fc34e8e8403c.RelativeSource = RelativeSource_9725343727aa41d8b96833126c84ca00;


            Binding_73b75de7ad0341d38883fc34e8e8403c.TemplateOwner = templateInstance_68d80f27031e4ed6aaed255212d7f79e;

            var Binding_58edf7e603de45479408b7528f708b7e = new global::Windows.UI.Xaml.Data.Binding();
            Binding_58edf7e603de45479408b7528f708b7e.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"FontSize");
            var RelativeSource_c64087ae46864b469b548a01d65a6a81 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_c64087ae46864b469b548a01d65a6a81.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_58edf7e603de45479408b7528f708b7e.RelativeSource = RelativeSource_c64087ae46864b469b548a01d65a6a81;


            Binding_58edf7e603de45479408b7528f708b7e.TemplateOwner = templateInstance_68d80f27031e4ed6aaed255212d7f79e;

            var Binding_4c408bfb3b164c2cb946d3e08c4caa9d = new global::Windows.UI.Xaml.Data.Binding();
            Binding_4c408bfb3b164c2cb946d3e08c4caa9d.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Foreground");
            var RelativeSource_84cea802454748beb50a780c193354fe = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_84cea802454748beb50a780c193354fe.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_4c408bfb3b164c2cb946d3e08c4caa9d.RelativeSource = RelativeSource_84cea802454748beb50a780c193354fe;


            Binding_4c408bfb3b164c2cb946d3e08c4caa9d.TemplateOwner = templateInstance_68d80f27031e4ed6aaed255212d7f79e;


            Grid_eec27f24f79c402082f984059523461b.Children.Add(Rectangle_05c2bbd0edb34cf298bfa53827030826);
            Grid_eec27f24f79c402082f984059523461b.Children.Add(UpDownTextBox_cc8ef7e728054a9583597c184b24380f);


            var RepeatButton_0e2351b63a0f457390eecdb05ce03a55 = new global::Windows.UI.Xaml.Controls.Primitives.RepeatButton();
            global::Windows.UI.Xaml.Controls.Grid.SetColumn(RepeatButton_0e2351b63a0f457390eecdb05ce03a55, (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"1"));
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("PART_DecrementButton", RepeatButton_0e2351b63a0f457390eecdb05ce03a55);
            RepeatButton_0e2351b63a0f457390eecdb05ce03a55.Name = "PART_DecrementButton";
            RepeatButton_0e2351b63a0f457390eecdb05ce03a55.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"4,0,0,0");
            RepeatButton_0e2351b63a0f457390eecdb05ce03a55.Content = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"➖");

            var RepeatButton_18e85dba18194e44836515fd50abfc1b = new global::Windows.UI.Xaml.Controls.Primitives.RepeatButton();
            global::Windows.UI.Xaml.Controls.Grid.SetColumn(RepeatButton_18e85dba18194e44836515fd50abfc1b, (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"2"));
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("PART_IncrementButton", RepeatButton_18e85dba18194e44836515fd50abfc1b);
            RepeatButton_18e85dba18194e44836515fd50abfc1b.Name = "PART_IncrementButton";
            RepeatButton_18e85dba18194e44836515fd50abfc1b.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"3,0,0,0");
            RepeatButton_18e85dba18194e44836515fd50abfc1b.Content = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"➕");

            Grid_7d29615f184042acac6b3036319bc19c.Children.Add(Grid_eec27f24f79c402082f984059523461b);
            Grid_7d29615f184042acac6b3036319bc19c.Children.Add(RepeatButton_0e2351b63a0f457390eecdb05ce03a55);
            Grid_7d29615f184042acac6b3036319bc19c.Children.Add(RepeatButton_18e85dba18194e44836515fd50abfc1b);


            Border_cf3ab70b1fc34acc8131c5bc5ce5cef8.Child = Grid_7d29615f184042acac6b3036319bc19c;

            var Binding_ea65ee99d4654121b2aff2d9b511688a = new global::Windows.UI.Xaml.Data.Binding();
            Binding_ea65ee99d4654121b2aff2d9b511688a.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_d207b136a95a417fa4510f0ec5291512 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_d207b136a95a417fa4510f0ec5291512.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_ea65ee99d4654121b2aff2d9b511688a.RelativeSource = RelativeSource_d207b136a95a417fa4510f0ec5291512;


            Binding_ea65ee99d4654121b2aff2d9b511688a.TemplateOwner = templateInstance_68d80f27031e4ed6aaed255212d7f79e;

            var Binding_e32828ec685846a8883905a81353b781 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_e32828ec685846a8883905a81353b781.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_5c4f7c024ad140a1980d06eb06db160d = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_5c4f7c024ad140a1980d06eb06db160d.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_e32828ec685846a8883905a81353b781.RelativeSource = RelativeSource_5c4f7c024ad140a1980d06eb06db160d;


            Binding_e32828ec685846a8883905a81353b781.TemplateOwner = templateInstance_68d80f27031e4ed6aaed255212d7f79e;

            var Binding_51730f076d984cb18192cce69cbe801e = new global::Windows.UI.Xaml.Data.Binding();
            Binding_51730f076d984cb18192cce69cbe801e.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_b0b9ebdb74b5491892e6c9529964f430 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_b0b9ebdb74b5491892e6c9529964f430.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_51730f076d984cb18192cce69cbe801e.RelativeSource = RelativeSource_b0b9ebdb74b5491892e6c9529964f430;


            Binding_51730f076d984cb18192cce69cbe801e.TemplateOwner = templateInstance_68d80f27031e4ed6aaed255212d7f79e;



            UpDownTextBox_cc8ef7e728054a9583597c184b24380f.SetBinding(global::Windows.UI.Xaml.Controls.Control.FontFamilyProperty, Binding_73b75de7ad0341d38883fc34e8e8403c);
            UpDownTextBox_cc8ef7e728054a9583597c184b24380f.SetBinding(global::Windows.UI.Xaml.Controls.Control.FontSizeProperty, Binding_58edf7e603de45479408b7528f708b7e);
            UpDownTextBox_cc8ef7e728054a9583597c184b24380f.SetBinding(global::Windows.UI.Xaml.Controls.Control.ForegroundProperty, Binding_4c408bfb3b164c2cb946d3e08c4caa9d);
            Border_cf3ab70b1fc34acc8131c5bc5ce5cef8.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_ea65ee99d4654121b2aff2d9b511688a);
            Border_cf3ab70b1fc34acc8131c5bc5ce5cef8.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_e32828ec685846a8883905a81353b781);
            Border_cf3ab70b1fc34acc8131c5bc5ce5cef8.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_51730f076d984cb18192cce69cbe801e);

            templateInstance_68d80f27031e4ed6aaed255212d7f79e.TemplateContent = Border_cf3ab70b1fc34acc8131c5bc5ce5cef8;
            return templateInstance_68d80f27031e4ed6aaed255212d7f79e;
        }
    }
}
#endif