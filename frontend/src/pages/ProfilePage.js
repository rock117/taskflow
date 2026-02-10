import React, { useState } from 'react';
import {
  Card,
  Form,
  Input,
  Button,
  Upload,
  Avatar,
  message,
  Divider,
  Row,
  Col,
  Space
} from 'antd';
import {
  UserOutlined,
  LockOutlined,
  UploadOutlined,
  MailOutlined,
  PhoneOutlined
} from '@ant-design/icons';
import { useSelector, useDispatch } from 'react-redux';
import { updateUser } from '../../store/authSlice';

const ProfilePage = () => {
  const [loading, setLoading] = useState(false);
  const [avatarUrl, setAvatarUrl] = useState('');
  const [form] = Form.useForm();
  
  const dispatch = useDispatch();
  const { user } = useSelector((state) => state.auth);

  const handleUpdateProfile = async (values) => {
    try {
      setLoading(true);
      const result = await dispatch(updateUser({
        updateUserDto: {
          id: user.id,
          ...values
        }
      })).unwrap();

      if (result.success) {
        message.success('个人信息更新成功');
      }
    } catch (error) {
      message.error(error.message || '更新失败');
    } finally {
      setLoading(false);
    }
  };

  const handleAvatarChange = (info) => {
    if (info.file.status === 'done') {
      setAvatarUrl(info.file.response?.data?.avatarUrl || '');
      message.success('头像上传成功');
    } else if (info.file.status === 'error') {
      message.error('头像上传失败');
    }
  };

  const uploadProps = {
    name: 'file',
    action: '/api/user/avatar',
    headers: {
      authorization: `Bearer ${user.token}`
    },
    showUploadList: false,
    beforeUpload: (file) => {
      const isJpgOrPng = file.type === 'image/jpeg' || file.type === 'image/png';
      if (!isJpgOrPng) {
        message.error('只能上传 JPG/PNG 格式的图片');
        return false;
      }
      const isLt2M = file.size / 1024 / 1024 < 2;
      if (!isLt2M) {
        message.error('图片大小不能超过 2MB');
        return false;
      }
      return true;
    }
  };

  return (
    <div>
      <h2 style={{ marginBottom: '24px' }}>个人资料</h2>

      <Row gutter={[24, 24]}>
        <Col xs={24} lg={8}>
          <Card title="头像">
            <div style={{ textAlign: 'center', padding: '24px 0' }}>
              <Avatar
                src={user.avatar || avatarUrl}
                icon={<UserOutlined />}
                size={120}
                style={{ marginBottom: '16px' }}
              />
              <Upload {...uploadProps} onChange={handleAvatarChange}>
                <Button icon={<UploadOutlined />}>更换头像</Button>
              </Upload>
              <div style={{ marginTop: '12px', color: '#999', fontSize: '12px' }}>
                支持 JPG、PNG 格式，大小不超过 2MB
              </div>
            </div>
          </Card>
        </Col>

        <Col xs={24} lg={16}>
          <Card title="基本信息">
            <Form
              form={form}
              layout="vertical"
              initialValues={{
                username: user.username,
                email: user.email,
                phone: user.phone
              }}
              onFinish={handleUpdateProfile}
            >
              <Form.Item
                label="用户名"
                name="username"
                rules={[
                  { required: true, message: '请输入用户名' },
                  { min: 2, message: '用户名至少2个字符' },
                  { max: 20, message: '用户名最多20个字符' }
                ]}
              >
                <Input prefix={<UserOutlined />} placeholder="请输入用户名" />
              </Form.Item>

              <Form.Item
                label="邮箱"
                name="email"
                rules={[
                  { required: true, message: '请输入邮箱' },
                  { type: 'email', message: '请输入有效的邮箱地址' }
                ]}
              >
                <Input prefix={<MailOutlined />} placeholder="请输入邮箱" />
              </Form.Item>

              <Form.Item
                label="手机号"
                name="phone"
              >
                <Input prefix={<PhoneOutlined />} placeholder="请输入手机号" />
              </Form.Item>

              <Form.Item>
                <Button type="primary" htmlType="submit" loading={loading} block>
                  保存
                </Button>
              </Form.Item>
            </Form>
          </Card>

          <Card title="修改密码" style={{ marginTop: '24px' }}>
            <Form layout="vertical">
              <Form.Item
                label="当前密码"
                name="currentPassword"
                rules={[{ required: true, message: '请输入当前密码' }]}
              >
                <Input.Password prefix={<LockOutlined />} placeholder="请输入当前密码" />
              </Form.Item>

              <Form.Item
                label="新密码"
                name="newPassword"
                rules={[
                  { required: true, message: '请输入新密码' },
                  { min: 6, message: '密码至少6个字符' }
                ]}
              >
                <Input.Password prefix={<LockOutlined />} placeholder="请输入新密码" />
              </Form.Item>

              <Form.Item
                label="确认新密码"
                name="confirmPassword"
                dependencies={['newPassword']}
                rules={[
                  { required: true, message: '请确认新密码' },
                  ({ getFieldValue }) => ({
                    validator(_, value) {
                      if (!value || getFieldValue('newPassword') === value) {
                        return Promise.resolve();
                      }
                      return Promise.reject(new Error('两次输入的密码不一致'));
                    }
                  })
                ]}
              >
                <Input.Password prefix={<LockOutlined />} placeholder="请确认新密码" />
              </Form.Item>

              <Form.Item>
                <Button type="primary" htmlType="submit" loading={loading} block>
                  修改密码
                </Button>
              </Form.Item>
            </Form>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default ProfilePage;