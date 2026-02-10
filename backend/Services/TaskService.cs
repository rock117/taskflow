using SqlSugar;
using TaskFlow.Web.DTOs;
using TaskFlow.Web.Entities;
using TaskFlow.Web.Core;

namespace TaskFlow.Web.Services;

/// <summary>
/// 任务服务实现
/// </summary>
public class TaskService : ITaskService
{
    private readonly ISqlSugarClient _db;

    public TaskService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<PagedResultDto<TaskDto>> GetTasksAsync(TaskQueryDto dto)
    {
        var query = _db.Queryable<Task>()
            .Includes(t => t.Project)
            .Includes(t => t.Creator)
            .Includes(t => t.Assignee);

        // 项目筛选
        if (!string.IsNullOrEmpty(dto.ProjectId))
        {
            query = query.Where(t => t.ProjectId == dto.ProjectId);
        }

        // 状态筛选
        if (!string.IsNullOrEmpty(dto.Status))
        {
            query = query.Where(t => t.Status == dto.Status);
        }

        // 优先级筛选
        if (!string.IsNullOrEmpty(dto.Priority))
        {
            query = query.Where(t => t.Priority == dto.Priority);
        }

        // 类型筛选
        if (!string.IsNullOrEmpty(dto.Type))
        {
            query = query.Where(t => t.Type == dto.Type);
        }

        // 分配者筛选
        if (!string.IsNullOrEmpty(dto.AssigneeId))
        {
            query = query.Where(t => t.AssigneeId == dto.AssigneeId);
        }

        // 创建者筛选
        if (!string.IsNullOrEmpty(dto.CreatorId))
        {
            query = query.Where(t => t.CreatorId == dto.CreatorId);
        }

        // 分配给我的任务
        if (dto.AssignedToMe == true)
        {
            // 这里需要从上下文获取当前用户ID
            // query = query.Where(t => t.AssigneeId == currentUserId);
        }

        // 我创建的任务
        if (dto.CreatedByMe == true)
        {
            // query = query.Where(t => t.CreatorId == currentUserId);
        }

        // 搜索筛选
        if (!string.IsNullOrEmpty(dto.Search))
        {
            query = query.Where(t => t.Title.Contains(dto.Search) || t.Description.Contains(dto.Search));
        }

        // 截止日期范围筛选
        if (dto.DueDateFrom != null)
        {
            query = query.Where(t => t.DueDate >= dto.DueDateFrom);
        }
        if (dto.DueDateTo != null)
        {
            query = query.Where(t => t.DueDate <= dto.DueDateTo);
        }

        // 创建日期范围筛选
        if (dto.CreatedFrom != null)
        {
            query = query.Where(t => t.CreatedAt >= dto.CreatedFrom);
        }
        if (dto.CreatedTo != null)
        {
            query = query.Where(t => t.CreatedAt <= dto.CreatedTo);
        }

        // 标签筛选
        if (dto.Tags != null && dto.Tags.Count > 0)
        {
            query = query.Where(t => t.Tags.Contains(dto.Tags[0]));
        }

        // 排序
        query = dto.SortBy.ToLower() switch
        {
            "title" => dto.SortDirection == "asc"
                ? query.OrderBy(t => t.Title)
                : query.OrderByDescending(t => t.Title),
            "priority" => dto.SortDirection == "asc"
                ? query.OrderBy(t => t.Priority)
                : query.OrderByDescending(t => t.Priority),
            "status" => dto.SortDirection == "asc"
                ? query.OrderBy(t => t.Status)
                : query.OrderByDescending(t => t.Status),
            "created_at" => dto.SortDirection == "asc"
                ? query.OrderBy(t => t.CreatedAt)
                : query.OrderByDescending(t => t.CreatedAt),
            "due_date" => dto.SortDirection == "asc"
                ? query.OrderBy(t => t.DueDate)
                : query.OrderByDescending(t => t.DueDate),
            "task_number" => dto.SortDirection == "asc"
                ? query.OrderBy(t => t.TaskNumber)
                : query.OrderByDescending(t => t.TaskNumber),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };

        // 分页
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ToListAsync();

        return new PagedResultDto<TaskDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = dto.Page,
            PageSize = dto.PageSize
        };
    }

    public async Task<TaskDto?> GetTaskByIdAsync(string taskId)
    {
        var task = await _db.Queryable<Task>()
            .Includes(t => t.Project)
            .Includes(t => t.Creator)
            .Includes(t => t.Assignee)
            .Includes(t => t.Comments)
            .Includes(t => t.Attachments)
            .Where(t => t.Id == taskId)
            .FirstAsync();

        return task == null ? null : MapToDto(task);
    }

    public async Task<TaskDto> CreateTaskAsync(string creatorId, CreateTaskDto dto)
    {
        // 验证项目存在
        var project = await _db.Queryable<Project>()
            .Where(p => p.Id == dto.ProjectId && p.Status == "active")
            .FirstAsync();

        if (project == null)
        {
            throw new NotFoundException("项目不存在或已停用");
        }

        // 生成任务编号
        var lastTask = await _db.Queryable<Task>()
            .Where(t => t.ProjectId == dto.ProjectId)
            .OrderByDescending(t => t.TaskNumber)
            .FirstAsync();

        var taskNumber = lastTask != null ? lastTask.TaskNumber + 1 : 1;

        // 验证分配者
        if (!string.IsNullOrEmpty(dto.AssigneeId))
        {
            var assignee = await _db.Queryable<User>()
                .Where(u => u.Id == dto.AssigneeId && u.IsActive)
                .FirstAsync();

            if (assignee == null)
            {
                throw new NotFoundException("分配的用户不存在或已被禁用");
            }
        }

        // 创建任务
        var task = new Task
        {
            ProjectId = dto.ProjectId,
            TaskNumber = taskNumber,
            Type = dto.Type,
            Title = dto.Title,
            Description = dto.Description,
            Status = "todo",
            Priority = dto.Priority,
            CreatorId = creatorId,
            AssigneeId = dto.AssigneeId,
            DueDate = dto.DueDate,
            EstimatedHours = dto.EstimatedHours,
            ActualHours = dto.ActualHours,
            Tags = DataHelper.SerializeListToJson(dto.Tags),
            Labels = DataHelper.SerializeToJsonb(dto.Labels),
            Metadata = "{}",
            AttachmentCount = 0,
            CommentCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdTask = await _db.Insertable(task).ExecuteReturnEntityAsync();

        // 创建系统评论
        await _db.Insertable(new Comment
        {
            TaskId = createdTask.Id,
            UserId = creatorId,
            Content = "created this task",
            IsSystem = true,
            SystemAction = "task_created",
            CreatedAt = DateTime.UtcNow
        }).ExecuteCommandAsync();

        // 重新加载关联数据
        var taskWithRelations = await _db.Queryable<Task>()
            .Includes(t => t.Project)
            .Includes(t => t.Creator)
            .Includes(t => t.Assignee)
            .Where(t => t.Id == createdTask.Id)
            .FirstAsync();

        return MapToDto(taskWithRelations);
    }

    public async Task<TaskDto> UpdateTaskAsync(string taskId, string userId, UpdateTaskDto dto)
    {
        var task = await _db.Queryable<Task>()
            .Where(t => t.Id == taskId)
            .FirstAsync();

        if (task == null)
        {
            throw new NotFoundException("任务不存在");
        }

        // 权限检查：创建者或分配者可以更新
        if (task.CreatorId != userId && task.AssigneeId != userId)
        {
            throw new UnauthorizedAccessException("无权限更新此任务");
        }

        // 检查状态变更
        var statusChanged = dto.Status != null && dto.Status != task.Status;

        // 检查分配者变更
        var assigneeChanged = dto.AssigneeId != task.AssigneeId;

        // 更新字段
        if (dto.Title != null)
            task.Title = dto.Title;
        if (dto.Description != null)
            task.Description = dto.Description;
        if (dto.Type != null)
            task.Type = dto.Type;
        if (dto.Status != null)
            task.Status = dto.Status;
        if (dto.Priority != null)
            task.Priority = dto.Priority;
        if (dto.AssigneeId != null)
            task.AssigneeId = dto.AssigneeId;
        if (dto.DueDate != null)
            task.DueDate = dto.DueDate;
        if (dto.EstimatedHours != null)
            task.EstimatedHours = dto.EstimatedHours;
        if (dto.ActualHours != null)
            task.ActualHours = dto.ActualHours;
        if (dto.Tags != null)
            task.Tags = DataHelper.SerializeListToJson(dto.Tags);
        if (dto.Labels != null)
            task.Labels = DataHelper.SerializeToJsonb(dto.Labels);
        if (dto.Resolution != null)
            task.Resolution = dto.Resolution;
        if (dto.Metadata != null)
            task.Metadata = DataHelper.SerializeToJsonb(dto.Metadata);

        // 更新时间戳
        if (statusChanged && dto.Status == "done")
        {
            task.CompletedAt = DateTime.UtcNow;
        }
        else if (statusChanged && task.Status == "done" && dto.Status != "done")
        {
            task.CompletedAt = null;
        }

        if (statusChanged && dto.Status == "in_progress" && task.Status != "in_progress")
        {
            task.StartedAt = DateTime.UtcNow;
        }

        task.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(task).ExecuteCommandAsync();

        // 创建系统评论
        if (statusChanged)
        {
            await _db.Insertable(new Comment
            {
                TaskId = task.Id,
                UserId = userId,
                Content = $"changed status from {task.Status} to {dto.Status}",
                IsSystem = true,
                SystemAction = "status_changed",
                Metadata = DataHelper.SerializeToJsonb(new { From = task.Status, To = dto.Status }),
                CreatedAt = DateTime.UtcNow
            }).ExecuteCommandAsync();
        }

        if (assigneeChanged)
        {
            var assigneeName = !string.IsNullOrEmpty(dto.AssigneeId)
                ? (await _db.Queryable<User>().Where(u => u.Id == dto.AssigneeId).FirstAsync())?.FullName ?? ""
                : "";
            var action = string.IsNullOrEmpty(assigneeName)
                ? "unassigned this task"
                : $"assigned this task to {assigneeName}";

            await _db.Insertable(new Comment
            {
                TaskId = task.Id,
                UserId = userId,
                Content = action,
                IsSystem = true,
                SystemAction = "assignee_changed",
                Metadata = DataHelper.SerializeToJsonb(new { Assignee = assigneeName }),
                CreatedAt = DateTime.UtcNow
            }).ExecuteCommandAsync();
        }

        // 重新加载关联数据
        var updatedTask = await _db.Queryable<Task>()
            .Includes(t => t.Project)
            .Includes(t => t.Creator)
            .Includes(t => t.Assignee)
            .Where(t => t.Id == task.Id)
            .FirstAsync();

        return MapToDto(updatedTask);
    }

    public async Task<bool> DeleteTaskAsync(string taskId, string userId)
    {
        var task = await _db.Queryable<Task>()
            .Where(t => t.Id == taskId)
            .FirstAsync();

        if (task == null)
        {
            throw new NotFoundException("任务不存在");
        }

        // 权限检查
        if (task.CreatorId != userId)
        {
            throw new UnauthorizedAccessException("只有任务创建者可以删除任务");
        }

        // 软删除
        await _db.Deleteable(task)
            .IsLogic()
            .ExecuteCommandAsync();

        return true;
    }

    public async Task<int> BatchDeleteTasksAsync(List<string> taskIds, string userId)
    {
        // 检查权限
        var tasks = await _db.Queryable<Task>()
            .Where(t => taskIds.Contains(t.Id))
            .ToListAsync();

        // 只能删除自己创建的任务
        var allowedTaskIds = tasks.Where(t => t.CreatorId == userId).Select(t => t.Id).ToList();

        if (allowedTaskIds.Count == 0)
        {
            throw new UnauthorizedAccessException("没有可删除的任务");
        }

        // 批量软删除
        var count = await _db.Updateable<Task>()
            .SetColumns(new Dictionary<string, object>
            {
                { "DeletedAt", DateTime.UtcNow }
            })
            .Where(t => allowedTaskIds.Contains(t.Id))
            .ExecuteCommandAsync();

        return count;
    }

    public async Task<TaskDto> UpdateTaskStatusAsync(string taskId, string userId, UpdateTaskStatusDto dto)
    {
        var task = await _db.Queryable<Task>()
            .Where(t => t.Id == taskId)
            .FirstAsync();

        if (task == null)
        {
            throw new NotFoundException("任务不存在");
        }

        var oldStatus = task.Status;

        task.Status = dto.Status;
        task.UpdatedAt = DateTime.UtcNow;

        // 更新时间戳
        if (dto.Status == "done" && oldStatus != "done")
        {
            task.CompletedAt = DateTime.UtcNow;
        }
        else if (dto.Status == "in_progress" && oldStatus != "in_progress")
        {
            task.StartedAt = DateTime.UtcNow;
        }
        else if (oldStatus == "done" && dto.Status != "done")
        {
            task.CompletedAt = null;
        }

        await _db.Updateable(task).ExecuteCommandAsync();

        // 创建系统评论
        await _db.Insertable(new Comment
        {
            TaskId = task.Id,
            UserId = userId,
            Content = $"changed status from {oldStatus} to {dto.Status}",
            IsSystem = true,
            SystemAction = "status_changed",
            Metadata = DataHelper.SerializeToJsonb(new { From = oldStatus, To = dto.Status }),
            CreatedAt = DateTime.UtcNow
        }).ExecuteCommandAsync();

        // 如果完成，创建完成评论
        if (dto.Status == "done")
        {
            await _db.Insertable(new Comment
            {
                TaskId = task.Id,
                UserId = userId,
                Content = "marked this task as completed",
                IsSystem = true,
                SystemAction = "task_completed",
                CreatedAt = DateTime.UtcNow
            }).ExecuteCommandAsync();
        }

        // 重新加载关联数据
        var updatedTask = await _db.Queryable<Task>()
            .Includes(t => t.Project)
            .Includes(t => t.Creator)
            .Includes(t => t.Assignee)
            .Where(t => t.Id == task.Id)
            .FirstAsync();

        return MapToDto(updatedTask);
    }

    public async Task<TaskDto> AssignTaskAsync(string taskId, string userId, AssignTaskDto dto)
    {
        var task = await _db.Queryable<Task>()
            .Includes(t => t.Assignee)
            .Where(t => t.Id == taskId)
            .FirstAsync();

        if (task == null)
        {
            throw new NotFoundException("任务不存在");
        }

        var oldAssigneeId = task.AssigneeId;

        // 验证分配者存在
        var assignee = await _db.Queryable<User>()
            .Where(u => u.Id == dto.AssigneeId && u.IsActive)
            .FirstAsync();

        if (assignee == null)
        {
            throw new NotFoundException("分配的用户不存在或已被禁用");
        }

        task.AssigneeId = dto.AssigneeId;
        task.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(task).ExecuteCommandAsync();

        // 创建系统评论
        var assigneeName = assignee.FullName ?? assignee.Username;
        var action = oldAssigneeId == null
            ? $"assigned this task to {assigneeName}"
            : $"reassigned this task from {(await _db.Queryable<User>().Where(u => u.Id == oldAssigneeId).FirstAsync())?.FullName ?? ""} to {assigneeName}";

        await _db.Insertable(new Comment
        {
            TaskId = task.Id,
            UserId = userId,
            Content = action,
            IsSystem = true,
            SystemAction = "assignee_changed",
            Metadata = DataHelper.SerializeToJsonb(new { Assignee = assigneeName }),
            CreatedAt = DateTime.UtcNow
        }).ExecuteCommandAsync();

        // 重新加载关联数据
        var updatedTask = await _db.Queryable<Task>()
            .Includes(t => t.Project)
            .Includes(t => t.Creator)
            .Includes(t => t.Assignee)
            .Where(t => t.Id == task.Id)
            .FirstAsync();

        return MapToDto(updatedTask);
    }

    public async Task<TaskDto> UnassignTaskAsync(string taskId, string userId)
    {
        var task = await _db.Queryable<Task>()
            .Where(t => t.Id == taskId)
            .FirstAsync();

        if (task == null)
        {
            throw new NotFoundException("任务不存在");
        }

        task.AssigneeId = null;
        task.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(task).ExecuteCommandAsync();

        // 创建系统评论
        await _db.Insertable(new Comment
        {
            TaskId = task.Id,
            UserId = userId,
            Content = "unassigned this task",
            IsSystem = true,
            SystemAction = "assignee_changed",
            CreatedAt = DateTime.UtcNow
        }).ExecuteCommandAsync();

        // 重新加载关联数据
        var updatedTask = await _db.Queryable<Task>()
            .Includes(t => t.Project)
            .Includes(t => t.Creator)
            .Where(t => t.Id == task.Id)
            .FirstAsync();

        return MapToDto(updatedTask);
    }

    public async Task<TaskDto> CompleteTaskAsync(string taskId, string userId, string? resolution = null)
    {
        var task = await _db.Queryable<Task>()
            .Where(t => t.Id == taskId)
            .FirstAsync();

        if (task == null)
        {
            throw new NotFoundException("任务不存在");
        }

        var oldStatus = task.Status;

        task.Status = "done";
        task.CompletedAt = DateTime.UtcNow;
        task.Resolution = resolution ?? task.Resolution;
        task.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(task).ExecuteCommandAsync();

        // 创建系统评论
        await _db.Insertable(new Comment
        {
            TaskId = task.Id,
            UserId = userId,
            Content = "marked this task as completed",
            IsSystem = true,
            SystemAction = "task_completed",
            CreatedAt = DateTime.UtcNow
        }).ExecuteCommandAsync();

        var updatedTask = await _db.Queryable<Task>()
            .Includes(t => t.Project)
            .Includes(t => t.Creator)
            .Includes(t => t.Assignee)
            .Where(t => t.Id == task.Id)
            .FirstAsync();

        return MapToDto(updatedTask);
    }

    public async Task<TaskDto> ReopenTaskAsync(string taskId, string userId)
    {
        var task = await _db.Queryable<Task>()
            .Where(t => t.Id == taskId)
            .FirstAsync();

        if (task == null)
        {
            throw new NotFoundException("任务不存在");
        }

        task.Status = "todo";
        task.CompletedAt = null;
        task.Resolution = null;
        task.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(task).ExecuteCommandAsync();

        // 创建系统评论
        await _db.Insertable(new Comment
        {
            TaskId = task.Id,
            UserId = userId,
            Content = "reopened this task",
            IsSystem = true,
            SystemAction = "task_reopened",
            CreatedAt = DateTime.UtcNow
        }).ExecuteCommandAsync();

        var updatedTask = await _db.Queryable<Task>()
            .Includes(t => t.Project)
            .Includes(t => t.Creator)
            .Includes(t => t.Assignee)
            .Where(t => t.Id == task.Id)
            .FirstAsync();

        return MapToDto(updatedTask);
    }

    public async Task<TaskDto> CancelTaskAsync(string taskId, string userId, string? reason = null)
    {
        var task = await _db.Queryable<Task>()
            .Where(t => t.Id == taskId)
            .FirstAsync();

        if (task == null)
        {
            throw new NotFoundException("任务不存在");
        }

        var oldStatus = task.Status;

        task.Status = "cancelled";
        task.Resolution = reason ?? task.Resolution;
        task.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(task).ExecuteCommandAsync();

        // 创建系统评论
        await _db.Insertable(new Comment
        {
            TaskId = task.Id,
            UserId = userId,
            Content = reason ?? "cancelled this task",
            IsSystem = true,
            SystemAction = "task_cancelled",
            CreatedAt = DateTime.UtcNow
        }).ExecuteCommandAsync();

        var updatedTask = await _db.Queryable<Task>()
            .Includes(t => t.Project)
            .Includes(t => t.Creator)
            .Includes(t => t.Assignee)
            .Where(t => t.Id == task.Id)
            .FirstAsync();

        return MapToDto(updatedTask);
    }

    public async Task<PagedResultDto<TaskDto>> GetMyTasksAsync(string userId, TaskQueryDto dto)
    {
        dto.AssigneeId = userId;
        return await GetTasksAsync(dto);
    }

    public async Task<PagedResultDto<TaskDto>> GetCreatedTasksAsync(string userId, TaskQueryDto dto)
    {
        dto.CreatorId = userId;
        return await GetTasksAsync(dto);
    }

    public async Task<List<TaskDto>> SearchTasksAsync(string query, string? userId = null, int limit = 10)
    {
        var queryObj = _db.Queryable<Task>()
            .Includes(t => t.Project)
            .Includes(t => t.Creator)
            .Includes(t => t.Assignee)
            .Where(t => t.Title.Contains(query) || t.Description.Contains(query));

        if (!string.IsNullOrEmpty(userId))
        {
            queryObj = queryObj.Where(t => t.AssigneeId == userId || t.CreatorId == userId);
        }

        var tasks = await queryObj
            .OrderByDescending(t => t.CreatedAt)
            .Take(limit)
            .ToListAsync();

        return tasks.Select(MapToDto).ToList();
    }

    public async Task<PagedResultDto<TaskDto>> GetProjectTasksAsync(string projectId, TaskQueryDto dto)
    {
        dto.ProjectId = projectId;
        return await GetTasksAsync(dto);
    }

    public async Task<int> BatchUpdateTasksAsync(BatchUpdateTaskDto dto)
    {
        var count = await _db.Updateable<Task>()
            .SetColumns(new Dictionary<string, object>())
            .Where(t => dto.TaskIds.Contains(t.Id))
            .ExecuteCommandAsync();

        return count;
    }

    public async Task<TaskStatisticsDto> GetTaskStatisticsAsync(string projectId)
    {
        var tasks = await _db.Queryable<Task>()
            .Where(t => t.ProjectId == projectId)
            .ToListAsync();

        var statistics = new TaskStatisticsDto
        {
            TotalTasks = tasks.Count,
            TodoTasks = tasks.Count(t => t.Status == "todo"),
            InProgressTasks = tasks.Count(t => t.Status == "in_progress"),
            DoneTasks = tasks.Count(t => t.Status == "done"),
            CancelledTasks = tasks.Count(t => t.Status == "cancelled"),
            OverdueTasks = tasks.Count(t => t.DueDate != null && t.DueDate < DateTime.UtcNow.Date && t.Status != "done"),
            ByStatus = new Dictionary<string, int>(),
            ByPriority = new Dictionary<string, int>(),
            ByType = new Dictionary<string, int>(),
            ByAssignee = new Dictionary<string, int>(),
            TotalEstimatedHours = tasks.Sum(t => t.EstimatedHours ?? 0),
            TotalActualHours = tasks.Sum(t => t.ActualHours ?? 0)
        };

        // 统计
        foreach (var task in tasks)
        {
            if (statistics.ByStatus.ContainsKey(task.Status))
                statistics.ByStatus[task.Status]++;
            else
                statistics.ByStatus[task.Status] = 1;

            if (statistics.ByPriority.ContainsKey(task.Priority))
                statistics.ByPriority[task.Priority]++;
            else
                statistics.ByPriority[task.Priority] = 1;

            if (statistics.ByType.ContainsKey(task.Type))
                statistics.ByType[task.Type]++;
            else
                statistics.ByType[task.Type] = 1;

            if (!string.IsNullOrEmpty(task.AssigneeId))
            {
                if (statistics.ByAssignee.ContainsKey(task.AssigneeId))
                    statistics.ByAssignee[task.AssigneeId]++;
                else
                    statistics.ByAssignee[task.AssigneeId] = 1;
            }
        }

        return statistics;
    }

    public async Task<Dictionary<string, int>> GetGlobalTaskStatisticsAsync(string userId)
    {
        var myTasks = await _db.Queryable<Task>()
            .Where(t => t.AssigneeId == userId)
            .ToListAsync();

        var result = new Dictionary<string, int>
        {
            ["Total"] = myTasks.Count,
            ["Todo"] = myTasks.Count(t => t.Status == "todo"),
            ["InProgress"] = myTasks.Count(t => t.Status == "in_progress"),
            ["Done"] = myTasks.Count(t => t.Status == "done"),
            ["Overdue"] = myTasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != "done")
        };

        return result;
    }

    public async Task<List<TaskDto>> GetOverdueTasksAsync(string? projectId = null, string? userId = null)
    {
        var query = _db.Queryable<Task>()
            .Includes(t => t.Project)
            .Includes(t => t.Creator)
            .Includes(t => t.Assignee)
            .Where(t => t.DueDate < DateTime.UtcNow)
            .Where(t => t.Status != "done");

        if (!string.IsNullOrEmpty(projectId))
        {
            query = query.Where(t => t.ProjectId == projectId);
        }

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(t => t.AssigneeId == userId);
        }

        var tasks = await query
            .OrderBy(t => t.DueDate)
            .ToListAsync();

        return tasks.Select(MapToDto).ToList();
    }

    public async Task<bool> HasTaskPermissionAsync(string taskId, string userId)
    {
        var task = await _db.Queryable<Task>()
            .Where(t => t.Id == taskId)
            .FirstAsync();

        if (task == null)
            return false;

        // 创建者、分配者或项目创建者有权限
        if (task.CreatorId == userId || task.AssigneeId == userId)
            return true;

        // TODO: 检查项目成员权限

        return false;
    }

    public async Task<TaskDto> AddTagAsync(string taskId, string tag)
    {
        var task = await _db.Queryable<Task>()
            .Where(t => t.Id == taskId)
            .FirstAsync();

        if (task == null)
        {
            throw new NotFoundException("任务不存在");
        }

        var tags = DataHelper.DeserializeListFromJson<string>(task.Tags);
        if (!tags.Contains(tag))
        {
            tags.Add(tag);
            task.Tags = DataHelper.SerializeListToJson(tags);
            task.UpdatedAt = DateTime.UtcNow;

            await _db.Updateable(task).ExecuteCommandAsync();
        }

        var updatedTask = await _db.Queryable<Task>()
            .Where(t => t.Id == taskId)
            .FirstAsync();

        return MapToDto(updatedTask);
    }

    public async Task<TaskDto> RemoveTagAsync(string taskId, string tag)
    {
        var task = await _db.Queryable<Task>()
            .Where(t => t.Id == taskId)
            .FirstAsync();

        if (task == null)
        {
            throw new NotFoundException("任务不存在");
        }

        var tags = DataHelper.DeserializeListFromJson<string>(task.Tags);
        if (tags.Contains(tag))
        {
            tags.Remove(tag);
            task.Tags = DataHelper.SerializeListToJson(tags);
            task.UpdatedAt = DateTime.UtcNow;

            await _db.Updateable(task).ExecuteCommandAsync();
        }

        var updatedTask = await _db.Queryable<Task>()
            .Where(t => t.Id == taskId)
            .FirstAsync();

        return MapToDto(updatedTask);
    }

    public async Task<TaskDto> UpdateTaskHoursAsync(string taskId, string userId, decimal actualHours)
    {
        var task = await _db.Queryable<Task>()
            .Where(t => t.Id == taskId)
            .FirstAsync();

        if (task == null)
        {
            throw new NotFoundException("任务不存在");
        }

        // 权限检查
        if (task.AssigneeId != userId)
        {
            throw new UnauthorizedAccessException("只有任务分配者可以更新工时");
        }

        task.ActualHours = actualHours;
        task.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(task).ExecuteCommandAsync();

        var updatedTask = await _db.Queryable<Task>()
            .Where(t => t.Id == taskId)
            .FirstAsync();

        return MapToDto(updatedTask);
    }

    public async Task<TaskDto> CopyTaskAsync(string taskId, string? newProjectId, string userId)
    {
        var originalTask = await _db.Queryable<Task>()
            .Includes(t => t.Project)
            .Where(t => t.Id == taskId)
            .FirstAsync();

        if (originalTask == null)
        {
            throw new NotFoundException("原任务不存在");
        }

        // 确定目标项目
        var targetProjectId = newProjectId ?? originalTask.ProjectId;
        var project = await _db.Queryable<Project>()
            .Where(p => p.Id == targetProjectId)
            .FirstAsync();

        if (project == null)
        {
            throw new NotFoundException("目标项目不存在");
        }

        // 生成新任务编号
        var lastTask = await _db.Queryable<Task>()
            .Where(t => t.ProjectId == targetProjectId)
            .OrderByDescending(t => t.TaskNumber)
            .FirstAsync();

        var newTaskNumber = lastTask != null ? lastTask.TaskNumber + 1 : 1;

        // 创建任务副本
        var newTask = new Task
        {
            ProjectId = targetProjectId,
            TaskNumber = newTaskNumber,
            Type = originalTask.Type,
            Title = $"{originalTask.Title} (Copy)",
            Description = originalTask.Description,
            Status = "todo",
            Priority = originalTask.Priority,
            CreatorId = userId,
            AssigneeId = null,
            DueDate = null,
            EstimatedHours = originalTask.EstimatedHours,
            ActualHours = null,
            Tags = originalTask.Tags,
            Labels = originalTask.Labels,
            Metadata = DataHelper.SerializeToJsonb(new { CopiedFrom = originalTask.Id }),
            AttachmentCount = 0,
            CommentCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdTask = await _db.Insertable(newTask).ExecuteReturnEntityAsync();

        var taskWithRelations = await _db.Queryable<Task>()
            .Includes(t => t.Project)
            .Includes(t => t.Creator)
            .Includes(t => t.Assignee)
            .Where(t => t.Id == createdTask.Id)
            .FirstAsync();

        return MapToDto(taskWithRelations);
    }

    public async Task<TaskDto> MoveTaskAsync(string taskId, string newProjectId, string userId)
    {
        var task = await _db.Queryable<Task>()
            .Where(t => t.Id == taskId)
            .FirstAsync();

        if (task == null)
        {
            throw new NotFoundException("任务不存在");
        }

        // 检查权限
        if (task.CreatorId != userId)
        {
            throw new UnauthorizedAccessException("只有任务创建者可以移动任务");
        }

        // 检查新项目存在
        var project = await _db.Queryable<Project>()
            .Where(p => p.Id == newProjectId)
            .FirstAsync();

        if (project == null)
        {
            throw new NotFoundException("目标项目不存在");
        }

        // 生成新任务编号
        var lastTask = await _db.Queryable<Task>()
            .Where(t => t.ProjectId == newProjectId)
            .OrderByDescending(t => t.TaskNumber)
            .FirstAsync();

        var newTaskNumber = lastTask != null ? lastTask.TaskNumber + 1 : 1;

        // 更新任务
        task.ProjectId = newProjectId;
        task.TaskNumber = newTaskNumber;
        task.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(task).ExecuteCommandAsync();

        var updatedTask = await _db.Queryable<Task>()
            .Includes(t => t.Project)
            .Includes(t => t.Creator)
            .Includes(t => t.Assignee)
            .Where(t => t.Id == taskId)
            .FirstAsync();

        return MapToDto(updatedTask);
    }

    public async Task<string> ExportTaskDataAsync(string taskId)
    {
        var task = await _db.Queryable<Task>()
            .Includes(t => t.Project)
            .Includes(t => t.Creator)
            .Includes(t => t.Assignee)
            .Includes(t => t.Comments)
            .Includes(t => t.Attachments)
            .Where(t => t.Id == taskId)
            .FirstAsync();

        if (task == null)
        {
            throw new NotFoundException("任务不存在");
        }

        var exportData = new
        {
            Task = task,
            Comments = task.Comments,
            Attachments = task.Attachments,
            Statistics = await GetTaskStatisticsAsync(task.ProjectId)
        };

        return DataHelper.SerializeToJson(exportData);
    }

    public async Task<List<TaskActivityLogDto>> GetTaskActivityLogsAsync(string taskId, int limit = 20)
    {
        var comments = await _db.Queryable<Comment>()
            .Includes(c => c.User)
            .Where(c => c.TaskId == taskId)
            .OrderByDescending(c => c.CreatedAt)
            .Take(limit)
            .ToListAsync();

        return comments.Select(c => new TaskActivityLogDto
        {
            Id = c.Id,
            Type = "comment",
            Content = c.Content,
            UserId = c.UserId,
            UserName = c.User?.Username ?? "",
            UserFullName = c.User?.FullName ?? "",
            UserAvatar = c.User?.Avatar,
            IsSystem = c.IsSystem,
            SystemAction = c.SystemAction,
            CreatedAt = c.CreatedAt
        }).ToList();
    }

    public async Task<TaskDependencyDto> GetTaskDependenciesAsync(string taskId)
    {
        // TODO: 实现任务依赖关系
        return new TaskDependencyDto
        {
            DependsOn = new List<string>(),
            Blocks = new List<string>(),
            Related = new List<string>()
        };
    }

    // 辅助方法：映射实体到 DTO
    private TaskDto MapToDto(Task task)
    {
        var taskDto = new TaskDto
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            TaskNumber = task.TaskNumber,
            Type = task.Type,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            CreatorId = task.CreatorId,
            AssigneeId = task.AssigneeId,
            DueDate = task.DueDate,
            EstimatedHours = task.EstimatedHours,
            ActualHours = task.ActualHours,
            Tags = DataHelper.DeserializeListFromJson<string>(task.Tags),
            Labels = DataHelper.ParseJsonb<Dictionary<string, object>>(task.Labels),
            Resolution = task.Resolution,
            CompletedAt = task.CompletedAt,
            StartedAt = task.StartedAt,
            Metadata = DataHelper.ParseJsonbToDictionary(task.Metadata),
            AttachmentCount = task.AttachmentCount,
            CommentCount = task.CommentCount,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };

        // 映射关联数据
        if (task.Project != null)
        {
            taskDto.ProjectKey = task.Project.Key;
            taskDto.Project = new ProjectSummaryDto
            {
                Id = task.Project.Id,
                Name = task.Project.Name,
                Key = task.Project.Key,
                Color = task.Project.Color,
                Icon = task.Project.Icon,
                Status = task.Project.Status
            };
        }

        if (task.Creator != null)
        {
            taskDto.Creator = new UserSummaryDto
            {
                Id = task.Creator.Id,
                Username = task.Creator.Username,
                FullName = task.Creator.FullName,
                Avatar = task.Creator.Avatar
            };
        }

        if (task.Assignee != null)
        {
            taskDto.Assignee = new UserSummaryDto
            {
                Id = task.Assignee.Id,
                Username = task.Assignee.Username,
                FullName = task.Assignee.FullName,
                Avatar = task.Assignee.Avatar
            };
        }

        return taskDto;
    }
}

/// <summary>
/// 任务活动日志 DTO
/// </summary>
public class TaskActivityLogDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string? UserAvatar { get; set; }
    public bool IsSystem { get; set; }
    public string? SystemAction { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 任务依赖关系 DTO
/// </summary>
public class TaskDependencyDto
{
    public List<string> DependsOn { get; set; } = new List<string>();
    public List<string> Blocks { get; set; } = new List<string>();
    public List<string> Related { get; set; } = new List<string>();
}
