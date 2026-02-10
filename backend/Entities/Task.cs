using SqlSugar;

namespace TaskFlow.Web.Entities;

/// <summary>
/// 任务实体
/// </summary>
[SugarTable("tasks")]
public class Task : BaseEntity
{
    /// <summary>
    /// 项目ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false, ColumnName = "project_id")]
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    /// 任务编号（项目内唯一）
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnName = "task_number")]
    public int TaskNumber { get; set; }

    /// <summary>
    /// 任务类型（bug/feature/task/improvement）
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = false, ColumnName = "type")]
    public string Type { get; set; } = "task";

    /// <summary>
    /// 任务标题
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = false, ColumnName = "title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 任务描述（富文本）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnName = "description")]
    public string? Description { get; set; }

    /// <summary>
    /// 任务状态（todo/in_progress/done/cancelled）
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = false, ColumnName = "status")]
    public string Status { get; set; } = "todo";

    /// <summary>
    /// 优先级（low/medium/high/urgent）
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = false, ColumnName = "priority")]
    public string Priority { get; set; } = "medium";

    /// <summary>
    /// 创建者ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false, ColumnName = "creator_id")]
    public string CreatorId { get; set; } = string.Empty;

    /// <summary>
    /// 分配者ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = true, ColumnName = "assignee_id")]
    public string? AssigneeId { get; set; }

    /// <summary>
    /// 截止日期
    /// </summary>
    [SugarColumn(ColumnDataType = "date", IsNullable = true, ColumnName = "due_date")]
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// 预估工时
    /// </summary>
    [SugarColumn(DecimalDigits = 2, Length = 10, IsNullable = true, ColumnName = "estimated_hours")]
    public decimal? EstimatedHours { get; set; }

    /// <summary>
    /// 实际工时
    /// </summary>
    [SugarColumn(DecimalDigits = 2, Length = 10, IsNullable = true, ColumnName = "actual_hours")]
    public decimal? ActualHours { get; set; }

    /// <summary>
    /// 标签数组
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnName = "tags")]
    public string? Tags { get; set; }

    /// <summary>
    /// 标签列表（JSONB）
    /// </summary>
    [SugarColumn(ColumnDataType = "jsonb", IsNullable = true, ColumnName = "labels")]
    public string? Labels { get; set; }

    /// <summary>
    /// 解决方案
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnName = "resolution")]
    public string? Resolution { get; set; }

    /// <summary>
    /// 完成时间
    /// </summary>
    [SugarColumn(ColumnDataType = "timestamp", IsNullable = true, ColumnName = "completed_at")]
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    [SugarColumn(ColumnDataType = "timestamp", IsNullable = true, ColumnName = "started_at")]
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// 任务元数据（JSONB）
    /// </summary>
    [SugarColumn(ColumnDataType = "jsonb", IsNullable = true, ColumnName = "metadata")]
    public string? Metadata { get; set; }

    /// <summary>
    /// 附件数量
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnName = "attachment_count", DefaultValue = "0")]
    public int AttachmentCount { get; set; } = 0;

    /// <summary>
    /// 评论数量
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnName = "comment_count", DefaultValue = "0")]
    public int CommentCount { get; set; } = 0;

    // 导航属性（不映射到数据库）

    /// <summary>
    /// 所属项目
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public Project? Project { get; set; }

    /// <summary>
    /// 创建者
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public User? Creator { get; set; }

    /// <summary>
    /// 分配者
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public User? Assignee { get; set; }

    /// <summary>
    /// 任务评论
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Comment>? Comments { get; set; }

    /// <summary>
    /// 任务附件
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Attachment>? Attachments { get; set; }
}
