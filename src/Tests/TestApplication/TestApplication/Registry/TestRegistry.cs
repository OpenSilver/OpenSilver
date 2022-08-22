using System.Collections.Generic;

namespace TestApplication
{
    public static class TestRegistry
    {
        public static List<ITreeItem> Tests = new List<ITreeItem>();

        static TestRegistry()
        {
            Tests.Add(new Test("Sandbox", "Sandbox"));
            Tests.Add(new Test("CheckBox indeterminate", "CheckBox indeterminate"));
            Tests.Add(new Test("Double Click", "Double Click"));
            Tests.Add(new Test("Right Click", "Right Click"));
            Tests.Add(new Test("ToolTip", "ToolTip"));
            Tests.Add(new Test("ContextMenu", "ContextMenu"));
            Tests.Add(new Test("DateTime", "DateTime"));
            Tests.Add(new Test("ItemsControl", "ItemsControl"));
            Tests.Add(new Test("ComboBox", "ComboBox"));
            Tests.Add(new Test("AutoCompleteBox", "AutoCompleteBox"));
            Tests.Add(new Test("TimePicker", "TimePicker"));
            Tests.Add(new Test("Accordion", "Accordion"));
            Tests.Add(new Test("Negative Margin", "Negative Margin"));
            Tests.Add(new Test("LinQ", "LinQ"));
            Tests.Add(new Test("Opacity", "IsHitTestVisible Opacity"));
            Tests.Add(new Test("IsHitTestVisible", "IsHitTestVisible Opacity"));
            Tests.Add(new Test("ChildWindow", "ChildWindow"));
            Tests.Add(new Test("PopupZIndex", "PopupZIndex"));
            Tests.Add(new Test("Viewbox", "Viewbox"));
            Tests.Add(new Test("Frame", "Frame")); //TODO: fix it
            Tests.Add(new Test("InkPresenter", "InkPresenter"));

            TestCategory events = new TestCategory("Events");
            events.Add(new Test("Grid with multiple elements", "Events/GridWithMultipleElements"));
            events.Add(new Test("Event 1", "Events/Event1"));
            events.Add(new Test("Event 2", "Events/Event2"));
            events.Add(new Test("Attach/Detach", "Events/AttachDetach"));
            events.Add(new Test("TextChanged", "Events/TextChanged"));
            events.Add(new Test("Focus", "Events/Focus"));
            events.Add(new Test("KeyDown", "Events/KeyDown"));
            events.Add(new Test("TextInput", "Events/TextInput"));
            Tests.Add(events);

            Tests.Add(new Test("Templated TextBox", "TemplatedTextBox"));
            Tests.Add(new Test("Behavior", "Behavior"));
            Tests.Add(new Test("Binding", "Binding"));
            Tests.Add(new Test("Transform", "Transform"));
            Tests.Add(new Test("Animation", "Animation"));
            Tests.Add(new Test("MouseMove/MouseCapture", "MouseMove_MouseCapture"));
            Tests.Add(new Test("Cursor", "Cursor"));
            Tests.Add(new Test("RadioButton", "RadioButton"));
            Tests.Add(new Test("Visibility", "Visibility"));
            Tests.Add(new Test("WrapPanel", "WrapPanel"));

            TestCategory grids = new TestCategory("Grids");
            grids.Add(new Test("Grid", "Grids/Grid"));
            grids.Add(new Test("Grid canvas overlapping bug", "Grids/Grid_canvas_overlapping_bug"));
            grids.Add(new Test("Grid Span", "Grids/Grid_Span"));
            grids.Add(new Test("Grid Splitter", "Grids/GridSplitter"));
            grids.Add(new Test("Alignment", "Grids/Alignment"));
            grids.Add(new Test("Grid Without Columns/Rows", "Grids/GridWithoutColumnsRows"));
            Tests.Add(grids);

            Tests.Add(new Test("DockPanel", "DockPanel"));
            Tests.Add(new Test("Canvas", "Canvas"));
            
            TestCategory alignments = new TestCategory("Alignments");
            alignments.Add(new Test("Bottom Right", "Alignments/BottomRight"));
            alignments.Add(new Test("Top Left", "Alignments/TopLeft"));
            alignments.Add(new Test("Center", "Alignments/Center"));
            alignments.Add(new Test("Stretch", "Alignments/Stretch"));
            Tests.Add(alignments);

            TestCategory stackpanels = new TestCategory("StackPanels");
            stackpanels.Add(new Test("Horizontal", "StackPanels/Horizontal"));
            stackpanels.Add(new Test("Vertical", "StackPanels/Vertical"));
            Tests.Add(stackpanels);
            
            Tests.Add(new Test("ScrollViewer", "ScrollViewer"));
            Tests.Add(new Test("ShadowDropEffect", "ShadowDropEffect"));

            TestCategory gradientBrushes = new TestCategory("GradientBrushes");
            gradientBrushes.Add(new Test("LinearGradientBrush", "GradientBrushes/LinearGradientBrush"));
            gradientBrushes.Add(new Test("RadialGradientBrush", "GradientBrushes/RadialGradientBrush"));
            Tests.Add(gradientBrushes);

            TestCategory paths = new TestCategory("Paths");
            paths.Add(new Test("Path", "Paths/Path"));
            paths.Add(new Test("Path Change", "Paths/PathChange"));
            Tests.Add(paths);

            Tests.Add(new Test("Style", "Style"));

            TestCategory listboxes = new TestCategory("ListBoxes");
            listboxes.Add(new Test("ListBox 1", "ListBoxes/ListBox1"));
            listboxes.Add(new Test("ListBox 2", "ListBoxes/ListBox2"));
            Tests.Add(listboxes);

            Tests.Add(new Test("IsolatedStorage", "IsolatedStorage"));
            Tests.Add(new Test("FileInfo", "FileInfo"));
            Tests.Add(new Test("Encoding.GetDecoder()", "Encoding_GetDecoder"));

            TestCategory datagrids = new TestCategory("DataGrids");
            datagrids.Add(new Test("DataGrid 1", "DataGrids/DataGrid1"));
            datagrids.Add(new Test("DataGrid 2", "DataGrids/DataGrid2"));
            datagrids.Add(new Test("DataGridColumn.Visibility", "DataGrids/DataGridColumn_Visibility"));
            Tests.Add(datagrids);

            Tests.Add(new Test("ICommand", "ICommand"));

            TestCategory validation = new TestCategory("Validation");
            validation.Add(new Test("Validation", "Validation"));
            validation.Add(new Test("ValidationSummary", "ValidationSummary"));
            Tests.Add(validation);

            Tests.Add(new Test("MediaElement", "MediaElement"));
            Tests.Add(new Test("Video Player", "VideoPlayer"));
            Tests.Add(new Test("Composite Controls", "CompositeControls"));
            Tests.Add(new Test("Nested Elements", "NestedElements"));
            Tests.Add(new Test("Printing", "Printing"));
            Tests.Add(new Test("PrintDocument", "PrintDocument"));
            Tests.Add(new Test("Text Properties", "TextProperties"));
            Tests.Add(new Test("Font Precedence", "FontPrecedence"));
            Tests.Add(new Test("Async/Await", "AsyncAwait"));
            Tests.Add(new Test("Image", "Image"));
            Tests.Add(new Test("Custom Control", "CustomControl"));
            Tests.Add(new Test("(De)Serialization", "DeSerialization"));
            Tests.Add(new Test("Two TextBoxes Horizontally", "TwoTextBoxesHorizontally"));
            Tests.Add(new Test("TextBox Properties", "TextBoxProperties"));

			TestCategory shapes = new TestCategory("Shapes");
			shapes.Add(new Test("Polygon", "Shapes/Polygon"));
			Tests.Add(shapes);

            Tests.Add(new Test("OpenFileDialog", "OpenFileDialog"));
        }
    }
}
