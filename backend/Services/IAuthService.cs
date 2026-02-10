using TaskFlow.Web.DTOs;
using TaskFlow.Web.Entities;

namespace TaskFlow.Web.Services;

/// <summary>
/// 认证服务接口
/// </summary>
public interface IAuthService : ITransient
{
    /// <summary>
    /// 用户注册
    /// </summary>
    /// <param name="dto">注册信息</param>
    /// <returns>登录响应（包含 token 和用户信息）</returns>
    Task<LoginResponseDto> RegisterAsync(RegisterDto dto);

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="dto">登录信息</param>
    /// <returns>登录响应（包含 token 和用户信息）</returns>
    Task<LoginResponseDto> LoginAsync(LoginDto dto);

    /// <summary>
    /// 用户登出
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <returns>登出成功</returns>
    Task<bool> LogoutAsync(string userId);

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <returns>用户信息</returns>
    Task<UserDto> GetCurrentUserAsync(string userId);

    /// <summary>
    /// 更新用户个人资料
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="dto">更新信息</param>
    /// <returns>更新后的用户信息</returns>
    Task<UserDto> UpdateProfileAsync(string userId, UpdateProfileDto dto);

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="dto">密码信息</param>
    /// <returns>修改成功</returns>
    Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto);

    /// <summary>
    /// 忘记密码 - 发送重置链接
    /// </summary>
    /// <param name="dto">邮箱信息</param>
    /// <returns>发送成功</returns>
    Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto);

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="dto">重置信息</param>
    /// <returns>重置成功</returns>
    Task<bool> ResetPasswordAsync(ResetPasswordDto dto);

    /// <summary>
    /// 刷新 Token
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="refreshToken">刷新令牌</param>
    /// <returns>新的 Token</returns>
    Task<LoginResponseDto> RefreshTokenAsync(string userId, string refreshToken);

    /// <summary>
    /// 验证 Token 有效性
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>是否有效</returns>
    bool ValidateToken(string token);

    /// <summary>
    /// 从 Token 获取用户 ID
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>用户 ID</returns>
    string? GetUserIdFromToken(string token);

    /// <summary>
    /// 搜索用户（用于 @提及）
    /// </summary>
    /// <param name="query">搜索关键词</param>
    /// <param name="limit">返回数量限制</param>
    /// <returns>用户列表</returns>
    Task<List<UserSearchResultDto>> SearchUsersAsync(string query, int limit = 10);

    /// <summary>
    /// 获取用户详情
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <returns>用户详情</returns>
    Task<UserDto?> GetUserByIdAsync(string userId);

    /// <summary>
    /// 检查用户名是否已存在
    /// </summary>
    /// <param name="username">用户名</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsUsernameExistsAsync(string username);

    /// <summary>
    /// 检查邮箱是否已存在
    /// </summary>
    /// <param name="email">邮箱地址</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsEmailExistsAsync(string email);

    /// <summary>
    /// 激活用户
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <returns>激活成功</returns>
    Task<bool> ActivateUserAsync(string userId);

    /// <summary>
    /// 停用用户
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <returns>停用成功</returns>
    Task<bool> DeactivateUserAsync(string userId);

    /// <summary>
    /// 更新最后登录时间
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <returns>更新成功</returns>
    Task<bool> UpdateLastLoginAsync(string userId);

    /// <summary>
    /// 生成密码重置令牌
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <returns>重置令牌</returns>
    Task<string> GeneratePasswordResetTokenAsync(string userId);

    /// <summary>
    /// 验证密码重置令牌
    /// </summary>
    /// <param name="token">重置令牌</param>
    /// <param name="userId">用户 ID</param>
    /// <returns>是否有效</returns>
    bool ValidatePasswordResetToken(string token, string userId);
}
