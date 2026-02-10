using Furion.DependencyInjection;
using Furion.FriendlyException;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.DTOs;
using TaskFlow.Services;

namespace TaskFlow.Controllers
{
    /// <summary>
    /// 认证控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase, ITransient
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="registerDto">注册信息</param>
        /// <returns>注册结果</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginDto">登录信息</param>
        /// <returns>登录结果（包含JWT Token）</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="refreshTokenDto">刷新Token信息</param>
        /// <returns>新的Token</returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(refreshTokenDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns>退出结果</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (Guid.TryParse(userId, out var guidId))
                {
                    await _authService.LogoutAsync(guidId);
                }
                return Ok(new { message = "退出登录成功" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 验证Token
        /// </summary>
        /// <returns>当前用户信息</returns>
        [HttpGet("verify")]
        [Authorize]
        public async Task<IActionResult> VerifyToken()
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
                return Unauthorized(new { message = "Token无效" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 发送验证码（邮箱验证）
        /// </summary>
        /// <param name="email">邮箱地址</param>
        /// <returns>发送结果</returns>
        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendVerificationCode([FromBody] dynamic request)
        {
            try
            {
                string email = request.email;
                await _authService.SendVerificationCodeAsync(email);
                return Ok(new { message = "验证码已发送" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 验证验证码
        /// </summary>
        /// <param name="request">验证码信息</param>
        /// <returns>验证结果</returns>
        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] dynamic request)
        {
            try
            {
                string email = request.email;
                string code = request.code;
                var isValid = await _authService.VerifyCodeAsync(email, code);
                return Ok(new { isValid });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="request">密码重置信息</param>
        /// <returns>重置结果</returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] dynamic request)
        {
            try
            {
                string email = request.email;
                string newPassword = request.newPassword;
                string code = request.code;
                await _authService.ResetPasswordAsync(email, newPassword, code);
                return Ok(new { message = "密码重置成功" });
            }
            catch (Exception ex)
            {
                throw Oops.Oh(ex.Message);
            }
        }
    }
}