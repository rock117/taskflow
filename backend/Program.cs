using Furion;
using Furion.DataEncryption;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SqlSugar;
using System.Text;
using TaskFlow.Web.Core;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. 添加 Furion 框架服务
// ============================================================
builder.Services.AddFurion(options =>
{
    options.ForceHttpJson = true; // 强制使用 JSON 序列化
});

// ============================================================
// 2. 配置 SqlSugar 数据库服务
// ============================================================
builder.Services.AddSqlSugar(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    // 配置 SqlSugar 连接
    options.ConfigureConnectionModels = it =>
    {
        it.ConfigureExternalService = new ConfigureExternalService
        {
            DataInfoCacheService = new SqlSugarDataCacheService() // 数据缓存服务
        };

        it.ConnectionStrings = connectionString;
        it.DbType = DbType.PostgreSQL; // 使用 PostgreSQL
        it.IsAutoCloseConnection = true; // 自动关闭连接
        it.InitKeyType = InitKeyType.Attribute; // 使用特性作为主键
        it.ConfigureExternalServices = new ConfigureExternalServices
        {
            EntityService = new SqlSugarEntityService(), // 实体服务
            DataInfoCacheService = new SqlSugarDataCacheService()
        };

        // 更多配置
        it.MoreSettings = new ConnMoreSettings
        {
            IsAutoRemoveDataCache = true, // 自动清除缓存
            IsWithNoLockQuery = true, // 使用 WITH(NOLOCK)
            SqlFuncServices = new SqlFuncServices() // SQL 函数服务
        };

        // AOP 配置
        it.AopEvents = new AopEvents
        {
            // 执行 SQL 前的拦截器
            OnLogExecuting = (sql, p) =>
            {
                // 输出 SQL 语句（开发环境）
                if (builder.Environment.IsDevelopment())
                {
                    Console.WriteLine($"[SQL] {sql}");
                    if (p != null && p.Length > 0)
                    {
                        Console.WriteLine($"[Parameters] {string.Join(", ", p.Select(x => $"{x.ParameterName}={x.Value}"))}");
                    }
                }
            },

            // 数据库操作错误拦截
            OnError = (exp) =>
            {
                Console.WriteLine($"[SQL Error] {exp.Message}");
            },

            // 数据查询后拦截
            OnLogExecuted = (sql, p) =>
            {
                // 可以记录执行时间等信息
            }
        };

        // 自动建表（开发环境）
        if (builder.Environment.IsDevelopment())
        {
            it.CreateDataBase = false; // 不自动创建数据库
            it.ConfigureExternalServices.DataInfoCacheService = new SqlSugarDataCacheService();
        }
    };

    // 配置可从 DI 容器中获取的 ISqlSugarClient
    options.CurrentConnectionConfig = new ConnectionConfig()
    {
        ConnectionString = connectionString,
        DbType = DbType.PostgreSQL,
        IsAutoCloseConnection = true,
        InitKeyType = InitKeyType.Attribute,
        ConfigureExternalServices = new ConfigureExternalServices
        {
            EntityService = new SqlSugarEntityService(),
            DataInfoCacheService = new SqlSugarDataCacheService()
        },
        MoreSettings = new ConnMoreSettings
        {
            IsAutoRemoveDataCache = true,
            IsWithNoLockQuery = true
        }
    };
});

// ============================================================
// 3. 配置 JWT 认证
// ============================================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero // 禁止时钟偏差
    };

    // JWT 事件处理
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            // 可以在这里添加额外的验证逻辑
            return Task.CompletedTask;
        }
    };
});

// ============================================================
// 4. 配置授权策略
// ============================================================
builder.Services.AddAuthorization(options =>
{
    // Admin 策略
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    // 项目创建者策略
    options.AddPolicy("ProjectCreator", policy =>
        policy.RequireAssertion(context =>
        {
            // 这里需要自定义授权处理器
            return true;
        }));
});

// ============================================================
// 5. 配置 CORS 跨域
// ============================================================
builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins ?? new[] { "http://localhost:3000" })
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ============================================================
// 6. 配置 Swagger API 文档
// ============================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TaskFlow API",
        Version = "v1",
        Description = "TaskFlow - 现代化任务管理系统 API 文档",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "TaskFlow Team",
            Email = "support@taskflow.com"
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // JWT 认证配置
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // XML 注释文件
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// ============================================================
// 7. 配置文件上传
// ============================================================
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.Parse(builder.Configuration["FileUpload:MaxFileSize"] ?? "104857600"); // 默认 100MB
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// ============================================================
// 8. 配置全局异常处理
// ============================================================
builder.Services.AddGlobalExceptionHandler(options =>
{
    options.ErrorMessageFormat = (context, exception) =>
    {
        return new UnifyResultVo
        {
            Code = context.Response.StatusCode,
            Message = exception.Message,
            Data = exception.Data,
            Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            TraceId = context.TraceIdentifier
        };
    };

    options.OnlyLogErrorMessage = false; // 记录完整的错误信息
    options.IgnoreExceptionTypes = new[]
    {
        typeof(UnauthorizedAccessException),
        typeof(InvalidOperationException)
    };
});

// ============================================================
// 9. 配置数据验证
// ============================================================
builder.Services.AddFluentValidation(options =>
{
    options.RegisterValidatorsFromAssemblyContaining<Program>();
    options.AutomaticValidationEnabled = true;
});

// ============================================================
// 10. 配置日志服务
// ============================================================
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();

    if (builder.Environment.IsProduction())
    {
        // 生产环境使用 Serilog
        loggingBuilder.AddSerilog();
    }
});

// ============================================================
// 11. 添加应用服务（依赖注入）
// ============================================================
// 这里将注册所有的服务和仓储
// builder.Services.AddScoped<IUserService, UserService>();
// builder.Services.AddScoped<IProjectService, ProjectService>();
// builder.Services.AddScoped<ITaskService, TaskService>();
// builder.Services.AddScoped<ICommentService, CommentService>();
// builder.Services.AddScoped<IAttachmentService, AttachmentService>();

// ============================================================
// 12. 配置 AutoMapper
// ============================================================
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ============================================================
// 13. 添加控制器
// ============================================================
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>(); // 全局异常过滤器
    options.Filters.Add<GlobalAuthorizeFilter>(); // 全局授权过滤器（公开接口可用 [AllowAnonymous]）
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
});

// ============================================================
// 14. 配置 HTTP 请求
// ============================================================
builder.Services.AddHttpClient();

// ============================================================
// 15. 构建应用
// ============================================================
var app = builder.Build();

// ============================================================
// 16. 配置中间件管道
// ============================================================

// 16.1 开发环境配置
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskFlow API v1");
        options.RoutePrefix = "swagger";
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        options.DefaultModelsExpandDepth(-1);
    });
}

// 16.2 全局异常处理
app.UseUnifyResultStatusCodes(); // 统一状态码处理

// 16.3 HTTPS 重定向（生产环境）
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// 16.4 静态文件服务
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/uploads",
    OnPrepareResponse = ctx =>
    {
        // 缓存配置
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=604800"); // 7天缓存
    }
});

// 16.5 路由
app.UseRouting();

// 16.6 CORS 跨域
app.UseCors();

// 16.7 认证和授权
app.UseAuthentication();
app.UseAuthorization();

// 16.8 终端端点（可选，主要用于健康检查）
app.MapEndpoints();

// ============================================================
// 17. 初始化数据库（开发环境）
// ============================================================
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

    // 自动创建数据库表（开发模式）
    db.CodeFirst.InitTables(
        typeof(Entity).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Namespace == "TaskFlow.Web.Entities")
            .ToArray()
    );

    Console.WriteLine("✅ Database tables initialized successfully.");
}

// ============================================================
// 18. 运行应用
// ============================================================
Console.WriteLine("🚀 TaskFlow API is starting...");
Console.WriteLine($"📝 Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"🔗 Base URL: {app.Urls.FirstOrDefault() ?? "http://localhost:5000"}");
Console.WriteLine($"📚 Swagger URL: {app.Urls.FirstOrDefault() ?? "http://localhost:5000"}/swagger");

app.Run();
