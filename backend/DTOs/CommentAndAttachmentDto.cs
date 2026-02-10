using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Web.DTOs;

/// <summary>
/// 创建评论请求 DTO
/// </summary>
public class CreateCommentDto
{
    /// <summary>
    /// 任务 ID
    /// </summary>
    [Required(ErrorMessage = "任务ID不能为空")]
    public string TaskId { get; set; } = string.Empty;

    /// <summary>
    /// 评论内容（支持富文本）
    /// </summary>
    [Required(ErrorMessage = "评论内容不能为空")]
    [StringLength(10000, MinimumLength = 1, ErrorMessage = "评论内容长度必须在 1-10000 字符之间")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 父评论 ID（用于回复）
    /// </summary>
    [RegularExpression(@"^[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}$", ErrorMessage = "无效的父评论ID")]
    public string? ParentId { get; set; }
}

/// <summary>
/// 更新评论请求 DTO
/// </summary>
public class UpdateCommentDto
{
    /// <summary>
    /// 评论内容（支持富文本）
    /// </summary>
    [Required(ErrorMessage = "评论内容不能为空")]
    [StringLength(10000, MinimumLength = 1, ErrorMessage = "评论内容长度必须在 1-10000 字符之间")]
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// 评论响应 DTO
/// </summary>
public class CommentDto
{
    /// <summary>
    /// 评论 ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 任务 ID
    /// </summary>
    public string TaskId { get; set; } = string.Empty;

    /// <summary>
    /// 评论内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 评论者 ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 父评论 ID
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// 是否编辑过
    /// </summary>
    public bool IsEdited { get; set; } = false;

    /// <summary>
    /// 编辑时间
    /// </summary>
    public DateTime? EditedAt { get; set; }

    /// <summary>
    /// @提及的用户 ID 数组
    /// </summary>
    public List<string>? Mentions { get; set; }

    /// <summary>
    /// 附件数量
    /// </summary>
    public int AttachmentCount { get; set; } = 0;

    /// <summary>
    /// 表情反应（Key: Emoji, Value: User ID 列表）
    /// </summary>
    public Dictionary<string, List<string>>? Reactions { get; set; }

    /// <summary>
    /// 评论元数据
    /// </summary>
    public object? Metadata { get; set; }

    /// <summary>
    /// 是否系统评论
    /// </summary>
    public bool IsSystem { get; set; } = false;

    /// <summary>
    /// 系统操作类型
    /// </summary>
    public string? SystemAction { get; set; }

    /// <summary>
    /// 评论者信息
    /// </summary>
    public UserSummaryDto? User { get; set; }

    /// <summary>
    /// 所属任务信息
    /// </summary>
    public TaskSummaryDto? Task { get; set; }

    /// <summary>
    /// 父评论信息
    /// </summary>
    public CommentDto? ParentComment { get; set; }

    /// <summary>
    /// 回复列表
    /// </summary>
    public List<CommentDto>? Replies { get; set; }

    /// <summary>
    /// 附件列表
    /// </summary>
    public List<AttachmentSummaryDto>? Attachments { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// 评论摘要 DTO
/// </summary>
public class CommentSummaryDto
{
    /// <summary>
    /// 评论 ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 评论内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 评论者 ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 评论者用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 评论者头像
    /// </summary>
    public string? UserAvatar { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 添加表情反应 DTO
/// </summary>
public class AddReactionDto
{
    /// <summary>
    /// 表情符号
    /// </summary>
    [Required(ErrorMessage = "表情符号不能为空")]
    [StringLength(10, ErrorMessage = "表情符号长度不能超过 10 字符")]
    public string Emoji { get; set; } = string.Empty;
}

/// <summary>
/// 附件响应 DTO
/// </summary>
public class AttachmentDto
{
    /// <summary>
    /// 附件 ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 任务 ID（可选）
    /// </summary>
    public string? TaskId { get; set; }

    /// <summary>
    /// 评论 ID（可选）
    /// </summary>
    public string? CommentId { get; set; }

    /// <summary>
    /// 上传者 ID
    /// </summary>
    public string UploadedBy { get; set; } = string.Empty;

    /// <summary>
    /// 文件名（生成的唯一文件名）
    /// </summary>
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    /// 原始文件名
    /// </summary>
    public string OriginalName { get; set; } = string.Empty;

    /// <summary>
    /// 文件路径（相对路径）
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 文件下载 URL
    /// </summary>
    public string DownloadUrl => $"/api/attachments/{Id}/download";

    /// <summary>
    /// 文件预览 URL
    /// </summary>
    public string? PreviewUrl => IsPreviewable ? $"/api/attachments/{Id}/preview" : null;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 格式化后的文件大小
    /// </summary>
    public string FormattedFileSize => FormatFileSize(FileSize);

    /// <summary>
    /// MIME 类型
    /// </summary>
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// 文件扩展名
    /// </summary>
    public string? FileExtension { get; set; }

    /// <summary>
    /// 文件分类（image/document/video/audio/archive/code/other）
    /// </summary>
    public string FileCategory { get; set; } = "other";

    /// <summary>
    /// 是否图片
    /// </summary>
    public bool IsImage => FileCategory == "image";

    /// <summary>
    /// 是否文档
    /// </summary>
    public bool IsDocument => FileCategory == "document";

    /// <summary>
    /// 是否可预览（图片、文档、代码）
    /// </summary>
    public bool IsPreviewable => new[] { "image", "document", "code" }.Contains(FileCategory);

    /// <summary>
    /// 缩略图路径
    /// </summary>
    public string? ThumbnailPath { get; set; }

    /// <summary>
    /// 缩略图 URL
    /// </summary>
    public string? ThumbnailUrl => ThumbnailPath != null ? $"/uploads/{ThumbnailPath}" : null;

    /// <summary>
    /// 附件元数据
    /// </summary>
    public object? Metadata { get; set; }

    /// <summary>
    /// 是否公开
    /// </summary>
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// 下载次数
    /// </summary>
    public int DownloadCount { get; set; } = 0;

    /// <summary>
    /// 最后下载时间
    /// </summary>
    public DateTime? LastDownloadedAt { get; set; }

    /// <summary>
    /// 病毒扫描状态
    /// </summary>
    public string VirusScanStatus { get; set; } = "pending";

    /// <summary>
    /// 上传者信息
    /// </summary>
    public UserSummaryDto? Uploader { get; set; }

    /// <summary>
    /// 所属任务
    /// </summary>
    public TaskSummaryDto? Task { get; set; }

    /// <summary>
    /// 所属评论
    /// </summary>
    public CommentSummaryDto? Comment { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 格式化文件大小
    /// </summary>
    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }
}

/// <summary>
/// 附件摘要 DTO
/// </summary>
public class AttachmentSummaryDto
{
    /// <summary>
    /// 附件 ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 原始文件名
    /// </summary>
    public string OriginalName { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// MIME 类型
    /// </summary>
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// 文件分类
    /// </summary>
    public string FileCategory { get; set; } = "other";

    /// <summary>
    /// 是否图片
    /// </summary>
    public bool IsImage => FileCategory == "image";

    /// <summary>
    /// 上传时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 批量删除评论 DTO
/// </summary>
public class BatchDeleteCommentDto
{
    /// <summary>
    /// 评论 ID 列表
    /// </summary>
    [Required(ErrorMessage = "评论ID列表不能为空")]
    [MinLength(1, ErrorMessage = "至少要选择一个评论")]
    public List<string> CommentIds { get; set; } = new List<string>();

    /// <summary>
    /// 删除原因（可选）
    /// </summary>
    [StringLength(500, ErrorMessage = "删除原因长度不能超过 500 字符")]
    public string? Reason { get; set; }
}

/// <summary>
/// 任务摘要 DTO
/// </summary>
public class TaskSummaryDto
{
    /// <summary>
    /// 任务 ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 任务标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 任务标识
    /// </summary>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// 任务状态
    /// </summary>
    public string Status { get; set; } = "todo";
}

/// <summary>
/// 评论列表查询参数 DTO
/// </summary>
public class CommentQueryDto : PaginationDto
{
    /// <summary>
    /// 任务 ID（必需）
    /// </summary>
    [Required(ErrorMessage = "任务ID不能为空")]
    public string TaskId { get; set; } = string.Empty;

    /// <summary>
    /// 是否包含回复
    /// </summary>
    public bool IncludeReplies { get; set; } = false;

    /// <summary>
    /// 只显示系统评论
    /// </summary>
    public bool OnlySystemComments { get; set; } = false;

    /// <summary>
    /// 排序字段（created_at）
    /// </summary>
    [RegularExpression(@"^(created_at|updated_at)$", ErrorMessage = "排序字段必须是 created_at 或 updated_at")]
    public string SortBy { get; set; } = "created_at";

    /// <summary>
    /// 排序方向（asc/desc）
    /// </summary>
    [RegularExpression(@"^(asc|desc)$", ErrorMessage = "排序方向必须是 asc 或 desc")]
    public string SortDirection { get; set; } = "asc";
}

/// <summary>
/// 附件列表查询参数 DTO
/// </summary>
public class AttachmentQueryDto : PaginationDto
{
    /// <summary>
    /// 任务 ID
    /// </summary>
    [RegularExpression(@"^[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}$", ErrorMessage = "无效的任务ID")]
    public string? TaskId { get; set; }

    /// <summary>
    /// 评论 ID
    /// </summary>
    [RegularExpression(@"^[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}$", ErrorMessage = "无效的评论ID")]
    public string? CommentId { get; set; }

    /// <summary>
    /// 上传者 ID
    /// </summary>
    [RegularExpression(@"^[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}$", ErrorMessage = "无效的用户ID")]
    public string? UploadedBy { get; set; }

    /// <summary>
    /// 文件分类筛选
    /// </summary>
    [RegularExpression(@"^(image|document|video|audio|archive|code|other)?$", ErrorMessage = "无效的文件分类")]
    public string? FileCategory { get; set; }

    /// <summary>
    /// 排序字段（created_at, file_size, file_name）
    /// </summary>
    [RegularExpression(@"^(created_at|file_size|original_name)$", ErrorMessage = "排序字段无效")]
    public string SortBy { get; set; } = "created_at";

    /// <summary>
    /// 排序方向（asc/desc）
    /// </summary>
    [RegularExpression(@"^(asc|desc)$", ErrorMessage = "排序方向必须是 asc 或 desc")]
    public string SortDirection { get; set; } = "desc";
}

/// <summary>
/// 文件上传响应 DTO
/// </summary>
public class FileUploadResultDto
{
    /// <summary>
    /// 上传的文件列表
    /// </summary>
    public List<AttachmentDto> Files { get; set; } = new List<AttachmentDto>();

    /// <summary>
    /// 总文件数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 成功文件数
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// 失败文件数
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// 总文件大小（字节）
    /// </summary>
    public long TotalSize { get; set; }

    /// <summary>
    /// 格式化后的总文件大小
    /// </summary>
    public string FormattedTotalSize => FormatFileSize(TotalSize);

    /// <summary>
    /// 错误信息
    /// </summary>
    public List<string> Errors { get; set; } = new List<string>();

    /// <summary>
    /// 是否全部成功
    /// </summary>
    public bool IsAllSuccess => SuccessCount > 0 && FailureCount == 0;

    /// <summary>
    /// 是否有失败
    /// </summary>
    public bool HasFailures => FailureCount > 0;

    /// <summary>
    /// 格式化文件大小
    /// </summary>
    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }
}
