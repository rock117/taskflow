using Furion.DependencyInjection;
using Furion.FriendlyException;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.DTOs;
using TaskFlow.Services;

namespace TaskFlow.Controllers
{
    /// <summary>
    /// 用户控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase, ITransient
    {
        private readonly IAuthService _authService;

        public UserController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns>用户信息</returns>
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    var user = await _authService.GetUserByIdAsync(guidId);
                    if (user != null)
                    {
                        return Ok(user);
                    }
                }
                return NotFound(new { message = "用户不存在" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="updateUserDto">更新用户DTO</param>
        /// <returns>更新结果</returns>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    updateUserDto.Id = guidId;
                    var result = await _authService.UpdateUserAsync(updateUserDto);
                    return Ok(result);
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="changePasswordDto">修改密码DTO</param>
        /// <returns>修改结果</returns>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    await _authService.ChangePasswordAsync(guidId, changePasswordDto);
                    return Ok(new { message = "密码修改成功" });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="file">头像文件</param>
        /// <returns>头像URL</returns>
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
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

                    var avatarUrl = await _authService.UploadAvatarAsync(guidId, file);
                    return Ok(new { avatarUrl });
                }
                return Unauthorized(new { message = "未授权" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 获取用户列表（管理员）
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>用户列表</returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetUserList(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? keyword = null)
        {
            try
            {
                var (users, total) = await _authService.GetUserListAsync(pageIndex, pageSize, keyword);
                return Ok(new { users, total, pageIndex, pageSize });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户详情</returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "用户不存在" });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }
    }
}