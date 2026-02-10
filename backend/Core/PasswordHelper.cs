using BCrypt.Net;

namespace TaskFlow.Web.Core;

/// <summary>
/// 密码工具类
/// </summary>
public static class PasswordHelper
{
    /// <summary>
    /// 密码加密
    /// </summary>
    /// <param name="password">明文密码</param>
    /// <param name="workFactor">工作因子（4-31），默认为 10</param>
    /// <returns>加密后的密码</returns>
    public static string HashPassword(string password, int workFactor = 10)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("密码不能为空", nameof(password));

        if (workFactor < 4 || workFactor > 31)
            throw new ArgumentOutOfRangeException(nameof(workFactor), "工作因子必须在 4-31 之间");

        return BCrypt.HashPassword(password, workFactor);
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    /// <param name="password">明文密码</param>
    /// <param name="hashedPassword">加密后的密码</param>
    /// <returns>是否匹配</returns>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("密码不能为空", nameof(password));

        if (string.IsNullOrEmpty(hashedPassword))
            throw new ArgumentException("加密后的密码不能为空", nameof(hashedPassword));

        try
        {
            return BCrypt.Verify(password, hashedPassword);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 检查密码是否需要重新加密
    /// </summary>
    /// <param name="hashedPassword">加密后的密码</param>
    /// <param name="workFactor">目标工作因子</param>
    /// <returns>是否需要重新加密</returns>
    public static bool NeedsRehash(string hashedPassword, int workFactor = 10)
    {
        if (string.IsNullOrEmpty(hashedPassword))
            return true;

        try
        {
            return BCrypt.PasswordNeedsRehash(hashedPassword, workFactor);
        }
        catch
        {
            return true;
        }
    }

    /// <summary>
    /// 生成随机密码
    /// </summary>
    /// <param name="length">密码长度，默认 12</param>
    /// <param name="includeSpecialChars">是否包含特殊字符，默认 true</param>
    /// <returns>随机密码</returns>
    public static string GenerateRandomPassword(int length = 12, bool includeSpecialChars = true)
    {
        if (length < 6)
            throw new ArgumentOutOfRangeException(nameof(length), "密码长度不能少于 6");

        const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string specialChars = "!@#$%^&*()_-+=<>?";

        var chars = lowerChars + upperChars + digits;
        if (includeSpecialChars)
            chars += specialChars;

        var random = new Random();
        var password = new char[length];

        // 确保密码包含至少一个小写字母、大写字母和数字
        password[0] = lowerChars[random.Next(lowerChars.Length)];
        password[1] = upperChars[random.Next(upperChars.Length)];
        password[2] = digits[random.Next(digits.Length)];

        if (includeSpecialChars)
        {
            password[3] = specialChars[random.Next(specialChars.Length)];
        }

        // 填充剩余字符
        for (int i = (includeSpecialChars ? 4 : 3); i < length; i++)
        {
            password[i] = chars[random.Next(chars.Length)];
        }

        // 打乱密码顺序
        for (int i = 0; i < length; i++)
        {
            int j = random.Next(length);
            (password[i], password[j]) = (password[j], password[i]);
        }

        return new string(password);
    }

    /// <summary>
    /// 检查密码强度
    /// </summary>
    /// <param name="password">密码</param>
    /// <param name="minLength">最小长度，默认 6</param>
    /// <returns>密码强度等级（0-4）</returns>
    public static int CheckPasswordStrength(string password, int minLength = 6)
    {
        if (string.IsNullOrEmpty(password))
            return 0;

        int strength = 0;

        // 长度检查
        if (password.Length >= minLength)
            strength++;
        if (password.Length >= 8)
            strength++;
        if (password.Length >= 12)
            strength++;

        // 复杂度检查
        bool hasLower = password.Any(char.IsLower);
        bool hasUpper = password.Any(char.IsUpper);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

        if (hasLower && hasUpper && hasDigit)
            strength++;

        if (hasLower && hasUpper && hasDigit && hasSpecial)
            strength++;

        return Math.Min(strength, 4);
    }

    /// <summary>
    /// 获取密码强度描述
    /// </summary>
    /// <param name="password">密码</param>
    /// <returns>密码强度描述</returns>
    public static string GetPasswordStrengthDescription(string password)
    {
        int strength = CheckPasswordStrength(password);

        return strength switch
        {
            0 => "非常弱",
            1 => "弱",
            2 => "中等",
            3 => "强",
            4 => "非常强",
            _ => "未知"
        };
    }

    /// <summary>
    /// 验证密码是否符合安全策略
    /// </summary>
    /// <param name="password">密码</param>
    /// <param name="minLength">最小长度</param>
    /// <param name="requireUppercase">是否要求大写字母</param>
    /// <param name="requireLowercase">是否要求小写字母</param>
    /// <param name="requireDigit">是否要求数字</param>
    /// <param name="requireSpecialChar">是否要求特殊字符</param>
    /// <returns>验证结果</returns>
    public static PasswordValidationResult ValidatePassword(
        string password,
        int minLength = 6,
        bool requireUppercase = false,
        bool requireLowercase = false,
        bool requireDigit = true,
        bool requireSpecialChar = false)
    {
        var result = new PasswordValidationResult { IsValid = true, Errors = new List<string>() };

        if (string.IsNullOrEmpty(password))
        {
            result.IsValid = false;
            result.Errors.Add("密码不能为空");
            return result;
        }

        // 长度检查
        if (password.Length < minLength)
        {
            result.IsValid = false;
            result.Errors.Add($"密码长度至少需要 {minLength} 位");
        }

        // 大写字母检查
        if (requireUppercase && !password.Any(char.IsUpper))
        {
            result.IsValid = false;
            result.Errors.Add("密码必须包含至少一个大写字母");
        }

        // 小写字母检查
        if (requireLowercase && !password.Any(char.IsLower))
        {
            result.IsValid = false;
            result.Errors.Add("密码必须包含至少一个小写字母");
        }

        // 数字检查
        if (requireDigit && !password.Any(char.IsDigit))
        {
            result.IsValid = false;
            result.Errors.Add("密码必须包含至少一个数字");
        }

        // 特殊字符检查
        if (requireSpecialChar && !password.Any(c => !char.IsLetterOrDigit(c)))
        {
            result.IsValid = false;
            result.Errors.Add("密码必须包含至少一个特殊字符");
        }

        // 常见弱密码检查
        var weakPasswords = new[]
        {
            "123456", "password", "12345678", "qwerty", "123456789",
            "abc123", "admin", "1234567", "welcome", "111111"
        };

        if (weakPasswords.Contains(password.ToLower()))
        {
            result.IsValid = false;
            result.Errors.Add("密码过于简单，请使用更强的密码");
        }

        return result;
    }

    /// <summary>
    /// 密码验证结果
    /// </summary>
    public class PasswordValidationResult
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 错误信息列表
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// 获取错误信息字符串
        /// </summary>
        /// <returns>错误信息</returns>
        public string GetErrorMessage()
        {
            return IsValid ? "" : string.Join("; ", Errors);
        }
    }
}
