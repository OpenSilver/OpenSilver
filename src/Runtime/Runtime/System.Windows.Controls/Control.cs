
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
using System.Windows.Controls.Primitives;

#if MIGRATION
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Data;
#else
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.Foundation;
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
        [Obsolete("This value is ignored. ControlTemplate is always applied.")]
        protected bool INTERNAL_DoNotApplyControlTemplate = false;

        /// <summary>
        /// Derived classes can set this flag to True in their constructor in order to disable the "GoToState" calls of this class related to PointerOver/Pressed/Disabled, and handle them by themselves. An example is the ToggleButton control, which contains states such as "CheckedPressed", "CheckedPointerOver", etc.
        /// </summary>
        protected bool DisableBaseControlHandlingOfVisualStates = false;

        internal override bool EnablePointerEventsCore
        {
            get
            {
                // Note: this should always be false, but we return true to prevent issue with
                // controls that do not have template (like ScrollViewer).
                return true;
            }
        }

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
                    GetCSSEquivalent = (instance) => new CSSEquivalent
                    {
                        Name = new List<string> { "background", "backgroundColor", "backgroundColorAlpha" },
                    },
                    MethodToUpdateDom = (d, e) =>
                    {
                        UIElement.SetPointerEvents((Control)d);
                    },
                });

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
                new PropertyMetadata((object)null)
                {
                    GetCSSEquivalent = (instance) => new CSSEquivalent
                    {
                        Name = new List<string> { "borderColor" },
                    }
                });

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
                new PropertyMetadata(new Thickness())
                {
                    MethodToUpdateDom = BorderThickness_MethodToUpdateDom,
                });

        private static void BorderThickness_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var control = (Control)d;
            if (!control.HasTemplate)
            {
                var newThickness = (Thickness)newValue;
                var domElement = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(control);
                domElement.borderStyle = "solid"; //todo: see if we should put this somewhere else
                domElement.borderWidth = $"{newThickness.Top.ToString()}px {newThickness.Right.ToString()}px {newThickness.Bottom.ToString()}px {newThickness.Left.ToString()}px";
            }
        }

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
                    GetCSSEquivalent = (instance) => new CSSEquivalent
                    {
                        Value = (inst, value) => ((FontWeight)value).Weight.ToInvariantString(),
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
                    GetCSSEquivalent = (instance) => new CSSEquivalent
                    {
                        Name = new List<string> { "color", "colorAlpha" },
                        ApplyAlsoWhenThereIsAControlTemplate = true // (See comment where this property is defined)
                    }
                });

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
#if GD_WIP
                            if (value is Binding binding)
                            {
                                value = binding.Source;
                                binding.Path.Path.Split('.')
                                    .ForEach(p =>
                                        value = value.GetType().GetProperty(p).GetValue(value)
                                    );
                            }

#endif                      // Note: We multiply by 1000 and then divide by 1000 so as to only keep 3 
                            // decimals at the most.
                            return (Math.Floor((double)value * 1000) / 1000).ToInvariantString() + "px"; 
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
                new FrameworkPropertyMetadata((object)null, FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    GetCSSEquivalent = INTERNAL_GetCSSEquivalentForTextDecorations,
                });

        internal static CSSEquivalent INTERNAL_GetCSSEquivalentForTextDecorations(DependencyObject instance)
        {
            return new CSSEquivalent
            {
                Value = (inst, value) => ((TextDecorationCollection)value)?.ToHtmlString() ?? string.Empty,
                Name = new List<string>(1) { "textDecoration" },
            };
        }
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
                    GetCSSEquivalent = INTERNAL_GetCSSEquivalentForTextDecorations,
                });

        internal static CSSEquivalent INTERNAL_GetCSSEquivalentForTextDecorations(DependencyObject instance)
        {
            return new CSSEquivalent
            {
                Value = (inst, value) =>
                {
#if BRIDGE
                    if (value != null) //todo: remove this line when Bridge.NET no longer raises exception on the following lines (cf. styles kit v1.1)
                    {
#endif
                        TextDecorations? newTextDecoration = (TextDecorations?)value;
                        if (newTextDecoration.HasValue)
                        {
                            switch (newTextDecoration)
                            {
                                case Windows.UI.Text.TextDecorations.OverLine:
                                    return "overline";
                                case Windows.UI.Text.TextDecorations.Strikethrough:
                                    return "line-through";
                                case Windows.UI.Text.TextDecorations.Underline:
                                    return "underline";
                                case Windows.UI.Text.TextDecorations.None:
                                default:
                                    return ""; // Note: this will reset the value.
                            }
                        }
                        else
                        {
                            return "";
                        }
#if BRIDGE
                    }
                    else
                    {
                        return "";
                    }
#endif
                },
                Name = new List<string> { "textDecoration" },
            };
        }
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
        /// Identifies the <see cref="Control.Padding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(
                nameof(Padding), 
                typeof(Thickness), 
                typeof(Control),
                new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure)
                { 
                    MethodToUpdateDom = Padding_MethodToUpdateDom,
                });

        private static void Padding_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var control = (Control)d;
            // if the parent is a canvas, we ignore this property and we want to ignore this
            // property if there is a ControlTemplate on this control.
            if (!(control.INTERNAL_VisualParent is Canvas) && !control.HasTemplate && !control.IsUnderCustomLayout) 
            {
                var innerDomElement = control.INTERNAL_InnerDomElement;
                if (innerDomElement != null)
                {
                    var styleOfInnerDomElement = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(innerDomElement);
                    Thickness newPadding = (Thickness)newValue;
                    
                    // todo: if the container has a padding, add it to the margin
                    styleOfInnerDomElement.boxSizing = "border-box";
                    styleOfInnerDomElement.padding = $"{newPadding.Top.ToString()}px {newPadding.Right.ToString()}px {newPadding.Bottom.ToString()}px {newPadding.Left.ToString()}px";
                }
            }
        }

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
        /// Identifies the <see cref="Control.HorizontalContentAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.Register(
                nameof(HorizontalContentAlignment), 
                typeof(HorizontalAlignment), 
                typeof(Control),
                new FrameworkPropertyMetadata(HorizontalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

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
        /// Identifies the <see cref="Control.VerticalContentAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalContentAlignmentProperty =
            DependencyProperty.Register(
                nameof(VerticalContentAlignment), 
                typeof(VerticalAlignment), 
                typeof(Control),
                new FrameworkPropertyMetadata(VerticalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

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
        /// Identifies the <see cref="Control.TabIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabIndexProperty =
            DependencyProperty.Register(
                nameof(TabIndex), 
                typeof(int), 
                typeof(Control), 
                new PropertyMetadata(int.MaxValue)
                {
                    MethodToUpdateDom = TabIndexProperty_MethodToUpdateDom,
                });

        private const int TABINDEX_BROWSER_MAX_VALUE = 32767;

        internal virtual bool INTERNAL_GetFocusInBrowser
        {
            get { return false; }
        }

        internal virtual void UpdateTabIndex(bool isTabStop, int tabIndex)
        {
            var domElementConcernedByFocus = this.INTERNAL_OptionalSpecifyDomElementConcernedByFocus ?? this.INTERNAL_OuterDomElement;
            if (!isTabStop || !this.IsEnabled)
            {
                this.PreventFocusEvents();
                INTERNAL_HtmlDomManager.SetDomElementAttribute(domElementConcernedByFocus, "tabIndex", this.INTERNAL_GetFocusInBrowser ? "-1" : string.Empty);
            }
            else
            {
                //Note: according to W3C, tabIndex needs to be between 0 and 32767 on browsers: https://www.w3.org/TR/html401/interact/forms.html#adef-tabindex
                //      also, the behaviour of the different browsers outside of these values can be different and therefore, we have to restrict the values.

                this.AllowFocusEvents();

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
                if (!this.UseSystemFocusVisuals ||
                    (groups != null && groups.Any(gr => ((IList<VisualState>)gr.States).Any(state => state.Name == "Focused"))))
                {

                    // this.INTERNAL_GetVisualStateGroups().ContainsVisualState("Focused")
                    INTERNAL_HtmlDomManager.SetDomElementStyleProperty(domElementConcernedByFocus, new List<string>() { "outline" }, "none");
                }
            }
        }

        internal static void TabIndexProperty_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var control = (Control)d;
            control.UpdateTabIndex(control.IsTabStop, (int)newValue);
        }

        internal static void TabStopProperty_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var control = (Control)d;
            control.UpdateTabIndex((bool)newValue, control.TabIndex);
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
                    MethodToUpdateDom = TabStopProperty_MethodToUpdateDom,
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

            control.ClearRegisteredNames();

            if (VisualTreeHelper.GetParent(control) != null)
            {
                control.InvalidateMeasureInternal();
            }
        }

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
        protected internal DependencyObject GetTemplateChild(string childName)
        {
            return (DependencyObject)this.TryFindTemplateChildFromName(childName);
        }

        internal void RaiseOnApplyTemplate()
        {
            this.OnApplyTemplate();
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
            if (IsTabStop)
            {
                INTERNAL_HtmlDomManager.SetFocus(this);
                return true; //todo: see if there is a way for this to fail, in which case we want to return false.
            }
            return false;
        }

        private bool _useSystemFocusVisuals = false;
        /// <summary>
        /// Determines whether the control displays the browser's default outline when Focused.
        /// This property is ignored for Controls with a Template that defines the "Focused" VisualState.
        /// The default value is False.
        /// </summary>
        public bool UseSystemFocusVisuals
        {
            get { return _useSystemFocusVisuals; }
            set { _useSystemFocusVisuals = value; } //todo: change the element in the visual tree?
        }

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
        }

        void Control_GotFocus(object sender, RoutedEventArgs e)
        {
            _isFocused = true;
            UpdateVisualStatesForFocus();
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
                return this._templateCache != null;
            }
        }

        internal void GoToState(string state)
        {
            VisualStateManager.GoToState(this, state, true);
        }

#if WORKINPROGRESS
#if MIGRATION
        //
        // Summary:
        //     Called before the System.Windows.UIElement.MouseRightButtonDown event occurs.
        //
        // Parameters:
        //   e:
        //     A System.Windows.Input.MouseButtonEventArgs that contains the event data.
        [OpenSilver.NotImplemented]
        protected virtual void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {

        }
#endif

        /// <summary>Called before the <see cref="E:System.Windows.UIElement.MouseWheel" /> event occurs to provide handling for the event in a derived class without attaching a delegate. </summary>
        /// <param name="e">A <see cref="T:System.Windows.Input.MouseWheelEventArgs" /> that contains the event data.</param>
        [OpenSilver.NotImplemented]
        protected virtual void OnMouseWheel(MouseWheelEventArgs e)
        {

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

        //
        // Summary:
        //     Called before the System.Windows.UIElement.TextInput event occurs.
        //
        // Parameters:
        //   e:
        //     A System.Windows.Input.TextCompositionEventArgs that contains the event data.
        [OpenSilver.NotImplemented]
        protected virtual void OnTextInput(TextCompositionEventArgs e)
        {

        }

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
#endif
        protected override Size MeasureOverride(Size availableSize)
        {
            IEnumerable<DependencyObject> childElements = VisualTreeHelper.GetVisualChildren(this);
            if (childElements.Count() == 0)
            {
                return new Size();
            }

            Size extent = new Size(0.0, 0.0);

            foreach (DependencyObject child in childElements)
            {
                if (child as FrameworkElement == null)
                    continue;

                FrameworkElement childElement = child as FrameworkElement; 
                childElement.Measure(availableSize);
                extent.Width += childElement.DesiredSize.Width;
                extent.Height += childElement.DesiredSize.Height;
            }
            return extent;
        }

    }
}
