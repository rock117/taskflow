import api from './api';

export const attachmentService = {
  /**
   * 上传文件
   */
  uploadFile: async (taskId, projectId, file) => {
    const formData = new FormData();
    formData.append('file', file);
    if (taskId) formData.append('taskId', taskId);
    if (projectId) formData.append('projectId', projectId);

    const response = await api.post('/attachments/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    return response.data;
  },

  /**
   * 批量上传文件
   */
  uploadFiles: async (taskId, projectId, files) => {
    const formData = new FormData();
    files.forEach(file => formData.append('files', file));
    if (taskId) formData.append('taskId', taskId);
    if (projectId) formData.append('projectId', projectId);

    const response = await api.post('/attachments/upload-batch', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    return response.data;
  },

  /**
   * 获取附件详情
   */
  getAttachment: async (attachmentId) => {
    const response = await api.get(`/attachments/${attachmentId}`);
    return response.data;
  },

  /**
   * 下载文件
   */
  downloadFile: (attachmentId, fileName) => {
    const url = `${api.defaults.baseURL}/attachments/${attachmentId}/download`;
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    link.click();
  },

  /**
   * 预览文件
   */
  previewFile: (attachmentId) => {
    const url = `${api.defaults.baseURL}/attachments/${attachmentId}/preview`;
    window.open(url, '_blank');
  },

  /**
   * 获取任务的附件列表
   */
  getTaskAttachments: async (taskId) => {
    const response = await api.get(`/attachments/task/${taskId}`);
    return response.data;
  },

  /**
   * 获取项目的附件列表
   */
  getProjectAttachments: async (projectId) => {
    const response = await api.get(`/attachments/project/${projectId}`);
    return response.data;
  },

  /**
   * 获取用户的附件列表
   */
  getMyAttachments: async (params = {}) => {
    const response = await api.get('/attachments/my-attachments', { params });
    return response.data;
  },

  /**
   * 更新附件信息
   */
  updateAttachment: async (attachmentId, data) => {
    const response = await api.put(`/attachments/${attachmentId}`, data);
    return response.data;
  },

  /**
   * 移动附件到其他任务
   */
  moveAttachment: async (attachmentId, newTaskId) => {
    const response = await api.post(`/attachments/${attachmentId}/move`, {
      newTaskId
    });
    return response.data;
  },

  /**
   * 删除附件
   */
  deleteAttachment: async (attachmentId) => {
    const response = await api.delete(`/attachments/${attachmentId}`);
    return response.data;
  },

  /**
   * 批量删除附件
   */
  batchDeleteAttachments: async (attachmentIds) => {
    const response = await api.post('/attachments/batch-delete', {
      attachmentIds
    });
    return response.data;
  },

  /**
   * 获取文件统计信息
   */
  getFileStatistics: async () => {
    const response = await api.get('/attachments/statistics');
    return response.data;
  }
};

export default attachmentService;