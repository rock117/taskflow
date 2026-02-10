using Furion.DependencyInjection;
using Furion.FriendlyException;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.DTOs;
using TaskFlow.Services;

namespace TaskFlow.Controllers
{
    /// <summary>
    /// 附件控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttachmentController : ControllerBase, ITransient
    {
        private readonly IAttachmentService _attachmentService;

        public AttachmentController(IAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="taskId">任务ID（可选）</param>
        /// <param name="projectId">项目ID（可选）</param>
        /// <param name="commentId">评论ID（可选）</param>
        /// <param name="file">文件</param>
        /// <returns>附件信息</returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(
            [FromQuery] Guid? taskId = null,
            [FromQuery] Guid? projectId = null,
            [FromQuery] Guid? commentId = null,
            IFormFile file = null)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    if (file == null || file.Length == 0)
                    {
                        return BadRequest(new { message = "请选择文件" });
                    }

                    var uploadFileDto = new UploadFileDto
                    {
                        UserId = guidId,
                        TaskId = taskId,
                        ProjectId = projectId,
                        CommentId = commentId,
                        FileName = file.FileName,
                        FileSize = file.Length,
                        FileType = GetFileType(file.FileName),
                        MimeType = file.ContentType,
                        FileStream = file.OpenReadStream()
                    };

                    var attachment = await _attachmentService.UploadFileAsync(uploadFileDto);
                    return Ok(attachment);
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 批量上传文件
        /// </summary>
        /// <param name="taskId">任务ID（可选）</param>
        /// <param name="projectId">项目ID（可选）</param>
        /// <param name="commentId">评论ID（可选）</param>
        /// <param name="files">文件列表</param>
        /// <returns>附件列表</returns>
        [HttpPost("upload-batch")]
        public async Task<IActionResult> UploadFilesBatch(
            [FromQuery] Guid? taskId = null,
            [FromQuery] Guid? projectId = null,
            [FromQuery] Guid? commentId = null,
            IFormFileCollection files = null)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    if (files == null || files.Count == 0)
                    {
                        return BadRequest(new { message = "请选择文件" });
                    }

                    var attachments = new List<object>();
                    foreach (var file in files)
                    {
                        var uploadFileDto = new UploadFileDto
                        {
                            UserId = guidId,
                            TaskId = taskId,
                            ProjectId = projectId,
                            CommentId = commentId,
                            FileName = file.FileName,
                            FileSize = file.Length,
                            FileType = GetFileType(file.FileName),
                            MimeType = file.ContentType,
                            FileStream = file.OpenReadStream()
                        };

                        var attachment = await _attachmentService.UploadFileAsync(uploadFileDto);
                        attachments.Add(attachment);
                    }

                    return Ok(new { attachments, count = attachments.Count });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 获取附件详情
        /// </summary>
        /// <param name="attachmentId">附件ID</param>
        /// <returns>附件详情</returns>
        [HttpGet("{attachmentId}")]
        public async Task<IActionResult> GetAttachment(Guid attachmentId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var attachment = await _attachmentService.GetAttachmentByIdAsync(attachmentId);
                    if (attachment == null)
                    {
                        return NotFound(new { message = "附件不存在" });
                    }
                    return Ok(attachment);
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="attachmentId">附件ID</param>
        /// <returns>文件流</returns>
        [HttpGet("{attachmentId}/download")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadFile(Guid attachmentId)
        {
            try
            {
                var (fileStream, fileName) = await _attachmentService.DownloadFileAsync(attachmentId);
                return File(fileStream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 预览文件
        /// </summary>
        /// <param name="attachmentId">附件ID</param>
        /// <returns>文件流</returns>
        [HttpGet("{attachmentId}/preview")]
        [AllowAnonymous]
        public async Task<IActionResult> PreviewFile(Guid attachmentId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var (fileStream, fileName) = await _attachmentService.DownloadFileAsync(attachmentId);
                    
                    // 根据文件类型返回适当的Content-Type
                    var attachment = await _attachmentService.GetAttachmentByIdAsync(attachmentId);
                    var contentType = attachment?.MimeType ?? "application/octet-stream";
                    
                    return File(fileStream, contentType);
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 获取任务的附件列表
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>附件列表</returns>
        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetTaskAttachments(Guid taskId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var attachments = await _attachmentService.GetTaskAttachmentsAsync(taskId, guidId);
                    return Ok(new { attachments, count = attachments.Count });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 获取项目的附件列表
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <returns>附件列表</returns>
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetProjectAttachments(Guid projectId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var attachments = await _attachmentService.GetProjectAttachmentsAsync(projectId, guidId);
                    return Ok(new { attachments, count = attachments.Count });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 获取用户的附件列表
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>附件列表</returns>
        [HttpGet("my-attachments")]
        public async Task<IActionResult> GetMyAttachments(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var (attachments, total) = await _attachmentService.GetUserAttachmentsAsync(guidId, pageIndex, pageSize);
                    return Ok(new { attachments, total, pageIndex, pageSize });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 更新附件信息
        /// </summary>
        /// <param name="attachmentId">附件ID</param>
        /// <param name="updateAttachmentDto">更新附件DTO</param>
        /// <returns>更新结果</returns>
        [HttpPut("{attachmentId}")]
        public async Task<IActionResult> UpdateAttachment(Guid attachmentId, [FromBody] UpdateAttachmentDto updateAttachmentDto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    updateAttachmentDto.UserId = guidId;
                    var result = await _attachmentService.UpdateAttachmentAsync(attachmentId, updateAttachmentDto);
                    if (!result)
                    {
                        return NotFound(new { message = "附件不存在或无权限修改" });
                    }
                    return Ok(new { message = "附件已更新" });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 移动附件到其他任务
        /// </summary>
        /// <param name="attachmentId">附件ID</param>
        /// <param name="newTaskId">新任务ID</param>
        /// <returns>移动结果</returns>
        [HttpPost("{attachmentId}/move")]
        public async Task<IActionResult> MoveAttachment(Guid attachmentId, [FromBody] dynamic request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    Guid newTaskId = request.newTaskId;
                    var result = await _attachmentService.MoveAttachmentAsync(attachmentId, newTaskId, guidId);
                    if (!result)
                    {
                        return BadRequest(new { message = "移动附件失败" });
                    }
                    return Ok(new { message = "附件已移动" });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="attachmentId">附件ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{attachmentId}")]
        public async Task<IActionResult> DeleteAttachment(Guid attachmentId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var result = await _attachmentService.DeleteAttachmentAsync(attachmentId, guidId);
                    if (!result)
                    {
                        return NotFound(new { message = "附件不存在或无权限删除" });
                    }
                    return Ok(new { message = "附件已删除" });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 批量删除附件
        /// </summary>
        /// <param name="request">批量删除请求</param>
        /// <returns>删除数量</returns>
        [HttpPost("batch-delete")]
        public async Task<IActionResult> BatchDeleteAttachments([FromBody] dynamic request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var attachmentIds = ((System.Text.Json.JsonElement)request.attachmentIds).EnumerateArray()
                        .Select(x => Guid.Parse(x.GetString())).ToList();
                    var count = await _attachmentService.BatchDeleteAttachmentsAsync(attachmentIds, guidId);
                    return Ok(new { message = $"已删除 {count} 个附件" });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 获取文件统计信息
        /// </summary>
        /// <returns>文件统计</returns>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetFileStatistics()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var statistics = await _attachmentService.GetFileStatisticsAsync(guidId);
                    return Ok(statistics);
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 根据文件名获取文件类型
        /// </summary>
        private string GetFileType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            return extension switch
            {
                ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".webp" => "image",
                ".pdf" => "document",
                ".doc" or ".docx" => "document",
                ".xls" or ".xlsx" => "spreadsheet",
                ".ppt" or ".pptx" => "presentation",
                ".txt" => "text",
                ".zip" or ".rar" or ".7z" => "archive",
                ".mp4" or ".avi" or ".mov" or ".mkv" => "video",
                ".mp3" or ".wav" => "audio",
                _ => "other"
            };
        }
    }
}