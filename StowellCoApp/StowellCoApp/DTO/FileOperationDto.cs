using System;
using System.Collections.Generic;

namespace StowellCoApp.DTO
{
    /// <summary>
    /// Neutral DTO for file/folder information independent of UI framework.
    /// Designed to be used by both API and Blazor components.
    /// </summary>
    public class FileDirectoryContent
    {
        /// <summary>
        /// Unique identifier for the file or folder
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the file or folder
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Virtual path to the file or folder
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Size of the file in bytes. 0 for folders.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// True if this is a file, false if it's a folder
        /// </summary>
        public bool IsFile { get; set; }

        /// <summary>
        /// File extension for files (e.g., ".txt", ".pdf")
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// True if the folder has child items
        /// </summary>
        public bool HasChild { get; set; }

        /// <summary>
        /// Date when the file or folder was created
        /// </summary>
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Date when the file or folder was last modified
        /// </summary>
        public DateTime? DateModified { get; set; }

        /// <summary>
        /// Physical filter path (typically the root folder path)
        /// </summary>
        public string FilterPath { get; set; }
    }

    /// <summary>
    /// Details information about a file or folder
    /// </summary>
    public class FileDetails
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public bool IsFile { get; set; }
        public string Size { get; set; }
        public string Modified { get; set; }
        public string Created { get; set; }
    }

    /// <summary>
    /// Error information for file operations
    /// </summary>
    public class ErrorDetails
    {
        public string Message { get; set; }
        public string Code { get; set; }
    }
}
