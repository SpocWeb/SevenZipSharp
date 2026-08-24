using System.ComponentModel;
using org.SpocWeb.root.Attributes;
namespace SevenZip
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides asynchronous archive extraction methods for reading files from 7-Zip archives to<br/>
    /// the file system or output streams.
    /// </summary>
    /// <remarks>
    /// ## Collaborators
    ///
    /// | Type | Relationship |
    /// |---|---|
    /// | <see cref="ExtractFileCallback"/> | Callback invoked for each archive member during callback-driven extraction. |
    ///
    /// ## Public Methods
    ///
    /// | Line | Method | Description |
    /// |--:|---|---|
    /// | 129 | <see cref="BeginExtractArchive"/> | Unpacks the whole archive asynchronously to the specified directory name at the specified priority. |
    /// | 141 | <see cref="ExtractArchiveAsync"/> | Unpacks the whole archive asynchronously to the specified directory name at the specified priority. |
    /// | 160 | <see cref="BeginExtractFile"/> | Unpacks the file asynchronously by its name to the specified stream. |
    /// | 173 | <see cref="ExtractFileAsync"/> | Unpacks the file asynchronously by its name to the specified stream. |
    /// | 289 | <see cref="BeginExtractFiles"/> | Extracts files from the archive asynchronously, giving a callback the choice what to do with each file. |
    /// | 303 | <see cref="ExtractFilesAsync"/> | Extracts files from the archive asynchronously, giving a callback the choice what to do with each file. |
    /// </remarks>
    /// <seealso cref="ExtractFileCallback">ExtractFileCallback: Callback invoked for each archive member during callback-driven extraction.</seealso>
    [DocState(Pass = 2, MTime = "2026-08-24T14:23:33Z", Digest = "7d2cdcccb7dadd2da9a8055bba8d94331a13ba54d26c520a70ae7f1cb6468524", Stale = false, Path = "SevenZipExtractorAsynchronous.cs", Since = "2026-08-23")]
    partial class SevenZipExtractor
    {
        #region Asynchronous core methods

        /// <summary>
        /// Recreates the instance of the SevenZipExtractor class.
        /// Used in asynchronous methods.
        /// </summary>
        [System.ComponentModel.Description("Recreates the instance of the SevenZipExtractor class.")]
        private void RecreateInstanceIfNeeded()
        {
            if (NeedsToBeRecreated)
            {
                NeedsToBeRecreated = false;
                Stream backupStream = null;
                string backupFileName = null;
                if (String.IsNullOrEmpty(_fileName))
                {
                    backupStream = _inStream;
                }
                else
                {
                    backupFileName = _fileName;
                }
                CommonDispose();
                if (backupStream == null)
                {
                    Init(backupFileName);
                }
                else
                {
                    Init(backupStream);
                }
            }
        }

        /// <inheritdoc />
        internal override void SaveContext()
        {
            DisposedCheck();
            _asynchronousDisposeLock = true;
            base.SaveContext();
        }

        /// <inheritdoc />
        internal override void ReleaseContext()
        {
            base.ReleaseContext();
            _asynchronousDisposeLock = false;
        }

        #endregion

        #region Delegates

        /// <summary>
        /// The delegate to use in BeginExtractArchive.
        /// </summary>
        /// <param name="directory">The directory where the files are to be unpacked.</param>
        private delegate void ExtractArchiveDelegate(string directory);

        /// <summary>
        /// The delegate to use in BeginExtractFile (by file name).
        /// </summary>
        /// <param name="fileName">The file full name in the archive file table.</param>
        /// <param name="stream">The stream where the file is to be unpacked.</param>
        private delegate void ExtractFileByFileNameDelegate(string fileName, Stream stream);

        /// <summary>
        /// The delegate to use in BeginExtractFile (by index).
        /// </summary>
        /// <param name="index">Index in the archive file table.</param>
        /// <param name="stream">The stream where the file is to be unpacked.</param>
        private delegate void ExtractFileByIndexDelegate(int index, Stream stream);

        /// <summary>
        /// The delegate to use in BeginExtractFiles(string directory, params int[] indexes).
        /// </summary>
        /// <param name="indexes">indexes of the files in the archive file table.</param>
        /// <param name="directory">Directory where the files are to be unpacked.</param>
        private delegate void ExtractFiles1Delegate(string directory, int[] indexes);

        /// <summary>
        /// The delegate to use in BeginExtractFiles(string directory, params string[] fileNames).
        /// </summary>
        /// <param name="fileNames">Full file names in the archive file table.</param>
        /// <param name="directory">Directory where the files are to be unpacked.</param>
        private delegate void ExtractFiles2Delegate(string directory, string[] fileNames);

        /// <summary>
        /// The delegate to use in BeginExtractFiles(ExtractFileCallback extractFileCallback).
        /// </summary>
        /// <param name="extractFileCallback">The callback to call for each file in the archive.</param>
        private delegate void ExtractFiles3Delegate(ExtractFileCallback extractFileCallback);
        #endregion

        /// <summary>
        /// Unpacks the whole archive asynchronously to the specified directory name at the specified priority.
        /// </summary>
        /// <param name="directory">The directory where the files are to be unpacked.</param>
        [System.ComponentModel.Description("Unpacks the whole archive asynchronously to the specified directory name at the specified priority.")]
        public void BeginExtractArchive(string directory)
        {
            SaveContext();
            Task.Run(() => new ExtractArchiveDelegate(ExtractArchive).Invoke(directory))
                .ContinueWith(_ => ReleaseContext());
        }

        /// <summary>
        /// Unpacks the whole archive asynchronously to the specified directory name at the specified priority.
        /// </summary>
        /// <param name="directory">The directory where the files are to be unpacked.</param>
        [System.ComponentModel.Description("Unpacks the whole archive asynchronously to the specified directory name at the specified priority.")]
        public async Task ExtractArchiveAsync(string directory)
        {
            try
            {
                SaveContext();
                await Task.Run(() => new ExtractArchiveDelegate(ExtractArchive).Invoke(directory));
            }
            finally
            {
                ReleaseContext();
            }
        }

        /// <summary>
        /// Unpacks the file asynchronously by its name to the specified stream.
        /// </summary>
        /// <param name="fileName">The file full name in the archive file table.</param>
        /// <param name="stream">The stream where the file is to be unpacked.</param>
        [System.ComponentModel.Description("Unpacks the file asynchronously by its name to the specified stream.")]
        public void BeginExtractFile(string fileName, Stream stream)
        {
            SaveContext();
            Task.Run(() => new ExtractFileByFileNameDelegate(ExtractFile).Invoke(fileName, stream))
                .ContinueWith(_ => ReleaseContext());
        }

        /// <summary>
        /// Unpacks the file asynchronously by its name to the specified stream.
        /// </summary>
        /// <param name="fileName">The file full name in the archive file table.</param>
        /// <param name="stream">The stream where the file is to be unpacked.</param>
        [System.ComponentModel.Description("Unpacks the file asynchronously by its name to the specified stream.")]
        public async Task ExtractFileAsync(string fileName, Stream stream)
        {
            try
            {
                SaveContext();
                await Task.Run(() => new ExtractFileByFileNameDelegate(ExtractFile).Invoke(fileName, stream));
            }
            finally
            {
                ReleaseContext();
            }
        }

        /// <summary>
        /// Unpacks the file asynchronously by its index to the specified stream.
        /// </summary>
        /// <param name="index">Index in the archive file table.</param>
        /// <param name="stream">The stream where the file is to be unpacked.</param>
        [System.ComponentModel.Description("Unpacks the file asynchronously by its index to the specified stream.")]
        public void BeginExtractFile(int index, Stream stream)
        {
            SaveContext();
            Task.Run(() => new ExtractFileByIndexDelegate(ExtractFile).Invoke(index, stream))
                .ContinueWith(_ => ReleaseContext());
        }

        /// <summary>
        /// Unpacks the file asynchronously by its name to the specified stream.
        /// </summary>
        /// <param name="index">Index in the archive file table.</param>
        /// <param name="stream">The stream where the file is to be unpacked.</param>
        [System.ComponentModel.Description("Unpacks the file asynchronously by its name to the specified stream.")]
        public async Task ExtractFileAsync(int index, Stream stream)
        {
            try
            {
                SaveContext();
                await Task.Run(() => new ExtractFileByIndexDelegate(ExtractFile).Invoke(index, stream));
            }
            finally
            {
                ReleaseContext();
            }
        }

        /// <summary>
        /// Unpacks files asynchronously by their indices to the specified directory.
        /// </summary>
        /// <param name="indexes">indexes of the files in the archive file table.</param>
        /// <param name="directory">Directory where the files are to be unpacked.</param>
        [System.ComponentModel.Description("Unpacks files asynchronously by their indices to the specified directory.")]
        public void BeginExtractFiles(string directory, params int[] indexes)
        {
            SaveContext();
            Task.Run(() => new ExtractFiles1Delegate(ExtractFiles).Invoke(directory, indexes))
                .ContinueWith(_ => ReleaseContext());
        }

        /// <summary>
        /// Unpacks files asynchronously by their indices to the specified directory.
        /// </summary>
        /// <param name="indexes">indexes of the files in the archive file table.</param>
        /// <param name="directory">Directory where the files are to be unpacked.</param>
        [System.ComponentModel.Description("Unpacks files asynchronously by their indices to the specified directory.")]
        public async Task ExtractFilesAsync(string directory, params int[] indexes)
        {
            try
            {
                SaveContext();
                await Task.Run(() => new ExtractFiles1Delegate(ExtractFiles).Invoke(directory, indexes));
            }
            finally
            {
                ReleaseContext();
            }
        }

        /// <summary>
        /// Unpacks files asynchronously by their full names to the specified directory.
        /// </summary>
        /// <param name="fileNames">Full file names in the archive file table.</param>
        /// <param name="directory">Directory where the files are to be unpacked.</param>
        [System.ComponentModel.Description("Unpacks files asynchronously by their full names to the specified directory.")]
        public void BeginExtractFiles(string directory, params string[] fileNames)
        {
            SaveContext();
            Task.Run(() => new ExtractFiles2Delegate(ExtractFiles).Invoke(directory, fileNames))
                .ContinueWith(_ => ReleaseContext());
        }

        /// <summary>
        /// Unpacks files asynchronously by their full names to the specified directory.
        /// </summary>
        /// <param name="fileNames">Full file names in the archive file table.</param>
        /// <param name="directory">Directory where the files are to be unpacked.</param>
        [System.ComponentModel.Description("Unpacks files asynchronously by their full names to the specified directory.")]
        public async Task ExtractFilesAsync(string directory, params string[] fileNames)
        {
            try
            {
                SaveContext();
                await Task.Run(() => new ExtractFiles2Delegate(ExtractFiles).Invoke(directory, fileNames));
            }
            finally
            {
                ReleaseContext();
            }
        }

        /// <summary>
        /// Extracts files from the archive asynchronously, giving a callback the choice what
        /// to do with each file. The order of the files is given by the archive.
        /// 7-Zip (and any other solid) archives are NOT supported.
        /// </summary>
        /// <param name="extractFileCallback">The callback to call for each file in the archive.</param>
        [System.ComponentModel.Description("Extracts files from the archive asynchronously, giving a callback the choice what to do with each file.")]
        public void BeginExtractFiles(ExtractFileCallback extractFileCallback)
        {
            SaveContext();
            Task.Run(() => new ExtractFiles3Delegate(ExtractFiles).Invoke(extractFileCallback))
                .ContinueWith(_ => ReleaseContext());
        }

        /// <summary>
        /// Extracts files from the archive asynchronously, giving a callback the choice what
        /// to do with each file. The order of the files is given by the archive.
        /// 7-Zip (and any other solid) archives are NOT supported.
        /// </summary>
        /// <param name="extractFileCallback">The callback to call for each file in the archive.</param>
        [System.ComponentModel.Description("Extracts files from the archive asynchronously, giving a callback the choice what to do with each file.")]
        public async Task ExtractFilesAsync(ExtractFileCallback extractFileCallback)
        {
            try
            {
                SaveContext();
                await Task.Run(() => new ExtractFiles3Delegate(ExtractFiles).Invoke(extractFileCallback));
            }
            finally
            {
                ReleaseContext();
            }
        }
    }
}
