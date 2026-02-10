import api from './api';

/**
 * 获取任务的所有评论
 * @param {String} taskId - 任务ID
 * @returns {Promise} 返回评论列表
 */
export const getTaskComments = async (taskId) => {
  const response = await api.get(`/comments/task/${taskId}`);
  return response.data;
};

/**
 * 创建新评论
 * @param {Object} commentData - 评论数据 { taskId, content, parentId }
 * @returns {Promise} 返回创建的评论
 */
export const createComment = async (commentData) => {
  const response = await api.post('/comments', commentData);
  return response.data;
};

/**
 * 获取单个评论详情
 * @param {String} commentId - 评论ID
 * @returns {Promise} 返回评论详情
 */
export const getComment = async (commentId) => {
  const response = await api.get(`/comments/${commentId}`);
  return response.data;
};

/**
 * 更新评论
 * @param {String} commentId - 评论ID
 * @param {Object} commentData - 评论更新数据 { content }
 * @returns {Promise} 返回更新后的评论
 */
export const updateComment = async (commentId, commentData) => {
  const response = await api.put(`/comments/${commentId}`, commentData);
  return response.data;
};

/**
 * 删除评论
 * @param {String} commentId - 评论ID
 * @returns {Promise} 返回删除响应
 */
export const deleteComment = async (commentId) => {
  const response = await api.delete(`/comments/${commentId}`);
  return response.data;
};

/**
 * 添加表情反应到评论
 * @param {String} commentId - 评论ID
 * @param {String} emoji - 表情符号
 * @returns {Promise} 返回更新后的反应数据
 */
export const addReaction = async (commentId, emoji) => {
  const response = await api.post(`/comments/${commentId}/reaction`, { emoji });
  return response.data;
};

/**
 * 从评论移除表情反应
 * @param {String} commentId - 评论ID
 * @param {String} emoji - 表情符号
 * @returns {Promise} 返回更新后的反应数据
 */
export const removeReaction = async (commentId, emoji) => {
  const response = await api.delete(`/comments/${commentId}/reaction`, { data: { emoji } });
  return response.data;
};

/**
 * 获取用户的所有评论
 * @param {Object} params - 查询参数 { page, limit }
 * @returns {Promise} 返回用户评论列表和分页信息
 */
export const getUserComments = async (params = {}) => {
  const response = await api.get(`/comments/user/${params.userId}`, { params });
  return response.data;
};

/**
 * 回复评论
 * @param {String} parentCommentId - 父评论ID
 * @param {Object} replyData - 回复数据 { taskId, content }
 * @returns {Promise} 返回创建的回复
 */
export const replyToComment = async (parentCommentId, replyData) => {
  const response = await api.post('/comments', {
    ...replyData,
    parentId: parentCommentId,
  });
  return response.data;
};

/**
 * 获取评论的回复
 * @param {String} commentId - 评论ID
 * @returns {Promise} 返回回复列表
 */
export const getCommentReplies = async (commentId) => {
  const response = await api.get(`/comments/${commentId}`);
  return response.data.replies || [];
};

/**
 * 批量删除评论
 * @param {Array} commentIds - 评论ID数组
 * @returns {Promise} 返回删除响应
 */
export const batchDeleteComments = async (commentIds) => {
  const promises = commentIds.map(id => api.delete(`/comments/${id}`));
  const responses = await Promise.all(promises);
  return responses.map(r => r.data);
};

export default {
  getTaskComments,
  createComment,
  getComment,
  updateComment,
  deleteComment,
  addReaction,
  removeReaction,
  getUserComments,
  replyToComment,
  getCommentReplies,
  batchDeleteComments,
};
