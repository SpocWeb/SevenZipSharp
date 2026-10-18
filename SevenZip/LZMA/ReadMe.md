---
digest:
  local-classes:
    LzmaDecodeStream:
      mtime: "2026-10-18T05:15:10Z"
      digest: "d5fbf109c374f62ad56ae32d1dd3bcd983b50571975397ff1e92d6f0abbc41e7"
    LzmaEncodeStream:
      mtime: "2026-10-18T05:15:10Z"
      digest: "76a787998b6a4f50d5463275b6bc0639b791183c1d3c7432c3585f3f06c49ba4"
    LzmaProgressCallback:
      mtime: "2026-09-25T20:10:48Z"
      digest: "d52e5afa51eaa5b99c848bfc9f78ba7985d6635d2aebd99ada8c7d42e65ae155"
  folders: {}
---
# LZMA

## Classes

| Class | Responsibility |
|---|---|
| [LzmaDecodeStream](LzmaDecodeStream.cs) | The stream which decompresses data with LZMA on the fly. |
| [LzmaEncodeStream](LzmaEncodeStream.cs) | The stream which compresses data with LZMA on the fly. |
| [LzmaProgressCallback](LzmaProgressCallback.cs) | Callback to implement the ICodeProgress interface |
