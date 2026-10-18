---
digest:
  local-classes:
    BitDecoder:
      mtime: "2026-10-18T05:15:11Z"
      digest: "d1dcb4c6e1f652d32fb314fc49a5f12cb15f536ac814155fda9cbee04575ceea"
    BitEncoder:
      mtime: "2026-10-18T05:15:11Z"
      digest: "f5ffaa18b597d65c4dc1ac09d0039eac0a83ddcf45b1ada907389129b48c4878"
    BitTreeDecoder:
      mtime: "2026-10-18T05:15:11Z"
      digest: "24d8748304d1fc3c16868bce8b838883cfc9078913dcc7136e4d3b8dce160069"
    BitTreeEncoder:
      mtime: "2026-10-18T05:15:11Z"
      digest: "3e6665e51e681e932d8f782eb90806585bd13db45b8f4b1c802a0992d66e6287"
    Decoder:
      mtime: "2026-10-18T05:15:10Z"
      digest: "1aa247d30f8d0a7c095c1d022b86f7f49f76003c73cc14278b47f765f8763f2e"
    Encoder:
      mtime: "2026-10-18T05:15:10Z"
      digest: "11d2d3ae5e45757bb98c8d0887b0ace587744bd6542e1de00f2f48c066c8f507"
  folders: {}
---
# RangeCoder

## Classes

| Class | Responsibility | Key Collaborators |
|---|---|---|
| [Encoder](RangeCoder.cs) | Encodes bit sequences using arithmetic range coding, maintaining a probability range and flushing encoded output to a stream. | `Stream`, `UInt64` |
| [Decoder](RangeCoder.cs) | Decodes bit sequences from a stream using arithmetic range coding by tracking a code value and probability range. | `Stream` |
| [BitEncoder](RangeCoderBit.cs) | Encodes binary symbols for range compression using adaptive probability estimates. | `UInt32`, `Encoder` |
| [BitDecoder](RangeCoderBit.cs) | Decodes binary symbols from range-compressed data using adaptive probability estimates. | `Decoder` |
| [BitTreeEncoder](RangeCoderBitTree.cs) | Encodes symbols using a hierarchical bit-tree model with range encoding. |  |
| [BitTreeDecoder](RangeCoderBitTree.cs) | Decodes symbols using a hierarchical bit-tree model with range decoding. |  |
