---
digest:
  local-classes:
    BinTree:
      mtime: "2026-10-18T05:15:10Z"
      digest: "8377379b3b0188221ab06ab72dc62da0325c79353ce77b4781f4b7f35493f406"
    IInWindowStream:
      mtime: "2026-10-18T05:15:10Z"
      digest: "ef4f11648adcbf03af14b7db402f310dbb179418da0a8e762ae7d9f10e552aac"
    IMatchFinder:
      mtime: "2026-10-18T05:15:10Z"
      digest: "3136ed5752cb7a4a1b12cecf5a41dfca607c0038a8c571d4e6655a8bdf2723ae"
    InWindow:
      mtime: "2026-10-18T05:15:10Z"
      digest: "e7fc3a08bc7803a01d3aa44f68ca209a25a8ba9e3eed59c05fa62f06e824f3fc"
    OutWindow:
      mtime: "2026-10-18T05:15:10Z"
      digest: "d07b9a965a5b1e5e651084ed25fc9f752894566cc9123f589c3072f9f00417e1"
  folders: {}
---
# LZ

## Classes

| Class | Responsibility | Key Collaborators |
|---|---|---|
| [IInWindowStream](IMatchFinder.cs) | Defines a windowed stream reader for compression algorithms to access specific bytes and match lengths. |  |
| [IMatchFinder](IMatchFinder.cs) | Extends the windowed stream to find and return matching patterns for compression. |  |
| [BinTree](LzBinTree.cs) | Binary tree-based match finder for LZ compression using hash tables and a sliding window to locate repeating patterns efficiently. |  |
| [InWindow](LzInWindow.cs) | Manages a sliding input buffer for LZ compression, tracking stream position and available bytes for pattern matching operations. | `Stream`, `Byte`, `UInt32` |
| [OutWindow](LzOutWindow.cs) | Manages a circular buffer window for LZ compression data, buffering and flushing data to an output stream. |  |
