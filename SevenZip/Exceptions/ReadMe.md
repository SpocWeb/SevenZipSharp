---
digest:
  local-classes:
    CompressionFailedException:
      mtime: "2026-10-18T05:15:10Z"
      digest: "e50c215a0f05d7134caff960051f2151863bae854c972d7cf89f20b6be1e9377"
    ExtractionFailedException:
      mtime: "2026-10-18T05:15:10Z"
      digest: "665cd89ba2285e94ba5d435de16ca0713d4bfebc209b89bf298ba51cf2eea334"
    LzmaException:
      mtime: "2026-09-25T20:10:48Z"
      digest: "a3c323420e890497054cb6eaa7722a0fcfb96306c7a1dd0c1d866057fd00f94d"
    SevenZipArchiveException:
      mtime: "2026-10-18T05:15:10Z"
      digest: "31214acfc614c42177d6b59b59b9f208c56fc074e18dd7377e10f2abeac801bf"
    SevenZipCompressionFailedException:
      mtime: "2026-10-18T05:15:10Z"
      digest: "3cb5be017223e55d07dbdf815ff66aa8587c8bb6fb58bda8d954a576982d3ef7"
    SevenZipException:
      mtime: "2026-09-25T20:10:48Z"
      digest: "72b28c3fc269b6d34f1a34ee59c085c9b16ad1e31daad46a871de15fd5a60d27"
    SevenZipExtractionFailedException:
      mtime: "2026-10-18T05:15:10Z"
      digest: "06f3599df5be18c7b24a358aa7f88225fc004441464b7a0207dc78d649d36c8a"
    SevenZipInvalidFileNamesException:
      mtime: "2026-10-18T05:15:10Z"
      digest: "a744ee3c411d0a9afd9ee960584d1c7b29c48dc42c9e3b4122dd2277144f1336"
    SevenZipLibraryException:
      mtime: "2026-10-18T05:15:10Z"
      digest: "1c375fc1ba824bec3f9824718cc016bcca1c798f7276ed2f1e2c534e4ef00981"
    SevenZipSfxValidationException:
      mtime: "2026-09-25T20:10:48Z"
      digest: "f5b7081dd8ff92fdc4e070f72c021ddf797a582ca574e19ecabbe8731154074a"
  folders: {}
---
# Exceptions

## Classes

| Class | Responsibility |
|---|---|
| [CompressionFailedException](CompressionFailedException.cs) | Exception class for ArchiveUpdateCallback. |
| [ExtractionFailedException](ExtractionFailedException.cs) | Exception class for ArchiveExtractCallback. |
| [LzmaException](LzmaException.cs) | Exception class for LZMA operations. |
| [SevenZipArchiveException](SevenZipArchiveException.cs) | Exception class for 7-zip archive open or read operations. |
| [SevenZipCompressionFailedException](SevenZipCompressionFailedException.cs) | Exception class for fail to create an archive in SevenZipCompressor. |
| [SevenZipException](SevenZipException.cs) | Base SevenZip exception class. |
| [SevenZipExtractionFailedException](SevenZipExtractionFailedException.cs) | Exception class for fail to extract an archive in SevenZipExtractor. |
| [SevenZipInvalidFileNamesException](SevenZipInvalidFileNamesException.cs) | Exception class for empty common root if file name array in SevenZipCompressor. |
| [SevenZipLibraryException](SevenZipLibraryException.cs) | Exception class for 7-zip library operations. |
| [SevenZipSfxValidationException](SevenZipSfxValidationException.cs) | Exception class for 7-zip sfx settings validation. |
