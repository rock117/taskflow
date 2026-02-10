# TaskFlow - 现代化任务管理系统

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![Furion](https://img.shields.io/badge/Furion-4.9.2-blue)](https://furion.baiqianlian.com/)
[![SqlSugar](https://img.shields.io/badge/SqlSugar-5.1.4-green)](https://www.donet5.com/Home/SqlSugar)
[![React](https://img.shields.io/badge/React-v18-61DAFB)](https://reactjs.org/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-v12-blue)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/License-MIT-yellow)](LICENSE)

TaskFlow 是一个现代化的项目任务管理系统，采用前后端分离架构，提供类似 JIRA 的核心功能，但更加轻量和易用。

## ✨ 功能特性

### 核心功能
- 🔐 **用户认证** - 安全的注册、登录系统（JWT Bearer 认证）
- 📁 **项目管理** - 创建、编辑项目，支持项目创建者管理
- 📋 **任务管理** - 完整的任务生命周期管理
  - 任务类型：Bug、Feature、Task、Improvement
  - 任务状态：待办、进行中、已完成、已取消
  - 任务优先级：低、中、高
- 💬 **评论系统** - 支持富文本评论，@提及用户
- 📎 **附件管理** - 文件上传、预览、下载
- 📊 **统计分析** - 任务统计、项目进度跟踪

### 技术特性
- 🎨 **现代化界面** - 基于 React 18 + Ant Design 5 的响应式设计
- 🔄 **Redux Toolkit** - 现代化的状态管理方案
- 🛡️ **JWT 认证** - 无状态认证，支持 Token 刷新
- 💾 **软删除** - 所有实体支持软删除，数据可恢复
- 📈 **CodeFirst** - SqlSugar 自动创建数据库表结构
- 🚀 **高性能** - 高性能 ORM + 数据库索引优化
- 🌐 **RESTful API** - 标准化的 API 接口设计

## 🚀 快速开始

### 环境要求

#### 后端
- .NET 8.0 SDK
- PostgreSQL 12+
- Windows/Linux/macOS

#### 前端
- Node.js 16+
- npm 8+ 或 yarn 1.22+

### 安装步骤

#### 1. 克隆项目
```bash
git clone https://github.com/yourusername/taskflow.git
cd taskflow
```

#### 2. 安装后端依赖
```bash
cd backend
dotnet restore
```

#### 3. 配置数据库
创建 PostgreSQL 数据库：
```sql
CREATE DATABASE taskflow;
```

修改 `backend/appsettings.json` 中的数据库连接字符串：
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=taskflow;Username=postgres;Password=your_password"
  }
}
```

#### 4. 初始化数据库
SqlSugar 使用 CodeFirst 模式，首次运行会自动创建表结构：
```bash
cd backend
dotnet run
# 首次启动会自动创建所有表，无需手动建表
```

#### 5. 启动后端服务
```bash
cd backend

# 开发模式
dotnet run

# 发布模式
dotnet publish -c Release -o ./publish
dotnet ./publish/TaskFlow.Web.dll
```

后端服务将在 `http://localhost:5000` 启动  
Swagger API 文档：`http://localhost:5000/swagger`

#### 6. 安装并启动前端
```bash
cd ../frontend
npm install

# 开发模式
npm start

# 生产模式
npm run build
```

前端应用将在 `http://localhost:3000` 启动

## 📖 API 文档

完整的 API 文档请查看：[API_DOCUMENTATION.md](API_DOCUMENTATION.md)

文档包含：
- 所有 API 端点的详细说明
- 请求和响应示例
- 统一响应格式说明
- 错误码说明
- Swagger 文档地址

---

## 📁 项目结构

完整的项目结构说明请查看：[PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md)

文档包含：
- 后端目录结构和文件说明
- 前端目录结构和文件说明
- 各层级的职责说明
- 数据流架构图
- 命名规范

---

## 🔧 开发指南

完整的开发指南请查看：[DEVELOPMENT_GUIDE.md](DEVELOPMENT_GUIDE.md)

文档包含：
- 环境搭建指南
- 开发规范说明
- 调试技巧
- 测试指南
- 性能优化建议
- 常见问题解决
- 安全最佳实践

#### 登录
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "Password123!"
}
```

响应示例：
```json
{
  "success": true,
  "message": "登录成功",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "refresh_token_here",
    "user": {
      "id": "uuid",
      "username": "johndoe",
      "email": "john@example.com"
    }
  },
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

### 项目相关 API

#### 创建项目
```http
POST /api/projects
Authorization: Bearer <token>
Content-Type: application/json

{
  "name": "My New Project",
  "description": "Project description",
  "startDate": "2026-01-01",
  "endDate": "2026-12-31"
}
```

#### 获取项目列表
```http
GET /api/projects?status=active&keyword=test
Authorization: Bearer <token>
```

### 任务相关 API

#### 创建任务
```http
POST /api/tasks
Authorization: Bearer <token>
Content-Type: application/json

{
  "projectId": "uuid-of-project",
  "title": "Fix login bug",
  "description": "Users cannot login with email",
  "type": "bug",
  "priority": "high",
  "assigneeId": "uuid-of-user",
  "dueDate": "2026-02-01"
}
```

#### 更新任务状态
```http
POST /api/tasks/{taskId}/status
Authorization: Bearer <token>
Content-Type: application/json

{
  "status": "inProgress"
}
```

### 评论相关 API

#### 添加评论
```http
POST /api/comments
Authorization: Bearer <token>
Content-Type: application/json

{
  "taskId": "uuid-of-task",
  "content": "This issue has been resolved in the latest commit."
}
```

### 附件相关 API

#### 上传附件
```http
POST /api/attachments/upload
Authorization: Bearer <token>
Content-Type: multipart/form-data

FormData:
- file: <binary>
- taskId: uuid-of-task (optional)
- projectId: uuid-of-project (optional)
```

## 📁 项目结构

```
taskflow/
├── backend/                      # .NET 后端项目
│   ├── Controllers/               # API 控制器
│   │   ├── AuthController.cs     # 认证控制器
│   │   ├── UserController.cs     # 用户控制器
│   │   ├── ProjectController.cs  # 项目控制器
│   │   ├── TaskController.cs     # 任务控制器
│   │   ├── CommentController.cs  # 评论控制器
│   │   └── AttachmentController.cs # 附件控制器
│   ├── Services/                 # 业务逻辑层
│   │   ├── AuthService.cs
│   │   ├── ProjectService.cs
│   │   ├── TaskService.cs
│   │   ├── CommentService.cs
│   │   └── AttachmentService.cs
│   ├── DTOs/                    # 数据传输对象
│   │   ├── AuthDto.cs
│   │   ├── ProjectDto.cs
│   │   ├── TaskDto.cs
│   │   └── CommentAndAttachmentDto.cs
│   ├── Entities/                 # 数据模型
│   │   ├── BaseEntity.cs        # 实体基类
│   │   ├── User.cs
│   │   ├── Project.cs
│   │   ├── Task.cs
│   │   ├── Comment.cs
│   │   └── Attachment.cs
│   ├── Core/                     # 核心工具类
│   │   ├── JwtHelper.cs
│   │   ├── PasswordHelper.cs
│   │   ├── FileHelper.cs
│   │   └── DataHelper.cs
│   ├── Filters/                  # 过滤器
│   │   ├── GlobalAuthorizeFilter.cs
│   │   └── GlobalExceptionFilter.cs
│   ├── Uploads/                  # 文件上传目录
│   ├── Program.cs                # 程序入口
│   ├── appsettings.json          # 应用配置
│   └── TaskFlow.Web.csproj       # 项目文件
│
├── frontend/                     # React 前端项目
│   ├── src/
│   │   ├── components/           # React 组件
│   │   │   ├── TaskCard.js
│   │   │   ├── KanbanBoard.js
│   │   │   ├── CommentForm.js
│   │   │   └── FileUpload.js
│   │   ├── pages/               # 页面组件
│   │   │   ├── LoginPage.js
│   │   │   ├── DashboardPage.js
│   │   │   ├── ProjectsPage.js
│   │   │   ├── TaskDetailPage.js
│   │   │   └── ProfilePage.js
│   │   ├── layouts/             # 布局组件
│   │   │   ├── MainLayout.js
│   │   │   └── AuthLayout.js
│   │   ├── services/            # API 服务
│   │   │   ├── api.js
│   │   │   ├── authService.js
│   │   │   ├── projectService.js
│   │   │   ├── taskService.js
│   │   │   ├── commentService.js
│   │   │   └── attachmentService.js
│   │   ├── store/               # Redux 状态管理
│   │   │   ├── authSlice.js
│   │   │   ├── projectSlice.js
│   │   │   ├── taskSlice.js
│   │   │   └── uiSlice.js
│   │   ├── App.js               # 主应用组件
│   │   ├── index.js             # React 入口
│   │   └── index.css            # 全局样式
│   ├── public/                   # 静态资源
│   └── package.json             # 依赖配置
│
├── docs/                        # 项目文档
│   ├── QUICK_START.md            # 快速启动指南
│   ├── PROJECT_OVERVIEW.md        # 项目架构总览
│   ├── PROJECT_SESSION.md         # 开发会话记录
│   └── PROJECT_COMPLETION_SUMMARY.md # 项目完成总结
└── README.md                    # 项目说明
```

## 🔧 开发指南

### 数据库模型

#### Users 表
- `Id` - UUID 主键
- `Username` - 用户名（唯一）
- `Email` - 邮箱（唯一）
- `PasswordHash` - 密码哈希（BCrypt 加密）
- `Avatar` - 头像
- `IsActive` - 是否激活
- `CreatedAt` - 创建时间
- `UpdatedAt` - 更新时间
- `IsDeleted` - 软删除标记

#### Projects 表
- `Id` - UUID 主键
- `Name` - 项目名称
- `Description` - 项目描述
- `OwnerId` - 创建者 ID
- `Status` - 状态
- `StartDate` - 开始日期
- `EndDate` - 结束日期
- `CreatedAt` - 创建时间
- `UpdatedAt` - 更新时间

#### Tasks 表
- `Id` - UUID 主键
- `ProjectId` - 项目 ID
- `Title` - 标题
- `Description` - 描述（富文本）
- `Type` - 类型
- `Status` - 状态（pending/inProgress/completed/cancelled）
- `Priority` - 优先级
- `AssigneeId` - 分配者 ID
- `DueDate` - 截止日期
- `Progress` - 进度 (0-100)
- `Tags` - 标签 (JSONB)
- `CreatedAt` - 创建时间
- `UpdatedAt` - 更新时间

#### Comments 表
- `Id` - UUID 主键
- `TaskId` - 任务 ID
- `UserId` - 用户 ID
- `Content` - 内容（富文本）
- `Mentions` - @提及用户 (JSON)
- `LikeCount` - 点赞数
- `CreatedAt` - 创建时间

#### Attachments 表
- `Id` - UUID 主键
- `TaskId` - 任务 ID
- `ProjectId` - 项目 ID
- `CommentId` - 评论 ID
- `UserId` - 上传者 ID
- `FileName` - 文件名
- `OriginalFileName` - 原始文件名
- `FilePath` - 文件路径
- `FileSize` - 文件大小
- `MimeType` - MIME 类型
- `CreatedAt` - 创建时间

### 开发命令

#### 后端开发
```bash
cd backend

# 还原依赖
dotnet restore

# 运行项目（开发环境，自动建表）
dotnet run

# 发布项目
dotnet publish -c Release -o ./publish

# 运行测试
dotnet test

# 查看项目信息
dotnet info
```

#### 前端开发
```bash
cd frontend

# 安装依赖
npm install

# 启动开发服务器
npm start

# 构建生产版本
npm run build

# 运行测试
npm test

# 代码检查
npm run lint
```

## 🌐 部署

### 后端部署

#### Docker 部署
```bash
cd backend
docker build -t taskflow-backend .
docker run -d -p 5000:5000 taskflow-backend
```

#### IIS 部署
1. 发布项目：`dotnet publish -c Release -o ./publish`
2. 将 `publish` 目录复制到 IIS 网站
3. 配置应用程序池（.NET CLR 版本：无托管代码）

#### Linux 部署
```bash
# 使用 systemd 服务
sudo nano /etc/systemd/system/taskflow.service

[Unit]
Description=TaskFlow API
After=network.target

[Service]
WorkingDirectory=/var/www/taskflow
ExecStart=/usr/bin/dotnet /var/www/taskflow/TaskFlow.Web.dll
Restart=always
RestartSec=10
SyslogIdentifier=taskflow
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target

# 启动服务
sudo systemctl enable taskflow
sudo systemctl start taskflow
```

### 前端部署

```bash
cd frontend
npm run build

# 将 build 目录部署到静态服务器（如 Nginx）
```

### Nginx 配置示例

```nginx
server {
    listen 80;
    server_name taskflow.example.com;

    # 前端静态文件
    location / {
        root /var/www/taskflow/frontend/build;
        try_files $uri /index.html;
    }

    # 后端 API 代理
    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }

    # Swagger 文档
    location /swagger {
        proxy_pass http://localhost:5000;
    }

    # 上传文件
    location /uploads {
        alias /var/www/taskflow/backend/Uploads;
    }
}
```

## 🤝 贡献指南

欢迎贡献代码！请遵循以下步骤：

1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

### 代码规范

#### 后端
- 遵循 C# 代码规范
- 使用 PascalCase 命名类、方法、属性
- 使用 camelCase 命名局部变量、参数
- 添加 XML 文档注释

#### 前端
- 遵循 ESLint 代码规范
- 使用函数组件和 Hooks
- 提交信息格式：`type(scope): description`

## 📄 许可证

本项目采用 MIT 许可证 - 详见 [LICENSE](LICENSE) 文件

## 📞 联系我们

- 项目主页：[https://github.com/yourusername/taskflow](https://github.com/yourusername/taskflow)
- 问题反馈：[Issues](https://github.com/yourusername/taskflow/issues)
- 邮箱：taskflow@example.com

## 🙏 致谢

- [.NET](https://dotnet.microsoft.com/)
- [Furion](https://furion.baiqianlian.com/)
- [SqlSugar](https://www.donet5.com/Home/SqlSugar)
- [React](https://reactjs.org/)
- [PostgreSQL](https://www.postgresql.org/)
- [Ant Design](https://ant.design/)

---

**生成时间**：2026年2月10日  
**项目版本**：v1.0.0  
**完成度**：92%  

Made with ❤️ by TaskFlow Team