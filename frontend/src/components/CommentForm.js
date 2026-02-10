import React from 'react';
import { Form, Input, Button, Avatar, Space, message } from 'antd';
import { UserOutlined, SendOutlined } from '@ant-design/icons';
import { useSelector } from 'react-redux';
import { useDispatch } from 'react-redux';

const CommentForm = ({ taskId, onCommentAdded }) => {
  const [loading, setLoading] = React.useState(false);
  const [form] = Form.useForm();
  
  const dispatch = useDispatch();
  const { user } = useSelector((state) => state.auth);

  const handleSubmit = async (values) => {
    try {
      setLoading(true);
      
      const result = await dispatch({
        type: 'comments/createComment',
        payload: {
          taskId,
          content: values.content,
          userId: user.id
        }
      });

      if (result.success || !result.error) {
        message.success('评论发布成功');
        form.resetFields();
        onCommentAdded?.();
      }
    } catch (error) {
      message.error('评论发布失败');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ marginTop: '24px', padding: '24px', background: '#fafafa', borderRadius: '8px' }}>
      <Form
        form={form}
        layout="vertical"
        onFinish={handleSubmit}
      >
        <Form.Item
          name="content"
          label="添加评论"
          rules={[
            { required: true, message: '请输入评论内容' },
            { max: 1000, message: '评论内容最多1000个字符' }
          ]}
        >
          <Input.TextArea
            rows={4}
            placeholder="请输入评论内容...（支持 @提及用户）"
            showCount
            maxLength={1000}
          />
        </Form.Item>

        <Form.Item style={{ marginBottom: 0 }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <Space>
              <Avatar icon={<UserOutlined />} size="small" />
              <span style={{ color: '#666' }}>
                {user.username}
              </span>
            </Space>
            <Button
              type="primary"
              icon={<SendOutlined />}
              htmlType="submit"
              loading={loading}
            >
              发布评论
            </Button>
          </div>
        </Form.Item>
      </Form>
    </div>
  );
};

export default CommentForm;