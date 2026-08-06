---
digest:
  local-classes:
    FileInfoEventArgs:
      mtime: "2026-08-06T06:59:29Z"
      digest: "097e5d4153c9d338237ae08235d531fed7d16a2a904e70c20e95de09caf7b56c"
    ICancellable:
      mtime: "2026-08-06T06:59:29Z"
      digest: "21d6c03f693b8217e6803e9bcca91b49a5aa00d99f12f2fc67548b2731d6d120"
    PercentDoneEventArgs:
      mtime: "2026-08-06T06:59:29Z"
      digest: "fcc9dfbeac1504d9102c3708677713d7c201d569b8af5f88940bc1efc32df227"
    ProgressEventArgs:
      mtime: "2026-08-06T06:59:29Z"
      digest: "c241f573236c3c082b26e9c4c7445c861e714b933c652bc94ea2fb4dcf893bcc"
  folders: {}
---
# EventArguments

This folder provides `FileInfoEventArgs`, `ICancellable`, `PercentDoneEventArgs` and related types. `FileInfoEventArgs` eventArgs used to report the file information which is going to be packed.

## Classes

| Class | Responsibility |
|---|---|
| [FileInfoEventArgs](FileInfoEventArgs.cs) | EventArgs used to report the file information which is going to be packed. |
| [ICancellable](ICancellable.cs) | The definition of the interface which supports the cancellation of a process. |
| [PercentDoneEventArgs](PercentDoneEventArgs.cs) | EventArgs for storing PercentDone property. |
| [ProgressEventArgs](ProgressEventArgs.cs) | The EventArgs class for accurate progress handling. |

## Architecture

Plain `EventArgs` payloads raised by `SevenZipExtractor`/`SevenZipCompressor` during
pack/unpack operations. `ICancellable` is implemented by the progress-carrying
argument types so a subscriber can request cancellation or skip from within an
event handler; there is no cross-type collaboration beyond that shared contract.

