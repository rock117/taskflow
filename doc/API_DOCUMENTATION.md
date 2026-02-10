# TaskFlow API 文档

本文档详细描述 TaskFlow 项目的所有 RESTful API 端点。

**基础 URL**：`http://localhost:5000/api`  
**API 版本**：v1.0  
**认证方式**：JWT Bearer Token

---

## 🔑 认证说明

所有需要认证的 API 都需要在请求头中携带 JWT Token：

```
Authorization: Bearer <your-jwt-token>
```

## 📋 统一响应格式

所有 API 响应都遵循统一格式：

```json
{
  "success": true,
  "message": "操作成功",
  "data": {},
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

| 字段 | 类型 | 说明 |
|------|------|------|
| success | boolean | 请求是否成功 |
| message | string | 响应消息 |
| data | object/array | 返回的数据 |
| timestamp | long | 时间戳（毫秒） |
| traceId | string | 追踪 ID（用于日志查询） |

---

## 🔐 认证相关 API

### 1. 用户注册

**端点**：`POST /api/auth/register`  
**认证**：不需要

**请求体**：
```json
{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "Password123!",
  "fullName": "John Doe"
}
```

**字段说明**：
| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| username | string | 是 | 用户名（2-20字符） |
| email | string | 是 | 邮箱地址（必须有效） |
| password | string | 是 | 密码（至少6个字符） |
| fullName | string | 否 | 全名 |

**响应示例**：
```json
{
  "success": true,
  "message": "注册成功",
  "data": {
    "user": {
      "id": "uuid",
      "username": "johndoe",
      "email": "john@example.com",
      "fullName": "John Doe",
      "avatar": null,
      "isActive": true
    }
  },
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

### 2. 用户登录

**端点**：`POST /api/auth/login`  
**认证**：不需要

**请求体**：
```json
{
  "email": "john@example.com",
  "password": "Password123!"
}
```

**响应示例**：
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
      "email": "john@example.com",
      "avatar": "http://example.com/avatar.jpg",
      "role": "user"
    },
    "expiresIn": 7200
  },
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

### 3. 刷新 Token

**端点**：`POST /api/auth/refresh-token`  
**认证**：不需要

**请求体**：
```json
{
  "refreshToken": "refresh_token_here"
}
```

**响应示例**：
```json
{
  "success": true,
  "message": "Token刷新成功",
  "data": {
    "token": "new_jwt_token_here",
    "expiresIn": 7200
  },
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

### 4. 退出登录

**端点**：`POST /api/auth/logout`  
**认证**：需要

**请求体**：无

**响应示例**：
```json
{
  "success": true,
  "message": "退出登录成功",
  "data": null,
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

### 5. 验证 Token

**端点**：`GET /api/auth/verify`  
**认证**：需要

**响应示例**：
```json
{
  "success": true,
  "message": "Token有效",
  "data": {
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

---

## 👤 用户管理 API

### 1. 获取当前用户信息

**端点**：`GET /api/user/me`  
**认证**：需要

**响应示例**：
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "id": "uuid",
    "username": "johndoe",
    "email": "john@example.com",
    "fullName": "John Doe",
    "avatar": "http://example.com/avatar.jpg",
    "phone": "13800138000",
    "isActive": true,
    "createdAt": "2026-01-01T00:00:00Z"
  },
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

### 2. 更新用户信息

**端点**：`PUT /api/user/me`  
**认证**：需要

**请求体**：
```json
{
  "fullName": "John Updated Doe",
  "phone": "13800138001"
}
```

### 3. 修改密码

**端点**：`POST /api/user/change-password`  
**认证**：需要

**请求体**：
```json
{
  "currentPassword": "oldPassword123!",
  "newPassword": "newPassword456!"
}
```

### 4. 上传头像

**端点**：`POST /api/user/avatar`  
**认证**：需要

**请求体**：`multipart/form-data`
- `file`: 图片文件（JPG/PNG，最大2MB）

**响应示例**：
```json
{
  "success": true,
  "message": "头像上传成功",
  "data": {
    "avatarUrl": "http://localhost:5000/uploads/avatars/uuid.jpg"
  },
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

### 5. 获取用户列表（管理员）

**端点**：`GET /api/user/list`  
**认证**：需要

**查询参数**：
| 参数 | 类型 | 必填 | 默认值 | 说明 |
|------|------|------|--------|------|
| pageIndex | integer | 否 | 1 | 页索引 |
| pageSize | integer | 否 | 20 | 页大小 |
| keyword | string | 否 | - | 搜索关键词 |

**请求示例**：
```
GET /api/user/list?pageIndex=1&pageSize=20&keyword=john
```

---

## 📁 项目管理 API

### 1. 创建项目

**端点**：`POST /api/projects`  
**认证**：需要

**请求体**：
```json
{
  "name": "My New Project",
  "description": "Project description",
  "key": "MNP",
  "startDate": "2026-01-01",
  "endDate": "2026-12-31"
}
```

**字段说明**：
| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| name | string | 是 | 项目名称（1-100字符） |
| description | string | 是 | 项目描述（最多500字符） |
| key | string | 否 | 项目键（2-10字符，大写） |
| startDate | string | 否 | 开始日期（YYYY-MM-DD） |
| endDate | string | 否 | 结束日期（YYYY-MM-DD） |

### 2. 更新项目

**端点**：`PUT /api/projects/{projectId}`  
**认证**：需要

**路径参数**：
- `projectId` - 项目 UUID

**请求体**：
```json
{
  "name": "Updated Project Name",
  "description": "Updated description",
  "status": "active"
}
```

### 3. 删除项目

**端点**：`DELETE /api/projects/{projectId}`  
**认证**：需要

**路径参数**：
- `projectId` - 项目 UUID

**响应示例**：
```json
{
  "success": true,
  "message": "项目已删除",
  "data": null,
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

### 4. 获取项目详情

**端点**：`GET /api/projects/{projectId}`  
**认证**：需要

**路径参数**：
- `projectId` - 项目 UUID

**响应示例**：
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "id": "uuid",
    "name": "My Project",
    "description": "Project description",
    "key": "MP",
    "status": "active",
    "ownerId": "uuid",
    "ownerName": "John Doe",
    "memberCount": 5,
    "taskCount": 12,
    "startDate": "2026-01-01T00:00:00Z",
    "endDate": "2026-12-31T00:00:00Z",
    "createdAt": "2026-01-01T00:00:00Z",
    "updatedAt": "2026-01-01T00:00:00Z"
  },
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

### 5. 获取用户的项目列表

**端点**：`GET /api/projects`  
**认证**：需要

**查询参数**：
| 参数 | 类型 | 必填 | 默认值 | 说明 |
|------|------|------|--------|------|
| pageIndex | integer | 否 | 1 | 页索引 |
| pageSize | integer | 否 | 20 | 页大小 |
| status | string | 否 | - | 项目状态（active/pending/completed/archived） |
| keyword | string | 否 | - | 搜索关键词 |

**请求示例**：
```
GET /api/projects?status=active&keyword=test&pageIndex=1&pageSize=20
```

### 6. 添加项目成员

**端点**：`POST /api/projects/{projectId}/members`  
**认证**：需要

**请求体**：
```json
{
  "memberId": "user-uuid",
  "role": "member"
}
```

**字段说明**：
- `memberId` - 用户 UUID
- `role` - 角色（owner/admin/member/observer）

### 7. 移除项目成员

**端点**：`DELETE /api/projects/{projectId}/members/{memberId}`  
**认证**：需要

**路径参数**：
- `projectId` - 项目 UUID
- `memberId` - 成员 UUID

### 8. 获取项目统计信息

**端点**：`GET /api/projects/{projectId}/statistics`  
**认证**：需要

**响应示例**：
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "totalTasks": 100,
    "completedTasks": 60,
    "inProgressTasks": 30,
    "pendingTasks": 10,
    "overdueTasks": 5,
    "completionRate": 60
  },
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

---

## 📋 任务管理 API

### 1. 创建任务

**端点**：`POST /api/tasks`  
**认证**：需要

**请求体**：
```json
{
  "projectId": "project-uuid",
  "title": "Fix login bug",
  "description": "Users cannot login with email",
  "type": "bug",
  "priority": "high",
  "assigneeId": "user-uuid",
  "dueDate": "2026-02-15",
  "tags": ["urgent", "frontend"]
}
```

**字段说明**：
| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| projectId | string | 是 | 项目 UUID |
| title | string | 是 | 任务标题（1-200字符） |
| description | string | 否 | 任务描述 |
| type | string | 否 | 类型（bug/feature/task/improvement） |
| priority | string | 否 | 优先级（low/medium/high） |
| assigneeId | string | 否 | 分配者 UUID |
| dueDate | string | 否 | 截止日期（YYYY-MM-DD） |
| tags | array | 否 | 标签列表 |

### 2. 更新任务

**端点**：`PUT /api/tasks/{taskId}`  
**认证**：需要

**路径参数**：
- `taskId` - 任务 UUID

**请求体**：
```json
{
  "title": "Updated task title",
  "description": "Updated description",
  "status": "inProgress",
  "priority": "medium",
  "assigneeId": "new-user-uuid",
  "dueDate": "2026-02-20",
  "progress": 50,
  "tags": ["urgent", "backend", "api"]
}
```

### 3. 删除任务

**端点**：`DELETE /api/tasks/{taskId}`  
**认证**：需要

**路径参数**：
- `taskId` - 任务 UUID

### 4. 获取任务详情

**端点**：`GET /api/tasks/{taskId}`  
**认证**：需要

**路径参数**：
- `taskId` - 任务 UUID

**响应示例**：
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "id": "uuid",
    "projectId": "project-uuid",
    "projectName": "My Project",
    "title": "Fix login bug",
    "description": "Users cannot login with email",
    "type": "bug",
    "status": "inProgress",
    "priority": "high",
    "assigneeId": "user-uuid",
    "assigneeName": "John Doe",
    "assigneeAvatar": "http://example.com/avatar.jpg",
    "creatorId": "creator-uuid",
    "creatorName": "Jane Smith",
    "dueDate": "2026-02-15",
    "progress": 50,
    "tags": ["urgent", "frontend"],
    "createdAt": "2026-01-01T00:00:00Z",
    "updatedAt": "2026-01-10T00:00:00Z",
    "comments": [],
    "attachments": []
  },
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

### 5. 获取项目的任务列表

**端点**：`GET /api/tasks/project/{projectId}`  
**认证**：需要

**路径参数**：
- `projectId` - 项目 UUID

**查询参数**：
| 参数 | 类型 | 必填 | 默认值 | 说明 |
|------|------|------|--------|------|
| pageIndex | integer | 否 | 1 | 页索引 |
| pageSize | integer | 否 | 20 | 页大小 |
| status | string | 否 | - | 任务状态 |
| priority | string | 否 | - | 优先级 |
| assigneeId | string | 否 | - | 分配者 ID |
| keyword | string | 否 | - | 搜索关键词 |

**请求示例**：
```
GET /api/tasks/project/project-uuid?status=inProgress&priority=high
```

### 6. 获取用户的任务列表

**端点**：`GET /api/tasks/my-tasks`  
**认证**：需要

**查询参数**：同上

### 7. 分配任务

**端点**：`POST /api/tasks/{taskId}/assign`  
**认证**：需要

**请求体**：
```json
{
  "assigneeId": "user-uuid"
}
```

### 8. 更改任务状态

**端点**：`POST /api/tasks/{taskId}/status`  
**认证**：需要

**请求体**：
```json
{
  "status": "inProgress"
}
```

**状态值**：
- `pending` - 待处理
- `inProgress` - 进行中
- `completed` - 已完成
- `cancelled` - 已取消

### 9. 批量更新任务状态

**端点**：`POST /api/tasks/batch-status`  
**认证**：需要

**请求体**：
```json
{
  "taskIds": ["uuid1", "uuid2", "uuid3"],
  "status": "completed"
}
```

### 10. 添加任务标签

**端点**：`POST /api/tasks/{taskId}/tags`  
**认证**：需要

**请求体**：
```json
{
  "tag": "urgent"
}
```

### 11. 移除任务标签

**端点**：`DELETE /api/tasks/{taskId}/tags/{tag}`  
**认证**：需要

**路径参数**：
- `taskId` - 任务 UUID
- `tag` - 标签名称

### 12. 获取任务统计信息

**端点**：`GET /api/tasks/project/{projectId}/statistics`  
**认证**：需要

**响应示例**：
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "totalTasks": 100,
    "pendingTasks": 10,
    "inProgressTasks": 30,
    "completedTasks": 55,
    "cancelledTasks": 5,
    "tasksByPriority": {
      "high": 20,
      "medium": 50,
      "low": 30
    },
    "tasksByType": {
      "bug": 30,
      "feature": 40,
      "task": 20,
      "improvement": 10
    }
  },
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

---

## 💬 评论管理 API

### 1. 创建评论

**端点**：`POST /api/comments`  
**认证**：需要

**请求体**：
```json
{
  "taskId": "task-uuid",
  "content": "This task has been completed in the latest commit.",
  "mentions": ["@johndoe"]
}
```

**字段说明**：
- `taskId` - 任务 UUID
- `content` - 评论内容（富文本，最多1000字符）
- `mentions` - @提及的用户列表（可选）

### 2. 获取评论详情

**端点**：`GET /api/comments/{commentId}`  
**认证**：不需要

**路径参数**：
- `commentId` - 评论 UUID

**响应示例**：
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "id": "uuid",
    "taskId": "task-uuid",
    "taskTitle": "Fix login bug",
    "userId": "user-uuid",
    "userName": "John Doe",
    "userAvatar": "http://example.com/avatar.jpg",
    "content": "This task has been completed in the latest commit.",
    "mentions": ["@johndoe"],
    "likeCount": 5,
    "isLikedByCurrentUser": true,
    "createdAt": "2026-01-10T10:30:00Z",
    "updatedAt": "2026-01-10T10:30:00Z",
    "attachments": []
  },
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

### 3. 获取任务的评论列表

**端点**：`GET /api/comments/task/{taskId}`  
**认证**：不需要

**路径参数**：
- `taskId` - 任务 UUID

**查询参数**：
| 参数 | 类型 | 必填 | 默认值 | 说明 |
|------|------|------|--------|------|
| pageIndex | integer | 否 | 1 | 页索引 |
| pageSize | integer | 否 | 20 | 页大小 |

### 4. 更新评论

**端点**：`PUT /api/comments/{commentId}`  
**认证**：需要

**请求体**：
```json
{
  "content": "Updated comment content",
  "mentions": ["@janedoe"]
}
```

### 5. 删除评论

**端点**：`DELETE /api/comments/{commentId}`  
**认证**：需要

**路径参数**：
- `commentId` - 评论 UUID

### 6. 切换评论点赞状态

**端点**：`POST /api/comments/{commentId}/like`  
**认证**：需要

**路径参数**：
- `commentId` - 评论 UUID

**响应示例**：
```json
{
  "success": true,
  "message": "已点赞",
  "data": {
    "isLiked": true,
    "likeCount": 6
  },
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

### 7. 获取用户的评论列表

**端点**：`GET /api/comments/my-comments`  
**认证**：需要

**查询参数**：
| 参数 | 类型 | 必填 | 默认值 | 说明 |
|------|------|------|--------|------|
| pageIndex | integer | 否 | 1 | 页索引 |
| pageSize | integer | 否 | 20 | 页大小 |

### 8. 批量删除评论

**端点**：`POST /api/comments/batch-delete`  
**认证**：需要

**请求体**：
```json
{
  "commentIds": ["uuid1", "uuid2", "uuid3"]
}
```

---

## 📎 附件管理 API

### 1. 上传文件

**端点**：`POST /api/attachments/upload`  
**认证**：需要

**请求体**：`multipart/form-data`
- `file`: 文件对象
- `taskId`: 任务 UUID（可选）
- `projectId`: 项目 UUID（可选）
- `commentId`: 评论 UUID（可选）

**文件限制**：
- 最大文件大小：100MB
- 支持的文件类型：所有类型

**响应示例**：
```json
{
  "success": true,
  "message": "文件上传成功",
  "data": {
    "id": "uuid",
    "fileName": "document.pdf",
    "originalFileName": "My Document.pdf",
    "fileSize": 1024000,
    "formattedSize": "1.00 MB",
    "fileExtension": "pdf",
    "fileType": "document",
    "mimeType": "application/pdf",
    "filePath": "/uploads/documents/uuid.pdf",
    "downloadUrl": "/api/attachments/uuid/download",
    "previewUrl": "/api/attachments/uuid/preview",
    "uploadedBy": "user-uuid",
    "uploaderName": "John Doe",
    "uploaderAvatar": "http://example.com/avatar.jpg",
    "createdAt": "2026-01-10T10:30:00Z"
  },
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

### 2. 批量上传文件

**端点**：`POST /api/attachments/upload-batch`  
**认证**：需要

**请求体**：`multipart/form-data`
- `files`: 文件数组（多个文件）
- `taskId`: 任务 UUID（可选）
- `projectId`: 项目 UUID（可选）
- `commentId`: 评论 UUID（可选）

### 3. 获取附件详情

**端点**：`GET /api/attachments/{attachmentId}`  
**认证**：不需要

**路径参数**：
- `attachmentId` - 附件 UUID

### 4. 下载文件

**端点**：`GET /api/attachments/{attachmentId}/download`  
**认证**：不需要（公开接口）

**路径参数**：
- `attachmentId` - 附件 UUID

**响应**：文件流（二进制）

### 5. 预览文件

**端点**：`GET /api/attachments/{attachmentId}/preview`  
**认证**：需要

**路径参数**：
- `attachmentId` - 附件 UUID

**响应**：文件流（根据 MIME 类型返回）

**可预览的文件类型**：
- 图片：image/jpeg, image/png, image/gif, image/webp
- PDF：application/pdf
- 文本：text/*

### 6. 获取任务的附件列表

**端点**：`GET /api/attachments/task/{taskId}`  
**认证**：需要

**路径参数**：
- `taskId` - 任务 UUID

**响应示例**：
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "attachments": [
      {
        "id": "uuid",
        "fileName": "document.pdf",
        "fileSize": 1024000,
        "formattedSize": "1.00 MB",
        "downloadUrl": "/api/attachments/uuid/download",
        "isPreviewable": true,
        "createdAt": "2026-01-10T10:30:00Z"
      }
    ],
    "count": 1
  },
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

### 7. 获取项目的附件列表

**端点**：`GET /api/attachments/project/{projectId}`  
**认证**：需要

**路径参数**：
- `projectId` - 项目 UUID

### 8. 获取用户的附件列表

**端点**：`GET /api/attachments/my-attachments`  
**认证**：需要

**查询参数**：
| 参数 | 类型 | 必填 | 默认值 | 说明 |
|------|------|------|--------|------|
| pageIndex | integer | 否 | 1 | 页索引 |
| pageSize | integer | 否 | 20 | 页大小 |

### 9. 更新附件信息

**端点**：`PUT /api/attachments/{attachmentId}`  
**认证**：需要

**路径参数**：
- `attachmentId` - 附件 UUID

**请求体**：
```json
{
  "fileName": "New Filename.pdf"
}
```

### 10. 移动附件到其他任务

**端点**：`POST /api/attachments/{attachmentId}/move`  
**认证**：需要

**路径参数**：
- `attachmentId` - 附件 UUID

**请求体**：
```json
{
  "newTaskId": "new-task-uuid"
}
```

### 11. 删除附件

**端点**：`DELETE /api/attachments/{attachmentId}`  
**认证**：需要

**路径参数**：
- `attachmentId` - 附件 UUID

### 12. 批量删除附件

**端点**：`POST /api/attachments/batch-delete`  
**认证**：需要

**请求体**：
```json
{
  "attachmentIds": ["uuid1", "uuid2", "uuid3"]
}
```

### 13. 获取文件统计信息

**端点**：`GET /api/attachments/statistics`  
**认证**：需要

**响应示例**：
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "totalFiles": 150,
    "totalSize": 524288000,
    "formattedTotalSize": "500.00 MB",
    "imageCount": 80,
    "documentCount": 50,
    "otherCount": 20
  },
  "timestamp": 1234567890,
  "traceId": "uuid"
}
```

---

## ❌ 错误码

| 错误码 | 说明 |
|--------|------|
| 200 | 请求成功 |
| 400 | 请求参数错误 |
| 401 | 未认证（Token 无效或过期） |
| 403 | 无权限 |
| 404 | 资源不存在 |
| 409 | 资源冲突 |
| 422 | 验证失败 |
| 500 | 服务器内部错误 |

---

## 🔗 Swagger 文档

完整的交互式 API 文档可在以下地址访问：

**Swagger UI**：`http://localhost:5000/swagger`

Swagger 文档提供了：
- 所有 API 端点的详细说明
- 在线测试 API 的界面
- 请求和响应示例
- 模型定义

---

## 📝 注意事项

1. **认证 Token**：
   - Token 有效期为 2 小时（7200秒）
   - 请在 Token 过期前使用 RefreshToken 刷新
   - 每个请求都需要在 Header 中携带 Token

2. **文件上传**：
   - 单文件最大 100MB
   - 支持所有文件类型
   - 上传后的文件路径会返回在 `filePath` 字段

3. **分页**：
   - 所有列表接口都支持分页
   - `pageIndex` 从 1 开始
   - `pageSize` 建议不超过 100

4. **时间格式**：
   - 所有日期时间使用 ISO 8601 格式
   - 时区：UTC
   - 示例：`2026-01-01T00:00:00Z`

5. **软删除**：
   - 大部分删除操作是软删除（`isDeleted = true`）
   - 删除的资源不会在列表中显示，但仍存在于数据库中

---

**最后更新**：2026年2月10日  
**API 版本**：v1.0.0