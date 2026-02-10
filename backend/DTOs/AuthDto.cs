using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Web.DTOs;

/// <summary>
/// 用户注册请求 DTO
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// 用户名（3-50字符，字母和数字）
    /// </summary>
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "用户名长度必须在 3-50 字符之间")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "用户名只能包含字母、数字和下划线")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱地址
    /// </summary>
    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "请输入有效的邮箱地址")]
    [StringLength(100, ErrorMessage = "邮箱长度不能超过 100 字符")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 密码（至少 6 字符）
    /// </summary>
    [Required(ErrorMessage = "密码不能为空")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在 6-100 字符之间")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*#?&]{6,}$", ErrorMessage = "密码必须包含至少一个字母和一个数字")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 确认密码
    /// </summary>
    [Required(ErrorMessage = "确认密码不能为空")]
    [Compare("Password", ErrorMessage = "两次输入的密码不一致")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// 全名（可选）
    /// </summary>
    [StringLength(100, ErrorMessage = "全名长度不能超过 100 字符")]
    public string? FullName { get; set; }
}

/// <summary>
/// 用户登录请求 DTO
/// </summary>
public class LoginDto
{
    /// <summary>
    /// 登录账号（用户名或邮箱）
    /// </summary>
    [Required(ErrorMessage = "登录账号不能为空")]
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// 登录响应 DTO
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// JWT 访问令牌
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// 刷新令牌（可选）
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// 令牌类型
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// 过期时间（Unix 时间戳）
    /// </summary>
    public long ExpiresAt { get; set; }

    /// <summary>
    /// 用户信息
    /// </summary>
    public UserDto User { get; set; } = null!;
}

/// <summary>
/// 更新用户信息请求 DTO
/// </summary>
public class UpdateProfileDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    [StringLength(50, MinimumLength = 3, ErrorMessage = "用户名长度必须在 3-50 字符之间")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "用户名只能包含字母、数字和下划线")]
    public string? Username { get; set; }

    /// <summary>
    /// 邮箱地址
    /// </summary>
    [EmailAddress(ErrorMessage = "请输入有效的邮箱地址")]
    [StringLength(100, ErrorMessage = "邮箱长度不能超过 100 字符")]
    public string? Email { get; set; }

    /// <summary>
    /// 全名
    /// </summary>
    [StringLength(100, ErrorMessage = "全名长度不能超过 100 字符")]
    public string? FullName { get; set; }

    /// <summary>
    /// 头像 URL
    /// </summary>
    [StringLength(255, ErrorMessage = "头像 URL 长度不能超过 255 字符")]
    [Url(ErrorMessage = "请输入有效的 URL")]
    public string? Avatar { get; set; }
}

/// <summary>
/// 修改密码请求 DTO
/// </summary>
public class ChangePasswordDto
{
    /// <summary>
    /// 当前密码
    /// </summary>
    [Required(ErrorMessage = "当前密码不能为空")]
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新密码
    /// </summary>
    [Required(ErrorMessage = "新密码不能为空")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在 6-100 字符之间")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*#?&]{6,}$", ErrorMessage = "密码必须包含至少一个字母和一个数字")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// 确认新密码
    /// </summary>
    [Required(ErrorMessage = "确认新密码不能为空")]
    [Compare("NewPassword", ErrorMessage = "两次输入的新密码不一致")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// 忘记密码请求 DTO
/// </summary>
public class ForgotPasswordDto
{
    /// <summary>
    /// 邮箱地址
    /// </summary>
    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "请输入有效的邮箱地址")]
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// 重置密码请求 DTO
/// </summary>
public class ResetPasswordDto
{
    /// <summary>
    /// 重置密码令牌
    /// </summary>
    [Required(ErrorMessage = "重置令牌不能为空")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// 新密码
    /// </summary>
    [Required(ErrorMessage = "新密码不能为空")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在 6-100 字符之间")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*#?&]{6,}$", ErrorMessage = "密码必须包含至少一个字母和一个数字")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// 确认新密码
    /// </summary>
    [Required(ErrorMessage = "确认新密码不能为空")]
    [Compare("NewPassword", ErrorMessage = "两次输入的新密码不一致")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// 刷新令牌请求 DTO
/// </summary>
public class RefreshTokenDto
{
    /// <summary>
    /// 刷新令牌
    /// </summary>
    [Required(ErrorMessage = "刷新令牌不能为空")]
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// 用户信息响应 DTO
/// </summary>
public class UserDto
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
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 全名
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// 头像 URL
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 角色（Admin/User）
    /// </summary>
    public string Role { get; set; } = "User";

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 统计信息
    /// </summary>
    public UserStatisticsDto? Statistics { get; set; }
}

/// <summary>
/// 用户统计信息 DTO
/// </summary>
public class UserStatisticsDto
{
    /// <summary>
    /// 创建的项目数
    /// </summary>
    public int CreatedProjectsCount { get; set; }

    /// <summary>
    /// 创建的任务数
    /// </summary>
    public int CreatedTasksCount { get; set; }

    /// <summary>
    /// 分配的任务数
    /// </summary>
    public int AssignedTasksCount { get; set; }

    /// <summary>
    /// 完成的任务数
    /// </summary>
    public int CompletedTasksCount { get; set; }

    /// <summary>
    /// 待办任务数
    /// </summary>
    public int TodoTasksCount { get; set; }

    /// <summary>
    /// 进行中的任务数
    /// </summary>
    public int InProgressTasksCount { get; set; }

    /// <summary>
    /// 发表的评论数
    /// </summary>
    public int CommentsCount { get; set; }

    /// <summary>
    /// 上传的附件数
    /// </summary>
    public int AttachmentsCount { get; set; }
}

/// <summary>
/// 用户搜索结果 DTO
/// </summary>
public class UserSearchResultDto
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
