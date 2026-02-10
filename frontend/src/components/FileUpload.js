import React, { useState } from 'react';
import { Upload, Button, List, Progress, Space, message, Card, Tag } from 'antd';
import {
  UploadOutlined,
  FileOutlined,
  DeleteOutlined,
  DownloadOutlined,
  EyeOutlined
} from '@ant-design/icons';
import { useSelector } from 'react-redux';
import { useDispatch } from 'react-redux';

const FileUpload = ({ taskId, projectId, attachments, onFileUploaded, onFileDeleted }) => {
  const [uploading, setUploading] = useState(false);
  const [fileList, setFileList] = useState(attachments || []);
  
  const dispatch = useDispatch();
  const { user } = useSelector((state) => state.auth);

  const handleUpload = async (file) => {
    const formData = new FormData();
    formData.append('file', file);
    if (taskId) formData.append('taskId', taskId);
    if (projectId) formData.append('projectId', projectId);

    try {
      setUploading(true);
      const response = await dispatch({
        type: 'attachments/uploadFile',
        payload: {
          file: formData,
          taskId,
          projectId
        }
      });

      if (response.success || !response.error) {
        message.success('文件上传成功');
        const newAttachment = {
          id: Date.now(),
          fileName: file.name,
          fileSize: file.size,
          formattedSize: formatFileSize(file.size),
          fileExtension: file.name.split('.').pop(),
          uploadTime: new Date().toISOString()
        };
        setFileList([...fileList, newAttachment]);
        onFileUploaded?.(newAttachment);
      }
    } catch (error) {
      message.error('文件上传失败');
    } finally {
      setUploading(false);
    }
    return false;
  };

  const handleDelete = async (attachmentId) => {
    try {
      const result = await dispatch({
        type: 'attachments/deleteAttachment',
        payload: {
          attachmentId,
          userId: user.id
        }
      });

      if (result.success || !result.error) {
        message.success('文件删除成功');
        setFileList(fileList.filter(f => f.id !== attachmentId));
        onFileDeleted?.(attachmentId);
      }
    } catch (error) {
      message.error('文件删除失败');
    }
  };

  const handleDownload = (attachment) => {
    const link = document.createElement('a');
    link.href = attachment.downloadUrl || `/api/attachments/${attachment.id}/download`;
    link.download = attachment.fileName;
    link.click();
  };

  const formatFileSize = (bytes) => {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  };

  const getFileIcon = (extension) => {
    const iconMap = {
      'pdf': '📄',
      'doc': '📝',
      'docx': '📝',
      'xls': '📊',
      'xlsx': '📊',
      'ppt': '📽️',
      'pptx': '📽️',
      'jpg': '🖼️',
      'jpeg': '🖼️',
      'png': '🖼️',
      'gif': '🖼️',
      'zip': '📦',
      'rar': '📦',
      'txt': '📃',
      'mp3': '🎵',
      'mp4': '🎬',
      'avi': '🎬'
    };
    return iconMap[extension.toLowerCase()] || '📎';
  };

  const uploadProps = {
    name: 'file',
    multiple: true,
    showUploadList: false,
    beforeUpload: handleUpload,
    headers: {
      authorization: `Bearer ${user.token}`
    }
  };

  return (
    <div>
      <Card
        title="附件"
        extra={
          <Upload {...uploadProps}>
            <Button
              icon={<UploadOutlined />}
              loading={uploading}
            >
              上传文件
            </Button>
          </Upload>
        }
        style={{ marginBottom: '24px' }}
      >
        {fileList.length === 0 ? (
          <div style={{ textAlign: 'center', padding: '40px 0', color: '#999' }}>
            <FileOutlined style={{ fontSize: '48px', marginBottom: '16px' }} />
            <p>暂无附件</p>
          </div>
        ) : (
          <List
            dataSource={fileList}
            renderItem={(item) => (
              <List.Item
                actions={[
                  <EyeOutlined
                    key="preview"
                    style={{ cursor: 'pointer', color: '#1890ff' }}
                    onClick={() => window.open(item.downloadUrl, '_blank')}
                  />,
                  <DownloadOutlined
                    key="download"
                    style={{ cursor: 'pointer', color: '#1890ff' }}
                    onClick={() => handleDownload(item)}
                  />,
                  <DeleteOutlined
                    key="delete"
                    style={{ cursor: 'pointer', color: '#ff4d4f' }}
                    onClick={() => handleDelete(item.id)}
                  />
                ]}
              >
                <List.Item.Meta
                  avatar={
                    <span style={{ fontSize: '24px' }}>
                      {getFileIcon(item.fileExtension)}
                    </span>
                  }
                  title={
                    <Space>
                      <span>{item.fileName}</span>
                      <Tag color="blue">{item.fileExtension?.toUpperCase()}</Tag>
                    </Space>
                  }
                  description={
                    <div>
                      <Space>
                        <span style={{ color: '#999' }}>
                          大小: {item.formattedSize}
                        </span>
                        <span style={{ color: '#999' }}>
                          上传时间: {item.uploadTime}
                        </span>
                      </Space>
                    </div>
                  }
                />
              </List.Item>
            )}
          />
        )}
      </Card>
    </div>
  );
};

export default FileUpload;