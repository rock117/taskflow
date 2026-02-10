import api from './api';

/**
 * 用户登录
 * @param {Object} credentials - 登录凭证 { login, password }
 * @returns {Promise} 返回包含 token 和 user 的响应
 */
export const login = async (credentials) => {
  const response = await api.post('/auth/login', credentials);
  return response.data;
};

/**
 * 用户注册
 * @param {Object} userData - 用户数据 { username, email, password, fullName }
 * @returns {Promise} 返回包含 token 和 user 的响应
 */
export const register = async (userData) => {
  const response = await api.post('/auth/register', userData);
  return response.data;
};

/**
 * 用户登出
 * @returns {Promise} 返回登出响应
 */
export const logout = async () => {
  const response = await api.post('/auth/logout');
  return response.data;
};

/**
 * 获取当前用户信息
 * @returns {Promise} 返回当前用户信息
 */
export const getCurrentUser = async () => {
  const response = await api.get('/auth/me');
  return response.data;
};

/**
 * 更新用户个人资料
 * @param {Object} userData - 用户更新数据 { username, email, fullName }
 * @returns {Promise} 返回更新后的用户信息
 */
export const updateProfile = async (userData) => {
  const response = await api.put('/auth/me', userData);
  return response.data;
};

/**
 * 修改密码
 * @param {Object} passwordData - 密码数据 { currentPassword, newPassword, confirmPassword }
 * @returns {Promise} 返回修改密码响应
 */
export const changePassword = async (passwordData) => {
  const response = await api.post('/auth/change-password', passwordData);
  return response.data;
};

/**
 * 请求密码重置
 * @param {Object} data - 包含 email 的对象
 * @returns {Promise} 返回请求密码重置响应
 */
export const forgotPassword = async (data) => {
  const response = await api.post('/auth/forgot-password', data);
  return response.data;
};

/**
 * 重置密码
 * @param {Object} data - 包含 token 和 newPassword 的对象
 * @returns {Promise} 返回重置密码响应
 */
export const resetPassword = async (data) => {
  const response = await api.post('/auth/reset-password', data);
  return response.data;
};

/**
 * 刷新 JWT token
 * @returns {Promise} 返回新的 token 和用户信息
 */
export const refreshToken = async () => {
  const response = await api.post('/auth/refresh-token');
  return response.data;
};

/**
 * 搜索用户
 * @param {Object} params - 搜索参数 { q, limit }
 * @returns {Promise} 返回用户列表
 */
export const searchUsers = async (params = {}) => {
  const response = await api.get('/users/search', { params });
  return response.data;
};

/**
 * 获取用户列表（管理员）
 * @param {Object} params - 查询参数 { page, limit }
 * @returns {Promise} 返回用户列表
 */
export const getUsers = async (params = {}) => {
  const response = await api.get('/users', { params });
  return response.data;
};

/**
 * 获取用户详情
 * @param {String} userId - 用户ID
 * @returns {Promise} 返回用户详情
 */
export const getUserById = async (userId) => {
  const response = await api.get(`/users/${userId}`);
  return response.data;
};

/**
 * 更新用户（管理员）
 * @param {String} userId - 用户ID
 * @param {Object} userData - 用户更新数据
 * @returns {Promise} 返回更新后的用户信息
 */
export const updateUser = async (userId, userData) => {
  const response = await api.put(`/users/${userId}`, userData);
  return response.data;
};

/**
 * 激活用户（管理员）
 * @param {String} userId - 用户ID
 * @returns {Promise} 返回激活后的用户信息
 */
export const activateUser = async (userId) => {
  const response = await api.post(`/users/${userId}/activate`);
  return response.data;
};

/**
 * 停用用户（管理员）
 * @param {String} userId - 用户ID
 * @returns {Promise} 返回停用后的用户信息
 */
export const deactivateUser = async (userId) => {
  const response = await api.post(`/users/${userId}/deactivate`);
  return response.data;
};

/**
 * 删除用户（管理员）
 * @param {String} userId - 用户ID
 * @returns {Promise} 返回删除响应
 */
export const deleteUser = async (userId) => {
  const response = await api.delete(`/users/${userId}`);
  return response.data;
};

export default {
  login,
  register,
  logout,
  getCurrentUser,
  updateProfile,
  changePassword,
  forgotPassword,
  resetPassword,
  refreshToken,
  searchUsers,
  getUsers,
  getUserById,
  updateUser,
  activateUser,
  deactivateUser,
  deleteUser,
};
