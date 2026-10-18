---
digest:
  local-classes:
    CoderPropId:
      mtime: "2026-10-18T05:22:32Z"
      digest: "f2870bb7734f2489c4458fb0cb673cec3d901b59007e945d2ca837ec8181eade"
    DataErrorException:
      mtime: "2026-10-18T05:22:32Z"
      digest: "2a54acf601042205aacae5aee63187feb5a00b4bcfccaffaf6aec4c5679c39b4"
    ICodeProgress:
      mtime: "2026-10-18T05:22:32Z"
      digest: "fbb6c8dad66643c6593060e6ff6413d90de67a7ed73d42d1bc7eb0389f24bfa1"
    ICoder:
      mtime: "2026-10-18T05:22:32Z"
      digest: "cb5cbf12d71d7b0f3bd00d2075455a9fa66fcd7a9cd3b8b7cfbb40a8f23e85a7"
    InvalidParamException:
      mtime: "2026-10-18T05:22:32Z"
      digest: "f2d9b2b9b0610d7243ec731a37c174a011116806f0d45f9d64c783653c069468"
    ISetCoderProperties:
      mtime: "2026-10-18T05:22:32Z"
      digest: "755162766db78b413267b067382883e26d39fde5bd0a37d166983b40309e88c7"
    ISetDecoderProperties:
      mtime: "2026-10-18T05:22:32Z"
      digest: "4cf33d8bdde3edb8feb51cfba66727968c5cb3ff777473d5f744eaa0c0503999"
    IWriteCoderProperties:
      mtime: "2026-10-18T05:22:32Z"
      digest: "0f1d34c56f42af96f516c12b7d17068a0f96f8fc3391b8a94ddde9747929fb5a"
  folders: {}
---
# sdk

## Classes

| Class | Responsibility |
|---|---|
| [DataErrorException](ICoder.cs) | The exception that is thrown when an error in input stream occurs during decoding. |
| [InvalidParamException](ICoder.cs) | The exception that is thrown when the value of an argument is outside the allowable range. |
| [ICodeProgress](ICoder.cs) | Callback progress interface. |
| [ICoder](ICoder.cs) | Stream coder interface |
| [ISetCoderProperties](ICoder.cs) | Defines the interface for setting encoder/decoder properties by identifier and value. |
| [IWriteCoderProperties](ICoder.cs) | Defines the interface for writing encoder/decoder properties to a stream. |
| [ISetDecoderProperties](ICoder.cs) | The ISetDecoderPropertiesinterface |
| [CoderPropId](ICoder.cs) | Provides the fields that represent properties idenitifiers for compressing. |
