using TaskFlow.Web.DTOs;
using TaskFlow.Web.Entities;

namespace TaskFlow.Web.Services;

/// <summary>
/// 项目服务接口
/// </summary>
public interface IProjectService : ITransient
{
    /// <summary>
    /// 获取项目列表（分页）
    /// </summary>
    /// <param name="dto">查询参数</param>
    /// <returns>分页的项目列表</returns>
    Task<PagedResultDto<ProjectDto>> GetProjectsAsync(ProjectQueryDto dto);

    /// <summary>
    /// 获取项目详情
    /// </summary>
    /// <param name="projectId">项目 ID</param>
    /// <returns>项目详情</returns>
    Task<ProjectDto?> GetProjectByIdAsync(string projectId);

    /// <summary>
    /// 创建新项目
    /// </summary>
    /// <param name="creatorId">创建者 ID</param>
    /// <param name="dto">项目信息</param>
    /// <returns>创建的项目</returns>
    Task<ProjectDto> CreateProjectAsync(string creatorId, CreateProjectDto dto);

    /// <summary>
    /// 更新项目
    /// </summary>
    /// <param name="projectId">项目 ID</param>
    /// <param name="userId">用户 ID（用于权限验证）</param>
    /// <param name="dto">更新信息</param>
    /// <returns>更新后的项目</returns>
    Task<ProjectDto> UpdateProjectAsync(string projectId, string userId, UpdateProjectDto dto);

    /// <summary>
    /// 删除项目
    /// </summary>
    /// <param name="projectId">项目 ID</param>
    /// <param name="userId">用户 ID（用于权限验证）</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteProjectAsync(string projectId, string userId);

    /// <summary>
    /// 归档项目
    /// </summary>
    /// <param name="projectId">项目 ID</param>
    /// <param name="userId">用户 ID（用于权限验证）</param>
    /// <returns>更新后的项目</returns>
    Task<ProjectDto> ArchiveProjectAsync(string projectId, string userId);

    /// <summary>
    /// 激活项目
    /// </summary>
    /// <param name="projectId">项目 ID</param>
    /// <param name="userId">用户 ID（用于权限验证）</param>
    /// <returns>更新后的项目</returns>
    Task<ProjectDto> ActivateProjectAsync(string projectId, string userId);

    /// <summary>
    /// 获取项目的任务列表
    /// </summary>
    /// <param name="projectId">项目 ID</param>
    /// <param name="dto">查询参数</param>
    /// <returns>分页的任务列表</returns>
    Task<PagedResultDto<TaskDto>> GetProjectTasksAsync(string projectId, TaskQueryDto dto);

    /// <summary>
    /// 获取项目统计信息
    /// </summary>
    /// <param name="projectId">项目 ID</param>
    /// <returns>统计信息</returns>
    Task<ProjectStatisticsDto> GetProjectStatisticsAsync(string projectId);

    /// <summary>
    /// 检查项目键是否已存在
    /// </summary>
    /// <param name="projectKey">项目键</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsProjectKeyExistsAsync(string projectKey);

    /// <summary>
    /// 检查用户是否有权限访问项目
    /// </summary>
    /// <param name="projectId">项目 ID</param>
    /// <param name="userId">用户 ID</param>
    /// <returns>是否有权限</returns>
    Task<bool> HasAccessPermissionAsync(string projectId, string userId);

    /// <summary>
    /// 获取用户的项目列表（按类型分类）
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <returns>项目列表（按状态分类）</returns>
    Task<Dictionary<string, List<ProjectDto>>> GetUserProjectsByStatusAsync(string userId);

    /// <summary>
    /// 搜索项目
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="query">搜索关键词</param>
    /// <param name="limit">返回数量限制</param>
    /// <returns>项目列表</returns>
    Task<List<ProjectDto>> SearchProjectsAsync(string userId, string query, int limit = 10);

    /// <summary>
    /// 批量更新项目状态
    /// </summary>
    /// <param name="projectIds">项目 ID 列表</param>
    /// <param name="status">新状态</param>
    /// <returns>更新的项目数</returns>
    Task<int> BatchUpdateProjectStatusAsync(List<string> projectIds, string status);

    /// <summary>
    /// 获取活跃项目统计
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <returns>统计信息</returns>
    Task<Dictionary<string, int>> GetActiveProjectStatisticsAsync(string userId);

    /// <summary>
    /// 导出项目数据
    /// </summary>
    /// <param name="projectId">项目 ID</param>
    /// <returns>导出数据（JSON）</returns>
    Task<string> ExportProjectDataAsync(string projectId);
}
