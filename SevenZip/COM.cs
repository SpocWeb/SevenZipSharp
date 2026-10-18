namespace SevenZip
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
#if NET45 || NETSTANDARD2_0
    using System.Security.Permissions;
#endif
    using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

#if UNMANAGED

    // ReSharper disable file ConvertToAutoProperty - For UWP compatibility.

    /// <summary>
    /// The structure to fix x64 and x32 variant size mismatch.
    /// </summary>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855
    /// </code>
    /// </example>
    [StructLayout(LayoutKind.Sequential)]
    internal struct PropArray
    {
        readonly uint _cElems;
        readonly IntPtr _pElems;
    }

    /// <summary>
    /// COM VARIANT structure with special interface routines.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 86 | <see cref="VarType"/> | Gets or sets variant type. |
    /// | 102 | <see cref="Value"/> | Gets or sets the pointer value of the COM variant |
    /// | 112 | <see cref="UInt32Value"/> | Gets or sets the UInt32 value of the COM variant. |
    /// | 122 | <see cref="Int32Value"/> | Gets or sets the UInt32 value of the COM variant. |
    /// | 132 | <see cref="Int64Value"/> | Gets or sets the Int64 value of the COM variant |
    /// | 142 | <see cref="UInt64Value"/> | Gets or sets the UInt64 value of the COM variant |
    /// | 152 | <see cref="Object"/> | Gets the object for this PropVariant. |
    /// | 251 | <see cref="operator =="/> | Determines whether the specified PropVariant instances are considered equal. |
    /// | 262 | <see cref="operator !="/> | Determines whether the specified PropVariant instances are not considered equal. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: fde425c42970eabda46c7979d1e6b662c4f9f6b9a64414a7edab0cb38651de36
    /// </code>
    /// </example>
    [StructLayout(LayoutKind.Explicit)]
    internal struct PropVariant
    {
        [FieldOffset(0)] private ushort _vt;

        /// <summary>
        /// FILETIME variant value.
        /// </summary>
        [FieldOffset(8)] private readonly FILETIME _fileTime;

        /// <summary>
        /// The PropArray instance to fix the variant size on x64 bit systems.
        /// </summary>
        [FieldOffset(8)]
        private readonly PropArray _propArray;

        [FieldOffset(8)] private IntPtr _value;
        [FieldOffset(8)] private uint _uInt32Value;
        [FieldOffset(8)] private int _int32Value;
        [FieldOffset(8)] private long _int64Value;
        [FieldOffset(8)] private ulong _uInt64Value;

        /// <summary>
        /// Gets or sets variant type.
        /// </summary>
        public VarEnum VarType
        {
            private get
            {
                return (VarEnum) _vt;
            }

            set
            {
                _vt = (ushort) value;
            }
        }

        /// <summary>
        /// Gets or sets the pointer value of the COM variant
        /// </summary>
        public IntPtr Value
        {
            get => _value;
            set => _value = value;
        }

        /// <summary>
        /// Gets or sets the UInt32 value of the COM variant.
        /// </summary>
        
        public uint UInt32Value
        {
            get => _uInt32Value;
            set => _uInt32Value = value;
        }

        /// <summary>
        /// Gets or sets the UInt32 value of the COM variant.
        /// </summary>
        
        public int Int32Value
        {
            get => _int32Value;
            set => _int32Value = value;
        }

        /// <summary>
        /// Gets or sets the Int64 value of the COM variant
        /// </summary>
        
        public long Int64Value
        {
            get => _int64Value;
            set => _int64Value = value;
        }

        /// <summary>
        /// Gets or sets the UInt64 value of the COM variant
        /// </summary>
        
        public ulong UInt64Value
        {
            get => _uInt64Value;
            set => _uInt64Value = value;
        }

        /// <summary>
        /// Gets the object for this PropVariant.
        /// </summary>
        /// <returns></returns>
        public object Object
        {
            get
            {
#if NET45 || NETSTANDARD2_0
                var sp = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
                sp.Demand();
#endif
                switch (VarType)
                {
                    case VarEnum.VT_BSTR:
                        return Marshal.PtrToStringBSTR(Value);
                    case VarEnum.VT_EMPTY:
                        return null;
                    case VarEnum.VT_FILETIME:
                        try
                        {
                            return DateTime.FromFileTime(Int64Value);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            return DateTime.MinValue;
                        }
                    default:
                        var propHandle = GCHandle.Alloc(this, GCHandleType.Pinned);
                        
                        try
                        {
                            return Marshal.GetObjectForNativeVariant(propHandle.AddrOfPinnedObject());
                        }
                        catch (NotSupportedException)
                        {
                            switch (VarType)
                            {
                                case VarEnum.VT_UI8:
                                    return UInt64Value;
                                case VarEnum.VT_UI4:
                                    return UInt32Value;
                                case VarEnum.VT_I8:
                                    return Int64Value;
                                case VarEnum.VT_I4:
                                    return Int32Value;
                                default:
                                    return 0;
                            }
                        }
                        finally
                        {
                            propHandle.Free();
                        }
                }
            }
        }

		/// <summary>
		/// Determines whether the specified System.Object is equal to the current PropVariant.
		/// </summary>
		/// <param name="obj">The System.Object to compare with the current PropVariant.</param>
		/// <returns>true if the specified System.Object is equal to the current PropVariant; otherwise, false.</returns>
		public override bool Equals(object obj) => obj is PropVariant variant && Equals(variant);

		/// <summary>
		/// Determines whether the specified PropVariant is equal to the current PropVariant.
		/// </summary>
		/// <param name="afi">The PropVariant to compare with the current PropVariant.</param>
		/// <returns>true if the specified PropVariant is equal to the current PropVariant; otherwise, false.</returns>
		private bool Equals(PropVariant afi)
        {
            if (afi.VarType != VarType)
            {
                return false;
            }

            if (VarType != VarEnum.VT_BSTR)
            {
                return afi.Int64Value == Int64Value;
            }

            return afi.Value == Value;
        }

		/// <summary>
		///  Serves as a hash function for a particular type.
		/// </summary>
		/// <returns> A hash code for the current PropVariant.</returns>
		public override int GetHashCode() => Value.GetHashCode();

		/// <summary>
		/// Returns a System.String that represents the current PropVariant.
		/// </summary>
		/// <returns>A System.String that represents the current PropVariant.</returns>
		public override string ToString() => "[" + Value + "] " + Int64Value.ToString(CultureInfo.CurrentCulture);

		/// <summary>
		/// Determines whether the specified PropVariant instances are considered equal.
		/// </summary>
		/// <param name="afi1">The first PropVariant to compare.</param>
		/// <param name="afi2">The second PropVariant to compare.</param>
		/// <returns>true if the specified PropVariant instances are considered equal; otherwise, false.</returns>
		public static bool operator ==(PropVariant afi1, PropVariant afi2)
        {
            return afi1.Equals(afi2);
        }

        /// <summary>
        /// Determines whether the specified PropVariant instances are not considered equal.
        /// </summary>
        /// <param name="afi1">The first PropVariant to compare.</param>
        /// <param name="afi2">The second PropVariant to compare.</param>
        /// <returns>true if the specified PropVariant instances are not considered equal; otherwise, false.</returns>
        public static bool operator !=(PropVariant afi1, PropVariant afi2)
        {
            return !afi1.Equals(afi2);
        }
    }

    /// <summary>
    /// Stores file extraction modes.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 293 | <see cref="Extract"/> | Extraction mode |
    /// | 297 | <see cref="Test"/> | Test mode |
    /// | 301 | <see cref="Skip"/> | Skip mode |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 9b1982c79908f12b8678fa2d99ee53caa4f5173c87ed1d959f776c7b4ef5dbb3
    /// </code>
    /// </example>
    internal enum AskMode
    {
        /// <summary>
        /// Extraction mode
        /// </summary>
        Extract = 0,
        /// <summary>
        /// Test mode
        /// </summary>
        Test,
        /// <summary>
        /// Skip mode
        /// </summary>
        Skip
    }

    /// <summary>
    /// Stores operation result values
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 336 | <see cref="Ok"/> | Success |
    /// | 340 | <see cref="UnsupportedMethod"/> | Method is unsupported |
    /// | 344 | <see cref="DataError"/> | Data error has occurred |
    /// | 348 | <see cref="CrcError"/> | CrcError has occurred |
    /// | 352 | <see cref="Unavailable"/> | File is unavailable |
    /// | 356 | <see cref="UnexpectedEnd"/> | Unexpected end of file |
    /// | 360 | <see cref="DataAfterEnd"/> | Data after end of archive |
    /// | 364 | <see cref="IsNotArc"/> | File is not archive |
    /// | 368 | <see cref="HeadersError"/> | Archive headers error |
    /// | 372 | <see cref="WrongPassword"/> | Wrong password |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 681b210f56b815391c780a3d7b7ab2ddb4e32c3e23abd9b3925746d8d56e8aca
    /// </code>
    /// </example>
    public enum OperationResult
    {
        /// <summary>
        /// Success
        /// </summary>
        Ok = 0,
        /// <summary>
        /// Method is unsupported
        /// </summary>
        UnsupportedMethod,
        /// <summary>
        /// Data error has occurred
        /// </summary>
        DataError,
        /// <summary>
        /// CrcError has occurred
        /// </summary>
        CrcError,
        /// <summary>
        /// File is unavailable
        /// </summary>
        Unavailable,
        /// <summary>
        /// Unexpected end of file
        /// </summary>
        UnexpectedEnd,
        /// <summary>
        /// Data after end of archive
        /// </summary>
        DataAfterEnd,
        /// <summary>
        /// File is not archive
        /// </summary>
        IsNotArc,
        /// <summary>
        /// Archive headers error
        /// </summary>
        HeadersError,
        /// <summary>
        /// Wrong password
        /// </summary>
        WrongPassword
    }

    /// <summary>
    /// Codes of item properties
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 495 | <see cref="NoProperty"/> | No property |
    /// | 496 | <see cref="MainSubfile"/> | Represents main Subfile. |
    /// | 500 | <see cref="HandlerItemIndex"/> | Handler item index |
    /// | 504 | <see cref="Path"/> | Item path |
    /// | 508 | <see cref="Name"/> | Item name |
    /// | 512 | <see cref="Extension"/> | Item extension |
    /// | 516 | <see cref="IsDirectory"/> | true if the item is a folder; otherwise, false |
    /// | 520 | <see cref="Size"/> | Item size |
    /// | 524 | <see cref="PackedSize"/> | Item packed sise; usually absent |
    /// | 528 | <see cref="Attributes"/> | Item attributes; usually absent |
    /// | 532 | <see cref="CreationTime"/> | Item creation time; usually absent |
    /// | 536 | <see cref="LastAccessTime"/> | Item last access time; usually absent |
    /// | 540 | <see cref="LastWriteTime"/> | Item last write time |
    /// | 544 | <see cref="Solid"/> | true if the item is solid; otherwise, false |
    /// | 548 | <see cref="Commented"/> | true if the item is commented; otherwise, false |
    /// | 552 | <see cref="Encrypted"/> | true if the item is encrypted; otherwise, false |
    /// | 556 | <see cref="SplitBefore"/> | (?) |
    /// | 560 | <see cref="SplitAfter"/> | (?) |
    /// | 564 | <see cref="DictionarySize"/> | Dictionary size(?) |
    /// | 568 | <see cref="Crc"/> | Item CRC checksum |
    /// | 572 | <see cref="Type"/> | Item type(?) |
    /// | 576 | <see cref="IsAnti"/> | (?) |
    /// | 580 | <see cref="Method"/> | Compression method |
    /// | 584 | <see cref="HostOS"/> | (?); usually absent |
    /// | 588 | <see cref="FileSystem"/> | Item file system; usually absent |
    /// | 592 | <see cref="User"/> | Item user(?); usually absent |
    /// | 596 | <see cref="Group"/> | Item group(?); usually absent |
    /// | 600 | <see cref="Block"/> | Bloack size(?) |
    /// | 604 | <see cref="Comment"/> | Item comment; usually absent |
    /// | 608 | <see cref="Position"/> | Item position |
    /// | 612 | <see cref="Prefix"/> | Item prefix(?) |
    /// | 616 | <see cref="NumSubDirs"/> | Number of subdirectories |
    /// | 620 | <see cref="NumSubFiles"/> | Numbers of subfiles |
    /// | 624 | <see cref="UnpackVersion"/> | The archive legacy unpacker version |
    /// | 628 | <see cref="Volume"/> | Volume(?) |
    /// | 632 | <see cref="IsVolume"/> | Is a volume |
    /// | 636 | <see cref="Offset"/> | Offset value(?) |
    /// | 640 | <see cref="Links"/> | Links(?) |
    /// | 644 | <see cref="NumBlocks"/> | Number of blocks |
    /// | 648 | <see cref="NumVolumes"/> | Number of volumes(?) |
    /// | 652 | <see cref="TimeType"/> | Time type(?) |
    /// | 656 | <see cref="Bit64"/> | 64-bit(?) |
    /// | 660 | <see cref="BigEndian"/> | BigEndian |
    /// | 664 | <see cref="Cpu"/> | Cpu(?) |
    /// | 668 | <see cref="PhysicalSize"/> | Physical archive size |
    /// | 672 | <see cref="HeadersSize"/> | Headers size |
    /// | 676 | <see cref="Checksum"/> | Archive checksum |
    /// | 677 | <see cref="Characts"/> | Represents characts. |
    /// | 678 | <see cref="Va"/> | Represents va. |
    /// | 679 | <see cref="Id"/> | Represents id. |
    /// | 680 | <see cref="ShortName"/> | Represents short Name. |
    /// | 681 | <see cref="CreatorApp"/> | Represents creator App. |
    /// | 682 | <see cref="SectorSize"/> | Represents sector Size. |
    /// | 683 | <see cref="PosixAttrib"/> | Represents posix Attrib. |
    /// | 684 | <see cref="SymLink"/> | Represents sym Link. |
    /// | 685 | <see cref="Error"/> | Represents error. |
    /// | 689 | <see cref="TotalSize"/> | (?) |
    /// | 693 | <see cref="FreeSpace"/> | (?) |
    /// | 697 | <see cref="ClusterSize"/> | Cluster size(?) |
    /// | 701 | <see cref="VolumeName"/> | Volume name(?) |
    /// | 705 | <see cref="LocalName"/> | Local item name(?); usually absent |
    /// | 709 | <see cref="Provider"/> | (?) |
    /// | 710 | <see cref="NtSecure"/> | Represents nt Secure. |
    /// | 711 | <see cref="IsAltStream"/> | Represents is Alt Stream. |
    /// | 712 | <see cref="IsAux"/> | Represents is Aux. |
    /// | 713 | <see cref="IsDeleted"/> | Represents is Deleted. |
    /// | 714 | <see cref="IsTree"/> | Represents is Tree. |
    /// | 715 | <see cref="Sha1"/> | Represents sha1. |
    /// | 716 | <see cref="Sha256"/> | Represents sha256. |
    /// | 717 | <see cref="ErrorType"/> | Represents error Type. |
    /// | 718 | <see cref="NumErrors"/> | Represents num Errors. |
    /// | 719 | <see cref="ErrorFlags"/> | Represents error Flags. |
    /// | 720 | <see cref="WarningFlags"/> | Represents warning Flags. |
    /// | 721 | <see cref="Warning"/> | Represents warning. |
    /// | 722 | <see cref="NumStreams"/> | Represents num Streams. |
    /// | 723 | <see cref="NumAltStreams"/> | Represents num Alt Streams. |
    /// | 724 | <see cref="AltStreamsSize"/> | Represents alt Streams Size. |
    /// | 725 | <see cref="VirtualSize"/> | Represents virtual Size. |
    /// | 726 | <see cref="UnpackSize"/> | Represents unpack Size. |
    /// | 727 | <see cref="TotalPhySize"/> | Represents total Phy Size. |
    /// | 731 | <see cref="VolumeIndex"/> | Index of the Volume |
    /// | 732 | <see cref="SubType"/> | Represents sub Type. |
    /// | 733 | <see cref="ShortComment"/> | Represents short Comment. |
    /// | 734 | <see cref="CodePage"/> | Represents code Page. |
    /// | 735 | <see cref="IsNotArcType"/> | Represents is Not Arc Type. |
    /// | 736 | <see cref="PhySizeCantBeDetected"/> | Represents phy Size Cant Be Detected. |
    /// | 737 | <see cref="ZerosTailIsAllowed"/> | Represents zeros Tail Is Allowed. |
    /// | 738 | <see cref="TailSize"/> | Represents tail Size. |
    /// | 739 | <see cref="EmbeddedStubSize"/> | Represents embedded Stub Size. |
    /// | 740 | <see cref="NtReparse"/> | Represents nt Reparse. |
    /// | 741 | <see cref="HardLink"/> | Represents hard Link. |
    /// | 742 | <see cref="INode"/> | Represents i Node. |
    /// | 743 | <see cref="StreamId"/> | Represents stream Id. |
    /// | 744 | <see cref="ReadOnly"/> | Represents read Only. |
    /// | 745 | <see cref="OutName"/> | Represents out Name. |
    /// | 746 | <see cref="CopyLink"/> | Represents copy Link. |
    /// | 747 | <see cref="NumDefined"/> | Represents num Defined. |
    /// | 751 | <see cref="UserDefined"/> | User defined property; usually absent |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 998d5092dbe6ae71bc83963cf257b26bec06309be76ede1b54f4899b180c6309
    /// </code>
    /// </example>
    internal enum ItemPropId : uint
    {
        /// <summary>
        /// No property
        /// </summary>
        NoProperty = 0,
        MainSubfile,
        /// <summary>
        /// Handler item index
        /// </summary>
        HandlerItemIndex,
        /// <summary>
        /// Item path
        /// </summary>
        Path,
        /// <summary>
        /// Item name
        /// </summary>
        Name,
        /// <summary>
        /// Item extension
        /// </summary>
        Extension,
        /// <summary>
        /// true if the item is a folder; otherwise, false
        /// </summary>
        IsDirectory,
        /// <summary>
        /// Item size
        /// </summary>
        Size,
        /// <summary>
        /// Item packed sise; usually absent
        /// </summary>
        PackedSize,
        /// <summary>
        /// Item attributes; usually absent
        /// </summary>
        Attributes,
        /// <summary>
        /// Item creation time; usually absent
        /// </summary>
        CreationTime,
        /// <summary>
        /// Item last access time; usually absent
        /// </summary>
        LastAccessTime,
        /// <summary>
        /// Item last write time
        /// </summary>
        LastWriteTime,
        /// <summary>
        /// true if the item is solid; otherwise, false
        /// </summary>
        Solid,
        /// <summary>
        /// true if the item is commented; otherwise, false
        /// </summary>
        Commented,
        /// <summary>
        /// true if the item is encrypted; otherwise, false
        /// </summary>
        Encrypted,
        /// <summary>
        /// (?)
        /// </summary>
        SplitBefore,
        /// <summary>
        /// (?)
        /// </summary>
        SplitAfter,
        /// <summary>
        /// Dictionary size(?)
        /// </summary>
        DictionarySize,
        /// <summary>
        /// Item CRC checksum
        /// </summary>
        Crc,
        /// <summary>
        /// Item type(?)
        /// </summary>
        Type,
        /// <summary>
        /// (?)
        /// </summary>
        IsAnti,
        /// <summary>
        /// Compression method
        /// </summary>
        Method,
        /// <summary>
        /// (?); usually absent
        /// </summary>
        HostOS,
        /// <summary>
        /// Item file system; usually absent
        /// </summary>
        FileSystem,
        /// <summary>
        /// Item user(?); usually absent
        /// </summary>
        User,
        /// <summary>
        /// Item group(?); usually absent
        /// </summary>
        Group,
        /// <summary>
        /// Bloack size(?)
        /// </summary>
        Block,
        /// <summary>
        /// Item comment; usually absent
        /// </summary>
        Comment,
        /// <summary>
        /// Item position
        /// </summary>
        Position,
        /// <summary>
        /// Item prefix(?)
        /// </summary>
        Prefix,
        /// <summary>
        /// Number of subdirectories
        /// </summary>
        NumSubDirs,
        /// <summary>
        /// Numbers of subfiles
        /// </summary>
        NumSubFiles,
        /// <summary>
        /// The archive legacy unpacker version
        /// </summary>
        UnpackVersion,
        /// <summary>
        /// Volume(?)
        /// </summary>
        Volume,
        /// <summary>
        /// Is a volume
        /// </summary>
        IsVolume,
        /// <summary>
        /// Offset value(?)
        /// </summary>
        Offset,
        /// <summary>
        /// Links(?)
        /// </summary>
        Links,
        /// <summary>
        /// Number of blocks
        /// </summary>
        NumBlocks,
        /// <summary>
        /// Number of volumes(?)
        /// </summary>
        NumVolumes,
        /// <summary>
        /// Time type(?)
        /// </summary>
        TimeType,
        /// <summary>
        /// 64-bit(?)
        /// </summary>
        Bit64,
        /// <summary>
        /// BigEndian
        /// </summary>
        BigEndian,
        /// <summary>
        /// Cpu(?)
        /// </summary>
        Cpu,
        /// <summary>
        /// Physical archive size
        /// </summary>
        PhysicalSize,
        /// <summary>
        /// Headers size
        /// </summary>
        HeadersSize,
        /// <summary>
        /// Archive checksum
        /// </summary>
        Checksum,
        Characts,
        Va,
        Id,
        ShortName,
        CreatorApp,
        SectorSize,
        PosixAttrib,
        SymLink,
        Error,
        /// <summary>
        /// (?)
        /// </summary>
        TotalSize,
        /// <summary>
        /// (?)
        /// </summary>
        FreeSpace,
        /// <summary>
        /// Cluster size(?)
        /// </summary>
        ClusterSize,
        /// <summary>
        /// Volume name(?)
        /// </summary>
        VolumeName,
        /// <summary>
        /// Local item name(?); usually absent
        /// </summary>
        LocalName,
        /// <summary>
        /// (?)
        /// </summary>
        Provider,
        NtSecure,
        IsAltStream,
        IsAux,
        IsDeleted,
        IsTree,
        Sha1,
        Sha256,
        ErrorType,
        NumErrors,
        ErrorFlags,
        WarningFlags,
        Warning,
        NumStreams,
        NumAltStreams,
        AltStreamsSize,
        VirtualSize,
        UnpackSize,
        TotalPhySize,
        /// <summary>
        /// Index of the Volume
        /// </summary>
        VolumeIndex,
        SubType,
        ShortComment,
        CodePage,
        IsNotArcType,
        PhySizeCantBeDetected,
        ZerosTailIsAllowed,
        TailSize,
        EmbeddedStubSize,
        NtReparse,
        HardLink,
        INode,
        StreamId,
        ReadOnly,
        OutName,
        CopyLink,
        NumDefined,
        /// <summary>
        /// User defined property; usually absent
        /// </summary>
        UserDefined = 0x10000
    }

    /// <summary>
    /// PropId string names dictionary wrapper.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 777 | <see cref="PropIdNames"/> | PropId string names |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 2363c12b359c941263c2c50cb81027417b524f67199981af60ec6cee6f771f11
    /// </code>
    /// </example>
    internal static class PropIdToName
    {
        /// <summary>
        /// PropId string names
        /// </summary>
        public static readonly Dictionary<ItemPropId, string> PropIdNames =
#region Initialization
            new Dictionary<ItemPropId, string>(46)
            {
                {ItemPropId.Path, "Path"},
                {ItemPropId.Name, "Name"},
                {ItemPropId.IsDirectory, "Folder"},
                {ItemPropId.Size, "Size"},
                {ItemPropId.PackedSize, "Packed Size"},
                {ItemPropId.Attributes, "Attributes"},
                {ItemPropId.CreationTime, "Created"},
                {ItemPropId.LastAccessTime, "Accessed"},
                {ItemPropId.LastWriteTime, "Modified"},
                {ItemPropId.Solid, "Solid"},
                {ItemPropId.Commented, "Commented"},
                {ItemPropId.Encrypted, "Encrypted"},
                {ItemPropId.SplitBefore, "Split Before"},
                {ItemPropId.SplitAfter, "Split After"},
                {
                    ItemPropId.DictionarySize,
                    "Dictionary Size"
                    },
                {ItemPropId.Crc, "CRC"},
                {ItemPropId.Type, "Type"},
                {ItemPropId.IsAnti, "Anti"},
                {ItemPropId.Method, "Method"},
                {ItemPropId.HostOS, "Host OS"},
                {ItemPropId.FileSystem, "File System"},
                {ItemPropId.User, "User"},
                {ItemPropId.Group, "Group"},
                {ItemPropId.Block, "Block"},
                {ItemPropId.Comment, "Comment"},
                {ItemPropId.Position, "Position"},
                {ItemPropId.Prefix, "Prefix"},
                {
                    ItemPropId.NumSubDirs,
                    "Number of subdirectories"
                    },
                {
                    ItemPropId.NumSubFiles,
                    "Number of subfiles"
                    },
                {
                    ItemPropId.UnpackVersion,
                    "Unpacker version"
                    },
                {ItemPropId.VolumeIndex, "VolumeIndex"},
                {ItemPropId.Volume, "Volume"},
                {ItemPropId.IsVolume, "IsVolume"},
                {ItemPropId.Offset, "Offset"},
                {ItemPropId.Links, "Links"},
                {
                    ItemPropId.NumBlocks,
                    "Number of blocks"
                    },
                {
                    ItemPropId.NumVolumes,
                    "Number of volumes"
                    },
                {ItemPropId.TimeType, "Time type"},
                {ItemPropId.Bit64, "64-bit"},
                {ItemPropId.BigEndian, "Big endian"},
                {ItemPropId.Cpu, "CPU"},
                {
                    ItemPropId.PhysicalSize,
                    "Physical Size"
                    },
                {ItemPropId.HeadersSize, "Headers Size"},
                {ItemPropId.Checksum, "Checksum"},
                {ItemPropId.FreeSpace, "Free Space"},
                {ItemPropId.ClusterSize, "Cluster Size"}
            };
#endregion
    }

    /// <summary>
    /// 7-zip IArchiveOpenCallback imported interface to handle the opening of an archive.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 883 | <see cref="SetTotal"/> | Sets total data size |
    /// | 892 | <see cref="SetCompleted"/> | Sets completed size |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 09663152b003e75821ec72330ce192025c8f023284b44cf53c960b9717dd045e
    /// </code>
    /// </example>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000600100000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IArchiveOpenCallback
    {
        // ref ulong replaced with IntPtr because handlers often pass null value
        // read actual value with Marshal.ReadInt64
        /// <summary>
        /// Sets total data size
        /// </summary>
        /// <param name="files">Files pointer</param>
        /// <param name="bytes">Total size in bytes</param>
        void SetTotal(
            IntPtr files,
            IntPtr bytes);

        /// <summary>
        /// Sets completed size
        /// </summary>
        /// <param name="files">Files pointer</param>
        /// <param name="bytes">Completed size in bytes</param>
        void SetCompleted(
            IntPtr files,
            IntPtr bytes);
    }

    /// <summary>
    /// 7-zip ICryptoGetTextPassword imported interface to get the archive password.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 925 | <see cref="CryptoGetTextPassword"/> | Gets password for the archive |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: c692a223a2afca33c0cd882c03f0fd060fed05c5eb870807c44dcc2da8576032
    /// </code>
    /// </example>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000500100000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICryptoGetTextPassword
    {
        /// <summary>
        /// Gets password for the archive
        /// </summary>
        /// <param name="password">Password for the archive</param>
        /// <returns>Zero if everything is OK</returns>
        [PreserveSig]
        int CryptoGetTextPassword(
            [MarshalAs(UnmanagedType.BStr)] out string password);
    }

    /// <summary>
    /// 7-zip ICryptoGetTextPassword2 imported interface for setting the archive password.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 959 | <see cref="CryptoGetTextPassword2"/> | Sets password for the archive |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 2d7dfabdbd1c470dbb6d4e7f57202555b49d67424d4fbd5fc7f3491fe727e0a0
    /// </code>
    /// </example>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000500110000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICryptoGetTextPassword2
    {
        /// <summary>
        /// Sets password for the archive
        /// </summary>
        /// <param name="passwordIsDefined">Specifies whether archive has a password or not (0 if not)</param>
        /// <param name="password">Password for the archive</param>
        /// <returns>Zero if everything is OK</returns>
        [PreserveSig]
        int CryptoGetTextPassword2(
            ref int passwordIsDefined,
            [MarshalAs(UnmanagedType.BStr)] out string password);
    }

    /// <summary>
    /// 7-zip IArchiveExtractCallback imported interface.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 996 | <see cref="SetTotal"/> | Gives the size of the unpacked archive files |
    /// | 1002 | <see cref="SetCompleted"/> | SetCompleted 7-zip function |
    /// | 1011 | <see cref="GetStream"/> | Gets the stream for file extraction |
    /// | 1021 | <see cref="PrepareOperation"/> | PrepareOperation 7-zip function |
    /// | 1027 | <see cref="SetOperationResult"/> | Sets the operation result |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 6af4f12e843fb6c26a14d3903976208bad540e81e5383f2c2666ce243cf51456
    /// </code>
    /// </example>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000600200000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IArchiveExtractCallback
    {
        /// <summary>
        /// Gives the size of the unpacked archive files
        /// </summary>
        /// <param name="total">Size of the unpacked archive files (in bytes)</param>
        void SetTotal(ulong total);

        /// <summary>
        /// SetCompleted 7-zip function
        /// </summary>
        /// <param name="completeValue"></param>
        void SetCompleted([In] ref ulong completeValue);

        /// <summary>
        /// Gets the stream for file extraction
        /// </summary>
        /// <param name="index">File index in the archive file table</param>
        /// <param name="outStream">Pointer to the stream</param>
        /// <param name="askExtractMode">Extraction mode</param>
        /// <returns>S_OK - OK, S_FALSE - skip this file</returns>
        [PreserveSig]
        int GetStream(
            uint index,
            [Out, MarshalAs(UnmanagedType.Interface)] out ISequentialOutStream outStream,
            AskMode askExtractMode);

        /// <summary>
        /// PrepareOperation 7-zip function
        /// </summary>
        /// <param name="askExtractMode">Ask mode</param>
        void PrepareOperation(AskMode askExtractMode);

        /// <summary>
        /// Sets the operation result
        /// </summary>
        /// <param name="operationResult">The operation result</param>
        void SetOperationResult(OperationResult operationResult);
    }

    /// <summary>
    /// 7-zip IArchiveUpdateCallback imported interface.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 1063 | <see cref="SetTotal"/> | Gives the size of the unpacked archive files. |
    /// | 1069 | <see cref="SetCompleted"/> | SetCompleted 7-zip internal function. |
    /// | 1079 | <see cref="GetUpdateItemInfo"/> | Gets archive update mode. |
    /// | 1091 | <see cref="GetProperty"/> | Gets the archive item property data. |
    /// | 1100 | <see cref="GetStream"/> | Gets the stream for reading. |
    /// | 1109 | <see cref="SetOperationResult"/> | Sets the result for currently performed operation. |
    /// | 1116 | <see cref="EnumProperties"/> | EnumProperties 7-zip internal function. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: d4a2ed897bbe0c304a8ebc7428e8a87f221fe219b8d29964b975a3f75e24a9e8
    /// </code>
    /// </example>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000600800000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IArchiveUpdateCallback
    {
        /// <summary>
        /// Gives the size of the unpacked archive files.
        /// </summary>
        /// <param name="total">Size of the unpacked archive files (in bytes)</param>
        void SetTotal(ulong total);

        /// <summary>
        /// SetCompleted 7-zip internal function.
        /// </summary>
        /// <param name="completeValue"></param>
        void SetCompleted([In] ref ulong completeValue);

        /// <summary>
        /// Gets archive update mode.
        /// </summary>
        /// <param name="index">File index</param>
        /// <param name="newData">1 if new, 0 if not</param>
        /// <param name="newProperties">1 if new, 0 if not</param>
        /// <param name="indexInArchive">-1 if doesn't matter</param>
        /// <returns></returns>
        [PreserveSig]
        int GetUpdateItemInfo(
            uint index, ref int newData,
            ref int newProperties, ref uint indexInArchive);

        /// <summary>
        /// Gets the archive item property data.
        /// </summary>
        /// <param name="index">Item index</param>
        /// <param name="propId">Property identifier</param>
        /// <param name="value">Property value</param>
        /// <returns>Zero if Ok</returns>
        [PreserveSig]
        int GetProperty(uint index, ItemPropId propId, ref PropVariant value);

        /// <summary>
        /// Gets the stream for reading.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <param name="inStream">The ISequentialInStream pointer for reading.</param>
        /// <returns>Zero if Ok</returns>
        [PreserveSig]
        int GetStream(
            uint index,
            [Out, MarshalAs(UnmanagedType.Interface)] out ISequentialInStream inStream);

        /// <summary>
        /// Sets the result for currently performed operation.
        /// </summary>
        /// <param name="operationResult">The result value.</param>
        void SetOperationResult(OperationResult operationResult);

        /// <summary>
        /// EnumProperties 7-zip internal function.
        /// </summary>
        /// <param name="enumerator">The enumerator pointer.</param>
        /// <returns></returns>
        long EnumProperties(IntPtr enumerator);
    }

    /// <summary>
    /// 7-zip IArchiveOpenVolumeCallback imported interface to handle archive volumes.
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 1148 | <see cref="GetProperty"/> | Gets the archive property data. |
    /// | 1158 | <see cref="GetStream"/> | Gets the stream for reading the volume. |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 21b77267fb0ebbd473d6a8adb91d29912eac58adc99c8622a13f6660146cbb17
    /// </code>
    /// </example>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000600300000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IArchiveOpenVolumeCallback
    {
        /// <summary>
        /// Gets the archive property data.
        /// </summary>
        /// <param name="propId">The property identificator.</param>
        /// <param name="value">The property value.</param>
        [PreserveSig]
        int GetProperty(
            ItemPropId propId, ref PropVariant value);

        /// <summary>
        /// Gets the stream for reading the volume.
        /// </summary>
        /// <param name="name">The volume file name.</param>
        /// <param name="inStream">The IInStream pointer for reading.</param>
        /// <returns>Zero if Ok</returns>
        [PreserveSig]
        int GetStream(
            [MarshalAs(UnmanagedType.LPWStr)] string name,
            [Out, MarshalAs(UnmanagedType.Interface)] out IInStream inStream);
    }    

    /// <summary>
    /// 7-zip ISequentialInStream imported interface
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 1198 | <see cref="Read"/> | Writes data to 7-zip packer |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: b7acdb467fb838301eb51e6aea406a5d84fc10c4646081f4332450ce81afb4f0
    /// </code>
    /// </example>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000300010000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISequentialInStream
    {
        /// <summary>
        /// Writes data to 7-zip packer
        /// </summary>
        /// <param name="data">Array of bytes available for writing</param>
        /// <param name="size">Array size</param>
        /// <returns>S_OK if success</returns>
        /// <remarks>If (size > 0) and there are bytes in stream, 
        /// this function must read at least 1 byte.
        /// This function is allowed to read less than "size" bytes.
        /// You must call Read function in loop, if you need exact amount of data.
        /// </remarks>
        int Read(
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] data,
            uint size);
    }

    /// <summary>
    /// 7-zip ISequentialOutStream imported interface
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 1240 | <see cref="Write"/> | Writes data to unpacked file stream |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 37e050224eeb88b6191fa17dc17bde22625040f74fd54aecdad22fc0b051acc2
    /// </code>
    /// </example>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000300020000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISequentialOutStream
    {
        /// <summary>
        /// Writes data to unpacked file stream
        /// </summary>
        /// <param name="data">Array of bytes available for reading</param>
        /// <param name="size">Array size</param>
        /// <param name="processedSize">Processed data size</param>
        /// <returns>S_OK if success</returns>
        /// <remarks>If size != 0, return value is S_OK and (*processedSize == 0),
        ///  then there are no more bytes in stream.
        /// If (size > 0) and there are bytes in stream, 
        /// this function must read at least 1 byte.
        /// This function is allowed to rwrite less than "size" bytes.
        /// You must call Write function in loop, if you need exact amount of data.
        /// </remarks>
        [PreserveSig]
        int Write(
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] data,
            uint size, IntPtr processedSize);
    }

    /// <summary>
    /// 7-zip IInStream imported interface
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 1276 | <see cref="Read"/> | Read routine |
    /// | 1286 | <see cref="Seek"/> | Seek routine |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 9fb750f3ef704d1b3a43d36baa7eb9909a3a76311a1d08cae2a6f599d6059292
    /// </code>
    /// </example>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000300030000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IInStream
    {
        /// <summary>
        /// Read routine
        /// </summary>
        /// <param name="data">Array of bytes to set</param>
        /// <param name="size">Array size</param>
        /// <returns>Zero if Ok</returns>
        int Read(
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] data,
            uint size);

        /// <summary>
        /// Seek routine
        /// </summary>
        /// <param name="offset">Offset value</param>
        /// <param name="seekOrigin">Seek origin value</param>
        /// <param name="newPosition">New position pointer</param>
        void Seek(
            long offset, SeekOrigin seekOrigin, IntPtr newPosition);
    }

    /// <summary>
    /// 7-zip IOutStream imported interface
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 1322 | <see cref="Write"/> | Write routine |
    /// | 1334 | <see cref="Seek"/> | Seek routine |
    /// | 1342 | <see cref="SetSize"/> | Set size routine |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 6f1af2255006e983f3e24977f24d0cc1e4f98512e33f6f1ecbdaf4fd378e876a
    /// </code>
    /// </example>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000300040000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IOutStream
    {
        /// <summary>
        /// Write routine
        /// </summary>
        /// <param name="data">Array of bytes to get</param>
        /// <param name="size">Array size</param>
        /// <param name="processedSize">Processed size</param>
        /// <returns>Zero if Ok</returns>
        [PreserveSig]
        int Write(
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] data,
            uint size,
            IntPtr processedSize);

        /// <summary>
        /// Seek routine
        /// </summary>
        /// <param name="offset">Offset value</param>
        /// <param name="seekOrigin">Seek origin value</param>
        /// <param name="newPosition">New position pointer</param>       
        void Seek(
            long offset, SeekOrigin seekOrigin, IntPtr newPosition);

        /// <summary>
        /// Set size routine
        /// </summary>
        /// <param name="newSize">New size value</param>
        /// <returns>Zero if Ok</returns>
        [PreserveSig]
        int SetSize(long newSize);
    }

    /// <summary>
    /// 7-zip essential in archive interface
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 1385 | <see cref="Open"/> | Opens archive for reading. |
    /// | 1394 | <see cref="Close"/> | Closes the archive. |
    /// | 1400 | <see cref="GetNumberOfItems"/> | Gets the number of files in the archive file table. |
    /// | 1408 | <see cref="GetProperty"/> | Retrieves specific property data. |
    /// | 1421 | <see cref="Extract"/> | Extracts files from the opened archive. |
    /// | 1433 | <see cref="GetArchiveProperty"/> | Gets archive property data |
    /// | 1441 | <see cref="GetNumberOfProperties"/> | Gets the number of properties |
    /// | 1450 | <see cref="GetPropertyInfo"/> | Gets property information |
    /// | 1460 | <see cref="GetNumberOfArchiveProperties"/> | Gets the number of archive properties |
    /// | 1469 | <see cref="GetArchivePropertyInfo"/> | Gets the archive property information |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: dca52381e5fde3ab3b6f0cb5494f6541a0843c701f30c6c2819ec21075027af0
    /// </code>
    /// </example>
    [ComImport]  
	[Guid("23170F69-40C1-278A-0000-000600600000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]	
    internal interface IInArchive
    {
        /// <summary>
        /// Opens archive for reading.
        /// </summary>
        /// <param name="stream">Archive file stream</param>
        /// <param name="maxCheckStartPosition">Maximum start position for checking</param>
        /// <param name="openArchiveCallback">Callback for opening archive</param>
        /// <returns></returns>
        [PreserveSig]
        int Open(
            IInStream stream,
            [In] ref ulong maxCheckStartPosition,
            [MarshalAs(UnmanagedType.Interface)] IArchiveOpenCallback openArchiveCallback);

        /// <summary>
        /// Closes the archive.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets the number of files in the archive file table  .          
        /// </summary>
        /// <returns>The number of files in the archive</returns>
        uint GetNumberOfItems();

        /// <summary>
        /// Retrieves specific property data.
        /// </summary>
        /// <param name="index">File index in the archive file table</param>
        /// <param name="propId">Property code</param>
        /// <param name="value">Property variant value</param>
        void GetProperty(
            uint index,
            ItemPropId propId,
            ref PropVariant value); // PropVariant

        /// <summary>
        /// Extracts files from the opened archive.
        /// </summary>
        /// <param name="indexes">indexes of files to be extracted (must be sorted)</param>
        /// <param name="numItems">0xFFFFFFFF means all files</param>
        /// <param name="testMode">testMode != 0 means "test files operation"</param>
        /// <param name="extractCallback">IArchiveExtractCallback for operations handling</param>
        /// <returns>0 if success</returns>
        [PreserveSig]
        int Extract(
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] uint[] indexes,
            uint numItems,
            int testMode,
            [MarshalAs(UnmanagedType.Interface)] IArchiveExtractCallback extractCallback);

        /// <summary>
        /// Gets archive property data
        /// </summary>
        /// <param name="propId">Archive property identificator</param>
        /// <param name="value">Archive property value</param>
        void GetArchiveProperty(
            ItemPropId propId, // PROPID
            ref PropVariant value); // PropVariant

        /// <summary>
        /// Gets the number of properties
        /// </summary>
        /// <returns>The number of properties</returns>
        uint GetNumberOfProperties();

        /// <summary>
        /// Gets property information
        /// </summary>
        /// <param name="index">Item index</param>
        /// <param name="name">Name</param>
        /// <param name="propId">Property identifier</param>
        /// <param name="varType">Variant type</param>
        void GetPropertyInfo(
            uint index,
            [MarshalAs(UnmanagedType.BStr)] out string name,
            out ItemPropId propId, // PROPID
            out ushort varType); //VARTYPE

        /// <summary>
        /// Gets the number of archive properties
        /// </summary>
        /// <returns>The number of archive properties</returns>
        uint GetNumberOfArchiveProperties();

        /// <summary>
        /// Gets the archive property information
        /// </summary>
        /// <param name="index">Item index</param>
        /// <param name="name">Name</param>
        /// <param name="propId">Property identifier</param>
        /// <param name="varType">Variant type</param>
        void GetArchivePropertyInfo(
            uint index,
            [MarshalAs(UnmanagedType.BStr)] out string name,
            out ItemPropId propId, // PROPID
            out ushort varType); //VARTYPE
    }

    /// <summary>
    /// 7-zip essential out archive interface
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 1507 | <see cref="UpdateItems"/> | Updates archive items |
    /// | 1517 | <see cref="GetFileTimeType"/> | Gets file time type(?) |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: 2d0cdbb99d4bd221e821f1f3116e6609d0929736220cc5f21ed51abfa89a7c2e
    /// </code>
    /// </example>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000600A00000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IOutArchive
    {
        /// <summary>
        /// Updates archive items
        /// </summary>
        /// <param name="outStream">The ISequentialOutStream pointer for writing the archive data</param>
        /// <param name="numItems">Number of archive items</param>
        /// <param name="updateCallback">The IArchiveUpdateCallback pointer</param>
        /// <returns>Zero if Ok</returns>
        [PreserveSig]
        int UpdateItems(
            [MarshalAs(UnmanagedType.Interface)] ISequentialOutStream outStream,
            uint numItems,
            [MarshalAs(UnmanagedType.Interface)] IArchiveUpdateCallback updateCallback);

        /// <summary>
        /// Gets file time type(?)
        /// </summary>
        /// <param name="type">Type pointer</param>
        void GetFileTimeType(IntPtr type);
    }

    /// <summary>
    /// 7-zip ISetProperties interface for setting various archive properties
    /// </summary>
    /// <remarks>
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 1550 | <see cref="SetProperties"/> | Sets the archive properties |
    /// </remarks>
    ///
    /// <example>
    /// <code language="yaml">
    /// pass: 2
    /// mtime: 2026-08-21T19:22:58Z
    /// digest: ff1086471da9051d23656fe3757e65ec8aa2a21b988a865642a7c6834bdf23e1
    /// </code>
    /// </example>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000600030000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISetProperties
    {
        /// <summary>
        /// Sets the archive properties
        /// </summary>
        /// <param name="names">The names of the properties</param>
        /// <param name="values">The values of the properties</param>
        /// <param name="numProperties">The properties count</param>
        /// <returns></returns>        
        int SetProperties(IntPtr names, IntPtr values, int numProperties);
    }
#endif
            }
