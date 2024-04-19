
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
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using OpenSilver.Compiler.Common;
using ILogger = OpenSilver.Compiler.Common.ILogger;
using System.Linq;
using System.Diagnostics;

namespace OpenSilver.Compiler
{
    public class XamlPreprocessor : Task
    {
        public const string CompiledXamlFilePathMetadata = "CompiledXamlFilePath";
        private const string XamlHash = "XamlHash";
        private const string XamlHashStart = "<" + XamlHash + ">";
        private const string XamlHashEnd = "</" + XamlHash + ">";

        [ThreadStatic]
        private static MD5 _hash;

        private static MD5 Hash => _hash ??= MD5.Create();

        private readonly ILogger _logger;
        private readonly Stopwatch _watch;
        private AssembliesInspector _assembliesInspector;
        private SupportedLanguage _language;

        public XamlPreprocessor()
        {
            _logger = new LoggerThatUsesTaskOutput(this);
            _watch = new Stopwatch();
        }

        [Required]
        public string Language { get; set; }

        [Required]
        public ITaskItem[] SourceFiles { get; set; }

        [Required]
        public ITaskItem[] ResolvedReferences { get; set; }

        [Required]
        public string AssemblyName { get; set; }

        [Required]
        public string IntermediateOutputPath { get; set; }

        [Required]
        public string OutputPath { get; set; }

        [Required]
        public string RootNamespace { get; set; }

        [Required]
        public bool IsSecondPass { get; set; }

        [Required]
        public bool VerifyHash { get; set; }

        [Required]
        public string OutputRootPath { get; set; }

        [Required]
        public string OutputAppFilesPath { get; set; }

        [Required]
        public string OutputLibrariesPath { get; set; }

        [Required]
        public string OutputResourcesPath { get; set; }

        [Output]
        public ITaskItem[] GeneratedFiles { get; set; }

        [Output]
        public ITaskItem[] RemovedFiles { get; set; }

        public override bool Execute()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            _language = LanguageHelpers.GetLanguage(Language);
            if (_language == SupportedLanguage.Unknown)
            {
                _logger.WriteError($"'{Language}' is not a supported language (C#, Visual Basic and F#).");
                return false;
            }

            _watch.Start();

            string operationName = $"OpenSilver: XamlPreprocessor (pass {(IsSecondPass ? "2" : "1")})";
            _logger.WriteMessage($"{operationName} starting...");

            var generatedFiles = new List<ITaskItem>();
            var removedFiles = new List<ITaskItem>();

            if (SourceFiles.Length > 0)
            {
                _assembliesInspector = LoadAssemblies();

                foreach (ITaskItem item in SourceFiles)
                {
                    if (!IsXamlFile(item))
                    {
                        continue;
                    }

                    string sourceFile = item.GetMetadata("FullPath");

                    try
                    {
                        ITaskItem generatedFile = GenerateOutputFile(item);
                        generatedFiles.Add(generatedFile);

                        ITaskItem removedFile = new TaskItem(item);
                        removedFile.SetMetadata(CompiledXamlFilePathMetadata, generatedFile.ItemSpec);
                        removedFiles.Add(removedFile);
                    }
                    catch (Exception ex)
                    {
                        string message = $"{string.Join(Environment.NewLine, GetInnerExceptions(ex).Select(e => e.Message))}";

                        if (ex is XamlParseException xamlException)
                        {
                            int lineNumber = xamlException.LineNumber;
                            int columnNumber = xamlException.LinePosition;
                            _logger.WriteError(message, sourceFile, lineNumber, columnNumber);
                        }
                        else
                        {
                            _logger.WriteError(message, sourceFile);
                        }
                    }
                }

                _assembliesInspector.Dispose();
            }

            GeneratedFiles = generatedFiles.ToArray();
            RemovedFiles = removedFiles.ToArray();

            if (_logger.HasErrors)
            {
                _logger.WriteError($"{operationName} failed after {_watch.ElapsedMilliseconds} ms with {_logger.ErrorsCount} errors.");
                _logger.WriteError("Note: the XAML editor sometimes raises errors that are misleading. To see only real non-misleading errors, make sure to close all the XAML editor windows/tabs before compiling.");
            }
            else
            {
                _logger.WriteMessage($"{operationName} finished after {_watch.ElapsedMilliseconds} ms.");
            }

            return !_logger.HasErrors;
        }

        private AssembliesInspector LoadAssemblies()
        {
            var inspector = new AssembliesInspector(_language);

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
                    _logger.WriteWarning($"Failed to load '{assemblyPath}': {ex.Message}");
                }
            }
        }

        private string GenerateCode(string xaml, string sourceFile, string fileIdentity)
        {
            string generatedCode = string.Empty;
            switch (_language)
            {
                case SupportedLanguage.CSharp:
                    generatedCode = ConvertingXamlToCSharp.Convert(
                        xaml,
                        sourceFile,
                        fileIdentity,
                        AssemblyName,
                        _assembliesInspector,
                        !IsSecondPass,
                        OutputRootPath,
                        OutputAppFilesPath,
                        OutputLibrariesPath,
                        OutputResourcesPath,
                        _logger);

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
                        AssemblyName,
                        RootNamespace,
                        _assembliesInspector,
                        !IsSecondPass,
                        OutputRootPath,
                        OutputAppFilesPath,
                        OutputLibrariesPath,
                        OutputResourcesPath,
                        _logger);

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
                        AssemblyName,
                        RootNamespace,
                        _assembliesInspector,
                        !IsSecondPass,
                        OutputRootPath,
                        OutputAppFilesPath,
                        OutputLibrariesPath,
                        OutputResourcesPath,
                        _logger);

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

            if (!VerifyHash || IsFileOutdated(xaml, outputFilePath))
            {
                TimeSpan start = _watch.Elapsed;

                string generatedCode = GenerateCode(xaml, sourceFilePath, fileIdentity);

                Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));
                using (var sw = new StreamWriter(outputFilePath))
                {
                    sw.Write(generatedCode);
                }

                _logger.WriteMessage($"  {fileIdentity} -> {outputFilePath} ({(_watch.Elapsed - start).TotalMilliseconds} ms).");
            }
            else
            {
                _logger.WriteMessage($"  '{outputFilePath}' is up to date.");
            }

            return new TaskItem(outputFilePath);
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
            _language switch
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
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }

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
