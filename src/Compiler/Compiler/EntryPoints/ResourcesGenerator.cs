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
							throw new ApplicationException($"RG1001: Input resource file '{_sourcePath}' exceeds maximum size of {int.MaxValue} bytes.");
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

		[Required]
		public ITaskItem[] ResourceFiles { get; set; }

		[Output]
		[Required]
		public ITaskItem OutputResourcesFile { get; set; }

		public override bool Execute()
		{
			try
			{
				using (ResourceWriter resWriter = new ResourceWriter(OutputResourcesFile.ItemSpec))
				{
					foreach (ITaskItem resourceFile in ResourceFiles)
					{
						string resFileName = resourceFile.ItemSpec;
					
						// We're handing off lifetime management for the stream.
						// True for the third argument tells resWriter to dispose of the stream when it's done.
						resWriter.AddResource(resFileName, new LazyFileStream(resFileName), true);
					}

					// Generate the .resources file.
					resWriter.Generate();
				}
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
	}
}