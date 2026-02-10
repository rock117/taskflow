import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Card,
  Descriptions,
  Tag,
  Button,
  Space,
  List,
  Avatar,
  message,
  Modal,
  Form,
  Input,
  Select,
  DatePicker,
  Progress,
  Tabs
} from 'antd';
import {
  ArrowLeftOutlined,
  EditOutlined,
  DeleteOutlined,
  UserOutlined,
  ClockCircleOutlined,
  CheckCircleOutlined
} from '@ant-design/icons';
import { useSelector, useDispatch } from 'react-redux';
import { fetchTaskById, updateTask } from '../../store/taskSlice';

const { TextArea } = Input;
const { Option } = Select;
const { TabPane } = Tabs;

const TaskDetailPage = () => {
  const { taskId } = useParams();
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const { currentTask } = useSelector((state) => state.tasks);
  const { user } = useSelector((state) => state.auth);
  
  const [loading, setLoading] = useState(false);
  const [isEditModalVisible, setIsEditModalVisible] = useState(false);
  const [activeTab, setActiveTab] = useState('comments');
  const [form] = Form.useForm();

  useEffect(() => {
    if (taskId) {
      loadTask();
    }
  }, [taskId]);

  const loadTask = async () => {
    setLoading(true);
    try {
      await dispatch(fetchTaskById(taskId));
    } catch (error) {
      message.error('加载任务详情失败');
    } finally {
      setLoading(false);
    }
  };

  const handleUpdateTask = async (values) => {
    try {
      setLoading(true);
      const result = await dispatch(updateTask({
        taskId,
        updateTaskDto: {
          ...values,
          currentUserId: user.id
        }
      })).unwrap();

      if (result.success) {
        message.success('任务更新成功');
        setIsEditModalVisible(false);
        loadTask();
      }
    } catch (error) {
      message.error(error.message || '更新任务失败');
    } finally {
      setLoading(false);
    }
  };

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
      case 'low': return 'default';
      case 'medium': return 'warning';
      case 'high': return 'error';
      default: return 'default';
    }
  };

  if (!currentTask) {
    return <div>加载中...</div>;
  }

  return (
    <div>
      <div style={{ marginBottom: '24px' }}>
        <Button
          icon={<ArrowLeftOutlined />}
          onClick={() => navigate(-1)}
        >
          返回
        </Button>
      </div>

      <Card
        title={
          <Space>
            <span>{currentTask.title}</span>
            <Tag color={getStatusColor(currentTask.status)}>{currentTask.status}</Tag>
            <Tag color={getPriorityColor(currentTask.priority)}>{currentTask.priority}</Tag>
          </Space>
        }
        extra={
          <Space>
            <Button
              type="primary"
              icon={<EditOutlined />}
              onClick={() => setIsEditModalVisible(true)}
            >
              编辑
            </Button>
            <Button
              danger
              icon={<DeleteOutlined />}
            >
              删除
            </Button>
          </Space>
        }
        loading={loading}
      >
        <Descriptions column={2} bordered>
          <Descriptions.Item label="任务标题" span={2}>
            {currentTask.title}
          </Descriptions.Item>
          <Descriptions.Item label="任务描述" span={2}>
            {currentTask.description}
          </Descriptions.Item>
          <Descriptions.Item label="创建时间">
            {currentTask.createdTime}
          </Descriptions.Item>
          <Descriptions.Item label="截止时间">
            {currentTask.dueDate}
          </Descriptions.Item>
          <Descriptions.Item label="创建者">
            <Space>
              <Avatar icon={<UserOutlined />} size="small" />
              <span>{currentTask.creatorName || user.username}</span>
            </Space>
          </Descriptions.Item>
          <Descriptions.Item label="指派给">
            <Space>
              <Avatar icon={<UserOutlined />} size="small" />
              <span>{currentTask.assigneeName || '未指派'}</span>
            </Space>
          </Descriptions.Item>
          <Descriptions.Item label="进度" span={2}>
            <Progress percent={currentTask.progress || 0} status="active" />
          </Descriptions.Item>
          <Descriptions.Item label="标签" span={2}>
            <Space wrap>
              {currentTask.tags?.map((tag, index) => (
                <Tag key={index}>{tag}</Tag>
              ))}
            </Space>
          </Descriptions.Item>
        </Descriptions>

        <Tabs
          activeKey={activeTab}
          onChange={setActiveTab}
          style={{ marginTop: '24px' }}
        >
          <TabPane tab="评论" key="comments">
            <div style={{ textAlign: 'center', padding: '40px 0', color: '#999' }}>
              暂无评论
            </div>
          </TabPane>
          <TabPane tab="附件" key="attachments">
            <div style={{ textAlign: 'center', padding: '40px 0', color: '#999' }}>
              暂无附件
            </div>
          </TabPane>
          <TabPane tab="活动记录" key="activities">
            <List
              dataSource={[]}
              renderItem={(item) => (
                <List.Item>
                  <List.Item.Meta
                    avatar={<Avatar icon={<ClockCircleOutlined />} />}
                    title={item.title}
                    description={item.description}
                  />
                </List.Item>
              )}
            />
          </TabPane>
        </Tabs>
      </Card>

      <Modal
        title="编辑任务"
        open={isEditModalVisible}
        onCancel={() => setIsEditModalVisible(false)}
        footer={[
          <Button key="cancel" onClick={() => setIsEditModalVisible(false)}>
            取消
          </Button>,
          <Button
            key="submit"
            type="primary"
            loading={loading}
            onClick={() => form.submit()}
          >
            保存
          </Button>
        ]}
      >
        <Form
          form={form}
          layout="vertical"
          initialValues={{
            title: currentTask.title,
            description: currentTask.description,
            status: currentTask.status,
            priority: currentTask.priority,
            progress: currentTask.progress,
            dueDate: currentTask.dueDate
          }}
          onFinish={handleUpdateTask}
        >
          <Form.Item
            name="title"
            label="任务标题"
            rules={[
              { required: true, message: '请输入任务标题' }
            ]}
          >
            <Input placeholder="请输入任务标题" />
          </Form.Item>

          <Form.Item
            name="description"
            label="任务描述"
          >
            <TextArea rows={4} placeholder="请输入任务描述" />
          </Form.Item>

          <Form.Item
            name="status"
            label="状态"
          >
            <Select>
              <Option value="pending">待处理</Option>
              <Option value="inProgress">进行中</Option>
              <Option value="completed">已完成</Option>
              <Option value="cancelled">已取消</Option>
            </Select>
          </Form.Item>

          <Form.Item
            name="priority"
            label="优先级"
          >
            <Select>
              <Option value="low">低</Option>
              <Option value="medium">中</Option>
              <Option value="high">高</Option>
            </Select>
          </Form.Item>

          <Form.Item
            name="progress"
            label="进度"
          >
            <Input type="number" min={0} max={100} placeholder="0-100" />
          </Form.Item>

          <Form.Item
            name="dueDate"
            label="截止日期"
          >
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default TaskDetailPage;