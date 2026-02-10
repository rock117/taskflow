using SqlSugar;

namespace TaskFlow.Web.Entities;

/// <summary>
/// 用户实体
/// </summary>
[SugarTable("users")]
public class User : BaseEntity
{
    /// <summary>
    /// 用户名（唯一）
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnName = "username")]
    public string Username { get; set; }

    /// <summary>
    /// 邮箱（唯一）
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false, ColumnName = "email")]
    public string Email { get; set; }

    /// <summary>
    /// 密码（加密）
    /// </summary>
    [SugarColumn(Length = 255, IsNullable = false, ColumnName = "password")]
    public string Password { get; set; }

    /// <summary>
    /// 全名
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnName = "full_name")]
    public string? FullName { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    [SugarColumn(Length = 255, IsNullable = true, ColumnName = "avatar")]
    public string? Avatar { get; set; }

    /// <summary>
    /// 角色（Admin/User）
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = false, ColumnName = "role")]
    public string Role { get; set; } = "User";

    /// <summary>
    /// 是否激活
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnName = "is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 最后登录时间
    /// </summary>
    [SugarColumn(ColumnDataType = "timestamp", IsNullable = true, ColumnName = "last_login")]
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// 重置密码令牌
    /// </summary>
    [SugarColumn(Length = 255, IsNullable = true, ColumnName = "reset_password_token")]
    public string? ResetPasswordToken { get; set; }

    /// <summary>
    /// 重置密码令牌过期时间
    /// </summary>
    [SugarColumn(ColumnDataType = "timestamp", IsNullable = true, ColumnName = "reset_password_expire")]
    public DateTime? ResetPasswordExpire { get; set; }

    // 导航属性（不映射到数据库）

    /// <summary>
    /// 创建的项目
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Project>? CreatedProjects { get; set; }

    /// <summary>
    /// 创建的任务
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Task>? CreatedTasks { get; set; }

    /// <summary>
    /// 分配的任务
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Task>? AssignedTasks { get; set; }

    /// <summary>
    /// 评论
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Comment>? Comments { get; set; }

    /// <summary>
    /// 上传的附件
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Attachment>? Attachments { get; set; }
}
