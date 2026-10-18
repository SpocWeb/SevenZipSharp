#if UNMANAGED

namespace SevenZip
{
    /// <summary>
    /// Zip encryption method enum.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 31 | <see cref="ZipCrypto"/> | ZipCrypto encryption method. |
    /// | 35 | <see cref="Aes128"/> | AES 128 bit encryption method. |
    /// | 39 | <see cref="Aes192"/> | AES 192 bit encryption method. |
    /// | 43 | <see cref="Aes256"/> | AES 256 bit encryption method. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: f30fd15401fc5bb1a92b7032d9036a60b2365a6351dc8c9399f381b11080e82d
    /// </code>
    /// </example>
    public enum ZipEncryptionMethod
    {
        /// <summary>
        /// ZipCrypto encryption method.
        /// </summary>
        ZipCrypto,
        /// <summary>
        /// AES 128 bit encryption method.
        /// </summary>
        Aes128,
        /// <summary>
        /// AES 192 bit encryption method.
        /// </summary>
        Aes192,
        /// <summary>
        /// AES 256 bit encryption method.
        /// </summary>
        Aes256
    }
}

#endif
