using System;
using Tigerspike.Solv.Chat.Enums;

namespace Tigerspike.Solv.Chat.Domain
{
    public class MessageFile
    {
        public string BucketName { get; set; }

        public string Key { get; set; }

        public string FileName { get; set; }

        public DateTime UpdatedDate { get; set; }

        public long Size { get; set; }

        public string ContentType { get; set; }

        public int FileType => ContentType.StartsWith("image/") ? (int) MessageFileType.Image : (int) MessageFileType.File;
    }
}