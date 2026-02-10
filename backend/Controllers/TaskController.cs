using Furion.DependencyInjection;
using Furion.FriendlyException;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.DTOs;
using TaskFlow.Services;

namespace TaskFlow.Controllers
{
    /// <summary>
    /// 任务控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase, ITransient
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="createTaskDto">创建任务DTO</param>
        /// <returns>任务信息</returns>
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    createTaskDto.CreatorId = guidId;
                    var task = await _taskService.CreateTaskAsync(createTaskDto);
                    return Ok(task);
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="updateTaskDto">更新任务DTO</param>
        /// <returns>更新后的任务信息</returns>
        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] UpdateTaskDto updateTaskDto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    updateTaskDto.CurrentUserId = guidId;
                    var task = await _taskService.UpdateTaskAsync(taskId, updateTaskDto);
                    if (task == null)
                    {
                        return NotFound(new { message = "任务不存在" });
                    }
                    return Ok(task);
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(Guid taskId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var result = await _taskService.DeleteTaskAsync(taskId, guidId);
                    if (!result)
                    {
                        return NotFound(new { message = "任务不存在或无权限删除" });
                    }
                    return Ok(new { message = "任务已删除" });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 获取任务详情
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>任务详情</returns>
        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTask(Guid taskId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var task = await _taskService.GetTaskByIdAsync(taskId, guidId);
                    if (task == null)
                    {
                        return NotFound(new { message = "任务不存在" });
                    }
                    return Ok(task);
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 获取项目的任务列表
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="status">任务状态</param>
        /// <param name="priority">优先级</param>
        /// <param name="assigneeId">指派给</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>任务列表</returns>
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetProjectTasks(
            Guid projectId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? status = null,
            [FromQuery] string? priority = null,
            [FromQuery] Guid? assigneeId = null,
            [FromQuery] string? keyword = null)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var (tasks, total) = await _taskService.GetProjectTasksAsync(
                        projectId, guidId, pageIndex, pageSize, status, priority, assigneeId, keyword);
                    return Ok(new { tasks, total, pageIndex, pageSize });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 获取用户的任务列表
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="status">任务状态</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>任务列表</returns>
        [HttpGet("my-tasks")]
        public async Task<IActionResult> GetMyTasks(
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
                    var (tasks, total) = await _taskService.GetUserTasksAsync(
                        guidId, pageIndex, pageSize, status, keyword);
                    return Ok(new { tasks, total, pageIndex, pageSize });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 分配任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="assigneeId">指派人ID</param>
        /// <returns>分配结果</returns>
        [HttpPost("{taskId}/assign")]
        public async Task<IActionResult> AssignTask(Guid taskId, [FromBody] dynamic request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    Guid assigneeId = request.assigneeId;
                    var result = await _taskService.AssignTaskAsync(taskId, assigneeId, guidId);
                    if (!result)
                    {
                        return BadRequest(new { message = "分配任务失败" });
                    }
                    return Ok(new { message = "任务已分配" });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 更改任务状态
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="status">新状态</param>
        /// <returns>更新结果</returns>
        [HttpPost("{taskId}/status")]
        public async Task<IActionResult> ChangeTaskStatus(Guid taskId, [FromBody] dynamic request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    string status = request.status;
                    var task = await _taskService.ChangeTaskStatusAsync(taskId, status, guidId);
                    if (task == null)
                    {
                        return BadRequest(new { message = "更改状态失败" });
                    }
                    return Ok(task);
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 批量更新任务状态
        /// </summary>
        /// <param name="request">批量更新请求</param>
        /// <returns>更新结果</returns>
        [HttpPost("batch-status")]
        public async Task<IActionResult> BatchChangeTaskStatus([FromBody] dynamic request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    List<Guid> taskIds = ((System.Text.Json.JsonElement)request.taskIds).EnumerateArray()
                        .Select(x => Guid.Parse(x.GetString())).ToList();
                    string status = request.status;
                    var count = await _taskService.BatchChangeTaskStatusAsync(taskIds, status, guidId);
                    return Ok(new { message = $"已更新 {count} 个任务的状态" });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 添加任务标签
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="tag">标签</param>
        /// <returns>添加结果</returns>
        [HttpPost("{taskId}/tags")]
        public async Task<IActionResult> AddTaskTag(Guid taskId, [FromBody] dynamic request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    string tag = request.tag;
                    var task = await _taskService.AddTaskTagAsync(taskId, tag, guidId);
                    if (task == null)
                    {
                        return BadRequest(new { message = "添加标签失败" });
                    }
                    return Ok(task);
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 移除任务标签
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="tag">标签</param>
        /// <returns>移除结果</returns>
        [HttpDelete("{taskId}/tags/{tag}")]
        public async Task<IActionResult> RemoveTaskTag(Guid taskId, string tag)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var task = await _taskService.RemoveTaskTagAsync(taskId, tag, guidId);
                    if (task == null)
                    {
                        return BadRequest(new { message = "移除标签失败" });
                    }
                    return Ok(task);
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 获取任务统计信息
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <returns>统计信息</returns>
        [HttpGet("project/{projectId}/statistics")]
        public async Task<IActionResult> GetTaskStatistics(Guid projectId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var statistics = await _taskService.GetTaskStatisticsAsync(projectId, guidId);
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