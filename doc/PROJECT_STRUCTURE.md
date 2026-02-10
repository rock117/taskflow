# TaskFlow 项目结构

本文档详细说明 TaskFlow 项目的目录结构、文件组织以及各模块的职责。

---

## 📂 项目根目录

```
taskflow/
├── backend/                      # .NET 8.0 后端项目
│   ├── Controllers/               # API 控制器层
│   ├── Services/                 # 业务逻辑服务层
│   ├── DTOs/                    # 数据传输对象
│   ├── Entities/                 # 数据实体模型
│   ├── Core/                     # 核心工具类
│   ├── Filters/                  # 过滤器
│   ├── Uploads/                  # 文件上传存储目录
│   ├── Logs/                     # 日志文件目录（运行时生成）
│   ├── bin/                      # 编译输出目录
│   ├── obj/                      # 编译中间目录
│   ├── Program.cs                # 程序入口
│   ├── appsettings.json          # 应用配置文件
│   ├── appsettings.Development.json
│   ├── appsettings.Production.json
│   └── TaskFlow.Web.csproj       # 项目文件
│
├── frontend/                     # React 18 前端项目
│   ├── src/
│   │   ├── components/           # React 可复用组件
│   │   ├── pages/               # 页面组件
│   │   ├── layouts/             # 布局组件
│   │   ├── services/            # API 服务层
│   │   ├── store/               # Redux 状态管理
│   │   ├── utils/               # 工具函数
│   │   ├── App.js               # 主应用组件
│   │   ├── index.js             # React 入口文件
│   │   └── index.css            # 全局样式
│   ├── public/                   # 静态资源
│   └── package.json             # 依赖配置
│
└── docs/                        # 项目文档
    ├── QUICK_START.md            # 快速启动指南
    ├── PROJECT_OVERVIEW.md        # 项目架构总览
    ├── PROJECT_SESSION.md         # 开发会话记录
    ├── API_DOCUMENTATION.md       # API 文档
    ├── PROJECT_STRUCTURE.md       # 项目结构（本文件）
    └── PROJECT_COMPLETION_SUMMARY.md # 项目完成总结
```

---

## 🔙 后端结构详解

### Controllers/ - API 控制器层

```
Controllers/
├── AuthController.cs         # 认证相关接口
│   ├── POST /api/auth/register        # 用户注册
│   ├── POST /api/auth/login           # 用户登录
│   ├── POST /api/auth/refresh-token    # 刷新 Token
│   ├── POST /api/auth/logout          # 退出登录
│   ├── GET  /api/auth/verify          # 验证 Token
│   ├── POST /api/auth/send-verification-code
│   └── POST /api/auth/forgot-password
│
├── UserController.cs         # 用户管理接口
│   ├── GET    /api/user/me                  # 获取当前用户信息
│   ├── PUT    /api/user/me                  # 更新用户信息
│   ├── POST   /api/user/change-password     # 修改密码
│   ├── POST   /api/user/avatar             # 上传头像
│   ├── GET    /api/user/list              # 获取用户列表
│   └── GET    /api/user/{userId}          # 获取用户详情
│
├── ProjectController.cs      # 项目管理接口
│   ├── POST   /api/projects                       # 创建项目
│   ├── PUT    /api/projects/{projectId}         # 更新项目
│   ├── DELETE /api/projects/{projectId}         # 删除项目
│   ├── GET    /api/projects/{projectId}         # 获取项目详情
│   ├── GET    /api/projects                     # 获取项目列表
│   ├── POST   /api/projects/{projectId}/members   # 添加项目成员
│   ├── DELETE /api/projects/{projectId}/members/{memberId}
│   └── GET    /api/projects/{projectId}/statistics # 获取项目统计
│
├── TaskController.cs        # 任务管理接口
│   ├── POST   /api/tasks                        # 创建任务
│   ├── PUT    /api/tasks/{taskId}                # 更新任务
│   ├── DELETE /api/tasks/{taskId}                # 删除任务
│   ├── GET    /api/tasks/{taskId}                # 获取任务详情
│   ├── GET    /api/tasks/project/{projectId}      # 获取项目任务列表
│   ├── GET    /api/tasks/my-tasks                # 获取用户任务列表
│   ├── POST   /api/tasks/{taskId}/assign         # 分配任务
│   ├── POST   /api/tasks/{taskId}/status         # 更改任务状态
│   ├── POST   /api/tasks/batch-status           # 批量更新状态
│   ├── POST   /api/tasks/{taskId}/tags           # 添加任务标签
│   ├── DELETE /api/tasks/{taskId}/tags/{tag}    # 删除任务标签
│   └── GET    /api/tasks/project/{projectId}/statistics # 获取任务统计
│
├── CommentController.cs     # 评论管理接口
│   ├── POST   /api/comments                       # 创建评论
│   ├── GET    /api/comments/{commentId}           # 获取评论详情
│   ├── GET    /api/comments/task/{taskId}         # 获取任务评论列表
│   ├── PUT    /api/comments/{commentId}           # 更新评论
│   ├── DELETE /api/comments/{commentId}           # 删除评论
│   ├── POST   /api/comments/{commentId}/like      # 切换点赞
│   ├── GET    /api/comments/my-comments          # 获取用户评论列表
│   └── POST   /api/comments/batch-delete       # 批量删除评论
│
└── AttachmentController.cs  # 附件管理接口
    ├── POST   /api/attachments/upload             # 上传文件
    ├── POST   /api/attachments/upload-batch        # 批量上传
    ├── GET    /api/attachments/{attachmentId}     # 获取附件详情
    ├── GET    /api/attachments/{attachmentId}/download # 下载文件
    ├── GET    /api/attachments/{attachmentId}/preview  # 预览文件
    ├── GET    /api/attachments/task/{taskId}      # 获取任务附件列表
    ├── GET    /api/attachments/project/{projectId} # 获取项目附件列表
    ├── GET    /api/attachments/my-attachments   # 获取用户附件列表
    ├── PUT    /api/attachments/{attachmentId}     # 更新附件信息
    ├── POST   /api/attachments/{attachmentId}/move   # 移动附件
    ├── DELETE /api/attachments/{attachmentId}     # 删除附件
    ├── POST   /api/attachments/batch-delete      # 批量删除
    └── GET    /api/attachments/statistics        # 获取文件统计
```

### Services/ - 业务逻辑服务层

```
Services/
├── IAuthService.cs          # 认证服务接口
├── AuthService.cs           # 认证服务实现
│   ├── RegisterAsync                    # 用户注册
│   ├── LoginAsync                       # 用户登录
│   ├── RefreshTokenAsync                # 刷新 Token
│   ├── LogoutAsync                      # 退出登录
│   ├── GetUserByIdAsync                 # 获取用户信息
│   ├── GetUserListAsync                 # 获取用户列表
│   ├── UpdateUserAsync                  # 更新用户信息
│   ├── ChangePasswordAsync              # 修改密码
│   ├── UploadAvatarAsync                 # 上传头像
│   ├── SendVerificationCodeAsync          # 发送验证码
│   └── VerifyCodeAsync                  # 验证验证码
│
├── IProjectService.cs       # 项目服务接口
├── ProjectService.cs        # 项目服务实现
│   ├── CreateProjectAsync                # 创建项目
│   ├── UpdateProjectAsync                # 更新项目
│   ├── DeleteProjectAsync                # 删除项目
│   ├── GetProjectByIdAsync              # 获取项目详情
│   ├── GetUserProjectsAsync              # 获取用户项目列表
│   ├── AddProjectMemberAsync             # 添加项目成员
│   ├── RemoveProjectMemberAsync          # 移除项目成员
│   └── GetProjectStatisticsAsync        # 获取项目统计
│
├── ITaskService.cs          # 任务服务接口
├── TaskService.cs           # 任务服务实现
│   ├── CreateTaskAsync                   # 创建任务
│   ├── UpdateTaskAsync                   # 更新任务
│   ├── DeleteTaskAsync                   # 删除任务
│   ├── GetTaskByIdAsync                 # 获取任务详情
│   ├── GetProjectTasksAsync             # 获取项目任务列表
│   ├── GetUserTasksAsync                # 获取用户任务列表
│   ├── AssignTaskAsync                   # 分配任务
│   ├── ChangeTaskStatusAsync             # 更改任务状态
│   ├── BatchChangeTaskStatusAsync         # 批量更新状态
│   ├── AddTaskTagAsync                  # 添加任务标签
│   ├── RemoveTaskTagAsync               # 移除任务标签
│   └── GetTaskStatisticsAsync           # 获取任务统计
│
├── ICommentService.cs       # 评论服务接口
├── CommentService.cs        # 评论服务实现
│   ├── CreateCommentAsync                # 创建评论
│   ├── GetCommentByIdAsync              # 获取评论详情
│   ├── GetTaskCommentsAsync             # 获取任务评论列表
│   ├── UpdateCommentAsync                # 更新评论
│   ├── DeleteCommentAsync                # 删除评论
│   ├── ToggleCommentLikeAsync            # 切换点赞
│   ├── GetUserCommentsAsync             # 获取用户评论列表
│   └── BatchDeleteCommentsAsync          # 批量删除评论
│
└── IAttachmentService.cs    # 附件服务接口
    └── AttachmentService.cs     # 附件服务实现
        ├── UploadFileAsync               # 上传文件
        ├── UploadFilesBatchAsync          # 批量上传
        ├── GetAttachmentByIdAsync         # 获取附件详情
        ├── DownloadFileAsync             # 下载文件
        ├── GetTaskAttachmentsAsync       # 获取任务附件列表
        ├── GetProjectAttachmentsAsync    # 获取项目附件列表
        ├── GetUserAttachmentsAsync       # 获取用户附件列表
        ├── UpdateAttachmentAsync          # 更新附件信息
        ├── MoveAttachmentAsync           # 移动附件
        ├── DeleteAttachmentAsync          # 删除附件
        ├── BatchDeleteAttachmentsAsync   # 批量删除附件
        └── GetFileStatisticsAsync        # 获取文件统计
```

### DTOs/ - 数据传输对象

```
DTOs/
├── AuthDto.cs              # 认证相关 DTO
│   ├── RegisterDto           # 注册 DTO
│   ├── LoginDto              # 登录 DTO
│   ├── LoginResponseDto      # 登录响应 DTO
│   ├── RefreshTokenDto       # 刷新 Token DTO
│   ├── ChangePasswordDto     # 修改密码 DTO
│   ├── UpdateUserDto        # 更新用户 DTO
│   └── UserResponseDto       # 用户响应 DTO
│
├── ProjectDto.cs          # 项目相关 DTO
│   ├── CreateProjectDto      # 创建项目 DTO
│   ├── UpdateProjectDto      # 更新项目 DTO
│   ├── ProjectResponseDto    # 项目响应 DTO
│   ├── ProjectStatisticsDto # 项目统计 DTO
│   └── AddProjectMemberDto  # 添加项目成员 DTO
│
├── TaskDto.cs             # 任务相关 DTO
│   ├── CreateTaskDto        # 创建任务 DTO
│   ├── UpdateTaskDto        # 更新任务 DTO
│   ├── TaskResponseDto      # 任务响应 DTO
│   ├── TaskStatisticsDto    # 任务统计 DTO
│   └── TaskQueryDto         # 任务查询 DTO
│
└── CommentAndAttachmentDto.cs
    ├── CreateCommentDto           # 创建评论 DTO
    ├── UpdateCommentDto           # 更新评论 DTO
    ├── CommentResponseDto         # 评论响应 DTO
    ├── UploadFileDto             # 上传文件 DTO
    ├── AttachmentResponseDto      # 附件响应 DTO
    └── FileStatisticsDto         # 文件统计 DTO
```

### Entities/ - 数据实体模型

```
Entities/
├── BaseEntity.cs         # 实体基类
│   ├── Id                    # UUID 主键
│   ├── CreatedTime           # 创建时间
│   ├── UpdatedTime           # 更新时间
│   ├── IsDeleted            # 软删除标记
│   └── TenantId             # 租户 ID（多租户支持）
│
├── User.cs              # 用户实体
│   ├── Username              # 用户名
│   ├── Email                 # 邮箱
│   ├── PasswordHash          # 密码哈希
│   ├── Avatar                # 头像
│   ├── FullName             # 全名
│   ├── Phone                 # 手机号
│   ├── IsActive              # 是否激活
│   └── Role                  # 角色
│
├── Project.cs           # 项目实体
│   ├── Name                  # 项目名称
│   ├── Description           # 项目描述
│   ├── Key                   # 项目键
│   ├── OwnerId               # 创建者 ID
│   ├── Status                # 项目状态
│   ├── StartDate             # 开始日期
│   └── EndDate               # 结束日期
│
├── Task.cs              # 任务实体
│   ├── ProjectId             # 项目 ID
│   ├── TaskNumber            # 任务编号
│   ├── Title                 # 任务标题
│   ├── Description           # 任务描述
│   ├── Type                  # 任务类型
│   ├── Status                # 任务状态
│   ├── Priority              # 优先级
│   ├── AssigneeId            # 分配者 ID
│   ├── DueDate               # 截止日期
│   ├── Progress              # 进度 (0-100)
│   └── Tags                  # 标签 (JSONB)
│
├── Comment.cs            # 评论实体
│   ├── TaskId                # 任务 ID
│   ├── UserId                # 用户 ID
│   ├── Content               # 评论内容
│   ├── Mentions              # @提及 (JSONB)
│   ├── LikeCount             # 点赞数
│   └── LikedUsers            # 点赞用户列表
│
└── Attachment.cs         # 附件实体
    ├── TaskId                # 任务 ID
    ├── ProjectId             # 项目 ID
    ├── CommentId             # 评论 ID
    ├── UserId                # 上传者 ID
    ├── FileName              # 文件名
    ├── OriginalFileName       # 原始文件名
    ├── FilePath              # 文件路径
    ├── FileSize              # 文件大小
    ├── MimeType              # MIME 类型
    └── FileType              # 文件类型
```

### Core/ - 核心工具类

```
Core/
├── JwtHelper.cs        # JWT 工具类
│   ├── GenerateToken              # 生成 JWT Token
│   ├── ValidateToken             # 验证 Token
│   ├── RefreshToken              # 刷新 Token
│   └── DecodeToken                # 解码 Token
│
├── PasswordHelper.cs   # 密码工具类
│   ├── HashPassword               # 哈希密码
│   ├── VerifyPassword            # 验证密码
│   ├── GenerateRandomPassword      # 生成随机密码
│   └── CheckPasswordStrength      # 检查密码强度
│
├── FileHelper.cs       # 文件工具类
│   ├── UploadFile                 # 上传文件
│   ├── DeleteFile                # 删除文件
│   ├── FormatFileSize             # 格式化文件大小
│   ├── GetFileExtension          # 获取文件扩展名
│   ├── ValidateFileType          # 验证文件类型
│   └── GetUploadPath             # 获取上传路径
│
└── DataHelper.cs      # 数据工具类
    ├── GetPaginatedResult        # 获取分页结果
    ├── MapToDto                  # 映射到 DTO
    ├── HandlePagination          # 处理分页参数
    └── ValidateSortOrder         # 验证排序规则
```

### Filters/ - 过滤器

```
Filters/
├── GlobalAuthorizeFilter.cs    # 全局授权过滤器
│   ├── 匿名路径检查
│   ├── JWT Token 验证
│   ├── 用户激活状态检查
│   └── RBAC 权限检查
│
└── GlobalExceptionFilter.cs   # 全局异常过滤器
    ├── 统一错误响应格式
    ├── 异常类型识别
    ├── 错误日志记录
    └── 敏感信息脱敏
```

### 配置文件

```
├── Program.cs               # 程序入口
│   ├── 服务注册
│   ├── 中间件配置
│   ├── 数据库初始化
│   └── Swagger 配置
│
├── appsettings.json         # 应用配置
│   ├── ConnectionStrings       # 数据库连接
│   ├── JwtSettings           # JWT 配置
│   ├── CorsSettings          # CORS 配置
│   ├── FileUploadSettings     # 文件上传配置
│   └── LoggingSettings       # 日志配置
│
└── TaskFlow.Web.csproj      # 项目文件
    ├── NuGet 包引用
    ├── 编译配置
    └── 目标框架版本
```

---

## 🎨 前端结构详解

### src/components/ - React 可复用组件

```
components/
├── TaskCard.js          # 任务卡片组件
│   ├── 任务信息展示
│   ├── 优先级标签
│   ├── 状态标签
│   └── 进度条
│
├── KanbanBoard.js       # 看板视图组件
│   ├── 拖拽功能
│   ├── 任务列（待办/进行中/已完成/已取消）
│   └── 任务状态变更
│
├── CommentForm.js       # 评论表单组件
│   ├── 富文本编辑器
│   ├── @提及功能
│   └── 提交按钮
│
└── FileUpload.js        # 文件上传组件
    ├── 单文件上传
    ├── 批量上传
    ├── 文件列表展示
    └── 下载/预览/删除功能
```

### src/pages/ - 页面组件

```
pages/
├── LoginPage.js           # 登录/注册页面
│   ├── Tab 切换（登录/注册）
│   ├── 表单验证
│   └── 错误提示
│
├── DashboardPage.js      # 仪表板页面
│   ├── 统计卡片
│   ├── 任务进度
│   ├── 项目概览
│   └── 效率图表
│
├── ProjectsPage.js       # 项目列表页面
│   ├── 搜索和筛选
│   ├── 项目卡片网格
│   ├── 创建/编辑项目弹窗
│   └── 项目成员管理
│
├── TaskDetailPage.js     # 任务详情页面
│   ├── 任务信息展示
│   ├── 评论列表
│   ├── 附件列表
│   ├── 活动记录
│   └── 编辑任务弹窗
│
├── ProfilePage.js        # 个人资料页面
│   ├── 头像上传
│   ├── 基本信息修改
│   └── 密码修改
│
└── NotFoundPage.js      # 404 页面
```

### src/layouts/ - 布局组件

```
layouts/
├── MainLayout.js         # 主布局
│   ├── 侧边栏菜单（可折叠）
│   ├── 顶部导航栏
│   ├── 用户信息展示
│   ├── 通知徽章
│   └── 内容区域
│
└── AuthLayout.js         # 认证布局
│   └── 居中登录框
```

### src/services/ - API 服务层

```
services/
├── api.js                # Axios 基础配置
│   ├── 请求/响应拦截器
│   ├── Token 自动刷新
│   ├── 统一错误处理
│   └── 请求日志
│
├── authService.js        # 认证服务
│   ├── login
│   ├── register
│   ├── logout
│   ├── refreshToken
│   └── verifyToken
│
├── projectService.js     # 项目服务
│   ├── getProjects
│   ├── getProjectById
│   ├── createProject
│   ├── updateProject
│   └── deleteProject
│
├── taskService.js        # 任务服务
│   ├── getTasks
│   ├── getTaskById
│   ├── createTask
│   ├── updateTask
│   ├── deleteTask
│   └── changeTaskStatus
│
├── commentService.js     # 评论服务
│   ├── getComments
│   ├── getCommentById
│   ├── createComment
│   ├── updateComment
│   ├── deleteComment
│   └── likeComment
│
└── attachmentService.js  # 附件服务
    ├── uploadFile
    ├── downloadFile
    ├── getAttachments
    ├── deleteAttachment
    └── getFileStatistics
```

### src/store/ - Redux 状态管理

```
store/
├── index.js                     # Store 配置
│   ├── Combine Reducers
│   ├── 配置中间件
│   └── 导出 Store
│
├── authSlice.js                 # 认证状态
│   ├── user               # 当前用户
│   ├── token              # JWT Token
│   ├── isAuthenticated    # 认证状态
│   ├── loading            # 加载状态
│   └── error              # 错误信息
│
├── projectSlice.js              # 项目状态
│   ├── projects           # 项目列表
│   ├── currentProject     # 当前项目
│   ├── loading            # 加载状态
│   └── pagination         # 分页信息
│
├── taskSlice.js                 # 任务状态
│   ├── tasks              # 任务列表
│   ├── currentTask        # 当前任务
│   ├── filters            # 筛选条件
│   ├── loading            # 加载状态
│   └── pagination         # 分页信息
│
└── uiSlice.js                    # UI 状态
    ├── sidebarCollapsed    # 侧边栏折叠状态
    ├── theme              # 主题
    ├── loading            # 全局加载状态
    └── notification       # 通知列表
```

### src/utils/ - 工具函数

```
utils/
├── formatDate.js          # 日期格式化
├── formatFileSize.js      # 文件大小格式化
├── validate.js           # 表单验证
└── storage.js            # 本地存储工具
```

### public/ - 静态资源

```
public/
├── index.html             # HTML 入口文件
├── favicon.ico           # 网站图标
└── manifest.json          # PWA 清单（可选）
```

---

## 📁 数据流说明

### 后端数据流

```
请求 → Controller → Service → Repository → Database → Entity
                   ↓
              DTO ← Response ←
```

**各层职责**：
- **Controller**：接收 HTTP 请求，参数验证，调用 Service，返回响应
- **Service**：业务逻辑处理，调用 Repository，数据转换
- **Repository**：数据库操作（SqlSugar ORM）
- **DTO**：数据传输对象，用于接口交互
- **Entity**：数据库实体模型，与数据库表对应

### 前端数据流

```
用户交互 → Component → Action → Reducer → Store → Component
                              ↓
                           Middleware
```

**Redux 数据流**：
- **Action**：描述发生了什么
- **Reducer**：纯函数，更新状态
- **Store**：全局状态容器
- **Middleware**：中间件（Redux Thunk 用于异步操作）

---

## 🔒 安全结构

### 后端安全

- **Filters/GlobalAuthorizeFilter.cs** - JWT 认证，权限检查
- **Core/PasswordHelper.cs** - 密码哈希（BCrypt）
- **Core/JwtHelper.cs** - Token 生成和验证

### 前端安全

- **services/api.js** - Token 自动添加到请求头
- **Token 过期自动刷新**
- **请求统一错误处理**
- **XSS 防护（React 自动转义）

---

## 📊 文件依赖关系

### 后端依赖关系

```
Controllers
    ↓ (依赖)
Services → DTOs → Entities → Core
    ↓ (使用)
Filters
```

### 前端依赖关系

```
Pages
    ↓ (使用)
Components
    ↓ (使用)
Services → Store
    ↓ (依赖)
Utils
```

---

## 🎯 文件命名规范

### 后端命名规范

| 类型 | 规范 | 示例 |
|------|------|------|
| 控制器 | *Controller.cs | AuthController.cs |
| 服务接口 | I*Service.cs | IAuthService.cs |
| 服务实现 | *Service.cs | AuthService.cs |
| DTO | *Dto.cs | AuthDto.cs |
| 实体 | *.cs | User.cs |
| 过滤器 | *Filter.cs | GlobalAuthorizeFilter.cs |
| 工具类 | *Helper.cs | JwtHelper.cs |

### 前端命名规范

| 类型 | 规范 | 示例 |
|------|------|------|
| 组件 | PascalCase | TaskCard.js |
| 工具函数 | camelCase | formatDate.js |
| 常量 | UPPER_SNAKE_CASE | API_BASE_URL |
| 组件样式 | *.module.css | TaskCard.module.css |

---

**最后更新**：2026年2月10日  
**文档版本**：v1.0.0