
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


#if MIGRATION
namespace System.Windows.Controls.Primitives
{
    internal class INTERNAL_DefaultScrollBarStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_6f0ea010fdc24c8381c60f33f55e7117 = new global::System.Windows.Style();
                Style_6f0ea010fdc24c8381c60f33f55e7117.TargetType = typeof(global::System.Windows.Controls.Primitives.ScrollBar);
                var Setter_a2eedab9facc4d0eb379847ecad57948 = new global::System.Windows.Setter();
                Setter_a2eedab9facc4d0eb379847ecad57948.Property = global::System.Windows.Controls.Primitives.ScrollBar.BackgroundProperty;
                Setter_a2eedab9facc4d0eb379847ecad57948.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFF1F1F1");

                var Setter_045d06392601441a9388d76bb4fb1afc = new global::System.Windows.Setter();
                Setter_045d06392601441a9388d76bb4fb1afc.Property = global::System.Windows.Controls.Primitives.ScrollBar.ForegroundProperty;
                Setter_045d06392601441a9388d76bb4fb1afc.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FF333333");

                var Setter_f926f1319e63465198de2242eb8faf24 = new global::System.Windows.Setter();
                Setter_f926f1319e63465198de2242eb8faf24.Property = global::System.Windows.Controls.Primitives.ScrollBar.TemplateProperty;
                var ControlTemplate_251390e190e44b098c399864df328b7e = new global::System.Windows.Controls.ControlTemplate();
                ControlTemplate_251390e190e44b098c399864df328b7e.TargetType = typeof(global::System.Windows.Controls.Primitives.ScrollBar);
                ControlTemplate_251390e190e44b098c399864df328b7e.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_251390e190e44b098c399864df328b7e);

                Setter_f926f1319e63465198de2242eb8faf24.Value = ControlTemplate_251390e190e44b098c399864df328b7e;


                Style_6f0ea010fdc24c8381c60f33f55e7117.Setters.Add(Setter_a2eedab9facc4d0eb379847ecad57948);
                Style_6f0ea010fdc24c8381c60f33f55e7117.Setters.Add(Setter_045d06392601441a9388d76bb4fb1afc);
                Style_6f0ea010fdc24c8381c60f33f55e7117.Setters.Add(Setter_f926f1319e63465198de2242eb8faf24);


                DefaultStyle = Style_6f0ea010fdc24c8381c60f33f55e7117;
            }

            return DefaultStyle;
        }



        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_73b5d2f326a3435fba59ec89e38775d3(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_fcd8c0f3ab4244ada6549373e783bc8a = new global::System.Windows.TemplateInstance();
            templateInstance_fcd8c0f3ab4244ada6549373e783bc8a.TemplateOwner = templateOwner;
            var Border_dcefb5d61e384d8ca9deb82d89301bc2 = new global::System.Windows.Controls.Border();
            Border_dcefb5d61e384d8ca9deb82d89301bc2.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFBBBBBB");




            templateInstance_fcd8c0f3ab4244ada6549373e783bc8a.TemplateContent = Border_dcefb5d61e384d8ca9deb82d89301bc2;
            return templateInstance_fcd8c0f3ab4244ada6549373e783bc8a;
        }



        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_cb10afad1faf4a09a5564aa126a9eb77(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_56ffe4021c144dbfa516e157ab88d210 = new global::System.Windows.TemplateInstance();
            templateInstance_56ffe4021c144dbfa516e157ab88d210.TemplateOwner = templateOwner;
            var Border_301a8a7d6c55499d83ed0daafc7f9a38 = new global::System.Windows.Controls.Border();
            Border_301a8a7d6c55499d83ed0daafc7f9a38.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFBBBBBB");




            templateInstance_56ffe4021c144dbfa516e157ab88d210.TemplateContent = Border_301a8a7d6c55499d83ed0daafc7f9a38;
            return templateInstance_56ffe4021c144dbfa516e157ab88d210;
        }



        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_251390e190e44b098c399864df328b7e(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_99c63e52661c4ede97eee3ab28ece2e9 = new global::System.Windows.TemplateInstance();
            templateInstance_99c63e52661c4ede97eee3ab28ece2e9.TemplateOwner = templateOwner;
            var Grid_b91fd3df9a45405e802d5d566e50e093 = new global::System.Windows.Controls.Grid();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Root", Grid_b91fd3df9a45405e802d5d566e50e093);
            Grid_b91fd3df9a45405e802d5d566e50e093.Name = "Root";
            var VisualStateGroup_5ba98e1a04294682be500f0fc2216f4f = new global::System.Windows.VisualStateGroup();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_5ba98e1a04294682be500f0fc2216f4f);
            VisualStateGroup_5ba98e1a04294682be500f0fc2216f4f.Name = "CommonStates";
            var VisualState_97648fcf91a641c989c24997694602d0 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Normal", VisualState_97648fcf91a641c989c24997694602d0);
            VisualState_97648fcf91a641c989c24997694602d0.Name = "Normal";

            var VisualState_130f55c0d86f40afb55c792505ae9e97 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("MouseOver", VisualState_130f55c0d86f40afb55c792505ae9e97);
            VisualState_130f55c0d86f40afb55c792505ae9e97.Name = "MouseOver";

            var VisualState_d3a9d2215eaf4bc7a63a7a6865d82f77 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_d3a9d2215eaf4bc7a63a7a6865d82f77);
            VisualState_d3a9d2215eaf4bc7a63a7a6865d82f77.Name = "Disabled";

            VisualStateGroup_5ba98e1a04294682be500f0fc2216f4f.States.Add(VisualState_97648fcf91a641c989c24997694602d0);
            VisualStateGroup_5ba98e1a04294682be500f0fc2216f4f.States.Add(VisualState_130f55c0d86f40afb55c792505ae9e97);
            VisualStateGroup_5ba98e1a04294682be500f0fc2216f4f.States.Add(VisualState_d3a9d2215eaf4bc7a63a7a6865d82f77);


            ((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_5ba98e1a04294682be500f0fc2216f4f);

            var Canvas_8c8634e2122e46ae908c5b52eb72a02a = new global::System.Windows.Controls.Canvas();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("HorizontalRoot", Canvas_8c8634e2122e46ae908c5b52eb72a02a);
            Canvas_8c8634e2122e46ae908c5b52eb72a02a.Name = "HorizontalRoot";
            Canvas_8c8634e2122e46ae908c5b52eb72a02a.Visibility = global::System.Windows.Visibility.Collapsed;
            var Button_2b42b162ca3345d599bab595d3aeb9b6 = new global::System.Windows.Controls.Button();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("HorizontalSmallDecrease", Button_2b42b162ca3345d599bab595d3aeb9b6);
            Button_2b42b162ca3345d599bab595d3aeb9b6.Name = "HorizontalSmallDecrease";
            Button_2b42b162ca3345d599bab595d3aeb9b6.Padding = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0");
            Button_2b42b162ca3345d599bab595d3aeb9b6.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");
            var Path_598e63d63d5e4bb7be04c6804959b8b3 = new global::System.Windows.Shapes.Path();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("ArrowLeft", Path_598e63d63d5e4bb7be04c6804959b8b3);
            Path_598e63d63d5e4bb7be04c6804959b8b3.Name = "ArrowLeft";
            Path_598e63d63d5e4bb7be04c6804959b8b3.Fill = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FF555555");
            Path_598e63d63d5e4bb7be04c6804959b8b3.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_598e63d63d5e4bb7be04c6804959b8b3.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_598e63d63d5e4bb7be04c6804959b8b3.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0,0,3,0");
            Path_598e63d63d5e4bb7be04c6804959b8b3.StrokeThickness = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"3");
            Path_598e63d63d5e4bb7be04c6804959b8b3.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Center;
            Path_598e63d63d5e4bb7be04c6804959b8b3.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
            Path_598e63d63d5e4bb7be04c6804959b8b3.Stretch = global::System.Windows.Media.Stretch.Fill;
            Path_598e63d63d5e4bb7be04c6804959b8b3.Data = (global::System.Windows.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Geometry), @"M 2,4.5 L 5.5,1 L 5.5,8");

            Button_2b42b162ca3345d599bab595d3aeb9b6.Content = Path_598e63d63d5e4bb7be04c6804959b8b3;


            var Button_04bc40faca864af1963d945f0057af6e = new global::System.Windows.Controls.Button();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("HorizontalSmallIncrease", Button_04bc40faca864af1963d945f0057af6e);
            Button_04bc40faca864af1963d945f0057af6e.Name = "HorizontalSmallIncrease";
            Button_04bc40faca864af1963d945f0057af6e.Padding = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0");
            Button_04bc40faca864af1963d945f0057af6e.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");
            var Path_7aacba5ad6484663951662b02a9a6d57 = new global::System.Windows.Shapes.Path();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("ArrowRight", Path_7aacba5ad6484663951662b02a9a6d57);
            Path_7aacba5ad6484663951662b02a9a6d57.Name = "ArrowRight";
            Path_7aacba5ad6484663951662b02a9a6d57.Fill = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FF555555");
            Path_7aacba5ad6484663951662b02a9a6d57.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_7aacba5ad6484663951662b02a9a6d57.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_7aacba5ad6484663951662b02a9a6d57.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0,0,3,0");
            Path_7aacba5ad6484663951662b02a9a6d57.StrokeThickness = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"3");
            Path_7aacba5ad6484663951662b02a9a6d57.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Center;
            Path_7aacba5ad6484663951662b02a9a6d57.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
            Path_7aacba5ad6484663951662b02a9a6d57.Stretch = global::System.Windows.Media.Stretch.Fill;
            Path_7aacba5ad6484663951662b02a9a6d57.Data = (global::System.Windows.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Geometry), @"M 2,1 L 5.5,4.5 L 2,8");

            Button_04bc40faca864af1963d945f0057af6e.Content = Path_7aacba5ad6484663951662b02a9a6d57;


            var Button_7f12cd6a5d924a749b6bc51ad491e6fa = new global::System.Windows.Controls.Button();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("HorizontalLargeDecrease", Button_7f12cd6a5d924a749b6bc51ad491e6fa);
            Button_7f12cd6a5d924a749b6bc51ad491e6fa.Name = "HorizontalLargeDecrease";
            Button_7f12cd6a5d924a749b6bc51ad491e6fa.Padding = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0");
            Button_7f12cd6a5d924a749b6bc51ad491e6fa.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");

            var Button_d2a8dd5f4a434317b791be081cc7e83a = new global::System.Windows.Controls.Button();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("HorizontalLargeIncrease", Button_d2a8dd5f4a434317b791be081cc7e83a);
            Button_d2a8dd5f4a434317b791be081cc7e83a.Name = "HorizontalLargeIncrease";
            Button_d2a8dd5f4a434317b791be081cc7e83a.Padding = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0");
            Button_d2a8dd5f4a434317b791be081cc7e83a.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");

            var Thumb_2be11030d3a547a098b80ff4403b6222 = new global::System.Windows.Controls.Primitives.Thumb();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("HorizontalThumb", Thumb_2be11030d3a547a098b80ff4403b6222);
            Thumb_2be11030d3a547a098b80ff4403b6222.Name = "HorizontalThumb";
            Thumb_2be11030d3a547a098b80ff4403b6222.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");
            var ControlTemplate_73b5d2f326a3435fba59ec89e38775d3 = new global::System.Windows.Controls.ControlTemplate();
            ControlTemplate_73b5d2f326a3435fba59ec89e38775d3.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_73b5d2f326a3435fba59ec89e38775d3);

            Thumb_2be11030d3a547a098b80ff4403b6222.Template = ControlTemplate_73b5d2f326a3435fba59ec89e38775d3;


            Canvas_8c8634e2122e46ae908c5b52eb72a02a.Children.Add(Button_2b42b162ca3345d599bab595d3aeb9b6);
            Canvas_8c8634e2122e46ae908c5b52eb72a02a.Children.Add(Button_04bc40faca864af1963d945f0057af6e);
            Canvas_8c8634e2122e46ae908c5b52eb72a02a.Children.Add(Button_7f12cd6a5d924a749b6bc51ad491e6fa);
            Canvas_8c8634e2122e46ae908c5b52eb72a02a.Children.Add(Button_d2a8dd5f4a434317b791be081cc7e83a);
            Canvas_8c8634e2122e46ae908c5b52eb72a02a.Children.Add(Thumb_2be11030d3a547a098b80ff4403b6222);

            var Binding_c6b8dbf096644f1688439e0a011d2410 = new global::System.Windows.Data.Binding();
            Binding_c6b8dbf096644f1688439e0a011d2410.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_40097b5b79a54f54a83d1e014c541733 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_40097b5b79a54f54a83d1e014c541733.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_c6b8dbf096644f1688439e0a011d2410.RelativeSource = RelativeSource_40097b5b79a54f54a83d1e014c541733;


            Binding_c6b8dbf096644f1688439e0a011d2410.TemplateOwner = templateInstance_99c63e52661c4ede97eee3ab28ece2e9;


            var Canvas_b0eecd9fb6a24708b5516f6e2a457c12 = new global::System.Windows.Controls.Canvas();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("VerticalRoot", Canvas_b0eecd9fb6a24708b5516f6e2a457c12);
            Canvas_b0eecd9fb6a24708b5516f6e2a457c12.Name = "VerticalRoot";
            Canvas_b0eecd9fb6a24708b5516f6e2a457c12.Visibility = global::System.Windows.Visibility.Collapsed;
            var Button_4553f3d1bb8043e180a3b29536d6ae8c = new global::System.Windows.Controls.Button();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("VerticalSmallDecrease", Button_4553f3d1bb8043e180a3b29536d6ae8c);
            Button_4553f3d1bb8043e180a3b29536d6ae8c.Name = "VerticalSmallDecrease";
            Button_4553f3d1bb8043e180a3b29536d6ae8c.Padding = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0");
            Button_4553f3d1bb8043e180a3b29536d6ae8c.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");
            var Path_3d8e9bf3aa8e4c24a4284bb07e490c65 = new global::System.Windows.Shapes.Path();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("ArrowTop", Path_3d8e9bf3aa8e4c24a4284bb07e490c65);
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.Name = "ArrowTop";
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.Fill = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FF555555");
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0,0,3,0");
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.StrokeThickness = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"3");
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Center;
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.Stretch = global::System.Windows.Media.Stretch.Fill;
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.Data = (global::System.Windows.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Geometry), @"M 4.5,2 L 1,5.5 L 8,5.5");

            Button_4553f3d1bb8043e180a3b29536d6ae8c.Content = Path_3d8e9bf3aa8e4c24a4284bb07e490c65;


            var Button_b12c57b4d4d44d0991cfc9e4057d5844 = new global::System.Windows.Controls.Button();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("VerticalSmallIncrease", Button_b12c57b4d4d44d0991cfc9e4057d5844);
            Button_b12c57b4d4d44d0991cfc9e4057d5844.Name = "VerticalSmallIncrease";
            Button_b12c57b4d4d44d0991cfc9e4057d5844.Padding = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0");
            Button_b12c57b4d4d44d0991cfc9e4057d5844.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");
            var Path_2e076ca6e7ad44c4ba724f4322cbdcdc = new global::System.Windows.Shapes.Path();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("ArrowBottom", Path_2e076ca6e7ad44c4ba724f4322cbdcdc);
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.Name = "ArrowBottom";
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.Fill = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FF555555");
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0,0,3,0");
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.StrokeThickness = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"3");
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Center;
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.Stretch = global::System.Windows.Media.Stretch.Fill;
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.Data = (global::System.Windows.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Geometry), @"M 1,2 L 4.5,5.5 L 8,2");

            Button_b12c57b4d4d44d0991cfc9e4057d5844.Content = Path_2e076ca6e7ad44c4ba724f4322cbdcdc;


            var Button_0270b75aed98484fb83aff92fb6e9ac4 = new global::System.Windows.Controls.Button();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("VerticalLargeDecrease", Button_0270b75aed98484fb83aff92fb6e9ac4);
            Button_0270b75aed98484fb83aff92fb6e9ac4.Name = "VerticalLargeDecrease";
            Button_0270b75aed98484fb83aff92fb6e9ac4.Padding = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0");
            Button_0270b75aed98484fb83aff92fb6e9ac4.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");

            var Button_6cea53355a694a2d8bb97367f8984035 = new global::System.Windows.Controls.Button();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("VerticalLargeIncrease", Button_6cea53355a694a2d8bb97367f8984035);
            Button_6cea53355a694a2d8bb97367f8984035.Name = "VerticalLargeIncrease";
            Button_6cea53355a694a2d8bb97367f8984035.Padding = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0");
            Button_6cea53355a694a2d8bb97367f8984035.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");

            var Thumb_e2b84f2d12e746e8935d10cf222a1b5a = new global::System.Windows.Controls.Primitives.Thumb();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("VerticalThumb", Thumb_e2b84f2d12e746e8935d10cf222a1b5a);
            Thumb_e2b84f2d12e746e8935d10cf222a1b5a.Name = "VerticalThumb";
            Thumb_e2b84f2d12e746e8935d10cf222a1b5a.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFBBBBBB");
            Thumb_e2b84f2d12e746e8935d10cf222a1b5a.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");
            var ControlTemplate_cb10afad1faf4a09a5564aa126a9eb77 = new global::System.Windows.Controls.ControlTemplate();
            ControlTemplate_cb10afad1faf4a09a5564aa126a9eb77.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_cb10afad1faf4a09a5564aa126a9eb77);

            Thumb_e2b84f2d12e746e8935d10cf222a1b5a.Template = ControlTemplate_cb10afad1faf4a09a5564aa126a9eb77;


            Canvas_b0eecd9fb6a24708b5516f6e2a457c12.Children.Add(Button_4553f3d1bb8043e180a3b29536d6ae8c);
            Canvas_b0eecd9fb6a24708b5516f6e2a457c12.Children.Add(Button_b12c57b4d4d44d0991cfc9e4057d5844);
            Canvas_b0eecd9fb6a24708b5516f6e2a457c12.Children.Add(Button_0270b75aed98484fb83aff92fb6e9ac4);
            Canvas_b0eecd9fb6a24708b5516f6e2a457c12.Children.Add(Button_6cea53355a694a2d8bb97367f8984035);
            Canvas_b0eecd9fb6a24708b5516f6e2a457c12.Children.Add(Thumb_e2b84f2d12e746e8935d10cf222a1b5a);

            var Binding_0067ba7c462745ec900bfad3584ead41 = new global::System.Windows.Data.Binding();
            Binding_0067ba7c462745ec900bfad3584ead41.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_7335092f8fbf4b31a5b810e3bd1763ea = new global::System.Windows.Data.RelativeSource();
            RelativeSource_7335092f8fbf4b31a5b810e3bd1763ea.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_0067ba7c462745ec900bfad3584ead41.RelativeSource = RelativeSource_7335092f8fbf4b31a5b810e3bd1763ea;


            Binding_0067ba7c462745ec900bfad3584ead41.TemplateOwner = templateInstance_99c63e52661c4ede97eee3ab28ece2e9;


            Grid_b91fd3df9a45405e802d5d566e50e093.Children.Add(Canvas_8c8634e2122e46ae908c5b52eb72a02a);
            Grid_b91fd3df9a45405e802d5d566e50e093.Children.Add(Canvas_b0eecd9fb6a24708b5516f6e2a457c12);



            Canvas_8c8634e2122e46ae908c5b52eb72a02a.SetBinding(global::System.Windows.Controls.Panel.BackgroundProperty, Binding_c6b8dbf096644f1688439e0a011d2410);
            Canvas_b0eecd9fb6a24708b5516f6e2a457c12.SetBinding(global::System.Windows.Controls.Panel.BackgroundProperty, Binding_0067ba7c462745ec900bfad3584ead41);

            templateInstance_99c63e52661c4ede97eee3ab28ece2e9.TemplateContent = Grid_b91fd3df9a45405e802d5d566e50e093;
            return templateInstance_99c63e52661c4ede97eee3ab28ece2e9;
        }

    }
}
#else
namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultScrollBarStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_6f0ea010fdc24c8381c60f33f55e7117 = new global::Windows.UI.Xaml.Style();
                Style_6f0ea010fdc24c8381c60f33f55e7117.TargetType = typeof(global::Windows.UI.Xaml.Controls.Primitives.ScrollBar);
                var Setter_a2eedab9facc4d0eb379847ecad57948 = new global::Windows.UI.Xaml.Setter();
                Setter_a2eedab9facc4d0eb379847ecad57948.Property = global::Windows.UI.Xaml.Controls.Primitives.ScrollBar.BackgroundProperty;
                Setter_a2eedab9facc4d0eb379847ecad57948.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFF1F1F1");

                var Setter_045d06392601441a9388d76bb4fb1afc = new global::Windows.UI.Xaml.Setter();
                Setter_045d06392601441a9388d76bb4fb1afc.Property = global::Windows.UI.Xaml.Controls.Primitives.ScrollBar.ForegroundProperty;
                Setter_045d06392601441a9388d76bb4fb1afc.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FF333333");

                var Setter_f926f1319e63465198de2242eb8faf24 = new global::Windows.UI.Xaml.Setter();
                Setter_f926f1319e63465198de2242eb8faf24.Property = global::Windows.UI.Xaml.Controls.Primitives.ScrollBar.TemplateProperty;
                var ControlTemplate_251390e190e44b098c399864df328b7e = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_251390e190e44b098c399864df328b7e.TargetType = typeof(global::Windows.UI.Xaml.Controls.Primitives.ScrollBar);
                ControlTemplate_251390e190e44b098c399864df328b7e.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_251390e190e44b098c399864df328b7e);

                Setter_f926f1319e63465198de2242eb8faf24.Value = ControlTemplate_251390e190e44b098c399864df328b7e;


                Style_6f0ea010fdc24c8381c60f33f55e7117.Setters.Add(Setter_a2eedab9facc4d0eb379847ecad57948);
                Style_6f0ea010fdc24c8381c60f33f55e7117.Setters.Add(Setter_045d06392601441a9388d76bb4fb1afc);
                Style_6f0ea010fdc24c8381c60f33f55e7117.Setters.Add(Setter_f926f1319e63465198de2242eb8faf24);


                DefaultStyle = Style_6f0ea010fdc24c8381c60f33f55e7117;
            }

            return DefaultStyle;
        }



        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_73b5d2f326a3435fba59ec89e38775d3(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_fcd8c0f3ab4244ada6549373e783bc8a = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_fcd8c0f3ab4244ada6549373e783bc8a.TemplateOwner = templateOwner;
            var Border_dcefb5d61e384d8ca9deb82d89301bc2 = new global::Windows.UI.Xaml.Controls.Border();
            Border_dcefb5d61e384d8ca9deb82d89301bc2.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFBBBBBB");




            templateInstance_fcd8c0f3ab4244ada6549373e783bc8a.TemplateContent = Border_dcefb5d61e384d8ca9deb82d89301bc2;
            return templateInstance_fcd8c0f3ab4244ada6549373e783bc8a;
        }



        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_cb10afad1faf4a09a5564aa126a9eb77(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_56ffe4021c144dbfa516e157ab88d210 = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_56ffe4021c144dbfa516e157ab88d210.TemplateOwner = templateOwner;
            var Border_301a8a7d6c55499d83ed0daafc7f9a38 = new global::Windows.UI.Xaml.Controls.Border();
            Border_301a8a7d6c55499d83ed0daafc7f9a38.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFBBBBBB");




            templateInstance_56ffe4021c144dbfa516e157ab88d210.TemplateContent = Border_301a8a7d6c55499d83ed0daafc7f9a38;
            return templateInstance_56ffe4021c144dbfa516e157ab88d210;
        }



        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_251390e190e44b098c399864df328b7e(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_99c63e52661c4ede97eee3ab28ece2e9 = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_99c63e52661c4ede97eee3ab28ece2e9.TemplateOwner = templateOwner;
            var Grid_b91fd3df9a45405e802d5d566e50e093 = new global::Windows.UI.Xaml.Controls.Grid();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Root", Grid_b91fd3df9a45405e802d5d566e50e093);
            Grid_b91fd3df9a45405e802d5d566e50e093.Name = "Root";
            var VisualStateGroup_5ba98e1a04294682be500f0fc2216f4f = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_5ba98e1a04294682be500f0fc2216f4f);
            VisualStateGroup_5ba98e1a04294682be500f0fc2216f4f.Name = "CommonStates";
            var VisualState_97648fcf91a641c989c24997694602d0 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Normal", VisualState_97648fcf91a641c989c24997694602d0);
            VisualState_97648fcf91a641c989c24997694602d0.Name = "Normal";

            var VisualState_130f55c0d86f40afb55c792505ae9e97 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("PointerOver", VisualState_130f55c0d86f40afb55c792505ae9e97);
            VisualState_130f55c0d86f40afb55c792505ae9e97.Name = "PointerOver";

            var VisualState_d3a9d2215eaf4bc7a63a7a6865d82f77 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_d3a9d2215eaf4bc7a63a7a6865d82f77);
            VisualState_d3a9d2215eaf4bc7a63a7a6865d82f77.Name = "Disabled";

            VisualStateGroup_5ba98e1a04294682be500f0fc2216f4f.States.Add(VisualState_97648fcf91a641c989c24997694602d0);
            VisualStateGroup_5ba98e1a04294682be500f0fc2216f4f.States.Add(VisualState_130f55c0d86f40afb55c792505ae9e97);
            VisualStateGroup_5ba98e1a04294682be500f0fc2216f4f.States.Add(VisualState_d3a9d2215eaf4bc7a63a7a6865d82f77);


            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_5ba98e1a04294682be500f0fc2216f4f);

            var Canvas_8c8634e2122e46ae908c5b52eb72a02a = new global::Windows.UI.Xaml.Controls.Canvas();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("HorizontalRoot", Canvas_8c8634e2122e46ae908c5b52eb72a02a);
            Canvas_8c8634e2122e46ae908c5b52eb72a02a.Name = "HorizontalRoot";
            Canvas_8c8634e2122e46ae908c5b52eb72a02a.Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
            var Button_2b42b162ca3345d599bab595d3aeb9b6 = new global::Windows.UI.Xaml.Controls.Button();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("HorizontalSmallDecrease", Button_2b42b162ca3345d599bab595d3aeb9b6);
            Button_2b42b162ca3345d599bab595d3aeb9b6.Name = "HorizontalSmallDecrease";
            Button_2b42b162ca3345d599bab595d3aeb9b6.Padding = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0");
            Button_2b42b162ca3345d599bab595d3aeb9b6.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");
            var Path_598e63d63d5e4bb7be04c6804959b8b3 = new global::Windows.UI.Xaml.Shapes.Path();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ArrowLeft", Path_598e63d63d5e4bb7be04c6804959b8b3);
            Path_598e63d63d5e4bb7be04c6804959b8b3.Name = "ArrowLeft";
            Path_598e63d63d5e4bb7be04c6804959b8b3.Fill = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FF555555");
            Path_598e63d63d5e4bb7be04c6804959b8b3.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_598e63d63d5e4bb7be04c6804959b8b3.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_598e63d63d5e4bb7be04c6804959b8b3.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0,0,3,0");
            Path_598e63d63d5e4bb7be04c6804959b8b3.StrokeThickness = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"3");
            Path_598e63d63d5e4bb7be04c6804959b8b3.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center;
            Path_598e63d63d5e4bb7be04c6804959b8b3.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            Path_598e63d63d5e4bb7be04c6804959b8b3.Stretch = global::Windows.UI.Xaml.Media.Stretch.Fill;
            Path_598e63d63d5e4bb7be04c6804959b8b3.Data = (global::Windows.UI.Xaml.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Geometry), @"M 2,4.5 L 5.5,1 L 5.5,8");

            Button_2b42b162ca3345d599bab595d3aeb9b6.Content = Path_598e63d63d5e4bb7be04c6804959b8b3;


            var Button_04bc40faca864af1963d945f0057af6e = new global::Windows.UI.Xaml.Controls.Button();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("HorizontalSmallIncrease", Button_04bc40faca864af1963d945f0057af6e);
            Button_04bc40faca864af1963d945f0057af6e.Name = "HorizontalSmallIncrease";
            Button_04bc40faca864af1963d945f0057af6e.Padding = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0");
            Button_04bc40faca864af1963d945f0057af6e.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");
            var Path_7aacba5ad6484663951662b02a9a6d57 = new global::Windows.UI.Xaml.Shapes.Path();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ArrowRight", Path_7aacba5ad6484663951662b02a9a6d57);
            Path_7aacba5ad6484663951662b02a9a6d57.Name = "ArrowRight";
            Path_7aacba5ad6484663951662b02a9a6d57.Fill = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FF555555");
            Path_7aacba5ad6484663951662b02a9a6d57.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_7aacba5ad6484663951662b02a9a6d57.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_7aacba5ad6484663951662b02a9a6d57.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0,0,3,0");
            Path_7aacba5ad6484663951662b02a9a6d57.StrokeThickness = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"3");
            Path_7aacba5ad6484663951662b02a9a6d57.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center;
            Path_7aacba5ad6484663951662b02a9a6d57.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            Path_7aacba5ad6484663951662b02a9a6d57.Stretch = global::Windows.UI.Xaml.Media.Stretch.Fill;
            Path_7aacba5ad6484663951662b02a9a6d57.Data = (global::Windows.UI.Xaml.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Geometry), @"M 2,1 L 5.5,4.5 L 2,8");

            Button_04bc40faca864af1963d945f0057af6e.Content = Path_7aacba5ad6484663951662b02a9a6d57;


            var Button_7f12cd6a5d924a749b6bc51ad491e6fa = new global::Windows.UI.Xaml.Controls.Button();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("HorizontalLargeDecrease", Button_7f12cd6a5d924a749b6bc51ad491e6fa);
            Button_7f12cd6a5d924a749b6bc51ad491e6fa.Name = "HorizontalLargeDecrease";
            Button_7f12cd6a5d924a749b6bc51ad491e6fa.Padding = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0");
            Button_7f12cd6a5d924a749b6bc51ad491e6fa.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");

            var Button_d2a8dd5f4a434317b791be081cc7e83a = new global::Windows.UI.Xaml.Controls.Button();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("HorizontalLargeIncrease", Button_d2a8dd5f4a434317b791be081cc7e83a);
            Button_d2a8dd5f4a434317b791be081cc7e83a.Name = "HorizontalLargeIncrease";
            Button_d2a8dd5f4a434317b791be081cc7e83a.Padding = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0");
            Button_d2a8dd5f4a434317b791be081cc7e83a.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");

            var Thumb_2be11030d3a547a098b80ff4403b6222 = new global::Windows.UI.Xaml.Controls.Primitives.Thumb();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("HorizontalThumb", Thumb_2be11030d3a547a098b80ff4403b6222);
            Thumb_2be11030d3a547a098b80ff4403b6222.Name = "HorizontalThumb";
            Thumb_2be11030d3a547a098b80ff4403b6222.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");
            var ControlTemplate_73b5d2f326a3435fba59ec89e38775d3 = new global::Windows.UI.Xaml.Controls.ControlTemplate();
            ControlTemplate_73b5d2f326a3435fba59ec89e38775d3.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_73b5d2f326a3435fba59ec89e38775d3);

            Thumb_2be11030d3a547a098b80ff4403b6222.Template = ControlTemplate_73b5d2f326a3435fba59ec89e38775d3;


            Canvas_8c8634e2122e46ae908c5b52eb72a02a.Children.Add(Button_2b42b162ca3345d599bab595d3aeb9b6);
            Canvas_8c8634e2122e46ae908c5b52eb72a02a.Children.Add(Button_04bc40faca864af1963d945f0057af6e);
            Canvas_8c8634e2122e46ae908c5b52eb72a02a.Children.Add(Button_7f12cd6a5d924a749b6bc51ad491e6fa);
            Canvas_8c8634e2122e46ae908c5b52eb72a02a.Children.Add(Button_d2a8dd5f4a434317b791be081cc7e83a);
            Canvas_8c8634e2122e46ae908c5b52eb72a02a.Children.Add(Thumb_2be11030d3a547a098b80ff4403b6222);

            var Binding_c6b8dbf096644f1688439e0a011d2410 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_c6b8dbf096644f1688439e0a011d2410.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_40097b5b79a54f54a83d1e014c541733 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_40097b5b79a54f54a83d1e014c541733.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_c6b8dbf096644f1688439e0a011d2410.RelativeSource = RelativeSource_40097b5b79a54f54a83d1e014c541733;


            Binding_c6b8dbf096644f1688439e0a011d2410.TemplateOwner = templateInstance_99c63e52661c4ede97eee3ab28ece2e9;


            var Canvas_b0eecd9fb6a24708b5516f6e2a457c12 = new global::Windows.UI.Xaml.Controls.Canvas();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("VerticalRoot", Canvas_b0eecd9fb6a24708b5516f6e2a457c12);
            Canvas_b0eecd9fb6a24708b5516f6e2a457c12.Name = "VerticalRoot";
            Canvas_b0eecd9fb6a24708b5516f6e2a457c12.Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
            var Button_4553f3d1bb8043e180a3b29536d6ae8c = new global::Windows.UI.Xaml.Controls.Button();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("VerticalSmallDecrease", Button_4553f3d1bb8043e180a3b29536d6ae8c);
            Button_4553f3d1bb8043e180a3b29536d6ae8c.Name = "VerticalSmallDecrease";
            Button_4553f3d1bb8043e180a3b29536d6ae8c.Padding = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0");
            Button_4553f3d1bb8043e180a3b29536d6ae8c.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");
            var Path_3d8e9bf3aa8e4c24a4284bb07e490c65 = new global::Windows.UI.Xaml.Shapes.Path();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ArrowTop", Path_3d8e9bf3aa8e4c24a4284bb07e490c65);
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.Name = "ArrowTop";
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.Fill = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FF555555");
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0,0,3,0");
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.StrokeThickness = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"3");
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center;
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.Stretch = global::Windows.UI.Xaml.Media.Stretch.Fill;
            Path_3d8e9bf3aa8e4c24a4284bb07e490c65.Data = (global::Windows.UI.Xaml.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Geometry), @"M 4.5,2 L 1,5.5 L 8,5.5");

            Button_4553f3d1bb8043e180a3b29536d6ae8c.Content = Path_3d8e9bf3aa8e4c24a4284bb07e490c65;


            var Button_b12c57b4d4d44d0991cfc9e4057d5844 = new global::Windows.UI.Xaml.Controls.Button();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("VerticalSmallIncrease", Button_b12c57b4d4d44d0991cfc9e4057d5844);
            Button_b12c57b4d4d44d0991cfc9e4057d5844.Name = "VerticalSmallIncrease";
            Button_b12c57b4d4d44d0991cfc9e4057d5844.Padding = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0");
            Button_b12c57b4d4d44d0991cfc9e4057d5844.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");
            var Path_2e076ca6e7ad44c4ba724f4322cbdcdc = new global::Windows.UI.Xaml.Shapes.Path();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ArrowBottom", Path_2e076ca6e7ad44c4ba724f4322cbdcdc);
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.Name = "ArrowBottom";
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.Fill = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FF555555");
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0,0,3,0");
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.StrokeThickness = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"3");
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center;
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.Stretch = global::Windows.UI.Xaml.Media.Stretch.Fill;
            Path_2e076ca6e7ad44c4ba724f4322cbdcdc.Data = (global::Windows.UI.Xaml.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Geometry), @"M 1,2 L 4.5,5.5 L 8,2");

            Button_b12c57b4d4d44d0991cfc9e4057d5844.Content = Path_2e076ca6e7ad44c4ba724f4322cbdcdc;


            var Button_0270b75aed98484fb83aff92fb6e9ac4 = new global::Windows.UI.Xaml.Controls.Button();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("VerticalLargeDecrease", Button_0270b75aed98484fb83aff92fb6e9ac4);
            Button_0270b75aed98484fb83aff92fb6e9ac4.Name = "VerticalLargeDecrease";
            Button_0270b75aed98484fb83aff92fb6e9ac4.Padding = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0");
            Button_0270b75aed98484fb83aff92fb6e9ac4.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");

            var Button_6cea53355a694a2d8bb97367f8984035 = new global::Windows.UI.Xaml.Controls.Button();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("VerticalLargeIncrease", Button_6cea53355a694a2d8bb97367f8984035);
            Button_6cea53355a694a2d8bb97367f8984035.Name = "VerticalLargeIncrease";
            Button_6cea53355a694a2d8bb97367f8984035.Padding = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0");
            Button_6cea53355a694a2d8bb97367f8984035.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");

            var Thumb_e2b84f2d12e746e8935d10cf222a1b5a = new global::Windows.UI.Xaml.Controls.Primitives.Thumb();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("VerticalThumb", Thumb_e2b84f2d12e746e8935d10cf222a1b5a);
            Thumb_e2b84f2d12e746e8935d10cf222a1b5a.Name = "VerticalThumb";
            Thumb_e2b84f2d12e746e8935d10cf222a1b5a.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFBBBBBB");
            Thumb_e2b84f2d12e746e8935d10cf222a1b5a.Cursor = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Arrow");
            var ControlTemplate_cb10afad1faf4a09a5564aa126a9eb77 = new global::Windows.UI.Xaml.Controls.ControlTemplate();
            ControlTemplate_cb10afad1faf4a09a5564aa126a9eb77.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_cb10afad1faf4a09a5564aa126a9eb77);

            Thumb_e2b84f2d12e746e8935d10cf222a1b5a.Template = ControlTemplate_cb10afad1faf4a09a5564aa126a9eb77;


            Canvas_b0eecd9fb6a24708b5516f6e2a457c12.Children.Add(Button_4553f3d1bb8043e180a3b29536d6ae8c);
            Canvas_b0eecd9fb6a24708b5516f6e2a457c12.Children.Add(Button_b12c57b4d4d44d0991cfc9e4057d5844);
            Canvas_b0eecd9fb6a24708b5516f6e2a457c12.Children.Add(Button_0270b75aed98484fb83aff92fb6e9ac4);
            Canvas_b0eecd9fb6a24708b5516f6e2a457c12.Children.Add(Button_6cea53355a694a2d8bb97367f8984035);
            Canvas_b0eecd9fb6a24708b5516f6e2a457c12.Children.Add(Thumb_e2b84f2d12e746e8935d10cf222a1b5a);

            var Binding_0067ba7c462745ec900bfad3584ead41 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_0067ba7c462745ec900bfad3584ead41.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_7335092f8fbf4b31a5b810e3bd1763ea = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_7335092f8fbf4b31a5b810e3bd1763ea.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_0067ba7c462745ec900bfad3584ead41.RelativeSource = RelativeSource_7335092f8fbf4b31a5b810e3bd1763ea;


            Binding_0067ba7c462745ec900bfad3584ead41.TemplateOwner = templateInstance_99c63e52661c4ede97eee3ab28ece2e9;


            Grid_b91fd3df9a45405e802d5d566e50e093.Children.Add(Canvas_8c8634e2122e46ae908c5b52eb72a02a);
            Grid_b91fd3df9a45405e802d5d566e50e093.Children.Add(Canvas_b0eecd9fb6a24708b5516f6e2a457c12);



            Canvas_8c8634e2122e46ae908c5b52eb72a02a.SetBinding(global::Windows.UI.Xaml.Controls.Panel.BackgroundProperty, Binding_c6b8dbf096644f1688439e0a011d2410);
            Canvas_b0eecd9fb6a24708b5516f6e2a457c12.SetBinding(global::Windows.UI.Xaml.Controls.Panel.BackgroundProperty, Binding_0067ba7c462745ec900bfad3584ead41);

            templateInstance_99c63e52661c4ede97eee3ab28ece2e9.TemplateContent = Grid_b91fd3df9a45405e802d5d566e50e093;
            return templateInstance_99c63e52661c4ede97eee3ab28ece2e9;
        }

    }
}
#endif