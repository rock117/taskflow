using TaskFlow.DTOs;

namespace TaskFlow.Services
{
    /// <summary>
    /// 附件服务接口
    /// </summary>
    public interface IAttachmentService
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="uploadFileDto">上传文件DTO</param>
        /// <returns>附件响应DTO</returns>
        Task<AttachmentResponseDto> UploadFileAsync(UploadFileDto uploadFileDto);

        /// <summary>
        /// 获取附件详情
        /// </summary>
        /// <param name="attachmentId">附件ID</param>
        /// <returns>附件响应DTO</returns>
        Task<AttachmentResponseDto?> GetAttachmentByIdAsync(Guid attachmentId);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="attachmentId">附件ID</param>
        /// <returns>文件流和文件名</returns>
        Task<(Stream FileStream, string FileName)> DownloadFileAsync(Guid attachmentId);

        /// <summary>
        /// 获取任务的附件列表
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="userId">当前用户ID</param>
        /// <returns>附件列表</returns>
        Task<List<AttachmentResponseDto>> GetTaskAttachmentsAsync(Guid taskId, Guid userId);

        /// <summary>
        /// 获取项目的附件列表
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="userId">当前用户ID</param>
        /// <returns>附件列表</returns>
        Task<List<AttachmentResponseDto>> GetProjectAttachmentsAsync(Guid projectId, Guid userId);

        /// <summary>
        /// 获取用户的附件列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>分页附件列表</returns>
        Task<(List<AttachmentResponseDto> Attachments, int TotalCount)> GetUserAttachmentsAsync(
            Guid userId, int pageIndex = 1, int pageSize = 20);

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="attachmentId">附件ID</param>
        /// <param name="userId">当前用户ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteAttachmentAsync(Guid attachmentId, Guid userId);

        /// <summary>
        /// 批量删除附件
        /// </summary>
        /// <param name="attachmentIds">附件ID列表</param>
        /// <param name="userId">当前用户ID</param>
        /// <returns>删除数量</returns>
        Task<int> BatchDeleteAttachmentsAsync(List<Guid> attachmentIds, Guid userId);

        /// <summary>
        /// 获取附件预览URL
        /// </summary>
        /// <param name="attachmentId">附件ID</param>
        /// <param name="userId">当前用户ID</param>
        /// <returns>预览URL</returns>
        Task<string?> GetPreviewUrlAsync(Guid attachmentId, Guid userId);

        /// <summary>
        /// 更新附件信息
        /// </summary>
        /// <param name="attachmentId">附件ID</param>
        /// <param name="updateAttachmentDto">更新附件DTO</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateAttachmentAsync(Guid attachmentId, UpdateAttachmentDto updateAttachmentDto);

        /// <summary>
        /// 移动附件到其他任务
        /// </summary>
        /// <param name="attachmentId">附件ID</param>
        /// <param name="newTaskId">新任务ID</param>
        /// <param name="userId">当前用户ID</param>
        /// <returns>是否成功</returns>
        Task<bool> MoveAttachmentAsync(Guid attachmentId, Guid newTaskId, Guid userId);

        /// <summary>
        /// 获取文件统计信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>文件统计</returns>
        Task<FileStatisticsDto> GetFileStatisticsAsync(Guid userId);
    }
}