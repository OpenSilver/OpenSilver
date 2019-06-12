using CSHTML5;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Resources
{
#if BRIDGE

    //#if WORKINPROGRESS
    /// <summary>
    /// Provides convenient access to culture-specific resources at run time.
    /// </summary>
    public class ResourceManager
    {
        /// <summary>
        /// Indicates the root name of the resource files that the System.Resources.ResourceManager
        /// searches for resources.
        /// </summary>
        protected string BaseNameField;

        /// <summary>
        /// Indicates the main System.Reflection.Assembly that contains the resources.
        /// </summary>
        protected Assembly MainAssembly;

        /// <summary>
        /// Initializes a new instance of the System.Resources.ResourceManager class that
        /// looks up resources contained in files derived from the specified root name using
        /// the given System.Reflection.Assembly.
        /// </summary>
        /// <param name="baseName">
        /// The root name of the resource file without its extension but including a fully
        /// qualified namespace name. For example, the root name for the resource file named
        /// "MyApplication.MyResource.en-US.resources" is "MyApplication.MyResource".
        /// </param>
        /// <param name="assembly">The main assembly for the resources.</param>
        /// <exception cref="System.ArgumentNullException">The baseName or assembly parameter is null.</exception>
        public ResourceManager(string baseName, Assembly assembly)
        {
            if (baseName == null)
                throw new ArgumentNullException("baseName");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            BaseNameField = baseName;
            MainAssembly = assembly;
        }

        /// <summary>
        /// Initializes a new instance of the System.Resources.ResourceManager class with
        /// default values.
        /// </summary>
        protected ResourceManager()
        {
        }

        //todo:
        //   T:System.InvalidOperationException:
        //     The value of the specified resource is not a System.String.
        //
        //   T:System.Resources.MissingManifestResourceException:
        //     No usable set of resources has been found, and there are no neutral culture resources.

        /// <summary>
        /// Gets the value of the System.String resource localized for the specified culture.
        /// </summary>
        /// <param name="name">The name of the resource to get.</param>
        /// <param name="culture">
        /// The culture for which the resource is localized. Note that if the resource is
        /// not localized for this culture, the lookup will fall back using the culture's
        /// System.Globalization.CultureInfo.Parent property, stopping after looking in the
        /// neutral culture.If this value is null, the System.Globalization.CultureInfo is
        /// obtained using the culture's System.Globalization.CultureInfo.CurrentUICulture
        /// property.
        /// </param>
        /// <returns>
        /// The value of the resource localized for the specified culture. If a best match
        /// is not possible, null is returned.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The name parameter is null</exception>
        public virtual string GetString(string name, CultureInfo culture)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (culture == null)
                culture = System.Globalization.CultureInfo.CurrentCulture;

            if (Interop.IsRunningInTheSimulator)
            {
                return name;
            }
            else
            {
                string pathInJSThroughBaseNameField = "";
                object currentElementInJSPath = Interop.ExecuteJavaScript("document.ResXFiles");
                string[] splittedPath = BaseNameField.Split('.');
                string result = null;
                try //todo-perfs: for better performance, replace the try/catch below with a check of "undefined" in the code below.
                {
                    foreach (string pathPart in splittedPath)
                    {
                        currentElementInJSPath = Interop.ExecuteJavaScript("{0}[{1}]", currentElementInJSPath, pathPart);
                    }
                    result = Convert.ToString(Interop.ExecuteJavaScript("{0}[{1}]", currentElementInJSPath, name));
                }
                catch (Exception)
                {
                    // The resource was not found. We return null.
                }
                return result;
            }
        }

        //todo:
        //   T:System.InvalidOperationException:
        //     The value of the specified resource is not a string.
        //
        //   T:System.Resources.MissingManifestResourceException:
        //     No usable set of resources has been found, and there are no neutral culture resources.

        /// <summary>
        /// Returns the value of the specified System.String resource.
        /// </summary>
        /// <param name="name">The name of the resource to get.</param>
        /// <returns>
        /// The value of the resource localized for the caller's current culture settings.
        /// If a match is not possible, null is returned.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The name parameter is null</exception>
        public virtual string GetString(string name)
        {
            return GetString(name, null);
        }

        /// <summary>
        /// Gets the root name of the resource files that the System.Resources.ResourceManager
        /// searches for resources.
        /// </summary>
        public virtual string BaseName
        {
            get
            {
                return BaseNameField;
            }
        }

        /// <summary>
        /// Gets or sets a Boolean value indicating whether the current instance of ResourceManager
        /// allows case-insensitive resource lookups in the System.Resources.ResourceManager.GetString(System.String)
        /// and System.Resources.ResourceManager.GetObject(System.String) methods.
        /// </summary>
        public virtual bool IgnoreCase { get; set; }

        #region not supported stuff
        //// Exceptions:
        ////   T:System.ArgumentNullException:
        ////     The resourceSource parameter is null.
        ////
        ////   T:System.ArgumentException:
        ////     assembly is not a run-time object.
        ///// <summary>
        ///// Creates a System.Resources.ResourceManager that looks up resources in satellite
        ///// assemblies based on information from the specified System.Type.
        ///// </summary>
        ///// <param name="resourceSource">
        ///// A type from which the System.Resources.ResourceManager derives all information
        ///// for finding .resources files.
        ///// </param>
        //public ResourceManager(Type resourceSource);
        ////
        //// Summary:
        ////     Initializes a new instance of the System.Resources.ResourceManager class that
        ////     looks up resources contained in files derived from the specified root name using
        ////     the given System.Reflection.Assembly.
        ////
        //// Parameters:
        ////   baseName:
        ////     The root name of the resource file without its extension and along with any fully
        ////     qualified namespace name. For example, the root name for the resource file named
        ////     "MyApplication.MyResource.en-US.resources" is "MyApplication.MyResource".
        ////
        ////   assembly:
        ////     The main assembly for the resources.
        ////
        ////   usingResourceSet:
        ////     The type of the custom System.Resources.ResourceSet to use. If null, the default
        ////     runtime System.Resources.ResourceSet object is used.
        ////
        //// Exceptions:
        ////   T:System.ArgumentException:
        ////     usingResourceset is not a derived class of System.Resources.ResourceSet.
        ////
        ////   T:System.ArgumentNullException:
        ////     The baseName or assembly parameter is null.
        //public ResourceManager(string baseName, Assembly assembly, Type usingResourceSet);
        ////
        //// Summary:
        ////     A constant readonly value indicating the version of resource file headers that
        ////     the current implementation of System.Resources.ResourceManager can interpret
        ////     and produce.
        //public static readonly int HeaderVersionNumber;
        ////
        //// Summary:
        ////     Holds the number used to identify resource files.
        //public static readonly int MagicNumber;
        ////
        //// Summary:
        ////     Gets the System.Type of the System.Resources.ResourceSet the System.Resources.ResourceManager
        ////     uses to construct a System.Resources.ResourceSet object.
        ////
        //// Returns:
        ////     The System.Type of the System.Resources.ResourceSet the System.Resources.ResourceManager
        ////     uses to construct a System.Resources.ResourceSet object.
        //public virtual Type ResourceSetType { get; }

        ////
        //// Summary:
        ////     Returns the System.Globalization.CultureInfo for the main assembly's neutral
        ////     resources by reading the value of the System.Resources.NeutralResourcesLanguageAttribute
        ////     on a specified System.Reflection.Assembly.
        ////
        //// Parameters:
        ////   a:
        ////     The assembly for which to return a System.Globalization.CultureInfo.
        ////
        //// Returns:
        ////     The culture from the System.Resources.NeutralResourcesLanguageAttribute, if found;
        ////     otherwise, System.Globalization.CultureInfo.InvariantCulture.
        //[SecuritySafeCritical]
        //protected static CultureInfo GetNeutralResourcesLanguage(Assembly a);
        ////
        //// Summary:
        ////     Returns the System.Version specified by the System.Resources.SatelliteContractVersionAttribute
        ////     attribute in the given assembly.
        ////
        //// Parameters:
        ////   a:
        ////     The assembly for which to look up the System.Resources.SatelliteContractVersionAttribute
        ////     attribute.
        ////
        //// Returns:
        ////     The satellite contract System.Version of the given assembly, or null if no version
        ////     was found.
        ////
        //// Exceptions:
        ////   T:System.ArgumentException:
        ////     The System.Version found in the assembly a is invalid.
        ////
        ////   T:System.ArgumentNullException:
        ////     a is null.
        //protected static Version GetSatelliteContractVersion(Assembly a);
        ////
        //// Summary:
        ////     Returns the value of the specified System.Object resource.
        ////
        //// Parameters:
        ////   name:
        ////     The name of the resource to get.
        ////
        //// Returns:
        ////     The value of the resource localized for the caller's current culture settings.
        ////     If a match is not possible, null is returned. The resource value can be null.
        ////
        //// Exceptions:
        ////   T:System.ArgumentNullException:
        ////     The name parameter is null.
        ////
        ////   T:System.Resources.MissingManifestResourceException:
        ////     No usable set of resources has been found, and there are no neutral culture resources.
        ////
        ////   T:System.NotSupportedException:
        ////     The resource depends on serialization.
        //public virtual object GetObject(string name);
        ////
        //// Summary:
        ////     Gets the value of the System.Object resource localized for the specified culture.
        ////
        //// Parameters:
        ////   name:
        ////     The name of the resource to get.
        ////
        ////   culture:
        ////     The culture for which the resource is localized. Note that if the resource is
        ////     not localized for this culture, the lookup will fall back using the culture's
        ////     System.Globalization.CultureInfo.Parent property, stopping after checking in
        ////     the neutral culture.If this value is null, the System.Globalization.CultureInfo
        ////     is obtained using the culture's System.Globalization.CultureInfo.CurrentUICulture
        ////     property.
        ////
        //// Returns:
        ////     The value of the resource, localized for the specified culture. If a "best match"
        ////     is not possible, null is returned.
        ////
        //// Exceptions:
        ////   T:System.ArgumentNullException:
        ////     The name parameter is null.
        ////
        ////   T:System.Resources.MissingManifestResourceException:
        ////     No usable set of resources have been found, and there are no neutral culture
        ////     resources.
        ////
        ////   T:System.NotSupportedException:
        ////     The resource depends on serialization.
        //public virtual object GetObject(string name, CultureInfo culture);
        ////
        //// Summary:
        ////     Gets the System.Resources.ResourceSet for a particular culture.
        ////
        //// Parameters:
        ////   culture:
        ////     The culture to look for.
        ////
        ////   createIfNotExists:
        ////     true to load the System.Resources.ResourceSet, if it has not been loaded yet;
        ////     otherwise, false.
        ////
        ////   tryParents:
        ////     true to try parent System.Globalization.CultureInfo objects to see if they exist,
        ////     if System.Resources.ResourceSet cannot be loaded; otherwise, false.
        ////
        //// Returns:
        ////     The specified resource set.
        ////
        //// Exceptions:
        ////   T:System.ArgumentNullException:
        ////     The culture parameter is null.
        //[SecuritySafeCritical]
        //public virtual ResourceSet GetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents);
        ////
        //// Summary:
        ////     Returns an System.IO.UnmanagedMemoryStream object from the specified resource.
        ////
        //// Parameters:
        ////   name:
        ////     The name of a resource.
        ////
        //// Returns:
        ////     An unmanaged memory stream.
        ////
        //// Exceptions:
        ////   T:System.InvalidOperationException:
        ////     The value of the specified resource is not a System.IO.MemoryStream object.
        ////
        ////   T:System.ArgumentNullException:
        ////     name is null.
        ////
        ////   T:System.Resources.MissingManifestResourceException:
        ////     No usable set of resources is found, and there are no neutral resources.
        //[ComVisible(false)]
        //public UnmanagedMemoryStream GetStream(string name);
        ////
        //// Summary:
        ////     Returns an System.IO.UnmanagedMemoryStream object from the specified resource,
        ////     using the specified culture.
        ////
        //// Parameters:
        ////   name:
        ////     The name of a resource.
        ////
        ////   culture:
        ////     The culture to use for the resource lookup. If culture is null, the culture for
        ////     the current thread is used.
        ////
        //// Returns:
        ////     An unmanaged memory stream.
        ////
        //// Exceptions:
        ////   T:System.InvalidOperationException:
        ////     The value of the specified resource is not a System.IO.MemoryStream object.
        ////
        ////   T:System.ArgumentNullException:
        ////     name is null.
        ////
        ////   T:System.Resources.MissingManifestResourceException:
        ////     No usable set of resources is found, and there are no neutral resources.
        //[ComVisible(false)]
        //public UnmanagedMemoryStream GetStream(string name, CultureInfo culture);
        ////
        //// Summary:
        ////     Tells the System.Resources.ResourceManager to call System.Resources.ResourceSet.Close
        ////     on all System.Resources.ResourceSet objects and release all resources.
        //public virtual void ReleaseAllResources();
        ////
        //// Summary:
        ////     Generates the name for the resource file for the given System.Globalization.CultureInfo.
        ////
        //// Parameters:
        ////   culture:
        ////     The culture for which a resource file name is constructed.
        ////
        //// Returns:
        ////     The name that can be used for a resource file for the given System.Globalization.CultureInfo.
        //protected virtual string GetResourceFileName(CultureInfo culture);
        ////
        //// Summary:
        ////     Provides the implementation for finding a System.Resources.ResourceSet.
        ////
        //// Parameters:
        ////   culture:
        ////     The culture to look for.
        ////
        ////   createIfNotExists:
        ////     true to load the System.Resources.ResourceSet, if it has not been loaded yet;
        ////     otherwise, false.
        ////
        ////   tryParents:
        ////     true to try parent System.Globalization.CultureInfo objects to see if they exist
        ////     if System.Resources.ResourceSet cannot be loaded; otherwise, false.
        ////
        //// Returns:
        ////     The specified resource set.
        ////
        //// Exceptions:
        ////   T:System.Resources.MissingManifestResourceException:
        ////     The main assembly does not contain a .resources file and it is required to look
        ////     up a resource.
        ////
        ////   T:System.ExecutionEngineException:
        ////     There was an internal error in the runtime.
        //[SecuritySafeCritical]
        //protected virtual ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents);
        #endregion
    }
    //#endif

#endif
}