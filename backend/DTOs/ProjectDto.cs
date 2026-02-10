using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Web.DTOs;

/// <summary>
/// 创建项目请求 DTO
/// </summary>
public class CreateProjectDto
{
    /// <summary>
    /// 项目名称（1-100字符）
    /// </summary>
    [Required(ErrorMessage = "项目名称不能为空")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "项目名称长度必须在 1-100 字符之间")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 项目描述
    /// </summary>
    [StringLength(1000, ErrorMessage = "项目描述长度不能超过 1000 字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 项目键（2-10字符，大写字母）
    /// </summary>
    [StringLength(10, MinimumLength = 2, ErrorMessage = "项目键长度必须在 2-10 字符之间")]
    [RegularExpression(@"^[A-Z]+$", ErrorMessage = "项目键只能包含大写字母")]
    public string? Key { get; set; }

    /// <summary>
    /// 开始日期
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 项目颜色（HEX 格式，如 #1890ff）
    /// </summary>
    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "项目颜色必须是有效的 HEX 颜色代码")]
    public string? Color { get; set; }

    /// <summary>
    /// 项目图标（Emoji 或图标名称）
    /// </summary>
    [StringLength(50, ErrorMessage = "项目图标长度不能超过 50 字符")]
    public string? Icon { get; set; }
}

/// <summary>
/// 更新项目请求 DTO
/// </summary>
public class UpdateProjectDto
{
    /// <summary>
    /// 项目名称
    /// </summary>
    [StringLength(100, MinimumLength = 1, ErrorMessage = "项目名称长度必须在 1-100 字符之间")]
    public string? Name { get; set; }

    /// <summary>
    /// 项目描述
    /// </summary>
    [StringLength(1000, ErrorMessage = "项目描述长度不能超过 1000 字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 项目状态（active/inactive/archived）
    /// </summary>
    [RegularExpression(@"^(active|inactive|archived)$", ErrorMessage = "项目状态必须是 active、inactive 或 archived")]
    public string? Status { get; set; }

    /// <summary>
    /// 开始日期
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 项目颜色
    /// </summary>
    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "项目颜色必须是有效的 HEX 颜色代码")]
    public string? Color { get; set; }

    /// <summary>
    /// 项目图标
    /// </summary>
    [StringLength(50, ErrorMessage = "项目图标长度不能超过 50 字符")]
    public string? Icon { get; set; }
}

/// <summary>
/// 项目响应 DTO
/// </summary>
public class ProjectDto
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
    /// 项目描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 项目键
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 项目标识（如 PROJ-123）
    /// </summary>
    public string Identifier => $"{Key}";

    /// <summary>
    /// 创建者 ID
    /// </summary>
    public string CreatorId { get; set; } = string.Empty;

    /// <summary>
    /// 项目状态
    /// </summary>
    public string Status { get; set; } = "active";

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive => Status == "active";

    /// <summary>
    /// 是否归档
    /// </summary>
    public bool IsArchived => Status == "archived";

    /// <summary>
    /// 是否过期（已过结束日期且仍在激活状态）
    /// </summary>
    public bool IsOverdue => EndDate != null && EndDate < DateTime.UtcNow && Status == "active";

    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 项目颜色
    /// </summary>
    public string? Color { get; set; } = "#1890ff";

    /// <summary>
    /// 项目图标
    /// </summary>
    public string? Icon { get; set; } = "📁";

    /// <summary>
    /// 项目设置（JSONB）
    /// </summary>
    public ProjectSettingsDto? Settings { get; set; }

    /// <summary>
    /// 项目元数据（JSONB）
    /// </summary>
    public object? Metadata { get; set; }

    /// <summary>
    /// 任务数量
    /// </summary>
    public int TaskCount { get; set; }

    /// <summary>
    /// 完成任务数
    /// </summary>
    public int CompletedTaskCount { get; set; }

    /// <summary>
    /// 完成进度（百分比）
    /// </summary>
    public double Progress => TaskCount > 0 ? (double)CompletedTaskCount / TaskCount * 100 : 0;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 创建者信息
    /// </summary>
    public UserSummaryDto? Creator { get; set; }

    /// <summary>
    /// 项目统计信息
    /// </summary>
    public ProjectStatisticsDto? Statistics { get; set; }
}

/// <summary>
/// 项目设置 DTO
/// </summary>
public class ProjectSettingsDto
{
    /// <summary>
    /// 是否允许上传附件
    /// </summary>
    public bool AllowAttachments { get; set; } = true;

    /// <summary>
    /// 是否允许评论
    /// </summary>
    public bool AllowComments { get; set; } = true;

    /// <summary>
    /// 是否需要审批
    /// </summary>
    public bool RequireApproval { get; set; } = false;

    /// <summary>
    /// 是否在更新时通知
    /// </summary>
    public bool NotifyOnUpdate { get; set; } = true;

    /// <summary>
    /// 默认分配者 ID
    /// </summary>
    public string? DefaultAssignee { get; set; }
}

/// <summary>
/// 项目统计信息 DTO
/// </summary>
public class ProjectStatisticsDto
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
    /// 按优先级统计
    /// </summary>
    public Dictionary<string, int> TasksByPriority { get; set; } = new();

    /// <summary>
    /// 按类型统计
    /// </summary>
    public Dictionary<string, int> TasksByType { get; set; } = new();
}

/// <summary>
/// 项目列表查询参数 DTO
/// </summary>
public class ProjectQueryDto : PaginationDto
{
    /// <summary>
    /// 项目状态筛选（active/inactive/archived）
    /// </summary>
    [RegularExpression(@"^(active|inactive|archived)?$", ErrorMessage = "状态必须是 active、inactive 或 archived")]
    public string? Status { get; set; }

    /// <summary>
    /// 搜索关键词（搜索项目名称或描述）
    /// </summary>
    [StringLength(100, ErrorMessage = "搜索关键词长度不能超过 100 字符")]
    public string? Search { get; set; }

    /// <summary>
    /// 创建者 ID 筛选
    /// </summary>
    public string? CreatorId { get; set; }

    /// <summary>
    /// 排序字段（name, created_at, status, task_count）
    /// </summary>
    [RegularExpression(@"^(name|created_at|status|task_count)$", ErrorMessage = "无效的排序字段")]
    public string SortBy { get; set; } = "created_at";

    /// <summary>
    /// 排序方向（asc/desc）
    /// </summary>
    [RegularExpression(@"^(asc|desc)$", ErrorMessage = "排序方向必须是 asc 或 desc")]
    public string SortDirection { get; set; } = "desc";
}

/// <summary>
/// 用户摘要 DTO
/// </summary>
public class UserSummaryDto
{
    /// <summary>
    /// 用户 ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 全名
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// 头像 URL
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName => string.IsNullOrEmpty(FullName) ? Username : FullName;

    /// <summary>
    /// 首字母
    /// </summary>
    public string Initial => string.IsNullOrEmpty(FullName)
        ? Username.Substring(0, 1).ToUpper()
        : FullName.Substring(0, 1).ToUpper();
}

/// <summary>
/// 分页基础 DTO
/// </summary>
public class PaginationDto
{
    /// <summary>
    /// 页码（从 1 开始）
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "页码必须大于 0")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每页大小（1-100）
    /// </summary>
    [Range(1, 100, ErrorMessage = "每页大小必须在 1-100 之间")]
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// 分页响应 DTO
/// </summary>
public class PagedResultDto<T>
{
    /// <summary>
    /// 数据列表
    /// </summary>
    public List<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// 总记录数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 当前页码
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// 是否有上一页
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// 是否有下一页
    /// </summary>
    public bool HasNextPage => Page < TotalPages;
}
