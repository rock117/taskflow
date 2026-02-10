# TaskFlow .NET 后端项目说明文档

## 📋 项目概述

TaskFlow 是一个现代化的任务管理系统后端 API，基于 .NET 8.0 开发，采用 Furion 框架和 SqlSugar ORM，提供 RESTful API 服务。

### 核心功能

- ✅ 用户认证与授权（JWT）
- ✅ 项目管理（CRUD）
- ✅ 任务管理（创建、更新、分配、状态流转）
- ✅ 评论系统（富文本、回复、@提及）
- ✅ 附件管理（上传、下载、预览）
- ✅ 全局异常处理
- ✅ 数据验证
- ✅ API 文档自动生成

---

## 🛠️ 技术栈

| 技术/框架 | 版本 | 用途 |
|-----------|------|------|
| .NET | 8.0 | 核心框架 |
| Furion | 4.9.2 | 应用框架（快速开发、AOP、依赖注入） |
| SqlSugar | 5.1.4.154 | ORM 框架 |
| Npgsql | 8.0.3 | PostgreSQL 数据库驱动 |
| JWT Bearer | 8.0.0 | 认证授权 |
| FluentValidation | 11.3.0 | 数据验证 |
| BCrypt.Net-Next | 4.0.3 | 密码加密 |
| Swashbuckle.AspNetCore | 6.5.0 | Swagger API 文档 |

---

## 📁 项目结构

```
TaskFlow.Backend/
├── Entities/                    # 实体层（数据模型）
│   ├── BaseEntity.cs           # 实体基类
│   ├── User.cs                 # 用户实体
│   ├── Project.cs              # 项目实体
│   ├── Task.cs                 # 任务实体
│   ├── Comment.cs              # 评论实体
│   └── Attachment.cs           # 附件实体
│
├── DTOs/                       # 数据传输对象
│   ├── Auth/                   # 认证相关 DTO
│   ├── Project/                # 项目相关 DTO
│   ├── Task/                   # 任务相关 DTO
│   └── Common/                 # 通用 DTO
│
├── Services/                    # 服务层（业务逻辑）
│   ├── IAuthService.cs         # 认证服务接口
│   ├── AuthService.cs          # 认证服务实现
│   ├── IProjectService.cs      # 项目服务接口
│   ├── ProjectService.cs       # 项目服务实现
│   └── ...                     # 其他服务
│
├── Controllers/                 # 控制器层（API 端点）
│   ├── AuthController.cs       # 认证控制器
│   ├── UserController.cs       # 用户控制器
│   ├── ProjectController.cs    # 项目控制器
│   ├── TaskController.cs       # 任务控制器
│   ├── CommentController.cs    # 评论控制器
│   └── AttachmentController.cs # 附件控制器
│
├── Core/                        # 核心工具类
│   ├── JwtHelper.cs           # JWT 工具
│   ├── PasswordHelper.cs      # 密码工具
│   ├── FileHelper.cs          # 文件工具
│   └── DataHelper.cs          # 数据工具
│
├── Validators/                  # 验证器
│   ├── RegisterValidator.cs    # 注册验证
│   ├── LoginValidator.cs      # 登录验证
│   └── ...                    # 其他验证器
│
├── Filters/                     # 过滤器
│   ├── GlobalAuthorizeFilter.cs      # 全局授权过滤器
│   └── GlobalExceptionFilter.cs      # 全局异常过滤器
│
├── Uploads/                     # 文件上传目录
│   ├── 2024/
│   │   └── 01/
│   └── Thumbnails/
│
├── Logs/                        # 日志目录
│   └── log-*.txt
│
├── Program.cs                   # 程序入口（启动配置）
├── appsettings.json             # 配置文件
├── appsettings.Development.json # 开发环境配置
├── appsettings.Production.json  # 生产环境配置
└── TaskFlow.Web.csproj          # 项目文件

```

---

## 🚀 快速开始

### 前置要求

- .NET 8.0 SDK 或更高版本
- PostgreSQL 12+ 数据库
- Visual Studio 2022 / VS Code / Rider

### 安装步骤

#### 1. 克隆项目

```bash
git clone https://github.com/yourusername/taskflow.git
cd taskflow/backend
```

#### 2. 还原依赖包

```bash
dotnet restore
```

#### 3. 配置数据库

编辑 `appsettings.json` 文件，修改数据库连接字符串：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=taskflow_db;Username=postgres;Password=your_password_here"
  }
}
```

#### 4. 创建数据库

```sql
-- 使用 PostgreSQL 客户端工具连接
CREATE DATABASE taskflow_db;
```

#### 5. 初始化数据库表

```bash
# 运行项目时会自动创建表（开发环境）
dotnet run
```

或在 Program.cs 中配置自动建表：

```csharp
db.CodeFirst.InitTables(
    typeof(Entity).Assembly.GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract && t.Namespace == "TaskFlow.Web.Entities")
        .ToArray()
);
```

#### 6. 运行项目

```bash
# 开发模式运行
dotnet run

# 或指定环境运行
dotnet run --environment Development
```

服务将在 `http://localhost:5000` 启动

---

## 📚 API 文档

项目启动后，访问 Swagger API 文档：

**开发环境**：http://localhost:5000/swagger

### API 端点总览

#### 认证相关 (`/api/auth`)

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| POST | `/api/auth/register` | 用户注册 | 否 |
| POST | `/api/auth/login` | 用户登录 | 否 |
| POST | `/api/auth/logout` | 用户登出 | 是 |
| GET | `/api/auth/me` | 获取当前用户 | 是 |
| PUT | `/api/auth/me` | 更新用户信息 | 是 |
| POST | `/api/auth/change-password` | 修改密码 | 是 |
| POST | `/api/auth/forgot-password` | 忘记密码 | 否 |
| POST | `/api/auth/reset-password` | 重置密码 | 否 |

#### 用户管理 (`/api/users`)

| 方法 | 端点 | 描述 | 认证 | 权限 |
|------|------|------|------|------|
| GET | `/api/users` | 获取用户列表 | 是 | Admin |
| GET | `/api/users/search` | 搜索用户 | 是 | - |
| GET | `/api/users/{id}` | 获取用户详情 | 是 | - |
| PUT | `/api/users/{id}` | 更新用户 | 是 | Admin |
| DELETE | `/api/users/{id}` | 删除用户 | 是 | Admin |
| POST | `/api/users/{id}/activate` | 激活用户 | 是 | Admin |
| POST | `/api/users/{id}/deactivate` | 停用用户 | 是 | Admin |

#### 项目管理 (`/api/projects`)

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| GET | `/api/projects` | 获取项目列表 | 是 |
| POST | `/api/projects` | 创建项目 | 是 |
| GET | `/api/projects/{id}` | 获取项目详情 | 是 |
| PUT | `/api/projects/{id}` | 更新项目 | 是 |
| DELETE | `/api/projects/{id}` | 删除项目 | 是 |
| POST | `/api/projects/{id}/archive` | 归档项目 | 是 |
| POST | `/api/projects/{id}/activate` | 激活项目 | 是 |
| GET | `/api/projects/{id}/tasks` | 获取项目任务 | 是 |
| GET | `/api/projects/{id}/statistics` | 获取项目统计 | 是 |

#### 任务管理 (`/api/tasks`)

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| GET | `/api/tasks` | 获取任务列表 | 是 |
| POST | `/api/tasks` | 创建任务 | 是 |
| GET | `/api/tasks/{id}` | 获取任务详情 | 是 |
| PUT | `/api/tasks/{id}` | 更新任务 | 是 |
| DELETE | `/api/tasks/{id}` | 删除任务 | 是 |
| PATCH | `/api/tasks/{id}/status` | 更新任务状态 | 是 |
| POST | `/api/tasks/{id}/assign` | 分配任务 | 是 |
| POST | `/api/tasks/{id}/unassign` | 取消分配任务 | 是 |

#### 评论管理 (`/api/comments`)

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| GET | `/api/comments/task/{taskId}` | 获取任务评论 | 是 |
| POST | `/api/comments` | 创建评论 | 是 |
| GET | `/api/comments/{id}` | 获取评论详情 | 是 |
| PUT | `/api/comments/{id}` | 更新评论 | 是 |
| DELETE | `/api/comments/{id}` | 删除评论 | 是 |
| POST | `/api/comments/{id}/reaction` | 添加表情反应 | 是 |
| DELETE | `/api/comments/{id}/reaction` | 删除表情反应 | 是 |

#### 附件管理 (`/api/attachments`)

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| POST | `/api/attachments/upload` | 上传文件 | 是 |
| GET | `/api/attachments/{id}` | 获取附件信息 | 是 |
| GET | `/api/attachments/{id}/download` | 下载文件 | 是 |
| GET | `/api/attachments/{id}/preview` | 预览文件 | 是 |
| DELETE | `/api/attachments/{id}` | 删除附件 | 是 |
| GET | `/api/attachments/task/{taskId}` | 获取任务附件 | 是 |

---

## 🔧 配置说明

### appsettings.json 配置项

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "PostgreSQL连接字符串"
  },
  "JwtSettings": {
    "SecretKey": "JWT密钥（至少32字符）",
    "Issuer": "签发者",
    "Audience": "受众",
    "ExpirationMinutes": 10080  // Token过期时间（分钟）
  },
  "FileUpload": {
    "MaxFileSize": 104857600,  // 最大文件大小（字节）
    "UploadPath": "Uploads"
  },
  "Cors": {
    "AllowedOrigins": ["允许的跨域源"]
  }
}
```

### 环境变量配置

可以通过环境变量覆盖配置：

```bash
# 数据库连接
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;..."

# JWT 配置
export JwtSettings__SecretKey="your-secret-key"
export JwtSettings__ExpirationMinutes="10080"
```

---

## 🗄️ 数据库设计

### 用户表 (users)

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | UUID | 主键 |
| username | VARCHAR(50) | 用户名（唯一） |
| email | VARCHAR(100) | 邮箱（唯一） |
| password | VARCHAR(255) | 密码（BCrypt加密） |
| full_name | VARCHAR(100) | 全名 |
| avatar | VARCHAR(255) | 头像URL |
| role | VARCHAR(20) | 角色（Admin/User） |
| is_active | BOOLEAN | 是否激活 |
| last_login | TIMESTAMP | 最后登录时间 |
| created_at | TIMESTAMP | 创建时间 |
| updated_at | TIMESTAMP | 更新时间 |
| deleted_at | TIMESTAMP | 删除时间（软删除） |

### 项目表 (projects)

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | UUID | 主键 |
| name | VARCHAR(100) | 项目名称 |
| description | TEXT | 项目描述 |
| key | VARCHAR(10) | 项目键（唯一） |
| creator_id | UUID | 创建者ID |
| status | VARCHAR(20) | 状态（active/inactive/archived） |
| start_date | DATE | 开始日期 |
| end_date | DATE | 结束日期 |
| color | VARCHAR(7) | 项目颜色 |
| icon | VARCHAR(50) | 项目图标 |
| settings | JSONB | 项目设置 |
| created_at | TIMESTAMP | 创建时间 |
| updated_at | TIMESTAMP | 更新时间 |
| deleted_at | TIMESTAMP | 删除时间（软删除） |

### 任务表 (tasks)

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | UUID | 主键 |
| project_id | UUID | 项目ID |
| task_number | INT | 任务编号（项目内唯一） |
| type | VARCHAR(20) | 类型（bug/feature/task/improvement） |
| title | VARCHAR(200) | 任务标题 |
| description | TEXT | 任务描述（富文本） |
| status | VARCHAR(20) | 状态（todo/in_progress/done/cancelled） |
| priority | VARCHAR(20) | 优先级（low/medium/high/urgent） |
| creator_id | UUID | 创建者ID |
| assignee_id | UUID | 分配者ID |
| due_date | DATE | 截止日期 |
| estimated_hours | DECIMAL | 预估工时 |
| actual_hours | DECIMAL | 实际工时 |
| tags | TEXT | 标签（JSON数组） |
| labels | JSONB | 标签（JSONB） |
| resolution | TEXT | 解决方案 |
| completed_at | TIMESTAMP | 完成时间 |
| started_at | TIMESTAMP | 开始时间 |
| metadata | JSONB | 元数据 |
| attachment_count | INT | 附件数量 |
| comment_count | INT | 评论数量 |
| created_at | TIMESTAMP | 创建时间 |
| updated_at | TIMESTAMP | 更新时间 |
| deleted_at | TIMESTAMP | 删除时间（软删除） |

### 评论表 (comments)

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | UUID | 主键 |
| task_id | UUID | 任务ID |
| user_id | UUID | 用户ID |
| content | TEXT | 评论内容（富文本） |
| parent_id | UUID | 父评论ID（用于回复） |
| is_edited | BOOLEAN | 是否编辑过 |
| edited_at | TIMESTAMP | 编辑时间 |
| mentions | TEXT | @提及的用户ID数组 |
| attachment_count | INT | 附件数量 |
| reactions | JSONB | 表情反应 |
| metadata | JSONB | 元数据 |
| is_system | BOOLEAN | 是否系统评论 |
| system_action | VARCHAR(50) | 系统操作类型 |
| created_at | TIMESTAMP | 创建时间 |
| updated_at | TIMESTAMP | 更新时间 |
| deleted_at | TIMESTAMP | 删除时间（软删除） |

### 附件表 (attachments)

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | UUID | 主键 |
| task_id | UUID | 任务ID（可选） |
| comment_id | UUID | 评论ID（可选） |
| uploaded_by | UUID | 上传者ID |
| filename | VARCHAR(255) | 文件名（生成的唯一文件名） |
| original_name | VARCHAR(255) | 原始文件名 |
| file_path | VARCHAR(500) | 文件路径 |
| file_size | BIGINT | 文件大小（字节） |
| mime_type | VARCHAR(100) | MIME类型 |
| file_extension | VARCHAR(10) | 文件扩展名 |
| file_category | VARCHAR(20) | 文件分类 |
| thumbnail_path | VARCHAR(500) | 缩略图路径 |
| metadata | JSONB | 元数据 |
| is_public | BOOLEAN | 是否公开 |
| download_count | INT | 下载次数 |
| last_downloaded_at | TIMESTAMP | 最后下载时间 |
| virus_scan_status | VARCHAR(20) | 病毒扫描状态 |
| virus_scan_date | TIMESTAMP | 病毒扫描日期 |
| created_at | TIMESTAMP | 创建时间 |
| updated_at | TIMESTAMP | 更新时间 |
| deleted_at | TIMESTAMP | 删除时间（软删除） |

---

## 🧪 开发与测试

### 运行测试

```bash
# 运行所有测试
dotnet test

# 运行特定项目测试
dotnet test --filter "FullyQualifiedName~TaskFlow.Tests"
```

### 代码生成

使用 Furion 的代码生成工具：

```bash
# 安装 Furion 工具
dotnet tool install --global Furion.Tools

# 生成代码
furion gen --table-name=users
furion gen --table-name=projects
```

### 数据库迁移

如果使用 Code First 方式：

```bash
# 生成迁移文件
dotnet ef migrations add InitialCreate

# 应用迁移
dotnet ef database update

# 回滚迁移
dotnet ef database update previous-migration
```

---

## 🚢 部署

### Docker 部署

创建 `Dockerfile`：

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TaskFlow.Web.csproj", "./"]
RUN dotnet restore "TaskFlow.Web.csproj"
COPY . .
RUN dotnet build "TaskFlow.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskFlow.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskFlow.Web.dll"]
```

构建并运行：

```bash
# 构建镜像
docker build -t taskflow-api .

# 运行容器
docker run -d -p 5000:80 --name taskflow-api taskflow-api
```

### 生产环境配置

1. 修改 `appsettings.Production.json`
2. 设置正确的数据库连接字符串
3. 配置强密码的 JWT 密钥
4. 启用 HTTPS
5. 配置日志级别
6. 启用文件上传的安全扫描

---

## 📝 开发指南

### 添加新的 API 端点

1. **创建 DTO**（在 `DTOs/` 目录）
2. **创建 Validator**（在 `Validators/` 目录）
3. **创建 Service 接口和实现**（在 `Services/` 目录）
4. **创建 Controller**（在 `Controllers/` 目录）
5. **在 Program.cs 中注册服务**

示例：

```csharp
// 1. 创建 DTO
public class CreateProjectDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
}

// 2. 创建 Validator
public class CreateProjectValidator : AbstractValidator<CreateProjectDto>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

// 3. 创建 Service
public interface IProjectService : ITransient
{
    Task<Project> CreateAsync(CreateProjectDto dto);
}

public class ProjectService : IProjectService
{
    private readonly ISqlSugarClient _db;
    
    public ProjectService(ISqlSugarClient db)
    {
        _db = db;
    }
    
    public async Task<Project> CreateAsync(CreateProjectDto dto)
    {
        var project = new Project { Name = dto.Name };
        return await _db.Insertable(project).ExecuteReturnEntityAsync();
    }
}

// 4. 创建 Controller
[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    
    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
    {
        var result = await _projectService.CreateAsync(dto);
        return Ok(result);
    }
}
```

### 添加全局异常处理

```csharp
public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        var response = new UnifyResultVo
        {
            Code = 500,
            Message = exception.Message,
            Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
        };
        
        context.Result = new JsonResult(response);
        context.ExceptionHandled = true;
    }
}
```

---

## 🔐 安全建议

1. **密码安全**：使用 BCrypt 加密密码
2. **JWT 配置**：生产环境使用强密钥，设置合理的过期时间
3. **SQL 注入防护**：使用参数化查询（SqlSugar 自动处理）
4. **XSS 防护**：对用户输入进行验证和编码
5. **文件上传**：限制文件类型和大小，病毒扫描
6. **HTTPS**：生产环境必须使用 HTTPS
7. **CORS**：严格配置允许的来源
8. **速率限制**：防止 API 暴力攻击
9. **日志记录**：记录所有重要操作和异常
10. **定期备份**：定期备份数据库

---

## 📄 许可证

MIT License

---

## 👥 贡献指南

欢迎提交 Issue 和 Pull Request！

1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

---

## 📞 联系我们

- 项目主页：https://github.com/yourusername/taskflow
- 问题反馈：https://github.com/yourusername/taskflow/issues
- 邮箱：support@taskflow.com

---

**Made with ❤️ by TaskFlow Team**