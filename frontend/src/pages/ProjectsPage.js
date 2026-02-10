import React, { useEffect, useState } from 'react';
import {
  Card,
  List,
  Button,
  Tag,
  Avatar,
  Input,
  Select,
  Space,
  Modal,
  Form,
  DatePicker,
  message
} from 'antd';
import {
  PlusOutlined,
  SearchOutlined,
  EyeOutlined,
  EditOutlined,
  DeleteOutlined,
  UserOutlined
} from '@ant-design/icons';
import { useSelector, useDispatch } from 'react-redux';
import { fetchProjects, createProject } from '../../store/projectSlice';
import { useNavigate } from 'react-router-dom';

const { Search } = Input;
const { Option } = Select;

const ProjectsPage = () => {
  const [loading, setLoading] = useState(false);
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [searchKeyword, setSearchKeyword] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [form] = Form.useForm();
  
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const { projects } = useSelector((state) => state.projects);
  const { user } = useSelector((state) => state.auth);

  useEffect(() => {
    loadProjects();
  }, []);

  const loadProjects = async () => {
    setLoading(true);
    try {
      await dispatch(fetchProjects());
    } catch (error) {
      message.error('加载项目列表失败');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateProject = async (values) => {
    try {
      setLoading(true);
      const result = await dispatch(createProject({
        name: values.name,
        description: values.description,
        ownerId: user.id,
        startDate: values.startDate?.format('YYYY-MM-DD'),
        endDate: values.endDate?.format('YYYY-MM-DD'),
        status: 'active'
      })).unwrap();

      if (result.success) {
        message.success('项目创建成功');
        setIsModalVisible(false);
        form.resetFields();
        loadProjects();
      }
    } catch (error) {
      message.error(error.message || '创建项目失败');
    } finally {
      setLoading(false);
    }
  };

  const filteredProjects = projects.filter(project => {
    const matchesKeyword = project.name.toLowerCase().includes(searchKeyword.toLowerCase()) ||
                         project.description?.toLowerCase().includes(searchKeyword.toLowerCase());
    const matchesStatus = statusFilter === 'all' || project.status === statusFilter;
    return matchesKeyword && matchesStatus;
  });

  const getStatusColor = (status) => {
    switch (status) {
      case 'active': return 'success';
      case 'pending': return 'warning';
      case 'completed': return 'default';
      case 'archived': return 'default';
      default: return 'default';
    }
  };

  return (
    <div>
      <div style={{
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginBottom: '24px'
      }}>
        <h2>项目管理</h2>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => setIsModalVisible(true)}
        >
          新建项目
        </Button>
      </div>

      <Card>
        <Space style={{ marginBottom: '16px', width: '100%' }} size="large">
          <Search
            placeholder="搜索项目"
            allowClear
            enterButton={<SearchOutlined />}
            style={{ width: 300 }}
            onSearch={setSearchKeyword}
            onChange={(e) => setSearchKeyword(e.target.value)}
          />
          <Select
            placeholder="选择状态"
            style={{ width: 150 }}
            value={statusFilter}
            onChange={setStatusFilter}
          >
            <Option value="all">全部状态</Option>
            <Option value="active">进行中</Option>
            <Option value="pending">待开始</Option>
            <Option value="completed">已完成</Option>
            <Option value="archived">已归档</Option>
          </Select>
        </Space>

        <List
          loading={loading}
          grid={{ gutter: 16, xs: 1, sm: 2, md: 2, lg: 3, xl: 3, xxl: 3 }}
          dataSource={filteredProjects}
          renderItem={(project) => (
            <List.Item>
              <Card
                hoverable
                style={{ width: '100%', height: '100%' }}
                actions={[
                  <EyeOutlined key="view" onClick={() => navigate(`/projects/${project.id}`)} />,
                  <EditOutlined key="edit" />,
                  <DeleteOutlined key="delete" />
                ]}
              >
                <Card.Meta
                  avatar={<Avatar icon={<UserOutlined />} />}
                  title={
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                      <span>{project.name}</span>
                      <Tag color={getStatusColor(project.status)}>{project.status}</Tag>
                    </div>
                  }
                  description={
                    <div>
                      <div style={{ marginBottom: '8px', color: '#666' }}>
                        {project.description?.substring(0, 80)}...
                      </div>
                      <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '12px', color: '#999' }}>
                        <span>创建者: {project.ownerName || user.username}</span>
                        <span>
                          {project.startDate && `${project.startDate} ~`}
                          {project.endDate && ` ${project.endDate}`}
                        </span>
                      </div>
                    </div>
                  }
                />
              </Card>
            </List.Item>
          )}
        />
      </Card>

      <Modal
        title="新建项目"
        open={isModalVisible}
        onCancel={() => setIsModalVisible(false)}
        footer={[
          <Button key="cancel" onClick={() => setIsModalVisible(false)}>
            取消
          </Button>,
          <Button
            key="submit"
            type="primary"
            loading={loading}
            onClick={() => form.submit()}
          >
            创建
          </Button>
        ]}
      >
        <Form
          form={form}
          layout="vertical"
          onFinish={handleCreateProject}
        >
          <Form.Item
            name="name"
            label="项目名称"
            rules={[
              { required: true, message: '请输入项目名称' },
              { max: 100, message: '项目名称最多100个字符' }
            ]}
          >
            <Input placeholder="请输入项目名称" />
          </Form.Item>

          <Form.Item
            name="description"
            label="项目描述"
            rules={[
              { required: true, message: '请输入项目描述' },
              { max: 500, message: '项目描述最多500个字符' }
            ]}
          >
            <Input.TextArea
              placeholder="请输入项目描述"
              rows={4}
            />
          </Form.Item>

          <Form.Item
            name="startDate"
            label="开始日期"
            rules={[{ required: true, message: '请选择开始日期' }]}
          >
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item
            name="endDate"
            label="结束日期"
          >
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default ProjectsPage;