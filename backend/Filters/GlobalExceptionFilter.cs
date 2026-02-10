using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace TaskFlow.Web.Filters;

/// <summary>
/// 全局异常过滤器
/// 用于统一处理应用程序中的异常并返回标准化的错误响应
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    /// <summary>
    /// 日志服务
    /// </summary>
    private readonly ILogger<GlobalExceptionFilter> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 异常处理方法
    /// </summary>
    /// <param name="context">异常上下文</param>
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        var httpContext = context.HttpContext;
        var traceId = httpContext.TraceIdentifier;

        // 记录异常日志
        LogError(exception, traceId);

        // 根据异常类型返回相应的响应
        var response = CreateErrorResponse(exception, traceId);

        context.Result = new ObjectResult(response)
        {
            StatusCode = (int)response.StatusCode
        };

        // 标记异常已处理
        context.ExceptionHandled = true;
    }

    /// <summary>
    /// 记录错误日志
    /// </summary>
    /// <param name="exception">异常对象</param>
    /// <param name="traceId">追踪ID</param>
    private void LogError(Exception exception, string traceId)
    {
        _logger.LogError(exception, "TraceId: {TraceId}, Message: {Message}", traceId, exception.Message);

        // 记录异常的堆栈跟踪（仅在开发环境）
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Stack Trace: {StackTrace}", exception.StackTrace);
        }
    }

    /// <summary>
    /// 创建错误响应对象
    /// </summary>
    /// <param name="exception">异常对象</param>
    /// <param name="traceId">追踪ID</param>
    /// <returns>错误响应</returns>
    private ErrorResponse CreateErrorResponse(Exception exception, string traceId)
    {
        return exception switch
        {
            UnauthorizedAccessException => new ErrorResponse
            {
                Success = false,
                StatusCode = HttpStatusCode.Unauthorized,
                Code = 401,
                Message = "未授权访问，请先登录",
                TraceId = traceId,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            },
            ArgumentException => new ErrorResponse
            {
                Success = false,
                StatusCode = HttpStatusCode.BadRequest,
                Code = 400,
                Message = exception.Message,
                TraceId = traceId,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            },
            InvalidOperationException => new ErrorResponse
            {
                Success = false,
                StatusCode = HttpStatusCode.BadRequest,
                Code = 400,
                Message = exception.Message,
                TraceId = traceId,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            },
            KeyNotFoundException => new ErrorResponse
            {
                Success = false,
                StatusCode = HttpStatusCode.NotFound,
                Code = 404,
                Message = exception.Message,
                TraceId = traceId,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            },
            NotImplementedException => new ErrorResponse
            {
                Success = false,
                StatusCode = HttpStatusCode.NotImplemented,
                Code = 501,
                Message = "该功能尚未实现",
                TraceId = traceId,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            },
            TimeoutException => new ErrorResponse
            {
                Success = false,
                StatusCode = HttpStatusCode.RequestTimeout,
                Code = 408,
                Message = "请求超时，请稍后重试",
                TraceId = traceId,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            },
            _ => new ErrorResponse
            {
                Success = false,
                StatusCode = HttpStatusCode.InternalServerError,
                Code = 500,
                Message = _logger.IsEnabled(LogLevel.Debug) 
                    ? exception.Message 
                    : "服务器内部错误，请稍后重试",
                TraceId = traceId,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            }
        };
    }
}

/// <summary>
/// 错误响应DTO
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// HTTP状态码
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// 业务错误码
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 追踪ID（用于日志追踪）
    /// </summary>
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    /// 时间戳（毫秒）
    /// </summary>
    public long Timestamp { get; set; }

    /// <summary>
    /// 附加数据（可选）
    /// </summary>
    public object? Data { get; set; }
}

/// <summary>
/// 业务异常类
/// 用于抛出自定义的业务错误
/// </summary>
public class BusinessException : Exception
{
    /// <summary>
    /// 错误码
    /// </summary>
    public int ErrorCode { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="errorCode">错误码</param>
    public BusinessException(string message, int errorCode = 400) : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// 构造函数（带内部异常）
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="errorCode">错误码</param>
    /// <param name="innerException">内部异常</param>
    public BusinessException(string message, int errorCode, Exception innerException) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// 验证异常类
/// 用于数据验证失败时抛出
/// </summary>
public class ValidationException : BusinessException
{
    /// <summary>
    /// 验证错误列表
    /// </summary>
    public List<ValidationError> Errors { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="errors">验证错误列表</param>
    public ValidationException(List<ValidationError> errors) 
        : base("数据验证失败", 400)
    {
        Errors = errors;
    }

    /// <summary>
    /// 构造函数（单个错误）
    /// </summary>
    /// <param name="field">字段名</param>
    /// <param name="message">错误消息</param>
    public ValidationException(string field, string message) 
        : this(new List<ValidationError> { new ValidationError { Field = field, Message = message } })
    {
    }
}

/// <summary>
/// 验证错误DTO
/// </summary>
public class ValidationError
{
    /// <summary>
    /// 字段名
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// 错误消息
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// 资源未找到异常类
/// </summary>
public class NotFoundException : BusinessException
{
    /// <summary>
    /// 资源类型
    /// </summary>
    public string ResourceType { get; }

    /// <summary>
    /// 资源ID
    /// </summary>
    public string ResourceId { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <param name="resourceId">资源ID</param>
    public NotFoundException(string resourceType, string resourceId) 
        : base($"{resourceType} (ID: {resourceId}) 不存在", 404)
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }
}

/// <summary>
/// 权限异常类
/// </summary>
public class PermissionException : BusinessException
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">错误消息</param>
    public PermissionException(string message = "权限不足") 
        : base(message, 403)
    {
    }
}