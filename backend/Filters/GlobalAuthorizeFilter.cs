using Furion.DataEncryption;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace TaskFlow.Web.Filters;

/// <summary>
/// 全局授权过滤器
/// 用于实现基于角色的访问控制（RBAC）
/// </summary>
public class GlobalAuthorizeFilter : IAuthorizationFilter
{
    /// <summary>
    /// 允许匿名访问的路径
    /// </summary>
    private static readonly string[] AnonymousPaths = new[]
    {
        "/api/auth/login",
        "/api/auth/register",
        "/api/auth/forgot-password",
        "/api/auth/reset-password",
        "/api/health",
        "/swagger",
        "/api-docs"
    };

    /// <summary>
    /// 管理员角色
    /// </summary>
    private const string AdminRole = "Admin";

    /// <summary>
    /// 用户角色
    /// </summary>
    private const string UserRole = "User";

    /// <summary>
    /// 构造函数
    /// </summary>
    public GlobalAuthorizeFilter()
    {
    }

    /// <summary>
    /// 授权检查方法
    /// </summary>
    /// <param name="context">授权上下文</param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // 1. 检查是否允许匿名访问
        if (AllowAnonymous(context))
        {
            context.Succeed();
            return;
        }

        // 2. 检查用户是否已认证
        var httpContext = context.Resource as HttpContext;
        if (httpContext == null)
        {
            context.Fail();
            return;
        }

        var user = httpContext.User;
        if (user == null || !user.Identity.IsAuthenticated)
        {
            // 未认证，返回 401 Unauthorized
            context.Fail();
            httpContext.Response.StatusCode = 401;
            httpContext.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "未授权访问，请先登录",
                code = 401,
                timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            });
            return;
        }

        // 3. 检查用户是否激活
        var isActive = user.FindFirst("IsActive")?.Value;
        if (isActive == "False")
        {
            // 用户已被禁用
            context.Fail();
            httpContext.Response.StatusCode = 403;
            httpContext.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "账户已被禁用，请联系管理员",
                code = 403,
                timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            });
            return;
        }

        // 4. 检查是否需要管理员权限
        var endpoint = context.Resource as Endpoint;
        if (endpoint != null)
        {
            var requiresAdmin = endpoint.Metadata.Any(m => m is RequiresAdminAttribute);
            if (requiresAdmin && !HasAdminRole(user))
            {
                // 需要管理员权限但用户不是管理员
                context.Fail();
                httpContext.Response.StatusCode = 403;
                httpContext.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "需要管理员权限才能访问此接口",
                    code = 403,
                    timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
                });
                return;
            }
        }

        // 5. 资源级权限检查（可选）
        var requiresProjectCreator = endpoint?.Metadata.Any(m => m is RequiresProjectCreatorAttribute);
        if (requiresProjectCreator)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var projectId = httpContext.Request.Query["projectId"]?.FirstOrDefault();

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(projectId))
            {
                var hasPermission = CheckProjectPermission(context, projectId, userId);
                if (!hasPermission)
                {
                    context.Fail();
                    httpContext.Response.StatusCode = 403;
                    httpContext.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = "没有权限访问此资源",
                        code = 403,
                        timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
                    });
                    return;
                }
            }
        }

        // 所有检查通过，授权成功
        context.Succeed();
    }

    /// <summary>
    /// 检查是否允许匿名访问
    /// </summary>
    /// <param name="context">授权上下文</param>
    /// <returns>是否允许匿名访问</returns>
    private bool AllowAllowAnonymous(AuthorizationFilterContext context)
    {
        var endpoint = context.Resource as Endpoint;
        if (endpoint != null)
        {
            // 检查端点是否有 [AllowAnonymous] 特性
            return endpoint.Metadata.Any(m => m is AllowAnonymousAttribute);
        }

        // 检查路径是否在匿名访问列表中
        var httpContext = context.Resource as HttpContext;
        if (httpContext != null)
        {
            var path = httpContext.Request.Path.Value?.ToLower();
            return AnonymousPaths.Any(p => path.StartsWith(p.ToLower()));
        }

        return false;
    }

    /// <summary>
    /// 检查用户是否有管理员角色
    /// </summary>
    /// <param name="user">当前用户</param>
    /// <returns>是否是管理员</returns>
    private bool HasAdminRole(ClaimsPrincipal user)
    {
        if (user == null)
            return false;

        var role = user.FindFirst(ClaimTypes.Role)?.Value;
        return role == AdminRole;
    }

    /// <summary>
    /// 检查用户是否有项目的操作权限
    /// </summary>
    /// <param name="context">授权上下文</param>
    /// <param name="projectId">项目ID</param>
    /// <param name="userId">用户ID</param>
    /// <returns>是否有权限</returns>
    private bool CheckProjectPermission(AuthorizationFilterContext context, string projectId, string userId)
    {
        // TODO: 从数据库或缓存中查询项目权限
        // 这里可以实现基于项目的权限检查
        // 例如：项目成员、项目负责人、项目创建者等

        // 临时返回 true，实际应该查询数据库
        return true;
    }
}

/// <summary>
/// 需要管理员权限特性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequiresAdminAttribute : Attribute
{
}

/// <summary>
/// 需要项目创建者权限特性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequiresProjectCreatorAttribute : Attribute
{
}

/// <summary>
/// 允许匿名访问特性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class AllowAnonymousAttribute : Attribute
{
}
