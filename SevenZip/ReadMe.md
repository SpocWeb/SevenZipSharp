---
digest:
  local-classes:
    ArchiveEmulationStreamProxy:
      mtime: "2026-08-06T07:02:34Z"
      digest: "b14a9fa6fab6a41dc1f0a6e8e63855885fd7a5eda9ac54f12e57337c682f00d3"
    LibraryFeature:
      mtime: "2026-08-06T06:59:29Z"
      digest: "5589d311440652e581fbcb84a7cce5df4f554c81ab94106f8a42bb611fe69bf0"
    SevenZipCompressor:
      mtime: "2026-08-06T06:59:49Z"
      digest: "18c86e8dfe6787af1c88517afc8511e99655e40bf1b0eeaf51a7dcdbe248d6d3"
    SevenZipExtractor:
      mtime: "2026-08-06T06:59:49Z"
      digest: "7d2cdcccb7dadd2da9a8055bba8d94331a13ba54d26c520a70ae7f1cb6468524"
  folders: {}
---
# SevenZip

This folder provides `SevenZipCompressor`, `LibraryFeature`, `ArchiveEmulationStreamProxy` and related types. `SevenZipCompressor` asynchronous (Begin*/*Async) overloads of SevenZipCompressor's pack and modify operations.

## Classes

| Class | Responsibility | Key Collaborators |
|---|---|---|
| [ArchiveEmulationStreamProxy](ArchiveEmulationStreamProxy.cs) | The Stream extension class to emulate the archive part of a stream. |  |
| [LibraryFeature](LibraryFeature.cs) | The set of features supported by the library. |  |
| [SevenZipCompressor](SevenZipCompressor.cs) | Class to pack data into archives supported by 7-Zip. | `Encoder`, `EventHandler`, `ProgressEventArgs` |
| [SevenZipCompressor](SevenZipCompressorAsynchronous.cs) | Asynchronous (Begin*/*Async) overloads of SevenZipCompressor's pack and modify operations. |  |
| [SevenZipExtractor](SevenZipExtractor.cs) | Class to unpack data from archives supported by 7-Zip. | `EventHandler`, `ProgressEventArgs` |
| [SevenZipExtractor](SevenZipExtractorAsynchronous.cs) | Asynchronous (Begin*/*Async) overloads of SevenZipExtractor's unpack operations. |  |

## Subsystems

| Folder | Class | Responsibility |
|---|---|---|
| [`EventArguments/`](EventArguments/ReadMe.md) | [FileInfoEventArgs](EventArguments/FileInfoEventArgs.cs) | EventArgs used to report the file information which is going to be packed. |
| [`EventArguments/`](EventArguments/ReadMe.md) | [ICancellable](EventArguments/ICancellable.cs) | The definition of the interface which supports the cancellation of a process. |
| [`EventArguments/`](EventArguments/ReadMe.md) | [PercentDoneEventArgs](EventArguments/PercentDoneEventArgs.cs) | EventArgs for storing PercentDone property. |
| [`EventArguments/`](EventArguments/ReadMe.md) | [ProgressEventArgs](EventArguments/ProgressEventArgs.cs) | The EventArgs class for accurate progress handling. |
| [`Exceptions/`](Exceptions/ReadMe.md) | [LzmaException](Exceptions/LzmaException.cs) | Exception class for LZMA operations. |
| [`Exceptions/`](Exceptions/ReadMe.md) | [SevenZipException](Exceptions/SevenZipException.cs) | Base SevenZip exception class. |
| [`Exceptions/`](Exceptions/ReadMe.md) | [SevenZipSfxValidationException](Exceptions/SevenZipSfxValidationException.cs) | Exception class for 7-zip sfx settings validation. |
| [`LZMA/`](LZMA/ReadMe.md) | [LzmaDecodeStream](LZMA/LzmaDecodeStream.cs) | The stream which decompresses data with LZMA on the fly. |
| [`LZMA/`](LZMA/ReadMe.md) | [LzmaEncodeStream](LZMA/LzmaEncodeStream.cs) | The stream which compresses data with LZMA on the fly. |
| [`LZMA/`](LZMA/ReadMe.md) | [LzmaProgressCallback](LZMA/LzmaProgressCallback.cs) | Callback to implement the ICodeProgress interface |
| [`sdk/`](sdk/ReadMe.md) | [DataErrorException](sdk/ICoder.cs) | The exception that is thrown when an error in input stream occurs during decoding. |
| [`sdk/`](sdk/ReadMe.md) | [InvalidParamException](sdk/ICoder.cs) | The exception that is thrown when the value of an argument is outside the allowable range. |
| [`sdk/`](sdk/ReadMe.md) | [ICodeProgress](sdk/ICoder.cs) | Callback progress interface. |
| [`sdk/`](sdk/ReadMe.md) | [ICoder](sdk/ICoder.cs) | Stream coder interface |
| [`sdk/`](sdk/ReadMe.md) | [ISetCoderProperties](sdk/ICoder.cs) | The ISetCoderProperties interface |
| [`sdk/`](sdk/ReadMe.md) | [IWriteCoderProperties](sdk/ICoder.cs) | The IWriteCoderProperties interface |
| [`sdk/`](sdk/ReadMe.md) | [ISetDecoderProperties](sdk/ICoder.cs) | The ISetDecoderPropertiesinterface |
| [`sdk/`](sdk/ReadMe.md) | [CoderPropId](sdk/ICoder.cs) | Provides the fields that represent properties idenitifiers for compressing. |

## Architecture

`SevenZipCompressor` and `SevenZipExtractor` are the library's two public entry
points; both wrap the native `7z.dll` via COM for general archive formats and,
for the 7z/LZMA-only fast path, drive the fully managed `LZMA/LzmaEncodeStream`
and `LzmaDecodeStream`, which in turn sit on top of `sdk/`'s vendored LZMA SDK.
Long-running operations raise `EventArguments/` events (`FileInfoEventArgs`,
`ProgressEventArgs`, ...) that support cooperative cancellation via
`ICancellable`; failures surface as `Exceptions/` types.

```mermaid
flowchart LR
    SevenZipCompressor["SevenZipCompressor"]
    SevenZipExtractor["SevenZipExtractor"]
    NativeCOM["native 7z.dll (COM)"]
    LzmaStreams["LZMA/ LzmaEncodeStream, LzmaDecodeStream"]
    Sdk["sdk/ vendored LZMA SDK"]
    EventArgs["EventArguments/"]
    Exceptions["Exceptions/"]

    SevenZipCompressor -->|packs via| NativeCOM
    SevenZipExtractor -->|unpacks via| NativeCOM
    SevenZipCompressor -->|fast-path packs via| LzmaStreams
    SevenZipExtractor -->|fast-path unpacks via| LzmaStreams
    LzmaStreams -->|built on| Sdk
    SevenZipCompressor -.->|raises| EventArgs
    SevenZipExtractor -.->|raises| EventArgs
    SevenZipCompressor -.->|throws| Exceptions
    SevenZipExtractor -.->|throws| Exceptions
```

## Entry Points

- [`SevenZipCompressor`](SevenZipCompressor.cs) — pack/update archives; async overloads in [`SevenZipCompressorAsynchronous.cs`](SevenZipCompressorAsynchronous.cs).
- [`SevenZipExtractor`](SevenZipExtractor.cs) — extract/list archives; async overloads in [`SevenZipExtractorAsynchronous.cs`](SevenZipExtractorAsynchronous.cs).

