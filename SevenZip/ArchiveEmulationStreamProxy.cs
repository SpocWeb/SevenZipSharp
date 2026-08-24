using System.ComponentModel;
using org.SpocWeb.root.Attributes;
namespace SevenZip
{
    using System;
    using System.IO;

    /// <summary>
    /// A Stream proxy that presents a portion of an underlying stream starting<br/>
    /// from a specified offset, remapping position and seek operations<br/>
    /// accordingly.
    /// </summary>
    /// <remarks>
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="Stream"/> | The wrapped stream whose operations are delegated and adapted. |
    /// | <see cref="SeekOrigin"/> | Enumeration used in Seek method to determine origin point. |
    ///
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 37 | <see cref="Offset"/> | Gets the file offset. |
    /// | 43 | <see cref="Source"/> | The source wrapped stream. |
    /// | 51 | <see cref="ArchiveEmulationStreamProxy"/> | Initializes a new instance of the ArchiveEmulationStream class. |
    /// | 95 | <see cref="Dispose"/> | Disposes the underlying source stream. |
    /// </remarks>
    /// <seealso cref="Stream">Stream — underlying operations are delegated with position adjustments.</seealso>
    [DocState(Pass = 2, MTime = "2026-08-24T14:17:09Z", Digest = "12066dd21476c4e3fc1ee07d1b0906de0a1dd6a10c5ffe4210abb9f9dbbad71f", Stale = false, Path = "ArchiveEmulationStreamProxy.cs", Since = "2026-08-23")]
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

		/// <summary>Disposes the underlying source stream.</summary>
		public new void Dispose() => Source.Dispose();

		/// <inheritdoc />
		public override void Close() => Source.Close();
	}
}
