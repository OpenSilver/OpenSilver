
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
    internal class INTERNAL_DefaultComboBoxStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_d7e6874321be4521b479d97df2d01b78 = new global::System.Windows.Style();
                Style_d7e6874321be4521b479d97df2d01b78.TargetType = typeof(global::System.Windows.Controls.ComboBox);
                var Setter_f8a0ff0a7950487cbdfc353ac932627f = new global::System.Windows.Setter();
                Setter_f8a0ff0a7950487cbdfc353ac932627f.Property = global::System.Windows.Controls.ComboBox.BackgroundProperty;
                Setter_f8a0ff0a7950487cbdfc353ac932627f.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"White");

                var Setter_03b511d15a94443983c576309f6646ff = new global::System.Windows.Setter();
                Setter_03b511d15a94443983c576309f6646ff.Property = global::System.Windows.Controls.ComboBox.ForegroundProperty;
                Setter_03b511d15a94443983c576309f6646ff.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Black");

                var Setter_889237c2dc7f4a2aa643d752a4a18456 = new global::System.Windows.Setter();
                Setter_889237c2dc7f4a2aa643d752a4a18456.Property = global::System.Windows.Controls.ComboBox.PaddingProperty;
                Setter_889237c2dc7f4a2aa643d752a4a18456.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"6,2,30,2");

                var Setter_94f670595f1b435b90a236994c5c4096 = new global::System.Windows.Setter();
                Setter_94f670595f1b435b90a236994c5c4096.Property = global::System.Windows.Controls.ComboBox.CursorProperty;
                Setter_94f670595f1b435b90a236994c5c4096.Value = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Hand");

                var Setter_9f41e1651e8446248018c92a7481088d = new global::System.Windows.Setter();
                Setter_9f41e1651e8446248018c92a7481088d.Property = global::System.Windows.Controls.ComboBox.TemplateProperty;
                var ControlTemplate_d2528842177b4d42b5fa6765e0841e5e = new global::System.Windows.Controls.ControlTemplate();
                ControlTemplate_d2528842177b4d42b5fa6765e0841e5e.TargetType = typeof(global::System.Windows.Controls.ComboBox);
                ControlTemplate_d2528842177b4d42b5fa6765e0841e5e.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_d2528842177b4d42b5fa6765e0841e5e);

                Setter_9f41e1651e8446248018c92a7481088d.Value = ControlTemplate_d2528842177b4d42b5fa6765e0841e5e;


                Style_d7e6874321be4521b479d97df2d01b78.Setters.Add(Setter_f8a0ff0a7950487cbdfc353ac932627f);
                Style_d7e6874321be4521b479d97df2d01b78.Setters.Add(Setter_03b511d15a94443983c576309f6646ff);
                Style_d7e6874321be4521b479d97df2d01b78.Setters.Add(Setter_889237c2dc7f4a2aa643d752a4a18456);
                Style_d7e6874321be4521b479d97df2d01b78.Setters.Add(Setter_94f670595f1b435b90a236994c5c4096);
                Style_d7e6874321be4521b479d97df2d01b78.Setters.Add(Setter_9f41e1651e8446248018c92a7481088d);


                DefaultStyle = Style_d7e6874321be4521b479d97df2d01b78;
            }
            return DefaultStyle;
        }

        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_d2528842177b4d42b5fa6765e0841e5e(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_82c2e68d6ef24c5e90d456a94e511620 = new global::System.Windows.TemplateInstance();
            templateInstance_82c2e68d6ef24c5e90d456a94e511620.TemplateOwner = templateOwner;
            var StackPanel_88ea3dbd18db4daf93783037aab66e3a = new global::System.Windows.Controls.StackPanel();

            var Border_20d9adcdf5c74d56aacd768851f0395b = new global::System.Windows.Controls.Border();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("OuterBorder", Border_20d9adcdf5c74d56aacd768851f0395b);
            Border_20d9adcdf5c74d56aacd768851f0395b.Name = "OuterBorder";
            var Grid_86d48d9e220e4b56a3265cb1b7073abf = new global::System.Windows.Controls.Grid();
            var ToggleButton_05554ddcffb4400da5d7d822c4ad4729 = new global::System.Windows.Controls.Primitives.ToggleButton();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("DropDownToggle", ToggleButton_05554ddcffb4400da5d7d822c4ad4729);
            ToggleButton_05554ddcffb4400da5d7d822c4ad4729.Name = "DropDownToggle";
            ToggleButton_05554ddcffb4400da5d7d822c4ad4729.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Stretch;
            ToggleButton_05554ddcffb4400da5d7d822c4ad4729.VerticalAlignment = global::System.Windows.VerticalAlignment.Stretch;
            ToggleButton_05554ddcffb4400da5d7d822c4ad4729.HorizontalContentAlignment = global::System.Windows.HorizontalAlignment.Right;
            ToggleButton_05554ddcffb4400da5d7d822c4ad4729.VerticalContentAlignment = global::System.Windows.VerticalAlignment.Center;
            var Path_970e4c9c536441aaa2b7cec321ab8522 = new global::System.Windows.Shapes.Path();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("arrow", Path_970e4c9c536441aaa2b7cec321ab8522);
            Path_970e4c9c536441aaa2b7cec321ab8522.Name = "arrow";
            Path_970e4c9c536441aaa2b7cec321ab8522.Visibility = global::System.Windows.Visibility.Visible;
            Path_970e4c9c536441aaa2b7cec321ab8522.Stroke = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Black");
            Path_970e4c9c536441aaa2b7cec321ab8522.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"13");
            Path_970e4c9c536441aaa2b7cec321ab8522.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_970e4c9c536441aaa2b7cec321ab8522.StrokeThickness = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"3");
            Path_970e4c9c536441aaa2b7cec321ab8522.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Center;
            Path_970e4c9c536441aaa2b7cec321ab8522.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
            Path_970e4c9c536441aaa2b7cec321ab8522.Stretch = global::System.Windows.Media.Stretch.None;
            Path_970e4c9c536441aaa2b7cec321ab8522.Data = (global::System.Windows.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Geometry), @"M 1.5,2.25 L 6.75,7.5 L 12,2.25");

            ToggleButton_05554ddcffb4400da5d7d822c4ad4729.Content = Path_970e4c9c536441aaa2b7cec321ab8522;


            var ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c = new global::System.Windows.Controls.ContentPresenter();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("ContentPresenter", ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c);
            ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c.Name = "ContentPresenter";
            ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c.IsHitTestVisible = (global::System.Boolean)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Boolean), @"False");
            ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c.MinHeight = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"24");
            var Binding_33874615a15f4f4a8e7a1b323f027661 = new global::System.Windows.Data.Binding();
            Binding_33874615a15f4f4a8e7a1b323f027661.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Padding");
            var RelativeSource_07f3d5f00ac24ae69b89f38d0c235cde = new global::System.Windows.Data.RelativeSource();
            RelativeSource_07f3d5f00ac24ae69b89f38d0c235cde.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_33874615a15f4f4a8e7a1b323f027661.RelativeSource = RelativeSource_07f3d5f00ac24ae69b89f38d0c235cde;


            Binding_33874615a15f4f4a8e7a1b323f027661.TemplateOwner = templateInstance_82c2e68d6ef24c5e90d456a94e511620;

            var Binding_7636a81645ff4681b77f33c835487470 = new global::System.Windows.Data.Binding();
            Binding_7636a81645ff4681b77f33c835487470.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Content");
            var RelativeSource_e76ae58c16854addaddd66c7a1c7c2f1 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_e76ae58c16854addaddd66c7a1c7c2f1.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_7636a81645ff4681b77f33c835487470.RelativeSource = RelativeSource_e76ae58c16854addaddd66c7a1c7c2f1;


            Binding_7636a81645ff4681b77f33c835487470.TemplateOwner = templateInstance_82c2e68d6ef24c5e90d456a94e511620;

            var Binding_5ac07c7327b640cc8f4d86a5562f82b1 = new global::System.Windows.Data.Binding();
            Binding_5ac07c7327b640cc8f4d86a5562f82b1.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"ContentTemplate");
            var RelativeSource_0cf506501910429a8f6910618ca3a865 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_0cf506501910429a8f6910618ca3a865.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_5ac07c7327b640cc8f4d86a5562f82b1.RelativeSource = RelativeSource_0cf506501910429a8f6910618ca3a865;


            Binding_5ac07c7327b640cc8f4d86a5562f82b1.TemplateOwner = templateInstance_82c2e68d6ef24c5e90d456a94e511620;


            Grid_86d48d9e220e4b56a3265cb1b7073abf.Children.Add(ToggleButton_05554ddcffb4400da5d7d822c4ad4729);
            Grid_86d48d9e220e4b56a3265cb1b7073abf.Children.Add(ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c);


            Border_20d9adcdf5c74d56aacd768851f0395b.Child = Grid_86d48d9e220e4b56a3265cb1b7073abf;

            var Binding_6ba1ab7c5d3b4b12902abe26bb950e80 = new global::System.Windows.Data.Binding();
            Binding_6ba1ab7c5d3b4b12902abe26bb950e80.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_4201ad0529f54230bb21db229f3bef75 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_4201ad0529f54230bb21db229f3bef75.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_6ba1ab7c5d3b4b12902abe26bb950e80.RelativeSource = RelativeSource_4201ad0529f54230bb21db229f3bef75;


            Binding_6ba1ab7c5d3b4b12902abe26bb950e80.TemplateOwner = templateInstance_82c2e68d6ef24c5e90d456a94e511620;

            var Binding_2d77a6f469534fe8ae41b6bb9c1561b5 = new global::System.Windows.Data.Binding();
            Binding_2d77a6f469534fe8ae41b6bb9c1561b5.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
            var RelativeSource_6c5f4f40220a4b309bdd18fc28fc3f1a = new global::System.Windows.Data.RelativeSource();
            RelativeSource_6c5f4f40220a4b309bdd18fc28fc3f1a.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_2d77a6f469534fe8ae41b6bb9c1561b5.RelativeSource = RelativeSource_6c5f4f40220a4b309bdd18fc28fc3f1a;


            Binding_2d77a6f469534fe8ae41b6bb9c1561b5.TemplateOwner = templateInstance_82c2e68d6ef24c5e90d456a94e511620;

            var Binding_401409c10d7a44d08f75e12adf5a1fec = new global::System.Windows.Data.Binding();
            Binding_401409c10d7a44d08f75e12adf5a1fec.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
            var RelativeSource_9455df17ab39482e83977458c783f35f = new global::System.Windows.Data.RelativeSource();
            RelativeSource_9455df17ab39482e83977458c783f35f.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_401409c10d7a44d08f75e12adf5a1fec.RelativeSource = RelativeSource_9455df17ab39482e83977458c783f35f;


            Binding_401409c10d7a44d08f75e12adf5a1fec.TemplateOwner = templateInstance_82c2e68d6ef24c5e90d456a94e511620;


            var Popup_71bbbf41c17841138e8da63a6e3cdbe0 = new global::System.Windows.Controls.Primitives.Popup();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Popup", Popup_71bbbf41c17841138e8da63a6e3cdbe0);
            Popup_71bbbf41c17841138e8da63a6e3cdbe0.Name = "Popup";
            Popup_71bbbf41c17841138e8da63a6e3cdbe0.IsOpen = (global::System.Boolean)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Boolean), @"False");
            var Border_ffceabf3c7fd4ca6bf00147427628def = new global::System.Windows.Controls.Border();
            Border_ffceabf3c7fd4ca6bf00147427628def.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"White");
            Border_ffceabf3c7fd4ca6bf00147427628def.BorderBrush = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Black");
            Border_ffceabf3c7fd4ca6bf00147427628def.BorderThickness = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");
            var ScrollViewer_75902183557049a3a0c20c1c02e344cf = new global::System.Windows.Controls.ScrollViewer();
            ScrollViewer_75902183557049a3a0c20c1c02e344cf.VerticalScrollBarVisibility = global::System.Windows.Controls.ScrollBarVisibility.Auto;
            ScrollViewer_75902183557049a3a0c20c1c02e344cf.HorizontalScrollBarVisibility = global::System.Windows.Controls.ScrollBarVisibility.Disabled;
            var ItemsPresenter_3395888e6b3247d79b98cf06e33db542 = new global::System.Windows.Controls.ItemsPresenter();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("ItemsHost", ItemsPresenter_3395888e6b3247d79b98cf06e33db542);
            ItemsPresenter_3395888e6b3247d79b98cf06e33db542.Name = "ItemsHost";

            ScrollViewer_75902183557049a3a0c20c1c02e344cf.Content = ItemsPresenter_3395888e6b3247d79b98cf06e33db542;

            var Binding_3cf6d6b574d54c96b2f3afda89bfc3a5 = new global::System.Windows.Data.Binding();
            Binding_3cf6d6b574d54c96b2f3afda89bfc3a5.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"MaxDropDownHeight");
            var RelativeSource_e259ece9a2184ea5ae4f20c97a5eb0ff = new global::System.Windows.Data.RelativeSource();
            RelativeSource_e259ece9a2184ea5ae4f20c97a5eb0ff.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_3cf6d6b574d54c96b2f3afda89bfc3a5.RelativeSource = RelativeSource_e259ece9a2184ea5ae4f20c97a5eb0ff;


            Binding_3cf6d6b574d54c96b2f3afda89bfc3a5.TemplateOwner = templateInstance_82c2e68d6ef24c5e90d456a94e511620;


            Border_ffceabf3c7fd4ca6bf00147427628def.Child = ScrollViewer_75902183557049a3a0c20c1c02e344cf;


            Popup_71bbbf41c17841138e8da63a6e3cdbe0.Child = Border_ffceabf3c7fd4ca6bf00147427628def;


            StackPanel_88ea3dbd18db4daf93783037aab66e3a.Children.Add(Border_20d9adcdf5c74d56aacd768851f0395b);
            StackPanel_88ea3dbd18db4daf93783037aab66e3a.Children.Add(Popup_71bbbf41c17841138e8da63a6e3cdbe0);



            ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c.SetBinding(global::System.Windows.FrameworkElement.MarginProperty, Binding_33874615a15f4f4a8e7a1b323f027661);
            ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c.SetBinding(global::System.Windows.Controls.ContentControl.ContentProperty, Binding_7636a81645ff4681b77f33c835487470);
            ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c.SetBinding(global::System.Windows.Controls.ContentControl.ContentTemplateProperty, Binding_5ac07c7327b640cc8f4d86a5562f82b1);
            Border_20d9adcdf5c74d56aacd768851f0395b.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_6ba1ab7c5d3b4b12902abe26bb950e80);
            Border_20d9adcdf5c74d56aacd768851f0395b.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_2d77a6f469534fe8ae41b6bb9c1561b5);
            Border_20d9adcdf5c74d56aacd768851f0395b.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_401409c10d7a44d08f75e12adf5a1fec);
            ScrollViewer_75902183557049a3a0c20c1c02e344cf.SetBinding(global::System.Windows.FrameworkElement.MaxHeightProperty, Binding_3cf6d6b574d54c96b2f3afda89bfc3a5);

            templateInstance_82c2e68d6ef24c5e90d456a94e511620.TemplateContent = StackPanel_88ea3dbd18db4daf93783037aab66e3a;
            return templateInstance_82c2e68d6ef24c5e90d456a94e511620;
        }
    }
}
#else
namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultComboBoxStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_d7e6874321be4521b479d97df2d01b78 = new global::Windows.UI.Xaml.Style();
                Style_d7e6874321be4521b479d97df2d01b78.TargetType = typeof(global::Windows.UI.Xaml.Controls.ComboBox);
                var Setter_f8a0ff0a7950487cbdfc353ac932627f = new global::Windows.UI.Xaml.Setter();
                Setter_f8a0ff0a7950487cbdfc353ac932627f.Property = global::Windows.UI.Xaml.Controls.ComboBox.BackgroundProperty;
                Setter_f8a0ff0a7950487cbdfc353ac932627f.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"White");

                var Setter_03b511d15a94443983c576309f6646ff = new global::Windows.UI.Xaml.Setter();
                Setter_03b511d15a94443983c576309f6646ff.Property = global::Windows.UI.Xaml.Controls.ComboBox.ForegroundProperty;
                Setter_03b511d15a94443983c576309f6646ff.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Black");

                var Setter_889237c2dc7f4a2aa643d752a4a18456 = new global::Windows.UI.Xaml.Setter();
                Setter_889237c2dc7f4a2aa643d752a4a18456.Property = global::Windows.UI.Xaml.Controls.ComboBox.PaddingProperty;
                Setter_889237c2dc7f4a2aa643d752a4a18456.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"6,2,30,2");

                var Setter_94f670595f1b435b90a236994c5c4096 = new global::Windows.UI.Xaml.Setter();
                Setter_94f670595f1b435b90a236994c5c4096.Property = global::Windows.UI.Xaml.Controls.ComboBox.CursorProperty;
                Setter_94f670595f1b435b90a236994c5c4096.Value = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Hand");

                var Setter_9f41e1651e8446248018c92a7481088d = new global::Windows.UI.Xaml.Setter();
                Setter_9f41e1651e8446248018c92a7481088d.Property = global::Windows.UI.Xaml.Controls.ComboBox.TemplateProperty;
                var ControlTemplate_d2528842177b4d42b5fa6765e0841e5e = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_d2528842177b4d42b5fa6765e0841e5e.TargetType = typeof(global::Windows.UI.Xaml.Controls.ComboBox);
                ControlTemplate_d2528842177b4d42b5fa6765e0841e5e.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_d2528842177b4d42b5fa6765e0841e5e);

                Setter_9f41e1651e8446248018c92a7481088d.Value = ControlTemplate_d2528842177b4d42b5fa6765e0841e5e;


                Style_d7e6874321be4521b479d97df2d01b78.Setters.Add(Setter_f8a0ff0a7950487cbdfc353ac932627f);
                Style_d7e6874321be4521b479d97df2d01b78.Setters.Add(Setter_03b511d15a94443983c576309f6646ff);
                Style_d7e6874321be4521b479d97df2d01b78.Setters.Add(Setter_889237c2dc7f4a2aa643d752a4a18456);
                Style_d7e6874321be4521b479d97df2d01b78.Setters.Add(Setter_94f670595f1b435b90a236994c5c4096);
                Style_d7e6874321be4521b479d97df2d01b78.Setters.Add(Setter_9f41e1651e8446248018c92a7481088d);


                DefaultStyle = Style_d7e6874321be4521b479d97df2d01b78;
            }

            return DefaultStyle;
        }

        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_d2528842177b4d42b5fa6765e0841e5e(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_82c2e68d6ef24c5e90d456a94e511620 = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_82c2e68d6ef24c5e90d456a94e511620.TemplateOwner = templateOwner;
            var StackPanel_88ea3dbd18db4daf93783037aab66e3a = new global::Windows.UI.Xaml.Controls.StackPanel();

            var Border_20d9adcdf5c74d56aacd768851f0395b = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("OuterBorder", Border_20d9adcdf5c74d56aacd768851f0395b);
            Border_20d9adcdf5c74d56aacd768851f0395b.Name = "OuterBorder";
            var Grid_86d48d9e220e4b56a3265cb1b7073abf = new global::Windows.UI.Xaml.Controls.Grid();
            var ToggleButton_05554ddcffb4400da5d7d822c4ad4729 = new global::Windows.UI.Xaml.Controls.Primitives.ToggleButton();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("DropDownToggle", ToggleButton_05554ddcffb4400da5d7d822c4ad4729);
            ToggleButton_05554ddcffb4400da5d7d822c4ad4729.Name = "DropDownToggle";
            ToggleButton_05554ddcffb4400da5d7d822c4ad4729.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch;
            ToggleButton_05554ddcffb4400da5d7d822c4ad4729.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Stretch;
            ToggleButton_05554ddcffb4400da5d7d822c4ad4729.HorizontalContentAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Right;
            ToggleButton_05554ddcffb4400da5d7d822c4ad4729.VerticalContentAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            var Path_970e4c9c536441aaa2b7cec321ab8522 = new global::Windows.UI.Xaml.Shapes.Path();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("arrow", Path_970e4c9c536441aaa2b7cec321ab8522);
            Path_970e4c9c536441aaa2b7cec321ab8522.Name = "arrow";
            Path_970e4c9c536441aaa2b7cec321ab8522.Visibility = global::Windows.UI.Xaml.Visibility.Visible;
            Path_970e4c9c536441aaa2b7cec321ab8522.Stroke = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Black");
            Path_970e4c9c536441aaa2b7cec321ab8522.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"13");
            Path_970e4c9c536441aaa2b7cec321ab8522.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_970e4c9c536441aaa2b7cec321ab8522.StrokeThickness = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"3");
            Path_970e4c9c536441aaa2b7cec321ab8522.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center;
            Path_970e4c9c536441aaa2b7cec321ab8522.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            Path_970e4c9c536441aaa2b7cec321ab8522.Stretch = global::Windows.UI.Xaml.Media.Stretch.None;
            Path_970e4c9c536441aaa2b7cec321ab8522.Data = (global::Windows.UI.Xaml.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Geometry), @"M 1.5,2.25 L 6.75,7.5 L 12,2.25");

            ToggleButton_05554ddcffb4400da5d7d822c4ad4729.Content = Path_970e4c9c536441aaa2b7cec321ab8522;


            var ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c = new global::Windows.UI.Xaml.Controls.ContentPresenter();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ContentPresenter", ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c);
            ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c.Name = "ContentPresenter";
            ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c.IsHitTestVisible = (global::System.Boolean)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Boolean), @"False");
            ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c.MinHeight = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"24");
            var Binding_33874615a15f4f4a8e7a1b323f027661 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_33874615a15f4f4a8e7a1b323f027661.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Padding");
            var RelativeSource_07f3d5f00ac24ae69b89f38d0c235cde = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_07f3d5f00ac24ae69b89f38d0c235cde.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_33874615a15f4f4a8e7a1b323f027661.RelativeSource = RelativeSource_07f3d5f00ac24ae69b89f38d0c235cde;


            Binding_33874615a15f4f4a8e7a1b323f027661.TemplateOwner = templateInstance_82c2e68d6ef24c5e90d456a94e511620;

            var Binding_7636a81645ff4681b77f33c835487470 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_7636a81645ff4681b77f33c835487470.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Content");
            var RelativeSource_e76ae58c16854addaddd66c7a1c7c2f1 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_e76ae58c16854addaddd66c7a1c7c2f1.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_7636a81645ff4681b77f33c835487470.RelativeSource = RelativeSource_e76ae58c16854addaddd66c7a1c7c2f1;


            Binding_7636a81645ff4681b77f33c835487470.TemplateOwner = templateInstance_82c2e68d6ef24c5e90d456a94e511620;

            var Binding_5ac07c7327b640cc8f4d86a5562f82b1 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_5ac07c7327b640cc8f4d86a5562f82b1.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"ContentTemplate");
            var RelativeSource_0cf506501910429a8f6910618ca3a865 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_0cf506501910429a8f6910618ca3a865.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_5ac07c7327b640cc8f4d86a5562f82b1.RelativeSource = RelativeSource_0cf506501910429a8f6910618ca3a865;


            Binding_5ac07c7327b640cc8f4d86a5562f82b1.TemplateOwner = templateInstance_82c2e68d6ef24c5e90d456a94e511620;


            Grid_86d48d9e220e4b56a3265cb1b7073abf.Children.Add(ToggleButton_05554ddcffb4400da5d7d822c4ad4729);
            Grid_86d48d9e220e4b56a3265cb1b7073abf.Children.Add(ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c);


            Border_20d9adcdf5c74d56aacd768851f0395b.Child = Grid_86d48d9e220e4b56a3265cb1b7073abf;

            var Binding_6ba1ab7c5d3b4b12902abe26bb950e80 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_6ba1ab7c5d3b4b12902abe26bb950e80.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_4201ad0529f54230bb21db229f3bef75 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_4201ad0529f54230bb21db229f3bef75.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_6ba1ab7c5d3b4b12902abe26bb950e80.RelativeSource = RelativeSource_4201ad0529f54230bb21db229f3bef75;


            Binding_6ba1ab7c5d3b4b12902abe26bb950e80.TemplateOwner = templateInstance_82c2e68d6ef24c5e90d456a94e511620;

            var Binding_2d77a6f469534fe8ae41b6bb9c1561b5 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_2d77a6f469534fe8ae41b6bb9c1561b5.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_6c5f4f40220a4b309bdd18fc28fc3f1a = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_6c5f4f40220a4b309bdd18fc28fc3f1a.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_2d77a6f469534fe8ae41b6bb9c1561b5.RelativeSource = RelativeSource_6c5f4f40220a4b309bdd18fc28fc3f1a;


            Binding_2d77a6f469534fe8ae41b6bb9c1561b5.TemplateOwner = templateInstance_82c2e68d6ef24c5e90d456a94e511620;

            var Binding_401409c10d7a44d08f75e12adf5a1fec = new global::Windows.UI.Xaml.Data.Binding();
            Binding_401409c10d7a44d08f75e12adf5a1fec.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_9455df17ab39482e83977458c783f35f = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_9455df17ab39482e83977458c783f35f.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_401409c10d7a44d08f75e12adf5a1fec.RelativeSource = RelativeSource_9455df17ab39482e83977458c783f35f;


            Binding_401409c10d7a44d08f75e12adf5a1fec.TemplateOwner = templateInstance_82c2e68d6ef24c5e90d456a94e511620;


            var Popup_71bbbf41c17841138e8da63a6e3cdbe0 = new global::Windows.UI.Xaml.Controls.Primitives.Popup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Popup", Popup_71bbbf41c17841138e8da63a6e3cdbe0);
            Popup_71bbbf41c17841138e8da63a6e3cdbe0.Name = "Popup";
            Popup_71bbbf41c17841138e8da63a6e3cdbe0.IsOpen = (global::System.Boolean)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Boolean), @"False");
            var Border_ffceabf3c7fd4ca6bf00147427628def = new global::Windows.UI.Xaml.Controls.Border();
            Border_ffceabf3c7fd4ca6bf00147427628def.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"White");
            Border_ffceabf3c7fd4ca6bf00147427628def.BorderBrush = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Black");
            Border_ffceabf3c7fd4ca6bf00147427628def.BorderThickness = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");
            var ScrollViewer_75902183557049a3a0c20c1c02e344cf = new global::Windows.UI.Xaml.Controls.ScrollViewer();
            ScrollViewer_75902183557049a3a0c20c1c02e344cf.VerticalScrollBarVisibility = global::Windows.UI.Xaml.Controls.ScrollBarVisibility.Auto;
            ScrollViewer_75902183557049a3a0c20c1c02e344cf.HorizontalScrollBarVisibility = global::Windows.UI.Xaml.Controls.ScrollBarVisibility.Disabled;
            var ItemsPresenter_3395888e6b3247d79b98cf06e33db542 = new global::Windows.UI.Xaml.Controls.ItemsPresenter();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ItemsHost", ItemsPresenter_3395888e6b3247d79b98cf06e33db542);
            ItemsPresenter_3395888e6b3247d79b98cf06e33db542.Name = "ItemsHost";

            ScrollViewer_75902183557049a3a0c20c1c02e344cf.Content = ItemsPresenter_3395888e6b3247d79b98cf06e33db542;

            var Binding_3cf6d6b574d54c96b2f3afda89bfc3a5 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_3cf6d6b574d54c96b2f3afda89bfc3a5.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"MaxDropDownHeight");
            var RelativeSource_e259ece9a2184ea5ae4f20c97a5eb0ff = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_e259ece9a2184ea5ae4f20c97a5eb0ff.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_3cf6d6b574d54c96b2f3afda89bfc3a5.RelativeSource = RelativeSource_e259ece9a2184ea5ae4f20c97a5eb0ff;


            Binding_3cf6d6b574d54c96b2f3afda89bfc3a5.TemplateOwner = templateInstance_82c2e68d6ef24c5e90d456a94e511620;


            Border_ffceabf3c7fd4ca6bf00147427628def.Child = ScrollViewer_75902183557049a3a0c20c1c02e344cf;


            Popup_71bbbf41c17841138e8da63a6e3cdbe0.Child = Border_ffceabf3c7fd4ca6bf00147427628def;


            StackPanel_88ea3dbd18db4daf93783037aab66e3a.Children.Add(Border_20d9adcdf5c74d56aacd768851f0395b);
            StackPanel_88ea3dbd18db4daf93783037aab66e3a.Children.Add(Popup_71bbbf41c17841138e8da63a6e3cdbe0);



            ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c.SetBinding(global::Windows.UI.Xaml.FrameworkElement.MarginProperty, Binding_33874615a15f4f4a8e7a1b323f027661);
            ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentProperty, Binding_7636a81645ff4681b77f33c835487470);
            ContentPresenter_9f86971e1dae47bc8ea39916b2b4fc4c.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentTemplateProperty, Binding_5ac07c7327b640cc8f4d86a5562f82b1);
            Border_20d9adcdf5c74d56aacd768851f0395b.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_6ba1ab7c5d3b4b12902abe26bb950e80);
            Border_20d9adcdf5c74d56aacd768851f0395b.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_2d77a6f469534fe8ae41b6bb9c1561b5);
            Border_20d9adcdf5c74d56aacd768851f0395b.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_401409c10d7a44d08f75e12adf5a1fec);
            ScrollViewer_75902183557049a3a0c20c1c02e344cf.SetBinding(global::Windows.UI.Xaml.FrameworkElement.MaxHeightProperty, Binding_3cf6d6b574d54c96b2f3afda89bfc3a5);

            templateInstance_82c2e68d6ef24c5e90d456a94e511620.TemplateContent = StackPanel_88ea3dbd18db4daf93783037aab66e3a;
            return templateInstance_82c2e68d6ef24c5e90d456a94e511620;
        }
    }
}
#endif