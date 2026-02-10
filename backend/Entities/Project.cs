using SqlSugar;

namespace TaskFlow.Web.Entities;

/// <summary>
/// 项目实体
/// </summary>
[SugarTable("projects")]
public class Project : BaseEntity
{
    /// <summary>
    /// 项目名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false, ColumnName = "name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 项目描述
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnName = "description")]
    public string? Description { get; set; }

    /// <summary>
    /// 项目键（唯一，大写）
    /// </summary>
    [SugarColumn(Length = 10, IsNullable = false, ColumnName = "key")]
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 创建者ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false, ColumnName = "creator_id")]
    public string CreatorId { get; set; } = string.Empty;

    /// <summary>
    /// 项目状态（active/inactive/archived）
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = false, ColumnName = "status")]
    public string Status { get; set; } = "active";

    /// <summary>
    /// 开始日期
    /// </summary>
    [SugarColumn(ColumnDataType = "date", IsNullable = true, ColumnName = "start_date")]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    [SugarColumn(ColumnDataType = "date", IsNullable = true, ColumnName = "end_date")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 项目颜色
    /// </summary>
    [SugarColumn(Length = 7, IsNullable = true, ColumnName = "color")]
    public string? Color { get; set; } = "#1890ff";

    /// <summary>
    /// 项目图标
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnName = "icon")]
    public string? Icon { get; set; } = "📁";

    /// <summary>
    /// 项目设置（JSONB）
    /// </summary>
    [SugarColumn(ColumnDataType = "jsonb", IsNullable = true, ColumnName = "settings")]
    public string? Settings { get; set; }

    /// <summary>
    /// 项目元数据（JSONB）
    /// </summary>
    [SugarColumn(ColumnDataType = "jsonb", IsNullable = true, ColumnName = "metadata")]
    public string? Metadata { get; set; }

    // 导航属性（不映射到数据库）

    /// <summary>
    /// 创建者
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public User? Creator { get; set; }

    /// <summary>
    /// 项目任务
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Task>? Tasks { get; set; }
}
