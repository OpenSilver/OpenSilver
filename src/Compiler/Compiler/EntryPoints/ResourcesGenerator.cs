
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
using System.IO;
using System.Resources;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace OpenSilver.Compiler;

public class ResourcesGenerator : Task
{
    // We want to avoid holding file handles for arbitrary lengths of time, so we pass instances of this class
    // to ResourceWriter which only opens the handle when it's being used.
    // We use flags in ResourceWriter so that it will opportunistically dispose the stream.
    // This has potential for delaying failures, but that's acceptable in this scenario.
    private class LazyFileStream : Stream
    {
        private readonly string _sourcePath;
        private FileStream _sourceStream;

        public LazyFileStream(string path)
        {
            _sourcePath = Path.GetFullPath(path);
        }

        private Stream SourceStream
        {
            get
            {
                if (_sourceStream == null)
                {
                    _sourceStream = new FileStream(_sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                    // limit size to System.Int32.MaxValue
                    long length = _sourceStream.Length;
                    if (length > int.MaxValue)
                    {
                        throw new ApplicationException($"RG1001: Input resource file '{_sourcePath}' exceeds maximum size of {int.MaxValue} bytes.");
                    }
                }

                return _sourceStream;
            }
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override void Flush() { }

        public override long Length => SourceStream.Length;

        public override long Position
        {
            get => SourceStream.Position;
            set => SourceStream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
            => SourceStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin)
            => SourceStream.Seek(offset, origin);

        public override void SetLength(long value)
            => throw new NotSupportedException(); // This is backed by a readonly file.

        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotSupportedException(); // This is backed by a readonly file.

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (null != _sourceStream)
                {
                    _sourceStream.Dispose();
                    _sourceStream = null;
                }
            }
        }
    }

    private readonly string _sourceDir;
    private string _outputPath;

    public ResourcesGenerator()
    {
        _sourceDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
    }

    [Required]
    public ITaskItem[] ResourceFiles { get; set; }

    ///<summary>
    /// The directory where the generated resources file lives.
    ///</summary>
    [Required]
    public string OutputPath
    {
        get { return _outputPath; }
        set
        {
            _outputPath = Path.GetFullPath(value);

            if (!_outputPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                _outputPath += Path.DirectorySeparatorChar;
        }
    }

    [Output]
    [Required]
    public ITaskItem OutputResourcesFile { get; set; }

    public override bool Execute()
    {
        if (!ValidResourceFiles(ResourceFiles))
        {
            // ValidResourceFiles has already showed up error message.
            // Just stop here.
            return false;
        }

        try
        {
            // create output directory
            if (!Directory.Exists(OutputPath))
            {
                Directory.CreateDirectory(OutputPath);
            }

            string resourcesFile = OutputResourcesFile.ItemSpec;

            Log.LogMessage(MessageImportance.Low, $"Generating .resources file: '{resourcesFile}'...");

            using (var resWriter = new ResourceWriter(resourcesFile))
            {
                foreach (ITaskItem resourceFile in ResourceFiles)
                {
                    string resFileName = resourceFile.ItemSpec;
                    string resourceId = GetResourceIdForResourceFile(resourceFile);

                    // We're handing off lifetime management for the stream.
                    // True for the third argument tells resWriter to dispose of the stream when it's done.
                    resWriter.AddResource(resourceId, new LazyFileStream(resFileName), true);

                    Log.LogMessage(MessageImportance.Low, $"Reading Resource file: '{resFileName}'...");
                    Log.LogMessage(MessageImportance.Low, $"Resource ID is '{resourceId}'.");
                }

                // Generate the .resources file.
                resWriter.Generate();
            }

            Log.LogMessage(MessageImportance.Low, $"Generated .resources file: '{resourcesFile}'.");
        }
        catch (Exception e)
        {
            string errorId = Log.ExtractMessageCode(e.Message, out string message);

            if (string.IsNullOrEmpty(errorId))
            {
                errorId = "RG1000";
                message = $"Unknown build error, '{message}' ";
            }

            Log.LogError(null, errorId, null, null, 0, 0, 0, 0, message, null);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Check if the passed files have valid path
    /// </summary>
    /// <param name="inputFiles"></param>
    /// <returns></returns>
    private bool ValidResourceFiles(ITaskItem[] inputFiles)
    {
        bool bValid = true;

        foreach (ITaskItem inputFile in inputFiles)
        {
            string strFileName;

            strFileName = inputFile.ItemSpec;

            if (!File.Exists(TaskHelper.CreateFullFilePath(strFileName, _sourceDir)))
            {
                bValid = false;
                Log.LogError($"BG1002: File '{strFileName}' cannot be found.");
            }
        }

        return bValid;
    }

    private string GetResourceIdForResourceFile(ITaskItem resFile)
    {
        return GetResourceIdForResourceFile(
            resFile.ItemSpec,
            resFile.GetMetadata("Link"),
            resFile.GetMetadata("LogicalName"),
            OutputPath,
            _sourceDir);
    }

    private static string GetResourceIdForResourceFile(
        string filePath,
        string linkAlias,
        string logicalName,
        string outputPath,
        string sourceDir)
    {
        string relPath;

        if (!string.IsNullOrEmpty(logicalName))
        {
            relPath = logicalName;
        }
        else
        {
            filePath = !string.IsNullOrEmpty(linkAlias) ? linkAlias : filePath;
            string fullFilePath = Path.GetFullPath(filePath);

            //
            // If the resFile, or it's perceived path, is relative to the StagingDir
            // (OutputPath here) take the relative path as resource id.
            // If the resFile is not relative to StagingDir, but relative
            // to the project directory, take this relative path as resource id.
            // Otherwise, just take the file name as resource id.
            //

            relPath = TaskHelper.GetRootRelativePath(outputPath, fullFilePath);

            if (string.IsNullOrEmpty(relPath))
            {
                relPath = TaskHelper.GetRootRelativePath(sourceDir, fullFilePath);
            }

            if (string.IsNullOrEmpty(relPath))
            {
                relPath = Path.GetFileName(fullFilePath);
            }
        }

        // Modify resource ID to correspond to canonicalized Uri format
        // i.e. - all lower case, use "/" as separator
        // ' ' is converted to escaped version %20
        //

        return ResourceIDHelper.GetResourceIDFromRelativePath(relPath, UriFormat.UriEscaped);
    }
}
