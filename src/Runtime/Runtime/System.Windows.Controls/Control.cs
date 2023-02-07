
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using CSHTML5.Internal;
using OpenSilver.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using System.ComponentModel;

#if MIGRATION
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents the base class for UI elements that use a ControlTemplate to define
    /// their appearance.
    /// </summary>
    public partial class Control : FrameworkElement
    {
        //COMMENT 26.03.2020:
        // ERROR DESCRIPTION:
        //  see Ticket #1711, problem about icons not appearing:
        //    In the project that was sent to us on the 26th of March 2020:
        //    BasePage.xaml -> ItemsControl named "icSubNavigation" -> ItemTemplate Property -> DataTemplate with the key: dtSubNavigationIcon:
        //      Properly generated in C# but when translated into js, the first letter of the methods that are called is lowercased.
        //  I could not find the conditions nor the reasons for such a case to appear so I settled with the following workaround:
        // WORKAROUND:
        //  Create an additional private method with the same signature except with the first character of its name lowercased, that calls the original one.
        // NOTE:
        //  This workaround was used only for the methods that were in that specific case, so we might need to use it for other methods if other cases appear.
        //  Workaround currently used on:
        //      - INTERNAL_GetVisualStateGroups
        //      - RegisterName
        //END OF COMMENT

        // Note: this should be protected and the Control class should be abstract.
        /// <summary>
        /// Represents the base class for UI elements that use a <see cref="ControlTemplate"/>
        /// to define their appearance.
        /// </summary>
        public Control()
        {
            // Initialize the _templateCache to the default value for TemplateProperty.
            // If the default value is non-null then wire it to the current instance.
            PropertyMetadata metadata = TemplateProperty.GetMetadata(this.GetType());
            ControlTemplate defaultValue = (ControlTemplate)metadata.DefaultValue;
            if (defaultValue != null)
            {
                OnTemplateChanged(this, new DependencyPropertyChangedEventArgs(null, defaultValue, TemplateProperty));
            }
        }

        private bool _isDisabled = false;

        /// <summary>
        /// Derived classes can set this flag in their constructor to prevent the "Template" property from being applied.
        /// </summary>
        [Obsolete(Helper.ObsoleteMemberMessage)]
        protected bool INTERNAL_DoNotApplyControlTemplate = false;

        /// <summary>
        /// Derived classes can set this flag to True in their constructor in order to disable the "GoToState" calls of this class related to PointerOver/Pressed/Disabled, and handle them by themselves. An example is the ToggleButton control, which contains states such as "CheckedPressed", "CheckedPointerOver", etc.
        /// </summary>
        protected bool DisableBaseControlHandlingOfVisualStates = false;

        //-----------------------
        // ISENABLED (OVERRIDE)
        //-----------------------

        protected internal override void ManageIsEnabled(bool isEnabled)
        {
            base.ManageIsEnabled(isEnabled); // Useful for setting the "disabled" attribute on the DOM element.

            //OnTabIndexPropertyChanged(TabIndex);
            UpdateTabIndex(IsTabStop, TabIndex);

            _isDisabled = !isEnabled; // We remember the value so that when we update the visual states, we know whether we should go to the "Disabled" state or not.
            UpdateVisualStates();
        }

        internal override void AddEventListeners()
        {
            NativeEventsHelper.AddEventListeners(this, true);
        }

        //-----------------------
        // BACKGROUND
        //-----------------------

        /// <summary>
        /// Gets or sets a brush that provides the background of the control.
        /// </summary>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Control.Background"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(
                nameof(Background), typeof(Brush), 
                typeof(Control), 
                new PropertyMetadata((object)null)
                {
                    MethodToUpdateDom2 = UpdateDomOnBackgroundChanged,
                });

        private static void UpdateDomOnBackgroundChanged(DependencyObject d, object oldValue, object newValue)
        {
            var control = (Control)d;
            if (!control.HasTemplate)
            {
                _ = Panel.RenderBackgroundAsync(control, (Brush)newValue);
            }
        }

        internal bool INTERNAL_IsLegacyVisualStates
        {
            get
            {
                if (StateGroupsRoot == null)
                {
                    return false;
                }

                IList<VisualStateGroup> groups = (Collection<VisualStateGroup>)this.GetValue(VisualStateManager.VisualStateGroupsProperty);
                if (groups == null)
                {
                    return false;
                }

                return groups.Any(gr => ((IList<VisualState>)gr.States).Any(state => state.Name == VisualStates.StateMouseOver));
            }
        }

        //-----------------------
        // BORDERBRUSH
        //-----------------------

        /// <summary>
        /// Gets or sets a brush that describes the border background of a control.
        /// </summary>
        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Control.BorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(
                nameof(BorderBrush), 
                typeof(Brush), 
                typeof(Control), 
                new PropertyMetadata((object)null));

        //-----------------------
        // BORDERTHICKNESS
        //-----------------------
        /// <summary>
        /// Gets or sets the thickness of the border.
        /// </summary>
        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Control.BorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(
                nameof(BorderThickness), 
                typeof(Thickness), 
                typeof(Control), 
                new PropertyMetadata(new Thickness()));

        //-----------------------
        // FONTWEIGHT
        //-----------------------

        /// <summary>
        /// Gets or sets the thickness of the specified font.
        /// </summary>
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Control.FontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register(
                nameof(FontWeight), 
                typeof(FontWeight), 
                typeof(Control),
                new FrameworkPropertyMetadata(FontWeights.Normal, FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    Inherits = true,
                    GetCSSEquivalent = (instance) => new CSSEquivalent
                    {
                        Value = (inst, value) => ((FontWeight)value).ToOpenTypeWeight().ToInvariantString(),
                        Name = new List<string> { "fontWeight" },
                        ApplyAlsoWhenThereIsAControlTemplate = true // (See comment where this property is defined)
                    }
                });

        /// <summary>
        /// Gets or sets the style in which the text is rendered.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Control.FontStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty =
            DependencyProperty.Register(
                nameof(FontStyle),
                typeof(FontStyle),
                typeof(Control),
                new FrameworkPropertyMetadata(
#if MIGRATION
                    FontStyles.Normal
#else
                    FontStyle.Normal
#endif
                    , FrameworkPropertyMetadataOptions.AffectsMeasure
                    )
                {
                    GetCSSEquivalent = (instance) => new CSSEquivalent
                    {
                        Value = (inst, value) => ((FontStyle)value).ToString().ToLower(),
                        Name = new List<string> { "fontStyle" },
                        ApplyAlsoWhenThereIsAControlTemplate = true // (See comment where this property is defined)
                    }
                });

        //-----------------------
        // FOREGROUND
        //-----------------------

        /// <summary>
        /// Gets or sets a brush that describes the foreground color.
        /// </summary>
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Control.Foreground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(
                nameof(Foreground), 
                typeof(Brush), 
                typeof(Control), 
                new PropertyMetadata(new SolidColorBrush(Colors.Black))
                {
                    MethodToUpdateDom2 = UpdateDomOnForegroundChanged,
                });

        private static void UpdateDomOnForegroundChanged(DependencyObject d, object oldValue, object newValue)
        {
            var control = (Control)d;
            var cssStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(control);
            switch (newValue)
            {
                case SolidColorBrush solid:
                    cssStyle.color = solid.INTERNAL_ToHtmlString();
                    break;

                case null:
                    cssStyle.color = string.Empty;
                    break;

                default:
                    // GradientBrush, ImageBrush and custom brushes are not supported.
                    // Keep using old brush.
                    break;
            }
        }

        //-----------------------
        // FONTFAMILY
        //----------------------

        /// <summary>
        /// Gets or sets the font used to display text in the control.
        /// </summary>
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Control.FontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register(
                nameof(FontFamily), 
                typeof(FontFamily), 
                typeof(Control),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    GetCSSEquivalent = (instance) => new CSSEquivalent
                    {
                        Value = (inst, value) => (value is FontFamily) ?
                            INTERNAL_FontsHelper.LoadFont(((FontFamily)value).Source, (UIElement)instance) :
                            string.Empty,
                        Name = new List<string> { "fontFamily" },
                        ApplyAlsoWhenThereIsAControlTemplate = true // (See comment where this property is defined)
                    }
                });

        //-----------------------
        // FONTSIZE
        //-----------------------

        /// <summary>
        /// Gets or sets the size of the text in this control.
        /// </summary>
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Control.FontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(
                nameof(FontSize), 
                typeof(double), 
                typeof(Control),
                new FrameworkPropertyMetadata(11d, FrameworkPropertyMetadataOptions.AffectsMeasure)
                { 
                    Inherits = true,
                    GetCSSEquivalent = (instance) => new CSSEquivalent
                    {
                        Value = (inst, value) =>
                        {
                            // Note: We multiply by 1000 and then divide by 1000 so as to only keep 3 
                            // decimals at the most.
                            return (Math.Floor(Convert.ToDouble(value) * 1000) / 1000).ToInvariantString() + "px"; 
                        },
                        Name = new List<string> { "fontSize" },
                        ApplyAlsoWhenThereIsAControlTemplate = true // (See comment where this property is defined)
                    },
                });

        //-----------------------
        // TEXTDECORATION
        //-----------------------
        // Note: this was moved from TextBlock because it is more practical for styling.
#if MIGRATION
        /// <summary>
        /// Gets or sets the text decorations (underline, strikethrough...).
        /// </summary>
        public TextDecorationCollection TextDecorations
        {
            get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
            set { SetValue(TextDecorationsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Control.TextDecorations"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextDecorationsProperty = 
            DependencyProperty.Register(
                nameof(TextDecorations),
                typeof(TextDecorationCollection),
                typeof(Control),
                new FrameworkPropertyMetadata(null)
                {
                    MethodToUpdateDom = static (d, newValue) =>
                    {
                        var domStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(((Control)d).INTERNAL_OuterDomElement);
                        domStyle.textDecoration = ((TextDecorationCollection)newValue)?.ToHtmlString() ?? string.Empty;
                    },
                });
#else
        /// <summary>
        /// Gets or sets the text decorations (underline, strikethrough...).
        /// </summary>
        public TextDecorations? TextDecorations
        {
            get { return (TextDecorations?)GetValue(TextDecorationsProperty); }
            set { SetValue(TextDecorationsProperty, value); }
        }

        /// <summary>
        /// Identifies the TextDecorations dependency property.
        /// </summary>
        public static readonly DependencyProperty TextDecorationsProperty =
            DependencyProperty.Register(
                nameof(TextDecorations), 
                typeof(TextDecorations?), 
                typeof(Control), 
                new PropertyMetadata((object)null)
                {
                    MethodToUpdateDom = static (d, newValue) =>
                    {
                        var domStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(((Control)d).INTERNAL_OuterDomElement);
                        var newTextDecoration = (TextDecorations?)newValue;
                        if (newTextDecoration.HasValue)
                        {
                            domStyle.textDecoration = newTextDecoration switch
                            {
                                Text.TextDecorations.OverLine => "overline",
                                Text.TextDecorations.Strikethrough => "line-through",
                                Text.TextDecorations.Underline => "underline",
                                _ => "",
                            };
                        }
                        else
                        {
                            domStyle.textDecoration = "";
                        }
                    },
                });
#endif

        //-----------------------
        // PADDING
        //-----------------------

        /// <summary>
        /// Gets or sets the distance between the border and its child object.
        /// </summary>
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Padding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(
                nameof(Padding),
                typeof(Thickness),
                typeof(Control),
                new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom = static (d, newValue) =>
                    {
                        var control = (Control)d;
                        // if the parent is a canvas, we ignore this property and we want to ignore this
                        // property if there is a ControlTemplate on this control.
                        if (control.INTERNAL_InnerDomElement != null && 
                            !control.HasTemplate && 
                            control.INTERNAL_VisualParent is not Canvas && 
                            !control.IsUnderCustomLayout)
                        {
                            var domStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(control.INTERNAL_InnerDomElement);
                            Thickness padding = (Thickness)newValue;

                            // todo: if the container has a padding, add it to the margin
                            domStyle.boxSizing = "border-box";
                            domStyle.padding = $"{padding.Top.ToInvariantString()}px {padding.Right.ToInvariantString()}px {padding.Bottom.ToInvariantString()}px {padding.Left.ToInvariantString()}px";
                        }
                    },
                });

        //-----------------------
        // HORIZONTALCONTENTALIGNMENT
        //-----------------------

        /// <summary>
        /// Gets or sets the horizontal alignment of the control's content.
        /// </summary>
        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="HorizontalContentAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.Register(
                nameof(HorizontalContentAlignment), 
                typeof(HorizontalAlignment), 
                typeof(Control),
                new FrameworkPropertyMetadata(HorizontalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsArrange));

        //-----------------------
        // VERTICALCONTENTALIGNMENT
        //-----------------------

        /// <summary>
        /// Gets or sets the vertical alignment of the control's content.
        /// </summary>
        public VerticalAlignment VerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="VerticalContentAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalContentAlignmentProperty =
            DependencyProperty.Register(
                nameof(VerticalContentAlignment), 
                typeof(VerticalAlignment), 
                typeof(Control),
                new FrameworkPropertyMetadata(VerticalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsArrange));

        //-----------------------
        // TABINDEX
        //-----------------------

        /// <summary>
        /// Gets or sets a value that determines the order in which elements receive
        /// focus when the user navigates through controls by pressing the Tab key.
        /// The default value is MaxValue
        /// </summary>
        public int TabIndex
        {
            get { return (int)GetValue(TabIndexProperty); }
            set { SetValue(TabIndexProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="TabIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabIndexProperty =
            DependencyProperty.Register(
                nameof(TabIndex), 
                typeof(int), 
                typeof(Control), 
                new PropertyMetadata(int.MaxValue)
                {
                    MethodToUpdateDom = static (d, newValue) =>
                    {
                        var control = (Control)d;
                        control.UpdateTabIndex(control.IsTabStop, (int)newValue);
                    },
                });

        private const int TABINDEX_BROWSER_MAX_VALUE = 32767;

        internal virtual void UpdateTabIndex(bool isTabStop, int tabIndex)
        {
            var domElementConcernedByFocus = GetFocusTarget();
            if (domElementConcernedByFocus == null)
                return;
            if (!isTabStop || !this.IsEnabled)
            {
                INTERNAL_HtmlDomManager.SetDomElementAttribute(domElementConcernedByFocus, "tabIndex", "-1");
            }
            else
            {
                //Note: according to W3C, tabIndex needs to be between 0 and 32767 on browsers: https://www.w3.org/TR/html401/interact/forms.html#adef-tabindex
                //      also, the behaviour of the different browsers outside of these values can be different and therefore, we have to restrict the values.
                //We translate the TabIndexes to have a little margin with negative TabIndexes:
                //this is because a negative tabIndex in html is equivalent to IsTabStop = false in CS.
                //this way, we make sure to keep the order of elements with TabIndexes between -100 and TABINDEX_BROWSER_MAX_VALUE - 100
                //100 is empirically chosen because the only reason I would see for a negative TabIndex would be if the person has already set some TabIndexes and forgot one that needs to come before that so he is likely to simply use small numbers.
                int index;
                if (tabIndex < (TABINDEX_BROWSER_MAX_VALUE - 100))
                {
                    index = Math.Max(tabIndex + 100, 0); //this is not ideal but it'll have do for now.
                }
                else
                {
                    index = TABINDEX_BROWSER_MAX_VALUE;
                }

                INTERNAL_HtmlDomManager.SetDomElementAttribute(domElementConcernedByFocus, "tabIndex", index.ToString()); //note: not replaced with GetCSSEquivalent because it uses SetDomeElementAttribute (so it's not the style)

                //in the case where the control should not have an outline even when focused or when the control has a template that defines the VisualState "Focused", we remove the default outline that browsers put:
                IList<VisualStateGroup> groups = this.StateGroupsRoot?.GetValue(VisualStateManager.VisualStateGroupsProperty) as Collection<VisualStateGroup>;
#pragma warning disable CS0618 // Type or member is obsolete
                if (!this.UseSystemFocusVisuals ||
#pragma warning restore CS0618 // Type or member is obsolete
                    (groups != null && groups.Any(gr => ((IList<VisualState>)gr.States).Any(state => state.Name == "Focused"))))
                {

                    // this.INTERNAL_GetVisualStateGroups().ContainsVisualState("Focused")
                    INTERNAL_HtmlDomManager.SetDomElementStyleProperty(domElementConcernedByFocus, new List<string>() { "outline" }, "none");
                }
            }
        }

        //-----------------------
        // ISTABSTOP
        //-----------------------

        /// <summary>
        /// Gets or sets a value that indicates whether a control is included in tab
        /// navigation.
        /// </summary>
        public bool IsTabStop
        {
            get { return (bool)GetValue(IsTabStopProperty); }
            set { SetValue(IsTabStopProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Control.IsTabStop"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTabStopProperty =
            DependencyProperty.Register(
                nameof(IsTabStop),    
                typeof(bool), 
                typeof(Control), 
                new PropertyMetadata(true)
                {
                    MethodToUpdateDom = static (d, newValue) =>
                    {
                        var control = (Control)d;
                        control.UpdateTabIndex((bool)newValue, control.TabIndex);
                    },
                });

        //-----------------------
        // TEMPLATE
        //-----------------------

        private ControlTemplate _templateCache;

        /// <summary>
        /// Gets or sets a control template.
        /// </summary>
        public ControlTemplate Template
        {
            get { return this._templateCache; }
            set { SetValue(TemplateProperty, value); }
        }

        // Internal Helper so the FrameworkElement could see this property
        internal override FrameworkTemplate TemplateInternal
        {
            get { return Template; }
        }

        // Internal Helper so the FrameworkElement could see the template cache
        internal override FrameworkTemplate TemplateCache
        {
            get { return _templateCache; }
            set { _templateCache = (ControlTemplate)value; }
        }

        /// <summary>
        /// Identifies the <see cref="Control.Template"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register(
                nameof(Template), 
                typeof(ControlTemplate), 
                typeof(Control), 
                new PropertyMetadata(null, OnTemplateChanged));

        private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control control = (Control)d;
            FrameworkElement.UpdateTemplateCache(control, (FrameworkTemplate)e.OldValue, (FrameworkTemplate)e.NewValue, TemplateProperty);

            if (control.IsConnectedToLiveTree)
            {
                control.InvalidateMeasureInternal();
            }
        }

        /// <summary>
        /// Loads the relevant control template so that its parts can be referenced.
        /// </summary>
        /// <returns>
        /// Returns whether the visual tree was rebuilt by this call. true indicates the
        /// tree was rebuilt; false indicates that the previous visual tree was retained.
        /// </returns>
        public new bool ApplyTemplate()
        {
            return base.ApplyTemplate();
        }

        /// <summary>
        /// Retrieves the named element in the instantiated ControlTemplate visual tree.
        /// </summary>
        /// <param name="childName">The name of the element to find.</param>
        /// <returns>
        /// The named element from the template, if the element is found. Can return
        /// null if no element with name childName was found in the template.
        /// </returns>
        protected internal new DependencyObject GetTemplateChild(string childName)
        {
            return base.GetTemplateChild(childName);
        }

        //-----------------------
        // OTHER
        //-----------------------

        /// <summary>
        /// Attempts to set the focus on the control.
        /// </summary>
        /// <returns>
        /// true if focus was set to the control, or focus was already on the control.
        /// false if the control is not focusable.
        /// </returns>
        public bool Focus()
        {
            if (!Keyboard.IsSubTreeFocusable(this))
            {
                return false;
            }

            return FocusElement(this) || FocusInTree(this);

            static bool FocusElement(Control c)
            {
                if (Keyboard.IsKeyboardFocusable(c))
                {
                    FocusManager.SetFocusedElement(c.INTERNAL_ParentWindow, c);
                    INTERNAL_HtmlDomManager.SetFocus(c);

                    return true;
                }

                return false;
            }

            static bool FocusInTree(UIElement uie)
            {
                int childrenCount = VisualTreeHelper.GetChildrenCount(uie);
                for (int i = 0; i < childrenCount; i++)
                {
                    UIElement child = VisualTreeHelper.GetChild(uie, i) as UIElement;
                    if (!Keyboard.IsSubTreeFocusable(child))
                    {
                        continue;
                    }

                    if ((child is Control c && FocusElement(c)) || FocusInTree(child))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool UseSystemFocusVisuals { get; set; }

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            if (!DisableBaseControlHandlingOfVisualStates)
            {
                // Go to the default state ("Normal" visual state):
                UpdateVisualStates();

                bool hasMouseOverState = false;
                bool hasPressedState = false;
                bool hasFocusedState = false;
                Collection<VisualStateGroup> groups = (Collection<VisualStateGroup>)this.StateGroupsRoot?.GetValue(VisualStateManager.VisualStateGroupsProperty);
                if (groups != null)
                {
                    foreach (VisualStateGroup group in groups)
                    {
                        foreach (VisualState state in group.States)
                        {
#if MIGRATION
                            if (state.Name == "MouseOver")
#else
                            if (state.Name == "PointerOver")
#endif
                            {
                                hasMouseOverState = true;
                            }
                            else if (state.Name == "Pressed")
                            {
                                hasPressedState = true;
                            }
                            else if (state.Name == "Focused")
                            {
                                hasFocusedState = true;
                            }
                        }
                    }
                }



                // Listen to the Pointer events:
                if (hasMouseOverState)
                {
                    // Note: We unregster the event before registering it because, in case the user removes the control from the visual tree and puts it back, the "OnApplyTemplate" is called again.
#if MIGRATION
                    this.MouseEnter -= Control_MouseEnter;
                    this.MouseEnter += Control_MouseEnter;
                    this.MouseLeave -= Control_MouseLeave;
                    this.MouseLeave += Control_MouseLeave;
#else
            this.PointerEntered -= Control_PointerEntered;
            this.PointerEntered += Control_PointerEntered;
            this.PointerExited -= Control_PointerExited;
            this.PointerExited += Control_PointerExited;
#endif
                }

                if (hasPressedState)
                {
                    // Note: We unregster the event before registering it because, in case the user removes the control from the visual tree and puts it back, the "OnApplyTemplate" is called again.
#if MIGRATION
                    this.MouseLeftButtonDown -= Control_MouseLeftButtonDown;
                    this.MouseLeftButtonDown += Control_MouseLeftButtonDown;
                    this.MouseLeftButtonUp -= Control_MouseLeftButtonUp;
                    this.MouseLeftButtonUp += Control_MouseLeftButtonUp;
#else
            this.PointerPressed -= Control_PointerPressed;
            this.PointerPressed += Control_PointerPressed;
            this.PointerReleased -= Control_PointerReleased;
            this.PointerReleased += Control_PointerReleased;
#endif
                }

                if (hasFocusedState)
                {
                    // Note: We unregster the event before registering it because, in case the user removes the control from the visual tree and puts it back, the "OnApplyTemplate" is called again.
                    //#if MIGRATION
                    //                    this.MouseLeftButtonDown -= Control_MouseLeftButtonDown;
                    //                    this.MouseLeftButtonDown += Control_MouseLeftButtonDown;
                    //                    this.MouseLeftButtonUp -= Control_MouseLeftButtonUp;
                    //                    this.MouseLeftButtonUp += Control_MouseLeftButtonUp;
                    //#else
                    this.GotFocus -= Control_GotFocus;
                    this.GotFocus += Control_GotFocus;
                    this.LostFocus -= Control_LostFocus;
                    this.LostFocus += Control_LostFocus;
                    //#endif
                }
            }
        }

        bool _isFocused = false;
        void Control_LostFocus(object sender, RoutedEventArgs e)
        {
            _isFocused = false;
            UpdateVisualStatesForFocus();

            if (!this._isInvalid)
                return;
            this.UpdateValidationState();
        }

        void Control_GotFocus(object sender, RoutedEventArgs e)
        {
            _isFocused = true;
            UpdateVisualStatesForFocus();

            if (!this._isInvalid)
                return;
            this.UpdateValidationState();
        }

        bool _isPointerOver = false;
        bool _isPressed = false;

#if MIGRATION
        void Control_MouseEnter(object sender, Input.MouseEventArgs e)
#else
void Control_PointerEntered(object sender, Input.PointerRoutedEventArgs e)
#endif
        {
            _isPointerOver = true;
            UpdateVisualStates();
        }

#if MIGRATION
        void Control_MouseLeave(object sender, Input.MouseEventArgs e)
#else
void Control_PointerExited(object sender, Input.PointerRoutedEventArgs e)
#endif
        {
            _isPointerOver = false;
            UpdateVisualStates();
        }

#if MIGRATION
        void Control_MouseLeftButtonDown(object sender, Input.MouseButtonEventArgs e)
#else
void Control_PointerPressed(object sender, Input.PointerRoutedEventArgs e)
#endif
        {
            _isPressed = true;
            UpdateVisualStates();
        }

#if MIGRATION
        void Control_MouseLeftButtonUp(object sender, Input.MouseButtonEventArgs e)
#else
void Control_PointerReleased(object sender, Input.PointerRoutedEventArgs e)
#endif
        {
            _isPressed = false;
            UpdateVisualStates();
        }

        internal virtual void UpdateVisualStates()
        {
            if (!DisableBaseControlHandlingOfVisualStates)
            {
                if (_isDisabled)
                    VisualStateManager.GoToState(this, "Disabled", true);
                else if (_isPressed)
                    VisualStateManager.GoToState(this, "Pressed", true);
                else if (_isPointerOver)
#if MIGRATION
                    VisualStateManager.GoToState(this, "MouseOver", true);
#else
                    VisualStateManager.GoToState(this, "PointerOver", true);
#endif
                else
                    VisualStateManager.GoToState(this, "Normal", true);


            }
        }

        void UpdateVisualStatesForFocus()
        {
            if (!DisableBaseControlHandlingOfVisualStates)
            {
                if (_isFocused)
                {
                    VisualStateManager.GoToState(this, "Focused", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Unfocused", true);
                }
            }
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
#if !BRIDGE
            return base.CreateDomElement(parentRef, out domElementWhereToPlaceChildren);
#else
            return CreateDomElement_WorkaroundBridgeInheritanceBug(parentRef, out domElementWhereToPlaceChildren);
#endif
        }

        /// <summary>
        /// This method is here to avoid creating the dom for a control which has a Template.
        /// It creates the basic dom elements in which we will be able to add the template.
        /// </summary>
        /// <param name="parentRef">The parent of the FrameworkElement</param>
        /// <param name="domElementWhereToPlaceChildren">The dom element where the FrameworkElement's template constructed children will be added.</param>
        /// <returns>The "root" dom element of the FrameworkElement.</returns>
        internal object CreateDomElementForControlTemplate(object parentRef, out object domElementWhereToPlaceChildren)
        {
            // I think this method should in most (all?) case return two divs, as if it was a frameworkElement.
#if !BRIDGE
            return base.CreateDomElement(parentRef, out domElementWhereToPlaceChildren);
#else
            return CreateDomElement_WorkaroundBridgeInheritanceBug(parentRef, out domElementWhereToPlaceChildren);
#endif
        }

        /// <summary>
        /// Returns a value that indicates whether the control is to be rendered with a ControlTemplate.
        /// </summary>
        internal bool HasTemplate
        {
            get
            {
                return this.TemplateCache != null;
            }
        }

        internal void GoToState(string state)
        {
            VisualStateManager.GoToState(this, state, true);
        }

        private bool _isInvalid;

        internal void ShowValidationError()
        {
            this._isInvalid = true;
            this.UpdateValidationState();
        }

        internal void HideValidationError()
        {
            this._isInvalid = false;
            this.UpdateValidationState();
        }

        internal void UpdateValidationState()
        {
            if (this._isInvalid)
            {
                VisualStateManager.GoToState(this, _isFocused ? "InvalidFocused" : "InvalidUnfocused", true);
            }                
            else
            {
                VisualStateManager.GoToState(this, "Valid", true);
            }
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CharacterSpacingProperty =
            DependencyProperty.Register(
                nameof(CharacterSpacing),
                typeof(int),
                typeof(Control),
                new PropertyMetadata(0));

        //
        // Summary:
        //     Gets or sets the distance between characters of text in the control measured
        //     in 1000ths of the font size.
        //
        // Returns:
        //     The distance between characters of text in the control measured in 1000ths of
        //     the font size. The default is 0.
        [OpenSilver.NotImplemented]
        public int CharacterSpacing
        {
            get { return (int)GetValue(CharacterSpacingProperty); }
            set { SetValue(CharacterSpacingProperty, value); }
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TabNavigationProperty = 
            DependencyProperty.Register(
                nameof(TabNavigation), 
                typeof(KeyboardNavigationMode), 
                typeof(Control), 
                new PropertyMetadata(KeyboardNavigationMode.Local));

        [OpenSilver.NotImplemented]
        public KeyboardNavigationMode TabNavigation
        {
            get { return (KeyboardNavigationMode)this.GetValue(Control.TabNavigationProperty); }
            set { this.SetValue(Control.TabNavigationProperty, value); }
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty FontStretchProperty = 
            DependencyProperty.Register(
                nameof(FontStretch), 
                typeof(FontStretch), 
                typeof(Control),
                new FrameworkPropertyMetadata(new FontStretch(), FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        ///     The stretch of the desired font.
        ///     This will only affect controls whose template uses the property
        ///     as a parameter. On other controls, the property will do nothing.
        /// </summary>
        [OpenSilver.NotImplemented]
        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        ////
        //// Summary:
        ////     Called before the System.Windows.UIElement.TextInput event occurs.
        ////
        //// Parameters:
        ////   e:
        ////     A System.Windows.Input.TextCompositionEventArgs that contains the event data.
        //protected virtual void OnTextInput(TextCompositionEventArgs e)
        //{

        //}

        [OpenSilver.NotImplemented]
        protected virtual void OnDrop(DragEventArgs e)
        {

        }

        [OpenSilver.NotImplemented]
        protected virtual void OnDragEnter(DragEventArgs e)
        {

        }

        [OpenSilver.NotImplemented]
        protected virtual void OnDragLeave(DragEventArgs e)
        {

        }

        [OpenSilver.NotImplemented]
        protected virtual void OnTextInputStart(TextCompositionEventArgs e)
        {

        }

        [OpenSilver.NotImplemented]
        protected virtual void OnTextInputUpdate(TextCompositionEventArgs e)
        {

        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            int count = VisualChildrenCount;

            if (count > 0)
            {
                UIElement child = GetVisualChild(0);
                if (child != null)
                {
                    child.Measure(availableSize);
                    return child.DesiredSize;
                }
            }

            return new Size(0.0, 0.0);
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            int count = VisualChildrenCount;

            if (count > 0)
            {
                UIElement child = GetVisualChild(0);
                if (child != null)
                {
                    child.Arrange(new Rect(finalSize));
                }
            }
            return finalSize;
        }
    }
}
