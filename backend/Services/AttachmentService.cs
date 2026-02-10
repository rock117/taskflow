using Furion.DependencyInjection;
using Furion.DatabaseAccessor;
using Mapster;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Core;
using TaskFlow.DTOs;
using TaskFlow.Entities;

namespace TaskFlow.Services
{
    /// <summary>
    /// 附件服务实现
    /// </summary>
    public class AttachmentService : IAttachmentService, ITransient
    {
        private readonly IRepository<Attachment> _attachmentRepo;
        private readonly IRepository<Task> _taskRepo;
        private readonly IRepository<Project> _projectRepo;
        private readonly IRepository<User> _userRepo;

        public AttachmentService(
            IRepository<Attachment> attachmentRepo,
            IRepository<Task> taskRepo,
            IRepository<Project> projectRepo,
            IRepository<User> userRepo)
        {
            _attachmentRepo = attachmentRepo;
            _taskRepo = taskRepo;
            _projectRepo = projectRepo;
            _userRepo = userRepo;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        public async Task<AttachmentResponseDto> UploadFileAsync(UploadFileDto uploadFileDto)
        {
            // 验证用户
            var user = await _userRepo.FirstOrDefaultAsync(u => u.Id == uploadFileDto.UserId);
            if (user == null)
                throw new InvalidOperationException("用户不存在");

            // 如果有关联任务，验证任务
            if (uploadFileDto.TaskId.HasValue)
            {
                var task = await _taskRepo.FirstOrDefaultAsync(t => t.Id == uploadFileDto.TaskId.Value);
                if (task == null)
                    throw new InvalidOperationException("任务不存在");
            }

            // 生成唯一文件名
            var fileExtension = Path.GetExtension(uploadFileDto.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

            // 确定文件存储路径
            var relativePath = FileHelper.GetUploadPath(uploadFileDto.FileType);
            var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", relativePath, uniqueFileName);

            // 确保目录存在
            var directory = Path.GetDirectoryName(fullFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 保存文件
            using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
            {
                await uploadFileDto.FileStream.CopyToAsync(fileStream);
            }

            // 创建附件记录
            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                FileName = uploadFileDto.FileName,
                OriginalFileName = uploadFileDto.FileName,
                FileSize = uploadFileDto.FileSize,
                FileType = uploadFileDto.FileType,
                FileExtension = fileExtension,
                FilePath = Path.Combine(relativePath, uniqueFileName),
                MimeType = uploadFileDto.MimeType,
                TaskId = uploadFileDto.TaskId,
                ProjectId = uploadFileDto.ProjectId,
                CommentId = uploadFileDto.CommentId,
                UserId = uploadFileDto.UserId,
                CreatedTime = DateTime.UtcNow,
                UpdatedTime = DateTime.UtcNow,
                IsDeleted = false
            };

            var newAttachment = await _attachmentRepo.InsertAsync(attachment);

            // 返回响应
            return await GetAttachmentResponseAsync(newAttachment);
        }

        /// <summary>
        /// 获取附件详情
        /// </summary>
        public async Task<AttachmentResponseDto?> GetAttachmentByIdAsync(Guid attachmentId)
        {
            var attachment = await _attachmentRepo
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == attachmentId && !a.IsDeleted);

            if (attachment == null) return null;

            return await GetAttachmentResponseAsync(attachment);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        public async Task<(Stream FileStream, string FileName)> DownloadFileAsync(Guid attachmentId)
        {
            var attachment = await _attachmentRepo
                .FirstOrDefaultAsync(a => a.Id == attachmentId && !a.IsDeleted);

            if (attachment == null)
                throw new InvalidOperationException("附件不存在");

            var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", attachment.FilePath);

            if (!File.Exists(fullFilePath))
                throw new InvalidOperationException("文件不存在");

            var fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read);
            return (fileStream, attachment.OriginalFileName);
        }

        /// <summary>
        /// 获取任务的附件列表
        /// </summary>
        public async Task<List<AttachmentResponseDto>> GetTaskAttachmentsAsync(Guid taskId, Guid userId)
        {
            var attachments = await _attachmentRepo
                .Include(a => a.User)
                .Where(a => a.TaskId == taskId && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedTime)
                .ToListAsync();

            var attachmentDtos = new List<AttachmentResponseDto>();
            foreach (var attachment in attachments)
            {
                attachmentDtos.Add(await GetAttachmentResponseAsync(attachment));
            }

            return attachmentDtos;
        }

        /// <summary>
        /// 获取项目的附件列表
        /// </summary>
        public async Task<List<AttachmentResponseDto>> GetProjectAttachmentsAsync(Guid projectId, Guid userId)
        {
            var attachments = await _attachmentRepo
                .Include(a => a.User)
                .Where(a => a.ProjectId == projectId && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedTime)
                .ToListAsync();

            var attachmentDtos = new List<AttachmentResponseDto>();
            foreach (var attachment in attachments)
            {
                attachmentDtos.Add(await GetAttachmentResponseAsync(attachment));
            }

            return attachmentDtos;
        }

        /// <summary>
        /// 获取用户的附件列表
        /// </summary>
        public async Task<(List<AttachmentResponseDto> Attachments, int TotalCount)> GetUserAttachmentsAsync(
            Guid userId, int pageIndex = 1, int pageSize = 20)
        {
            var query = _attachmentRepo
                .Include(a => a.User)
                .Where(a => a.UserId == userId && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedTime);

            var totalCount = await query.CountAsync();

            var attachments = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var attachmentDtos = new List<AttachmentResponseDto>();
            foreach (var attachment in attachments)
            {
                attachmentDtos.Add(await GetAttachmentResponseAsync(attachment));
            }

            return (attachmentDtos, totalCount);
        }

        /// <summary>
        /// 删除附件
        /// </summary>
        public async Task<bool> DeleteAttachmentAsync(Guid attachmentId, Guid userId)
        {
            var attachment = await _attachmentRepo
                .FirstOrDefaultAsync(a => a.Id == attachmentId && !a.IsDeleted);

            if (attachment == null)
                return false;

            // 验证权限（只有上传者或管理员可以删除）
            if (attachment.UserId != userId)
                throw new UnauthorizedAccessException("无权限删除此附件");

            // 软删除数据库记录
            attachment.IsDeleted = true;
            attachment.UpdatedTime = DateTime.UtcNow;

            await _attachmentRepo.UpdateAsync(attachment);

            // 可选：物理删除文件
            var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", attachment.FilePath);
            if (File.Exists(fullFilePath))
            {
                try
                {
                    File.Delete(fullFilePath);
                }
                catch
                {
                    // 文件删除失败不影响数据库记录的删除
                }
            }

            return true;
        }

        /// <summary>
        /// 批量删除附件
        /// </summary>
        public async Task<int> BatchDeleteAttachmentsAsync(List<Guid> attachmentIds, Guid userId)
        {
            var attachments = await _attachmentRepo
                .Where(a => attachmentIds.Contains(a.Id) && !a.IsDeleted && a.UserId == userId)
                .ToListAsync();

            foreach (var attachment in attachments)
            {
                attachment.IsDeleted = true;
                attachment.UpdatedTime = DateTime.UtcNow;

                // 删除物理文件
                var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", attachment.FilePath);
                if (File.Exists(fullFilePath))
                {
                    try
                    {
                        File.Delete(fullFilePath);
                    }
                    catch
                    {
                        // 文件删除失败不影响数据库记录的删除
                    }
                }
            }

            await _attachmentRepo.UpdateRangeAsync(attachments);
            return attachments.Count;
        }

        /// <summary>
        /// 获取附件预览URL
        /// </summary>
        public async Task<string?> GetPreviewUrlAsync(Guid attachmentId, Guid userId)
        {
            var attachment = await _attachmentRepo
                .FirstOrDefaultAsync(a => a.Id == attachmentId && !a.IsDeleted);

            if (attachment == null)
                throw new InvalidOperationException("附件不存在");

            // 检查是否可预览
            var previewableTypes = new[] { "image/", "application/pdf", "text/" };
            if (!previewableTypes.Any(t => attachment.MimeType.StartsWith(t)))
                return null;

            return $"/api/attachments/{attachmentId}/preview";
        }

        /// <summary>
        /// 更新附件信息
        /// </summary>
        public async Task<bool> UpdateAttachmentAsync(Guid attachmentId, UpdateAttachmentDto updateAttachmentDto)
        {
            var attachment = await _attachmentRepo
                .FirstOrDefaultAsync(a => a.Id == attachmentId && !a.IsDeleted);

            if (attachment == null)
                return false;

            // 验证权限
            if (attachment.UserId != updateAttachmentDto.UserId)
                throw new UnauthorizedAccessException("无权限修改此附件");

            // 更新文件名
            if (!string.IsNullOrEmpty(updateAttachmentDto.FileName))
            {
                attachment.OriginalFileName = updateAttachmentDto.FileName;
            }

            attachment.UpdatedTime = DateTime.UtcNow;

            await _attachmentRepo.UpdateAsync(attachment);
            return true;
        }

        /// <summary>
        /// 移动附件到其他任务
        /// </summary>
        public async Task<bool> MoveAttachmentAsync(Guid attachmentId, Guid newTaskId, Guid userId)
        {
            var attachment = await _attachmentRepo
                .FirstOrDefaultAsync(a => a.Id == attachmentId && !a.IsDeleted);

            if (attachment == null)
                return false;

            // 验证权限
            if (attachment.UserId != userId)
                throw new UnauthorizedAccessException("无权限移动此附件");

            // 验证新任务是否存在
            var task = await _taskRepo.FirstOrDefaultAsync(t => t.Id == newTaskId);
            if (task == null)
                throw new InvalidOperationException("目标任务不存在");

            attachment.TaskId = newTaskId;
            attachment.UpdatedTime = DateTime.UtcNow;

            await _attachmentRepo.UpdateAsync(attachment);
            return true;
        }

        /// <summary>
        /// 获取文件统计信息
        /// </summary>
        public async Task<FileStatisticsDto> GetFileStatisticsAsync(Guid userId)
        {
            var attachments = await _attachmentRepo
                .Where(a => a.UserId == userId && !a.IsDeleted)
                .ToListAsync();

            var statistics = new FileStatisticsDto
            {
                TotalFiles = attachments.Count,
                TotalSize = attachments.Sum(a => a.FileSize),
                ImageCount = attachments.Count(a => a.MimeType.StartsWith("image/")),
                DocumentCount = attachments.Count(a => 
                    a.MimeType == "application/pdf" || 
                    a.MimeType.Contains("document") ||
                    a.MimeType.Contains("spreadsheet") ||
                    a.MimeType.Contains("presentation")),
                OtherCount = attachments.Count - 
                    attachments.Count(a => a.MimeType.StartsWith("image/")) - 
                    attachments.Count(a => 
                        a.MimeType == "application/pdf" || 
                        a.MimeType.Contains("document") ||
                        a.MimeType.Contains("spreadsheet") ||
                        a.MimeType.Contains("presentation"))
            };

            return statistics;
        }

        /// <summary>
        /// 构建附件响应DTO
        /// </summary>
        private async Task<AttachmentResponseDto> GetAttachmentResponseAsync(Attachment attachment)
        {
            var response = attachment.Adapt<AttachmentResponseDto>();
            
            if (attachment.User != null)
            {
                response.UploaderName = attachment.User.Username;
                response.UploaderAvatar = attachment.User.Avatar;
            }

            // 格式化文件大小
            response.FormattedSize = FileHelper.FormatFileSize(attachment.FileSize);
            
            // 生成下载URL
            response.DownloadUrl = $"/api/attachments/{attachment.Id}/download";
            
            // 判断是否可预览
            response.IsPreviewable = attachment.MimeType.StartsWith("image/") || 
                                     attachment.MimeType == "application/pdf";

            return response;
        }
    }
}