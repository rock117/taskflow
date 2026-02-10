# TaskFlow - 快速启动指南

## 🚀 5分钟快速上手

### 前置要求
- Node.js 14+ 已安装
- PostgreSQL 12+ 已安装并运行
- Git（可选）

### 第一步：安装依赖

```bash
# 进入后端目录
cd taskflow/backend
npm install

# 进入前端目录（另开终端）
cd taskflow/frontend
npm install
```

### 第二步：数据库设置

1. 创建数据库
```sql
-- 登录 PostgreSQL
psql -U postgres

-- 创建数据库
CREATE DATABASE taskflow_db;

-- 退出
\q
```

2. 配置环境变量
```bash
# 在 backend 目录下
cp env.example .env

# 编辑 .env 文件，修改数据库密码
# Windows 用户使用记事本或其他编辑器打开
# DB_PASSWORD=你的postgres密码
```

### 第三步：启动应用

**终端1 - 启动后端**
```bash
cd taskflow/backend
npm run dev
```
后端将在 http://localhost:5000 运行

**终端2 - 启动前端**
```bash
cd taskflow/frontend
npm start
```
前端将在 http://localhost:3000 打开

### 第四步：开始使用

1. 打开浏览器访问 http://localhost:3000
2. 点击"注册"创建新账户
3. 登录系统
4. 创建你的第一个项目
5. 开始创建和管理任务！

## 📝 默认测试账户

如果你运行了种子数据：
```bash
cd backend
npm run db:seed
```

可以使用以下账户登录：
- **管理员**: admin@taskflow.com / admin123
- **普通用户**: user@taskflow.com / user123

## 🎯 核心功能快速体验

### 创建项目
1. 点击顶部"新建项目"按钮
2. 输入项目名称（如："网站重构"）
3. 项目会自动生成唯一标识（如：WZ-1）

### 创建任务
1. 进入项目
2. 点击"新建任务"
3. 选择任务类型（Bug/Feature/Task/Improvement）
4. 填写任务详情
5. 可以分配给其他用户

### 任务管理
- **拖拽**: 直接拖动任务卡片改变状态
- **快速操作**: 点击任务卡片查看详情
- **评论**: 在任务详情页添加评论
- **附件**: 支持上传文件和图片

## 🔧 常见问题

### 1. 数据库连接失败
**问题**: "Unable to connect to the database"
**解决**: 
- 确认 PostgreSQL 正在运行
- 检查 .env 文件中的数据库配置
- 确认数据库 taskflow_db 已创建

### 2. 端口被占用
**问题**: "Port 5000/3000 is already in use"
**解决**:
- 修改 .env 中的 PORT
- 或关闭占用端口的程序

### 3. npm install 失败
**问题**: 依赖安装失败
**解决**:
```bash
# 清理缓存
npm cache clean --force
# 删除 node_modules
rm -rf node_modules
# 重新安装
npm install
```

## 🛠️ 开发模式命令

### 后端命令
```bash
npm run dev          # 开发模式（热重载）
npm start           # 生产模式
npm run db:migrate  # 运行数据库迁移
npm run db:seed     # 填充测试数据
npm test            # 运行测试
```

### 前端命令
```bash
npm start           # 开发服务器
npm run build       # 构建生产版本
npm test            # 运行测试
npm run eject       # 弹出配置（谨慎）
```

## 📊 项目结构说明

```
taskflow/
├── backend/            # 后端 API
│   ├── src/
│   │   ├── models/     # 数据模型
│   │   ├── routes/     # API 路由
│   │   ├── middleware/ # 中间件
│   │   └── config/     # 配置文件
│   └── uploads/        # 上传文件存储
│
├── frontend/          # 前端应用
│   ├── src/
│   │   ├── components/ # React 组件
│   │   ├── pages/     # 页面组件
│   │   ├── services/  # API 服务
│   │   └── store/     # Redux 状态
│   └── public/        # 静态资源
│
└── docs/             # 文档
```

## 🌟 下一步

1. **探索功能**
   - 尝试不同的任务类型和优先级
   - 使用富文本编辑器编写任务描述
   - 上传附件和图片

2. **团队协作**
   - 邀请团队成员（需要他们注册）
   - 分配任务给不同成员
   - 使用评论进行沟通

3. **自定义配置**
   - 修改项目颜色和图标
   - 设置任务截止日期
   - 使用标签组织任务

## 💡 提示和技巧

- **快捷键**: 
  - `Ctrl + /` - 搜索
  - `N` - 新建任务（在项目页面）
  - `Esc` - 关闭弹窗

- **任务标识**: 每个任务都有唯一标识，如 `PROJ-123`

- **状态流转**: 
  - 待办 → 进行中 → 已完成
  - 任何状态都可以改为"已取消"

- **优先级颜色**:
  - 🔴 紧急 - 红色
  - 🟠 高 - 橙色  
  - 🟡 中 - 黄色
  - 🟢 低 - 绿色

## 📞 需要帮助？

- 查看完整文档：[README.md](README.md)
- 提交问题：在项目中创建 Issue
- 查看 API 文档：访问 http://localhost:5000/api-docs

---

🎉 **恭喜！** 你已经成功启动了 TaskFlow。开始管理你的项目吧！