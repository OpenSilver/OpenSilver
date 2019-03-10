
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

#if false

#if MIGRATION
namespace System.Windows.Controls
{
    internal class INTERNAL_DefaultProgressBarStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_220b8e4b12e549dd86f73e774bee30a7 = new global::System.Windows.Style();
                Style_220b8e4b12e549dd86f73e774bee30a7.TargetType = typeof(global::System.Windows.Controls.ProgressBar);
                var Setter_1d48be9528ff479eb992f61342b8ed00 = new global::System.Windows.Setter();
                Setter_1d48be9528ff479eb992f61342b8ed00.Property = global::System.Windows.Controls.ProgressBar.MaximumProperty;
                Setter_1d48be9528ff479eb992f61342b8ed00.Value = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"100");

                var Setter_1a85179d800a4691b54baae49c3d8aa4 = new global::System.Windows.Setter();
                Setter_1a85179d800a4691b54baae49c3d8aa4.Property = global::System.Windows.Controls.ProgressBar.BackgroundProperty;
                Setter_1a85179d800a4691b54baae49c3d8aa4.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Gray");

                var Setter_0dace24a699b4a148728b7d38a487cc2 = new global::System.Windows.Setter();
                Setter_0dace24a699b4a148728b7d38a487cc2.Property = global::System.Windows.Controls.ProgressBar.ForegroundProperty;
                Setter_0dace24a699b4a148728b7d38a487cc2.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Blue");

                var Setter_3808d6e9e2aa47799aa677dd14b45cfc = new global::System.Windows.Setter();
                Setter_3808d6e9e2aa47799aa677dd14b45cfc.Property = global::System.Windows.Controls.ProgressBar.TemplateProperty;
                var ControlTemplate_cb31ed612d244fd58aa64042bc419537 = new global::System.Windows.Controls.ControlTemplate();
                ControlTemplate_cb31ed612d244fd58aa64042bc419537.TargetType = typeof(global::System.Windows.Controls.ProgressBar);
                ControlTemplate_cb31ed612d244fd58aa64042bc419537.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_cb31ed612d244fd58aa64042bc419537);

                Setter_3808d6e9e2aa47799aa677dd14b45cfc.Value = ControlTemplate_cb31ed612d244fd58aa64042bc419537;


                Style_220b8e4b12e549dd86f73e774bee30a7.Setters.Add(Setter_1d48be9528ff479eb992f61342b8ed00);
                Style_220b8e4b12e549dd86f73e774bee30a7.Setters.Add(Setter_1a85179d800a4691b54baae49c3d8aa4);
                Style_220b8e4b12e549dd86f73e774bee30a7.Setters.Add(Setter_0dace24a699b4a148728b7d38a487cc2);
                Style_220b8e4b12e549dd86f73e774bee30a7.Setters.Add(Setter_3808d6e9e2aa47799aa677dd14b45cfc);




                DefaultStyle = Style_220b8e4b12e549dd86f73e774bee30a7;
            }

            return DefaultStyle;
        }

        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_cb31ed612d244fd58aa64042bc419537(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_d72aae7172324b038efcdd410575590a = new global::System.Windows.TemplateInstance();
            templateInstance_d72aae7172324b038efcdd410575590a.TemplateOwner = templateOwner;
            var Canvas_49ffcd1ce86748b4a50c7215a26ba822 = new global::System.Windows.Controls.Canvas();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("RootCanvas", Canvas_49ffcd1ce86748b4a50c7215a26ba822);
            Canvas_49ffcd1ce86748b4a50c7215a26ba822.Name = "RootCanvas";
            var Rectangle_ae7e1d68442f44d29706c85656055fc2 = new global::System.Windows.Shapes.Rectangle();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("RectangleBehind", Rectangle_ae7e1d68442f44d29706c85656055fc2);
            Rectangle_ae7e1d68442f44d29706c85656055fc2.Name = "RectangleBehind";
            var Binding_5ffb7685b796455f946f92f0c8005665 = new global::System.Windows.Data.Binding();
            Binding_5ffb7685b796455f946f92f0c8005665.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_7887ba0e62374d829443584d324f3d25 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_7887ba0e62374d829443584d324f3d25.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_5ffb7685b796455f946f92f0c8005665.RelativeSource = RelativeSource_7887ba0e62374d829443584d324f3d25;


            Binding_5ffb7685b796455f946f92f0c8005665.TemplateOwner = templateInstance_d72aae7172324b038efcdd410575590a;


            var Rectangle_801390188ac6495391f90e6a877da71f = new global::System.Windows.Shapes.Rectangle();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("RectangleInFront", Rectangle_801390188ac6495391f90e6a877da71f);
            Rectangle_801390188ac6495391f90e6a877da71f.Name = "RectangleInFront";
            var Binding_5ed878753a3b4efe8d92de66b9bab080 = new global::System.Windows.Data.Binding();
            Binding_5ed878753a3b4efe8d92de66b9bab080.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Foreground");
            var RelativeSource_fa20d292fe744bfa83fe3ab5a0b25a85 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_fa20d292fe744bfa83fe3ab5a0b25a85.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_5ed878753a3b4efe8d92de66b9bab080.RelativeSource = RelativeSource_fa20d292fe744bfa83fe3ab5a0b25a85;


            Binding_5ed878753a3b4efe8d92de66b9bab080.TemplateOwner = templateInstance_d72aae7172324b038efcdd410575590a;


            Canvas_49ffcd1ce86748b4a50c7215a26ba822.Children.Add(Rectangle_ae7e1d68442f44d29706c85656055fc2);
            Canvas_49ffcd1ce86748b4a50c7215a26ba822.Children.Add(Rectangle_801390188ac6495391f90e6a877da71f);



            Rectangle_ae7e1d68442f44d29706c85656055fc2.SetBinding(global::System.Windows.Shapes.Shape.FillProperty, Binding_5ffb7685b796455f946f92f0c8005665);
            Rectangle_801390188ac6495391f90e6a877da71f.SetBinding(global::System.Windows.Shapes.Shape.FillProperty, Binding_5ed878753a3b4efe8d92de66b9bab080);

            templateInstance_d72aae7172324b038efcdd410575590a.TemplateContent = Canvas_49ffcd1ce86748b4a50c7215a26ba822;
            return templateInstance_d72aae7172324b038efcdd410575590a;
        }
        

    }
}
#else
namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultProgressBarStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_220b8e4b12e549dd86f73e774bee30a7 = new global::Windows.UI.Xaml.Style();
                Style_220b8e4b12e549dd86f73e774bee30a7.TargetType = typeof(global::Windows.UI.Xaml.Controls.ProgressBar);
                var Setter_1d48be9528ff479eb992f61342b8ed00 = new global::Windows.UI.Xaml.Setter();
                Setter_1d48be9528ff479eb992f61342b8ed00.Property = global::Windows.UI.Xaml.Controls.ProgressBar.MaximumProperty;
                Setter_1d48be9528ff479eb992f61342b8ed00.Value = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"100");

                var Setter_1a85179d800a4691b54baae49c3d8aa4 = new global::Windows.UI.Xaml.Setter();
                Setter_1a85179d800a4691b54baae49c3d8aa4.Property = global::Windows.UI.Xaml.Controls.ProgressBar.BackgroundProperty;
                Setter_1a85179d800a4691b54baae49c3d8aa4.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Gray");

                var Setter_0dace24a699b4a148728b7d38a487cc2 = new global::Windows.UI.Xaml.Setter();
                Setter_0dace24a699b4a148728b7d38a487cc2.Property = global::Windows.UI.Xaml.Controls.ProgressBar.ForegroundProperty;
                Setter_0dace24a699b4a148728b7d38a487cc2.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Blue");

                var Setter_3808d6e9e2aa47799aa677dd14b45cfc = new global::Windows.UI.Xaml.Setter();
                Setter_3808d6e9e2aa47799aa677dd14b45cfc.Property = global::Windows.UI.Xaml.Controls.ProgressBar.TemplateProperty;
                var ControlTemplate_cb31ed612d244fd58aa64042bc419537 = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_cb31ed612d244fd58aa64042bc419537.TargetType = typeof(global::Windows.UI.Xaml.Controls.ProgressBar);
                ControlTemplate_cb31ed612d244fd58aa64042bc419537.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_cb31ed612d244fd58aa64042bc419537);

                Setter_3808d6e9e2aa47799aa677dd14b45cfc.Value = ControlTemplate_cb31ed612d244fd58aa64042bc419537;


                Style_220b8e4b12e549dd86f73e774bee30a7.Setters.Add(Setter_1d48be9528ff479eb992f61342b8ed00);
                Style_220b8e4b12e549dd86f73e774bee30a7.Setters.Add(Setter_1a85179d800a4691b54baae49c3d8aa4);
                Style_220b8e4b12e549dd86f73e774bee30a7.Setters.Add(Setter_0dace24a699b4a148728b7d38a487cc2);
                Style_220b8e4b12e549dd86f73e774bee30a7.Setters.Add(Setter_3808d6e9e2aa47799aa677dd14b45cfc);




                DefaultStyle = Style_220b8e4b12e549dd86f73e774bee30a7;
            }

            return DefaultStyle;
        }

        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_cb31ed612d244fd58aa64042bc419537(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_d72aae7172324b038efcdd410575590a = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_d72aae7172324b038efcdd410575590a.TemplateOwner = templateOwner;
            var Canvas_49ffcd1ce86748b4a50c7215a26ba822 = new global::Windows.UI.Xaml.Controls.Canvas();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("RootCanvas", Canvas_49ffcd1ce86748b4a50c7215a26ba822);
            Canvas_49ffcd1ce86748b4a50c7215a26ba822.Name = "RootCanvas";
            var Rectangle_ae7e1d68442f44d29706c85656055fc2 = new global::Windows.UI.Xaml.Shapes.Rectangle();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("RectangleBehind", Rectangle_ae7e1d68442f44d29706c85656055fc2);
            Rectangle_ae7e1d68442f44d29706c85656055fc2.Name = "RectangleBehind";
            var Binding_5ffb7685b796455f946f92f0c8005665 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_5ffb7685b796455f946f92f0c8005665.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_7887ba0e62374d829443584d324f3d25 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_7887ba0e62374d829443584d324f3d25.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_5ffb7685b796455f946f92f0c8005665.RelativeSource = RelativeSource_7887ba0e62374d829443584d324f3d25;


            Binding_5ffb7685b796455f946f92f0c8005665.TemplateOwner = templateInstance_d72aae7172324b038efcdd410575590a;


            var Rectangle_801390188ac6495391f90e6a877da71f = new global::Windows.UI.Xaml.Shapes.Rectangle();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("RectangleInFront", Rectangle_801390188ac6495391f90e6a877da71f);
            Rectangle_801390188ac6495391f90e6a877da71f.Name = "RectangleInFront";
            var Binding_5ed878753a3b4efe8d92de66b9bab080 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_5ed878753a3b4efe8d92de66b9bab080.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Foreground");
            var RelativeSource_fa20d292fe744bfa83fe3ab5a0b25a85 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_fa20d292fe744bfa83fe3ab5a0b25a85.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_5ed878753a3b4efe8d92de66b9bab080.RelativeSource = RelativeSource_fa20d292fe744bfa83fe3ab5a0b25a85;


            Binding_5ed878753a3b4efe8d92de66b9bab080.TemplateOwner = templateInstance_d72aae7172324b038efcdd410575590a;


            Canvas_49ffcd1ce86748b4a50c7215a26ba822.Children.Add(Rectangle_ae7e1d68442f44d29706c85656055fc2);
            Canvas_49ffcd1ce86748b4a50c7215a26ba822.Children.Add(Rectangle_801390188ac6495391f90e6a877da71f);



            Rectangle_ae7e1d68442f44d29706c85656055fc2.SetBinding(global::Windows.UI.Xaml.Shapes.Shape.FillProperty, Binding_5ffb7685b796455f946f92f0c8005665);
            Rectangle_801390188ac6495391f90e6a877da71f.SetBinding(global::Windows.UI.Xaml.Shapes.Shape.FillProperty, Binding_5ed878753a3b4efe8d92de66b9bab080);

            templateInstance_d72aae7172324b038efcdd410575590a.TemplateContent = Canvas_49ffcd1ce86748b4a50c7215a26ba822;
            return templateInstance_d72aae7172324b038efcdd410575590a;
        }
        

    }
}
#endif

#endif