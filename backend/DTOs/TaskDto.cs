using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Web.DTOs;

/// <summary>
/// 创建任务请求 DTO
/// </summary>
public class CreateTaskDto
{
    /// <summary>
    /// 项目 ID
    /// </summary>
    [Required(ErrorMessage = "项目ID不能为空")]
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    /// 任务类型（bug/feature/task/improvement）
    /// </summary>
    [Required(ErrorMessage = "任务类型不能为空")]
    [RegularExpression(@"^(bug|feature|task|improvement)$", ErrorMessage = "任务类型必须是 bug、feature、task 或 improvement")]
    public string Type { get; set; } = "task";

    /// <summary>
    /// 任务标题（1-200字符）
    /// </summary>
    [Required(ErrorMessage = "任务标题不能为空")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "任务标题长度必须在 1-200 字符之间")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 任务描述（富文本）
    /// </summary>
    [StringLength(5000, ErrorMessage = "任务描述长度不能超过 5000 字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 任务优先级（low/medium/high/urgent）
    /// </summary>
    [RegularExpression(@"^(low|medium|high|urgent)$", ErrorMessage = "优先级必须是 low、medium、high 或 urgent")]
    public string Priority { get; set; } = "medium";

    /// <summary>
    /// 分配给用户 ID（可选）
    /// </summary>
    [RegularExpression(@"^[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}$", ErrorMessage = "无效的用户ID")]
    public string? AssigneeId { get; set; }

    /// <summary>
    /// 截止日期
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// 预估工时（小时）
    /// </summary>
    [Range(0, 9999, ErrorMessage = "预估工时必须在 0-9999 之间")]
    public decimal? EstimatedHours { get; set; }

    /// <summary>
    /// 标签数组
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// 标签列表（JSONB）
    /// </summary>
    public object? Labels { get; set; }
}

/// <summary>
/// 更新任务请求 DTO
/// </summary>
public class UpdateTaskDto
{
    /// <summary>
    /// 任务标题
    /// </summary>
    [StringLength(200, MinimumLength = 1, ErrorMessage = "任务标题长度必须在 1-200 字符之间")]
    public string? Title { get; set; }

    /// <summary>
    /// 任务描述（富文本）
    /// </summary>
    [StringLength(5000, ErrorMessage = "任务描述长度不能超过 5000 字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 任务类型
    /// </summary>
    [RegularExpression(@"^(bug|feature|task|improvement)$", ErrorMessage = "任务类型必须是 bug、feature、task 或 improvement")]
    public string? Type { get; set; }

    /// <summary>
    /// 任务状态（todo/in_progress/done/cancelled）
    /// </summary>
    [RegularExpression(@"^(todo|in_progress|done|cancelled)$", ErrorMessage = "任务状态必须是 todo、in_progress、done 或 cancelled")]
    public string? Status { get; set; }

    /// <summary>
    /// 任务优先级
    /// </summary>
    [RegularExpression(@"^(low|medium|high|urgent)$", ErrorMessage = "优先级必须是 low、medium、high 或 urgent")]
    public string? Priority { get; set; }

    /// <summary>
    /// 分配给用户 ID
    /// </summary>
    [RegularExpression(@"^[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}$", ErrorMessage = "无效的用户ID")]
    public string? AssigneeId { get; set; }

    /// <summary>
    /// 截止日期
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// 预估工时
    /// </summary>
    [Range(0, 9999, ErrorMessage = "预估工时必须在 0-9999 之间")]
    public decimal? EstimatedHours { get; set; }

    /// <summary>
    /// 实际工时
    /// </summary>
    [Range(0, 9999, ErrorMessage = "实际工时必须在 0-9999 之间")]
    public decimal? ActualHours { get; set; }

    /// <summary>
    /// 标签数组
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// 标签列表
    /// </summary>
    public object? Labels { get; set; }

    /// <summary>
    /// 解决方案
    /// </summary>
    [StringLength(2000, ErrorMessage = "解决方案长度不能超过 2000 字符")]
    public string? Resolution { get; set; }

    /// <summary>
    /// 任务元数据
    /// </summary>
    public object? Metadata { get; set; }
}

/// <summary>
/// 任务响应 DTO
/// </summary>
public class TaskDto
{
    /// <summary>
    /// 任务 ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 项目 ID
    /// </summary>
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    /// 任务编号（项目内唯一）
    /// </summary>
    public int TaskNumber { get; set; }

    /// <summary>
    /// 任务类型
    /// </summary>
    public string Type { get; set; } = "task";

    /// <summary>
    /// 任务标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 任务描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 任务状态
    /// </summary>
    public string Status { get; set; } = "todo";

    /// <summary>
    /// 任务优先级
    /// </summary>
    public string Priority { get; set; } = "medium";

    /// <summary>
    /// 是否过期（截止日期已过且未完成）
    /// </summary>
    public bool IsOverdue => DueDate != null && DueDate < DateTime.UtcNow.Date && Status != "done";

    /// <summary>
    /// 是否已完成
    /// </summary>
    public bool IsCompleted => Status == "done";

    /// <summary>
    /// 是否进行中
    /// </summary>
    public bool IsInProgress => Status == "in_progress";

    /// <summary>
    /// 是否已取消
    /// </summary>
    public bool IsCancelled => Status == "cancelled";

    /// <summary>
    /// 创建者 ID
    /// </summary>
    public string CreatorId { get; set; } = string.Empty;

    /// <summary>
    /// 分配者 ID
    /// </summary>
    public string? AssigneeId { get; set; }

    /// <summary>
    /// 截止日期
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// 预估工时
    /// </summary>
    public decimal? EstimatedHours { get; set; }

    /// <summary>
    /// 实际工时
    /// </summary>
    public decimal? ActualHours { get; set; }

    /// <summary>
    /// 工时完成度
    /// </summary>
    public double HoursProgress => EstimatedHours.HasValue && EstimatedHours > 0
        ? (double)(ActualHours ?? 0) / (double)EstimatedHours.Value * 100
        : 0;

    /// <summary>
    /// 标签数组
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// 标签列表
    /// </summary>
    public object? Labels { get; set; }

    /// <summary>
    /// 解决方案
    /// </summary>
    public string? Resolution { get; set; }

    /// <summary>
    /// 完成时间
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// 工作时长（从开始到完成的时间）
    /// </summary>
    public TimeSpan? WorkDuration
    {
        get
        {
            if (StartedAt != null && CompletedAt != null)
                return CompletedAt - StartedAt;
            return null;
        }
    }

    /// <summary>
    /// 任务元数据
    /// </summary>
    public object? Metadata { get; set; }

    /// <summary>
    /// 附件数量
    /// </summary>
    public int AttachmentCount { get; set; }

    /// <summary>
    /// 评论数量
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// 任务标识（如 PROJ-123）
    /// </summary>
    public string Identifier => $"{ProjectKey}-{TaskNumber}";

    /// <summary>
    /// 项目键（内部使用）
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public string ProjectKey { get; set; } = string.Empty;

    /// <summary>
    /// 所属项目
    /// </summary>
    public ProjectSummaryDto? Project { get; set; }

    /// <summary>
    /// 创建者信息
    /// </summary>
    public UserSummaryDto? Creator { get; set; }

    /// <summary>
    /// 分配者信息
    /// </summary>
    public UserSummaryDto? Assignee { get; set; }

    /// <summary>
    /// 最新评论（前3条）
    /// </summary>
    public List<CommentSummaryDto>? LatestComments { get; set; }

    /// <summary>
    /// 附件列表（前3个）
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
/// 更新任务状态 DTO
/// </summary>
public class UpdateTaskStatusDto
{
    /// <summary>
    /// 新状态
    /// </summary>
    [Required(ErrorMessage = "状态不能为空")]
    [RegularExpression(@"^(todo|in_progress|done|cancelled)$", ErrorMessage = "状态必须是 todo、in_progress、done 或 cancelled")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 状态变更原因（可选）
    /// </summary>
    [StringLength(500, ErrorMessage = "状态变更原因长度不能超过 500 字符")]
    public string? Reason { get; set; }
}

/// <summary>
/// 分配任务 DTO
/// </summary>
public class AssignTaskDto
{
    /// <summary>
    /// 分配给用户 ID
    /// </summary>
    [Required(ErrorMessage = "分配用户ID不能为空")]
    [RegularExpression(@"^[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}$", ErrorMessage = "无效的用户ID")]
    public string AssigneeId { get; set; } = string.Empty;

    /// <summary>
    /// 分配原因（可选）
    /// </summary>
    [StringLength(500, ErrorMessage = "分配原因长度不能超过 500 字符")]
    public string? Reason { get; set; }
}

/// <summary>
/// 任务列表查询参数 DTO
/// </summary>
public class TaskQueryDto : PaginationDto
{
    /// <summary>
    /// 项目 ID
    /// </summary>
    [RegularExpression(@"^[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}$", ErrorMessage = "无效的项目ID")]
    public string? ProjectId { get; set; }

    /// <summary>
    /// 任务状态筛选
    /// </summary>
    [RegularExpression(@"^(todo|in_progress|done|cancelled)?$", ErrorMessage = "状态必须是 todo、in_progress、done 或 cancelled")]
    public string? Status { get; set; }

    /// <summary>
    /// 任务优先级筛选
    /// </summary>
    [RegularExpression(@"^(low|medium|high|urgent)?$", ErrorMessage = "优先级必须是 low、medium、high 或 urgent")]
    public string? Priority { get; set; }

    /// <summary>
    /// 任务类型筛选
    /// </summary>
    [RegularExpression(@"^(bug|feature|task|improvement)?$", ErrorMessage = "类型必须是 bug、feature、task 或 improvement")]
    public string? Type { get; set; }

    /// <summary>
    /// 分配给用户 ID
    /// </summary>
    [RegularExpression(@"^[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}$", ErrorMessage = "无效的用户ID")]
    public string? AssigneeId { get; set; }

    /// <summary>
    /// 创建者 ID
    /// </summary>
    [RegularExpression(@"^[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}$", ErrorMessage = "无效的用户ID")]
    public string? CreatorId { get; set; }

    /// <summary>
    /// 是否只查看分配给我的任务
    /// </summary>
    public bool? AssignedToMe { get; set; }

    /// <summary>
    /// 是否只查看我创建的任务
    /// </summary>
    public bool? CreatedByMe { get; set; }

    /// <summary>
    /// 搜索关键词（搜索标题或描述）
    /// </summary>
    [StringLength(100, ErrorMessage = "搜索关键词长度不能超过 100 字符")]
    public string? Search { get; set; }

    /// <summary>
    /// 截止日期范围（开始）
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? DueDateFrom { get; set; }

    /// <summary>
    /// 截止日期范围（结束）
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? DueDateTo { get; set; }

    /// <summary>
    /// 创建日期范围（开始）
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? CreatedFrom { get; set; }

    /// <summary>
    /// 创建日期范围（结束）
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? CreatedTo { get; set; }

    /// <summary>
    /// 标签筛选
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// 排序字段（title, priority, status, created_at, due_date, task_number）
    /// </summary>
    [RegularExpression(@"^(title|priority|status|created_at|due_date|task_number)$", ErrorMessage = "无效的排序字段")]
    public string SortBy { get; set; } = "created_at";

    /// <summary>
    /// 排序方向（asc/desc）
    /// </summary>
    [RegularExpression(@"^(asc|desc)$", ErrorMessage = "排序方向必须是 asc 或 desc")]
    public string SortDirection { get; set; } = "desc";
}

/// <summary>
/// 批量更新任务 DTO
/// </summary>
public class BatchUpdateTaskDto
{
    /// <summary>
    /// 任务 ID 列表
    /// </summary>
    [Required(ErrorMessage = "任务ID列表不能为空")]
    public List<string> TaskIds { get; set; } = new List<string>();

    /// <summary>
    /// 要更新的字段和值
    /// </summary>
    [Required(ErrorMessage = "更新数据不能为空")]
    public Dictionary<string, object> Updates { get; set; } = new Dictionary<string, object>();
}

/// <summary>
/// 批量删除任务 DTO
/// </summary>
public class BatchDeleteTaskDto
{
    /// <summary>
    /// 任务 ID 列表
    /// </summary>
    [Required(ErrorMessage = "任务ID列表不能为空")]
    [MinLength(1, ErrorMessage = "至少要选择一个任务")]
    public List<string> TaskIds { get; set; } = new List<string>();

    /// <summary>
    /// 删除原因（可选）
    /// </summary>
    [StringLength(500, ErrorMessage = "删除原因长度不能超过 500 字符")]
    public string? Reason { get; set; }
}

/// <summary>
/// 任务统计 DTO
/// </summary>
public class TaskStatisticsDto
{
    /// <summary>
    /// 总任务数
    /// </summary>
    public int TotalTasks { get; set; }

    /// <summary>
    /// 待办任务数
    /// </summary>
    public int TodoTasks { get; set; }

    /// <summary>
    /// 进行中任务数
    /// </summary>
    public int InProgressTasks { get; set; }

    /// <summary>
    /// 已完成任务数
    /// </summary>
    public int DoneTasks { get; set; }

    /// <summary>
    /// 已取消任务数
    /// </summary>
    public int CancelledTasks { get; set; }

    /// <summary>
    /// 过期任务数
    /// </summary>
    public int OverdueTasks { get; set; }

    /// <summary>
    /// 完成率（百分比）
    /// </summary>
    public double CompletionRate => TotalTasks > 0 ? (double)DoneTasks / TotalTasks * 100 : 0;

    /// <summary>
    /// 按状态统计
    /// </summary>
    public Dictionary<string, int> ByStatus { get; set; } = new Dictionary<string, int>();

    /// <summary>
    /// 按优先级统计
    /// </summary>
    public Dictionary<string, int> ByPriority { get; set; } = new Dictionary<string, int>();

    /// <summary>
    /// 按类型统计
    /// </summary>
    public Dictionary<string, int> ByType { get; set; } = new Dictionary<string, int>();

    /// <summary>
    /// 按分配人统计
    /// </summary>
    public Dictionary<string, int> ByAssignee { get; set; } = new Dictionary<string, int>();

    /// <summary>
    /// 总预估工时
    /// </summary>
    public decimal TotalEstimatedHours { get; set; }

    /// <summary>
    /// 总实际工时
    /// </summary>
    public decimal TotalActualHours { get; set; }

    /// <summary>
    /// 工时偏差（实际 - 预估）
    /// </summary>
    public decimal HoursVariance => TotalActualHours - TotalEstimatedHours;

    /// <summary>
    /// 工时偏差率（百分比）
    /// </summary>
    public double HoursVarianceRate => TotalEstimatedHours > 0 ? (double)(TotalActualHours - TotalEstimatedHours) / (double)TotalEstimatedHours * 100 : 0;
}

/// <summary>
/// 项目摘要 DTO
/// </summary>
public class ProjectSummaryDto
{
    /// <summary>
    /// 项目 ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 项目名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 项目键
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 项目颜色
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// 项目图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 项目状态
    /// </summary>
    public string Status { get; set; } = "active";
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
    /// 评论者姓名
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
    /// 上传时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
