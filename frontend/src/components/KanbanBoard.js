import React, { useState } from 'react';
import { Row, Col, Card, Space, Button, Tag, Avatar, Badge } from 'antd';
import {
  PlusOutlined,
  MoreOutlined,
  UserOutlined,
  ClockCircleOutlined
} from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';

const KanbanBoard = ({ tasks, onTaskDrop, onStatusChange }) => {
  const navigate = useNavigate();

  const columns = [
    { id: 'pending', title: '待处理', color: 'default' },
    { id: 'inProgress', title: '进行中', color: 'processing' },
    { id: 'completed', title: '已完成', color: 'success' },
    { id: 'cancelled', title: '已取消', color: 'error' }
  ];

  const handleDragStart = (e, taskId) => {
    e.dataTransfer.setData('taskId', taskId);
  };

  const handleDragOver = (e) => {
    e.preventDefault();
  };

  const handleDrop = (e, newStatus) => {
    e.preventDefault();
    const taskId = e.dataTransfer.getData('taskId');
    if (onStatusChange) {
      onStatusChange(taskId, newStatus);
    }
  };

  const getTaskCount = (status) => {
    return tasks.filter(task => task.status === status).length;
  };

  return (
    <div style={{ height: 'calc(100vh - 200px)', overflowX: 'auto' }}>
      <Row gutter={16} style={{ minWidth: '1200px' }}>
        {columns.map(column => (
          <Col key={column.id} span={6} style={{ height: '100%' }}>
            <Card
              title={
                <Space>
                  <span>{column.title}</span>
                  <Badge count={getTaskCount(column.id)} style={{ backgroundColor: '#52c41a' }} />
                </Space>
              }
              extra={<PlusOutlined style={{ cursor: 'pointer' }} />}
              style={{
                height: '100%',
                backgroundColor: column.id === 'completed' ? '#f6ffed' : column.id === 'cancelled' ? '#fff2f0' : '#fafafa'
              }}
              bodyStyle={{
                padding: '12px',
                height: 'calc(100% - 56px)',
                overflow: 'auto'
              }}
              onDragOver={handleDragOver}
              onDrop={(e) => handleDrop(e, column.id)}
            >
              {tasks
                .filter(task => task.status === column.id)
                .map(task => (
                  <Card
                    key={task.id}
                    size="small"
                    style={{
                      marginBottom: '12px',
                      cursor: 'pointer',
                      borderLeft: task.priority === 'high' ? '4px solid #ff4d4f' : 
                                 task.priority === 'medium' ? '4px solid #faad14' : '4px solid #52c41a'
                    }}
                    draggable
                    onDragStart={(e) => handleDragStart(e, task.id)}
                    onClick={() => navigate(`/tasks/${task.id}`)}
                  >
                    <div style={{ marginBottom: '8px' }}>
                      <Space size="small">
                        <Tag color={column.color}>{task.status}</Tag>
                        <Tag color={task.priority === 'high' ? 'red' : task.priority === 'medium' ? 'orange' : 'green'}>
                          {task.priority}
                        </Tag>
                      </Space>
                    </div>
                    
                    <h4 style={{ margin: '0 0 8px 0', fontSize: '14px' }}>
                      {task.title}
                    </h4>
                    
                    <p style={{ 
                      margin: 0, 
                      color: '#666', 
                      fontSize: '12px',
                      overflow: 'hidden',
                      textOverflow: 'ellipsis',
                      whiteSpace: 'nowrap'
                    }}>
                      {task.description}
                    </p>

                    <div style={{ 
                      display: 'flex', 
                      justifyContent: 'space-between', 
                      alignItems: 'center',
                      marginTop: '12px',
                      paddingTop: '8px',
                      borderTop: '1px solid #f0f0f0'
                    }}>
                      <Space size="small">
                        <Avatar icon={<UserOutlined />} size="small" />
                        <span style={{ fontSize: '12px', color: '#999' }}>
                          {task.assigneeName || '未指派'}
                        </span>
                      </Space>
                      
                      {task.dueDate && (
                        <Space size="small">
                          <ClockCircleOutlined style={{ fontSize: '12px', color: '#999' }} />
                          <span style={{ fontSize: '12px', color: '#999' }}>
                            {task.dueDate}
                          </span>
                        </Space>
                      )}
                    </div>

                    {task.tags && task.tags.length > 0 && (
                      <div style={{ marginTop: '8px' }}>
                        <Space size="small" wrap>
                          {task.tags.slice(0, 3).map((tag, index) => (
                            <Tag key={index} style={{ fontSize: '11px' }}>{tag}</Tag>
                          ))}
                          {task.tags.length > 3 && (
                            <Tag style={{ fontSize: '11px' }}>+{task.tags.length - 3}</Tag>
                          )}
                        </Space>
                      </div>
                    )}

                    {task.attachments && task.attachments.length > 0 && (
                      <div style={{ marginTop: '8px' }}>
                        <Tag icon={<span>📎</span>} style={{ fontSize: '11px' }}>
                          {task.attachments.length} 附件
                        </Tag>
                      </div>
                    )}
                  </Card>
                ))}
              </Card>
            </Col>
        ))}
      </Row>
    </div>
  );
};

export default KanbanBoard;