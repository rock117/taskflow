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
    /// 评论服务实现
    /// </summary>
    public class CommentService : ICommentService, ITransient
    {
        private readonly IRepository<Comment> _commentRepo;
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Task> _taskRepo;
        private readonly IRepository<Attachment> _attachmentRepo;

        public CommentService(
            IRepository<Comment> commentRepo,
            IRepository<User> userRepo,
            IRepository<Task> taskRepo,
            IRepository<Attachment> attachmentRepo)
        {
            _commentRepo = commentRepo;
            _userRepo = userRepo;
            _taskRepo = taskRepo;
            _attachmentRepo = attachmentRepo;
        }

        /// <summary>
        /// 创建评论
        /// </summary>
        public async Task<CommentResponseDto> CreateCommentAsync(CreateCommentDto createCommentDto)
        {
            // 验证任务是否存在
            var task = await _taskRepo.FirstOrDefaultAsync(t => t.Id == createCommentDto.TaskId);
            if (task == null)
                throw new InvalidOperationException("任务不存在");

            // 验证用户是否存在
            var user = await _userRepo.FirstOrDefaultAsync(u => u.Id == createCommentDto.UserId);
            if (user == null)
                throw new InvalidOperationException("用户不存在");

            // 创建评论
            var comment = createCommentDto.Adapt<Comment>();
            comment.Id = Guid.NewGuid();
            comment.CreatedTime = DateTime.UtcNow;
            comment.UpdatedTime = DateTime.UtcNow;
            comment.IsDeleted = false;
            comment.LikeCount = 0;

            // 处理@提及
            if (!string.IsNullOrEmpty(createCommentDto.Mentions))
            {
                comment.Mentions = createCommentDto.Mentions;
            }

            var newComment = await _commentRepo.InsertAsync(comment);

            // 返回响应
            return await GetCommentResponseAsync(newComment);
        }

        /// <summary>
        /// 获取评论详情
        /// </summary>
        public async Task<CommentResponseDto?> GetCommentByIdAsync(Guid commentId)
        {
            var comment = await _commentRepo
                .Include(c => c.User)
                .Include(c => c.Task)
                .Include(c => c.Attachments.Where(a => !a.IsDeleted))
                .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

            if (comment == null) return null;

            return await GetCommentResponseAsync(comment);
        }

        /// <summary>
        /// 获取任务的评论列表
        /// </summary>
        public async Task<(List<CommentResponseDto> Comments, int TotalCount)> GetTaskCommentsAsync(
            Guid taskId, int pageIndex = 1, int pageSize = 20)
        {
            var query = _commentRepo
                .Include(c => c.User)
                .Include(c => c.Attachments.Where(a => !a.IsDeleted))
                .Where(c => c.TaskId == taskId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedTime);

            var totalCount = await query.CountAsync();

            var comments = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var commentDtos = new List<CommentResponseDto>();
            foreach (var comment in comments)
            {
                commentDtos.Add(await GetCommentResponseAsync(comment));
            }

            return (commentDtos, totalCount);
        }

        /// <summary>
        /// 更新评论
        /// </summary>
        public async Task<bool> UpdateCommentAsync(Guid commentId, UpdateCommentDto updateCommentDto)
        {
            var comment = await _commentRepo.FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);
            if (comment == null)
                return false;

            // 验证权限
            if (comment.UserId != updateCommentDto.CurrentUserId)
                throw new UnauthorizedAccessException("无权限修改此评论");

            // 更新内容
            if (!string.IsNullOrEmpty(updateCommentDto.Content))
            {
                comment.Content = updateCommentDto.Content;
            }

            // 更新@提及
            comment.Mentions = updateCommentDto.Mentions;
            comment.UpdatedTime = DateTime.UtcNow;

            await _commentRepo.UpdateAsync(comment);
            return true;
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        public async Task<bool> DeleteCommentAsync(Guid commentId)
        {
            var comment = await _commentRepo.FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);
            if (comment == null)
                return false;

            comment.IsDeleted = true;
            comment.UpdatedTime = DateTime.UtcNow;

            await _commentRepo.UpdateAsync(comment);
            return true;
        }

        /// <summary>
        /// 切换评论点赞状态
        /// </summary>
        public async Task<bool> ToggleCommentLikeAsync(Guid commentId, Guid userId)
        {
            var comment = await _commentRepo.FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);
            if (comment == null)
                throw new InvalidOperationException("评论不存在");

            // 获取点赞用户列表
            var likedUsers = new List<Guid>();
            if (!string.IsNullOrEmpty(comment.LikedUsers))
            {
                likedUsers = comment.LikedUsers.Split(',').Select(Guid.Parse).ToList();
            }

            var isLiked = likedUsers.Contains(userId);

            if (isLiked)
            {
                // 取消点赞
                likedUsers.Remove(userId);
                comment.LikeCount = Math.Max(0, comment.LikeCount - 1);
            }
            else
            {
                // 点赞
                likedUsers.Add(userId);
                comment.LikeCount++;
            }

            comment.LikedUsers = string.Join(",", likedUsers);
            comment.UpdatedTime = DateTime.UtcNow;

            await _commentRepo.UpdateAsync(comment);
            return !isLiked;
        }

        /// <summary>
        /// 获取用户的评论列表
        /// </summary>
        public async Task<(List<CommentResponseDto> Comments, int TotalCount)> GetUserCommentsAsync(
            Guid userId, int pageIndex = 1, int pageSize = 20)
        {
            var query = _commentRepo
                .Include(c => c.User)
                .Include(c => c.Task)
                .Include(c => c.Attachments.Where(a => !a.IsDeleted))
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedTime);

            var totalCount = await query.CountAsync();

            var comments = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var commentDtos = new List<CommentResponseDto>();
            foreach (var comment in comments)
            {
                commentDtos.Add(await GetCommentResponseAsync(comment));
            }

            return (commentDtos, totalCount);
        }

        /// <summary>
        /// 批量删除评论
        /// </summary>
        public async Task<int> BatchDeleteCommentsAsync(List<Guid> commentIds)
        {
            var comments = await _commentRepo
                .Where(c => commentIds.Contains(c.Id) && !c.IsDeleted)
                .ToListAsync();

            foreach (var comment in comments)
            {
                comment.IsDeleted = true;
                comment.UpdatedTime = DateTime.UtcNow;
            }

            await _commentRepo.UpdateRangeAsync(comments);
            return comments.Count;
        }

        /// <summary>
        /// 构建评论响应DTO
        /// </summary>
        private async Task<CommentResponseDto> GetCommentResponseAsync(Comment comment)
        {
            var response = comment.Adapt<CommentResponseDto>();
            
            // 设置用户信息
            if (comment.User != null)
            {
                response.UserAvatar = comment.User.Avatar;
                response.UserName = comment.User.Username;
            }

            // 设置附件信息
            if (comment.Attachments != null && comment.Attachments.Any())
            {
                response.Attachments = comment.Attachments.Adapt<List<AttachmentDto>>();
            }

            // 解析点赞用户
            if (!string.IsNullOrEmpty(comment.LikedUsers))
            {
                response.IsLikedByCurrentUser = response.CurrentUserId.HasValue && 
                    comment.LikedUsers.Split(',').Contains(response.CurrentUserId.ToString());
            }

            return response;
        }
    }
}