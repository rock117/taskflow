import api from './api';

/**
 * 获取任务列表
 * @param {Object} params - 查询参数 { page, limit, status, priority, type, projectId, assignedToMe, createdByMe, search }
 * @returns {Promise} 返回任务列表和分页信息
 */
export const getTasks = async (params = {}) => {
  const response = await api.get('/tasks', { params });
  return response.data;
};

/**
 * 获取任务详情
 * @param {String} taskId - 任务ID
 * @returns {Promise} 返回任务详情
 */
export const getTaskById = async (taskId) => {
  const response = await api.get(`/tasks/${taskId}`);
  return response.data;
};

/**
 * 创建新任务
 * @param {Object} taskData - 任务数据 { projectId, type, title, description, priority, assigneeId, dueDate, estimatedHours, tags }
 * @returns {Promise} 返回创建的任务
 */
export const createTask = async (taskData) => {
  const response = await api.post('/tasks', taskData);
  return response.data;
};

/**
 * 更新任务
 * @param {String} taskId - 任务ID
 * @param {Object} taskData - 任务更新数据
 * @returns {Promise} 返回更新后的任务
 */
export const updateTask = async (taskId, taskData) => {
  const response = await api.put(`/tasks/${taskId}`, taskData);
  return response.data;
};

/**
 * 删除任务
 * @param {String} taskId - 任务ID
 * @returns {Promise} 返回删除响应
 */
export const deleteTask = async (taskId) => {
  const response = await api.delete(`/tasks/${taskId}`);
  return response.data;
};

/**
 * 更新任务状态
 * @param {String} taskId - 任务ID
 * @param {String} status - 新状态 { todo, in_progress, done, cancelled }
 * @returns {Promise} 返回更新后的任务
 */
export const updateTaskStatus = async (taskId, status) => {
  const response = await api.patch(`/tasks/${taskId}/status`, { status });
  return response.data;
};

/**
 * 分配任务给用户
 * @param {String} taskId - 任务ID
 * @param {String} assigneeId - 被分配用户ID
 * @returns {Promise} 返回更新后的任务
 */
export const assignTask = async (taskId, assigneeId) => {
  const response = await api.post(`/tasks/${taskId}/assign`, { assigneeId });
  return response.data;
};

/**
 * 取消任务分配
 * @param {String} taskId - 任务ID
 * @returns {Promise} 返回更新后的任务
 */
export const unassignTask = async (taskId) => {
  const response = await api.post(`/tasks/${taskId}/unassign`);
  return response.data;
};

/**
 * 获取分配给我的任务
 * @param {Object} params - 查询参数 { page, limit, status, priority, type, search }
 * @returns {Promise} 返回任务列表和分页信息
 */
export const getMyTasks = async (params = {}) => {
  const response = await api.get('/tasks', {
    params: { ...params, assignedToMe: true }
  });
  return response.data;
};

/**
 * 获取我创建的任务
 * @param {Object} params - 查询参数 { page, limit, status, priority, type, search }
 * @returns {Promise} 返回任务列表和分页信息
 */
export const getCreatedTasks = async (params = {}) => {
  const response = await api.get('/tasks', {
    params: { ...params, createdByMe: true }
  });
  return response.data;
};

/**
 * 搜索任务
 * @param {String} query - 搜索关键词
 * @param {Object} params - 其他查询参数
 * @returns {Promise} 返回任务列表
 */
export const searchTasks = async (query, params = {}) => {
  const response = await api.get('/tasks', {
    params: { ...params, search: query }
  });
  return response.data;
};

/**
 * 批量更新任务
 * @param {Array} tasks - 任务数组 [{ id, status, priority }]
 * @returns {Promise} 返回更新后的任务数组
 */
export const batchUpdateTasks = async (tasks) => {
  const promises = tasks.map(task =>
    api.put(`/tasks/${task.id}`, task)
  );
  const responses = await Promise.all(promises);
  return responses.map(r => r.data);
};

export default {
  getTasks,
  getTaskById,
  createTask,
  updateTask,
  deleteTask,
  updateTaskStatus,
  assignTask,
  unassignTask,
  getMyTasks,
  getCreatedTasks,
  searchTasks,
  batchUpdateTasks,
};
