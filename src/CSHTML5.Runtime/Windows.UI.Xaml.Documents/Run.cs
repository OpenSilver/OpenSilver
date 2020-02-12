
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



using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    /// <summary>
    /// Represents a discrete section of formatted or unformatted text.
    /// </summary>
    [ContentProperty("Text")]
    public sealed partial class Run : Inline
    {
        private const string defaultText = "\u00A0"; // We add a space at the end of the text so that two <Run> tags do not appear to touch each other (eg. <Run>This is a</Run><Run>test.</Run>).

        /// <summary>
        /// Get or Set the Text property
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Identifies the Text dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Run), new PropertyMetadata(string.Empty, OnTextPropertyChanged, CoerceTextProperty) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Run run = (Run)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(run))
            {
                INTERNAL_HtmlDomManager.SetContentString(run, (string)e.NewValue);
            }
        }

        private static object CoerceTextProperty(DependencyObject d, object baseValue)
        {
            if (string.IsNullOrEmpty((string)baseValue))
            {
                return defaultText;
            }
            return baseValue;
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            dynamic span = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("span", parentRef, this);
            domElementWhereToPlaceChildren = span;
            return span;
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();
            INTERNAL_HtmlDomManager.SetContentString(this, (string)CoerceTextProperty(this, this.Text));
        }
    }
}
