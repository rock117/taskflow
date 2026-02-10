using SqlSugar;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskFlow.Web.Core;

/// <summary>
/// 数据工具类
/// </summary>
public static class DataHelper
{
    /// <summary>
    /// 分页参数
    /// </summary>
    public class PaginationParams
    {
        private int _page = 1;
        private int _pageSize = 10;

        /// <summary>
        /// 页码（从 1 开始）
        /// </summary>
        public int Page
        {
            get => _page;
            set => _page = Math.Max(1, value);
        }

        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = Math.Min(100, Math.Max(1, value));
        }

        /// <summary>
        /// 计算偏移量
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public int Offset => (Page - 1) * PageSize;
    }

    /// <summary>
    /// 分页结果
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPreviousPage => Page > 1;

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage => Page < TotalPages;

        /// <summary>
        /// 创建空结果
        /// </summary>
        public static PagedResult<T> Empty()
        {
            return new PagedResult<T>
            {
                Items = new List<T>(),
                TotalCount = 0,
                Page = 1,
                PageSize = 10
            };
        }

        /// <summary>
        /// 从列表创建分页结果
        /// </summary>
        public static PagedResult<T> Create(List<T> items, int totalCount, int page, int pageSize)
        {
            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }

    /// <summary>
    /// 从查询创建分页结果
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="query">SqlSugar 查询</param>
    /// <param name="page">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns>分页结果</returns>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        ISugarQueryable<T> query,
        int page = 1,
        int pageSize = 10)
    {
        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToListAsync();

        return PagedResult<T>.Create(items, totalCount, page, pageSize);
    }

    /// <summary>
    /// 排序参数
    /// </summary>
    public class SortParams
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// 排序方向（asc/desc）
        /// </summary>
        public string Direction { get; set; } = "asc";

        /// <summary>
        /// 是否升序
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsAscending => Direction?.ToLower() == "asc";

        /// <summary>
        /// 是否降序
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsDescending => Direction?.ToLower() == "desc";
    }

    /// <summary>
    /// 应用排序
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="query">SqlSugar 查询</param>
    /// <param name="sortParams">排序参数</param>
    /// <returns>排序后的查询</returns>
    public static ISugarQueryable<T> ApplySorting<T>(
        ISugarQueryable<T> query,
        SortParams sortParams)
    {
        if (string.IsNullOrEmpty(sortParams.Field))
            return query;

        if (sortParams.IsDescending)
        {
            return query.OrderBy($"{sortParams.Field} DESC");
        }
        else
        {
            return query.OrderBy($"{sortParams.Field} ASC");
        }
    }

    /// <summary>
    /// JSON 序列化选项
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// 对象序列化为 JSON
    /// </summary>
    /// <param name="obj">要序列化的对象</param>
    /// <returns>JSON 字符串</returns>
    public static string SerializeToJson(object? obj)
    {
        if (obj == null)
            return string.Empty;

        return JsonSerializer.Serialize(obj, JsonOptions);
    }

    /// <summary>
    /// JSON 反序列化为对象
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="json">JSON 字符串</param>
    /// <returns>反序列化的对象</returns>
    public static T? DeserializeFromJson<T>(string json)
    {
        if (string.IsNullOrEmpty(json))
            return default;

        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    /// <summary>
    /// JSON 字符串转对象（匿名类型）
    /// </summary>
    /// <param name="json">JSON 字符串</param>
    /// <returns>对象</returns>
    public static object? DeserializeFromJson(string json)
    {
        if (string.IsNullOrEmpty(json))
            return null;

        return JsonSerializer.Deserialize<object>(json, JsonOptions);
    }

    /// <summary>
    /// 安全地解析 JSONB 字符串为字典
    /// </summary>
    /// <param name="jsonbString">JSONB 字符串</param>
    /// <returns>字典</returns>
    public static Dictionary<string, object> ParseJsonbToDictionary(string? jsonbString)
    {
        if (string.IsNullOrEmpty(jsonbString))
            return new Dictionary<string, object>();

        try
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonbString, JsonOptions);
            return dict ?? new Dictionary<string, object>();
        }
        catch
        {
            return new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// 安全地解析 JSONB 字符串为指定类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="jsonbString">JSONB 字符串</param>
    /// <returns>解析后的对象</returns>
    public static T? ParseJsonb<T>(string? jsonbString) where T : class, new()
    {
        if (string.IsNullOrEmpty(jsonbString))
            return new T();

        try
        {
            return JsonSerializer.Deserialize<T>(jsonbString, JsonOptions);
        }
        catch
        {
            return new T();
        }
    }

    /// <summary>
    /// 将对象序列化为 JSONB 字符串
    /// </summary>
    /// <param name="obj">要序列化的对象</param>
    /// <returns>JSONB 字符串</returns>
    public static string SerializeToJsonb(object? obj)
    {
        if (obj == null)
            return "{}";

        return JsonSerializer.Serialize(obj, JsonOptions);
    }

    /// <summary>
    /// 列表序列化为 JSON 数组字符串
    /// </summary>
    /// <typeparam name="T">列表元素类型</typeparam>
    /// <param name="list">列表</param>
    /// <returns>JSON 数组字符串</returns>
    public static string SerializeListToJson<T>(List<T>? list)
    {
        if (list == null || list.Count == 0)
            return "[]";

        return JsonSerializer.Serialize(list, JsonOptions);
    }

    /// <summary>
    /// 从 JSON 数组字符串反序列化为列表
    /// </summary>
    /// <typeparam name="T">列表元素类型</typeparam>
    /// <param name="jsonArray">JSON 数组字符串</param>
    /// <returns>列表</returns>
    public static List<T> DeserializeListFromJson<T>(string? jsonArray)
    {
        if (string.IsNullOrEmpty(jsonArray))
            return new List<T>();

        try
        {
            return JsonSerializer.Deserialize<List<T>>(jsonArray, JsonOptions) ?? new List<T>();
        }
        catch
        {
            return new List<T>();
        }
    }

    /// <summary>
    /// 批量删除（软删除）
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="db">SqlSugar 客户端</param>
    /// <param name="ids">要删除的 ID 列表</param>
    /// <returns>删除的记录数</returns>
    public static async Task<int> BatchDeleteAsync<T>(ISqlSugarClient db, List<string> ids) where T : class, new()
    {
        if (ids == null || ids.Count == 0)
            return 0;

        return await db.Deleteable<T>()
                      .In(ids)
                      .IsLogic()
                      .ExecuteCommandAsync();
    }

    /// <summary>
    /// 批量更新
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="db">SqlSugar 客户端</param>
    /// <param name="entities">要更新的实体列表</param>
    /// <returns>更新的记录数</returns>
    public static async Task<int> BatchUpdateAsync<T>(ISqlSugarClient db, List<T> entities) where T : class, new()
    {
        if (entities == null || entities.Count == 0)
            return 0;

        return await db.Updateable(entities)
                      .ExecuteCommandAsync();
    }

    /// <summary>
    /// 批量插入
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="db">SqlSugar 客户端</param>
    /// <param name="entities">要插入的实体列表</param>
    /// <returns>插入的记录数</returns>
    public static async Task<int> BatchInsertAsync<T>(ISqlSugarClient db, List<T> entities) where T : class, new()
    {
        if (entities == null || entities.Count == 0)
            return 0;

        return await db.Insertable(entities)
                      .ExecuteCommandAsync();
    }

    /// <summary>
    /// 批量插入或更新（Upsert）
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="db">SqlSugar 客户端</param>
    /// <param name="entities">实体列表</param>
    /// <param name="expression">更新条件表达式</param>
    /// <returns>影响的记录数</returns>
    public static async Task<int> BatchUpsertAsync<T>(
        ISqlSugarClient db,
        List<T> entities,
        System.Linq.Expressions.Expression<Func<T, object>>? expression = null) where T : class, new()
    {
        if (entities == null || entities.Count == 0)
            return 0;

        return await db.Storageable(entities)
                      .SplitUpdate(expression)
                      .SplitInsert()
                      .ExecuteCommandAsync();
    }

    /// <summary>
    /// 创建查询条件表达式（用于动态查询）
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="field">字段名</param>
    /// <param name="operator">操作符（eq, ne, gt, gte, lt, lte, like, in）</param>
    /// <param name="value">值</param>
    /// <returns>表达式</returns>
    public static System.Linq.Expressions.Expression<Func<T, bool>>? CreateExpression<T>(
        string field,
        string? @operator,
        object? value)
    {
        if (string.IsNullOrEmpty(field) || value == null)
            return null;

        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
        var property = System.Linq.Expressions.Expression.Property(parameter, field);
        var constant = System.Linq.Expressions.Expression.Constant(value, value.GetType());

        System.Linq.Expressions.Expression? comparison = null;

        switch (@operator?.ToLower())
        {
            case "eq":
                comparison = System.Linq.Expressions.Expression.Equal(property, constant);
                break;
            case "ne":
                comparison = System.Linq.Expressions.Expression.NotEqual(property, constant);
                break;
            case "gt":
                comparison = System.Linq.Expressions.Expression.GreaterThan(property, constant);
                break;
            case "gte":
                comparison = System.Linq.Expressions.Expression.GreaterThanOrEqual(property, constant);
                break;
            case "lt":
                comparison = System.Linq.Expressions.Expression.LessThan(property, constant);
                break;
            case "lte":
                comparison = System.Linq.Expressions.Expression.LessThanOrEqual(property, constant);
                break;
            case "like":
                comparison = System.Linq.Expressions.Expression.Call(
                    property,
                    typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                    constant);
                break;
            case "in":
                // in 操作符需要特殊处理
                var methodInfo = typeof(System.Linq.Enumerable)
                    .GetMethods()
                    .FirstOrDefault(m => m.Name == "Contains" && m.GetParameters().Length == 2)?
                    .MakeGenericMethod(typeof(T));

                if (methodInfo != null && value is System.Collections.IList list)
                {
                    var containsCall = System.Linq.Expressions.Expression.Call(
                        null,
                        methodInfo,
                        System.Linq.Expressions.Expression.Constant(list),
                        parameter);

                    return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(
                        containsCall,
                        parameter);
                }
                break;
        }

        if (comparison == null)
            return null;

        return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(comparison, parameter);
    }

    /// <summary>
    /// 检查字符串是否为空或空白
    /// </summary>
    /// <param name="value">字符串值</param>
    /// <returns>是否为空或空白</returns>
    public static bool IsNullOrWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 安全地获取字符串值
    /// </summary>
    /// <param name="value">字符串值</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>字符串值或默认值</returns>
    public static string GetValueOrDefault(string? value, string defaultValue = "")
    {
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    }

    /// <summary>
    /// 安全地获取整数值
    /// </summary>
    /// <param name="value">整数值</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>整数值或默认值</returns>
    public static int GetValueOrDefault(int? value, int defaultValue = 0)
    {
        return value ?? defaultValue;
    }

    /// <summary>
    /// 安全地获取 DateTime 值
    /// </summary>
    /// <param name="value">DateTime 值</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>DateTime 值或默认值</returns>
    public static DateTime GetValueOrDefault(DateTime? value, DateTime? defaultValue = null)
    {
        return value ?? defaultValue ?? DateTime.UtcNow;
    }

    /// <summary>
    /// 安全地获取布尔值
    /// </summary>
    /// <param name="value">布尔值</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>布尔值或默认值</returns>
    public static bool GetValueOrDefault(bool? value, bool defaultValue = false)
    {
        return value ?? defaultValue;
    }

    /// <summary>
    /// 检查是否为 null 或默认值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="value">值</param>
    /// <returns>是否为 null 或默认值</returns>
    public static bool IsNullOrDefault<T>(T? value) where T : struct
    {
        return value == null || value.Equals(default(T));
    }

    /// <summary>
    /// 安全地截断字符串
    /// </summary>
    /// <param name="value">字符串值</param>
    /// <param name="maxLength">最大长度</param>
    /// <param name="suffix">后缀（如 "..."）</param>
    /// <returns>截断后的字符串</returns>
    public static string Truncate(string? value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value ?? "";

        return value.Substring(0, maxLength - suffix.Length) + suffix;
    }

    /// <summary>
    /// 验证 GUID 格式
    /// </summary>
    /// <param name="value">GUID 字符串</param>
    /// <returns>是否有效</returns>
    public static bool IsValidGuid(string? value)
    {
        return Guid.TryParse(value, out _);
    }

    /// <summary>
    /// 转换为 GUID（如果无效则返回新 GUID）
    /// </summary>
    /// <param name="value">GUID 字符串</param>
    /// <returns>GUID</returns>
    public static Guid ToGuid(string? value)
    {
        if (Guid.TryParse(value, out var guid))
            return guid;

        return Guid.NewGuid();
    }

    /// <summary>
    /// 合并多个不为空的值
    /// </summary>
    /// <param name="values">值列表</param>
    /// <returns>合并后的字符串</returns>
    public static string JoinNotNull(params string?[] values)
    {
        return string.Join("", values.Where(v => !string.IsNullOrWhiteSpace(v)));
    }

    /// <summary>
    /// 格式化列表为逗号分隔的字符串
    /// </summary>
    /// <typeparam name="T">列表元素类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="separator">分隔符</param>
    /// <returns>格式化后的字符串</returns>
    public static string FormatList<T>(List<T>? list, string separator = ", ")
    {
        if (list == null || list.Count == 0)
            return "";

        return string.Join(separator, list);
    }

    /// <summary>
    /// 分割字符串为列表
    /// </summary>
    /// <param name="value">字符串</param>
    /// <param name="separator">分隔符</param>
    /// <param name="removeEmptyEntries">是否移除空项</param>
    /// <returns>字符串列表</returns>
    public static List<string> SplitToList(string? value, string separator = ",", bool removeEmptyEntries = true)
    {
        if (string.IsNullOrEmpty(value))
            return new List<string>();

        var options = removeEmptyEntries
            ? StringSplitOptions.RemoveEmptyEntries
            : StringSplitOptions.None;

        return value.Split(separator, options).ToList();
    }

    /// <summary>
    /// 计算分页的总页数
    /// </summary>
    /// <param name="totalCount">总记录数</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns>总页数</returns>
    public static int CalculateTotalPages(int totalCount, int pageSize)
    {
        if (pageSize <= 0)
            return 0;

        return (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    /// <summary>
    /// 验证分页参数
    /// </summary>
    /// <param name="page">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <param name="maxPageSize">最大每页大小</param>
    /// <returns>验证后的分页参数</returns>
    public static (int Page, int PageSize) ValidatePaginationParams(int page, int pageSize, int maxPageSize = 100)
    {
        return (
            Math.Max(1, page),
            Math.Min(maxPageSize, Math.Max(1, pageSize))
        );
    }

    /// <summary>
    /// 从查询字符串解析分页参数
    /// </summary>
    /// <param name="query">查询字符串集合</param>
    /// <param name="defaultPageSize">默认每页大小</param>
    /// <returns>分页参数</returns>
    public static PaginationParams ParsePaginationFromQuery(System.Collections.Generic.IQueryCollection? query, int defaultPageSize = 10)
    {
        var pageStr = query?["page"];
        var pageSizeStr = query?["pageSize"];

        var page = int.TryParse(pageStr, out var p) ? p : 1;
        var pageSize = int.TryParse(pageSizeStr, out var ps) ? ps : defaultPageSize;

        return new PaginationParams { Page = page, PageSize = pageSize };
    }

    /// <summary>
    /// 从查询字符串解析排序参数
    /// </summary>
    /// <param name="query">查询字符串集合</param>
    /// <returns>排序参数</returns>
    public static SortParams ParseSortFromQuery(System.Collections.Generic.IQueryCollection? query)
    {
        var sortBy = query?["sortBy"].FirstOrDefault();
        var sortOrder = query?["sortOrder"].FirstOrDefault();

        return new SortParams
        {
            Field = sortBy ?? "",
            Direction = sortOrder ?? "asc"
        };
    }

    /// <summary>
    /// 克隆对象（浅拷贝）
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="source">源对象</param>
    /// <returns>克隆后的对象</returns>
    public static T? Clone<T>(T? source) where T : class
    {
        if (source == null)
            return null;

        var json = SerializeToJson(source);
        return DeserializeFromJson<T>(json);
    }

    /// <summary>
    /// 更新对象的非空属性
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="target">目标对象</param>
    /// <param name="source">源对象</param>
    public static void UpdateNotNullProperties<T>(T target, T source) where T : class
    {
        if (target == null || source == null)
            return;

        var properties = typeof(T).GetProperties();
        foreach (var prop in properties)
        {
            if (prop.CanWrite && prop.CanRead)
            {
                var value = prop.GetValue(source);
                if (value != null)
                {
                    prop.SetValue(target, value);
                }
            }
        }
    }

    /// <summary>
    /// 获取对象的属性值字典
    /// </summary>
    /// <param name="obj">对象</param>
    /// <returns>属性字典</returns>
    public static Dictionary<string, object> GetPropertyDictionary(object? obj)
    {
        var dict = new Dictionary<string, object>();
        if (obj == null)
            return dict;

        var properties = obj.GetType().GetProperties();
        foreach (var prop in properties)
        {
            if (prop.CanRead)
            {
                var value = prop.GetValue(obj);
                dict[prop.Name] = value ?? "";
            }
        }

        return dict;
    }
}
