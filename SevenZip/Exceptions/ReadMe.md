---
digest:
  local-classes:
    LzmaException:
      mtime: "2026-08-06T06:59:29Z"
      digest: "a3c323420e890497054cb6eaa7722a0fcfb96306c7a1dd0c1d866057fd00f94d"
    SevenZipException:
      mtime: "2026-08-06T06:59:29Z"
      digest: "72b28c3fc269b6d34f1a34ee59c085c9b16ad1e31daad46a871de15fd5a60d27"
    SevenZipSfxValidationException:
      mtime: "2026-08-06T06:59:29Z"
      digest: "f5b7081dd8ff92fdc4e070f72c021ddf797a582ca574e19ecabbe8731154074a"
  folders: {}
---
# Exceptions

This folder provides `SevenZipException`, `LzmaException`, `SevenZipSfxValidationException` and related types. `SevenZipException` base SevenZip exception class.

## Classes

| Class | Responsibility |
|---|---|
| [LzmaException](LzmaException.cs) | Exception class for LZMA operations. |
| [SevenZipException](SevenZipException.cs) | Base SevenZip exception class. |
| [SevenZipSfxValidationException](SevenZipSfxValidationException.cs) | Exception class for 7-zip sfx settings validation. |

## Architecture

`SevenZipException` is the common base for the library's checked failure modes;
`LzmaException` and `SevenZipSfxValidationException` each specialize it for one
failure domain (malformed LZMA stream data, invalid SFX settings) so callers can
catch either the specific type or the shared base.

