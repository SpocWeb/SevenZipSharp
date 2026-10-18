---
digest:
  local-classes:
    Base:
      mtime: "2026-10-18T05:15:10Z"
      digest: "6c890c503f6c0ceda0888617f2317cf0a075bdb020e505a03898c812769c46bb"
    Decoder:
      mtime: "2026-10-18T05:15:10Z"
      digest: "5ee0c376cd6864e7ac51a06a62f92526bd811cf3face7e116b66ef8c588a63c3"
    Decoder2:
      mtime: "2026-10-18T05:15:10Z"
      digest: "8bce2f7c1986f69dda8433b87b954f3373bbe647f9abf87851cc7bdbcf43f5fe"
    EMatchFinderType:
      mtime: "2026-10-18T05:15:10Z"
      digest: "17f8d9d24ad32b9db1a51e9c5b292b182208d0e98026c4cd3637dac495aef2f9"
    Encoder:
      mtime: "2026-10-18T05:15:10Z"
      digest: "b14608aa30bfb7ec09c323e12c00431012f7cde990cd08413ea904adab4145e4"
    Encoder2:
      mtime: "2026-10-18T05:15:10Z"
      digest: "ae91a8cdca004807af2931d961e583ba60e6509a049b4446dcb44e94ec737e3c"
    LenDecoder:
      mtime: "2026-10-18T05:15:10Z"
      digest: "f5929c9a3ccea2f31422980049487ef26affe7f2e533797a967d1804e7849c2d"
    LenEncoder:
      mtime: "2026-10-18T05:15:10Z"
      digest: "e6174d61a4dd8d28a1cdb1bab053d2a31961330d316673b52ed13bda3ed55a06"
    LenPriceTableEncoder:
      mtime: "2026-10-18T05:15:10Z"
      digest: "8c1080afb1921087312aded327262da66fe65330f7e43ef48cf616c89e63b377"
    LiteralDecoder:
      mtime: "2026-10-18T05:15:10Z"
      digest: "385ae594f02437adc6faeaf9cc57b0098214c2e671609d91e10f94306ed56958"
    LiteralEncoder:
      mtime: "2026-10-18T05:15:10Z"
      digest: "32dfcd907dfed8934ec4c099526f4ca8d4ca0aac22bf5b21cbec5c8b564c6296"
    Optimal:
      mtime: "2026-10-18T05:15:10Z"
      digest: "93a0deb52772a4b399b4e675c5bb25f7a5e93f3ead5ca106603ae02ac923d2d1"
    State:
      mtime: "2026-10-18T05:15:10Z"
      digest: "59737f0501ed960b4514767c70dcc0e5871dd31ae9141ea42d67237cf1cfcb88"
  folders: {}
---
# LZMA

## Classes

| Class | Responsibility | Key Collaborators |
|---|---|---|
| [Base](LzmaBase.cs) | Provides LZMA compression algorithm constants and state management for encoder/decoder implementations. |  |
| [State](LzmaBase.cs) | Encapsulates the current LZMA decoder state and provides transitions for character matches, string matches, and repeats. |  |
| [Decoder](LzmaDecoder.cs) | The LZMA decoder class | `BitDecoder`, `LenDecoder`, `LiteralDecoder`, `OutWindow`, `BitTreeDecoder`, `Decoder`, `ICodeProgress` |
| [LenDecoder](LzmaDecoder.cs) | Decodes length values for literal and match sequences in LZMA decompression. | `BitTreeDecoder`, `BitDecoder`, `Decoder` |
| [LiteralDecoder](LzmaDecoder.cs) | Decodes literal bytes using context-dependent Huffman-like decoders based on match history and position. | `Decoder2`, `Decoder` |
| [Decoder2](LzmaDecoder.cs) | Decodes a single literal byte using binary tree arithmetic decoding. | `BitDecoder`, `Decoder` |
| [Encoder](LzmaEncoder.cs) | The LZMA encoder class | `UInt32`, `Byte`, `BitEncoder`, `LenPriceTableEncoder`, `LiteralEncoder`, `Optimal`, `BitTreeEncoder`, `Encoder`, `IMatchFinder`, `EMatchFinderType`, `State`, `ICodeProgress`, `CoderPropId`, `LenEncoder` |
| [LenEncoder](LzmaEncoder.cs) | Encodes match lengths using a tiered bit tree approach that scales from low to high values. | `BitTreeEncoder`, `BitEncoder` |
| [LenPriceTableEncoder](LzmaEncoder.cs) | Caches precomputed encoder prices for length symbols to accelerate repeated price lookups. | `LenEncoder` |
| [LiteralEncoder](LzmaEncoder.cs) | Encodes literal bytes using context-dependent bit tree encoders based on position and previous byte. | `Encoder2` |
| [Encoder2](LzmaEncoder.cs) | Encodes a single literal byte using binary tree range encoding with adaptive contexts. | `BitEncoder` |
| [Optimal](LzmaEncoder.cs) | Tracks the optimal parsing state and cost for each position in dynamic programming optimization. |  |
| [EMatchFinderType](LzmaEncoder.cs) | Specifies the binary tree match finder algorithm variant used for string matching during compression. |  |
