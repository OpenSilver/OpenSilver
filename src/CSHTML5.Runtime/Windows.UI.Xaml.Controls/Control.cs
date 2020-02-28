

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#else
using Windows.UI.Xaml.Media;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
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
        private UIElement _renderedControlTemplate = null;
        private bool _isDisabled = false;

        /// <summary>
        /// Derived classes can set this flag in their constructor to prevent the "Template" property from being applied.
        /// </summary>
        protected bool INTERNAL_DoNotApplyControlTemplate = false;


        /// <summary>
        /// Derived classes can set this flag to True in their constructor in order to disable the "GoToState" calls of this class related to PointerOver/Pressed/Disabled, and handle them by themselves. An example is the ToggleButton control, which contains states such as "CheckedPressed", "CheckedPointerOver", etc.
        /// </summary>
        protected bool DisableBaseControlHandlingOfVisualStates = false;

#if REVAMPPOINTEREVENTS
        internal override bool INTERNAL_ManageFrameworkElementPointerEventsAvailability()
        {
            return true;
        }
#endif

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
        /// Identifies the Background dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(Control), new PropertyMetadata(null
#if REVAMPPOINTEREVENTS
                , Background_Changed
#endif
                )
            {
                GetCSSEquivalent = (instance) =>
                    {
                        return new CSSEquivalent()
                        {
                            Name = new List<string> { "background", "backgroundColor", "backgroundColorAlpha" },
                        };
                    }
                    //GetCSSEquivalents = (instance) =>
                    //    {
                    //        Control control = (Control)instance;
                    //        List<CSSEquivalent> result = new List<CSSEquivalent>();
                    //        if (control.Background != null)
                    //        {
                    //            Brush background = (Brush)control.Background;
                    //            List<KeyValuePair<DependencyObject, DependencyProperty>> tuples = new List<KeyValuePair<DependencyObject, DependencyProperty>>();
                    //            //we make a copy of the list of tuples so that we can modify it while going though it.
                    //            foreach (KeyValuePair<DependencyObject, DependencyProperty> tuple in background.PropertiesWhereUsed)
                    //            {
                    //                tuples.Add(tuple);
                    //            }
                    //            foreach (KeyValuePair<DependencyObject, DependencyProperty> tuple in tuples)
                    //            {
                    //                UIElement uiElement = tuple.Key as UIElement;
                    //                DependencyProperty dependencyProperty = tuple.Value;
                    //                if (uiElement != null)
                    //                {
                    //                    if (INTERNAL_VisualTreeManager.IsElementInVisualTree(uiElement))
                    //                    {
                    //                        PropertyMetadata propertyMetadata = dependencyProperty.GetTypeMetaData(uiElement.GetType());
                    //                        //if (propertyMetadata.GetCSSEquivalent != null) //todo: temporary test so remove it?
                    //                        //{
                    //                            //var parentPropertyCSSEquivalent = propertyMetadata.GetCSSEquivalent(uiElement);
                    //                            List<string> names = new List<string> { "background", "backgroundColor", "backgroundColorAlpha" };

                    //                            result.Add(new CSSEquivalent()
                    //                            {
                    //                                Name = names,

                    //                                Value = (inst, value) =>
                    //                                {
                    //                                    Dictionary<string, object> valuesDict = new Dictionary<string, object>();
                    //                                    foreach (string name in names)
                    //                                    {
                    //                                        if (!name.EndsWith("Alpha"))
                    //                                        {
                    //                                            valuesDict.Add(name, ((Color)value).INTERNAL_ToHtmlStringForVelocity());
                    //                                        }
                    //                                        else
                    //                                        {
                    //                                            valuesDict.Add(name, ((double)((Color)value).A) / 255);
                    //                                        }
                    //                                    }
                    //                                    return valuesDict;
                    //                                },
                    //                                //DomElement = parentPropertyCSSEquivalent.DomElement,
                    //                                UIElement = uiElement
                    //                            });
                    //                        //}
                    //                        //else
                    //                        //{
                    //                        //    //we want to create a CSSEquivalent that will just make the UIElement call the property callback if any:
                    //                        //    if (propertyMetadata.PropertyChangedCallback != null)
                    //                        //    {
                    //                        //        result.Add(new CSSEquivalent()
                    //                        //        {
                    //                        //            UIElement = uiElement,
                    //                        //            CallbackMethod = propertyMetadata.PropertyChangedCallback,
                    //                        //            DependencyProperty = dependencyProperty
                    //                        //        });
                    //                        //    }
                    //                        //}
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    //Commented because it could be a Setter in a Style
                    //                    //throw new NotSupportedException("A solidColorBrush cannot currently be set inside a class that desn't inherit from UIElement.");
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            result.Add(new CSSEquivalent()
                    //                {
                    //                    Name = new List<string> { "background", "backgroundColor", "backgroundColorAlpha" },
                    //                });
                    //        }
                    //        return result;

                    //    }
            }
            );

#if REVAMPPOINTEREVENTS
        internal virtual void OnBackgroundPropertyChanged(Brush background)
        {
            INTERNAL_UpdateCssPointerEvents(this);
        }

        private static void Background_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control control = (Control)d;
            control.OnBackgroundPropertyChanged(e.NewValue as Brush);
        }
#endif

        internal bool INTERNAL_IsLegacyVisualStates
        {
            get
            {
                return this.INTERNAL_GetVisualStateGroups().ContainsVisualState(VisualStates.StateMouseOver);
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
        /// Identifies the BorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(Control), new PropertyMetadata(null)
            {
                GetCSSEquivalent = (instance) =>
                {
                    return new CSSEquivalent()
                    {
                        Name = new List<string> { "borderColor" },
                    };
                }
            }
            );

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
        /// Identifies the BorderThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(Control), new PropertyMetadata(new Thickness()) { MethodToUpdateDom = BorderThickness_MethodToUpdateDom });

        static void BorderThickness_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var control = (Control)d;
            if (!control.HasTemplate && newValue != null) //todo: check why sometimes this is null.
            {
                var newThickness = (Thickness)newValue;
                dynamic domElement = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(control);
                domElement.borderStyle = "solid"; //todo: see if we should put this somewhere else
                domElement.borderWidth = newThickness.Top + "px " + newThickness.Right + "px " + newThickness.Bottom + "px " + newThickness.Left + "px ";
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
        /// Identifies the FontWeight dependency property.
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(Control), new PropertyMetadata(FontWeights.Normal)
            {
                GetCSSEquivalent = (instance) =>
                {
                    return new CSSEquivalent()
                    {
                        Value = (inst, value) =>
                        {
                            if (value != null)
                            {
                                return ((FontWeight)value).Weight.ToString();
                            }
                            else
                            {
                                return "";
                            }
                        },
                        Name = new List<string> { "fontWeight" },
                        ApplyAlsoWhenThereIsAControlTemplate = true // (See comment where this property is defined)
                    };
                }
            }
            );


        /// <summary>
        /// Gets or sets the style in which the text is rendered.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }
        /// <summary>
        /// Identifies the FontStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty =
#if MIGRATION
            DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(Control), new PropertyMetadata(FontStyles.Normal)
#else
            DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(Control), new PropertyMetadata(FontStyle.Normal)
#endif
            {
                GetCSSEquivalent = (instance) =>
                {
                    return new CSSEquivalent()
                    {
                        Value = (inst, value) =>
                        {
                            if (value != null)
                            {
                                return ((FontStyle)value).ToString().ToLower();
                            }
                            else
                            {
                                return "";
                            }
                        },
                        Name = new List<string> { "fontStyle" },
                        ApplyAlsoWhenThereIsAControlTemplate = true // (See comment where this property is defined)
                    };
                }
            }
            );



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
        /// Identifies the Foreground dependency property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(Control), new PropertyMetadata(new SolidColorBrush(Colors.Black))
            {
                GetCSSEquivalent = (instance) =>
                {
                    return new CSSEquivalent()
                    {
                        Name = new List<string> { "color", "colorAlpha" },
                        ApplyAlsoWhenThereIsAControlTemplate = true // (See comment where this property is defined)
                    };
                }
            }
            );


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
        /// Identifies the FontFamily dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(Control), new PropertyMetadata(null) //todo: put a default value?
            {
                GetCSSEquivalent = (instance) =>
                {
                    return new CSSEquivalent()
                    {
                        Value = (inst, value) =>
                        {
                            if (value != null)
                            {
                                if (value is FontFamily)
                                {
                                    return INTERNAL_FontsHelper.LoadFont(((FontFamily)value).Source, (UIElement)instance);
                                }
                                else
                                {
                                    try
                                    {
                                        return INTERNAL_FontsHelper.LoadFont(value.ToString(), (UIElement)instance);
                                    }
                                    catch
                                    {
                                        return "";
                                    }
                                }
                            }
                            else
                            {
                                return "";
                            }
                        },
                        Name = new List<string> { "fontFamily" },
                        ApplyAlsoWhenThereIsAControlTemplate = true // (See comment where this property is defined)
                    };
                }
            }
            );


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
        /// Identifies the FontSize dependency property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(Control), new PropertyMetadata(11d)
            {
                MethodToUpdateDom = (instance, newValue) =>
                {
                    // When the FontSize changes, we also want to set the "Line-Height" CSS property in order to get the exact same result as in Silverlight:
                    var domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification((UIElement)instance);
                    domStyle.lineHeight = "125%";
                },
                GetCSSEquivalent = (instance) =>
                {
                    return new CSSEquivalent()
                    {
                        Value = (inst, value) =>
                        {
                            return (Math.Floor((double)value * 1000) / 1000).ToString() + "px"; // Note: We multiply by 1000 and then divide by 1000 so as to only keep 3 decimals at the most.
                        },
                        Name = new List<string> { "fontSize" },
                        ApplyAlsoWhenThereIsAControlTemplate = true // (See comment where this property is defined)
                    };
                }
            }
            );

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
        /// Identifies the TextDecorations dependency property.
        /// </summary>
        public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register("TextDecorations",
                                                                                                        typeof(TextDecorationCollection),
                                                                                                        typeof(Control),
                                                                                                        new PropertyMetadata(System.Windows.TextDecorations.None)
                                                                                                        {
                                                                                                            GetCSSEquivalent = INTERNAL_GetCSSEquivalentForTextDecorations
                                                                                                        });
        internal static CSSEquivalent INTERNAL_GetCSSEquivalentForTextDecorations(DependencyObject instance)
        {
            return new CSSEquivalent()
            {
                Value = (inst, value) =>
                {
                    TextDecorationCollection newTextDecoration = (TextDecorationCollection)value;
                    if (newTextDecoration == System.Windows.TextDecorations.OverLine)
                    {
                        return "overline";
                    }
                    else if (newTextDecoration == System.Windows.TextDecorations.Strikethrough)
                    {
                        return "line-through";
                    }
                    else if (newTextDecoration == System.Windows.TextDecorations.Underline)
                    {
                        return "underline";
                    }
                    else
                    {
                        return string.Empty;
                    }
                },
                Name = new List<string> { "textDecoration" },
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
            DependencyProperty.Register("TextDecorations", typeof(TextDecorations?), typeof(Control), new PropertyMetadata(null)
            {
                GetCSSEquivalent = INTERNAL_GetCSSEquivalentForTextDecorations
            }
            );

        internal static CSSEquivalent INTERNAL_GetCSSEquivalentForTextDecorations(DependencyObject instance)
        {
            return new CSSEquivalent()
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

        // Returns:
        //     The dimensions of the space between the border and its child as a Thickness
        //     value. Thickness is a structure that stores dimension values using pixel
        //     measures.
        /// <summary>
        /// Gets or sets the distance between the border and its child object.
        /// </summary>
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }
        /// <summary>
        /// Identifies the Padding dependency property.
        /// </summary>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(Thickness), typeof(Control), new PropertyMetadata(new Thickness()) { MethodToUpdateDom = Padding_MethodToUpdateDom });
        private static void Padding_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var control = (Control)d;
            if (!(control.INTERNAL_VisualParent is Canvas) && !control.HasTemplate) //if the parent is a canvas, we ignore this property and we want to ignore this property if there is a ControlTemplate on this control.
            {
                var innerDomElement = control.INTERNAL_InnerDomElement;
                if (innerDomElement != null)
                {
                    var styleOfInnerDomElement = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(innerDomElement);
                    Thickness newPadding;
                    if (newValue != null)
                        newPadding = (Thickness)newValue;
                    else
                        newPadding = new Thickness();
                    if (newPadding == null) //if it is null, we want 0 everywhere
                    {
                        newPadding = new Thickness();
                    }
                    //todo: if the container has a padding, add it to the margin
                    styleOfInnerDomElement.boxSizing = "border-box";
                    styleOfInnerDomElement.paddingLeft = newPadding.Left + "px";
                    styleOfInnerDomElement.paddingTop = newPadding.Top + "px";
                    styleOfInnerDomElement.paddingRight = newPadding.Right + "px";
                    styleOfInnerDomElement.paddingBottom = newPadding.Bottom + "px";
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
        /// Identifies the HorizontalContentAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(Control), new PropertyMetadata(HorizontalAlignment.Center));


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
        /// Identifies the VerticalContentAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalContentAlignmentProperty =
            DependencyProperty.Register("VerticalContentAlignment", typeof(VerticalAlignment), typeof(Control), new PropertyMetadata(VerticalAlignment.Center));


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
        public static readonly DependencyProperty TabIndexProperty =
            DependencyProperty.Register("TabIndex", typeof(int), typeof(Control), new PropertyMetadata(int.MaxValue)
            {
                MethodToUpdateDom = TabIndexProperty_MethodToUpdateDom,
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
            });

        const int TABINDEX_BROWSER_MAX_VALUE = 32767;

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
                if (!this.UseSystemFocusVisuals || this.INTERNAL_GetVisualStateGroups().ContainsVisualState("Focused"))
                {
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
        /// Identifies the Control.IsTabStop dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTabStopProperty =
            DependencyProperty.Register("IsTabStop", typeof(bool), typeof(Control), new PropertyMetadata(true)
            {
                //MethodToUpdateDom = TabIndexProperty_MethodToUpdateDom,
                MethodToUpdateDom = TabStopProperty_MethodToUpdateDom,
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never //there is no point in calling this since it does the exact same thing as the one for TabIndex, which is always called.
            });

        //-----------------------
        // TEMPLATE
        //-----------------------

        internal bool INTERNAL_IsTemplated = false; //todo: use only this or HasTemplate (whith IsTemplated's efficiency, which means not reading the DependencyProperty and HasTemplate's accuracy, which means taking into consideration INTERNAL_DoNotApplyControlTemplate).

        /// <summary>
        /// Gets or sets a control template.
        /// </summary>
        public ControlTemplate Template
        {
            get { return (ControlTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }
        /// <summary>
        /// Identifies the Template dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(ControlTemplate), typeof(Control), new PropertyMetadata(null, Template_Changed));

        private static void Template_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control control = (Control)d;
            ControlTemplate controlTemplate = (ControlTemplate)e.NewValue;
            if (controlTemplate == null)
            {
                control.INTERNAL_IsTemplated = false;
            }
            else
            {
                control.INTERNAL_IsTemplated = true;
            }
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(control))
            {

                //detach the former control:
                if (control._renderedControlTemplate != null)
                {
                    INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(control._renderedControlTemplate, control);
                    control._renderedControlTemplate = null;
                    control.ClearRegisteredNames();
                    control.INTERNAL_GetVisualStateGroups().Clear();
                }

                //attach the new control:
                //attacher controlTemplate.INTERNAL_InstantiateFrameworkTemplate();
                FrameworkElement generatedControl;
                if (controlTemplate != null && !control.INTERNAL_DoNotApplyControlTemplate)
                {
                    // Apply the control template:
                    generatedControl = controlTemplate.INTERNAL_InstantiateAndAttachControlTemplate(templateOwner: control);
                    control._renderedControlTemplate = generatedControl;
                }
                else
                {
                    //todo: display the child the same way as in Content_Changed.
                }
            }
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


        #region ---------- INameScope implementation ----------
        //note: copy from UserControl
        Dictionary<string, object> _nameScopeDictionary = new Dictionary<string, object>();

        /// <summary>
        /// Finds the UIElement with the specified name. Returns null if not found.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <returns>The object with the specified name if any; otherwise null.</returns>
        private object TryFindTemplateChildFromName(string name)
        {
            //todo: see if this fits to the behaviour it should have.
            if (_nameScopeDictionary.ContainsKey(name))
                return _nameScopeDictionary[name];
            else
                return null;
        }

        public void RegisterName(string name, object scopedElement)
        {
            if (_nameScopeDictionary.ContainsKey(name) && _nameScopeDictionary[name] != scopedElement)
                throw new ArgumentException(string.Format("Cannot register duplicate name '{0}' in this scope.", name));

            _nameScopeDictionary[name] = scopedElement;
        }

        public void UnregisterName(string name)
        {
            if (!_nameScopeDictionary.ContainsKey(name))
                throw new ArgumentException(string.Format("Name '{0}' was not found.", name));

            _nameScopeDictionary.Remove(name);
        }

        void ClearRegisteredNames()
        {
            _nameScopeDictionary.Clear();
        }


        #endregion














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


        private INTERNAL_VisualStateGroupCollection _visualStateGroups;
        public INTERNAL_VisualStateGroupCollection INTERNAL_GetVisualStateGroups()
        {
            if (_visualStateGroups == null)
            {
                _visualStateGroups = new INTERNAL_VisualStateGroupCollection();
            }
            return _visualStateGroups;
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

                // Listen to the Pointer events:
                if (_visualStateGroups != null
#if MIGRATION
                    && _visualStateGroups.ContainsVisualState("MouseOver"))
#else
 && _visualStateGroups.ContainsVisualState("PointerOver"))
#endif
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

                if (_visualStateGroups != null && _visualStateGroups.ContainsVisualState("Pressed"))
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

                if (_visualStateGroups != null && _visualStateGroups.ContainsVisualState("Focused"))
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

        void UpdateVisualStates()
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
                return Template != null && INTERNAL_DoNotApplyControlTemplate == false;
            }
        }

        internal override void INTERNAL_OnVisualParentChanged()
        {
            this.UpdateTabIndex(this.IsTabStop, this.TabIndex);
            base.INTERNAL_OnVisualParentChanged();
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
        protected virtual void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {

        }
#endif

        /// <summary>Called before the <see cref="E:System.Windows.UIElement.MouseWheel" /> event occurs to provide handling for the event in a derived class without attaching a delegate. </summary>
        /// <param name="e">A <see cref="T:System.Windows.Input.MouseWheelEventArgs" /> that contains the event data.</param>
        protected virtual void OnMouseWheel(MouseWheelEventArgs e)
        {

        }

        public static readonly DependencyProperty TabNavigationProperty = DependencyProperty.Register("TabNavigation", typeof(KeyboardNavigationMode), typeof(Control), new PropertyMetadata(KeyboardNavigationMode.Local));

        public KeyboardNavigationMode TabNavigation
        {
            get { return (KeyboardNavigationMode)this.GetValue(Control.TabNavigationProperty); }
            set { this.SetValue(Control.TabNavigationProperty, value); }
        }

        public static readonly DependencyProperty FontStretchProperty = DependencyProperty.Register("FontStretch", typeof(FontStretch), typeof(Control), new PropertyMetadata(default(FontStretch)));

        /// <summary>
        ///     The stretch of the desired font.
        ///     This will only affect controls whose template uses the property
        ///     as a parameter. On other controls, the property will do nothing.
        /// </summary>
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
        protected virtual void OnTextInput(TextCompositionEventArgs e)
        {

        }

        public bool ApplyTemplate()
        {
            return false;
        }

        protected virtual void OnDrop(DragEventArgs e)
        {

        }

        protected virtual void OnDragEnter(DragEventArgs e)
        {

        }

        protected virtual void OnDragLeave(DragEventArgs e)
        {

        }

        protected virtual void OnTextInputStart(TextCompositionEventArgs e)
        {

        }

        protected virtual void OnTextInputUpdate(TextCompositionEventArgs e)
        {

        }
#endif
    }
}
