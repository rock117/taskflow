using TaskFlow.Web.DTOs;
using TaskFlow.Web.Entities;

namespace TaskFlow.Web.Services;

/// <summary>
/// 任务服务接口
/// </summary>
public interface ITaskService : ITransient
{
    /// <summary>
    /// 获取任务列表（分页）
    /// </summary>
    /// <param name="dto">查询参数</param>
    /// <returns>分页的任务列表</returns>
    Task<PagedResultDto<TaskDto>> GetTasksAsync(TaskQueryDto dto);

    /// <summary>
    /// 获取任务详情
    /// </summary>
    /// <param name="taskId">任务 ID</param>
    /// <returns>任务详情</returns>
    Task<TaskDto?> GetTaskByIdAsync(string taskId);

    /// <summary>
    /// 创建新任务
    /// </summary>
    /// <param name="creatorId">创建者 ID</param>
    /// <param name="dto">任务信息</param>
    /// <returns>创建的任务</returns>
    Task<TaskDto> CreateTaskAsync(string creatorId, CreateTaskDto dto);

    /// <summary>
    /// 更新任务
    /// </summary>
    /// <param name="taskId">任务 ID</param>
    /// <param name="userId">用户 ID（用于权限验证）</param>
    /// <param name="dto">更新信息</param>
    /// <returns>更新后的任务</returns>
    Task<TaskDto> UpdateTaskAsync(string taskId, string userId, UpdateTaskDto dto);

    /// <summary>
    /// 删除任务
    /// </summary>
    /// <param name="taskId">任务 ID</param>
    /// <param name="userId">用户 ID（用于权限验证）</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteTaskAsync(string taskId, string userId);

    /// <summary>
    /// 批量删除任务
    /// </summary>
    /// <param name="taskIds">任务 ID 列表</param>
    /// <param name="userId">用户 ID（用于权限验证）</param>
    /// <returns>删除的任务数</returns>
    Task<int> BatchDeleteTasksAsync(List<string> taskIds, string userId);

    /// <summary>
    /// 更新任务状态
    /// </summary>
    /// <param name="taskId">任务 ID</param>
    /// <param name="userId">用户 ID（用于权限验证）</param>
    /// <param name="dto">状态信息</param>
    /// <returns>更新后的任务</returns>
    Task<TaskDto> UpdateTaskStatusAsync(string taskId, string userId, UpdateTaskStatusDto dto);

    /// <summary>
    /// 分配任务给用户
    /// </summary>
    /// <param name="taskId">任务 ID</param>
    /// <param name="userId">用户 ID（用于权限验证）</param>
    /// <param name="dto">分配信息</param>
    /// <returns>更新后的任务</returns>
    Task<TaskDto> AssignTaskAsync(string taskId, string userId, AssignTaskDto dto);

    /// <summary>
    /// 取消任务分配
    /// </summary>
    /// <param name="taskId">任务 ID</param>
    /// <param name="userId">用户 ID（用于权限验证）</param>
    /// <returns>更新后的任务</returns>
    Task<TaskDto> UnassignTaskAsync(string taskId, string userId);

    /// <summary>
    /// 完成任务
    /// </summary>
    /// <param name="taskId">任务 ID</param>
    /// <param name="userId">用户 ID（用于权限验证）</param>
    /// <param name="resolution">解决方案</param>
    /// <returns>更新后的任务</returns>
    Task<TaskDto> CompleteTaskAsync(string taskId, string userId, string? resolution = null);

    /// <summary>
    /// 重新打开任务
    /// </summary>
    /// <param name="taskId">任务 ID</param>
    /// <param name="userId">用户 ID（用于权限验证）</param>
    /// <returns>更新后的任务</returns>
    Task<TaskDto> ReopenTaskAsync(string taskId, string userId);

    /// <summary>
    /// 取消任务
    /// </summary>
    /// <param name="taskId">任务 ID</param>
    /// <param name="userId">用户 ID（用于权限验证）</param>
    /// <param name="reason">取消原因</param>
    /// <returns>更新后的任务</returns>
    Task<TaskDto> CancelTaskAsync(string taskId, string userId, string? reason = null);

    /// <summary>
    /// 获取分配给我的任务
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="dto">查询参数</param>
    /// <returns>分页的任务列表</returns>
    Task<PagedResultDto<TaskDto>> GetMyTasksAsync(string userId, TaskQueryDto dto);

    /// <summary>
    /// 获取我创建的任务
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="dto">查询参数</param>
    /// <returns>分页的任务列表</returns>
    Task<PagedResultDto<TaskDto>> GetCreatedTasksAsync(string userId, TaskQueryDto dto);

    /// <summary>
    /// 搜索任务
    /// </summary>
    /// <param name="query">搜索关键词</param>
    /// <param name="userId">用户 ID</param>
    /// <param name="limit">返回数量限制</param>
    /// <returns>任务列表</returns>
    Task<List<TaskDto>> SearchTasksAsync(string query, string? userId = null, int limit = 10);

    /// <summary>
    /// 获取项目任务
    /// </summary>
    /// <param name="projectId">项目 ID</param>
    /// <param name="dto">查询参数</param>
    /// <returns>分页的任务列表</returns>
    Task<PagedResultDto<TaskDto>> GetProjectTasksAsync(string projectId, TaskQueryDto dto);

    /// <summary>
    /// 批量更新任务
    /// </summary>
    /// <param name="dto">批量更新信息</param>
    /// <returns>更新的任务数</returns>
    Task<int> BatchUpdateTasksAsync(BatchUpdateTaskDto dto);

    /// <summary>
    /// 获取任务统计信息
    /// </summary>
    /// <param name="projectId">项目 ID</param>
    /// <returns>统计信息</returns>
    Task<TaskStatisticsDto> GetTaskStatisticsAsync(string projectId);

    /// <summary>
    /// 获取全局任务统计信息
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <returns>统计信息</returns>
    Task<Dictionary<string, int>> GetGlobalTaskStatisticsAsync(string userId);

    /// <summary>
    /// 获取过期任务
    /// </summary>
    /// <param name="projectId">项目 ID（可选）</param>
    /// <param name="userId">用户 ID（可选）</param>
    /// <returns>过期任务列表</returns>
    Task<List<TaskDto>> GetOverdueTasksAsync(string? projectId = null, string? userId = null);

    /// <summary>
    /// 检查用户是否有权限操作任务
    /// </summary>
    /// <param name="taskId">任务 ID</param>
    /// <param name="userId">用户 ID</param>
    /// <returns>是否有权限</returns>
    Task<bool> HasTaskPermissionAsync(string taskId, string userId);

    /// <summary>
    /// 添加任务标签
    /// </summary>
    /// <param name="taskId">任务 ID</param>
    /// <param name="tag">标签</param>
    /// <returns>更新后的任务</returns>
    Task<TaskDto> AddTagAsync(string taskId, string tag);

    /// <summary>
    /// 移除任务标签
    /// </summary>
    /// <param name="taskId">任务 ID</param>
    /// <param name="tag">标签</param>
    /// <returns>更新后的任务</returns>
    Task<TaskDto> RemoveTagAsync(string taskId, string tag);

    /// <summary>
    /// 更新任务工时
    /// </summary>
    /// <param name="taskId">任务 ID</param>
    /// <param name="userId">用户 ID</param>
    /// <param name="actualHours">实际工时</param>
    /// <returns>更新后的任务</returns>
    Task<TaskDto> UpdateTaskHoursAsync(string taskId, string userId, decimal actualHours);

    /// <summary>
    /// 复制任务
    /// </summary>
    /// <param name="taskId">要复制的任务 ID</param>
    /// <param name="newProjectId">新项目 ID（可选）</param>
    /// <param name="userId">用户 ID（用于权限验证）</param>
    /// <returns>新任务</returns>
    Task<TaskDto> CopyTaskAsync(string taskId, string? newProjectId, string userId);

    /// <summary>
    /// 移动任务到其他项目
    /// </summary>
    /// <param name="taskId">任务 ID</param>
    /// <param name="newProjectId">新项目 ID</param>
    /// <param name="userId">用户 ID（用于权限验证）</param>
    /// <returns>更新后的任务</returns>
    Task<TaskDto> MoveTaskAsync(string taskId, string newProjectId, string userId);

    /// <summary>
    /// 导出任务数据
    /// </summary>
    /// <param name="taskId">任务 ID</param>
    /// <returns>导出数据（JSON）</returns>
    Task<string> ExportTaskDataAsync(string taskId);

    /// <summary>
    /// 获取任务的活动日志
    /// </summary>
    /// <param name="taskId">任务 ID</param>
    /// <param name="limit">返回数量限制</param>
    /// <returns>活动日志列表</returns>
    Task<List<TaskActivityLogDto>> GetTaskActivityLogsAsync(string taskId, int limit = 20);

    /// <summary>
    /// 获取任务的依赖关系
    /// </summary>
    /// <param name="taskId">任务 ID</param>
    /// <returns>依赖关系信息</returns>
    Task<TaskDependencyDto> GetTaskDependenciesAsync(string taskId);
}
