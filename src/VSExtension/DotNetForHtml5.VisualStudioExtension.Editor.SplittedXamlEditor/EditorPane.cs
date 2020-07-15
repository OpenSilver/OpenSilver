using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DotNetForHtml5.VisualStudioExtension.Editor.XamlDesigner;
using System.Windows.Forms.Integration;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.ComponentModelHost;

namespace DotNetForHtml5.VisualStudioExtension.Editor.SplittedXamlEditor
{
    public partial class EditorPane : UserControl,
        IVsWindowPane,
        IOleCommandTarget
        , IVsWindowFrameNotify
        , IVsWindowFrameNotify2
        , IVsDeferredDocView
        , IVsFindTarget      //to implement find and replace capabilities within the editor
        , IVsFindTarget2     // Implements Find and Replace capabilities within your editor
        , IVsCodeWindow      // Represents a multiple-document interface (MDI) child that contains one or more code views
        , IVsCodeWindowEx    // Provides methods to determine and customize some of the behavior of a code window
        , IVsTextEditorPropertyContainer // Manages properties of text editors
        //, IVsUIElementPane   // Implemented by packages that support creating document windows or tool windows
        /*
        , IVsTextViewEvents  // Notifier of events occurring on the text view object
        , IVsTextView        // Manages the text view of an editor window and contains methods to manage the text view. The view is essentially the editor window shown in the user interface (UI)
        , IVsTextLines
        , IVsTextBuffer
         */
    {
        IVsTextLines _textLines;
        string _fileName;
        IVsHierarchy _hierarchy;
        uint _itemid;
        string _pszPhysicalView;
        IntPtr _punkDocData;
        IVsWindowFrame _subFrame;
        IVsCodeWindow _subCodeWindow;
        Microsoft.VisualStudio.OLE.Interop.IServiceProvider _serviceProvider;
        TextLineEventListener _textLineEventListener;
        XamlDesignerControl _xamlDesignerControl;
        ServiceProvider _vsServiceProvider;

        public EditorPane(
            IVsTextLines textLines,
            string fileName,
            IVsHierarchy hierarchy,
            uint itemid,
            string pszPhysicalView,
            IntPtr punkDocData,
            ServiceProvider vsServiceProvider)
        {
            InitializeComponent();

            // Remember values:
            _textLines = textLines;
            _fileName = fileName;
            _hierarchy = hierarchy;
            _itemid = itemid;
            _pszPhysicalView = pszPhysicalView;
            _punkDocData = punkDocData;
            _vsServiceProvider = vsServiceProvider;

            // Handle VS theme colors:
            ApplyThemeColors();
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;

            // Get the current Project and Solution:
            EnvDTE80.DTE2 dte = (EnvDTE80.DTE2)_vsServiceProvider.GetService(typeof(EnvDTE.DTE));
            EnvDTE.Solution solution = dte.Solution;
            var projectItem = solution.FindProjectItem(fileName);
            EnvDTE.Project project = projectItem.ContainingProject;

            // Create and position the XAML designer control:
            _xamlDesignerControl = new XamlDesignerControl(project, solution, fileName);
            ElementHost elementHost = new ElementHost() { Dock = System.Windows.Forms.DockStyle.Fill };
            xamlDesignerContainer.Controls.Add(elementHost);
            elementHost.Child = _xamlDesignerControl;

            // Listen to changes in the text buffer:
            _textLineEventListener = new TextLineEventListener(textLines);
            _textLineEventListener.TextChanged += textLineEventListener_TextChanged;
        }

        void textLineEventListener_TextChanged(object sender, EventArgs e)
        {
            // Read text:
            string text = GetText(_textLines);

            // Update XAML design preview:
            _xamlDesignerControl.Refresh(text);
        }

        #region IVsWindowPane Members

        public int ClosePane()
        {
            // Unregister all events:
            VSColorTheme.ThemeChanged -= VSColorTheme_ThemeChanged;
            _textLineEventListener.TextChanged -= textLineEventListener_TextChanged;

            // Dispose all resources:
            _textLineEventListener.Dispose();
            _xamlDesignerControl.Dispose();

            return VSConstants.S_OK;
        }

        void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        {
            ApplyThemeColors();
        }

        public int CreatePaneWindow(IntPtr hwndParent, int x, int y, int cx, int cy, out IntPtr hwnd)
        {
            Win32Methods.SetParent(Handle, hwndParent);
            hwnd = Handle;
            Size = new System.Drawing.Size(cx - x, cy - y);

            return VSConstants.S_OK;
        }

        public int GetDefaultSize(SIZE[] pSize)
        {
            if (pSize.Length >= 1)
            {
                pSize[0].cx = Size.Width;
                pSize[0].cy = Size.Height;
            }
            return VSConstants.S_OK;
        }

        public int LoadViewState(IStream pStream)
        {
            return VSConstants.S_OK;
        }

        public int SaveViewState(IStream pStream)
        {
            return VSConstants.S_OK;
        }

        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            _serviceProvider = psp;

            if (_subCodeWindow == null)
                LoadXamlEditor();

            return VSConstants.S_OK;
        }

        public int TranslateAccelerator(MSG[] lpmsg)
        {
            // todo: add keyboard accelerator handling here
            //return VSConstants.S_FALSE;
            return ((IVsWindowPane)_subCodeWindow).TranslateAccelerator(lpmsg);
        }

        #endregion

        #region IOleCommandTarget Members

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (_subCodeWindow != null)
                return ((IOleCommandTarget)_subCodeWindow).Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            else
                return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (_subCodeWindow != null)
                return ((IOleCommandTarget)_subCodeWindow).QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
            else
                return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;
        }

        #endregion

        #region IVsDeferredDocView Members

        //Note: Not sure if it is useful...
        public int get_CmdUIGuid(out Guid pGuidCmdId)
        {
            pGuidCmdId = GuidList.guidEditorCmdSet;
            return VSConstants.S_OK;
        }

        //Note: Not sure if it is useful...
        public int get_DocView(out IntPtr ppUnkDocView)
        {
            ppUnkDocView = Marshal.GetIUnknownForObject(this);
            return VSConstants.S_OK;
        }

        #endregion

        void LoadXamlEditor()
        {
            //Guid editorGuid = new Guid("FA3CD31E-987B-443A-9B81-186104E8DAC1"); // XML Editor
            //Guid editorGuid = new Guid("DEE6CEF9-3BCA-449A-82A6-FC757D6956FB"); // XSD Editor
            //Guid editorGuid = new Guid("f11acc28-31d1-4c80-a34b-f4e09d3d753c"); // XAML UI Designer (TabbedViewEditorFactory)
            //Guid editorGuid = new Guid("a4f9ff65-a78c-4650-866d-5069cc4127cf"); // XAML Text Editor (XamlTabEditorFactory)
            Guid editorGuid = VSConstants.VsEditorFactoryGuid.TextEditor_guid;

            IVsWindowFrame frame;
            IVsProject3 vsProject = (IVsProject3)_hierarchy;
            Guid _emptyGuid = Guid.Empty;
            uint editorFlags = (uint)__VSSPECIFICEDITORFLAGS.VSSPECIFICEDITOR_UseEditor | (uint)__VSSPECIFICEDITORFLAGS.VSSPECIFICEDITOR_DoOpen;

            ErrorHandler.ThrowOnFailure(
                vsProject.OpenItemWithSpecific(
                _itemid,
                editorFlags,
                ref editorGuid,
                _pszPhysicalView,
                ref _emptyGuid,
                _punkDocData,
                out frame));

            IVsWindowFrame windowFrameOrig = GetFrame();
            Debug.Assert(windowFrameOrig != null);
            ErrorHandler.ThrowOnFailure(frame.SetProperty((int)__VSFPROPID2.VSFPROPID_ParentFrame, windowFrameOrig));
            ErrorHandler.ThrowOnFailure(frame.SetProperty((int)__VSFPROPID2.VSFPROPID_ParentHwnd, GetChildContainerHandle()));
            //ErrorHandler.ThrowOnFailure(frame.SetProperty((int)__VSFPROPID.VSFPROPID_pszPhysicalView, "MainFrame"));
            _subFrame = frame;
            var IID_IVsCodeWindow = typeof(IVsCodeWindow).GUID;
            IntPtr intPtr;
            _subFrame.QueryViewInterface(ref IID_IVsCodeWindow, out intPtr);
            _subCodeWindow = Marshal.GetObjectForIUnknown(intPtr) as IVsCodeWindow;
            //ErrorHandler.ThrowOnFailure(((IVsWindowFrame2)frame).ActivateOwnerDockedWindow());

            /*
            IVsTextView vsTextView;
            ((IVsCodeWindow)_subCodeWindow).GetPrimaryView(out vsTextView);

            // Enable the "Quick Find" feature of Visual Studio:
            EnableAutonomousFind(windowFrameOrig, vsTextView);
            EnableAutonomousFind(_subFrame, vsTextView);

            foreach (Type theInterface in _subCodeWindow.GetType().GetInterfaces())
            {
                string name = theInterface.Name;
                string toString = theInterface.ToString();
            }
            */
        }

        void ShowXamlEditor()
        {
            ErrorHandler.ThrowOnFailure(_subFrame.Show());
        }

        IVsWindowFrame GetFrame()
        {
            IVsWindowFrame windowFrame = null;
            if (_serviceProvider != null)
            {
                ServiceProvider sp = new ServiceProvider(_serviceProvider);
                windowFrame = sp.GetService(typeof(SVsWindowFrame)) as IVsWindowFrame;
            }
            return windowFrame;
        }

        IntPtr GetChildContainerHandle()
        {
            return xamlTextEditorContainer.Handle;
        }

        #region IVsWindowFrameNotify2 Members

        public int OnClose(ref uint pgrfSaveOptions)
        {
            var result = _subFrame.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_PromptSave);

            splitContainer1.Visible = false;

            return result;
        }

        #endregion

        #region IVsWindowFrameNotify Members

        public int OnDockableChange(int fDockable)
        {
            return VSConstants.S_OK;
        }

        public int OnMove()
        {
            return VSConstants.S_OK;
        }

        public int OnShow(int fShow)
        {
            if (fShow == (int)__FRAMESHOW.FRAMESHOW_WinShown)
                ShowXamlEditor();

            return VSConstants.S_OK;
        }

        public int OnSize()
        {
            return VSConstants.S_OK;
        }

        #endregion

        void UpdateSubFrameSize()
        {
            Guid nullGuid = Guid.Empty;
            if (_subFrame != null)
                ErrorHandler.ThrowOnFailure(_subFrame.SetFramePos(VSSETFRAMEPOS.SFP_fSize, ref nullGuid, xamlTextEditorContainer.Left, xamlTextEditorContainer.Top, xamlTextEditorContainer.Width, xamlTextEditorContainer.Height));
        }

        private void xamlTextEditorContainer_Resize(object sender, EventArgs e)
        {
            UpdateSubFrameSize();
        }

        void ApplyThemeColors()
        {
            var backColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
            var foreColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);
            this.BackColor = backColor;
            this.ForeColor = foreColor;
        }

        static string GetText(IVsTextLines buffer)
        {
            // Create span for all lines:
            TextSpan entireSpan = new TextSpan();
            buffer.GetLastLineIndex(out entireSpan.iEndLine, out entireSpan.iEndIndex);

            // Get text:
            string text;
            buffer.GetLineText(entireSpan.iStartLine, entireSpan.iStartIndex, entireSpan.iEndLine, entireSpan.iEndIndex, out text);

            return text;
        }


        #region IVsFindTarget Members

        /// <summary>
        /// Return the object that was requested
        /// </summary>
        /// <param name="propid">Id of the requested object</param>
        /// <param name="pvar">Object returned</param>
        /// <returns>HResult</returns>
        int IVsFindTarget.GetProperty(uint propid, out object pvar)
        {
            return ((IVsFindTarget)_subCodeWindow).GetProperty(propid, out pvar);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grfOptions"></param>
        /// <param name="ppSpans"></param>
        /// <param name="ppTextImage"></param>
        int IVsFindTarget.GetSearchImage(uint grfOptions, IVsTextSpanSet[] ppSpans, out IVsTextImage ppTextImage)
        {
            return ((IVsFindTarget)_subCodeWindow).GetSearchImage(grfOptions, ppSpans, out ppTextImage);
        }

        /// <summary>
        /// Retrieve a previously stored object
        /// </summary>
        /// <returns>The object that is being asked</returns>
        int IVsFindTarget.GetFindState(out object ppunk)
        {
            return ((IVsFindTarget)_subCodeWindow).GetFindState(out ppunk);
        }

        /// <summary>
        /// Search for the string in the text of our editor.
        /// Options specify how we do the search. No need to implement this since we implement IVsTextImage
        /// </summary>
        /// <param name="pszSearch">Search string</param>
        /// <param name="grfOptions">Search options</param>
        /// <param name="fResetStartPoint">Is this a new search?</param>
        /// <param name="pHelper">We are not using it</param>
        /// <param name="pResult">True if we found the search string</param>
        int IVsFindTarget.Find(string pszSearch, uint grfOptions, int fResetStartPoint, IVsFindHelper pHelper, out uint pResult)
        {
            return ((IVsFindTarget)_subCodeWindow).Find(pszSearch, grfOptions, fResetStartPoint, pHelper, out pResult);
        }

        /// <summary>
        /// Bring the focus to a specific position in the document
        /// </summary>
        /// <param name="pts">Location where to move the cursor to</param>
        int IVsFindTarget.NavigateTo(TextSpan[] pts)
        {
            return ((IVsFindTarget)_subCodeWindow).NavigateTo(pts);
        }

        /// <summary>
        /// Get current cursor location
        /// </summary>
        /// <param name="pts">Current location</param>
        /// <returns>HResult</returns>
        int IVsFindTarget.GetCurrentSpan(TextSpan[] pts)
        {
            return ((IVsFindTarget)_subCodeWindow).GetCurrentSpan(pts);
        }

        /// <summary>
        /// Highlight a given text span. No need to implement
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        int IVsFindTarget.MarkSpan(TextSpan[] pts)
        {
            return ((IVsFindTarget)_subCodeWindow).MarkSpan(pts);
        }

        /// <summary>
        /// Replace a string in the text. No need to implement since we implement IVsTextImage
        /// </summary>
        /// <param name="pszSearch">string containing the search text</param>
        /// <param name="pszReplace">string containing the replacement text</param>
        /// <param name="grfOptions">Search options available</param>
        /// <param name="fResetStartPoint">flag to reset the search start point</param>
        /// <param name="pHelper">IVsFindHelper interface object</param>
        /// <param name="pfReplaced">returns whether replacement was successful or not</param>
        /// <returns></returns>
        int IVsFindTarget.Replace(string pszSearch, string pszReplace, uint grfOptions, int fResetStartPoint, IVsFindHelper pHelper, out int pfReplaced)
        {
            return ((IVsFindTarget)_subCodeWindow).Replace(pszSearch, pszReplace, grfOptions, fResetStartPoint, pHelper, out pfReplaced);
        }

        /// <summary>
        /// Store an object that will later be returned
        /// </summary>
        /// <returns>The object that is being stored</returns>
        int IVsFindTarget.SetFindState(object pUnk)
        {
            return ((IVsFindTarget)_subCodeWindow).SetFindState(pUnk);
        }


        /// <summary>
        /// This implementation does not use notification
        /// </summary>
        /// <param name="notification"></param>
        int IVsFindTarget.NotifyFindTarget(uint notification)
        {
            return ((IVsFindTarget)_subCodeWindow).NotifyFindTarget(notification);
        }

        /// <summary>
        /// Specify which search option we support.
        /// </summary>
        /// <param name="pfImage">Do we support IVsTextImage?</param>
        /// <param name="pgrfOptions">Supported options</param>
        int IVsFindTarget.GetCapabilities(bool[] pfImage, uint[] pgrfOptions)
        {
            return ((IVsFindTarget)_subCodeWindow).GetCapabilities(pfImage, pgrfOptions);
        }

        /// <summary>
        /// Return the Screen coordinates of the matched string. No need to implement
        /// </summary>
        /// <param name="prc"></param>
        /// <returns></returns>
        int IVsFindTarget.GetMatchRect(RECT[] prc)
        {
            return ((IVsFindTarget)_subCodeWindow).GetMatchRect(prc);
        }

        #endregion


        #region IVsCodeWindow Members

        int IVsCodeWindow.GetPrimaryView(out IVsTextView ppView)
        {
            return ((IVsCodeWindow)_subCodeWindow).GetPrimaryView(out ppView);
        }

        int IVsCodeWindow.GetSecondaryView(out IVsTextView ppView)
        {
            return ((IVsCodeWindow)_subCodeWindow).GetSecondaryView(out ppView);
        }

        int IVsCodeWindow.GetLastActiveView(out IVsTextView ppView)
        {
            return ((IVsCodeWindow)_subCodeWindow).GetLastActiveView(out ppView);
        }

        int IVsCodeWindow.Close()
        {
            return ((IVsCodeWindow)_subCodeWindow).Close();
        }

        int IVsCodeWindow.GetBuffer(out IVsTextLines ppBuffer)
        {
            return ((IVsCodeWindow)_subCodeWindow).GetBuffer(out ppBuffer);
        }

        int IVsCodeWindow.GetEditorCaption(READONLYSTATUS dwReadOnly, out string pbstrEditorCaption)
        {
            return ((IVsCodeWindow)_subCodeWindow).GetEditorCaption(dwReadOnly, out pbstrEditorCaption);
        }

        int IVsCodeWindow.GetViewClassID(out Guid pclsidView)
        {
            return ((IVsCodeWindow)_subCodeWindow).GetViewClassID(out pclsidView);
        }

        int IVsCodeWindow.SetBaseEditorCaption(string[] pszBaseEditorCaption)
        {
            return ((IVsCodeWindow)_subCodeWindow).SetBaseEditorCaption(pszBaseEditorCaption);
        }

        int IVsCodeWindow.SetBuffer(IVsTextLines pBuffer)
        {
            return ((IVsCodeWindow)_subCodeWindow).SetBuffer(pBuffer);
        }

        int IVsCodeWindow.SetViewClassID(ref Guid clsidView)
        {
            return ((IVsCodeWindow)_subCodeWindow).SetViewClassID(ref clsidView);
        }

        #endregion

        #region IVsTextEditorPropertyContainer Members

        public int GetProperty(VSEDITPROPID idProp, out object pvar)
        {
            return ((IVsTextEditorPropertyContainer)_subCodeWindow).GetProperty(idProp, out pvar);
        }

        public int RemoveProperty(VSEDITPROPID idProp)
        {
            return ((IVsTextEditorPropertyContainer)_subCodeWindow).RemoveProperty(idProp);
        }

        public int SetProperty(VSEDITPROPID idProp, object var)
        {
            return ((IVsTextEditorPropertyContainer)_subCodeWindow).SetProperty(idProp, var);
        }

        #endregion

        #region IVsCodeWindowEx Members

        public int Initialize(uint grfCodeWindowBehaviorFlags, VSUSERCONTEXTATTRIBUTEUSAGE usageAuxUserContext, string szNameAuxUserContext, string szValueAuxUserContext, uint InitViewFlags, INITVIEW[] pInitView)
        {
            return ((IVsCodeWindowEx)_subCodeWindow).Initialize(grfCodeWindowBehaviorFlags, usageAuxUserContext, szNameAuxUserContext, szValueAuxUserContext, InitViewFlags, pInitView);
        }

        public int IsReadOnly()
        {
            return ((IVsCodeWindowEx)_subCodeWindow).IsReadOnly();
        }

        #endregion

        #region IVsFindTarget2

        public int NavigateTo2(IVsTextSpanSet pSpans, TextSelMode iSelMode)
        {
            return ((IVsFindTarget2)_subCodeWindow).NavigateTo2(pSpans, iSelMode);
        }

        #endregion

        //#region IVsUIElementPane Members

        //public int CloseUIElementPane()
        //{
        //    return VSConstants.E_NOTIMPL;

        //    if (_subCodeWindow != null)
        //        return ((IVsUIElementPane)_subCodeWindow).CloseUIElementPane();
        //    else
        //        return VSConstants.S_OK;
        //}

        //public int CreateUIElementPane(out object punkUIElement)
        //{
        //    punkUIElement = null;
        //    return VSConstants.E_NOTIMPL;

        //    if (_subCodeWindow == null)
        //        LoadXamlEditor();

        //    punkUIElement = _subCodeWindow;

        //    return VSConstants.S_OK;
        //}

        //public int GetDefaultUIElementSize(SIZE[] psize)
        //{
        //    return VSConstants.E_NOTIMPL;

        //    if (_subCodeWindow != null)
        //        return ((IVsUIElementPane)_subCodeWindow).GetDefaultUIElementSize(psize);
        //    else
        //        return VSConstants.S_OK;
        //}

        //public int LoadUIElementState(IStream pstream)
        //{
        //    return VSConstants.E_NOTIMPL;

        //    if (_subCodeWindow != null)
        //        return ((IVsUIElementPane)_subCodeWindow).LoadUIElementState(pstream);
        //    else
        //        return VSConstants.S_OK;
        //}

        //public int SaveUIElementState(IStream pstream)
        //{
        //    return VSConstants.E_NOTIMPL;

        //    if (_subCodeWindow != null)
        //        return ((IVsUIElementPane)_subCodeWindow).SaveUIElementState(pstream);
        //    else
        //        return VSConstants.S_OK;
        //}

        //public int SetUIElementSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider pSP)
        //{
        //    return VSConstants.E_NOTIMPL;

        //    if (_subCodeWindow != null)
        //        return ((IVsUIElementPane)_subCodeWindow).SetUIElementSite(pSP);
        //    else
        //        return VSConstants.S_OK;
        //}

        //public int TranslateUIElementAccelerator(MSG[] lpmsg)
        //{
        //    return VSConstants.E_NOTIMPL;

        //    if (_subCodeWindow != null)
        //        return ((IVsUIElementPane)_subCodeWindow).TranslateUIElementAccelerator(lpmsg);
        //    else
        //        return VSConstants.S_OK;
        //}

        //#endregion

        /*

        #region IVsTextViewEvents Members

        public void OnChangeCaretLine(IVsTextView pView, int iNewLine, int iOldLine)
        {
        }

        public void OnChangeScrollInfo(IVsTextView pView, int iBar, int iMinUnit, int iMaxUnits, int iVisibleUnits, int iFirstVisibleUnit)
        {
        }

        public void OnKillFocus(IVsTextView pView)
        {
        }

        public void OnSetBuffer(IVsTextView pView, IVsTextLines pBuffer)
        {
        }

        public void OnSetFocus(IVsTextView pView)
        {
        }

        #endregion

        void EnableAutonomousFind(IVsWindowFrame frame, IVsTextView textView)
        {
            //object obj = null;
            //ErrorHandler.ThrowOnFailure(frame.GetProperty(-3001, out obj), null);
            //IVsCodeWindow vsCodeWindow = obj as IVsCodeWindow;
            //ErrorHandler.ThrowOnFailure(vsCodeWindow.GetPrimaryView(out textView), null);


            if (frame == null)
                throw new ArgumentNullException("frame");

            if (textView == null)
                throw new ArgumentNullException("textView");

            object obj = null;
            frame.GetProperty(-3002, out obj);
            Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider = obj as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
            IObjectWithSite objectWithSite = textView as IObjectWithSite;

            Debug.Assert(objectWithSite != null);

            objectWithSite.SetSite(serviceProvider);

            IComponentModel service = _vsServiceProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            IVsEditorAdaptersFactoryService vsEditorAdaptersFactoryService = service.GetService<IVsEditorAdaptersFactoryService>();

            ITextView wpfTextView = vsEditorAdaptersFactoryService.GetWpfTextView(textView);

            Debug.Assert(wpfTextView != null);

            wpfTextView.Options.SetOptionValue("Enable Autonomous Find", true);
        }











        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.AdviseTextLinesEvents(IVsTextLinesEvents pSink, out uint pdwCookie)
        {
            pdwCookie = 0;
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.CanReplaceLines(int iStartLine, int iStartIndex, int iEndLine, int iEndIndex, int iNewLen)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.CopyLineText(int iStartLine, int iStartIndex, int iEndLine, int iEndIndex, IntPtr pszBuf, ref int pcchBuf)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.CreateEditPoint(int iLine, int iIndex, out object ppEditPoint)
        {
            ppEditPoint = null;
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.CreateLineMarker(int iMarkerType, int iStartLine, int iStartIndex, int iEndLine, int iEndIndex, IVsTextMarkerClient pClient, IVsTextLineMarker[] ppMarker)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.CreateTextPoint(int iLine, int iIndex, out object ppTextPoint)
        {
            ppTextPoint = null;
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.EnumMarkers(int iStartLine, int iStartIndex, int iEndLine, int iEndIndex, int iMarkerType, uint dwFlags, out IVsEnumLineMarkers ppEnum)
        {
            ppEnum = null;
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.FindMarkerByLineIndex(int iMarkerType, int iStartingLine, int iStartingIndex, uint dwFlags, out IVsTextLineMarker ppMarker)
        {
            ppMarker = null;
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.GetLineData(int iLine, LINEDATA[] pLineData, MARKERDATA[] pMarkerData)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.GetLineDataEx(uint dwFlags, int iLine, int iStartIndex, int iEndIndex, LINEDATAEX[] pLineData, MARKERDATA[] pMarkerData)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.GetLineText(int iStartLine, int iStartIndex, int iEndLine, int iEndIndex, out string pbstrBuf)
        {
            pbstrBuf = null;
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.GetMarkerData(int iTopLine, int iBottomLine, MARKERDATA[] pMarkerData)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.GetPairExtents(TextSpan[] pSpanIn, TextSpan[] pSpanOut)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.IVsTextLinesReserved1(int iLine, LINEDATA[] pLineData, int fAttributes)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.ReleaseLineData(LINEDATA[] pLineData)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.ReleaseLineDataEx(LINEDATAEX[] pLineData)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.ReleaseMarkerData(MARKERDATA[] pMarkerData)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.ReloadLines(int iStartLine, int iStartIndex, int iEndLine, int iEndIndex, IntPtr pszText, int iNewLen, TextSpan[] pChangedSpan)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.ReplaceLines(int iStartLine, int iStartIndex, int iEndLine, int iEndIndex, IntPtr pszText, int iNewLen, TextSpan[] pChangedSpan)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.ReplaceLinesEx(uint dwFlags, int iStartLine, int iStartIndex, int iEndLine, int iEndIndex, IntPtr pszText, int iNewLen, TextSpan[] pChangedSpan)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextLines.UnadviseTextLinesEvents(uint dwCookie)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.AddCommandFilter(IOleCommandTarget pNewCmdTarg, out IOleCommandTarget ppNextCmdTarg)
        {
            ppNextCmdTarg = null;
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.CenterColumns(int iLine, int iLeftCol, int iColCount)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.CenterLines(int iTopLine, int iCount)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.ClearSelection(int fMoveToAnchor)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.CloseView()
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.EnsureSpanVisible(TextSpan span)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.GetBuffer(out IVsTextLines ppBuffer)
        {
            ppBuffer = null;
            IVsTextLines currentlyFocusedTextLines = _textLines;
            if (currentlyFocusedTextLines == null)
            {
                return -2147467263;
            }
            ppBuffer = currentlyFocusedTextLines;
            return 0;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.GetCaretPos(out int piLine, out int piColumn)
        {
            piLine = 0;
            piColumn = 0;
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.GetLineAndColumn(int iPos, out int piLine, out int piIndex)
        {
            piLine = 0;
            piIndex = 0;
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.GetLineHeight(out int piLineHeight)
        {
            piLineHeight = 0;
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.GetNearestPosition(int iLine, int iCol, out int piPos, out int piVirtualSpaces)
        {
            piPos = 0;
            piVirtualSpaces = 0;
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.GetPointOfLineColumn(int iLine, int iCol, POINT[] ppt)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.GetScrollInfo(int iBar, out int piMinUnit, out int piMaxUnit, out int piVisibleUnits, out int piFirstVisibleUnit)
        {
            piMinUnit = 0;
            piMaxUnit = 0;
            piVisibleUnits = 0;
            piFirstVisibleUnit = 0;
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.GetSelectedText(out string pbstrText)
        {
            pbstrText = null;
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.GetSelection(out int piAnchorLine, out int piAnchorCol, out int piEndLine, out int piEndCol)
        {
            piAnchorLine = 0;
            piAnchorCol = 0;
            piEndLine = 0;
            piEndCol = 0;
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.GetSelectionDataObject(out Microsoft.VisualStudio.OLE.Interop.IDataObject ppIDataObject)
        {
            ppIDataObject = null;
            return -2147467263;
        }

        TextSelMode Microsoft.VisualStudio.TextManager.Interop.IVsTextView.GetSelectionMode()
        {
            return TextSelMode.SM_STREAM;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.GetSelectionSpan(TextSpan[] pSpan)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.GetTextStream(int iTopLine, int iTopCol, int iBottomLine, int iBottomCol, out string pbstrText)
        {
            pbstrText = null;
            return -2147467263;
        }

        IntPtr Microsoft.VisualStudio.TextManager.Interop.IVsTextView.GetWindowHandle()
        {
            return IntPtr.Zero;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.GetWordExtent(int iLine, int iCol, uint dwFlags, TextSpan[] pSpan)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.HighlightMatchingBrace(uint dwFlags, uint cSpans, TextSpan[] rgBaseSpans)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.Initialize(IVsTextLines pBuffer, IntPtr hwndParent, uint InitFlags, INITVIEW[] pInitView)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.PositionCaretForEditing(int iLine, int cIndentLevels)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.RemoveCommandFilter(IOleCommandTarget pCmdTarg)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.ReplaceTextOnLine(int iLine, int iStartCol, int iCharsToReplace, string pszNewText, int iNewLen)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.RestrictViewRange(int iMinLine, int iMaxLine, IVsViewRangeClient pClient)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.SendExplicitFocus()
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.SetBuffer(IVsTextLines pBuffer)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.SetCaretPos(int iLine, int iColumn)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.SetScrollPosition(int iBar, int iFirstVisibleUnit)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.SetSelection(int iAnchorLine, int iAnchorCol, int iEndLine, int iEndCol)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.SetSelectionMode(TextSelMode iSelMode)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.SetTopLine(int iBaseLine)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.UpdateCompletionStatus(IVsCompletionSet pCompSet, uint dwFlags)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.UpdateTipWindow(IVsTipWindow pTipWindow, uint dwFlags)
        {
            return -2147467263;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsTextView.UpdateViewFrameCaption()
        {
            return -2147467263;
        }





        public int GetLanguageServiceID(out Guid pguidLangService)
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer != null)
            {
                return vsTextBuffer.GetLanguageServiceID(out pguidLangService);
            }
            pguidLangService = Guid.Empty;
            return -2147467263;
        }

        public int GetLastLineIndex(out int piLine, out int piIndex)
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer != null)
            {
                return vsTextBuffer.GetLastLineIndex(out piLine, out piIndex);
            }
            piLine = 0;
            piIndex = 0;
            return -2147467263;
        }

        public int GetLengthOfLine(int iLine, out int piLength)
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer == null)
            {
                piLength = 0;
                return -2147467263;
            }
            return vsTextBuffer.GetLengthOfLine(iLine, out piLength);
        }

        public int GetLineCount(out int piLineCount)
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer != null)
            {
                return vsTextBuffer.GetLineCount(out piLineCount);
            }
            piLineCount = 0;
            return -2147467263;
        }

        public int GetLineIndexOfPosition(int iPosition, out int piLine, out int piColumn)
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer != null)
            {
                return vsTextBuffer.GetLineIndexOfPosition(iPosition, out piLine, out piColumn);
            }
            piLine = 0;
            piColumn = 0;
            return -2147467263;
        }

        public int GetPositionOfLine(int iLine, out int piPosition)
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer == null)
            {
                piPosition = 0;
                return -2147467263;
            }
            return vsTextBuffer.GetPositionOfLine(iLine, out piPosition);
        }

        public int GetPositionOfLineIndex(int iLine, int iIndex, out int piPosition)
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer == null)
            {
                piPosition = 0;
                return -2147467263;
            }
            return vsTextBuffer.GetPositionOfLineIndex(iLine, iIndex, out piPosition);
        }

        //public Microsoft.VisualStudio.OLE.Interop.IServiceProvider GetServiceProvider()
        //{
        //    return this.GetService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider)) as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
        //}

        public int GetSize(out int piLength)
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer != null)
            {
                return vsTextBuffer.GetSize(out piLength);
            }
            piLength = 0;
            return -2147467263;
        }

        public int GetStateFlags(out uint pdwReadOnlyFlags)
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer != null)
            {
                return vsTextBuffer.GetStateFlags(out pdwReadOnlyFlags);
            }
            pdwReadOnlyFlags = 0;
            return -2147467263;
        }

        //public IVsTrackSelectionEx GetTrackSelectionService()
        //{
        //    return this.GetService(typeof(SVsTrackSelectionEx)) as IVsTrackSelectionEx;
        //}

        public int GetUndoManager(out IOleUndoManager ppUndoManager)
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer != null)
            {
                return vsTextBuffer.GetUndoManager(out ppUndoManager);
            }
            ppUndoManager = null;
            return -2147467263;
        }

        private IVsTextBuffer GetVsTextBuffer()
        {
            return _textLines;
        }

        //public void InitFrame(IVsWindowFrame frame_, WindowPosition p, string originalCodeFile_)
        //{
        //    this.frame = frame_;
        //    this.originalCodeFile = originalCodeFile_;
        //    this.persistFileFormatCurFile = originalCodeFile_;
        //    this.frame.SetProperty(-5023, "Task Canvas");
        //    this.frame.SetProperty(-4010, null);
        //    this.frame.SetProperty(-5022, true);
        //    if (TaskCanvasPackage.GetGlobalSettings().tabPosition == GlobalSettings.TabPosition.Floating)
        //    {
        //        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => this.InitFloatingWindow(p)));
        //    }
        //}

        public int InitializeContent(string pszText, int iLength)
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer == null)
            {
                return -2147467263;
            }
            return vsTextBuffer.InitializeContent(pszText, iLength);
        }

        public int LockBuffer()
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer == null)
            {
                return -2147467263;
            }
            return vsTextBuffer.LockBuffer();
        }

        public int LockBufferEx(uint dwFlags)
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer == null)
            {
                return -2147467263;
            }
            return vsTextBuffer.LockBufferEx(dwFlags);
        }

        public int Reload(int fUndoable)
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer == null)
            {
                return -2147467263;
            }
            return vsTextBuffer.Reload(fUndoable);
        }

        public int Reserved1()
        {
            return -2147467263;
        }

        public int Reserved10()
        {
            return -2147467263;
        }

        public int Reserved2()
        {
            return -2147467263;
        }

        public int Reserved3()
        {
            return -2147467263;
        }

        public int Reserved4()
        {
            return -2147467263;
        }

        public int Reserved5()
        {
            return -2147467263;
        }

        public int Reserved6()
        {
            return -2147467263;
        }

        public int Reserved7()
        {
            return -2147467263;
        }

        public int Reserved8()
        {
            return -2147467263;
        }

        public int Reserved9()
        {
            return -2147467263;
        }

        public int SetLanguageServiceID(ref Guid guidLangService)
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer == null)
            {
                return -2147467263;
            }
            return vsTextBuffer.SetLanguageServiceID(ref guidLangService);
        }

        public int SetStateFlags(uint dwReadOnlyFlags)
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer == null)
            {
                return -2147467263;
            }
            return vsTextBuffer.SetStateFlags(dwReadOnlyFlags);
        }


        public int UnlockBuffer()
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer == null)
            {
                return -2147467263;
            }
            return vsTextBuffer.UnlockBuffer();
        }

        public int UnlockBufferEx(uint dwFlags)
        {
            IVsTextBuffer vsTextBuffer = this.GetVsTextBuffer();
            if (vsTextBuffer == null)
            {
                return -2147467263;
            }
            return vsTextBuffer.UnlockBufferEx(dwFlags);
        }
        
        */
    }
}
