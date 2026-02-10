using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskFlow.Web.Entities;

namespace TaskFlow.Web.Core;

/// <summary>
/// JWT 工具类
/// </summary>
public static class JwtHelper
{
    /// <summary>
    /// 生成 JWT Token
    /// </summary>
    /// <param name="user">用户信息</param>
    /// <param name="secretKey">密钥</param>
    /// <param name="issuer">签发者</param>
    /// <param name="audience">受众</param>
    /// <param name="expirationMinutes">过期时间（分钟）</param>
    /// <returns>JWT Token</returns>
    public static string GenerateToken(
        User user,
        string secretKey,
        string issuer,
        string audience,
        int expirationMinutes = 10080)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrEmpty(secretKey))
            throw new ArgumentException("密钥不能为空", nameof(secretKey));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(expirationMinutes).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Username ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Role, user.Role ?? "User"),
            new Claim("fullName", user.FullName ?? ""),
            new Claim("avatar", user.Avatar ?? "")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 生成刷新 Token
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="secretKey">密钥</param>
    /// <param name="issuer">签发者</param>
    /// <param name="audience">受众</param>
    /// <param name="expirationDays">过期时间（天）</param>
    /// <returns>刷新 Token</returns>
    public static string GenerateRefreshToken(
        string userId,
        string secretKey,
        string issuer,
        string audience,
        int expirationDays = 30)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("用户ID不能为空", nameof(userId));

        if (string.IsNullOrEmpty(secretKey))
            throw new ArgumentException("密钥不能为空", nameof(secretKey));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddDays(expirationDays).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim("tokenType", "refresh")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(expirationDays),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 验证 JWT Token
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <param name="secretKey">密钥</param>
    /// <param name="issuer">签发者</param>
    /// <param name="audience">受众</param>
    /// <returns>是否有效</returns>
    public static bool ValidateToken(
        string token,
        string secretKey,
        string? issuer = null,
        string? audience = null)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        if (string.IsNullOrEmpty(secretKey))
            throw new ArgumentException("密钥不能为空", nameof(secretKey));

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = !string.IsNullOrEmpty(issuer),
                ValidIssuer = issuer,
                ValidateAudience = !string.IsNullOrEmpty(audience),
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            tokenHandler.ValidateToken(token, validationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 从 Token 中获取 Claims
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>Claims</returns>
    public static ClaimsPrincipal? GetClaimsFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ReadJwtToken(token);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 从 Token 中获取用户 ID
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>用户 ID</returns>
    public static string? GetUserIdFromToken(string token)
    {
        var claims = GetClaimsFromToken(token);
        return claims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// 从 Token 中获取用户名
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>用户名</returns>
    public static string? GetUsernameFromToken(string token)
    {
        var claims = GetClaimsFromToken(token);
        return claims?.FindFirst(ClaimTypes.Name)?.Value;
    }

    /// <summary>
    /// 从 Token 中获取邮箱
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>邮箱</returns>
    public static string? GetEmailFromToken(string token)
    {
        var claims = GetClaimsFromToken(token);
        return claims?.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// 从 Token 中获取角色
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>角色</returns>
    public static string? GetRoleFromToken(string token)
    {
        var claims = GetClaimsFromToken(token);
        return claims?.FindFirst(ClaimTypes.Role)?.Value;
    }

    /// <summary>
    /// 从 Token 中获取过期时间
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>过期时间</returns>
    public static DateTime? GetExpirationFromToken(string token)
    {
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        return jwtToken.ValidTo;
    }

    /// <summary>
    /// 检查 Token 是否过期
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>是否过期</returns>
    public static bool IsTokenExpired(string token)
    {
        var expiration = GetExpirationFromToken(token);
        return expiration == null || expiration < DateTime.UtcNow;
    }

    /// <summary>
    /// 检查 Token 是否即将过期（5分钟内）
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <param name="bufferMinutes">缓冲时间（分钟）</param>
    /// <returns>是否即将过期</returns>
    public static bool IsTokenExpiringSoon(string token, int bufferMinutes = 5)
    {
        var expiration = GetExpirationFromToken(token);
        if (expiration == null)
            return true;

        return expiration < DateTime.UtcNow.AddMinutes(bufferMinutes);
    }

    /// <summary>
    /// 解码 Token 并返回完整信息
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>Token 信息</returns>
    public static TokenInfo? DecodeToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            return new TokenInfo
            {
                UserId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                Username = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                Email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                Role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
                FullName = jwtToken.Claims.FirstOrDefault(c => c.Type == "fullName")?.Value,
                Avatar = jwtToken.Claims.FirstOrDefault(c => c.Type == "avatar")?.Value,
                IssuedAt = jwtToken.IssuedAt,
                ExpiresAt = jwtToken.ValidTo,
                TokenType = jwtToken.Claims.FirstOrDefault(c => c.Type == "tokenType")?.Value
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Token 信息类
    /// </summary>
    public class TokenInfo
    {
        /// <summary>
        /// 用户 ID
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// 全名
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// 签发时间
        /// </summary>
        public DateTime? IssuedAt { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Token 类型
        /// </summary>
        public string? TokenType { get; set; }

        /// <summary>
        /// 是否过期
        /// </summary>
        public bool IsExpired => ExpiresAt == null || ExpiresAt < DateTime.UtcNow;

        /// <summary>
        /// 是否即将过期
        /// </summary>
        public bool IsExpiringSoon => ExpiresAt != null && ExpiresAt < DateTime.UtcNow.AddMinutes(5);
    }
}
