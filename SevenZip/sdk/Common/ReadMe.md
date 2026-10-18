---
digest:
  local-classes:
    CRC:
      mtime: "2026-10-18T05:15:10Z"
      digest: "a62c9e1f557b21fcddf7f2d6a49a8a73c1cf31c72ad87d114fe9b666003ddd0c"
    InBuffer:
      mtime: "2026-09-25T20:10:48Z"
      digest: "13a96da22f1bacae96f168bf0175bc3bc9188de44f9117e2752b3f80bc52a181"
    OutBuffer:
      mtime: "2026-10-18T05:15:10Z"
      digest: "dc587e4a8cb5c15ee0fdedbb93a554a85895a196b5872eabc963341ba99fb4a6"
  folders: {}
---
# Common

## Classes

| Class | Responsibility |
|---|---|
| [CRC](CRC.cs) | Computes CRC32 checksums using polynomial division with precomputed lookup table. |
| [InBuffer](InBuffer.cs) | Implements the input buffer work |
| [OutBuffer](OutBuffer.cs) | Buffers bytes for efficient writing to an underlying output stream. |
