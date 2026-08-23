using org.SpocWeb.root.Attributes;
namespace SevenZip.Sdk.Compression.LZ
{
    using System;
    using System.IO;

    /// <summary>TODO: LLM</summary>
    [DocState(Pass = 2, MTime = "2026-08-23T11:34:46Z", Digest = "ed0b95c6c3d6f414b5859ffd8e79edf8aa58dc9a06d5b4e6bc069d2e52b912c6", Stale = true, Path = "sdk/Compress/LZ/IMatchFinder.cs", Since = "2026-08-23")]
    internal interface IInWindowStream
    {

        /// <summary>TODO: LLM</summary>
        void SetStream(Stream inStream);

        /// <summary>TODO: LLM</summary>
        void Init();

        /// <summary>TODO: LLM</summary>
        void ReleaseStream();

        /// <summary>TODO: LLM</summary>
        Byte GetIndexByte(Int32 index);

        /// <summary>TODO: LLM</summary>
        UInt32 GetMatchLen(Int32 index, UInt32 distance, UInt32 limit);

        /// <summary>TODO: LLM</summary>
        UInt32 GetNumAvailableBytes();
    }

    /// <inheritdoc cref="IInWindowStream"/>
    [DocState(Pass = 2, MTime = "2026-08-23T11:34:46Z", Digest = "6d66ee628326d43048c3e3f20b9f2fccf21e37e7b15298b2338b874def24126e", Stale = true, Path = "sdk/Compress/LZ/IMatchFinder.cs", Since = "2026-08-23")]
    internal interface IMatchFinder : IInWindowStream
    {

        /// <summary>TODO: LLM</summary>
        void Create(UInt32 historySize, UInt32 keepAddBufferBefore,
                    UInt32 matchMaxLen, UInt32 keepAddBufferAfter);

        /// <summary>TODO: LLM</summary>
        UInt32 GetMatches(UInt32[] distances);

        /// <summary>TODO: LLM</summary>
        void Skip(UInt32 num);
    }
}