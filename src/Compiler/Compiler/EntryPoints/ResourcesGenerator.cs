using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using System.Resources;

namespace DotNetForHtml5.Compiler
{
	// This task adds all the "Resource" files to the "AssemblyName.g.resources" file
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
							throw new ApplicationException($"Resource {_sourcePath} is bigger than the max allowed size ({int.MaxValue})");
						}
					}

					return _sourceStream;
				}
			}

			public override bool CanRead => true;

			public override bool CanSeek => true;

			public override bool CanWrite => false;

			public override void Flush()
			{
			}

			public override long Length => SourceStream.Length;

			public override long Position
			{
				get => SourceStream.Position;
				set => SourceStream.Position = value;
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				return SourceStream.Read(buffer, offset, count);
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				return SourceStream.Seek(offset, origin);
			}

			public override void SetLength(long value)
			{
				// This is backed by a readonly file.
				throw new NotSupportedException();
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				// This is backed by a readonly file.
				throw new NotSupportedException();
			}

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

		public ResourcesGenerator()
		{
			// SourceDir = $"{Directory.GetCurrentDirectory()}/";
		}

		[Required]
		public ITaskItem[] ResourceFiles { get; set; }

		[Output]
		[Required]
		public ITaskItem[] OutputResourcesFile { get; set; }

		// private string SourceDir { get; set; }

		public override bool Execute()
		{
			string resourcesFile = OutputResourcesFile[0].ItemSpec;

			using (ResourceWriter resWriter = new ResourceWriter(resourcesFile))
			{
				foreach (ITaskItem resourceFile in ResourceFiles)
				{
					string resFileName = resourceFile.ItemSpec;
					// string resourceId = GetResourceIdForResourceFile(resourceFile);

					// We're handing off lifetime management for the stream.
					// True for the third argument tells resWriter to dispose of the stream when it's done.
					resWriter.AddResource(resFileName, new LazyFileStream(resFileName), true);

					// Log.LogMessageFromResources(MessageImportance.Low, SRID.ReadResourceFile, resFileName);
					// Log.LogMessageFromResources(MessageImportance.Low, SRID.ResourceId, resourceId);
				}

				// Generate the .resources file.
				resWriter.Generate();
			}

			return true;
		}

		// private string GetResourceIdForResourceFile(ITaskItem resFile)
		// {
		// 	return GetResourceIdForResourceFile(
		// 		resFile.ItemSpec,
		// 		resFile.GetMetadata("Link"),
		// 		resFile.GetMetadata("LogicalName"),
		// 		SourceDir);
		// }
		//
		// internal static string GetResourceIdForResourceFile(
		// 	string filePath,
		// 	string linkAlias,
		// 	string logicalName,
		// 	string sourceDir)
		// {
		// 	string relPath = string.Empty;
		//
		// 	// Please note the subtle distinction between <Link /> and <LogicalName />. 
		// 	// <Link /> is treated as a fully resolvable path and is put through the same 
		// 	// transformations as the original file path. <LogicalName /> on the other hand 
		// 	// is treated as an alias for the given resource and is used as is. Whether <Link /> 
		// 	// was meant to be treated thus is debatable. Nevertheless in .Net 4.5 it would 
		// 	// amount to a breaking change to have to change the behavior of <Link /> and 
		// 	// hence the choice to support <LogicalName /> with the desired semantics. All 
		// 	// said in most of the regular scenarios using <Link /> or <Logical /> will result in 
		// 	// the same resourceId being picked.
		//
		// 	if (!string.IsNullOrEmpty(logicalName))
		// 	{
		// 		// Use the LogicalName when there is one
		// 		relPath = logicalName;
		// 	}
		// 	else
		// 	{
		// 		// Always use the Link tag if it's specified.
		// 		// This is the way the resource appears in the project.
		// 		linkAlias = ReplaceXAMLWithBAML(filePath, linkAlias, requestExtensionChange);
		// 		filePath = !string.IsNullOrEmpty(linkAlias) ? linkAlias : filePath;
		// 		string fullFilePath = Path.GetFullPath(filePath);
		//
		// 		//
		// 		// If the resFile, or it's perceived path, is relative to the StagingDir
		// 		// (OutputPath here) take the relative path as resource id.
		// 		// If the resFile is not relative to StagingDir, but relative
		// 		// to the project directory, take this relative path as resource id.
		// 		// Otherwise, just take the file name as resource id.
		// 		//
		//
		// 		relPath = TaskHelper.GetRootRelativePath(outputPath, fullFilePath);
		//
		// 		if (string.IsNullOrEmpty(relPath))
		// 		{
		// 			relPath = TaskHelper.GetRootRelativePath(sourceDir, fullFilePath);
		// 		}
		//
		// 		if (string.IsNullOrEmpty(relPath))
		// 		{
		// 			relPath = Path.GetFileName(fullFilePath);
		// 		}
		// 	}
		//
		// 	// Modify resource ID to correspond to canonicalized Uri format
		// 	// i.e. - all lower case, use "/" as separator
		// 	// ' ' is converted to escaped version %20
		// 	//
		//
		// 	// string resourceId = ResourceIDHelper.GetResourceIDFromRelativePath(relPath);
		//
		// 	string resourceId = relPath;
		//
		// 	return resourceId;
		// }
	}
}