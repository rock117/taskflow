using SqlSugar;

namespace TaskFlow.Web.Entities;

/// <summary>
/// 附件实体
/// </summary>
[SugarTable("attachments")]
public class Attachment : BaseEntity
{
    /// <summary>
    /// 任务ID（可选）
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = true, ColumnName = "task_id")]
    public string? TaskId { get; set; }

    /// <summary>
    /// 评论ID（可选）
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = true, ColumnName = "comment_id")]
    public string? CommentId { get; set; }

    /// <summary>
    /// 上传者ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false, ColumnName = "uploaded_by")]
    public string UploadedBy { get; set; } = string.Empty;

    /// <summary>
    /// 文件名（生成的唯一文件名）
    /// </summary>
    [SugarColumn(Length = 255, IsNullable = false, ColumnName = "filename")]
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    /// 原始文件名
    /// </summary>
    [SugarColumn(Length = 255, IsNullable = false, ColumnName = "original_name")]
    public string OriginalName { get; set; } = string.Empty;

    /// <summary>
    /// 文件路径
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = false, ColumnName = "file_path")]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnName = "file_size")]
    public long FileSize { get; set; }

    /// <summary>
    /// MIME类型
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false, ColumnName = "mime_type")]
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// 文件扩展名
    /// </summary>
    [SugarColumn(Length = 10, IsNullable = true, ColumnName = "file_extension")]
    public string? FileExtension { get; set; }

    /// <summary>
    /// 文件分类（image/document/video/audio/archive/code/other）
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = false, ColumnName = "file_category")]
    public string FileCategory { get; set; } = "other";

    /// <summary>
    /// 缩略图路径
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnName = "thumbnail_path")]
    public string? ThumbnailPath { get; set; }

    /// <summary>
    /// 附件元数据（JSONB格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "jsonb", IsNullable = true, ColumnName = "metadata")]
    public string? Metadata { get; set; }

    /// <summary>
    /// 是否公开
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnName = "is_public")]
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// 下载次数
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnName = "download_count", DefaultValue = "0")]
    public int DownloadCount { get; set; } = 0;

    /// <summary>
    /// 最后下载时间
    /// </summary>
    [SugarColumn(ColumnDataType = "timestamp", IsNullable = true, ColumnName = "last_downloaded_at")]
    public DateTime? LastDownloadedAt { get; set; }

    /// <summary>
    /// 病毒扫描状态（pending/clean/infected/error）
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = false, ColumnName = "virus_scan_status", DefaultValue = "'pending'")]
    public string VirusScanStatus { get; set; } = "pending";

    /// <summary>
    /// 病毒扫描日期
    /// </summary>
    [SugarColumn(ColumnDataType = "timestamp", IsNullable = true, ColumnName = "virus_scan_date")]
    public DateTime? VirusScanDate { get; set; }

    // 导航属性（不映射到数据库）

    /// <summary>
    /// 上传者
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public User? Uploader { get; set; }

    /// <summary>
    /// 所属任务
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public Task? Task { get; set; }

    /// <summary>
    /// 所属评论
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public Comment? Comment { get; set; }
}
