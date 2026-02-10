import api from './api';

/**
 * 获取项目列表
 * @param {Object} params - 查询参数 { page, limit, status, search }
 * @returns {Promise} 返回项目列表和分页信息
 */
export const getProjects = async (params = {}) => {
  const response = await api.get('/projects', { params });
  return response.data;
};

/**
 * 获取项目详情
 * @param {String} projectId - 项目ID
 * @returns {Promise} 返回项目详情
 */
export const getProjectById = async (projectId) => {
  const response = await api.get(`/projects/${projectId}`);
  return response.data;
};

/**
 * 创建新项目
 * @param {Object} projectData - 项目数据 { name, description, key, startDate, endDate, color, icon }
 * @returns {Promise} 返回创建的项目
 */
export const createProject = async (projectData) => {
  const response = await api.post('/projects', projectData);
  return response.data;
};

/**
 * 更新项目
 * @param {String} projectId - 项目ID
 * @param {Object} projectData - 项目更新数据
 * @returns {Promise} 返回更新后的项目
 */
export const updateProject = async (projectId, projectData) => {
  const response = await api.put(`/projects/${projectId}`, projectData);
  return response.data;
};

/**
 * 删除项目
 * @param {String} projectId - 项目ID
 * @returns {Promise} 返回删除响应
 */
export const deleteProject = async (projectId) => {
  const response = await api.delete(`/projects/${projectId}`);
  return response.data;
};

/**
 * 归档项目
 * @param {String} projectId - 项目ID
 * @returns {Promise} 返回归档后的项目
 */
export const archiveProject = async (projectId) => {
  const response = await api.post(`/projects/${projectId}/archive`);
  return response.data;
};

/**
 * 激活项目
 * @param {String} projectId - 项目ID
 * @returns {Promise} 返回激活后的项目
 */
export const activateProject = async (projectId) => {
  const response = await api.post(`/projects/${projectId}/activate`);
  return response.data;
};

/**
 * 获取项目的任务列表
 * @param {String} projectId - 项目ID
 * @param {Object} params - 查询参数 { page, limit, status, priority, type, assignee, search }
 * @returns {Promise} 返回任务列表和分页信息
 */
export const getProjectTasks = async (projectId, params = {}) => {
  const response = await api.get(`/projects/${projectId}/tasks`, { params });
  return response.data;
};

/**
 * 获取项目统计信息
 * @param {String} projectId - 项目ID
 * @returns {Promise} 返回项目统计数据
 */
export const getProjectStatistics = async (projectId) => {
  const response = await api.get(`/projects/${projectId}/statistics`);
  return response.data;
};

export default {
  getProjects,
  getProjectById,
  createProject,
  updateProject,
  deleteProject,
  archiveProject,
  activateProject,
  getProjectTasks,
  getProjectStatistics,
};
