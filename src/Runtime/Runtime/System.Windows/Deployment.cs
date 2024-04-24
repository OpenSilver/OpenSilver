
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

namespace System.Windows;

/// <summary>
/// Provides application part and localization information in the application manifest
/// when deploying a Silverlight-based application.
/// </summary>
public sealed class Deployment : DependencyObject
{
    private static Deployment _current;

    /// <summary>
    /// Initializes a new instance of the <see cref="Deployment"/> class.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The <see cref="Current"/> property has already been initialized.
    /// </exception>
    public Deployment()
    {
        if (_current != null)
        {
            throw new InvalidOperationException();
        }

        SetDeployment(this);
    }

    /// <summary>
    /// Gets the current <see cref="Deployment"/> object.
    /// </summary>
    public static Deployment Current
    {
        get
        {
            if (_current == null)
            {
                _ = new Deployment();
            }

            return _current;
        }
    }

    private static readonly DependencyPropertyKey EntryPointAssemblyPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(EntryPointAssembly),
            typeof(string),
            typeof(Deployment),
            null);

    /// <summary>
    /// Identifies the <see cref="EntryPointAssembly"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EntryPointAssemblyProperty = EntryPointAssemblyPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a string name that identifies which part in the <see cref="Deployment"/>
    /// is the entry point assembly.
    /// </summary>
    /// <returns>
    /// A string that names the assembly that should be used as the entry point assembly.
    /// This is expected to be the name of one of the assemblies you specified as an
    /// <see cref="AssemblyPart"/>.
    /// </returns>
    public string EntryPointAssembly => (string)GetValue(EntryPointAssemblyProperty);

    private static readonly DependencyPropertyKey EntryPointTypePropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(EntryPointType),
            typeof(string),
            typeof(Deployment),
            null);

    /// <summary>
    /// Identifies the <see cref="EntryPointType"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EntryPointTypeProperty = EntryPointTypePropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a string that identifies the namespace and type name of the class that contains
    /// the <see cref="Application"/> entry point for your application.
    /// </summary>
    /// <returns>
    /// The namespace and type name of the class that contains the <see cref="Application"/>
    /// entry point.
    /// </returns>
    public string EntryPointType => (string)GetValue(EntryPointTypeProperty);

    private static void SetDeployment(Deployment deployment)
    {
        Application app = Application.Current;
        if (app != null)
        {
            Type appType = app.GetType();

            deployment.SetValueInternal(EntryPointAssemblyPropertyKey, appType.Assembly.GetName().Name);
            deployment.SetValueInternal(EntryPointTypePropertyKey, appType.FullName);
            deployment.SetValueInternal(PartsPropertyKey, new AssemblyPartCollection());
            deployment.SetValueInternal(ExternalPartsPropertyKey, new ExternalPartCollection());
            deployment.SetValueInternal(InBrowserSettingsPropertyKey, new InBrowserSettings());
            deployment.SetValueInternal(OutOfBrowserSettingsPropertyKey, new OutOfBrowserSettings());

            _current = deployment;
        }
    }
    
    private static readonly DependencyPropertyKey OutOfBrowserSettingsPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(OutOfBrowserSettings),
            typeof(OutOfBrowserSettings),
            typeof(Deployment),
            null);

    /// <summary>
    /// Identifies the <see cref="OutOfBrowserSettings"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty OutOfBrowserSettingsProperty = OutOfBrowserSettingsPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets an object that contains information about the application that is used for
    /// out-of-browser support.
    /// </summary>
    /// <returns>
    /// Information about the application that is used for out-of-browser support.
    /// </returns>
    [OpenSilver.NotImplemented]
    public OutOfBrowserSettings OutOfBrowserSettings => (OutOfBrowserSettings)GetValue(OutOfBrowserSettingsProperty);

    private static readonly DependencyPropertyKey InBrowserSettingsPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(InBrowserSettings),
            typeof(InBrowserSettings),
            typeof(Deployment),
            null);

    /// <summary>
    /// Identifies the <see cref="InBrowserSettings"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty InBrowserSettingsProperty = InBrowserSettingsPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets an object that contains information about the application that is used for
    /// in-browser support.
    /// </summary>
    /// <returns>
    /// An object that contains information about the application that is used for in-browser
    /// support.
    /// </returns>
    [OpenSilver.NotImplemented]
    public InBrowserSettings InBrowserSettings => (InBrowserSettings)GetValue(InBrowserSettingsProperty);

    private static readonly DependencyPropertyKey PartsPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(Parts),
            typeof(AssemblyPartCollection),
            typeof(Deployment),
            null);

    /// <summary>
    /// Identifies the <see cref="Parts"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty PartsProperty = PartsPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a collection of assembly parts that are included in the deployment.
    /// </summary>
    /// <returns>
    /// The collection of assembly parts. The default is an empty collection.
    /// </returns>
    [OpenSilver.NotImplemented]
    public AssemblyPartCollection Parts => (AssemblyPartCollection)GetValue(PartsProperty);

    private static readonly DependencyPropertyKey ExternalPartsPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(ExternalParts),
            typeof(ExternalPartCollection),
            typeof(Deployment),
            null);

    /// <summary>
    /// Identifies the <see cref="ExternalParts"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty ExternalPartsProperty = ExternalPartsPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a collection of <see cref="ExternalPart"/> instances that represent the
    /// external assemblies required by the application.
    /// </summary>
    /// <returns>
    /// The collection of external assembly parts. The default is an empty collection.
    /// </returns>
    [OpenSilver.NotImplemented]
    public ExternalPartCollection ExternalParts => (ExternalPartCollection)GetValue(ExternalPartsProperty);

    private static readonly DependencyPropertyKey ExternalCallersFromCrossDomainPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(ExternalCallersFromCrossDomain),
            typeof(CrossDomainAccess),
            typeof(Deployment),
            new PropertyMetadata(CrossDomainAccess.NoAccess));

    /// <summary>
    /// Identifies the <see cref="ExternalCallersFromCrossDomain"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty ExternalCallersFromCrossDomainProperty = ExternalCallersFromCrossDomainPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value that indicates the level of access that cross-domain callers have
    /// to the Silverlight-based application in this deployment.
    /// </summary>
    /// <returns>
    /// A value that indicates the access level of cross-domain callers.
    /// </returns>
    [OpenSilver.NotImplemented]
    public CrossDomainAccess ExternalCallersFromCrossDomain => (CrossDomainAccess)GetValue(ExternalCallersFromCrossDomainProperty);

    private static readonly DependencyPropertyKey RuntimeVersionPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(RuntimeVersion),
            typeof(string),
            typeof(Deployment),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Identifies the <see cref="RuntimeVersion"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty RuntimeVersionProperty = RuntimeVersionPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the Silverlight runtime version that this deployment supports.
    /// </summary>
    /// <returns>
    /// The Silverlight runtime version that this deployment supports.
    /// </returns>
    [OpenSilver.NotImplemented]
    public string RuntimeVersion => (string)GetValue(RuntimeVersionProperty);
}

