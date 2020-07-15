using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.VisualStudioExtension.Editor.SplittedXamlEditor
{
    [Guid(GuidList.guidEditorFactoryString)]
    public sealed class EditorFactory : IVsEditorFactory, IDisposable // Factory for creating our editor object. Extends from the IVsEditoryFactory interface
    {
        public const string Extension = ".xaml";

        private SplittedXamlEditorPackage editorPackage;
        private ServiceProvider vsServiceProvider;

        public EditorFactory(SplittedXamlEditorPackage package)
        {
            this.editorPackage = package;
        }

        #region IVsEditorFactory Members

        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp) // Used for initialization of the editor in the environment. "psp" is a pointer to the service provider. Can be used to obtain instances of other interfaces.
        {
            vsServiceProvider = new ServiceProvider(psp);
            return VSConstants.S_OK;
        }

        public object GetService(Type serviceType)
        {
            return vsServiceProvider.GetService(serviceType);
        }

        public int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
            pbstrPhysicalView = null;

            if (VSConstants.LOGVIEWID_Primary == rguidLogicalView)
                return VSConstants.S_OK;        // primary view uses NULL as pbstrPhysicalView
            else if (VSConstants.LOGVIEWID.Designer_guid == rguidLogicalView)
                return VSConstants.S_OK;        // primary view uses NULL as pbstrPhysicalView
            else if (VSConstants.LOGVIEWID.TextView_guid == rguidLogicalView) // Our editor supports FindInFiles, therefore we need to declare support for LOGVIEWID_TextView.
                return VSConstants.S_OK;        // primary view uses NULL as pbstrPhysicalView
            else
                return VSConstants.E_NOTIMPL;   // you must return E_NOTIMPL for any unrecognized rguidLogicalView values

            /*
            pbstrPhysicalView = "MainFrame";

            if (VSConstants.LOGVIEWID_Primary == rguidLogicalView)
                return VSConstants.S_OK;        // primary view uses NULL as pbstrPhysicalView
            else if (VSConstants.LOGVIEWID.Code_guid == rguidLogicalView)
                return VSConstants.S_OK;
            else if (VSConstants.LOGVIEWID.Designer_guid == rguidLogicalView)
                return VSConstants.S_OK;
            //else if (VSConstants.LOGVIEWID.Designer_guid == rguidLogicalView)
            //    return VSConstants.E_NOTIMPL;        // primary view uses NULL as pbstrPhysicalView
            else if (VSConstants.LOGVIEWID.TextView_guid == rguidLogicalView) // Our editor supports FindInFiles, therefore we need to declare support for LOGVIEWID_TextView.
                return VSConstants.S_OK;        // primary view uses NULL as pbstrPhysicalView
            else
                return VSConstants.E_NOTIMPL;   // you must return E_NOTIMPL for any unrecognized rguidLogicalView values
            */
        }

        public int Close()
        {
            return VSConstants.S_OK;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public int CreateEditorInstance(
                        uint grfCreateDoc, // Flags determining when to create the editor. Only open and silent flags are valid
                        string pszMkDocument, // path to the file to be opened
                        string pszPhysicalView, // name of the physical view
                        IVsHierarchy pvHier, // pointer to the IVsHierarchy interface
                        uint itemid, // Item identifier of this editor instance
                        System.IntPtr punkDocDataExisting, // This parameter is used to determine if a document buffer (DocData object) has already been created
                        out System.IntPtr ppunkDocView, // Pointer to the IUnknown interface for the DocView object
                        out System.IntPtr ppunkDocData, // Pointer to the IUnknown interface for the DocData object
                        out string pbstrEditorCaption, // Caption mentioned by the editor for the doc window
                        out Guid pguidCmdUI, // the Command UI Guid. Any UI element that is visible in the editor has to use this GUID. This is specified in the .vsct file.
                        out int pgrfCDW) // Flags for CreateDocumentWindow
        {
            // Initialize to null:
            ppunkDocView = IntPtr.Zero;
            ppunkDocData = IntPtr.Zero;
            pguidCmdUI = GuidList.guidEditorFactory;
            pgrfCDW = 0;
            pbstrEditorCaption = null;

            // Quit if the project is not of type "C#/XAML for HTML5":
            bool isCSharpXamlForHtml5 = IsCSharpXamlForHtml5(pszMkDocument);
            if (!isCSharpXamlForHtml5)
                return VSConstants.VS_E_UNSUPPORTEDFORMAT;

            // Validate inputs:
            if ((grfCreateDoc & (VSConstants.CEF_OPENFILE | VSConstants.CEF_SILENT)) == 0)
            {
                return VSConstants.E_INVALIDARG;
            }


            //TEST:
            if (punkDocDataExisting != IntPtr.Zero)
            {
                return VSConstants.VS_E_INCOMPATIBLEDOCDATA;
            }

            IVsTextLines textBuffer = null;

            // punkDocDataExisting is null which means the file is not yet open. We need to create a new text buffer object 

            // Get the ILocalRegistry interface so we can use it to create the text buffer from the shell's local registry:
            try
            {
                ILocalRegistry localRegistry = (ILocalRegistry)GetService(typeof(SLocalRegistry));
                if (localRegistry != null)
                {
                    IntPtr ptr;
                    Guid iid = typeof(IVsTextLines).GUID;
                    Guid CLSID_VsTextBuffer = typeof(VsTextBufferClass).GUID;
                    localRegistry.CreateInstance(CLSID_VsTextBuffer, null, ref iid, 1, out ptr);
                    try
                    {
                        textBuffer = Marshal.GetObjectForIUnknown(ptr) as IVsTextLines;
                    }
                    finally
                    {
                        Marshal.Release(ptr); // Release RefCount from CreateInstance call
                    }

                    IObjectWithSite objWSite = (IObjectWithSite)textBuffer; // It is important to site the TextBuffer object
                    if (objWSite != null)
                    {
                        Microsoft.VisualStudio.OLE.Interop.IServiceProvider oleServiceProvider = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)GetService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider));
                        objWSite.SetSite(oleServiceProvider);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Can not get IVsCfgProviderEventsHelper" + ex.Message);
                throw;
            }
            
            ppunkDocData = Marshal.GetIUnknownForObject(textBuffer);

            var editorControl = new EditorPane(textBuffer, pszMkDocument, pvHier, itemid, pszPhysicalView, ppunkDocData, vsServiceProvider);
            ppunkDocView = Marshal.GetIUnknownForObject(editorControl);

            pbstrEditorCaption = "";
            return VSConstants.S_OK;
        }

        #endregion

        bool IsCSharpXamlForHtml5(string pszMkDocument)
        {
            //-------------------------------------------------
            // Check if project is of type "C#/XAML for HTML5":
            //-------------------------------------------------
            bool isCSharpXamlForHtml5 = false; // default value.
            EnvDTE80.DTE2 dte = (EnvDTE80.DTE2)vsServiceProvider.GetService(typeof(EnvDTE.DTE));
            var projectItem = dte.Solution.FindProjectItem(pszMkDocument);
            var project = projectItem.ContainingProject;
            string projectUniqueName = project.UniqueName;
            IVsSolution solution = GetService(typeof(SVsSolution)) as IVsSolution;
            IVsHierarchy hierarchy;
            solution.GetProjectOfUniqueName(projectUniqueName, out hierarchy);
            IVsBuildPropertyStorage buildPropertyStorage = hierarchy as IVsBuildPropertyStorage;
            if (buildPropertyStorage != null)
            {
                string value;
                buildPropertyStorage.GetPropertyValue("IsCSharpXamlForHtml5", "Debug", (uint)_PersistStorageType.PST_PROJECT_FILE, out value);
                if (value != null && value.Trim().ToLower() == "true")
                    isCSharpXamlForHtml5 = true;
            }

            return isCSharpXamlForHtml5;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing)
                {
                    if (vsServiceProvider != null)
                    {
                        vsServiceProvider.Dispose();
                        vsServiceProvider = null;
                    }
                }
            }
        }
        #endregion
    }
}
