using Furion.DependencyInjection;
using Furion.FriendlyException;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.DTOs;
using TaskFlow.Services;

namespace TaskFlow.Controllers
{
    /// <summary>
    /// 评论控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase, ITransient
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// 创建评论
        /// </summary>
        /// <param name="createCommentDto">创建评论DTO</param>
        /// <returns>评论信息</returns>
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createCommentDto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    createCommentDto.UserId = guidId;
                    var comment = await _commentService.CreateCommentAsync(createCommentDto);
                    return Ok(comment);
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 获取评论详情
        /// </summary>
        /// <param name="commentId">评论ID</param>
        /// <returns>评论详情</returns>
        [HttpGet("{commentId}")]
        public async Task<IActionResult> GetComment(Guid commentId)
        {
            try
            {
                var comment = await _commentService.GetCommentByIdAsync(commentId);
                if (comment == null)
                {
                    return NotFound(new { message = "评论不存在" });
                }
                return Ok(comment);
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 获取任务的评论列表
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>评论列表</returns>
        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetTaskComments(
            Guid taskId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var (comments, total) = await _commentService.GetTaskCommentsAsync(taskId, pageIndex, pageSize);
                return Ok(new { comments, total, pageIndex, pageSize });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 更新评论
        /// </summary>
        /// <param name="commentId">评论ID</param>
        /// <param name="updateCommentDto">更新评论DTO</param>
        /// <returns>更新结果</returns>
        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] UpdateCommentDto updateCommentDto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    updateCommentDto.CurrentUserId = guidId;
                    var result = await _commentService.UpdateCommentAsync(commentId, updateCommentDto);
                    if (!result)
                    {
                        return NotFound(new { message = "评论不存在或无权限修改" });
                    }
                    return Ok(new { message = "评论已更新" });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="commentId">评论ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            try
            {
                var result = await _commentService.DeleteCommentAsync(commentId);
                if (!result)
                {
                    return NotFound(new { message = "评论不存在" });
                }
                return Ok(new { message = "评论已删除" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 切换评论点赞状态
        /// </summary>
        /// <param name="commentId">评论ID</param>
        /// <returns>点赞状态</returns>
        [HttpPost("{commentId}/like")]
        public async Task<IActionResult> ToggleCommentLike(Guid commentId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var isLiked = await _commentService.ToggleCommentLikeAsync(commentId, guidId);
                    return Ok(new { isLiked, message = isLiked ? "已点赞" : "已取消点赞" });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 获取用户的评论列表
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>评论列表</returns>
        [HttpGet("my-comments")]
        public async Task<IActionResult> GetMyComments(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var (comments, total) = await _commentService.GetUserCommentsAsync(guidId, pageIndex, pageSize);
                    return Ok(new { comments, total, pageIndex, pageSize });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 批量删除评论
        /// </summary>
        /// <param name="request">批量删除请求</param>
        /// <returns>删除数量</returns>
        [HttpPost("batch-delete")]
        public async Task<IActionResult> BatchDeleteComments([FromBody] dynamic request)
        {
            try
            {
                var commentIds = ((System.Text.Json.JsonElement)request.commentIds).EnumerateArray()
                    .Select(x => Guid.Parse(x.GetString())).ToList();
                var count = await _commentService.BatchDeleteCommentsAsync(commentIds);
                return Ok(new { message = $"已删除 {count} 条评论" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }
    }
}