using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;

namespace DotNetForHtml5.VisualStudioExtension.Editor.XamlDesigner
{
    /// <summary>
    /// Interaction logic for XamlDesignerControl.xaml
    /// </summary>
    public partial class XamlDesignerControl : UserControl, IDisposable
    {
        static readonly XNamespace DefaultXamlNamespace = @"http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        DispatcherTimer _timerToRefresh = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 1) };
        string _sourceXaml;
        string _sourceXamlOfLastRefresh;
        string _sourceXamlFileNameAndPath;
        Dictionary<string, EnvDTE.Project> _assemblyNameToProjectDictionary;
        ResourcesCache _resourcesCache = new ResourcesCache(); // This avoids reloading the ResourceDictionaries referenced by the XAML file (such as with "MergedDictionaries" or in App.xaml) at every refresh, to improve performance.
        bool _nextRefreshRequiresUserToExplicitelyClickRefresh; // This is useful in case of DataTemplate or ControlTemplate error, which results in a message box (because only with a message box the UnhandledException does not loop). We introduced this boolean to avoid displaying too many message boxes: we wait for the user to deliberately attempt to refresh.
        EnvDTE.Project _currentProject;
        EnvDTE.Solution _currentSolution;

        const string ErrorMessageTitleTemplate = "The XAML code below is badly formatted or invalid: {0}";
        const string FailureMessageTitleTemplate = "The user interface cannot be previewed.";
        const string FailureMessageSubTitleTemplate = "Possible cause: {0}\r\n\r\nNOTE: if you are using a custom type or a user control at that location, please ignore this message because custom types and user controls cannot be previewed in this version of the XAML editor.";
        // Commented because it is now supported since Beta 11.8:
        //const string FailureMessageSubTitleForResourceDictionary = "The current version of the XAML Editor is unable to preview resources not defined in the same XAML file. The app may still run as expected.";
        const string TipsTextBlockFormat_One = "{0} migration tip";
        const string TipsTextBlockFormat_MoreThanOne = "{0} migration tips";

        // For memory and speed optimization, keep a dictionary of types:
        static Dictionary<string, Type> _typesAlreadyFound = new Dictionary<string, Type>();

        public XamlDesignerControl(EnvDTE.Project currentProject, EnvDTE.Solution currentSolution, string sourceXamlFileNameAndPath)
        {
            InitializeComponent();

            WarningsToggleButton.Visibility = Visibility.Collapsed;
            WarningsToggleButton.IsChecked = false;

            LoadDisplaySize();
            UpdatePreviewSizeBasedOnCurrentState();

            _timerToRefresh.Tick += _timerToRefresh_Tick;
            Application.Current.DispatcherUnhandledException += Application_DispatcherUnhandledException;

            _currentProject = currentProject;
            _currentSolution = currentSolution;
            _sourceXamlFileNameAndPath = sourceXamlFileNameAndPath;

            // Make a dictionary that contains all the assembly names and their corresponding projects:
            _assemblyNameToProjectDictionary = MakeAssemblyNameToProjectDictionary(_currentProject);
        }

        static Dictionary<string, EnvDTE.Project> MakeAssemblyNameToProjectDictionary(EnvDTE.Project currentProject)
        {
            // This method returns a dictionary that contains all the assembly names and their corresponding projects (among the referenced projects and the project itself):

            var assemblyNameToProjectDictionary = new Dictionary<string, EnvDTE.Project>();
            foreach (EnvDTE.Project proj in SolutionProjectsHelper.GetReferencedProjets(currentProject, includeTheProjectItselfAmondTheResults: true))
            {
                if (proj.Properties != null) // This may be null if the project is still loading. //todo: in that case, should we notify the user, do something else?
                {
                    EnvDTE.Property property = proj.Properties.Item("AssemblyName");
                    if (property != null && property.Value != null)
                    {
                        string assemblyName = property.Value.ToString();
                        assemblyNameToProjectDictionary[assemblyName] = proj; // If an item with the same key exists, it will be replaced, otherwise it will be added.
                    }
                }
            }

            return assemblyNameToProjectDictionary;
        }

        void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("XAML Designer Error: " + e.Exception.Message + (e.Exception.InnerException != null ? Environment.NewLine + Environment.NewLine + e.Exception.InnerException.Message : "") + Environment.NewLine + Environment.NewLine + "Close and re-open the XAML designer to refresh.");
            e.Handled = true; // Note: this would not be enough if we didn't use it in conjunction with "ControlForWrappingLaterXamlExceptions.cs" (otherwise it would result in an infinite UnhandledException loop).
            // Also see comments in the class "ControlForWrappingLaterXamlExceptions.cs"
            // Also see comments where the variable "_nextRefreshRequiresUserToExplicitelyClickRefresh" is defined.
            _timerToRefresh.Stop();
            _nextRefreshRequiresUserToExplicitelyClickRefresh = true;
        }

        void PrepareAndShowError(Exception e, string sourceXaml)
        {
            // Commented because it is now supported since Beta 11.8:
            //if (e.Message != null && e.Message.Contains("System.Windows.ResourceDictionary"))
            //{
            //    ShowError(FailureMessageTitleTemplate, FailureMessageSubTitleForResourceDictionary + Environment.NewLine + Environment.NewLine + e.Message + (e.InnerException != null ? Environment.NewLine + Environment.NewLine + e.InnerException.Message ?? "" : ""));
            //}
            //else if (e.Message != null && e.Message.Contains("System.Windows.StaticResourceExtension"))
            //{
            //    ShowError(FailureMessageTitleTemplate, FailureMessageSubTitleForResourceDictionary + Environment.NewLine + Environment.NewLine + (e.InnerException != null ? e.InnerException.Message ?? "" : e.Message));
            //}

            if (sourceXaml.StartsWith("<ResourceDictionary"))
            {
                ShowError("Resource dictionaries cannot be previewed.", "");
            }
            else
            {
                ShowError(FailureMessageTitleTemplate, string.Format(FailureMessageSubTitleTemplate, ConvertExceptionToString(e)), removeLineNumbers: true);
            }
        }

        static string ConvertExceptionToString(Exception ex)
        {
            return ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message ?? "" : "");
        }

        public void Refresh(string sourceXaml)
        {
            _sourceXaml = sourceXaml;

            // Start timer to refresh: (this prevents refreshing too often)
            _timerToRefresh.Stop(); // This is to reset the delay so as to postpone if called multiple times.
            _timerToRefresh.Start();
        }

        void _timerToRefresh_Tick(object sender, EventArgs e)
        {
            _timerToRefresh.Stop();
            if (!_nextRefreshRequiresUserToExplicitelyClickRefresh) // See comment where this variable is defined.
            {
                ImmediateRefresh();
            }
            else
            {
                ShowButtonToRefresh();
            }
        }

        void ImmediateRefresh()
        {
            if (_sourceXaml != _sourceXamlOfLastRefresh)
            {
                _sourceXamlOfLastRefresh = _sourceXaml;
                TheControlForWrappingLaterXamlExceptions.Content = null;
                ShowMainView();

                //Note: when adding namespaces to the following line, you probably also want to add them to the similar declaration in the "Model.cs" class.
                //string sourceXaml = @"<Border " + GetUsefulAttributesFromXamlRoot() + @" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:Silverlight3Toolkit=""clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Toolkit"">" + _model.XamlSource + @"</Border>";
                object obj;

                string processedXaml;
                string errorsIfAny;
                HashSet<string> warningsAndTips;
                if (ProcessXamlForDisplay(_sourceXaml, _sourceXamlFileNameAndPath, _currentProject, _currentSolution, _assemblyNameToProjectDictionary, _resourcesCache, out processedXaml, out errorsIfAny, out warningsAndTips))
                {
                    //ShowError("DEBUG", processedXaml);
                    //return;

                    bool thereWasAnException = false;
                    try
                    {
                        using (StringReader stringReader = new StringReader(processedXaml))
                        {
                            using (XmlReader xmlReader = XmlReader.Create(stringReader))
                            {
                                obj = XamlReader.Load(xmlReader);
                            }
                        }
                        if (obj is FrameworkElement)
                        {
                            FrameworkElement root = obj as FrameworkElement;
                            TheControlForWrappingLaterXamlExceptions.Content = root;
                        }
                        else
                        {
                            //todo: what are the following lines for?
                            TextBlock txtblk = new TextBlock();
                            txtblk.Text = obj.GetType().ToString();
                            TheControlForWrappingLaterXamlExceptions.Content = txtblk;
                        }
                    }
                    catch (Exception ex)
                    {
                        PrepareAndShowError(ex, _sourceXaml);
                        obj = null;
                        thereWasAnException = true;
                    }
                    if (obj == null && !thereWasAnException)
                    {
                        ShowError("NULL object", "");
                    }
                }
                else
                {
                    ShowError(string.Format(ErrorMessageTitleTemplate, errorsIfAny), "");
                }

                // Display/Hide warnings and tips:
                if (warningsAndTips.Count > 0)
                {
                    WarningsItemsControl.ItemsSource = warningsAndTips;
                    WarningsToggleButton.Visibility = Visibility.Visible;
                    TipsTextBlock.Text = string.Format((warningsAndTips.Count > 1 ? TipsTextBlockFormat_MoreThanOne : TipsTextBlockFormat_One), warningsAndTips.Count.ToString());
                }
                else
                {
                    WarningsToggleButton.Visibility = Visibility.Collapsed;
                    WarningsToggleButton.IsChecked = false; // This will collapse the visibility of the items which are data-bound to the ToggleButton.
                    WarningsItemsControl.ItemsSource = null;
                }
            }
        }

        static bool ProcessXamlForDisplay(string sourceXaml, string sourceXamlFileNameAndPath, EnvDTE.Project currentProject, EnvDTE.Solution currentSolution, Dictionary<string, EnvDTE.Project> assemblyNameToProjectDictionary, ResourcesCache resourcesCache, out string outputXaml, out string errorsIfAny, out HashSet<string> warningsAndTips)
        {
            warningsAndTips = new HashSet<string>();

            // Default output values:
            outputXaml = sourceXaml;
            errorsIfAny = "";

            //---------------------------------------
            // Remove the content of all the "HtmlPresenter" nodes, because the content may be not well formatted and may lead to a syntax error when parsing the XDocument:
            //---------------------------------------
            sourceXaml = HtmlPresenterRemover.RemoveHtmlPresenterNodes(sourceXaml);

            //---------------------------------------
            // Read the XDocument:
            //---------------------------------------
            System.Xml.Linq.XDocument xdoc;
            try
            {
                xdoc = System.Xml.Linq.XDocument.Parse(sourceXaml);
            }
            catch (Exception ex)
            {
                errorsIfAny = ex.Message;
                return false;
            }

            //---------------------------------------
            // Remove the "x:Class" attribute if any:
            //---------------------------------------

            if (xdoc.Root != null)
            {
                // Remove the "x:Class" attribute if any:
                XNamespace xNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
                XAttribute classAttributeIfAny = xdoc.Root.Attribute(xNamespace + "Class");
                if (classAttributeIfAny != null)
                    classAttributeIfAny.Remove();
            }

            //---------------------------------------
            // Replace the root control with a UserControl (if it is not already one) and keep only the properties and attributes supported by the UserControl class:
            //---------------------------------------

            ProcessNodeToMakeItCompatibleWithWpf.ReplaceRootWithUserControl(xdoc);


            //---------------------------------------
            // Get the styles and other resources defined in App.xaml
            //---------------------------------------

            IEnumerable<XElement> appDotXamlResources;
            string appDotXamlFullPath;
            if (resourcesCache.AppDotXamlResources != null) // This avoids reloading App.xaml at every refresh, to improve performance.
            {
                // Read from cache:
                appDotXamlResources = resourcesCache.AppDotXamlResources;
                appDotXamlFullPath = resourcesCache.AppDotXamlFullPath;
            }
            else
            {
                // Attempt to find App.xaml and read it:
                appDotXamlResources = ResolvingReferencedXamlResources.GetAppDotXamlResources(currentProject, currentSolution, out appDotXamlFullPath);
                resourcesCache.AppDotXamlResources = appDotXamlResources;
                resourcesCache.AppDotXamlFullPath = appDotXamlFullPath;
            }

            //---------------------------------------
            // Resolve all the "Merged Dictionaries", and merge them so that there are no more references to other XAML files:
            //---------------------------------------

            // Process the resources defined in App.xaml:
            if (appDotXamlResources != null)
            {
                try
                {
                    foreach (XElement element in appDotXamlResources)
                    {
                        ResolvingReferencedXamlResources.ResolveAndMergeTheMergedDictionaries(element, appDotXamlFullPath, assemblyNameToProjectDictionary, new HashSet<string>(), resourcesCache);
                    }
                }
                catch (Exception ex)
                {
                    errorsIfAny = ex.Message;
                    return false;
                }
            }

            // Process the resources defined in the current file:
            try
            {
                ResolvingReferencedXamlResources.ResolveAndMergeTheMergedDictionaries(xdoc.Root, sourceXamlFileNameAndPath, assemblyNameToProjectDictionary, new HashSet<string>(), resourcesCache);
            }
            catch (Exception ex)
            {
                errorsIfAny = ex.Message;
                return false;
            }

            //---------------------------------------
            // Surround the document with a control in which we inject all the resources that we found in the end-user's "App.xaml" file:
            //---------------------------------------
            
            if (appDotXamlResources != null && appDotXamlResources.Any())
            {
                // Put those resouces in a control that will surround the xaml of the current page:
                var surroundingControlForResources = new XElement(DefaultXamlNamespace + "Border", xdoc.Root);
                surroundingControlForResources.Add(new XAttribute(XNamespace.Xmlns + "x", @"http://schemas.microsoft.com/winfx/2006/xaml")); // Note: This is required in case the XAML code contains the "x" prefix inside Markup Extensions (such as "{x:Null}"). It is not needed for the "x" prefix in elements and attributes (such as "x:Name"), because those are automatically imported when copying nodes, where as markup extensions are considered as simple strings by "Linq to XML".
                surroundingControlForResources.Add(new XAttribute("Tag", "AddedByDesignerToInjectAppDotXamlResources"));
                var resourcesContainer = new XElement(DefaultXamlNamespace + "Border.Resources");
                surroundingControlForResources.AddFirst(resourcesContainer);
                resourcesContainer.Add(appDotXamlResources);

                // Replace the document with the one surrounded by the new control:
                xdoc = new XDocument(surroundingControlForResources);
            }

#if Display_The_Processed_XAML
            // Uncomment to display the processed XAML:
            errorsIfAny = xdoc.Root.ToString();
            outputXaml = xdoc.Root.ToString();
            return false;
#endif

            //---------------------------------------
            // Iterate through the document using a Stack<XElement> (pre-order traversal, no recursion) and process the nodes:
            //---------------------------------------

            XElement root = xdoc.Root;
            Stack<System.Xml.Linq.XElement> stack = new Stack<System.Xml.Linq.XElement>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                System.Xml.Linq.XElement current = stack.Pop();
                bool elementWasReplacedWithPlaceholder = false; // default value

                //---------------------------------------
                // Process node to make it compatible with WPF (remove events such as PointerPressed, change "Page" to "UserControl", etc.):
                //---------------------------------------
                ProcessNodeToMakeItCompatibleWithWpf.Process(current, ref warningsAndTips);

                //---------------------------------------
                // Remove unknown nodes and attributes, and display "Cannot preview this element" instead:
                //---------------------------------------

                // Verify that the node is not a property:
                if (!current.Name.LocalName.Contains("."))
                {
                    bool isKnownType = false;

                    // Check to see if the element corresponds to a known type (only for elements that are supposed to be in the default namespace):
                    Type type;
                    if (current.Name.NamespaceName == DefaultXamlNamespace
                        && TryGetType(current.Name, out type))
                    {
                        isKnownType = true;

                        // List all the event handlers of the type:
                        List<string> eventHandlers = new List<string>();
                        foreach (EventInfo eventInfo in type.GetEvents())
                        {
                            eventHandlers.Add(eventInfo.Name);
                        }

                        // Duplicate the list of attributes so that when we remove one, it doesn't affect the iteration:
                        List<System.Xml.Linq.XAttribute> attributesListCopy = new List<System.Xml.Linq.XAttribute>();
                        foreach (System.Xml.Linq.XAttribute attr in current.Attributes())
                        {
                            attributesListCopy.Add(attr);
                        }

                        // Remove event handlers:
                        foreach (System.Xml.Linq.XAttribute attr in attributesListCopy)
                        {
                            // Check if the attribute is an event handler:
                            //test+= "  ||||  " + attr.Name.LocalName + "  " + attr.Name.NamespaceName + "  " + (!attr.Name.LocalName.Contains(".")).ToString() + "  " + eventHandlers.Contains(attr.Name.LocalName).ToString();
                            if (!attr.Name.LocalName.Contains(".") && eventHandlers.Contains(attr.Name.LocalName))
                            {
                                // Remove the attrbute:
                                attr.Remove();
                            }
                        }
                    }
                    else if (current.Name.NamespaceName.EndsWith("assembly=mscorlib"))
                    {
                        isKnownType = true;
                    }

                    // If not known type, replace the element with a placeholder that says "Unable to display":
                    if (!isKnownType)
                    {
                        // Create a border with inside a TextBlock that says "Unable to display":
                        System.Xml.Linq.XElement msg = new System.Xml.Linq.XElement(DefaultXamlNamespace + "Border");
                        System.Xml.Linq.XElement msgTxt = new System.Xml.Linq.XElement(DefaultXamlNamespace + "TextBlock");

                        // Add the newly created stuff to the XAML:
                        current.ReplaceWith(msg);
                        msg.Add(msgTxt);

                        // Set some attributes:
                        msg.Add(new XAttribute("Background", "DarkRed"));
                        msg.Add(new XAttribute("Opacity", "0.5"));
                        msg.Add(new XAttribute("Tag", "AddedByDesignerAsPlaceholder"));
                        msgTxt.Add(new XAttribute("Text", "Unable to preview this element"));
                        msgTxt.Add(new XAttribute("TextWrapping", "Wrap"));
                        msgTxt.Add(new XAttribute("TextAlignment", "Center"));
                        msgTxt.Add(new XAttribute("HorizontalAlignment", "Stretch"));
                        msgTxt.Add(new XAttribute("VerticalAlignment", "Center"));
                        msgTxt.Add(new XAttribute("FontSize", "12"));
                        msgTxt.Add(new XAttribute("Foreground", "#AAFFFFFF"));
                        msgTxt.Add(new XAttribute("Tag", "AddedByDesignerForRendering"));

                        // Give to that Border the same positioning information as the element that it replaces:
                        MoveAttributeIfAny("Width", current, msg);
                        MoveAttributeIfAny("Height", current, msg);
                        MoveAttributeIfAny("HorizontalAlignment", current, msg);
                        MoveAttributeIfAny("VerticalAlignment", current, msg);
                        MoveAttributeIfAny("Margin", current, msg);
                        MoveAttributeIfAny("Opacity", current, msg);
                        MoveAttributeIfAny("MaxWidth", current, msg);
                        MoveAttributeIfAny("MaxHeight", current, msg);
                        MoveAttributeIfAny("MinWidth", current, msg);
                        MoveAttributeIfAny("MinHeight", current, msg);
                        MoveAttributeIfAny("Visibility", current, msg);
                        MoveAttributeIfAny("Canvas.Left", current, msg);
                        MoveAttributeIfAny("Canvas.Top", current, msg);
                        MoveAttributeIfAny((XNamespace)"clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Toolkit" + "DockPanel.Dock", current, msg);
                        MoveAttributeIfAny((XNamespace)"http://schemas.microsoft.com/winfx/2006/xaml" + "Name", current, msg);
                        MoveAttributeIfAny((XNamespace)"http://schemas.microsoft.com/winfx/2006/xaml" + "Key", current, msg);

                        // Remember that the element was replaced:
                        elementWasReplacedWithPlaceholder = true;
                    }
                }

                // Continue with the children of this element:
                if (!elementWasReplacedWithPlaceholder)
                {
                    foreach (XElement element in current.Elements())
                    {
                        stack.Push(element);
                    }
                }
            }



            errorsIfAny = ""; //xdoc.ToString();
            outputXaml = root.ToString();
            return true;
        }

        static void MoveAttributeIfAny(System.Xml.Linq.XName attributeName, System.Xml.Linq.XElement from, System.Xml.Linq.XElement to)
        {
            System.Xml.Linq.XAttribute attr = from.Attribute(attributeName);
            if (attr != null)
            {
                // Remove the attribute from its current parent:
                attr.Remove();

                // Add the attribute to its new parent
                to.Add(attr);
            }
        }

        void ShowError(string errorTitle, string errorSubTitle, bool removeLineNumbers = false)
        {
            if (removeLineNumbers)
            {
                // Remove information about line number because it is wrong:
                errorSubTitle = Regex.Replace(errorSubTitle, @"Line\snumber.*line\sposition\s'[0-9]*'.", string.Empty);
            }

            TxtErrorTitle.Text = errorTitle;
            TxtErrorSubTitle.Text = (string.IsNullOrEmpty(errorSubTitle) ? "" : errorSubTitle);
            TxtErrorSubTitle.Visibility = (string.IsNullOrEmpty(errorSubTitle) ? Visibility.Collapsed : Visibility.Visible);
            OuterContainer.Visibility = Visibility.Collapsed;
            BorderForButtonToRefresh.Visibility = Visibility.Collapsed;
            BorderForErrorMessages.Visibility = Visibility.Visible;
        }

        void ShowButtonToRefresh()
        {
            BorderForErrorMessages.Visibility = Visibility.Collapsed;
            OuterContainer.Visibility = Visibility.Collapsed;
            BorderForButtonToRefresh.Visibility = Visibility.Visible;
        }

        void ShowMainView()
        {
            BorderForErrorMessages.Visibility = Visibility.Collapsed;
            BorderForButtonToRefresh.Visibility = Visibility.Collapsed;
            OuterContainer.Visibility = Visibility.Visible;
        }

        static bool TryGetType(System.Xml.Linq.XName xName, out Type outputType)
        {
            //todo: take advantage of the information stored inside the XName structure to better resolve the type.
            string nameToResolve = xName.LocalName;

            Assembly[] assembliesToLookInto = new Assembly[]
            {
                typeof(Button).Assembly, // Note: this is "PresentationFramework"
                typeof(SolidColorBrush).Assembly, // Note: this is "PresentationCore"
                typeof(Matrix).Assembly, // Note: this is "WindowsBase"
            };

            if (_typesAlreadyFound.ContainsKey(nameToResolve))
            {
                outputType = _typesAlreadyFound[nameToResolve];
                return true;
            }
            else
            {
                foreach (var assembly in assembliesToLookInto)
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.Name == nameToResolve)
                        {
                            outputType = type;
                            _typesAlreadyFound.Add(nameToResolve, type);
                            return true;
                        }
                    }
                }
                outputType = null;
                return false;

                /*
                string fullQualifiedNameTemplate = (new Button().GetType().AssemblyQualifiedName).Replace("Button", "{0}");
                Type elementType = System.Type.GetType(String.Format(fullQualifiedNameTemplate, nameToResolve));
                if (elementType != null)
                {
                    outputType = elementType;
                    _typesAlreadyFound.Add(nameToResolve, elementType);
                    return true;
                }
                else
                {
                    outputType = null;
                    return false;
                }
                */
            }
        }

        private void DisplaySize_Click(object sender, RoutedEventArgs e)
        {
            SaveDisplaySize();
            UpdatePreviewSizeBasedOnCurrentState();
        }

        void UpdatePreviewSizeBasedOnCurrentState()
        {
            if (MainContainer != null)
            {
                if (DisplaySize_Phone.IsChecked == true)
                {
                    //---------------------
                    // Phone Display Size
                    //---------------------

                    OptionsForDisplaySize_Phone.Visibility = Visibility.Visible;
                    OptionsForDisplaySize_Tablet.Visibility = Visibility.Collapsed;
                    OptionsForDisplaySize_Desktop.Visibility = Visibility.Collapsed;
                    PhoneDecoration1.Visibility = Visibility.Visible;
                    PhoneDecoration2.Visibility = Visibility.Visible;
                    MainBorder.Background = new SolidColorBrush(Color.FromArgb(255, 34, 34, 34));
                    MainBorder.HorizontalAlignment = HorizontalAlignment.Center;
                    MainBorder.VerticalAlignment = VerticalAlignment.Center;
                    MainScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    MainScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    MainContainer.ClipToBounds = true;

                    if (DisplaySize_Phone_Landscape.IsChecked == true)
                    {
                        SetMainContainerSize(480, 320);
                        MainContainer.Margin = new Thickness(60, 10, 60, 10);
                    }
                    else
                    {
                        SetMainContainerSize(319.9, 480); // Note: we use 319.9 here instead of 320 because there appears to be some odd graphical glitch when using the value 320.
                        MainContainer.Margin = new Thickness(10, 60, 10, 60);
                    }
                }
                else if (DisplaySize_Tablet.IsChecked == true)
                {
                    //---------------------
                    // Tablet Display Size
                    //---------------------

                    OptionsForDisplaySize_Phone.Visibility = Visibility.Collapsed;
                    OptionsForDisplaySize_Tablet.Visibility = Visibility.Visible;
                    OptionsForDisplaySize_Desktop.Visibility = Visibility.Collapsed;
                    PhoneDecoration1.Visibility = Visibility.Visible;
                    PhoneDecoration2.Visibility = Visibility.Visible;
                    MainBorder.Background = new SolidColorBrush(Color.FromArgb(255, 34, 34, 34));
                    MainBorder.HorizontalAlignment = HorizontalAlignment.Center;
                    MainBorder.VerticalAlignment = VerticalAlignment.Center;
                    MainScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    MainScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    MainContainer.ClipToBounds = true;

                    if (DisplaySize_Tablet_Landscape.IsChecked == true)
                    {
                        SetMainContainerSize(1024, 768);
                        MainContainer.Margin = new Thickness(60, 10, 60, 10);
                    }
                    else
                    {
                        SetMainContainerSize(768, 1024);
                        MainContainer.Margin = new Thickness(10, 60, 10, 60);
                    }
                }
                else if (DisplaySize_Desktop.IsChecked == true)
                {
                    //---------------------
                    // Desktop Display Size
                    //---------------------

                    OptionsForDisplaySize_Phone.Visibility = Visibility.Collapsed;
                    OptionsForDisplaySize_Tablet.Visibility = Visibility.Collapsed;
                    OptionsForDisplaySize_Desktop.Visibility = Visibility.Visible;
                    PhoneDecoration1.Visibility = Visibility.Collapsed;
                    PhoneDecoration2.Visibility = Visibility.Collapsed;
                    MainBorder.Background = new SolidColorBrush(Colors.Transparent);
                    MainBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
                    MainBorder.VerticalAlignment = VerticalAlignment.Stretch;
                    MainScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    MainScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    MainContainer.ClipToBounds = true;

                    if (DisplaySize_Desktop_1920_1080.IsChecked == true)
                    {
                        SetMainContainerSize(1920, 1080);
                    }
                    else if (DisplaySize_Desktop_1366_768.IsChecked == true)
                    {
                        SetMainContainerSize(1366, 768);
                    }
                    else
                    {
                        SetMainContainerSize(1024, 768);
                    }
                    MainContainer.Margin = new Thickness(0, 0, 0, 0);
                }
                else if (DisplaySize_SizeToContent.IsChecked == true)
                {
                    //---------------------
                    // "Size To Content" Display Size
                    //---------------------

                    OptionsForDisplaySize_Phone.Visibility = Visibility.Collapsed;
                    OptionsForDisplaySize_Tablet.Visibility = Visibility.Collapsed;
                    OptionsForDisplaySize_Desktop.Visibility = Visibility.Collapsed;
                    PhoneDecoration1.Visibility = Visibility.Collapsed;
                    PhoneDecoration2.Visibility = Visibility.Collapsed;
                    MainBorder.Background = new SolidColorBrush(Colors.Transparent);
                    MainBorder.HorizontalAlignment = HorizontalAlignment.Center;
                    MainBorder.VerticalAlignment = VerticalAlignment.Top;
                    MainScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    MainScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    MainContainer.ClipToBounds = false; // Note: this is because by default the root of a page is a Canvas, so often the user may wonder why nothing appears on screen, and the reason is that the Canvas does not take the size of its content. By not "clipping to bounds", we help the user to understand that the content is outside of the boundaries.

                    SetMainContainerSize(double.NaN, double.NaN);
                    MainContainer.Margin = new Thickness(0, 0, 0, 0);
                }
                else if (DisplaySize_FitScreen.IsChecked == true)
                {
                    //---------------------
                    // "Fit Screen" Display Size
                    //---------------------

                    OptionsForDisplaySize_Phone.Visibility = Visibility.Collapsed;
                    OptionsForDisplaySize_Tablet.Visibility = Visibility.Collapsed;
                    OptionsForDisplaySize_Desktop.Visibility = Visibility.Collapsed;
                    PhoneDecoration1.Visibility = Visibility.Collapsed;
                    PhoneDecoration2.Visibility = Visibility.Collapsed;
                    MainBorder.Background = new SolidColorBrush(Colors.Transparent);
                    MainBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
                    MainBorder.VerticalAlignment = VerticalAlignment.Stretch;
                    MainScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    MainScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    MainContainer.ClipToBounds = true;

                    SetMainContainerSize(double.NaN, double.NaN);
                    MainContainer.Margin = new Thickness(0, 0, 0, 0);
                }
                else
                {
                    MessageBox.Show("Error: no display size selected. Please report this error to the authors.");
                }
            }
        }

        void SaveDisplaySize()
        {
            //-----------
            // Display size (Phone, Tablet, or Desktop)
            //-----------
            int displaySize = 0;
            if (DisplaySize_Phone.IsChecked == true)
                displaySize = 0;
            else if (DisplaySize_Tablet.IsChecked == true)
                displaySize = 1;
            else if (DisplaySize_Desktop.IsChecked == true)
                displaySize = 2;
            else if (DisplaySize_SizeToContent.IsChecked == true)
                displaySize = 3;
            else if (DisplaySize_FitScreen.IsChecked == true)
                displaySize = 4;
            Properties.Settings.Default.DisplaySize = displaySize;

            //-----------
            // Phone orientation (Portrait or Landscape)
            //-----------
            int displaySize_Phone_Orientation = 0;
            if (DisplaySize_Phone_Portrait.IsChecked == true)
                displaySize_Phone_Orientation = 0;
            else if (DisplaySize_Phone_Landscape.IsChecked == true)
                displaySize_Phone_Orientation = 1;
            Properties.Settings.Default.DisplaySize_Phone_Orientation = displaySize_Phone_Orientation;

            //-----------
            // Tablet orientation (Portrait or Landscape)
            //-----------
            int displaySize_Tablet_Orientation = 0;
            if (DisplaySize_Tablet_Portrait.IsChecked == true)
                displaySize_Tablet_Orientation = 0;
            else if (DisplaySize_Tablet_Landscape.IsChecked == true)
                displaySize_Tablet_Orientation = 1;
            Properties.Settings.Default.DisplaySize_Tablet_Orientation = displaySize_Tablet_Orientation;

            //-----------
            // Desktop resolution
            //-----------
            int displaySize_Desktop_Resolution = 0;
            if (DisplaySize_Desktop_1024_768.IsChecked == true)
                displaySize_Desktop_Resolution = 0;
            else if (DisplaySize_Desktop_1366_768.IsChecked == true)
                displaySize_Desktop_Resolution = 1;
            else if (DisplaySize_Desktop_1920_1080.IsChecked == true)
                displaySize_Desktop_Resolution = 2;
            Properties.Settings.Default.DisplaySize_Desktop_Resolution = displaySize_Desktop_Resolution;

            // SAVE:
            Properties.Settings.Default.Save();
        }

        void LoadDisplaySize()
        {
            //-----------
            // Display size (Phone, Tablet, or Desktop)
            //-----------
            int displaySize = Properties.Settings.Default.DisplaySize;
            switch (displaySize)
            {
                case 0:
                    DisplaySize_Phone.IsChecked = true;
                    break;
                case 1:
                    DisplaySize_Tablet.IsChecked = true;
                    break;
                case 2:
                    DisplaySize_Desktop.IsChecked = true;
                    break;
                case 3:
                    DisplaySize_SizeToContent.IsChecked = true;
                    break;
                default:
                case 4:
                    DisplaySize_FitScreen.IsChecked = true;
                    break;
            }

            //-----------
            // Phone orientation (Portrait or Landscape)
            //-----------
            int displaySize_Phone_Orientation = Properties.Settings.Default.DisplaySize_Phone_Orientation;
            switch (displaySize_Phone_Orientation)
            {
                case 1:
                    DisplaySize_Phone_Landscape.IsChecked = true;
                    break;
                case 0:
                default:
                    DisplaySize_Phone_Portrait.IsChecked = true;
                    break;
            }

            //-----------
            // Tablet orientation (Portrait or Landscape)
            //-----------
            int displaySize_Tablet_Orientation = Properties.Settings.Default.DisplaySize_Tablet_Orientation;
            switch (displaySize_Tablet_Orientation)
            {
                case 1:
                    DisplaySize_Tablet_Landscape.IsChecked = true;
                    break;
                case 0:
                default:
                    DisplaySize_Tablet_Portrait.IsChecked = true;
                    break;
            }

            //-----------
            // Desktop resolution
            //-----------
            int displaySize_Desktop_Resolution = Properties.Settings.Default.DisplaySize_Desktop_Resolution;
            switch (displaySize_Desktop_Resolution)
            {
                case 1:
                    DisplaySize_Desktop_1366_768.IsChecked = true;
                    break;
                case 2:
                    DisplaySize_Desktop_1920_1080.IsChecked = true;
                    break;
                case 0:
                default:
                    DisplaySize_Desktop_1024_768.IsChecked = true;
                    break;
            }
        }

        void SetMainContainerSize(double width, double height)
        {
            MainContainer.Width = width;
            MainContainer.Height = height;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _timerToRefresh.Stop();
            _timerToRefresh = null;
            Application.Current.DispatcherUnhandledException -= Application_DispatcherUnhandledException;
        }

        #endregion

        private void ButtonCloseWarnings_Click(object sender, RoutedEventArgs e)
        {
            WarningsToggleButton.IsChecked = false;
        }

        private void WarningsHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        private void ButtonClickToRefresh_Click(object sender, RoutedEventArgs e)
        {
            _nextRefreshRequiresUserToExplicitelyClickRefresh = false;
            _timerToRefresh.Stop();
            ImmediateRefresh();
        }

        void ControlForWrappingLaterXamlExceptions_ExceptionOccurred(object sender, EventArgs e)
        {
            _timerToRefresh.Stop();
            _nextRefreshRequiresUserToExplicitelyClickRefresh = true;
        }
    }
}
