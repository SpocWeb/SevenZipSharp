---
digest:
  local-classes:
    CRC:
      mtime: "2026-08-06T06:59:49Z"
      digest: "2ee8cc3dce49cd93565ebc02159610dd69857be80f315ab7a747ccdcd8e4bd91"
    InBuffer:
      mtime: "2026-08-06T06:30:31Z"
      digest: "13a96da22f1bacae96f168bf0175bc3bc9188de44f9117e2752b3f80bc52a181"
    OutBuffer:
      mtime: "2026-08-06T06:59:49Z"
      digest: "306d8854442334d29b238c814c84c9a529d004e702f6b8126275271957f555a0"
  folders: {}
---
# Common

This folder provides `OutBuffer`, `CRC`, `InBuffer` and related types. `OutBuffer` buffers bytes written by the LZMA encoder and flushes them in blocks to an underlying Stream.

## Classes

| Class | Responsibility |
|---|---|
| [CRC](CRC.cs) | Computes the standard CRC-32 (polynomial 0xEDB88320) checksum over a byte stream. |
| [InBuffer](InBuffer.cs) | Implements the input buffer work |
| [OutBuffer](OutBuffer.cs) | Buffers bytes written by the LZMA encoder and flushes them in blocks to an underlying Stream. |

## Architecture

Low-level, allocation-conscious primitives shared by the LZ and LZMA coders:
`InBuffer`/`OutBuffer` batch byte-level stream I/O so the bit-level codecs in
`sdk/Compress` never touch the underlying `Stream` one byte at a time, and `CRC`
supplies the lookup table used both for archive integrity checks and as a cheap
hash inside `sdk/Compress/LZ`'s match finder.

