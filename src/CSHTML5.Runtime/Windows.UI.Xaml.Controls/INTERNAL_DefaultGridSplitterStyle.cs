
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
    internal class INTERNAL_DefaultGridSplitterStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_c5c31696b72643cdba2270801e251155 = new global::System.Windows.Style();
                Style_c5c31696b72643cdba2270801e251155.TargetType = typeof(global::System.Windows.Controls.GridSplitter);
                var Setter_0e1932b8228e4144aefc35722e7b9b35 = new global::System.Windows.Setter();
                Setter_0e1932b8228e4144aefc35722e7b9b35.Property = global::System.Windows.Controls.GridSplitter.BackgroundProperty;
                Setter_0e1932b8228e4144aefc35722e7b9b35.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFF0F0F0");

                var Setter_3ef319c39e394dbd97ca9e37fbc51850 = new global::System.Windows.Setter();
                Setter_3ef319c39e394dbd97ca9e37fbc51850.Property = global::System.Windows.Controls.GridSplitter.TemplateProperty;
                var ControlTemplate_7ccfff71a0114b1295e3d943a9e6e0c7 = new global::System.Windows.Controls.ControlTemplate();
                ControlTemplate_7ccfff71a0114b1295e3d943a9e6e0c7.TargetType = typeof(global::System.Windows.Controls.GridSplitter);
                ControlTemplate_7ccfff71a0114b1295e3d943a9e6e0c7.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_7ccfff71a0114b1295e3d943a9e6e0c7);

                Setter_3ef319c39e394dbd97ca9e37fbc51850.Value = ControlTemplate_7ccfff71a0114b1295e3d943a9e6e0c7;


                Style_c5c31696b72643cdba2270801e251155.Setters.Add(Setter_0e1932b8228e4144aefc35722e7b9b35);
                Style_c5c31696b72643cdba2270801e251155.Setters.Add(Setter_3ef319c39e394dbd97ca9e37fbc51850);


                DefaultStyle = Style_c5c31696b72643cdba2270801e251155;

            }

            return DefaultStyle;
        }

        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_7ccfff71a0114b1295e3d943a9e6e0c7(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_8450f63a5d83462e8f83459f689dc38c = new global::System.Windows.TemplateInstance();
            templateInstance_8450f63a5d83462e8f83459f689dc38c.TemplateOwner = templateOwner;
            var Grid_7d5f63f475a54a5bbff239be7e8be75e = new global::System.Windows.Controls.Grid();
            var Thumb_6726f3e455d64fbab3cf89bb054990f8 = new global::System.Windows.Controls.Primitives.Thumb();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("PART_Thumb", Thumb_6726f3e455d64fbab3cf89bb054990f8);
            Thumb_6726f3e455d64fbab3cf89bb054990f8.Name = "PART_Thumb";
            Thumb_6726f3e455d64fbab3cf89bb054990f8.Opacity = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");

            var ContentPresenter_dad0417808c74b25ac4c5e5182a5992a = new global::System.Windows.Controls.ContentPresenter();
            ContentPresenter_dad0417808c74b25ac4c5e5182a5992a.IsHitTestVisible = (global::System.Boolean)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Boolean), @"False");
            var Binding_68e8976a53a44a37a63274c826a3281b = new global::System.Windows.Data.Binding();
            Binding_68e8976a53a44a37a63274c826a3281b.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Element");
            var RelativeSource_7dfbc1f104f2418e9ccf585fb2f3a82d = new global::System.Windows.Data.RelativeSource();
            RelativeSource_7dfbc1f104f2418e9ccf585fb2f3a82d.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_68e8976a53a44a37a63274c826a3281b.RelativeSource = RelativeSource_7dfbc1f104f2418e9ccf585fb2f3a82d;


            Binding_68e8976a53a44a37a63274c826a3281b.TemplateOwner = templateInstance_8450f63a5d83462e8f83459f689dc38c;

            var Binding_d0ae12447320407e8b2b25cb5fd88bcb = new global::System.Windows.Data.Binding();
            Binding_d0ae12447320407e8b2b25cb5fd88bcb.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"ContentTemplate");
            var RelativeSource_6c048aeffe8f46d4be81b962305286a7 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_6c048aeffe8f46d4be81b962305286a7.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_d0ae12447320407e8b2b25cb5fd88bcb.RelativeSource = RelativeSource_6c048aeffe8f46d4be81b962305286a7;


            Binding_d0ae12447320407e8b2b25cb5fd88bcb.TemplateOwner = templateInstance_8450f63a5d83462e8f83459f689dc38c;


            Grid_7d5f63f475a54a5bbff239be7e8be75e.Children.Add(Thumb_6726f3e455d64fbab3cf89bb054990f8);
            Grid_7d5f63f475a54a5bbff239be7e8be75e.Children.Add(ContentPresenter_dad0417808c74b25ac4c5e5182a5992a);

            var Binding_369df61b15e2483cb102f8860ba54d8d = new global::System.Windows.Data.Binding();
            Binding_369df61b15e2483cb102f8860ba54d8d.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
            var RelativeSource_841fdd2e411c415fb0cc3ece5a76c8ef = new global::System.Windows.Data.RelativeSource();
            RelativeSource_841fdd2e411c415fb0cc3ece5a76c8ef.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_369df61b15e2483cb102f8860ba54d8d.RelativeSource = RelativeSource_841fdd2e411c415fb0cc3ece5a76c8ef;


            Binding_369df61b15e2483cb102f8860ba54d8d.TemplateOwner = templateInstance_8450f63a5d83462e8f83459f689dc38c;



            ContentPresenter_dad0417808c74b25ac4c5e5182a5992a.SetBinding(global::System.Windows.Controls.ContentControl.ContentProperty, Binding_68e8976a53a44a37a63274c826a3281b);
            ContentPresenter_dad0417808c74b25ac4c5e5182a5992a.SetBinding(global::System.Windows.Controls.ContentControl.ContentTemplateProperty, Binding_d0ae12447320407e8b2b25cb5fd88bcb);
            Grid_7d5f63f475a54a5bbff239be7e8be75e.SetBinding(global::System.Windows.Controls.Panel.BackgroundProperty, Binding_369df61b15e2483cb102f8860ba54d8d);

            templateInstance_8450f63a5d83462e8f83459f689dc38c.TemplateContent = Grid_7d5f63f475a54a5bbff239be7e8be75e;
            return templateInstance_8450f63a5d83462e8f83459f689dc38c;
        }
    }
}
#else
namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultGridSplitterStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_c5c31696b72643cdba2270801e251155 = new global::Windows.UI.Xaml.Style();
                Style_c5c31696b72643cdba2270801e251155.TargetType = typeof(global::Windows.UI.Xaml.Controls.GridSplitter);
                var Setter_0e1932b8228e4144aefc35722e7b9b35 = new global::Windows.UI.Xaml.Setter();
                Setter_0e1932b8228e4144aefc35722e7b9b35.Property = global::Windows.UI.Xaml.Controls.GridSplitter.BackgroundProperty;
                Setter_0e1932b8228e4144aefc35722e7b9b35.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFF0F0F0");

                var Setter_3ef319c39e394dbd97ca9e37fbc51850 = new global::Windows.UI.Xaml.Setter();
                Setter_3ef319c39e394dbd97ca9e37fbc51850.Property = global::Windows.UI.Xaml.Controls.GridSplitter.TemplateProperty;
                var ControlTemplate_7ccfff71a0114b1295e3d943a9e6e0c7 = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_7ccfff71a0114b1295e3d943a9e6e0c7.TargetType = typeof(global::Windows.UI.Xaml.Controls.GridSplitter);
                ControlTemplate_7ccfff71a0114b1295e3d943a9e6e0c7.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_7ccfff71a0114b1295e3d943a9e6e0c7);

                Setter_3ef319c39e394dbd97ca9e37fbc51850.Value = ControlTemplate_7ccfff71a0114b1295e3d943a9e6e0c7;


                Style_c5c31696b72643cdba2270801e251155.Setters.Add(Setter_0e1932b8228e4144aefc35722e7b9b35);
                Style_c5c31696b72643cdba2270801e251155.Setters.Add(Setter_3ef319c39e394dbd97ca9e37fbc51850);


                DefaultStyle = Style_c5c31696b72643cdba2270801e251155;

            }

            return DefaultStyle;
        }

        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_7ccfff71a0114b1295e3d943a9e6e0c7(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_8450f63a5d83462e8f83459f689dc38c = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_8450f63a5d83462e8f83459f689dc38c.TemplateOwner = templateOwner;
            var Grid_7d5f63f475a54a5bbff239be7e8be75e = new global::Windows.UI.Xaml.Controls.Grid();
            var Thumb_6726f3e455d64fbab3cf89bb054990f8 = new global::Windows.UI.Xaml.Controls.Primitives.Thumb();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("PART_Thumb", Thumb_6726f3e455d64fbab3cf89bb054990f8);
            Thumb_6726f3e455d64fbab3cf89bb054990f8.Name = "PART_Thumb";
            Thumb_6726f3e455d64fbab3cf89bb054990f8.Opacity = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"0");

            var ContentPresenter_dad0417808c74b25ac4c5e5182a5992a = new global::Windows.UI.Xaml.Controls.ContentPresenter();
            ContentPresenter_dad0417808c74b25ac4c5e5182a5992a.IsHitTestVisible = (global::System.Boolean)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Boolean), @"False");
            var Binding_68e8976a53a44a37a63274c826a3281b = new global::Windows.UI.Xaml.Data.Binding();
            Binding_68e8976a53a44a37a63274c826a3281b.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Element");
            var RelativeSource_7dfbc1f104f2418e9ccf585fb2f3a82d = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_7dfbc1f104f2418e9ccf585fb2f3a82d.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_68e8976a53a44a37a63274c826a3281b.RelativeSource = RelativeSource_7dfbc1f104f2418e9ccf585fb2f3a82d;


            Binding_68e8976a53a44a37a63274c826a3281b.TemplateOwner = templateInstance_8450f63a5d83462e8f83459f689dc38c;

            var Binding_d0ae12447320407e8b2b25cb5fd88bcb = new global::Windows.UI.Xaml.Data.Binding();
            Binding_d0ae12447320407e8b2b25cb5fd88bcb.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"ContentTemplate");
            var RelativeSource_6c048aeffe8f46d4be81b962305286a7 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_6c048aeffe8f46d4be81b962305286a7.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_d0ae12447320407e8b2b25cb5fd88bcb.RelativeSource = RelativeSource_6c048aeffe8f46d4be81b962305286a7;


            Binding_d0ae12447320407e8b2b25cb5fd88bcb.TemplateOwner = templateInstance_8450f63a5d83462e8f83459f689dc38c;


            Grid_7d5f63f475a54a5bbff239be7e8be75e.Children.Add(Thumb_6726f3e455d64fbab3cf89bb054990f8);
            Grid_7d5f63f475a54a5bbff239be7e8be75e.Children.Add(ContentPresenter_dad0417808c74b25ac4c5e5182a5992a);

            var Binding_369df61b15e2483cb102f8860ba54d8d = new global::Windows.UI.Xaml.Data.Binding();
            Binding_369df61b15e2483cb102f8860ba54d8d.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_841fdd2e411c415fb0cc3ece5a76c8ef = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_841fdd2e411c415fb0cc3ece5a76c8ef.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_369df61b15e2483cb102f8860ba54d8d.RelativeSource = RelativeSource_841fdd2e411c415fb0cc3ece5a76c8ef;


            Binding_369df61b15e2483cb102f8860ba54d8d.TemplateOwner = templateInstance_8450f63a5d83462e8f83459f689dc38c;



            ContentPresenter_dad0417808c74b25ac4c5e5182a5992a.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentProperty, Binding_68e8976a53a44a37a63274c826a3281b);
            ContentPresenter_dad0417808c74b25ac4c5e5182a5992a.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentTemplateProperty, Binding_d0ae12447320407e8b2b25cb5fd88bcb);
            Grid_7d5f63f475a54a5bbff239be7e8be75e.SetBinding(global::Windows.UI.Xaml.Controls.Panel.BackgroundProperty, Binding_369df61b15e2483cb102f8860ba54d8d);

            templateInstance_8450f63a5d83462e8f83459f689dc38c.TemplateContent = Grid_7d5f63f475a54a5bbff239be7e8be75e;
            return templateInstance_8450f63a5d83462e8f83459f689dc38c;
        }
    }
}
#endif