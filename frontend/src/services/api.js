import axios from 'axios';

// 创建axios实例
const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL || 'http://localhost:5000/api',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// 请求拦截器
api.interceptors.request.use(
  (config) => {
    // 从localStorage获取token
    const token = localStorage.getItem('token');

    // 如果有token，添加到请求头
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    // 添加时间戳防止缓存
    if (config.method === 'get') {
      config.params = {
        ...config.params,
        _t: Date.now(),
      };
    }

    return config;
  },
  (error) => {
    console.error('请求错误:', error);
    return Promise.reject(error);
  }
);

// 响应拦截器
api.interceptors.response.use(
  (response) => {
    // 统一处理响应数据
    return response;
  },
  (error) => {
    console.error('响应错误:', error);

    if (error.response) {
      // 服务器返回了错误响应
      const { status, data } = error.response;

      // 处理不同的HTTP状态码
      switch (status) {
        case 401:
          // 未授权，token过期或无效
          localStorage.removeItem('token');
          localStorage.removeItem('user');

          // 如果不是登录/注册页面，重定向到登录页
          if (!window.location.pathname.includes('/login') &&
              !window.location.pathname.includes('/register')) {
            window.location.href = '/login';
          }

          return Promise.reject({
            ...error,
            message: '登录已过期，请重新登录',
          });

        case 403:
          // 禁止访问
          return Promise.reject({
            ...error,
            message: '没有权限执行此操作',
          });

        case 404:
          // 资源不存在
          return Promise.reject({
            ...error,
            message: data?.message || '请求的资源不存在',
          });

        case 422:
        case 400:
          // 验证错误或请求错误
          return Promise.reject({
            ...error,
            message: data?.message || '请求参数错误',
            details: data?.details || [],
          });

        case 429:
          // 请求过于频繁
          return Promise.reject({
            ...error,
            message: data?.message || '请求过于频繁，请稍后再试',
            retryAfter: data?.retryAfter,
          });

        case 500:
          // 服务器内部错误
          return Promise.reject({
            ...error,
            message: '服务器内部错误，请稍后再试',
          });

        case 503:
          // 服务不可用
          return Promise.reject({
            ...error,
            message: '服务暂时不可用，请稍后再试',
          });

        default:
          return Promise.reject({
            ...error,
            message: data?.message || '请求失败，请稍后再试',
          });
      }
    } else if (error.request) {
      // 请求已发出但没有收到响应
      return Promise.reject({
        ...error,
        message: '网络连接失败，请检查网络设置',
      });
    } else {
      // 请求配置错误
      return Promise.reject({
        ...error,
        message: '请求配置错误',
      });
    }
  }
);

// API工具函数
export const apiUtils = {
  // GET请求
  get: (url, config) => api.get(url, config),

  // POST请求
  post: (url, data, config) => api.post(url, data, config),

  // PUT请求
  put: (url, data, config) => api.put(url, data, config),

  // PATCH请求
  patch: (url, data, config) => api.patch(url, data, config),

  // DELETE请求
  delete: (url, config) => api.delete(url, config),

  // 文件上传
  upload: (url, file, onUploadProgress) => {
    const formData = new FormData();
    formData.append('file', file);

    return api.post(url, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
      onUploadProgress: (progressEvent) => {
        if (onUploadProgress) {
          const percentCompleted = Math.round(
            (progressEvent.loaded * 100) / progressEvent.total
          );
          onUploadProgress(percentCompleted);
        }
      },
    });
  },

  // 多文件上传
  uploadMultiple: (url, files, onUploadProgress) => {
    const formData = new FormData();
    files.forEach((file, index) => {
      formData.append(`files`, file);
    });

    return api.post(url, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
      onUploadProgress: (progressEvent) => {
        if (onUploadProgress) {
          const percentCompleted = Math.round(
            (progressEvent.loaded * 100) / progressEvent.total
          );
          onUploadProgress(percentCompleted);
        }
      },
    });
  },

  // 下载文件
  download: async (url, filename) => {
    const response = await api.get(url, {
      responseType: 'blob',
    });

    // 创建下载链接
    const downloadUrl = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement('a');
    link.href = downloadUrl;
    link.setAttribute('download', filename);
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(downloadUrl);
  },

  // 取消请求
  createCancelToken: () => {
    const CancelToken = axios.CancelToken;
    const source = CancelToken.source();
    return {
      token: source.token,
      cancel: source.cancel,
    };
  },

  // 构建查询参数
  buildQueryParams: (params) => {
    const queryParams = new URLSearchParams();

    Object.keys(params).forEach(key => {
      const value = params[key];

      if (value !== undefined && value !== null && value !== '') {
        if (Array.isArray(value)) {
          value.forEach(item => queryParams.append(key, item));
        } else {
          queryParams.append(key, value);
        }
      }
    });

    return queryParams.toString();
  },

  // 处理API响应
  handleResponse: (response) => {
    if (response.data && response.data.success !== false) {
      return response.data;
    }
    throw new Error(response.data?.message || '请求失败');
  },

  // 格式化错误消息
  formatError: (error) => {
    if (error.response?.data?.message) {
      return error.response.data.message;
    }
    if (error.response?.data?.details) {
      return error.response.data.details.map(d => d.message).join(', ');
    }
    if (error.message) {
      return error.message;
    }
    return '未知错误';
  },
};

export default api;
