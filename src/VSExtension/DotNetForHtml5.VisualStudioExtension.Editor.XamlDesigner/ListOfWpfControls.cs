using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.VisualStudioExtension.Editor.XamlDesigner
{
    static class ListOfWpfControls
    {
        public static HashSet<string> HashSetOfWpfControls;

        public static HashSet<string> GetHashSetOfWpfControls()
        {
            if (HashSetOfWpfControls == null)
            {
                HashSetOfWpfControls = new HashSet<string>()
                {
"AccessText"                                  ,
"ActiveXHost"                                 ,
"AdornedElementPlaceholder"                   ,
"AdornerDecorator"                            ,
"AdornerLayer"                                ,
"Border"                                      ,
"BulletDecorator"                             ,
"Button"                                      ,
"Calendar"                                    ,
"CalendarButton"                              ,
"CalendarDayButton"                           ,
"CalendarItem"                                ,
"Canvas"                                      ,
"CheckBox"                                    ,
"ComboBox"                                    ,
"ComboBoxItem"                                ,
"ContentControl"                              ,
"ContentPresenter"                            ,
"ContextMenu"                                 ,
"Control"                                     ,
"DataGrid"                                    ,
"DataGridCell"                                ,
"DataGridCellsPanel"                          ,
"DataGridCellsPresenter"                      ,
"DataGridColumnHeader"                        ,
"DataGridColumnHeadersPresenter"              ,
"DataGridDetailsPresenter"                    ,
"DataGridRow"                                 ,
"DataGridRowHeader"                           ,
"DataGridRowsPresenter"                       ,
"DatePicker"                                  ,
"DatePickerTextBox"                           ,
"Decorator"                                   ,
"DockPanel"                                   ,
"DocumentPageView"                            ,
"DocumentReference"                           ,
"DocumentViewer"                              ,
"Ellipse"                                     ,
"Expander"                                    ,
"FixedPage"                                   ,
"FlowDocumentPageViewer"                      ,
"FlowDocumentReader"                          ,
"FlowDocumentScrollViewer"                    ,
"Frame"                                       ,
"Glyphs"                                      ,
"Grid"                                        ,
"GridSplitter"                                ,
"GridViewColumnHeader"                        ,
"GridViewHeaderRowPresenter"                  ,
"GridViewRowPresenter"                        ,
"GroupBox"                                    ,
"GroupItem"                                   ,
"HeaderedContentControl"                      ,
"HeaderedItemsControl"                        ,
"Image"                                       ,
"InkCanvas"                                   ,
"InkPresenter"                                ,
"ItemsControl"                                ,
"ItemsPresenter"                              ,
"Label"                                       ,
"Line"                                        ,
"ListBox"                                     ,
"ListBoxItem"                                 ,
"ListView"                                    ,
"ListViewItem"                                ,
"MediaElement"                                ,
"Menu"                                        ,
"MenuItem"                                    ,
"NavigationWindow"                            ,
"Page"                                        ,
"PageContent"                                 ,
"PageFunction`1"                              ,
"PasswordBox"                                 ,
"Path"                                        ,
"Polygon"                                     ,
"Polyline"                                    ,
"Popup"                                       ,
"ProgressBar"                                 ,
"RadioButton"                                 ,
"Rectangle"                                   ,
"RepeatButton"                                ,
"ResizeGrip"                                  ,
"RichTextBox"                                 ,
"ScrollBar"                                   ,
"ScrollContentPresenter"                      ,
"ScrollViewer"                                ,
"SelectiveScrollingGrid"                      ,
"Separator"                                   ,
"Slider"                                      ,
"StackPanel"                                  ,
"StatusBar"                                   ,
"StatusBarItem"                               ,
"StickyNoteControl"                           ,
"TabControl"                                  ,
"TabItem"                                     ,
"TabPanel"                                    ,
"TextBlock"                                   ,
"TextBox"                                     ,
"Thumb"                                       ,
"TickBar"                                     ,
"ToggleButton"                                ,
"ToolBar"                                     ,
"ToolBarOverflowPanel"                        ,
"ToolBarPanel"                                ,
"ToolBarTray"                                 ,
"ToolTip"                                     ,
"Track"                                       ,
"TreeView"                                    ,
"TreeViewItem"                                ,
"UniformGrid"                                 ,
"UserControl"                                 ,
"Viewbox"                                     ,
"Viewport3D"                                  ,
"VirtualizingStackPanel"                      ,
"WebBrowser"                                  ,
"Window"                                      ,
"WrapPanel"                                   
                };
            }
            return HashSetOfWpfControls;

            //---------------------------------------------------------------------------------------------
            //Note: the list above was obtained by creating a new WPF app and running the following code:
            //    List<string> l = new List<string>();
            //    Assembly a = Assembly.GetAssembly(typeof(Button));
            //    foreach (var type in a.GetTypes())
            //    {
            //        if (type.IsSubclassOf(typeof(FrameworkElement)) && !type.IsAbstract && type.IsPublic)
            //        {
            //            l.Add(type.Name);
            //        }
            //    }
            //    l.Sort();
            //    string res = String.Join(Environment.NewLine, l);
            //    MessageBox.Show(res);
            //---------------------------------------------------------------------------------------------
        }

        public static HashSet<string> HashSetOfUserControlAttributes;

        public static HashSet<string> GetHashSetOfUserControlAttributes()
        {
            if (HashSetOfUserControlAttributes == null)
            {
                HashSetOfUserControlAttributes = new HashSet<string>()
                {
"AllowDrop"                      ,
"Background"                     ,
"BindingGroup"                   ,
"BitmapEffect"                   ,
"BitmapEffectInput"              ,
"BorderBrush"                    ,
"BorderThickness"                ,
"CacheMode"                      ,
"Clip"                           ,
"ClipToBounds"                   ,
"Content"                        ,
"ContentStringFormat"            ,
"ContentTemplate"                ,
"ContentTemplateSelector"        ,
"ContextMenu"                    ,
"Cursor"                         ,
"DataContext"                    ,
"Effect"                         ,
"FlowDirection"                  ,
"Focusable"                      ,
"FocusVisualStyle"               ,
"FontFamily"                     ,
"FontSize"                       ,
"FontStretch"                    ,
"FontStyle"                      ,
"FontWeight"                     ,
"ForceCursor"                    ,
"Foreground"                     ,
"Height"                         ,
"HorizontalAlignment"            ,
"HorizontalContentAlignment"     ,
"InputScope"                     ,
"IsEnabled"                      ,
"IsHitTestVisible"               ,
"IsManipulationEnabled"          ,
"IsTabStop"                      ,
"Language"                       ,
"LayoutTransform"                ,
"Margin"                         ,
"MaxHeight"                      ,
"MaxWidth"                       ,
"MinHeight"                      ,
"MinWidth"                       ,
"Name"                           ,
"Opacity"                        ,
"OpacityMask"                    ,
"OverridesDefaultStyle"          ,
"Padding"                        ,
"RenderSize"                     ,
"RenderTransform"                ,
"RenderTransformOrigin"          ,
"Resources"                      ,
"SnapsToDevicePixels"            ,
"Style"                          ,
"TabIndex"                       ,
"Tag"                            ,
"Template"                       ,
"ToolTip"                        ,
"Uid"                            ,
"UseLayoutRounding"              ,
"VerticalAlignment"              ,
"VerticalContentAlignment"       ,
"Visibility"                     ,
"Width"                          
                };
            }
            return HashSetOfUserControlAttributes;

            //---------------------------------------------------------------------------------------------
            //Note: the list above was obtained by creating a new WPF app and running the following code:
            //    List<string> l = new List<string>();
            //    var t = typeof(UserControl);
            //    foreach (var member in t.GetMembers())
            //    {
            //        if (member.MemberType == MemberTypes.Field && ((FieldInfo)member).IsPublic)
            //        {
            //            l.Add(member.Name);
            //        }
            //        else if (member.MemberType == MemberTypes.Property && ((PropertyInfo)member).CanWrite && ((PropertyInfo)member).GetSetMethod(true).IsPublic)
            //        {
            //            l.Add(member.Name);
            //        }
            //    }
            //    l.Sort();
            //    string res = String.Join(Environment.NewLine, l);
            //    MessageBox.Show(res);
            //---------------------------------------------------------------------------------------------
        }

        public static HashSet<string> HashSetOfBorderAttributes;

        public static HashSet<string> GetHashSetOfBorderAttributes()
        {
            if (HashSetOfBorderAttributes == null)
            {
                HashSetOfBorderAttributes = new HashSet<string>()
                {
"AllowDrop"               ,
"Background"              ,
"BackgroundProperty"      ,
"BindingGroup"            ,
"BitmapEffect"            ,
"BitmapEffectInput"       ,
"BorderBrush"             ,
"BorderBrushProperty"     ,
"BorderThickness"         ,
"BorderThicknessProperty" ,
"CacheMode"               ,
"Child"                   ,
"Clip"                    ,
"ClipToBounds"            ,
"ContextMenu"             ,
"CornerRadius"            ,
"CornerRadiusProperty"    ,
"Cursor"                  ,
"DataContext"             ,
"Effect"                  ,
"FlowDirection"           ,
"Focusable"               ,
"FocusVisualStyle"        ,
"ForceCursor"             ,
"Height"                  ,
"HorizontalAlignment"     ,
"InputScope"              ,
"IsEnabled"               ,
"IsHitTestVisible"        ,
"IsManipulationEnabled"   ,
"Language"                ,
"LayoutTransform"         ,
"Margin"                  ,
"MaxHeight"               ,
"MaxWidth"                ,
"MinHeight"               ,
"MinWidth"                ,
"Name"                    ,
"Opacity"                 ,
"OpacityMask"             ,
"OverridesDefaultStyle"   ,
"Padding"                 ,
"PaddingProperty"         ,
"RenderSize"              ,
"RenderTransform"         ,
"RenderTransformOrigin"   ,
"Resources"               ,
"SnapsToDevicePixels"     ,
"Style"                   ,
"Tag"                     ,
"ToolTip"                 ,
"Uid"                     ,
"UseLayoutRounding"       ,
"VerticalAlignment"       ,
"Visibility"              ,
"Width"                   
                };
            }
            return HashSetOfBorderAttributes;

            //---------------------------------------------------------------------------------------------
            //Note: the list above was obtained by creating a new WPF app and running the following code:
            //    List<string> l = new List<string>();
            //    var t = typeof(Border);
            //    foreach (var member in t.GetMembers())
            //    {
            //        if (member.MemberType == MemberTypes.Field && ((FieldInfo)member).IsPublic)
            //        {
            //            l.Add(member.Name);
            //        }
            //        else if (member.MemberType == MemberTypes.Property && ((PropertyInfo)member).CanWrite && ((PropertyInfo)member).GetSetMethod(true).IsPublic)
            //        {
            //            l.Add(member.Name);
            //        }
            //    }
            //    l.Sort();
            //    string res = String.Join(Environment.NewLine, l);
            //    MessageBox.Show(res);
            //---------------------------------------------------------------------------------------------
        }

        public static HashSet<string> HashSetOfTextBlockAttributes;

        public static HashSet<string> GetHashSetOfTextBlockAttributes()
        {
            if (HashSetOfTextBlockAttributes == null)
            {
                HashSetOfTextBlockAttributes = new HashSet<string>()
                {
"AllowDrop"                    ,
"Background"                   ,
"BackgroundProperty"           ,
"BaselineOffset"               ,
"BaselineOffsetProperty"       ,
"BindingGroup"                 ,
"BitmapEffect"                 ,
"BitmapEffectInput"            ,
"CacheMode"                    ,
"Clip"                         ,
"ClipToBounds"                 ,
"ContextMenu"                  ,
"Cursor"                       ,
"DataContext"                  ,
"Effect"                       ,
"FlowDirection"                ,
"Focusable"                    ,
"FocusVisualStyle"             ,
"FontFamily"                   ,
"FontFamilyProperty"           ,
"FontSize"                     ,
"FontSizeProperty"             ,
"FontStretch"                  ,
"FontStretchProperty"          ,
"FontStyle"                    ,
"FontStyleProperty"            ,
"FontWeight"                   ,
"FontWeightProperty"           ,
"ForceCursor"                  ,
"Foreground"                   ,
"ForegroundProperty"           ,
"Height"                       ,
"HorizontalAlignment"          ,
"InputScope"                   ,
"IsEnabled"                    ,
"IsHitTestVisible"             ,
"IsHyphenationEnabled"         ,
"IsHyphenationEnabledProperty" ,
"IsManipulationEnabled"        ,
"Language"                     ,
"LayoutTransform"              ,
"LineHeight"                   ,
"LineHeightProperty"           ,
"LineStackingStrategy"         ,
"LineStackingStrategyProperty" ,
"Margin"                       ,
"MaxHeight"                    ,
"MaxWidth"                     ,
"MinHeight"                    ,
"MinWidth"                     ,
"Name"                         ,
"Opacity"                      ,
"OpacityMask"                  ,
"OverridesDefaultStyle"        ,
"Padding"                      ,
"PaddingProperty"              ,
"RenderSize"                   ,
"RenderTransform"              ,
"RenderTransformOrigin"        ,
"Resources"                    ,
"SnapsToDevicePixels"          ,
"Style"                        ,
"Tag"                          ,
"Text"                         ,
"TextAlignment"                ,
"TextAlignmentProperty"        ,
"TextDecorations"              ,
"TextDecorationsProperty"      ,
"TextEffects"                  ,
"TextEffectsProperty"          ,
"TextProperty"                 ,
"TextTrimming"                 ,
"TextTrimmingProperty"         ,
"TextWrapping"                 ,
"TextWrappingProperty"         ,
"ToolTip"                      ,
"Uid"                          ,
"UseLayoutRounding"            ,
"VerticalAlignment"            ,
"Visibility"                   ,
"Width"
                };
            }
            return HashSetOfTextBlockAttributes;

            //---------------------------------------------------------------------------------------------
            //Note: the list above was obtained by creating a new WPF app and running the following code:
            //    List<string> l = new List<string>();
            //    var t = typeof(TextBlock);
            //    foreach (var member in t.GetMembers())
            //    {
            //        if (member.MemberType == MemberTypes.Field && ((FieldInfo)member).IsPublic)
            //        {
            //            l.Add(member.Name);
            //        }
            //        else if (member.MemberType == MemberTypes.Property && ((PropertyInfo)member).CanWrite && ((PropertyInfo)member).GetSetMethod(true).IsPublic)
            //        {
            //            l.Add(member.Name);
            //        }
            //    }
            //    l.Sort();
            //    string res = String.Join(Environment.NewLine, l);
            //    MessageBox.Show(res);
            //---------------------------------------------------------------------------------------------
        }

        public static HashSet<string> ContentPresenterNonCompatibleWPFAttribute;

        public static HashSet<string> GetHashSetOfNonCompatibleAttributeOfContentPresenter()
        {
            if (ContentPresenterNonCompatibleWPFAttribute == null)
            {
                ContentPresenterNonCompatibleWPFAttribute = new HashSet<string>()
        {
"Foreground",
"Background",
"FontSize",
"FontWeight",
"Padding",
"HorizontalContentAlignment",
"VerticalContentAlignment",
"Template",
"BorderThickness",
"BorderBrush"
        };
            }
            return ContentPresenterNonCompatibleWPFAttribute;
        }

    }
}
