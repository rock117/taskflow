using Furion.DependencyInjection;
using Furion.FriendlyException;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.DTOs;
using TaskFlow.Services;

namespace TaskFlow.Controllers
{
    /// <summary>
    /// 项目控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase, ITransient
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// 创建项目
        /// </summary>
        /// <param name="createProjectDto">创建项目DTO</param>
        /// <returns>项目信息</returns>
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto createProjectDto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    createProjectDto.OwnerId = guidId;
                    var project = await _projectService.CreateProjectAsync(createProjectDto);
                    return Ok(project);
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 更新项目
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="updateProjectDto">更新项目DTO</param>
        /// <returns>更新后的项目信息</returns>
        [HttpPut("{projectId}")]
        public async Task<IActionResult> UpdateProject(Guid projectId, [FromBody] UpdateProjectDto updateProjectDto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    updateProjectDto.CurrentUserId = guidId;
                    var project = await _projectService.UpdateProjectAsync(projectId, updateProjectDto);
                    if (project == null)
                    {
                        return NotFound(new { message = "项目不存在" });
                    }
                    return Ok(project);
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProject(Guid projectId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var result = await _projectService.DeleteProjectAsync(projectId, guidId);
                    if (!result)
                    {
                        return NotFound(new { message = "项目不存在或无权限删除" });
                    }
                    return Ok(new { message = "项目已删除" });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 获取项目详情
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <returns>项目详情</returns>
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetProject(Guid projectId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var project = await _projectService.GetProjectByIdAsync(projectId, guidId);
                    if (project == null)
                    {
                        return NotFound(new { message = "项目不存在" });
                    }
                    return Ok(project);
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 获取用户的项目列表
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="status">项目状态</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>项目列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetUserProjects(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? status = null,
            [FromQuery] string? keyword = null)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var (projects, total) = await _projectService.GetUserProjectsAsync(
                        guidId, pageIndex, pageSize, status, keyword);
                    return Ok(new { projects, total, pageIndex, pageSize });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 添加项目成员
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="addMemberDto">添加成员DTO</param>
        /// <returns>添加结果</returns>
        [HttpPost("{projectId}/members")]
        public async Task<IActionResult> AddProjectMember(Guid projectId, [FromBody] AddProjectMemberDto addMemberDto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    addMemberDto.CurrentUserId = guidId;
                    var result = await _projectService.AddProjectMemberAsync(projectId, addMemberDto);
                    if (!result)
                    {
                        return BadRequest(new { message = "添加成员失败" });
                    }
                    return Ok(new { message = "成员已添加" });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 移除项目成员
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="memberId">成员ID</param>
        /// <returns>移除结果</returns>
        [HttpDelete("{projectId}/members/{memberId}")]
        public async Task<IActionResult> RemoveProjectMember(Guid projectId, Guid memberId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var result = await _projectService.RemoveProjectMemberAsync(projectId, memberId, guidId);
                    if (!result)
                    {
                        return BadRequest(new { message = "移除成员失败" });
                    }
                    return Ok(new { message = "成员已移除" });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 获取项目统计信息
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <returns>统计信息</returns>
        [HttpGet("{projectId}/statistics")]
        public async Task<IActionResult> GetProjectStatistics(Guid projectId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var statistics = await _projectService.GetProjectStatisticsAsync(projectId, guidId);
                    if (statistics == null)
                    {
                        return NotFound(new { message = "项目不存在" });
                    }
                    return Ok(statistics);
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }
    }
}