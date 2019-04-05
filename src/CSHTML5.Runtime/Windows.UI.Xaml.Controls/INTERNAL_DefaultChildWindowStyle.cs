
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
    internal class INTERNAL_DefaultChildWindowStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {





var Style_64ad7c9c63ed4e8bbaa7059400c08a6f = new global::System.Windows.Style();
Style_64ad7c9c63ed4e8bbaa7059400c08a6f.TargetType = typeof(global::System.Windows.Controls.ChildWindow);
var Setter_647be68aec824ee591116fc20dddbce2 = new global::System.Windows.Setter();
Setter_647be68aec824ee591116fc20dddbce2.Property = global::System.Windows.Controls.ChildWindow.HorizontalAlignmentProperty;
Setter_647be68aec824ee591116fc20dddbce2.Value = global::System.Windows.HorizontalAlignment.Center;

var Setter_9bd7e4b2eb8c43568d060436581f27c0 = new global::System.Windows.Setter();
Setter_9bd7e4b2eb8c43568d060436581f27c0.Property = global::System.Windows.Controls.ChildWindow.VerticalAlignmentProperty;
Setter_9bd7e4b2eb8c43568d060436581f27c0.Value = global::System.Windows.VerticalAlignment.Center;

var Setter_631d2af001a04b5f920dff56742afaf5 = new global::System.Windows.Setter();
Setter_631d2af001a04b5f920dff56742afaf5.Property = global::System.Windows.Controls.ChildWindow.HorizontalContentAlignmentProperty;
Setter_631d2af001a04b5f920dff56742afaf5.Value = global::System.Windows.HorizontalAlignment.Stretch;

var Setter_62140b5b875e4d62ad48551d1f789d4c = new global::System.Windows.Setter();
Setter_62140b5b875e4d62ad48551d1f789d4c.Property = global::System.Windows.Controls.ChildWindow.VerticalContentAlignmentProperty;
Setter_62140b5b875e4d62ad48551d1f789d4c.Value = global::System.Windows.VerticalAlignment.Stretch;

var Setter_82acba98e92442e6beec2e5cd1900752 = new global::System.Windows.Setter();
Setter_82acba98e92442e6beec2e5cd1900752.Property = global::System.Windows.Controls.ChildWindow.BorderThicknessProperty;
Setter_82acba98e92442e6beec2e5cd1900752.Value = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"2");

var Setter_8a7a4e48464041d08d382e3719de61c4 = new global::System.Windows.Setter();
Setter_8a7a4e48464041d08d382e3719de61c4.Property = global::System.Windows.Controls.ChildWindow.BorderBrushProperty;
Setter_8a7a4e48464041d08d382e3719de61c4.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFE2E2E2");

var Setter_cafb4a20ed0f48ff82bb4ced149a1182 = new global::System.Windows.Setter();
Setter_cafb4a20ed0f48ff82bb4ced149a1182.Property = global::System.Windows.Controls.ChildWindow.OverlayBrushProperty;
Setter_cafb4a20ed0f48ff82bb4ced149a1182.Value = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#7F000000");

var Setter_664d44e6d0f6438e99e169fcce9e403b = new global::System.Windows.Setter();
Setter_664d44e6d0f6438e99e169fcce9e403b.Property = global::System.Windows.Controls.ChildWindow.OverlayOpacityProperty;
Setter_664d44e6d0f6438e99e169fcce9e403b.Value = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"1");

var Setter_d7ef8f0592b24004be39893a40f27d1b = new global::System.Windows.Setter();
Setter_d7ef8f0592b24004be39893a40f27d1b.Property = global::System.Windows.Controls.ChildWindow.TemplateProperty;
var ControlTemplate_b661c9bca5874e93b66ba37eec850d96 = new global::System.Windows.Controls.ControlTemplate();
ControlTemplate_b661c9bca5874e93b66ba37eec850d96.TargetType = typeof(global::System.Windows.Controls.ChildWindow);
ControlTemplate_b661c9bca5874e93b66ba37eec850d96.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_b661c9bca5874e93b66ba37eec850d96);

Setter_d7ef8f0592b24004be39893a40f27d1b.Value = ControlTemplate_b661c9bca5874e93b66ba37eec850d96;


Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_647be68aec824ee591116fc20dddbce2);
Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_9bd7e4b2eb8c43568d060436581f27c0);
Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_631d2af001a04b5f920dff56742afaf5);
Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_62140b5b875e4d62ad48551d1f789d4c);
Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_82acba98e92442e6beec2e5cd1900752);
Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_8a7a4e48464041d08d382e3719de61c4);
Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_cafb4a20ed0f48ff82bb4ced149a1182);
Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_664d44e6d0f6438e99e169fcce9e403b);
Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_d7ef8f0592b24004be39893a40f27d1b);


               DefaultStyle = Style_64ad7c9c63ed4e8bbaa7059400c08a6f;
            }
            return DefaultStyle;






    
        }



        private static global::System.Windows.TemplateInstance Instantiate_ControlTemplate_b661c9bca5874e93b66ba37eec850d96(global::System.Windows.FrameworkElement templateOwner)
        {
var templateInstance_939af67689af491b8a43baa14bf60e61 = new global::System.Windows.TemplateInstance();
templateInstance_939af67689af491b8a43baa14bf60e61.TemplateOwner = templateOwner;
var Grid_303391c44429425e8bae3462af97f914 = new global::System.Windows.Controls.Grid();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Root", Grid_303391c44429425e8bae3462af97f914);
Grid_303391c44429425e8bae3462af97f914.Name = "Root";
var Grid_63e050e5353c4016bee3ab2703c5fc1e = new global::System.Windows.Controls.Grid();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Overlay", Grid_63e050e5353c4016bee3ab2703c5fc1e);
Grid_63e050e5353c4016bee3ab2703c5fc1e.Name = "Overlay";
Grid_63e050e5353c4016bee3ab2703c5fc1e.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Stretch;
Grid_63e050e5353c4016bee3ab2703c5fc1e.VerticalAlignment = global::System.Windows.VerticalAlignment.Stretch;
Grid_63e050e5353c4016bee3ab2703c5fc1e.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0");
var Binding_8bcdb4bc316d4c158ac8af78e190cc2a = new global::System.Windows.Data.Binding();
Binding_8bcdb4bc316d4c158ac8af78e190cc2a.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"OverlayBrush");
var RelativeSource_c24bd92e11bb4c6fa4a187c912c884d2 = new global::System.Windows.Data.RelativeSource();
RelativeSource_c24bd92e11bb4c6fa4a187c912c884d2.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_8bcdb4bc316d4c158ac8af78e190cc2a.RelativeSource = RelativeSource_c24bd92e11bb4c6fa4a187c912c884d2;


Binding_8bcdb4bc316d4c158ac8af78e190cc2a.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;

var Binding_d15eb3d8d9284262826be3fd8a567ca9 = new global::System.Windows.Data.Binding();
Binding_d15eb3d8d9284262826be3fd8a567ca9.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"OverlayOpacity");
var RelativeSource_d9907db8f1184dfa99fb31bb188df289 = new global::System.Windows.Data.RelativeSource();
RelativeSource_d9907db8f1184dfa99fb31bb188df289.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_d15eb3d8d9284262826be3fd8a567ca9.RelativeSource = RelativeSource_d9907db8f1184dfa99fb31bb188df289;


Binding_d15eb3d8d9284262826be3fd8a567ca9.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;


var Border_ac973cac7206487bbd7668d20a97126e = new global::System.Windows.Controls.Border();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("ContentRoot", Border_ac973cac7206487bbd7668d20a97126e);
Border_ac973cac7206487bbd7668d20a97126e.Name = "ContentRoot";
Border_ac973cac7206487bbd7668d20a97126e.RenderTransformOrigin = (global::System.Windows.Point)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Point), @"0.5,0.5");
var Border_423d8fe602004386902fc81d1a25d89c = new global::System.Windows.Controls.Border();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("ContentContainer", Border_423d8fe602004386902fc81d1a25d89c);
Border_423d8fe602004386902fc81d1a25d89c.Name = "ContentContainer";
Border_423d8fe602004386902fc81d1a25d89c.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFF6F6F6");
var Grid_76c09bf7565d4851b3485501aa861ffa = new global::System.Windows.Controls.Grid();
var RowDefinition_ea5dfeec04b34ce0b49cf367e17b5fa2 = new global::System.Windows.Controls.RowDefinition();
RowDefinition_ea5dfeec04b34ce0b49cf367e17b5fa2.Height = (global::System.Windows.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.GridLength), @"Auto");

var RowDefinition_214bd3d1974a48cb89b1678be5cee77f = new global::System.Windows.Controls.RowDefinition();

Grid_76c09bf7565d4851b3485501aa861ffa.RowDefinitions.Add(RowDefinition_ea5dfeec04b34ce0b49cf367e17b5fa2);
Grid_76c09bf7565d4851b3485501aa861ffa.RowDefinitions.Add(RowDefinition_214bd3d1974a48cb89b1678be5cee77f);

var Border_efed7fad940e42e4b9f2ef7f99241750 = new global::System.Windows.Controls.Border();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("Chrome", Border_efed7fad940e42e4b9f2ef7f99241750);
Border_efed7fad940e42e4b9f2ef7f99241750.Name = "Chrome";
Border_efed7fad940e42e4b9f2ef7f99241750.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"32");
Border_efed7fad940e42e4b9f2ef7f99241750.BorderBrush = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFE2E2E2");
Border_efed7fad940e42e4b9f2ef7f99241750.BorderThickness = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"0,0,0,1");
Border_efed7fad940e42e4b9f2ef7f99241750.Background = (global::System.Windows.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Media.Brush), @"#FFEEEEEE");
var Grid_4bc9d8922d0c48ca8806241215bd56c1 = new global::System.Windows.Controls.Grid();
var ColumnDefinition_3344f0b78db94da78f110d538eef1506 = new global::System.Windows.Controls.ColumnDefinition();

var ColumnDefinition_536a17694c8c471d9a4dafff5ea27eeb = new global::System.Windows.Controls.ColumnDefinition();
ColumnDefinition_536a17694c8c471d9a4dafff5ea27eeb.Width = (global::System.Windows.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.GridLength), @"30");

Grid_4bc9d8922d0c48ca8806241215bd56c1.ColumnDefinitions.Add(ColumnDefinition_3344f0b78db94da78f110d538eef1506);
Grid_4bc9d8922d0c48ca8806241215bd56c1.ColumnDefinitions.Add(ColumnDefinition_536a17694c8c471d9a4dafff5ea27eeb);

var ContentControl_ab4af4179c3c441e8107b6f04d0ab1fa = new global::System.Windows.Controls.ContentControl();
ContentControl_ab4af4179c3c441e8107b6f04d0ab1fa.FontWeight = (global::System.Windows.FontWeight)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.FontWeight), @"Bold");
ContentControl_ab4af4179c3c441e8107b6f04d0ab1fa.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Stretch;
ContentControl_ab4af4179c3c441e8107b6f04d0ab1fa.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
ContentControl_ab4af4179c3c441e8107b6f04d0ab1fa.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"6,0,6,0");
var Binding_817dffb2c6d24b56b84ba830c021eb88 = new global::System.Windows.Data.Binding();
Binding_817dffb2c6d24b56b84ba830c021eb88.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Title");
var RelativeSource_6ba57413d247418c9c58ca29321c4248 = new global::System.Windows.Data.RelativeSource();
RelativeSource_6ba57413d247418c9c58ca29321c4248.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_817dffb2c6d24b56b84ba830c021eb88.RelativeSource = RelativeSource_6ba57413d247418c9c58ca29321c4248;


Binding_817dffb2c6d24b56b84ba830c021eb88.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;


var Button_0c5ac5e1edce4bb9b3db1859acea544a = new global::System.Windows.Controls.Button();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("CloseButton", Button_0c5ac5e1edce4bb9b3db1859acea544a);
Button_0c5ac5e1edce4bb9b3db1859acea544a.Name = "CloseButton";
Button_0c5ac5e1edce4bb9b3db1859acea544a.Content = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"X");
global::System.Windows.Controls.Grid.SetColumn(Button_0c5ac5e1edce4bb9b3db1859acea544a,(global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"1"));
Button_0c5ac5e1edce4bb9b3db1859acea544a.FontSize = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"16");
Button_0c5ac5e1edce4bb9b3db1859acea544a.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Center;
Button_0c5ac5e1edce4bb9b3db1859acea544a.VerticalAlignment = global::System.Windows.VerticalAlignment.Center;
Button_0c5ac5e1edce4bb9b3db1859acea544a.HorizontalContentAlignment = global::System.Windows.HorizontalAlignment.Center;
Button_0c5ac5e1edce4bb9b3db1859acea544a.VerticalContentAlignment = global::System.Windows.VerticalAlignment.Center;
Button_0c5ac5e1edce4bb9b3db1859acea544a.Padding = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"8,3");

Grid_4bc9d8922d0c48ca8806241215bd56c1.Children.Add(ContentControl_ab4af4179c3c441e8107b6f04d0ab1fa);
Grid_4bc9d8922d0c48ca8806241215bd56c1.Children.Add(Button_0c5ac5e1edce4bb9b3db1859acea544a);


Border_efed7fad940e42e4b9f2ef7f99241750.Child = Grid_4bc9d8922d0c48ca8806241215bd56c1;


var Border_b2fd71baf6b847d2af116fcffe818d60 = new global::System.Windows.Controls.Border();
Border_b2fd71baf6b847d2af116fcffe818d60.Margin = (global::System.Windows.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.Thickness), @"7");
global::System.Windows.Controls.Grid.SetRow(Border_b2fd71baf6b847d2af116fcffe818d60,(global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"1"));
var ContentPresenter_7cf499f53c6d433591f2a0beaa835702 = new global::System.Windows.Controls.ContentPresenter();
((global::System.Windows.Controls.Control)templateOwner).RegisterName("ContentPresenter", ContentPresenter_7cf499f53c6d433591f2a0beaa835702);
ContentPresenter_7cf499f53c6d433591f2a0beaa835702.Name = "ContentPresenter";
var Binding_626b78d2ba674046aa886e5b995a8132 = new global::System.Windows.Data.Binding();
Binding_626b78d2ba674046aa886e5b995a8132.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Content");
var RelativeSource_f12ea6877eb043309d1a9a0505208986 = new global::System.Windows.Data.RelativeSource();
RelativeSource_f12ea6877eb043309d1a9a0505208986.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_626b78d2ba674046aa886e5b995a8132.RelativeSource = RelativeSource_f12ea6877eb043309d1a9a0505208986;


Binding_626b78d2ba674046aa886e5b995a8132.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;

var Binding_a6c67aca6fca43f9a67cb8fe2e00a354 = new global::System.Windows.Data.Binding();
Binding_a6c67aca6fca43f9a67cb8fe2e00a354.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"ContentTemplate");
var RelativeSource_e044a4dd9e1341c4bbf26ae23738bd3f = new global::System.Windows.Data.RelativeSource();
RelativeSource_e044a4dd9e1341c4bbf26ae23738bd3f.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_a6c67aca6fca43f9a67cb8fe2e00a354.RelativeSource = RelativeSource_e044a4dd9e1341c4bbf26ae23738bd3f;


Binding_a6c67aca6fca43f9a67cb8fe2e00a354.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;

var Binding_dd03fb02fdd849cab1872dbd5fd5f790 = new global::System.Windows.Data.Binding();
Binding_dd03fb02fdd849cab1872dbd5fd5f790.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"HorizontalContentAlignment");
var RelativeSource_c0904e69c98142bbbc50373ff508e802 = new global::System.Windows.Data.RelativeSource();
RelativeSource_c0904e69c98142bbbc50373ff508e802.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_dd03fb02fdd849cab1872dbd5fd5f790.RelativeSource = RelativeSource_c0904e69c98142bbbc50373ff508e802;


Binding_dd03fb02fdd849cab1872dbd5fd5f790.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;

var Binding_767ac44d67a245849aa62d7852633ce4 = new global::System.Windows.Data.Binding();
Binding_767ac44d67a245849aa62d7852633ce4.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"VerticalContentAlignment");
var RelativeSource_4d56ef314f3c4bd48fe7f0a29298d375 = new global::System.Windows.Data.RelativeSource();
RelativeSource_4d56ef314f3c4bd48fe7f0a29298d375.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_767ac44d67a245849aa62d7852633ce4.RelativeSource = RelativeSource_4d56ef314f3c4bd48fe7f0a29298d375;


Binding_767ac44d67a245849aa62d7852633ce4.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;


Border_b2fd71baf6b847d2af116fcffe818d60.Child = ContentPresenter_7cf499f53c6d433591f2a0beaa835702;

var Binding_71dd525db9fa46ac8648553995cec4a5 = new global::System.Windows.Data.Binding();
Binding_71dd525db9fa46ac8648553995cec4a5.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Background");
var RelativeSource_2203f79dda0b40afb3a74048e1b880e6 = new global::System.Windows.Data.RelativeSource();
RelativeSource_2203f79dda0b40afb3a74048e1b880e6.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_71dd525db9fa46ac8648553995cec4a5.RelativeSource = RelativeSource_2203f79dda0b40afb3a74048e1b880e6;


Binding_71dd525db9fa46ac8648553995cec4a5.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;


Grid_76c09bf7565d4851b3485501aa861ffa.Children.Add(Border_efed7fad940e42e4b9f2ef7f99241750);
Grid_76c09bf7565d4851b3485501aa861ffa.Children.Add(Border_b2fd71baf6b847d2af116fcffe818d60);


Border_423d8fe602004386902fc81d1a25d89c.Child = Grid_76c09bf7565d4851b3485501aa861ffa;

var Binding_37240cd789a44c6ba1453bf29ae004c4 = new global::System.Windows.Data.Binding();
Binding_37240cd789a44c6ba1453bf29ae004c4.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderThickness");
var RelativeSource_32feb236f88b4eba83dc8bf2f852a2d2 = new global::System.Windows.Data.RelativeSource();
RelativeSource_32feb236f88b4eba83dc8bf2f852a2d2.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_37240cd789a44c6ba1453bf29ae004c4.RelativeSource = RelativeSource_32feb236f88b4eba83dc8bf2f852a2d2;


Binding_37240cd789a44c6ba1453bf29ae004c4.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;

var Binding_7a4b805c509448059e0cf37d98ef8407 = new global::System.Windows.Data.Binding();
Binding_7a4b805c509448059e0cf37d98ef8407.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"BorderBrush");
var RelativeSource_7f21be18791a4e85aa90f68098ef5c14 = new global::System.Windows.Data.RelativeSource();
RelativeSource_7f21be18791a4e85aa90f68098ef5c14.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_7a4b805c509448059e0cf37d98ef8407.RelativeSource = RelativeSource_7f21be18791a4e85aa90f68098ef5c14;


Binding_7a4b805c509448059e0cf37d98ef8407.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;


Border_ac973cac7206487bbd7668d20a97126e.Child = Border_423d8fe602004386902fc81d1a25d89c;

var Binding_7e2986644cfa40e3b931ba561353e079 = new global::System.Windows.Data.Binding();
Binding_7e2986644cfa40e3b931ba561353e079.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"HorizontalAlignment");
var RelativeSource_d32b713635404ed09f0fb8680055678f = new global::System.Windows.Data.RelativeSource();
RelativeSource_d32b713635404ed09f0fb8680055678f.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_7e2986644cfa40e3b931ba561353e079.RelativeSource = RelativeSource_d32b713635404ed09f0fb8680055678f;


Binding_7e2986644cfa40e3b931ba561353e079.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;

var Binding_e6bc91e0f62a4474824ca0677b667ea6 = new global::System.Windows.Data.Binding();
Binding_e6bc91e0f62a4474824ca0677b667ea6.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"VerticalAlignment");
var RelativeSource_531f636fc5414b86b6ab079b0cf1c202 = new global::System.Windows.Data.RelativeSource();
RelativeSource_531f636fc5414b86b6ab079b0cf1c202.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_e6bc91e0f62a4474824ca0677b667ea6.RelativeSource = RelativeSource_531f636fc5414b86b6ab079b0cf1c202;


Binding_e6bc91e0f62a4474824ca0677b667ea6.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;

var Binding_6676b4d376394ba981b24943746da61e = new global::System.Windows.Data.Binding();
Binding_6676b4d376394ba981b24943746da61e.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Height");
var RelativeSource_a8733721dc14442eb41c74d456be8990 = new global::System.Windows.Data.RelativeSource();
RelativeSource_a8733721dc14442eb41c74d456be8990.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_6676b4d376394ba981b24943746da61e.RelativeSource = RelativeSource_a8733721dc14442eb41c74d456be8990;


Binding_6676b4d376394ba981b24943746da61e.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;

var Binding_e3e89f8c3a7949ebbe79b5d79691af5b = new global::System.Windows.Data.Binding();
Binding_e3e89f8c3a7949ebbe79b5d79691af5b.Path = (global::System.Windows.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Windows.PropertyPath), @"Width");
var RelativeSource_a53b0729936d42d5b92cb86f8ee38322 = new global::System.Windows.Data.RelativeSource();
RelativeSource_a53b0729936d42d5b92cb86f8ee38322.Mode = global::System.Windows.Data.RelativeSourceMode.TemplatedParent;

Binding_e3e89f8c3a7949ebbe79b5d79691af5b.RelativeSource = RelativeSource_a53b0729936d42d5b92cb86f8ee38322;


Binding_e3e89f8c3a7949ebbe79b5d79691af5b.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;


Grid_303391c44429425e8bae3462af97f914.Children.Add(Grid_63e050e5353c4016bee3ab2703c5fc1e);
Grid_303391c44429425e8bae3462af97f914.Children.Add(Border_ac973cac7206487bbd7668d20a97126e);



Grid_63e050e5353c4016bee3ab2703c5fc1e.SetBinding(global::System.Windows.Controls.Panel.BackgroundProperty, Binding_8bcdb4bc316d4c158ac8af78e190cc2a);
Grid_63e050e5353c4016bee3ab2703c5fc1e.SetBinding(global::System.Windows.UIElement.OpacityProperty, Binding_d15eb3d8d9284262826be3fd8a567ca9);
ContentControl_ab4af4179c3c441e8107b6f04d0ab1fa.SetBinding(global::System.Windows.Controls.ContentControl.ContentProperty, Binding_817dffb2c6d24b56b84ba830c021eb88);
ContentPresenter_7cf499f53c6d433591f2a0beaa835702.SetBinding(global::System.Windows.Controls.ContentControl.ContentProperty, Binding_626b78d2ba674046aa886e5b995a8132);
ContentPresenter_7cf499f53c6d433591f2a0beaa835702.SetBinding(global::System.Windows.Controls.ContentControl.ContentTemplateProperty, Binding_a6c67aca6fca43f9a67cb8fe2e00a354);
ContentPresenter_7cf499f53c6d433591f2a0beaa835702.SetBinding(global::System.Windows.FrameworkElement.HorizontalAlignmentProperty, Binding_dd03fb02fdd849cab1872dbd5fd5f790);
ContentPresenter_7cf499f53c6d433591f2a0beaa835702.SetBinding(global::System.Windows.FrameworkElement.VerticalAlignmentProperty, Binding_767ac44d67a245849aa62d7852633ce4);
Border_b2fd71baf6b847d2af116fcffe818d60.SetBinding(global::System.Windows.Controls.Border.BackgroundProperty, Binding_71dd525db9fa46ac8648553995cec4a5);
Border_423d8fe602004386902fc81d1a25d89c.SetBinding(global::System.Windows.Controls.Border.BorderThicknessProperty, Binding_37240cd789a44c6ba1453bf29ae004c4);
Border_423d8fe602004386902fc81d1a25d89c.SetBinding(global::System.Windows.Controls.Border.BorderBrushProperty, Binding_7a4b805c509448059e0cf37d98ef8407);
Border_ac973cac7206487bbd7668d20a97126e.SetBinding(global::System.Windows.FrameworkElement.HorizontalAlignmentProperty, Binding_7e2986644cfa40e3b931ba561353e079);
Border_ac973cac7206487bbd7668d20a97126e.SetBinding(global::System.Windows.FrameworkElement.VerticalAlignmentProperty, Binding_e6bc91e0f62a4474824ca0677b667ea6);
Border_ac973cac7206487bbd7668d20a97126e.SetBinding(global::System.Windows.FrameworkElement.HeightProperty, Binding_6676b4d376394ba981b24943746da61e);
Border_ac973cac7206487bbd7668d20a97126e.SetBinding(global::System.Windows.FrameworkElement.WidthProperty, Binding_e3e89f8c3a7949ebbe79b5d79691af5b);

templateInstance_939af67689af491b8a43baa14bf60e61.TemplateContent = Grid_303391c44429425e8bae3462af97f914;
return templateInstance_939af67689af491b8a43baa14bf60e61;
        }



        }
}
#else

namespace Windows.UI.Xaml.Controls
{
    internal class INTERNAL_DefaultChildWindowStyle
    {
        static Style DefaultStyle;

        public static Style GetDefaultStyle()
        {
            if (DefaultStyle == null)
            {





                var Style_64ad7c9c63ed4e8bbaa7059400c08a6f = new global::Windows.UI.Xaml.Style();
                Style_64ad7c9c63ed4e8bbaa7059400c08a6f.TargetType = typeof(global::Windows.UI.Xaml.Controls.ChildWindow);
                var Setter_647be68aec824ee591116fc20dddbce2 = new global::Windows.UI.Xaml.Setter();
                Setter_647be68aec824ee591116fc20dddbce2.Property = global::Windows.UI.Xaml.Controls.ChildWindow.HorizontalAlignmentProperty;
                Setter_647be68aec824ee591116fc20dddbce2.Value = global::Windows.UI.Xaml.HorizontalAlignment.Center;

                var Setter_9bd7e4b2eb8c43568d060436581f27c0 = new global::Windows.UI.Xaml.Setter();
                Setter_9bd7e4b2eb8c43568d060436581f27c0.Property = global::Windows.UI.Xaml.Controls.ChildWindow.VerticalAlignmentProperty;
                Setter_9bd7e4b2eb8c43568d060436581f27c0.Value = global::Windows.UI.Xaml.VerticalAlignment.Center;

                var Setter_631d2af001a04b5f920dff56742afaf5 = new global::Windows.UI.Xaml.Setter();
                Setter_631d2af001a04b5f920dff56742afaf5.Property = global::Windows.UI.Xaml.Controls.ChildWindow.HorizontalContentAlignmentProperty;
                Setter_631d2af001a04b5f920dff56742afaf5.Value = global::Windows.UI.Xaml.HorizontalAlignment.Stretch;

                var Setter_62140b5b875e4d62ad48551d1f789d4c = new global::Windows.UI.Xaml.Setter();
                Setter_62140b5b875e4d62ad48551d1f789d4c.Property = global::Windows.UI.Xaml.Controls.ChildWindow.VerticalContentAlignmentProperty;
                Setter_62140b5b875e4d62ad48551d1f789d4c.Value = global::Windows.UI.Xaml.VerticalAlignment.Stretch;

                var Setter_82acba98e92442e6beec2e5cd1900752 = new global::Windows.UI.Xaml.Setter();
                Setter_82acba98e92442e6beec2e5cd1900752.Property = global::Windows.UI.Xaml.Controls.ChildWindow.BorderThicknessProperty;
                Setter_82acba98e92442e6beec2e5cd1900752.Value = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"2");

                var Setter_8a7a4e48464041d08d382e3719de61c4 = new global::Windows.UI.Xaml.Setter();
                Setter_8a7a4e48464041d08d382e3719de61c4.Property = global::Windows.UI.Xaml.Controls.ChildWindow.BorderBrushProperty;
                Setter_8a7a4e48464041d08d382e3719de61c4.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFE2E2E2");

                var Setter_cafb4a20ed0f48ff82bb4ced149a1182 = new global::Windows.UI.Xaml.Setter();
                Setter_cafb4a20ed0f48ff82bb4ced149a1182.Property = global::Windows.UI.Xaml.Controls.ChildWindow.OverlayBrushProperty;
                Setter_cafb4a20ed0f48ff82bb4ced149a1182.Value = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#7F000000");

                var Setter_664d44e6d0f6438e99e169fcce9e403b = new global::Windows.UI.Xaml.Setter();
                Setter_664d44e6d0f6438e99e169fcce9e403b.Property = global::Windows.UI.Xaml.Controls.ChildWindow.OverlayOpacityProperty;
                Setter_664d44e6d0f6438e99e169fcce9e403b.Value = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"1");

                var Setter_d7ef8f0592b24004be39893a40f27d1b = new global::Windows.UI.Xaml.Setter();
                Setter_d7ef8f0592b24004be39893a40f27d1b.Property = global::Windows.UI.Xaml.Controls.ChildWindow.TemplateProperty;
                var ControlTemplate_b661c9bca5874e93b66ba37eec850d96 = new global::Windows.UI.Xaml.Controls.ControlTemplate();
                ControlTemplate_b661c9bca5874e93b66ba37eec850d96.TargetType = typeof(global::Windows.UI.Xaml.Controls.ChildWindow);
                ControlTemplate_b661c9bca5874e93b66ba37eec850d96.SetMethodToInstantiateFrameworkTemplate(Instantiate_ControlTemplate_b661c9bca5874e93b66ba37eec850d96);

                Setter_d7ef8f0592b24004be39893a40f27d1b.Value = ControlTemplate_b661c9bca5874e93b66ba37eec850d96;


                Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_647be68aec824ee591116fc20dddbce2);
                Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_9bd7e4b2eb8c43568d060436581f27c0);
                Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_631d2af001a04b5f920dff56742afaf5);
                Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_62140b5b875e4d62ad48551d1f789d4c);
                Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_82acba98e92442e6beec2e5cd1900752);
                Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_8a7a4e48464041d08d382e3719de61c4);
                Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_cafb4a20ed0f48ff82bb4ced149a1182);
                Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_664d44e6d0f6438e99e169fcce9e403b);
                Style_64ad7c9c63ed4e8bbaa7059400c08a6f.Setters.Add(Setter_d7ef8f0592b24004be39893a40f27d1b);


                DefaultStyle = Style_64ad7c9c63ed4e8bbaa7059400c08a6f;
            }
            return DefaultStyle;







        }



        private static global::Windows.UI.Xaml.TemplateInstance Instantiate_ControlTemplate_b661c9bca5874e93b66ba37eec850d96(global::Windows.UI.Xaml.FrameworkElement templateOwner)
        {
            var templateInstance_939af67689af491b8a43baa14bf60e61 = new global::Windows.UI.Xaml.TemplateInstance();
            templateInstance_939af67689af491b8a43baa14bf60e61.TemplateOwner = templateOwner;
            var Grid_303391c44429425e8bae3462af97f914 = new global::Windows.UI.Xaml.Controls.Grid();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Root", Grid_303391c44429425e8bae3462af97f914);
            Grid_303391c44429425e8bae3462af97f914.Name = "Root";
            var Grid_63e050e5353c4016bee3ab2703c5fc1e = new global::Windows.UI.Xaml.Controls.Grid();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Overlay", Grid_63e050e5353c4016bee3ab2703c5fc1e);
            Grid_63e050e5353c4016bee3ab2703c5fc1e.Name = "Overlay";
            Grid_63e050e5353c4016bee3ab2703c5fc1e.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch;
            Grid_63e050e5353c4016bee3ab2703c5fc1e.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Stretch;
            Grid_63e050e5353c4016bee3ab2703c5fc1e.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0");
            var Binding_8bcdb4bc316d4c158ac8af78e190cc2a = new global::Windows.UI.Xaml.Data.Binding();
            Binding_8bcdb4bc316d4c158ac8af78e190cc2a.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"OverlayBrush");
            var RelativeSource_c24bd92e11bb4c6fa4a187c912c884d2 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_c24bd92e11bb4c6fa4a187c912c884d2.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_8bcdb4bc316d4c158ac8af78e190cc2a.RelativeSource = RelativeSource_c24bd92e11bb4c6fa4a187c912c884d2;


            Binding_8bcdb4bc316d4c158ac8af78e190cc2a.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;

            var Binding_d15eb3d8d9284262826be3fd8a567ca9 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_d15eb3d8d9284262826be3fd8a567ca9.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"OverlayOpacity");
            var RelativeSource_d9907db8f1184dfa99fb31bb188df289 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_d9907db8f1184dfa99fb31bb188df289.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_d15eb3d8d9284262826be3fd8a567ca9.RelativeSource = RelativeSource_d9907db8f1184dfa99fb31bb188df289;


            Binding_d15eb3d8d9284262826be3fd8a567ca9.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;


            var Border_ac973cac7206487bbd7668d20a97126e = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ContentRoot", Border_ac973cac7206487bbd7668d20a97126e);
            Border_ac973cac7206487bbd7668d20a97126e.Name = "ContentRoot";
            Border_ac973cac7206487bbd7668d20a97126e.RenderTransformOrigin = (global::Windows.Foundation.Point)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.Foundation.Point), @"0.5,0.5");
            var Border_423d8fe602004386902fc81d1a25d89c = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ContentContainer", Border_423d8fe602004386902fc81d1a25d89c);
            Border_423d8fe602004386902fc81d1a25d89c.Name = "ContentContainer";
            Border_423d8fe602004386902fc81d1a25d89c.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFF6F6F6");
            var Grid_76c09bf7565d4851b3485501aa861ffa = new global::Windows.UI.Xaml.Controls.Grid();
            var RowDefinition_ea5dfeec04b34ce0b49cf367e17b5fa2 = new global::Windows.UI.Xaml.Controls.RowDefinition();
            RowDefinition_ea5dfeec04b34ce0b49cf367e17b5fa2.Height = (global::Windows.UI.Xaml.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.GridLength), @"Auto");

            var RowDefinition_214bd3d1974a48cb89b1678be5cee77f = new global::Windows.UI.Xaml.Controls.RowDefinition();

            Grid_76c09bf7565d4851b3485501aa861ffa.RowDefinitions.Add(RowDefinition_ea5dfeec04b34ce0b49cf367e17b5fa2);
            Grid_76c09bf7565d4851b3485501aa861ffa.RowDefinitions.Add(RowDefinition_214bd3d1974a48cb89b1678be5cee77f);

            var Border_efed7fad940e42e4b9f2ef7f99241750 = new global::Windows.UI.Xaml.Controls.Border();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("Chrome", Border_efed7fad940e42e4b9f2ef7f99241750);
            Border_efed7fad940e42e4b9f2ef7f99241750.Name = "Chrome";
            Border_efed7fad940e42e4b9f2ef7f99241750.Height = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"32");
            Border_efed7fad940e42e4b9f2ef7f99241750.BorderBrush = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFE2E2E2");
            Border_efed7fad940e42e4b9f2ef7f99241750.BorderThickness = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"0,0,0,1");
            Border_efed7fad940e42e4b9f2ef7f99241750.Background = (global::Windows.UI.Xaml.Media.Brush)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Media.Brush), @"#FFEEEEEE");
            var Grid_4bc9d8922d0c48ca8806241215bd56c1 = new global::Windows.UI.Xaml.Controls.Grid();
            var ColumnDefinition_3344f0b78db94da78f110d538eef1506 = new global::Windows.UI.Xaml.Controls.ColumnDefinition();

            var ColumnDefinition_536a17694c8c471d9a4dafff5ea27eeb = new global::Windows.UI.Xaml.Controls.ColumnDefinition();
            ColumnDefinition_536a17694c8c471d9a4dafff5ea27eeb.Width = (global::Windows.UI.Xaml.GridLength)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.GridLength), @"30");

            Grid_4bc9d8922d0c48ca8806241215bd56c1.ColumnDefinitions.Add(ColumnDefinition_3344f0b78db94da78f110d538eef1506);
            Grid_4bc9d8922d0c48ca8806241215bd56c1.ColumnDefinitions.Add(ColumnDefinition_536a17694c8c471d9a4dafff5ea27eeb);

            var ContentControl_ab4af4179c3c441e8107b6f04d0ab1fa = new global::Windows.UI.Xaml.Controls.ContentControl();
            ContentControl_ab4af4179c3c441e8107b6f04d0ab1fa.FontWeight = (global::Windows.UI.Text.FontWeight)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Text.FontWeight), @"Bold");
            ContentControl_ab4af4179c3c441e8107b6f04d0ab1fa.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Stretch;
            ContentControl_ab4af4179c3c441e8107b6f04d0ab1fa.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            ContentControl_ab4af4179c3c441e8107b6f04d0ab1fa.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"6,0,6,0");
            var Binding_817dffb2c6d24b56b84ba830c021eb88 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_817dffb2c6d24b56b84ba830c021eb88.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Title");
            var RelativeSource_6ba57413d247418c9c58ca29321c4248 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_6ba57413d247418c9c58ca29321c4248.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_817dffb2c6d24b56b84ba830c021eb88.RelativeSource = RelativeSource_6ba57413d247418c9c58ca29321c4248;


            Binding_817dffb2c6d24b56b84ba830c021eb88.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;


            var Button_0c5ac5e1edce4bb9b3db1859acea544a = new global::Windows.UI.Xaml.Controls.Button();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("CloseButton", Button_0c5ac5e1edce4bb9b3db1859acea544a);
            Button_0c5ac5e1edce4bb9b3db1859acea544a.Name = "CloseButton";
            Button_0c5ac5e1edce4bb9b3db1859acea544a.Content = (global::System.Object)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Object), @"X");
            global::Windows.UI.Xaml.Controls.Grid.SetColumn(Button_0c5ac5e1edce4bb9b3db1859acea544a, (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"1"));
            Button_0c5ac5e1edce4bb9b3db1859acea544a.FontSize = (global::System.Double)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Double), @"16");
            Button_0c5ac5e1edce4bb9b3db1859acea544a.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center;
            Button_0c5ac5e1edce4bb9b3db1859acea544a.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            Button_0c5ac5e1edce4bb9b3db1859acea544a.HorizontalContentAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center;
            Button_0c5ac5e1edce4bb9b3db1859acea544a.VerticalContentAlignment = global::Windows.UI.Xaml.VerticalAlignment.Center;
            Button_0c5ac5e1edce4bb9b3db1859acea544a.Padding = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"8,3");

            Grid_4bc9d8922d0c48ca8806241215bd56c1.Children.Add(ContentControl_ab4af4179c3c441e8107b6f04d0ab1fa);
            Grid_4bc9d8922d0c48ca8806241215bd56c1.Children.Add(Button_0c5ac5e1edce4bb9b3db1859acea544a);


            Border_efed7fad940e42e4b9f2ef7f99241750.Child = Grid_4bc9d8922d0c48ca8806241215bd56c1;


            var Border_b2fd71baf6b847d2af116fcffe818d60 = new global::Windows.UI.Xaml.Controls.Border();
            Border_b2fd71baf6b847d2af116fcffe818d60.Margin = (global::Windows.UI.Xaml.Thickness)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.Thickness), @"7");
            global::Windows.UI.Xaml.Controls.Grid.SetRow(Border_b2fd71baf6b847d2af116fcffe818d60, (global::System.Int32)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::System.Int32), @"1"));
            var ContentPresenter_7cf499f53c6d433591f2a0beaa835702 = new global::Windows.UI.Xaml.Controls.ContentPresenter();
            ((global::Windows.UI.Xaml.Controls.Control)templateOwner).RegisterName("ContentPresenter", ContentPresenter_7cf499f53c6d433591f2a0beaa835702);
            ContentPresenter_7cf499f53c6d433591f2a0beaa835702.Name = "ContentPresenter";
            var Binding_626b78d2ba674046aa886e5b995a8132 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_626b78d2ba674046aa886e5b995a8132.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Content");
            var RelativeSource_f12ea6877eb043309d1a9a0505208986 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_f12ea6877eb043309d1a9a0505208986.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_626b78d2ba674046aa886e5b995a8132.RelativeSource = RelativeSource_f12ea6877eb043309d1a9a0505208986;


            Binding_626b78d2ba674046aa886e5b995a8132.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;

            var Binding_a6c67aca6fca43f9a67cb8fe2e00a354 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_a6c67aca6fca43f9a67cb8fe2e00a354.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"ContentTemplate");
            var RelativeSource_e044a4dd9e1341c4bbf26ae23738bd3f = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_e044a4dd9e1341c4bbf26ae23738bd3f.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_a6c67aca6fca43f9a67cb8fe2e00a354.RelativeSource = RelativeSource_e044a4dd9e1341c4bbf26ae23738bd3f;


            Binding_a6c67aca6fca43f9a67cb8fe2e00a354.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;

            var Binding_dd03fb02fdd849cab1872dbd5fd5f790 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_dd03fb02fdd849cab1872dbd5fd5f790.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"HorizontalContentAlignment");
            var RelativeSource_c0904e69c98142bbbc50373ff508e802 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_c0904e69c98142bbbc50373ff508e802.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_dd03fb02fdd849cab1872dbd5fd5f790.RelativeSource = RelativeSource_c0904e69c98142bbbc50373ff508e802;


            Binding_dd03fb02fdd849cab1872dbd5fd5f790.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;

            var Binding_767ac44d67a245849aa62d7852633ce4 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_767ac44d67a245849aa62d7852633ce4.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"VerticalContentAlignment");
            var RelativeSource_4d56ef314f3c4bd48fe7f0a29298d375 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_4d56ef314f3c4bd48fe7f0a29298d375.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_767ac44d67a245849aa62d7852633ce4.RelativeSource = RelativeSource_4d56ef314f3c4bd48fe7f0a29298d375;


            Binding_767ac44d67a245849aa62d7852633ce4.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;


            Border_b2fd71baf6b847d2af116fcffe818d60.Child = ContentPresenter_7cf499f53c6d433591f2a0beaa835702;

            var Binding_71dd525db9fa46ac8648553995cec4a5 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_71dd525db9fa46ac8648553995cec4a5.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Background");
            var RelativeSource_2203f79dda0b40afb3a74048e1b880e6 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_2203f79dda0b40afb3a74048e1b880e6.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_71dd525db9fa46ac8648553995cec4a5.RelativeSource = RelativeSource_2203f79dda0b40afb3a74048e1b880e6;


            Binding_71dd525db9fa46ac8648553995cec4a5.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;


            Grid_76c09bf7565d4851b3485501aa861ffa.Children.Add(Border_efed7fad940e42e4b9f2ef7f99241750);
            Grid_76c09bf7565d4851b3485501aa861ffa.Children.Add(Border_b2fd71baf6b847d2af116fcffe818d60);


            Border_423d8fe602004386902fc81d1a25d89c.Child = Grid_76c09bf7565d4851b3485501aa861ffa;

            var Binding_37240cd789a44c6ba1453bf29ae004c4 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_37240cd789a44c6ba1453bf29ae004c4.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderThickness");
            var RelativeSource_32feb236f88b4eba83dc8bf2f852a2d2 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_32feb236f88b4eba83dc8bf2f852a2d2.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_37240cd789a44c6ba1453bf29ae004c4.RelativeSource = RelativeSource_32feb236f88b4eba83dc8bf2f852a2d2;


            Binding_37240cd789a44c6ba1453bf29ae004c4.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;

            var Binding_7a4b805c509448059e0cf37d98ef8407 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_7a4b805c509448059e0cf37d98ef8407.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"BorderBrush");
            var RelativeSource_7f21be18791a4e85aa90f68098ef5c14 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_7f21be18791a4e85aa90f68098ef5c14.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_7a4b805c509448059e0cf37d98ef8407.RelativeSource = RelativeSource_7f21be18791a4e85aa90f68098ef5c14;


            Binding_7a4b805c509448059e0cf37d98ef8407.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;


            Border_ac973cac7206487bbd7668d20a97126e.Child = Border_423d8fe602004386902fc81d1a25d89c;

            var Binding_7e2986644cfa40e3b931ba561353e079 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_7e2986644cfa40e3b931ba561353e079.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"HorizontalAlignment");
            var RelativeSource_d32b713635404ed09f0fb8680055678f = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_d32b713635404ed09f0fb8680055678f.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_7e2986644cfa40e3b931ba561353e079.RelativeSource = RelativeSource_d32b713635404ed09f0fb8680055678f;


            Binding_7e2986644cfa40e3b931ba561353e079.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;

            var Binding_e6bc91e0f62a4474824ca0677b667ea6 = new global::Windows.UI.Xaml.Data.Binding();
            Binding_e6bc91e0f62a4474824ca0677b667ea6.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"VerticalAlignment");
            var RelativeSource_531f636fc5414b86b6ab079b0cf1c202 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_531f636fc5414b86b6ab079b0cf1c202.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_e6bc91e0f62a4474824ca0677b667ea6.RelativeSource = RelativeSource_531f636fc5414b86b6ab079b0cf1c202;


            Binding_e6bc91e0f62a4474824ca0677b667ea6.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;

            var Binding_6676b4d376394ba981b24943746da61e = new global::Windows.UI.Xaml.Data.Binding();
            Binding_6676b4d376394ba981b24943746da61e.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Height");
            var RelativeSource_a8733721dc14442eb41c74d456be8990 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_a8733721dc14442eb41c74d456be8990.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_6676b4d376394ba981b24943746da61e.RelativeSource = RelativeSource_a8733721dc14442eb41c74d456be8990;


            Binding_6676b4d376394ba981b24943746da61e.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;

            var Binding_e3e89f8c3a7949ebbe79b5d79691af5b = new global::Windows.UI.Xaml.Data.Binding();
            Binding_e3e89f8c3a7949ebbe79b5d79691af5b.Path = (global::Windows.UI.Xaml.PropertyPath)global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(typeof(global::Windows.UI.Xaml.PropertyPath), @"Width");
            var RelativeSource_a53b0729936d42d5b92cb86f8ee38322 = new global::Windows.UI.Xaml.Data.RelativeSource();
            RelativeSource_a53b0729936d42d5b92cb86f8ee38322.Mode = global::Windows.UI.Xaml.Data.RelativeSourceMode.TemplatedParent;

            Binding_e3e89f8c3a7949ebbe79b5d79691af5b.RelativeSource = RelativeSource_a53b0729936d42d5b92cb86f8ee38322;


            Binding_e3e89f8c3a7949ebbe79b5d79691af5b.TemplateOwner = templateInstance_939af67689af491b8a43baa14bf60e61;


            Grid_303391c44429425e8bae3462af97f914.Children.Add(Grid_63e050e5353c4016bee3ab2703c5fc1e);
            Grid_303391c44429425e8bae3462af97f914.Children.Add(Border_ac973cac7206487bbd7668d20a97126e);



            Grid_63e050e5353c4016bee3ab2703c5fc1e.SetBinding(global::Windows.UI.Xaml.Controls.Panel.BackgroundProperty, Binding_8bcdb4bc316d4c158ac8af78e190cc2a);
            Grid_63e050e5353c4016bee3ab2703c5fc1e.SetBinding(global::Windows.UI.Xaml.UIElement.OpacityProperty, Binding_d15eb3d8d9284262826be3fd8a567ca9);
            ContentControl_ab4af4179c3c441e8107b6f04d0ab1fa.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentProperty, Binding_817dffb2c6d24b56b84ba830c021eb88);
            ContentPresenter_7cf499f53c6d433591f2a0beaa835702.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentProperty, Binding_626b78d2ba674046aa886e5b995a8132);
            ContentPresenter_7cf499f53c6d433591f2a0beaa835702.SetBinding(global::Windows.UI.Xaml.Controls.ContentControl.ContentTemplateProperty, Binding_a6c67aca6fca43f9a67cb8fe2e00a354);
            ContentPresenter_7cf499f53c6d433591f2a0beaa835702.SetBinding(global::Windows.UI.Xaml.FrameworkElement.HorizontalAlignmentProperty, Binding_dd03fb02fdd849cab1872dbd5fd5f790);
            ContentPresenter_7cf499f53c6d433591f2a0beaa835702.SetBinding(global::Windows.UI.Xaml.FrameworkElement.VerticalAlignmentProperty, Binding_767ac44d67a245849aa62d7852633ce4);
            Border_b2fd71baf6b847d2af116fcffe818d60.SetBinding(global::Windows.UI.Xaml.Controls.Border.BackgroundProperty, Binding_71dd525db9fa46ac8648553995cec4a5);
            Border_423d8fe602004386902fc81d1a25d89c.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderThicknessProperty, Binding_37240cd789a44c6ba1453bf29ae004c4);
            Border_423d8fe602004386902fc81d1a25d89c.SetBinding(global::Windows.UI.Xaml.Controls.Border.BorderBrushProperty, Binding_7a4b805c509448059e0cf37d98ef8407);
            Border_ac973cac7206487bbd7668d20a97126e.SetBinding(global::Windows.UI.Xaml.FrameworkElement.HorizontalAlignmentProperty, Binding_7e2986644cfa40e3b931ba561353e079);
            Border_ac973cac7206487bbd7668d20a97126e.SetBinding(global::Windows.UI.Xaml.FrameworkElement.VerticalAlignmentProperty, Binding_e6bc91e0f62a4474824ca0677b667ea6);
            Border_ac973cac7206487bbd7668d20a97126e.SetBinding(global::Windows.UI.Xaml.FrameworkElement.HeightProperty, Binding_6676b4d376394ba981b24943746da61e);
            Border_ac973cac7206487bbd7668d20a97126e.SetBinding(global::Windows.UI.Xaml.FrameworkElement.WidthProperty, Binding_e3e89f8c3a7949ebbe79b5d79691af5b);

            templateInstance_939af67689af491b8a43baa14bf60e61.TemplateContent = Grid_303391c44429425e8bae3462af97f914;
            return templateInstance_939af67689af491b8a43baa14bf60e61;
        }



    }
}
#endif