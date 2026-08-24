using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk
{

    /// <summary>Computes CRC32 checksums using polynomial division with precomputed <br/>
    /// lookup table.</summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 23 | <see cref="Table"/> | Gets the table. |
    /// | 54 | <see cref="Init"/> | Resets the CRC value to its initial state. |
    /// | 57 | <see cref="UpdateByte"/> | Updates the CRC value with a single byte. |
    /// | 60 | <see cref="Update"/> | Updates the CRC value with a range of bytes from the given array. |
    /// | 67 | <see cref="GetDigest"/> | Returns the finalized CRC value. |
    /// </remarks>
    [DocState(Pass = 2, MTime = "2026-08-24T13:55:58Z", Digest = "a62c9e1f557b21fcddf7f2d6a49a8a73c1cf31c72ad87d114fe9b666003ddd0c", Stale = false, Path = "sdk/Common/CRC.cs", Since = "2026-08-23")]
    internal class CRC
    {

        /// <summary>Gets the table.</summary>
        public static readonly uint[] Table;

        private uint _value = 0xFFFFFFFF;

        /// <summary>Populates the CRC polynomial lookup table.</summary>
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

		/// <summary>Resets the CRC value to its initial state.</summary>
		public void Init() => _value = 0xFFFFFFFF;

		/// <summary>Updates the CRC value with a single byte.</summary>
		public void UpdateByte(byte b) => _value = Table[(((byte) (_value)) ^ b)] ^ (_value >> 8);

		/// <summary>Updates the CRC value with a range of bytes from the given array.</summary>
		public void Update(byte[] data, uint offset, uint size)
        {
            for (uint i = 0; i < size; i++)
                _value = Table[(((byte) (_value)) ^ data[offset + i])] ^ (_value >> 8);
        }

		/// <summary>Returns the finalized CRC value.</summary>
		public uint GetDigest() => _value ^ 0xFFFFFFFF;
	}
}