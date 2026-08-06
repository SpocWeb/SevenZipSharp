namespace SevenZip.Sdk
{

    /// <summary>Computes the standard CRC-32 (polynomial 0xEDB88320) checksum over a byte stream.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 9 | <see cref="Table"/> | Gets the table. |
    /// | 40 | <see cref="Init"/> | Resets the running checksum to its initial state, discarding any prior updates. |
    /// | 43 | <see cref="UpdateByte"/> | Folds a single byte b into the running checksum. |
    /// | 47 | <see cref="Update"/> | Folds size bytes of data, starting at offset,  into the running checksum. |
    /// | 55 | <see cref="GetDigest"/> | Gets the finalized CRC-32 value of all bytes processed so far. |
    /// </remarks>
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-06T06:59:29Z
    /// digest: 2ee8cc3dce49cd93565ebc02159610dd69857be80f315ab7a747ccdcd8e4bd91
    /// </code>
    /// </example>
    internal class CRC
    {

        /// <summary>Gets the table.</summary>
        public static readonly uint[] Table;

        private uint _value = 0xFFFFFFFF;

        /// <summary>Initializes a new instance of <see cref="CRC"/>.</summary>
        static CRC()
        {
            Table = new uint[256];
            const uint kPoly = 0xEDB88320;
            
            for (uint i = 0; i < 256; i++)
            {
                var r = i;
                
                for (var j = 0; j < 8; j++)
                {
                    if ((r & 1) != 0)
                    {
                        r = (r >> 1) ^ kPoly;
                    }
                    else
                    {
                        r >>= 1;
                    }
                }

                Table[i] = r;
            }
        }

		/// <summary>Resets the running checksum to its initial state, discarding any prior updates.</summary>
		public void Init() => _value = 0xFFFFFFFF;

		/// <summary>Folds a single byte <paramref name="b"/> into the running checksum.</summary>
		public void UpdateByte(byte b) => _value = Table[(((byte) (_value)) ^ b)] ^ (_value >> 8);

		/// <summary>Folds <paramref name="size"/> bytes of <paramref name="data"/>, starting at <paramref name="offset"/>,<br/>
		/// into the running checksum.</summary>
		public void Update(byte[] data, uint offset, uint size)
        {
            for (uint i = 0; i < size; i++)
                _value = Table[(((byte) (_value)) ^ data[offset + i])] ^ (_value >> 8);
        }

		/// <summary>Gets the finalized CRC-32 value of all bytes processed so far.</summary>
		/// <returns>The inverted running checksum, i.e. the final CRC-32 digest.</returns>
		public uint GetDigest() => _value ^ 0xFFFFFFFF;
	}
}