# TaskFlow - 现代化项目任务管理系统

## 📋 项目概述

TaskFlow 是一个功能完整、现代化的项目任务管理系统，采用前后端分离架构，提供类似 JIRA 的核心功能，但更加轻量和易用。

### 核心特性

✨ **用户认证与授权**
- 用户注册和登录系统
- JWT 身份认证
- 基于角色的访问控制（RBAC）
- 密码加密存储（BCrypt）

📁 **项目管理**
- 创建、编辑、删除项目
- 项目创建者权限管理
- 项目状态管理（活跃/归档）
- 项目键自动生成（如 PROJ-123）
- 项目颜色和图标自定义

📋 **任务管理**
- 完整的任务生命周期管理
- 任务类型：Bug、Feature、Task、Improvement
- 任务状态：待办、进行中、已完成、已取消
- 任务优先级：低、中、高、紧急
- 任务分配给团队成员
- 任务截止日期和工时跟踪
- 任务标签和分类

💬 **评论系统**
- 富文本评论支持
- 评论回复功能
- @提及用户通知
- 表情反应
- 系统自动评论（状态变更记录）

📎 **附件管理**
- 多文件上传支持
- 图片预览
- 文件下载
- 附件分类（图片、文档、视频等）
- 文件大小和类型限制

🎨 **现代化界面**
- 基于 Ant Design 的响应式设计
- 看板视图和列表视图
- 拖拽式任务管理
- 深色/浅色主题切换
- 移动端友好

---

## 🏗️ 技术架构

### 整体架构

```
┌─────────────────────────────────────────────────────────┐
│                    Frontend (React)                  │
│  ┌─────────────┐  ┌──────────────┐  ┌──────────┐ │
│  │  Components │  │    Pages     │  │  Store   │ │
│  └─────────────┘  └──────────────┘  └──────────┘ │
└──────────────────────┬──────────────────────────────┘
                       │ HTTP/HTTPS
                       │ RESTful API
┌──────────────────────┴──────────────────────────────┐
│              Backend (.NET 8.0 / Furion)          │
│  ┌─────────────┐  ┌──────────────┐  ┌──────────┐ │
│  │ Controllers │→│   Services   │→│   ORM    │ │
│  └─────────────┘  └──────────────┘  └──────────┘ │
└──────────────────────┬──────────────────────────────┘
                       │ SQL
┌──────────────────────┴──────────────────────────────┐
│           PostgreSQL Database                       │
│  Users | Projects | Tasks | Comments | Attachments  │
└───────────────────────────────────────────────────────┘
```

### 前端技术栈

| 技术 | 版本 | 用途 |
|------|------|------|
| React | 18.2 | UI 框架 |
| Redux Toolkit | 2.0.1 | 状态管理 |
| Ant Design | 5.12.8 | UI 组件库 |
| React Router DOM | 6.21.1 | 路由管理 |
| Axios | 1.6.5 | HTTP 请求 |
| React-Quill | 2.0.0 | 富文本编辑器 |
| Day.js | 1.11.10 | 日期处理 |

### 后端技术栈

| 技术 | 版本 | 用途 |
|------|------|------|
| .NET | 8.0 | 核心框架 |
| Furion | 4.9.2 | 应用框架 |
| SqlSugar | 5.1.4.154 | ORM 框架 |
| Npgsql | 8.0.3 | PostgreSQL 驱动 |
| JWT Bearer | 8.0.0 | 身份认证 |
| FluentValidation | 11.3.0 | 数据验证 |
| BCrypt.Net-Next | 4.0.3 | 密码加密 |
| Swashbuckle.AspNetCore | 6.5.0 | Swagger API 文档 |

---

## 📁 项目结构

```
taskflow/
├── backend/                    # .NET 后端项目
│   ├── Entities/              # 实体层（数据模型）
│   │   ├── BaseEntity.cs     # 实体基类
│   │   ├── User.cs          # 用户实体
│   │   ├── Project.cs       # 项目实体
│   │   ├── Task.cs          # 任务实体
│   │   ├── Comment.cs       # 评论实体
│   │   └── Attachment.cs    # 附件实体
│   ├── DTOs/                 # 数据传输对象
│   ├── Services/             # 服务层（业务逻辑）
│   ├── Controllers/          # 控制器层（API 端点）
│   ├── Validators/           # 数据验证器
│   ├── Core/                 # 核心工具类
│   ├── Uploads/              # 文件上传目录
│   ├── Logs/                 # 日志目录
│   ├── Program.cs           # 程序入口
│   ├── appsettings.json     # 配置文件
│   └── TaskFlow.Web.csproj # 项目文件
│
├── frontend/                   # React 前端项目
│   ├── src/
│   │   ├── components/     # React 组件
│   │   ├── pages/         # 页面组件
│   │   ├── services/      # API 服务
│   │   ├── store/         # Redux 状态管理
│   │   ├── layouts/       # 布局组件
│   │   ├── utils/         # 工具函数
│   │   ├── styles/        # 样式文件
│   │   ├── App.js         # 主应用组件
│   │   └── index.js       # 入口文件
│   ├── public/             # 静态资源
│   └── package.json       # 依赖配置
│
├── docs/                     # 项目文档
│   ├── QUICK_START.md      # 快速启动指南
│   ├── backend/README.md   # 后端详细文档
│   └── PROJECT_OVERVIEW.md # 本文档
│
└── README.md               # 项目说明

```

---

## 🗄️ 数据库设计

### 核心数据表

#### 1. Users（用户表）
- 存储用户信息
- 包含认证和角色信息
- 支持软删除

#### 2. Projects（项目表）
- 存储项目信息
- 关联创建者
- 支持项目状态管理

#### 3. Tasks（任务表）
- 存储任务信息
- 关联项目和用户
- 支持任务状态和优先级

#### 4. Comments（评论表）
- 存储评论信息
- 关联任务和用户
- 支持回复和提及

#### 5. Attachments（附件表）
- 存储附件信息
- 关联任务和评论
- 支持文件分类

### 数据库关系图

```
Users (1) ──< Created >── (N) Projects
Users (1) ──< Created >── (N) Tasks
Users (1) ──< Assigned >── (N) Tasks
Users (1) ──< Created >── (N) Comments
Users (1) ──< Uploaded >── (N) Attachments

Projects (1) ──< Contains >── (N) Tasks
Tasks (1) ──< Has >── (N) Comments
Tasks (1) ──< Has >── (N) Attachments
Comments (1) ──< Has >── (N) Attachments
Comments (1) ──< Has Replies >── (N) Comments
```

---

## 🔌 API 接口

### 认证相关

- `POST /api/auth/register` - 用户注册
- `POST /api/auth/login` - 用户登录
- `POST /api/auth/logout` - 用户登出
- `GET /api/auth/me` - 获取当前用户
- `PUT /api/auth/me` - 更新用户信息
- `POST /api/auth/change-password` - 修改密码

### 项目管理

- `GET /api/projects` - 获取项目列表
- `POST /api/projects` - 创建项目
- `GET /api/projects/{id}` - 获取项目详情
- `PUT /api/projects/{id}` - 更新项目
- `DELETE /api/projects/{id}` - 删除项目
- `POST /api/projects/{id}/archive` - 归档项目
- `POST /api/projects/{id}/activate` - 激活项目

### 任务管理

- `GET /api/tasks` - 获取任务列表
- `POST /api/tasks` - 创建任务
- `GET /api/tasks/{id}` - 获取任务详情
- `PUT /api/tasks/{id}` - 更新任务
- `DELETE /api/tasks/{id}` - 删除任务
- `PATCH /api/tasks/{id}/status` - 更新任务状态
- `POST /api/tasks/{id}/assign` - 分配任务
- `POST /api/tasks/{id}/unassign` - 取消分配任务

### 评论管理

- `GET /api/comments/task/{taskId}` - 获取任务评论
- `POST /api/comments` - 创建评论
- `PUT /api/comments/{id}` - 更新评论
- `DELETE /api/comments/{id}` - 删除评论
- `POST /api/comments/{id}/reaction` - 添加表情反应
- `DELETE /api/comments/{id}/reaction` - 删除表情反应

### 附件管理

- `POST /api/attachments/upload` - 上传文件
- `GET /api/attachments/{id}` - 获取附件信息
- `GET /api/attachments/{id}/download` - 下载文件
- `GET /api/attachments/{id}/preview` - 预览文件
- `DELETE /api/attachments/{id}` - 删除附件

---

## 🚀 快速开始

### 前置要求

- Node.js 14+ 和 npm
- .NET 8.0 SDK
- PostgreSQL 12+

### 后端启动

```bash
cd backend

# 还原依赖包
dotnet restore

# 配置数据库连接
# 编辑 appsettings.json

# 运行项目
dotnet run

# 访问 API 文档
# http://localhost:5000/swagger
```

### 前端启动

```bash
cd frontend

# 安装依赖
npm install

# 启动开发服务器
npm start

# 访问应用
# http://localhost:3000
```

---

## 🎯 核心功能使用流程

### 1. 用户注册和登录

1. 访问 http://localhost:3000
2. 点击"注册"按钮
3. 填写用户信息（用户名、邮箱、密码）
4. 注册成功后自动登录
5. 或使用已有账户登录

### 2. 创建项目

1. 登录后进入仪表板
2. 点击"新建项目"按钮
3. 填写项目信息：
   - 项目名称
   - 项目描述（可选）
   - 项目键（自动生成）
   - 项目颜色和图标
4. 创建成功后进入项目

### 3. 创建任务

1. 在项目页面点击"新建任务"
2. 填写任务信息：
   - 任务类型（Bug/Feature/Task/Improvement）
   - 任务标题
   - 任务描述（富文本）
   - 优先级（低/中/高/紧急）
   - 分配给团队成员
   - 截止日期
3. 创建成功后任务出现在看板中

### 4. 任务管理

- **拖拽任务**：直接拖动任务卡片到不同状态列
- **编辑任务**：点击任务卡片查看和编辑详情
- **添加评论**：在任务详情页添加评论
- **上传附件**：支持上传图片、文档等文件
- **分配任务**：将任务分配给团队成员

### 5. 任务状态流转

```
待办 → 进行中 → 已完成
  ↓        ↓        ↓
  └── 已取消 ←────────┘
```

---

## 🔒 安全特性

### 认证安全
- JWT Token 认证
- Token 过期机制
- 密码 BCrypt 加密
- 登录失败限流

### 数据安全
- SQL 注入防护（SqlSugar 参数化查询）
- XSS 防护
- CORS 跨域控制
- 敏感数据脱敏

### 文件安全
- 文件类型白名单
- 文件大小限制
- 文件名随机化
- 病毒扫描（可选）

### API 安全
- HTTPS 强制（生产环境）
- 速率限制
- 请求签名验证
- 全局异常处理

---

## 📊 系统特性

### 性能优化
- 前端代码分割
- 组件懒加载
- API 响应缓存
- 数据库查询优化
- 图片压缩和懒加载

### 可扩展性
- 模块化架构
- 插件化设计
- 微服务就绪
- 水平扩展支持

### 监控和日志
- 结构化日志记录
- 错误追踪
- 性能监控
- 用户行为分析

---

## 🎨 UI/UX 特性

### 响应式设计
- 支持桌面端、平板、手机
- 自适应布局
- 触摸友好

### 主题支持
- 浅色/深色主题
- 自定义配色方案
- 主题持久化

### 国际化
- 支持中文/英文切换
- 日期时间本地化
- 数字格式化

### 交互体验
- 拖拽操作
- 快捷键支持
- 加载状态提示
- 操作反馈

---

## 🔧 配置说明

### 后端配置（appsettings.json）

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "PostgreSQL连接字符串"
  },
  "JwtSettings": {
    "SecretKey": "JWT密钥",
    "ExpirationMinutes": 10080
  },
  "FileUpload": {
    "MaxFileSize": 104857600
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000"]
  }
}
```

### 前端配置

```javascript
// src/services/api.js
const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL || 'http://localhost:5000/api',
  timeout: 30000
});
```

---

## 📝 开发指南

### 后端开发

1. **创建新的 API 端点**
   - 创建 DTO（数据传输对象）
   - 创建 Validator（验证器）
   - 创建 Service（服务层）
   - 创建 Controller（控制器）
   - 在 Program.cs 中注册服务

2. **添加新的数据库表**
   - 创建 Entity（实体类）
   - 配置 SqlSugar 特性
   - 运行项目自动创建表

3. **调试技巧**
   - 使用 Swagger 测试 API
   - 查看 SQL 日志
   - 使用断点调试

### 前端开发

1. **创建新页面**
   - 在 `src/pages/` 目录创建页面组件
   - 在 `App.js` 中添加路由
   - 在菜单中添加导航

2. **创建新组件**
   - 在 `src/components/` 目录创建组件
   - 使用 Ant Design 组件
   - 遵循组件命名规范

3. **状态管理**
   - 在 Redux store 中添加 slice
   - 定义 actions 和 reducers
   - 在组件中使用 useSelector 和 useDispatch

---

## 🚀 部署指南

### Docker 部署

```bash
# 构建并启动所有服务
docker-compose up -d

# 查看日志
docker-compose logs -f

# 停止服务
docker-compose down
```

### 手动部署

#### 后端部署
```bash
cd backend
dotnet publish -c Release
# 将 publish 目录部署到服务器
dotnet TaskFlow.Web.dll
```

#### 前端部署
```bash
cd frontend
npm run build
# 将 build 目录部署到静态服务器
```

---

## 📄 许可证

MIT License

---

## 👥 团队

- **项目经理**：负责项目规划和协调
- **后端开发**：.NET 开发团队
- **前端开发**：React 开发团队
- **UI/UX 设计**：界面和交互设计
- **测试工程师**：功能测试和质量保证

---

## 📞 联系方式

- 项目主页：https://github.com/yourusername/taskflow
- 问题反馈：https://github.com/yourusername/taskflow/issues
- 邮箱：support@taskflow.com

---

## 🎉 致谢

感谢所有为 TaskFlow 项目做出贡献的开发者和用户！

---

**Made with ❤️ by TaskFlow Team**