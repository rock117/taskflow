import React, { useEffect, useState } from 'react';
import { Row, Col, Card, Statistic, List, Progress, Tag, Avatar } from 'antd';
import {
  ProjectOutlined,
  CheckSquareOutlined,
  ClockCircleOutlined,
  UserOutlined,
  ArrowUpOutlined,
  ArrowDownOutlined
} from '@ant-design/icons';
import { useSelector } from 'react-redux';
import { fetchTasks } from '../../store/taskSlice';
import { fetchProjects } from '../../store/projectSlice';
import { useDispatch } from 'react-redux';

const DashboardPage = () => {
  const dispatch = useDispatch();
  const { tasks, taskStats } = useSelector((state) => state.tasks);
  const { projects } = useSelector((state) => state.projects);
  const { user } = useSelector((state) => state.auth);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    setLoading(true);
    try {
      await Promise.all([
        dispatch(fetchTasks()),
        dispatch(fetchProjects())
      ]);
    } catch (error) {
      console.error('Failed to load dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  const recentTasks = tasks.slice(0, 5);

  return (
    <div>
      <h2 style={{ marginBottom: '24px' }}>仪表板</h2>

      <Row gutter={[16, 16]}>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="项目总数"
              value={projects.length}
              prefix={<ProjectOutlined />}
              valueStyle={{ color: '#3f8600' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="任务总数"
              value={tasks.length}
              prefix={<CheckSquareOutlined />}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="进行中"
              value={taskStats?.inProgress || 0}
              prefix={<ClockCircleOutlined />}
              valueStyle={{ color: '#1890ff' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="已完成"
              value={taskStats?.completed || 0}
              prefix={<CheckSquareOutlined />}
              valueStyle={{ color: '#52c41a' }}
            />
          </Card>
        </Col>
      </Row>

      <Row gutter={[16, 16]} style={{ marginTop: '24px' }}>
        <Col xs={24} lg={16}>
          <Card
            title="任务进度"
            extra={<a href="/tasks">查看全部</a>}
            loading={loading}
          >
            {tasks.length === 0 ? (
              <div style={{ textAlign: 'center', padding: '40px 0', color: '#999' }}>
                暂无任务
              </div>
            ) : (
              <List
                dataSource={recentTasks}
                renderItem={(task) => (
                  <List.Item
                    actions={[
                      <Tag color={task.status === 'completed' ? 'success' : 'processing'}>
                        {task.status}
                      </Tag>
                    ]}
                  >
                    <List.Item.Meta
                      avatar={<Avatar icon={<CheckSquareOutlined />} />}
                      title={task.title}
                      description={
                        <div>
                          <div>{task.description?.substring(0, 50)}...</div>
                          <Progress
                            percent={task.progress || 0}
                            size="small"
                            style={{ marginTop: '8px' }}
                          />
                        </div>
                      }
                    />
                  </List.Item>
                )}
              />
            )}
          </Card>
        </Col>

        <Col xs={24} lg={8}>
          <Card
            title="项目概览"
            extra={<a href="/projects">查看全部</a>}
            loading={loading}
          >
            {projects.length === 0 ? (
              <div style={{ textAlign: 'center', padding: '40px 0', color: '#999' }}>
                暂无项目
              </div>
            ) : (
              <List
                dataSource={projects.slice(0, 5)}
                renderItem={(project) => (
                  <List.Item>
                    <List.Item.Meta
                      avatar={<Avatar icon={<ProjectOutlined />} />}
                      title={project.name}
                      description={
                        <div>
                          <Tag color={project.status === 'active' ? 'success' : 'default'}>
                            {project.status}
                          </Tag>
                        </div>
                      }
                    />
                  </List.Item>
                )}
              />
            )}
          </Card>
        </Col>
      </Row>

      <Row gutter={[16, 16]} style={{ marginTop: '24px' }}>
        <Col xs={24} lg={12}>
          <Card title="任务统计" loading={loading}>
            {taskStats && (
              <div>
                <div style={{ marginBottom: '16px' }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '8px' }}>
                    <span>待处理</span>
                    <Tag color="warning">{taskStats.pending || 0}</Tag>
                  </div>
                  <Progress percent={((taskStats.pending || 0) / tasks.length) * 100} status="active" />
                </div>
                <div style={{ marginBottom: '16px' }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '8px' }}>
                    <span>进行中</span>
                    <Tag color="processing">{taskStats.inProgress || 0}</Tag>
                  </div>
                  <Progress percent={((taskStats.inProgress || 0) / tasks.length) * 100} status="active" />
                </div>
                <div style={{ marginBottom: '16px' }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '8px' }}>
                    <span>已完成</span>
                    <Tag color="success">{taskStats.completed || 0}</Tag>
                  </div>
                  <Progress percent={((taskStats.completed || 0) / tasks.length) * 100} status="success" />
                </div>
              </div>
            )}
          </Card>
        </Col>

        <Col xs={24} lg={12}>
          <Card title="本周效率" loading={loading}>
            <Row gutter={16}>
              <Col span={12}>
                <Statistic
                  title="完成任务"
                  value={12}
                  prefix={<ArrowUpOutlined />}
                  suffix="/周"
                  valueStyle={{ color: '#3f8600' }}
                />
              </Col>
              <Col span={12}>
                <Statistic
                  title="处理时间"
                  value={2.4}
                  prefix={<ClockCircleOutlined />}
                  suffix="小时"
                  precision={1}
                />
              </Col>
            </Row>
            <div style={{ marginTop: '16px', textAlign: 'center', color: '#999' }}>
              <UserOutlined /> 你的工作效率提升了 15%
            </div>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default DashboardPage;