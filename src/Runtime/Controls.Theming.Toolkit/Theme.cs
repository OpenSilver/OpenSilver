// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using OpenSilver.Internal;
using OpenSilver.Internal.Xaml;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Resources;

namespace System.Windows.Controls.Theming
{
    /// <summary>
    /// Implicitly applies a set of styles to all descendent FrameworkElements.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class Theme : ContentControl
    {
        /// <summary>
        /// Gets the ResourceDictionary corresponding to the current theme.
        /// </summary>
        public ResourceDictionary ThemeResources { get; private set; }

        /// <summary>
        /// Stores the ResourceDictionary corresponding to the current application theme.
        /// </summary>
        private static ResourceDictionary _applicationThemeResources;

        /// <summary>
        /// Stores the Uri corresponding to the application theme resources.
        /// </summary>
        private static Uri _applicationThemeUri;

        /// <summary>
        /// Initializes a new instance of the Theme class.
        /// </summary>
        public Theme()
        {
            DefaultStyleKey = typeof(Theme);
            ThemeResources = new ResourceDictionary();
        }

        /// <summary>
        /// Initializes a new instance of the Theme class.
        /// </summary>
        /// <param name="themeAssembly">
        /// Assembly with the embedded resource containing the theme to apply.
        /// </param>
        /// <param name="themeResourceName">
        /// Name of the embedded resource containing the theme to apply.
        /// </param>
        [OpenSilver.NotImplemented]
        protected Theme(Assembly themeAssembly, string themeResourceName)
            : this()
        {
            if (themeAssembly == null)
            {
                throw new ArgumentNullException("themeAssembly");
            }

#if SL_TOOLKIT
            // Get the resource stream for the theme.
            using (Stream stream = themeAssembly.GetManifestResourceStream(themeResourceName))
            {
                if (stream == null)
                {
                    throw new ResourceNotFoundException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Properties.Resources.Theme_ResourceNotFound,
                            themeResourceName),
                        new Uri(themeResourceName, UriKind.Relative));
                }

                // Load the theme
                ThemeResources = LoadThemeResources(stream, Resources);
            }
#else
            throw new NotSupportedException("This constructor is not supported. Use Theme(Uri themeUri) instead.");
#endif // SL_TOOLKIT
        }

        /// <summary>
        /// Initializes a new instance of the Theme class.
        /// </summary>
        /// <param name="themeResourceStream">
        /// A resource stream containing the theme to apply.
        /// </param>
        [OpenSilver.NotImplemented]
        protected Theme(Stream themeResourceStream)
            : this()
        {
            if (themeResourceStream == null)
            {
                throw new ArgumentNullException("themeResourceStream");
            }

#if SL_TOOLKIT
            ThemeResources = LoadThemeResources(themeResourceStream, Resources);
#else
            throw new NotSupportedException("This constructor is not supported. Use Theme(Uri themeUri) instead.");
#endif // SL_TOOLKIT
        }

        /// <summary>
        /// Initializes a new instance of the Theme class.
        /// </summary>
        /// <param name="themeUri">URI of a ResourceDictionary containing a theme.</param>
        protected Theme(Uri themeUri)
            : this()
        {
            ThemeUri = themeUri;
        }

        /// <summary>
        /// Gets or sets the URI of a ResourceDictionary containing a theme.
        /// </summary>
        public Uri ThemeUri
        {
            get { return (Uri)GetValue(ThemeUriProperty); }
            set { SetValue(ThemeUriProperty, value); }
        }

        /// <summary>
        /// Identifies the ThemeUri DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ThemeUriProperty =
            DependencyProperty.Register(
                "ThemeUri",
                typeof(Uri),
                typeof(Theme),
                new PropertyMetadata(OnThemeUriPropertyChanged));

        /// <summary>
        /// Handles changes to the ThemeUri DependencyProperty.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnThemeUriPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Theme)o).OnThemeUriPropertyChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        /// <summary>
        /// Handles changes to the ThemeUri property.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnThemeUriPropertyChanged(Uri oldValue, Uri newValue)
        {
            LoadAndApplyThemeFromUri(newValue, ThemeResources, Resources, newTheme => ThemeResources = newTheme);
        }

        /// <summary>
        /// Gets the current application-level theme Uri.
        /// </summary>
        /// <param name="app">Application instance.</param>
        /// <returns>Theme Uri.</returns>
        public static Uri GetApplicationThemeUri(Application app)
        {
            return _applicationThemeUri;
        }

        /// <summary>
        /// Sets the current application-level theme Uri.
        /// </summary>
        /// <param name="app">Application instance.</param>
        /// <param name="themeUri">Theme Uri.</param>
        public static void SetApplicationThemeUri(Application app, Uri themeUri)
        {
            _applicationThemeUri = themeUri;
            LoadAndApplyThemeFromUri(_applicationThemeUri, _applicationThemeResources, Application.Current.Resources, newTheme => _applicationThemeResources = newTheme);
        }

#if SL_TOOLKIT
        /// <summary>
        /// Loads and applies a theme from a specified Uri.
        /// </summary>
        /// <param name="themeUri">Theme Uri.</param>
        /// <param name="currentTheme">Current theme.</param>
        /// <param name="owner">ResourceDictionary owner.</param>
        /// <param name="onNewThemeAvailable">Action called when the new theme instance is available.</param>
        private static void LoadAndApplyThemeFromUri(Uri themeUri, ResourceDictionary currentTheme, ResourceDictionary owner, Action<ResourceDictionary> onNewThemeAvailable)
        {
            if (null != themeUri)
            {
                try
                {
                    // Try to load the URI as a resource stream
                    StreamResourceInfo streamResourceInfo = Application.GetResourceStream(themeUri);
                    if (null != streamResourceInfo)
                    {
                        onNewThemeAvailable(LoadAndApplyThemeFromStream(streamResourceInfo.Stream, currentTheme, owner));
                        return;
                    }
                }
                catch (ArgumentException)
                {
                    // Not a resource stream; ignore
                }

                // Try to load the URI by downloading it
                WebClient webClient = new WebClient();
                webClient.OpenReadCompleted += delegate (object sender, OpenReadCompletedEventArgs e)
                {
                    if (null == e.Error)
                    {
                        onNewThemeAvailable(LoadAndApplyThemeFromStream(e.Result, currentTheme, owner));
                    }
                };
                webClient.OpenReadAsync(themeUri);
            }
            else if (null != currentTheme)
            {
                // themeUri is null; unload current theme (if any)
                UnloadThemeResources(currentTheme, owner);
                onNewThemeAvailable(null);
            }
        }

        /// <summary>
        /// Loads and applies a theme from a Stream.
        /// </summary>
        /// <param name="stream">Stream containing the theme ResourceDictionary.</param>
        /// <param name="currentTheme">Current theme.</param>
        /// <param name="owner">ResourceDictionary owner.</param>
        /// <returns>New current theme.</returns>
        private static ResourceDictionary LoadAndApplyThemeFromStream(Stream stream, ResourceDictionary currentTheme, ResourceDictionary owner)
        {
            // Remember old theme
            ResourceDictionary previousTheme = currentTheme;
            try
            {
                // (Try to) load new theme
                currentTheme = null;
                currentTheme = LoadThemeResources(stream, owner);
            }
            finally
            {
                // Unload old theme *after* setting new theme to minimize UI flicker
                UnloadThemeResources(previousTheme, owner);
            }
            return currentTheme;
        }

        /// <summary>
        /// Load a theme from a Stream.
        /// </summary>
        /// <param name="stream">A Stream containing the theme to load.</param>
        /// <param name="owner">ResourceDictionary owner.</param>
        /// <returns>ResourceDictionary corresponding to the loaded theme.</returns>
        private static ResourceDictionary LoadThemeResources(Stream stream, ResourceDictionary owner)
        {
            // Load the theme
            ResourceDictionary resources = null;
            using (stream)
            {
                resources = ResourceParser.Parse(stream, true);
                owner.MergedDictionaries.Add(resources);
            }
            return resources;
        }
#else
        /// <summary>
        /// Loads and applies a theme from a specified Uri.
        /// </summary>
        /// <param name="themeUri">Theme Uri.</param>
        /// <param name="currentTheme">Current theme.</param>
        /// <param name="owner">ResourceDictionary owner.</param>
        /// <param name="onNewThemeAvailable">Action called when the new theme instance is available.</param>
        private static void LoadAndApplyThemeFromUri(Uri themeUri, ResourceDictionary currentTheme, ResourceDictionary owner, Action<ResourceDictionary> onNewThemeAvailable)
        {
            if (null != themeUri)
            {
                try
                {
                    string themeUriStr = themeUri.ToString(); 
                    if (IsComponentUri(themeUriStr))
                    {
                        IXamlComponentFactory factory = GetXamlComponentFactory(themeUriStr);
                        if (factory != null)
                        {
                            onNewThemeAvailable(LoadAndApplyThemeFromStream(factory, currentTheme, owner));
                            return;
                        }
                    }
                }
                catch (ArgumentException)
                {
                    // Not a resource stream; ignore
                }
            }
            else if (null != currentTheme)
            {
                // themeUri is null; unload current theme (if any)
                UnloadThemeResources(currentTheme, owner);
                onNewThemeAvailable(null);
            }
        }

        /// <summary>
        /// Loads and applies a theme from a Stream.
        /// </summary>
        /// <param name="factory">Factory containing the theme ResourceDictionary.</param>
        /// <param name="currentTheme">Current theme.</param>
        /// <param name="owner">ResourceDictionary owner.</param>
        /// <returns>New current theme.</returns>
        private static ResourceDictionary LoadAndApplyThemeFromStream(IXamlComponentFactory factory, ResourceDictionary currentTheme, ResourceDictionary owner)
        {
            // Remember old theme
            ResourceDictionary previousTheme = currentTheme;
            try
            {
                // (Try to) load new theme
                currentTheme = null;
                currentTheme = LoadThemeResources(factory, owner);
            }
            finally
            {
                // Unload old theme *after* setting new theme to minimize UI flicker
                UnloadThemeResources(previousTheme, owner);
            }
            return currentTheme;
        }

        /// <summary>
        /// Load a theme from a Stream.
        /// </summary>
        /// <param name="factory">A factory containing the theme to load.</param>
        /// <param name="owner">ResourceDictionary owner.</param>
        /// <returns>ResourceDictionary corresponding to the loaded theme.</returns>
        private static ResourceDictionary LoadThemeResources(IXamlComponentFactory factory, ResourceDictionary owner)
        {
            // Load the theme
            ResourceDictionary resources = (ResourceDictionary)factory.CreateComponent();
            owner.MergedDictionaries.Add(resources);
            return resources;
        }

        private static bool IsComponentUri(string uri)
        {
            int index = uri.IndexOf(';');
            if (index > -1)
            {
                return uri.Substring(index).StartsWith(";component/");
            }

            return false;
        }

        private static string ExtractAssemblyNameFromComponentUri(string uri)
        {
            int offset = uri[0] == '/' ? 1 : 0;
            return uri.Substring(offset, uri.IndexOf(';') - offset);
        }

        private static IXamlComponentFactory GetXamlComponentFactory(string componentUri)
        {
            string className = XamlResourcesHelper.GenerateClassNameFromComponentUri(componentUri);
            string assemblyName = ExtractAssemblyNameFromComponentUri(componentUri);

            Type factoryType = Type.GetType($"{className}, {assemblyName}");
            if (factoryType != null)
            {
                return Activator.CreateInstance(factoryType) as IXamlComponentFactory;
            }

            return null;
        }
#endif // SL_TOOLKIT

        /// <summary>
        /// Unloads the specified theme ResourceDictionary.
        /// </summary>
        /// <param name="theme">ResourceDictionary to unload.</param>
        /// <param name="owner">ResourceDictionary owner.</param>
        private static void UnloadThemeResources(ResourceDictionary theme, ResourceDictionary owner)
        {
            owner.MergedDictionaries.Remove(theme);
        }
    }
}