---
digest:
  local-classes:
    FileCheckerTestData:
      mtime: "2026-08-06T07:02:34Z"
      digest: "fa7a89f3b2d9944b3333bb79cdad1f85e5348c2ae2a018b5c685126d679bc06d"
    FileCheckerTests:
      mtime: "2026-08-06T07:02:34Z"
      digest: "00cdec4095cd6faa29684326c9c1ddda13fcff07c3fad574c6e5e32f89b818c5"
    LibraryManagerTests:
      mtime: "2026-08-06T06:59:49Z"
      digest: "102293e94063feb14a2ee31d677bda21ef595637df16cf2c7f64ba35bfafce03"
    MiscellaneousTests:
      mtime: "2026-08-06T06:59:49Z"
      digest: "26782b32c60853e3fc9d29a1bc56ac8e23e85e2cecf63ebe1ebe5f6c0bffd369"
    SevenZipCompressorAsynchronousTests:
      mtime: "2026-08-06T06:59:49Z"
      digest: "443dd3b5eaeb0c19dbda88700012ef2cbd96e07d18c2601caf18a74cc340046c"
    SevenZipCompressorCustomParameterTests:
      mtime: "2026-08-06T07:02:34Z"
      digest: "c01abb6091bf59ec49d3b4eb9ca8d438d4f6730537ebb83cb12d6ef5b5540f5b"
    SevenZipCompressorTests:
      mtime: "2026-08-06T06:59:49Z"
      digest: "2881180b57f40b5e2ac27f69d9df434dc7669e7c6ac3be67ce9ad067775130c2"
    SevenZipExtractorAsynchronousTests:
      mtime: "2026-08-06T06:59:49Z"
      digest: "fff1830b02b33e3b563ad797e03ec0353d38ee52623aaf8392011933bc9024ff"
    SevenZipExtractorTests:
      mtime: "2026-08-06T07:02:34Z"
      digest: "8ecf22186c0ca90d5c2283ab27d5d863c9793d29fa43b0d4066a7fb47f5497ac"
    TestBase:
      mtime: "2026-08-06T06:59:49Z"
      digest: "862eec8aede190b60ea50a4c3a2f037e48ce25a98d0f89f545c6f087143fb9fc"
    TestFile:
      mtime: "2026-08-06T07:02:34Z"
      digest: "6b8b55b97e299d81c17c4dba2c079c6e91ea59f37bd466c65fc677f86efc8917"
  folders: {}
---
# SevenZip.Tests

This folder provides `SevenZipCompressorTests`, `SevenZipExtractorTests`, `SevenZipCompressorCustomParameterTests` and related types. `SevenZipCompressorTests` tests for seven Zip Compressor.

## Classes

| Class | Responsibility |
|---|---|
| [FileCheckerTestData](FileCheckerTests.cs) | Test data to use for CheckFileSignatureTest. |
| [FileCheckerTests](FileCheckerTests.cs) | Tests for file Checker. |
| [LibraryManagerTests](LibraryManagerTests.cs) | Tests for library Manager. |
| [MiscellaneousTests](MiscellaneousTests.cs) | Tests for miscellaneous. |
| [SevenZipCompressorAsynchronousTests](SevenZipCompressorAsynchronousTests.cs) | Tests for seven Zip Compressor Asynchronous. |
| [SevenZipCompressorCustomParameterTests](SevenZipCompressorCustomParameterTests.cs) | Verifies that CustomParameters is only honored  when combined with a compatible compression method. |
| [SevenZipCompressorTests](SevenZipCompressorTests.cs) | Tests for seven Zip Compressor. |
| [SevenZipExtractorAsynchronousTests](SevenZipExtractorAsynchronousTests.cs) | Tests for seven Zip Extractor Asynchronous. |
| [SevenZipExtractorTests](SevenZipExtractorTests.cs) | Tests for seven Zip Extractor. |
| [TestFile](SevenZipExtractorTests.cs) | Simple wrapper to get better names for ExtractDifferentFormatsTest results. |
| [TestBase](TestBase.cs) | base used in tests. |

## Architecture

`TestBase` is the shared `[SetUp]`/`[TearDown]` NUnit fixture (output/temp-file
management) all test-fixture classes inherit from; each `*Tests` class then
exercises one public entry point of the `../SevenZip` project (`SevenZipCompressor`,
`SevenZipExtractor`, `FileChecker`, `LibraryManager`) against the archives in
`TestData/`.

