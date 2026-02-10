using SqlSugar;

namespace TaskFlow.Web.Entities;

/// <summary>
/// 评论实体
/// </summary>
[SugarTable("comments")]
public class Comment : BaseEntity
{
    /// <summary>
    /// 任务ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false, ColumnName = "task_id")]
    public string TaskId { get; set; } = string.Empty;

    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false, ColumnName = "user_id")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 评论内容（富文本）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = false, ColumnName = "content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 父评论ID（用于回复）
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = true, ColumnName = "parent_id")]
    public string? ParentId { get; set; }

    /// <summary>
    /// 是否编辑过
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnName = "is_edited")]
    public bool IsEdited { get; set; } = false;

    /// <summary>
    /// 编辑时间
    /// </summary>
    [SugarColumn(ColumnDataType = "timestamp", IsNullable = true, ColumnName = "edited_at")]
    public DateTime? EditedAt { get; set; }

    /// <summary>
    /// @提及的用户ID数组
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnName = "mentions")]
    public string? Mentions { get; set; }

    /// <summary>
    /// 附件数量
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnName = "attachment_count", DefaultValue = "0")]
    public int AttachmentCount { get; set; } = 0;

    /// <summary>
    /// 表情反应（JSONB格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "jsonb", IsNullable = true, ColumnName = "reactions")]
    public string? Reactions { get; set; }

    /// <summary>
    /// 评论元数据（JSONB格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "jsonb", IsNullable = true, ColumnName = "metadata")]
    public string? Metadata { get; set; }

    /// <summary>
    /// 是否系统评论
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnName = "is_system")]
    public bool IsSystem { get; set; } = false;

    /// <summary>
    /// 系统操作类型
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnName = "system_action")]
    public string? SystemAction { get; set; }

    // 导航属性（不映射到数据库）

    /// <summary>
    /// 评论者
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public User? User { get; set; }

    /// <summary>
    /// 所属任务
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public Task? Task { get; set; }

    /// <summary>
    /// 回复列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Comment>? Replies { get; set; }

    /// <summary>
    /// 父评论
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public Comment? ParentComment { get; set; }

    /// <summary>
    /// 附件列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Attachment>? Attachments { get; set; }
}
