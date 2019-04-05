
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



#if MIGRATION
namespace System.Windows.Controls
{
    internal class INTERNAL_DefaultToolTipStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_1fce9d8b240f43699d4cfadb91e47890 = new global::System.Windows.Style();
                Style_1fce9d8b240f43699d4cfadb91e47890.TargetType = typeof(global::System.Windows.Controls.ToolTip);
                var Setter_e06844f9581641d8a6d18a4ef4a89688 = new global::System.Windows.Setter();
                Setter_e06844f9581641d8a6d18a4ef4a89688.Property = global::System.Windows.Controls.ToolTip.ForegroundProperty;
                Setter_e06844f9581641d8a6d18a4ef4a89688.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FF666666");

                var Setter_33c260a08ab846cb8b740f18d45efed0 = new global::System.Windows.Setter();
                Setter_33c260a08ab846cb8b740f18d45efed0.Property = global::System.Windows.Controls.ToolTip.BackgroundProperty;
                Setter_33c260a08ab846cb8b740f18d45efed0.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"White");

                var Setter_914c018d6c6746a599cefcc8e2e2b65c = new global::System.Windows.Setter();
                Setter_914c018d6c6746a599cefcc8e2e2b65c.Property = global::System.Windows.Controls.ToolTip.BorderBrushProperty;
                Setter_914c018d6c6746a599cefcc8e2e2b65c.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Gray");

                var Setter_6d6c6ecf1a454eceb4c3d7118e4303ab = new global::System.Windows.Setter();
                Setter_6d6c6ecf1a454eceb4c3d7118e4303ab.Property = global::System.Windows.Controls.ToolTip.BorderThicknessProperty;
                Setter_6d6c6ecf1a454eceb4c3d7118e4303ab.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"2");

                var Setter_b6bc401e4f9041abba01031b23ada71c = new global::System.Windows.Setter();
                Setter_b6bc401e4f9041abba01031b23ada71c.Property = global::System.Windows.Controls.ToolTip.FontSizeProperty;
                Setter_b6bc401e4f9041abba01031b23ada71c.Value = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"12");

                var Setter_fb9da313d9a044d69f8e659c96d6e9a8 = new global::System.Windows.Setter();
                Setter_fb9da313d9a044d69f8e659c96d6e9a8.Property = global::System.Windows.Controls.ToolTip.PaddingProperty;
                Setter_fb9da313d9a044d69f8e659c96d6e9a8.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"10,6,10,7");

                var Setter_1638d838d2db41688eeb3b796ceaca61 = new global::System.Windows.Setter();
                Setter_1638d838d2db41688eeb3b796ceaca61.Property = global::System.Windows.Controls.ToolTip.TemplateProperty;
                var ControlTemplate_3fa1f80e9e6e4ec2bb8ddaf591950b02 = new global::System.Windows.Controls.ControlTemplate();
                ControlTemplate_3fa1f80e9e6e4ec2bb8ddaf591950b02.TargetType = typeof(global::System.Windows.Controls.ToolTip);
                ControlTemplate_3fa1f80e9e6e4ec2bb8ddaf591950b02.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_3fa1f80e9e6e4ec2bb8ddaf591950b02);

                Setter_1638d838d2db41688eeb3b796ceaca61.Value = ControlTemplate_3fa1f80e9e6e4ec2bb8ddaf591950b02;


                Style_1fce9d8b240f43699d4cfadb91e47890.Setters.Add(Setter_e06844f9581641d8a6d18a4ef4a89688);
                Style_1fce9d8b240f43699d4cfadb91e47890.Setters.Add(Setter_33c260a08ab846cb8b740f18d45efed0);
                Style_1fce9d8b240f43699d4cfadb91e47890.Setters.Add(Setter_914c018d6c6746a599cefcc8e2e2b65c);
                Style_1fce9d8b240f43699d4cfadb91e47890.Setters.Add(Setter_6d6c6ecf1a454eceb4c3d7118e4303ab);
                Style_1fce9d8b240f43699d4cfadb91e47890.Setters.Add(Setter_b6bc401e4f9041abba01031b23ada71c);
                Style_1fce9d8b240f43699d4cfadb91e47890.Setters.Add(Setter_fb9da313d9a044d69f8e659c96d6e9a8);
                Style_1fce9d8b240f43699d4cfadb91e47890.Setters.Add(Setter_1638d838d2db41688eeb3b796ceaca61);


                DefaultStyle = Style_1fce9d8b240f43699d4cfadb91e47890;
            }

            return DefaultStyle;
        }



        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_3fa1f80e9e6e4ec2bb8ddaf591950b02(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_b3ca292c907742a284e8481793a4dd0c = new global::System.Windows.TemplateInstance();
            templateInstance_b3ca292c907742a284e8481793a4dd0c.TemplateOwner = templateOwner;
            var Border_b17e5b1f5958447d9318cc54f3d93c32 = new global::System.Windows.Controls.Border();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("LayoutRoot", Border_b17e5b1f5958447d9318cc54f3d93c32);
            Border_b17e5b1f5958447d9318cc54f3d93c32.Name = "LayoutRoot";
            var ContentPresenter_abfcda7e9f35491690b9dde27cf9582f = new global::System.Windows.Controls.ContentPresenter();
            var Binding_146cea452c694e0395995cb1eff9ba1b = new global::System.Windows.Data.Binding();
            Binding_146cea452c694e0395995cb1eff9ba1b.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"ContentTemplate");
            var RelativeSource_ddb054a750774fc8894df6f9e387aa1d = new global::System.Windows.Data.RelativeSource();
            RelativeSource_ddb054a750774fc8894df6f9e387aa1d.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_146cea452c694e0395995cb1eff9ba1b.RelativeSource = RelativeSource_ddb054a750774fc8894df6f9e387aa1d;


            Binding_146cea452c694e0395995cb1eff9ba1b.TemplateOwner = templateInstance_b3ca292c907742a284e8481793a4dd0c;

            var Binding_12d5b1fbd02a4c389bcd75c152fc8bcd = new global::System.Windows.Data.Binding();
            Binding_12d5b1fbd02a4c389bcd75c152fc8bcd.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Content");
            var RelativeSource_00489e5430aa4d0bbbb386e2a4cb4c5e = new global::System.Windows.Data.RelativeSource();
            RelativeSource_00489e5430aa4d0bbbb386e2a4cb4c5e.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_12d5b1fbd02a4c389bcd75c152fc8bcd.RelativeSource = RelativeSource_00489e5430aa4d0bbbb386e2a4cb4c5e;


            Binding_12d5b1fbd02a4c389bcd75c152fc8bcd.TemplateOwner = templateInstance_b3ca292c907742a284e8481793a4dd0c;

            var Binding_f6856b00b01c4949adbfa7d7eb138047 = new global::System.Windows.Data.Binding();
            Binding_f6856b00b01c4949adbfa7d7eb138047.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Padding");
            var RelativeSource_17d7ca33dc184472a36858c8f05cb252 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_17d7ca33dc184472a36858c8f05cb252.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_f6856b00b01c4949adbfa7d7eb138047.RelativeSource = RelativeSource_17d7ca33dc184472a36858c8f05cb252;


            Binding_f6856b00b01c4949adbfa7d7eb138047.TemplateOwner = templateInstance_b3ca292c907742a284e8481793a4dd0c;


            Border_b17e5b1f5958447d9318cc54f3d93c32.Child = ContentPresenter_abfcda7e9f35491690b9dde27cf9582f;

            var Binding_f7a1c9bac29c44d19f7197cce7d417d3 = new global::System.Windows.Data.Binding();
            Binding_f7a1c9bac29c44d19f7197cce7d417d3.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
            var RelativeSource_94f8889d9e204045b3de694995f8b982 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_94f8889d9e204045b3de694995f8b982.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_f7a1c9bac29c44d19f7197cce7d417d3.RelativeSource = RelativeSource_94f8889d9e204045b3de694995f8b982;


            Binding_f7a1c9bac29c44d19f7197cce7d417d3.TemplateOwner = templateInstance_b3ca292c907742a284e8481793a4dd0c;

            var Binding_c8aa62ed456b4ae2b9b438f92ed9acce = new global::System.Windows.Data.Binding();
            Binding_c8aa62ed456b4ae2b9b438f92ed9acce.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
            var RelativeSource_768beddafb544b6b977e5cf5e1647722 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_768beddafb544b6b977e5cf5e1647722.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_c8aa62ed456b4ae2b9b438f92ed9acce.RelativeSource = RelativeSource_768beddafb544b6b977e5cf5e1647722;


            Binding_c8aa62ed456b4ae2b9b438f92ed9acce.TemplateOwner = templateInstance_b3ca292c907742a284e8481793a4dd0c;

            var Binding_82f75b1847c54527a5eaa8d0cc4da36d = new global::System.Windows.Data.Binding();
            Binding_82f75b1847c54527a5eaa8d0cc4da36d.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_b4771e62d8674080ac7d40b42b921d07 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_b4771e62d8674080ac7d40b42b921d07.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_82f75b1847c54527a5eaa8d0cc4da36d.RelativeSource = RelativeSource_b4771e62d8674080ac7d40b42b921d07;


            Binding_82f75b1847c54527a5eaa8d0cc4da36d.TemplateOwner = templateInstance_b3ca292c907742a284e8481793a4dd0c;



            ContentPresenter_abfcda7e9f35491690b9dde27cf9582f.SetBinding(global::System.Windows.Controls.ContentControl.ContentTemplateProperty, Binding_146cea452c694e0395995cb1eff9ba1b);
            ContentPresenter_abfcda7e9f35491690b9dde27cf9582f.SetBinding(global::System.Windows.Controls.ContentControl.ContentProperty, Binding_12d5b1fbd02a4c389bcd75c152fc8bcd);
            ContentPresenter_abfcda7e9f35491690b9dde27cf9582f.SetBinding(global::System.Windows.FrameworkElement.MarginProperty, Binding_f6856b00b01c4949adbfa7d7eb138047);
            Border_b17e5b1f5958447d9318cc54f3d93c32.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_f7a1c9bac29c44d19f7197cce7d417d3);
            Border_b17e5b1f5958447d9318cc54f3d93c32.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_c8aa62ed456b4ae2b9b438f92ed9acce);
            Border_b17e5b1f5958447d9318cc54f3d93c32.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_82f75b1847c54527a5eaa8d0cc4da36d);

            templateInstance_b3ca292c907742a284e8481793a4dd0c.TemplateContent = Border_b17e5b1f5958447d9318cc54f3d93c32;
            return templateInstance_b3ca292c907742a284e8481793a4dd0c;
        }

    }
}


#else

namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultToolTipStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_1fce9d8b240f43699d4cfadb91e47890 = new global::Windows.UI.Xaml.Style();
                Style_1fce9d8b240f43699d4cfadb91e47890.TargetType = typeof(global::Windows.UI.Xaml.Controls.ToolTip);
                var Setter_e06844f9581641d8a6d18a4ef4a89688 = new global::Windows.UI.Xaml.Setter();
                Setter_e06844f9581641d8a6d18a4ef4a89688.Property = global::Windows.UI.Xaml.Controls.ToolTip.ForegroundProperty;
                Setter_e06844f9581641d8a6d18a4ef4a89688.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FF666666");

                var Setter_33c260a08ab846cb8b740f18d45efed0 = new global::Windows.UI.Xaml.Setter();
                Setter_33c260a08ab846cb8b740f18d45efed0.Property = global::Windows.UI.Xaml.Controls.ToolTip.BackgroundProperty;
                Setter_33c260a08ab846cb8b740f18d45efed0.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"White");

                var Setter_914c018d6c6746a599cefcc8e2e2b65c = new global::Windows.UI.Xaml.Setter();
                Setter_914c018d6c6746a599cefcc8e2e2b65c.Property = global::Windows.UI.Xaml.Controls.ToolTip.BorderBrushProperty;
                Setter_914c018d6c6746a599cefcc8e2e2b65c.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Gray");

                var Setter_6d6c6ecf1a454eceb4c3d7118e4303ab = new global::Windows.UI.Xaml.Setter();
                Setter_6d6c6ecf1a454eceb4c3d7118e4303ab.Property = global::Windows.UI.Xaml.Controls.ToolTip.BorderThicknessProperty;
                Setter_6d6c6ecf1a454eceb4c3d7118e4303ab.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"2");

                var Setter_b6bc401e4f9041abba01031b23ada71c = new global::Windows.UI.Xaml.Setter();
                Setter_b6bc401e4f9041abba01031b23ada71c.Property = global::Windows.UI.Xaml.Controls.ToolTip.FontSizeProperty;
                Setter_b6bc401e4f9041abba01031b23ada71c.Value = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"12");

                var Setter_fb9da313d9a044d69f8e659c96d6e9a8 = new global::Windows.UI.Xaml.Setter();
                Setter_fb9da313d9a044d69f8e659c96d6e9a8.Property = global::Windows.UI.Xaml.Controls.ToolTip.PaddingProperty;
                Setter_fb9da313d9a044d69f8e659c96d6e9a8.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"10,6,10,7");

                var Setter_1638d838d2db41688eeb3b796ceaca61 = new global::Windows.UI.Xaml.Setter();
                Setter_1638d838d2db41688eeb3b796ceaca61.Property = global::Windows.UI.Xaml.Controls.ToolTip.TemplateProperty;
                var ControlTemplate_3fa1f80e9e6e4ec2bb8ddaf591950b02 = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_3fa1f80e9e6e4ec2bb8ddaf591950b02.TargetType = typeof(global::Windows.UI.Xaml.Controls.ToolTip);
                ControlTemplate_3fa1f80e9e6e4ec2bb8ddaf591950b02.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_3fa1f80e9e6e4ec2bb8ddaf591950b02);

                Setter_1638d838d2db41688eeb3b796ceaca61.Value = ControlTemplate_3fa1f80e9e6e4ec2bb8ddaf591950b02;


                Style_1fce9d8b240f43699d4cfadb91e47890.Setters.Add(Setter_e06844f9581641d8a6d18a4ef4a89688);
                Style_1fce9d8b240f43699d4cfadb91e47890.Setters.Add(Setter_33c260a08ab846cb8b740f18d45efed0);
                Style_1fce9d8b240f43699d4cfadb91e47890.Setters.Add(Setter_914c018d6c6746a599cefcc8e2e2b65c);
                Style_1fce9d8b240f43699d4cfadb91e47890.Setters.Add(Setter_6d6c6ecf1a454eceb4c3d7118e4303ab);
                Style_1fce9d8b240f43699d4cfadb91e47890.Setters.Add(Setter_b6bc401e4f9041abba01031b23ada71c);
                Style_1fce9d8b240f43699d4cfadb91e47890.Setters.Add(Setter_fb9da313d9a044d69f8e659c96d6e9a8);
                Style_1fce9d8b240f43699d4cfadb91e47890.Setters.Add(Setter_1638d838d2db41688eeb3b796ceaca61);


                DefaultStyle = Style_1fce9d8b240f43699d4cfadb91e47890;
            }

            return DefaultStyle;
        }



        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_3fa1f80e9e6e4ec2bb8ddaf591950b02(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_b3ca292c907742a284e8481793a4dd0c = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_b3ca292c907742a284e8481793a4dd0c.TemplateOwner = templateOwner;
            var Border_b17e5b1f5958447d9318cc54f3d93c32 = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("LayoutRoot", Border_b17e5b1f5958447d9318cc54f3d93c32);
            Border_b17e5b1f5958447d9318cc54f3d93c32.Name = "LayoutRoot";
            var ContentPresenter_abfcda7e9f35491690b9dde27cf9582f = new global::Windows.UI.Xaml.Controls.ContentPresenter();
            var Binding_146cea452c694e0395995cb1eff9ba1b = new global::Windows.UI.Xaml.Data.Binding();
            Binding_146cea452c694e0395995cb1eff9ba1b.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"ContentTemplate");
            var RelativeSource_ddb054a750774fc8894df6f9e387aa1d = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_ddb054a750774fc8894df6f9e387aa1d.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_146cea452c694e0395995cb1eff9ba1b.RelativeSource = RelativeSource_ddb054a750774fc8894df6f9e387aa1d;


            Binding_146cea452c694e0395995cb1eff9ba1b.TemplateOwner = templateInstance_b3ca292c907742a284e8481793a4dd0c;

            var Binding_12d5b1fbd02a4c389bcd75c152fc8bcd = new global::Windows.UI.Xaml.Data.Binding();
            Binding_12d5b1fbd02a4c389bcd75c152fc8bcd.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Content");
            var RelativeSource_00489e5430aa4d0bbbb386e2a4cb4c5e = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_00489e5430aa4d0bbbb386e2a4cb4c5e.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_12d5b1fbd02a4c389bcd75c152fc8bcd.RelativeSource = RelativeSource_00489e5430aa4d0bbbb386e2a4cb4c5e;


            Binding_12d5b1fbd02a4c389bcd75c152fc8bcd.TemplateOwner = templateInstance_b3ca292c907742a284e8481793a4dd0c;

            var Binding_f6856b00b01c4949adbfa7d7eb138047 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_f6856b00b01c4949adbfa7d7eb138047.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Padding");
            var RelativeSource_17d7ca33dc184472a36858c8f05cb252 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_17d7ca33dc184472a36858c8f05cb252.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_f6856b00b01c4949adbfa7d7eb138047.RelativeSource = RelativeSource_17d7ca33dc184472a36858c8f05cb252;


            Binding_f6856b00b01c4949adbfa7d7eb138047.TemplateOwner = templateInstance_b3ca292c907742a284e8481793a4dd0c;


            Border_b17e5b1f5958447d9318cc54f3d93c32.Child = ContentPresenter_abfcda7e9f35491690b9dde27cf9582f;

            var Binding_f7a1c9bac29c44d19f7197cce7d417d3 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_f7a1c9bac29c44d19f7197cce7d417d3.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_94f8889d9e204045b3de694995f8b982 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_94f8889d9e204045b3de694995f8b982.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_f7a1c9bac29c44d19f7197cce7d417d3.RelativeSource = RelativeSource_94f8889d9e204045b3de694995f8b982;


            Binding_f7a1c9bac29c44d19f7197cce7d417d3.TemplateOwner = templateInstance_b3ca292c907742a284e8481793a4dd0c;

            var Binding_c8aa62ed456b4ae2b9b438f92ed9acce = new global::Windows.UI.Xaml.Data.Binding();
            Binding_c8aa62ed456b4ae2b9b438f92ed9acce.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_768beddafb544b6b977e5cf5e1647722 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_768beddafb544b6b977e5cf5e1647722.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_c8aa62ed456b4ae2b9b438f92ed9acce.RelativeSource = RelativeSource_768beddafb544b6b977e5cf5e1647722;


            Binding_c8aa62ed456b4ae2b9b438f92ed9acce.TemplateOwner = templateInstance_b3ca292c907742a284e8481793a4dd0c;

            var Binding_82f75b1847c54527a5eaa8d0cc4da36d = new global::Windows.UI.Xaml.Data.Binding();
            Binding_82f75b1847c54527a5eaa8d0cc4da36d.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_b4771e62d8674080ac7d40b42b921d07 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_b4771e62d8674080ac7d40b42b921d07.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_82f75b1847c54527a5eaa8d0cc4da36d.RelativeSource = RelativeSource_b4771e62d8674080ac7d40b42b921d07;


            Binding_82f75b1847c54527a5eaa8d0cc4da36d.TemplateOwner = templateInstance_b3ca292c907742a284e8481793a4dd0c;



            ContentPresenter_abfcda7e9f35491690b9dde27cf9582f.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentTemplateProperty, Binding_146cea452c694e0395995cb1eff9ba1b);
            ContentPresenter_abfcda7e9f35491690b9dde27cf9582f.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentProperty, Binding_12d5b1fbd02a4c389bcd75c152fc8bcd);
            ContentPresenter_abfcda7e9f35491690b9dde27cf9582f.SetBinding(global::Windows.UI.Xaml.FrameworkElement.MarginProperty, Binding_f6856b00b01c4949adbfa7d7eb138047);
            Border_b17e5b1f5958447d9318cc54f3d93c32.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_f7a1c9bac29c44d19f7197cce7d417d3);
            Border_b17e5b1f5958447d9318cc54f3d93c32.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_c8aa62ed456b4ae2b9b438f92ed9acce);
            Border_b17e5b1f5958447d9318cc54f3d93c32.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_82f75b1847c54527a5eaa8d0cc4da36d);

            templateInstance_b3ca292c907742a284e8481793a4dd0c.TemplateContent = Border_b17e5b1f5958447d9318cc54f3d93c32;
            return templateInstance_b3ca292c907742a284e8481793a4dd0c;
        }

    }
}
#endif