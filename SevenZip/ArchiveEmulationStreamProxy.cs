namespace SevenZip
{
    using System;
    using System.IO;

    /// <summary>
    /// The Stream extension class to emulate the archive part of a stream.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 22 | <see cref="Offset"/> | Gets the file offset. |
    /// | 27 | <see cref="Source"/> | The source wrapped stream. |
    /// | 34 | <see cref="ArchiveEmulationStreamProxy"/> | Initializes a new instance of the ArchiveEmulationStream class. |
    /// | 77 | <see cref="Dispose"/> | Disposes the wrapped Source stream. |
    ///
    /// ## Collaborators
    ///
    /// | Type | Role |
    /// |---|---|
    /// | <see cref="SeekOrigin"/> | Passed as a parameter. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2022-08-28T11:01:42Z
    /// digest: b14a9fa6fab6a41dc1f0a6e8e63855885fd7a5eda9ac54f12e57337c682f00d3
    /// </code>
    /// </example>
    internal class ArchiveEmulationStreamProxy : Stream, IDisposable
    {
        /// <summary>
        /// Gets the file offset.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// The source wrapped stream.
        /// </summary>
        public Stream Source { get; }

        /// <summary>
        /// Initializes a new instance of the ArchiveEmulationStream class.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        /// <param name="offset">The stream offset.</param>
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

		/// <summary>Disposes the wrapped <see cref="Source"/> stream.</summary>
		public new void Dispose() => Source.Dispose();

		/// <inheritdoc />
		public override void Close() => Source.Close();
	}
}
