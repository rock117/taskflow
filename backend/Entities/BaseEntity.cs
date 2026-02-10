using SqlSugar;

namespace TaskFlow.Web.Entities;

/// <summary>
/// 实体基类
/// </summary>
[SugarTable]
public abstract class BaseEntity
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, ColumnName = "id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDataType = "timestamp", IsNullable = true, ColumnName = "created_at")]
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDataType = "timestamp", IsNullable = true, ColumnName = "updated_at")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 删除时间（软删除）
    /// </summary>
    [SugarColumn(ColumnDataType = "timestamp", IsNullable = true, ColumnName = "deleted_at")]
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public bool IsDeleted => DeletedAt != null;
}
