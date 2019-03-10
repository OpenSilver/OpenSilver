
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
    internal class INTERNAL_DefaultViewboxStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_77a588f6edc64a5e8bc656c9a95b9431 = new global::System.Windows.Style();
                Style_77a588f6edc64a5e8bc656c9a95b9431.TargetType = typeof(global::System.Windows.Controls.Viewbox);
                var Setter_37a51523c69a4098878c3fa5ab118536 = new global::System.Windows.Setter();
                Setter_37a51523c69a4098878c3fa5ab118536.Property = global::System.Windows.Controls.Viewbox.ClipToBoundsProperty;
                Setter_37a51523c69a4098878c3fa5ab118536.Value = (global::System.Boolean)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Boolean), @"True");

                var Setter_fba52fea244b46f19046922ce00d191b = new global::System.Windows.Setter();
                Setter_fba52fea244b46f19046922ce00d191b.Property = global::System.Windows.Controls.Viewbox.TemplateProperty;
                var ControlTemplate_6c4d02d4d9bb4705910552a5e82ba6db = new global::System.Windows.Controls.ControlTemplate();
                ControlTemplate_6c4d02d4d9bb4705910552a5e82ba6db.TargetType = typeof(global::System.Windows.Controls.Viewbox);
                ControlTemplate_6c4d02d4d9bb4705910552a5e82ba6db.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_6c4d02d4d9bb4705910552a5e82ba6db);

                Setter_fba52fea244b46f19046922ce00d191b.Value = ControlTemplate_6c4d02d4d9bb4705910552a5e82ba6db;


                Style_77a588f6edc64a5e8bc656c9a95b9431.Setters.Add(Setter_37a51523c69a4098878c3fa5ab118536);
                Style_77a588f6edc64a5e8bc656c9a95b9431.Setters.Add(Setter_fba52fea244b46f19046922ce00d191b);


                DefaultStyle = Style_77a588f6edc64a5e8bc656c9a95b9431;
            }

            return DefaultStyle;
        }




        static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_6c4d02d4d9bb4705910552a5e82ba6db(global::System.Windows.FrameworkElement templateOwner)
        {
            var templateInstance_afd8731f84664ee7869ebc710198bc74 = new global::System.Windows.TemplateInstance();
            templateInstance_afd8731f84664ee7869ebc710198bc74.TemplateOwner = templateOwner;
            var Canvas_76c5efc7e54f42a2a1f979bea34a10d4 = new global::System.Windows.Controls.Canvas();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("RootCanvas", Canvas_76c5efc7e54f42a2a1f979bea34a10d4);
            Canvas_76c5efc7e54f42a2a1f979bea34a10d4.Name = "RootCanvas";
            var ContentPresenter_c1bbf47361e14d578e96c403ddd418d7 = new global::System.Windows.Controls.ContentPresenter();
            ((global::System.Windows.Controls.Control)templateOwner).RegisterName("Child", ContentPresenter_c1bbf47361e14d578e96c403ddd418d7);
            ContentPresenter_c1bbf47361e14d578e96c403ddd418d7.Name = "Child";
            var Binding_83a79b3ac6c5403db946e5064b89949f = new global::System.Windows.Data.Binding();
            Binding_83a79b3ac6c5403db946e5064b89949f.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"HorizontalAlignment");
            var RelativeSource_78808037afca4f5d89ce2dde90bdbcab = new global::System.Windows.Data.RelativeSource();
            RelativeSource_78808037afca4f5d89ce2dde90bdbcab.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_83a79b3ac6c5403db946e5064b89949f.RelativeSource = RelativeSource_78808037afca4f5d89ce2dde90bdbcab;


            Binding_83a79b3ac6c5403db946e5064b89949f.TemplateOwner = templateInstance_afd8731f84664ee7869ebc710198bc74;

            var Binding_bb8c0ddfc42943ba957c53cbf26cc253 = new global::System.Windows.Data.Binding();
            Binding_bb8c0ddfc42943ba957c53cbf26cc253.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"VerticalAlignment");
            var RelativeSource_1b354f2ee5964974a02f31f8fb2a400f = new global::System.Windows.Data.RelativeSource();
            RelativeSource_1b354f2ee5964974a02f31f8fb2a400f.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_bb8c0ddfc42943ba957c53cbf26cc253.RelativeSource = RelativeSource_1b354f2ee5964974a02f31f8fb2a400f;


            Binding_bb8c0ddfc42943ba957c53cbf26cc253.TemplateOwner = templateInstance_afd8731f84664ee7869ebc710198bc74;

            var Binding_13f023d0b6e44028b00b8e56eea426ac = new global::System.Windows.Data.Binding();
            Binding_13f023d0b6e44028b00b8e56eea426ac.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Content");
            var RelativeSource_4e1197bcef9f459ead5c4023f0241ee9 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_4e1197bcef9f459ead5c4023f0241ee9.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_13f023d0b6e44028b00b8e56eea426ac.RelativeSource = RelativeSource_4e1197bcef9f459ead5c4023f0241ee9;


            Binding_13f023d0b6e44028b00b8e56eea426ac.TemplateOwner = templateInstance_afd8731f84664ee7869ebc710198bc74;

            var Binding_08887a59fd0c4eebb0ebd2de7562148e = new global::System.Windows.Data.Binding();
            Binding_08887a59fd0c4eebb0ebd2de7562148e.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"ContentTemplate");
            var RelativeSource_8c0de59bddcc4fff9eca8eaaec7e6891 = new global::System.Windows.Data.RelativeSource();
            RelativeSource_8c0de59bddcc4fff9eca8eaaec7e6891.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

            Binding_08887a59fd0c4eebb0ebd2de7562148e.RelativeSource = RelativeSource_8c0de59bddcc4fff9eca8eaaec7e6891;


            Binding_08887a59fd0c4eebb0ebd2de7562148e.TemplateOwner = templateInstance_afd8731f84664ee7869ebc710198bc74;


            Canvas_76c5efc7e54f42a2a1f979bea34a10d4.Children.Add(ContentPresenter_c1bbf47361e14d578e96c403ddd418d7);



            ContentPresenter_c1bbf47361e14d578e96c403ddd418d7.SetBinding(global::System.Windows.FrameworkElement.HorizontalAlignmentProperty, Binding_83a79b3ac6c5403db946e5064b89949f);
            ContentPresenter_c1bbf47361e14d578e96c403ddd418d7.SetBinding(global::System.Windows.FrameworkElement.VerticalAlignmentProperty, Binding_bb8c0ddfc42943ba957c53cbf26cc253);
            ContentPresenter_c1bbf47361e14d578e96c403ddd418d7.SetBinding(global::System.Windows.Controls.ContentControl.ContentProperty, Binding_13f023d0b6e44028b00b8e56eea426ac);
            ContentPresenter_c1bbf47361e14d578e96c403ddd418d7.SetBinding(global::System.Windows.Controls.ContentControl.ContentTemplateProperty, Binding_08887a59fd0c4eebb0ebd2de7562148e);

            templateInstance_afd8731f84664ee7869ebc710198bc74.TemplateContent = Canvas_76c5efc7e54f42a2a1f979bea34a10d4;
            return templateInstance_afd8731f84664ee7869ebc710198bc74;
        }
    }
}
#else
namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultViewboxStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {
                var Style_77a588f6edc64a5e8bc656c9a95b9431 = new global::Windows.UI.Xaml.Style();
                Style_77a588f6edc64a5e8bc656c9a95b9431.TargetType = typeof(global::Windows.UI.Xaml.Controls.Viewbox);
                var Setter_37a51523c69a4098878c3fa5ab118536 = new global::Windows.UI.Xaml.Setter();
                Setter_37a51523c69a4098878c3fa5ab118536.Property = global::Windows.UI.Xaml.Controls.Viewbox.ClipToBoundsProperty;
                Setter_37a51523c69a4098878c3fa5ab118536.Value = (global::System.Boolean)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Boolean), @"True");

                var Setter_fba52fea244b46f19046922ce00d191b = new global::Windows.UI.Xaml.Setter();
                Setter_fba52fea244b46f19046922ce00d191b.Property = global::Windows.UI.Xaml.Controls.Viewbox.TemplateProperty;
                var ControlTemplate_6c4d02d4d9bb4705910552a5e82ba6db = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_6c4d02d4d9bb4705910552a5e82ba6db.TargetType = typeof(global::Windows.UI.Xaml.Controls.Viewbox);
                ControlTemplate_6c4d02d4d9bb4705910552a5e82ba6db.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_6c4d02d4d9bb4705910552a5e82ba6db);

                Setter_fba52fea244b46f19046922ce00d191b.Value = ControlTemplate_6c4d02d4d9bb4705910552a5e82ba6db;


                Style_77a588f6edc64a5e8bc656c9a95b9431.Setters.Add(Setter_37a51523c69a4098878c3fa5ab118536);
                Style_77a588f6edc64a5e8bc656c9a95b9431.Setters.Add(Setter_fba52fea244b46f19046922ce00d191b);


                DefaultStyle = Style_77a588f6edc64a5e8bc656c9a95b9431;
            }

            return DefaultStyle;
        }




        static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_6c4d02d4d9bb4705910552a5e82ba6db(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_afd8731f84664ee7869ebc710198bc74 = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_afd8731f84664ee7869ebc710198bc74.TemplateOwner = templateOwner;
            var Canvas_76c5efc7e54f42a2a1f979bea34a10d4 = new global::Windows.UI.Xaml.Controls.Canvas();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("RootCanvas", Canvas_76c5efc7e54f42a2a1f979bea34a10d4);
            Canvas_76c5efc7e54f42a2a1f979bea34a10d4.Name = "RootCanvas";
            var ContentPresenter_c1bbf47361e14d578e96c403ddd418d7 = new global::Windows.UI.Xaml.Controls.ContentPresenter();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Child", ContentPresenter_c1bbf47361e14d578e96c403ddd418d7);
            ContentPresenter_c1bbf47361e14d578e96c403ddd418d7.Name = "Child";
            var Binding_83a79b3ac6c5403db946e5064b89949f = new global::Windows.UI.Xaml.Data.Binding();
            Binding_83a79b3ac6c5403db946e5064b89949f.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"HorizontalAlignment");
            var RelativeSource_78808037afca4f5d89ce2dde90bdbcab = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_78808037afca4f5d89ce2dde90bdbcab.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_83a79b3ac6c5403db946e5064b89949f.RelativeSource = RelativeSource_78808037afca4f5d89ce2dde90bdbcab;


            Binding_83a79b3ac6c5403db946e5064b89949f.TemplateOwner = templateInstance_afd8731f84664ee7869ebc710198bc74;

            var Binding_bb8c0ddfc42943ba957c53cbf26cc253 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_bb8c0ddfc42943ba957c53cbf26cc253.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"VerticalAlignment");
            var RelativeSource_1b354f2ee5964974a02f31f8fb2a400f = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_1b354f2ee5964974a02f31f8fb2a400f.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_bb8c0ddfc42943ba957c53cbf26cc253.RelativeSource = RelativeSource_1b354f2ee5964974a02f31f8fb2a400f;


            Binding_bb8c0ddfc42943ba957c53cbf26cc253.TemplateOwner = templateInstance_afd8731f84664ee7869ebc710198bc74;

            var Binding_13f023d0b6e44028b00b8e56eea426ac = new global::Windows.UI.Xaml.Data.Binding();
            Binding_13f023d0b6e44028b00b8e56eea426ac.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Content");
            var RelativeSource_4e1197bcef9f459ead5c4023f0241ee9 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_4e1197bcef9f459ead5c4023f0241ee9.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_13f023d0b6e44028b00b8e56eea426ac.RelativeSource = RelativeSource_4e1197bcef9f459ead5c4023f0241ee9;


            Binding_13f023d0b6e44028b00b8e56eea426ac.TemplateOwner = templateInstance_afd8731f84664ee7869ebc710198bc74;

            var Binding_08887a59fd0c4eebb0ebd2de7562148e = new global::Windows.UI.Xaml.Data.Binding();
            Binding_08887a59fd0c4eebb0ebd2de7562148e.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"ContentTemplate");
            var RelativeSource_8c0de59bddcc4fff9eca8eaaec7e6891 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_8c0de59bddcc4fff9eca8eaaec7e6891.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_08887a59fd0c4eebb0ebd2de7562148e.RelativeSource = RelativeSource_8c0de59bddcc4fff9eca8eaaec7e6891;


            Binding_08887a59fd0c4eebb0ebd2de7562148e.TemplateOwner = templateInstance_afd8731f84664ee7869ebc710198bc74;


            Canvas_76c5efc7e54f42a2a1f979bea34a10d4.Children.Add(ContentPresenter_c1bbf47361e14d578e96c403ddd418d7);



            ContentPresenter_c1bbf47361e14d578e96c403ddd418d7.SetBinding(global::Windows.UI.Xaml.FrameworkElement.HorizontalAlignmentProperty, Binding_83a79b3ac6c5403db946e5064b89949f);
            ContentPresenter_c1bbf47361e14d578e96c403ddd418d7.SetBinding(global::Windows.UI.Xaml.FrameworkElement.VerticalAlignmentProperty, Binding_bb8c0ddfc42943ba957c53cbf26cc253);
            ContentPresenter_c1bbf47361e14d578e96c403ddd418d7.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentProperty, Binding_13f023d0b6e44028b00b8e56eea426ac);
            ContentPresenter_c1bbf47361e14d578e96c403ddd418d7.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentTemplateProperty, Binding_08887a59fd0c4eebb0ebd2de7562148e);

            templateInstance_afd8731f84664ee7869ebc710198bc74.TemplateContent = Canvas_76c5efc7e54f42a2a1f979bea34a10d4;
            return templateInstance_afd8731f84664ee7869ebc710198bc74;
        }


    }
}
#endif