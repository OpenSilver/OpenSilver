
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

using System;
using System.Windows.Browser;
using System.Windows.Input;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    /// <summary>
    /// Provides an inline-level content element that provides facilities for hosting hyperlinks.
    /// </summary>
    public sealed class Hyperlink : Span
    {
        private JavaScriptCallback _clickCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="Hyperlink"/> class.
        /// </summary>
        public Hyperlink()
        {
            Foreground = new SolidColorBrush(Color.FromArgb(255, 51, 124, 187));
            Cursor = Cursors.Hand;
#if MIGRATION
            TextDecorations = Windows.TextDecorations.Underline;
#else
            TextDecorations = Text.TextDecorations.Underline;
#endif
        }

        /// <summary>
        /// Occurs when the left mouse button is clicked on a <see cref="Hyperlink"/>.
        /// </summary>
        public event RoutedEventHandler Click;

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(Hyperlink),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets a command to associate with the <see cref="Hyperlink"/>.
        /// </summary>
        /// <returns>
        /// A command to associate with the <see cref="Hyperlink"/>. The default 
        /// is null.
        /// </returns>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                nameof(CommandParameter),
                typeof(object),
                typeof(Hyperlink),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets command parameters associated with the command specified by the
        /// <see cref="Command"/> property.
        /// </summary>
        /// <returns>
        /// An object specifying parameters for the command specified by the <see cref="Command"/>
        /// property. The default is null.
        /// </returns>
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MouseOverForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseOverForegroundProperty =
            DependencyProperty.Register(
                nameof(MouseOverForeground),
                typeof(Brush),
                typeof(Hyperlink),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 237, 110, 0)))
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        const string MouseOverForegroundVariable = "--mouse-over-color";

                        Hyperlink hyperlink = (Hyperlink)d;
                        string color = (Brush)newValue switch
                        {
                            SolidColorBrush solidColorBrush => solidColorBrush.INTERNAL_ToHtmlString(),
                            _ => string.Empty,
                        };

                        INTERNAL_HtmlDomManager.GetDomElementStyleForModification(hyperlink.INTERNAL_OuterDomElement).setProperty(
                            MouseOverForegroundVariable,
                            color);
                    },
                });

        /// <summary>
        /// Gets or sets the brush that paints the foreground color when the mouse pointer
        /// moves over the <see cref="Hyperlink"/>.
        /// </summary>
        /// <returns>
        /// The brush that paints the foreground color when the mouse pointer moves over
        /// the <see cref="Hyperlink"/>.
        /// </returns>
        public Brush MouseOverForeground
        {
            get => (Brush)GetValue(MouseOverForegroundProperty);
            set => SetValue(MouseOverForegroundProperty, value);
        }

#if MIGRATION
        /// <summary>
        /// Identifies the <see cref="MouseOverTextDecorations"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseOverTextDecorationsProperty =
            DependencyProperty.Register(
                nameof(MouseOverTextDecorations),
                typeof(TextDecorationCollection),
                typeof(Hyperlink),
                new PropertyMetadata(Windows.TextDecorations.Underline)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        const string MouseOverTextDecorationsVariable = "--mouse-over-decoration";

                        Hyperlink hyperlink = (Hyperlink)d;
                        string value = ((TextDecorationCollection)newValue)?.ToHtmlString() ?? string.Empty;
                        INTERNAL_HtmlDomManager.GetDomElementStyleForModification(hyperlink.INTERNAL_OuterDomElement).setProperty(
                            MouseOverTextDecorationsVariable,
                            value);
                    },
                });

        /// <summary>
        /// Gets or sets the <see cref="TextDecorationCollection"/> that decorates the <see cref="Hyperlink"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="TextDecorationCollection"/> that decorates the <see cref="Hyperlink"/>.
        /// </returns>
        public TextDecorationCollection MouseOverTextDecorations
        {
            get => (TextDecorationCollection)GetValue(MouseOverTextDecorationsProperty);
            set => SetValue(MouseOverTextDecorationsProperty, value);
        }
#else
        /// <summary>
        /// Identifies the <see cref="MouseOverTextDecorations"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseOverTextDecorationsProperty =
            DependencyProperty.Register(
                nameof(MouseOverTextDecorations),
                typeof(Text.TextDecorations?),
                typeof(Hyperlink),
                new PropertyMetadata(Text.TextDecorations.Underline)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        const string MouseOverTextDecorationsVariable = "--mouse-over-decoration";

                        Hyperlink hyperlink = (Hyperlink)d;
                        string value = (Text.TextDecorations?)newValue switch
                        {
                            Text.TextDecorations.OverLine => "overline",
                            Text.TextDecorations.Strikethrough => "line-through",
                            Text.TextDecorations.Underline => "underline",
                            _ => string.Empty,
                        };
                        INTERNAL_HtmlDomManager.GetDomElementStyleForModification(hyperlink.INTERNAL_OuterDomElement).setProperty(
                            MouseOverTextDecorationsVariable,
                            value);
                    },
                });

        /// <summary>
        /// Gets or sets the <see cref="Text.TextDecorations"/> that decorates the <see cref="Hyperlink"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Text.TextDecorations"/> that decorates the <see cref="Hyperlink"/>.
        /// </returns>
        public Text.TextDecorations? MouseOverTextDecorations
        {
            get => (Text.TextDecorations?)GetValue(MouseOverTextDecorationsProperty);
            set => SetValue(MouseOverTextDecorationsProperty, value);
        }
#endif

        /// <summary>
        /// Identifies the <see cref="NavigateUri"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigateUriProperty =
            DependencyProperty.Register(
                nameof(NavigateUri),
                typeof(Uri),
                typeof(Hyperlink),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets a URI to navigate to when the <see cref="Hyperlink"/>
        /// is activated.
        /// </summary>
        /// <returns>
        /// The URI to navigate to when the <see cref="Hyperlink"/> is activated.
        /// The default is null.
        /// </returns>
        public Uri NavigateUri
        {
            get => (Uri)GetValue(NavigateUriProperty);
            set => SetValue(NavigateUriProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TargetName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetNameProperty =
            DependencyProperty.Register(
                nameof(TargetName),
                typeof(string),
                typeof(Hyperlink),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the name of a target window or frame for the <see cref="Hyperlink"/>.
        /// </summary>
        /// <returns>
        /// A string that specifies the name of a target window or frame for the <see cref="Hyperlink"/>.
        /// </returns>
        public string TargetName
        {
            get => (string)GetValue(TargetNameProperty);
            set => SetValue(TargetNameProperty, value);
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();
            INTERNAL_HtmlDomManager.AddCSSClass(INTERNAL_OuterDomElement, "opensilver-hyperlink");
        }

        public override void INTERNAL_AttachToDomEvents()
        {
            base.INTERNAL_AttachToDomEvents();

            _clickCallback = JavaScriptCallback.Create(OnClickNative, true);

            string sDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement);
            string sClickCallback = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_clickCallback);
            OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                $"{sDiv}.addEventListener('click', function (e) {{ {sClickCallback}(); }});");
        }

        public override void INTERNAL_DetachFromDomEvents()
        {
            base.INTERNAL_DetachFromDomEvents();

            _clickCallback?.Dispose();
            _clickCallback = null;
        }

        private void OnClickNative() => OnClick();

        private void OnClick()
        {
            Click?.Invoke(this, new RoutedEventArgs { OriginalSource = this });

            ExecuteCommand();

            if (NavigateUri is Uri uri)
            {
                HtmlPage.Window.Navigate(uri, GetTargetName());
            }
        }

        private void ExecuteCommand()
        {
            if (Command is ICommand command)
            {
                object parameter = CommandParameter;
                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }
        }

        private string GetTargetName()
        {
            string targetName = TargetName;
            return string.IsNullOrEmpty(targetName) ? "_blank" : targetName;
        }
    }
}
