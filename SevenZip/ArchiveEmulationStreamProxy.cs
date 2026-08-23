using System.ComponentModel;
using org.SpocWeb.root.Attributes;
namespace SevenZip
{
    using System;
    using System.IO;

    /// <summary>
    /// The Stream extension class to emulate the archive part of a stream.
    /// </summary>
    [DocState(Pass = 2, MTime = "2026-08-23T11:34:25Z", Digest = "f1e253f9417578b25d2859f25ec4eb33b8e4d4e8e5e4fda8e98c4154b74aae15", Stale = true, Path = "ArchiveEmulationStreamProxy.cs", Since = "2026-08-23")]
    internal class ArchiveEmulationStreamProxy : Stream, IDisposable
    {
        /// <summary>
        /// Gets the file offset.
        /// </summary>
        [System.ComponentModel.Description("Gets the file offset.")]
        public int Offset { get; }

        /// <summary>
        /// The source wrapped stream.
        /// </summary>
        [System.ComponentModel.Description("The source wrapped stream.")]
        public Stream Source { get; }

        /// <summary>
        /// Initializes a new instance of the ArchiveEmulationStream class.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        /// <param name="offset">The stream offset.</param>
        [System.ComponentModel.Description("Initializes a new instance of the ArchiveEmulationStream class.")]
        public ArchiveEmulationStreamProxy(Stream stream, int offset)
        {
            Source = stream;
            Offset = offset;
            Source.Position = offset;
        }

        /// <inheritdoc />
        public override bool CanRead => Source.CanRead;

        /// <inheritdoc />
        public override bool CanSeek => Source.CanSeek;

        /// <inheritdoc />
        public override bool CanWrite => Source.CanWrite;

		/// <inheritdoc />
		public override void Flush() => Source.Flush();

		/// <inheritdoc />
		public override long Length => Source.Length - Offset;

        /// <inheritdoc />
        public override long Position
        {
            get => Source.Position - Offset;
            set => Source.Position = value;
        }

		/// <inheritdoc />
		public override int Read(byte[] buffer, int offset, int count) => Source.Read(buffer, offset, count);

		/// <inheritdoc />
		public override long Seek(long offset, SeekOrigin origin) => Source.Seek(origin == SeekOrigin.Begin ? offset + Offset : offset,
				origin) - Offset;

		/// <inheritdoc />
		public override void SetLength(long value) => Source.SetLength(value);

		/// <inheritdoc />
		public override void Write(byte[] buffer, int offset, int count) => Source.Write(buffer, offset, count);

		/// <summary>TODO: LLM</summary>
		public new void Dispose() => Source.Dispose();

		/// <inheritdoc />
		public override void Close() => Source.Close();
	}
}
