import { createSlice } from '@reduxjs/toolkit';

// 初始状态
const initialState = {
  // 侧边栏
  sidebarCollapsed: false,
  sidebarWidth: 240,

  // 模态框
  modals: {
    createProject: false,
    createTask: false,
    editProject: false,
    editTask: false,
    deleteConfirm: false,
  },
  modalData: {},

  // 抽屉
  drawers: {
    taskDetail: false,
    commentList: false,
  },
  drawerData: {},

  // 通知/Toast
  notifications: [],

  // 加载状态
  globalLoading: false,

  // 主题
  theme: 'light',

  // 布局模式
  layoutMode: 'default', // default, compact, wide

  // 当前视图模式（用于任务看板）
  viewMode: 'board', // board, list, calendar

  // 拖拽相关
  drag: {
    isDragging: false,
    draggedItem: null,
    draggedItemType: null,
  },

  // 搜索
  searchVisible: false,
  searchQuery: '',

  // 当前选中的项目
  selectedProjectId: null,

  // 当前选中的任务
  selectedTaskId: null,

  // 过滤器展开状态
  filtersExpanded: false,

  // 响应式断点
  breakpoint: 'lg',
};

// Slice
const uiSlice = createSlice({
  name: 'ui',
  initialState,
  reducers: {
    // 侧边栏相关
    toggleSidebar: (state) => {
      state.sidebarCollapsed = !state.sidebarCollapsed;
    },
    setSidebarCollapsed: (state, action) => {
      state.sidebarCollapsed = action.payload;
    },
    setSidebarWidth: (state, action) => {
      state.sidebarWidth = action.payload;
    },

    // 模态框相关
    openModal: (state, action) => {
      const { modalName, data } = action.payload;
      state.modals[modalName] = true;
      if (data) {
        state.modalData[modalName] = data;
      }
    },
    closeModal: (state, action) => {
      const modalName = action.payload;
      state.modals[modalName] = false;
      state.modalData[modalName] = null;
    },
    closeAllModals: (state) => {
      Object.keys(state.modals).forEach(key => {
        state.modals[key] = false;
      });
      state.modalData = {};
    },

    // 抽屉相关
    openDrawer: (state, action) => {
      const { drawerName, data } = action.payload;
      state.drawers[drawerName] = true;
      if (data) {
        state.drawerData[drawerName] = data;
      }
    },
    closeDrawer: (state, action) => {
      const drawerName = action.payload;
      state.drawers[drawerName] = false;
      state.drawerData[drawerName] = null;
    },
    closeAllDrawers: (state) => {
      Object.keys(state.drawers).forEach(key => {
        state.drawers[key] = false;
      });
      state.drawerData = {};
    },

    // 通知相关
    addNotification: (state, action) => {
      const notification = {
        id: Date.now(),
        type: 'info', // success, error, warning, info
        duration: 4.5,
        ...action.payload,
      };
      state.notifications.push(notification);
    },
    removeNotification: (state, action) => {
      state.notifications = state.notifications.filter(
        n => n.id !== action.payload
      );
    },
    clearNotifications: (state) => {
      state.notifications = [];
    },

    // 加载状态
    setGlobalLoading: (state, action) => {
      state.globalLoading = action.payload;
    },

    // 主题
    setTheme: (state, action) => {
      state.theme = action.payload;
    },
    toggleTheme: (state) => {
      state.theme = state.theme === 'light' ? 'dark' : 'light';
    },

    // 布局模式
    setLayoutMode: (state, action) => {
      state.layoutMode = action.payload;
    },

    // 视图模式
    setViewMode: (state, action) => {
      state.viewMode = action.payload;
    },

    // 拖拽相关
    setDragging: (state, action) => {
      const { isDragging, item, itemType } = action.payload;
      state.drag.isDragging = isDragging;
      if (item !== undefined) {
        state.drag.draggedItem = item;
      }
      if (itemType !== undefined) {
        state.drag.draggedItemType = itemType;
      }
    },
    clearDrag: (state) => {
      state.drag = initialState.drag;
    },

    // 搜索
    setSearchVisible: (state, action) => {
      state.searchVisible = action.payload;
    },
    setSearchQuery: (state, action) => {
      state.searchQuery = action.payload;
    },
    toggleSearch: (state) => {
      state.searchVisible = !state.searchVisible;
      if (state.searchVisible) {
        state.searchQuery = '';
      }
    },

    // 选中的项目/任务
    setSelectedProjectId: (state, action) => {
      state.selectedProjectId = action.payload;
    },
    setSelectedTaskId: (state, action) => {
      state.selectedTaskId = action.payload;
    },

    // 过滤器
    setFiltersExpanded: (state, action) => {
      state.filtersExpanded = action.payload;
    },
    toggleFiltersExpanded: (state) => {
      state.filtersExpanded = !state.filtersExpanded;
    },

    // 响应式断点
    setBreakpoint: (state, action) => {
      state.breakpoint = action.payload;
      // 在小屏幕上自动收起侧边栏
      if (['xs', 'sm'].includes(action.payload)) {
        state.sidebarCollapsed = true;
      }
    },

    // 重置UI状态
    resetUI: () => initialState,
  },
});

// 导出 actions
export const {
  toggleSidebar,
  setSidebarCollapsed,
  setSidebarWidth,
  openModal,
  closeModal,
  closeAllModals,
  openDrawer,
  closeDrawer,
  closeAllDrawers,
  addNotification,
  removeNotification,
  clearNotifications,
  setGlobalLoading,
  setTheme,
  toggleTheme,
  setLayoutMode,
  setViewMode,
  setDragging,
  clearDrag,
  setSearchVisible,
  setSearchQuery,
  toggleSearch,
  setSelectedProjectId,
  setSelectedTaskId,
  setFiltersExpanded,
  toggleFiltersExpanded,
  setBreakpoint,
  resetUI,
} = uiSlice.actions;

// 导出 selectors
export const selectSidebarCollapsed = (state) => state.ui.sidebarCollapsed;
export const selectSidebarWidth = (state) => state.ui.sidebarWidth;
export const selectModal = (state, modalName) => state.ui.modals[modalName];
export const selectModalData = (state, modalName) => state.ui.modalData[modalName];
export const selectDrawer = (state, drawerName) => state.ui.drawers[drawerName];
export const selectDrawerData = (state, drawerName) => state.ui.drawerData[drawerName];
export const selectNotifications = (state) => state.ui.notifications;
export const selectGlobalLoading = (state) => state.ui.globalLoading;
export const selectTheme = (state) => state.ui.theme;
export const selectLayoutMode = (state) => state.ui.layoutMode;
export const selectViewMode = (state) => state.ui.viewMode;
export const selectDrag = (state) => state.ui.drag;
export const selectSearchVisible = (state) => state.ui.searchVisible;
export const selectSearchQuery = (state) => state.ui.searchQuery;
export const selectSelectedProjectId = (state) => state.ui.selectedProjectId;
export const selectSelectedTaskId = (state) => state.ui.selectedTaskId;
export const selectFiltersExpanded = (state) => state.ui.filtersExpanded;
export const selectBreakpoint = (state) => state.ui.breakpoint;

// 导出 reducer
export default uiSlice.reducer;
