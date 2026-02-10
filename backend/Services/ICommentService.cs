using TaskFlow.DTOs;

namespace TaskFlow.Services
{
    /// <summary>
    /// 评论服务接口
    /// </summary>
    public interface ICommentService
    {
        /// <summary>
        /// 创建评论
        /// </summary>
        /// <param name="createCommentDto">创建评论DTO</param>
        /// <returns>评论响应DTO</returns>
        Task<CommentResponseDto> CreateCommentAsync(CreateCommentDto createCommentDto);

        /// <summary>
        /// 获取评论详情
        /// </summary>
        /// <param name="commentId">评论ID</param>
        /// <returns>评论响应DTO</returns>
        Task<CommentResponseDto?> GetCommentByIdAsync(Guid commentId);

        /// <summary>
        /// 获取任务的评论列表
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>分页评论列表</returns>
        Task<(List<CommentResponseDto> Comments, int TotalCount)> GetTaskCommentsAsync(
            Guid taskId, int pageIndex = 1, int pageSize = 20);

        /// <summary>
        /// 更新评论
        /// </summary>
        /// <param name="commentId">评论ID</param>
        /// <param name="updateCommentDto">更新评论DTO</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateCommentAsync(Guid commentId, UpdateCommentDto updateCommentDto);

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="commentId">评论ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteCommentAsync(Guid commentId);

        /// <summary>
        /// 切换评论点赞状态
        /// </summary>
        /// <param name="commentId">评论ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns>点赞状态</returns>
        Task<bool> ToggleCommentLikeAsync(Guid commentId, Guid userId);

        /// <summary>
        /// 获取用户的评论列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>分页评论列表</returns>
        Task<(List<CommentResponseDto> Comments, int TotalCount)> GetUserCommentsAsync(
            Guid userId, int pageIndex = 1, int pageSize = 20);

        /// <summary>
        /// 批量删除评论
        /// </summary>
        /// <param name="commentIds">评论ID列表</param>
        /// <returns>删除数量</returns>
        Task<int> BatchDeleteCommentsAsync(List<Guid> commentIds);
    }
}