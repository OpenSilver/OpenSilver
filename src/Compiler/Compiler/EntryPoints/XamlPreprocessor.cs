
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

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using MSTask = Microsoft.Build.Utilities.Task;

namespace OpenSilver.Compiler
{
    public class XamlPreprocessor : MSTask
    {
        public const string CompiledXamlFilePathMetadata = "CompiledXamlFilePath";
        private const string XamlHash = "XamlHash";
        private const string XamlHashStart = "<" + XamlHash + ">";
        private const string XamlHashEnd = "</" + XamlHash + ">";

        [ThreadStatic]
        private static MD5 _hash;

        private static MD5 Hash => _hash ??= MD5.Create();

        private readonly Stopwatch _watch;
        private readonly Lazy<AssembliesInspector> _assembliesInspector;
        private readonly Lazy<CoreTypesConverter> _coreTypesConverter;
        private SupportedLanguage _supportedLanguage = SupportedLanguage.Unknown;
        private string _language;
        private XamlPreprocessorOptions _options;

        private AssembliesInspector AssembliesInspector => _assembliesInspector.Value;

        private CoreTypesConverter CoreTypesConverter => _coreTypesConverter.Value;

        public XamlPreprocessor()
        {
            _watch = new Stopwatch();

            _assembliesInspector = new Lazy<AssembliesInspector>(LoadAssemblies);
            _coreTypesConverter = new Lazy<CoreTypesConverter>(GetCoreTypesConverter);
        }

        [Required]
        public string Language
        {
            get => _language;
            set
            {
                _language = value;
                _supportedLanguage = LanguageHelpers.GetLanguage(_language);
            }
        }

        [Required]
        public int MaxDegreeOfParallelism { get; set; }

        [Required]
        public ITaskItem[] PageFiles { get; set; }

        [Required]
        public ITaskItem[] ApplicationDefinitionFiles { get; set; }

        [Required]
        public ITaskItem[] ContentFiles { get; set; }

        [Required]
        public ITaskItem[] ResolvedReferences { get; set; }

        [Required]
        public string AssemblyName { get; set; }

        [Required]
        public string IntermediateOutputPath { get; set; }

        [Required]
        public string OutputPath { get; set; }

        public string RootNamespace { get; set; }

        public string Options { get; set; }

        [Required]
        public bool IsSecondPass { get; set; }

        [Required]
        public bool VerifyHash { get; set; }

        [Required]
        public bool IsDesignTime { get; set; }

        [Output]
        public ITaskItem[] GeneratedFiles { get; set; }

        [Output]
        public ITaskItem[] ProcessedPageFiles { get; set; }

        [Output]
        public ITaskItem[] ProcessedApplicationDefinitionFiles { get; set; }

        [Output]
        public ITaskItem[] ProcessedContentFiles { get; set; }

        public override bool Execute()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            if (MaxDegreeOfParallelism == 0 || MaxDegreeOfParallelism < -1)
            {
                Log.LogWarning($"'{MaxDegreeOfParallelism}' is not a valid value for MaxDegreeOfParallelism. Supported values are -1 or non-zero positive integers.");
                MaxDegreeOfParallelism = 1;
            }

            if (_supportedLanguage == SupportedLanguage.Unknown)
            {
                Log.LogError($"'{Language}' is not a supported language (C#, Visual Basic and F#).");
                return false;
            }

            _options = ParseOptions(Options, XamlPreprocessorOptions.Auto);

            _watch.Start();

            string operationName = $"OpenSilver: XamlPreprocessor (pass {(IsSecondPass ? "2" : "1")})";
            Log.LogMessage($"{operationName} starting...");

            var generatedFiles = new List<ITaskItem>();

            (var generatedPageFiles, var processedPageFiles) = ProcessFiles(PageFiles);
            (var generatedApplicationDefinitionFiles, var processedApplicationDefinitionFiles) = ProcessFiles(ApplicationDefinitionFiles);
            (var generatedContentFiles, var processedContentFiles) = ProcessFiles(ContentFiles);

            if (_assembliesInspector.IsValueCreated)
            {
                _assembliesInspector.Value.Dispose();
            }

            generatedFiles.AddRange(generatedPageFiles);
            generatedFiles.AddRange(generatedApplicationDefinitionFiles);
            generatedFiles.AddRange(generatedContentFiles);

            GeneratedFiles = generatedFiles.ToArray();
            ProcessedPageFiles = processedPageFiles;
            ProcessedApplicationDefinitionFiles = processedApplicationDefinitionFiles;
            ProcessedContentFiles = processedContentFiles;

            if (Log.HasLoggedErrors)
            {
                Log.LogMessage(MessageImportance.High, $"{operationName} failed after {_watch.ElapsedMilliseconds} ms.");
                Log.LogMessage(MessageImportance.High, "Note: the XAML editor sometimes raises errors that are misleading. To see only real non-misleading errors, make sure to close all the XAML editor windows/tabs before compiling.");
            }
            else
            {
                Log.LogMessage($"{operationName} finished after {_watch.ElapsedMilliseconds} ms.");
            }

            return IsDesignTime || !Log.HasLoggedErrors;
        }

        private (ITaskItem[] GeneratedFiles, ITaskItem[] ProcessedFiles) ProcessFiles(ITaskItem[] sourceFiles)
        {
            ITaskItem[] xamlSourceFiles = sourceFiles.Where(IsXamlFile).ToArray();

            var generatedFiles = new ITaskItem[xamlSourceFiles.Length];
            var processedFiles = new ITaskItem[xamlSourceFiles.Length];

            if (xamlSourceFiles.Length > 0)
            {
                Parallel.For(0, xamlSourceFiles.Length, new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism }, i =>
                {
                    ITaskItem item = xamlSourceFiles[i];
                    string sourceFile = item.GetMetadata("FullPath");

                    try
                    {
                        ITaskItem generatedFile = GenerateOutputFile(item);
                        generatedFiles[i] = generatedFile;

                        ITaskItem processedFile = new TaskItem(item);
                        processedFile.SetMetadata(CompiledXamlFilePathMetadata, generatedFile.ItemSpec);
                        processedFiles[i] = processedFile;
                    }
                    catch (XamlParseException ex) when (ex.HasLineInfo())
                    {
                        string message = $"{string.Join(Environment.NewLine, GetInnerExceptions(ex).Select(e => e.Message))}";
                        Log.LogError(string.Empty, string.Empty, string.Empty, sourceFile, ex.LineNumber, ex.LinePosition, 0, 0, message);
                    }
                    catch (XmlException ex)
                    {
                        string message = $"{string.Join(Environment.NewLine, GetInnerExceptions(ex).Select(e => e.Message))}";
                        Log.LogError(string.Empty, string.Empty, string.Empty, sourceFile, ex.LineNumber, ex.LinePosition, 0, 0, message);
                    }
                    catch (Exception ex)
                    {
                        Log.LogErrorFromException(ex, true, true, sourceFile);
                    }
                });
            }

            return (generatedFiles, processedFiles);
        }

        private AssembliesInspector LoadAssemblies()
        {
            var inspector = new AssembliesInspector(AssemblyName, _supportedLanguage);

            foreach (ITaskItem reference in ResolvedReferences)
            {
                LoadAssembly(reference.ItemSpec);
            }

            if (IsSecondPass)
            {
                LoadAssembly(Path.Combine(OutputPath, $"{AssemblyName}.dll"));
            }

            return inspector;

            void LoadAssembly(string assemblyPath)
            {
                try
                {
                    inspector.LoadAssembly(assemblyPath);
                }
                catch (Exception ex)
                {
                    Log.LogWarning($"Failed to load '{assemblyPath}': {ex.Message}");
                }
            }
        }

        private CoreTypesConverter GetCoreTypesConverter()
        {
            return _supportedLanguage switch
            {
                SupportedLanguage.CSharp => new CoreTypesConverterCS(AssembliesInspector, AssemblyName),
                SupportedLanguage.VBNet => new CoreTypesConverterVB(AssembliesInspector, AssemblyName),
                SupportedLanguage.FSharp => new CoreTypesConverterFS(AssembliesInspector, AssemblyName),
                _ => throw new InvalidOperationException($"'{Language}' is not a supported language (C#, Visual Basic and F#)."),
            };
        }

        private string GenerateCode(string xaml, string sourceFile, string fileIdentity, XamlPreprocessorOptions options)
        {
            string generatedCode = string.Empty;
            switch (_supportedLanguage)
            {
                case SupportedLanguage.CSharp:
                    generatedCode = ConvertingXamlToCSharp.Convert(
                        xaml,
                        sourceFile,
                        fileIdentity,
                        new ConversionSettings(AssemblyName, AssembliesInspector, CoreTypesConverter, SystemTypesHelper.CSharp, TypeReferenceHelper.CSharp, options),
                        !IsSecondPass);

                    generatedCode = CreateCSHeaderContainingHash(xaml)
                        + Environment.NewLine
                        + Environment.NewLine
                        + generatedCode;
                    break;

                case SupportedLanguage.VBNet:
                    generatedCode = ConvertingXamlToVB.Convert(
                        xaml,
                        sourceFile,
                        fileIdentity,
                        RootNamespace,
                        new ConversionSettings(AssemblyName, AssembliesInspector, CoreTypesConverter, SystemTypesHelper.VisualBasic, TypeReferenceHelper.VisualBasic, options),
                        !IsSecondPass);

                    generatedCode = CreateVBHeaderContainingHash(xaml)
                        + Environment.NewLine
                        + Environment.NewLine
                        + generatedCode;
                    break;

                case SupportedLanguage.FSharp:
                    generatedCode = ConvertingXamlToFS.Convert(
                        xaml,
                        sourceFile,
                        fileIdentity,
                        RootNamespace,
                        new ConversionSettings(AssemblyName, AssembliesInspector, CoreTypesConverter, SystemTypesHelper.FSharp, TypeReferenceHelper.FSharp, options),
                        !IsSecondPass);

                    generatedCode = CreateFSHeaderContainingHash(xaml)
                        + Environment.NewLine
                        + Environment.NewLine
                        + generatedCode;
                    break;
            };
            return generatedCode;
        }

        private ITaskItem GenerateOutputFile(ITaskItem item)
        {
            string outputFilePath = GetOutputFile(item);
            string sourceFilePath = item.GetMetadata("FullPath");
            string fileIdentity = GetFileIdentity(item);
            string xaml = ReadFileContent(sourceFilePath);
            XamlPreprocessorOptions options = GetXamlProcessorOptions(item);

            if (!VerifyHash || IsFileOutdated(xaml, outputFilePath))
            {
                TimeSpan start = _watch.Elapsed;

                string generatedCode = GenerateCode(xaml, sourceFilePath, fileIdentity, options);

                Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));
                using (var sw = new StreamWriter(outputFilePath))
                {
                    sw.Write(generatedCode);
                }

                Log.LogMessage($"  {fileIdentity} -> {outputFilePath} ({(_watch.Elapsed - start).TotalMilliseconds} ms).");
            }
            else
            {
                Log.LogMessage($"  '{outputFilePath}' is up to date.");
            }

            return new TaskItem(outputFilePath);
        }

        private XamlPreprocessorOptions ParseOptions(string value, XamlPreprocessorOptions fallback)
        {
            if (!XamlPreprocessorOptionsHelpers.TryParse(value, out XamlPreprocessorOptions options))
            {
                Log.LogWarning($"'{value}' is not a supported xaml preprocessor option (Auto, Optimize or a XPath that evaluates to a node-set).");
                return fallback;
            }
            return options;
        }

        private static bool IsFileOutdated(string xaml, string outputFile)
        {
            // Check if the output file exists:
            if (!File.Exists(outputFile))
            {
                return true;
            }

            // Read the header of the output file (the first line of the file), which contains the hash of the previous XAML that it was compiled from:
            using (var reader = new StreamReader(outputFile))
            {
                string fileHeader = reader.ReadLine();

                int x1 = fileHeader.IndexOf(XamlHashStart);
                int x2 = fileHeader.IndexOf(XamlHashEnd);

                if (x1 > 0 && x2 > 0 && x1 < x2)
                {
                    string previousXamlHash = fileHeader.Substring(x1 + XamlHashStart.Length, (x2 - (x1 + XamlHashStart.Length)));
                    string xamlHash = ComputeHash(xaml);

                    return previousXamlHash != xamlHash;
                }
            }

            return true;
        }

        private string GetExtension() =>
            _supportedLanguage switch
            {
                SupportedLanguage.CSharp => "cs",
                SupportedLanguage.VBNet => "vb",
                SupportedLanguage.FSharp => "fs",
                _ => throw new InvalidOperationException(),
            };

        private static bool IsXamlFile(ITaskItem item) =>
            string.Equals(item.GetMetadata("Extension"), ".xaml", StringComparison.OrdinalIgnoreCase);

        private string GetOutputFile(ITaskItem item) => Path.Combine(IntermediateOutputPath, GetFileName(item));

        private string GetFileName(ITaskItem item)
        {
            string fileIdentity = GetFileIdentity(item);

            if (IsSecondPass)
            {
                return $"{fileIdentity}.g.{GetExtension()}";
            }
            else
            {
                return $"{fileIdentity}.g.i.{GetExtension()}";
            }
        }

        private XamlPreprocessorOptions GetXamlProcessorOptions(ITaskItem item)
        {
            string options = item.GetMetadata("OpenSilverXamlPreprocessorOptions");
            if (string.IsNullOrEmpty(options))
            {
                return _options;
            }
            return ParseOptions(options, _options);
        }

        private static string ReadFileContent(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }

        private static string GetFileIdentity(ITaskItem item)
        {
            string identity = item.GetMetadata("Link");
            if (string.IsNullOrEmpty(identity))
            {
                identity = item.GetMetadata("Identity");
            }
            return identity;
        }

        private static string CreateCSHeaderContainingHash(string xaml)
        {
            string fileHash = ComputeHash(xaml);
            return $"// <OpenSilver><{XamlHash}>{fileHash}</{XamlHash}><CompilationDate>{DateTime.Now.ToString(CultureInfo.InvariantCulture)}</CompilationDate></OpenSilver>";
        }

        private static string CreateVBHeaderContainingHash(string xaml)
        {
            string fileHash = ComputeHash(xaml);
            return $"' <OpenSilver><{XamlHash}>{fileHash}</{XamlHash}><CompilationDate>{DateTime.Now.ToString(CultureInfo.InvariantCulture)}</CompilationDate></OpenSilver>";
        }

        private static string CreateFSHeaderContainingHash(string xaml)
        {
            string fileHash = ComputeHash(xaml);
            return $"// <OpenSilver><{XamlHash}>{fileHash}</{XamlHash}><CompilationDate>{DateTime.Now.ToString(CultureInfo.InvariantCulture)}</CompilationDate></OpenSilver>";
        }

        private static string ComputeHash(string str)
        {
            var hash = Hash.ComputeHash(Encoding.UTF8.GetBytes(str));

            var sb = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        private static IEnumerable<Exception> GetInnerExceptions(Exception ex)
        {
            Debug.Assert(ex is not null);

            var innerException = ex;
            do
            {
                yield return innerException;
                innerException = innerException.InnerException;
            }
            while (innerException != null);
        }
    }
}
