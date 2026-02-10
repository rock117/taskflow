using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Text;

namespace TaskFlow.Web.Core;

/// <summary>
/// 文件工具类
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// 允许的文件扩展名
    /// </summary>
    public static readonly string[] AllowedExtensions = new[]
    {
        // 图片
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".webp",
        // 文档
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
        // 文本
        ".txt", ".csv", ".html", ".css", ".js", ".json", ".xml",
        // 压缩包
        ".zip", ".rar", ".7z", ".tar", ".gz",
        // 视频
        ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".webm",
        // 音频
        ".mp3", ".wav", ".flac", ".aac", ".ogg"
    };

    /// <summary>
    /// 允许的 MIME 类型
    /// </summary>
    public static readonly string[] AllowedMimeTypes = new[]
    {
        // 图片
        "image/jpeg", "image/jpg", "image/png", "image/gif", "image/svg+xml", "image/webp",
        // 文档
        "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-powerpoint", "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        // 文本
        "text/plain", "text/csv", "text/html", "text/css", "text/javascript", "application/json", "application/xml",
        // 压缩包
        "application/zip", "application/x-rar-compressed", "application/x-7z-compressed", "application/x-tar", "application/gzip",
        // 视频
        "video/mp4", "video/mpeg", "video/quicktime", "video/x-msvideo", "video/webm",
        // 音频
        "audio/mpeg", "audio/wav", "audio/webm", "audio/ogg"
    };

    /// <summary>
    /// 图片 MIME 类型
    /// </summary>
    public static readonly string[] ImageMimeTypes = new[]
    {
        "image/jpeg", "image/jpg", "image/png", "image/gif", "image/svg+xml", "image/webp"
    };

    /// <summary>
    /// 文档 MIME 类型
    /// </summary>
    public static readonly string[] DocumentMimeTypes = new[]
    {
        "application/pdf", "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation"
    };

    /// <summary>
    /// 验证文件扩展名
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <returns>是否有效</returns>
    public static bool IsAllowedExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return false;

        var extension = Path.GetExtension(fileName).ToLower();
        return AllowedExtensions.Contains(extension);
    }

    /// <summary>
    /// 验证 MIME 类型
    /// </summary>
    /// <param name="mimeType">MIME 类型</param>
    /// <returns>是否有效</returns>
    public static bool IsAllowedMimeType(string mimeType)
    {
        if (string.IsNullOrEmpty(mimeType))
            return false;

        return AllowedMimeTypes.Contains(mimeType.ToLower());
    }

    /// <summary>
    /// 获取文件分类
    /// </summary>
    /// <param name="mimeType">MIME 类型</param>
    /// <param name="extension">文件扩展名</param>
    /// <returns>文件分类</returns>
    public static string GetFileCategory(string? mimeType, string? extension)
    {
        if (ImageMimeTypes.Contains(mimeType))
            return "image";

        if (DocumentMimeTypes.Contains(mimeType))
            return "document";

        if (!string.IsNullOrEmpty(mimeType))
        {
            if (mimeType.StartsWith("video/"))
                return "video";
            if (mimeType.StartsWith("audio/"))
                return "audio";
            if (mimeType.Contains("zip") || mimeType.Contains("rar") || mimeType.Contains("compressed"))
                return "archive";
        }

        if (!string.IsNullOrEmpty(extension))
        {
            var ext = extension.ToLower();
            if (new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".webp" }.Contains(ext))
                return "image";
            if (new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx" }.Contains(ext))
                return "document";
            if (new[] { ".zip", ".rar", ".7z", ".tar", ".gz" }.Contains(ext))
                return "archive";
            if (new[] { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".webm" }.Contains(ext))
                return "video";
            if (new[] { ".mp3", ".wav", ".flac", ".aac", ".ogg" }.Contains(ext))
                return "audio";
        }

        return "other";
    }

    /// <summary>
    /// 生成唯一文件名
    /// </summary>
    /// <param name="originalFileName">原始文件名</param>
    /// <returns>唯一文件名</returns>
    public static string GenerateUniqueFileName(string originalFileName)
    {
        if (string.IsNullOrEmpty(originalFileName))
            throw new ArgumentException("文件名不能为空", nameof(originalFileName));

        var extension = Path.GetExtension(originalFileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
        var safeName = string.Join("_", fileNameWithoutExtension.Split(Path.GetInvalidFileNameChars()));
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var random = new Random().Next(1000, 9999);

        return $"{safeName}_{timestamp}_{random}{extension}";
    }

    /// <summary>
    /// 生成文件存储路径（按日期分组）
    /// </summary>
    /// <param name="uploadPath">基础上传路径</param>
    /// <param name="fileName">文件名</param>
    /// <returns>完整存储路径</returns>
    public static string GenerateFilePath(string uploadPath, string fileName)
    {
        var now = DateTime.UtcNow;
        var yearMonth = now.ToString("yyyy/MM");

        return Path.Combine(uploadPath, yearMonth, fileName);
    }

    /// <summary>
    /// 生成缩略图路径
    /// </summary>
    /// <param name="originalPath">原始文件路径</param>
    /// <returns>缩略图路径</returns>
    public static string GenerateThumbnailPath(string originalPath)
    {
        var directory = Path.GetDirectoryName(originalPath);
        var fileName = Path.GetFileNameWithoutExtension(originalPath);
        var extension = ".jpg";

        return Path.Combine(directory, "thumbnails", $"{fileName}_thumb{extension}");
    }

    /// <summary>
    /// 格式化文件大小
    /// </summary>
    /// <param name="bytes">字节数</param>
    /// <returns>格式化后的文件大小</returns>
    public static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }

    /// <summary>
    /// 生成缩略图（仅支持图片）
    /// </summary>
    /// <param name="sourcePath">源文件路径</param>
    /// <param name="destPath">目标文件路径</param>
    /// <param name="width">缩略图宽度</param>
    /// <param name="height">缩略图高度</param>
    /// <returns>是否成功</returns>
    public static async Task<bool> GenerateThumbnailAsync(
        string sourcePath,
        string destPath,
        int width = 200,
        int height = 200)
    {
        try
        {
            // 确保目标目录存在
            var destDir = Path.GetDirectoryName(destPath);
            if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            // 使用 ImageSharp 生成缩略图
            using var image = await Image.LoadAsync(sourcePath);
            image.Mutate(x => x
                .Resize(new ResizeOptions
                {
                    Size = new Size(width, height),
                    Mode = ResizeMode.Crop
                }));

            await image.SaveAsJpegAsync(destPath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 验证文件大小
    /// </summary>
    /// <param name="fileSize">文件大小（字节）</param>
    /// <param name="maxSize">最大允许大小（字节）</param>
    /// <returns>是否有效</returns>
    public static bool ValidateFileSize(long fileSize, long maxSize)
    {
        return fileSize > 0 && fileSize <= maxSize;
    }

    /// <summary>
    /// 验证 IFormFile
    /// </summary>
    /// <param name="file">上传的文件</param>
    /// <param name="maxSize">最大文件大小</param>
    /// <returns>验证结果</returns>
    public static FileValidationResult ValidateFile(IFormFile file, long maxSize = 104857600)
    {
        var result = new FileValidationResult { IsValid = true };

        if (file == null || file.Length == 0)
        {
            result.IsValid = false;
            result.Errors.Add("文件不能为空");
            return result;
        }

        // 验证文件大小
        if (!ValidateFileSize(file.Length, maxSize))
        {
            result.IsValid = false;
            result.Errors.Add($"文件大小不能超过 {FormatFileSize(maxSize)}");
            return result;
        }

        // 验证文件扩展名
        if (!IsAllowedExtension(file.FileName))
        {
            result.IsValid = false;
            result.Errors.Add($"不支持的文件类型: {Path.GetExtension(file.FileName)}");
            return result;
        }

        // 验证 MIME 类型
        if (!IsAllowedMimeType(file.ContentType))
        {
            result.IsValid = false;
            result.Errors.Add($"不支持的 MIME 类型: {file.ContentType}");
            return result;
        }

        return result;
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>是否成功</returns>
    public static bool DeleteFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 删除文件和缩略图
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>是否成功</returns>
    public static bool DeleteFileWithThumbnail(string filePath)
    {
        var success = DeleteFile(filePath);

        if (!string.IsNullOrEmpty(filePath))
        {
            var thumbnailPath = GenerateThumbnailPath(filePath);
            DeleteFile(thumbnailPath);
        }

        return success;
    }

    /// <summary>
    /// 文件验证结果
    /// </summary>
    public class FileValidationResult
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 错误信息列表
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// 获取错误信息字符串
        /// </summary>
        /// <returns>错误信息</returns>
        public string GetErrorMessage()
        {
            return IsValid ? "" : string.Join("; ", Errors);
        }
    }

    /// <summary>
    /// 文件上传结果
    /// </summary>
    public class FileUploadResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 文件名（生成的唯一文件名）
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// 原始文件名
        /// </summary>
        public string? OriginalName { get; set; }

        /// <summary>
        /// 文件路径（相对路径）
        /// </summary>
        public string? FilePath { get; set; }

        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// MIME 类型
        /// </summary>
        public string? MimeType { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string? FileExtension { get; set; }

        /// <summary>
        /// 文件分类
        /// </summary>
        public string? FileCategory { get; set; }

        /// <summary>
        /// 缩略图路径
        /// </summary>
        public string? ThumbnailPath { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string? Error { get; set; }
    }

    /// <summary>
    /// 批量文件上传结果
    /// </summary>
    public class BatchFileUploadResult
    {
        /// <summary>
        /// 总文件数
        /// </summary>
        public int TotalFiles { get; set; }

        /// <summary>
        /// 成功文件数
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 失败文件数
        /// </summary>
        public int FailureCount { get; set; }

        /// <summary>
        /// 上传结果列表
        /// </summary>
        public List<FileUploadResult> Results { get; set; } = new List<FileUploadResult>();

        /// <summary>
        /// 成功的文件列表
        /// </summary>
        public List<FileUploadResult> SuccessFiles => Results.Where(r => r.Success).ToList();

        /// <summary>
        /// 失败的文件列表
        /// </summary>
        public List<FileUploadResult> FailedFiles => Results.Where(r => !r.Success).ToList();
    }
}
