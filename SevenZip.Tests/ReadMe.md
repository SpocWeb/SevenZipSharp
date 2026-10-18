---
digest:
  local-classes:
    FileCheckerTestData:
      mtime: "2026-10-18T05:15:11Z"
      digest: "fa7a89f3b2d9944b3333bb79cdad1f85e5348c2ae2a018b5c685126d679bc06d"
    FileCheckerTests:
      mtime: "2026-10-18T05:15:11Z"
      digest: "96a24fc88d6ee4960a6b32c2aa678c6c00cc0d7029a64f76b4b91198fbcac0d7"
    LibraryManagerTests:
      mtime: "2026-09-25T20:10:48Z"
      digest: "102293e94063feb14a2ee31d677bda21ef595637df16cf2c7f64ba35bfafce03"
    MiscellaneousTests:
      mtime: "2026-10-18T05:15:11Z"
      digest: "d12e9101e5c6b3f0265d86f0d2cca1579f51d51534342c47740f3c59cec2a915"
    SevenZipCompressorAsynchronousTests:
      mtime: "2026-09-25T20:10:48Z"
      digest: "443dd3b5eaeb0c19dbda88700012ef2cbd96e07d18c2601caf18a74cc340046c"
    SevenZipCompressorCustomParameterTests:
      mtime: "2026-10-18T05:15:11Z"
      digest: "c01abb6091bf59ec49d3b4eb9ca8d438d4f6730537ebb83cb12d6ef5b5540f5b"
    SevenZipCompressorTests:
      mtime: "2026-10-18T05:15:11Z"
      digest: "2881180b57f40b5e2ac27f69d9df434dc7669e7c6ac3be67ce9ad067775130c2"
    SevenZipExtractorAsynchronousTests:
      mtime: "2026-10-18T05:15:11Z"
      digest: "fff1830b02b33e3b563ad797e03ec0353d38ee52623aaf8392011933bc9024ff"
    SevenZipExtractorTests:
      mtime: "2026-10-18T05:15:11Z"
      digest: "6c115e43f34fe0d457b67f7d5618e301561beef2b3eca541a4a04a2267d619b8"
    TestBase:
      mtime: "2026-10-18T05:15:11Z"
      digest: "c263d14bb22485fb7f3066d9eb2bf8da79b05bd6aa3dce2da238ff74ec2d5400"
    TestFile:
      mtime: "2026-10-18T05:15:11Z"
      digest: "2f4b897ceab1031536ede96e7d07d89ef51437f57b3267bf10a7014218f1628e"
  folders: {}
---
# SevenZip.Tests

## Classes

| Class | Responsibility | Key Collaborators |
|---|---|---|
| [FileCheckerTestData](FileCheckerTests.cs) | Test data to use for CheckFileSignatureTest. |  |
| [FileCheckerTests](FileCheckerTests.cs) | Tests for file Checker. |  |
| [LibraryManagerTests](LibraryManagerTests.cs) | Tests for library Manager. |  |
| [MiscellaneousTests](MiscellaneousTests.cs) | Tests for miscellaneous. |  |
| [SevenZipCompressorAsynchronousTests](SevenZipCompressorAsynchronousTests.cs) | Tests for seven Zip Compressor Asynchronous. |  |
| [SevenZipCompressorCustomParameterTests](SevenZipCompressorCustomParameterTests.cs) | Tests that custom compression parameters are correctly applied across different archive formats and compression methods. |  |
| [SevenZipCompressorTests](SevenZipCompressorTests.cs) | Tests for the SevenZip Compressor. | `SevenZipCompressor`, `SevenZipExtractor`, `CompressionMethod` |
| [SevenZipExtractorAsynchronousTests](SevenZipExtractorAsynchronousTests.cs) | Tests for seven Zip Extractor Asynchronous. |  |
| [SevenZipExtractorTests](SevenZipExtractorTests.cs) | Tests for seven Zip Extractor. | `TestFile` |
| [TestFile](SevenZipExtractorTests.cs) | Simple wrapper to get better names for ExtractDifferentFormatsTest results. |  |
| [TestBase](TestBase.cs) | base used in tests. |  |
