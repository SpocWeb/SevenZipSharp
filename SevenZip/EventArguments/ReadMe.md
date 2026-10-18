---
digest:
  local-classes:
    ExtractFileCallbackArgs:
      mtime: "2026-10-18T05:07:18Z"
      digest: "63bdd2fd420d0f6c22b6285d5c73177dad595ff8ef3f824fab6f840d2c415a80"
    ExtractFileCallbackReason:
      mtime: "2026-10-18T05:15:10Z"
      digest: "45a7f39417d934dfe74dd60c5f8d0de65cac9a0662f874399f0ebf575e526dc3"
    FileInfoEventArgs:
      mtime: "2026-10-18T05:15:10Z"
      digest: "097e5d4153c9d338237ae08235d531fed7d16a2a904e70c20e95de09caf7b56c"
    FileNameEventArgs:
      mtime: "2026-10-18T05:15:10Z"
      digest: "0bc68123074526de9ceceb6218b611008b145f19abe53ee25723c91946a585d7"
    FileOverwriteEventArgs:
      mtime: "2026-10-18T05:15:10Z"
      digest: "2973ff5ab42578525e6a8073d6e34abc3764f03b74219b26b65c21e28ab29df8"
    ICancellable:
      mtime: "2026-09-25T20:10:48Z"
      digest: "21d6c03f693b8217e6803e9bcca91b49a5aa00d99f12f2fc67548b2731d6d120"
    IntEventArgs:
      mtime: "2026-10-18T05:15:10Z"
      digest: "3d6f1a8169bfc317f174ae1359240cb6eb9d118ece68f35f95d9d5db7bbae2b7"
    OpenEventArgs:
      mtime: "2026-10-18T05:15:10Z"
      digest: "3e1c058e45a275bfad8003133a02447f0d5c4fcd20e103b320312d2336a862f8"
    PercentDoneEventArgs:
      mtime: "2026-09-25T20:10:48Z"
      digest: "fcc9dfbeac1504d9102c3708677713d7c201d569b8af5f88940bc1efc32df227"
    ProgressEventArgs:
      mtime: "2026-09-25T20:10:48Z"
      digest: "c241f573236c3c082b26e9c4c7445c861e714b933c652bc94ea2fb4dcf893bcc"
  folders: {}
---
# EventArguments

## Classes

| Class | Responsibility |
|---|---|
| [ExtractFileCallback](ExtractFileCallback.cs) | Callback delegate for SevenZipExtractor.ExtractFiles(ExtractFileCallback). |
| [ExtractFileCallbackArgs](ExtractFileCallbackArgs.cs) | The arguments passed to ExtractFileCallback. |
| [ExtractFileCallbackReason](ExtractFileCallbackReason.cs) | The reason for calling ExtractFileCallback. |
| [FileInfoEventArgs](FileInfoEventArgs.cs) | EventArgs used to report the file information which is going to be packed. |
| [FileNameEventArgs](FileNameEventArgs.cs) | EventArgs class which stores the file name. |
| [FileOverwriteEventArgs](FileOverwriteEventArgs.cs) | EventArgs for FileExists event, stores the file name and asks whether to overwrite it in case it already exists. |
| [ICancellable](ICancellable.cs) | The definition of the interface which supports the cancellation of a process. |
| [IntEventArgs](IntEventArgs.cs) | Stores an int number |
| [OpenEventArgs](OpenEventArgs.cs) | EventArgs used to report the size of unpacked archive data |
| [PercentDoneEventArgs](PercentDoneEventArgs.cs) | EventArgs for storing PercentDone property. |
| [ProgressEventArgs](ProgressEventArgs.cs) | The EventArgs class for accurate progress handling. |
