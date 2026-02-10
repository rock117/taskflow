# TaskFlow - Modern Task Management System

[![Node.js](https://img.shields.io/badge/Node.js-v14%2B-green)](https://nodejs.org/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-v12%2B-blue)](https://www.postgresql.org/)
[![React](https://img.shields.io/badge/React-v18-61DAFB)](https://reactjs.org/)
[![License](https://img.shields.io/badge/License-MIT-yellow)](LICENSE)

TaskFlow 是一个现代化的项目任务管理系统，提供类似 JIRA 的功能，但更加轻量和易用。

## ✨ 功能特性

### 核心功能
- 🔐 **用户认证** - 安全的注册、登录系统（JWT 认证）
- 📁 **项目管理** - 创建、编辑项目，支持项目创建者管理
- 📋 **任务管理** - 完整的任务生命周期管理
  - 任务类型：Bug、Feature、Task、Improvement
  - 任务状态：待办、进行中、已完成、已取消
  - 任务优先级：低、中、高、紧急
- 💬 **评论系统** - 支持富文本评论，@提及用户
- 📎 **附件管理** - 文件上传、预览、下载
- 📊 **统计分析** - 任务统计、项目进度跟踪

### 技术特性
- 🎨 **现代化界面** - 基于 React + Ant Design 的响应式设计
- 📝 **富文本编辑** - 支持 Markdown 和所见即所得编辑
- 🔄 **实时更新** - WebSocket 支持的实时协作
- 🔒 **安全性** - 密码加密、SQL 注入防护、XSS 防护
- 📈 **高性能** - 数据库索引优化、缓存策略
- 🌍 **国际化** - 支持多语言（中文/英文）

## 🚀 快速开始

### 环境要求
- Node.js >= 14.0
- PostgreSQL >= 12.0
- npm >= 6.0 或 yarn >= 1.22

### 安装步骤

#### 1. 克隆项目
```bash
git clone https://github.com/yourusername/taskflow.git
cd taskflow
```

#### 2. 安装后端依赖
```bash
cd backend
npm install
```

#### 3. 配置数据库
创建 PostgreSQL 数据库：
```sql
CREATE DATABASE taskflow_db;
```

#### 4. 配置环境变量
复制环境变量示例文件并修改：
```bash
cp env.example .env
```

编辑 `.env` 文件，配置数据库连接和其他参数：
```env
# 数据库配置
DB_HOST=localhost
DB_PORT=5432
DB_NAME=taskflow_db
DB_USER=postgres
DB_PASSWORD=your_password

# JWT 密钥（请修改为随机字符串）
JWT_SECRET=your-super-secret-jwt-key-change-this-in-production

# 其他配置...
```

#### 5. 初始化数据库
```bash
# 运行数据库迁移
npm run db:migrate

# (可选) 添加示例数据
npm run db:seed
```

#### 6. 启动后端服务
```bash
# 开发模式
npm run dev

# 生产模式
npm start
```

后端服务将在 http://localhost:5000 启动

#### 7. 安装并启动前端
```bash
cd ../frontend
npm install
npm start
```

前端应用将在 http://localhost:3000 启动

## 📖 API 文档

### 认证相关 API

#### 注册用户
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "password123",
  "fullName": "John Doe"
}
```

#### 登录
```http
POST /api/auth/login
Content-Type: application/json

{
  "login": "john@example.com",
  "password": "password123"
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
  "key": "MNP"
}
```

#### 获取项目列表
```http
GET /api/projects
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
  "assigneeId": "uuid-of-user"
}
```

#### 更新任务状态
```http
PATCH /api/tasks/:taskId/status
Authorization: Bearer <token>
Content-Type: application/json

{
  "status": "in_progress"
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
- commentId: uuid-of-comment (optional)
```

## 📁 项目结构

```
taskflow/
├── backend/                  # 后端代码
│   ├── src/
│   │   ├── config/          # 配置文件
│   │   ├── controllers/    # 控制器
│   │   ├── models/         # 数据模型
│   │   ├── routes/         # 路由定义
│   │   ├── middleware/     # 中间件
│   │   ├── services/       # 业务逻辑
│   │   └── utils/          # 工具函数
│   ├── uploads/            # 上传文件存储
│   ├── tests/              # 测试文件
│   ├── .env                # 环境变量
│   ├── package.json        # 依赖配置
│   └── server.js           # 服务入口
│
├── frontend/               # 前端代码
│   ├── src/
│   │   ├── components/    # React 组件
│   │   ├── pages/        # 页面组件
│   │   ├── services/     # API 服务
│   │   ├── store/        # Redux 状态管理
│   │   ├── hooks/        # 自定义 Hooks
│   │   ├── utils/        # 工具函数
│   │   └── styles/       # 样式文件
│   ├── public/           # 静态资源
│   └── package.json      # 依赖配置
│
├── docs/                 # 项目文档
├── docker-compose.yml    # Docker 配置
└── README.md            # 项目说明
```

## 🔧 开发指南

### 数据库模型

#### Users 表
- `id` - UUID 主键
- `username` - 用户名（唯一）
- `email` - 邮箱（唯一）
- `password` - 密码（加密存储）
- `fullName` - 全名
- `avatar` - 头像
- `role` - 角色（admin/user）
- `isActive` - 是否激活
- `createdAt` - 创建时间
- `updatedAt` - 更新时间

#### Projects 表
- `id` - UUID 主键
- `name` - 项目名称
- `description` - 项目描述
- `key` - 项目键（唯一，大写）
- `creatorId` - 创建者 ID
- `status` - 状态（active/inactive/archived）
- `createdAt` - 创建时间
- `updatedAt` - 更新时间

#### Tasks 表
- `id` - UUID 主键
- `projectId` - 项目 ID
- `taskNumber` - 任务编号（项目内唯一）
- `type` - 类型（bug/feature/task/improvement）
- `title` - 标题
- `description` - 描述（富文本）
- `status` - 状态（todo/in_progress/done/cancelled）
- `priority` - 优先级（low/medium/high/urgent）
- `creatorId` - 创建者 ID
- `assigneeId` - 分配者 ID
- `dueDate` - 截止日期
- `createdAt` - 创建时间
- `updatedAt` - 更新时间

#### Comments 表
- `id` - UUID 主键
- `taskId` - 任务 ID
- `userId` - 用户 ID
- `content` - 内容（富文本）
- `parentId` - 父评论 ID（用于回复）
- `createdAt` - 创建时间
- `updatedAt` - 更新时间

#### Attachments 表
- `id` - UUID 主键
- `taskId` - 任务 ID（可选）
- `commentId` - 评论 ID（可选）
- `uploadedBy` - 上传者 ID
- `filename` - 文件名
- `originalName` - 原始文件名
- `filePath` - 文件路径
- `fileSize` - 文件大小
- `mimeType` - MIME 类型
- `createdAt` - 创建时间

### 开发命令

```bash
# 后端开发
cd backend
npm run dev              # 启动开发服务器（带热重载）
npm test                 # 运行测试
npm run lint            # 代码检查
npm run db:migrate      # 运行数据库迁移
npm run db:seed         # 填充示例数据

# 前端开发
cd frontend
npm start               # 启动开发服务器
npm run build          # 构建生产版本
npm test               # 运行测试
npm run lint           # 代码检查
```

## 🌐 部署

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

1. **后端部署**
```bash
cd backend
npm install --production
NODE_ENV=production npm start
```

2. **前端部署**
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

    # API 代理
    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }

    # 上传文件
    location /uploads {
        alias /var/www/taskflow/backend/uploads;
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
- 使用 ESLint 进行代码检查
- 遵循 Airbnb JavaScript 代码规范
- 提交信息格式：`type(scope): description`

## 📄 许可证

本项目采用 MIT 许可证 - 详见 [LICENSE](LICENSE) 文件

## 📞 联系我们

- 项目主页：[https://github.com/yourusername/taskflow](https://github.com/yourusername/taskflow)
- 问题反馈：[Issues](https://github.com/yourusername/taskflow/issues)
- 邮箱：taskflow@example.com

## 🙏 致谢

- [React](https://reactjs.org/)
- [Node.js](https://nodejs.org/)
- [PostgreSQL](https://www.postgresql.org/)
- [Ant Design](https://ant.design/)
- [Sequelize](https://sequelize.org/)
- [Express.js](https://expressjs.com/)

---

Made with ❤️ by TaskFlow Team