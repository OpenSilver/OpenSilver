
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
    internal class INTERNAL_DefaultTabControlStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_e2e69fba84224bbea908fc8a24de3fda = new global::System.Windows.Style();
                Style_e2e69fba84224bbea908fc8a24de3fda.TargetType = typeof(global::System.Windows.Controls.TabControl);
                var Setter_2c089c57f48d4eb9b881333bbed6cbb1 = new global::System.Windows.Setter();
                Setter_2c089c57f48d4eb9b881333bbed6cbb1.Property = global::System.Windows.Controls.TabControl.BackgroundProperty;
                Setter_2c089c57f48d4eb9b881333bbed6cbb1.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"White");

                var Setter_9bf85328dccc4bdb9ec491dcdce1d93f = new global::System.Windows.Setter();
                Setter_9bf85328dccc4bdb9ec491dcdce1d93f.Property = global::System.Windows.Controls.TabControl.BorderBrushProperty;
                Setter_9bf85328dccc4bdb9ec491dcdce1d93f.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFDDDDDD");

                var Setter_d5707bd23d264212a876e46b90456e33 = new global::System.Windows.Setter();
                Setter_d5707bd23d264212a876e46b90456e33.Property = global::System.Windows.Controls.TabControl.BorderThicknessProperty;
                Setter_d5707bd23d264212a876e46b90456e33.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"1,1,1,1");

                var Setter_67000aa5c0874a1ba8872d498c7d4a23 = new global::System.Windows.Setter();
                Setter_67000aa5c0874a1ba8872d498c7d4a23.Property = global::System.Windows.Controls.TabControl.PaddingProperty;
                Setter_67000aa5c0874a1ba8872d498c7d4a23.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"5");

                var Setter_3e84010eb9d54a4ca1eb0436520c43ce = new global::System.Windows.Setter();
                Setter_3e84010eb9d54a4ca1eb0436520c43ce.Property = global::System.Windows.Controls.TabControl.HorizontalContentAlignmentProperty;
                Setter_3e84010eb9d54a4ca1eb0436520c43ce.Value = global::System.Windows.HorizontalAlignment.Stretch;

                var Setter_94a12d76762445418d2329563f5939dd = new global::System.Windows.Setter();
                Setter_94a12d76762445418d2329563f5939dd.Property = global::System.Windows.Controls.TabControl.VerticalContentAlignmentProperty;
                Setter_94a12d76762445418d2329563f5939dd.Value = global::System.Windows.VerticalAlignment.Stretch;

                var Setter_3841204e6d074f6abcc38553640d0ee1 = new global::System.Windows.Setter();
                Setter_3841204e6d074f6abcc38553640d0ee1.Property = global::System.Windows.Controls.TabControl.TemplateProperty;
                var ControlTemplate_f285ec007d6749e58d03a0ffe7f13fee = new global::System.Windows.Controls.ControlTemplate();
                ControlTemplate_f285ec007d6749e58d03a0ffe7f13fee.TargetType = typeof(global::System.Windows.Controls.TabControl);
                ControlTemplate_f285ec007d6749e58d03a0ffe7f13fee.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_f285ec007d6749e58d03a0ffe7f13fee);

                Setter_3841204e6d074f6abcc38553640d0ee1.Value = ControlTemplate_f285ec007d6749e58d03a0ffe7f13fee;


                Style_e2e69fba84224bbea908fc8a24de3fda.Setters.Add(Setter_2c089c57f48d4eb9b881333bbed6cbb1);
                Style_e2e69fba84224bbea908fc8a24de3fda.Setters.Add(Setter_9bf85328dccc4bdb9ec491dcdce1d93f);
                Style_e2e69fba84224bbea908fc8a24de3fda.Setters.Add(Setter_d5707bd23d264212a876e46b90456e33);
                Style_e2e69fba84224bbea908fc8a24de3fda.Setters.Add(Setter_67000aa5c0874a1ba8872d498c7d4a23);
                Style_e2e69fba84224bbea908fc8a24de3fda.Setters.Add(Setter_3e84010eb9d54a4ca1eb0436520c43ce);
                Style_e2e69fba84224bbea908fc8a24de3fda.Setters.Add(Setter_94a12d76762445418d2329563f5939dd);
                Style_e2e69fba84224bbea908fc8a24de3fda.Setters.Add(Setter_3841204e6d074f6abcc38553640d0ee1);


                DefaultStyle = Style_e2e69fba84224bbea908fc8a24de3fda;
            }

            return DefaultStyle;
        }


        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_f285ec007d6749e58d03a0ffe7f13fee(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_a842f28ff4464f0eb1bfd1953c493dcd = new global::System.Windows.TemplateInstance();
            templateInstance_a842f28ff4464f0eb1bfd1953c493dcd.TemplateOwner = templateOwner;
            var Border_21102c4e75c24d6abc620c79361ccf53 = new global::System.Windows.Controls.Border();
            var VisualStateGroup_55a3e723117b40b3b1477188904192d0 = new global::System.Windows.VisualStateGroup();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_55a3e723117b40b3b1477188904192d0);
            VisualStateGroup_55a3e723117b40b3b1477188904192d0.Name = "CommonStates";
            var VisualState_fcaf408c1b0543e5b4658598b417e760 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Normal", VisualState_fcaf408c1b0543e5b4658598b417e760);
            VisualState_fcaf408c1b0543e5b4658598b417e760.Name = "Normal";

            var VisualState_69e02d36ffaf4a12989af5a28e937ea3 = new global::System.Windows.VisualState();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_69e02d36ffaf4a12989af5a28e937ea3);
            VisualState_69e02d36ffaf4a12989af5a28e937ea3.Name = "Disabled";

            VisualStateGroup_55a3e723117b40b3b1477188904192d0.States.Add(VisualState_fcaf408c1b0543e5b4658598b417e760);
            VisualStateGroup_55a3e723117b40b3b1477188904192d0.States.Add(VisualState_69e02d36ffaf4a12989af5a28e937ea3);


            ((global::System.Windows.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_55a3e723117b40b3b1477188904192d0);

            var Grid_665e7e4dada840ed9e6a2276d3fb278f = new global::System.Windows.Controls.Grid();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("TemplateTop", Grid_665e7e4dada840ed9e6a2276d3fb278f);
            Grid_665e7e4dada840ed9e6a2276d3fb278f.Name = "TemplateTop";
            Grid_665e7e4dada840ed9e6a2276d3fb278f.Visibility = global::System.Windows.Visibility.Collapsed;
            var RowDefinition_59ecc101a2ab48b09caa081498c14023 = new global::System.Windows.Controls.RowDefinition();
            RowDefinition_59ecc101a2ab48b09caa081498c14023.Height = (global::System.Windows.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.GridLength), @"Auto");

            var RowDefinition_21f1184c4924495da23fc0f4175ca1dd = new global::System.Windows.Controls.RowDefinition();
            RowDefinition_21f1184c4924495da23fc0f4175ca1dd.Height = (global::System.Windows.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.GridLength), @"*");

            Grid_665e7e4dada840ed9e6a2276d3fb278f.RowDefinitions.Add(RowDefinition_59ecc101a2ab48b09caa081498c14023);
            Grid_665e7e4dada840ed9e6a2276d3fb278f.RowDefinitions.Add(RowDefinition_21f1184c4924495da23fc0f4175ca1dd);

            var TabPanel_c142a0c1693848da815d48b249fdf8a9 = new global::System.Windows.Controls.TabPanel();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("TabPanelTop", TabPanel_c142a0c1693848da815d48b249fdf8a9);
            TabPanel_c142a0c1693848da815d48b249fdf8a9.Name = "TabPanelTop";

            var Border_cfde29ecceb9463c8000dea4033be743 = new global::System.Windows.Controls.Border();
            global::System.Windows.Controls.Grid.SetRow(Border_cfde29ecceb9463c8000dea4033be743, (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"1"));
            Border_cfde29ecceb9463c8000dea4033be743.CornerRadius = (global::System.Windows.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.CornerRadius), @"0,0,3,3");
            var ContentPresenter_7b1d7d7ad4e34cd388586bd2cb70beda = new global::System.Windows.Controls.ContentPresenter();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("ContentTop", ContentPresenter_7b1d7d7ad4e34cd388586bd2cb70beda);
            ContentPresenter_7b1d7d7ad4e34cd388586bd2cb70beda.Name = "ContentTop";
            var Binding_d31bcf6b622d4225a726313d3fe90350 = new global::System.Windows.Data.Binding();
            Binding_d31bcf6b622d4225a726313d3fe90350.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"HorizontalContentAlignment");
            var RelativeSource_3049e3094ace4de58868dd822866a0fd = new global::System.Windows.Data.RelativeSource();
            RelativeSource_3049e3094ace4de58868dd822866a0fd.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_d31bcf6b622d4225a726313d3fe90350.RelativeSource = RelativeSource_3049e3094ace4de58868dd822866a0fd;


            Binding_d31bcf6b622d4225a726313d3fe90350.TemplateOwner = templateInstance_a842f28ff4464f0eb1bfd1953c493dcd;

            var Binding_de961e366f3048939400ea056dd906cd = new global::System.Windows.Data.Binding();
            Binding_de961e366f3048939400ea056dd906cd.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"VerticalContentAlignment");
            var RelativeSource_0c2898b35d9443e88740a57b382120b1 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_0c2898b35d9443e88740a57b382120b1.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_de961e366f3048939400ea056dd906cd.RelativeSource = RelativeSource_0c2898b35d9443e88740a57b382120b1;


            Binding_de961e366f3048939400ea056dd906cd.TemplateOwner = templateInstance_a842f28ff4464f0eb1bfd1953c493dcd;

            var Binding_0a124b01975849dd9ec0b043c8318fe9 = new global::System.Windows.Data.Binding();
            Binding_0a124b01975849dd9ec0b043c8318fe9.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Content");
            var RelativeSource_51876b7b25624c309cabfb5220566948 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_51876b7b25624c309cabfb5220566948.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_0a124b01975849dd9ec0b043c8318fe9.RelativeSource = RelativeSource_51876b7b25624c309cabfb5220566948;


            Binding_0a124b01975849dd9ec0b043c8318fe9.TemplateOwner = templateInstance_a842f28ff4464f0eb1bfd1953c493dcd;

            var Binding_07230075d28a4484b3faef8f63c5d361 = new global::System.Windows.Data.Binding();
            Binding_07230075d28a4484b3faef8f63c5d361.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"ContentTemplate");
            var RelativeSource_c58f2fc6fe504647bd1ad103281ff5e7 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_c58f2fc6fe504647bd1ad103281ff5e7.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_07230075d28a4484b3faef8f63c5d361.RelativeSource = RelativeSource_c58f2fc6fe504647bd1ad103281ff5e7;


            Binding_07230075d28a4484b3faef8f63c5d361.TemplateOwner = templateInstance_a842f28ff4464f0eb1bfd1953c493dcd;


            Border_cfde29ecceb9463c8000dea4033be743.Child = ContentPresenter_7b1d7d7ad4e34cd388586bd2cb70beda;

            var Binding_0030e8e2311f4894957c0180df81c8d8 = new global::System.Windows.Data.Binding();
            Binding_0030e8e2311f4894957c0180df81c8d8.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
            var RelativeSource_f04bb15302544998a038b6fb975d54b4 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_f04bb15302544998a038b6fb975d54b4.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_0030e8e2311f4894957c0180df81c8d8.RelativeSource = RelativeSource_f04bb15302544998a038b6fb975d54b4;


            Binding_0030e8e2311f4894957c0180df81c8d8.TemplateOwner = templateInstance_a842f28ff4464f0eb1bfd1953c493dcd;

            var Binding_c1899564200646b2ae83c187467331b7 = new global::System.Windows.Data.Binding();
            Binding_c1899564200646b2ae83c187467331b7.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
            var RelativeSource_63d2098a8eef4feeaf2baede8dcc2502 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_63d2098a8eef4feeaf2baede8dcc2502.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_c1899564200646b2ae83c187467331b7.RelativeSource = RelativeSource_63d2098a8eef4feeaf2baede8dcc2502;


            Binding_c1899564200646b2ae83c187467331b7.TemplateOwner = templateInstance_a842f28ff4464f0eb1bfd1953c493dcd;

            var Binding_707407ed2f16447c855170762e2dadc5 = new global::System.Windows.Data.Binding();
            Binding_707407ed2f16447c855170762e2dadc5.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_a3955f5e1eec458dbce39df5b9e3e8fa = new global::System.Windows.Data.RelativeSource();
            RelativeSource_a3955f5e1eec458dbce39df5b9e3e8fa.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_707407ed2f16447c855170762e2dadc5.RelativeSource = RelativeSource_a3955f5e1eec458dbce39df5b9e3e8fa;


            Binding_707407ed2f16447c855170762e2dadc5.TemplateOwner = templateInstance_a842f28ff4464f0eb1bfd1953c493dcd;


            Grid_665e7e4dada840ed9e6a2276d3fb278f.Children.Add(TabPanel_c142a0c1693848da815d48b249fdf8a9);
            Grid_665e7e4dada840ed9e6a2276d3fb278f.Children.Add(Border_cfde29ecceb9463c8000dea4033be743);


            Border_21102c4e75c24d6abc620c79361ccf53.Child = Grid_665e7e4dada840ed9e6a2276d3fb278f;



            ContentPresenter_7b1d7d7ad4e34cd388586bd2cb70beda.SetBinding(global::System.Windows.FrameworkElement.HorizontalAlignmentProperty, Binding_d31bcf6b622d4225a726313d3fe90350);
            ContentPresenter_7b1d7d7ad4e34cd388586bd2cb70beda.SetBinding(global::System.Windows.FrameworkElement.VerticalAlignmentProperty, Binding_de961e366f3048939400ea056dd906cd);
            ContentPresenter_7b1d7d7ad4e34cd388586bd2cb70beda.SetBinding(global::System.Windows.Controls.ContentPresenter.ContentProperty, Binding_0a124b01975849dd9ec0b043c8318fe9);
            ContentPresenter_7b1d7d7ad4e34cd388586bd2cb70beda.SetBinding(global::System.Windows.Controls.ContentPresenter.ContentTemplateProperty, Binding_07230075d28a4484b3faef8f63c5d361);
            Border_cfde29ecceb9463c8000dea4033be743.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_0030e8e2311f4894957c0180df81c8d8);
            Border_cfde29ecceb9463c8000dea4033be743.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_c1899564200646b2ae83c187467331b7);
            Border_cfde29ecceb9463c8000dea4033be743.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_707407ed2f16447c855170762e2dadc5);

            templateInstance_a842f28ff4464f0eb1bfd1953c493dcd.TemplateContent = Border_21102c4e75c24d6abc620c79361ccf53;
            return templateInstance_a842f28ff4464f0eb1bfd1953c493dcd;
        }
    }
}
#else
namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultTabControlStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_e2e69fba84224bbea908fc8a24de3fda = new global::Windows.UI.Xaml.Style();
                Style_e2e69fba84224bbea908fc8a24de3fda.TargetType = typeof(global::Windows.UI.Xaml.Controls.TabControl);
                var Setter_2c089c57f48d4eb9b881333bbed6cbb1 = new global::Windows.UI.Xaml.Setter();
                Setter_2c089c57f48d4eb9b881333bbed6cbb1.Property = global::Windows.UI.Xaml.Controls.TabControl.BackgroundProperty;
                Setter_2c089c57f48d4eb9b881333bbed6cbb1.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"White");

                var Setter_9bf85328dccc4bdb9ec491dcdce1d93f = new global::Windows.UI.Xaml.Setter();
                Setter_9bf85328dccc4bdb9ec491dcdce1d93f.Property = global::Windows.UI.Xaml.Controls.TabControl.BorderBrushProperty;
                Setter_9bf85328dccc4bdb9ec491dcdce1d93f.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFDDDDDD");

                var Setter_d5707bd23d264212a876e46b90456e33 = new global::Windows.UI.Xaml.Setter();
                Setter_d5707bd23d264212a876e46b90456e33.Property = global::Windows.UI.Xaml.Controls.TabControl.BorderThicknessProperty;
                Setter_d5707bd23d264212a876e46b90456e33.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"1,1,1,1");

                var Setter_67000aa5c0874a1ba8872d498c7d4a23 = new global::Windows.UI.Xaml.Setter();
                Setter_67000aa5c0874a1ba8872d498c7d4a23.Property = global::Windows.UI.Xaml.Controls.TabControl.PaddingProperty;
                Setter_67000aa5c0874a1ba8872d498c7d4a23.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"5");

                var Setter_3e84010eb9d54a4ca1eb0436520c43ce = new global::Windows.UI.Xaml.Setter();
                Setter_3e84010eb9d54a4ca1eb0436520c43ce.Property = global::Windows.UI.Xaml.Controls.TabControl.HorizontalContentAlignmentProperty;
                Setter_3e84010eb9d54a4ca1eb0436520c43ce.Value = global::Windows.UI.Xaml.HorizontalAlignment.Stretch;

                var Setter_94a12d76762445418d2329563f5939dd = new global::Windows.UI.Xaml.Setter();
                Setter_94a12d76762445418d2329563f5939dd.Property = global::Windows.UI.Xaml.Controls.TabControl.VerticalContentAlignmentProperty;
                Setter_94a12d76762445418d2329563f5939dd.Value = global::Windows.UI.Xaml.VerticalAlignment.Stretch;

                var Setter_3841204e6d074f6abcc38553640d0ee1 = new global::Windows.UI.Xaml.Setter();
                Setter_3841204e6d074f6abcc38553640d0ee1.Property = global::Windows.UI.Xaml.Controls.TabControl.TemplateProperty;
                var ControlTemplate_f285ec007d6749e58d03a0ffe7f13fee = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_f285ec007d6749e58d03a0ffe7f13fee.TargetType = typeof(global::Windows.UI.Xaml.Controls.TabControl);
                ControlTemplate_f285ec007d6749e58d03a0ffe7f13fee.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_f285ec007d6749e58d03a0ffe7f13fee);

                Setter_3841204e6d074f6abcc38553640d0ee1.Value = ControlTemplate_f285ec007d6749e58d03a0ffe7f13fee;


                Style_e2e69fba84224bbea908fc8a24de3fda.Setters.Add(Setter_2c089c57f48d4eb9b881333bbed6cbb1);
                Style_e2e69fba84224bbea908fc8a24de3fda.Setters.Add(Setter_9bf85328dccc4bdb9ec491dcdce1d93f);
                Style_e2e69fba84224bbea908fc8a24de3fda.Setters.Add(Setter_d5707bd23d264212a876e46b90456e33);
                Style_e2e69fba84224bbea908fc8a24de3fda.Setters.Add(Setter_67000aa5c0874a1ba8872d498c7d4a23);
                Style_e2e69fba84224bbea908fc8a24de3fda.Setters.Add(Setter_3e84010eb9d54a4ca1eb0436520c43ce);
                Style_e2e69fba84224bbea908fc8a24de3fda.Setters.Add(Setter_94a12d76762445418d2329563f5939dd);
                Style_e2e69fba84224bbea908fc8a24de3fda.Setters.Add(Setter_3841204e6d074f6abcc38553640d0ee1);


                DefaultStyle = Style_e2e69fba84224bbea908fc8a24de3fda;
            }

            return DefaultStyle;
        }


        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_f285ec007d6749e58d03a0ffe7f13fee(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_a842f28ff4464f0eb1bfd1953c493dcd = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_a842f28ff4464f0eb1bfd1953c493dcd.TemplateOwner = templateOwner;
            var Border_21102c4e75c24d6abc620c79361ccf53 = new global::Windows.UI.Xaml.Controls.Border();
            var VisualStateGroup_55a3e723117b40b3b1477188904192d0 = new global::Windows.UI.Xaml.VisualStateGroup();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CommonStates", VisualStateGroup_55a3e723117b40b3b1477188904192d0);
            VisualStateGroup_55a3e723117b40b3b1477188904192d0.Name = "CommonStates";
            var VisualState_fcaf408c1b0543e5b4658598b417e760 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Normal", VisualState_fcaf408c1b0543e5b4658598b417e760);
            VisualState_fcaf408c1b0543e5b4658598b417e760.Name = "Normal";

            var VisualState_69e02d36ffaf4a12989af5a28e937ea3 = new global::Windows.UI.Xaml.VisualState();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Disabled", VisualState_69e02d36ffaf4a12989af5a28e937ea3);
            VisualState_69e02d36ffaf4a12989af5a28e937ea3.Name = "Disabled";

            VisualStateGroup_55a3e723117b40b3b1477188904192d0.States.Add(VisualState_fcaf408c1b0543e5b4658598b417e760);
            VisualStateGroup_55a3e723117b40b3b1477188904192d0.States.Add(VisualState_69e02d36ffaf4a12989af5a28e937ea3);


            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).INTERNAL_GetVisualStateGroups().Add(VisualStateGroup_55a3e723117b40b3b1477188904192d0);

            var Grid_665e7e4dada840ed9e6a2276d3fb278f = new global::Windows.UI.Xaml.Controls.Grid();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("TemplateTop", Grid_665e7e4dada840ed9e6a2276d3fb278f);
            Grid_665e7e4dada840ed9e6a2276d3fb278f.Name = "TemplateTop";
            Grid_665e7e4dada840ed9e6a2276d3fb278f.Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
            var RowDefinition_59ecc101a2ab48b09caa081498c14023 = new global::Windows.UI.Xaml.Controls.RowDefinition();
            RowDefinition_59ecc101a2ab48b09caa081498c14023.Height = (global::Windows.UI.Xaml.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.GridLength), @"Auto");

            var RowDefinition_21f1184c4924495da23fc0f4175ca1dd = new global::Windows.UI.Xaml.Controls.RowDefinition();
            RowDefinition_21f1184c4924495da23fc0f4175ca1dd.Height = (global::Windows.UI.Xaml.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.GridLength), @"*");

            Grid_665e7e4dada840ed9e6a2276d3fb278f.RowDefinitions.Add(RowDefinition_59ecc101a2ab48b09caa081498c14023);
            Grid_665e7e4dada840ed9e6a2276d3fb278f.RowDefinitions.Add(RowDefinition_21f1184c4924495da23fc0f4175ca1dd);

            var TabPanel_c142a0c1693848da815d48b249fdf8a9 = new global::Windows.UI.Xaml.Controls.TabPanel();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("TabPanelTop", TabPanel_c142a0c1693848da815d48b249fdf8a9);
            TabPanel_c142a0c1693848da815d48b249fdf8a9.Name = "TabPanelTop";

            var Border_cfde29ecceb9463c8000dea4033be743 = new global::Windows.UI.Xaml.Controls.Border();
            global::Windows.UI.Xaml.Controls.Grid.SetRow(Border_cfde29ecceb9463c8000dea4033be743, (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"1"));
            Border_cfde29ecceb9463c8000dea4033be743.CornerRadius = (global::Windows.UI.Xaml.CornerRadius)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.CornerRadius), @"0,0,3,3");
            var ContentPresenter_7b1d7d7ad4e34cd388586bd2cb70beda = new global::Windows.UI.Xaml.Controls.ContentPresenter();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ContentTop", ContentPresenter_7b1d7d7ad4e34cd388586bd2cb70beda);
            ContentPresenter_7b1d7d7ad4e34cd388586bd2cb70beda.Name = "ContentTop";
            var Binding_d31bcf6b622d4225a726313d3fe90350 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_d31bcf6b622d4225a726313d3fe90350.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"HorizontalContentAlignment");
            var RelativeSource_3049e3094ace4de58868dd822866a0fd = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_3049e3094ace4de58868dd822866a0fd.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_d31bcf6b622d4225a726313d3fe90350.RelativeSource = RelativeSource_3049e3094ace4de58868dd822866a0fd;


            Binding_d31bcf6b622d4225a726313d3fe90350.TemplateOwner = templateInstance_a842f28ff4464f0eb1bfd1953c493dcd;

            var Binding_de961e366f3048939400ea056dd906cd = new global::Windows.UI.Xaml.Data.Binding();
            Binding_de961e366f3048939400ea056dd906cd.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"VerticalContentAlignment");
            var RelativeSource_0c2898b35d9443e88740a57b382120b1 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_0c2898b35d9443e88740a57b382120b1.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_de961e366f3048939400ea056dd906cd.RelativeSource = RelativeSource_0c2898b35d9443e88740a57b382120b1;


            Binding_de961e366f3048939400ea056dd906cd.TemplateOwner = templateInstance_a842f28ff4464f0eb1bfd1953c493dcd;

            var Binding_0a124b01975849dd9ec0b043c8318fe9 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_0a124b01975849dd9ec0b043c8318fe9.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Content");
            var RelativeSource_51876b7b25624c309cabfb5220566948 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_51876b7b25624c309cabfb5220566948.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_0a124b01975849dd9ec0b043c8318fe9.RelativeSource = RelativeSource_51876b7b25624c309cabfb5220566948;


            Binding_0a124b01975849dd9ec0b043c8318fe9.TemplateOwner = templateInstance_a842f28ff4464f0eb1bfd1953c493dcd;

            var Binding_07230075d28a4484b3faef8f63c5d361 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_07230075d28a4484b3faef8f63c5d361.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"ContentTemplate");
            var RelativeSource_c58f2fc6fe504647bd1ad103281ff5e7 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_c58f2fc6fe504647bd1ad103281ff5e7.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_07230075d28a4484b3faef8f63c5d361.RelativeSource = RelativeSource_c58f2fc6fe504647bd1ad103281ff5e7;


            Binding_07230075d28a4484b3faef8f63c5d361.TemplateOwner = templateInstance_a842f28ff4464f0eb1bfd1953c493dcd;


            Border_cfde29ecceb9463c8000dea4033be743.Child = ContentPresenter_7b1d7d7ad4e34cd388586bd2cb70beda;

            var Binding_0030e8e2311f4894957c0180df81c8d8 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_0030e8e2311f4894957c0180df81c8d8.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_f04bb15302544998a038b6fb975d54b4 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_f04bb15302544998a038b6fb975d54b4.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_0030e8e2311f4894957c0180df81c8d8.RelativeSource = RelativeSource_f04bb15302544998a038b6fb975d54b4;


            Binding_0030e8e2311f4894957c0180df81c8d8.TemplateOwner = templateInstance_a842f28ff4464f0eb1bfd1953c493dcd;

            var Binding_c1899564200646b2ae83c187467331b7 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_c1899564200646b2ae83c187467331b7.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_63d2098a8eef4feeaf2baede8dcc2502 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_63d2098a8eef4feeaf2baede8dcc2502.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_c1899564200646b2ae83c187467331b7.RelativeSource = RelativeSource_63d2098a8eef4feeaf2baede8dcc2502;


            Binding_c1899564200646b2ae83c187467331b7.TemplateOwner = templateInstance_a842f28ff4464f0eb1bfd1953c493dcd;

            var Binding_707407ed2f16447c855170762e2dadc5 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_707407ed2f16447c855170762e2dadc5.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_a3955f5e1eec458dbce39df5b9e3e8fa = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_a3955f5e1eec458dbce39df5b9e3e8fa.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_707407ed2f16447c855170762e2dadc5.RelativeSource = RelativeSource_a3955f5e1eec458dbce39df5b9e3e8fa;


            Binding_707407ed2f16447c855170762e2dadc5.TemplateOwner = templateInstance_a842f28ff4464f0eb1bfd1953c493dcd;


            Grid_665e7e4dada840ed9e6a2276d3fb278f.Children.Add(TabPanel_c142a0c1693848da815d48b249fdf8a9);
            Grid_665e7e4dada840ed9e6a2276d3fb278f.Children.Add(Border_cfde29ecceb9463c8000dea4033be743);


            Border_21102c4e75c24d6abc620c79361ccf53.Child = Grid_665e7e4dada840ed9e6a2276d3fb278f;



            ContentPresenter_7b1d7d7ad4e34cd388586bd2cb70beda.SetBinding(global::Windows.UI.Xaml.FrameworkElement.HorizontalAlignmentProperty, Binding_d31bcf6b622d4225a726313d3fe90350);
            ContentPresenter_7b1d7d7ad4e34cd388586bd2cb70beda.SetBinding(global::Windows.UI.Xaml.FrameworkElement.VerticalAlignmentProperty, Binding_de961e366f3048939400ea056dd906cd);
            ContentPresenter_7b1d7d7ad4e34cd388586bd2cb70beda.SetBinding(global::Windows.UI.Xaml.Controls.ContentPresenter.ContentProperty, Binding_0a124b01975849dd9ec0b043c8318fe9);
            ContentPresenter_7b1d7d7ad4e34cd388586bd2cb70beda.SetBinding(global::Windows.UI.Xaml.Controls.ContentPresenter.ContentTemplateProperty, Binding_07230075d28a4484b3faef8f63c5d361);
            Border_cfde29ecceb9463c8000dea4033be743.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_0030e8e2311f4894957c0180df81c8d8);
            Border_cfde29ecceb9463c8000dea4033be743.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_c1899564200646b2ae83c187467331b7);
            Border_cfde29ecceb9463c8000dea4033be743.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_707407ed2f16447c855170762e2dadc5);

            templateInstance_a842f28ff4464f0eb1bfd1953c493dcd.TemplateContent = Border_21102c4e75c24d6abc620c79361ccf53;
            return templateInstance_a842f28ff4464f0eb1bfd1953c493dcd;
        }
    }
}
#endif