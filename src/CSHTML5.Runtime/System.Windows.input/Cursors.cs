
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

namespace System.Windows.Input
{
    /// <summary>
    /// Defines a set of default cursors.
    /// </summary>
    public static class Cursors
    {
        internal static Dictionary<INTERNAL_CursorsEnum, string> INTERNAL_cursorEnumToCursorString;

        internal static void FillCursorTypeToStringDictionary()
        {
            if (INTERNAL_cursorEnumToCursorString == null)
            {
                INTERNAL_cursorEnumToCursorString = new Dictionary<INTERNAL_CursorsEnum, string>();
            }
            INTERNAL_cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.AppStarting, "progress");
            INTERNAL_cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.Arrow, "default");
            //cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.ArrowCD, "default");             //<-- no equivalent?
            INTERNAL_cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.Cross, "crosshair");
            INTERNAL_cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.Hand, "pointer");
            INTERNAL_cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.Help, "help");
            INTERNAL_cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.IBeam, "text");
            INTERNAL_cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.No, "not-allowed");
            INTERNAL_cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.None, "none");
            //cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.Pen, "default");                 //<-- no equivalent?
            //cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.ScrollAll, "all-scroll");       //not exactly it
            //cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.ScrollE, "default");         //
            //cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.ScrollN, "default");         //
            //cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.ScrollNE, "default");        //
            //cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.ScrollNS, "default");        //
            //cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.ScrollNW, "default");        //
            //cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.ScrollS, "default");        //
            //cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.ScrollSE, "default");        //
            //cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.ScrollSW, "default");        //
            //cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.ScrollW, "default");        //
            //cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.ScrollWE, "default");        //
            INTERNAL_cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.SizeAll, "move");
            INTERNAL_cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.SizeNESW, "nesw-resize");
            INTERNAL_cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.SizeNS, "ns-resize");
            INTERNAL_cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.SizeNWSE, "nwse-resize");
            INTERNAL_cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.SizeWE, "ew-resize");
            //cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.UpArrow, "default");         //
            INTERNAL_cursorEnumToCursorString.Add(INTERNAL_CursorsEnum.Wait, "wait");
        }

        internal enum INTERNAL_CursorsEnum
        {
            AppStarting,
            Arrow,
            //ArrowCD,
            Cross,
            Hand,
            Help,
            IBeam,
            No,
            None,
            //Pen,
            //ScrollAll,
            //ScrollE,
            //ScrollN,
            //ScrollNE,
            //ScrollNS,
            //ScrollNW,
            //ScrollS,
            //ScrollSE,
            //ScrollSW,
            //ScrollW,
            //ScrollWE,
            SizeAll,
            SizeNESW,
            SizeNS,
            SizeNWSE,
            SizeWE,
            //UpArrow,
            Wait
        }

        /// <summary>
        /// Gets the System.Windows.Input.Cursor that appears when an application is
        /// starting.
        /// </summary>
        public static Cursor AppStarting
        {
            get
            {
                Cursor cursor = new Cursor();
                cursor._cursorHtmlString = INTERNAL_cursorEnumToCursorString[INTERNAL_CursorsEnum.AppStarting];
                return cursor;
            }
        }

        /// <summary>
        /// Gets the Arrow System.Windows.Input.Cursor.
        /// </summary>
        public static Cursor Arrow
        {
            get
            {
                Cursor cursor = new Cursor();
                cursor._cursorHtmlString = INTERNAL_cursorEnumToCursorString[INTERNAL_CursorsEnum.Arrow];
                return cursor;
            }
        }

        ///// <summary>
        ///// Gets the arrow with a compact disk System.Windows.Input.Cursor.
        ///// </summary>
        //public static Cursor ArrowCD { get; }

        /// <summary>
        /// Gets the crosshair System.Windows.Input.Cursor.
        /// </summary>
        public static Cursor Cross
        {
            get
            {
                Cursor cursor = new Cursor();
                cursor._cursorHtmlString = INTERNAL_cursorEnumToCursorString[INTERNAL_CursorsEnum.Cross];
                return cursor;
            }
        }

        /// <summary>
        /// Gets a hand System.Windows.Input.Cursor.
        /// </summary>
        public static Cursor Hand
        {
            get
            {
                Cursor cursor = new Cursor();
                cursor._cursorHtmlString = INTERNAL_cursorEnumToCursorString[INTERNAL_CursorsEnum.Hand];
                return cursor;
            }
        }

        /// <summary>
        ///  Gets a help System.Windows.Input.Cursor which is a combination of an arrow
        ///  and a question mark.
        ///  </summary>
        public static Cursor Help
        {
            get
            {
                Cursor cursor = new Cursor();
                cursor._cursorHtmlString = INTERNAL_cursorEnumToCursorString[INTERNAL_CursorsEnum.Help];
                return cursor;
            }
        }

        /// <summary>
        /// Gets an I-beam System.Windows.Input.Cursor, which is used to show where the
        /// text cursor appears when the mouse is clicked.
        /// </summary>
        public static Cursor IBeam
        {
            get
            {
                Cursor cursor = new Cursor();
                cursor._cursorHtmlString = INTERNAL_cursorEnumToCursorString[INTERNAL_CursorsEnum.IBeam];
                return cursor;
            }
        }
        
        /// <summary>
        /// Gets a System.Windows.Input.Cursor with which indicates that a particular
        /// region is invalid for a given operation.
        /// </summary>
        public static Cursor No
        {
            get
            {
                Cursor cursor = new Cursor();
                cursor._cursorHtmlString = INTERNAL_cursorEnumToCursorString[INTERNAL_CursorsEnum.No];
                return cursor;
            }
        }
        
        /// <summary>
        /// Gets a special cursor that is invisible.
        /// </summary>
        public static Cursor None
        {
            get
            {
                Cursor cursor = new Cursor();
                cursor._cursorHtmlString = INTERNAL_cursorEnumToCursorString[INTERNAL_CursorsEnum.None];
                return cursor;
            }
        }
        
        ///// <summary>
        ///// Gets a pen System.Windows.Input.Cursor.
        ///// </summary>
        //public static Cursor Pen { get; }
        
        ///// <summary>
        ///// Gets the scroll all System.Windows.Input.Cursor.
        ///// </summary>
        //public static Cursor ScrollAll { get; }
        
        ///// <summary>
        ///// Gets the scroll east System.Windows.Input.Cursor.
        ///// </summary>
        //public static Cursor ScrollE { get; }
        
        ///// <summary>
        ///// Gets the scroll north System.Windows.Input.Cursor.
        ///// </summary>
        //public static Cursor ScrollN { get; }
        
        ///// <summary>
        ///// Gets the scroll northeast cursor.
        ///// </summary>
        //public static Cursor ScrollNE { get; }
        
        ///// <summary>
        ///// Gets the scroll north/south cursor.
        ///// </summary>
        //public static Cursor ScrollNS { get; }
        
        ///// <summary>
        ///// Gets a scroll northwest cursor.
        ///// </summary>
        //public static Cursor ScrollNW { get; }
        
        ///// <summary>
        ///// Gets the scroll south System.Windows.Input.Cursor.
        ///// </summary>
        //public static Cursor ScrollS { get; }
        
        ///// <summary>
        ///// Gets a south/east scrolling System.Windows.Input.Cursor.
        ///// </summary>
        //public static Cursor ScrollSE { get; }
        
        ///// <summary>
        ///// Gets the scroll southwest System.Windows.Input.Cursor.
        ///// </summary>
        //public static Cursor ScrollSW { get; }
        
        ///// <summary>
        ///// Gets the scroll west System.Windows.Input.Cursor.
        ///// </summary>
        //public static Cursor ScrollW { get; }
        
        ///// <summary>
        ///// Gets a west/east scrolling System.Windows.Input.Cursor.
        ///// </summary>
        //public static Cursor ScrollWE { get; }
        
        /// <summary>
        /// Gets a four-headed sizing System.Windows.Input.Cursor, which consists of
        /// four joined arrows that point north, south, east, and west.
        /// </summary>
        public static Cursor SizeAll
        {
            get
            {
                Cursor cursor = new Cursor();
                cursor._cursorHtmlString = INTERNAL_cursorEnumToCursorString[INTERNAL_CursorsEnum.SizeAll];
                return cursor;
            }
        }
        
        /// <summary>
        /// Gets a two-headed northeast/southwest sizing System.Windows.Input.Cursor.
        /// </summary>
        public static Cursor SizeNESW
        {
            get
            {
                Cursor cursor = new Cursor();
                cursor._cursorHtmlString = INTERNAL_cursorEnumToCursorString[INTERNAL_CursorsEnum.SizeNESW];
                return cursor;
            }
        }
        
        /// <summary>
        /// Gets a two-headed north/south sizing System.Windows.Input.Cursor.
        /// </summary>
        public static Cursor SizeNS
        {
            get
            {
                Cursor cursor = new Cursor();
                cursor._cursorHtmlString = INTERNAL_cursorEnumToCursorString[INTERNAL_CursorsEnum.SizeNS];
                return cursor;
            }
        }
        
        /// <summary>
        /// Gets a two-headed northwest/southeast sizing System.Windows.Input.Cursor.
        /// </summary>
        public static Cursor SizeNWSE
        {
            get
            {
                Cursor cursor = new Cursor();
                cursor._cursorHtmlString = INTERNAL_cursorEnumToCursorString[INTERNAL_CursorsEnum.SizeNWSE];
                return cursor;
            }
        }
        
        /// <summary>
        /// Gets a two-headed west/east sizing System.Windows.Input.Cursor.
        /// </summary>
        public static Cursor SizeWE
        {
            get
            {
                Cursor cursor = new Cursor();
                cursor._cursorHtmlString = INTERNAL_cursorEnumToCursorString[INTERNAL_CursorsEnum.SizeWE];
                return cursor;
            }
        }
        
        ///// <summary>
        ///// Gets an up arrow System.Windows.Input.Cursor, which is typically used to
        ///// identify an insertion point.
        ///// </summary>
        //public static Cursor UpArrow { get; }
        
        /// <summary>
        /// Specifies a wait (or hourglass) System.Windows.Input.Cursor.
        /// </summary>
        public static Cursor Wait
        {
            get
            {
                Cursor cursor = new Cursor();
                cursor._cursorHtmlString = INTERNAL_cursorEnumToCursorString[INTERNAL_CursorsEnum.Wait];
                return cursor;
            }
        }
    }
}