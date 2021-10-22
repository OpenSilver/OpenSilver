
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
    internal class INTERNAL_DefaultAutoCompleteBoxStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_af59b8b24edb44f68b94ec0141a5f51e = new global::System.Windows.Style();
                Style_af59b8b24edb44f68b94ec0141a5f51e.TargetType = typeof(global::System.Windows.Controls.AutoCompleteBox);
                var Setter_e69866ca86c34f37b4f419d5c26ae8d4 = new global::System.Windows.Setter();
                Setter_e69866ca86c34f37b4f419d5c26ae8d4.Property = global::System.Windows.Controls.AutoCompleteBox.MinimumPrefixLengthProperty;
                Setter_e69866ca86c34f37b4f419d5c26ae8d4.Value = (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"1");

                var Setter_4949c23d17b5462eb75a1aa14bdd2f6d = new global::System.Windows.Setter();
                Setter_4949c23d17b5462eb75a1aa14bdd2f6d.Property = global::System.Windows.Controls.AutoCompleteBox.MinimumPopulateDelayProperty;
                Setter_4949c23d17b5462eb75a1aa14bdd2f6d.Value = (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"0");

                var Setter_d6fcde97b1914395a9d6b1fd2022eb59 = new global::System.Windows.Setter();
                Setter_d6fcde97b1914395a9d6b1fd2022eb59.Property = global::System.Windows.Controls.AutoCompleteBox.BackgroundProperty;
                Setter_d6fcde97b1914395a9d6b1fd2022eb59.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"White");

                var Setter_2724d7ba6d594225ba718da3882218d1 = new global::System.Windows.Setter();
                Setter_2724d7ba6d594225ba718da3882218d1.Property = global::System.Windows.Controls.AutoCompleteBox.ForegroundProperty;
                Setter_2724d7ba6d594225ba718da3882218d1.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Black");

                var Setter_f7c3b246a22a48a1b1aa80e50cb2e950 = new global::System.Windows.Setter();
                Setter_f7c3b246a22a48a1b1aa80e50cb2e950.Property = global::System.Windows.Controls.AutoCompleteBox.PaddingProperty;
                Setter_f7c3b246a22a48a1b1aa80e50cb2e950.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"6,2,30,2");

                var Setter_6530d27d6cc54cc6b2bf39db55a9f7fb = new global::System.Windows.Setter();
                Setter_6530d27d6cc54cc6b2bf39db55a9f7fb.Property = global::System.Windows.Controls.AutoCompleteBox.CursorProperty;
                Setter_6530d27d6cc54cc6b2bf39db55a9f7fb.Value = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Hand");

                var Setter_882112aac2e44c16b8cfcde04868f57c = new global::System.Windows.Setter();
                Setter_882112aac2e44c16b8cfcde04868f57c.Property = global::System.Windows.Controls.AutoCompleteBox.TemplateProperty;
                var ControlTemplate_3c33b65970c541478d70fb65c9715b36 = new global::System.Windows.Controls.ControlTemplate();
                ControlTemplate_3c33b65970c541478d70fb65c9715b36.TargetType = typeof(global::System.Windows.Controls.AutoCompleteBox);
                ControlTemplate_3c33b65970c541478d70fb65c9715b36.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_3c33b65970c541478d70fb65c9715b36);

                Setter_882112aac2e44c16b8cfcde04868f57c.Value = ControlTemplate_3c33b65970c541478d70fb65c9715b36;


                Style_af59b8b24edb44f68b94ec0141a5f51e.Setters.Add(Setter_e69866ca86c34f37b4f419d5c26ae8d4);
                Style_af59b8b24edb44f68b94ec0141a5f51e.Setters.Add(Setter_4949c23d17b5462eb75a1aa14bdd2f6d);
                Style_af59b8b24edb44f68b94ec0141a5f51e.Setters.Add(Setter_d6fcde97b1914395a9d6b1fd2022eb59);
                Style_af59b8b24edb44f68b94ec0141a5f51e.Setters.Add(Setter_2724d7ba6d594225ba718da3882218d1);
                Style_af59b8b24edb44f68b94ec0141a5f51e.Setters.Add(Setter_f7c3b246a22a48a1b1aa80e50cb2e950);
                Style_af59b8b24edb44f68b94ec0141a5f51e.Setters.Add(Setter_6530d27d6cc54cc6b2bf39db55a9f7fb);
                Style_af59b8b24edb44f68b94ec0141a5f51e.Setters.Add(Setter_882112aac2e44c16b8cfcde04868f57c);


                DefaultStyle = Style_af59b8b24edb44f68b94ec0141a5f51e;
            }

            return DefaultStyle;
        }

        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_3c33b65970c541478d70fb65c9715b36(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_9cf6f4031e244e378fbd44b6da78d542 = new global::System.Windows.TemplateInstance();
            templateInstance_9cf6f4031e244e378fbd44b6da78d542.TemplateOwner = templateOwner;
            var StackPanel_285680a0177a41e79eff544367e580f4 = new global::System.Windows.Controls.StackPanel();

            var Border_27d65d57cd084f3eb3867bd4318fbc3d = new global::System.Windows.Controls.Border();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("OuterBorder", Border_27d65d57cd084f3eb3867bd4318fbc3d);
            Border_27d65d57cd084f3eb3867bd4318fbc3d.Name = "OuterBorder";
            var Grid_702cfd4eccf44d1ab3820e53ffaf7a02 = new global::System.Windows.Controls.Grid();
            var ToggleButton_25d231cfe9064899839e7f5938a42ad4 = new global::System.Windows.Controls.Primitives.ToggleButton();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("DropDownToggle", ToggleButton_25d231cfe9064899839e7f5938a42ad4);
            ToggleButton_25d231cfe9064899839e7f5938a42ad4.Name = "DropDownToggle";
            ToggleButton_25d231cfe9064899839e7f5938a42ad4.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Stretch;
            ToggleButton_25d231cfe9064899839e7f5938a42ad4.VerticalAlignment = global::System.Windows.VerticalAlignment.Stretch;
            ToggleButton_25d231cfe9064899839e7f5938a42ad4.HorizontalContentAlignment = global::System.Windows.HorizontalAlignment.Right;
            ToggleButton_25d231cfe9064899839e7f5938a42ad4.VerticalContentAlignment = global::System.Windows.VerticalAlignment.Center;
            ToggleButton_25d231cfe9064899839e7f5938a42ad4.Visibility = global::System.Windows.Visibility.Collapsed;
            var Path_c434a408d2ad4c19b543923c76b79332 = new global::System.Windows.Shapes.Path();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("arrow", Path_c434a408d2ad4c19b543923c76b79332);
            Path_c434a408d2ad4c19b543923c76b79332.Name = "arrow";
            Path_c434a408d2ad4c19b543923c76b79332.Visibility = global::System.Windows.Visibility.Visible;
            Path_c434a408d2ad4c19b543923c76b79332.Stroke = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Black");
            Path_c434a408d2ad4c19b543923c76b79332.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"13");
            Path_c434a408d2ad4c19b543923c76b79332.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_c434a408d2ad4c19b543923c76b79332.StrokeThickness = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"3");
            Path_c434a408d2ad4c19b543923c76b79332.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Center;
            Path_c434a408d2ad4c19b543923c76b79332.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
            Path_c434a408d2ad4c19b543923c76b79332.Stretch = global::System.Windows.Media.Stretch.Fill;
            Path_c434a408d2ad4c19b543923c76b79332.Data = (global::System.Windows.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Geometry), @"M 1,1.5 L 4.5,5 L 8,1.5");

            ToggleButton_25d231cfe9064899839e7f5938a42ad4.Content = Path_c434a408d2ad4c19b543923c76b79332;


            var TextBox_d5d5e82862824f008dcde072fa2252aa = new global::System.Windows.Controls.TextBox();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("PART_TextBox", TextBox_d5d5e82862824f008dcde072fa2252aa);
            TextBox_d5d5e82862824f008dcde072fa2252aa.Name = "PART_TextBox";
            var Binding_f559f93a7d0942d2a17d88186096d106 = new global::System.Windows.Data.Binding();
            Binding_f559f93a7d0942d2a17d88186096d106.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Text");
            Binding_f559f93a7d0942d2a17d88186096d106.Mode = global::System.Windows.Data.BindingMode.TwoWay;
            var RelativeSource_6038741616a04005933efbd77422536d = new global::System.Windows.Data.RelativeSource();
            RelativeSource_6038741616a04005933efbd77422536d.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_f559f93a7d0942d2a17d88186096d106.RelativeSource = RelativeSource_6038741616a04005933efbd77422536d;


            Binding_f559f93a7d0942d2a17d88186096d106.TemplateOwner = templateInstance_9cf6f4031e244e378fbd44b6da78d542;


            Grid_702cfd4eccf44d1ab3820e53ffaf7a02.Children.Add(ToggleButton_25d231cfe9064899839e7f5938a42ad4);
            Grid_702cfd4eccf44d1ab3820e53ffaf7a02.Children.Add(TextBox_d5d5e82862824f008dcde072fa2252aa);


            Border_27d65d57cd084f3eb3867bd4318fbc3d.Child = Grid_702cfd4eccf44d1ab3820e53ffaf7a02;

            var Binding_3c42e9f9a63640b7af3b2f5a4baae5bf = new global::System.Windows.Data.Binding();
            Binding_3c42e9f9a63640b7af3b2f5a4baae5bf.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_59b93c1a74a943f887c925b1e15be11a = new global::System.Windows.Data.RelativeSource();
            RelativeSource_59b93c1a74a943f887c925b1e15be11a.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_3c42e9f9a63640b7af3b2f5a4baae5bf.RelativeSource = RelativeSource_59b93c1a74a943f887c925b1e15be11a;


            Binding_3c42e9f9a63640b7af3b2f5a4baae5bf.TemplateOwner = templateInstance_9cf6f4031e244e378fbd44b6da78d542;

            var Binding_770feaae588b4551b205a60e41588f10 = new global::System.Windows.Data.Binding();
            Binding_770feaae588b4551b205a60e41588f10.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
            var RelativeSource_cc16e563a9c64ac1a2ec1de25864bdee = new global::System.Windows.Data.RelativeSource();
            RelativeSource_cc16e563a9c64ac1a2ec1de25864bdee.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_770feaae588b4551b205a60e41588f10.RelativeSource = RelativeSource_cc16e563a9c64ac1a2ec1de25864bdee;


            Binding_770feaae588b4551b205a60e41588f10.TemplateOwner = templateInstance_9cf6f4031e244e378fbd44b6da78d542;

            var Binding_d915b91d4f76440489a36543faa32878 = new global::System.Windows.Data.Binding();
            Binding_d915b91d4f76440489a36543faa32878.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
            var RelativeSource_87f66a27501e481883df26e83d12a020 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_87f66a27501e481883df26e83d12a020.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_d915b91d4f76440489a36543faa32878.RelativeSource = RelativeSource_87f66a27501e481883df26e83d12a020;


            Binding_d915b91d4f76440489a36543faa32878.TemplateOwner = templateInstance_9cf6f4031e244e378fbd44b6da78d542;


            var Popup_7e1c46b466c54030be7cc857807fe2fd = new global::System.Windows.Controls.Primitives.Popup();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Popup", Popup_7e1c46b466c54030be7cc857807fe2fd);
            Popup_7e1c46b466c54030be7cc857807fe2fd.Name = "Popup";
            Popup_7e1c46b466c54030be7cc857807fe2fd.IsOpen = (global::System.Boolean)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Boolean), @"False");
            var Border_e86fd73fc01c4bfd8d7e2c3598b8197d = new global::System.Windows.Controls.Border();
            Border_e86fd73fc01c4bfd8d7e2c3598b8197d.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"White");
            Border_e86fd73fc01c4bfd8d7e2c3598b8197d.BorderBrush = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"Black");
            Border_e86fd73fc01c4bfd8d7e2c3598b8197d.BorderThickness = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1");
            var ScrollViewer_febc1aea8aab4404a2899bc90468013e = new global::System.Windows.Controls.ScrollViewer();
            ScrollViewer_febc1aea8aab4404a2899bc90468013e.VerticalScrollBarVisibility = global::System.Windows.Controls.ScrollBarVisibility.Auto;
            ScrollViewer_febc1aea8aab4404a2899bc90468013e.HorizontalScrollBarVisibility = global::System.Windows.Controls.ScrollBarVisibility.Disabled;
            var ItemsPresenter_477893b8306546b7a300d0eb6d5278fe = new global::System.Windows.Controls.ItemsPresenter();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("ItemsHost", ItemsPresenter_477893b8306546b7a300d0eb6d5278fe);
            ItemsPresenter_477893b8306546b7a300d0eb6d5278fe.Name = "ItemsHost";

            ScrollViewer_febc1aea8aab4404a2899bc90468013e.Content = ItemsPresenter_477893b8306546b7a300d0eb6d5278fe;

            var Binding_a4e0a7bdc9404505a5305270c56d9f6c = new global::System.Windows.Data.Binding();
            Binding_a4e0a7bdc9404505a5305270c56d9f6c.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"MaxDropDownHeight");
            var RelativeSource_5e5e6d1ad4bd425596aca461ef4fa564 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_5e5e6d1ad4bd425596aca461ef4fa564.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_a4e0a7bdc9404505a5305270c56d9f6c.RelativeSource = RelativeSource_5e5e6d1ad4bd425596aca461ef4fa564;


            Binding_a4e0a7bdc9404505a5305270c56d9f6c.TemplateOwner = templateInstance_9cf6f4031e244e378fbd44b6da78d542;


            Border_e86fd73fc01c4bfd8d7e2c3598b8197d.Child = ScrollViewer_febc1aea8aab4404a2899bc90468013e;


            Popup_7e1c46b466c54030be7cc857807fe2fd.Child = Border_e86fd73fc01c4bfd8d7e2c3598b8197d;


            StackPanel_285680a0177a41e79eff544367e580f4.Children.Add(Border_27d65d57cd084f3eb3867bd4318fbc3d);
            StackPanel_285680a0177a41e79eff544367e580f4.Children.Add(Popup_7e1c46b466c54030be7cc857807fe2fd);



            TextBox_d5d5e82862824f008dcde072fa2252aa.SetBinding(global::System.Windows.Controls.TextBox.TextProperty, Binding_f559f93a7d0942d2a17d88186096d106);
            Border_27d65d57cd084f3eb3867bd4318fbc3d.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_3c42e9f9a63640b7af3b2f5a4baae5bf);
            Border_27d65d57cd084f3eb3867bd4318fbc3d.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_770feaae588b4551b205a60e41588f10);
            Border_27d65d57cd084f3eb3867bd4318fbc3d.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_d915b91d4f76440489a36543faa32878);
            ScrollViewer_febc1aea8aab4404a2899bc90468013e.SetBinding(global::System.Windows.FrameworkElement.MaxHeightProperty, Binding_a4e0a7bdc9404505a5305270c56d9f6c);

            templateInstance_9cf6f4031e244e378fbd44b6da78d542.TemplateContent = StackPanel_285680a0177a41e79eff544367e580f4;
            return templateInstance_9cf6f4031e244e378fbd44b6da78d542;
        }
    }
}
#else
namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultAutoCompleteBoxStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_af59b8b24edb44f68b94ec0141a5f51e = new global::Windows.UI.Xaml.Style();
                Style_af59b8b24edb44f68b94ec0141a5f51e.TargetType = typeof(global::Windows.UI.Xaml.Controls.AutoCompleteBox);
                var Setter_e69866ca86c34f37b4f419d5c26ae8d4 = new global::Windows.UI.Xaml.Setter();
                Setter_e69866ca86c34f37b4f419d5c26ae8d4.Property = global::Windows.UI.Xaml.Controls.AutoCompleteBox.MinimumPrefixLengthProperty;
                Setter_e69866ca86c34f37b4f419d5c26ae8d4.Value = (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"1");

                var Setter_4949c23d17b5462eb75a1aa14bdd2f6d = new global::Windows.UI.Xaml.Setter();
                Setter_4949c23d17b5462eb75a1aa14bdd2f6d.Property = global::Windows.UI.Xaml.Controls.AutoCompleteBox.MinimumPopulateDelayProperty;
                Setter_4949c23d17b5462eb75a1aa14bdd2f6d.Value = (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"0");

                var Setter_d6fcde97b1914395a9d6b1fd2022eb59 = new global::Windows.UI.Xaml.Setter();
                Setter_d6fcde97b1914395a9d6b1fd2022eb59.Property = global::Windows.UI.Xaml.Controls.AutoCompleteBox.BackgroundProperty;
                Setter_d6fcde97b1914395a9d6b1fd2022eb59.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"White");

                var Setter_2724d7ba6d594225ba718da3882218d1 = new global::Windows.UI.Xaml.Setter();
                Setter_2724d7ba6d594225ba718da3882218d1.Property = global::Windows.UI.Xaml.Controls.AutoCompleteBox.ForegroundProperty;
                Setter_2724d7ba6d594225ba718da3882218d1.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Black");

                var Setter_f7c3b246a22a48a1b1aa80e50cb2e950 = new global::Windows.UI.Xaml.Setter();
                Setter_f7c3b246a22a48a1b1aa80e50cb2e950.Property = global::Windows.UI.Xaml.Controls.AutoCompleteBox.PaddingProperty;
                Setter_f7c3b246a22a48a1b1aa80e50cb2e950.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"6,2,30,2");

                var Setter_6530d27d6cc54cc6b2bf39db55a9f7fb = new global::Windows.UI.Xaml.Setter();
                Setter_6530d27d6cc54cc6b2bf39db55a9f7fb.Property = global::Windows.UI.Xaml.Controls.AutoCompleteBox.CursorProperty;
                Setter_6530d27d6cc54cc6b2bf39db55a9f7fb.Value = (global::System.Windows.Input.Cursor)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Input.Cursor), @"Hand");

                var Setter_882112aac2e44c16b8cfcde04868f57c = new global::Windows.UI.Xaml.Setter();
                Setter_882112aac2e44c16b8cfcde04868f57c.Property = global::Windows.UI.Xaml.Controls.AutoCompleteBox.TemplateProperty;
                var ControlTemplate_3c33b65970c541478d70fb65c9715b36 = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_3c33b65970c541478d70fb65c9715b36.TargetType = typeof(global::Windows.UI.Xaml.Controls.AutoCompleteBox);
                ControlTemplate_3c33b65970c541478d70fb65c9715b36.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_3c33b65970c541478d70fb65c9715b36);

                Setter_882112aac2e44c16b8cfcde04868f57c.Value = ControlTemplate_3c33b65970c541478d70fb65c9715b36;


                Style_af59b8b24edb44f68b94ec0141a5f51e.Setters.Add(Setter_e69866ca86c34f37b4f419d5c26ae8d4);
                Style_af59b8b24edb44f68b94ec0141a5f51e.Setters.Add(Setter_4949c23d17b5462eb75a1aa14bdd2f6d);
                Style_af59b8b24edb44f68b94ec0141a5f51e.Setters.Add(Setter_d6fcde97b1914395a9d6b1fd2022eb59);
                Style_af59b8b24edb44f68b94ec0141a5f51e.Setters.Add(Setter_2724d7ba6d594225ba718da3882218d1);
                Style_af59b8b24edb44f68b94ec0141a5f51e.Setters.Add(Setter_f7c3b246a22a48a1b1aa80e50cb2e950);
                Style_af59b8b24edb44f68b94ec0141a5f51e.Setters.Add(Setter_6530d27d6cc54cc6b2bf39db55a9f7fb);
                Style_af59b8b24edb44f68b94ec0141a5f51e.Setters.Add(Setter_882112aac2e44c16b8cfcde04868f57c);


                DefaultStyle = Style_af59b8b24edb44f68b94ec0141a5f51e;
            }

            return DefaultStyle;
        }

        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_3c33b65970c541478d70fb65c9715b36(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_9cf6f4031e244e378fbd44b6da78d542 = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_9cf6f4031e244e378fbd44b6da78d542.TemplateOwner = templateOwner;
            var StackPanel_285680a0177a41e79eff544367e580f4 = new global::Windows.UI.Xaml.Controls.StackPanel();

            var Border_27d65d57cd084f3eb3867bd4318fbc3d = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("OuterBorder", Border_27d65d57cd084f3eb3867bd4318fbc3d);
            Border_27d65d57cd084f3eb3867bd4318fbc3d.Name = "OuterBorder";
            var Grid_702cfd4eccf44d1ab3820e53ffaf7a02 = new global::Windows.UI.Xaml.Controls.Grid();
            var ToggleButton_25d231cfe9064899839e7f5938a42ad4 = new global::Windows.UI.Xaml.Controls.Primitives.ToggleButton();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("DropDownToggle", ToggleButton_25d231cfe9064899839e7f5938a42ad4);
            ToggleButton_25d231cfe9064899839e7f5938a42ad4.Name = "DropDownToggle";
            ToggleButton_25d231cfe9064899839e7f5938a42ad4.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch;
            ToggleButton_25d231cfe9064899839e7f5938a42ad4.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Stretch;
            ToggleButton_25d231cfe9064899839e7f5938a42ad4.HorizontalContentAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Right;
            ToggleButton_25d231cfe9064899839e7f5938a42ad4.VerticalContentAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            ToggleButton_25d231cfe9064899839e7f5938a42ad4.Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
            var Path_c434a408d2ad4c19b543923c76b79332 = new global::Windows.UI.Xaml.Shapes.Path();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("arrow", Path_c434a408d2ad4c19b543923c76b79332);
            Path_c434a408d2ad4c19b543923c76b79332.Name = "arrow";
            Path_c434a408d2ad4c19b543923c76b79332.Visibility = global::Windows.UI.Xaml.Visibility.Visible;
            Path_c434a408d2ad4c19b543923c76b79332.Stroke = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Black");
            Path_c434a408d2ad4c19b543923c76b79332.Width = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"13");
            Path_c434a408d2ad4c19b543923c76b79332.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"9");
            Path_c434a408d2ad4c19b543923c76b79332.StrokeThickness = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"3");
            Path_c434a408d2ad4c19b543923c76b79332.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center;
            Path_c434a408d2ad4c19b543923c76b79332.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            Path_c434a408d2ad4c19b543923c76b79332.Stretch = global::Windows.UI.Xaml.Media.Stretch.Fill;
            Path_c434a408d2ad4c19b543923c76b79332.Data = (global::Windows.UI.Xaml.Media.Geometry)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Geometry), @"M 1,1.5 L 4.5,5 L 8,1.5");

            ToggleButton_25d231cfe9064899839e7f5938a42ad4.Content = Path_c434a408d2ad4c19b543923c76b79332;


            var TextBox_d5d5e82862824f008dcde072fa2252aa = new global::Windows.UI.Xaml.Controls.TextBox();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("PART_TextBox", TextBox_d5d5e82862824f008dcde072fa2252aa);
            TextBox_d5d5e82862824f008dcde072fa2252aa.Name = "PART_TextBox";
            var Binding_f559f93a7d0942d2a17d88186096d106 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_f559f93a7d0942d2a17d88186096d106.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Text");
            Binding_f559f93a7d0942d2a17d88186096d106.Mode = global::Windows.UI.Xaml.Data.BindingMode.TwoWay;
            var RelativeSource_6038741616a04005933efbd77422536d = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_6038741616a04005933efbd77422536d.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_f559f93a7d0942d2a17d88186096d106.RelativeSource = RelativeSource_6038741616a04005933efbd77422536d;


            Binding_f559f93a7d0942d2a17d88186096d106.TemplateOwner = templateInstance_9cf6f4031e244e378fbd44b6da78d542;


            Grid_702cfd4eccf44d1ab3820e53ffaf7a02.Children.Add(ToggleButton_25d231cfe9064899839e7f5938a42ad4);
            Grid_702cfd4eccf44d1ab3820e53ffaf7a02.Children.Add(TextBox_d5d5e82862824f008dcde072fa2252aa);


            Border_27d65d57cd084f3eb3867bd4318fbc3d.Child = Grid_702cfd4eccf44d1ab3820e53ffaf7a02;

            var Binding_3c42e9f9a63640b7af3b2f5a4baae5bf = new global::Windows.UI.Xaml.Data.Binding();
            Binding_3c42e9f9a63640b7af3b2f5a4baae5bf.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_59b93c1a74a943f887c925b1e15be11a = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_59b93c1a74a943f887c925b1e15be11a.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_3c42e9f9a63640b7af3b2f5a4baae5bf.RelativeSource = RelativeSource_59b93c1a74a943f887c925b1e15be11a;


            Binding_3c42e9f9a63640b7af3b2f5a4baae5bf.TemplateOwner = templateInstance_9cf6f4031e244e378fbd44b6da78d542;

            var Binding_770feaae588b4551b205a60e41588f10 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_770feaae588b4551b205a60e41588f10.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_cc16e563a9c64ac1a2ec1de25864bdee = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_cc16e563a9c64ac1a2ec1de25864bdee.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_770feaae588b4551b205a60e41588f10.RelativeSource = RelativeSource_cc16e563a9c64ac1a2ec1de25864bdee;


            Binding_770feaae588b4551b205a60e41588f10.TemplateOwner = templateInstance_9cf6f4031e244e378fbd44b6da78d542;

            var Binding_d915b91d4f76440489a36543faa32878 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_d915b91d4f76440489a36543faa32878.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_87f66a27501e481883df26e83d12a020 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_87f66a27501e481883df26e83d12a020.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_d915b91d4f76440489a36543faa32878.RelativeSource = RelativeSource_87f66a27501e481883df26e83d12a020;


            Binding_d915b91d4f76440489a36543faa32878.TemplateOwner = templateInstance_9cf6f4031e244e378fbd44b6da78d542;


            var Popup_7e1c46b466c54030be7cc857807fe2fd = new global::Windows.UI.Xaml.Controls.Primitives.Popup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Popup", Popup_7e1c46b466c54030be7cc857807fe2fd);
            Popup_7e1c46b466c54030be7cc857807fe2fd.Name = "Popup";
            Popup_7e1c46b466c54030be7cc857807fe2fd.IsOpen = (global::System.Boolean)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Boolean), @"False");
            var Border_e86fd73fc01c4bfd8d7e2c3598b8197d = new global::Windows.UI.Xaml.Controls.Border();
            Border_e86fd73fc01c4bfd8d7e2c3598b8197d.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"White");
            Border_e86fd73fc01c4bfd8d7e2c3598b8197d.BorderBrush = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"Black");
            Border_e86fd73fc01c4bfd8d7e2c3598b8197d.BorderThickness = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1");
            var ScrollViewer_febc1aea8aab4404a2899bc90468013e = new global::Windows.UI.Xaml.Controls.ScrollViewer();
            ScrollViewer_febc1aea8aab4404a2899bc90468013e.VerticalScrollBarVisibility = global::Windows.UI.Xaml.Controls.ScrollBarVisibility.Auto;
            ScrollViewer_febc1aea8aab4404a2899bc90468013e.HorizontalScrollBarVisibility = global::Windows.UI.Xaml.Controls.ScrollBarVisibility.Disabled;
            var ItemsPresenter_477893b8306546b7a300d0eb6d5278fe = new global::Windows.UI.Xaml.Controls.ItemsPresenter();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ItemsHost", ItemsPresenter_477893b8306546b7a300d0eb6d5278fe);
            ItemsPresenter_477893b8306546b7a300d0eb6d5278fe.Name = "ItemsHost";

            ScrollViewer_febc1aea8aab4404a2899bc90468013e.Content = ItemsPresenter_477893b8306546b7a300d0eb6d5278fe;

            var Binding_a4e0a7bdc9404505a5305270c56d9f6c = new global::Windows.UI.Xaml.Data.Binding();
            Binding_a4e0a7bdc9404505a5305270c56d9f6c.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"MaxDropDownHeight");
            var RelativeSource_5e5e6d1ad4bd425596aca461ef4fa564 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_5e5e6d1ad4bd425596aca461ef4fa564.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_a4e0a7bdc9404505a5305270c56d9f6c.RelativeSource = RelativeSource_5e5e6d1ad4bd425596aca461ef4fa564;


            Binding_a4e0a7bdc9404505a5305270c56d9f6c.TemplateOwner = templateInstance_9cf6f4031e244e378fbd44b6da78d542;


            Border_e86fd73fc01c4bfd8d7e2c3598b8197d.Child = ScrollViewer_febc1aea8aab4404a2899bc90468013e;


            Popup_7e1c46b466c54030be7cc857807fe2fd.Child = Border_e86fd73fc01c4bfd8d7e2c3598b8197d;


            StackPanel_285680a0177a41e79eff544367e580f4.Children.Add(Border_27d65d57cd084f3eb3867bd4318fbc3d);
            StackPanel_285680a0177a41e79eff544367e580f4.Children.Add(Popup_7e1c46b466c54030be7cc857807fe2fd);



            TextBox_d5d5e82862824f008dcde072fa2252aa.SetBinding(global::Windows.UI.Xaml.Controls.TextBox.TextProperty, Binding_f559f93a7d0942d2a17d88186096d106);
            Border_27d65d57cd084f3eb3867bd4318fbc3d.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_3c42e9f9a63640b7af3b2f5a4baae5bf);
            Border_27d65d57cd084f3eb3867bd4318fbc3d.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_770feaae588b4551b205a60e41588f10);
            Border_27d65d57cd084f3eb3867bd4318fbc3d.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_d915b91d4f76440489a36543faa32878);
            ScrollViewer_febc1aea8aab4404a2899bc90468013e.SetBinding(global::Windows.UI.Xaml.FrameworkElement.MaxHeightProperty, Binding_a4e0a7bdc9404505a5305270c56d9f6c);

            templateInstance_9cf6f4031e244e378fbd44b6da78d542.TemplateContent = StackPanel_285680a0177a41e79eff544367e580f4;
            return templateInstance_9cf6f4031e244e378fbd44b6da78d542;
        }
    }
}
#endif