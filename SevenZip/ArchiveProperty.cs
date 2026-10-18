#if UNMANAGED

namespace SevenZip
{
    /// <summary>
    /// Archive property struct.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 31 | <see cref="Name"/> | Gets the name of the archive property. |
    /// | 36 | <see cref="Value"/> | Gets the value of the archive property. |
    /// | 70 | <see cref="operator =="/> | Determines whether the specified ArchiveProperty instances are considered equal. |
    /// | 81 | <see cref="operator !="/> | Determines whether the specified ArchiveProperty instances are not considered equal. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 7736db164d25747731568052f969520a8654a0b94c1cba61d17c6e7053e01aa2
    /// </code>
    /// </example>
    public struct ArchiveProperty
    {
        /// <summary>
        /// Gets the name of the archive property.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the value of the archive property.
        /// </summary>
        public object Value { get; internal set; }

		/// <summary>
		/// Determines whether the specified System.Object is equal to the current ArchiveProperty.
		/// </summary>
		/// <param name="obj">The System.Object to compare with the current ArchiveProperty.</param>
		/// <returns>true if the specified System.Object is equal to the current ArchiveProperty; otherwise, false.</returns>
		public override bool Equals(object obj) => (obj is ArchiveProperty property) && Equals(property);

		/// <summary>
		/// Determines whether the specified ArchiveProperty is equal to the current ArchiveProperty.
		/// </summary>
		/// <param name="afi">The ArchiveProperty to compare with the current ArchiveProperty.</param>
		/// <returns>true if the specified ArchiveProperty is equal to the current ArchiveProperty; otherwise, false.</returns>
		public bool Equals(ArchiveProperty afi) => afi.Name == Name && afi.Value == Value;

		/// <summary>
		///  Serves as a hash function for a particular type.
		/// </summary>
		/// <returns> A hash code for the current ArchiveProperty.</returns>
		public override int GetHashCode() => Name.GetHashCode() ^ Value.GetHashCode();

		/// <summary>
		/// Returns a System.String that represents the current ArchiveProperty.
		/// </summary>
		/// <returns>A System.String that represents the current ArchiveProperty.</returns>
		public override string ToString() => Name + " = " + Value;

		/// <summary>
		/// Determines whether the specified ArchiveProperty instances are considered equal.
		/// </summary>
		/// <param name="afi1">The first ArchiveProperty to compare.</param>
		/// <param name="afi2">The second ArchiveProperty to compare.</param>
		/// <returns>true if the specified ArchiveProperty instances are considered equal; otherwise, false.</returns>
		public static bool operator ==(ArchiveProperty afi1, ArchiveProperty afi2)
        {
            return afi1.Equals(afi2);
        }

        /// <summary>
        /// Determines whether the specified ArchiveProperty instances are not considered equal.
        /// </summary>
        /// <param name="afi1">The first ArchiveProperty to compare.</param>
        /// <param name="afi2">The second ArchiveProperty to compare.</param>
        /// <returns>true if the specified ArchiveProperty instances are not considered equal; otherwise, false.</returns>
        public static bool operator !=(ArchiveProperty afi1, ArchiveProperty afi2)
        {
            return !afi1.Equals(afi2);
        }
    }
}

#endif
