# TaskFlow 开发指南

本文档为 TaskFlow 项目提供详细的开发指南，包括环境搭建、开发规范、调试技巧和常见问题解决。

---

## 🔧 环境搭建

### 后端环境搭建

#### 1. 安装 .NET SDK

**Windows**：
```bash
# 下载并安装 .NET 8.0 SDK
https://dotnet.microsoft.com/download/dotnet/8.0

# 验证安装
dotnet --version
# 应输出：8.0.xxx
```

**macOS**（使用 Homebrew）：
```bash
brew install --cask dotnet-sdk
dotnet --version
```

**Linux**（Ubuntu/Debian）：
```bash
# 添加 Microsoft 包存储库
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

# 安装 .NET SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

#### 2. 安装 PostgreSQL

**Windows**：
```bash
# 下载并安装 PostgreSQL
https://www.postgresql.org/download/windows/

# 或使用 Chocolatey
choco install postgresql
```

**macOS**：
```bash
brew install postgresql@14
brew services start postgresql@14
```

**Linux**：
```bash
sudo apt-get install postgresql postgresql-contrib
sudo systemctl start postgresql
```

#### 3. 创建数据库

```bash
# 以 postgres 用户登录
sudo -u postgres psql

# 创建数据库
CREATE DATABASE taskflow;

# 创建用户
CREATE USER taskflow_user WITH PASSWORD 'your_password';

# 授权
GRANT ALL PRIVILEGES ON DATABASE taskflow TO taskflow_user;

# 退出
\q
```

#### 4. 还原依赖

```bash
cd taskflow/backend
dotnet restore
```

#### 5. 配置数据库连接

编辑 `appsettings.json`：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=taskflow;Username=taskflow_user;Password=your_password"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-jwt-key-change-in-production-at-least-32-chars",
    "Issuer": "TaskFlow",
    "Audience": "TaskFlowUsers",
    "ExpirationInMinutes": 120
  },
  "FileUpload": {
    "MaxFileSize": "104857600",
    "AllowedExtensions": ".jpg,.jpeg,.png,.gif,.pdf,.doc,.docx,.xls,.xlsx,.txt,.zip"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:3001",
      "http://127.0.0.1:3000"
    ]
  }
}
```

#### 6. 运行项目

```bash
# 开发模式
dotnet run

# 发布模式
dotnet publish -c Release -o ./publish
```

访问 Swagger 文档：`http://localhost:5000/swagger`

---

### 前端环境搭建

#### 1. 安装 Node.js

**Windows/macOS/Linux**：
```bash
# 下载并安装 Node.js 16+
https://nodejs.org/

# 验证安装
node --version
npm --version
```

#### 2. 安装依赖

```bash
cd taskflow/frontend
npm install
```

#### 3. 配置环境变量（可选）

创建 `.env` 文件：

```env
REACT_APP_API_BASE_URL=http://localhost:5000/api
REACT_APP_ENVIRONMENT=development
```

#### 4. 运行项目

```bash
# 开发模式
npm start

# 生产模式构建
npm run build
```

访问前端应用：`http://localhost:3000`

---

## 📝 开发规范

### 后端开发规范

#### 1. 命名规范

| 类型 | 规范 | 示例 |
|------|------|------|
| 类名 | PascalCase | `AuthService.cs` |
| 接口 | I + PascalCase | `IAuthService.cs` |
| 方法名 | PascalCase | `GetUserByIdAsync()` |
| 属性 | PascalCase | `UserName` |
| 私有字段 | _camelCase | `_userId` |
| 常量 | UPPER_SNAKE_CASE | `MAX_RETRY_COUNT` |

#### 2. 代码注释规范

```csharp
/// <summary>
/// 类/方法的 XML 文档注释
/// </summary>
/// <param name="paramName">参数说明</param>
/// <returns>返回值说明</returns>

/// <summary>
/// 根据用户ID获取用户信息
/// </summary>
/// <param name="userId">用户ID</param>
/// <returns>用户信息</returns>
public async Task<UserDto> GetUserByIdAsync(Guid userId)
{
    // TODO: 添加缓存
    var user = await _userRepository.FirstOrDefaultAsync(u => u.Id == userId);
    return user.Adapt<UserDto>();
}
```

#### 3. 异常处理规范

```csharp
// 使用业务异常
if (user == null)
{
    throw new NotFoundException("User", userId.ToString());
}

if (!ModelState.IsValid)
{
    throw new ValidationException(ModelState);
}

// 全局异常过滤器会捕获并返回统一格式
```

### 前端开发规范

#### 1. 命名规范

| 类型 | 规范 | 示例 |
|------|------|------|
| 组件 | PascalCase | `TaskCard.js` |
| 文件名 | camelCase | `taskCard.js` |
| 函数/方法 | camelCase | `getUserById()` |
| 变量/常量 | camelCase | `userId`, `API_BASE_URL` |
| 类组件 | PascalCase | `const TaskCard = (props) => {...}` |
| 函数组件 | camelCase | `const taskCard = (props) => {...}` |

#### 2. 组件结构规范

```javascript
import React, { useState, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { message } from 'antd';
import { useNavigate } from 'react-router-dom';

const Component = ({ prop1, prop2 }) => {
  // 1. Hooks
  const [state, setState] = useState(null);
  const dispatch = useDispatch();
  const navigate = useNavigate();
  
  // 2. Selectors
  const { user } = useSelector(state => state.auth);
  
  // 3. Effects
  useEffect(() => {
    loadData();
  }, []);
  
  // 4. 事件处理函数
  const handleClick = async () => {
    try {
      await dispatch(someAction());
      message.success('操作成功');
    } catch (error) {
      message.error(error.message);
    }
  };
  
  // 5. 渲染
  return (
    <div>
      {/* JSX 内容 */}
    </div>
  );
};

export default Component;
```

#### 3. Redux 异步 Action 规范

```javascript
// Redux Toolkit 的 async Thunk action
export const loginUser = (credentials) => async (dispatch) => {
  try {
    dispatch(loginStart());
    const response = await authService.login(credentials);
    dispatch(loginSuccess(response.data));
  } catch (error) {
    dispatch(loginFailure(error.message));
  }
};
```

---

## 🐛 调试技巧

### 后端调试

#### 1. 使用 Visual Studio 断点调试

```csharp
// 在代码行号左侧点击，添加断点
// 按 F5 开始调试
// F10：逐过程序
// F11：进入函数
// Shift+F11：跳出函数
// Shift+F5：运行到光标位置
```

#### 2. 使用 Logger

```csharp
private readonly ILogger<AuthService> _logger;

public async Task<UserDto> GetUserByIdAsync(Guid userId)
{
    _logger.LogInformation("获取用户信息: {UserId}", userId);
    
    try
    {
        var user = await _userRepository.FirstOrDefaultAsync(u => u.Id == userId);
        _logger.LogInformation("用户信息获取成功: {UserId}", userId);
        return user.Adapt<UserDto>();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "获取用户信息失败: {UserId}", userId);
        throw;
    }
}
```

#### 3. 查看 SQL 日志

开发环境会自动输出 SQL 语句到控制台：

```
[SQL] SELECT * FROM "Users" WHERE "Id" = @Id0
[Parameters] Id0=uuid
```

#### 4. 使用 Swagger 测试 API

访问 `http://localhost:5000/swagger`：
- 可以直接在浏览器中测试 API
- 查看请求和响应格式
- 查看模型定义

### 前端调试

#### 1. 使用浏览器开发者工具

**Console**：
```javascript
console.log('Debug info:', data);
console.error('Error:', error);
console.table(users);
```

**Network 标签页**：
- 查看所有网络请求
- 查看请求/响应详情
- 复制 cURL 命令

#### 2. 使用 Redux DevTools

安装浏览器扩展：
- Chrome: Redux DevTools
- Firefox: Redux DevTools

查看状态树：
```
State
  auth
    user
    token
    isAuthenticated
    loading
  projects
    tasks
    ui
```

#### 3. 使用 React DevTools

查看组件树：
- Props 和 State
- Hooks 状态
- 组件性能分析

#### 4. 添加 Source Map

开发环境会自动生成 source map，可以在浏览器中直接调试源码。

---

## 🧪 测试指南

### 后端测试

#### 1. 单元测试

```csharp
using Xunit;
using FluentAssertions;
using TaskFlow.Services;
using TaskFlow.DTOs;

public class AuthServiceTests
{
    private readonly IAuthService _authService;
    
    public AuthServiceTests()
    {
        // 设置测试环境
        // 使用 TestServer 或 Mock
    }
    
    [Fact]
    public async Task Register_ValidInput_ReturnsSuccess()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!"
        };
        
        // Act
        var result = await _authService.RegisterAsync(registerDto);
        
        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }
    
    [Theory]
    [InlineData(null, "test@example.com", "Password123!")]
    [InlineData("testuser", null, "Password123!")]
    [InlineData("testuser", "test@example.com", null)]
    [InlineData("testuser", "test@example.com", "")]
    public async Task Register_InvalidInput_ThrowsException(
        string username, string email, string password)
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = username,
            Email = email,
            Password = password
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _authService.RegisterAsync(registerDto)
        );
    }
}
```

#### 2. 集成测试

```csharp
[Fact]
public async Task Login_WithValidCredentials_ReturnsToken()
{
    // Arrange
    var client = _factory.CreateClient();
    
    var loginDto = new LoginDto
    {
        Email = "test@example.com",
        Password = "Password123!"
    };
    
    // Act
    var response = await client.PostAsJsonAsync("/api/auth/login", loginDto);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    
    var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>();
    result.Success.Should().BeTrue();
    result.Data.Token.Should().NotBeNullOrEmpty();
}
```

### 前端测试

#### 1. 组件测试

```javascript
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { Provider } from 'react-redux';
import { configureStore } from '@reduxjs/toolkit';
import TaskCard from '../components/TaskCard';
import authSlice from '../store/authSlice';

// Mock store
const store = configureStore({
  reducer: {
    auth: authSlice
  }
});

describe('TaskCard Component', () => {
  it('renders task information', () => {
    const task = {
      id: '1',
      title: 'Test Task',
      status: 'pending',
      priority: 'high',
      progress: 50
    };
    
    render(
      <Provider store={store}>
        <TaskCard task={task} />
      </Provider>
    );
    
    expect(screen.getByText('Test Task')).toBeInTheDocument();
    expect(screen.getByText('pending')).toBeInTheDocument();
  });
  
  it('calls onEdit when edit button is clicked', async () => {
    const onEdit = jest.fn();
    const task = { id: '1', title: 'Test Task' };
    
    render(
      <Provider store={store}>
        <TaskCard task={task} onEdit={onEdit} />
      </Provider>
    );
    
    const editButton = screen.getByRole('button', { name: /edit/i });
    fireEvent.click(editButton);
    
    await waitFor(() => {
      expect(onEdit).toHaveBeenCalledTimes(1);
    });
  });
});
```

#### 2. Redux 测试

```javascript
import authReducer, { login, logout } from '../store/authSlice';
import { configureStore } from '@reduxjs/toolkit';

describe('authSlice', () => {
  it('should handle initial state', () => {
    const store = configureStore({ reducer: { auth: authReducer } });
    const state = store.getState().auth;
    
    expect(state).toEqual({
      user: null,
      token: null,
      isAuthenticated: false,
      loading: false,
      error: null
    });
  });
  
  it('should handle login', () => {
    const user = { id: '1', username: 'testuser' };
    const action = login.fulfilled(user);
    
    const state = authReducer(
      { user: null, token: null, isAuthenticated: false },
      action
    );
    
    expect(state.isAuthenticated).toBe(true);
    expect(state.user).toEqual(user);
  });
});
```

---

## 🔄 开发工作流

### Git 分支策略

```
main           # 主分支（生产环境）
├── develop     # 开发分支
└── feature/*   # 功能分支
```

**分支命名规范**：
- 功能分支：`feature/feature-name`
- 修复分支：`fix/bug-description`
- 热修复分支：`hotfix/critical-bug`

### 提交规范

```bash
# 提交信息格式
<type>(<scope>): <description>

# 类型
feat:     新功能
fix:      修复 Bug
docs:     文档更新
style:     代码格式调整
refactor:  代码重构
perf:      性能优化
test:      添加测试
chore:     构建/工具链/依赖更新

# 示例
feat(auth): add JWT token refresh functionality
fix(task): resolve issue with task status update
docs(api): update API documentation
```

### Code Review 流程

1. 创建 Pull Request
2. 自动运行 CI/CD 检查
3. 代码审查（至少 1 人审查）
4. 修改意见讨论
5. 合并到目标分支

---

## 📊 性能优化

### 后端性能优化

#### 1. 数据库查询优化

```csharp
// 使用索引
[SqlSugar.SugarColumn(IsPrimaryKey = true)]
public Guid Id { get; set; }

// 只查询需要的字段
var users = await _userRepository
    .Select(u => new { u.Id, u.Username, u.Email })
    .ToListAsync();

// 使用 Include 优化关联查询
var tasks = await _taskRepository
    .Include(t => t.Project)
    .Include(t => t.Assignee)
    .ToListAsync();
```

#### 2. 缓存策略

```csharp
// 使用内存缓存
private readonly IMemoryCache _cache;

public async Task<ProjectDto> GetProjectByIdAsync(Guid projectId)
{
    var cacheKey = $"project_{projectId}";
    
    if (_cache.TryGetValue(cacheKey, out ProjectDto cachedProject))
    {
        return cachedProject;
    }
    
    var project = await _projectRepository
        .FirstOrDefaultAsync(p => p.Id == projectId);
    
    var projectDto = project.Adapt<ProjectDto>();
    _cache.Set(cacheKey, projectDto, TimeSpan.FromMinutes(30));
    
    return projectDto;
}
```

### 前端性能优化

#### 1. 组件优化

```javascript
// 使用 React.memo 避免不必要的重渲染
export default React.memo(TaskCard);

// 使用 useMemo 缓存计算结果
const sortedTasks = useMemo(() => {
    return tasks.sort((a, b) => b.priority - a.priority);
}, [tasks]);

// 使用 useCallback 缓存回调函数
const handleEdit = useCallback((taskId) => {
    onEdit(taskId);
}, [onEdit]);
```

#### 2. 代码分割

```javascript
import { lazy, Suspense } from 'react';

// 路由级代码分割
const DashboardPage = lazy(() => import('./pages/DashboardPage'));
const ProjectsPage = lazy(() => import('./pages/ProjectsPage'));

// 使用 Suspense
<Suspense fallback={<Loading />}>
  <DashboardPage />
</Suspense>
```

#### 3. 虚拟列表

```javascript
// 长列表使用虚拟滚动
import { FixedSizeList } from 'react-window';

<FixedSizeList
  height={600}
  itemCount={tasks.length}
  itemSize={100}
  width={400}
>
  {({ index, style }) => (
    <div style={style}>
      <TaskCard task={tasks[index]} />
    </div>
  )}
</FixedSizeList>
```

---

## 🚀 常见问题解决

### 后端常见问题

#### 1. 数据库连接失败

```csharp
// 检查 appsettings.json 连接字符串格式
// Host=localhost;Port=5432;Database=taskflow;Username=postgres;Password=your_password

// 确保 PostgreSQL 服务已启动
// Windows: Services.msc -> PostgreSQL
// Linux: sudo systemctl status postgresql
```

#### 2. CORS 跨域错误

```csharp
// 在 appsettings.json 中配置 CORS
"Cors": {
  "AllowedOrigins": [
    "http://localhost:3000",
    "http://127.0.0.1:3000"
  ]
}

// 在 Program.cs 中启用 CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});
```

#### 3. JWT Token 验证失败

```csharp
// 检查 SecretKey 是否一致（前后端）
// 确保 Token 未过期
// 检查 ClockSkew 配置（设为 0）
```

### 前端常见问题

#### 1. API 请求 401

```javascript
// 检查 Token 是否存在
const token = localStorage.getItem('token');
if (!token) {
  // 跳转到登录页
  navigate('/login');
}

// 检查 Token 是否过期
// 在 Axios 拦截器中处理 401
if (error.response?.status === 401) {
  // 尝试刷新 Token
  await refreshToken();
}
```

#### 2. Redux 状态更新问题

```javascript
// 确保使用了不可变更新
const newState = { ...state, field: newValue };

// 在 reducer 中返回新状态
return newState;

// 不要直接修改状态
state.field = newValue; // ❌ 错误
```

#### 3. 组件不重新渲染

```javascript
// 检查依赖项数组
useEffect(() => {
  loadData();
}, [dependency]); // 确保正确包含所有依赖

// 使用 key 属性（列表）
tasks.map(task => <TaskCard key={task.id} task={task} />)
```

---

## 🔐 安全最佳实践

### 后端安全

1. **SQL 注入防护**
   - 使用 SqlSugar ORM，避免原生 SQL
   - 使用参数化查询

2. **密码安全**
   - 使用 BCrypt 哈希密码
   - 密码复杂度验证

3. **XSS 防护**
   - 输出时进行 HTML 编码
   - Content-Type 设置为 application/json

4. **CORS 配置**
   - 明确指定允许的源
   - 生产环境不要使用 `AllowAnyOrigin`

### 前端安全

1. **XSS 防护**
   - React 自动转义 JSX
   - 避免使用 `dangerouslySetInnerHTML`

2. **Token 存储**
   - 使用 localStorage
   - 设置合适的过期时间
   - HTTPS 环境使用 Secure Cookie

3. **输入验证**
   - 客户端和服务端双重验证
   - 使用 Ant Design 的验证规则

---

## 📚 推荐资源

### .NET 学习资源
- [Microsoft Learn](https://learn.microsoft.com/dotnet/)
- [.NET Blog](https://devblogs.microsoft.com/dotnet/)
- [Furion 文档](https://furion.baiqianlian.com/)
- [SqlSugar 文档](https://www.donet5.com/Home/SqlSugar)

### React 学习资源
- [React 官方文档](https://react.dev/)
- [Redux Toolkit 文档](https://redux-toolkit.js.org/)
- [Ant Design 文档](https://ant.design/)

### 工具推荐
- **IDE**: Visual Studio 2022 / VS Code
- **API 测试**: Postman / Insomnia
- **数据库工具**: pgAdmin / DBeaver
- **Git 客户端**: SourceTree / GitKraken

---

**最后更新**：2026年2月10日  
**文档版本**：v1.0.0