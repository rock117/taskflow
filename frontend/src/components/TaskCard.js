import React from 'react';
import { Card, Tag, Avatar, Progress, Space, Dropdown, Tooltip } from 'antd';
import {
  EditOutlined,
  DeleteOutlined,
  UserOutlined,
  CalendarOutlined,
  FlagOutlined
} from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';

const TaskCard = ({ task, onEdit, onDelete }) => {
  const navigate = useNavigate();

  const getStatusColor = (status) => {
    switch (status) {
      case 'pending': return 'default';
      case 'inProgress': return 'processing';
      case 'completed': return 'success';
      case 'cancelled': return 'error';
      default: return 'default';
    }
  };

  const getPriorityColor = (priority) => {
    switch (priority) {
      case 'low': return 'green';
      case 'medium': return 'orange';
      case 'high': return 'red';
      default: return 'default';
    }
  };

  const priorityLabels = {
    low: '低',
    medium: '中',
    high: '高'
  };

  const statusLabels = {
    pending: '待处理',
    inProgress: '进行中',
    completed: '已完成',
    cancelled: '已取消'
  };

  return (
    <Card
      hoverable
      style={{ marginBottom: '16px' }}
      actions={[
        <Tooltip title="编辑" key="edit">
          <EditOutlined onClick={() => onEdit?.(task)} />
        </Tooltip>,
        <Tooltip title="删除" key="delete">
          <DeleteOutlined onClick={() => onDelete?.(task.id)} />
        </Tooltip>
      ]}
      onClick={() => navigate(`/tasks/${task.id}`)}
    >
      <div style={{ marginBottom: '12px' }}>
        <Space size="small">
          <Tag color={getPriorityColor(task.priority)}>
            <FlagOutlined /> {priorityLabels[task.priority]}
          </Tag>
          <Tag color={getStatusColor(task.status)}>
            {statusLabels[task.status]}
          </Tag>
        </Space>
      </div>

      <h3 style={{ marginBottom: '8px', marginTop: 0 }}>{task.title}</h3>
      <p style={{ color: '#666', marginBottom: '12px', minHeight: '40px' }}>
        {task.description?.substring(0, 100)}...
      </p>

      <div style={{ marginBottom: '12px' }}>
        <Progress
          percent={task.progress || 0}
          size="small"
          status={task.progress === 100 ? 'success' : 'active'}
        />
      </div>

      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Space size="small">
          <Avatar icon={<UserOutlined />} size="small" />
          <span style={{ fontSize: '12px', color: '#666' }}>
            {task.assigneeName || '未指派'}
          </span>
        </Space>
        <Space size="small">
          <CalendarOutlined style={{ color: '#999' }} />
          <span style={{ fontSize: '12px', color: '#666' }}>
            {task.dueDate || '无截止日期'}
          </span>
        </Space>
      </div>

      {task.tags && task.tags.length > 0 && (
        <div style={{ marginTop: '12px' }}>
          <Space size="small" wrap>
            {task.tags.map((tag, index) => (
              <Tag key={index}>{tag}</Tag>
            ))}
          </Space>
        </div>
      )}
    </Card>
  );
};

export default TaskCard;