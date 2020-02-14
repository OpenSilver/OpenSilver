using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;


public static class ClientSideResourceRegister
{
    private static HashSet<Type> _resources = new HashSet<Type>();

    ///// <summary>
    ///// Every Resource class need to be registered in order to be hacked so it use the custom client side resource manager
    ///// </summary>
    public static void RegisterResource(Type classType)
    {
        _resources.Add(classType);
    }

    ///// <summary>
    ///// When all Resources are registered, we hack them by calling Startup
    ///// </summary>
    public static void Startup()
    {
        foreach (var res in _resources)
        {
            try
            {
                ClientSideResourceManager.HackResourceClass(res);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }
    }
}

public class ClientSideResourceManager : ResourceManager
{
    /// <summary>
    /// <see cref="ResourceReader"/> need to create a read stream from assembly manifest to get to the .resources file.
    /// </summary>
    private Assembly NeutralResourceAssembly { get; }

    /// <summary>
    /// The full resource file name to use when no any other specific <see cref="CultureInfo"/> matches.
    /// </summary>
    private string NeutralResourceName { get; }

    /// <summary>
    /// <see cref="CultureInfo"/> is hash-equal when its culture is the same even if they are of different class instance.
    /// </summary>
    private Dictionary<CultureInfo, string> cultureInfoToResourceName = new Dictionary<CultureInfo, string>();

    /// <param name="asm">A single assembly to search for the neutral resource and all other culture variants.</param>
    /// <param name="resourceBaseName">
    /// Put a name of neutral resource that will be in the .dll without .resources here.
    /// 
    /// If you use "Embedded resource" feature in Visual Studio, the name will be generated according to where
    /// your .resx file is. Each folder hierarchy added a dot leading to the file name. But you can also change that
    /// (https://blogs.msdn.microsoft.com/msbuild/2005/10/06/how-to-change-the-name-of-embedded-resources/)
    /// 
    /// This will be used to derive the name of other culture variants. This base name should contains no _ character.
    /// </param>
    public ClientSideResourceManager(Assembly asm, string resourceBaseName) : base()
    {
        this.NeutralResourceAssembly = asm;
        string neutralResourceTypeName = resourceBaseName;

        string[] names = NeutralResourceAssembly.GetManifestResourceNames();

        bool foundNeutral = false;
        foreach (var n in names)
        {
            if (n.Contains(neutralResourceTypeName))
            {
                Console.WriteLine(neutralResourceTypeName + " match " + n);
                //Try to extract the culture code part.
                //This algorithm is quite crude but did the job.
                int underscoreIndex = n.IndexOf("_");
                if (underscoreIndex == -1)
                {
                    this.NeutralResourceName = n;
                    foundNeutral = true;
                }
                else
                {
                    //Get the code path with .resx, then remove the .resx and we got the code part only.
                    string code = (n.Split('_')[1]).Split('.')[0];
                    cultureInfoToResourceName.Add(CultureInfo.CreateSpecificCulture(code), n);
                }
            }
        }
        if (!foundNeutral)
        {
            string errorMsg = "You should include a neutral culture resource of the name " + neutralResourceTypeName + ".resources\n";
            errorMsg += "Possible resources names were:\n";
            foreach (var n in names)
                errorMsg += n + "\n";
            throw new Exception(errorMsg);
        }
    }

    /// <summary>
    /// A utility class to instatiate this <see cref="ClientSideResourceManager"/> instance, 
    /// and put it in place of the default one in the generated static class from .resx file.
    /// So you can continue using the generated class, and get correct culture fallback at client side power
    /// from <see cref="ClientSideResourceManager"/> at the same time.
    /// </summary>
    /// <param name="classType">Class type of the generated class from .resx file.
    /// Be sure to generate from only the neutral one.</param>
    public static void HackResourceClass(Type classType)
    {
        var newRm = new ClientSideResourceManager(classType.Assembly, classType.FullName);
        var fieldToHack = classType.GetField("resourceMan", BindingFlags.Static | BindingFlags.NonPublic);
        fieldToHack.SetValue(null, newRm);
    }

    /// <summary>
    /// A utility class to instatiate this <see cref="ClientSideResourceManager"/> instance, 
    /// and put it in place of the default one in the generated static class from .resx file.
    /// So you can continue using the generated class, and get correct culture fallback at client side power
    /// from <see cref="ClientSideResourceManager"/> at the same time.
    /// </summary>
    /// <param name="classType">Class type of the generated class from .resx file.
    /// Be sure to generate from only the neutral one.</param>
    /// <param name="overrideClassNamespace">class namespace of the type if resource namespace is override by resource generator tool.
    /// Be sure to generate from only the neutral one.</param>
    public static void HackResourceClass(Type classType, string overrideClassNamespace)
    {
        var newRm = new ClientSideResourceManager(classType.Assembly, overrideClassNamespace);
        var fieldToHack = classType.GetField("resourceMan", BindingFlags.Static | BindingFlags.NonPublic);
        fieldToHack.SetValue(null, newRm);
    }

    /// <summary>
    /// We have cached all possible names at constructor.
    /// </summary>
    protected override string GetResourceFileName(CultureInfo cultureInfo)
    {
        if (cultureInfoToResourceName.TryGetValue(cultureInfo, out var fileName))
        {
            return fileName;
        }
        else
        {
            return this.NeutralResourceName;
        }
    }

    /// <summary>
    /// Dispose it too!
    /// </summary>
    private IResourceReader GetReaderForCulture(CultureInfo cultureInfo)
        => new ResourceReader(NeutralResourceAssembly.GetManifestResourceStream(GetResourceFileName(cultureInfo)));

    /// <summary>
    /// It will be called automatically as a part of fallback process when a term is not found for one culture.
    /// The <paramref name="culture"/> that is coming in will change automatically.
    /// 
    /// I guess the reader is also disposed by the caller that I didn't override.
    /// </summary>
    protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        => new ResourceSet(GetReaderForCulture(culture));
}