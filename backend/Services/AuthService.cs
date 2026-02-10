using Microsoft.Extensions.Options;
using SqlSugar;
using TaskFlow.Web.Core;
using TaskFlow.Web.DTOs;
using TaskFlow.Web.Entities;

namespace TaskFlow.Web.Services;

/// <summary>
/// 认证服务实现
/// </summary>
public class AuthService : IAuthService
{
    private readonly ISqlSugarClient _db;
    private readonly IConfiguration _configuration;
    private readonly JwtSettings _jwtSettings;

    public AuthService(ISqlSugarClient db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
        _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
            ?? new JwtSettings();
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterDto dto)
    {
        // 检查用户名是否已存在
        var existingUser = await _db.Queryable<User>()
            .Where(u => u.Username == dto.Username || u.Email == dto.Email)
            .FirstAsync();

        if (existingUser != null)
        {
            if (existingUser.Username == dto.Username)
                throw new UnauthorizedAccessException("用户名已存在");
            if (existingUser.Email == dto.Email)
                throw new UnauthorizedAccessException("邮箱已被注册");
        }

        // 加密密码
        var hashedPassword = PasswordHelper.HashPassword(dto.Password);

        // 创建用户
        var user = new User
        {
            Username = dto.Username.ToLower(),
            Email = dto.Email.ToLower(),
            Password = hashedPassword,
            FullName = dto.FullName,
            Role = "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdUser = await _db.Insertable(user).ExecuteReturnEntityAsync();

        // 生成JWT Token
        var token = JwtHelper.GenerateToken(
            createdUser,
            _jwtSettings.SecretKey,
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            _jwtSettings.ExpirationMinutes
        );

        // 生成刷新Token
        var refreshToken = JwtHelper.GenerateRefreshToken(
            createdUser.Id,
            _jwtSettings.SecretKey,
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            30
        );

        // 更新最后登录时间
        await UpdateLastLoginAsync(createdUser.Id);

        return new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes).ToUnixTimeMilliseconds(),
            User = new UserDto
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                FullName = createdUser.FullName,
                Avatar = createdUser.Avatar,
                Role = createdUser.Role,
                IsActive = createdUser.IsActive,
                LastLogin = createdUser.LastLogin,
                CreatedAt = createdUser.CreatedAt,
                UpdatedAt = createdUser.UpdatedAt
            }
        };
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        // 查找用户
        var user = await _db.Queryable<User>()
            .Where(u => u.Username == dto.Login.ToLower() || u.Email == dto.Login.ToLower())
            .Where(u => u.IsActive)
            .FirstAsync();

        if (user == null)
        {
            throw new UnauthorizedAccessException("用户名或密码错误");
        }

        // 验证密码
        var isValidPassword = PasswordHelper.VerifyPassword(dto.Password, user.Password);
        if (!isValidPassword)
        {
            throw new UnauthorizedAccessException("用户名或密码错误");
        }

        // 生成JWT Token
        var token = JwtHelper.GenerateToken(
            user,
            _jwtSettings.SecretKey,
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            _jwtSettings.ExpirationMinutes
        );

        // 生成刷新Token
        var refreshToken = JwtHelper.GenerateRefreshToken(
            user.Id,
            _jwtSettings.SecretKey,
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            30
        );

        // 更新最后登录时间
        await UpdateLastLoginAsync(user.Id);

        return new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes).ToUnixTimeMilliseconds(),
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Avatar = user.Avatar,
                Role = user.Role,
                IsActive = user.IsActive,
                LastLogin = user.LastLogin,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }
        };
    }

    public async Task<bool> LogoutAsync(string userId)
    {
        // 在JWT系统中，登出主要在客户端处理
        // 这里可以清除用户的刷新token等
        var user = await _db.Queryable<User>()
            .Where(u => u.Id == userId)
            .FirstAsync();

        if (user != null)
        {
            // 清除重置token
            user.ResetPasswordToken = null;
            user.ResetPasswordExpire = null;
            await _db.Updateable(user).ExecuteCommandAsync();
        }

        return true;
    }

    public async Task<UserDto> GetCurrentUserAsync(string userId)
    {
        var user = await _db.Queryable<User>()
            .Where(u => u.Id == userId)
            .Where(u => u.IsActive)
            .FirstAsync();

        if (user == null)
        {
            throw new UnauthorizedAccessException("用户不存在或已被禁用");
        }

        // 获取统计信息
        var statistics = new UserStatisticsDto
        {
            CreatedProjectsCount = await _db.Queryable<Project>()
                .Where(p => p.CreatorId == userId)
                .CountAsync(),
            CreatedTasksCount = await _db.Queryable<Task>()
                .Where(t => t.CreatorId == userId)
                .CountAsync(),
            AssignedTasksCount = await _db.Queryable<Task>()
                .Where(t => t.AssigneeId == userId)
                .CountAsync(),
            CompletedTasksCount = await _db.Queryable<Task>()
                .Where(t => t.AssigneeId == userId && t.Status == "done")
                .CountAsync(),
            TodoTasksCount = await _db.Queryable<Task>()
                .Where(t => t.AssigneeId == userId && t.Status == "todo")
                .CountAsync(),
            InProgressTasksCount = await _db.Queryable<Task>()
                .Where(t => t.AssigneeId == userId && t.Status == "in_progress")
                .CountAsync(),
            CommentsCount = await _db.Queryable<Comment>()
                .Where(c => c.UserId == userId)
                .CountAsync(),
            AttachmentsCount = await _db.Queryable<Attachment>()
                .Where(a => a.UploadedBy == userId)
                .CountAsync()
        };

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Avatar = user.Avatar,
            Role = user.Role,
            IsActive = user.IsActive,
            LastLogin = user.LastLogin,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Statistics = statistics
        };
    }

    public async Task<UserDto> UpdateProfileAsync(string userId, UpdateProfileDto dto)
    {
        var user = await _db.Queryable<User>()
            .Where(u => u.Id == userId)
            .FirstAsync();

        if (user == null)
        {
            throw new UnauthorizedAccessException("用户不存在");
        }

        // 检查用户名是否被占用
        if (dto.Username != null && dto.Username != user.Username)
        {
            var existingUsername = await _db.Queryable<User>()
                .Where(u => u.Username == dto.Username.ToLower())
                .Where(u => u.Id != userId)
                .FirstAsync();

            if (existingUsername != null)
            {
                throw new UnauthorizedAccessException("用户名已被占用");
            }
        }

        // 检查邮箱是否被占用
        if (dto.Email != null && dto.Email != user.Email)
        {
            var existingEmail = await _db.Queryable<User>()
                .Where(u => u.Email == dto.Email.ToLower())
                .Where(u => u.Id != userId)
                .FirstAsync();

            if (existingEmail != null)
            {
                throw new UnauthorizedAccessException("邮箱已被占用");
            }
        }

        // 更新用户信息
        if (!string.IsNullOrEmpty(dto.Username))
            user.Username = dto.Username.ToLower();
        if (!string.IsNullOrEmpty(dto.Email))
            user.Email = dto.Email.ToLower();
        if (!string.IsNullOrEmpty(dto.FullName))
            user.FullName = dto.FullName;
        if (!string.IsNullOrEmpty(dto.Avatar))
            user.Avatar = dto.Avatar;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(user).ExecuteCommandAsync();

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Avatar = user.Avatar,
            Role = user.Role,
            IsActive = user.IsActive,
            LastLogin = user.LastLogin,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto)
    {
        var user = await _db.Queryable<User>()
            .Where(u => u.Id == userId)
            .FirstAsync();

        if (user == null)
        {
            throw new UnauthorizedAccessException("用户不存在");
        }

        // 验证当前密码
        var isCurrentPasswordValid = PasswordHelper.VerifyPassword(dto.CurrentPassword, user.Password);
        if (!isCurrentPasswordValid)
        {
            throw new UnauthorizedAccessException("当前密码错误");
        }

        // 更新密码
        var hashedNewPassword = PasswordHelper.HashPassword(dto.NewPassword);
        user.Password = hashedNewPassword;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(user).ExecuteCommandAsync();

        return true;
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var user = await _db.Queryable<User>()
            .Where(u => u.Email == dto.Email.ToLower())
            .Where(u => u.IsActive)
            .FirstAsync();

        if (user == null)
        {
            // 为了安全，即使用户不存在也返回成功
            return true;
        }

        // 生成重置token
        var resetToken = user.GeneratePasswordResetToken();
        await _db.Updateable(user).ExecuteCommandAsync();

        // TODO: 发送密码重置邮件
        // 这里应该集成邮件服务发送包含重置链接的邮件
        // 重置链接格式：https://yourdomain.com/reset-password?token={resetToken}

        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _db.Queryable<User>()
            .Where(u => u.ResetPasswordToken == dto.Token)
            .Where(u => u.ResetPasswordExpire != null && u.ResetPasswordExpire > DateTime.UtcNow)
            .FirstAsync();

        if (user == null)
        {
            throw new UnauthorizedAccessException("重置链接无效或已过期");
        }

        // 更新密码
        var hashedPassword = PasswordHelper.HashPassword(dto.NewPassword);
        user.Password = hashedPassword;
        user.ResetPasswordToken = null;
        user.ResetPasswordExpire = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(user).ExecuteCommandAsync();

        return true;
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(string userId, string refreshToken)
    {
        // 验证刷新token
        var isValidRefreshToken = JwtHelper.ValidateToken(
            refreshToken,
            _jwtSettings.SecretKey,
            _jwtSettings.Issuer,
            _jwtSettings.Audience
        );

        if (!isValidRefreshToken)
        {
            throw new UnauthorizedAccessException("刷新token无效");
        }

        // 获取用户信息
        var user = await _db.Queryable<User>()
            .Where(u => u.Id == userId)
            .Where(u => u.IsActive)
            .FirstAsync();

        if (user == null)
        {
            throw new UnauthorizedAccessException("用户不存在");
        }

        // 生成新的访问token
        var newToken = JwtHelper.GenerateToken(
            user,
            _jwtSettings.SecretKey,
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            _jwtSettings.ExpirationMinutes
        );

        // 生成新的刷新token
        var newRefreshToken = JwtHelper.GenerateRefreshToken(
            user.Id,
            _jwtSettings.SecretKey,
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            30
        );

        return new LoginResponseDto
        {
            Token = newToken,
            RefreshToken = newRefreshToken,
            TokenType = "Bearer",
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes).ToUnixTimeMilliseconds(),
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Avatar = user.Avatar,
                Role = user.Role,
                IsActive = user.IsActive,
                LastLogin = user.LastLogin,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }
        };
    }

    public bool ValidateToken(string token)
    {
        return JwtHelper.ValidateToken(
            token,
            _jwtSettings.SecretKey,
            _jwtSettings.Issuer,
            _jwtSettings.Audience
        );
    }

    public string? GetUserIdFromToken(string token)
    {
        return JwtHelper.GetUserIdFromToken(token);
    }

    public async Task<List<UserSearchResultDto>> SearchUsersAsync(string query, int limit = 10)
    {
        var users = await _db.Queryable<User>()
            .Where(u => u.IsActive)
            .Where(u => u.Username.Contains(query) || u.Email.Contains(query) || u.FullName.Contains(query))
            .OrderBy(u => u.Username)
            .Take(limit)
            .Select(u => new UserSearchResultDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Avatar = u.Avatar
            })
            .ToListAsync();

        return users;
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _db.Queryable<User>()
            .Where(u => u.Id == userId)
            .FirstAsync();

        if (user == null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Avatar = user.Avatar,
            Role = user.Role,
            IsActive = user.IsActive,
            LastLogin = user.LastLogin,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<bool> IsUsernameExistsAsync(string username)
    {
        var count = await _db.Queryable<User>()
            .Where(u => u.Username == username.ToLower())
            .CountAsync();

        return count > 0;
    }

    public async Task<bool> IsEmailExistsAsync(string email)
    {
        var count = await _db.Queryable<User>()
            .Where(u => u.Email == email.ToLower())
            .CountAsync();

        return count > 0;
    }

    public async Task<bool> ActivateUserAsync(string userId)
    {
        var user = await _db.Queryable<User>()
            .Where(u => u.Id == userId)
            .FirstAsync();

        if (user == null)
            return false;

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(user).ExecuteCommandAsync();

        return true;
    }

    public async Task<bool> DeactivateUserAsync(string userId)
    {
        var user = await _db.Queryable<User>()
            .Where(u => u.Id == userId)
            .Where(u => u.Role != "Admin") // 不能停用管理员
            .FirstAsync();

        if (user == null)
            return false;

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(user).ExecuteCommandAsync();

        return true;
    }

    public async Task<bool> UpdateLastLoginAsync(string userId)
    {
        var user = await _db.Queryable<User>()
            .Where(u => u.Id == userId)
            .FirstAsync();

        if (user == null)
            return false;

        user.LastLogin = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(user).ExecuteCommandAsync();

        return true;
    }

    public async Task<string> GeneratePasswordResetTokenAsync(string userId)
    {
        var user = await _db.Queryable<User>()
            .Where(u => u.Id == userId)
            .FirstAsync();

        if (user == null)
            throw new UnauthorizedAccessException("用户不存在");

        var resetToken = user.GeneratePasswordResetToken();
        await _db.Updateable(user).ExecuteCommandAsync();

        return resetToken;
    }

    public bool ValidatePasswordResetToken(string token, string userId)
    {
        return JwtHelper.GetUserIdFromToken(token) == userId;
    }
}
