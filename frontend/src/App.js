import React from 'react';
import { Routes, Route, Navigate, useLocation } from 'react-router-dom';
import { ConfigProvider, Layout, Spin } from 'antd';
import { useSelector } from 'react-redux';
import MainLayout from './layouts/MainLayout';
import AuthLayout from './layouts/AuthLayout';

// 页面组件
import LoginPage from './pages/LoginPage';
import DashboardPage from './pages/DashboardPage';
import ProjectsPage from './pages/ProjectsPage';
import TaskDetailPage from './pages/TaskDetailPage';
import ProfilePage from './pages/ProfilePage';
import NotFoundPage from './pages/NotFoundPage';

// 受保护路由组件
const ProtectedRoute = ({ children }) => {
  const { isAuthenticated, loading } = useSelector((state) => state.auth);

  if (loading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <Spin size="large" />
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return children;
};

// 公共路由组件（未登录时重定向到主页）
const PublicRoute = ({ children }) => {
  const { isAuthenticated, loading } = useSelector((state) => state.auth);

  if (loading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <Spin size="large" />
      </div>
    );
  }

  if (isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  return children;
};

function App() {
  const location = useLocation();

  // 根据路由决定使用哪个布局
  const isAuthRoute = location.pathname === '/login';

  return (
    <ConfigProvider>
      {isAuthRoute ? (
        <AuthLayout>
          <Routes>
            <Route
              path="/login"
              element={
                <PublicRoute>
                    <LoginPage />
                  </PublicRoute>
                }
            />
            <Route path="*" element={<Navigate to="/login" replace />} />
          </Routes>
        </AuthLayout>
      ) : (
        <MainLayout>
          <Routes>
            <Route
              path="/"
              element={
                <ProtectedRoute>
                    <DashboardPage />
                  </ProtectedRoute>
                }
            />
            <Route
              path="/dashboard"
              element={
                <ProtectedRoute>
                    <DashboardPage />
                  </ProtectedRoute>
                }
            />
            <Route
              path="/projects"
              element={
                <ProtectedRoute>
                    <ProjectsPage />
                  </ProtectedRoute>
                }
            />
            <Route
              path="/tasks/:taskId"
              element={
                <ProtectedRoute>
                    <TaskDetailPage />
                  </ProtectedRoute>
                }
            />
            <Route
              path="/profile"
              element={
                <ProtectedRoute>
                    <ProfilePage />
                  </ProtectedRoute>
                }
            />
            <Route
              path="/login"
              element={<Navigate to="/" replace />}
            />
            <Route path="*" element={<NotFoundPage />} />
          </Routes>
        </MainLayout>
      )}
    </ConfigProvider>
  );
}

export default App;
